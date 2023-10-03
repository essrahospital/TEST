using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace GlobalCMS
{
    public partial class LogViewerBrowser : Form
    {
        public static string LogFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "logs");

        public LogViewerBrowser()
        {
            InitializeComponent();
        }

        private void LoadLogBTN_Click(object sender, System.EventArgs e)
        {
            var whichLog = "eoloadfail.log";

            var logDropDwn = WhichLogDropDwn.Text;
            if (logDropDwn != "")
            {
                if (logDropDwn == "LoadFailed Log - When Browser Fails To Load URL") { whichLog = "eoloadfail.log"; }
                if (logDropDwn == "Javascript Stack - When Browser Crashes Due To Rendering Problems") { whichLog = "eostack.log"; }
                if (logDropDwn == "Exception Log - When the Browser Crashes Due To Browser Code Error") { whichLog = "eoexception.log"; }

                var LogFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "logs", whichLog);
                if (File.Exists(LogFile))
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

                    listView1.Enabled = true;
                    LoadLogBTN.Text = "Load Log File";
                }
            }

        }
    }
}
