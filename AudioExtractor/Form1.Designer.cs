namespace AudioExtractor
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.ListBox lstAudioStreams;
        private System.Windows.Forms.Button btnExtract;
        private System.Windows.Forms.Label extractionStatusLabel;
        private System.Windows.Forms.ProgressBar extractionProgressBar;
        private System.Windows.Forms.Label remainingTimeLabel;
        private System.Windows.Forms.Label lblSelectFile;
        private System.Windows.Forms.GroupBox grpContainer;
        private System.Windows.Forms.RadioButton rbWav;
        private System.Windows.Forms.RadioButton rbFlac;
        private System.Windows.Forms.GroupBox grpChannelLayout;
        private System.Windows.Forms.RadioButton rbSingleFile;
        private System.Windows.Forms.RadioButton rbMonoFiles;
        private System.Windows.Forms.Label lblContainerFormat;
        private System.Windows.Forms.Label lblChannelLayout;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            txtFilePath = new TextBox();
            btnBrowse = new Button();
            lstAudioStreams = new ListBox();
            btnExtract = new Button();
            extractionStatusLabel = new Label();
            extractionProgressBar = new ProgressBar();
            remainingTimeLabel = new Label();
            lblSelectFile = new Label();
            grpContainer = new GroupBox();
            rbWav = new RadioButton();
            rbFlac = new RadioButton();
            grpChannelLayout = new GroupBox();
            rbSingleFile = new RadioButton();
            rbMonoFiles = new RadioButton();
            lblContainerFormat = new Label();
            lblChannelLayout = new Label();
            grpContainer.SuspendLayout();
            grpChannelLayout.SuspendLayout();
            SuspendLayout();
            // 
            // txtFilePath
            // 
            txtFilePath.Location = new Point(20, 40);
            txtFilePath.Name = "txtFilePath";
            txtFilePath.ReadOnly = true;
            txtFilePath.Size = new Size(400, 23);
            txtFilePath.TabIndex = 0;
            // 
            // btnBrowse
            // 
            btnBrowse.Location = new Point(430, 40);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(75, 23);
            btnBrowse.TabIndex = 1;
            btnBrowse.Text = "Browse";
            btnBrowse.Click += BtnBrowse_Click;
            // 
            // lstAudioStreams
            // 
            lstAudioStreams.Location = new Point(20, 69);
            lstAudioStreams.Name = "lstAudioStreams";
            lstAudioStreams.Size = new Size(485, 79);
            lstAudioStreams.TabIndex = 3;
            // 
            // btnExtract
            // 
            btnExtract.Location = new Point(20, 289);
            btnExtract.Name = "btnExtract";
            btnExtract.Size = new Size(485, 30);
            btnExtract.TabIndex = 6;
            btnExtract.Text = "Extract Audio";
            btnExtract.Click += BtnExtract_Click;
            // 
            // extractionStatusLabel
            // 
            extractionStatusLabel.Location = new Point(20, 355);
            extractionStatusLabel.Name = "extractionStatusLabel";
            extractionStatusLabel.Size = new Size(300, 23);
            extractionStatusLabel.TabIndex = 8;
            extractionStatusLabel.Text = "Status";
            // 
            // extractionProgressBar
            // 
            extractionProgressBar.Location = new Point(20, 325);
            extractionProgressBar.Name = "extractionProgressBar";
            extractionProgressBar.Size = new Size(485, 23);
            extractionProgressBar.TabIndex = 7;
            // 
            // remainingTimeLabel
            // 
            remainingTimeLabel.Location = new Point(330, 355);
            remainingTimeLabel.Name = "remainingTimeLabel";
            remainingTimeLabel.Size = new Size(175, 23);
            remainingTimeLabel.TabIndex = 9;
            remainingTimeLabel.Text = "Remaining Time";
            // 
            // lblSelectFile
            // 
            lblSelectFile.Location = new Point(20, 20);
            lblSelectFile.Name = "lblSelectFile";
            lblSelectFile.Size = new Size(200, 23);
            lblSelectFile.TabIndex = 2;
            lblSelectFile.Text = "Select MKV Input File";
            // 
            // grpContainer
            // 
            grpContainer.Controls.Add(rbWav);
            grpContainer.Controls.Add(rbFlac);
            grpContainer.Location = new Point(20, 186);
            grpContainer.Name = "grpContainer";
            grpContainer.Padding = new Padding(0, 0, 10, 10);
            grpContainer.Size = new Size(235, 97);
            grpContainer.TabIndex = 4;
            grpContainer.TabStop = false;
            // 
            // rbWav
            // 
            rbWav.Checked = true;
            rbWav.Location = new Point(10, 25);
            rbWav.Name = "rbWav";
            rbWav.Size = new Size(100, 24);
            rbWav.TabIndex = 0;
            rbWav.TabStop = true;
            rbWav.Text = "WAV";
            // 
            // rbFlac
            // 
            rbFlac.Location = new Point(10, 55);
            rbFlac.Name = "rbFlac";
            rbFlac.Size = new Size(100, 24);
            rbFlac.TabIndex = 1;
            rbFlac.Text = "FLAC";
            // 
            // grpChannelLayout
            // 
            grpChannelLayout.Controls.Add(rbSingleFile);
            grpChannelLayout.Controls.Add(rbMonoFiles);
            grpChannelLayout.Location = new Point(270, 186);
            grpChannelLayout.Name = "grpChannelLayout";
            grpChannelLayout.Padding = new Padding(0, 0, 10, 10);
            grpChannelLayout.Size = new Size(235, 97);
            grpChannelLayout.TabIndex = 5;
            grpChannelLayout.TabStop = false;
            // 
            // rbSingleFile
            // 
            rbSingleFile.AutoSize = true;
            rbSingleFile.Checked = true;
            rbSingleFile.Location = new Point(10, 25);
            rbSingleFile.Name = "rbSingleFile";
            rbSingleFile.Size = new Size(154, 19);
            rbSingleFile.TabIndex = 0;
            rbSingleFile.TabStop = true;
            rbSingleFile.Text = "Multichannel Spatial File";
            // 
            // rbMonoFiles
            // 
            rbMonoFiles.AutoSize = true;
            rbMonoFiles.Location = new Point(10, 55);
            rbMonoFiles.Name = "rbMonoFiles";
            rbMonoFiles.Size = new Size(131, 19);
            rbMonoFiles.TabIndex = 1;
            rbMonoFiles.Text = "Separate Mono Files";
            // 
            // lblContainerFormat
            // 
            lblContainerFormat.Location = new Point(20, 173);
            lblContainerFormat.Name = "lblContainerFormat";
            lblContainerFormat.Size = new Size(235, 20);
            lblContainerFormat.TabIndex = 4;
            lblContainerFormat.Text = "Audio Container Format";
            lblContainerFormat.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblChannelLayout
            // 
            lblChannelLayout.Location = new Point(267, 173);
            lblChannelLayout.Name = "lblChannelLayout";
            lblChannelLayout.Size = new Size(225, 20);
            lblChannelLayout.TabIndex = 5;
            lblChannelLayout.Text = "Channel Layout";
            lblChannelLayout.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            ClientSize = new Size(560, 387);
            Controls.Add(txtFilePath);
            Controls.Add(btnBrowse);
            Controls.Add(lblChannelLayout);
            Controls.Add(lblContainerFormat);
            Controls.Add(lblSelectFile);
            Controls.Add(lstAudioStreams);
            Controls.Add(grpContainer);
            Controls.Add(grpChannelLayout);
            Controls.Add(btnExtract);
            Controls.Add(extractionProgressBar);
            Controls.Add(extractionStatusLabel);
            Controls.Add(remainingTimeLabel);
            Name = "Form1";
            Text = "MKV Audio Extractor";
            grpContainer.ResumeLayout(false);
            grpChannelLayout.ResumeLayout(false);
            grpChannelLayout.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
