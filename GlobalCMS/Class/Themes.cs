namespace GlobalCMS
{
    class Themes
    {
        /// <summary>
        /// Default - Default Skin
        /// 12345   - CMWebHosting
        /// 34754   - CruciallyDigital
        /// 55763   - CX Auto
        /// 76541   - Gforces [Netdirector]
        /// </summary>
        public static void Generate(string SkinID)
        {
            var version = About.GetVersion("Main");
            if (SkinID != "Default")
            {
                if (SkinID == "12345")
                {
                    MainForm.LogoBottomCorner.Image = Properties.Resources.SKIN_12345_LOGO;                 // Set Bottom Right Logo
                    MainForm.TaskbarIcon.Icon = Properties.Resources.SKIN_12345_ICO;                        // Set ICO for the System Tray Icon
                    MainForm.TaskbarIcon.Text = "CMWebHosting Monitoring Solution - v" + version;           // Set System Tray Hover Over
                    MainForm.FrmObj.Text = "CMWebHosting Monitoring Solution - v" + version; ;              // Set overall Title
                    MainForm.FrmObj.Icon = Properties.Resources.SKIN_12345_ICO;                             // Set overall ICON for Application
                    if (GCMSSystem.CheckOpened("About"))
                    {
                        About.logoBottomCorner.Image = Properties.Resources.SKIN_12345_LOGO;                    // Set Logo on About Form
                    }
                }
                if (SkinID == "34754")
                {
                    MainForm.LogoBottomCorner.Image = Properties.Resources.SKIN_34754_LOGO;                 // Set Bottom Right Logo
                    MainForm.TaskbarIcon.Icon = Properties.Resources.SKIN_34754_ICO;                        // Set ICO for the System Tray Icon
                    MainForm.TaskbarIcon.Text = "CruciallyDigital Monitoring Solution - v" + version;       // Set System Tray Hover Over
                    MainForm.FrmObj.Text = "CruciallyDigital Monitoring Solution - v" + version; ;          // Set overall Title
                    MainForm.FrmObj.Icon = Properties.Resources.SKIN_34754_ICO;                             // Set overall ICON for Application
                    if (GCMSSystem.CheckOpened("About"))
                    {
                        About.logoBottomCorner.Image = Properties.Resources.SKIN_34754_LOGO;                    // Set Logo on About Form
                    }
                }
                if (SkinID == "55763")
                {
                    MainForm.LogoBottomCorner.Image = Properties.Resources.SKIN_55763_LOGO;                 // Set Bottom Right Logo
                    MainForm.TaskbarIcon.Icon = Properties.Resources.SKIN_55763_ICO;                        // Set ICO for the System Tray Icon
                    MainForm.TaskbarIcon.Text = "CX Auto Monitoring Solution - v" + version;                // Set System Tray Hover Over
                    MainForm.FrmObj.Text = "CX Auto Monitoring Solution - v" + version; ;                   // Set overall Title
                    MainForm.FrmObj.Icon = Properties.Resources.SKIN_55763_ICO;                             // Set overall ICON for Application
                    if (GCMSSystem.CheckOpened("About"))
                    {
                        About.logoBottomCorner.Image = Properties.Resources.SKIN_55763_LOGO;                    // Set Logo on About Form
                    }
                }
                if (SkinID == "76541")
                {
                    MainForm.LogoBottomCorner.Image = Properties.Resources.SKIN_76541_LOGO;                 // Set Bottom Right Logo
                    MainForm.TaskbarIcon.Icon = Properties.Resources.SKIN_76541_ICO;                        // Set ICO for the System Tray Icon
                    MainForm.TaskbarIcon.Text = "NetDirector Showroom Solutions  - v" + version;            // Set System Tray Hover Over
                    MainForm.FrmObj.Text = "NetDirector Showroom Solutions - v" + version; ;                // Set overall Title
                    MainForm.FrmObj.Icon = Properties.Resources.SKIN_76541_ICO;                             // Set overall ICON for Application
                    if (GCMSSystem.CheckOpened("About"))
                    {
                        About.logoBottomCorner.Image = Properties.Resources.SKIN_76541_LOGO;                    // Set Logo on About Form
                    }
                }
                if (SkinID != "12345" && SkinID != "34754" && SkinID != "55763" && SkinID != "76541")
                {
                    // If the SkinID doesnt exist then load the defaults
                    MainForm.LogoBottomCorner.Image = Properties.Resources.SKIN_DEFAULT_LOGO;               // Set Bottom Right Logo
                    MainForm.TaskbarIcon.Icon = Properties.Resources.SKIN_DEFAULT_ICO;                      // Set ICO for the System Tray Icon
                    MainForm.TaskbarIcon.Text = "GlobalCMS Monitoring Solution - v" + version;              // Set System Tray Hover Over
                    MainForm.FrmObj.Text = "GlobalCMS Monitoring Solution - v" + version; ;                 // Set overall Title
                    MainForm.FrmObj.Icon = Properties.Resources.SKIN_DEFAULT_ICO;                           // Set overall ICON for Application
                    if (GCMSSystem.CheckOpened("About"))
                    {
                        About.logoBottomCorner.Image = Properties.Resources.SKIN_DEFAULT_LOGO;                  // Set Logo on About Form
                    }
                }
            }
            else
            {
                // Template for Skin - Default
                MainForm.LogoBottomCorner.Image = Properties.Resources.SKIN_DEFAULT_LOGO;               // Set Bottom Right Logo
                MainForm.TaskbarIcon.Icon = Properties.Resources.SKIN_DEFAULT_ICO;                      // Set ICO for the System Tray Icon
                MainForm.TaskbarIcon.Text = "GlobalCMS Monitoring Solution - v" + version;              // Set System Tray Hover Over
                MainForm.FrmObj.Text = "GlobalCMS Monitoring Solution - v" + version; ;                 // Set overall Title
                MainForm.FrmObj.Icon = Properties.Resources.SKIN_DEFAULT_ICO;                           // Set overall ICON for Application
                if (GCMSSystem.CheckOpened("About"))
                {
                    About.logoBottomCorner.Image = Properties.Resources.SKIN_DEFAULT_LOGO;                  // Set Logo on About Form
                }
            }
        }
        public static void GenerateSplash(string SkinID)
        {
            if (SkinID == "Default")
            {
                SplashScreen.LoadingScreenIMG.Image = Properties.Resources.SKIN_DEFAULT_SPLASH;
            }
            if (SkinID != "Default")
            {
                if (SkinID == "12345") { SplashScreen.LoadingScreenIMG.Image = Properties.Resources.SKIN_12345_SPLASH; }
                else if (SkinID == "34754") { SplashScreen.LoadingScreenIMG.Image = Properties.Resources.SKIN_34754_SPLASH;  }
                else if (SkinID == "55763") { SplashScreen.LoadingScreenIMG.Image = Properties.Resources.SKIN_55763_SPLASH; }
                else if (SkinID == "76541") { SplashScreen.LoadingScreenIMG.Image = Properties.Resources.SKIN_76541_SPLASH; }
                else
                {
                    // If ID in config.ini is wrong then go to Default instead
                    SplashScreen.LoadingScreenIMG.Image = Properties.Resources.SKIN_DEFAULT_SPLASH;
                }
            }
        }
    }
}
