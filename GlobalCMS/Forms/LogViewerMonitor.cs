using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace GlobalCMS
{
    public partial class LogViewerMonitor : Form
    {
        public static string LogFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "logs", "system.log");

        public LogViewerMonitor()
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
            watch.Filter = "system.log";
            watch.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite;
            watch.Changed += new FileSystemEventHandler(FileChanged);
            watch.EnableRaisingEvents = true;

            listView1.Enabled = true;
            LoadLogBTN.Text = "Load Log File";
        }
    }

    public static class Ext
    {
        private const long OneKb = 1024;
        private const long OneMb = OneKb * 1024;
        private const long OneGb = OneMb * 1024;
        private const long OneTb = OneGb * 1024;

        public static string ToPrettySize(this int value, int decimalPlaces = 0)
        {
            return ((long)value).ToPrettySize(decimalPlaces);
        }

        public static string ToPrettySize(this long value, int decimalPlaces = 0)
        {
            var asTb = Math.Round((double)value / OneTb, decimalPlaces);
            var asGb = Math.Round((double)value / OneGb, decimalPlaces);
            var asMb = Math.Round((double)value / OneMb, decimalPlaces);
            var asKb = Math.Round((double)value / OneKb, decimalPlaces);
            string chosenValue = asTb > 1 ? string.Format("{0} TB", asTb)
                : asGb > 1 ? string.Format("{0} GB", asGb)
                : asMb > 1 ? string.Format("{0} MB", asMb)
                : asKb > 1 ? string.Format("{0} KB", asKb)
                : string.Format("{0} B", Math.Round((double)value, decimalPlaces));
            return chosenValue;
        }
    }
}
