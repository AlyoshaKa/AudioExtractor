This is a GUI for ffmpeg. It's value is to avoid needing to remember specific commands and flags for a cli.

It accepts an MKV as input, and then allows the user to select the audio streams to extract either in wav or flac formats.
The user can select either lossless or lossy decoding, and select to output either as multichannel file or seperate, single channel files.

The program can import an mkv with multiple audio streams, including dts:x or dolby atmos encoded streams and then extract whatever streams are desired.

It requires ffmpeg to be installed on the host machine, and for the environmental variables setting in windows to include the path to ffmpeg.exe.


Instructions to Add ffmpeg.exe to system PATH
1.	Hit Start, search for Environment Variables
2.	Click Edit the system environment variables
3.	Under System variables, find Path → Edit
4.	Add the folder path where ffmpeg.exe lives

