
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
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
        private double fileDuration; // seconds

        static readonly string[] PcmCodecs = {
            "pcm_s16le", "pcm_s24le", "pcm_s32le", "pcm_u8", "pcm_f32le"
        };

        public Form1()
        {
            InitializeComponent();
            radioMultichannelFile.Checked = true;
            radioWAV.Checked = true;
        }

        private void btnSelectMKV_Click(object, EventArgs e)
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

            string streamJson = RunProcOut("ffprobe",
                $"-v quiet -print_format json -show_streams -select_streams a "{selectedFilePath}"");
            var probe = JsonSerializer.Deserialize<FFProbeResult>(streamJson);
            if (probe?.Streams == null) return;

            fileDuration = GetDuration(selectedFilePath);

            foreach (var s in probe.Streams)
            {
                string layout = string.IsNullOrEmpty(s.ChannelLayout) ? "unknown" : s.ChannelLayout;
                int sr = 0;
                if (!string.IsNullOrWhiteSpace(s.SampleRate) &&
                    int.TryParse(s.SampleRate, NumberStyles.Integer, CultureInfo.InvariantCulture, out var tmp))
                    sr = tmp;

                string desc = $"Stream {s.Index}: {s.CodecName}, {s.Channels}ch, layout={layout}, {sr}Hz, {s.BitsPerRawSample}-bit";

                lstAudioStreams.Items.Add(new AudioStreamItem {
                    Display = desc,
                    StreamIndex = s.Index.ToString(CultureInfo.InvariantCulture),
                    CodecName = s.CodecName,
                    Channels = s.Channels,
                    ChannelLayout = s.ChannelLayout,
                    BitsPerRawSample = s.BitsPerRawSample,
                    SampleRate = sr
                });
            }

            if (lstAudioStreams.Items.Count > 0)
                lstAudioStreams.SelectedIndex = 0;
        }

        private void lstAudioStreams_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(lstAudioStreams.SelectedItem is AudioStreamItem item)) return;
            selectedAudioStreamIndex = item.StreamIndex;

            bool isPcm = IsPcm(item.CodecName);
            lblStreamType.Text = isPcm ? "Stream type: PCM" : "Stream type: Non-PCM";
            groupBox3.Enabled = !isPcm;
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
            string pcmCodec = item.BitsPerRawSample switch {
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
                        args = $"-y -i "{selectedFilePath}" " +
                               $"-filter_complex "[0:a:{global}]pan=mono|c0=c{i}[out]" " +
                               $"-map "[out]" -c:a {pcmCodec} -ac 1 -ar {sr} -f wav -rf64 auto "{outPath}"";
                    }
                    else
                    {
                        outPath = Path.Combine(outDir, $"stream_{global}_{chName}.flac");
                        args = $"-y -i "{selectedFilePath}" " +
                               $"-filter_complex "[0:a:{global}]pan=mono|c0=c{i}[out]" " +
                               $"-map "[out]" -c:a flac "{outPath}"";
                    }

                    await RunWithProgress(args);
                }
            }
            else
            {
                string outPath, args;

                if (isPcm || radioWAV.Checked)
                {
                    outPath = Path.Combine(outDir, $"stream_{global}.wav");
                    args = $"-y -i "{selectedFilePath}" -map 0:{global} " +
                           $"-c:a {pcmCodec} -ac {chCount} -ar {sr} -f wav -rf64 auto "{outPath}"";
                }
                else
                {
                    outPath = Path.Combine(outDir, $"stream_{global}.flac");
                    args = $"-y -i "{selectedFilePath}" -map 0:{global} -c:a flac "{outPath}"";
                }

                await RunWithProgress(args);
            }

            lblTimeEstimate.Text = "Done.";
        }

        private string[] GetChannelNamesForCodec(string codec, string layout, int chCount)
        {
            var standardMap = new Dictionary<int, string[]>
            {
                {1, new[] { "C" }},
                {2, new[] { "FL", "FR" }},
                {3, new[] { "FL", "FR", "C" }},
                {4, new[] { "FL", "FR", "BL", "BR" }},
                {5, new[] { "FL", "FR", "C", "BL", "BR" }},
                {6, new[] { "FL", "FR", "C", "LFE", "SL", "SR" }},
                {7, new[] { "FL", "FR", "C", "LFE", "BC", "SL", "SR" }},
                {8, new[] { "FL", "FR", "C", "LFE", "BL", "BR", "SL", "SR" }}
            };

            codec = codec?.ToLowerInvariant() ?? "";

            if (!string.IsNullOrWhiteSpace(layout))
            {
                var parsed = layout.Split(new[] { '+', ',' }, StringSplitOptions.RemoveEmptyEntries)
                                   .Select(x => x.Trim().ToUpperInvariant())
                                   .ToArray();
                if (parsed.Length == chCount)
                    return parsed;
            }

            if (standardMap.ContainsKey(chCount))
                return standardMap[chCount];

            return Enumerable.Range(0, chCount).Select(i => $"ch{i}").ToArray();
        }

        private double GetDuration(string path)
        {
            string fmtJson = RunProcOut("ffprobe", $"-v quiet -print_format json -show_format "{path}"");
            var fmtInfo = JsonSerializer.Deserialize<FFProbeFormatResult>(fmtJson);
            if (fmtInfo?.Format?.Duration != null &&
                double.TryParse(fmtInfo.Format.Duration, NumberStyles.Float, CultureInfo.InvariantCulture, out var d))
                return d;
            return 0;
        }

        private bool IsPcm(string codec)
            => Array.Exists(PcmCodecs, c => c.Equals(codec, StringComparison.OrdinalIgnoreCase));

        private async Task RunWithProgress(string arguments)
        {
            var psi = new ProcessStartInfo {
                FileName = "ffmpeg",
                Arguments = arguments,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var p = new Process { StartInfo = psi, EnableRaisingEvents = true };
            p.ErrorDataReceived += (s, e) => {
                if (string.IsNullOrEmpty(e.Data)) return;
                var m = Regex.Match(e.Data, @"time=(\d+:\d+:\d+\.?\d*)");
                if (!m.Success) return;
                if (!TimeSpan.TryParse(m.Groups[1].Value, out var ts)) return;

                int pct = fileDuration > 0
                    ? (int)(ts.TotalSeconds / fileDuration * 100)
                    : 0;

                progressBar.Invoke((MethodInvoker)(() =>
                    progressBar.Value = Math.Min(pct, 100)));
                lblTimeEstimate.Invoke((MethodInvoker)(() =>
                    lblTimeEstimate.Text = $"Processed {ts:hh\:mm\:ss} / {TimeSpan.FromSeconds(fileDuration):hh\:mm\:ss}"));
            };

            p.Start();
            p.BeginErrorReadLine();
            await Task.Run(() => p.WaitForExit());
        }

        private string RunProcOut(string file, string args)
        {
            var psi = new ProcessStartInfo {
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

        private class AudioStreamInfo
        {
            [JsonPropertyName("index")]
            public int Index { get; set; }
            [JsonPropertyName("codec_name")]
            public string CodecName { get; set; }
            [JsonPropertyName("channels")]
            public int Channels { get; set; }
            [JsonPropertyName("channel_layout")]
            public string ChannelLayout { get; set; }
            [JsonPropertyName("bits_per_raw_sample")]
            public string BitsPerRawSample { get; set; }
            [JsonPropertyName("sample_rate")]
            public string SampleRate { get; set; }
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

        private class AudioStreamItem
        {
            public string Display { get; set; }
            public string StreamIndex { get; set; }
            public string CodecName { get; set; }
            public int Channels { get; set; }
            public string ChannelLayout { get; set; }
            public string BitsPerRawSample { get; set; }
            public int SampleRate { get; set; }
            public override string ToString() => Display;
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
            => e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop)
                         ? DragDropEffects.Copy
                         : DragDropEffects.None;

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (files.Length == 0) return;
            selectedFilePath = files[0];
            txtMKVFilePath.Text = selectedFilePath;
            LoadAudioStreamsFromFile();
        }

        private void txtMKVFilePath_DragEnter(object sender, DragEventArgs e)
            => Form1_DragEnter(sender, e);

        private void txtMKVFilePath_DragDrop(object sender, DragEventArgs e)
            => Form1_DragDrop(sender, e);
    }
}
