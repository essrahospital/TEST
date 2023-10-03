using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace GlobalCMS
{
    public partial class Decommission : Form
    {
        public Decommission()
        {
            InitializeComponent();

            var macAddy = GCMSSystem.GetMACAddress();
            macLabel.Text = macAddy;

            string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
            var MyIni = new IniFile(iniFile);
            var deviceName = MyIni.Read("deviceName", "Monitor");
            var deviceUUID = MyIni.Read("deviceUUID", "Monitor");

            if (!MainForm.isDebug)
            {
                // Make sure we shut down Chrome (1st) and NodeJS (2nd)
                foreach (var process in Process.GetProcessesByName("chrome"))
                {
                    try
                    {
                        process.StartInfo.Verb = "runas";
                        process.Kill();
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            foreach (var process in Process.GetProcessesByName("node32"))
            {
                try
                {
                    process.StartInfo.Verb = "runas";
                    process.Kill();
                }
                catch (Exception)
                {

                }
            }
            foreach (var process in Process.GetProcessesByName("node64"))
            {
                try
                {
                    process.StartInfo.Verb = "runas";
                    process.Kill();
                }
                catch (Exception)
                {

                }
            }


            using (var client = new WebClient())
            {
                // Create the $_POST Data for the HTTP Request
                var values = new NameValueCollection
                {
                    ["deviceName"] = deviceName,
                    ["deviceMAC"] = GCMSSystem.GetMACAddress(),
                    ["deviceUUID"] = deviceUUID
                };

                var responseString = "";
                try
                {
                    var response = client.UploadValues("https://api.globalcms.co.uk/v2/outboundDecom.php", values);
                    responseString = Encoding.Default.GetString(response);
                }
                catch
                {
                    responseString = "Error";
                }
            }

        }

        private void Decommission_Load(object sender, EventArgs e)
        {
            FrmObj = this;

            BringToFront();
            Focus();
            Activate();

            GCMSSystem.Chrome.Unload();
            GCMSSystem.Chrome.UpdatePref();
            SignageBrowser.FrmObj.Close();
        }

        static Decommission _frmObj;
        public static Decommission FrmObj
        {
            get { return _frmObj; }
            set { _frmObj = value; }
        }
    }
}
