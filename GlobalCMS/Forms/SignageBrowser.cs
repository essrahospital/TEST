using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using EO.WebBrowser;
using EO.WebEngine;
using Shell32;

namespace GlobalCMS
{
    public partial class SignageBrowser : Form
    {
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public static bool airServerMirroring = MainForm.airServerMirroring;
        public static bool airServerConnected = false;
        public static bool airServerConnectedTopOnce = true;
        public static int airServerConnectedCounter = 0;
        public static bool isXFrame = false;
        public static bool isSignageLoaded = false;

        string browserDebugMode = "Off";
        string MySystemKeyboard = "Default";
        int nExitCommand = CommandIds.RegisterUserCommand("Exit");
        int nMaintCommand = CommandIds.RegisterUserCommand("Maint");

        string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
        string signParamsFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "signageParams.ini");

        public static float curMasterVol = 0;

        public SignageBrowser()
        {
            try
            {
                curMasterVol = AudioManager.GetMasterVolume();
            } catch { }

            System.Windows.Forms.Cursor.Hide();
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()));
            FrmObj = this;
            WindowState = FormWindowState.Maximized;

            var MyIni = new IniFile(iniFile);
            var BrowserDebugMode = MyIni.Read("Debug", "Browser");
            MySystemKeyboard = MyIni.Read("Keyboard", "Browser");
            var exRAM = MyIni.Read("ExRAM", "Browser");

            browserDebugMode = BrowserDebugMode;

            // Add the Licence for EO Webbrowser
            Runtime.AddLicense(
                "Au30EO2s3MKetZ9Zl6TNF+ic3PIEEMidtbjB5LZup7jH4bNss7P9FOKe5ff2" +
                "9ON3hI6xy59Zs/D6DuSn6un26bto4+30EO2s3OnPuIlZl6Sx5+Cl4/MI6YxD" +
                "l6Sxy59Zl6TNDOOdl/gKG+R2mcng2cKh6fP+EKFZ7ekDHuio5cGz3a9np6ax" +
                "2r1GgaSxy591puX9F+6wtZGby59Zl8AAHeOe6c3/Ee5Z2+UFELxbqLTA3K5r" +
                "p7XJzZ+s7ObWI++i6ekE7PN2mbXB2rBoqbTD26FZ7ekDHuio5cGz3Ldbl7PP" +
                "uIlZl6Sx5/Ki3vLyH/Sr3MLAE8uT2NgFBM+Lptv59dedqO3Z+s16tMHN2vKi" +
                "3vLyH/Sr3MKetbto4+30EO2s3MKetXXj");

            Engine.Default.Options.AllowProprietaryMediaFormats();
            Engine.Default.AllowRestart = true;

            Runtime.DefaultEngineOptions.CachePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()) + "\\cache";
            Runtime.DefaultEngineOptions.ExtraCommandLineArgs = "--touch-events --disable-pinch --disable-usb-keyboard-detect --autoplay-policy=no-user-gesture-required --disable-site-isolation-trials --disable-features=CrossSiteDocumentBlockingIfIsolating,CrossSiteDocumentBlockingAlways --disable-web-security " + signParamsFile;
            Runtime.DefaultEngineOptions.DisableGPU = false;
            Runtime.DefaultEngineOptions.DisableSpellChecker = true;
            Runtime.DefaultEngineOptions.RemoteDebugPort = 30000;

            EO.Base.Runtime.EnableCrashReport = false;          //Disable the automatic report
            if (exRAM == "On")
            {
                EO.Base.Runtime.EnableEOWP = true;
                EO.Base.Runtime.InitWorkerProcessExecutable(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "exRAM.exe"));
            }
            if (exRAM == "Off")
            {
                EO.Base.Runtime.EnableEOWP = false;
                try
                {
                    if (File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "exRAM.exe")))
                    {
                        File.Delete(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "exRAM.exe"));
                    }
                }
                catch { }
            }
            EO.Base.Runtime.CrashDataAvailable += Eo_error;     //Handle CrashDataAvailable event
            EO.Base.Runtime.Exception += Eo_error2;             //Handle CrashDataAvailable event

            // Create the Options for the Browser Engine
            BrowserOptions options = new BrowserOptions
            {
                AllowZooming = false,
                EnableWebSecurity = false,
                AllowPlugins = true,
                AllowJavaScript = true,
                AllowJavaScriptAccessClipboard = false,
                AllowJavaScriptCloseWindow = false,
                AllowJavaScriptDOMPaste = false,
                LoadImages = true,
                UserStyleSheet = "html { height: 100vh !important; overflow: scroll; overflow-x: hidden; } ::-webkit-scrollbar { width: 0px; background: transparent; } #iframeBox { height: 100vh !important; } #viewport { height: 100vh !important; } "
            };
            EngineOptions.Default.SetDefaultBrowserOptions(options);            // Apply Options to the Browser Engine
            InitializeComponent();

            if (BrowserDebugMode == "On")
            {
                WebView.ShowDebugUI();
                FormBorderStyle = FormBorderStyle.FixedSingle;
                ControlBox = true;
                System.Windows.Forms.Cursor.Show();
                TopMost = false;
            }

            if (airServerConnected)
            {
                FormBorderStyle = FormBorderStyle.FixedSingle;
                TopMost = false;
            }

            if (!airServerConnected)
            {
                // Bring to Front and Focus
                BringToFront();
                Focus();
                Activate();
            }

            airServerMirroring = MainForm.airServerMirroring;           // Make Sure Variable has actually Set correctly since maybe its delayed updated
            if (airServerMirroring)
            {
                CheckForAirServerClient.Enabled = true;
            }

            if (MainForm.isInteractive) { TopMost = false; }

            var DesktopScr = MyIni.Read("Desktop", "Browser");
            if (DesktopScr == "Mosaic")
            {
                var numberScreens = 1;
                var mon2 = MyIni.Read("Monitor2", "Display");
                if (mon2 != "Disabled") { numberScreens++; }
                var mon3 = MyIni.Read("Monitor3", "Display");
                if (mon3 != "Disabled") { numberScreens++; }
                var mon4 = MyIni.Read("Monitor4", "Display");
                if (mon4 != "Disabled") { numberScreens++; }
                var mon5 = MyIni.Read("Monitor5", "Display");
                if (mon5 != "Disabled") { numberScreens++; }
                var mon6 = MyIni.Read("Monitor6", "Display");
                if (mon6 != "Disabled") { numberScreens++; }

                // If the Desktop Setting for the Browser is set to Mosaic this means that we want it to span 
                // using the Virtual Desktop System rather than individual screen
                int screenLeft = SystemInformation.VirtualScreen.Left;
                int screenTop = SystemInformation.VirtualScreen.Top;

                var resStr = MyIni.Read("Resolution", "Display");
                string[] resTokens = resStr.Split('x');
                var resW = Convert.ToInt32(resTokens[0]);
                var resH = Convert.ToInt32(resTokens[1]);

                // int screenWidth = SystemInformation.VirtualScreen.Width;
                // int screenHeight = SystemInformation.VirtualScreen.Height;
                int screenWidth = resW * numberScreens;
                int screenHeight = resH * numberScreens;

                // Set the Size of the Form and the Location of the Form, before telling the Window State to be "Normal".
                // If the system is in Debug Mode, there will be a 2px gap both on the left and right side, this is due to the Styling of the Form.
                Size = new Size(screenWidth, screenHeight);
                Location = new Point(screenLeft, screenTop);
                WindowState = FormWindowState.Normal;

                if (!MainForm.isSignageLoaded)
                {
                    // System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(1000)).ContinueWith( task => MainForm.RestartSignageForMosaic());
                }
            }
            FormClosed += new FormClosedEventHandler(SignageBrowser_FormClosed);
            SetupBrowser();     // Once everything is loaded up right, then startup the SignageBrowser
        }

        private void SignageBrowser_FormClosed(object sender, FormClosedEventArgs e)
        {
            WebView.CloseDebugUI();
            isSignageLoaded = false;
            MainForm.isSignageLoaded = false;
        }
        private void SignageBrowser_FormClosing(object sender, FormClosingEventArgs e)
        {
            ClearCookies();
            WebView.CloseDebugUI();
            isSignageLoaded = false;
            MainForm.isSignageLoaded = false;
            e.Cancel = true;
        }
        public static void CloseForm()
        {
            WebView.CloseDebugUI();
            isSignageLoaded = false;
            MainForm.isSignageLoaded = false;
            FrmObj.Dispose();
        }
        private void SetupBrowser()
        {
            // wBrowser.WebView.Url = "http://html5test.com/";                        // This is a test url that tests the performance of the browser
            // wBrowser.WebView.Url = "http://www.youtube.com/";                      // This is a test url that you can test video playback
            // wBrowser.WebView.Url = "http://www.toyota.ae/new-cars/camry/";         // This is a test url that you can test webgl
            // wBrowser.WebView.Url = "http://cadillac.aljomaihauto.com/new-vehicles/2018-xt5/?quailty=hq";         // This is a test url that you can test webgl in high quality
            // wBrowser.WebView.Url = "http://127.0.0.1:444";
            var MyIni = new IniFile(iniFile);
            var MySystemLoad = MyIni.Read("Load", "Browser");
            var BrowserSSL = MyIni.Read("SSL", "Browser");

            if (MySystemLoad == "Default")
            {
                if (BrowserSSL == "On")
                {
                    wBrowser.WebView.Url = "file://" + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace("#", "%23").Replace("\\", "/") + "/signage/preloader.html?ssl=on";
                } else
                {
                    wBrowser.WebView.Url = "file://" + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace("#", "%23").Replace("\\", "/") + "/signage/preloader.html?ssl=off";
                }
            }
            else
            {
                wBrowser.WebView.Url = MySystemLoad;
                MainForm.FrmObj.CheckSNAP.Stop();
                MainForm.FrmObj.CheckSNAP.Interval = 60000 + (MainForm.snapDelay * 1000);
            }

            // Setup Handlers
            wBrowser.WebView.StatusMessageChanged += WebView_StatusMessageChanged;
            wBrowser.WebView.IsLoadingChanged += WebView_IsLoadingChanged;
            wBrowser.WebView.UrlChanged += WebView_UrlChanged;
            wBrowser.WebView.TitleChanged += WebView_TitleChanged;
            wBrowser.WebView.CanGoBackChanged += WebView_CanGoBackChanged;
            wBrowser.WebView.CanGoForwardChanged += WebView_CanGoForwardChanged;
            wBrowser.WebView.ShouldForceDownload += WebView_ShouldForceDownload;
            wBrowser.WebView.BeforeDownload += WebView_BeforeDownload;
            wBrowser.WebView.NewWindow += WebView_NewWindow;
            if (MySystemKeyboard == "Application")
            {
                wBrowser.WebView.FocusedNodeChanged += WebView_FocusNodeChanged;
            }
            wBrowser.WebView.Closed += WebView_Closed;

            wBrowser.WebView.LoadFailed += new LoadFailedEventHandler(WebView_LoadFailed);
            wBrowser.WebView.RenderUnresponsive += new RenderUnresponsiveEventHandler(WebView_RenderUnresponsive);

            wBrowser.WebView.BeforePrint += new BeforePrintHandler(WebView_BeforePrint);
            wBrowser.WebView.BeforeContextMenu += new BeforeContextMenuHandler(WebView_BeforeContextMenu);
            wBrowser.WebView.CertificateError += new CertificateErrorHandler(WebView_CertError);
            wBrowser.WebView.BeforeRequestLoad += new BeforeRequestLoadHandler(WebView_LoadHandler);
            wBrowser.WebView.ConsoleMessage += new ConsoleMessageHandler(WebView_Console);
            wBrowser.WebView.Command += new CommandHandler(WebView_Command);

            wBrowser.WebView.JSExtInvoke += new JSExtInvokeHandler(WebView_JSExtInvoke);                // This is to allow Internal JS to run commands in .NET

            // Setup Hotkeys
            wBrowser.WebView.Shortcuts = new EO.WebBrowser.Shortcut[]
            {
                new EO.WebBrowser.Shortcut(CommandIds.Reload, KeyCode.F5, false, false, false),         // Hotkey - F5 (Reload)
                new EO.WebBrowser.Shortcut(nExitCommand, KeyCode.E, true, true, false),                 // Hotkey - CTRL + ALT + E (Exit Window)
                new EO.WebBrowser.Shortcut(CommandIds.Print, KeyCode.P, true, true, false),             // Hotkey - CTRL + ALT + P (Print)
                new EO.WebBrowser.Shortcut(nMaintCommand, KeyCode.M, true, true, false),                // Hotkey - CTRL + ALT + M (Maint Mode on Monitor)
            };

            // Update UI status
            WebView_UrlChanged(this, EventArgs.Empty);
            WebView_IsLoadingChanged(this, EventArgs.Empty);
            WebView_CanGoForwardChanged(this, EventArgs.Empty);
            WebView_CanGoBackChanged(this, EventArgs.Empty);

            if (MySystemKeyboard == "Javascript")
            {
                // Run Aftercare Javascript to allow us to Inject our own OSK
                wBrowser.WebView.JSInitCode = AddOSK(MySystemLoad, MySystemKeyboard);              // JSInitCode for On Screen Keyboard
            }
            MainForm.snapCounter = 0;
            isSignageLoaded = true;
            MainForm.isSignageLoaded = true;
            Debug.WriteLine("Current Signage Loaded Status: " + MainForm.isSignageLoaded.ToString());
        }

        public static string AddOSK(string MySystemLoad, string MySystemKeyboard)
        {
            var theOut = "";
            if (MySystemKeyboard == "Javascript")
            {
                Random r = new Random();
                int n = r.Next();

                if (MySystemLoad == "Default")
                {
                    theOut = "document.addEventListener(\"DOMContentLoaded\", function(event){ " +
                        "function addCSS() { " +
                            "var s = document.createElement('link'); " +
                            "s.setAttribute('rel', 'stylesheet'); " +
                            "s.setAttribute('href', '//sdk.globalcms.co.uk/preview/osk/css/gcmsKeyboard.css?rand=" + n + "'); " +
                            "document.getElementsByTagName('head')[0].appendChild(s); " +
                        "} " +

                        "function addScript( src, location ) { " +
                            "var s = document.createElement('script'); " +
                            "s.setAttribute('src', src); " +

                            "if (location == 'footer') { " +
                                "document.body.appendChild(s); " +
                            "} else { " +
                                "document.getElementsByTagName('head')[0].appendChild(s); " +
                            "} " +
                        "} " +

                        "function updateInputs() { " +
                            "var elementInputs = document.getElementsByTagName('input'); " +
                            "for (var i = 0; i < elementInputs.length; i++) elementInputs[i].className += ' gcmsKeyboard keyboard ' " +
                        "} " +

                        "function updateTxtBox() { " +
                            "var elementTxtBox = document.getElementsByTagName('textarea'); " +
                            "for (var i = 0; i < elementTxtBox.length; i++) elementTxtBox[i].className += ' gcmsKeyboard keyboard ' " +
                        "} " +

                        "setTimeout(function() { " +
                            "addCSS(); " +
                            "updateInputs(); " +
                            "updateTxtBox(); " +
                            "addScript( '//sdk.globalcms.co.uk/preview/osk/js/gcmsKeyboard2.js?rand=" + n + "', 'footer' ); " +
                        "}, 5000)" +
                    "}); ";
                }
                else
                {
                    theOut = "document.addEventListener(\"DOMContentLoaded\", function(event){ " +
                        "function addCSS() { " +
                            "var s = document.createElement('link'); " +
                            "s.setAttribute('rel', 'stylesheet'); " +
                            "s.setAttribute('href', '//sdk.globalcms.co.uk/preview/osk/css/gcmsKeyboard.css?rand=" + n + "'); " +
                            "document.getElementsByTagName('head')[0].appendChild(s); " +
                        "} " +

                        "function addScript( src, location ) { " +
                            "var s = document.createElement('script'); " +
                            "s.setAttribute('src', src); " +

                            "if (location == 'footer') { " +
                                "document.body.appendChild(s); " +
                            "} else { " +
                                "document.getElementsByTagName('head')[0].appendChild(s); " +
                            "} " +
                        "} " +

                        "function updateInputs() { " +
                            "var elementInputs = document.getElementsByTagName('input'); " +
                            "for (var i = 0; i < elementInputs.length; i++) elementInputs[i].className += ' gcmsKeyboard keyboard ' " +
                        "} " +

                        "function updateTxtBox() { " +
                            "var elementTxtBox = document.getElementsByTagName('textarea'); " +
                            "for (var i = 0; i < elementTxtBox.length; i++) elementTxtBox[i].className += ' gcmsKeyboard keyboard ' " +
                        "} " +

                        "setTimeout(function() { " +
                            "addScript( '//sdk.globalcms.co.uk/preview/osk/js/jquery.js?rand=" + n + "', 'header' ); " +
                            "addCSS(); " +
                            "updateInputs(); " +
                            "updateTxtBox(); " +
                            "addScript( '//sdk.globalcms.co.uk/preview/osk/js/gcmsKeyboard2.js?rand=" + n + "', 'footer' ); " +
                        "}, 5000)" +
                    "}); ";
                }
            }
            return theOut;
        }

        private void WebView_Command(object sender, CommandEventArgs e)
        {
            WebView webView = (WebView)sender;
            if (e.CommandId == nExitCommand)
            {
                // This Triggers the Form to close and to Close up the DebugUI
                WebView.CloseDebugUI();
                System.Windows.Forms.Cursor.Show();
                GCMSSystem.OSK.StopOSK();
                isSignageLoaded = false;
                MainForm.isSignageLoaded = false;
                Close();
            }
            if (e.CommandId == nMaintCommand)
            {
                // This is to Trigger the Monitor to go into Maint Mode, and Shutdown the Signage
                // Make sure Interactive is turned off
                MainForm.isInteractive = false;
                GCMSSystem.NodeSocket.Send("endinteractive");

                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                MyIni.Write("powersaveMode", "FALSE", "Network");
                MyIni.Write("powersaveMode2", "FALSE", "Network");
                MyIni.Write("maintMode", "TRUE", "Network");

                MainForm.FrmObj.powerModeLabel.Text = "Maintenance";
                MainForm.FrmObj.powerModeLabel.ForeColor = Color.FromArgb(255, 128, 0);

                bool isSignageEnabled = GCMSSystem.Chrome.IsSignageEnabled();
                if (isSignageEnabled)
                {
                    var nodeRunningNow = "NO";
                    var nodeProcessNew = Process.GetProcesses().Any(p => p.ProcessName.Contains("node"));
                    if (nodeProcessNew)
                    {
                        nodeRunningNow = "YES";
                    }

                    if (nodeRunningNow == "NO")
                    {
                        var osArch = GCMSSystem.GetOSArch();
                        var nodeEXE = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\node32.exe";
                        if (osArch == "x64")
                        {
                            nodeEXE = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\node64.exe";
                        }

                        // NodeJS isnt running
                        Process nodeJS = new Process();
                        nodeJS.StartInfo.FileName = nodeEXE;
                        nodeJS.StartInfo.Arguments = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "signage.js") + " " + Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "settings.conf");
                        nodeJS.StartInfo.Verb = "runas";
                        nodeJS.StartInfo.UseShellExecute = false;
                        nodeJS.StartInfo.EnvironmentVariables["LOCALAPPDATA"] = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                        nodeJS.StartInfo.RedirectStandardOutput = false;
                        nodeJS.StartInfo.RedirectStandardError = false;
                        nodeJS.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                        var sh = (IShellDispatch4)Activator.CreateInstance(
                               Type.GetTypeFromProgID("Shell.Application"));
                        sh.ShellExecute(nodeEXE, Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "signage.js") + " " + Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "settings.conf"), null, null, 0);
                    }

                    if (GCMSSystem.Chrome.whichVer == 1 && !MainForm.isDebug)
                    {
                        GCMSSystem.Chrome.Unload();
                        GCMSSystem.Chrome.UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear
                    }

                    if (GCMSSystem.Chrome.whichVer == 2)
                    {
                        if (Application.OpenForms["SignageBrowser"] != null)
                        {
                            GCMSSystem.Chrome.Unload();
                        }
                    }
                }

                if (!CheckOpened("EngineeringTools"))
                {
                    var toolForm = new EngineerTools();
                    toolForm.Show();
                }

                if (isSignageEnabled)
                {
                    MainForm.FrmObj.CheckSNAP.Stop();
                    MainForm.FrmObj.CheckSNAP.Interval = 60000 + (MainForm.snapDelay * 1000);
                }

                WebView.CloseDebugUI();
                System.Windows.Forms.Cursor.Show();
                GCMSSystem.OSK.StopOSK();
                isSignageLoaded = false;
                MainForm.isSignageLoaded = false;
                Close();

            }
        }
        private void WebView_LoadFailed(object sender, LoadFailedEventArgs e)
        {
            var theError = "e.ErrorCode: " + e.ErrorCode + " ### e.ErrorMessage: " + e.ErrorMessage + " ### e.Url: " + e.Url;
            var LogFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()) + "\\logs\\eoloadfail.log";
            File.WriteAllText(LogFile, theError.ToString());
        }
        private void WebView_RenderUnresponsive(object sender, RenderUnresponsiveEventArgs e)
        {
            var theError = e.ToString();
            var LogFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()) + "\\logs\\eostack.log";
            File.WriteAllText(LogFile, theError.ToString());
        }
        private void Eo_error(object sender, EO.Base.CrashDataEventArgs e)
        {
            var LogFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()) + "\\logs\\eocrash.dat";
            File.WriteAllBytes(LogFile, e.Data);
        }
        private void Eo_error2(object sender, EO.Base.ExceptionEventArgs e)
        {
            var theError = e.ErrorException;
            var LogFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()) + "\\logs\\eoexception.log";
            File.WriteAllText(LogFile, theError.ToString());
        }
        private void WebView_BeforePrint(object sender, BeforePrintEventArgs e)
        {
            bool isLandscape = false;
            bool isInColor = false;
            int paperSizeW = Convert.ToInt32(100 * 8.27);
            int paperSizeH = Convert.ToInt32(100 * 11.69);

            var MyIni = new IniFile(iniFile);
            var printDefault = MyIni.Read("Default", "Printer");
            var printOverride = MyIni.Read("PrinterOverride", "Printer");
            var printAdv = MyIni.Read("PrinterAdv", "Printer");
            var printerPrintPreview = MyIni.Read("PrintPreview", "Printer");

            var printIn = MyIni.Read("PrintIn", "Printer");
            if (printIn == "Landscape") { isLandscape = true; }
            var printType = MyIni.Read("PrintType", "Printer");
            if (printType == "Colour") { isInColor = true; }
            var paperSize = MyIni.Read("PaperSize", "Printer");
            if (paperSize == "Receipt")
            {
                paperSizeW = Convert.ToInt32(100 * 3.14);
                paperSizeH = Convert.ToInt32(100 * 230.00);
            }

            var paperMarginStr = MyIni.Read("PaperMargin", "Printer");
            double paperMarginValue = Convert.ToDouble(paperMarginStr);
            int paperMargin = Convert.ToInt32(paperMarginValue) * 100;

            var printCopies = MyIni.Read("Copies", "Printer");
            e.PrinterSettings.Copies = Convert.ToInt16(printCopies);

            // Advanced Settings for even more customisation - must be manually turned on via config.ini
            if (printAdv == "On")
            {
                e.PageSettings.Color = isInColor;                                                           // Print in Colour or Black and White
                e.PageSettings.Landscape = isLandscape;                                                     // Print in Landscape [Default: Portrait)
                e.PageSettings.Margins = new Margins(paperMargin, paperMargin, paperMargin, paperMargin);   // 100 = 1 inch margin (hundreths of a inch)

                // Default is 8.27 inch x 11.69 inch [A4]
                e.PageSettings.PaperSize = new PaperSize("Custom", paperSizeW, paperSizeH);
            }

            if (printDefault != "On")
            {
                e.PrinterSettings.PrinterName = printOverride;
            }

            if (printerPrintPreview == "On") { e.Continue(true); }          // Show Print Preview
            if (printerPrintPreview == "Off") { e.Continue(false); }        // Hide Print Preview
        }
        private void WebView_JSExtInvoke(object sender, JSExtInvokeArgs e)
        {
            switch (e.FunctionName)
            {
                case "Interactive":
                    string whichURL = e.Arguments[0] as string;
                    string howLong = e.Arguments[1] as string;
                    wBrowser.WebView.Url = whichURL;
                    isXFrame = true;
                    GCMSSystem.NodeSocket.Send("startinteractive");
                    break;
            }
        }
        private void WebView_FocusNodeChanged(object sender, DOMNodeEventArgs e)
        {
            if ((e.Node != null) && (e.Node.TagName == "INPUT" || e.Node.TagName == "TEXTAREA") && e.Node.IsEditable)
            {
                if (MySystemKeyboard == "Application")
                {
                    TopMost = false;
                    GCMSSystem.OSK.StartOSK();
                    var keyboardProcessGUID = "Keyboard";
                    try
                    {
                        WindowHelper.BringToFront(keyboardProcessGUID);
                    }
                    catch { }
                    SignageBrowser.FrmObj.Activate();
                }
            }
            else {
                if (MySystemKeyboard == "Application")
                {
                    GCMSSystem.OSK.StopOSK();
                    TopMost = true;
                }
            }
        }
        private void WebView_NewWindow(object sender, NewWindowEventArgs e)
        {
            e.Accepted = false;
        }
        private void WebView_BeforeContextMenu(object sender, BeforeContextMenuEventArgs e)
        {
           e.Menu.Items.Clear();
        }

        private void WebView_Console(object sender, ConsoleMessageEventArgs e)
        {

        }

        private void WebView_CertError(object sender, CertificateErrorEventArgs e)
        {
            e.Continue();
        }

        private void WebView_LoadHandler(object sender, BeforeRequestLoadEventArgs e)
        {
            var MyIni = new IniFile(iniFile);
            var MyReferer = MyIni.Read("Referer", "Browser");
            if (MyReferer != "")
            {
                e.Request.Headers["Referer"] = MyReferer;         // Load from config.ini to allow the user to hijack the referal to bypass XSS Security
                
            }
        }

        private void WebView_BeforeDownload(object sender, BeforeDownloadEventArgs e)
        {
            MessageBox.Show(@"Download Handled for file: " + e.Item.MimeType);
        }

        private void WebView_ShouldForceDownload(object sender, ShouldForceDownloadEventArgs e)
        {
            //Force download PDF files. You can also check e.Url 
            //to force download certain Urls
            if (e.MimeType == "application/pdf")
                e.ForceDownload = true;
        }

        private void WebView_TitleChanged(object sender, EventArgs e)
        {
            if (MainForm.isDebug)
            { 
                Text = string.Format("Signage Browser - {0}", wBrowser.WebView.Title);
            }
        }

        private void WebView_Closed(object sender, EventArgs e)
        {
            Engine.Default.Stop(true);

            GC.Collect();
            GC.WaitForFullGCComplete();
            GC.WaitForPendingFinalizers();

            // Engine.Default.Start();
        }

        private void WebView_UrlChanged(object sender, EventArgs e)
        {
           
        }

        void WebView_StatusMessageChanged(object sender, EventArgs e)
        {
            
        }

        void WebView_IsLoadingChanged(object sender, EventArgs e)
        {
            // Update status bar to display "Loading..." or "Ready"
            WebView_StatusMessageChanged(this, EventArgs.Empty);
        }

        void WebView_CanGoBackChanged(object sender, EventArgs e)
        {

        }

        void WebView_CanGoForwardChanged(object sender, EventArgs e)
        {

        }

        public static void ClearCookies()
        {
            Engine.Default.Stop(true);
            Engine.CleanUpCacheFolders(CacheFolderCleanUpPolicy.AllVersions);

            var cachePatch = @Path.GetDirectoryName(Runtime.DefaultEngineOptions.CachePath) + "\\cache";
            DirectoryInfo di = new DirectoryInfo(cachePatch);
            foreach (FileInfo file in di.GetFiles())
            {
                try
                {
                    file.Delete();
                }
                catch
                {
                    if (MainForm.isDebug)
                    {
                        // Debug.WriteLine(string.Format("Error Removing File - {0}", file.Name));
                    }
                }
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                try
                {
                    if (dir.Name != "Cache" && dir.Name != "GPUCache")
                    {
                        dir.Delete(true);
                    }
                }
                catch
                {
                    if (MainForm.isDebug)
                    {
                        // Debug.WriteLine(string.Format("Error Removing Folder - {0}", dir.Name));
                    }
                }
            }

            GC.Collect();
            GC.WaitForFullGCComplete();
            GC.WaitForPendingFinalizers();

            Engine.Default.Start();
        }

        public static bool CheckOpened(string name)
        {
            FormCollection fc = Application.OpenForms;

            foreach (Form frm in fc)
            {
                if (frm.Text == name)
                {
                    return true;
                }
            }
            return false;
        }

        static SignageBrowser _frmObj;
        public static SignageBrowser FrmObj
        {
            get { return _frmObj; }
            set { _frmObj = value; }
        }

        private void SignageBrowser_Load(object sender, EventArgs e)
        {
            FrmObj = this;
        }

        private void AlwaysOnTop_Tick(object sender, EventArgs e)
        {
            var MyIni = new IniFile(iniFile);
            var EngineerMode = MyIni.Read("Network", "maintMode");
            var LowPowerMode1 = MyIni.Read("Network", "powersaveMode");
            var LowPowerMode2 = MyIni.Read("Network", "powersaveMode2");

            if (EngineerMode == "TRUE" || LowPowerMode1 == "TRUE" || LowPowerMode2 == "TRUE") { Close(); return; }

            // Bring to Front and Focus
            if ((!airServerMirroring || !airServerConnected) && !MainForm.isInteractive)
            {
                // if (!MainForm.frmObj.isInteractive) { TopMost = true; }
                if (browserDebugMode == "On")
                {
                    FormBorderStyle = FormBorderStyle.FixedSingle;
                    System.Windows.Forms.Cursor.Show();
                    TopMost = false;
                }
                BringToFront();
                Focus();
                Activate();
            }

            if (airServerConnected)
            { 
                TopMost = false;
            }

            if (MainForm.isInteractive)
            {
                TopMost = false;
            }
        }

        private void CheckForAirServerClient_Tick(object sender, EventArgs e)
        {
            var MyIni = new IniFile(iniFile);
            var EngineerMode = MyIni.Read("Network", "maintMode");
            var LowPowerMode1 = MyIni.Read("Network", "powersaveMode");
            var LowPowerMode2 = MyIni.Read("Network", "powersaveMode2");

            if (EngineerMode == "TRUE" || LowPowerMode1 == "TRUE" || LowPowerMode2 == "TRUE") { Close(); return; }

            if (MainForm.isInteractive) { TopMost = false; }

            // We wait until the counter hits 10, this will give the system enough time to load AirServer
            if (!MainForm.isInteractive) { airServerConnectedCounter++; }
            if (airServerConnectedCounter >= 10 && !MainForm.isInteractive)
            {
                Process _airserverProcess = null;
                try
                {
                    _airserverProcess = Process.GetProcesses().Where(x => x.ProcessName.Equals("AirServer")).DefaultIfEmpty(null).FirstOrDefault();
                }
                catch { }

                if (_airserverProcess != null)
                {
                    var _windowHandle = FindWindow(null, "AirServer® Universal");
                    var _parent = GetParent(_windowHandle);
                    if (_parent != _airserverProcess.MainWindowHandle)
                    {
                        // var airserverLockFrm = new AirServerLock();
                        // airserverLockFrm.Show();

                        // Detected the AirServer has a connected client
                        airServerConnected = true;

                        MainForm.FrmObj.TrialLicTxt.Text = "MIRRORING";
                        MainForm.FrmObj.TrialLicTxt.Visible = true;

                        if (airServerConnectedTopOnce)
                        {
                            // Bring the AirServer Window to the front
                            IntPtr hWnd = _airserverProcess.MainWindowHandle;
                            SetForegroundWindow(hWnd);
                            airServerConnectedTopOnce = false;
                        }
                        TopMost = false;
                        try
                        {
                            // AudioManager.SetMasterVolumeMute(true);
                            AudioManager.SetMasterVolume(1);
                        }
                        catch { }
                        Hide();
                    }
                    else
                    {
                        // AirServer has 0 connected clients
                        Show();
                        try
                        {
                            AudioManager.SetMasterVolumeMute(false);
                            AudioManager.SetMasterVolume(curMasterVol);
                        }
                        catch { }
                        airServerConnected = false;
                        airServerConnectedTopOnce = true;

                        MainForm.FrmObj.TrialLicTxt.Visible = false;
                        MainForm.FrmObj.TrialLicTxt.Text = "TRIAL LICENCE";
                    }
                }
                else
                {
                    // AirServer has 0 connected clients
                    airServerConnected = false;
                    airServerConnectedTopOnce = true;
                    MainForm.FrmObj.TrialLicTxt.Visible = false;
                    MainForm.FrmObj.TrialLicTxt.Text = "TRIAL LICENCE";
                }
            }

            if (airServerConnectedCounter > 1000 && !MainForm.isInteractive)
            {
                airServerConnectedCounter = 10;
            }
        }
    }
}