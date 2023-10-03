using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace GlobalCMS
{
    public partial class About : Form
    {
        public static bool isBETA = true;
        public static string GetVersion(string whichString)
        {
            string version;
            if (whichString == "Main")
            {
                version = "2.0.2.9";
            }
            else
            {
                version = "0.2.1.9";
                if (isBETA) { version += "-BETA"; }
            }
            return version;
        }

        public About()
        {
            InitializeComponent();

            string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
            var MyIni = new IniFile(iniFile);
            var MySkin = MyIni.Read("SkinID", "Skin");
            try { Themes.Generate(MySkin); } catch { }
        }

        private void About_Load(object sender, EventArgs e)
        {
            FrmObj = this;

            string appGuid = "{8F6F0AC4-B9A1-45fd-A8CF-72F04E6BDE8F}";        // App GUI to check for it running twice
            string version = GetVersion("Main");                              // Version
            string subversion = GetVersion("Subversion");                     // Sub Version

            AppGUID.Text = "GUID: " +appGuid.ToString();
            AppVersion.Text = version.ToString();
            AppSubVersion.Text = subversion.ToString();
            BrowserVersion.Text = MainForm.eoVersion.ToString();
            AirServerVersion.Text = GCMSSystem.AirServer.Version().ToString();
            NexmosphereVer.Text = Nexmosphere.NexmosphereVersion.ToString();
            MainForm.FrmObj.TaskbarContextMenu.Items[0].Enabled = false;

            BringToFront();
            Focus();

            string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
            if (File.Exists(iniFile))
            {
                var MyIni = new IniFile(iniFile);
                var myLic = MyIni.Read("licType", "Licence");                   // Get Licence Type from the config.ini file
                if (myLic == "SEC")
                {
                    AppLicence.Text = "PRO";
                }
                else
                {
                    AppLicence.Text = "STD";
                }
            }
            else
            {
                AppLicence.Text = "";
            }

            string signageVersion;
            string signageSubVersion;
            using (StreamReader streamReader = new StreamReader(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "version.txt"), Encoding.UTF8))
            {
                signageVersion = streamReader.ReadToEnd();
            }
            using (StreamReader streamReader = new StreamReader(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "subversion.txt"), Encoding.UTF8))
            {
                signageSubVersion = streamReader.ReadToEnd();
            }

            SignVersion.Text = signageVersion;
            SignSubVersion.Text = signageSubVersion;


        }

        // Events
        private void CloseBTN_Click(object sender, EventArgs e)
        {
            Close();
            MainForm.FrmObj.TaskbarContextMenu.Items[0].Enabled = true;
        }

        static About _frmObj;
        public static About FrmObj
        {
            get { return _frmObj; }
            set { _frmObj = value; }
        }
    }
}
