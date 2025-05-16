namespace AudioExtractorApp
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblProgressPercent;
        private System.Windows.Forms.Label lblTimeRemaining;

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSelectMKV;
        private System.Windows.Forms.TextBox txtMKVFilePath;
        private System.Windows.Forms.Label lblAudioStreams;
        private System.Windows.Forms.ListBox lstAudioStreams;
        private System.Windows.Forms.TextBox txtStreamDetails;
        private System.Windows.Forms.Label lblStreamType;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioSeparateFiles;
        private System.Windows.Forms.RadioButton radioMultichannelFile;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton radioWAV;
        private System.Windows.Forms.RadioButton radioFLAC;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblTimeEstimate;
        private System.Windows.Forms.Button btnExtractAudio;
        private System.Windows.Forms.OpenFileDialog openFileDialog;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSelectMKV = new System.Windows.Forms.Button();
            this.txtMKVFilePath = new System.Windows.Forms.TextBox();
            this.lblAudioStreams = new System.Windows.Forms.Label();
            this.lstAudioStreams = new System.Windows.Forms.ListBox();
            this.txtStreamDetails = new System.Windows.Forms.TextBox();
            this.lblStreamType = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioSeparateFiles = new System.Windows.Forms.RadioButton();
            this.radioMultichannelFile = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.radioWAV = new System.Windows.Forms.RadioButton();
            this.radioFLAC = new System.Windows.Forms.RadioButton();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblProgressPercent = new System.Windows.Forms.Label();
            this.lblTimeEstimate = new System.Windows.Forms.Label();
            this.lblTimeRemaining = new System.Windows.Forms.Label();
            this.btnExtractAudio = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();

            // groupBox1
            this.groupBox1.Controls.Add(this.btnSelectMKV);
            this.groupBox1.Controls.Add(this.txtMKVFilePath);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(400, 70);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.Text = "Select MKV/MP4 File";

            // btnSelectMKV
            this.btnSelectMKV.Location = new System.Drawing.Point(320, 28);
            this.btnSelectMKV.Name = "btnSelectMKV";
            this.btnSelectMKV.Size = new System.Drawing.Size(75, 23);
            this.btnSelectMKV.TabIndex = 1;
            this.btnSelectMKV.Text = "Browse";
            this.btnSelectMKV.UseVisualStyleBackColor = true;
            this.btnSelectMKV.Click += new System.EventHandler(this.btnSelectMKV_Click);

            // txtMKVFilePath
            this.txtMKVFilePath.AllowDrop = true;
            this.txtMKVFilePath.Location = new System.Drawing.Point(6, 28);
            this.txtMKVFilePath.Name = "txtMKVFilePath";
            this.txtMKVFilePath.Size = new System.Drawing.Size(300, 20);
            this.txtMKVFilePath.TabIndex = 0;
            this.txtMKVFilePath.DragEnter += new System.Windows.Forms.DragEventHandler(this.txtMKVFilePath_DragEnter);
            this.txtMKVFilePath.DragDrop += new System.Windows.Forms.DragEventHandler(this.txtMKVFilePath_DragDrop);

            // lblAudioStreams
            this.lblAudioStreams.AutoSize = true;
            this.lblAudioStreams.Location = new System.Drawing.Point(12, 95);
            this.lblAudioStreams.Name = "lblAudioStreams";
            this.lblAudioStreams.Size = new System.Drawing.Size(85, 13);
            this.lblAudioStreams.TabIndex = 7;
            this.lblAudioStreams.Text = "Audio Streams:";

            // lstAudioStreams
            this.lstAudioStreams.FormattingEnabled = true;
            this.lstAudioStreams.Location = new System.Drawing.Point(12, 115);
            this.lstAudioStreams.Name = "lstAudioStreams";
            this.lstAudioStreams.Size = new System.Drawing.Size(400, 95);
            this.lstAudioStreams.TabIndex = 6;
            this.lstAudioStreams.SelectedIndexChanged += new System.EventHandler(this.lstAudioStreams_SelectedIndexChanged);

            // txtStreamDetails
            this.txtStreamDetails.Location = new System.Drawing.Point(12, 215);
            this.txtStreamDetails.Multiline = true;
            this.txtStreamDetails.Name = "txtStreamDetails";
            this.txtStreamDetails.ReadOnly = true;
            this.txtStreamDetails.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStreamDetails.Size = new System.Drawing.Size(400, 120);
            this.txtStreamDetails.TabIndex = 8;
            this.txtStreamDetails.Font = new System.Drawing.Font("Consolas", 9F);

            // lblStreamType
            this.lblStreamType.AutoSize = true;
            this.lblStreamType.Location = new System.Drawing.Point(12, 345);
            this.lblStreamType.Name = "lblStreamType";
            this.lblStreamType.Size = new System.Drawing.Size(84, 13);
            this.lblStreamType.TabIndex = 9;
            this.lblStreamType.Text = "Stream type: N/A";

            // groupBox2
            this.groupBox2.Controls.Add(this.radioSeparateFiles);
            this.groupBox2.Controls.Add(this.radioMultichannelFile);
            this.groupBox2.Location = new System.Drawing.Point(12, 365);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(400, 60);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.Text = "Channel Layout";

            // radioSeparateFiles
            this.radioSeparateFiles.AutoSize = true;
            this.radioSeparateFiles.Location = new System.Drawing.Point(6, 38);
            this.radioSeparateFiles.Name = "radioSeparateFiles";
            this.radioSeparateFiles.Size = new System.Drawing.Size(138, 17);
            this.radioSeparateFiles.TabIndex = 1;
            this.radioSeparateFiles.TabStop = true;
            this.radioSeparateFiles.Text = "Separate Files (Mono)";
            this.radioSeparateFiles.UseVisualStyleBackColor = true;

            // radioMultichannelFile
            this.radioMultichannelFile.AutoSize = true;
            this.radioMultichannelFile.Location = new System.Drawing.Point(6, 19);
            this.radioMultichannelFile.Name = "radioMultichannelFile";
            this.radioMultichannelFile.Size = new System.Drawing.Size(135, 17);
            this.radioMultichannelFile.TabIndex = 0;
            this.radioMultichannelFile.TabStop = true;
            this.radioMultichannelFile.Text = "Single Multichannel File";
            this.radioMultichannelFile.UseVisualStyleBackColor = true;

            // groupBox3
            this.groupBox3.Controls.Add(this.radioWAV);
            this.groupBox3.Controls.Add(this.radioFLAC);
            this.groupBox3.Location = new System.Drawing.Point(12, 435);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(400, 60);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.Text = "Output Format (Non‑PCM only)";

            // radioWAV
            this.radioWAV.AutoSize = true;
            this.radioWAV.Location = new System.Drawing.Point(6, 19);
            this.radioWAV.Name = "radioWAV";
            this.radioWAV.Size = new System.Drawing.Size(112, 17);
            this.radioWAV.TabIndex = 0;
            this.radioWAV.TabStop = true;
            this.radioWAV.Text = "WAV (decode to PCM)";
            this.radioWAV.UseVisualStyleBackColor = true;

            // radioFLAC
            this.radioFLAC.AutoSize = true;
            this.radioFLAC.Location = new System.Drawing.Point(6, 38);
            this.radioFLAC.Name = "radioFLAC";
            this.radioFLAC.Size = new System.Drawing.Size(111, 17);
            this.radioFLAC.TabIndex = 1;
            this.radioFLAC.TabStop = true;
            this.radioFLAC.Text = "FLAC (lossless)";
            this.radioFLAC.UseVisualStyleBackColor = true;

            // lblProgressPercent
            this.lblProgressPercent.AutoSize = true;
            this.lblProgressPercent.Location = new System.Drawing.Point(12, 500);
            this.lblProgressPercent.Name = "lblProgressPercent";
            this.lblProgressPercent.Size = new System.Drawing.Size(27, 13);
            this.lblProgressPercent.TabIndex = 15;
            this.lblProgressPercent.Text = "0%";

            // progressBar
            this.progressBar.Location = new System.Drawing.Point(12, 515);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(400, 20);
            this.progressBar.TabIndex = 12;

            // lblTimeEstimate
            this.lblTimeEstimate.AutoSize = true;
            this.lblTimeEstimate.Location = new System.Drawing.Point(12, 540);
            this.lblTimeEstimate.Name = "lblTimeEstimate";
            this.lblTimeEstimate.Size = new System.Drawing.Size(94, 13);
            this.lblTimeEstimate.TabIndex = 13;
            this.lblTimeEstimate.Text = "Processed: —";

            // lblTimeRemaining
            this.lblTimeRemaining.AutoSize = true;
            this.lblTimeRemaining.Location = new System.Drawing.Point(12, 560);
            this.lblTimeRemaining.Name = "lblTimeRemaining";
            this.lblTimeRemaining.Size = new System.Drawing.Size(108, 13);
            this.lblTimeRemaining.TabIndex = 16;
            this.lblTimeRemaining.Text = "Time remaining: —";

            // btnExtractAudio
            this.btnExtractAudio.Location = new System.Drawing.Point(337, 585);
            this.btnExtractAudio.Name = "btnExtractAudio";
            this.btnExtractAudio.Size = new System.Drawing.Size(75, 23);
            this.btnExtractAudio.TabIndex = 14;
            this.btnExtractAudio.Text = "Extract";
            this.btnExtractAudio.UseVisualStyleBackColor = true;
            this.btnExtractAudio.Click += new System.EventHandler(this.btnExtractAudio_Click);

            // openFileDialog
            this.openFileDialog.Filter = "MKV/MP4 files (*.mkv;*.mp4)|*.mkv;*.mp4";
            this.openFileDialog.Title = "Select Media File";

            // Form1
            this.AllowDrop = true;
            this.ClientSize = new System.Drawing.Size(424, 620);
            this.Controls.Add(this.btnExtractAudio);
            this.Controls.Add(this.lblTimeRemaining);
            this.Controls.Add(this.lblTimeEstimate);
            this.Controls.Add(this.lblProgressPercent);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.lblStreamType);
            this.Controls.Add(this.txtStreamDetails);
            this.Controls.Add(this.lstAudioStreams);
            this.Controls.Add(this.lblAudioStreams);
            this.Controls.Add(this.groupBox1);
            this.Name = "Form1";
            this.Text = "Audio Extractor";
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);

            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
