using System;
using System.Windows.Forms;

namespace GlobalCMS
{
    public partial class DeviceLocked : Form
    {
        public static bool isLocked = true;
        public DeviceLocked()
        {
            InitializeComponent();
        }

        static DeviceLocked _frmObj;
        public static DeviceLocked FrmObj
        {
            get { return _frmObj; }
            set { _frmObj = value; }
        }

        private void CheckTimer_Tick(object sender, EventArgs e)
        {
            var isLocked2 = MainForm.FrmObj.powerModeLabel.Text;
            if (isLocked2 == "Normal" || isLocked2 == "Normal / Online" || isLocked2 == "Normal / Offline") { isLocked = false; }

            if (!isLocked)
            {
                isLocked = false;
                Close();
            }
        }
    }
}
