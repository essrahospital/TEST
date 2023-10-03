using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace GlobalCMS
{
    public partial class AirServerConfig : Form
    {
        string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");      // Application Config

        public AirServerConfig()
        {
            InitializeComponent();
            var MyIni = new IniFile(iniFile);
            DeviceAS.Text = GCMSSystem.AirServer.DeviceName();
            PasscodeAS.Text = GCMSSystem.AirServer.Passcode();
        }

        private void UpdateBTN_Click(object sender, EventArgs e)
        {
            var MyIni = new IniFile(iniFile);
            string PasscodeTxt = PasscodeAS.Text;
            string DeviceNameTxt = DeviceAS.Text;
            var PasscodeInt = Convert.ToInt32(PasscodeTxt);

            GCMSSystem.AirServer.Stop();            // AirServer Must Be Stopped Before Running Configs
            GCMSSystem.AirServer.Kill();
            GCMSSystem.AirServer.SetPassword(PasscodeInt);
            GCMSSystem.AirServer.SetName(DeviceNameTxt);
            GCMSSystem.AirServer.Start();
            MessageBox.Show("AirServer Config Updated", "AirServer Config", MessageBoxButtons.OK);
        }
    }
}
