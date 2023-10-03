using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using EO.WebBrowser;
using EO.WebEngine;

namespace GlobalCMS
{
    public partial class LiveSupport : Form
    {
        string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");

        public LiveSupport()
        {
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

            Runtime.DefaultEngineOptions.ExtraCommandLineArgs = "--touch-events --disable-pinch --disable-usb-keyboard-detect --autoplay-policy=no-user-gesture-required --disable-site-isolation-trials --disable-features=CrossSiteDocumentBlockingIfIsolating,CrossSiteDocumentBlockingAlways --disable-web-security";
            Runtime.DefaultEngineOptions.DisableGPU = false;
            Runtime.DefaultEngineOptions.DisableSpellChecker = true;
            Runtime.DefaultEngineOptions.RemoteDebugPort = 30000;

            EO.Base.Runtime.EnableEOWP = false;
            EO.Base.Runtime.EnableCrashReport = false;
            EO.Base.Runtime.CrashDataAvailable += Eo_error;
            EO.Base.Runtime.Exception += Eo_error2;

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

            // Bring to Front and Focus
            BringToFront();
            Focus();
            Activate();

            SetupBrowser();
        }

        private void SetupBrowser()
        {
            var MyIni = new IniFile(iniFile);
            var deviceID = MyIni.Read("deviceName", "Monitor");             // Required for Tracking Stats

            // wBrowser.WebView.Url = "http://html5test.com/";              // Demo URL for Testing        
            wBrowser.WebView.Url = "http://api.globalcms.co.uk/LiveChat/?deviceID=" + deviceID;                 // Load the URL for Live Support Chat        

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

            // wBrowser.WebView.GiveFocus += WebView_GiveFocus;
            // wBrowser.WebView.GotFocus += WebView_GotFocus;

            wBrowser.WebView.Closed += WebView_Closed;

            wBrowser.WebView.BeforeContextMenu += new BeforeContextMenuHandler(WebView_BeforeContextMenu);
            wBrowser.WebView.CertificateError += new CertificateErrorHandler(WebView_CertError);
            wBrowser.WebView.BeforeRequestLoad += new BeforeRequestLoadHandler(WebView_LoadHandler);
            wBrowser.WebView.ConsoleMessage += new ConsoleMessageHandler(WebView_Console);

            // Update UI status
            WebView_UrlChanged(this, EventArgs.Empty);
            WebView_IsLoadingChanged(this, EventArgs.Empty);
            WebView_CanGoForwardChanged(this, EventArgs.Empty);
            WebView_CanGoBackChanged(this, EventArgs.Empty);
        }

        private void Eo_error(object sender, EO.Base.CrashDataEventArgs e)
        {

        }
        private void Eo_error2(object sender, EO.Base.ExceptionEventArgs e)
        {

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
            // Debug.WriteLine(e.Message);   This is now disabled as DEVTOOLS has been integrated as an option in config.ini
        }

        private void WebView_GotFocus(object sender, EventArgs e)
        {

        }
        private void WebView_GiveFocus(object sender, GiveFocusEventArgs e)
        {

        }

        private void WebView_CertError(object sender, CertificateErrorEventArgs e)
        {
            e.Continue();
        }

        private void WebView_LoadHandler(object sender, BeforeRequestLoadEventArgs e)
        {
            
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
            Text = "SupportChat";
        }

        private void WebView_Closed(object sender, EventArgs e)
        {
            Engine.Default.Stop(true);

            GC.Collect();
            GC.WaitForFullGCComplete();
            GC.WaitForPendingFinalizers();

            Engine.Default.Start();
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

        static LiveSupport _frmObj;
        public static LiveSupport frmObj
        {
            get { return _frmObj; }
            set { _frmObj = value; }
        }

        private void LiveSupport_Load(object sender, EventArgs e)
        {
            frmObj = this;
        }
    }
}
