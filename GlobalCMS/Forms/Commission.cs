using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Net;
using Microsoft.Win32;
using System.Threading;
using System.Collections.Specialized;
// using System.Drawing;
// using IronBarCode;

namespace GlobalCMS
{
    public partial class Commission : Form
    {
        public Commission()
        {
            InitializeComponent();
            string version = About.GetVersion("Main");                              // Monitor Version
            string subversion = About.GetVersion("Subversion");                     // Monitor Sub Version

            string signageVersion;
            string signageSubVersion;
            using (StreamReader streamReader = new StreamReader(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "version.txt"), Encoding.UTF8))
            {
                signageVersion = streamReader.ReadToEnd();
            }
            using (StreamReader streamReader = new StreamReader(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "subversion.txt"), Encoding.UTF8))
            {
                signageSubVersion = streamReader.ReadToEnd();
            }

            string versionString = "Monitor: " + version + " (" + subversion + ")  /  Signage: " +signageVersion+ " (" + signageSubVersion + ")";
            VersionLabel.Text = versionString;
        }

        private void Commission_Load(object sender, EventArgs e)
        {
            FrmObj = this;

            // Bring to Front and Focus
            BringToFront();
            Focus();
            Activate();

            string localIP = GCMSSystem.GetIP("LAN");                              // Global Variable
            string vpnIP = GCMSSystem.GetIP("VPN");                                // Global Variable
            string wanIP = GCMSSystem.GetIP("WAN");                                // Global Variable

            var macAddr = GCMSSystem.GetMACAddress();
            macLabel.Text = macAddr;

            // Internal (LAN) IP Details
            using (System.Net.Sockets.Socket socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, 0))
            {
                socket.Connect("api.globalcms.co.uk", 80);
                System.Net.IPEndPoint endPoint = socket.LocalEndPoint as System.Net.IPEndPoint;
                localIP = endPoint.Address.ToString();
                if (!localIP.Contains("192.168.") && !localIP.Contains("10.") && !localIP.Contains("172.32.") && !localIP.Contains("172.16.8.") && !localIP.Contains("172.16.73.") && !localIP.Contains("172.30.") && !localIP.Contains("172.17.") && !localIP.Contains("172.20."))
                {
                    localIP = "Not Connected";
                }
                devLAN.Text = localIP;
            }
            // Secure (VPN) IP Details
            using (System.Net.Sockets.Socket socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, 0))
            {
                socket.Connect("172.16.0.2", 80);
                System.Net.IPEndPoint endPoint = socket.LocalEndPoint as System.Net.IPEndPoint;
                vpnIP = endPoint.Address.ToString();
                if (!vpnIP.Contains("172.16."))
                {
                    vpnIP = "Not Connected";
                }
                devVPN.Text = vpnIP;
            }
            // Internet (WAN) IP Details
            devWAN.Text = wanIP;

        }

        private void ComTimer_Tick(object sender, EventArgs e)
        {

            // Tick Box's
            int installRDP = checkedListBox1.Items.IndexOf("Installing Remote Desktop");
            int installDeviceID = checkedListBox1.Items.IndexOf("Setting Up Device ID");
            int installConfigINI = checkedListBox1.Items.IndexOf("Installing Configuration File");
            int installVPN = checkedListBox1.Items.IndexOf("Installing Secure Network");
            int installBiosHardware = checkedListBox1.Items.IndexOf("Submitting Bios and System Information to Server");
            int installBackupConfigINI = checkedListBox1.Items.IndexOf("Creating Configuration Backup Failsafe");
            int installThirdParty = checkedListBox1.Items.IndexOf("Installing 3rd Party Applications");

            var macAddr = GCMSSystem.GetMACAddress();
            // If the device doesnt yet have a MAC Address meaning the NIC hasnt yet fully connected thus not being an Active Network Connection
            // Hold the System until the MAC Address is found and locked in, as this is a timer we should just return the function null until such
            // time as the MAC is found
            macLabel.Text = macAddr;
            var localIP = GCMSSystem.GetIP("LAN");
            var vpnIP = GCMSSystem.GetIP("VPN");
            var wanIP = GCMSSystem.GetIP("WAN");
            // Internal (LAN) IP Details
            using (System.Net.Sockets.Socket socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, 0))
            {
                socket.Connect("api.globalcms.co.uk", 80);
                System.Net.IPEndPoint endPoint = socket.LocalEndPoint as System.Net.IPEndPoint;
                localIP = endPoint.Address.ToString();
                if (!localIP.Contains("192.168.") && !localIP.Contains("10.") && !localIP.Contains("172.32.") && !localIP.Contains("172.16.8.") && !localIP.Contains("172.16.73.") && !localIP.Contains("172.30.") && !localIP.Contains("172.17.") && !localIP.Contains("172.20."))
                {
                    localIP = "Not Connected";
                }
                devLAN.Text = localIP;
            }
            // Secure (VPN) IP Details
            using (System.Net.Sockets.Socket socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, 0))
            {
                socket.Connect("172.16.0.2", 80);
                System.Net.IPEndPoint endPoint = socket.LocalEndPoint as System.Net.IPEndPoint;
                vpnIP = endPoint.Address.ToString();
                if (!vpnIP.Contains("172.16."))
                {
                    vpnIP = "Not Connected";
                }
                devVPN.Text = vpnIP;
            }
            // Internet (WAN) IP Details
            devWAN.Text = wanIP;

            if (macLabel.Text == "00:00:00:00:00:00" || GCMSSystem.RemoveWhitespace(macLabel.Text) == "")
            {
                macAddr = GCMSSystem.GetMACAddress();
                macLabel.Text = macAddr;
            }
            if (macAddr == "" || macAddr == null || macAddr == "null") {
                System.Windows.Forms.Application.Restart();
                Environment.Exit(0);
            }

            var networkURL = "http://172.16.0.2";
            // Styling a QR code and adding annotation text
            // var MyBarCode = BarcodeWriter.CreateBarcode(macAddr, BarcodeWriterEncoding.Code128);
            // MyBarCode.AddBarcodeValueTextBelowBarcode();
            // MyBarCode.SetMargins(5);
            // MyBarCode.ChangeBackgroundColor(Color.FromArgb(0, Color.Black));
            // MyBarCode.ResizeTo(175, 65);
            // TheBarCode.Image = MyBarCode.Render();
            // return;

            // Check if we can get to the VPN Network - If Not Failover to Internet Based Commission
            var pingTest = Ping(networkURL);
            if (!pingTest)
            {
                networkURL = "https://api.globalcms.co.uk";
            }

            var responseString1 = PreSetup(macAddr, networkURL, "PartA", "");
            // Debug.WriteLine("PreSetup - Part A : " + responseString1);
            var responseString2 = PreSetup(macAddr, networkURL, "PartB", responseString1);              // This captures the New CLIENTID
            // Debug.WriteLine("PreSetup - Part B : " + responseString2);
            if (responseString2 != "NONE" && responseString2 != "" && (!responseString2.Contains("MySQL")))
            {
                InstallationProgressBar.Value = 10;

                // System has detected that the device has been triggered for commissioning
                triggerStatusTxt.Text = "Commission Triggered";
                triggerStatusTxt.ForeColor = System.Drawing.Color.FromArgb(0, 192, 0);

                // Step 1 - Download / Install and Config TightVNC for Remote Desktop
                if (!File.Exists("tightvnc.msi"))
                {
                    DownloadFile(networkURL + "/monitorUpdate/install/tightvnc.msi", "tightvnc.msi");
                }

                var responseString3 = GetVNCPass(networkURL, responseString2);
                // Debug.WriteLine("User VNC Password : " + responseString3);
                var vncInstalled = CheckInstalled("TightVNC");
                // Debug.WriteLine("VNC Installed? : " + Convert.ToString(vncInstalled));
                triggerStatusTxt.Text = "Commission Running";
                if (vncInstalled)
                {
                    // VNC is already installed - we need to therefor uninstall it to take our configuration
                    Process process1 = new Process();
                    process1.StartInfo.FileName = "msiexec";
                    process1.StartInfo.Arguments = "-x \"" + Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\tightvnc.msi\" /quiet";
                    process1.StartInfo.Verb = "runas";
                    process1.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    process1.Start();

                    // VNC isnt installed so we need to start MSIExec to install the software silently
                    Process process2 = new Process();
                    process2.StartInfo.FileName = "msiexec";
                    process2.StartInfo.Arguments = "/quiet /i \"" + Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\tightvnc.msi\" ALLUSERS=\"1\" /quiet /norestart ADDLOCAL=\"Server,Viewer\" VIEWER_ASSOCIATE_VNC_EXTENSION=1 SERVER_REGISTER_AS_SERVICE=1 SERVER_ADD_FIREWALL_EXCEPTION=1 VIEWER_ADD_FIREWALL_EXCEPTION=1 SERVER_ALLOW_SAS=1 SET_USEVNCAUTHENTICATION=1 VALUE_OF_USEVNCAUTHENTICATION=1 SET_PASSWORD=1 VALUE_OF_PASSWORD=" + responseString3 + " SET_USECONTROLAUTHENTICATION=1 VALUE_OF_USECONTROLAUTHENTICATION=1 SET_CONTROLPASSWORD=1 VALUE_OF_CONTROLPASSWORD=" + responseString3;
                    process2.StartInfo.Verb = "runas";
                    process2.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    process2.Start();

                    // Tick Box and Adjust the Progress Bar
                    InstallationProgressBar.Value = 20;
                    if (installRDP >= 0)
                    {
                        checkedListBox1.SetItemChecked(installRDP, true);
                    }
                }
                else
                {
                    // VNC isnt installed so we need to start MSIExec to install the software silently
                    Process process = new Process();
                    process.StartInfo.FileName = "msiexec";
                    process.StartInfo.Arguments = "/quiet /i \"" + Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\tightvnc.msi\" ALLUSERS=\"1\" /quiet /norestart ADDLOCAL=\"Server,Viewer\" VIEWER_ASSOCIATE_VNC_EXTENSION=1 SERVER_REGISTER_AS_SERVICE=1 SERVER_ADD_FIREWALL_EXCEPTION=1 VIEWER_ADD_FIREWALL_EXCEPTION=1 SERVER_ALLOW_SAS=1 SET_USEVNCAUTHENTICATION=1 VALUE_OF_USEVNCAUTHENTICATION=1 SET_PASSWORD=1 VALUE_OF_PASSWORD=" + responseString3 + " SET_USECONTROLAUTHENTICATION=1 VALUE_OF_USECONTROLAUTHENTICATION=1 SET_CONTROLPASSWORD=1 VALUE_OF_CONTROLPASSWORD=" + responseString3;
                    process.StartInfo.Verb = "runas";
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    process.Start();
                    // Tick Box and Adjust the Progress Bar
                    InstallationProgressBar.Value = 20;
                    if (installRDP >= 0)
                    {
                        checkedListBox1.SetItemChecked(installRDP, true);
                    }
                }

                // Are we pushing a new config or an old one?
                bool isPushConfig = CheckForPush(macAddr, networkURL, responseString2);
                // Debug.WriteLine("Is Device Being Pushed a Config? : " + isPushConfig.ToString());

                // Step 2 - Call FullSetup.php to trigger setup of all the details required
                if (!isPushConfig)
                {
                    // If new device we need to set it up on the server rather than grabbing the details
                    var responseString4 = FullSetup(macAddr, networkURL, responseString2);
                    // Debug.WriteLine("Setup? : " + Convert.ToString(responseString4));
                }
                InstallationProgressBar.Value = 30;
                if (installDeviceID >= 0)
                {
                    checkedListBox1.SetItemChecked(installDeviceID, true);
                }

                // Step 3 - Create config.ini File
                var responseString5 = GenINI(macAddr, networkURL, responseString2);
                // Debug.WriteLine("INI String : " + responseString5);
                string[] varINI = responseString5.Split(',');
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                MyIni.Write("clientID", varINI[0], "Monitor");
                MyIni.Write("deviceName", varINI[1], "Monitor");
                MyIni.Write("deviceUUID", varINI[2], "Monitor");
                MyIni.Write("deviceMAC", macAddr, "Monitor");
                MyIni.Write("hardwareMAC", macAddr, "Monitor");
                MyIni.Write("poorInternet", "Disabled", "Monitor");
                MyIni.Write("poorInternetDelay", "0", "Monitor");
                MyIni.Write("ShellSecurity", "On", "Monitor");
                MyIni.Write("UpdateNetwork", "Auto", "Monitor");

                MyIni.Write("licType", varINI[4], "Licence");
                MyIni.Write("AS", "NotInstalled", "Licence");

                MyIni.Write("Signage", "Enabled", "Serv");
                MyIni.Write("VPN", "Enabled", "Serv");
                MyIni.Write("OHM", "Enabled", "Serv");
                MyIni.Write("Env", "Disabled", "Serv");
                MyIni.Write("Mirroring", "Enabled", "Serv");

                MyIni.Write("platform", varINI[3], "Sign");
                MyIni.Write("CH", "30_45", "Sign");
                MyIni.Write("SignageLoader", "2", "Sign");

                MyIni.Write("selectedNetwork", "Primary", "Network");
                MyIni.Write("numberAttempts", "0", "Network");
                MyIni.Write("maintMode", varINI[6], "Network");
                MyIni.Write("powersaveMode", varINI[7], "Network");
                MyIni.Write("powersaveMode2", varINI[7], "Network");
                MyIni.Write("timezone", "GMT Standard Time", "Network");

                MyIni.Write("SkinID", "Default", "Skin");

                MyIni.Write("CookieCleaner", "Off", "Interactive");

                MyIni.Write("Load", "Default", "Browser");
                MyIni.Write("Referer", "", "Browser");
                MyIni.Write("Debug", "Off", "Browser");
                MyIni.Write("Keyboard", "Default", "Browser");
                MyIni.Write("SSL", "Off", "Browser");
                MyIni.Write("ExRAM", "Off", "Browser");

                MyIni.Write("Default", "On", "Printer");
                MyIni.Write("Copies", "1", "Printer");
                MyIni.Write("PrintIn", "Portrait", "Printer");
                MyIni.Write("PrintType", "Black and White", "Printer");
                MyIni.Write("PaperSize", "A4", "Printer");
                MyIni.Write("PaperMargin", "1.00", "Printer");
                MyIni.Write("PrinterOverride", "", "Printer");
                MyIni.Write("PrinterAdv", "Off", "Printer");
                MyIni.Write("PrintPreview", "Off", "Printer");

                MyIni.Write("Force", "Off", "Display");
                MyIni.Write("Resolution", "1920x1080", "Display");
                MyIni.Write("Scaling", "100", "Display");
                MyIni.Write("Orientation", "Landscape", "Display");

                MyIni.Write("MasterLevel", "1", "Audio");

                MyIni.Write("Setup", "Yes", "Setup");

                GCMSSystem.Skin.Update();
                try
                {
                    File.WriteAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\nodejsData\\syncid", varINI[5]);
                }
                catch { }

                InstallationProgressBar.Value = 40;
                if (installConfigINI >= 0)
                {
                    checkedListBox1.SetItemChecked(installConfigINI, true);
                }
                // Debug.WriteLine("INI File : Written");

                string user = GCMSSystem.Chrome.GetCurrentMachineUser();
                string path = Path.Combine("C:\\", "Users", user, "AppData", "Roaming", "pritunl", "profiles");

                // Step 4 - Run VPN Create User and to Download VPN Files to Machine
                if (varINI[4] == "SEC")
                {
                    var responseString6 = "";
                    if (isPushConfig)
                    {
                        responseString6 = PushVPN(macAddr, networkURL, responseString2);
                    }
                    else
                    {
                        responseString6 = CreateVPN(macAddr, networkURL, responseString2);
                    }
                    // Debug.WriteLine("VPN Server Responce? : " + responseString6);
                    DirectoryInfo di = new DirectoryInfo(path);
                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                    Debug.WriteLine("VPN System : " + responseString6);
                    if (!File.Exists(path + "\\" + responseString6 + ".conf"))
                    {
                        DownloadFile(networkURL + "/v2/vpnSystem/" + responseString6 + ".conf", path + "\\" + responseString6 + ".conf");
                    }
                    if (!File.Exists(path + "\\" + responseString6 + ".ovpn"))
                    {
                        DownloadFile(networkURL + "/v2/vpnSystem/" + responseString6 + ".ovpn", path + "\\" + responseString6 + ".ovpn");
                    }
                }

                InstallationProgressBar.Value = 50;
                if (installVPN >= 0)
                {
                    checkedListBox1.SetItemChecked(installVPN, true);
                }

                // Step 5 - Get and Submit Bios Information & System Information
                using (StreamWriter sw = File.AppendText("info~" + macAddr.Replace(":", "_") + ".txt"))
                {
                    sw.WriteLine("BIOS Properties:");
                    sw.WriteLine("-----------------------------------------------------------------------------");
                    sw.WriteLine("BIOS Name: " + BIOSInfo.Name);
                    sw.WriteLine("BIOS Version: " + BIOSInfo.BIOSVersion);
                    sw.WriteLine("Manufacturer: " + BIOSInfo.Manufacturer);
                    sw.WriteLine("SMBIOSBIOSVersion: " + BIOSInfo.SMBIOSBIOSVersion);
                }
                InstallationProgressBar.Value = InstallationProgressBar.Value + 10;
                if (installBiosHardware >= 0)
                {
                    checkedListBox1.SetItemChecked(installBiosHardware, true);
                }
                // UploadInfo("info~" + macAddr.Replace(":", "_") + ".txt");                // Needs a rework and rethink, could be collected each time the machine boots up so incase of hardware changes
                // Debug.WriteLine("Sending System Information To Server : Complete");

                // Step 6 - Create Config.ini Backup File for Failsafe System
                try
                {
                    File.Copy(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\config\\config.ini", Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\config\\config.ini.backup", true);
                }
                catch { }

                InstallationProgressBar.Value = 75;
                if (installBackupConfigINI >= 0)
                {
                    checkedListBox1.SetItemChecked(installBackupConfigINI, true);
                }

                // Step 7 - Install any 3rd Party Software
                if (!File.Exists("python.msi"))
                {
                    DownloadFile(networkURL + "/monitorUpdate/install/python.msi", "python.msi");
                }
                var pythonInstalled = CheckInstalled("Python");
                // Debug.WriteLine("Python Installed? : " + Convert.ToString(pythonInstalled));
                if (pythonInstalled)
                {
                    // Python is already installed - we need to therefor uninstall it to take our configuration
                    Process process3 = new Process();
                    process3.StartInfo.FileName = "msiexec";
                    process3.StartInfo.Arguments = "-x \"" + Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\python.msi\" /quiet /norestart ADDLOCAL=ALL";
                    process3.StartInfo.Verb = "runas";
                    process3.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    process3.Start();
                    process3.WaitForExit(60000);

                    // Python isnt installed so we need to start MSIExec to install the software silently
                    Process process4 = new Process();
                    process4.StartInfo.FileName = "msiexec";
                    process4.StartInfo.Arguments = "/quiet /i \"" + Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\python.msi\" ALLUSERS=\"1\" /quiet /norestart ADDLOCAL=ALL";
                    process4.StartInfo.Verb = "runas";
                    process4.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    process4.Start();

                    // Tick Box and Adjust the Progress Bar
                    InstallationProgressBar.Value = InstallationProgressBar.Value + 10;
                    if (installThirdParty >= 0)
                    {
                        checkedListBox1.SetItemChecked(installThirdParty, true);
                    }
                    process4.WaitForExit(60000);
                }
                else
                {
                    // Python isnt installed so we need to start MSIExec to install the software silently
                    Process process4 = new Process();
                    process4.StartInfo.FileName = "msiexec";
                    process4.StartInfo.Arguments = "/quiet /i \"" + Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\python.msi\" ALLUSERS=\"1\" /quiet /norestart ADDLOCAL=ALL";
                    process4.StartInfo.Verb = "runas";
                    process4.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                    process4.Start();
                    // Tick Box and Adjust the Progress Bar
                    InstallationProgressBar.Value = InstallationProgressBar.Value + 10;
                    if (installThirdParty >= 0)
                    {
                        checkedListBox1.SetItemChecked(installThirdParty, true);
                    }
                    process4.WaitForExit(60000);
                }

                // Step 8 - Remove any leftover EXE Files to clean up after the Commission
                try
                {
                    File.Delete(@Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\python.msi");
                }
                catch { }
                try
                {
                    File.Delete(@Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\tightvnc.msi");
                }
                catch { }
                try
                {
                    File.Delete(@Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\info~" + macAddr.Replace(":", "_") + ".txt");
                }
                catch { }

                InstallationProgressBar.Value = 100;

                triggerStatusTxt.Text = "Commission Complete";
                ComTimer.Stop();

                // Set the Timezone so that on reboot it has the correct TimezoneIDs
                try
                {
                    SetSystemTimeZone.SetTimezone(macAddr, varINI[2], networkURL);
                }
                catch { }

                // Set Screenscaling to 100
                try
                {
                    ScreenScaling.SetScaleFactor("100");
                }
                catch { }

                // Set the PCs Name, Requires a Reboot to lock in, so we do this before the Reboot Command is issued
                try
                {
                    SetMachineName(varINI[1]);
                }
                catch { }

                Thread.Sleep(5000);                // Wait for 5 seconds

                // Before we do the final Reboot.
                // Test all variables, and if any are messed up, then simply delete the config file/backup config file
                // and then have the system auto recommision the device once rebooted
                bool finalChecksumPass = false;
                var newClientID = MyIni.Read("clientID", "Monitor");
                var newDeviceName = MyIni.Read("deviceName", "Monitor");
                var newDeviceUUID = MyIni.Read("deviceUUID", "Monitor");
                var newDeviceMAC = MyIni.Read("deviceMAC", "Monitor");
                var newHardwareMAC = MyIni.Read("hardwareMAC", "Monitor");
                var newLicType = MyIni.Read("licType", "Licence");
                var newPlatform = MyIni.Read("platform", "Sign");
                var newPwrSave = MyIni.Read("powersaveMode", "Network");
                // Check all above Variables and if any are not what should be expected, then the config has corrupted and needs regen
                // after reboot. 
                if (newClientID != "" && newClientID != "NotFound" && newClientID != "Error") { finalChecksumPass = true; } else { finalChecksumPass = false; }
                if (newDeviceName != "" && newDeviceName != "NotFound" && newDeviceName != "Error") { finalChecksumPass = true; } else { finalChecksumPass = false; }
                if (newDeviceUUID != "" && newDeviceUUID != "NotFound" && newDeviceUUID != "Error") { finalChecksumPass = true; } else { finalChecksumPass = false; }
                if (newDeviceMAC != "" && newDeviceMAC != "NotFound" && newDeviceMAC != "Error") { finalChecksumPass = true; } else { finalChecksumPass = false; }
                if (newHardwareMAC != "" && newHardwareMAC != "NotFound" && newHardwareMAC != "Error") { finalChecksumPass = true; } else { finalChecksumPass = false; }
                if (newLicType != "" && newLicType != "NotFound" && newLicType != "Error") { finalChecksumPass = true; } else { finalChecksumPass = false; }
                if (newPlatform != "" && newPlatform != "NotFound" && newPlatform != "Error") { finalChecksumPass = true; } else { finalChecksumPass = false; }
                if (newPwrSave != "" && newPwrSave != "NotFound" && newPwrSave != "Error") { finalChecksumPass = true; } else { finalChecksumPass = false; }

                // If everything has passed then ignore function below.
                // However if the finalChecksumPass is still false, then we need to remove the config.ini file for regen on reboot
                if (!finalChecksumPass)
                {
                    // Make 100% sure that the file exists before we try to delete it
                    if (File.Exists(iniFile))
                    {
                        try { File.Delete(iniFile); } catch { }
                    }
                }

                try
                {
                    ProcessStartInfo proc = new ProcessStartInfo
                    {
                        FileName = "shutdown.exe",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        Arguments = "-r -t 0"
                    };
                    Process.Start(proc);
                }
                catch { }
            }
        }

        // Private Form Only Functions
        public class SetSystemTimeZone
        {
            public static string SetTimezone(string macAddy, string uuidStr, string networkURL)
            {
                string timeZoneId;
                using (var client = new WebClient())
                {
                    // Create the $_POST Data for the HTTP Request
                    var values = new NameValueCollection
                    {
                        ["devUUID"] = uuidStr,
                        ["hardwareMAC"] = macAddy
                    };

                    try
                    {
                        var response = client.UploadValues(networkURL + "/v2/getTimezone.php", values);
                        timeZoneId = Encoding.UTF8.GetString(response);
                    }
                    catch
                    {
                        timeZoneId = "Error";
                    }
                }
                if (timeZoneId != "Error")
                {
                    //timeZoneId = Regex.Replace(timeZoneId, @"\t|\n|\r", "");
                    timeZoneId = timeZoneId.Trim();
                    var process = Process.Start(new ProcessStartInfo
                    {
                        FileName = "tzutil.exe",
                        Arguments = "/s \"" + timeZoneId + "\"",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    });
                    process.Start();
                    string output = "";
                    while (!process.StandardOutput.EndOfStream)
                    {
                        string line = process.StandardOutput.ReadLine();
                        output = output + line + "\n";
                    }
                    // Console.WriteLine(output);
                    // process.WaitForExit();
                     
                    // Read INI File for Config.ini
                    string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                    IniFile MyIni = new IniFile(iniFile);
                    MyIni.Write("timezone", "", "Network");
                    MyIni.Write("timezone", timeZoneId, "Network");
                }
                return "Done";
            }
        }

        private bool Ping(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Timeout = 3000;
                request.AllowAutoRedirect = false; // find out if this site is up and don't follow a redirector
                request.Method = "HEAD";

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
        private string PreSetup(string macAddr, string networkURL, string whichPart, string prevPartOutput)
        {
            string responseString;
            using (var client = new WebClient())
            {
                // Create the $_POST Data for the HTTP Request
                var values = new NameValueCollection
                {
                    ["function"] = whichPart,
                    ["hardwareMAC"] = macAddr
                };

                try
                {
                    var response = client.UploadValues(networkURL + "/v2/preSetup.php", values);
                    responseString = Encoding.UTF8.GetString(response);
                    responseString = GCMSSystem.RemoveWhitespace(responseString);
                }
                catch
                {
                    responseString = "Error";
                }
            }
            return responseString;
        }
        private string FullSetup(string macAddr, string networkURL, string clientID)
        {
            string responseString;
            using (var client = new WebClient())
            {
                // Create the $_POST Data for the HTTP Request
                var values = new NameValueCollection
                {
                    ["clientID"] = clientID,
                    ["hardwareMAC"] = macAddr
                };

                try
                {
                    var response = client.UploadValues(networkURL + "/v2/newDevice.php", values);
                    responseString = Encoding.UTF8.GetString(response);
                    responseString = GCMSSystem.RemoveWhitespace(responseString);
                }
                catch
                {
                    responseString = "Error";
                }
            }
            return responseString;
        }
        private string GenINI(string macAddr, string networkURL, string clientID)
        {
            string responseString;
            using (var client = new WebClient())
            {
                // Create the $_POST Data for the HTTP Request
                var values = new NameValueCollection
                {
                    ["clientID"] = clientID,
                    ["hardwareMAC"] = macAddr
                };

                try
                {
                    var response = client.UploadValues(networkURL + "/v2/genINI.php", values);
                    responseString = Encoding.UTF8.GetString(response);
                    responseString = GCMSSystem.RemoveWhitespace(responseString);
                }
                catch
                {
                    responseString = "Error";
                }
            }
            return responseString;
        }
        private bool CheckForPush(string macAddr, string networkURL, string clientID)
        {
            string responseString;
            bool theOut = false;
            using (var client = new WebClient())
            {
                // Create the $_POST Data for the HTTP Request
                var values = new NameValueCollection
                {
                    ["clientID"] = clientID,
                    ["hardwareMAC"] = macAddr
                };

                try
                {
                    var response = client.UploadValues(networkURL + "/v2/checkMAC.php", values);
                    responseString = Encoding.UTF8.GetString(response);
                    responseString = GCMSSystem.RemoveWhitespace(responseString);
                    if (responseString == "NEW")
                    {
                        theOut = false;
                    }
                    if (responseString == "REGEN")
                    {
                        theOut = true;
                    }
                }
                catch
                {
                    theOut = false;
                }
            }
            return theOut;
        }
        private string CreateVPN(string macAddr, string networkURL, string clientID)
        {
            string responseString;
            using (var client = new WebClient())
            {
                // Create the $_POST Data for the HTTP Request
                var values = new NameValueCollection
                {
                    ["clientID"] = clientID,
                    ["hardwareMAC"] = macAddr
                };

                try
                {
                    var response = client.UploadValues(networkURL + "/v2/vpnSystem/setupVPN.php", values);
                    responseString = Encoding.UTF8.GetString(response);
                    responseString = GCMSSystem.RemoveWhitespace(responseString);
                }
                catch
                {
                    responseString = "Error";
                }
            }
            return responseString;
        }
        private string PushVPN(string macAddr, string networkURL, string clientID)
        {
            string responseString;
            using (var client = new WebClient())
            {
                // Create the $_POST Data for the HTTP Request
                var values = new NameValueCollection
                {
                    ["clientID"] = clientID,
                    ["hardwareMAC"] = macAddr
                };

                try
                {
                    var response = client.UploadValues(networkURL + "/v2/vpnSystem/pushVPN.php", values);
                    responseString = Encoding.UTF8.GetString(response);
                    responseString = GCMSSystem.RemoveWhitespace(responseString);
                }
                catch
                {
                    responseString = "Error";
                }
            }
            return responseString;
        }
        public string GetVNCPass(string networkURL, string clientID)
        {
            string responseString;
            using (var client = new WebClient())
            {
                // Create the $_POST Data for the HTTP Request
                var values = new NameValueCollection
                {
                    ["clientID"] = clientID
                };

                try
                {
                    var response = client.UploadValues(networkURL + "/v2/vncPass.php", values);
                    responseString = Encoding.UTF8.GetString(response);
                    responseString = GCMSSystem.RemoveWhitespace(responseString);
                }
                catch
                {
                    responseString = "Error";
                }
            }
            return responseString;
        }
        private string DownloadFile(string downloadFile, string savedFile)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(downloadFile, savedFile);
            }
            return "Failed";
        }
        public static bool CheckInstalled(string c_name)
        {
            string displayName;

            string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey);
            if (key != null)
            {
                foreach (RegistryKey subkey in key.GetSubKeyNames().Select(keyName => key.OpenSubKey(keyName)))
                {
                    displayName = subkey.GetValue("DisplayName") as string;
                    if (displayName != null && displayName.Contains(c_name))
                    {
                        return true;
                    }
                }
                key.Close();
            }

            registryKey = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
            key = Registry.LocalMachine.OpenSubKey(registryKey);
            if (key != null)
            {
                foreach (RegistryKey subkey in key.GetSubKeyNames().Select(keyName => key.OpenSubKey(keyName)))
                {
                    displayName = subkey.GetValue("DisplayName") as string;
                    if (displayName != null && displayName.Contains(c_name))
                    {
                        return true;
                    }
                }
                key.Close();
            }
            return false;
        }
        public void SetMachineName(string newName)
        {
            var process = Process.Start(new ProcessStartInfo
            {
                FileName = "WMIC.exe",
                Arguments = "computersystem where caption='" + System.Environment.MachineName + "' rename " + newName,
                UseShellExecute = false,
                CreateNoWindow = true
            });
            process.WaitForExit(60000);
        }

        static Commission _frmObj;
        public static Commission FrmObj
        {
            get { return _frmObj; }
            set { _frmObj = value; }
        }

        private void MacTimer_Tick(object sender, EventArgs e)
        {
            if (macLabel.Text == "00:00:00:00:00:00" || macLabel.Text == "")
            {
                var macAddr = GCMSSystem.GetMACAddress();
                macLabel.Text = macAddr;
            }
            var localIP = GCMSSystem.GetIP("LAN");
            var vpnIP = GCMSSystem.GetIP("VPN");
            var wanIP = GCMSSystem.GetIP("WAN");
            // Internal (LAN) IP Details
            using (System.Net.Sockets.Socket socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, 0))
            {
                socket.Connect("api.globalcms.co.uk", 80);
                System.Net.IPEndPoint endPoint = socket.LocalEndPoint as System.Net.IPEndPoint;
                localIP = endPoint.Address.ToString();
                if (!localIP.Contains("192.168.") && !localIP.Contains("10.") && !localIP.Contains("172.32.") && !localIP.Contains("172.16.8.") && !localIP.Contains("172.16.73.") && !localIP.Contains("172.30.") && !localIP.Contains("172.17.") && !localIP.Contains("172.20."))
                {
                    localIP = "Not Connected";
                }
                devLAN.Text = localIP;
            }
            // Secure (VPN) IP Details
            using (System.Net.Sockets.Socket socket = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Dgram, 0))
            {
                socket.Connect("172.16.0.2", 80);
                System.Net.IPEndPoint endPoint = socket.LocalEndPoint as System.Net.IPEndPoint;
                vpnIP = endPoint.Address.ToString();
                if (!vpnIP.Contains("172.16."))
                {
                    vpnIP = "Not Connected";
                }
                devVPN.Text = vpnIP;
            }
            // Internet (WAN) IP Details
            devWAN.Text = wanIP;
        }
    }
}
