using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace GlobalCMS
{
    public partial class SplashScreen : Form
    {
        int runTime = 0;
        bool runTimer = false;

        private static SplashScreen _frmObj;
        public static SplashScreen FrmObj
        {
            get { return _frmObj; }
            set { _frmObj = value; }
        }

        public SplashScreen()
        {
            InitializeComponent();
            // Check Internal Log Files for being too large
            string systemLog = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\system.log";
            if (File.Exists(systemLog))
            {
                FileInfo systemLogSize = new FileInfo(systemLog);
                if (systemLogSize.Length > 102400)
                {
                    try { File.Delete(systemLog); }
                    catch { }
                }
                try
                {
                    File.Delete(systemLog);
                }
                catch { }
                try
                {
                    File.Create(systemLog).Dispose();
                }
                catch { }
            }

            string signageLog = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\signageOutput.log";
            if (File.Exists(signageLog))
            {
                FileInfo signageLogSize = new FileInfo(signageLog);
                if (signageLogSize.Length > 102400)
                {
                    try { File.Delete(signageLog); }
                    catch { }
                }
                try
                {
                    File.Delete(signageLog);
                }
                catch { }
                try
                {
                    File.Create(signageLog).Dispose();
                }
                catch { }
            }
            // Continue to load the system
            bool isSkined = GCMSSystem.Skin.Check();
            if (!isSkined)
            {
                LoadingScreenIMG.Image = null;
                Height = 40;
                Width = 400;
            }
            else
            {
                runTimer = true;
            }
        }

        private void SplashScreen_Load(object sender, EventArgs e)
        {
            if (runTimer)
            {
                LoadingTimer.Enabled = true;
            }
            else
            {
                SkinTimer.Enabled = true;
            }
        }

        private void LoadingTimer_Tick(object sender, EventArgs e)
        {
            if (runTime == 0)
            {
                // Load up the RESOURCE for the given Splashscreen
                var whichSkin = GCMSSystem.Skin.GetSkinID();
                try { Themes.GenerateSplash(whichSkin); } catch { }
                LoadingScreenIMG.Show();
                Show();
            }

            runTime++;
            if (runTime == 1)
            {
                LoadingLabel.Text = "Loading Resources";
            }
            if (runTime == 2)
            {
                LoadingLabel.Text = "Dusting Off Spellbooks";
            }
            if (runTime == 3)
            {
                LoadingLabel.Text = "Don't Panic";
            }
            if (runTime == 4)
            {
                LoadingLabel.Text = "Generating Plans for Faster-Than-Light Travel";
            }
            if (runTime == 5)
            {
                LoadingTimer.Enabled = false;
                Hide();
                var mainForm = new MainForm();
                mainForm.Show();
                
            }
        }

        private void SkinTimer_Tick(object sender, EventArgs e)
        {
            runTime++;
            if (runTime == 2)
            {
                LoadingLabel.Text = "Getting Skin Information From Server";
            }
            if (runTime == 3)
            {
                LoadingLabel.Text = "Downloading Skin Data";
            }
            if (runTime == 4)
            {
                LoadingLabel.Text = "Setting Variables";
            }
            if (runTime == 5)
            {
                LoadingLabel.Text = "Restarting Application Please Wait.....";
                SkinTimer.Enabled = false;              // Stop the Skin Timer

                // Restart the Application and Load up the Skinner System
                runTime = 0;

                var whichSkin = GCMSSystem.Skin.GetSkinID();
                Themes.GenerateSplash(whichSkin);
                Height = 600;
                Width = 600;
                LoadingScreenIMG.Show();

                // Recenter the Form to Center of Screen
                CenterToScreen();
                LoadingLabel.Text = "Loading System";
                LoadingTimer.Enabled = true;                // Start the Loader again
            }
        }
    }
}
