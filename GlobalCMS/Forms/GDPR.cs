using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GlobalCMS
{
    public partial class GDPR : Form
    {
        public GDPR()
        {
            InitializeComponent();

            // Bring to Front and Focus
            BringToFront();
            Focus();
            Activate();
        }

        private void GDPR_Load(object sender, EventArgs e)
        {
            frmObj = this;
            if (GCMSSystem.Chrome.whichVer == 1)
            {
                // Create the Dynamic Timer
                Timer tmr;
                tmr = new Timer();
                tmr.Tick += delegate
                {
                    tmr.Stop();
                    if (GCMSSystem.Chrome.whichVer == 2)
                    {
                        bool isSignageEnabled = GCMSSystem.Chrome.IsSignageEnabled();
                        if (isSignageEnabled)
                        {
                            GCMSSystem.Chrome.Load();
                        }
                    }
                    Hide();
                };

                // How Long do we want to run the Timer for
                tmr.Interval = (int)TimeSpan.FromSeconds(3).TotalMilliseconds;

                // Start the Timer
                tmr.Start();
            }
            if (GCMSSystem.Chrome.whichVer == 2)
            {
                // Create the Dynamic Timer
                Timer tmr;
                tmr = new Timer();
                tmr.Tick += delegate
                {
                    tmr.Stop();
                    bool isSignageEnabled = GCMSSystem.Chrome.IsSignageEnabled();
                    if (isSignageEnabled)
                    {
                        GCMSSystem.Chrome.Load();
                    }
                    Hide();
                };

                // How Long do we want to run the Timer for
                tmr.Interval = (int)TimeSpan.FromSeconds(3).TotalMilliseconds;

                // Start the Timer
                tmr.Start();
            }
        }

        static GDPR _frmObj;
        public static GDPR frmObj
        {
            get { return _frmObj; }
            set { _frmObj = value; }
        }
    }
}
