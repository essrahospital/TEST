using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Management;
using System.Net;
using Microsoft.Win32;
using System.Collections.Specialized;
using System.Drawing;
using System.Reflection;
using Shell32;
using neotrix_hdmi_tool;
using System.Windows.Input;
using System.Net.NetworkInformation;
using System.IO.Ports;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;

namespace GlobalCMS
{
    public partial class MainForm : Form
    {
        private static MainForm _frmObj;
        public static MainForm FrmObj
        {
            get { return _frmObj; }
            set { _frmObj = value; }
        }

        public static bool isDebug = false;
        public static bool isOnline = false;
        public static bool isAutoCookieCleaner = false;
        public static bool isTrial = false;
        public static bool isInLockdown = false;
        public static bool isUselessInternet = false;
        public static bool isSignageLoaded = false;
        public static string isUselessInternetTxt = "Disabled";
        public static bool forceResolution = false;
        public static int signageLoader;
        public static string pingMS;
        public static string eoVersion = "20.1.45.0";
        public static bool omronSensor = false;
        public static string EOversionInfo = eoVersion;
        public static string osArch = GCMSSystem.GetOSArch();

        public static bool airServerMirroring = false;
        public static bool nexmoshpereSensors = false;
        public static bool hardenedShell = false;
        public static string NetworkOverride = "Auto";

        public static int hardSNAP = 0;
        public static int snapCounter = 0;
        public static int snapDelay = 0;
        public static int curAudioLevel = 0;

        // Global Timer to store for Interaction
        public Timer Interaction = new Timer();
        public static bool isInteractive = false;

        // Global Timer for dealing with user inputs 
        Timer activityTimer = new Timer();
        TimeSpan activityThreshold = TimeSpan.FromMilliseconds(1000);

        Commission openedForm = null;
        // Load Skin Details
        string version = About.GetVersion("Main");                        // Version
        string subversion = About.GetVersion("Subversion");               // Sub Version

        string localIP = GCMSSystem.GetIP("LAN");                                    // Global Variable for Local IP
        string vpnIP = GCMSSystem.GetIP("VPN");                                      // Global Variable for VPN IP
        string wanIP = GCMSSystem.GetIP("WAN");                                      // Global Variable

        string networkURL;                                                // Which Network URL to use
        string networkIP;                                                 // Which Network IP to use - This is for the Ping Checker
        string networkName;                                               // Network Name - Placeholder to take variable later on
        public string networkNameShort;                                          // Network Name Short - A Single Letter P (Public) / S (Secure)

        string signageVersion;                                            // Blank signageVersion for loading signage\version.txt file into
        string signageSubVersion;                                         // Blank signageSubVersion for loading signage\subversion.txt into
        string screen_signage_running;                                    // Blank for checking if signage is running or not

        long trialStart = 0;                                              // Blank Long Record for Trial Start UnixTime
        long trialEnd = 0;                                                // Blank Long Record for Trial End UnixTime

        int nProcessID = Process.GetCurrentProcess().Id;     // This is the process ID for the CURRENT process, to look up Child PIDs you need the PARENT PID
        WebServer Webserver1 = null;          
        SocketServer Socketserver1 = null;

        string lockFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "decom.lock");      // Decom Lockdown File
        string trialFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "trial.lock");    // Trial Lockdown File
        string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");      // Application Config
        string signParamFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "signageParams.ini");     // Signage Config
        string powerParamsFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "powerConfig.ini");   // Stored Power Config
        string pidFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "application.pid");      // Application PID

        public MainForm()
        {
            // !!!! IMPORTANT !!!! Due to starting this application as a service, we need to make sure we set the WORKING DIR to the Assembly Directory !!!! IMPORTANT !!!!
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()));
            CheckForIllegalCrossThreadCalls = false;                                    // This allows other forms in the application to cross send data from multi threads
            GCMSSystem.FileLogger.Log(" ");                                             // Add to top of the log so that it doesnt get sent every single time
            GCMSSystem.FileLogger.Log("==================================");            // Add to top of the log so that it doesnt get sent every single time
            GCMSSystem.FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - Started Application with pID : " + nProcessID);

            // Embed as many of the DLL Files that are able to be Embedded Resources (Remember : Add DLL to the libs folder and then add ref for it)
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string resourceName = new AssemblyName(args.Name).Name + ".dll";
                string resource = Array.Find(GetType().Assembly.GetManifestResourceNames(), element => element.EndsWith(resourceName));

                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
                {
                    byte[] assemblyData = new byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };

            InitializeComponent();                                           // int System

            // Update from old version1 to new version2
            if (Directory.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "downloadtmp")))
            {
                // Detected The Download TMP Folder - Means that this has just been updated, Delete TMP Folder
                try
                {
                    Directory.Delete(@Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "downloadtmp"), true);
                }
                catch { }
            }
            if (File.Exists(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "screenlock.exe")))
            {
                try
                {
                    File.Delete(@Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "screenlock.exe"));
                }
                catch { }
            }
            try { GCMSSystem.SysInternal.Update(); } catch { }
            try { GCMSSystem.PreStartup(); } catch { }
            try { GCMSSystem.CheckKeyboardInstall(); } catch { }
            try { GCMSSystem.EOCheck(); } catch { }
            try { GCMSSystem.AirServer.Kill(); } catch { }               // To Make sure that if the program crashes it will Kill AirServer 1st before loading it
            try
            {
                File.WriteAllText(pidFile, "");         // Blank the PID File so that the service can check this file for a valid PID
            }
            catch { }

            // Windows Based Service Checker / Installer and Startup.
            var serviceFile = GCMSSystem.WinService.CheckInstalled("GlobalCMS");
            if (serviceFile == "Not_Installed")
            {
                // Windows Service is NOT INSTALLED so we need to call the class to Install the Service
                var serviceFileExe = new string[] {
                    @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\GlobalCMS_Service.exe"
                };

                try
                {
                    GCMSSystem.WinService.Install(serviceFileExe, "GlobalCMS");
                }
                catch { }
            }
            var serviceRunning = GCMSSystem.WinService.CheckRunning("GlobalCMS");
            if (serviceRunning == "Stopped")
            {
                if (!isDebug)
                {
                    try
                    {
                        GCMSSystem.WinService.Start("GlobalCMS");
                    }
                    catch { }
                }

            }

             // Check Chrome is already open if so to close it
            var startChromeRunning = "No";
            bool chromeProcessStart = false;
            try
            {
                chromeProcessStart = Process.GetProcesses().Any(p => p.ProcessName.Contains("chrome"));
            }
            catch { }

            if (chromeProcessStart)
            {
                startChromeRunning = "Yes";
            }

            if (startChromeRunning == "Yes")
            {
                if (!isDebug)
                {
                    // Make sure that Node isnt running
                    foreach (var process in Process.GetProcessesByName("node32"))
                    {
                        try
                        {
                            process.StartInfo.Verb = "runas";
                            process.Kill();
                            process.WaitForExit(60000);
                        }
                        catch { }
                    }
                    foreach (var process in Process.GetProcessesByName("node64"))
                    {
                        try
                        {
                            process.StartInfo.Verb = "runas";
                            process.Kill();
                            process.WaitForExit(60000);
                        }
                        catch { }
                    }

                    try
                    {
                        GCMSSystem.Chrome.Unload();
                        GCMSSystem.Chrome.UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear
                    }
                    catch { }
                }
            }

            if (!File.Exists(signParamFile))
            {
                try
                {
                    File.Create(signParamFile).Dispose();
                }
                catch { }
            }
            if (!File.Exists(powerParamsFile))
            {
                try
                {
                    File.Create(powerParamsFile).Dispose();
                }
                catch { }
                GCMSSystem.TriggerSystem("UPDATEOFFLINEPWR", true, false);
            }

            try
            {
                GCMSSystem.Chrome.UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear
            }
            catch { }

            // Checker to see if this program is running in "Trail Mode"
            bool trailLock = false;
            var trialRemaining = "0";
            if (File.Exists(trialFile))
            {
                isTrial = true;

                DateTime trailCreation = File.GetCreationTime(trialFile);
                DateTime trailModification = File.GetLastWriteTime(trialFile);

                int trailDayLimit = 30;

                trialStart = GCMSSystem.UnixTime.Convert(trailModification);
                DateTime trialEndDate = trailCreation.AddDays(trailDayLimit);
                trialEnd = GCMSSystem.UnixTime.Convert(trialEndDate);

                DateTime today = DateTime.Today;

                var totalDays = (today - trailModification).TotalDays;
                if (totalDays > trailDayLimit)
                {
                    trailLock = true;
                }

                trialRemaining = Math.Round((trailDayLimit - totalDays)).ToString();

                // Start the Checker for Trial Licence to make sure the Timebomb kicks in!
                CheckTrial.Interval = 60000;
                CheckTrial.Enabled = true;
            }
            if (!File.Exists(trialFile))
            {
                isTrial = false;
                trailLock = false;
            }

            if (!localIP.Contains("192.168.") && !localIP.Contains("10.") && !localIP.Contains("172.32.") && !localIP.Contains("172.16.8.") && !localIP.Contains("172.16.73.") && !localIP.Contains("172.30.") && !localIP.Contains("172.17.") && !localIP.Contains("172.20."))
            {
                localIP = "Not Connected";
            }

            if (!vpnIP.Contains("172.16."))
            {
                vpnIP = "Loading ...";
            }

            // IP Details
            devVPN.Text = vpnIP;
            devWAN.Text = wanIP;
            devLAN.Text = localIP;

            TaskbarContextMenu.Items[2].Text = "LAN \t" + localIP;
            TaskbarContextMenu.Items[3].Text = "WAN \t" + wanIP;
            TaskbarContextMenu.Items[4].Text = "VPN \t" + vpnIP;

            // MAC Address
            var macAddr = GCMSSystem.GetMACAddress();
            if (File.Exists(iniFile))
            {
                // Read INI File for Config.ini
                var MyIni = new IniFile(iniFile);
                Visible = true;
                Enabled = true;
                WindowState = FormWindowState.Minimized;

                try { GCMSSystem.TriggerSystem("UPDATEAVACONF", true, false); } catch { }
                try { GCMSSystem.TriggerSystem("MONON", true, false); } catch { }
                try { GCMSSystem.TriggerSystem("SCREENON", true, true); } catch { }
                try { EngineerTools.DetectNetworkAdapter(); } catch { }

                // Checksum for a broken INI File, if any variables are missing to regen its INI File, so delete the broken INI and simply restart program


                // Add Variable for the Security Lockdowns
                var ShellSecurity = MyIni.Read("ShellSecurity", "Monitor");
                if (ShellSecurity == "")
                {
                    // If the Value for Referer doesnt exist then to backfill with it being "On"
                    MyIni.Write("ShellSecurity", "On", "Monitor");
                    hardenedShell = true;
                }
                if (ShellSecurity == "On") { hardenedShell = true; }
                if (ShellSecurity == "Off") { hardenedShell = false; }
                if (!isDebug && hardenedShell)
                {
                    try
                    {
                        Taskbar.Hide();
                    }
                    catch { }
                }
                // Add Variable for overriding the Network we are downloading from
                var ForceNetwork = MyIni.Read("UpdateNetwork", "Monitor");
                if (ForceNetwork == "")
                {
                    // If the Value for Referer doesnt exist then to backfill with it being "Auto"
                    MyIni.Write("UpdateNetwork", "Auto", "Monitor");
                    ForceNetwork = "Auto";
                }
                NetworkOverride = ForceNetwork;

                // Check for a machine that has been previously setup but then Force Decommissioned for some reason or another
                var checkForDecom = GCMSSystem.CheckForDecom(macAddr, MyIni.Read("clientID", "Monitor"), MyIni.Read("deviceName", "Monitor"));
                if (checkForDecom == "NOEXIST")
                {
                    GCMSSystem.TriggerSystem("DECOM", false, false);
                }
                if (checkForDecom == "DECOMMISSIONED" && !File.Exists(lockFile))
                {
                    GCMSSystem.TriggerSystem("DECOM", false, false);
                }
                if (checkForDecom == "COMMISSIONED" && File.Exists(lockFile))
                {
                    GCMSSystem.TriggerSystem("REACTIVATE", false, false);
                }

                try
                {
                    LastLogMsgOpt.Text = DateTime.Now.ToString("dd MMM HH:mm:ss") + " - Started Application with pID : " + nProcessID;
                }
                catch { }

                string appIniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "launcher.ini");
                if (!File.Exists(appIniFile))
                {
                    File.Create(appIniFile).Dispose();
                }
                const int BufferSize = 128;
                using (var fileStream = File.OpenRead(appIniFile))
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        string[] separatingChars = { "?" };
                        string[] lineData = line.Split(separatingChars, StringSplitOptions.RemoveEmptyEntries);
                        var isApplicationEnabled = lineData[0];
                        var whatApplicationName = lineData[1];
                        var whatApplicationMonitor = lineData[2];

                        bool isAppRunning = false;
                        try
                        {
                            if (osArch == "x64") { 
                                isAppRunning = GCMSSystem.ProgramIsRunning(@whatApplicationMonitor);
                            }
                        }
                        catch { }
                        if (!isAppRunning)
                        {
                            if (isApplicationEnabled == "Enabled")
                            {
                                try
                                {
                                    Process appLauncher = new Process();
                                    appLauncher.StartInfo.FileName = whatApplicationName;
                                    appLauncher.StartInfo.Verb = "runas";
                                    appLauncher.Start();
                                }
                                catch { }
                            }
                        }
                    }
                }

                // Setup which Network we should run over
                var MyNetwork = MyIni.Read("licType", "Licence");
                if (MyNetwork != "SEC")
                {
                    devVPN.Text = "Unavailable";
                }

                // Different Running Modes
                var meLowPowerMode1 = MyIni.Read("powersaveMode", "Network");               // Non RS232 Low Power Mode
                var meLowPowerMode2 = MyIni.Read("powersaveMode2", "Network");              // RS232 Low Power Mode
                var meMaintMode = MyIni.Read("maintMode", "Network");                       // Maintenance Mode

                var MySkin = MyIni.Read("SkinID", "Skin");
                try { Themes.Generate(MySkin); } catch { }

                var jsonCallHomeTimer = MyIni.Read("CH", "Sign");
                if (jsonCallHomeTimer == "")
                {
                    try { GCMSSystem.Chrome.UpdateCallHome(); } catch { }
                }

                // Interent Mode Variables
                var poorInternetConnection = MyIni.Read("poorInternet", "Monitor");
                if (poorInternetConnection == "")
                {
                    // If the Value for poorInternet doesnt exist then to backfill with it being "Disabled"
                    MyIni.Write("poorInternet", "Disabled", "Monitor");             // Disabled
                    poorInternetConnection = "Disabled";
                }
                if (poorInternetConnection == "Disabled") { isUselessInternet = false; isUselessInternetTxt = "Disabled"; }
                if (poorInternetConnection == "Enabled") { isUselessInternet = true; isUselessInternetTxt = "Enabled"; }

                var poorInternetDelay = MyIni.Read("poorInternetDelay", "Monitor");
                if (poorInternetDelay == "")
                {
                    // If the Value for poorInternet doesnt exist then to backfill with it being "Disabled"
                    MyIni.Write("poorInternetDelay", "0", "Monitor");             // Disabled
                    poorInternetDelay = "0";
                }
                snapDelay = Convert.ToInt32(poorInternetDelay);

                // Mosaic Screen Variables
                var envServerService = MyIni.Read("Env", "Serv");
                if (envServerService == "")
                {
                    // If the Value for ENV doesnt exist then to backfill with it being "Disabled"
                    MyIni.Write("Env", "Disabled", "Serv");             // Disabled = Hide Form rather than Shutdown the service
                    envServerService = "Disabled";
                }

                // AirServer (Mirroring) Variables
                var airServerService = MyIni.Read("Mirroring", "Serv");
                if (airServerService == "")
                {
                    // If the Value for AirServer (Mirroring) doesnt exist then to backfill with it being "Enabled"
                    MyIni.Write("Mirroring", "Enabled", "Serv");
                    airServerService = "Disabled";
                }
                var airServerServiceLic = MyIni.Read("AS", "Licence");
                if (airServerServiceLic == "")
                {
                    // If the Value for AirServer (Mirroring) doesnt exist then to backfill with it being "Disabled"
                    MyIni.Write("AS", "NotInstalled", "Licence");
                }
                if (airServerService == "Enabled" && airServerServiceLic != "NotInstalled") { MainForm.airServerMirroring = true; }           // Set the Global Variable for airServerService
                if (airServerService == "Disabled" && airServerServiceLic != "NotInstalled") { MainForm.airServerMirroring = false; }         // Set the Global Variable for airServerService

                // Add Variable for the Audio Level Settings - Now Required for AirServer
                var AudioLevel = MyIni.Read("MasterLevel", "Audio");
                if (AudioLevel == "")
                {
                    // If the Value for Referer doesnt exist then to backfill with it being ""
                    MyIni.Write("MasterLevel", "1", "Audio");
                    curAudioLevel = 1;
                }

                // Do we want the Cookie Cleaner to run after Interactive Mode finishes?
                var autoCookieCleaner = MyIni.Read("CookieCleaner", "Interactive");
                if (autoCookieCleaner == "")
                {
                    // If the Value for CookieCleaner doesnt exist then to backfill with it being "Off"
                    MyIni.Write("CookieCleaner", "Off", "Interactive");
                    autoCookieCleaner = "Off";
                }
                // Add Variable for the Browser EOWP Enable / Disable
                var ExRAM = MyIni.Read("ExRAM", "Browser");
                if (ExRAM == "")
                {
                    // If the Value for ExRAM doesnt exist then to backfill with it being "Off"
                    MyIni.Write("ExRAM", "Off", "Browser");
                }
                try
                {
                    var exRAM_File = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "exRAM.exe");
                    if (File.Exists(exRAM_File))
                    {
                        var exRAM_FileVersion = FileVersionInfo.GetVersionInfo(exRAM_File).FileVersion;
                        // If the exRAM file is a different version to version of installed EO then this is becuase EO has also been updated in its version
                        // So we need to remove the exRAM.exe file to let it re-generate
                        if (exRAM_FileVersion != eoVersion)
                        {
                            File.Delete(exRAM_File);
                        }
                    }
                }
                catch { }

                // Mosaic Variables
                var DesktopScr = MyIni.Read("Desktop", "Browser");
                if (DesktopScr == "")
                {
                    // If the Value for DesktopScr doesnt exist then to backfill with it being "Standard"
                    MyIni.Write("Desktop", "Standard", "Browser");             // Standard = Single Monitor Version / Mosaic = Multi Monitor
                }

                // Add Variable for the Browser Referer
                var BrowserLoad = MyIni.Read("Load", "Browser");
                if (BrowserLoad == "")
                {
                    // If the Value for Referer doesnt exist then to backfill with it being ""
                    MyIni.Write("Load", "Default", "Browser");
                }
                // Add Variable for the Browser Referer
                var MyReferer = MyIni.Read("Referer", "Browser");
                if (MyReferer == "")
                {
                    // If the Value for Referer doesnt exist then to backfill with it being ""
                    MyIni.Write("Referer", "", "Browser");
                }
                // Add Variable for the Browser Referer
                var BrowserDebug = MyIni.Read("Debug", "Browser");
                if (BrowserDebug == "")
                {
                    // If the Value for Browser Debug doesnt exist then to backfill with it being "Off"
                    MyIni.Write("Debug", "Off", "Browser");
                }

                // Add Variable for the Browser Keyboard
                var BrowserKeyboard = MyIni.Read("Keyboard", "Browser");
                if (BrowserKeyboard == "")
                {
                    // If the Value for Browser Keyboard doesnt exist then to backfill with it being "Default"
                    MyIni.Write("Keyboard", "Default", "Browser");
                }

                // Add Variable for the Browser Keyboard
                var BrowserSSL = MyIni.Read("SSL", "Browser");
                if (BrowserSSL == "")
                {
                    // If the Value for Browser SSL doesnt exist then to backfill with it being "Off"
                    MyIni.Write("SSL", "Off", "Browser");
                    string signageConfFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "settings.conf");
                    var SettingsIni = new IniFile(signageConfFile);
                    SettingsIni.Write("sslOn", "false", "core");
                }

                // Add Variable for the Default Printer On/Off
                var PrinterDefault = MyIni.Read("Default", "Printer");
                if (PrinterDefault == "")
                {
                    // If the Value for Printer Default doesnt exist then to backfill with it being "On"
                    MyIni.Write("Default", "On", "Printer");
                }

                // Add Variable for the Printer Copies
                var PrinterCopies = MyIni.Read("Copies", "Printer");
                if (PrinterCopies == "")
                {
                    // If the Value for Printer Copies doesnt exist then to backfill with it being "1"
                    MyIni.Write("Copies", "1", "Printer");
                }

                // Add Variable for the Printer - Print In
                var PrinterPrintIn = MyIni.Read("PrintIn", "Printer");
                if (PrinterPrintIn == "")
                {
                    // If the Value for Printer Print In doesnt exist then to backfill with it being "Portrait"
                    MyIni.Write("PrintIn", "Portrait", "Printer");
                }

                // Add Variable for the Printer - Print Type
                var PrinterPrintType = MyIni.Read("PrintType", "Printer");
                if (PrinterPrintType == "")
                {
                    // If the Value for Printer Print Type doesnt exist then to backfill with it being "BW"
                    MyIni.Write("PrintType", "Black and White", "Printer");
                }

                var PrinterPaperSize = MyIni.Read("PaperSize", "Printer");
                if (PrinterPaperSize == "")
                {
                    // If the Value for Printer Paper Size doesnt exist then to backfill with it being "A4"
                    MyIni.Write("PaperSize", "A4", "Printer");
                }

                var PrinterPaperMargin = MyIni.Read("PaperMargin", "Printer");
                if (PrinterPaperMargin == "")
                {
                    // If the Value for Printer Paper Margin doesnt exist then to backfill with it being "1.00"
                    MyIni.Write("PaperMargin", "1.00", "Printer");
                }

                // Add Variable for the Printer Name (If Not Default)
                var PrinterOverride = MyIni.Read("PrinterOverride", "Printer");
                if (PrinterOverride == "")
                {
                    // If the Value for Printer Override doesnt exist then to backfill with it being ""
                    MyIni.Write("PrinterOverride", "", "Printer");
                }

                // Add Variable for the Printer Advanced Settings (If Not Default)
                var PrintPreview = MyIni.Read("PrintPreview", "Printer");
                if (PrintPreview == "")
                {
                    // If the Value for Printer Override doesnt exist then to backfill with it being ""
                    MyIni.Write("PrintPreview", "Off", "Printer");
                }

                // Downloader Bandwidth Lock
                var downloaderBandwidthLock = MyIni.Read("MaxDownload", "Monitor");
                if (downloaderBandwidthLock == "") { MyIni.Write("MaxDownload", "100", "Monitor"); downloaderBandwidthLock = "100"; }

                // Add Variable for the Printer Advanced Settings (If Not Default)
                var PrinterAdv = MyIni.Read("PrinterAdv", "Printer");
                if (PrinterAdv == "")
                {
                    // If the Value for Printer Override doesnt exist then to backfill with it being ""
                    MyIni.Write("PrinterAdv", "Off", "Printer");
                }

                // Which Signage Loader are we using --- 1: Chrome     2: Our Own Browser
                var signageLoaderEnabled = MyIni.Read("Signage", "Serv");
                var signageLoaderVer = MyIni.Read("SignageLoader", "Sign");
                if (signageLoaderVer == "")
                {
                    // If the Value for CookieCleaner doesnt exist then to backfill with it being "Off"
                    MyIni.Write("SignageLoader", "2", "Sign");
                    signageLoaderVer = "2";
                }
                signageLoader = Convert.ToInt32(signageLoaderVer);
                // Debug.WriteLine("Signage Loader Version : " + signageLoader);

                // Time to load the Variables for setting the Graphics.
                var forceGFX = MyIni.Read("Force", "Display");

                var cloneScr = MyIni.Read("Clone", "Display");
                if (cloneScr == "") { MyIni.Write("Clone", "Off", "Display"); cloneScr = "Off"; }

                var forceGFX_Res = MyIni.Read("Resolution", "Display");
                var forceGFX_Scaling = MyIni.Read("Scaling", "Display");
                var forceGFX_Orientation = MyIni.Read("Orientation", "Display");

                var forceGFX_Monitor1 = MyIni.Read("Monitor1", "Display");
                if (forceGFX_Monitor1 == "") { MyIni.Write("Monitor1", "Disabled", "Display"); forceGFX_Monitor1 = "Landscape"; }

                var forceGFX_Monitor2 = MyIni.Read("Monitor2", "Display");
                if (forceGFX_Monitor2 == "") { MyIni.Write("Monitor2", "Disabled", "Display"); forceGFX_Monitor2 = "Disabled"; }

                var forceGFX_Monitor3 = MyIni.Read("Monitor3", "Display");
                if (forceGFX_Monitor3 == "") { MyIni.Write("Monitor3", "Disabled", "Display"); forceGFX_Monitor3 = "Disabled"; }

                var forceGFX_Monitor4 = MyIni.Read("Monitor4", "Display");
                if (forceGFX_Monitor4 == "") { MyIni.Write("Monitor4", "Disabled", "Display"); forceGFX_Monitor4 = "Disabled"; }

                var forceGFX_Monitor5 = MyIni.Read("Monitor5", "Display");
                if (forceGFX_Monitor5 == "") { MyIni.Write("Monitor5", "Disabled", "Display"); forceGFX_Monitor5 = "Disabled"; }

                var forceGFX_Monitor6 = MyIni.Read("Monitor6", "Display");
                if (forceGFX_Monitor6 == "") { MyIni.Write("Monitor6", "Disabled", "Display"); forceGFX_Monitor6 = "Disabled"; }

                if (forceGFX == "")
                {
                    // If the Value for Display Force doesnt exist then to backfill with it being "Off"
                    MyIni.Write("Force", "Off", "Display");
                    forceResolution = false;
                    GCMSSystem.TriggerSystem("UPDATESCRCFG", false, false);               // If there is no record we need to check to see if the user has a custom screen config setup
                }
                if (forceGFX == "On") { forceResolution = true; }

                if (autoCookieCleaner != "")
                {
                    if (autoCookieCleaner == "On") { isAutoCookieCleaner = true; } else { isAutoCookieCleaner = false; }
                }

                curAudioLevel = Convert.ToInt32(MyIni.Read("MasterLevel", "Audio"));                       // Master Volume Control

                if (!File.Exists(lockFile))
                {
                    CheckVpnService();       // VPN Checker Service
                    CheckerService(meLowPowerMode1, meLowPowerMode2, meMaintMode);       // Run checker service to kick in all the different programs/services
                }

                if (MyNetwork == "SEC")
                {
                    // Due to SSL Issues over an IP Address - We Use IP Instead of Domain
                    networkURL = "http://172.16.0.2";
                    networkIP = "172.16.0.2";
                    networkName = "Secure";
                    networkNameShort = "S";
                }
                else
                {
                    // Due to SSL Issues over an IP Address - We Use SSL for Domain
                    networkURL = "https://api.globalcms.co.uk";
                    networkIP = "api.globalcms.co.uk";
                    networkName = "Public";
                    networkNameShort = "P";
                }

                if (MyNetwork == "SEC")
                {
                    var pingTest = GCMSSystem.Ping(networkURL);
                    if (!pingTest)
                    {
                        networkURL = "https://api.globalcms.co.uk";
                        networkIP = "api.globalcms.co.uk";
                        networkName = "Public";
                        networkNameShort = "P";
                    }
                    else
                    {
                        networkURL = "http://172.16.0.2";
                        networkIP = "172.16.0.2";
                        networkName = "Secure";
                        networkNameShort = "S";
                    }
                }
                if (NetworkOverride != "Auto" && NetworkOverride == "Public") { networkURL = "https://api.globalcms.co.uk"; networkIP = "api.globalcms.co.uk"; networkName = "Public"; networkNameShort = "P"; }
                if (NetworkOverride != "Auto" && NetworkOverride == "Secure") { networkURL = "http://172.16.0.2"; networkIP = "172.16.0.2"; networkName = "Secure"; networkNameShort = "S"; }

                var signFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage");
                // Backup Checker to make sure that everything that should be in the Folder is in the Signage Folder
                if (!Directory.Exists(signFolder))
                {
                    GCMSSystem.FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [ERR] Detected Issue with Signage Folder - Redownload Signage Version");
                    try
                    {
                        GCMSSystem.DownloadFileSingle(networkURL + "/v2/signageUpdate/signage.zip", "signage.zip");
                    }
                    catch { }

                    // Unzip Signage.zip to the Signage Folder inside monitor main root DIR
                    var signageZipFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage.zip";
                    var signageZipFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    try
                    {
                        System.IO.Compression.ZipFile.ExtractToDirectory(signageZipFile, signageZipFolder);
                    }
                    catch { }

                    // We need to wait to allow the system to finish extracting the file
                    System.Threading.Thread.Sleep(10000);                                                                    // Wait for 10 seconds

                    // For some reaon the last 2 files dont seem to extract form the zip so lets place a backup plan in place for that
                    var websocketJS = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\websocket.js";
                    if (!File.Exists(websocketJS))
                    {
                        GCMSSystem.DownloadFileSingle(networkURL + "/v2/signageUpdate/websocket.js", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\websocket.js");
                    }
                    var WindowsMessageRelayEXE = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\WindowsMessageRelay.exe";
                    if (!File.Exists(WindowsMessageRelayEXE))
                    {
                        GCMSSystem.DownloadFileSingle(networkURL + "/v2/signageUpdate/WindowsMessageRelay.exe", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\WindowsMessageRelay.exe");
                    }

                    // Move all content from temp folder back to the signage\public\content
                    try
                    {
                        File.Delete(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage.zip");
                    }
                    catch { }

                    var browserSSL = MyIni.Read("SSL", "Browser");
                    var sslon = "false";
                    if (browserSSL == "On")
                    {
                        sslon = "true";
                    }
                    string signageConfFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "settings.conf");
                    var SettingsIni = new IniFile(signageConfFile);
                    SettingsIni.Write("sslOn", sslon, "core");

                    // Once this has fixed itself, we need to clear the signage files so that it triggers the content to re-download to the machine
                    EngineerTools.ClearSignageFiles(false);
                    GCMSSystem.Chrome.UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear

                    // Now we just have to wait for the Checker to recheck and see that the signage isnt running and then to start it all back up
                    GCMSSystem.FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [AUTOFIX] Repaired Signage");
                }

                var powerMode = "FALSE";
                var maintMode = "FALSE";
                if ((meLowPowerMode1 == "TRUE" || meLowPowerMode2 == "TRUE") && meMaintMode == "FALSE")
                {
                    powerModeLabel.Text = "Low Power";
                    powerModeLabel.ForeColor = Color.FromArgb(0, 192, 0);
                    powerMode = "TRUE";
                    maintMode = "FALSE";
                }
                if (meMaintMode == "TRUE" && meLowPowerMode1 == "FALSE" && meLowPowerMode2 == "FALSE")
                {
                    powerModeLabel.Text = "Maintenance";
                    powerModeLabel.ForeColor = Color.FromArgb(255, 128, 0);
                    powerMode = "FALSE";
                    maintMode = "TRUE";
                }
                if (meLowPowerMode1 == "FALSE" && meLowPowerMode2 == "FALSE" && meMaintMode == "FALSE")
                {
                    powerModeLabel.Text = "Normal / Online";
                    powerModeLabel.ForeColor = Color.FromArgb(0, 0, 0);
                    powerMode = "FALSE";
                    maintMode = "FALSE";
                }
                if ((meLowPowerMode1 == "TRUE" || meLowPowerMode2 == "TRUE") && meMaintMode == "TRUE")
                {
                    powerModeLabel.Text = "Low Power";
                    powerModeLabel.ForeColor = Color.FromArgb(0, 0, 192);
                    powerMode = "TRUE";
                    maintMode = "TRUE";
                }

                if (meLowPowerMode1 == "TRUE" || meLowPowerMode2 == "TRUE" || meMaintMode == "TRUE")
                {
                    // Shutdown Chrome Kiosk Mode so that you can access the Machine
                    if (GCMSSystem.Chrome.whichVer == 1 && !isDebug)
                    {
                        GCMSSystem.Chrome.Unload();
                        GCMSSystem.Chrome.UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear
                    }
                    if (GCMSSystem.Chrome.whichVer == 4 && !isDebug)
                    {
                        GCMSSystem.Chrome.Unload();
                        GCMSSystem.Chrome.UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear
                    }

                    if (meLowPowerMode1 == "TRUE")
                    {
                        GCMSSystem.TriggerSystem("MONOFF", false, false);
                    }
                    if (meLowPowerMode2 == "TRUE")
                    {
                        GCMSSystem.TriggerSystem("SCREENOFF", false, false);
                    }
                }

                // After Running of Pritunl for the Connection we need to shut it down, but we 1st need to make sure we 
                // are connected to the VPN Network and we can get a PING
                var pingTest2 = GCMSSystem.Ping(networkURL);
                if (MyNetwork == "SEC" && pingTest2)
                {
                    foreach (var process in Process.GetProcessesByName("pritunl"))
                    {
                        try
                        {
                            process.StartInfo.Verb = "runas";
                            process.Kill();
                        } catch { }
                    }
                }

                ///////////////////////////
                try
                {
                    var checkSEN = "Disconnected";
                    SENOpt.Text = checkSEN;
                    if (checkSEN == "Disconnected")
                    {
                        SENOpt.ForeColor = Color.FromArgb(192, 0, 0);
                    }
                    else
                    {
                        SENOpt.ForeColor = Color.FromArgb(0, 192, 0);
                    }

                    var checkAVA = "Disconnected";
                    AVAOpt.Text = checkAVA;
                    if (checkAVA == "Disconnected")
                    {
                        AVAOpt.ForeColor = Color.FromArgb(192, 0, 0);
                    }
                    else
                    {
                        AVAOpt.ForeColor = Color.FromArgb(0, 192, 0);
                    }

                    var checkWSK = "Disconnected";
                    WSKOpt.Text = checkWSK;
                    if (checkWSK == "Disconnected")
                    {
                        WSKOpt.ForeColor = Color.FromArgb(192, 0, 0);
                    }
                    else
                    {
                        WSKOpt.ForeColor = Color.FromArgb(0, 192, 0);
                    }
                }
                catch { }

                try
                {
                    ManagementObjectCollection mbsList = null;
                    ManagementObjectSearcher mbs = new ManagementObjectSearcher("Select * From Win32_USBHub");
                    mbsList = mbs.Get();
                    foreach (ManagementObject mo in mbsList)
                    {
                        if (mo["Name"].ToString() == "USB Mass Storage Device")
                        {
                            USBDriveOpt.Text = "Connected";
                        }
                    }
                    if (USBDriveOpt.Text == "Connected")
                    {
                        USBDriveOpt.ForeColor = Color.FromArgb(0, 192, 0);
                    }
                    else
                    {
                        USBDriveOpt.ForeColor = Color.FromArgb(192, 0, 0);
                    }
                }
                catch { }

                try
                {
                    bool nexmosphereBox = SerialPort.GetPortNames().Any(x => x == "COM12");
                    if (nexmosphereBox)
                    {
                        NEXOpt.Text = "Connected";
                        NEXOpt.ForeColor = Color.FromArgb(0, 192, 0);
                        nexmoshpereSensors = true;
                    }
                    else
                    {
                        NEXOpt.Text = " Disconnected";
                        NEXOpt.ForeColor = Color.FromArgb(192, 0, 0);
                        nexmoshpereSensors = false;
                    }
                }
                catch {
                    NEXOpt.Text = " Disconnected";
                    NEXOpt.ForeColor = Color.FromArgb(192, 0, 0);
                    nexmoshpereSensors = false;
                }

                try
                {
                    ManagementObjectCollection mbsList = null;
                    ManagementObjectSearcher mbs = new ManagementObjectSearcher("Select * From Win32_USBHub");
                    mbsList = mbs.Get();
                    foreach (ManagementObject mo in mbsList)
                    {
                        if (mo["Name"].ToString() == "USB Mass Storage Device")
                        {
                            USBDriveOpt.Text = "Connected";
                        }
                        if (mo["Name"].ToString().Contains("TV"))
                        {
                            WiFiCardOpt.Text = "Connected";
                        }
                    }
                    if (USBDriveOpt.Text == "Connected")
                    {
                        USBDriveOpt.ForeColor = Color.FromArgb(0, 192, 0);
                    }
                    else
                    {
                        USBDriveOpt.ForeColor = Color.FromArgb(192, 0, 0);
                    }
                    if (WiFiCardOpt.Text == "Connected")
                    {
                        WiFiCardOpt.ForeColor = Color.FromArgb(0, 192, 0);
                    }
                    else
                    {
                        WiFiCardOpt.ForeColor = Color.FromArgb(192, 0, 0);
                    }
                }
                catch { }

                try
                {
                    bool isTouchDevice = Tablet.TabletDevices.Cast<TabletDevice>().Any(dev => dev.Type == TabletDeviceType.Touch);
                    if (isTouchDevice)
                    {
                        TouchscreenOpt.Text = "Connected";
                        TouchscreenOpt.ForeColor = Color.FromArgb(0, 192, 0);
                    }
                    else
                    {
                        TouchscreenOpt.Text = "Disconnected";
                        TouchscreenOpt.ForeColor = Color.FromArgb(192, 0, 0);
                    }
                }
                catch { }

                try
                {
                    bool portExists = SerialPort.GetPortNames().Any(x => x == "COM1");
                    if (portExists)
                    {
                        if (!GCMSSystem.CheckOpened("RS232"))
                        {
                            // Form Rs232 = new Rs232();
                            // Rs232.Show();                // Disabled Until Complete
                        }
                        RS232Opt.Text = "Available";
                        RS232Opt.ForeColor = Color.FromArgb(0, 192, 0);
                        SerialPort port = new SerialPort("COM1");
                        if (port.IsOpen)
                        {
                            RS232Opt.Text = "In Use";
                            RS232Opt.ForeColor = Color.FromArgb(192, 0, 0);
                        }
                    }
                    else
                    {
                        RS232Opt.Text = "Not Available";
                        RS232Opt.ForeColor = Color.FromArgb(192, 0, 0);
                    }
                }
                catch {
                    RS232Opt.Text = "Not Available";
                    RS232Opt.ForeColor = Color.FromArgb(192, 0, 0);
                }

                try
                {
                    bool portExists2 = SerialPort.GetPortNames().Any(x => x == "COM10");
                    if (portExists2)
                    {
                        SensorOpt.Text = "Inserted";
                        SensorOpt.ForeColor = Color.FromArgb(0, 192, 0);
                        SerialPort port2 = new SerialPort("COM10");
                        if (port2.IsOpen)
                        {
                            SensorOpt.Text = "In Use";
                            SensorOpt.ForeColor = Color.FromArgb(192, 0, 0);
                        }
                        omronSensor = true;
                    }
                    else
                    {
                        SensorOpt.Text = "Not Available";
                        SensorOpt.ForeColor = Color.FromArgb(192, 0, 0);
                        omronSensor = false;
                    }
                }
                catch {
                    SensorOpt.Text = "Not Available";
                    SensorOpt.ForeColor = Color.FromArgb(192, 0, 0);
                    omronSensor = false;
                }

                // Read INI and place in appropriate sections
                var device_name = MyIni.Read("deviceName", "Monitor");
                var device_uuid = MyIni.Read("deviceUUID", "Monitor");
                devName.Text = device_name;
                devUUID.Text = device_uuid;

                try
                {
                    GCMSSystem.SetSystemTimeZone.SetTimezone(macAddr, device_uuid, networkURL);
                }
                catch { }

                // Other Text Vars
                devMAC.Text = macAddr;
                systemNetworkOpt.Text = networkNameShort;

                // WMI Calls - WINDOWS ONLY
                var cpuWMI = new ManagementObjectSearcher("SELECT * FROM Win32_Processor").Get().Cast<ManagementObject>().First();
                var osWMI = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem").Get().Cast<ManagementObject>().First();
                var gpuWMI = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController").Get().Cast<ManagementObject>().First();
                var hddCWMI = new ManagementObjectSearcher("SELECT * FROM Win32_logicaldisk").Get().Cast<ManagementObject>().First();

                // Check if Drive D Exists
                bool DriveD = GCMSSystem.DriveExists("D:\\");
                // If D:\ Exists & is a Disk Drive NOT CD-ROM
                var device_hdd_d = "";
                var totalHDD = "0";
                var freeHDD = "0";

                var freeC = Convert.ToString(Convert.ToDouble(hddCWMI["FreeSpace"]));
                var totalC = Convert.ToString(Convert.ToDouble(hddCWMI["Size"]));

                var freeD = "0";
                var totalD = "0";

                if (DriveD)
                {
                    var hddDWMI = new ManagementObjectSearcher("SELECT * FROM Win32_logicaldisk WHERE driveType = \"3\" AND name = \"D:\" ").Get().Cast<ManagementObject>().Last();
                    device_hdd_d = (string)hddDWMI["DeviceID"] + " ";
                    device_hdd_d += Convert.ToString(Math.Round(Convert.ToDouble(hddDWMI["Size"]) / 1048576000, 0)) + " GB";

                    totalHDD = Convert.ToString(Convert.ToDouble(hddCWMI["Size"]) + Convert.ToDouble(hddDWMI["Size"]));
                    freeHDD = Convert.ToString(Convert.ToDouble(hddCWMI["FreeSpace"]) + Convert.ToDouble(hddDWMI["FreeSpace"]));

                    freeD = Convert.ToString(Convert.ToDouble(hddDWMI["FreeSpace"]));
                    totalD = Convert.ToString(Convert.ToDouble(hddDWMI["Size"]));
                }
                else
                {
                    totalHDD = Convert.ToString(Convert.ToDouble(hddCWMI["Size"]));
                    freeHDD = Convert.ToString(Convert.ToDouble(hddCWMI["FreeSpace"]));
                }

                // CPU, OS, RAM, GPU Details
                var device_cpu = (string)cpuWMI["Name"];
                var device_os = (string)osWMI["Caption"];
                string releaseId = "Unknown";
                try
                {
                    releaseId = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId", "").ToString();
                }
                catch { }
                var device_ram = GCMSSystem.GetRAMsize("Formatted");
                var device_gpu = (string)gpuWMI["Name"];

                // HDD and CPU Load
                var device_hdd_c = (string)hddCWMI["DeviceID"] + " ";
                device_hdd_c += Convert.ToString(Math.Round(Convert.ToDouble(hddCWMI["Size"]) / 1048576000, 0)) + " GB";
                device_hdd_c += "  ";

                var device_cpu_load_raw = Convert.ToString(cpuWMI["LoadPercentage"]) + "";
                var device_cpu_load = Convert.ToString(cpuWMI["LoadPercentage"]) + "% Load";

                // Remove any unwanted text here
                device_os = device_os.Replace("Microsoft ", "");
                if (releaseId != "Unknown")
                {
                    device_os += " (" + releaseId + ")";
                }

                device_cpu =
                   device_cpu
                   .Replace("(TM)", "")
                   .Replace("(tm)", "™")
                   .Replace("(R)", "®")
                   .Replace("(r)", "®")
                   .Replace("(C)", "©")
                   .Replace("(c)", "©")
                   .Replace("    ", " ")
                   .Replace("  ", " ");

                // System Hardware Labels
                CPUArch.Text = device_cpu;
                devOS.Text = device_os;
                ramAmount.Text = device_ram;
                gfxCard.Text = device_gpu;
                hddAmounts.Text = device_hdd_c + device_hdd_d;
                cpuLoadLabel.Text = device_cpu_load;
                ramLoadLabel.Text = GCMSSystem.GetRamLoad();

                // Service Checks
                InternetConnectionOpt.Text = GCMSSystem.CheckForInternetConnection();
                if (InternetConnectionOpt.Text == "Online")
                {
                    InternetConnectionOpt.ForeColor = Color.FromArgb(0, 192, 0);
                }
                SecureConnectionOpt.Text = GCMSSystem.CheckForVPNConnection(networkURL, networkIP);
                if (SecureConnectionOpt.Text == "Online")
                {
                    SecureConnectionOpt.ForeColor = Color.FromArgb(0, 192, 0);
                }
                SignageSystemOpt.Text = GCMSSystem.CheckForSignage();
                if (SignageSystemOpt.Text == "Online")
                {
                    SignageSystemOpt.ForeColor = Color.FromArgb(0, 192, 0);
                    screen_signage_running = "YES";
                }
                else
                {
                    screen_signage_running = "NO";
                }

                bool isSignageEnabled = GCMSSystem.Chrome.IsSignageEnabled();

                bool isHDMIConnected = false;
                try
                {
                    isHDMIConnected = Main.Check_HDMI_Output();           // Is the HDMI Cable plugged into the machine?
                    // Debug.WriteLine("Detecting HDMI Devices Connected : " + isHDMIConnected);
                }
                catch { }

                bool isVGAConnected = false;
                try
                {
                    isVGAConnected = GCMSSystem.Display.Check("0");            // Is there a Non HDMI Port Cable plugged into the machine?    
                    // Debug.WriteLine("Detecting Non HDMI Devices : " + isVGAConnected);
                }
                catch { }

                // Now we know the Devices Connected, if any report back as connected then we need to assign myDisplay to "ONLINE"
                var myDisplay = "OFFLINE";
                if (isHDMIConnected || isVGAConnected)
                {
                    myDisplay = "ONLINE";
                    SCROpt.Text = "Connected";
                    SCROpt.ForeColor = Color.FromArgb(0, 192, 0);
                }
                else
                {
                    SCROpt.Text = "Disconnected";
                    SCROpt.ForeColor = Color.FromArgb(192, 0, 0);
                    if (!isHDMIConnected && !isVGAConnected)
                    {
                        SCROpt.Text = "Not Supported";
                    }
                }

                // Read Current Signage Version & SubVersion
                using (StreamReader streamReader = new StreamReader(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "version.txt"), Encoding.UTF8))
                {
                    signageVersion = streamReader.ReadToEnd();
                }
                using (StreamReader streamReader = new StreamReader(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "subversion.txt"), Encoding.UTF8))
                {
                    signageSubVersion = streamReader.ReadToEnd();
                }

                var screen_res = Screen.PrimaryScreen.Bounds.Width.ToString() + "x" + Screen.PrimaryScreen.Bounds.Height.ToString();
                if (forceGFX_Res == "")
                {
                    MyIni.Write("Resolution", screen_res, "Display");
                }
                if (forceGFX_Scaling == "")
                {
                    MyIni.Write("Scaling", "100", "Display");
                }
                if (forceGFX_Orientation == "")
                {
                    MyIni.Write("Orientation", "Landscape", "Display");
                }

                // If Force Resolution Set to On then to force the system to set the Resolution to given options
                if (forceResolution)
                {
                    var resStr = MyIni.Read("Resolution", "Display");
                    var resScaling = MyIni.Read("Scaling", "Display");
                    string[] resTokens = resStr.Split('x');

                    var setW = Convert.ToInt32(resTokens[0]);
                    var setH = Convert.ToInt32(resTokens[1]);

                    // Debug.WriteLine("W : " + setW.ToString());
                    // Debug.WriteLine("H : " + setH.ToString());

                    try
                    {
                        ScreenScaling.SetScaleFactor(resScaling);
                    }
                    catch { }

                    if (forceGFX_Orientation != "Multiscreen") { 
                        try
                        {
                            ScreenResolution.SetResoltion(setW, setH);        // Set the Screen Resolution to whats contained in the config.ini
                        }
                        catch { }
                    }

                    if (forceGFX_Orientation != "Multiscreen")
                    {
                        if (forceGFX_Orientation == "Landscape")
                        {
                            try
                            {
                                ScreenRotation.SetOrientation(1, 0);
                            }
                            catch { }
                        }
                        if (forceGFX_Orientation == "Landscape-Flip")
                        {
                            try
                            {
                                ScreenRotation.SetOrientation(1, 90);
                            }
                            catch { }
                        }
                        if (forceGFX_Orientation == "Portrait")
                        {
                            try
                            {
                                ScreenRotation.SetOrientation(1, 45);
                            }
                            catch { }
                        }
                        if (forceGFX_Orientation == "Portrait-Flip")
                        {
                            try
                            {
                                ScreenRotation.SetOrientation(1, 135);
                            }
                            catch { }
                        }
                    }
                    if (forceGFX_Orientation == "Multiscreen")
                    {
                        // If Multiscreen we need to fire the Screen Rotation for each Monitor or Windows 
                        // goes very strange just changing only 1 screen when multiple screens attached
                        if (forceGFX_Monitor1 != "Disabled")
                        {
                            if (forceGFX_Monitor1 == "Landscape")
                            {
                                try { ScreenRotation.SetOrientation(1, 0); } catch { }
                            }
                            if (forceGFX_Monitor1 == "Landscape-Flip")
                            {
                                try { ScreenRotation.SetOrientation(1, 90); } catch { }
                            }
                            if (forceGFX_Monitor1 == "Portrait")
                            {
                                try { ScreenRotation.SetOrientation(1, 45); } catch { }
                            }
                            if (forceGFX_Monitor1 == "Portrait-Flip")
                            {
                                try { ScreenRotation.SetOrientation(1, 135); } catch { }
                            }
                        }
                        if (forceGFX_Monitor2 != "Disabled")
                        {
                            if (forceGFX_Monitor2 == "Landscape")
                            {
                                try { ScreenRotation.SetOrientation(2, 0); } catch { }
                            }
                            if (forceGFX_Monitor2 == "Landscape-Flip")
                            {
                                try { ScreenRotation.SetOrientation(2, 90); } catch { }
                            }
                            if (forceGFX_Monitor2 == "Portrait")
                            {
                                try { ScreenRotation.SetOrientation(2, 45); } catch { }
                            }
                            if (forceGFX_Monitor2 == "Portrait-Flip")
                            {
                                try { ScreenRotation.SetOrientation(2, 135); } catch { }
                            }
                        }
                        if (forceGFX_Monitor3 != "Disabled")
                        {
                            if (forceGFX_Monitor3 == "Landscape")
                            {
                                try { ScreenRotation.SetOrientation(3, 0); } catch { }
                            }
                            if (forceGFX_Monitor3 == "Landscape-Flip")
                            {
                                try { ScreenRotation.SetOrientation(3, 90); } catch { }
                            }
                            if (forceGFX_Monitor3 == "Portrait")
                            {
                                try { ScreenRotation.SetOrientation(3, 45); } catch { }
                            }
                            if (forceGFX_Monitor3 == "Portrait-Flip")
                            {
                                try { ScreenRotation.SetOrientation(3, 135); } catch { }
                            }
                        }
                        if (forceGFX_Monitor4 != "Disabled")
                        {
                            if (forceGFX_Monitor4 == "Landscape")
                            {
                                try { ScreenRotation.SetOrientation(4, 0); } catch { }
                            }
                            if (forceGFX_Monitor4 == "Landscape-Flip")
                            {
                                try { ScreenRotation.SetOrientation(4, 90); } catch { }
                            }
                            if (forceGFX_Monitor4 == "Portrait")
                            {
                                try { ScreenRotation.SetOrientation(4, 45); } catch { }
                            }
                            if (forceGFX_Monitor4 == "Portrait-Flip")
                            {
                                try { ScreenRotation.SetOrientation(4, 135); } catch { }
                            }
                        }
                        if (forceGFX_Monitor5 != "Disabled")
                        {
                            if (forceGFX_Monitor5 == "Landscape")
                            {
                                try { ScreenRotation.SetOrientation(5, 0); } catch { }
                            }
                            if (forceGFX_Monitor5 == "Landscape-Flip")
                            {
                                try { ScreenRotation.SetOrientation(5, 90); } catch { }
                            }
                            if (forceGFX_Monitor5 == "Portrait")
                            {
                                try { ScreenRotation.SetOrientation(5, 45); } catch { }
                            }
                            if (forceGFX_Monitor5 == "Portrait-Flip")
                            {
                                try { ScreenRotation.SetOrientation(5, 135); } catch { }
                            }
                        }
                        if (forceGFX_Monitor6 != "Disabled")
                        {
                            if (forceGFX_Monitor6 == "Landscape")
                            {
                                try { ScreenRotation.SetOrientation(6, 0); } catch { }
                            }
                            if (forceGFX_Monitor6 == "Landscape-Flip")
                            {
                                try { ScreenRotation.SetOrientation(6, 90); } catch { }
                            }
                            if (forceGFX_Monitor6 == "Portrait")
                            {
                                try { ScreenRotation.SetOrientation(6, 45); } catch { }
                            }
                            if (forceGFX_Monitor6 == "Portrait-Flip")
                            {
                                try { ScreenRotation.SetOrientation(6, 135); } catch { }
                            }
                        }
                    }
                }
                if (cloneScr == "On")
                {
                    System.Threading.Thread.Sleep(5000);
                    var process = Process.Start(new ProcessStartInfo
                    {
                        FileName = Path.Combine(Environment.SystemDirectory, "DisplaySwitch.exe"),
                        Arguments = "/clone",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    });
                    process.Start();
                }

                var totalRAM = GCMSSystem.GetRAMsize("Not-Formatted");
                var freeRAM = GCMSSystem.GetRamFree();

                double myTemperatureInCelsius;
                var cpuTempC = 0;
                try
                {
                    var curTemp = Convert.ToDouble(GCMSSystem.GetCPUTemp());
                    curTemp = curTemp / 10;
                    myTemperatureInCelsius = Temperature.FromKelvin(curTemp).Celsius;
                    cpuTempC = (int)myTemperatureInCelsius;
                }
                catch { }

                var contentCount = "0";
                try
                {
                    // contentCount = GCMSSystem.HowManyContent();
                }
                catch { }

                var currentCanvas = "Unknown";
                List<string> launchArr1 = new List<string>();
                List<string> launchArr2 = new List<string>();
                using (var fileStream = File.OpenRead(appIniFile))
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        var appFile = line.Split('\\').Last().Split('.').First();

                        var appRunning = "NO";
                        var appLauncherProcess = Process.GetProcesses().Any(p => p.ProcessName.Contains(appFile));
                        if (appLauncherProcess)
                        {
                            appRunning = "YES";
                        }
                        launchArr1.Add(line);
                        launchArr2.Add(appRunning);
                    }
                }

                bool isTeamViewerInstalled = false;
                try
                {
                    isTeamViewerInstalled = GCMSSystem.TeamViewer.IsInstaled();
                }
                catch { }

                string teamviewerInstalled = "No";
                bool teamviewerRunningEXE = false;
                string teamviewerRunning = "No";
                string teamviewerVersion = "";
                string teamviewerID = "";
                if (isTeamViewerInstalled)
                {
                    try
                    {
                        teamviewerRunningEXE = Process.GetProcesses().Any(p => p.ProcessName.Contains("TeamViewer"));
                        if (teamviewerRunningEXE)
                        {
                            teamviewerRunning = "Yes";
                        }
                    }
                    catch { }
                    teamviewerInstalled = "Yes";
                    try
                    {
                        teamviewerVersion = GCMSSystem.TeamViewer.Version();
                    }
                    catch { }
                    try
                    {
                        teamviewerID = GCMSSystem.TeamViewer.ID();
                    }
                    catch { }
                }

                using (Ping p = new Ping())
                {
                    try
                    {
                        pingMS = p.Send("api.globalcms.co.uk").RoundtripTime.ToString();
                    } catch { }
                }

                int pendingWindowsUpdates = 0;
                try
                {
                    pendingWindowsUpdates = GCMSSystem.WindowsEnv.CheckUpdates();
                }
                catch { }

                try
                {
                    if (!isDebug)
                    {
                        AudioManager.SetMasterVolume(curAudioLevel);                // This uses the Variable from the config.ini to set the Master Volume
                    }
                }
                catch { }

                var PowerIni = new IniFile(powerParamsFile);
                var powerStatus = PowerIni.Read("Status", "System");
                if (!GCMSSystem.CheckOpened("PowerTimer") && powerStatus != "")
                {
                    // new System.Threading.Thread(() => new PowerTimerWindow().ShowDialog()).Start();
                    var powerTimerWindow = new PowerTimerWindow();
                    powerTimerWindow.Show();
                }

                if (!GCMSSystem.CheckOpened("Updates Queue"))
                {
                    new System.Threading.Thread(() => new DownloaderManager().ShowDialog()).Start();
                    // Form DownloaderManager = new DownloaderManager();
                    // DownloaderManager.Show();
                }

                // Call Home on init, however the rest will be deligated over to a Timer
                using (var client = new WebClient())
                {
                    // Create the $_POST Data for the HTTP Request
                    var values = new NameValueCollection
                    {
                        ["clientID"] = MyIni.Read("clientID", "Monitor"),
                        ["deviceID"] = MyIni.Read("deviceName", "Monitor"),
                        ["deviceMAC"] = MyIni.Read("deviceMAC", "Monitor"),
                        ["deviceUUID"] = MyIni.Read("deviceUUID", "Monitor"),
                        ["hardwareMAC"] = MyIni.Read("hardwareMAC", "Monitor"),
                        ["wanIP"] = wanIP,
                        ["vpnIP"] = vpnIP,
                        ["lanIP"] = localIP,
                        ["signageRunning"] = screen_signage_running,
                        ["avaRunning"] = GCMSSystem.CheckForAVA(),
                        ["theRes"] = screen_res,
                        ["gfxStr"] = "",
                        ["monVer"] = version,
                        ["monSubVer"] = subversion,
                        ["backupFile"] = "ERROR",
                        ["theOS"] = device_os,
                        ["theOSBuild"] = releaseId,
                        ["cpuLoad"] = device_cpu_load_raw,
                        ["cpuTemp"] = cpuTempC.ToString(),
                        ["gpuLoad"] = "0",
                        ["gpuTemp"] = "0",
                        ["hddTotal"] = totalHDD,
                        ["hddFree"] = freeHDD,
                        ["totalC"] = totalC,
                        ["totalD"] = totalD,
                        ["freeC"] = freeC,
                        ["freeD"] = freeD,
                        ["hddTemp"] = "0",
                        ["totalRam"] = totalRAM,
                        ["freeRam"] = freeRAM,
                        ["curTime"] = DateTime.Now.ToString("HH:mm"),
                        ["signVersion"] = signageVersion,
                        ["signSubVersion"] = signageSubVersion,
                        ["chromeVersion"] = GCMSSystem.CheckChromeVersion(),
                        ["myDisplay"] = myDisplay,
                        ["maintMode"] = maintMode,
                        ["lowpowerMode"] = powerMode,
                        ["senBoard"] = SENOpt.Text,
                        ["signContentCount"] = contentCount,
                        ["signLoader"] = signageLoaderEnabled,
                        ["signLoaderVer"] = signageLoaderVer.ToString(),
                        ["EOVersion"] = EOversionInfo.ToString(),
                        ["currentCanvas"] = currentCanvas,
                        ["launchArr1"] = string.Join(",", launchArr1.ToArray()),
                        ["launchArr2"] = string.Join(",", launchArr2.ToArray()),
                        ["teamviewerRunning"] = teamviewerRunning,
                        ["teamviewerInstalled"] = teamviewerInstalled,
                        ["teamviewerVersion"] = teamviewerVersion,
                        ["teamviewerID"] = teamviewerID,
                        ["omronStatus"] = SensorOpt.Text,
                        ["omronTemp"] = OmronSensor.OmronData.Temp,
                        ["omronHumidty"] = OmronSensor.OmronData.Humidity,
                        ["omronLight"] = OmronSensor.OmronData.Light,
                        ["omronPressure"] = OmronSensor.OmronData.Pressure,
                        ["omronNoise"] = OmronSensor.OmronData.Noise,
                        ["omronETVOC"] = OmronSensor.OmronData.eTVOC,
                        ["omronECO2"] = OmronSensor.OmronData.eCO2,
                        ["omronVibrateSI"] = OmronSensor.OmronData.Vibrate_SI,
                        ["omronVibratePGA"] = OmronSensor.OmronData.Vibrate_PGA,
                        ["omronVibrateSeismicIntensity"] = OmronSensor.OmronData.Vibrate_SeismicIntensity,
                        ["omronVibrateAccelerationX"] = OmronSensor.OmronData.Vibrate_AccelerationX,
                        ["omronVibrateAccelerationY"] = OmronSensor.OmronData.Vibrate_AccelerationY,
                        ["omronVibrateAccelerationZ"] = OmronSensor.OmronData.Vibrate_AccelerationZ,
                        ["omronHeatStrokeRisk"] = OmronSensor.OmronData.HeatStrokeRisk,
                        ["omronDiscomfortIndex"] = OmronSensor.OmronData.DiscomfortIndex,
                        ["omronNlLabelTxt"] = OmronSensor.OmronData.NlLabelTxt,
                        ["omronDILabelTxt"] = OmronSensor.OmronData.DILabelTxt,
                        ["omronHSILabelTxt"] = OmronSensor.OmronData.HSILabelTxt,
                        ["omronLlLabelTxt"] = OmronSensor.OmronData.LlLabelTxt,
                        ["omronETVOCLabelTxt"] = OmronSensor.OmronData.eTVOCLabelTxt,
                        ["omronECO2LabelTxt"] = OmronSensor.OmronData.eCO2LabelTxt,
                        ["airserverCode"] = GCMSSystem.AirServer.Passcode(),
                        ["airserverVersion"] = GCMSSystem.AirServer.Version().ToString(),
                        ["airserverInstalled"] = GCMSSystem.AirServer.IsInstaled().ToString(),
                        ["airserverRunning"] = GCMSSystem.AirServer.IsRunning().ToString(),
                        ["pendingWindowsUpdates"] = pendingWindowsUpdates.ToString(),
                        ["isTrial"] = isTrial.ToString(),
                        ["trialStart"] = trialStart.ToString(),
                        ["trialEnd"] = trialEnd.ToString(),
                        ["signageURL"] = MyIni.Read("Load", "Browser"),
                        ["signageXSS"] = MyIni.Read("Referer", "Browser"),
                        ["signageDebug"] = MyIni.Read("Debug", "Browser"),
                        ["signageSSL"] = MyIni.Read("SSL", "Browser"),
                        ["signageKeyboard"] = MyIni.Read("Keyboard", "Browser"),
                        ["poorInternet"] = MyIni.Read("poorInternet", "Monitor"),
                        ["currentNIC"] = EngineerTools.DetectNetworkAdapter2(),
                        ["pingMS"] = pingMS
                    };
                    var responseString = "";
                    try
                    {
                        var response = client.UploadValues(networkURL + "/v2/inbound.php", values);
                        responseString = Encoding.Default.GetString(response);
                        isOnline = true;
                        powerModeLabel.Text = "Normal / Online";
                        powerModeLabel.ForeColor = Color.FromArgb(0, 0, 0);
                    }
                    catch
                    {
                        responseString = "Error";
                        isOnline = false;
                        powerModeLabel.Text = "Normal / Offline";
                        powerModeLabel.ForeColor = Color.FromArgb(0, 0, 0);
                        GCMSSystem.FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [ERR] Cannot Call API Server [" + networkNameShort + "] - Check Internet/Firewall");
                    }
                    LastHeartbeatLabel.Text = Convert.ToString(responseString);
                }

                DataLoggingTimer.Start();
                if (isSignageEnabled)
                {
                    if (meLowPowerMode1 == "FALSE" || meLowPowerMode2 == "FALSE")
                    {

                        CheckSNAP.Start();
                    }
                }
                LowTimer.Start();

                // 3rd Part Application Launcher
                LauncherTimer.Enabled = true;
                LauncherTimer.Start();

                if (File.Exists(trialFile))
                {
                    TrialLicTxt.Visible = true;
                    if (trialRemaining == "1")
                    {
                        TrialLicTxt.Text = "TRIAL LICENCE (Expires in " + trialRemaining + " day)";
                    }
                    else
                    {
                        TrialLicTxt.Text = "TRIAL LICENCE (Expires in " + trialRemaining + " days)";
                    }
                } else
                {
                    TrialLicTxt.Visible = false;
                }

                var lockdownFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "globalcms.lock");
                if (File.Exists(lockdownFile))
                {
                    isInLockdown = true;
                    powerModeLabel.Text = "Lockdown";
                    powerModeLabel.ForeColor = Color.FromArgb(255, 0, 0);
                }

                // Check for decom.lock
                if (File.Exists(lockFile) || trailLock || isInLockdown)
                {
                    if (!isInLockdown)
                    {
                        // If decom.lock or trial.lock exists then to load the automated decommission system
                        // Stop all Timers
                        CheckStatsTimer.Enabled = false;
                        CheckStatsTimer.Stop();
                        CallHomeTimer.Enabled = false;
                        CallHomeTimer.Stop();
                        CheckServicesTimer.Enabled = false;
                        CheckServicesTimer.Stop();
                        CheckSNAP.Enabled = false;
                        CheckSNAP.Stop();
                        LauncherTimer.Enabled = false;
                        LauncherTimer.Stop();

                        // Make Main Window Disabled and Minimized
                        Visible = false;
                        Enabled = false;
                        WindowState = FormWindowState.Minimized;
                        // Open the Decommission Form
                        var decomForm = new Decommission();
                        decomForm.Show();
                    }
                    else
                    {
                        CheckSNAP.Enabled = false;
                        CheckSNAP.Stop();
                        LauncherTimer.Enabled = false;
                        LauncherTimer.Stop();

                        var lockdwnForm = new DeviceLocked();
                        lockdwnForm.Show();
                    }

                    GCMSSystem.Chrome.Unload();
                    GCMSSystem.Chrome.UpdatePref();
                    return;
                }

                if (meMaintMode == "TRUE")
                {
                    CheckVpnService();       // Check VPN Service (This makes sure VPN is running)
                    if (!GCMSSystem.CheckOpened("EngineeringTools")) {
                        var toolForm = new EngineerTools();
                        toolForm.Show();
                    }
                    if (!isSignageEnabled)
                    {
                        CheckSNAP.Stop();
                    }

                    GCMSSystem.Chrome.Unload();
                    GCMSSystem.Chrome.UpdatePref();
                    if (!isDebug)
                    {
                        if (hardenedShell)
                        {
                            try
                            {
                                Taskbar.Show();
                            }
                            catch { }
                        }
                    }
                }

                if (omronSensor)
                {
                    var omronService = MyIni.Read("Env", "Serv");

                    if (!GCMSSystem.CheckOpened("OmronSensor"))
                    {
                        Form OmronDebug = new OmronSensor();
                        OmronDebug.Show();
                        if (omronService == "Disabled")
                        {
                            OmronDebug.Visible = false;
                        }
                    }
                }

                if (nexmoshpereSensors)
                {
                    if (!GCMSSystem.CheckOpened("Nexmosphere") && nexmoshpereSensors)
                    {
                        Form NexusDebug = new Nexmosphere();
                        NexusDebug.Show();
                    }
                }

                if (airServerMirroring)
                {
                    try
                    {
                        if (!GCMSSystem.AirServer.IsRunning())
                        {
                            GCMSSystem.AirServer.Start();               // Start AirServer
                        }
                    }
                    catch { }
                }

                try
                {
                    Webserver1 = new WebServer(300);        // Start WebServer that gives us a way to remotely check system log on port 300
                }
                catch { }
                try
                {
                    Socketserver1 = new SocketServer();     // Start SocketServer that gives us a way to send internal websockets to the Monitor program on port 2525
                }
                catch { }
                if (!isDebug)
                {
                    if (hardenedShell)
                    {
                        try
                        {
                            Taskbar.Hide();
                        }
                        catch { }
                    }
                }
            }
            else
            {
                // If there is no config.ini then to load the automated setup service
                // Stop all Timers
                CheckStatsTimer.Enabled = false;
                CheckStatsTimer.Stop();
                CallHomeTimer.Enabled = false;
                CallHomeTimer.Stop();
                CheckServicesTimer.Enabled = false;
                CheckServicesTimer.Stop();
                CheckSNAP.Enabled = false;
                CheckSNAP.Stop();
                // Make Main Window Disabled and Minimized
                Visible = false;
                Enabled = false;
                WindowState = FormWindowState.Minimized;
                // Open the Commissioning Form
                openedForm = new Commission();
                openedForm.Show();
            }

            // Write the Process ID into the PID file
            try
            {
                File.WriteAllText(pidFile, nProcessID.ToString());
            }
            catch { }
        }

        // Functions for Buttons
        private void MaintModeBTN_Click(object sender, EventArgs e)
        {
            // Make sure Interactive is turned off
            isInteractive = false;

            LauncherTimer.Enabled = false;
            LauncherTimer.Interval = 60000;

            string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
            var MyIni = new IniFile(iniFile);
            MyIni.Write("powersaveMode", "FALSE", "Network");
            MyIni.Write("powersaveMode2", "FALSE", "Network");
            MyIni.Write("maintMode", "TRUE", "Network");

            powerModeLabel.Text = "Maintenance";
            powerModeLabel.ForeColor = Color.FromArgb(255, 128, 0);

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

                    var sh = (IShellDispatch4)Activator.CreateInstance( Type.GetTypeFromProgID("Shell.Application") );
                    sh.ShellExecute(nodeEXE, Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "signage.js") + " " + Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "settings.conf"), null, null, 0);
                }

                if (GCMSSystem.Chrome.whichVer == 1 && !isDebug)
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
                if (GCMSSystem.Chrome.whichVer == 4 && !isDebug)
                {
                    GCMSSystem.Chrome.Unload();
                    GCMSSystem.Chrome.UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear
                }
            }

            if (!GCMSSystem.CheckOpened("EngineeringTools"))
            {
                var toolForm = new EngineerTools();
                toolForm.Show();
            }
            if (isSignageEnabled)
            {
                CheckSNAP.Stop();
            }
            WindowState = FormWindowState.Minimized;
        }

        private void SignageDebugBTN_Click(object sender, EventArgs e)
        {
            var SystemDebug = new SystemDebug();
            SystemDebug.Show();
        }

        private string CheckVpnService()
        {
            var pingTest = GCMSSystem.Ping("172.16.0.2");
            if (!pingTest)
            {
                string user = GCMSSystem.Chrome.GetCurrentMachineUser();
                string path = Path.Combine("C:\\", "Users", user, "AppData", "Roaming", "pritunl", "profiles");
                string mac = GCMSSystem.GetMACAddress().Replace(":", "-");
                string ovpnFile = path + "\\" + mac + ".ovpn";
                // VPN isnt connected so we shall try and reconnect it internally
                GCMSSystem.VPNClient.Load(ovpnFile);
            }
            return "Started Process";
        }
        // Checker Service - This checks for if everything that needs to be running, and if it is running.
        // If it isnt running then to reload the process. Also makes sure Chrome Kiosk Mode Sits on Top
        private string CheckerService(string meLowPowerMode1, string meLowPowerMode2, string meMaintMode)
        {
            string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
            var MyIni = new IniFile(iniFile);
            var EngineerMode = MyIni.Read("Network", "maintMode");
            var LowPowerMode1 = MyIni.Read("Network", "powersaveMode");
            var LowPowerMode2 = MyIni.Read("Network", "powersaveMode2");

            var startup = true;
            var signFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage");

            if (meLowPowerMode1 == "FALSE" && meLowPowerMode2 == "FALSE")
            {
                // Variables for each CheckSum - Default to NO
                var chromeRunning = "NO";
                var nodeRunning = "NO";

                var chromeEXE = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\Google\\Chrome\\Application\\chrome.exe";
                var nodeEXE = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "node32.exe");

                if (osArch == "x64")
                {
                    //nodeEXE = Directory.GetCurrentDirectory() + "\\signage\\node64.exe";
                    nodeEXE = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "node64.exe");
                }

                var chromeProcess = Process.GetProcesses().Any(p => p.ProcessName.Contains("chrome"));
                if (chromeProcess)
                {
                    chromeRunning = "YES";
                }
                var nodeProcess = Process.GetProcesses().Any(p => p.ProcessName.Contains("node"));
                if (nodeProcess)
                {
                    nodeRunning = "YES";
                }

                if (signageLoader == 2)
                {
                    var BrowserFrm = Application.OpenForms["SignageBrowser"];
                    if (BrowserFrm == null)
                    {
                        chromeRunning = "NO";
                    }
                }

                if (chromeRunning == "NO")
                {
                    GCMSSystem.Chrome.UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear

                    // Backup Checker to make sure that everything that should be in the Folder is in the Signage Folder
                    if (Directory.Exists(signFolder))
                    {
                        if (meMaintMode == "FALSE")
                        {
                            GCMSSystem.Chrome.Load();
                        }
                    }
                    if (!startup) { GCMSSystem.RemoteLog.Send(DateTime.Now.ToString("dd MMM HH:mm:ss"), "Signage2"); };
                }
                if (nodeRunning == "NO")
                {
                    if (Directory.Exists(signFolder))
                    {
                        // NodeJS isnt running - maybe it has crashed, been closed etc
                        Process nodeJS = new Process();
                        nodeJS.StartInfo.FileName = nodeEXE;
                        nodeJS.StartInfo.Arguments = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "signage.js") + " " + Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "settings.conf");
                        nodeJS.StartInfo.Verb = "runas";
                        nodeJS.StartInfo.UseShellExecute = false;
                        nodeJS.StartInfo.EnvironmentVariables["LOCALAPPDATA"] = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                        nodeJS.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        nodeJS.StartInfo.WorkingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString());
                        var sh = (IShellDispatch4)Activator.CreateInstance( Type.GetTypeFromProgID("Shell.Application") );
                        sh.ShellExecute(nodeEXE, Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "signage.js") + " " + Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "settings.conf"), null, null, 0);
                        if (!startup) { GCMSSystem.RemoteLog.Send(DateTime.Now.ToString("dd MMM HH:mm:ss"), "Signage"); }
                    }
                }

                // Bring Google Chrome Kiosk To ON TOP
                bool isSignageEnabled = GCMSSystem.Chrome.IsSignageEnabled();
                if (isSignageEnabled && Directory.Exists(signFolder))
                {
                    if (GCMSSystem.Chrome.whichVer == 1 && !isDebug)
                    {
                        var chromeProcessGUID = "- Google Chrome";
                        WindowHelper.BringToFront(chromeProcessGUID);
                    }
                    if (GCMSSystem.Chrome.whichVer == 4)
                    {
                        var chromeProcessGUID = "- Google Chrome";
                        WindowHelper.BringToFront(chromeProcessGUID);
                    }
                }
                // Make sure that the Monitor itself is set to Minimized
                WindowState = FormWindowState.Minimized;
            }
            startup = false;
            Debug.WriteLine("Current Signage Loaded Status [Timer]: " + MainForm.isSignageLoaded.ToString());
            return "Complete";
        }

        // When the Form Loads - If we need to load anything delayed - Load it here
        private void MainForm_Load(object sender, EventArgs e)
        {
            FrmObj = this;
            FormClosing += new FormClosingEventHandler(MainForm_FormClosed);
            FormClosed += new FormClosedEventHandler(MainForm_FormClosed);

            // This is to register the events / timer for Interactive Mode on the Signage
            Interaction.Enabled = false;

            // This is to register the user inputs
            activityTimer.Tick += ActivityWorker_Tick;
            activityTimer.Interval = 200;
            activityTimer.Enabled = true;

            // This registers the App incase of Crashing out, will restart the application
            GCMSSystem.RegisterApplicationRecoveryAndRestart();

            if (isDebug) { TestBrowserBTN.Visible = true; } else { TestBrowserBTN.Visible = false; }
            CurRunningVerTxt.Text = About.GetVersion("Main").ToString() + " / " + About.GetVersion("SubVersion").ToString();
        }

        // Closing Events
        private void MainForm_FormClosed(object sender, EventArgs e)
        {
            TreeViewer.helper.killChildProcesses(nProcessID);
            foreach (var process in Process.GetProcessesByName("node32"))
            {
                try
                {
                    GCMSSystem.KillProcessAndChildrens(process.Id); // Get PID of node32, and kill 'it' as well as any other children it was spawned, on top of the killchildren line above
                    process.StartInfo.Verb = "runas";
                    process.Kill();
                }
                catch { }
            }
            foreach (var process in Process.GetProcessesByName("node64"))
            {
                try
                {
                    GCMSSystem.KillProcessAndChildrens(process.Id); // Get PID of node32, and kill 'it' as well as any other children it was spawned, on top of the killchildren line above
                    process.StartInfo.Verb = "runas";
                    process.Kill();
                }
                catch { }
            }
            foreach (var process in Process.GetProcessesByName("pritunl"))
            {
                try
                {
                    process.StartInfo.Verb = "runas";
                    process.Kill();
                }
                catch { }
            }
            Webserver1.Stop();
            Socketserver1.Stop();
            if (!isDebug) {
                GCMSSystem.Chrome.Unload();
                if (hardenedShell)
                {
                    try
                    {
                        Taskbar.Show();
                    }
                    catch { }
                }
                GCMSSystem.Chrome.UpdatePref();
            }

            try
            {
                File.Delete(pidFile);
            }
            catch { }

            GCMSSystem.AirServer.Stop();
            GCMSSystem.UnregisterApplicationRecoveryAndRestart();
            TaskbarIcon.Dispose();
            Application.Exit();
        }

        void ActivityWorker_Tick(object sender, EventArgs e)
        {
            if (User32Interop.GetLastInput() < activityThreshold)
            {
                // Restart Timer If Enabled and User Input Detected
                if (isInteractive)
                {
                    Interaction.Stop();
                    int interval = Interaction.Interval;
                    Interaction.Interval = interval;
                    Interaction.Start();
                }
            }
        }
        public void Interaction_Tick(object sender, EventArgs e)
        {
            string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
            var MyIni = new IniFile(iniFile);
            var myKeyboard = MyIni.Read("Load", "Keyboard");

            if (isInteractive)
            {
                var curWebView = "Error";
                try
                {
                    curWebView = SignageBrowser.FrmObj.wBrowser.WebView.Url;
                }
                catch { }

                isInteractive = false;      // Set to False to Stop the Timer Checker

                if (!isInLockdown)
                {
                    // Reset the Mode Back to Normal
                    powerModeLabel.Text = "Normal / Online";
                    powerModeLabel.ForeColor = Color.FromArgb(0, 0, 0);
                }
                // Stop the Interactive Timer and Restart the Checker for Interactive
                if (isAutoCookieCleaner)
                {
                    GCMSSystem.Chrome.ClearCookies(true);
                }

                if (myKeyboard == "Application")
                {
                    // Make sure that the Onscreen Keyboard is Closed (if being used)
                    GCMSSystem.OSK.StopOSK();
                    // if we are using the Application OSK then we also need to clear the last position of the keyboard as this is a full reset
                    // Open up the XML Document ready to modify the location
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.Load(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "keyboard", "Layouts", "Default.xml"));
                    XmlElement documentElement = xmlDocument.DocumentElement;

                    // Set the New Attributes in the XML
                    documentElement.SetAttribute("top", "0");
                    documentElement.SetAttribute("left", "0");

                    // Save the Updates into the XML File
                    xmlDocument.Save(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "keyboard", "Layouts", "Default.xml"));
                }

                // Send The End Interactive Websocket - "endinteractive"
                GCMSSystem.NodeSocket.Send("endinteractive");
                GCMSSystem.InteractiveLog.Send("End");

                Interaction.Stop();
                int interval = Interaction.Interval;
                Interaction.Interval = interval;
                Interaction.Start();

                if (curWebView != "Error" && curWebView != "http://127.0.0.1:444/" && curWebView != "https://127.0.0.1:444/")
                {
                    // Detected that the Kiosk has moved outside of 127.0.0.1 so we need to trigger the function for forwarding back to 127.0.0.1
                    var MySystemLoad = MyIni.Read("Load", "Browser");
                    var MySystemKeyboard = MyIni.Read("Keyboard", "Browser");
                    var BrowserSSL = MyIni.Read("SSL", "Browser");

                    if (MySystemLoad == "Default")
                    {
                        if (SignageBrowser.isXFrame)
                        {
                            // This means that it has had to leave our system for Printing Support and other Elements outside of a iFrame
                            if (BrowserSSL == "On")
                            {
                                SignageBrowser.FrmObj.wBrowser.WebView.Url = "https://127.0.0.1:444";
                            }
                            else
                            {
                                SignageBrowser.FrmObj.wBrowser.WebView.Url = "http://127.0.0.1:444";
                            }
                            SignageBrowser.isXFrame = false;        // Reset for Next Use
                        }
                        else
                        {
                            if (BrowserSSL == "On")
                            {
                                SignageBrowser.FrmObj.wBrowser.WebView.Url = "file://" + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace("#", "%23").Replace("\\", "/") + "/signage/preloader.html?ssl=on";
                            }
                            else
                            {
                                SignageBrowser.FrmObj.wBrowser.WebView.Url = "file://" + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace("#", "%23").Replace("\\", "/") + "/signage/preloader.html?ssl=off";
                            }
                        }
                    }
                }
            }
            powerModeLabel.Text = "Normal / Online";
            powerModeLabel.ForeColor = Color.FromArgb(0, 0, 0);
            CheckForInteractive.Start();
        }

        // Timers
        public void CheckStatsTimer_Tick(object sender, EventArgs e)
        {
            var cpuWMI = new ManagementObjectSearcher("SELECT * FROM Win32_Processor").Get().Cast<ManagementObject>().First();
            var device_cpu_load = Convert.ToString(cpuWMI["LoadPercentage"]) + "% Load";
            cpuLoadLabel.Text = device_cpu_load;
            ramLoadLabel.Text = GCMSSystem.GetRamLoad();

            var currentMAC = GCMSSystem.GetMACAddress();
            devMAC.Text = currentMAC;

            string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
            var MyIni = new IniFile(iniFile);
            devName.Text = MyIni.Read("deviceName", "Monitor");
            devUUID.Text = MyIni.Read("deviceUUID", "Monitor");

            string localIP_check = GCMSSystem.GetIP("LAN");                                    // Global Variable for Local IP
            string vpnIP_check = GCMSSystem.GetIP("VPN");                                      // Global Variable for VPN IP
            string wanIP_check = GCMSSystem.GetIP("WAN");                                      // Global Variable

            // IP Details
            devVPN.Text = vpnIP_check;
            devWAN.Text = wanIP_check;
            devLAN.Text = localIP_check;

            TaskbarContextMenu.Items[2].Text = "LAN \t" + localIP_check;
            TaskbarContextMenu.Items[3].Text = "WAN \t" + wanIP_check;
            TaskbarContextMenu.Items[4].Text = "VPN \t" + vpnIP_check;

            // Service Checks
            // Internet Check
            InternetConnectionOpt.Text = GCMSSystem.CheckForInternetConnection();
            if (InternetConnectionOpt.Text == "Online")
            {
                InternetConnectionOpt.ForeColor = Color.FromArgb(0, 192, 0);
            }
            else
            {
                InternetConnectionOpt.ForeColor = Color.FromArgb(192, 0, 0);

            }
            // Secure (VPN) Check
            SecureConnectionOpt.Text = GCMSSystem.CheckForVPNConnection(networkURL, networkIP);
            if (SecureConnectionOpt.Text == "Online")
            {
                SecureConnectionOpt.ForeColor = Color.FromArgb(0, 192, 0);
                devVPN.Text = GCMSSystem.GetIP("VPN");
                TaskbarContextMenu.Items[4].Text = "VPN \t" + GCMSSystem.GetIP("VPN");
            }
            else
            {
                SecureConnectionOpt.ForeColor = Color.FromArgb(192, 0, 0);

            }
            // Signage Check
            SignageSystemOpt.Text = GCMSSystem.CheckForSignage();
            if (SignageSystemOpt.Text == "Online")
            {
                SignageSystemOpt.ForeColor = Color.FromArgb(0, 192, 0);
            }
            else
            {
                SignageSystemOpt.ForeColor = Color.FromArgb(192, 0, 0);

            }

            ///////////////////////////
            try
            {
                var checkSEN = GCMSSystem.NodeSocket.Check("SEN");
                SENOpt.Text = checkSEN;
                if (checkSEN == "Disconnected")
                {
                    SENOpt.ForeColor = Color.FromArgb(192, 0, 0);
                }
                else
                {
                    SENOpt.ForeColor = Color.FromArgb(0, 192, 0);
                }

                var checkAVA = GCMSSystem.NodeSocket.Check("AVA");
                AVAOpt.Text = checkAVA;
                if (checkAVA == "Disconnected")
                {
                    AVAOpt.ForeColor = Color.FromArgb(192, 0, 0);
                }
                else
                {
                    AVAOpt.ForeColor = Color.FromArgb(0, 192, 0);
                }

                var checkWSK = GCMSSystem.NodeSocket.Check("WSK");
                WSKOpt.Text = checkWSK;
                if (checkWSK == "Disconnected")
                {
                    WSKOpt.ForeColor = Color.FromArgb(192, 0, 0);
                }
                else
                {
                    WSKOpt.ForeColor = Color.FromArgb(0, 192, 0);
                }
            }
            catch { }

            try
            {
                bool nexmosphereBox = SerialPort.GetPortNames().Any(x => x == "COM12");
                if (nexmosphereBox)
                {
                    NEXOpt.Text = "Connected";
                    NEXOpt.ForeColor = Color.FromArgb(0, 192, 0);
                    nexmoshpereSensors = true;
                }
                else
                {
                    NEXOpt.Text = " Disconnected";
                    NEXOpt.ForeColor = Color.FromArgb(192, 0, 0);
                    nexmoshpereSensors = false;
                }
            } catch
            {
                NEXOpt.Text = " Disconnected";
                NEXOpt.ForeColor = Color.FromArgb(192, 0, 0);
                nexmoshpereSensors = false;
            }


            try
            {
                ManagementObjectCollection mbsList = null;
                ManagementObjectSearcher mbs = new ManagementObjectSearcher("Select * From Win32_USBHub");
                mbsList = mbs.Get();
                foreach (ManagementObject mo in mbsList)
                {
                    if (mo["Name"].ToString() == "USB Mass Storage Device")
                    {
                        USBDriveOpt.Text = "Connected";
                    }
                    if (mo["Name"].ToString().Contains("Wireless"))
                    {
                        WiFiCardOpt.Text = "Connected";
                    }
                }
                if (USBDriveOpt.Text == "Connected")
                {
                    USBDriveOpt.ForeColor = Color.FromArgb(0, 192, 0);
                }
                else
                {
                    USBDriveOpt.ForeColor = Color.FromArgb(192, 0, 0);
                }
                if (WiFiCardOpt.Text == "Connected")
                {
                    WiFiCardOpt.ForeColor = Color.FromArgb(0, 192, 0);
                }
                else
                {
                    WiFiCardOpt.ForeColor = Color.FromArgb(192, 0, 0);
                }
            }
            catch { }

            try
            {
                foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (nic.Name.Contains("3G"))
                    {
                        InternetOpt.Text = "Connected";
                    }
                    if (nic.Name.Contains("4G"))
                    {
                        InternetOpt.Text = "Connected";
                    }
                    if (nic.Name.Contains("HSPA"))
                    {
                        InternetOpt.Text = "Connected";
                    }
                }
                if (InternetOpt.Text == "Connected")
                {
                    InternetOpt.ForeColor = Color.FromArgb(0, 192, 0);
                }
                else
                {
                    InternetOpt.ForeColor = Color.FromArgb(192, 0, 0);
                }
            }
            catch { }

            try
            {
                bool isTouchDevice = Tablet.TabletDevices.Cast<TabletDevice>().Any(dev => dev.Type == TabletDeviceType.Touch);
                if (isTouchDevice)
                {
                    TouchscreenOpt.Text = "Connected";
                    TouchscreenOpt.ForeColor = Color.FromArgb(0, 192, 0);
                }
                else
                {
                    TouchscreenOpt.Text = "Disconnected";
                    TouchscreenOpt.ForeColor = Color.FromArgb(192, 0, 0);
                }
            }
            catch { }

            try
            {
                bool portExists = SerialPort.GetPortNames().Any(x => x == "COM1");
                if (portExists)
                {
                    if (!GCMSSystem.CheckOpened("RS232"))
                    {
                        Form Rs232 = new Rs232();
                        // Rs232.Show();
                    }
                    RS232Opt.Text = "Available";
                    RS232Opt.ForeColor = Color.FromArgb(0, 192, 0);
                    SerialPort port = new SerialPort("COM1");
                    if (port.IsOpen)
                    {
                        RS232Opt.Text = "In Use";
                        RS232Opt.ForeColor = Color.FromArgb(192, 0, 0);
                    }
                }
                else
                {
                    RS232Opt.Text = "Not Available";
                    RS232Opt.ForeColor = Color.FromArgb(192, 0, 0);
                }
            }
            catch { }

            try
            {
                bool portExists2 = SerialPort.GetPortNames().Any(x => x == "COM10");
                if (portExists2)
                {
                    SensorOpt.Text = "Inserted";
                    SensorOpt.ForeColor = Color.FromArgb(0, 192, 0);
                    SerialPort port2 = new SerialPort("COM10");
                    if (port2.IsOpen)
                    {
                        SensorOpt.Text = "In Use";
                        SensorOpt.ForeColor = Color.FromArgb(192, 0, 0);
                    }
                    omronSensor = true;
                }
                else
                {
                    SensorOpt.Text = "Not Available";
                    SensorOpt.ForeColor = Color.FromArgb(192, 0, 0);
                    omronSensor = false;
                }
            }
            catch { }

            ///////////////////////////
            if (omronSensor)
            {
                var omronService = MyIni.Read("Env", "Serv");

                if (!GCMSSystem.CheckOpened("OmronSensor"))
                {
                    Form OmronDebug = new OmronSensor();
                    OmronDebug.Show();
                    if (omronService == "Disabled")
                    {
                        OmronDebug.Visible = false;
                    }
                }
            }

            if (nexmoshpereSensors)
            {
                if (!GCMSSystem.CheckOpened("Nexmosphere") && nexmoshpereSensors)
                {
                    Form NexusDebug = new Nexmosphere();
                    NexusDebug.Show();
                }
            }

            if (airServerMirroring)
            {
                try
                {
                    if (!GCMSSystem.AirServer.IsRunning())
                    {
                        GCMSSystem.AirServer.Start();               // Start AirServer
                    }
                }
                catch { }
            }
        }
        public void CheckServicesTimer_Tick(object sender, EventArgs e)
        {
            // Read INI File for Config.ini
            string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
            var MyIni = new IniFile(iniFile);
            // Setup which Network we should run over
            var MyNetwork = MyIni.Read("licType", "Licence");
            // Different Running Modes
            var EngineerMode = MyIni.Read("powersaveMode", "Network");               // Non RS232 Low Power Mode
            var LowPowerMode1 = MyIni.Read("powersaveMode2", "Network");             // RS232 Low Power Mode
            var LowPowerMode2 = MyIni.Read("maintMode", "Network");                  // Maintenance Mode
            if (EngineerMode == "TRUE" || LowPowerMode1 == "TRUE" || LowPowerMode2 == "TRUE") { return; }
            CheckerService(LowPowerMode1, LowPowerMode2, EngineerMode);
        }
        public void CallHomeTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                File.WriteAllText(pidFile, nProcessID.ToString());      // Update the PID file, so the system can see that its active
            }
            catch { }

            try { EngineerTools.DetectNetworkAdapter(); } catch { }

            // Call Home on init, however the rest will be deligated over to a Timer
            using (var client = new WebClient())
            {
                // Read INI File for Config.ini
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                // Setup which Network we should run over
                var MyNetwork = MyIni.Read("licType", "Licence");
                if (MyNetwork == "SEC")
                {
                    // Due to SSL Issues over an IP Address - We Use IP Instead of Domain
                    networkURL = "http://172.16.0.2";
                    networkIP = "172.16.0.2";
                    networkName = "Secure";
                    networkNameShort = "S";
                }
                else
                {
                    // Due to SSL Issues over an IP Address - We Use SSL for Domain
                    networkURL = "https://api.globalcms.co.uk";
                    networkIP = "api.globalcms.co.uk";
                    networkName = "Public";
                    networkNameShort = "P";
                }
                if (MyNetwork == "SEC")
                {
                    var pingTest = GCMSSystem.Ping(networkURL);
                    if (!pingTest)
                    {
                        networkURL = "https://api.globalcms.co.uk";
                        networkIP = "api.globalcms.co.uk";
                        networkName = "Public";
                        networkNameShort = "P";
                    }
                    else
                    {
                        networkURL = "http://172.16.0.2";
                        networkIP = "172.16.0.2";
                        networkName = "Secure";
                        networkNameShort = "S";
                    }
                }
                if (NetworkOverride != "Auto" && NetworkOverride == "Public") { networkURL = "https://api.globalcms.co.uk"; networkIP = "api.globalcms.co.uk"; networkName = "Public"; networkNameShort = "P"; }
                if (NetworkOverride != "Auto" && NetworkOverride == "Secure") { networkURL = "http://172.16.0.2"; networkIP = "172.16.0.2"; networkName = "Secure"; networkNameShort = "S"; }

                // Other Text Vars
                systemNetworkOpt.Text = networkNameShort;

                // Different Running Modes
                var meLowPowerMode1 = MyIni.Read("powersaveMode", "Network");               // Non RS232 Low Power Mode
                var meLowPowerMode2 = MyIni.Read("powersaveMode2", "Network");              // RS232 Low Power Mode
                var meMaintMode = MyIni.Read("maintMode", "Network");                       // Maintenance Mode

                var powerMode = "FALSE";
                var maintMode = "FALSE";
                if (!isInLockdown) 
                { 
                    if ((meLowPowerMode1 == "TRUE" || meLowPowerMode2 == "TRUE") && meMaintMode == "FALSE" && !isInteractive)
                    {
                        powerModeLabel.Text = "Low Power";
                        powerModeLabel.ForeColor = Color.FromArgb(0, 192, 0);
                        powerMode = "TRUE";
                        maintMode = "FALSE";

                        if (meLowPowerMode1 == "TRUE" || meLowPowerMode2 == "FALSE") {
                            GCMSSystem.TriggerSystem("MONOFF", true, true);
                        }
                        if (meLowPowerMode1 == "FALSE" || meLowPowerMode2 == "TRUE")
                        {
                            GCMSSystem.TriggerSystem("SCREENOFF", true, true);
                        }
                    }
                    if (meMaintMode == "TRUE" && meLowPowerMode1 == "FALSE" && meLowPowerMode2 == "FALSE" && !isInteractive)
                    {
                        powerModeLabel.Text = "Maintenance";
                        powerModeLabel.ForeColor = Color.FromArgb(255, 128, 0);
                        powerMode = "FALSE";
                        maintMode = "TRUE";
                    }
                    if (meLowPowerMode1 == "FALSE" && meLowPowerMode2 == "FALSE" && meMaintMode == "FALSE" && !isInteractive)
                    {
                        powerModeLabel.Text = "Normal / Online";
                        powerModeLabel.ForeColor = Color.FromArgb(0, 0, 0);
                        powerMode = "FALSE";
                        maintMode = "FALSE";

                        // This should make sure that if its in NORMAL mode that it should not keep the ScreenLock Form Open
                        try
                        {
                            if (GCMSSystem.CheckOpened("ScreenLock")) { 
                                Application.OpenForms["ScreenLock"].Close();
                            }
                        }
                        catch { }

                    }
                    if ((meLowPowerMode1 == "TRUE" || meLowPowerMode2 == "TRUE") && meMaintMode == "TRUE" && !isInteractive)
                    {
                        powerModeLabel.Text = "Low Power";
                        powerModeLabel.ForeColor = Color.FromArgb(0, 0, 192);
                        powerMode = "TRUE";
                        maintMode = "TRUE";
                    }
                }
                ///////////////////////////
                try
                {
                    var checkSEN = GCMSSystem.NodeSocket.Check("SEN");
                    SENOpt.Text = checkSEN;
                    if (checkSEN == "Disconnected")
                    {
                        SENOpt.ForeColor = Color.FromArgb(192, 0, 0);
                    }
                    else
                    {
                        SENOpt.ForeColor = Color.FromArgb(0, 192, 0);
                    }

                    var checkAVA = GCMSSystem.NodeSocket.Check("AVA");
                    AVAOpt.Text = checkAVA;
                    if (checkAVA == "Disconnected")
                    {
                        AVAOpt.ForeColor = Color.FromArgb(192, 0, 0);
                    }
                    else
                    {
                        AVAOpt.ForeColor = Color.FromArgb(0, 192, 0);
                    }

                    var checkWSK = GCMSSystem.NodeSocket.Check("WSK");
                    WSKOpt.Text = checkWSK;
                    if (checkWSK == "Disconnected")
                    {
                        WSKOpt.ForeColor = Color.FromArgb(192, 0, 0);
                    }
                    else
                    {
                        WSKOpt.ForeColor = Color.FromArgb(0, 192, 0);
                    }
                }
                catch { }

                try
                {
                    ManagementObjectCollection mbsList = null;
                    ManagementObjectSearcher mbs = new ManagementObjectSearcher("Select * From Win32_USBHub");
                    mbsList = mbs.Get();
                    foreach (ManagementObject mo in mbsList)
                    {
                        if (mo["Name"].ToString() == "USB Mass Storage Device")
                        {
                            USBDriveOpt.Text = "Connected";
                        }
                        if (mo["Name"].ToString().Contains("Wireless"))
                        {
                            WiFiCardOpt.Text = "Connected";
                        }
                    }
                    if (USBDriveOpt.Text == "Connected")
                    {
                        USBDriveOpt.ForeColor = Color.FromArgb(0, 192, 0);
                    }
                    else
                    {
                        USBDriveOpt.ForeColor = Color.FromArgb(192, 0, 0);
                    }
                    if (WiFiCardOpt.Text == "Connected")
                    {
                        WiFiCardOpt.ForeColor = Color.FromArgb(0, 192, 0);
                    }
                    else
                    {
                        WiFiCardOpt.ForeColor = Color.FromArgb(192, 0, 0);
                    }
                }
                catch { }

                try
                {
                    foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                    {
                        if (nic.Name.Contains("3G"))
                        {
                            InternetOpt.Text = "Connected";
                        }
                        if (nic.Name.Contains("4G"))
                        {
                            InternetOpt.Text = "Connected";
                        }
                        if (nic.Name.Contains("HSPA"))
                        {
                            InternetOpt.Text = "Connected";
                        }
                    }
                    if (InternetOpt.Text == "Connected")
                    {
                        InternetOpt.ForeColor = Color.FromArgb(0, 192, 0);
                    }
                    else
                    {
                        InternetOpt.ForeColor = Color.FromArgb(192, 0, 0);
                    }
                }
                catch { }

                try
                {
                    bool isTouchDevice = Tablet.TabletDevices.Cast<TabletDevice>().Any(dev => dev.Type == TabletDeviceType.Touch);
                    if (isTouchDevice)
                    {
                        TouchscreenOpt.Text = "Connected";
                        TouchscreenOpt.ForeColor = Color.FromArgb(0, 192, 0);
                    }
                    else
                    {
                        TouchscreenOpt.Text = "Disconnected";
                        TouchscreenOpt.ForeColor = Color.FromArgb(192, 0, 0);
                    }
                }
                catch { }

                try
                {
                    bool portExists = SerialPort.GetPortNames().Any(x => x == "COM1");
                    if (portExists)
                    {
                        RS232Opt.Text = "Available";
                        RS232Opt.ForeColor = Color.FromArgb(0, 192, 0);
                        SerialPort port = new SerialPort("COM1");
                        if (port.IsOpen)
                        {
                            RS232Opt.Text = "In Use";
                            RS232Opt.ForeColor = Color.FromArgb(192, 0, 0);
                        }
                    }
                    else
                    {
                        RS232Opt.Text = "Not Available";
                        RS232Opt.ForeColor = Color.FromArgb(192, 0, 0);
                    }
                }
                catch { }
                ///////////////////////////

                // WMI Calls - WINDOWS ONLY
                var cpuWMI = new ManagementObjectSearcher("SELECT * FROM Win32_Processor").Get().Cast<ManagementObject>().First();
                var osWMI = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem").Get().Cast<ManagementObject>().First();
                var gpuWMI = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController").Get().Cast<ManagementObject>().First();
                var hddCWMI = new ManagementObjectSearcher("SELECT * FROM Win32_logicaldisk").Get().Cast<ManagementObject>().First();

                // Check if Drive D Exists
                bool DriveD = GCMSSystem.DriveExists("D:\\");
                // If D:\ Exists & is a Disk Drive NOT CD-ROM
                var device_hdd_d = "0";
                var totalHDD = "0";
                var freeHDD = "0";

                var freeC = Convert.ToString(Convert.ToDouble(hddCWMI["FreeSpace"]));
                var totalC = Convert.ToString(Convert.ToDouble(hddCWMI["Size"]));

                var freeD = "0";
                var totalD = "0";

                if (DriveD)
                {
                    var hddDWMI = new ManagementObjectSearcher("SELECT * FROM Win32_logicaldisk WHERE driveType = \"3\" AND name = \"D:\" ").Get().Cast<ManagementObject>().Last();
                    device_hdd_d = (string)hddDWMI["DeviceID"] + " ";
                    device_hdd_d += Convert.ToString(Math.Round(Convert.ToDouble(hddDWMI["Size"]) / 1048576000, 0)) + " GB";

                    totalHDD = Convert.ToString(Convert.ToDouble(hddCWMI["Size"]) + Convert.ToDouble(hddDWMI["Size"]));
                    freeHDD = Convert.ToString(Convert.ToDouble(hddCWMI["FreeSpace"]) + Convert.ToDouble(hddDWMI["FreeSpace"]));

                    freeD = Convert.ToString(Convert.ToDouble(hddDWMI["FreeSpace"]));
                    totalD = Convert.ToString(Convert.ToDouble(hddDWMI["Size"]));
                }
                else
                {
                    totalHDD = Convert.ToString(Convert.ToDouble(hddCWMI["Size"]));
                    freeHDD = Convert.ToString(Convert.ToDouble(hddCWMI["FreeSpace"]));
                }

                // CPU, OS, RAM, GPU Details
                var device_cpu = (string)cpuWMI["Name"];
                var device_os = (string)osWMI["Caption"];
                string releaseId = "Unknown";
                try
                {
                    releaseId = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId", "").ToString();
                }
                catch { }
                var device_ram = GCMSSystem.GetRAMsize("Formatted");
                var device_gpu = (string)gpuWMI["Name"];

                // HDD and CPU Load
                var device_hdd_c = (string)hddCWMI["DeviceID"] + " ";
                device_hdd_c += Convert.ToString(Math.Round(Convert.ToDouble(hddCWMI["Size"]) / 1048576000, 0)) + " GB";
                device_hdd_c += "  ";

                var device_cpu_load_raw = Convert.ToString(cpuWMI["LoadPercentage"]) + "";
                var device_cpu_load = Convert.ToString(cpuWMI["LoadPercentage"]) + "% Load";

                // Remove any unwanted text here
                device_os = device_os.Replace("Microsoft ", "");
                if (releaseId != "Unknown")
                {
                    device_os += " (" + releaseId + ")";
                }
                device_cpu =
                   device_cpu
                   .Replace("(TM)", "")
                   .Replace("(tm)", "™")
                   .Replace("(R)", "®")
                   .Replace("(r)", "®")
                   .Replace("(C)", "©")
                   .Replace("(c)", "©")
                   .Replace("    ", " ")
                   .Replace("  ", " ");

                // System Hardware Labels
                CPUArch.Text = device_cpu;
                devOS.Text = device_os;
                ramAmount.Text = device_ram;
                gfxCard.Text = device_gpu;
                hddAmounts.Text = device_hdd_c + device_hdd_d;
                cpuLoadLabel.Text = device_cpu_load;
                ramLoadLabel.Text = GCMSSystem.GetRamLoad();

                // Service Checks
                InternetConnectionOpt.Text = GCMSSystem.CheckForInternetConnection();
                if (InternetConnectionOpt.Text == "Online")
                {
                    InternetConnectionOpt.ForeColor = Color.FromArgb(0, 192, 0);
                }
                SecureConnectionOpt.Text = GCMSSystem.CheckForVPNConnection(networkURL, networkIP);
                if (SecureConnectionOpt.Text == "Online")
                {
                    SecureConnectionOpt.ForeColor = Color.FromArgb(0, 192, 0);
                }
                SignageSystemOpt.Text = GCMSSystem.CheckForSignage();
                if (SignageSystemOpt.Text == "Online")
                {
                    SignageSystemOpt.ForeColor = Color.FromArgb(0, 192, 0);
                    screen_signage_running = "YES";
                }
                else
                {
                    screen_signage_running = "NO";
                }

                if (meLowPowerMode1 == "TRUE" || meLowPowerMode2 == "TRUE")
                {
                    SignageSystemOpt.Text = "Online";
                    SignageSystemOpt.ForeColor = Color.FromArgb(0, 192, 0);
                    screen_signage_running = "YES";
                }

                bool isSignageEnabled = GCMSSystem.Chrome.IsSignageEnabled();
                if (!isSignageEnabled)
                {
                    SignageSystemOpt.ForeColor = Color.FromArgb(192, 0, 0);
                    SignageSystemOpt.Text = "Offline";
                }

                bool isHDMIConnected = false;
                try
                {
                    isHDMIConnected = Main.Check_HDMI_Output();           // Is the HDMI Cable plugged into the machine?
                    // Debug.WriteLine("Detecting HDMI Devices Connected : " + isHDMIConnected);
                }
                catch { }

                bool isVGAConnected = false;
                try
                {
                    isVGAConnected = GCMSSystem.Display.Check("0");            // Is there a Non HDMI Port Cable plugged into the machine?    
                    // Debug.WriteLine("Detecting Non HDMI Devices : " + isVGAConnected);
                }
                catch { }

                // Now we know the Devices Connected, if any report back as connected then we need to assign myDisplay to "ONLINE"
                var myDisplay = "OFFLINE";
                if (isHDMIConnected || isVGAConnected)
                {
                    myDisplay = "ONLINE";
                    SCROpt.Text = "Connected";
                    SCROpt.ForeColor = Color.FromArgb(0, 192, 0);
                }
                else
                {
                    SCROpt.Text = "Disconnected";
                    SCROpt.ForeColor = Color.FromArgb(192, 0, 0);
                    if (!isHDMIConnected && !isVGAConnected)
                    {
                        SCROpt.Text = "Not Supported";
                    }
                }

                // Read Current Signage Version & SubVersion
                using (StreamReader streamReader = new StreamReader(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "version.txt"), Encoding.UTF8))
                {
                    signageVersion = streamReader.ReadToEnd();
                }
                using (StreamReader streamReader = new StreamReader(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "subversion.txt"), Encoding.UTF8))
                {
                    signageSubVersion = streamReader.ReadToEnd();
                }

                var screen_res = Screen.PrimaryScreen.Bounds.Width.ToString() + "x" + Screen.PrimaryScreen.Bounds.Height.ToString();

                var totalRAM = GCMSSystem.GetRAMsize("Not-Formatted");
                var freeRAM = GCMSSystem.GetRamFree();

                double myTemperatureInCelsius;
                var cpuTempC = 0;
                try
                {
                    var curTemp = Convert.ToDouble(GCMSSystem.GetCPUTemp());
                    curTemp = curTemp / 10;
                    myTemperatureInCelsius = Temperature.FromKelvin(curTemp).Celsius;
                    cpuTempC = (int)myTemperatureInCelsius;
                }
                catch { }

                var contentCount = "0";
                try
                {
                    contentCount = GCMSSystem.HowManyContent();
                }
                catch
                {
                    contentCount = "0";
                }

                var currentCanvas = "Unknown";
                var signageLoaderEnabled = MyIni.Read("Signage", "Serv");
                var signageLoaderVer = MyIni.Read("SignageLoader", "Sign");
                var signageURL = MyIni.Read("Load", "Browser");
                var signageXSS = MyIni.Read("Referer", "Browser");
                var signageDebug = MyIni.Read("Debug", "Browser");
                var signageSSL = MyIni.Read("SSL", "Browser");
                var signageKeyboard = MyIni.Read("Keyboard", "Browser");

                var eoDLL = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\EO.Base.dll";
                var EOversionInfo = FileVersionInfo.GetVersionInfo(eoDLL).FileVersion;

                string appIniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "launcher.ini");
                const int BufferSize = 128;
                List<string> launchArr1 = new List<string>();
                List<string> launchArr2 = new List<string>();
                using (var fileStream = File.OpenRead(appIniFile))
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        var appFile = line.Split('\\').Last().Split('.').First();

                        var appRunning = "NO";
                        var appLauncherProcess = Process.GetProcesses().Any(p => p.ProcessName.Contains(appFile));
                        if (appLauncherProcess)
                        {
                            appRunning = "YES";
                        }
                        launchArr1.Add(line);
                        launchArr2.Add(appRunning);
                    }
                }

                bool isTeamViewerInstalled = false;
                try
                {
                    isTeamViewerInstalled = GCMSSystem.TeamViewer.IsInstaled();
                }
                catch { }

                string teamviewerInstalled = "No";
                bool teamviewerRunningEXE = false;
                string teamviewerRunning = "No";
                string teamviewerVersion = "";
                string teamviewerID = "";
                if (isTeamViewerInstalled)
                {
                    try
                    {
                        teamviewerRunningEXE = Process.GetProcesses().Any(p => p.ProcessName.Contains("TeamViewer"));
                        if (teamviewerRunningEXE)
                        {
                            teamviewerRunning = "Yes";
                        }
                    }
                    catch { }
                    teamviewerInstalled = "Yes";
                    try
                    {
                        teamviewerVersion = GCMSSystem.TeamViewer.Version();
                    }
                    catch { }
                    try
                    {
                        teamviewerID = GCMSSystem.TeamViewer.ID();
                    }
                    catch { }
                }

                using (Ping p = new Ping())
                {
                    try
                    {
                        pingMS = p.Send("api.globalcms.co.uk").RoundtripTime.ToString();
                    }
                    catch { }
                }

                if (omronSensor)
                {
                    // Create a JSON Object of the OmronSensor so that it can drop this json into a flat file that could be read by signage
                    dynamic jsonObject = new JObject();
                    jsonObject.Status = SensorOpt.Text;
                    jsonObject.Temp = OmronSensor.OmronData.Temp;
                    jsonObject.Humidity = OmronSensor.OmronData.Humidity;
                    jsonObject.Light = OmronSensor.OmronData.Light;
                    jsonObject.Pressure = OmronSensor.OmronData.Pressure;
                    jsonObject.Noise = OmronSensor.OmronData.Noise;
                    jsonObject.eTVOC = OmronSensor.OmronData.eTVOC;
                    jsonObject.eCO2 = OmronSensor.OmronData.eCO2;
                    jsonObject.Vibrate_SI = OmronSensor.OmronData.Vibrate_SI;
                    jsonObject.Vibrate_PGA = OmronSensor.OmronData.Vibrate_PGA;
                    jsonObject.Vibrate_SeismicIntensity = OmronSensor.OmronData.Vibrate_SeismicIntensity;
                    jsonObject.Vibrate_AccelerationX = OmronSensor.OmronData.Vibrate_AccelerationX;
                    jsonObject.Vibrate_AccelerationY = OmronSensor.OmronData.Vibrate_AccelerationY;
                    jsonObject.Vibrate_AccelerationZ = OmronSensor.OmronData.Vibrate_AccelerationZ;
                    jsonObject.HeatStrokeRisk = OmronSensor.OmronData.HeatStrokeRisk;
                    jsonObject.DiscomfortIndex = OmronSensor.OmronData.DiscomfortIndex;
                    jsonObject.NlLabelTxt = OmronSensor.OmronData.NlLabelTxt;
                    jsonObject.DILabelTxt = OmronSensor.OmronData.DILabelTxt;
                    jsonObject.HSILabelTxt = OmronSensor.OmronData.HSILabelTxt;
                    jsonObject.LlLabelTxt = OmronSensor.OmronData.LlLabelTxt;
                    jsonObject.eTVOCLabelTxt = OmronSensor.OmronData.eTVOCLabelTxt;
                    jsonObject.eCO2LabelTxt = OmronSensor.OmronData.eCO2LabelTxt;

                    var jsonStr = "";
                    try
                    {
                        jsonStr = JsonConvert.SerializeObject(jsonObject);
                    }
                    catch { }

                    try
                    {
                        // Write the JSON Object to flat flie
                        File.WriteAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "logs", "env.json"), jsonStr);
                    }
                    catch
                    {
                        // Debug.WriteLine("Error Writing JSON to File");
                    }
                }

                int pendingWindowsUpdates = 0;
                try
                {
                    pendingWindowsUpdates = GCMSSystem.WindowsEnv.CheckUpdates();
                }
                catch { }

                // Create the $_POST Data for the HTTP Request
                var values = new NameValueCollection
                {
                    ["clientID"] = MyIni.Read("clientID", "Monitor"),
                    ["deviceID"] = MyIni.Read("deviceName", "Monitor"),
                    ["deviceMAC"] = MyIni.Read("deviceMAC", "Monitor"),
                    ["deviceUUID"] = MyIni.Read("deviceUUID", "Monitor"),
                    ["hardwareMAC"] = MyIni.Read("hardwareMAC", "Monitor"),
                    ["wanIP"] = wanIP,
                    ["vpnIP"] = vpnIP,
                    ["lanIP"] = localIP,
                    ["signageRunning"] = screen_signage_running,
                    ["avaRunning"] = GCMSSystem.CheckForAVA(),
                    ["theRes"] = screen_res,
                    ["gfxStr"] = "",
                    ["monVer"] = version,
                    ["monSubVer"] = subversion,
                    ["backupFile"] = "ERROR",
                    ["theOS"] = device_os,
                    ["theOSBuild"] = releaseId,
                    ["cpuLoad"] = device_cpu_load_raw,
                    ["cpuTemp"] = cpuTempC.ToString(),
                    ["gpuLoad"] = "0",
                    ["gpuTemp"] = "0",
                    ["hddTotal"] = totalHDD,
                    ["hddFree"] = freeHDD,
                    ["totalC"] = totalC,
                    ["totalD"] = totalD,
                    ["freeC"] = freeC,
                    ["freeD"] = freeD,
                    ["hddTemp"] = "0",
                    ["totalRam"] = totalRAM,
                    ["freeRam"] = freeRAM,
                    ["curTime"] = DateTime.Now.ToString("HH:mm"),
                    ["signVersion"] = signageVersion,
                    ["signSubVersion"] = signageSubVersion,
                    ["chromeVersion"] = GCMSSystem.CheckChromeVersion(),
                    ["myDisplay"] = myDisplay,
                    ["maintMode"] = maintMode,
                    ["lowpowerMode"] = powerMode,
                    ["senBoard"] = SENOpt.Text,
                    ["signContentCount"] = contentCount,
                    ["signLoader"] = signageLoaderEnabled,
                    ["signLoaderVer"] = signageLoaderVer.ToString(),
                    ["EOVersion"] = EOversionInfo.ToString(),
                    ["currentCanvas"] = currentCanvas,
                    ["launchArr1"] = string.Join(",", launchArr1.ToArray()),
                    ["launchArr2"] = string.Join(",", launchArr2.ToArray()),
                    ["teamviewerRunning"] = teamviewerRunning,
                    ["teamviewerInstalled"] = teamviewerInstalled,
                    ["teamviewerVersion"] = teamviewerVersion,
                    ["teamviewerID"] = teamviewerID,
                    ["omronStatus"] = SensorOpt.Text,
                    ["omronTemp"] = OmronSensor.OmronData.Temp,
                    ["omronHumidty"] = OmronSensor.OmronData.Humidity,
                    ["omronLight"] = OmronSensor.OmronData.Light,
                    ["omronPressure"] = OmronSensor.OmronData.Pressure,
                    ["omronNoise"] = OmronSensor.OmronData.Noise,
                    ["omronETVOC"] = OmronSensor.OmronData.eTVOC,
                    ["omronECO2"] = OmronSensor.OmronData.eCO2,
                    ["omronVibrateSI"] = OmronSensor.OmronData.Vibrate_SI,
                    ["omronVibratePGA"] = OmronSensor.OmronData.Vibrate_PGA,
                    ["omronVibrateSeismicIntensity"] = OmronSensor.OmronData.Vibrate_SeismicIntensity,
                    ["omronVibrateAccelerationX"] = OmronSensor.OmronData.Vibrate_AccelerationX,
                    ["omronVibrateAccelerationY"] = OmronSensor.OmronData.Vibrate_AccelerationY,
                    ["omronVibrateAccelerationZ"] = OmronSensor.OmronData.Vibrate_AccelerationZ,
                    ["omronHeatStrokeRisk"] = OmronSensor.OmronData.HeatStrokeRisk,
                    ["omronDiscomfortIndex"] = OmronSensor.OmronData.DiscomfortIndex,
                    ["omronNlLabelTxt"] = OmronSensor.OmronData.NlLabelTxt,
                    ["omronDILabelTxt"] = OmronSensor.OmronData.DILabelTxt,
                    ["omronHSILabelTxt"] = OmronSensor.OmronData.HSILabelTxt,
                    ["omronLlLabelTxt"] = OmronSensor.OmronData.LlLabelTxt,
                    ["omronETVOCLabelTxt"] = OmronSensor.OmronData.eTVOCLabelTxt,
                    ["omronECO2LabelTxt"] = OmronSensor.OmronData.eCO2LabelTxt,
                    ["airserverCode"] = GCMSSystem.AirServer.Passcode(),
                    ["airserverVersion"] = GCMSSystem.AirServer.Version().ToString(),
                    ["airserverInstalled"] = GCMSSystem.AirServer.IsInstaled().ToString(),
                    ["airserverRunning"] = GCMSSystem.AirServer.IsRunning().ToString(),
                    ["pendingWindowsUpdates"] = pendingWindowsUpdates.ToString(),
                    ["isTrial"] = isTrial.ToString(),
                    ["trialStart"] = trialStart.ToString(),
                    ["trialEnd"] = trialEnd.ToString(),
                    ["signageURL"] = MyIni.Read("Load", "Browser"),
                    ["signageXSS"] = MyIni.Read("Referer", "Browser"),
                    ["signageDebug"] = MyIni.Read("Debug", "Browser"),
                    ["signageSSL"] = MyIni.Read("SSL", "Browser"),
                    ["signageKeyboard"] = MyIni.Read("Keyboard", "Browser"),
                    ["poorInternet"] = MyIni.Read("poorInternet", "Monitor"),
                    ["currentNIC"] = EngineerTools.DetectNetworkAdapter2(),
                    ["pingMS"] = pingMS
                };

                var responseString = "";
                try
                {
                    var response = client.UploadValues(networkURL + "/v2/inbound.php", values);
                    responseString = Encoding.Default.GetString(response);
                    isOnline = true;
                    powerModeLabel.Text = "Normal / Online";
                    powerModeLabel.ForeColor = Color.FromArgb(0, 0, 0);
                }
                catch
                {
                    isOnline = false;
                    powerModeLabel.Text = "Normal / Offline";
                    powerModeLabel.ForeColor = Color.FromArgb(0, 0, 0);
                    responseString = "Error";
                    GCMSSystem.FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [ERR] Cannot Call API Server [" + networkNameShort + "] - Check Internet/Firewall");
                }
                LastHeartbeatLabel.Text = Convert.ToString(responseString);
            }
        }
        public void ReceivedTriggersTimer_Tick(object sender, EventArgs e)
        {
            using (var client = new WebClient())
            {
                // Read INI File for Config.ini
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                // Setup which Network we should run over
                var MyNetwork = MyIni.Read("licType", "Licence");
                if (MyNetwork == "SEC")
                {
                    // Due to SSL Issues over an IP Address - We Use IP Instead of Domain
                    networkURL = "http://172.16.0.2";
                    networkIP = "172.16.0.2";
                    networkName = "Secure";
                }
                else
                {
                    // Due to SSL Issues over an IP Address - We Use SSL for Domain
                    networkURL = "https://api.globalcms.co.uk";
                    networkIP = "api.globalcms.co.uk";
                    networkName = "Public";
                }
                if (MyNetwork == "SEC")
                {
                    var pingTest = GCMSSystem.Ping(networkURL);
                    if (!pingTest)
                    {
                        networkURL = "https://api.globalcms.co.uk";
                        networkIP = "api.globalcms.co.uk";
                        networkName = "Public";
                        networkNameShort = "P";
                    }
                    else
                    {
                        networkURL = "http://172.16.0.2";
                        networkIP = "172.16.0.2";
                        networkName = "Secure";
                        networkNameShort = "S";
                    }
                }
                if (NetworkOverride != "Auto" && NetworkOverride == "Public") { networkURL = "https://api.globalcms.co.uk"; networkIP = "api.globalcms.co.uk"; networkName = "Public"; networkNameShort = "P"; }
                if (NetworkOverride != "Auto" && NetworkOverride == "Secure") { networkURL = "http://172.16.0.2"; networkIP = "172.16.0.2"; networkName = "Secure"; networkNameShort = "S"; }

                // Create the $_POST Data for the HTTP Request
                var values = new NameValueCollection
                {
                    ["clientID"] = MyIni.Read("clientID", "Monitor"),
                    ["deviceID"] = MyIni.Read("deviceName", "Monitor"),
                    ["deviceMAC"] = MyIni.Read("deviceMAC", "Monitor"),
                    ["deviceUUID"] = MyIni.Read("deviceUUID", "Monitor"),
                    ["hardwareMAC"] = MyIni.Read("hardwareMAC", "Monitor")
                };

                var responseString = "";
                try
                {
                    var response = client.UploadValues(networkURL + "/v2/outbound.php", values);
                    responseString = Encoding.Default.GetString(response);
                    responseString = GCMSSystem.RemoveWhitespace(responseString);
                    isOnline = true;
                    powerModeLabel.Text = "Normal / Online";
                    powerModeLabel.ForeColor = Color.FromArgb(0, 0, 0);
                }
                catch
                {
                    responseString = "Error";
                    isOnline = false;
                    powerModeLabel.Text = "Normal / Offline";
                    powerModeLabel.ForeColor = Color.FromArgb(0, 0, 0);
                }

                if (responseString != "Error")
                {
                    GCMSSystem.TriggerSystem(responseString, false, false);
                }

                // Make Sure that if the Power Timer Window is always there, if it has been shut for what ever reason, it will re-open and start timers
                var PowerIni = new IniFile(powerParamsFile);
                var powerStatus = PowerIni.Read("Status", "System");
                if (!GCMSSystem.CheckOpened("PowerTimer") && powerStatus != "")
                {
                    // new System.Threading.Thread(() => new PowerTimerWindow().ShowDialog()).Start();
                    // var powerTimerWindow = new PowerTimerWindow();
                    // powerTimerWindow.Show();
                }
            }
        }
        public void CheckSNAP_Tick(object sender, EventArgs e)
        {
            string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
            var MyIni = new IniFile(iniFile);
            var EngineerMode = MyIni.Read("Network", "maintMode");
            var LowPowerMode1 = MyIni.Read("Network", "powersaveMode");
            var LowPowerMode2 = MyIni.Read("Network", "powersaveMode2");

            if (EngineerMode == "TRUE" || LowPowerMode1 == "TRUE" || LowPowerMode2 == "TRUE") { return; }
            if (SignageBrowser.airServerConnected) { return; }      // If in AirServer Mode we need to ignore other checkers
            string responseString;
            try
            {
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                try
                {
                    HttpWebRequest request = GCMSSystem.GetRequest("http://localhost:444/browserConnected");
                    WebResponse webResponse = request.GetResponse();
                    responseString = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                }
                catch
                {
                    responseString = "false";
                }
            }
            catch
            {
                responseString = "false";
            }

            var actualResponse = Convert.ToBoolean(responseString);
            // Setup which Network we should run over
            var maintMode = MyIni.Read("maintMode", "Network");
            var meLowPowerMode1 = MyIni.Read("powersaveMode", "Network");               // Non RS232 Low Power Mode
            var meLowPowerMode2 = MyIni.Read("powersaveMode2", "Network");              // RS232 Low Power Mode
            var MySystemLoad = MyIni.Read("Load", "Browser");
            if (MySystemLoad != "Default")
            {
                CheckSNAP.Stop();
                CheckSNAP.Interval = 60000 + (MainForm.snapDelay * 1000);
            }

            if (maintMode == "TRUE" || meLowPowerMode1 == "TRUE" || meLowPowerMode2 == "TRUE" && !isDebug)
            {
                snapCounter = 0;
            }

            if (!actualResponse && maintMode == "FALSE" && meLowPowerMode1 == "FALSE" && meLowPowerMode2 == "FALSE" && !isDebug)
            {
                // If Node is not connected to the browser, this either means that the browser isnt opened or that the browser has SNAPPED. 
                GCMSSystem.FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [AUTOFIX] SNAP on Browser Detected");
                GCMSSystem.RemoteLog.Send(DateTime.Now.ToString("dd MMM HH:mm:ss"), "SNAP");
                snapCounter++;

                // Chrome and Reload into the correct options (Kiosk Mode)
                var chromeRunning = "NO";
                var chromeProcess = Process.GetProcesses().Any(p => p.ProcessName.Contains("chrome"));
                if (chromeProcess)
                {
                    chromeRunning = "YES";
                }

                if (GCMSSystem.Chrome.whichVer == 2)
                {
                    var BrowserFrm = Application.OpenForms["SignageBrowser"];
                    if (BrowserFrm == null)
                    {
                        chromeRunning = "NO";
                    }
                }

                if (chromeRunning == "YES")
                {
                    GCMSSystem.Chrome.Unload();
                    if (GCMSSystem.Chrome.whichVer == 1)
                    {
                        GCMSSystem.Chrome.UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear
                    }
                }

                // Bring Google Chrome Kiosk To ON TOP
                bool isSignageEnabled = GCMSSystem.Chrome.IsSignageEnabled();
                if (isSignageEnabled)
                {
                    if (GCMSSystem.Chrome.whichVer == 1 && !isDebug)
                    {
                        GCMSSystem.Chrome.Load();
                        var chromeProcessGUID = "- Google Chrome";
                        WindowHelper.BringToFront(chromeProcessGUID);
                    }

                    if (GCMSSystem.Chrome.whichVer == 2)
                    {
                        var BrowserFrm = Application.OpenForms["SignageBrowser"];
                        if (BrowserFrm == null)
                        {
                            GCMSSystem.Chrome.Load();
                        }
                    }

                    if (GCMSSystem.Chrome.whichVer == 4 && !isDebug)
                    {
                        GCMSSystem.Chrome.Load();
                        var chromeProcessGUID = "- Google Chrome";
                        WindowHelper.BringToFront(chromeProcessGUID);
                    }
                }

                if (snapCounter >= 3)
                {
                    GCMSSystem.TriggerSystem("RESTARTSIGN", true, false);
                    GCMSSystem.RemoteLog.Send(DateTime.Now.ToString("dd MMM HH:mm:ss"), "SNAPAUTORESTART");
                    MainForm.isSignageLoaded = false;
                    SignageBrowser.isSignageLoaded = false;
                    hardSNAP++;
                }

                if (hardSNAP == 5)
                {
                    // If the unit has been snapping for 15mins [5 x 3mins (60second loop)] without managing to fix itself, then to REBOOT player
                    GCMSSystem.TriggerSystem("REBOOT", true, false);
                }
            }

            if (actualResponse && (maintMode == "FALSE" || meLowPowerMode1 == "FALSE" || meLowPowerMode2 == "FALSE"))
            {
                snapCounter = 0;
            }
        }
        private void DataLoggingTimer_Tick(object sender, EventArgs e)
        {
            var fullstamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
            var MyIni = new IniFile(iniFile);
            var uuidStr = MyIni.Read("deviceUUID", "Monitor");
            var clientID = MyIni.Read("clientID", "Monitor");
            var deviceName = MyIni.Read("deviceName", "Monitor");
            var vpnRunning = "";
            var nodeRunning = "NO";
            var avaRunning = "NO";
            var node32Process = Process.GetProcesses().Any(p => p.ProcessName.Contains("node32"));
            if (node32Process)
            {
                nodeRunning = "YES";
            }
            var node64Process = Process.GetProcesses().Any(p => p.ProcessName.Contains("node64"));
            if (node64Process)
            {
                nodeRunning = "YES";
            }
            var avaProcess = Process.GetProcesses().Any(p => p.ProcessName.Contains("ava"));
            if (avaProcess)
            {
                avaRunning = "YES";
            }

            var MyNetwork = MyIni.Read("licType", "Licence");
            if (MyNetwork == "SEC")
            {
                // Due to SSL Issues over an IP Address - We Use IP Instead of Domain
                networkURL = "http://172.16.0.2";
                networkIP = "172.16.0.2";
                networkName = "INT";
                vpnRunning = "YES";
            }
            else
            {
                // Due to SSL Issues over an IP Address - We Use SSL for Domain
                networkURL = "https://api.globalcms.co.uk";
                networkIP = "api.globalcms.co.uk";
                networkName = "EXT";
                vpnRunning = "NO";
            }
            var pingTest = GCMSSystem.Ping(networkURL);
            if (!pingTest)
            {
                networkURL = "https://api.globalcms.co.uk";
                networkIP = "api.globalcms.co.uk";
                networkName = "EXT";
                vpnRunning = "NO";
            }
            else
            {
                networkURL = "http://172.16.0.2";
                networkIP = "172.16.0.2";
                networkName = "INT";
                vpnRunning = "YES";
            }
            if (NetworkOverride != "Auto" && NetworkOverride == "Public") { networkURL = "https://api.globalcms.co.uk"; networkIP = "api.globalcms.co.uk"; networkName = "Public"; networkNameShort = "P"; }
            if (NetworkOverride != "Auto" && NetworkOverride == "Secure") { networkURL = "http://172.16.0.2"; networkIP = "172.16.0.2"; networkName = "Secure"; networkNameShort = "S"; }

            var logStr = "";
            logStr += "[" + fullstamp + "]";
            logStr += "Connection:" + networkName + ",";
            logStr += "ClientID:" + clientID + ",";
            logStr += "DeviceName:" + deviceName + ",";
            logStr += "DeviceUUID:" + uuidStr + ",";
            logStr += "VPN:" + vpnRunning + ",";
            logStr += "Signage:" + nodeRunning + ",";
            logStr += "AVA:" + avaRunning + ",";
            logStr += "OHM:YES";

            GCMSSystem.DataLogger.Log(logStr);
        }
        private void LowTimer_Tick(object sender, EventArgs e)
        {
            var ramFree = Convert.ToDouble(GCMSSystem.GetRamFree());
            if (ramFree < 100000000)        // If Under 100MB
            {
                GCMSSystem.RemoteLog.Send(DateTime.Now.ToString("dd MMM HH:mm:ss"), "LOWRAM");
                GCMSSystem.Chrome.Reload();
            }

            var hddWMI = new ManagementObjectSearcher("SELECT * FROM Win32_logicaldisk").Get().Cast<ManagementObject>().First();
            var cDrive = Convert.ToDouble(hddWMI["FreeSpace"]);

            if (cDrive < 404717148)         // If Under 400MB
            {
                GCMSSystem.RemoteLog.Send(DateTime.Now.ToString("dd MMM HH:mm:ss"), "LOWHDD");
                // GCMSSystem.SystemCleaner.Run();          // Disabled currently due to SystemCleaner not working as expected
            }
        }
        private void CheckForInteractive_Tick(object sender, EventArgs e)   // See its inside a proper timer held inside the form that checks for if interactive is active or not
        {
            // Read INI File for Config.ini
            string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
            var MyIni = new IniFile(iniFile);
            // Setup which Network we should run over
            var MyNetwork = MyIni.Read("licType", "Licence");
            // Different Running Modes
            var EngineerMode = MyIni.Read("powersaveMode", "Network");               // Non RS232 Low Power Mode
            var LowPowerMode1 = MyIni.Read("powersaveMode2", "Network");             // RS232 Low Power Mode
            var LowPowerMode2 = MyIni.Read("maintMode", "Network");                  // Maintenance Mode
            if (EngineerMode == "TRUE" || LowPowerMode1 == "TRUE" || LowPowerMode2 == "TRUE") { return; }

            string responseString;
            try
            {
                // Get the details from the /interactive endpoint of the Digital Signage
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                try
                {
                    HttpWebRequest request = GCMSSystem.GetRequest("http://localhost:444/interactive");
                    WebResponse webResponse = request.GetResponse();
                    responseString = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                }
                catch
                {
                    responseString = "Error";
                }
            }
            catch
            {
                responseString = "Error";
            }

            if (isDebug) {
                // Debug.WriteLine(responseString);
            }

            if (responseString != "Error" && !isInteractive)
            {
                var data = (JObject)JsonConvert.DeserializeObject(responseString);
                bool interActive = data["active"].Value<bool>();
                if (isDebug)
                {
                    // Debug.WriteLine(interActive);
                }

                if (interActive)
                {
                    isInteractive = true;               // Tell the system that it is running an Interactive URL

                    GCMSSystem.InteractiveLog.Send("Start");

                    // JSON Entry for Time Started - When the user started the Interaction (UNIXTIME)
                    long interStart = data["timeStarted"].Value<long>();
                    int interDuration;
                    try
                    {
                        interDuration = data["duration"].Value<int>();
                    }
                    catch
                    {
                        interDuration = 60;                         // If for whatever reason there is no Duration - Default back to 60 Seconds (1min)
                    }

                    string interRunCountDown;
                    if (interDuration > 60)
                    {
                        interRunCountDown = (interDuration / 60) + " Min";                  // Tag for the Running Mode
                    }
                    else
                    {
                        interRunCountDown = interDuration + " Sec";                         // Tag for the Running Mode
                    }

                    if (isDebug)
                    {
                        // Debug.WriteLine("Interactive Mode Activate Detected");
                    }

                    Interaction.Tick += new EventHandler(Interaction_Tick);
                    Interaction.Interval = (int)TimeSpan.FromSeconds(interDuration).TotalMilliseconds;
                    Interaction.Enabled = true;

                    powerModeLabel.Text = "Interactive";
                    powerModeLabel.ForeColor = Color.FromArgb(138, 43, 226);

                    CheckForInteractive.Stop();
                }
            }
        }
        private void CheckForNewSignage_Tick(object sender, EventArgs e)
        {
            if (!isUselessInternet)
            {
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                var MyNetwork = MyIni.Read("licType", "Licence");
                if (MyNetwork == "SEC")
                {
                    // Due to SSL Issues over an IP Address - We Use IP Instead of Domain
                    networkURL = "http://172.16.0.2";
                    networkIP = "172.16.0.2";
                    networkName = "INT";
                }
                else
                {
                    // Due to SSL Issues over an IP Address - We Use SSL for Domain
                    networkURL = "https://api.globalcms.co.uk";
                    networkIP = "api.globalcms.co.uk";
                    networkName = "EXT";
                }
                var pingTest = GCMSSystem.Ping(networkURL);
                if (!pingTest)
                {
                    networkURL = "https://api.globalcms.co.uk";
                    networkIP = "api.globalcms.co.uk";
                    networkName = "EXT";
                }
                else
                {
                    networkURL = "http://172.16.0.2";
                    networkIP = "172.16.0.2";
                    networkName = "INT";
                }
                if (NetworkOverride != "Auto" && NetworkOverride == "Public") { networkURL = "https://api.globalcms.co.uk"; networkIP = "api.globalcms.co.uk"; networkName = "Public"; networkNameShort = "P"; }
                if (NetworkOverride != "Auto" && NetworkOverride == "Secure") { networkURL = "http://172.16.0.2"; networkIP = "172.16.0.2"; networkName = "Secure"; networkNameShort = "S"; }

                string remoteVersion;
                string localVersion;

                // Get the Local Version String
                using (StreamReader streamReader = new StreamReader(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "version.txt"), Encoding.UTF8))
                {
                    localVersion = streamReader.ReadToEnd();
                }

                // Get the Remote Version String from Server
                try
                {
                    System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                    try
                    {
                        HttpWebRequest request = GCMSSystem.GetRequest(networkURL + "/v2/getVersionSign.php");
                        WebResponse webResponse = request.GetResponse();
                        remoteVersion = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                    }
                    catch
                    {
                        remoteVersion = "Error";
                    }
                }
                catch
                {
                    remoteVersion = "Error";
                }

                // Debug.WriteLine("Remote Version: " + remoteVersion);
                // Debug.WriteLine("Local Version: " + localVersion);
                var signFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage");

                // Backup Checker to make sure that everything that should be in the Folder is in the Signage Folder
                if (!Directory.Exists(signFolder))
                {
                    bool zipDownloaded = false;
                    GCMSSystem.FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [ERR] Detected Issue with Signage Folder - Redownload Signage Version");
                    try
                    {
                        GCMSSystem.DownloadFileSingle(networkURL + "/v2/signageUpdate/signage.zip", "signage.zip");
                    }
                    catch { }

                    // Unzip Signage.zip to the Signage Folder inside monitor main root DIR
                    var signageZipFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage.zip";
                    var signageZipFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    try
                    {
                        System.IO.Compression.ZipFile.ExtractToDirectory(signageZipFile, signageZipFolder);
                        zipDownloaded = true;
                    }
                    catch { }

                    if (zipDownloaded)
                    {
                        // We need to wait to allow the system to finish extracting the file
                        System.Threading.Thread.Sleep(10000);                                                                    // Wait for 10 seconds

                        // For some reaon the last 2 files dont seem to extract form the zip so lets place a backup plan in place for that
                        var websocketJS = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\websocket.js";
                        if (!File.Exists(websocketJS))
                        {
                            GCMSSystem.DownloadFileSingle(networkURL + "/v2/signageUpdate/websocket.js", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\websocket.js");
                        }
                        var WindowsMessageRelayEXE = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\WindowsMessageRelay.exe";
                        if (!File.Exists(WindowsMessageRelayEXE))
                        {
                            GCMSSystem.DownloadFileSingle(networkURL + "/v2/signageUpdate/WindowsMessageRelay.exe", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\WindowsMessageRelay.exe");
                        }

                        // Move all content from temp folder back to the signage\public\content
                        try
                        {
                            File.Delete(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage.zip");
                        }
                        catch { }


                        // Once this has fixed itself, we need to clear the signage files so that it triggers the content to re-download to the machine
                        EngineerTools.ClearSignageFiles(false);
                        GCMSSystem.Chrome.UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear

                        // Now we just have to wait for the Checker to recheck and see that the signage isnt running and then to start it all back up
                        GCMSSystem.FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [AUTOFIX] Repaired Signage");
                    }
                }

                // If Local Version is not the Same as Remote Version then its either corrupt or out of date
                if (localVersion != remoteVersion && remoteVersion != "Error")
                {
                    GCMSSystem.FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Update Signage");

                    var chromeRunning = "NO";
                    var nodeRunning = "NO";

                    // Shutdown Google Chrome and NodeJS so we can update the Signage
                    var chromeProcess = Process.GetProcesses().Any(p => p.ProcessName.Contains("chrome"));
                    if (chromeProcess)
                    {
                        chromeRunning = "YES";
                    }
                    var node32Process = Process.GetProcesses().Any(p => p.ProcessName.Contains("node32"));
                    if (node32Process)
                    {
                        nodeRunning = "YES";
                    }
                    var node64Process = Process.GetProcesses().Any(p => p.ProcessName.Contains("node64"));
                    if (node64Process)
                    {
                        nodeRunning = "YES";
                    }

                    if (chromeRunning == "YES")
                    {
                        // Chrome is running - probably due to someone opening it while in Maintenance Mode
                        GCMSSystem.Chrome.Unload();
                        GCMSSystem.Chrome.UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear
                    }

                    if (nodeRunning == "YES")
                    {
                        // Node32 is running - probably due to someone opening it while in Maintenance Mode
                        foreach (var process in Process.GetProcessesByName("node32"))
                        {
                            try
                            {
                                GCMSSystem.KillProcessAndChildrens(process.Id); // Get PID of node32, and kill 'it' as well as any other children it was spawned, on top of the killchildren line above
                                process.StartInfo.Verb = "runas";
                                process.Kill();
                                process.WaitForExit(60000);
                            }
                            catch (Exception)
                            {

                            }
                        }
                        // Node64 is running - probably due to someone opening it while in Maintenance Mode
                        foreach (var process in Process.GetProcessesByName("node64"))
                        {
                            try
                            {
                                GCMSSystem.KillProcessAndChildrens(process.Id); // Get PID of node64, and kill 'it' as well as any other children it was spawned, on top of the killchildren line above
                                process.StartInfo.Verb = "runas";
                                process.Kill();
                                process.WaitForExit(60000);
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }

                    // Move all content from signage\public\content to the temp folder
                    var sourceDIR = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\public\\content";
                    var destDIR = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage_tmp";

                    // Shutdown Timers associated with Signage and the NodeJS
                    CheckServicesTimer.Stop();
                    CheckSNAP.Stop();

                    if (MyNetwork == "SEC")
                    {
                        // Due to SSL Issues over an IP Address - We Use IP Instead of Domain
                        networkURL = "http://172.16.0.2";
                        networkIP = "172.16.0.2";
                        networkName = "Secure";
                    }
                    else
                    {
                        // Due to SSL Issues over an IP Address - We Use SSL for Domain
                        networkURL = "https://api.globalcms.co.uk";
                        networkIP = "api.globalcms.co.uk";
                        networkName = "Public";
                    }
                    var pingTest2 = GCMSSystem.Ping(networkURL);
                    if (!pingTest2)
                    {
                        networkURL = "https://api.globalcms.co.uk";
                        networkIP = "api.globalcms.co.uk";
                        networkName = "Public";
                    }
                    else
                    {
                        networkURL = "http://172.16.0.2";
                        networkIP = "172.16.0.2";
                        networkName = "Secure";
                    }
                    if (NetworkOverride != "Auto" && NetworkOverride == "Public") { networkURL = "https://api.globalcms.co.uk"; networkIP = "api.globalcms.co.uk"; networkName = "Public"; networkNameShort = "P"; }
                    if (NetworkOverride != "Auto" && NetworkOverride == "Secure") { networkURL = "http://172.16.0.2"; networkIP = "172.16.0.2"; networkName = "Secure"; networkNameShort = "S"; }

                    var finalPing = GCMSSystem.Ping(networkURL);
                    if (finalPing)
                    {
                        // Download latest Signage.zip from API Server
                        bool downloadSignageComplete = false;
                        try
                        {
                            GCMSSystem.DownloadFileSingle(networkURL + "/v2/signageUpdate/signage.zip", "signage.zip");
                            downloadSignageComplete = true;
                        }
                        catch { }

                        if (downloadSignageComplete)
                        {
                            // Move content folder to temp folder inside of monitor main root DIR
                            try
                            {
                                Directory.Move(sourceDIR, destDIR);
                            }
                            catch { }

                            // Delete Whole Signage Folder
                            try
                            {
                                Directory.Delete(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage", true);
                            }
                            catch { }

                            // Unzip Signage.zip to the Signage Folder inside monitor main root DIR
                            var signageZipFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage.zip";
                            var signageZipFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                            try
                            {
                                System.IO.Compression.ZipFile.ExtractToDirectory(signageZipFile, signageZipFolder);
                            }
                            catch { }

                            // We need to wait to allow the system to finish extracting the file
                            System.Threading.Thread.Sleep(10000);                                                                    // Wait for 10 seconds

                            // For some reaon the last 2 files dont seem to extract form the zip so lets place a backup plan in place for that
                            var websocketJS = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\websocket.js";
                            if (!File.Exists(websocketJS))
                            {
                                GCMSSystem.DownloadFileSingle(networkURL + "/v2/signageUpdate/websocket.js", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\websocket.js");
                            }
                            var WindowsMessageRelayEXE = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\WindowsMessageRelay.exe";
                            if (!File.Exists(WindowsMessageRelayEXE))
                            {
                                GCMSSystem.DownloadFileSingle(networkURL + "/v2/signageUpdate/WindowsMessageRelay.exe", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\WindowsMessageRelay.exe");
                            }

                            // Move all content from temp folder back to the signage\public\content
                            try
                            {
                                Directory.Delete(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\public\\content", true);
                                Directory.Move(destDIR, sourceDIR);
                                File.Delete(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage.zip");
                            }
                            catch { }
                        }
                    }
                    // Restart NodeJS and then Chrome
                    var chromeEXE = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\Google\\Chrome\\Application\\chrome.exe";
                    var nodeEXE = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\node32.exe";
                    if (osArch == "x64")
                    {
                        nodeEXE = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\node64.exe";
                    }

                    var browserSSL = MyIni.Read("SSL", "Browser");
                    var sslon = "false";
                    if (browserSSL == "On")
                    {
                        sslon = "true";
                    }
                    string signageConfFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "settings.conf");
                    var SettingsIni = new IniFile(signageConfFile);
                    SettingsIni.Write("sslOn", sslon, "core");

                    Process nodeJS = new Process();
                    nodeJS.StartInfo.FileName = nodeEXE;
                    //nodeJS.StartInfo.Arguments = Directory.GetCurrentDirectory() + "\\signage\\signage.js " + Directory.GetCurrentDirectory() + "\\signage\\settings.conf";
                    nodeJS.StartInfo.Arguments = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "signage.js") + " " + Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "settings.conf");
                    nodeJS.StartInfo.Verb = "runas";
                    nodeJS.StartInfo.UseShellExecute = false;
                    nodeJS.StartInfo.EnvironmentVariables["LOCALAPPDATA"] = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    nodeJS.StartInfo.RedirectStandardOutput = false;
                    nodeJS.StartInfo.RedirectStandardError = false;
                    nodeJS.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    // nodeJS.Start();
                    var sh = (Shell32.IShellDispatch4)Activator.CreateInstance(
                           Type.GetTypeFromProgID("Shell.Application"));
                    sh.ShellExecute(nodeEXE, Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "signage.js") + " " + Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "settings.conf"), null, null, 0);

                    GCMSSystem.Chrome.UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear
                    GCMSSystem.Chrome.Load();

                    CheckServicesTimer.Start();
                    bool isSignageEnabled = GCMSSystem.Chrome.IsSignageEnabled();
                    if (isSignageEnabled)
                    {
                        var meLowPowerMode1 = MyIni.Read("powersaveMode", "Network");               // Non RS232 Low Power Mode
                        var meLowPowerMode2 = MyIni.Read("powersaveMode2", "Network");              // RS232 Low Power Mode
                        var meMaintMode = MyIni.Read("maintMode", "Network");                       // Maintenance Mode

                        if (meLowPowerMode1 == "FALSE" || meLowPowerMode2 == "FALSE")
                        {
                            CheckSNAP.Start();
                        }

                    }
                }
            }
            GC.Collect(GC.MaxGeneration);           // Once an hr run the Garabage Collector Service to clear down tmp files
        }
        private void TaskbarAbout_Click(object sender, EventArgs e)
        {
            Form frm2 = new About();
            frm2.Show();
        }
        private void TaskbarShutdownService_Click(object sender, EventArgs e)
        {
            CheckStatsTimer.Enabled = false;
            CheckStatsTimer.Stop();
            CallHomeTimer.Enabled = false;
            CallHomeTimer.Stop();
            CheckServicesTimer.Enabled = false;
            CheckServicesTimer.Stop();
            CheckSNAP.Enabled = false;
            CheckSNAP.Stop();

            // Chrome is currently running, so we need to shut down the application
            GCMSSystem.Chrome.Unload();
            GCMSSystem.Chrome.UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear

            // By default Hide all of the Forms
            List<Form> openForms = new List<Form>();
            foreach (Form f in Application.OpenForms)
            {
                openForms.Add(f);
            }
            foreach (Form f in openForms)
            {
                if (f.Name == "MainForm")
                {
                    f.Hide();
                }
                if (f.Name != "MainForm")
                {
                    f.Close();
                }
            }

            // Create the Dynamic Timer
            Timer tmr;
            tmr = new Timer();
            tmr.Tick += delegate {
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var lockFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "decom.lock"); 

                // Read INI File for Config.ini
                var MyIni = new IniFile(iniFile);
                // Setup which Network we should run over
                var MyNetwork = MyIni.Read("licType", "Licence");
                // Different Running Modes
                var meLowPowerMode1 = MyIni.Read("powersaveMode", "Network");               // Non RS232 Low Power Mode
                var meLowPowerMode2 = MyIni.Read("powersaveMode2", "Network");              // RS232 Low Power Mode
                var meMaintMode = MyIni.Read("maintMode", "Network");                       // Maintenance Mode

                foreach (Form f in openForms)
                {
                    if (f.Name == "MainForm")
                    {
                        f.Show();
                    }
                }

                if (!File.Exists(iniFile) && !File.Exists(lockFile))
                {
                    Form frm3 = new Commission();
                    frm3.Show();
                }

                if (File.Exists(lockFile)) 
                {
                    Form frm4 = new Decommission();
                    frm4.Show();
                }

                if (meMaintMode == "TRUE")
                {
                    if (!GCMSSystem.CheckOpened("EngineeringTools"))
                    {
                        var toolForm = new EngineerTools();
                        toolForm.Show();
                    }
                }
                else
                {
                    GCMSSystem.Chrome.UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear
                    GCMSSystem.Chrome.Load();
                }

                CheckStatsTimer.Enabled = true;
                CheckStatsTimer.Start();
                CallHomeTimer.Enabled = true;
                CallHomeTimer.Start();
                CheckServicesTimer.Enabled = true;
                CheckServicesTimer.Start();
                CheckSNAP.Enabled = true;
                CheckSNAP.Start();

                tmr.Stop();
            };

            // How Long do we want to run the Timer for
            tmr.Interval = (int)TimeSpan.FromMinutes(30).TotalMilliseconds;

            // Start the Timer
            tmr.Start();
        }
        public static void RestartSignageForMosaic()
        {
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

                    var sh = (Shell32.IShellDispatch4)Activator.CreateInstance(
                            Type.GetTypeFromProgID("Shell.Application"));
                    sh.ShellExecute(nodeEXE, Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "signage.js") + " " + Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "settings.conf"), null, null, 0);
                }

                // Shutdown any potential apps that could be running
                foreach (var dxProcess in Process.GetProcessesByName("dxdiag"))
                {
                    try
                    {
                        dxProcess.StartInfo.Verb = "runas";
                        dxProcess.Kill();
                        dxProcess.WaitForExit(5000);
                    }
                    catch (Exception)
                    {

                    }
                }
                foreach (var cleanerProcess in Process.GetProcessesByName("cleanmgr"))
                {
                    try
                    {
                        cleanerProcess.StartInfo.Verb = "runas";
                        cleanerProcess.Kill();
                        cleanerProcess.WaitForExit(5000);
                    }
                    catch (Exception)
                    {

                    }
                }

                if (GCMSSystem.Chrome.whichVer == 1 && !MainForm.isDebug)
                {
                    GCMSSystem.Chrome.Unload();
                    GCMSSystem.Chrome.UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear
                    GCMSSystem.Chrome.Load();
                }
                if (GCMSSystem.Chrome.whichVer == 2)
                {
                    GCMSSystem.Chrome.Unload();
                    GCMSSystem.Chrome.Load();
                }
                isSignageLoaded = true;
            }
        }
        private void TestBrowserBTN_Click(object sender, EventArgs e)
        {
            if (System.Windows.Forms.Application.OpenForms["SignageBrowser"] == null)
            {
                Form SignageBrowser = new SignageBrowser();
                SignageBrowser.Show();
            }
        }
        private void ComPortsBTN_Click(object sender, EventArgs e)
        {
            if (!GCMSSystem.CheckOpened("ComDebug"))
            {
                Form ComDebug = new ComDebug();
                ComDebug.Show();
            }
        }
        private void LogoBottomCorner_Click(object sender, EventArgs e)
        {
            if (!GCMSSystem.CheckOpened("About"))
            {
                Form aboutForm = new About();
                aboutForm.Show();
            }
            else
            {
                About.FrmObj.BringToFront();
                About.FrmObj.Focus();
            }
        }
        private void NEXLabel_Click(object sender, EventArgs e)
        {
            if (!GCMSSystem.CheckOpened("Nexmosphere") && nexmoshpereSensors)
            {
                Form Nexmosphere = new Nexmosphere();
                Nexmosphere.Show();
            }
        }

        private void USBSensorDebug_Click(object sender, EventArgs e)
        {
            if (!GCMSSystem.CheckOpened("OmronSensor") && SensorOpt.Text != "Disconnected" && SensorOpt.Text != "Not Available") {
                Form OmronDebug = new OmronSensor();
                OmronDebug.Show();
            }
        }

        private void RS232Picture_Click(object sender, EventArgs e)
        {
            if (!GCMSSystem.CheckOpened("RS232"))
            {
                Form Rs232 = new Rs232();
                Rs232.Show();
            }
        }

        private void LauncherTimer_Tick(object sender, EventArgs e)
        {
            string appIniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "launcher.ini");

            const int BufferSize = 128;
            using (var fileStream = File.OpenRead(appIniFile))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    string[] separatingChars = { "?" };
                    string[] lineData = line.Split(separatingChars, StringSplitOptions.RemoveEmptyEntries);
                    var isApplicationEnabled = lineData[0];
                    var whatApplicationName = lineData[1];
                    var whatApplicationMonitor = lineData[2];

                    // Debug.WriteLine("Is Enabled? : " + isApplicationEnabled);
                    // Debug.WriteLine("Application : " + whatApplicationName);
                    // Debug.WriteLine("Monitor? : " + whatApplicationMonitor);

                    bool isAppRunning = false;
                    try
                    {
                        if (osArch == "x64")
                        {
                            isAppRunning = GCMSSystem.ProgramIsRunning(@whatApplicationMonitor);
                        }
                    }
                    catch { }
                    // Debug.WriteLine("Launch App Running? " + whatApplicationName + " : " + whatApplicationMonitor + " : " + isAppRunning);
                    if (!isAppRunning)
                    {
                        if (isApplicationEnabled == "Enabled")
                        {
                            if (isDebug)
                            {
                                Debug.WriteLine("Launching : " + whatApplicationName);
                            }
                            try
                            {
                                Process appLauncher = new Process();
                                appLauncher.StartInfo.FileName = whatApplicationName;
                                appLauncher.StartInfo.Verb = "runas";
                                appLauncher.Start();
                            }
                            catch { }
                        }
                    }
                }
            }
        }

        private void CheckTrial_Tick(object sender, EventArgs e)
        {
            // Checker to see if this program is running in "Trail Mode"
            var trialFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "trial.lock");
            var trialRemaining = "0";
            if (File.Exists(trialFile) && isTrial)
            {
                isTrial = true;
                DateTime trailCreation = File.GetCreationTime(trialFile);
                DateTime trailModification = File.GetLastWriteTime(trialFile);

                int trailDayLimit = 30;

                trialStart = GCMSSystem.UnixTime.Convert(trailModification);
                DateTime trialEndDate = trailCreation.AddDays(trailDayLimit);
                trialEnd = GCMSSystem.UnixTime.Convert(trialEndDate);

                DateTime today = DateTime.Today;

                var totalDays = (today - trailModification).TotalDays;
                trialRemaining = Math.Round((trailDayLimit - totalDays)).ToString();
                if (trialRemaining == "1")
                {
                    TrialLicTxt.Text = "TRIAL LICENCE (Expires in " + trialRemaining + " day)";
                }
                else
                {
                    TrialLicTxt.Text = "TRIAL LICENCE (Expires in " + trialRemaining + " days)";
                }

                if (totalDays > trailDayLimit)
                {
                    CheckStatsTimer.Enabled = false;
                    CheckStatsTimer.Stop();
                    CallHomeTimer.Enabled = false;
                    CallHomeTimer.Stop();
                    CheckServicesTimer.Enabled = false;
                    CheckServicesTimer.Stop();
                    CheckSNAP.Enabled = false;
                    CheckSNAP.Stop();
                    LauncherTimer.Enabled = false;
                    LauncherTimer.Stop();
                    // Make Main Window Disabled and Minimized
                    Visible = false;
                    Enabled = false;
                    WindowState = FormWindowState.Minimized;
                    // Open the Commissioning Form
                    var decomForm = new Decommission();
                    decomForm.Show();
                    return;
                }
            }
            else
            {
                isTrial = false;
            }
        }
    }
}