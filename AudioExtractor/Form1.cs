using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AudioExtractorApp
{
    public partial class Form1 : Form
    {
        private string selectedFilePath;
        private string selectedAudioStreamIndex;
        private double fileDuration;

        static readonly string[] PcmCodecs = {
            "pcm_s16le", "pcm_s24le", "pcm_s32le", "pcm_u8", "pcm_f32le"
        };

        public Form1()
        {
            InitializeComponent();

            radioMultichannelFile.Checked = true;
            radioWAV.Checked = true;

            txtStreamDetails.ReadOnly = true;
            txtStreamDetails.Multiline = true;
            txtStreamDetails.ScrollBars = ScrollBars.Vertical;
            txtStreamDetails.Font = new System.Drawing.Font("Consolas", 9F);
        }

        private void btnSelectMKV_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;

            selectedFilePath = openFileDialog.FileName;
            txtMKVFilePath.Text = selectedFilePath;
            LoadAudioStreamsFromFile();
        }

        private void LoadAudioStreamsFromFile()
        {
            lstAudioStreams.Items.Clear();
            if (string.IsNullOrEmpty(selectedFilePath)) return;

            string json = RunProcOut("ffprobe",
                $"-v quiet -print_format json -show_streams -show_entries stream=index,codec_name,codec_long_name,codec_type,channels,channel_layout,bits_per_raw_sample,sample_rate,bit_rate,duration,tags,disposition -select_streams a \"{selectedFilePath}\"");

            var probe = JsonSerializer.Deserialize<FFProbeResult>(json);
            if (probe?.Streams == null) return;

            fileDuration = GetDuration(selectedFilePath);

            foreach (var s in probe.Streams)
            {
                string desc = $"Stream {s.Index}: {s.CodecName}, {s.Channels}ch";
                lstAudioStreams.Items.Add(new AudioStreamItem
                {
                    Display = desc,
                    StreamIndex = s.Index.ToString(CultureInfo.InvariantCulture),
                    CodecName = s.CodecName,
                    CodecLongName = s.CodecLongName,
                    Channels = s.Channels,
                    ChannelLayout = s.ChannelLayout,
                    BitsPerRawSample = s.BitsPerRawSample,
                    SampleRate = ParseInt(s.SampleRate),
                    BitRate = s.BitRate,
                    Duration = s.Duration,
                    Tags = s.Tags,
                    Disposition = s.Disposition
                });
            }

            if (lstAudioStreams.Items.Count > 0)
                lstAudioStreams.SelectedIndex = 0;
        }

        private void lstAudioStreams_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstAudioStreams.SelectedItem is not AudioStreamItem item) return;

            selectedAudioStreamIndex = item.StreamIndex;
            bool isPcm = IsPcm(item.CodecName);
            lblStreamType.Text = isPcm ? "Stream type: PCM" : "Stream type: Non-PCM";
            groupBox3.Enabled = !isPcm;

            var sb = new StringBuilder();
            sb.AppendLine($"Stream Index    : {item.StreamIndex}");
            sb.AppendLine($"Codec           : {item.CodecName}");
            if (!string.IsNullOrWhiteSpace(item.CodecLongName))
                sb.AppendLine($"Codec Name      : {item.CodecLongName}");
            sb.AppendLine($"Channels        : {item.Channels}");
            sb.AppendLine($"Channel Layout  : {item.ChannelLayout ?? "(unknown)"}");
            sb.AppendLine($"Sample Rate     : {item.SampleRate} Hz");
            string bitDepth;
                string codec = item.CodecName?.Trim().ToLowerInvariant() ?? "";

                if (!string.IsNullOrWhiteSpace(item.BitsPerRawSample))
                {
                    bitDepth = $"{item.BitsPerRawSample}-bit";
                }
                else
                {
                    bitDepth = codec switch
                    {
                        "dts" or "dts_hd_ma" or "dts_hd_hra" or "dts_es" => "8-bit",
                        "ac3" or "eac3"                                 => "16-bit",
                        "truehd" or "mlp"                               => "24-bit",  // MLP is base for TrueHD
                        "flac"                                          => "16–24-bit",
                        "pcm_s16le"                                     => "16-bit",
                        "pcm_s24le"                                     => "24-bit",
                        "pcm_s32le"                                     => "32-bit",
                        "pcm_u8"                                        => "8-bit",
                        "pcm_f32le"                                     => "32-bit (float)",
                        _                                               => "(unknown)"
                    };
                }

            sb.AppendLine($"Bit Depth       : {bitDepth}");

            sb.AppendLine($"Bit Rate        : {(string.IsNullOrWhiteSpace(item.BitRate) ? "(unknown)" : item.BitRate)}");
            string duration = !string.IsNullOrWhiteSpace(item.Duration)
                ? item.Duration
                : TimeSpan.FromSeconds(fileDuration).ToString(@"hh\:mm\:ss");

            sb.AppendLine($"Duration        : {duration}");


            if (item.Tags != null)
            {
                foreach (var kv in item.Tags)
                    sb.AppendLine($"Tag: {kv.Key,-12}: {kv.Value}");
            }

            if (item.Disposition != null)
            {
                foreach (var kv in item.Disposition.Where(kv => kv.Value == 1))
                    sb.AppendLine($"Disposition     : {kv.Key}");
            }

            txtStreamDetails.Text = sb.ToString();
        }

        private async void btnExtractAudio_Click(object sender, EventArgs e)
{
    if (string.IsNullOrEmpty(selectedFilePath) || string.IsNullOrEmpty(selectedAudioStreamIndex))
    {
        MessageBox.Show("Select a file and an audio stream first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
    }

    using var folderDlg = new FolderBrowserDialog();
    if (folderDlg.ShowDialog() != DialogResult.OK) return;
    string outDir = folderDlg.SelectedPath;

    var item = (AudioStreamItem)lstAudioStreams.SelectedItem;
    int global = int.Parse(item.StreamIndex, CultureInfo.InvariantCulture);
    bool isPcm = IsPcm(item.CodecName);
    bool separate = radioSeparateFiles.Checked;
    int chCount = item.Channels;

    progressBar.Value = 0;
    lblTimeEstimate.Text = "Starting…";

    int sr = item.SampleRate > 0 ? item.SampleRate : 48000;
    string pcmCodec = item.BitsPerRawSample switch
    {
        "32" => "pcm_s32le",
        "24" => "pcm_s24le",
        "8"  => "pcm_u8",
        _    => "pcm_s16le"
    };

    string[] names = GetChannelNamesForCodec(item.CodecName, item.ChannelLayout, chCount);

    if (separate)
    {
        for (int i = 0; i < chCount; i++)
        {
            string chName = names[i];
            string outPath, args;

            if (isPcm || radioWAV.Checked)
            {
                outPath = Path.Combine(outDir, $"stream_{global}_{chName}.wav");
                args = $"-y -i \"{selectedFilePath}\" -filter_complex \"[0:a:{global}]pan=mono|c0=c{i}[out]\" -map \"[out]\" -c:a {pcmCodec} -ac 1 -ar {sr} -f wav -rf64 auto \"{outPath}\"";
            }
            else
            {
                outPath = Path.Combine(outDir, $"stream_{global}_{chName}.flac");
                args = $"-y -i \"{selectedFilePath}\" -filter_complex \"[0:a:{global}]pan=mono|c0=c{i}[out]\" -map \"[out]\" -c:a flac \"{outPath}\"";
            }

            await RunWithProgress(args);
        }
    }
    else
    {
        string ext = (isPcm || radioWAV.Checked) ? "wav" : "flac";
        string baseName = Path.GetFileNameWithoutExtension(selectedFilePath);
        string outPath = Path.Combine(outDir, $"{baseName}.{ext}");

        string args;
        if (ext == "wav")
        {
            args = $"-y -i \"{selectedFilePath}\" -map 0:{global} -c:a {pcmCodec} -ac {chCount} -ar {sr} -f wav -rf64 auto \"{outPath}\"";
        }
        else
        {
            args = $"-y -i \"{selectedFilePath}\" -map 0:{global} -c:a flac \"{outPath}\"";
        }

        await RunWithProgress(args);
    }

    lblTimeEstimate.Text = "Done.";
}


        private async Task RunWithProgress(string arguments)
{
    var psi = new ProcessStartInfo
    {
        FileName = "ffmpeg",
        Arguments = arguments,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    using var proc = new Process { StartInfo = psi, EnableRaisingEvents = true };

    proc.ErrorDataReceived += (s, e) =>
    {
        if (string.IsNullOrEmpty(e.Data)) return;

        // Match time=00:01:23.456 or time=00:01:23,456
        var match = Regex.Match(e.Data, @"time=(\d{2}):(\d{2}):(\d{2})[.,](\d{1,3})");
        if (!match.Success) return;

        if (int.TryParse(match.Groups[1].Value, out int hours) &&
            int.TryParse(match.Groups[2].Value, out int minutes) &&
            int.TryParse(match.Groups[3].Value, out int seconds) &&
            int.TryParse(match.Groups[4].Value.PadRight(3, '0'), out int millis))
        {
            var current = new TimeSpan(0, hours, minutes, seconds, millis);
            double processed = current.TotalSeconds;
            int pct = fileDuration > 0 ? (int)(processed / fileDuration * 100) : 0;
            double remaining = Math.Max(fileDuration - processed, 0);
            var eta = TimeSpan.FromSeconds(remaining);

            progressBar.Invoke((MethodInvoker)(() =>
                progressBar.Value = Math.Min(pct, 100)));

            lblProgressPercent.Invoke((MethodInvoker)(() =>
                lblProgressPercent.Text = $"{pct}%"));

            lblTimeEstimate.Invoke((MethodInvoker)(() =>
                lblTimeEstimate.Text = $"Processed {current:hh\\:mm\\:ss} / {TimeSpan.FromSeconds(fileDuration):hh\\:mm\\:ss}"));

            lblTimeRemaining.Invoke((MethodInvoker)(() =>
                lblTimeRemaining.Text = $"Estimated time left: {eta:hh\\:mm\\:ss}"));
        }
    };

    proc.Start();
    proc.BeginErrorReadLine();
    await Task.Run(() => proc.WaitForExit());

    progressBar.Invoke((MethodInvoker)(() => progressBar.Value = 100));
}



        private string[] GetChannelNamesForCodec(string codec, string layout, int chCount)
        {
            var defaultNames = Enumerable.Range(0, chCount).Select(i => $"ch{i}").ToArray();

            if (!string.IsNullOrWhiteSpace(layout))
            {
                var parsed = layout
                    .Split(new[] { '+', ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim().ToUpperInvariant())
                    .ToArray();

                if (parsed.Length == chCount)
                    return parsed.Select(MapToMediaInfoName).ToArray();
            }

            var standardMap = new Dictionary<int, string[]>
            {
                {1, new[] { "C" }},
                {2, new[] { "L", "R" }},
                {3, new[] { "L", "R", "C" }},
                {4, new[] { "L", "R", "Ls", "Rs" }},
                {5, new[] { "L", "R", "C", "Ls", "Rs" }},
                {6, new[] { "L", "R", "C", "LFE", "Ls", "Rs" }},
                {7, new[] { "L", "R", "C", "LFE", "Ls", "Rs", "Cs" }},
                {8, new[] { "L", "R", "C", "LFE", "Lb", "Rb", "Ls", "Rs" }}
            };

            if (standardMap.TryGetValue(chCount, out var known))
                return known;

            return defaultNames;
        }

        private string MapToMediaInfoName(string ffmpegName) => ffmpegName.ToUpperInvariant() switch
        {
            "FL" => "L", "FR" => "R", "FC" => "C", "LFE" => "LFE",
            "BL" => "Lb", "BR" => "Rb", "BC" => "Cs", "SL" => "Ls", "SR" => "Rs",
            "FLC" => "Lc", "FRC" => "Rc", "TFL" => "Tfl", "TFC" => "Tfc", "TFR" => "Tfr",
            "TC" => "Tc", "TRL" => "Trl", "TRC" => "Trc", "TRR" => "Trr", "LFE2" => "LFE2",
            "CS" => "Cs", _ => ffmpegName
        };

        private int ParseInt(string s) => int.TryParse(s, out var val) ? val : 0;

        private double GetDuration(string path)
        {
            string json = RunProcOut("ffprobe", $"-v quiet -print_format json -show_format \"{path}\"");
            var result = JsonSerializer.Deserialize<FFProbeFormatResult>(json);
            if (result?.Format?.Duration != null &&
                double.TryParse(result.Format.Duration, NumberStyles.Float, CultureInfo.InvariantCulture, out var dur))
                return dur;
            return 0;
        }

        private bool IsPcm(string codec) =>
            Array.Exists(PcmCodecs, c => c.Equals(codec, StringComparison.OrdinalIgnoreCase));

        private string RunProcOut(string file, string args)
        {
            var psi = new ProcessStartInfo
            {
                FileName = file,
                Arguments = args,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var proc = Process.Start(psi);
            string outp = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            return outp;
        }

        private class FFProbeResult
        {
            [JsonPropertyName("streams")]
            public List<AudioStreamInfo> Streams { get; set; }
        }

        private class FFProbeFormatResult
        {
            [JsonPropertyName("format")]
            public FormatSection Format { get; set; }
        }

        private class FormatSection
        {
            [JsonPropertyName("duration")]
            public string Duration { get; set; }
        }

        private class AudioStreamInfo
        {
            [JsonPropertyName("index")] public int Index { get; set; }
            [JsonPropertyName("codec_name")] public string CodecName { get; set; }
            [JsonPropertyName("codec_long_name")] public string CodecLongName { get; set; }
            [JsonPropertyName("codec_type")] public string CodecType { get; set; }
            [JsonPropertyName("channels")] public int Channels { get; set; }
            [JsonPropertyName("channel_layout")] public string ChannelLayout { get; set; }
            [JsonPropertyName("bits_per_raw_sample")] public string BitsPerRawSample { get; set; }
            [JsonPropertyName("sample_rate")] public string SampleRate { get; set; }
            [JsonPropertyName("bit_rate")] public string BitRate { get; set; }
            [JsonPropertyName("duration")] public string Duration { get; set; }
            [JsonPropertyName("tags")] public Dictionary<string, string> Tags { get; set; }
            [JsonPropertyName("disposition")] public Dictionary<string, int> Disposition { get; set; }
        }

        private class AudioStreamItem
        {
            public string Display { get; set; }
            public string StreamIndex { get; set; }
            public string CodecName { get; set; }
            public string CodecLongName { get; set; }
            public int Channels { get; set; }
            public string ChannelLayout { get; set; }
            public string BitsPerRawSample { get; set; }
            public int SampleRate { get; set; }
            public string BitRate { get; set; }
            public string Duration { get; set; }
            public Dictionary<string, string> Tags { get; set; }
            public Dictionary<string, int> Disposition { get; set; }
            public override string ToString() => Display;
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0)
                {
                    selectedFilePath = files[0];
                    txtMKVFilePath.Text = selectedFilePath;
                    LoadAudioStreamsFromFile();
                }
            }
        }

        private void txtMKVFilePath_DragEnter(object sender, DragEventArgs e)
            => Form1_DragEnter(sender, e);

        private void txtMKVFilePath_DragDrop(object sender, DragEventArgs e)
            => Form1_DragDrop(sender, e);
    }
}
