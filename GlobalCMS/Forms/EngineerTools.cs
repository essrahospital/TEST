using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shell32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace GlobalCMS
{
    public partial class EngineerTools : Form
    {
        string lastRFID;
        string networkURL;                                                // Which Network URL to use 
        string networkNameShort;                                          // Network Name Short - A Single Letter P (Public) / S (Secure)

        string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
        string powerFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "powerConfig.ini");
        string signageIniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "settings.conf");
        public static bool nexmoshpereSensors = false;

        public EngineerTools()
        {
            MainForm.isSignageLoaded = false;

            if (!MainForm.isDebug)
            {
                if (MainForm.hardenedShell)
                {   
                    try
                    {
                        Taskbar.Show();
                    }
                    catch { } 

                    // Restart explorer.exe to fully unlock the explorer.exe options
                    try
                    {
                        string explorer = string.Format("{0}\\{1}", Environment.GetEnvironmentVariable("WINDIR"), "explorer.exe");
                        Process process = new Process();
                        process.StartInfo.FileName = explorer;
                        process.StartInfo.UseShellExecute = true;
                        process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                        process.Start();
                    }
                    catch { }
                }
            }
            InitializeComponent();
            var serviceRunning = GCMSSystem.WinService.CheckRunning("GlobalCMS");
            ServiceRunningTxt.Text = serviceRunning;
            if (serviceRunning == "Stopped") { ShutdownServiceBTN.Enabled = false; ServiceRunningTxt.ForeColor = Color.FromArgb(192, 0, 0); }
            if (serviceRunning == "Running") { ShutdownServiceBTN.Enabled = true; ServiceRunningTxt.ForeColor = Color.FromArgb(0, 192, 0); }

            IniFile MyIni = new IniFile(iniFile);
            var MySkin = MyIni.Read("SkinID", "Skin");
            try { Themes.Generate(MySkin); } catch { }

            try
            {
                bool nexmosphereBox = SerialPort.GetPortNames().Any(x => x == "COM12");
                if (nexmosphereBox) { nexmoshpereSensors = true; RestartNexusBTN.Enabled = true; } else { nexmoshpereSensors = false; RestartNexusBTN.Enabled = false; }
            }
            catch { nexmoshpereSensors = false; RestartNexusBTN.Enabled = false; }
            System.Windows.Forms.Cursor.Show();
            MainForm.isSignageLoaded = false;
            SignageBrowser.isSignageLoaded = false;
        }

        static readonly string[] SizeSuffixes = { "bytes", "KBPS", "MBPS", "GBPS", "TBPS", "PBPS", "EBPS", "ZBPS", "YBPS" };

        private void EngineerTools_Load(object sender, EventArgs e)
        {
            IniFile MyIni = new IniFile(iniFile);
            IniFile PowerIni = new IniFile(powerFile);
            IniFile SignageIni = new IniFile(signageIniFile);

            GCMSSystem.NodeSocket.Send("endinteractive");
            bool isDebug = MainForm.isDebug;
            // This is to make sure that on loading of the Maint Tools Form, that it will 100% shut down Google Chrome
            if (!isDebug)
            {
                GCMSSystem.Chrome.Unload();
                GCMSSystem.Chrome.UpdatePref();
            }

            FrmObj = this;

            // Bring to Front and Focus
            BringToFront();
            Focus();
            Activate();

            var machineOS = GCMSSystem.GetOS();
            // Debug.WriteLine("Machine OS : " + machineOS);

            if (machineOS.StartsWith("Windows"))
            {
                // Set all dropdown values to their Defaults
                // Hardware
                UpdaterNetwork.SelectedIndex = 0;
                LowInternetMode.SelectedIndex = 1;
                LowInternetModeDelay.SelectedIndex = 0;
                ScalingDropdown.SelectedIndex = 0;
                ResolutionDropdown.SelectedIndex = 2;
                DesktopDropdown.SelectedIndex = 0;

                PrinterSettingsPrintPreview.SelectedIndex = 0;
                PrinterSettingsInLandscape.SelectedIndex = 0;
                PrinterSettingsInColor.SelectedIndex = 0;
                PrinterSettingsPaperSize.SelectedIndex = 0;
                PrinterSettingsMargin.SelectedIndex = 9;
                PrinterSettingsCopies.SelectedIndex = 0;

                // Signage
                SignageLoader.SelectedIndex = 0;
                DebugMode.SelectedIndex = 1;
                SSLMode.SelectedIndex = 1;
                SSLMode.Enabled = false;                // Temp Until SSL is patched and fixed
                GCMSKeyboard.SelectedIndex = 0;
                // Tests
                InteractiveTestChoice.SelectedIndex = 0;
                InteractiveTestServer.SelectedIndex = 0;

                /*
                if (machineOS == "Windows7" || machineOS == "Windows8" || machineOS == "Windows10")
                {
                    if (GCMSSystem.AirServer.IsInstaled())
                    {
                        AirServerInstallBTN.Enabled = false;
                        if (GCMSSystem.AirServer.IsRunning())
                        {
                            AirServerUninstallBTN.Enabled = false;
                            AirServerStopBTN.Enabled = true;
                            AirServerPasscode.Text = "Code: " + GCMSSystem.AirServer.Passcode();
                            AirServerInstallStatus.Text = "RUNNING";
                            AirServerInstallStatus.ForeColor = Color.FromArgb(0, 192, 0);
                            AirServerChgPassBTN.Enabled = false;
                            AirServerStartBTN.Enabled = false;
                        }
                        else
                        {
                            AirServerUninstallBTN.Enabled = true;
                            AirServerStopBTN.Enabled = false;
                            AirServerPasscode.Text = "Code: " + GCMSSystem.AirServer.Passcode();
                            AirServerInstallStatus.Text = "INSTALLED";
                            AirServerInstallStatus.ForeColor = Color.FromArgb(0, 192, 0);
                            AirServerChgPassBTN.Enabled = true;
                            AirServerStartBTN.Enabled = true;
                        }
                    }
                    else
                    {
                        AirServerInstallBTN.Enabled = true;
                        AirServerUninstallBTN.Enabled = false;
                        AirServerStartBTN.Enabled = false;
                        AirServerStopBTN.Enabled = false;
                        AirServerChgPassBTN.Enabled = false;
                        AirServerPasscode.Text = "";
                        AirServerInstallStatus.Text = "NOT INSTALLED";
                        AirServerInstallStatus.ForeColor = Color.FromArgb(192, 0, 0);
                    }
                }
                */

                var hddCWMI = new ManagementObjectSearcher("SELECT * FROM Win32_logicaldisk").Get().Cast<ManagementObject>().First();
                var hddCSize = Convert.ToDouble(hddCWMI["Size"]);
                var hddCFree = Convert.ToDouble(hddCWMI["FreeSpace"]);
                var hddCInUse = hddCSize - hddCFree;

                double device_hdd_c_percent = 0;
                double device_hdd_d_percent = 0;

                device_hdd_c_percent = Math.Round((hddCInUse / hddCSize) * 100, 0);
                GenConfigListView(MyIni);
                GenPowerConfigView(PowerIni);

                LicTypeLabel.Text = MyIni.Read("licType", "Licence");
                DeviceIDLabel.Text = MyIni.Read("deviceName", "Monitor");
                DeviceMACLabel.Text = MyIni.Read("hardwareMAC", "Monitor");
                DeviceUUIDLabel.Text = MyIni.Read("deviceUUID", "Monitor");

                string vpnIP_check = GCMSSystem.GetIP("VPN");             // Global Variable for VPN IP
                string lanIP_check = GCMSSystem.GetIP("LAN");             // Global Variable for LAN IP
                string wanIP_check = GCMSSystem.GetIP("WAN");             // Global Variable for WAN IP
                DetectNetworkAdapter();                                 // Function to detect which network interface is being used
                devLAN.Text = lanIP_check;
                devVPN.Text = vpnIP_check;
                devWAN.Text = wanIP_check;

                bool DriveD = GCMSSystem.DriveExists("D:\\");
                // If D:\ Exists & is a Disk Drive NOT CD-ROM
                var device_hdd_d = "";
                if (DriveD)
                {
                    var hddDWMI = new ManagementObjectSearcher("SELECT * FROM Win32_logicaldisk WHERE driveType = \"3\" AND name = \"D:\" ").Get().Cast<ManagementObject>().Last();

                    var hddDSize = Convert.ToDouble(hddDWMI["Size"]);
                    var hddDFree = Convert.ToDouble(hddDWMI["FreeSpace"]);
                    var hddDInUse = hddDSize - hddDFree;
                    device_hdd_d_percent = Math.Round((hddDInUse / hddDSize) * 100, 0);

                    device_hdd_d = "Drive " + (string)hddDWMI["DeviceID"] + " ";
                    device_hdd_d += Convert.ToString(Math.Round(Convert.ToDouble(hddDWMI["Size"]) / 1048576000, 0)) + " GB (" + device_hdd_d_percent.ToString() + "% Full)";
                }

                var device_hdd_c = "Drive " + (string)hddCWMI["DeviceID"] + " ";
                device_hdd_c += Convert.ToString(Math.Round(Convert.ToDouble(hddCWMI["Size"]) / 1048576000, 0)) + " GB (" + device_hdd_c_percent.ToString() + "% Full)";
                if (DriveD)
                {
                    device_hdd_c += "     ";
                }
                hddAmounts.Text = device_hdd_c + device_hdd_d;

                var signageEnabled = MyIni.Read("Signage", "Serv");
                var autoCookieCleaner = MyIni.Read("CookieCleaner", "Interactive");
                var signageLoader = MyIni.Read("SignageLoader", "Sign");
                if (autoCookieCleaner == "")
                {
                    // If the Value for CookieCleaner doesnt exist then to backfill with it being "Off"
                    MyIni.Write("CookieCleaner", "Off", "Interactive");
                    autoCookieCleaner = "Off";
                }

                if (signageEnabled == "Enabled")
                {
                    DisableSignageTriggerBTN.Text = "Disable Signage Loading";
                }
                else
                {
                    DisableSignageTriggerBTN.Text = "Enable Signage Loading";
                }

                if (autoCookieCleaner == "On")
                {
                    AutoCookieCleanerBTN.Text = "Disable Auto Cookie Clearer";
                }
                else
                {
                    AutoCookieCleanerBTN.Text = "Enable Auto Cookie Clearer";
                }

                if (signageLoader == "1")       // Loader #1 - Google Chrome (Must be Version 71)
                {
                    SignageLoader.Text = "Chrome";
                }
                if (signageLoader == "2")       // Loader #2 - GlobalCMS (EO)
                {
                    SignageLoader.Text = "Core";
                }

                var ForceNetwork = MyIni.Read("UpdateNetwork", "Monitor");
                if (ForceNetwork == "")
                {
                    // If the Value for Referer doesnt exist then to backfill with it being "Auto"
                    MyIni.Write("UpdateNetwork", "Auto", "Monitor");
                    ForceNetwork = "Auto";
                }
                UpdaterNetwork.SelectedIndex = UpdaterNetwork.FindStringExact(ForceNetwork);           // Set the Dropdown to Value in ini

                var poorInternetConnection = MyIni.Read("poorInternet", "Monitor");
                if (poorInternetConnection == "")
                {
                    // If the Value for poorInternet doesnt exist then to backfill with it being "Disabled"
                    MyIni.Write("poorInternet", "Disabled", "Monitor");             // Disabled
                    poorInternetConnection = "Disabled";
                }
                var poorInternetDelay = MyIni.Read("poorInternetDelay", "Monitor");
                if (poorInternetDelay == "")
                {
                    // If the Value for poorInternet doesnt exist then to backfill with it being "Disabled"
                    MyIni.Write("poorInternetDelay", "0", "Monitor");             // Disabled
                }
                LowInternetMode.SelectedIndex = LowInternetMode.FindStringExact(poorInternetConnection);           // Set the Dropdown to Value in ini
                LowInternetModeDelay.SelectedIndex = LowInternetModeDelay.FindStringExact(poorInternetDelay);      // Set the Dropdown to Value in ini

                if (poorInternetConnection == "Disabled") { MainForm.isUselessInternet = false; }
                if (poorInternetConnection == "Enabled") { MainForm.isUselessInternet = true; }

                var ShellSecurity = MyIni.Read("ShellSecurity", "Monitor");
                if (ShellSecurity == "")
                {
                    // If the Value for Referer doesnt exist then to backfill with it being ""
                    MyIni.Write("ShellSecurity", "On", "Monitor");
                    ShellSecurity = "On";
                    MainForm.hardenedShell = true;
                }
                if (ShellSecurity == "On") { UnlockUnitSecurityBTN.Text = "Disable Shell Security"; MainForm.hardenedShell = true; }
                if (ShellSecurity == "Off") { UnlockUnitSecurityBTN.Text = "Enable Shell Security"; MainForm.hardenedShell = false; }

                // Force Graphics Overrides
                var gfxForce = MyIni.Read("Force", "Display");
                if (gfxForce == "")
                {
                    // If the Value for Force doesnt exist then to backfill with it being "Off"
                    MyIni.Write("Force", "Off", "Display");
                    gfxForce = "Off";
                }
                if (gfxForce == "On")
                {
                    gfxForceOn.Checked = true;
                    gfxForceOff.Checked = false;
                }
                else
                {
                    gfxForceOn.Checked = false;
                    gfxForceOff.Checked = true;
                }

                // Force - GFX Resolution
                var gfxRes = MyIni.Read("Resolution", "Display");
                if (gfxRes == "")
                {
                    // If the Value for Force doesnt exist then to backfill with it being "Off"
                    MyIni.Write("Resolution", "1920x1080", "Display");
                    gfxRes = "1920x1080";
                }

                string[] resolution = gfxRes.Split('x');            // Explode as the Strings contain a space between the "x"
                ResolutionDropdown.SelectedIndex = ResolutionDropdown.FindStringExact(resolution[0] + " x " + resolution[1]);           // Set the Dropdown to Value in ini

                // Force - GFX Scaling
                var gfxScale = MyIni.Read("Scaling", "Display");
                if (gfxScale == "")
                {
                    // If the Value for Force doesnt exist then to backfill with it being "Off"
                    MyIni.Write("Scaling", "100", "Display");
                    gfxScale = "100";
                }
                ScalingDropdown.SelectedIndex = ScalingDropdown.FindStringExact(gfxScale + "%");           // Set the Dropdown to Value in ini

                // Force - GFX Orientation
                var gfxOrientation = MyIni.Read("Orientation", "Display");
                if (gfxOrientation == "")
                {
                    // If the Value for Force doesnt exist then to backfill with it being "Off"
                    MyIni.Write("Orientation", "Landscape", "Display");
                    gfxOrientation = "Landscape";
                }

                if (gfxOrientation == "Multiple")
                {
                    MultiscreenBTN.Visible = true;
                    GfxOrientationM.Checked = true;
                    GfxOrientationL1.Checked = false;
                    GfxOrientationL2.Checked = false;
                    GfxOrientationP1.Checked = false;
                    GfxOrientationP2.Checked = false;
                }
                if (gfxOrientation == "Landscape")
                {
                    MultiscreenBTN.Visible = false;
                    GfxOrientationM.Checked = false;
                    GfxOrientationL1.Checked = true;
                    GfxOrientationL2.Checked = false;
                    GfxOrientationP1.Checked = false;
                    GfxOrientationP2.Checked = false;
                }
                if (gfxOrientation == "Portrait")
                {
                    MultiscreenBTN.Visible = false;
                    GfxOrientationM.Checked = false;
                    GfxOrientationL1.Checked = false;
                    GfxOrientationL2.Checked = false;
                    GfxOrientationP1.Checked = true;
                    GfxOrientationP2.Checked = false;
                }
                if (gfxOrientation == "Landscape-Flip")
                {
                    MultiscreenBTN.Visible = false;
                    GfxOrientationM.Checked = false;
                    GfxOrientationL1.Checked = false;
                    GfxOrientationL2.Checked = true;
                    GfxOrientationP1.Checked = false;
                    GfxOrientationP2.Checked = false;
                }
                if (gfxOrientation == "Portrait-Flip")
                {
                    MultiscreenBTN.Visible = false;
                    GfxOrientationM.Checked = false;
                    GfxOrientationL1.Checked = false;
                    GfxOrientationL2.Checked = false;
                    GfxOrientationP1.Checked = false;
                    GfxOrientationP2.Checked = true;
                }

                var browserReferer = MyIni.Read("Referer", "Browser");
                BrowserReferer.Text = browserReferer;

                var browserLoader = MyIni.Read("SignageLoader", "Sign");
                if (browserLoader == "1")
                {
                    SignageLoader.Text = "Chrome";
                }
                if (browserLoader == "2")
                {
                    SignageLoader.Text = "Core";
                }

                var syncID_File = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\nodejsData\\syncid";
                try
                {
                    var syncID = File.ReadAllText(syncID_File);
                    BrowserSyncID.Text = syncID;
                    
                }
                catch { BrowserSyncID.Text = ""; }

                var browserURL = MyIni.Read("Load", "Browser");
                if (browserURL == "Default")
                {
                    browserURL = "";
                }
                BrowserURL.Text = browserURL;

                var exRAM = MyIni.Read("ExRAM", "Browser");
                ExRAM.Text = exRAM;

                var DesktopScr = MyIni.Read("Desktop", "Browser");
                DesktopDropdown.Text = DesktopScr;

                var browserDebug = MyIni.Read("Debug", "Browser");
                DebugMode.Text = browserDebug;

                var browserSSL = MyIni.Read("SSL", "Browser");
                SSLMode.Text = browserSSL;

                var browserKeyboard = MyIni.Read("Keyboard", "Browser");
                if (browserKeyboard == "Default")
                {
                    GCMSKeyboard.Text = "Windows Default";
                }
                if (browserKeyboard == "Javascript")
                {
                    GCMSKeyboard.Text = "Injected Javascript";
                }
                if (browserKeyboard == "Application")
                {
                    GCMSKeyboard.Text = "Internal Application";
                }

                // Fill the Printer Dropdown with the Installed Printers on the machine
                foreach (string printer in PrinterSettings.InstalledPrinters)
                {
                    PrinterSettingsPrinter.Items.Add(printer);
                }

                var printerDefault = MyIni.Read("Default", "Printer");
                if (printerDefault == "On")
                {
                    UseDefaultPrinterOn.Checked = true;
                    UseDefaultPrinterOff.Checked = false;
                }
                else
                {
                    UseDefaultPrinterOn.Checked = false;
                    UseDefaultPrinterOff.Checked = true;
                }

                var printerOverride = MyIni.Read("PrinterOverride", "Printer");
                if (printerOverride != "")
                {
                    PrinterSettingsPrinter.Text = printerOverride;
                }

                var printerPrintPreview = MyIni.Read("PrintPreview", "Printer");
                PrinterSettingsPrintPreview.Text = printerPrintPreview;

                var printerPrintIn = MyIni.Read("PrintIn", "Printer");
                PrinterSettingsInLandscape.Text = printerPrintIn;

                var printerPrintInColor = MyIni.Read("PrintType", "Printer");
                PrinterSettingsInColor.Text = printerPrintInColor;

                var printerPrintPaperSize = MyIni.Read("PaperSize", "Printer");
                PrinterSettingsPaperSize.Text = printerPrintPaperSize;

                var printerPrintPaperMargin = MyIni.Read("PaperMargin", "Printer");
                PrinterSettingsMargin.Text = printerPrintPaperMargin;

                var printerCopies = MyIni.Read("Copies", "Printer");
                PrinterSettingsCopies.Text = printerCopies;

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
                        // listBox1.Items.Add(line);
                        var lineData = line.Split('?');
                        var lineItem = new ListViewItem(new[] { lineData[0], lineData[1], lineData[2] });
                        listBox1.Items.Add(lineItem);
                    }

                    listBox1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                    listBox1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                }

                var MyNetwork = MyIni.Read("licType", "Licence");
                if (MyNetwork == "SEC")
                {
                    // Due to SSL Issues over an IP Address - We Use IP Instead of Domain
                    networkURL = "http://172.16.0.2";
                    networkNameShort = "S";
                }
                else
                {
                    // Due to SSL Issues over an IP Address - We Use SSL for Domain
                    networkURL = "https://api.globalcms.co.uk";
                    networkNameShort = "P";
                }

                if (MyNetwork == "SEC")
                {
                    var pingTest = Ping(networkURL);
                    if (!pingTest)
                    {
                        networkURL = "https://api.globalcms.co.uk";
                        networkNameShort = "P";
                    }
                    else
                    {
                        networkURL = "http://172.16.0.2";
                        networkNameShort = "S";
                    }
                }
            }
        }

        static EngineerTools _frmObj;
        public static EngineerTools FrmObj
        {
            get { return _frmObj; }
            set { _frmObj = value; }
        }
        private static void RunExtendedTests(bool firstRun)
        {
            string networkURL;                                                // Which Network URL to use
            string networkIP;                                                 // Which Network IP to use - This is for the Ping Checker
            string signageVersion;                                            // Blank signageVersion for loading signage\version.txt file into
            string signageSubVersion;                                         // Blank signageSubVersion for loading signage\subversion.txt into

            FrmObj.serverAPILabel2.Text = "";
            FrmObj.serverAPPSLabel2.Text = "";
            FrmObj.serverVPNLabel2.Text = "";
            FrmObj.serverWWWLabel2.Text = "";
            FrmObj.InternetConnectionOpt2.Text = "";
            FrmObj.SecureConnectionOpt2.Text = "";
            FrmObj.SignageSystemOpt2.Text = "";
            FrmObj.devVPN2.Text = "";
            FrmObj.AppVersion.Text = "";
            FrmObj.AppSubVersion.Text = "";
            FrmObj.BrowserVersion.Text = "";
            FrmObj.AirServerVersion.Text = "";
            FrmObj.NexmosphereVer.Text = "";

            string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
            var MyIni = new IniFile(iniFile);
            var licType = MyIni.Read("licType", "Licence");

            FrmObj.AppVersion.Text = About.GetVersion("Main");
            FrmObj.AppSubVersion.Text = About.GetVersion("Subversion");
            FrmObj.BrowserVersion.Text = MainForm.eoVersion.ToString();
            FrmObj.AirServerVersion.Text = GCMSSystem.AirServer.Version().ToString();
            FrmObj.NexmosphereVer.Text = Nexmosphere.NexmosphereVersion.ToString();

            // Read Current Signage Version & SubVersion
            using (StreamReader streamReader = new StreamReader(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "version.txt"), Encoding.UTF8))
            {
                signageVersion = streamReader.ReadToEnd();
            }
            using (StreamReader streamReader = new StreamReader(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "subversion.txt"), Encoding.UTF8))
            {
                signageSubVersion = streamReader.ReadToEnd();
            }
            FrmObj.SignVersion.Text = signageVersion;
            FrmObj.SignSubVersion.Text = signageSubVersion;

            // Get the Remote Version String from Server
            try
            {
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                try
                {
                    HttpWebRequest request = FrmObj.GetRequest("http://api.globalcms.co.uk/v2/getVersion.php");
                    WebResponse webResponse = request.GetResponse();
                    FrmObj.AppVersionAvailable.Text = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                }
                catch { }
            }
            catch { }
            try
            {
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                try
                {
                    HttpWebRequest request = FrmObj.GetRequest("http://api.globalcms.co.uk/v2/getSubVersion.php");
                    WebResponse webResponse = request.GetResponse();
                    FrmObj.AppSubVersionAvailable.Text = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                }
                catch { }
            }
            catch { }
            try
            {
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                try
                {
                    HttpWebRequest request = FrmObj.GetRequest("http://api.globalcms.co.uk/v2/getVersionSign.php");
                    WebResponse webResponse = request.GetResponse();
                    FrmObj.SignVersionAvailable.Text = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                }
                catch { }
            }
            catch { }
            try
            {
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                try
                {
                    HttpWebRequest request = FrmObj.GetRequest("http://api.globalcms.co.uk/v2/getSubVersionSign.php");
                    WebResponse webResponse = request.GetResponse();
                    FrmObj.SignSubVersionAvailable.Text = new StreamReader(webResponse.GetResponseStream()).ReadToEnd();
                }
                catch { }
            }
            catch { }

            var apiServerPing = Ping("https://api.globalcms.co.uk");
            var appsServerPing = Ping("https://apps.globalcms.co.uk");
            var vpnServerPing = Ping("https://vpn.globalcms.co.uk:9700");
            var wwwServerPing = Ping("https://www.globalcms.co.uk");

            if (apiServerPing)
            {
                FrmObj.serverAPILabel2.Text = "ONLINE";
                FrmObj.serverAPILabel2.ForeColor = Color.FromArgb(0, 192, 0);
            }
            if (appsServerPing)
            {
                FrmObj.serverAPPSLabel2.Text = "ONLINE";
                FrmObj.serverAPPSLabel2.ForeColor = Color.FromArgb(0, 192, 0);
            }
            if (vpnServerPing)
            {
                FrmObj.serverVPNLabel2.Text = "ONLINE";
                FrmObj.serverVPNLabel2.ForeColor = Color.FromArgb(0, 192, 0);
            }
            if (wwwServerPing)
            {
                FrmObj.serverWWWLabel2.Text = "ONLINE";
                FrmObj.serverWWWLabel2.ForeColor = Color.FromArgb(0, 192, 0);
            }

            var MyNetwork = MyIni.Read("licType", "Licence");
            if (MyNetwork == "SEC")
            {
                // Due to SSL Issues over an IP Address - We Use IP Instead of Domain
                networkURL = "http://172.16.0.2";
                networkIP = "172.16.0.2";
            }
            else
            {
                // Due to SSL Issues over an IP Address - We Use SSL for Domain
                networkURL = "https://api.globalcms.co.uk";
                networkIP = "api.globalcms.co.uk";
            }

            if (MyNetwork == "SEC")
            {
                var pingTest = Ping(networkURL);
                if (!pingTest)
                {
                    networkURL = "https://api.globalcms.co.uk";
                    networkIP = "api.globalcms.co.uk";
                }
                else
                {
                    networkURL = "http://172.16.0.2";
                    networkIP = "172.16.0.2";
                }
            }
            FrmObj.InternetConnectionOpt2.Text = GCMSSystem.CheckForInternetConnection().ToUpper();
            if (FrmObj.InternetConnectionOpt2.Text == "ONLINE")
            {
                FrmObj.InternetConnectionOpt2.ForeColor = Color.FromArgb(0, 192, 0);
            }
            FrmObj.SecureConnectionOpt2.Text = GCMSSystem.CheckForVPNConnection(networkURL, networkIP).ToUpper();
            if (FrmObj.SecureConnectionOpt2.Text == "ONLINE")
            {
                FrmObj.SecureConnectionOpt2.ForeColor = Color.FromArgb(0, 192, 0);
            }
            FrmObj.SignageSystemOpt2.Text = GCMSSystem.CheckForSignage().ToUpper();
            if (FrmObj.SignageSystemOpt2.Text == "ONLINE")
            {
                FrmObj.SignageSystemOpt2.ForeColor = Color.FromArgb(0, 192, 0);
            }

            string vpnIP_check = GCMSSystem.GetIP("VPN");                                      // Global Variable for VPN IP
            FrmObj.devVPN2.Text = vpnIP_check;

            if (!firstRun)
            {
                MessageBox.Show("Tests Complete", "Engineering Tools", MessageBoxButtons.OK);
            }
        }
        public static void ClearSignageFiles(bool showCompleteMsg)
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
                catch (Exception)
                {

                }
            }
            foreach (var process in Process.GetProcessesByName("node64"))
            {
                try
                {
                    process.StartInfo.Verb = "runas";
                    process.Kill();
                    process.WaitForExit(60000);
                }
                catch (Exception)
                {

                }
            }

            // Delete all Signage Files that are used by Signage System
            File.Delete(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\logs\\nodejsData\\cacheLinks");
            File.Delete(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\logs\\nodejsData\\content_run_amounts");
            File.Delete(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\logs\\nodejsData\\oldPlayerData");
            File.Delete(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\logs\\nodejsData\\playerData");

            // Delete all content from Signage Content Folder
            DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\signage\\public\\content");
            foreach (FileInfo file in di.GetFiles())
            {
                try
                {
                    file.Delete();
                }
                catch
                {

                }
            }

            // Show a message box to say it has been completed
            if (showCompleteMsg)
            {
                MessageBox.Show("Clear Signage Files Complete", "Engineering Tools", MessageBoxButtons.OK);
            }
        }
        private static void GenPowerConfigView(IniFile PowerIni)
        {
            var powerStatus = PowerIni.Read("Status", "System");
            if (powerStatus != "") { FrmObj.PowerStatusDrpDwn.Text = powerStatus; }
            if (powerStatus == "") { FrmObj.PowerStatusDrpDwn.Text = "Off"; }

            var powerType = PowerIni.Read("Type", "System");
            if (powerType != "") {
                if (powerType == "Virtual") { FrmObj.PowerScreenTypeDrpDwn.Text = "Software Emulation"; }
                if (powerType == "RS232") { FrmObj.PowerScreenTypeDrpDwn.Text = "RS232 Serial Cable"; }
            }
            if (powerType == "") { FrmObj.PowerScreenTypeDrpDwn.Text = "Software Emulation"; }

            var powerScheduleMon = PowerIni.Read("Mon", "Schedule");
            var powerScheduleTue = PowerIni.Read("Tue", "Schedule");
            var powerScheduleWed = PowerIni.Read("Wed", "Schedule");
            var powerScheduleThu = PowerIni.Read("Thu", "Schedule");
            var powerScheduleFri = PowerIni.Read("Fri", "Schedule");
            var powerScheduleSat = PowerIni.Read("Sat", "Schedule");
            var powerScheduleSun = PowerIni.Read("Sun", "Schedule");

            // Split the String into an Array for On and Off
            var monStr = powerScheduleMon.Split(',');
            var monOn = monStr[0];
            var monOff = monStr[1];
            var monAllDay = monStr[2];

            var tueStr = powerScheduleTue.Split(',');
            var tueOn = tueStr[0];
            var tueOff = tueStr[1];
            var tueAllDay = tueStr[2];

            var wedStr = powerScheduleWed.Split(',');
            var wedOn = wedStr[0];
            var wedOff = wedStr[1];
            var wedAllDay = wedStr[2];

            var thuStr = powerScheduleThu.Split(',');
            var thuOn = thuStr[0];
            var thuOff = thuStr[1];
            var thuAllDay = thuStr[2];

            var friStr = powerScheduleFri.Split(',');
            var friOn = friStr[0];
            var friOff = friStr[1];
            var friAllDay = friStr[2];

            var satStr = powerScheduleSat.Split(',');
            var satOn = satStr[0];
            var satOff = satStr[1];
            var satAllDay = satStr[2];

            var sunStr = powerScheduleSun.Split(',');
            var sunOn = sunStr[0];
            var sunOff = sunStr[1];
            var sunAllDay = sunStr[2];

            // Add All Variables into the List View
            var lineMon = new ListViewItem(new[] { "Monday", monOn, monOff, monAllDay });
            var lineTue = new ListViewItem(new[] { "Tuesday", tueOn, tueOff, tueAllDay });
            var lineWed = new ListViewItem(new[] { "Wednesday", wedOn, wedOff, wedAllDay });
            var lineThu = new ListViewItem(new[] { "Thursday", thuOn, thuOff, thuAllDay });
            var lineFri = new ListViewItem(new[] { "Friday", friOn, friOff, friAllDay });
            var lineSat = new ListViewItem(new[] { "Saturday", satOn, satOff, satAllDay });
            var lineSun = new ListViewItem(new[] { "Sunday", sunOn, sunOff, sunAllDay });

            // Push Each ListView Item into the ListView
            FrmObj.PowerINIListView.Items.Add(lineMon);
            FrmObj.PowerINIListView.Items.Add(lineTue);
            FrmObj.PowerINIListView.Items.Add(lineWed);
            FrmObj.PowerINIListView.Items.Add(lineThu);
            FrmObj.PowerINIListView.Items.Add(lineFri);
            FrmObj.PowerINIListView.Items.Add(lineSat);
            FrmObj.PowerINIListView.Items.Add(lineSun);
        }
        private static void GenConfigListView(IniFile MyIni)
        {
            // Add the SyncID for the Signage into the Debug List
            var syncID_File = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\nodejsData\\syncid";
            try
            {
                var syncID = File.ReadAllText(syncID_File);
                var lineItemSyncID = new ListViewItem(new[] { "Signage", "syncID", "Sync Key For Signage ", syncID });
                FrmObj.ConfigINIListView.Items.Add(lineItemSyncID);
            }
            catch { }

            // Add all entries under the Monitor Section in the Config.ini
            var clientID = MyIni.Read("clientID", "Monitor");
            var lineItemClientID = new ListViewItem(new[] { "Monitor", "clientID", "Client ID", clientID });
            FrmObj.ConfigINIListView.Items.Add(lineItemClientID);
            var deviceName = MyIni.Read("deviceName", "Monitor");
            var lineItemDeviceName = new ListViewItem(new[] { "Monitor", "deviceName", "Device Name", deviceName });
            FrmObj.ConfigINIListView.Items.Add(lineItemDeviceName);
            var deviceUUID = MyIni.Read("deviceUUID", "Monitor");
            var lineItemDeviceUUID = new ListViewItem(new[] { "Monitor", "deviceUUID", "Device UUID", deviceUUID });
            FrmObj.ConfigINIListView.Items.Add(lineItemDeviceUUID);
            var hardwareMAC = MyIni.Read("hardwareMAC", "Monitor");
            var lineItemHardwareMAC = new ListViewItem(new[] { "Monitor", "hardwareMAC", "Hardware MAC ID", hardwareMAC });
            FrmObj.ConfigINIListView.Items.Add(lineItemHardwareMAC);

            // Add all entries under the Licence Section in the Config.ini
            var licType = MyIni.Read("licType", "Licence");
            var lineItemLicType = new ListViewItem(new[] { "Licence", "licType", "Licence Type", licType });
            FrmObj.ConfigINIListView.Items.Add(lineItemLicType);
            var licAS = MyIni.Read("AS", "Licence");
            var lineItemLicAS = new ListViewItem(new[] { "Licence", "AS", "AirServer Licence", licAS });
            FrmObj.ConfigINIListView.Items.Add(lineItemLicAS);

            // Add all entries under the Service Section in the Config.ini
            var servSignage = MyIni.Read("Signage", "Serv");
            var lineItemServSignage = new ListViewItem(new[] { "Serv", "Signage", "Signage Service", servSignage });
            FrmObj.ConfigINIListView.Items.Add(lineItemServSignage);
            var servVPN = MyIni.Read("VPN", "Serv");
            var lineItemServVPN = new ListViewItem(new[] { "Serv", "VPN", "VPN Service", servVPN });
            FrmObj.ConfigINIListView.Items.Add(lineItemServVPN);
            var servMirroring = MyIni.Read("Mirroring", "Serv");
            var lineItemServMirroring = new ListViewItem(new[] { "Serv", "Mirroring", "AirServer Mirroring Service", servMirroring });
            FrmObj.ConfigINIListView.Items.Add(lineItemServMirroring);
            var servEnv = MyIni.Read("Env", "Serv");
            var lineItemServEnv = new ListViewItem(new[] { "Serv", "Env", "Environmental Sensor Service", servEnv });
            FrmObj.ConfigINIListView.Items.Add(lineItemServEnv);

            // Add all entries under the Signage Section in the Config.ini
            var signPlatform = MyIni.Read("platform", "Sign");
            var lineItemSignPlatform = new ListViewItem(new[] { "Sign", "platform", "Signage Platform", signPlatform });
            FrmObj.ConfigINIListView.Items.Add(lineItemSignPlatform);
            var signLoader = MyIni.Read("SignageLoader", "Sign");
            var lineItemSignLoader = new ListViewItem(new[] { "Sign", "SignageLoader", "Signage Loader", signLoader });
            FrmObj.ConfigINIListView.Items.Add(lineItemSignLoader);
            var signCH = MyIni.Read("CH", "Sign");
            var lineItemSignCH = new ListViewItem(new[] { "Sign", "CH", "Signage Call Home Timer", signCH });
            FrmObj.ConfigINIListView.Items.Add(lineItemSignCH);

            // Add all entries under the Network Section in the Config.ini
            var networkWhich = MyIni.Read("selectedNetwork", "Network");
            var lineItemNetworkWhich = new ListViewItem(new[] { "Network", "selectedNetwork", "Which Connected Network", networkWhich });
            FrmObj.ConfigINIListView.Items.Add(lineItemNetworkWhich);
            var networkMaintMode = MyIni.Read("maintMode", "Network");
            var lineItemMaintMode = new ListViewItem(new[] { "Network", "maintMode", "Maintenance Mode Setting", networkMaintMode });
            FrmObj.ConfigINIListView.Items.Add(lineItemMaintMode);
            var networkPowerSave1 = MyIni.Read("powersaveMode", "Network");
            var lineItemPowerSave1 = new ListViewItem(new[] { "Network", "powersaveMode", "Power Saving Mode 1", networkPowerSave1 });
            FrmObj.ConfigINIListView.Items.Add(lineItemPowerSave1);
            var networkPowerSave2 = MyIni.Read("powersaveMode2", "Network");
            var lineItemPowerSave2 = new ListViewItem(new[] { "Network", "powersaveMode2", "Power Saving Mode 2", networkPowerSave2 });
            FrmObj.ConfigINIListView.Items.Add(lineItemPowerSave2);
            var networkTZ = MyIni.Read("timezone", "Network");
            var lineItemNetworkTZ = new ListViewItem(new[] { "Network", "timezone", "Network Timezone", networkTZ });
            FrmObj.ConfigINIListView.Items.Add(lineItemNetworkTZ);

            // Add all entries under the Skin Section in the Config.ini
            var skinID = MyIni.Read("skinID", "Skin");
            var lineItemSkinID = new ListViewItem(new[] { "Skin", "skinID", "Which Skin To Use", skinID });
            FrmObj.ConfigINIListView.Items.Add(lineItemSkinID);

            // Add all entries under the Skin Section in the Config.ini
            var volSetting = MyIni.Read("MasterLevel", "Audio");
            var lineItemVol = new ListViewItem(new[] { "Audio", "MasterLevel", "Sound Output Level", volSetting });
            FrmObj.ConfigINIListView.Items.Add(lineItemVol);

            // Add all entries under the Interactive Section in the Config.ini
            var interactiveCookieCleaner = MyIni.Read("CookieCleaner", "Interactive");
            var lineItemInteractiveCookieCleaner = new ListViewItem(new[] { "Interactive", "CookieCleaner", "Wipe After Interactive", interactiveCookieCleaner });
            FrmObj.ConfigINIListView.Items.Add(lineItemInteractiveCookieCleaner);

            // Add all entries under the Display Section in the Config.ini
            var displayForce = MyIni.Read("Force", "Display");
            var lineItemDisplayForce = new ListViewItem(new[] { "Display", "Force", "Force GFX Settings", displayForce });
            FrmObj.ConfigINIListView.Items.Add(lineItemDisplayForce);
            var displayResolution = MyIni.Read("Resolution", "Display");
            var lineItemDisplayResolution = new ListViewItem(new[] { "Display", "Resolution", "Resolution To Set", displayResolution });
            FrmObj.ConfigINIListView.Items.Add(lineItemDisplayResolution);
            var displayScaling = MyIni.Read("Scaling", "Display");
            var lineItemDisplayScaling = new ListViewItem(new[] { "Display", "Scaling", "Display Scaling To Set", displayScaling });
            FrmObj.ConfigINIListView.Items.Add(lineItemDisplayScaling);
            var displayOrientation = MyIni.Read("Orientation", "Display");
            var lineItemDisplayOrientation = new ListViewItem(new[] { "Display", "Orientation", "Display Orientation To Set", displayOrientation });
            FrmObj.ConfigINIListView.Items.Add(lineItemDisplayOrientation);

            // Add all entries under the Browser Section in the Config.ini
            var browserReferer = MyIni.Read("Referer", "Browser");
            var lineItemBrowserReferer = new ListViewItem(new[] { "Browser", "Referer", "Browser Referal URL", browserReferer });
            FrmObj.ConfigINIListView.Items.Add(lineItemBrowserReferer);
            var browserLoad = MyIni.Read("Load", "Browser");
            var lineItemBrowserLoad = new ListViewItem(new[] { "Browser", "Load", "Browser Load URL", browserLoad });
            FrmObj.ConfigINIListView.Items.Add(lineItemBrowserLoad);
            var browserDebug = MyIni.Read("Debug", "Browser");
            var lineItemBrowserDebug = new ListViewItem(new[] { "Browser", "Debug", "Browser Debug Show", browserDebug });
            FrmObj.ConfigINIListView.Items.Add(lineItemBrowserDebug);
            var browserKeyboard = MyIni.Read("Keyboard", "Browser");
            var lineItemBrowserKeyboard = new ListViewItem(new[] { "Browser", "Keyboard", "Browser Keyboard To Use", browserKeyboard });
            FrmObj.ConfigINIListView.Items.Add(lineItemBrowserKeyboard);
            var browserSSL = MyIni.Read("SSL", "Browser");
            var lineItemBrowserSSL = new ListViewItem(new[] { "Browser", "SSL", "Browser SSL", browserSSL });
            FrmObj.ConfigINIListView.Items.Add(lineItemBrowserSSL);

            // Add all entries under the Printer Section in the Config.ini
            var printerDefault = MyIni.Read("Default", "Printer");
            var lineItemPrinterDefault = new ListViewItem(new[] { "Printer", "Default", "Printer To Use Default", printerDefault });
            FrmObj.ConfigINIListView.Items.Add(lineItemPrinterDefault);
            var printerOverride = MyIni.Read("PrinterOverride", "Printer");
            var lineItemPrinterOverride = new ListViewItem(new[] { "Printer", "PrinterOverride", "Printer Override", printerOverride });
            FrmObj.ConfigINIListView.Items.Add(lineItemPrinterOverride);

            var printerPrintIn = MyIni.Read("PrintIn", "Printer");
            var lineItemPrinterPrintIn = new ListViewItem(new[] { "Printer", "PrintIn", "Print In", printerPrintIn });
            FrmObj.ConfigINIListView.Items.Add(lineItemPrinterPrintIn);
            var printerPrintType = MyIni.Read("PrintType", "Printer");
            var lineItemPrinterPrintType = new ListViewItem(new[] { "Printer", "PrintType", "Print Type", printerPrintType });
            FrmObj.ConfigINIListView.Items.Add(lineItemPrinterPrintType);
            var printerPaperSize = MyIni.Read("PaperSize", "Printer");
            var lineItemPrinterPaperSize = new ListViewItem(new[] { "Printer", "PaperSize", "Print Paper Size", printerPaperSize });
            FrmObj.ConfigINIListView.Items.Add(lineItemPrinterPaperSize);
            var printerPaperMargin = MyIni.Read("PaperMargin", "Printer");
            var lineItemPrinterPaperMargin = new ListViewItem(new[] { "Printer", "PaperMargin", "Print Paper Margins", printerPaperMargin });
            FrmObj.ConfigINIListView.Items.Add(lineItemPrinterPaperMargin);
            var printerCopies = MyIni.Read("Copies", "Printer");
            var lineItemPrinterCopies = new ListViewItem(new[] { "Printer", "Copies", "Copies To Print", printerCopies });
            FrmObj.ConfigINIListView.Items.Add(lineItemPrinterCopies);

            FrmObj.ConfigINIListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            FrmObj.ConfigINIListView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }
        private static bool Ping(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Timeout = 3000;
                request.AllowAutoRedirect = false; // find out if this site is up and don't follow a redirector
                request.Method = "HEAD";

                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                using (var response = request.GetResponse())
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        private static long CheckInternetSpeed()
        {
            // Create Object Of WebClient
            WebClient wc = new WebClient();

            //DateTime Variable To Store Download Start Time.
            DateTime dt1 = DateTime.Now;

            //Number Of Bytes Downloaded Are Stored In data
            byte[] data = wc.DownloadData("http://api.globalcms.co.uk/testfiles/10MB.testfile");

            //DateTime Variable To Store Download End Time.
            DateTime dt2 = DateTime.Now;

            var kbps = Math.Round((data.Length * 10) / (dt2 - dt1).TotalSeconds, 2);

            return Convert.ToInt64(kbps);
        }
        static string SizeSuffix(long value, int decimalPlaces = 0)
        {
            if (value < 0)
            {
                throw new ArgumentException("Bytes should not be negative", "value");
            }
            var mag = (int)Math.Max(0, Math.Log(value, 1024));
            var adjustedSize = Math.Round(value / Math.Pow(1024, mag), decimalPlaces);
            return string.Format("{0} {1}", adjustedSize, SizeSuffixes[mag]);
        }
        private static void IPConfig()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();

            List<string> ipDetails = new List<string>();            // Blank List to store records in

            foreach (NetworkInterface adapter in nics)
            {
                // Only display informatin for interfaces that support IPv4.
                if (adapter.Supports(NetworkInterfaceComponent.IPv4) == false)
                {
                    continue;
                }

                // Ignore anything with TAP / Virtualbox or Loopback from being shown
                if (!adapter.Description.Contains("TAP") && !adapter.Description.Contains("VirtualBox") && !adapter.Description.Contains("Loopback"))
                {
                    IPInterfaceProperties adapterProperties = adapter.GetIPProperties();
                    // Try to get the IPv4 interface properties.
                    IPv4InterfaceProperties p = adapterProperties.GetIPv4Properties();
                    IPAddressCollection dnsAddresses = adapterProperties.DnsAddresses;

                    if (p == null)
                    {
                        continue;
                    }

                    // Display the IPv4 specific data.
                    ipDetails.Add("Adapter \t\t\t" + adapter.Description);
                    ipDetails.Add("");
                    ipDetails.Add("IP Address \t\t" + GCMSSystem.GetIP("LAN"));
                    foreach (IPAddress dnsAdress in dnsAddresses)
                    {
                        var ServerLength = dnsAdress.ToString().Length;
                        if (ServerLength < 16)
                        {
                            ipDetails.Add("DNS Server \t\t" + dnsAdress);
                        }
                    }
                    ipDetails.Add("");
                    ipDetails.Add("Index \t\t\t" + p.Index);
                    ipDetails.Add("MTU \t\t\t" + p.Mtu);
                    ipDetails.Add("APIPA active \t\t" + p.IsAutomaticPrivateAddressingActive);
                    ipDetails.Add("APIPA enabled \t\t" + p.IsAutomaticPrivateAddressingEnabled);
                    ipDetails.Add("Forwarding enabled \t" + p.IsForwardingEnabled);
                    ipDetails.Add("Uses WINS \t\t" + p.UsesWins);
                    ipDetails.Add("");
                }
            }

            var message = string.Join(Environment.NewLine, ipDetails);                  // Convert the list to a message that MessageBox will understand
            MessageBox.Show(message, "Engineering Tools", MessageBoxButtons.OK);
        }

        public static void DetectNetworkAdapter()
        {
            foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                var InterFaceDesc = netInterface.Description;
                if (!InterFaceDesc.Contains("TAP") && !InterFaceDesc.Contains("VPN"))
                {
                    if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        if (netInterface.OperationalStatus == OperationalStatus.Up)
                        {
                            // FrmObj.devNetwork.Text = netInterface.NetworkInterfaceType.ToString();
                            if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                            {
                                try
                                {
                                    if (GCMSSystem.CheckOpened("EngineerTools"))
                                    {
                                        FrmObj.devNetwork.Text = "Wireless";
                                    }
                                }
                                catch { }
                                MainForm.FrmObj.WiFiCardOpt.Text = "Connected";
                                MainForm.FrmObj.WiFiCardOpt.ForeColor = Color.FromArgb(0, 192, 0);
                            }
                            if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                            {
                                try
                                {
                                    if (GCMSSystem.CheckOpened("EngineerTools"))
                                    {
                                        FrmObj.devNetwork.Text = "Ethernet";
                                    }
                                }
                                catch { }
                                MainForm.FrmObj.WiFiCardOpt.Text = "Disconnected";
                                MainForm.FrmObj.WiFiCardOpt.ForeColor = Color.FromArgb(192, 0, 0);
                            }
                        }
                    }
                }
            }
        }
        public static string DetectNetworkAdapter2()
        {
            var outStr = "";
            foreach (NetworkInterface netInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                var InterFaceDesc = netInterface.Description;
                if (!InterFaceDesc.Contains("TAP") && !InterFaceDesc.Contains("VPN"))
                {
                    if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 || netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                    {
                        if (netInterface.OperationalStatus == OperationalStatus.Up)
                        {
                            // FrmObj.devNetwork.Text = netInterface.NetworkInterfaceType.ToString();
                            if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                            {
                                outStr = "Wireless";
                            }
                            if (netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                            {
                                outStr = "Ethernet";
                            }
                        }
                    }
                }
            }
            return outStr;
        }

        /// BUTTONS
        private void ReRunBTN_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 4;
            RunExtendedTests(false);
        }

        private void ClearSignageFilesBTN_Click(object sender, EventArgs e)
        {
            ClearSignageFiles(true);
        }

        private void BandwidthSpeedBTN_Click(object sender, EventArgs e)
        {
            var inetSpeed = CheckInternetSpeed();
            var inetActualSpeed = SizeSuffix(inetSpeed, 2);

            MessageBox.Show("Internet Bandwidth Speed\n\n" + inetActualSpeed, "Engineering Tools", MessageBoxButtons.OK);
        }

        private void IPConfigLabel_Click(object sender, EventArgs e)
        {
            IPConfig();
        }

        private void FlushDNSBTN_Click(object sender, EventArgs e)
        {
            Process dns = new Process();
            dns.StartInfo.FileName = "ipconfig.exe";
            dns.StartInfo.Arguments = "/flushdns";
            dns.StartInfo.Verb = "runas";
            dns.Start();
            dns.WaitForExit(10000);
            MessageBox.Show("DNS Flushed Successfully", "Engineering Tools", MessageBoxButtons.OK);
        }

        private void DiskCleanupBTN_Click(object sender, EventArgs e)
        {
            GCMSSystem.SystemCleaner.Run();
        }

        private void RestartNicBTN_Click(object sender, EventArgs e)
        {
            IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

            // Stop the option is there is no WiFi or LAN found
            if (nics == null || nics.Length < 1)
            {
                return;
            }

            // Grab all Nic Interfaces
            foreach (NetworkInterface adapter in nics)
            {
                var activeNic = "LAN";
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    activeNic = "WIFI";
                }
                // Restart The Given Nic
                GCMSSystem.Network.Restart(activeNic);
                MessageBox.Show("Network Restarted", "Engineering Tools", MessageBoxButtons.OK);
            }
        }

        private void DirectXInfoBTN_Click(object sender, EventArgs e)
        {
            Process dx = new Process();
            dx.StartInfo.FileName = "dxdiag.exe";
            dx.StartInfo.Arguments = "/whql:off";
            dx.StartInfo.Verb = "runas";
            dx.Start();
        }

        private void ClearCookiesBTN_Click(object sender, EventArgs e)
        {
            GCMSSystem.Chrome.Unload();                     // Due to Chrome using SQL to store cookies we need to make sure Chrome is shutdown to unlock the files
            GCMSSystem.Chrome.ClearCookies(false);          // Clear Cookies
            MessageBox.Show("Cookies Cleared", "Engineering Tools", MessageBoxButtons.OK);
            GCMSSystem.Chrome.Load();
        }

        private void EDIDDataBTN_Click(object sender, EventArgs e)
        {
            var edidMessage = GCMSSystem.EDID.Get();
            MessageBox.Show(edidMessage, "Engineering Tools", MessageBoxButtons.OK);
        }
        private void DeviceManagerBTN_Click(object sender, EventArgs e)
        {
            Form devManager = new DeviceManager();
            devManager.Show();
        }


        /////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////
        /////////////////////////////////////////////////////////////

        // Registry Updaters
        private void DisableSignageTriggerBTN_Click(object sender, EventArgs e)
        {
            string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
            var MyIni = new IniFile(iniFile);

            var curButtonText = DisableSignageTriggerBTN.Text;
            if (curButtonText == "Disable Signage Loading")
            {
                MyIni.Write("Signage", "Disabled", "Serv");
                DisableSignageTriggerBTN.Text = "Enable Signage Loading";
                MessageBox.Show("Signage Disabled From Loading on Startup", "Engineering Tools", MessageBoxButtons.OK);
            }
            else
            {
                MyIni.Write("Signage", "Enabled", "Serv");
                DisableSignageTriggerBTN.Text = "Disable Signage Loading";
                MessageBox.Show("Signage Enabled For Loading on Startup", "Engineering Tools", MessageBoxButtons.OK);
            }
        }

        // Exit Maint Mode
        private void ExitMaintModeBTN_Click(object sender, EventArgs e)
        {
            string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
            var MyIni = new IniFile(iniFile);
            MyIni.Write("powersaveMode", "FALSE", "Network");
            MyIni.Write("powersaveMode2", "FALSE", "Network");
            MyIni.Write("maintMode", "FALSE", "Network");

            MainForm.FrmObj.powerModeLabel.Text = "Normal / Online";
            MainForm.FrmObj.powerModeLabel.ForeColor = Color.FromArgb(0, 0, 0);

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
                    if (System.Windows.Forms.Application.OpenForms["SignageBrowser"] == null)
                    {
                        GCMSSystem.Chrome.Load();
                    }
                }
            }
            if (!MainForm.isDebug)
            {
                if (MainForm.hardenedShell)
                {
                    try
                    {
                        Taskbar.Hide();
                    }
                    catch { }
                }
            }
            Close();
        }

        private void RollSignageBackBTN_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are You Sure?", "Engineering Tools", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                // Update Signage
                GCMSSystem.FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Roll Back Signage");

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
                MainForm.FrmObj.CheckServicesTimer.Stop();
                MainForm.FrmObj.CheckSNAP.Stop();

                // Move content folder to temp folder inside of monitor main root DIR
                try
                {
                    Directory.Move(sourceDIR, destDIR);
                }
                catch
                {

                }

                // Delete Whole Signage Folder
                try
                {
                    Directory.Delete(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage", true);
                }
                catch
                {

                }

                string networkURL;
                // Which Network Should We use?
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                var MyNetwork = MyIni.Read("licType", "Licence");
                var uuidStr = MyIni.Read("deviceUUID", "Monitor");
                if (MyNetwork == "SEC")
                {
                    // Due to SSL Issues over an IP Address - We Use IP Instead of Domain
                    networkURL = "http://172.16.0.2";
                }
                else
                {
                    // Due to SSL Issues over an IP Address - We Use SSL for Domain
                    networkURL = "https://api.globalcms.co.uk";
                }
                var pingTest = GCMSSystem.Ping(networkURL);
                if (!pingTest)
                {
                    networkURL = "https://api.globalcms.co.uk";
                }
                else
                {
                    networkURL = "http://172.16.0.2";
                }
                if (MainForm.NetworkOverride != "Auto" && MainForm.NetworkOverride == "Public") { networkURL = "https://api.globalcms.co.uk"; }
                if (MainForm.NetworkOverride != "Auto" && MainForm.NetworkOverride == "Secure") { networkURL = "http://172.16.0.2"; }

                // Download latest Signage.zip from API Server
                try
                {
                    GCMSSystem.DownloadFileSingle(networkURL + "/v2/signageUpdate/signageLast.zip", "signage.zip");
                }
                catch
                {

                }

                // Unzip Signage.zip to the Signage Folder inside monitor main root DIR
                var signageZipFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage.zip";
                var signageZipFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                try
                {
                    System.IO.Compression.ZipFile.ExtractToDirectory(signageZipFile, signageZipFolder);
                }
                catch
                {

                }

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
                catch
                {

                }

                // Restart NodeJS and then Chrome
                var osArch = GCMSSystem.GetOSArch();
                var chromeEXE = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\Google\\Chrome\\Application\\chrome.exe";
                var nodeEXE = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\node32.exe";
                if (osArch == "x64")
                {
                    nodeEXE = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\node64.exe";
                }

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
            }
        }

        private void AutoCookieCleanerBTN_Click(object sender, EventArgs e)
        {
            string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
            var MyIni = new IniFile(iniFile);

            var curButtonText = AutoCookieCleanerBTN.Text;
            if (curButtonText == "Enable Auto Cookie Clearer")
            {
                MyIni.Write("CookieCleaner", "On", "Interactive");
                AutoCookieCleanerBTN.Text = "Disable Auto Cookie Clearer";
                MainForm.isAutoCookieCleaner = true;
                MessageBox.Show("Cookie Cleaner Enabled at end of Interactive", "Engineering Tools", MessageBoxButtons.OK);
            }
            else
            {
                MyIni.Write("CookieCleaner", "Off", "Interactive");
                AutoCookieCleanerBTN.Text = "Enable Auto Cookie Clearer";
                MainForm.isAutoCookieCleaner = false;
                MessageBox.Show("Cookie Cleaner Disabled at end of Interactive", "Engineering Tools", MessageBoxButtons.OK);
            }
        }

        private void UpdateScreenConfigBTN_Click(object sender, EventArgs e)
        {
            // When the Button is clicked we need to grab all the settings that have been set, and then store them into the config.ini
            string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
            var MyIni = new IniFile(iniFile);

            // Force Graphics Overrides
            var gfxForce = "Off";
            if (gfxForceOn.Checked)
            {
                gfxForce = "On";
            }
            MyIni.Write("Force", gfxForce, "Display");

            // Desktop Setting (Either Standard or Mosaic
            var DesktopStr = DesktopDropdown.Text;
            MyIni.Write("Desktop", DesktopStr, "Browser");

            // Force - GFX Resolution
            string gfxResolution = ResolutionDropdown.GetItemText(ResolutionDropdown.SelectedItem);
            MyIni.Write("Resolution", GCMSSystem.RemoveWhitespace(gfxResolution), "Display");

            // Force - GFX Scaling
            string gfxScaling = ScalingDropdown.GetItemText(ScalingDropdown.SelectedItem);
            MyIni.Write("Scaling", GCMSSystem.RemoveWhitespace(gfxScaling).Replace("%", ""), "Display");

            // Force - GFX Orientation
            var gfxOrientation = "Landscape";
            if (GfxOrientationM.Checked)
            {
                gfxOrientation = "Multiple";
            }
            if (GfxOrientationL2.Checked)
            {
                gfxOrientation = "Landscape-Flip";
            }
            if (GfxOrientationP1.Checked)
            {
                gfxOrientation = "Portrait";
            }
            if (GfxOrientationP2.Checked)
            {
                gfxOrientation = "Portrait-Flip";
            }
            MyIni.Write("Orientation", gfxOrientation, "Display");

            // Debug.WriteLine("Scaling Factor: " +ScreenScaling.GetScalingFactor());

            // If the user has checked the box to set all these settings now, then to apply them
            if (UpdateScreenConfigNow.Checked)
            {
                ScreenScaling.SetScaleFactor(GCMSSystem.RemoveWhitespace(gfxScaling).Replace("%", ""));                  // Set the Scale Factor
                if (gfxOrientation != "Multiple")
                {
                    string[] resTokens = GCMSSystem.RemoveWhitespace(gfxResolution).Split('x');
                    var setW = Convert.ToInt32(resTokens[0]);
                    var setH = Convert.ToInt32(resTokens[1]);
                    ScreenResolution.SetResoltion(setW, setH);                                                          // Set the Screen Resolution
                }
                if (gfxOrientation != "Multiple")
                {
                    if (gfxOrientation == "Landscape")
                    {
                        ScreenRotation.SetOrientation(1, 0);
                    }
                    if (gfxOrientation == "Landscape-Flip")
                    {
                        ScreenRotation.SetOrientation(1, 90);
                    }
                    if (gfxOrientation == "Portrait")
                    {
                        ScreenRotation.SetOrientation(1, 45);
                    }
                    if (gfxOrientation == "Portrait-Flip")
                    {
                        ScreenRotation.SetOrientation(1, 135);
                    }
                }

                // Create the Dynamic Timer
                Timer tmr;
                tmr = new Timer();
                tmr.Tick += delegate
                {
                    tmr.Stop();
                    UpdateScreenConfigNow.Checked = false;
                };

                // How Long do we want to run the Timer for
                tmr.Interval = (int)TimeSpan.FromSeconds(2).TotalMilliseconds;

                // Start the Timer
                tmr.Start();
            }
            MessageBox.Show("Sceen Config Updated\nIf you have updated the scaling please reboot the PC", "Engineering Tools", MessageBoxButtons.OK);
        }

        private void StartScanner_Click(object sender, EventArgs e)
        {
            RFIDTimer.Enabled = true;
            RFIDTimer.Start();
            ScannerStatusLabel.Text = "Scanner Running";
            ScannerStatusLabel.ForeColor = Color.FromArgb(0, 192, 0);

            StartScanner.Enabled = false;
            StopScanner.Enabled = true;
            CancelScanner.Enabled = true;

        }

        private void StopScanner_Click(object sender, EventArgs e)
        {
            lastRFID = "";
            RFIDTimer.Stop();
            RFIDTimer.Enabled = false;

            if (ScannerStatusLabel.Text == "Scanner Running")
            {
                ScannerStatusLabel.Text = "Scanner Not Running";
                ScannerStatusLabel.ForeColor = Color.FromArgb(0, 0, 0);

                string networkURL;

                int listViewCount = listView1.Items.Count;
                if (listViewCount > 0)
                {
                    string RFIDCardString = string.Empty;
                    foreach (ListViewItem anItem in listView1.Items)
                    {
                        RFIDCardString += anItem.Text + ",";
                    }

                    var rfidPrefix = "None";
                    if (RFIDPrefixInput.Text != "")
                    {
                        rfidPrefix = RFIDPrefixInput.Text;
                    }

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
                        }
                        else
                        {
                            // Due to SSL Issues over an IP Address - We Use SSL for Domain
                            networkURL = "https://api.globalcms.co.uk";
                        }
                        if (MyNetwork == "SEC")
                        {
                            var pingTest = Ping(networkURL);
                            if (!pingTest)
                            {
                                networkURL = "https://api.globalcms.co.uk";
                            }
                            else
                            {
                                networkURL = "http://172.16.0.2";
                            }
                        }
                        if (MainForm.NetworkOverride != "Auto" && MainForm.NetworkOverride == "Public") { networkURL = "https://api.globalcms.co.uk"; }
                        if (MainForm.NetworkOverride != "Auto" && MainForm.NetworkOverride == "Secure") { networkURL = "http://172.16.0.2"; }

                        // Create the $_POST Data for the HTTP Request
                        var values = new NameValueCollection
                        {
                            ["clientID"] = MyIni.Read("clientID", "Monitor"),
                            ["deviceID"] = MyIni.Read("deviceName", "Monitor"),
                            ["deviceMAC"] = MyIni.Read("deviceMAC", "Monitor"),
                            ["deviceUUID"] = MyIni.Read("deviceUUID", "Monitor"),
                            ["hardwareMAC"] = MyIni.Read("hardwareMAC", "Monitor"),
                            ["rfidPrefix"] = rfidPrefix,
                            ["rfidCards"] = RFIDCardString
                        };

                        var responseString = "";
                        try
                        {
                            var response = client.UploadValues(networkURL + "/v2/inboundRFIDScanner.php", values);
                            responseString = Encoding.Default.GetString(response);
                            listView1.Items.Clear();
                            listView1.Refresh();
                            GCMSSystem.NodeSocket.Send("clearLastRFID");
                        }
                        catch
                        {
                            responseString = "Error";
                            ScannerStatusLabel.Text = "Server Error";
                        }
                    }

                    StartScanner.Enabled = true;
                    StopScanner.Enabled = false;
                    CancelScanner.Enabled = false;
                    RFIDCounter.Text = "0";
                }
            }
        }

        private void CancelScanner_Click(object sender, EventArgs e)
        {
            lastRFID = "";
            RFIDTimer.Stop();
            RFIDTimer.Enabled = false;

            if (ScannerStatusLabel.Text == "Scanner Running")
            {
                ScannerStatusLabel.Text = "Scanner Not Running";
                ScannerStatusLabel.ForeColor = Color.FromArgb(0, 0, 0);

                int listViewCount = listView1.Items.Count;
                if (listViewCount > 0)
                {
                    listView1.Items.Clear();
                    listView1.Refresh();
                    GCMSSystem.NodeSocket.Send("clearLastRFID");
                }

                StartScanner.Enabled = true;
                StopScanner.Enabled = false;
                CancelScanner.Enabled = false;
                RFIDCounter.Text = "0";

            }
        }

        private void RFIDTimer_Tick(object sender, EventArgs e)
        {
            string responseString;
            try
            {
                // Get the details from the /interactive endpoint of the Digital Signage
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                try
                {
                    HttpWebRequest request = GetRequest("http://localhost:444/lastRFID");
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

            if (responseString != "Error")
            {
                if (responseString != "false")
                {
                    var data = (JObject)JsonConvert.DeserializeObject(responseString);
                    var rfidCard = data["id"].Value<string>();
                    if (rfidCard != lastRFID)
                    {
                        if (!listView1.Items.ContainsKey(rfidCard))
                        {
                            //copy item
                            lastRFID = rfidCard;

                            Stream str = Properties.Resources.BEEP;
                            System.Media.SoundPlayer snd = new System.Media.SoundPlayer(str);
                            snd.Play();

                            listView1.Items.Add(lastRFID, lastRFID, null);
                            int curCounter = Convert.ToInt32(RFIDCounter.Text);
                            int newCounter = curCounter + 1;

                            RFIDCounter.Text = newCounter.ToString();

                        }
                    }
                }
            }
            GC.Collect(GC.MaxGeneration);
        }

        private HttpWebRequest GetRequest(string url, string httpMethod = "GET", bool allowAutoRedirect = true)
        {
            Uri uri = new Uri(url);
            HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";

            request.Timeout = Convert.ToInt32(new TimeSpan(0, 5, 0).TotalMilliseconds);
            request.Method = httpMethod;
            return request;
        }

        private void UpdateBrowserReferer_Click(object sender, EventArgs e)
        {
            if (BrowserSyncID.Enabled)
            {
                var syncID_File = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\nodejsData\\syncid";
                try
                {
                    File.WriteAllText(syncID_File, BrowserSyncID.Text);
                    GCMSSystem.NodeSocket.Send("__update_playerdata");
                }
                catch { }
            }

            if (SyncIDUnlockBTN.Text == "Lock")
            {
                BrowserSyncID.Enabled = false;
                SyncIDUnlockBTN.Text = "Unlock";
            }

            // Read INI File for Config.ini
            string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
            var MyIni = new IniFile(iniFile);

            string signageConfFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "settings.conf");
            var SettingsIni = new IniFile(signageConfFile);

            string browserLoad = "Default";
            if (BrowserURL.Text != "")
            {
                bool browserURLStringValid = false;
                if (BrowserURL.Text.StartsWith("http://") || BrowserURL.Text.StartsWith("https://"))
                {
                    browserURLStringValid = true;
                }
                else
                {
                    BrowserURL.Text = "http://" + BrowserURL.Text;
                    browserURLStringValid = true;
                }

                if (browserURLStringValid)
                {
                    browserLoad = BrowserURL.Text;
                }
            }
            MyIni.Write("Load", browserLoad, "Browser");

            bool refererURLStringValid = false;
            if (BrowserReferer.Text.StartsWith("http://") || BrowserReferer.Text.StartsWith("https://"))
            {
                refererURLStringValid = true;
            }
            else
            {
                BrowserReferer.Text = "http://" + BrowserReferer.Text;
                refererURLStringValid = true;
            }

            if (refererURLStringValid)
            {
                if (BrowserReferer.Text == "http://")
                {
                    BrowserReferer.Text = "";
                }
                MyIni.Write("Referer", BrowserReferer.Text, "Browser");
            }

            // EOWP (for systems that have memory crashes) Overrides
            var exRAM = ExRAM.Text;
            MyIni.Write("exRAM", exRAM, "Browser");

            // Browser Debug Overrides
            var browserDebug = DebugMode.Text;
            MyIni.Write("Debug", browserDebug, "Browser");

            // Browser Debug Overrides
            var browserSSL = SSLMode.Text;
            MyIni.Write("SSL", browserSSL, "Browser");
            if (browserSSL == "On")
            {
                SettingsIni.Write("sslOn", "true", "core");
            }
            else
            {
                SettingsIni.Write("sslOn", "false", "core");
            }

            // Browser Keyboard Overrides
            var browserKeyboard = "Default";
            if (GCMSKeyboard.Text == "Injected Javascript")
            {
                browserKeyboard = "Javascript";
            }
            if (GCMSKeyboard.Text == "Internal Application")
            {
                browserKeyboard = "Application";
            }
            MyIni.Write("Keyboard", browserKeyboard, "Browser");

            // Signage Loader
            var browserLoader = 2;
            if (SignageLoader.Text == "Core")
            {
                browserLoader = 2;
            }
            if (SignageLoader.Text == "Chrome")
            {
                browserLoader = 1;
            }
            MyIni.Write("SignageLoader", browserLoader.ToString(), "Sign");

            MainForm.signageLoader = browserLoader;

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

            MessageBox.Show("Browser Settings Updated", "Engineering Tools", MessageBoxButtons.OK);
        }

        private void UpdatePrinterSettingsBTN_Click(object sender, EventArgs e)
        {
            // Read INI File for Config.ini
            string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
            var MyIni = new IniFile(iniFile);

            // Printer Default
            var printerDefault = "On";
            if (UseDefaultPrinterOff.Checked)
            {
                printerDefault = "Off";
            }
            MyIni.Write("Default", printerDefault, "Printer");

            // Printer Override
            var printerOverride = PrinterSettingsPrinter.Text;
            MyIni.Write("PrinterOverride", printerOverride, "Printer");

            // Print Preview
            var printPreview = PrinterSettingsPrintPreview.Text;
            MyIni.Write("PrintPreview", printPreview, "Printer");

            // Print In
            var printerPrintIn = PrinterSettingsInLandscape.Text;
            MyIni.Write("PrintIn", printerPrintIn, "Printer");

            // Print Type
            var printerPrintType = PrinterSettingsInColor.Text;
            MyIni.Write("PrintType", printerPrintType, "Printer");

            // Paper Size
            var printerPaperSize = PrinterSettingsPaperSize.Text;
            MyIni.Write("PaperSize", printerPaperSize, "Printer");

            // Paper Margin
            var printerPaperMargin = PrinterSettingsMargin.Text;
            MyIni.Write("PaperMargin", printerPaperMargin, "Printer");

            // Printer Copies
            var printerCopies = PrinterSettingsCopies.Text;
            MyIni.Write("Copies", printerCopies, "Printer");

            MessageBox.Show("Printer Settings Updated", "Engineering Tools", MessageBoxButtons.OK);
        }

        private void OnlineToolsBTN_Click(object sender, EventArgs e)
        {
            Process.Start("https://apps.globalcms.co.uk/benchmark/");
        }

        private void ClearInteractiveBTN_Click(object sender, EventArgs e)
        {
            if (MainForm.isInteractive)
            {
                MainForm.isInteractive = false;              // Set to False to Stop the Timer Checker
                MainForm.FrmObj.Interaction.Enabled = false;        // Turn off the Interaction Tick

                // Reset the Mode Back to Normal
                MainForm.FrmObj.powerModeLabel.Text = "Normal / Online";
                MainForm.FrmObj.powerModeLabel.ForeColor = Color.FromArgb(0, 0, 0);

                // Stop the Interactive Timer and Restart the Checker for Interactive
                if (MainForm.isAutoCookieCleaner)
                {
                    GCMSSystem.Chrome.ClearCookies(true);
                }
            }
            // Send The End Interactive Websocket - "endinteractive"
            GCMSSystem.NodeSocket.Send("endinteractive");

            MainForm.FrmObj.CheckForInteractive.Start();
            MessageBox.Show("Interactive Cleared", "Engineering Tools", MessageBoxButtons.OK);
        }

        private void AppLauncherAddBTN_Click(object sender, EventArgs e)
        {
            string input = Microsoft.VisualBasic.Interaction.InputBox("Please supply the full path to the application to initially load", "Monitoring Solution", "", 0, 0);
            if (input != "")
            {
                string input2 = Microsoft.VisualBasic.Interaction.InputBox("Please supply the full path to the application to monitor", "Monitoring Solution", "", 0, 0);
                if (input2 != "")
                {
                    var appStr = "Disabled?" + input + "?" + input2;
                    string appIniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "launcher.ini");
                    using (StreamWriter w = File.AppendText(appIniFile))
                    {
                        w.WriteLine(appStr);
                        var lineItem = new ListViewItem(new[] { "Disabled", input, input2 });
                        listBox1.Items.Add(lineItem);

                        listBox1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        listBox1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                    }
                }
            }
        }

        private void AppLauncherDelBTN_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are You Sure?", "Engineering Tools", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                string appIniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "launcher.ini");
                foreach (ListViewItem eachItem in listBox1.SelectedItems)
                {
                    var appDeleteCol1 = eachItem.SubItems[0].Text;
                    var appDeleteCol2 = eachItem.SubItems[1].Text;
                    var appDeleteCol3 = eachItem.SubItems[2].Text;

                    var whichItem = appDeleteCol1 + "?" + appDeleteCol2 + "?" + appDeleteCol3;
                    var tempFile = Path.GetTempFileName();
                    var linesToKeep = File.ReadLines(appIniFile).Where(l => l != whichItem);

                    File.WriteAllLines(tempFile, linesToKeep);
                    File.Delete(appIniFile);
                    File.Move(tempFile, appIniFile);

                    listBox1.Items.Remove(eachItem);
                }
            }
        }

        private void AppLauncherSwitchBTN_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem eachItem in listBox1.SelectedItems)
            {
                DialogResult dialogResult = MessageBox.Show("Are You Sure?", "Engineering Tools", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    string appIniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "launcher.ini");

                    var appDeleteCol1 = eachItem.SubItems[0].Text;
                    var appDeleteCol2 = eachItem.SubItems[1].Text;
                    var appDeleteCol3 = eachItem.SubItems[2].Text;

                    var whichItem = appDeleteCol1 + "?" + appDeleteCol2 + "?" + appDeleteCol3;
                    int whichLine = FindLine(whichItem, appIniFile);

                    string[] separatingChars = { "?" };
                    string[] lineData = whichItem.Split(separatingChars, StringSplitOptions.RemoveEmptyEntries);
                    var isApplicationEnabled = lineData[0];
                    var whatApplicationName = lineData[1];
                    var whatApplicationMonitor = lineData[2];

                    string toChange = "Disabled?" + whatApplicationName + "?" + whatApplicationMonitor;
                    string toChangeTxt = "Disabled";
                    if (isApplicationEnabled == "Disabled")
                    {
                        toChange = "Enabled?" + whatApplicationName + "?" + whatApplicationMonitor;
                        toChangeTxt = "Enabled";
                    }

                    LineChanger(toChange, appIniFile, whichLine);
                    eachItem.SubItems[0].Text = toChangeTxt;
                }
            }
        }
        static int FindLine(string searchText, string fileName)
        {
            int counter = 1;
            string line;

            // Read the file and display it line by line.
            StreamReader file = new System.IO.StreamReader(fileName);
            while ((line = file.ReadLine()) != null)
            {
                if (line.Contains(searchText))
                {
                    // Debug.WriteLine(counter.ToString() + ": " + line);
                    file.Close();
                    return counter;
                }

                counter++;
            }
            file.Close();
            return counter;
        }
        static void LineChanger(string newText, string fileName, int line_to_edit)
        {
            string[] arrLine = File.ReadAllLines(fileName);
            arrLine[line_to_edit - 1] = newText;
            File.WriteAllLines(fileName, arrLine);
        }

        private void AirServerInstallBTN_Click(object sender, EventArgs e)
        {
            // Which Network Should We use?
            string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
            var MyIni = new IniFile(iniFile);
            var MyNetwork = MyIni.Read("licType", "Licence");
            var uuidStr = MyIni.Read("deviceUUID", "Monitor");
            if (MyNetwork == "SEC")
            {
                // Due to SSL Issues over an IP Address - We Use IP Instead of Domain
                networkURL = "http://172.16.0.2";
                networkNameShort = "S";
            }
            else
            {
                // Due to SSL Issues over an IP Address - We Use SSL for Domain
                networkURL = "https://api.globalcms.co.uk";
                networkNameShort = "P";
            }
            var pingTest = GCMSSystem.Ping(networkURL);
            if (!pingTest)
            {
                networkURL = "https://api.globalcms.co.uk";
                networkNameShort = "P";
            }
            else
            {
                networkURL = "http://172.16.0.2";
                networkNameShort = "S";
            }
            if (MainForm.NetworkOverride != "Auto" && MainForm.NetworkOverride == "Public") { networkURL = "https://api.globalcms.co.uk"; networkNameShort = "P"; }
            if (MainForm.NetworkOverride != "Auto" && MainForm.NetworkOverride == "Secure") { networkURL = "http://172.16.0.2"; networkNameShort = "S"; }


            GCMSSystem.AirServer.Install(networkURL);
            var activateCode = GCMSSystem.AirServer.GetCode(networkURL, networkNameShort);
            GCMSSystem.AirServer.Activate(activateCode);
            GCMSSystem.AirServer.SetPassword(0);

            AirServerUninstallBTN.Enabled = true;
            AirServerStartBTN.Enabled = true;
            AirServerStopBTN.Enabled = true;
            AirServerInstallBTN.Enabled = false;
            AirServerChgPassBTN.Enabled = true;

            MessageBox.Show("AirServer Mirroring Installed", "Engineering Tools", MessageBoxButtons.OK);
        }
        private void AirServerUninstallBTN_Click(object sender, EventArgs e)
        {
            // Which Network Should We use?
            string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
            var MyIni = new IniFile(iniFile);
            var MyNetwork = MyIni.Read("licType", "Licence");
            var uuidStr = MyIni.Read("deviceUUID", "Monitor");
            if (MyNetwork == "SEC")
            {
                // Due to SSL Issues over an IP Address - We Use IP Instead of Domain
                networkURL = "http://172.16.0.2";
            }
            else
            {
                // Due to SSL Issues over an IP Address - We Use SSL for Domain
                networkURL = "https://api.globalcms.co.uk";
            }
            var pingTest = GCMSSystem.Ping(networkURL);
            if (!pingTest)
            {
                networkURL = "https://api.globalcms.co.uk";
            }
            else
            {
                networkURL = "http://172.16.0.2";
            }
            if (MainForm.NetworkOverride != "Auto" && MainForm.NetworkOverride == "Public") { networkURL = "https://api.globalcms.co.uk"; }
            if (MainForm.NetworkOverride != "Auto" && MainForm.NetworkOverride == "Secure") { networkURL = "http://172.16.0.2"; }

            GCMSSystem.AirServer.Kill();
            GCMSSystem.AirServer.Deactivate();
            GCMSSystem.AirServer.Uninstall(networkURL);

            AirServerUninstallBTN.Enabled = false;
            AirServerStartBTN.Enabled = false;
            AirServerStopBTN.Enabled = false;
            AirServerInstallBTN.Enabled = true;
            AirServerChgPassBTN.Enabled = false;

            MessageBox.Show("AirServer Mirroring Uninstalled", "Engineering Tools", MessageBoxButtons.OK);

        }
        private void AirServerStartBTN_Click(object sender, EventArgs e)
        {
            GCMSSystem.AirServer.Start();
            AirServerInstallStatus.Text = "CHECKING";
            AirServerInstallStatus.ForeColor = Color.FromArgb(255, 165, 0);

            AirServerUninstallBTN.Enabled = false;
            AirServerStartBTN.Enabled = false;
            AirServerStopBTN.Enabled = false;
            AirServerInstallBTN.Enabled = false;
            AirServerChgPassBTN.Enabled = false;
        }

        private void AirServerStopBTN_Click(object sender, EventArgs e)
        {
            GCMSSystem.AirServer.Kill();
            GCMSSystem.AirServer.Stop();

            AirServerInstallStatus.Text = "CHECKING";
            AirServerInstallStatus.ForeColor = Color.FromArgb(255, 165, 0);

            AirServerUninstallBTN.Enabled = false;
            AirServerStartBTN.Enabled = false;
            AirServerStopBTN.Enabled = false;
            AirServerInstallBTN.Enabled = false;
            AirServerChgPassBTN.Enabled = false;
        }

        private void AirServerChgPassBTN_Click(object sender, EventArgs e)
        {
            /*      Replaced Code for a FRM to use as a config rather than just changing passcode
            int theCode = 0;
            var newCode = Microsoft.VisualBasic.Interaction.InputBox("Please supply a new connection passcode - numerical only", "Engineering Tools", "");
            if (newCode != "")
            {
                theCode = Convert.ToInt32(newCode);
                GCMSSystem.AirServer.Stop();                                    // AirServer must be stopped and not running for changes to be made
                GCMSSystem.AirServer.SetPassword(theCode);
                AirServerPasscode.Text = "Code: " + newCode;
                GCMSSystem.AirServer.Start();                                   // Restart AirServer
                MessageBox.Show("AirServer Mirroring Passcode Updated", "Engineering Tools", MessageBoxButtons.OK);
            }
            */
            if (!GCMSSystem.CheckOpened("AirServerConfig"))
            {
                Form AirServerConfig = new AirServerConfig();
                AirServerConfig.Show();
            }
        }

        private void CopyIntTestBTN_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();                                      // Clear Contents of Clipboard

            StringBuilder sb = new StringBuilder();
            foreach (var item in InteractiveResults.Items)
            {
                if (item is ListViewItem l)
                {
                    foreach (ListViewItem.ListViewSubItem sub in l.SubItems)
                    {
                        sb.Append(sub.Text + "\t");
                    }
                }
                sb.AppendLine();
            }
            Clipboard.SetDataObject(sb.ToString());
            MessageBox.Show("Output Copied to Clipboard", "Engineering Tools", MessageBoxButtons.OK);
        }

        private void SaveIntTestBTN_Click(object sender, EventArgs e)
        {
            string filename = "";
            SaveFileDialog sfd = new SaveFileDialog
            {
                Title = "Save To File",
                Filter = "Text File (.txt) | *.txt",
                InitialDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString())
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                filename = sfd.FileName.ToString();
                if (filename != "")
                {
                    using (StreamWriter sw = new StreamWriter(filename))
                    {
                        foreach (var item in InteractiveResults.Items)
                        {
                            if (item is ListViewItem l)
                            {
                                foreach (ListViewItem.ListViewSubItem sub in l.SubItems)
                                {
                                    sw.WriteLine(sub.Text + "\t");
                                }
                            }
                        }
                    }
                    MessageBox.Show("Output Saved", "Engineering Tools", MessageBoxButtons.OK);
                }
            }
        }

        private void StartIntTestBTN_Click(object sender, EventArgs e)
        {

            CopyIntTestBTN.Enabled = true;
            SaveIntTestBTN.Enabled = true;

            var SelectedServer = "api.globalcms.co.uk";
            InteractiveResults.Items.Clear();
            var whichTest = InteractiveTestChoice.Text;
            var whichServer = InteractiveTestServer.Text;
            if (whichServer == "API Server") { SelectedServer = "api.globalcms.co.uk"; }
            if (whichServer == "Apps Server") { SelectedServer = "apps.globalcms.co.uk"; }
            if (whichServer == "VPN Server") { SelectedServer = "vpn.globalcms.co.uk"; }
            if (whichServer == "VPN Router") { SelectedServer = "172.16.0.2"; }
            if (whichServer == "Web Server") { SelectedServer = "www.globalcms.co.uk"; }

            ColumnHeader header = new ColumnHeader
            {
                Text = "",
                Name = "col1"
            };
            InteractiveResults.Columns.Add(header);

            if (whichTest == "Ping Test")
            {
                try
                {
                    int c = 10;
                    IPAddress ipAddress;
                    if (whichServer != "VPN Router") { ipAddress = Dns.GetHostEntry(SelectedServer).AddressList[0]; } else { ipAddress = IPAddress.Parse(SelectedServer); }
                    for (int i = 0; i < c; i++)
                    {
                        Ping ping = new Ping();
                        PingReply pingReply = ping.Send(ipAddress);
                        var resultStr = "Reply from " + ipAddress + ":  bytes = " + pingReply.Buffer.Count().ToString() + "  time = " + pingReply.RoundtripTime.ToString() + "ms  TTL = " + pingReply.Options.Ttl.ToString();
                        InteractiveResults.Items.Add(resultStr);

                        InteractiveResults.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                        InteractiveResults.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                        InteractiveResults.Items[InteractiveResults.Items.Count - 1].EnsureVisible();

                        System.Threading.Thread.Sleep(500);
                    }
                }
                catch (SocketException)
                {
                    MessageBox.Show("Could not resolve host name.");
                }
                catch (PingException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (ArgumentNullException)
                {
                    MessageBox.Show("Please enter the host name or IP address to ping.");
                }
            }
            if (whichTest == "Trace Route")
            {
                string hostname = SelectedServer;
                IPAddress ipAddress;
                if (whichServer != "VPN Router") { ipAddress = Dns.GetHostEntry(SelectedServer).AddressList[0]; } else { ipAddress = IPAddress.Parse(SelectedServer); }
                int timeout = 1000;     // 1000ms or 1 second
                int max_ttl = 30;       // max number of servers allowed to be found
                int current_ttl = 0;    // used for tracking how many servers have been found.
                const int bufferSize = 32;
                Stopwatch s1 = new Stopwatch();
                Stopwatch s2 = new Stopwatch();
                byte[] buffer = new byte[bufferSize];
                new Random().NextBytes(buffer);
                Ping pinger = new Ping();
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    WriteListBox($"Started ICMP Trace route on {ipAddress}");
                    for (int ttl = 1; ttl <= max_ttl; ttl++)
                    {
                        current_ttl++;
                        s1.Start();
                        s2.Start();
                        PingOptions options = new PingOptions(ttl, true);
                        PingReply reply = null;
                        try
                        {
                            reply = pinger.Send(hostname, timeout, buffer, options);
                        }
                        catch
                        {
                            WriteListBox("Error");
                            break; //the rest of the code relies on reply not being null so...
                        }
                        if (reply != null) //dont need this but anyway...
                        {
                            //the traceroute bits :)
                            if (reply.Status == IPStatus.TtlExpired)
                            {
                                //address found after yours on the way to the destination
                                WriteListBox($"[{ttl}] - Route: {reply.Address} - Time: {s2.ElapsedMilliseconds} ms");

                                s1.Reset();
                                s2.Reset();

                                continue; //continue to the other bits to find more servers
                            }
                            if (reply.Status == IPStatus.TimedOut)
                            {
                                //this would occour if it takes too long for the server to reply or if a server has the ICMP port closed (quite common for this).
                                WriteListBox($"[{ttl}] - Timeout on hop. Continuing.");

                                s1.Reset();
                                s2.Reset();

                                continue;
                            }
                            if (reply.Status == IPStatus.Success)
                            {
                                //the ICMP packet has reached the destination (the hostname)
                                WriteListBox($"[{ttl}] - Route: {reply.Address} - Time: {s2.ElapsedMilliseconds} ms");

                                WriteListBox($" ");
                                WriteListBox($"Completed Trace Route");

                                s1.Stop();
                                s2.Stop();
                            }
                        }
                        break;
                    }
                });
            }
        }

        private void RestartSignageBTN_Click(object sender, EventArgs e)
        {
            GCMSSystem.TriggerSystem("RESTARTSIGN", false, false);
            MessageBox.Show("Restart Signage Triggered", "Engineering Tools", MessageBoxButtons.OK);
        }

        private void RestartNexusBTN_Click(object sender, EventArgs e)
        {
            if (GCMSSystem.CheckOpened("Nexmosphere") && nexmoshpereSensors)
            {
                Nexmosphere.frmObj.Close();

                Form NexusDebug = new Nexmosphere();
                NexusDebug.Show();
                MessageBox.Show("Restart Nexus Triggered", "Engineering Tools", MessageBoxButtons.OK);
            }
        }

        private void RestartEnvBTN_Click(object sender, EventArgs e)
        {
            IniFile MyIni = new IniFile(iniFile);
            var omronService = MyIni.Read("Env", "Serv");
            if (GCMSSystem.CheckOpened("OmronSensor"))
            {
                Form OmronDebug = new OmronSensor();
                OmronDebug.Show();
                if (omronService == "Disabled")
                {
                    OmronDebug.Visible = false;
                }
            }
        }

        private void WriteListBox(string text)
        {
            FrmObj.BeginInvoke(new Action(() => {
                InteractiveResults.Items.Add(text);

                InteractiveResults.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                InteractiveResults.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                InteractiveResults.Items[InteractiveResults.Items.Count - 1].EnsureVisible();

            }));
        }

        private void LiveChatBTN_Click(object sender, EventArgs e)
        {
            if (!GCMSSystem.CheckOpened("SupportChat"))
            {
                Form LiveSupport = new LiveSupport();
                LiveSupport.Show();
            }
        }

        private void Tab_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabControl1.TabPages["tabPage6"])
            {
                RunExtendedTests(true);
            }
        }

        private void ShutdownServiceBTN_Click(object sender, EventArgs e)
        {
            try
            {
                var isServiceInstalled = GCMSSystem.WinService.CheckInstalled("GlobalCMS");
                if (isServiceInstalled == "Installed")
                {
                    GCMSSystem.WinService.Stop("GlobalCMS");
                }
            }
            catch { }
        }

        private void SyncIDUnlockBTN_Click(object sender, EventArgs e)
        {
            if (SyncIDUnlockBTN.Text == "Unlock") { 
                BrowserSyncID.Enabled = true;
                SyncIDUnlockBTN.Text = "Lock";
            }
            else
            {
                BrowserSyncID.Enabled = false;
                SyncIDUnlockBTN.Text = "Unlock";
            }
        }

        private void UpdateInternetModeBTN_Click(object sender, EventArgs e)
        {
            // Read INI File for Config.ini
            string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
            var MyIni = new IniFile(iniFile);

            // Printer Override
            var lowInternetMode = LowInternetMode.Text;
            var lowInternetModeDelay = LowInternetModeDelay.Text;
            MyIni.Write("poorInternet", lowInternetMode, "Monitor");
            MyIni.Write("poorInternetDelay", lowInternetModeDelay, "Monitor");

            if (lowInternetMode == "Enabled") { MainForm.isUselessInternet = true; }
            if (lowInternetMode == "Disabled") { MainForm.isUselessInternet = false; }

            MainForm.FrmObj.CheckSNAP.Interval = 60000 + (Convert.ToInt32(lowInternetModeDelay) * 1000);
            MainForm.snapDelay = Convert.ToInt32(lowInternetModeDelay);
            MessageBox.Show("Internet Mode Set", "Engineering Tools", MessageBoxButtons.OK);
        }

        private void PrintTestBTN_Click(object sender, EventArgs e)
        {
            Printer.PrintTestPage(null, null);
        }

        private void UnlockUnitSecurityBTN_Click(object sender, EventArgs e)
        {
            IniFile MyIni = new IniFile(iniFile);
            var shellStatus = "On";
            var unlockBTN = UnlockUnitSecurityBTN.Text;

            if (unlockBTN == "Enable Shell Security") {
                UnlockUnitSecurityBTN.Text = "Disable Shell Security";
                shellStatus = "On";
                Taskbar.Hide();
            } else {
                UnlockUnitSecurityBTN.Text = "Enable Shell Security";
                shellStatus = "Off";
                Taskbar.Show();
            }
            MyIni.Write("ShellSecurity", shellStatus, "Monitor");
            // System.Windows.Forms.Application.Restart();
            // Environment.Exit(0);
        }

        private void MonitorLogsBTN_Click(object sender, EventArgs e)
        {
            if (!GCMSSystem.CheckOpened("LogViewerMonitor"))
            {
                Form LogViewerMonitor = new LogViewerMonitor();
                LogViewerMonitor.Show();
            }
        }

        private void SignageLogsBTN_Click(object sender, EventArgs e)
        {
            if (!GCMSSystem.CheckOpened("LogViewerSignage"))
            {
                Form LogViewerSignage = new LogViewerSignage();
                LogViewerSignage.Show();
            }
        }

        private void BrowserLogsBTN_Click(object sender, EventArgs e)
        {
            if (!GCMSSystem.CheckOpened("LogViewerBrowser"))
            {
                Form LogViewerBrowser = new LogViewerBrowser();
                LogViewerBrowser.Show();
            }
        }

        private void RestartAppBTN_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Restart();
            Environment.Exit(0);
        }

        private void UpdaterNetworkSaveBTN_Click(object sender, EventArgs e)
        {
            IniFile MyIni = new IniFile(iniFile);
            var updaterNetworkOption = UpdaterNetwork.Text;
            MyIni.Write("UpdateNetwork", updaterNetworkOption, "Monitor");
            MainForm.NetworkOverride = updaterNetworkOption;
            MessageBox.Show("Updater Network Set", "Engineering Tools", MessageBoxButtons.OK);
        }

        private void PowerUpdateSettingsBTN_Click(object sender, EventArgs e)
        {
            var powerTypeValue = "Virtual";
            var powerStatus = PowerStatusDrpDwn.Text;
            var powerType = PowerScreenTypeDrpDwn.Text;
            if (powerType == "Software Emulation") { powerTypeValue = "Virtual"; }
            if (powerType == "RS232 Serial Cable") { powerTypeValue = "RS232"; }

            IniFile MyIni = new IniFile(powerFile);
            MyIni.Write("Status", powerStatus, "System");
            MyIni.Write("Type", powerTypeValue, "System");

            MessageBox.Show("Power Configuration Updated", "Engineering Tools", MessageBoxButtons.OK);
        }

        private void PowerGrabPowerCnfLocalBTN_Click(object sender, EventArgs e)
        {
            IniFile PowerIni = new IniFile(powerFile);
            FrmObj.PowerINIListView.Items.Clear();
            GenPowerConfigView(PowerIni);
            MessageBox.Show("Power Configuration Loaded From Local", "Engineering Tools", MessageBoxButtons.OK);
        }

        private void PowerGrabPowerCnfRemoteBTN_Click(object sender, EventArgs e)
        {
            IniFile PowerIni = new IniFile(powerFile);
            FrmObj.PowerINIListView.Items.Clear();
            GCMSSystem.TriggerSystem("UPDATEOFFLINEPWR", false, false);
            GenPowerConfigView(PowerIni);
            MessageBox.Show("Power Configuration Loaded From Remote Server", "Engineering Tools", MessageBoxButtons.OK);
        }

        private void PowerScrPowerOnBTN_Click(object sender, EventArgs e)
        {
            IniFile PowerIni = new IniFile(powerFile);
            var powerType = PowerIni.Read("Type", "System");
            if (powerType == "Virtual") { GCMSSystem.TriggerSystem("MONON", false, false); }
            if (powerType == "RS232") { GCMSSystem.TriggerSystem("SCREENON", false, true); }
            MessageBox.Show("Power Screen On", "Engineering Tools", MessageBoxButtons.OK);
        }

        private void PowerScrPowerOffBTN_Click(object sender, EventArgs e)
        {
            IniFile PowerIni = new IniFile(powerFile);
            var powerType = PowerIni.Read("Type", "System");
            if (powerType == "Virtual") { GCMSSystem.TriggerSystem("MONOFF", false, false); }
            if (powerType == "RS232") { GCMSSystem.TriggerSystem("SCREENOFF", false, true); }
            MessageBox.Show("Power Screen Off", "Engineering Tools", MessageBoxButtons.OK);
        }

        private void PowerRebootPCBTN_Click(object sender, EventArgs e)
        {
            GCMSSystem.TriggerSystem("REBOOT", false, false);
        }

        private void MultiscreenBTN_Click(object sender, EventArgs e)
        {
            if (!GCMSSystem.CheckOpened("Multiscreen"))
            {
                Form Multiscreen = new Multiscreen();
                Multiscreen.Show();
            }
        }

        private void GfxOrientationM_CheckedChanged(object sender, EventArgs e)
        {
            MultiscreenBTN.Visible = true;
        }

        private void GfxOrientationL1_CheckedChanged(object sender, EventArgs e)
        {
            MultiscreenBTN.Visible = false;
        }

        private void GfxOrientationP1_CheckedChanged(object sender, EventArgs e)
        {
            MultiscreenBTN.Visible = false;
        }

        private void GfxOrientationL2_CheckedChanged(object sender, EventArgs e)
        {
            MultiscreenBTN.Visible = false;
        }

        private void GfxOrientationP2_CheckedChanged(object sender, EventArgs e)
        {
            MultiscreenBTN.Visible = false;
        }

        private void AirServerCheckerTimer_Tick(object sender, EventArgs e)
        {
            var machineOS = GCMSSystem.GetOS();
            if (machineOS == "Windows7" || machineOS == "Windows8" || machineOS == "Windows10")
            {
                if (GCMSSystem.AirServer.IsInstaled())
                {
                    AirServerInstallBTN.Enabled = false;
                    if (GCMSSystem.AirServer.IsRunning())
                    {
                        AirServerUninstallBTN.Enabled = false;
                        AirServerStopBTN.Enabled = true;
                        AirServerPasscode.Text = "Code: " + GCMSSystem.AirServer.Passcode();
                        AirServerInstallStatus.Text = "RUNNING";
                        AirServerInstallStatus.ForeColor = Color.FromArgb(0, 192, 0);
                        AirServerChgPassBTN.Enabled = true;
                        AirServerStartBTN.Enabled = false;
                    }
                    else
                    {
                        AirServerUninstallBTN.Enabled = true;
                        AirServerStopBTN.Enabled = false;
                        AirServerPasscode.Text = "Code: " + GCMSSystem.AirServer.Passcode();
                        AirServerInstallStatus.Text = "INSTALLED";
                        AirServerInstallStatus.ForeColor = Color.FromArgb(0, 192, 0);
                        AirServerChgPassBTN.Enabled = true;
                        AirServerStartBTN.Enabled = true;
                    }
                }
                else
                {
                    AirServerInstallBTN.Enabled = true;
                    AirServerUninstallBTN.Enabled = false;
                    AirServerStartBTN.Enabled = false;
                    AirServerStopBTN.Enabled = false;
                    AirServerChgPassBTN.Enabled = false;
                    AirServerPasscode.Text = "";
                    AirServerInstallStatus.Text = "NOT INSTALLED";
                    AirServerInstallStatus.ForeColor = Color.FromArgb(192, 0, 0);
                }
            }
        }
    }
}
