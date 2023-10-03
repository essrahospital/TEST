using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Downloader;

namespace GlobalCMS
{
    public partial class DownloaderManager : Form
    {
        private static bool DownloaderDebug = false;
        private static DownloaderManager _frmObj;
        public static DownloaderManager FrmObj
        {
            get { return _frmObj; }
            set { _frmObj = value; }
        }

        public static string tmpFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "downloadtmp");
        public static string currentDownloadFile = "";
        public static string avgDownloadSpeed = "0bps";
        public static int currentDownload = 0;
        public static bool isFirstDownload = true;

        private static string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");      // Application Config
        private static IniFile MyIni = new IniFile(iniFile);
        public static long MaxDownload = 1024 * 1024 * Convert.ToInt32(MyIni.Read("MaxDownload", "Monitor"));

        public DownloadConfiguration downloadOpt = new DownloadConfiguration()
        {
            // file parts to download, default value is 1
            ChunkCount = 8,
            // How many Chunks to download at the same time, while queing the remaining chunks
            ParallelCount = 4,
            // download speed. Default value is 100 (100Mbit)
            MaximumBytesPerSecond = MaxDownload,
            // timeout (millisecond) per stream block reader, default values is 1000
            Timeout = 1000,
            // download parts of file as parallel or not. Default value is false
            ParallelDownload = true,
            // minimum size of chunking to download a file in multiple parts, default value is 512
            MinimumSizeOfChunking = 1024 * 3,
            // Before starting the download, reserve the storage space of the file as file size
            ReserveStorageSpaceBeforeStartingDownload = true,
        };

        public DownloaderManager()
        {
            InitializeComponent();
            FrmObj = this;
            this.Load += new System.EventHandler(this.LoadEvent);
            // Rectangle workingArea = Screen.GetWorkingArea(this);
            // this.Location = new Point(workingArea.Right - Size.Width, workingArea.Bottom - Size.Height);
        }

        private void LoadEvent(object sender, EventArgs e)
        {
            if (!DownloaderDebug)
            {
                Form form = (Form)sender;
                form.ShowInTaskbar = false;
                form.WindowState = FormWindowState.Minimized;
            }
        }

        async private void TestDownloadBTN_Click(object sender, EventArgs e)
        {
            string filePath = tmpFolder;
            string fileName = "testFile.pdf";
            string fileURL = "https://cliffordmapp.co.uk/testFile.pdf";
            await DownloadFile(downloadOpt, filePath, fileName, fileURL);
        }

        async public Task<string> DownloadFile(DownloadConfiguration downloadOpt, string FilePath, string FileName, string FileURL)
        {
            string path = @FilePath;
            string file = @FileName;
            string url = @FileURL;
            try
            {
                IDownload downloader = DownloadBuilder.New()
                    .WithUrl(url)
                    .WithDirectory(path)
                    .WithFileName(file)
                    .WithConfiguration(downloadOpt)
                    .Build();

                // Provide `FileName` and `TotalBytesToReceive` at the start of each downloads
                downloader.DownloadStarted += OnDownloadStarted;
                // Provide any information about download progress, like progress percentage of sum of chunks, total speed, average speed, total received bytes
                downloader.DownloadProgressChanged += OnDownloadProgressChanged;
                // Download completed event that can include occurred errors or cancelled or download completed successfully.
                downloader.DownloadFileCompleted += OnDownloadFileCompleted;
                var _cachedResult = await downloader.StartAsync();
            }
            catch { }
            return "Complete";
        }

        // Functions for Async Functions
        private void OnDownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            // FrmObj.DownloadView.Items[currentDownload].SubItems[3].Text = "0";
            CurrentDownloadID.Text = currentDownload.ToString();
            if (!DownloaderDebug)
            {
                this.ShowInTaskbar = false;
                this.WindowState = FormWindowState.Minimized;
            }
            GCMSSystem.FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - Download of " + currentDownloadFile + " Completed at an Average Speed of " + avgDownloadSpeed);
            currentDownloadFile = "";
            avgDownloadSpeed = "0bps";
        }
        private static void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (sender is DownloadService ds)
            {
                try
                {
                    UpdateDownloadInfo(e, ds.IsPaused, e.ActiveChunks);
                }
                catch { }
            }
        }
        private void OnDownloadStarted(object sender, DownloadStartedEventArgs e)
        {
            if (DownloaderDebug)
            {
                this.ShowInTaskbar = true;
                this.WindowState = FormWindowState.Normal;
            }
            var FileName = e.FileName;
            var actualFile = Path.GetFileName(FileName);
            currentDownloadFile = actualFile;
            var dateTime = DateTime.Now.ToString("dd MMM - HH:mm");

            DownloadView.Items.Add(
                new ListViewItem(new[] { dateTime, actualFile, "0", "0", "0%" })
            );
            if (!isFirstDownload) { currentDownload += 1; }
            if (isFirstDownload) { isFirstDownload = false; }
            GCMSSystem.FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - Download of " + actualFile + " Started");
        }
        private static void UpdateDownloadInfo(DownloadProgressChangedEventArgs e, bool isPaused, int ChunkCount)
        {
            double nonZeroSpeed = e.BytesPerSecondSpeed + 0.0001;
            int estimateTime = (int)((e.TotalBytesToReceive - e.ReceivedBytesSize) / nonZeroSpeed);
            bool isMinutes = estimateTime >= 60;
            string timeLeftUnit = "seconds";

            if (isMinutes)
            {
                timeLeftUnit = "minutes";
                estimateTime /= 60;
            }

            if (estimateTime < 0)
            {
                estimateTime = 0;
                timeLeftUnit = "unknown";
            }
            timeLeftUnit = timeLeftUnit.ToString();

            string percentComplete = Math.Round(e.ProgressPercentage, 2).ToString() + "%";
            string avgSpeed = CalcMemoryMensurableSpeed(e.AverageBytesPerSecondSpeed * 10);
            string speed = CalcMemoryMensurableSpeed(e.BytesPerSecondSpeed);
            string bytesReceived = CalcMemoryMensurableUnit(e.ReceivedBytesSize);
            string totalBytesToReceive = CalcMemoryMensurableUnit(e.TotalBytesToReceive);
            if (DownloaderDebug)
            {
                FrmObj.DownloadView.Items[currentDownload].SubItems[2].Text = bytesReceived + "/" + totalBytesToReceive;
                FrmObj.DownloadView.Items[currentDownload].SubItems[3].Text = avgSpeed;
                FrmObj.DownloadView.Items[currentDownload].SubItems[4].Text = percentComplete;
            }
            avgDownloadSpeed = avgSpeed;
        }

        // Measuring Functions
        private static string CalcMemoryMensurableUnit(double bytes)
        {
            double kb = bytes / 1024; // · 1024 Bytes = 1 Kilobyte 
            double mb = kb / 1024; // · 1024 Kilobytes = 1 Megabyte 
            double gb = mb / 1024; // · 1024 Megabytes = 1 Gigabyte 
            double tb = gb / 1024; // · 1024 Gigabytes = 1 Terabyte 

            string result =
                tb > 1 ? $"{tb:0.##}TB" :
                gb > 1 ? $"{gb:0.##}GB" :
                mb > 1 ? $"{mb:0.##}MB" :
                kb > 1 ? $"{kb:0.##}KB" :
                $"{bytes:0.##}B";

            result = result.Replace("/", ".");
            return result;
        }
        private static string CalcMemoryMensurableSpeed(double bytes)
        {
            double kb = bytes / 1024; // · 1024 Bytes = 1 Kilobyte 
            double mb = kb / 1024; // · 1024 Kilobytes = 1 Megabyte 
            double gb = mb / 1024; // · 1024 Megabytes = 1 Gigabyte 
            double tb = gb / 1024; // · 1024 Gigabytes = 1 Terabyte 

            string result =
                tb > 1 ? $"{tb:0.#}Tbps" :
                gb > 1 ? $"{gb:0.#}Gbps" :
                mb > 1 ? $"{mb:0.#}Mbps" :
                kb > 1 ? $"{kb:0.#}Kbps" :
                $"{bytes:0.##}Bbps";

            result = result.Replace("/", ".");
            return result;
        }

    }
}
