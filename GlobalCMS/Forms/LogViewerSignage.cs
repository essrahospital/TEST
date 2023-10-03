using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace GlobalCMS
{
    public partial class LogViewerSignage : Form
    {
        public static string LogFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "logs", "signageOutput.log");

        public LogViewerSignage()
        {
            InitializeComponent();

            FileInfo info = new FileInfo(LogFile);
            LastModValue.Text = info.LastWriteTimeUtc.ToString();
            LogSizeValue.Text = Ext.ToPrettySize(info.Length, 2);
            string timeNow = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        }

        private static void FileChanged(object source, FileSystemEventArgs e)
        {
            if (e.FullPath == LogFile)
            {
                Debug.WriteLine("File has Changed");
            }
        }

        private void LoadLogBTN_Click(object sender, System.EventArgs e)
        {
            LoadLogBTN.Text = "Loading ....";
            listView1.Enabled = false;
            listView1.Items.Clear();
            ColumnHeader header = new ColumnHeader
            {
                Text = "",
                Name = "col1"
            };
            listView1.Columns.Add(header);

            foreach (var line in File.ReadLines(LogFile))
            {
                listView1.Items.Add(line);
            }

            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            listView1.Items[listView1.Items.Count - 1].EnsureVisible();

            var watch = new FileSystemWatcher();
            watch.Path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "logs");
            watch.Filter = "signageOutput.log";
            watch.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite;
            watch.Changed += new FileSystemEventHandler(FileChanged);
            watch.EnableRaisingEvents = true;

            listView1.Enabled = true;
            LoadLogBTN.Text = "Load Log File";
        }
    }
}
