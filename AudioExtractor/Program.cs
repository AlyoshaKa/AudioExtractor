using System;
using System.Windows.Forms;
using AudioExtractorApp;

namespace AudioExtractor
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}