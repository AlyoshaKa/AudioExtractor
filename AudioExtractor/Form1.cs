using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AudioExtractor
{
    public partial class Form1 : Form
    {
        private string selectedFilePath = string.Empty;

        public Form1()
        {
            InitializeComponent();
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "MKV Files (*.mkv)|*.mkv|All Files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                selectedFilePath = ofd.FileName;
                txtFilePath.Text = selectedFilePath;
                LoadAudioStreams();
            }
        }

        private void LoadAudioStreams()
        {
            lstAudioStreams.Items.Clear();
            var startInfo = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-i \"{selectedFilePath}\"",
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            using var reader = process.StandardError;
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Contains("Audio:"))
                {
                    lstAudioStreams.Items.Add(line.Trim());
                }
            }
        }

        private TimeSpan GetVideoDuration(string filePath)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "ffprobe",
                Arguments = $"-v error -show_entries format=duration -of default=noprint_wrappers=1:nokey=1 \"{filePath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(startInfo);
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return double.TryParse(result, out double seconds) ? TimeSpan.FromSeconds(seconds) : TimeSpan.Zero;
        }

        private async void BtnExtract_Click(object sender, EventArgs e)
        {
            if (lstAudioStreams.SelectedIndex == -1)
            {
                MessageBox.Show("Please select an audio stream.");
                return;
            }

            string selectedStream = lstAudioStreams.SelectedItem.ToString();
            var match = Regex.Match(selectedStream, @"#0:(\d+)");
            if (!match.Success || !int.TryParse(match.Groups[1].Value, out int streamIndex))
            {
                MessageBox.Show("Could not parse audio stream index.");
                return;
            }

            string ext = rbFlac.Checked ? "flac" : "wav";
            string codec = rbFlac.Checked ? "flac" : "pcm_s24le";
            string outputPath = Path.ChangeExtension(selectedFilePath, ext);

            extractionProgressBar.Value = 0;
            extractionStatusLabel.Text = "Starting...";
            remainingTimeLabel.Text = "N/A";

            await Task.Run(() =>
            {
                try
                {
                    var startInfo = new ProcessStartInfo
                    {
                        FileName = "ffmpeg",
                        Arguments = $"-y -i \"{selectedFilePath}\" -map 0:{streamIndex} -c:a {codec} \"{outputPath}\"",
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using var process = Process.Start(startInfo);
                    using var reader = process.StandardError;
                    string line;
                    var duration = GetVideoDuration(selectedFilePath);
                    var regex = new Regex(@"time=(\d+):(\d+):(\d+).(\d+)");

                    while ((line = reader.ReadLine()) != null)
                    {
                        var timeMatch = regex.Match(line);
                        if (timeMatch.Success)
                        {
                            try
                            {
                                var current = new TimeSpan(
                                    int.Parse(timeMatch.Groups[1].Value),
                                    int.Parse(timeMatch.Groups[2].Value),
                                    int.Parse(timeMatch.Groups[3].Value))
                                    .Add(TimeSpan.FromMilliseconds(int.Parse(timeMatch.Groups[4].Value) * 10));

                                int progress = (int)(current.TotalSeconds / duration.TotalSeconds * 100);
                                progress = Math.Clamp(progress, 0, 100);

                                Invoke(() =>
                                {
                                    extractionProgressBar.Value = progress;
                                    UpdateRemainingTime((int)(duration.TotalSeconds - current.TotalSeconds));
                                });
                            }
                            catch { }
                        }
                    }

                    process.WaitForExit();

                    Invoke(() =>
                    {
                        extractionProgressBar.Value = 100;
                        extractionStatusLabel.Text = $"Done: {Path.GetFileName(outputPath)}";
                        remainingTimeLabel.Text = "Done";
                    });
                }
                catch (Exception ex)
                {
                    Invoke(() =>
                    {
                        extractionStatusLabel.Text = $"Error: {ex.Message}";
                        remainingTimeLabel.Text = "Error";
                    });
                }
            });
        }

        private void UpdateRemainingTime(int secondsRemaining)
        {
            // Calculate minutes and seconds from total seconds
            TimeSpan timeRemaining = TimeSpan.FromSeconds(secondsRemaining);

            // Format remaining time as minutes:seconds
            string formattedTime = string.Format("{0:D2}:{1:D2}", timeRemaining.Minutes, timeRemaining.Seconds);

            // Update the label with formatted time
            remainingTimeLabel.Text = $"Remaining Time: {formattedTime}";
        }

        private void grpContainer_Enter(object sender, EventArgs e)
        {

        }

        private void lblContainerFormat_Click(object sender, EventArgs e)
        {

        }

        private void lblChannelLayout_Click(object sender, EventArgs e)
        {

        }

        private void grpChannelLayout_Enter(object sender, EventArgs e)
        {

        }
    }
}
