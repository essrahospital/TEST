using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace GlobalCMS
{
    public partial class ScreenLock : Form
    {
        public static bool isLocked = true;
        string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");

        public ScreenLock()
        {
            InitializeComponent();
        }

        static ScreenLock _frmObj;
        public static ScreenLock frmObj
        {
            get { return _frmObj; }
            set { _frmObj = value; }
        }

        private void CheckTimer_Tick(object sender, EventArgs e)
        {
            var MyIni = new IniFile(iniFile);

            var MaintMode = MyIni.Read("maintMode", "Network");                       // Maintenance Mode
            if (MaintMode == "TRUE") { Close(); return; }
        }
    }
}
