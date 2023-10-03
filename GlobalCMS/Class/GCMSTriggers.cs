using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Drawing.Imaging;
using System.Net;
using System.Text;
using System.Windows.Forms;
using CodeScales.Http.Methods;
using CodeScales.Http.Entity;
using CodeScales.Http.Entity.Mime;
using System.Drawing;
using System.Linq;
using System.Collections.Specialized;
using EDIDPull;
using System.Threading.Tasks;
using System.ServiceProcess;
using System.Configuration.Install;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Net.Http;
using System.Management;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using WUApiLib;
using System.Net.Sockets;
using Microsoft.WindowsAPICodePack.ApplicationServices;

namespace GlobalCMS
{
    class GCMSSystem
    {
        internal static IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;
        internal static int WTS_UserName = 5;

        public static Decommission OpenedForm { get; private set; }

        public static string TriggerSystem(string whichTrigger, bool lowPowerTrigger, bool bypassChecksum)
        {
            string networkURL;                                                // Which Network URL to use
            string networkNameShort;                                          // Network Name Short - A Single Letter P (Public) / S (Secure)

            var actualTrigger = RemoveWhitespace(whichTrigger);
            // MAC Address
            var macAddr = GetMACAddress();
            if (actualTrigger == "HELLO")
            {
                // Test Trigger for testing if a machine is receiving commands, without the need to actually trigger anything.
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] HELLO Test String");
            }
            if (actualTrigger == "REBOOT")
            {
                // Reboot Machine Instantly
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Reboot");
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
            if (actualTrigger == "CREATE")
            {
                // For Creating a given backup File
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Create Backup File");
            }
            if (actualTrigger == "REBUILD")
            {
                // For Rebuilding from a given backup File
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Rebuilding From Backup File");
            }
            if (actualTrigger == "CAPTURE")
            {
                // Capture the Screen (GUI Capture)
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] GUI Capture");
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                var macStr = macAddr.Replace(":", "-");
                var uuidStr = MyIni.Read("deviceUUID", "Monitor");
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
                    var pingTest = GCMSSystem.Ping(networkURL);
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

                var image = ScreenCapture.CaptureDesktop();
                image.Save(@Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\snapshot_" + uuidStr + ".jpg", ImageFormat.Jpeg);

                // Once the file has been captured send over to the server for storage
                if (!MainForm.isUselessInternet)
                {
                    SendCaptureFile(networkURL, "snapshot_" + uuidStr + ".jpg", uuidStr);
                }
                File.Delete(@Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\snapshot_" + uuidStr + ".jpg");
            }
            if (actualTrigger == "SCREENON")
            {
                // RS232 Monitor ON
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                var macStr = macAddr.Replace(":", "-");
                var uuidStr = MyIni.Read("deviceUUID", "Monitor");
                var MyNetwork = MyIni.Read("licType", "Licence");

                var curPowerSave1 = MyIni.Read("powersaveMode", "Network");
                var curPowerSave2 = MyIni.Read("powersaveMode2", "Network");

                if (bypassChecksum)
                {
                    if (!lowPowerTrigger)
                    {
                        FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Screen On via RS232");
                    }
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
                        var pingTest = GCMSSystem.Ping(networkURL);
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

                    // Trigger the RS232 Serial Command
                    using (var client = new WebClient())
                    {
                        // Create the $_POST Data for the HTTP Request
                        var values = new NameValueCollection
                        {
                            ["clientID"] = MyIni.Read("clientID", "Monitor"),
                            ["deviceID"] = MyIni.Read("deviceName", "Monitor"),
                            ["deviceUUID"] = MyIni.Read("deviceUUID", "Monitor"),
                            ["theCtrlCode"] = "Screen",
                        };

                        var responseString = "";
                        try
                        {
                            var response = client.UploadValues(networkURL + "/v2/getControlCodeData.php", values);
                            responseString = Encoding.Default.GetString(response);
                            responseString = GCMSSystem.RemoveWhitespace(responseString);
                        }
                        catch
                        {
                            responseString = "Error";
                        }

                        if (responseString != "Error")
                        {
                            if (!File.Exists("logs/ctrlCodes/" + responseString + "_powerOn.py") && responseString != "Error")
                            {
                                DownloadFileSingle(networkURL + "/monitorUpdate/scripts/controlcodes/" + responseString + "_powerOn.py", "logs/ctrlCodes/" + responseString + "_powerOn.py");
                            }

                            // Trigger RS232 Python Script
                            try
                            {
                                var pythonRun = Python.Run("logs/ctrlCodes/" + responseString + "_powerOn.py");
                            }
                            catch { FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [ERR] Screen On via Python Failed"); }
                        }
                    }

                    // Set Params
                    MyIni.Write("powersaveMode", "FALSE", "Network");
                    MyIni.Write("powersaveMode2", "FALSE", "Network");

                    if (!MainForm.isInLockdown && !CheckOpened("SignageBrowser"))
                    {
                        var isMaintMode = MyIni.Read("maintMode", "Network");
                        if (isMaintMode == "TRUE")
                        {
                            MainForm.FrmObj.powerModeLabel.Text = "Maintenance";
                            MainForm.FrmObj.powerModeLabel.ForeColor = Color.FromArgb(255, 128, 0);
                        }
                        else
                        {
                            MainForm.FrmObj.powerModeLabel.Text = "Normal / Online";
                            MainForm.FrmObj.powerModeLabel.ForeColor = Color.FromArgb(0, 0, 0);
                        }

                        try
                        {
                            if (GCMSSystem.CheckOpened("ScreenLock"))
                            {
                                Application.OpenForms["ScreenLock"].Close();
                            }
                        }
                        catch { }

                        MainForm.FrmObj.CheckSNAP.Interval = 60000 + (MainForm.snapDelay * 1000);
                        MainForm.FrmObj.CheckSNAP.Start();

                        Chrome.UpdatePref();
                        Chrome.Load();
                    }
                }
                if (curPowerSave1 == "FALSE" && curPowerSave2 == "TRUE" && !bypassChecksum)
                {
                    if (!lowPowerTrigger)
                    {
                        FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Screen On via RS232");
                    }
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
                        var pingTest = GCMSSystem.Ping(networkURL);
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

                    // Trigger the RS232 Serial Command
                    using (var client = new WebClient())
                    {
                        // Create the $_POST Data for the HTTP Request
                        var values = new NameValueCollection
                        {
                            ["clientID"] = MyIni.Read("clientID", "Monitor"),
                            ["deviceID"] = MyIni.Read("deviceName", "Monitor"),
                            ["deviceUUID"] = MyIni.Read("deviceUUID", "Monitor"),
                            ["theCtrlCode"] = "Screen",
                        };

                        var responseString = "";
                        try
                        {
                            var response = client.UploadValues(networkURL + "/v2/getControlCodeData.php", values);
                            responseString = Encoding.Default.GetString(response);
                            responseString = GCMSSystem.RemoveWhitespace(responseString);
                        }
                        catch
                        {
                            responseString = "Error";
                        }

                        if (responseString != "Error")
                        {
                            if (!File.Exists("logs/ctrlCodes/" + responseString + "_powerOn.py") && responseString != "Error")
                            {
                                DownloadFileSingle(networkURL + "/monitorUpdate/scripts/controlcodes/" + responseString + "_powerOn.py", "logs/ctrlCodes/" + responseString + "_powerOn.py");
                            }

                            // Trigger RS232 Python Script
                            try
                            {
                                var pythonRun = Python.Run("logs/ctrlCodes/" + responseString + "_powerOn.py");
                            }
                            catch { FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [ERR] Screen On via Python Failed"); }
                        }
                    }

                    // Set Params
                    MyIni.Write("powersaveMode", "FALSE", "Network");
                    MyIni.Write("powersaveMode2", "FALSE", "Network");

                    if (!MainForm.isInLockdown)
                    {
                        var isMaintMode = MyIni.Read("maintMode", "Network");
                        if (isMaintMode == "TRUE")
                        {
                            MainForm.FrmObj.powerModeLabel.Text = "Maintenance";
                            MainForm.FrmObj.powerModeLabel.ForeColor = Color.FromArgb(255, 128, 0);
                        }
                        else
                        {
                            MainForm.FrmObj.powerModeLabel.Text = "Normal / Online";
                            MainForm.FrmObj.powerModeLabel.ForeColor = Color.FromArgb(0, 0, 0);
                        }

                        try
                        {
                            ScreenLock.frmObj.Hide();
                        }
                        catch { }
                        try
                        {
                            ScreenLock.frmObj.Close();
                        }
                        catch { }

                        try
                        {
                            Application.OpenForms["ScreenLock"].Hide();
                        }
                        catch { }
                        try
                        {
                            Application.OpenForms["ScreenLock"].Close();
                        }
                        catch { }

                        MainForm.FrmObj.CheckSNAP.Interval = 60000 + (MainForm.snapDelay * 1000);
                        MainForm.FrmObj.CheckSNAP.Start();

                        var cloneScr = MyIni.Read("Clone", "Display");
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

                        Chrome.UpdatePref();
                        Chrome.Load();
                    }
                }
            }
            if (actualTrigger == "SCREENOFF")
            {
                // RS232 Monitor OFF
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                var macStr = macAddr.Replace(":", "-");
                var uuidStr = MyIni.Read("deviceUUID", "Monitor");
                var MyNetwork = MyIni.Read("licType", "Licence");

                var curPowerSave1 = MyIni.Read("powersaveMode", "Network");
                var curPowerSave2 = MyIni.Read("powersaveMode2", "Network");

                if (bypassChecksum)
                {
                    if (!lowPowerTrigger)
                    {
                        FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Screen Off via RS232");
                    }
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
                        var pingTest = GCMSSystem.Ping(networkURL);
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

                    // Trigger the RS232 Serial Command
                    using (var client = new WebClient())
                    {
                        // Create the $_POST Data for the HTTP Request
                        var values = new NameValueCollection
                        {
                            ["clientID"] = MyIni.Read("clientID", "Monitor"),
                            ["deviceID"] = MyIni.Read("deviceName", "Monitor"),
                            ["deviceUUID"] = MyIni.Read("deviceUUID", "Monitor"),
                            ["theCtrlCode"] = "Screen",
                        };

                        var responseString = "";
                        try
                        {
                            var response = client.UploadValues(networkURL + "/v2/getControlCodeData.php", values);
                            responseString = Encoding.Default.GetString(response);
                            responseString = GCMSSystem.RemoveWhitespace(responseString);
                        }
                        catch
                        {
                            responseString = "Error";
                        }

                        if (responseString != "Error")
                        {
                            if (!File.Exists("logs/ctrlCodes/" + responseString + "_powerOff.py"))
                            {
                                DownloadFileSingle(networkURL + "/monitorUpdate/scripts/controlcodes/" + responseString + "_powerOff.py", "logs/ctrlCodes/" + responseString + "_powerOff.py");
                            }

                            try
                            {
                                // Trigger RS232 Python Script
                                var pythonRun = Python.Run("logs/ctrlCodes/" + responseString + "_powerOff.py");
                            }
                            catch { FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [ERR] Screen Off via Python Failed"); }
                        }
                    }

                    if (!GCMSSystem.CheckOpened("ScreenLock"))
                    {
                        var screenLock = new ScreenLock();
                        screenLock.Show();
                    }

                    // Set Params
                    MyIni.Write("powersaveMode", "FALSE", "Network");
                    MyIni.Write("powersaveMode2", "TRUE", "Network");

                    if (!MainForm.isInLockdown)
                    {
                        var isMaintMode = MyIni.Read("maintMode", "Network");
                        if (isMaintMode == "TRUE")
                        {
                            MainForm.FrmObj.powerModeLabel.Text = "Low Power";
                            MainForm.FrmObj.powerModeLabel.ForeColor = Color.FromArgb(0, 0, 192);
                        }
                        else
                        {
                            MainForm.FrmObj.powerModeLabel.Text = "Low Power";
                            MainForm.FrmObj.powerModeLabel.ForeColor = Color.FromArgb(0, 192, 0);
                        }

                        MainForm.FrmObj.CheckSNAP.Stop();
                        MainForm.FrmObj.CheckSNAP.Interval = 60000 + (MainForm.snapDelay * 1000);

                        Chrome.Unload();
                        Chrome.UpdatePref();
                    }
                }

                if (curPowerSave1 == "FALSE" && curPowerSave2 == "FALSE" && !bypassChecksum)
                {
                    if (!lowPowerTrigger)
                    {
                        FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Screen Off via RS232");
                    }
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
                        var pingTest = GCMSSystem.Ping(networkURL);
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

                    // Trigger the RS232 Serial Command
                    using (var client = new WebClient())
                    {
                        // Create the $_POST Data for the HTTP Request
                        var values = new NameValueCollection
                        {
                            ["clientID"] = MyIni.Read("clientID", "Monitor"),
                            ["deviceID"] = MyIni.Read("deviceName", "Monitor"),
                            ["deviceUUID"] = MyIni.Read("deviceUUID", "Monitor"),
                            ["theCtrlCode"] = "Screen",
                        };

                        var responseString = "";
                        try
                        {
                            var response = client.UploadValues(networkURL + "/v2/getControlCodeData.php", values);
                            responseString = Encoding.Default.GetString(response);
                            responseString = GCMSSystem.RemoveWhitespace(responseString);
                        }
                        catch
                        {
                            responseString = "Error";
                        }

                        if (responseString != "Error")
                        {
                            if (!File.Exists("logs/ctrlCodes/" + responseString + "_powerOff.py"))
                            {
                                DownloadFileSingle(networkURL + "/monitorUpdate/scripts/controlcodes/" + responseString + "_powerOff.py", "logs/ctrlCodes/" + responseString + "_powerOff.py");
                            }

                            try
                            {
                                // Trigger RS232 Python Script
                                var pythonRun = Python.Run("logs/ctrlCodes/" + responseString + "_powerOff.py");
                            }
                            catch { FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [ERR] Screen Off via Python Failed"); }
                        }
                    }

                    if (!GCMSSystem.CheckOpened("ScreenLock"))
                    {
                        var screenLock = new ScreenLock();
                        screenLock.Show();
                    }

                    // Set Params
                    MyIni.Write("powersaveMode", "FALSE", "Network");
                    MyIni.Write("powersaveMode2", "TRUE", "Network");

                    if (!MainForm.isInLockdown)
                    {
                        var isMaintMode = MyIni.Read("maintMode", "Network");
                        if (isMaintMode == "TRUE")
                        {
                            MainForm.FrmObj.powerModeLabel.Text = "Low Power";
                            MainForm.FrmObj.powerModeLabel.ForeColor = Color.FromArgb(0, 0, 192);
                        }
                        else
                        {
                            MainForm.FrmObj.powerModeLabel.Text = "Low Power";
                            MainForm.FrmObj.powerModeLabel.ForeColor = Color.FromArgb(0, 192, 0);
                        }

                        MainForm.FrmObj.CheckSNAP.Stop();
                        MainForm.FrmObj.CheckSNAP.Interval = 60000 + (MainForm.snapDelay * 1000);

                        Chrome.Unload();
                        Chrome.UpdatePref();
                    }
                }
                MainForm.isSignageLoaded = false;
                SignageBrowser.isSignageLoaded = false;
            }
            if (actualTrigger == "MONON")
            {
                // Software Monitor ON
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);

                var curPowerSave1 = MyIni.Read("powersaveMode", "Network");
                var curPowerSave2 = MyIni.Read("powersaveMode2", "Network");

                if (bypassChecksum) {
                    if (!MainForm.isInLockdown && !CheckOpened("SignageBrowser"))
                    {
                        if (!lowPowerTrigger)
                        {
                            FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Screen On via Software Emulation");
                        }
                        MyIni.Write("powersaveMode", "FALSE", "Network");
                        MyIni.Write("powersaveMode2", "FALSE", "Network");

                        var isMaintMode = MyIni.Read("maintMode", "Network");
                        if (isMaintMode == "TRUE")
                        {
                            MainForm.FrmObj.powerModeLabel.Text = "Maintenance";
                            MainForm.FrmObj.powerModeLabel.ForeColor = Color.FromArgb(255, 128, 0);
                        }
                        else
                        {
                            MainForm.FrmObj.powerModeLabel.Text = "Normal / Online";
                            MainForm.FrmObj.powerModeLabel.ForeColor = Color.FromArgb(0, 0, 0);
                        }

                        try
                        {
                            ScreenLock.frmObj.Hide();
                        }
                        catch { }
                        try
                        {
                            ScreenLock.frmObj.Close();
                        }
                        catch { }

                        try
                        {
                            Application.OpenForms["ScreenLock"].Hide();
                        }
                        catch { }
                        try
                        {
                            Application.OpenForms["ScreenLock"].Close();
                        }
                        catch { }

                        MainForm.FrmObj.CheckSNAP.Interval = 60000 + (MainForm.snapDelay * 1000);
                        MainForm.FrmObj.CheckSNAP.Start();

                        Chrome.UpdatePref();
                        Chrome.Load();
                        try
                        {
                            MonitorControl.Software("ON", MonitorControl.MonitorState.ON);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("e: " + e.ToString());
                            FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [ERR] Screen On via Software Emulation Failed");
                        }
                    }
                }
                if (curPowerSave1 == "TRUE" && curPowerSave2 == "FALSE" && !bypassChecksum)
                {
                    if (!MainForm.isInLockdown)
                    {
                        if (!lowPowerTrigger)
                        {
                            FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Screen On via Software Emulation");
                        }
                        MyIni.Write("powersaveMode", "FALSE", "Network");
                        MyIni.Write("powersaveMode2", "FALSE", "Network");

                        var isMaintMode = MyIni.Read("maintMode", "Network");
                        if (isMaintMode == "TRUE")
                        {
                            MainForm.FrmObj.powerModeLabel.Text = "Maintenance";
                            MainForm.FrmObj.powerModeLabel.ForeColor = Color.FromArgb(255, 128, 0);
                        }
                        else
                        {
                            MainForm.FrmObj.powerModeLabel.Text = "Normal / Online";
                            MainForm.FrmObj.powerModeLabel.ForeColor = Color.FromArgb(0, 0, 0);
                        }

                        try
                        {
                            ScreenLock.frmObj.Hide();
                        }
                        catch { }
                        try
                        {
                            ScreenLock.frmObj.Close();
                        }
                        catch { }

                        try
                        {
                            Application.OpenForms["ScreenLock"].Hide();
                        }
                        catch { }
                        try
                        {
                            Application.OpenForms["ScreenLock"].Close();
                        }
                        catch { }

                        MainForm.FrmObj.CheckSNAP.Interval = 60000 + (MainForm.snapDelay * 1000);
                        MainForm.FrmObj.CheckSNAP.Start();

                        Chrome.UpdatePref();
                        Chrome.Load();

                        var cloneScr = MyIni.Read("Clone", "Display");
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

                        try
                        {
                            MonitorControl.Software("ON", MonitorControl.MonitorState.ON);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("e: " + e.ToString());
                            FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [ERR] Screen On via Software Emulation Failed");
                        }
                    }
                }
            }
            if (actualTrigger == "MONOFF")
            {
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                MyIni.Write("powersaveMode", "TRUE", "Network");
                MyIni.Write("powersaveMode2", "FALSE", "Network");

                var curPowerSave1 = MyIni.Read("powersaveMode", "Network");
                var curPowerSave2 = MyIni.Read("powersaveMode2", "Network");

                if (bypassChecksum) {
                    if (!GCMSSystem.CheckOpened("ScreenLock"))
                    {
                        var screenLock = new ScreenLock();
                        screenLock.Show();
                    }

                    // Software Monitor OFF
                    if (!lowPowerTrigger)
                    {
                        FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Screen Off via Software Emulation");
                    }
                    if (!MainForm.isInLockdown)
                    {
                        var isMaintMode = MyIni.Read("maintMode", "Network");
                        if (isMaintMode == "TRUE")
                        {
                            MainForm.FrmObj.powerModeLabel.Text = "Low Power";
                            MainForm.FrmObj.powerModeLabel.ForeColor = Color.FromArgb(0, 0, 192);
                        }
                        else
                        {
                            MainForm.FrmObj.powerModeLabel.Text = "Low Power";
                            MainForm.FrmObj.powerModeLabel.ForeColor = Color.FromArgb(0, 192, 0);
                        }

                        MainForm.FrmObj.CheckSNAP.Stop();
                        MainForm.FrmObj.CheckSNAP.Interval = 60000 + (MainForm.snapDelay * 1000);

                        Chrome.Unload();
                        Chrome.UpdatePref();
                        try
                        {
                            MonitorControl.Software("OFF", MonitorControl.MonitorState.OFF);
                        }
                        catch { FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [ERR] Screen Off via Software Emulation Failed"); }
                        MainForm.isSignageLoaded = false;
                        SignageBrowser.isSignageLoaded = false;
                    }
                }
                if (curPowerSave1 == "TRUE" && curPowerSave2 == "FALSE" && !bypassChecksum)
                {
                    if (!GCMSSystem.CheckOpened("ScreenLock"))
                    {
                        var screenLock = new ScreenLock();
                        screenLock.Show();
                    }

                    // Software Monitor OFF
                    if (!lowPowerTrigger)
                    {
                        FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Screen Off via Software Emulation");
                    }
                    if (!MainForm.isInLockdown)
                    {
                        var isMaintMode = MyIni.Read("maintMode", "Network");
                        if (isMaintMode == "TRUE")
                        {
                            MainForm.FrmObj.powerModeLabel.Text = "Low Power";
                            MainForm.FrmObj.powerModeLabel.ForeColor = Color.FromArgb(0, 0, 192);
                        }
                        else
                        {
                            MainForm.FrmObj.powerModeLabel.Text = "Low Power";
                            MainForm.FrmObj.powerModeLabel.ForeColor = Color.FromArgb(0, 192, 0);
                        }

                        MainForm.FrmObj.CheckSNAP.Stop();
                        MainForm.FrmObj.CheckSNAP.Interval = 60000 + (MainForm.snapDelay * 1000);

                        Chrome.Unload();
                        Chrome.UpdatePref();

                        try
                        {
                            MonitorControl.Software("OFF", MonitorControl.MonitorState.OFF);
                        }
                        catch { FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [ERR] Screen Off via Software Emulation Failed"); }
                        MainForm.isSignageLoaded = false;
                        SignageBrowser.isSignageLoaded = false;
                    }
                }
            }
            if (actualTrigger == "MUTE")
            {
                // Mute Audio
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Vol MUTE");
                AudioManager.SetMasterVolumeMute(true);

            }
            if (actualTrigger == "UNMUTE")
            {
                // Unmute Audio
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Vol UNMUTE");
                AudioManager.SetMasterVolumeMute(false);
            }
            if (actualTrigger == "VOL5")
            {
                // Audio Volume - 5%
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Vol 5%");
                SignageBrowser.curMasterVol = 5;
                AudioManager.SetMasterVolume(5);
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                MyIni.Write("MasterLevel", "5", "Audio");
            }
            if (actualTrigger == "VOL10")
            {
                // Audio Volume - 10%
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Vol 10%");
                SignageBrowser.curMasterVol = 10;
                AudioManager.SetMasterVolume(10);
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                MyIni.Write("MasterLevel", "10", "Audio");
            }
            if (actualTrigger == "VOL15")
            {
                // Audio Volume - 15%
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Vol 15%");
                SignageBrowser.curMasterVol = 15;
                AudioManager.SetMasterVolume(15);
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                MyIni.Write("MasterLevel", "15", "Audio");
            }
            if (actualTrigger == "VOL20")
            {
                // Audio Volume - 20%
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Vol 20%");
                SignageBrowser.curMasterVol = 20;
                AudioManager.SetMasterVolume(20);
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                MyIni.Write("MasterLevel", "20", "Audio");
            }
            if (actualTrigger == "VOL25")
            {
                // Audio Volume - 25%
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Vol 25%");
                SignageBrowser.curMasterVol = 25;
                AudioManager.SetMasterVolume(25);
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                MyIni.Write("MasterLevel", "25", "Audio");
            }
            if (actualTrigger == "VOL30")
            {
                // Audio Volume - 30%
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Vol 30%");
                SignageBrowser.curMasterVol = 30;
                AudioManager.SetMasterVolume(30);
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                MyIni.Write("MasterLevel", "30", "Audio");
            }
            if (actualTrigger == "VOL35")
            {
                // Audio Volume - 35%
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Vol 35%");
                SignageBrowser.curMasterVol = 35;
                AudioManager.SetMasterVolume(35);
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                MyIni.Write("MasterLevel", "35", "Audio");
            }
            if (actualTrigger == "VOL40")
            {
                // Audio Volume - 40%
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Vol 40%");
                SignageBrowser.curMasterVol = 40;
                AudioManager.SetMasterVolume(40);
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                MyIni.Write("MasterLevel", "40", "Audio");
            }
            if (actualTrigger == "VOL45")
            {
                // Audio Volume - 45%
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Vol 45%");
                SignageBrowser.curMasterVol = 45;
                AudioManager.SetMasterVolume(45);
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                MyIni.Write("MasterLevel", "45", "Audio");
            }
            if (actualTrigger == "VOL50")
            {
                // Audio Volume - 50%
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Vol 50%");
                SignageBrowser.curMasterVol = 50;
                AudioManager.SetMasterVolume(50);
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                MyIni.Write("MasterLevel", "50", "Audio");
            }
            if (actualTrigger == "VOL55")
            {
                // Audio Volume - 55%
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Vol 55%");
                SignageBrowser.curMasterVol = 55;
                AudioManager.SetMasterVolume(55);
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                MyIni.Write("MasterLevel", "55", "Audio");
            }
            if (actualTrigger == "VOL60")
            {
                // Audio Volume - 60%
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Vol 60%");
                SignageBrowser.curMasterVol = 60;
                AudioManager.SetMasterVolume(60);
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                MyIni.Write("MasterLevel", "60", "Audio");
            }
            if (actualTrigger == "VOL65")
            {
                // Audio Volume - 65%
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Vol 65%");
                SignageBrowser.curMasterVol = 65;
                AudioManager.SetMasterVolume(65);
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                MyIni.Write("MasterLevel", "65", "Audio");
            }
            if (actualTrigger == "VOL70")
            {
                // Audio Volume - 70%
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Vol 70%");
                SignageBrowser.curMasterVol = 70;
                AudioManager.SetMasterVolume(70);
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                MyIni.Write("MasterLevel", "70", "Audio");
            }
            if (actualTrigger == "VOL75")
            {
                // Audio Volume - 75%
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Vol 75%");
                SignageBrowser.curMasterVol = 75;
                AudioManager.SetMasterVolume(75);
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                MyIni.Write("MasterLevel", "75", "Audio");
            }
            if (actualTrigger == "VOL80")
            {
                // Audio Volume - 80%
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Vol 80%");
                SignageBrowser.curMasterVol = 80;
                AudioManager.SetMasterVolume(80);
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                MyIni.Write("MasterLevel", "80", "Audio");
            }
            if (actualTrigger == "VOL85")
            {
                // Audio Volume - 85%
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Vol 85%");
                SignageBrowser.curMasterVol = 85;
                AudioManager.SetMasterVolume(85);
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                MyIni.Write("MasterLevel", "85", "Audio");
            }
            if (actualTrigger == "VOL90")
            {
                // Audio Volume - 90%
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Vol 90%");
                SignageBrowser.curMasterVol = 90;
                AudioManager.SetMasterVolume(90);
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                MyIni.Write("MasterLevel", "90", "Audio");
            }
            if (actualTrigger == "VOL95")
            {
                // Audio Volume - 95%
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Vol 95%");
                SignageBrowser.curMasterVol = 95;
                AudioManager.SetMasterVolume(95);
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                MyIni.Write("MasterLevel", "95", "Audio");
            }
            if (actualTrigger == "VOL100")
            {
                // Audio Volume - 100%
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Vol 100%");
                SignageBrowser.curMasterVol = 100;
                AudioManager.SetMasterVolume(100);
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                MyIni.Write("MasterLevel", "100", "Audio");
            }
            if (actualTrigger == "FIXVNC")
            {
                // VNC for whatever reason isnt working correctly - Re-Install
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Fix VNC");
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                var macStr = macAddr.Replace(":", "-");
                var uuidStr = MyIni.Read("deviceUUID", "Monitor");
                var clientID = MyIni.Read("clientID", "Monitor");
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

                // VNC for whatever reason isnt working correctly - Re-Install
                // if (!File.Exists("tightvnc.msi"))
                // {
                // var downloadFile = networkURL + "/monitorUpdate/install/tightvnc.msi";
                // var savedFile = "tightvnc.msi";
                // using (var client = new WebClient())
                // {
                // client.DownloadFile(downloadFile, savedFile);
                //  }
                // }
                DownloadFileSingle(networkURL + "/monitorUpdate/install/tightvnc.msi", Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "tightvnc.msi"));

                var responseString3 = GetVNCPass(networkURL, clientID);
                var vncInstalled = Commission.CheckInstalled("TightVNC");
                if (vncInstalled)
                {
                    try
                    {
                        // VNC is already installed - we need to therefor uninstall it to take our configuration
                        Process process1 = new Process();
                        process1.StartInfo.FileName = "msiexec";
                        process1.StartInfo.Arguments = "-x \"" + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\tightvnc.msi\" /quiet";
                        process1.StartInfo.Verb = "runas";
                        process1.Start();
                        process1.WaitForExit(30000);
                    }
                    catch { }
                }

                // VNC isnt installed so we need to start MSIExec to install the software silently
                try
                {
                    Process process2 = new Process();
                    process2.StartInfo.FileName = "msiexec";
                    process2.StartInfo.Arguments = "/quiet /i \"" + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\tightvnc.msi\" ALLUSERS=\"1\" /quiet /norestart ADDLOCAL=\"Server,Viewer\" VIEWER_ASSOCIATE_VNC_EXTENSION=1 SERVER_REGISTER_AS_SERVICE=1 SERVER_ADD_FIREWALL_EXCEPTION=1 VIEWER_ADD_FIREWALL_EXCEPTION=1 SERVER_ALLOW_SAS=1 SET_USEVNCAUTHENTICATION=1 VALUE_OF_USEVNCAUTHENTICATION=1 SET_PASSWORD=1 VALUE_OF_PASSWORD=" + responseString3 + " SET_USECONTROLAUTHENTICATION=1 VALUE_OF_USECONTROLAUTHENTICATION=1 SET_CONTROLPASSWORD=1 VALUE_OF_CONTROLPASSWORD=" + responseString3;
                    process2.StartInfo.Verb = "runas";
                    process2.Start();
                    process2.WaitForExit(60000);
                }
                catch { }

                try
                {
                    File.Delete(@Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\tightvnc.msi");
                }
                catch { }
            }
            if (actualTrigger == "FIXVPN")
            {
                // VPN for whatever reason isnt working correctly - Re-Install
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Fix VPN");
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                var macStr = macAddr.Replace(":", "-");
                var uuidStr = MyIni.Read("deviceUUID", "Monitor");
                var clientID = MyIni.Read("clientID", "Monitor");
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

                if (MyNetwork == "SEC")
                {
                    string user = Chrome.GetCurrentMachineUser();
                    string path = Path.Combine("C:\\", "Users", user, "AppData", "Roaming", "pritunl", "profiles");

                    var responseString = FixVPN(macAddr, networkURL, clientID);
                    DirectoryInfo di = new DirectoryInfo(path);
                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                    var confFile = networkURL + "/v2/vpnSystem/" + responseString + ".conf";
                    var confSavedFile = path + "\\" + responseString + ".conf";
                    using (var conf = new WebClient())
                    {
                        conf.DownloadFile(confFile, confSavedFile);
                    }
                    var ovpnFile = networkURL + "/v2/vpnSystem/" + responseString + ".ovpn";
                    var ovpnSavedFile = path + "\\" + responseString + ".ovpn";
                    using (var ovpn = new WebClient())
                    {
                        ovpn.DownloadFile(ovpnFile, ovpnSavedFile);
                    }
                }
                // After Installing VPN Profiles - Restart Pritunl
                foreach (var process in Process.GetProcessesByName("pritunl"))
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

                var pritunlEXE = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\Pritunl\\pritunl.exe";
                var pritunlRunning = "NO";
                Process[] pname = Process.GetProcessesByName("pritunl");
                if (pname.Length > 0) {
                    pritunlRunning = "YES";
                }
                if (pritunlRunning == "NO")
                {
                    Process pritunl = new Process();
                    pritunl.StartInfo.FileName = pritunlEXE;
                    pritunl.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    pritunl.Start();
                    pritunl.WaitForExit(60000);
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
            if (actualTrigger == "RESTARTVPN")
            {
                // VPN for whatever reason isnt working correctly - Re-Install
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Restart VPN");
               
                // After Installing VPN Profiles - Restart Pritunl
                foreach (var process in Process.GetProcessesByName("pritunl"))
                {
                    try
                    {
                        process.StartInfo.Verb = "runas";
                        process.Kill();
                        process.WaitForExit(60000);
                    }
                    catch { }
                }

                var pritunlEXE = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\Pritunl\\pritunl.exe";
                var pritunlRunning = "NO";
                Process[] pname = Process.GetProcessesByName("pritunl");
                if (pname.Length > 0) { pritunlRunning = "YES"; }

                if (pritunlRunning == "NO")
                {
                    Process pritunl = new Process();
                    pritunl.StartInfo.FileName = pritunlEXE;
                    pritunl.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    pritunl.Start();
                }

            }
            if (actualTrigger == "DECOM")
            {
                // Decommission Asset
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Decommission Asset");
                // Stop Timers
                MainForm.FrmObj.CallHomeTimer.Stop();
                MainForm.FrmObj.CheckStatsTimer.Stop();
                MainForm.FrmObj.CheckServicesTimer.Stop();
                MainForm.FrmObj.CheckSNAP.Stop();
                MainForm.FrmObj.CheckSNAP.Interval = 60000 + (MainForm.snapDelay * 1000);

                // Dump Lock File into C:\Windows so that it can detect if its Decommissioned
                // So if the program is re-opened when the file exists that it will automatically load
                // the DECOM system
                var lockFile = Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\decom.lock";
                File.WriteAllText(lockFile, "Asset Decommissioned");

                // Disable Main Form
                MainForm.FrmObj.Enabled = false;
                MainForm.FrmObj.WindowState = FormWindowState.Minimized;
                // Load New Decommissioned Form
                OpenedForm = new Decommission();
                OpenedForm.Show();

                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                var MyNetwork = MyIni.Read("licType", "Licence");
                var deviceName = MyIni.Read("deviceName", "Monitor");
                var deviceUUID = MyIni.Read("deviceUUID", "Monitor");
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

                using (var client = new WebClient())
                {
                    // Create the $_POST Data for the HTTP Request
                    var values = new NameValueCollection
                    {
                        ["deviceName"] = deviceName,
                        ["deviceMAC"] = macAddr,
                        ["deviceUUID"] = deviceUUID
                    };

                    var responseString = "";
                    try
                    {
                        var response = client.UploadValues(networkURL + "/v2/outboundDecom.php", values);
                        responseString = Encoding.Default.GetString(response);
                    }
                    catch
                    {
                        responseString = "Error";
                    }
                }
            }
            if (actualTrigger == "REACTIVATE")
            {
                // Decommission Asset
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Decommission Re-Activate");
                // Start Timers
                MainForm.FrmObj.CallHomeTimer.Start();
                MainForm.FrmObj.CheckStatsTimer.Start();
                MainForm.FrmObj.CheckServicesTimer.Start();
                MainForm.FrmObj.CheckSNAP.Start();
                // Enable Main Form
                MainForm.FrmObj.Enabled = true;
                MainForm.FrmObj.WindowState = FormWindowState.Minimized;
                // Close Lock Form
                Decommission.FrmObj.Close();

                // Remove the Decom.lock file so that it will automatically kick in properly again
                var lockFile = Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\decom.lock";
                File.Delete(lockFile);

                // Variables for each CheckSum - Default to NO
                var chromeRunning = "NO";

                // Restart Google Chrome in Kiosk Mode and load up the Preloader.html
                // Check again if Chrome has been opened while the user is in Maintenance Mode
                var chromeProcess = Process.GetProcesses().Any(p => p.ProcessName.Contains("chrome"));
                if (chromeProcess)
                {
                    chromeRunning = "YES";
                }

                if (chromeRunning == "YES")
                {
                    // Chrome is running - probably due to someone opening it while in Maintenance Mode
                    Chrome.Unload();
                    Chrome.UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear
                }

                var osArch = GCMSSystem.GetOSArch();
                var chromeEXE = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\Google\\Chrome\\Application\\chrome.exe";
                var nodeEXE = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\node32.exe";
                if (osArch == "x64")
                {
                    nodeEXE = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\node64.exe";
                }

                // NodeJS isnt running
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

                Chrome.UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear
                Chrome.Load();

                // Bring Monitor to Minimized Mode so that you can see it.
                MainForm.FrmObj.WindowState = FormWindowState.Minimized;

            }
            if (actualTrigger == "MOVE2POOL")
            {
                // Update Software
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Return To Pool (Full Decommission)");
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                var MyNetwork = MyIni.Read("licType", "Licence");
                var deviceName = MyIni.Read("deviceName", "Monitor");
                var uuidStr = MyIni.Read("deviceUUID", "Monitor");

                string user = Chrome.GetCurrentMachineUser();
                string path = Path.Combine("C:\\", "Users", user, "AppData", "Roaming", "pritunl", "profiles");

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

                // Delete profiles already installed
                DirectoryInfo di = new DirectoryInfo(path);
                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }

                // Download NEWSETUP user profiles
                try
                {
                    // DownloadFile(networkURL + "/vpnSystem/newsetup.conf", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\pritunl\\profiles\\newsetup.conf");
                    DownloadFileSingle(networkURL + "/vpnSystem/newsetup.conf", path + "\\newsetup.conf");
                }
                catch
                {

                }
                try
                {
                    // DownloadFile(networkURL + "/vpnSystem/newsetup.ovpn", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\pritunl\\profiles\\newsetup.ovpn");
                    DownloadFileSingle(networkURL + "/vpnSystem/newsetup.conf", path + "\\newsetup.ovpn");
                }
                catch
                {

                }

                // Now we have the original NEWSETUP installed lets reload Pritunl so that it will reload onto the correct user account
                Pritunl.Reload();

                // Clear up the Config.ini and Remove it
                try
                {
                    File.Delete(@Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\config\\config.ini");
                    File.Delete(@Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\config\\config.ini.backup");
                }
                catch
                {

                }

                // Clear up the Decom.lock File and Remove it
                try
                {
                    var lockFile = @Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\decom.lock";
                    File.Delete(lockFile);
                }
                catch
                {

                }

                using (var client = new WebClient())
                {
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
                        ["deviceName"] = deviceName,
                        ["deviceMAC"] = macAddr,
                        ["deviceUUID"] = uuidStr
                    };

                    var responseString = "";
                    try
                    {
                        var response = client.UploadValues(networkURL + "/v2/outboundReturn.php", values);
                        responseString = Encoding.Default.GetString(response);
                    }
                    catch
                    {
                        responseString = "Error";
                    }
                }

                try
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

                    // Make sure to STOP SNAP detection or it could theoretically kick in a SNAP before MOVE2POOL completes
                    MainForm.FrmObj.CheckSNAP.Stop();
                    MainForm.FrmObj.CheckSNAP.Interval = 60000 + (MainForm.snapDelay * 1000);

                    // Delete all Signage Files that are used by Signage System
                    try
                    {
                        File.Delete(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\nodejsData\\cacheLinks");
                    }
                    catch { }
                    try
                    {
                        File.Delete(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\nodejsData\\content_run_amounts");
                    }
                    catch { }
                    try
                    {
                        File.Delete(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\nodejsData\\oldPlayerData");
                    }
                    catch { }
                    try
                    {
                        File.Delete(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\nodejsData\\playerData");
                    }
                    catch { }
                    try
                    {
                        File.Delete(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\nodejsData\\syncid");
                    }
                    catch { }
                    try
                    {
                        File.Delete(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\nodejsData\\deviceActivated");
                    }
                    catch { }
                    
                    // Delete all internal logs used by the Monitor itself
                    try
                    {
                        File.Delete(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\data.log");
                    }
                    catch { }
                    try
                    {
                        File.Delete(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\events.log");
                    }
                    catch { }
                    try
                    {
                        File.Delete(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\system.log");
                    }
                    catch { }
                    try
                    {
                        File.Delete(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\signageOutput.log");
                    }
                    catch { }
                    
                    // Delete all content from Signage Content Folder
                    DirectoryInfo signageContent = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\public\\content");
                    foreach (FileInfo file in signageContent.GetFiles())
                    {
                        try
                        {
                            file.Delete();
                        }
                        catch
                        {

                        }
                    }
                }
                catch { }

                // Uninstall VNC so that when its re-installed it will take all the new settings
                // VNC for whatever reason isnt working correctly - Re-Install
                if (!File.Exists("tightvnc.msi"))
                {
                    var downloadFile = networkURL + "/monitorUpdate/install/tightvnc.msi";
                    var savedFile = "tightvnc.msi";
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(downloadFile, savedFile);
                    }
                }

                try
                {
                    Process process1 = new Process();
                    process1.StartInfo.FileName = "msiexec";
                    process1.StartInfo.Arguments = "-x \"" + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\tightvnc.msi\" /quiet";
                    process1.StartInfo.Verb = "runas";
                    process1.Start();
                    process1.WaitForExit(30000);
                }
                catch { }

                // Once we have used the msi to uninstall VNC - Delete the TightVNC Installer
                try
                {
                    File.Delete(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\tightvnc.msi");
                }
                catch { }

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
            if (actualTrigger == "UPDATE")
            {
                if (!MainForm.isUselessInternet)
                {
                    FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Update Software");
                    Updater.DownloadMonitor(false);
                }
            }
            if (actualTrigger == "UPDATEOLD")
            {
                if (!MainForm.isUselessInternet)
                {
                    // Update Software - Download updatetool.bat this is a self timebombed batch file which downloads GlobalCMS Monitor + Service and all DLL Files
                    FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Update Software");
                    DownloadFileSingle("http://api.globalcms.co.uk/v2/monitorUpdate/curl.exe", @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\curl.exe");
                    DownloadFileSingle("http://api.globalcms.co.uk/v2/monitorUpdate/updatetool.bat", @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\updatetool.bat");

                    var updateTool = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\updatetool.bat";
                    if (File.Exists(updateTool))
                    {
                        try
                        {
                            Process updateProc = new Process();
                            updateProc.StartInfo.FileName = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\updatetool.bat";
                            updateProc.StartInfo.Verb = "runas";
                            updateProc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                            updateProc.Start();
                        }
                        catch { }
                    }
                }
            }
            if (actualTrigger == "SENDLOG")
            {
                if (!MainForm.isUselessInternet)
                {
                    // Send Log Home to Server when requested
                    FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Download Logs");
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

                    SendLogFile(networkURL, "logs\\data.log", uuidStr, "SYSTEM");
                    File.Delete(@Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\data.log");
                }
            }
            if (actualTrigger == "SENDAVALOG")
            {
                if (!MainForm.isUselessInternet)
                {
                    // Send AVA Log Home to Server when requested
                    FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Download AVA Logs");
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

                    SendLogFile(networkURL, "logs\\ava.log", uuidStr, "AVA");
                    try
                    {
                        File.Delete(@Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\ava.log");
                    }
                    catch { }

                    try
                    {
                        File.Create(@Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\ava.log").Dispose();
                    }
                    catch { }
                }
            }
            if (actualTrigger == "MAINT")
            {
                // Maintenance Mode
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Maintenance Mode");
                // Stop all Timers that are not needed while in Maintenance Mode
                MainForm.FrmObj.CheckServicesTimer.Stop();
                MainForm.FrmObj.CheckStatsTimer.Stop();
                MainForm.FrmObj.CheckSNAP.Stop();
                MainForm.FrmObj.CheckSNAP.Interval = 60000 + (MainForm.snapDelay * 1000);

                MainForm.FrmObj.LauncherTimer.Enabled = false;
                MainForm.FrmObj.LauncherTimer.Stop();
                MainForm.FrmObj.LauncherTimer.Interval = 60000;

                // Set the Variables
                MainForm.FrmObj.powerModeLabel.Text = "Maintenance";
                MainForm.FrmObj.powerModeLabel.ForeColor = Color.FromArgb(255, 128, 0);
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                MyIni.Write("maintMode", "TRUE", "Network");

                // Shutdown Chrome Kiosk Mode so that you can access the Machine
                Chrome.Unload();
                Chrome.UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear

                // Bring Monitor to Normal Mode so that you can see it.
                MainForm.FrmObj.WindowState = FormWindowState.Normal;

                var toolForm = new EngineerTools();
                toolForm.Show();
            }
            if (actualTrigger == "NORMAL")
            {
                // Normal Mode
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Normal Mode");

                // Set the Variables
                MainForm.FrmObj.powerModeLabel.Text = "Normal / Online";
                MainForm.FrmObj.powerModeLabel.ForeColor = Color.FromArgb(0, 0, 0);
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                MyIni.Write("maintMode", "FALSE", "Network");

                // Variables for each CheckSum - Default to NO
                var chromeRunning = "NO";

                // Restart Google Chrome in Kiosk Mode and load up the Preloader.html
                // Check again if Chrome has been opened while the user is in Maintenance Mode
                var chromeProcess = Process.GetProcesses().Any(p => p.ProcessName.Contains("chrome"));
                if (chromeProcess)
                {
                    chromeRunning = "YES";
                }

                if (chromeRunning == "YES")
                {
                    // Chrome is running - probably due to someone opening it while in Maintenance Mode
                    Chrome.Unload();
                }

                Chrome.UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear
                Chrome.Load();

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

                // Start all Timers that are stopped due to Maintenance Mode
                MainForm.FrmObj.CheckServicesTimer.Start();
                MainForm.FrmObj.CheckStatsTimer.Start();
                MainForm.FrmObj.CheckSNAP.Start();
                MainForm.FrmObj.LauncherTimer.Interval = 60000;

                MainForm.FrmObj.LauncherTimer.Enabled = true;
                MainForm.FrmObj.LauncherTimer.Start();
                MainForm.FrmObj.LauncherTimer.Interval = 60000;

                // Bring Monitor to Minimized Mode so that you can see it.
                MainForm.FrmObj.WindowState = FormWindowState.Minimized;

                EngineerTools.FrmObj.Hide();
            }
            if (actualTrigger == "UPDATESIGN")
            {
                if (!MainForm.isUselessInternet)
                {
                    FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Update Signage");
                    Updater.DownloadSignage(false);
                }
            }
            if (actualTrigger == "UPDATESIGNOLD")
            {
                if (!MainForm.isUselessInternet)
                {
                    // if (Chrome.whichVer == 2)
                    // {
                    bool isSignageEnabled = Chrome.IsSignageEnabled();
                    if (isSignageEnabled)
                    {
                        var updateForm = new Updating();
                        updateForm.Show();
                    }
                    // }

                    // Update Signage
                    FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Update Signage");

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
                        Chrome.Unload();
                        Chrome.UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear
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
                    MainForm.FrmObj.CheckSNAP.Interval = 60000 + (MainForm.snapDelay * 1000);

                    // Move content folder to temp folder inside of monitor main root DIR
                    try
                    {
                        Directory.Move(sourceDIR, destDIR);
                    }
                    catch
                    {

                    }

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

                    bool fileDownloaded = false;
                    // Download latest Signage.zip from API Server
                    try
                    {
                        DownloadFileSingle(networkURL + "/v2/signageUpdate/signage.zip", "signage.zip");
                        fileDownloaded = true;
                    }
                    catch { }

                    if (fileDownloaded)
                    {
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
                            ZipFile.ExtractToDirectory(signageZipFile, signageZipFolder);
                        }
                        catch { }

                        // We need to wait to allow the system to finish extracting the file
                        System.Threading.Thread.Sleep(5000);                                                                    // Wait for 5 seconds

                        // For some reaon the last 2 files dont seem to extract form the zip so lets place a backup plan in place for that
                        var websocketJS = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\websocket.js";
                        if (!File.Exists(websocketJS))
                        {
                            DownloadFileSingle(networkURL + "/v2/signageUpdate/websocket.js", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\websocket.js");
                        }
                        var WindowsMessageRelayEXE = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\WindowsMessageRelay.exe";
                        if (!File.Exists(WindowsMessageRelayEXE))
                        {
                            DownloadFileSingle(networkURL + "/v2/signageUpdate/WindowsMessageRelay.exe", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\WindowsMessageRelay.exe");
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

                        var browserSSL = MyIni.Read("SSL", "Browser");
                        var sslon = "false";
                        if (browserSSL == "On")
                        {
                            sslon = "true";
                        }
                        string signageConfFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "settings.conf");
                        var SettingsIni = new IniFile(signageConfFile);
                        SettingsIni.Write("sslOn", sslon, "core");

                        Chrome.UpdateCallHome();            // On Updating and Extracting the new signage we need to make sure we lock back onto which Timers to use

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

                        Chrome.UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear
                        Chrome.Load();
                        Updating.frmObj.Close();
                    }
                }
            }
            if (actualTrigger == "RESTARTWIFI")
            {
                // Restart WiFi Card installed in machine
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Restart WiFi NIC");
                Network.Restart("WIFI");
            }
            if (actualTrigger == "RESTARTLAN")
            {
                // Restart LAN Card installed in machine
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Restart LAN NIC");
                Network.Restart("LAN");
            }
            if (actualTrigger == "RESTARTSIGN")
            {
                // Restart Signage
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Restart Signage");
                Chrome.Unload();
                Chrome.UpdatePref();
                Chrome.Load();
            }
            if (actualTrigger == "RESETTIME")
            {
                // Reset the Timezone to Given Params
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Set Timezone");

                // Get which Timezone the Current SiteID is using and set this as the System Timezone
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                // MAC Address
                var macStr = macAddr.Replace(":", "-");
                var uuidStr = MyIni.Read("deviceUUID", "Monitor");
                var clientID = MyIni.Read("clientID", "Monitor");
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

                SetSystemTimeZone.SetTimezone(macStr, uuidStr, networkURL);
            }
            if (actualTrigger == "UPDATESIGNPARAMS")
            {
                // Update signageParams.ini File
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Update Signage Loading Params");
                // Variables for each CheckSum - Default to NO
                var chromeRunning = "NO";
                var node32Running = "NO";
                var node64Running = "NO";
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                // MAC Address
                var macStr = macAddr.Replace(":", "-");
                var uuidStr = MyIni.Read("deviceUUID", "Monitor");
                var clientID = MyIni.Read("clientID", "Monitor");
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

                // Check again if Chrome has been opened while the user is in Maintenance Mode
                var node32Process = Process.GetProcesses().Any(p => p.ProcessName.Contains("node32"));
                if (node32Process)
                {
                    node32Running = "YES";
                }

                // node32 is running
                if (node32Running == "YES")
                {
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
                }
                var node64Process = Process.GetProcesses().Any(p => p.ProcessName.Contains("node64"));
                if (node64Process)
                {
                    node64Running = "YES";
                }

                // node64 is running
                if (node64Running == "YES")
                {
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
                var chromeProcess = Process.GetProcesses().Any(p => p.ProcessName.Contains("chrome"));
                if (chromeProcess)
                {
                    chromeRunning = "YES";
                }

                if (chromeRunning == "YES")
                {
                    // Chrome is running - probably due to someone opening it while in Maintenance Mode
                    Chrome.Unload();
                }

                Chrome.UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear

                // Once all programs have been shutdown we need to restart them in the order
                // NODE --> CHROME
                // Variables for each CheckSum - Default to NO
                var chromeRunningNow = "NO";
                var nodeRunningNow = "NO";

                var osArch = GCMSSystem.GetOSArch();
                var chromeEXE = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\Google\\Chrome\\Application\\chrome.exe";
                var nodeEXE = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\node32.exe";
                if (osArch == "x64")
                {
                    nodeEXE = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\node64.exe";
                }
                // GCMSSystem.FileLogger.Log("Arch : " + osArch);

                var chromeProcessNew = Process.GetProcesses().Any(p => p.ProcessName.Contains("chrome"));
                if (chromeProcessNew)
                {
                    chromeRunningNow = "YES";
                }
                var nodeProcessNew = Process.GetProcesses().Any(p => p.ProcessName.Contains("node"));
                if (nodeProcessNew)
                {
                    nodeRunningNow = "YES";
                }

                // Before we reload the Signage, update the Signage Params file
                File.Delete(@Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\config\\signageParams.ini");
                var paramFile = GetSignageParams(networkURL, macAddr);
                File.WriteAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\config\\signageParams.ini", paramFile);

                if (nodeRunningNow == "NO")
                {
                    // NodeJS isnt running
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
                }
                if (chromeRunningNow == "NO")
                {
                    Chrome.Load();
                }
                // Bring Google Chrome Kiosk To ON TOP
                bool isSignageEnabled = Chrome.IsSignageEnabled();
                if (isSignageEnabled)
                {
                    if (Chrome.whichVer == 1 && !MainForm.isDebug)
                    {
                        var chromeProcessGUID = "- Google Chrome";
                        WindowHelper.BringToFront(chromeProcessGUID);
                    }
                }
            }
            if (actualTrigger == "UPDATESIGNCONFIG")
            {
                // Update Signage Settings File
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Update Signage Settings");
                // Variables for each CheckSum - Default to NO
                var chromeRunning = "NO";
                var node32Running = "NO";
                var node64Running = "NO";
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                // MAC Address
                var macStr = macAddr.Replace(":", "-");
                var uuidStr = MyIni.Read("deviceUUID", "Monitor");
                var clientID = MyIni.Read("clientID", "Monitor");
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

                // Check again if Chrome has been opened while the user is in Maintenance Mode
                var node32Process = Process.GetProcesses().Any(p => p.ProcessName.Contains("node32"));
                if (node32Process)
                {
                    node32Running = "YES";
                }

                // node32 is running
                if (node32Running == "YES")
                {
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
                }
                var node64Process = Process.GetProcesses().Any(p => p.ProcessName.Contains("node64"));
                if (node64Process)
                {
                    node64Running = "YES";
                }

                // node64 is running
                if (node64Running == "YES")
                {
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
                var chromeProcess = Process.GetProcesses().Any(p => p.ProcessName.Contains("chrome"));
                if (chromeProcess)
                {
                    chromeRunning = "YES";
                }

                if (chromeRunning == "YES")
                {
                    // Chrome is running - probably due to someone opening it while in Maintenance Mode
                    Chrome.Unload();
                }

                Chrome.UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear

                // Once all programs have been shutdown we need to restart them in the order
                // NODE --> CHROME
                // Variables for each CheckSum - Default to NO
                var chromeRunningNow = "NO";
                var nodeRunningNow = "NO";

                var osArch = GCMSSystem.GetOSArch();
                var chromeEXE = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\Google\\Chrome\\Application\\chrome.exe";
                var nodeEXE = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\node32.exe";
                if (osArch == "x64")
                {
                    nodeEXE = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\node64.exe";
                }
                // GCMSSystem.FileLogger.Log("Arch : " + osArch);

                var chromeProcessNew = Process.GetProcesses().Any(p => p.ProcessName.Contains("chrome"));
                if (chromeProcessNew)
                {
                    chromeRunningNow = "YES";
                }
                var nodeProcessNew = Process.GetProcesses().Any(p => p.ProcessName.Contains("node"));
                if (nodeProcessNew)
                {
                    nodeRunningNow = "YES";
                }

                // Before we reload the Signage, update the Signage Settings file
                var SettingsIni = new IniFile("signage/settings.conf");
                var settingsFile = GetSignageConfig(networkURL, macAddr);

                string[] varINI = settingsFile.Split(',');
                SettingsIni.Write("preloading", varINI[0], "core");
                SettingsIni.Write("midnightRefresh", varINI[1], "core");
                SettingsIni.Write("avaOn", varINI[2], "ava");
                SettingsIni.Write("utilioOn", varINI[3], "utilio");
                SettingsIni.Write("apiOn", varINI[4], "websocket_api");

                if (nodeRunningNow == "NO")
                {
                    // NodeJS isnt running
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
                }
                if (chromeRunningNow == "NO")
                {
                    Chrome.Load();
                }
                // Bring Google Chrome Kiosk To ON TOP
                bool isSignageEnabled = Chrome.IsSignageEnabled();
                if (isSignageEnabled)
                {
                    if (Chrome.whichVer == 1 && !MainForm.isDebug)
                    {
                        var chromeProcessGUID = "- Google Chrome";
                        WindowHelper.BringToFront(chromeProcessGUID);
                    }
                }
            }
            if (actualTrigger == "UPDATECHROME")
            {
                // Update Google Chrome
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Update Kiosk Wrapper");
            }
            if (actualTrigger == "UPDATESIGNSYNC")
            {
                // Clone Sync ID - Installs the Given Sync ID to syncid file
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Clone SyncID");
                // Variables for each CheckSum - Default to NO
                var chromeRunning = "NO";
                var node32Running = "NO";
                var node64Running = "NO";
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                // MAC Address
                var macStr = macAddr.Replace(":", "-");
                var uuidStr = MyIni.Read("deviceUUID", "Monitor");
                var clientID = MyIni.Read("clientID", "Monitor");
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

                // Check again if Chrome has been opened while the user is in Maintenance Mode
                var node32Process = Process.GetProcesses().Any(p => p.ProcessName.Contains("node32"));
                if (node32Process)
                {
                    node32Running = "YES";
                }

                // node32 is running
                if (node32Running == "YES")
                {
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
                }
                var node64Process = Process.GetProcesses().Any(p => p.ProcessName.Contains("node64"));
                if (node64Process)
                {
                    node64Running = "YES";
                }

                // node64 is running
                if (node64Running == "YES")
                {
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
                var chromeProcess = Process.GetProcesses().Any(p => p.ProcessName.Contains("chrome"));
                if (chromeProcess)
                {
                    chromeRunning = "YES";
                }

                if (chromeRunning == "YES")
                {
                    // Chrome is running - probably due to someone opening it while in Maintenance Mode
                    Chrome.Unload();
                }

                Chrome.UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear

                // Once all programs have been shutdown we need to restart them in the order
                // NODE --> CHROME
                // Variables for each CheckSum - Default to NO
                var chromeRunningNow = "NO";
                var nodeRunningNow = "NO";

                var osArch = GCMSSystem.GetOSArch();
                var chromeEXE = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\Google\\Chrome\\Application\\chrome.exe";
                var nodeEXE = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\node32.exe";
                if (osArch == "x64")
                {
                    nodeEXE = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\node64.exe";
                }
                // GCMSSystem.FileLogger.Log("Arch : " + osArch);

                var chromeProcessNew = Process.GetProcesses().Any(p => p.ProcessName.Contains("chrome"));
                if (chromeProcessNew)
                {
                    chromeRunningNow = "YES";
                }
                var nodeProcessNew = Process.GetProcesses().Any(p => p.ProcessName.Contains("node"));
                if (nodeProcessNew)
                {
                    nodeRunningNow = "YES";
                }

                // Before we reload the Signage, update the Signage SyncID File
                File.Delete(@Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\nodejsData\\syncid");
                var syncID = GetSyncID(networkURL, macAddr, uuidStr);
                File.WriteAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\nodejsData\\syncid", syncID);

                if (nodeRunningNow == "NO")
                {
                    // NodeJS isnt running
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
                }
                if (chromeRunningNow == "NO")
                {
                    Chrome.Load();
                }
                // Bring Google Chrome Kiosk To ON TOP
                bool isSignageEnabled = Chrome.IsSignageEnabled();
                if (isSignageEnabled)
                {
                    if (Chrome.whichVer == 1 && !MainForm.isDebug)
                    {
                        var chromeProcessGUID = "- Google Chrome";
                        WindowHelper.BringToFront(chromeProcessGUID);
                    }
                }
            }
            if (actualTrigger == "WBSKT")
            {
                // Monitor has recieved a Websocket from somewhere now we need to capture that and redirect it to the NODEJS Local Server
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
                        var pingTest = GCMSSystem.Ping(networkURL);
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
                        ["hardwareMAC"] = MyIni.Read("hardwareMAC", "Monitor")
                    };

                    var responseString = "";
                    try
                    {
                        var response = client.UploadValues(networkURL + "/v2/outboundWebsocket.php", values);
                        responseString = Encoding.Default.GetString(response);
                        responseString = GCMSSystem.RemoveWhitespace(responseString);
                        responseString = responseString.ToUpper();
                    }
                    catch
                    {
                        responseString = "Error";
                    }

                    FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] WEBSOCKET : " + responseString);
                    try
                    {
                        var socketStr = NodeSocket.Send(responseString);
                        // Debug.WriteLine("Websocket Node Response  : " + socketStr);
                    }
                    catch { }
                }
            }
            if (actualTrigger == "SKIN")
            {
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Apply Application Skin");
                Skin.Update();
                // Once Skin has been applied we need to reboot the machine for the skin to fully take effect
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
            if (actualTrigger == "ROTATEDEFAULT") 
            {
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Rotate 0 degree");
                ScreenRotation.SetOrientation(1, 0);
            }
            if (actualTrigger == "ROTATERIGHT")
            {
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Rotate 45 degree");
                ScreenRotation.SetOrientation(1, 45);
            }
            if (actualTrigger == "ROTATEBOTTOM")
            {
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Rotate 90 degree");
                ScreenRotation.SetOrientation(1, 90);
            }
            if (actualTrigger == "ROTATELEFT")
            {
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Rotate 135 degree");
                ScreenRotation.SetOrientation(1, 135);
            }
            if (actualTrigger == "LOADERSWITCH")
            {
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Switch Signage Loader");
                Chrome.Unload();
                Chrome.UpdatePref();

                // This will load the current signage loader version and then either switch from 1 to 2 -or- 2 to 1
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                // Setup which Network we should run over
                var curLoader = MyIni.Read("SignageLoader", "Sign");
                if (curLoader == "1")
                {
                    // Loader 1 currently so we need to set the new Loader to 2
                    MyIni.Write("SignageLoader", "2", "Sign");
                    MainForm.signageLoader = 2;
                }
                else
                {
                    // Loader 2 currently so we need to set the new Loader to 1
                    MyIni.Write("SignageLoader", "1", "Sign");
                    MainForm.signageLoader = 1;
                }

                // Once Loader has been applied we need to reboot the machine for the skin to fully take effect
                Chrome.Load();
            }
            if (actualTrigger == "SIGNAGELOADING")
            {
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Signage Loading Switch");
                Chrome.Unload();
                Chrome.UpdatePref();

                // This will load the current signage loader version and then either switch from 1 to 2 -or- 2 to 1
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                // Setup which Network we should run over
                var curLoading = MyIni.Read("Signage", "Serv");
                if (curLoading == "Enabled")
                {
                    // Loading Currently Enabled so we need to Disable it
                    MyIni.Write("Signage", "Disabled", "Serv");
                    // Once Loader has been applied we need to reboot the machine for the skin to fully take effect
                    Chrome.Unload();        // Just to make sure !!!
                }
                else
                {
                    // Loading Currently Disabled so we need to Enable it
                    MyIni.Write("Signage", "Enabled", "Serv");
                    // Once Loader has been applied we need to reboot the machine for the skin to fully take effect
                    Chrome.Load();
                }
            }
            if (actualTrigger == "UPDATESCRCFG")
            {
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] UPDATE SCREEN CONFIG");
                // This function grabs the Screen Config stored on server, and updates the config.ini with new values
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
                        var pingTest = GCMSSystem.Ping(networkURL);
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
                        ["hardwareMAC"] = MyIni.Read("hardwareMAC", "Monitor")
                    };

                    var responseString = "";
                    try
                    {
                        var response = client.UploadValues(networkURL + "/v2/outboundScreenConfig.php", values);
                        responseString = Encoding.Default.GetString(response);
                        responseString = GCMSSystem.RemoveWhitespace(responseString);
                    }
                    catch
                    {
                        responseString = "Error";
                    }

                    if (responseString != "Error")
                    {
                        string[] screenConfig = responseString.Split(',');
                        string forceSettings = screenConfig[0];
                        string resolution = screenConfig[1];
                        string orientation = screenConfig[2];
                        string scaling = screenConfig[3];
                        string cloneScreens = screenConfig[4];

                        MyIni.Write("Force", forceSettings, "Display");
                        MyIni.Write("Resolution", resolution, "Display");
                        MyIni.Write("Scaling", scaling, "Display");
                        MyIni.Write("Orientation", orientation, "Display");
                        MyIni.Write("Clone", cloneScreens, "Display");

                        if (cloneScreens == "On")
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

                        string[] resTokens = resolution.Split('x');

                        var setW = Convert.ToInt32(resTokens[0]);
                        var setH = Convert.ToInt32(resTokens[1]);

                        try
                        {
                            ScreenScaling.SetScaleFactor(scaling);                  // Set the Scale Factor
                        }
                        catch { }
                        try
                        {
                            ScreenResolution.SetResoltion(setW, setH);              // Set the Screen Resolution
                        }
                        catch { }

                        if (orientation == "Landscape")
                        {
                            ScreenRotation.SetOrientation(1, 0);
                        }
                        if (orientation == "Landscape-Flip")
                        {
                            ScreenRotation.SetOrientation(1, 90);
                        }
                        if (orientation == "Portrait")
                        {
                            ScreenRotation.SetOrientation(1, 45);
                        }
                        if (orientation == "Portrait-Flip")
                        {
                            ScreenRotation.SetOrientation(1, 135);
                        }
                    }
                }
            }
            if (actualTrigger == "UPDATELAUNCHER")
            {
                // This function updates the launcher.ini so that it can launch 3rd party applications
                string appIniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "launcher.ini");

                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] UPDATE 3RD PARTY LAUNCH CONFIG");
                // This function grabs the Screen Config stored on server, and updates the config.ini with new values
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
                        var pingTest = GCMSSystem.Ping(networkURL);
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
                        ["hardwareMAC"] = MyIni.Read("hardwareMAC", "Monitor")
                    };

                    var responseString = "";
                    try
                    {
                        var response = client.UploadValues(networkURL + "/v2/outboundLaunchConfig.php", values);
                        responseString = Encoding.UTF8.GetString(response);
                        // Debug.WriteLine(responseString);
                    }
                    catch
                    {
                        responseString = "Error";
                    }

                    if (responseString != "Error")
                    {
                        if (responseString == "")
                        {
                            File.WriteAllText(appIniFile, string.Empty);
                        }
                        else
                        {
                            File.WriteAllText(appIniFile, string.Empty);
                            var outputData = responseString.Split(',');
                            for (int i = 0; i < outputData.Length; i++)
                            {
                                if (outputData[i] != "" && outputData[i] != null)
                                {
                                    using (StreamWriter w = File.AppendText(appIniFile))
                                    {
                                        if (!string.IsNullOrEmpty(outputData[i]))
                                        {
                                            w.WriteLine(outputData[i]);
                                        }
                                    }
                                }
                            }

                            File.WriteAllLines(appIniFile, File.ReadAllLines(appIniFile).Where(l => !string.IsNullOrWhiteSpace(l)));
                        }
                    }
                }
            }
            if (actualTrigger == "LOCKTRIAL")
            {
                // This function is for dynamically adding the Trial Period on a machine
                var trialFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "trial.lock");
                if (!File.Exists(trialFile))
                {
                    File.Create(trialFile).Dispose();

                    MainForm.FrmObj.TrialLicTxt.Text = "TRIAL LICENCE (Expires in 30 days)";
                    MainForm.isTrial = true;
                    MainForm.FrmObj.TrialLicTxt.Visible = true;

                    MainForm.FrmObj.CheckTrial.Interval = 60000;
                    MainForm.FrmObj.CheckTrial.Enabled = true;
                }
            }
            if (actualTrigger == "UNLOCKTRIAL")
            {
                // This function is for dynamically removing the Trial Period on a machine
                var trialFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "trial.lock");
                if (File.Exists(trialFile))
                {
                    try
                    {
                        File.Delete(trialFile);
                    }
                    catch { }

                    MainForm.isTrial = false;
                    MainForm.FrmObj.TrialLicTxt.Visible = false;

                    MainForm.FrmObj.CheckTrial.Interval = 60000;
                    MainForm.FrmObj.CheckTrial.Enabled = false;
                }
            }
            if (actualTrigger == "DOWNLOADBETA")
            {
                if (!MainForm.isUselessInternet)
                {
                    FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Update To BETA Monitor");
                    Updater.DownloadMonitor(true);
                }
            }
            if (actualTrigger == "DOWNLOADBETAOLD")
            {
                if (!MainForm.isUselessInternet)
                {
                    // Update with BETA Software - Download updatebetatool.bat this is a self timebombed batch file which downloads GlobalCMS BETA Monitor + Service and all DLL Files
                    FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Update To BETA Monitor");
                    DownloadFileSingle("http://api.globalcms.co.uk/v2/monitorUpdate/curl.exe", @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\curl.exe");
                    DownloadFileSingle("http://api.globalcms.co.uk/v2/monitorUpdate/updatebetatool.bat", @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\updatebetatool.bat");

                    var updateTool = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\updatebetatool.bat";
                    if (File.Exists(updateTool))
                    {
                        try
                        {
                            Process updateProc = new Process();
                            updateProc.StartInfo.FileName = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\updatebetatool.bat";
                            updateProc.StartInfo.Verb = "runas";
                            updateProc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                            updateProc.Start();
                        }
                        catch { }
                    }
                }
            }
            if (actualTrigger == "DOWNLOADBETASIGN")
            {
                if (!MainForm.isUselessInternet)
                {
                    FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Update To BETA Signage");
                    Updater.DownloadSignage(true);
                }
            }
            if (actualTrigger == "DOWNLOADBETASIGNOLD")
            {
                if (!MainForm.isUselessInternet)
                {
                    // Update with BETA Signage - Download the beta signage.zip 
                    // if (Chrome.whichVer == 2)
                    // {
                    bool isSignageEnabled = Chrome.IsSignageEnabled();
                    if (isSignageEnabled)
                    {
                        var updateForm = new Updating();
                        updateForm.Show();
                    }
                    // }

                    // Update Signage
                    FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Update To BETA Signage");

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
                        Chrome.Unload();
                        Chrome.UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear
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
                    MainForm.FrmObj.CheckSNAP.Interval = 60000 + (MainForm.snapDelay * 1000);

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
                        DownloadFileSingle(networkURL + "/v2/signageUpdate/beta/signage.zip", "signage.zip");
                    }
                    catch
                    {

                    }

                    // Unzip Signage.zip to the Signage Folder inside monitor main root DIR
                    var signageZipFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage.zip";
                    var signageZipFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    try
                    {
                        ZipFile.ExtractToDirectory(signageZipFile, signageZipFolder);
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
                        DownloadFileSingle(networkURL + "/v2/signageUpdate/websocket.js", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\websocket.js");
                    }
                    var WindowsMessageRelayEXE = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\WindowsMessageRelay.exe";
                    if (!File.Exists(WindowsMessageRelayEXE))
                    {
                        DownloadFileSingle(networkURL + "/v2/signageUpdate/WindowsMessageRelay.exe", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\WindowsMessageRelay.exe");
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

                    var browserSSL = MyIni.Read("SSL", "Browser");
                    var sslon = "false";
                    if (browserSSL == "On")
                    {
                        sslon = "true";
                    }
                    string signageConfFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "settings.conf");
                    var SettingsIni = new IniFile(signageConfFile);
                    SettingsIni.Write("sslOn", sslon, "core");

                    Chrome.UpdateCallHome();            // On Updating and Extracting the new signage we need to make sure we lock back onto which Timers to use

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

                    Chrome.UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear
                    Chrome.Load();
                    // if (Chrome.whichVer == 2)
                    // {
                    Updating.frmObj.Close();
                    // }
                }
            }
            if (actualTrigger == "FLUSHCACHE")
            {
                // This function is to allow flushing the cache on the fly from the portal
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Flush Cache");
                Chrome.Unload();                    // Due to Chrome using SQL to store cookies we need to make sure Chrome is shutdown to unlock the files
                Chrome.UpdatePref();
                Chrome.ClearCookies(true);          // Clear Cookies
                Chrome.Load();
            }
            if (actualTrigger == "SETSIGNCALLHOME")
            {
                // This function is to allow flushing the cache on the fly from the portal
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Set Signage Call Home Timer");
                Chrome.Unload();                     // Due to Chrome using SQL to store cookies we need to make sure Chrome is shutdown to unlock the files
                Chrome.UpdatePref();
                Chrome.UpdateCallHome();
                Chrome.Load();
            }
            if (actualTrigger == "LOCKDOWN")
            {
                // This function locks down the system and places a lockdown screen ontop
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] LOCKDOWN!");

                // Stop all Timers that are not needed while locked
                MainForm.FrmObj.CheckServicesTimer.Stop();
                MainForm.FrmObj.CheckStatsTimer.Stop();
                MainForm.FrmObj.CheckSNAP.Stop();
                MainForm.FrmObj.CheckSNAP.Interval = 60000 + (MainForm.snapDelay * 1000);

                MainForm.FrmObj.LauncherTimer.Enabled = false;
                MainForm.FrmObj.LauncherTimer.Stop();
                MainForm.FrmObj.LauncherTimer.Interval = 60000;

                MainForm.FrmObj.CheckForInteractive.Enabled = false;
                MainForm.FrmObj.CheckForInteractive.Stop();
                MainForm.FrmObj.CheckForInteractive.Interval = 5000;

                Chrome.Unload();
                Chrome.UpdatePref();

                // Place a file on the drive to lockdown the machine
                var lockFile = Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\globalcms.lock";
                File.WriteAllText(lockFile, "Asset In Lockdown Mode");

                MainForm.FrmObj.powerModeLabel.Text = "Lockdown";
                MainForm.FrmObj.powerModeLabel.ForeColor = Color.FromArgb(255, 0, 0);

                MainForm.isInLockdown = true;

                // Load the form for Lockdown
                var deviceLocked = new DeviceLocked();
                deviceLocked.Show();
            }
            if (actualTrigger == "LOCKDOWNOFF")
            {
                // This function locks down the system and places a lockdown screen ontop
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] LOCKDOWN REMOVED");
                Chrome.Unload();
                Chrome.UpdatePref();

                // Remove the Decom.lock file so that it will automatically kick in properly again
                var lockFile = Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\globalcms.lock";
                File.Delete(lockFile);

                // Start all Timers that are needed
                MainForm.FrmObj.CheckServicesTimer.Start();
                MainForm.FrmObj.CheckStatsTimer.Start();
                MainForm.FrmObj.CheckSNAP.Start();
                MainForm.FrmObj.CheckSNAP.Interval = 60000 + (MainForm.snapDelay * 1000);

                MainForm.FrmObj.LauncherTimer.Enabled = true;
                MainForm.FrmObj.LauncherTimer.Start();
                MainForm.FrmObj.LauncherTimer.Interval = 60000;

                Chrome.Load();

                MainForm.FrmObj.CheckForInteractive.Enabled = true;
                MainForm.FrmObj.CheckForInteractive.Start();
                MainForm.FrmObj.CheckForInteractive.Interval = 5000;

                MainForm.isInLockdown = false;

                MainForm.FrmObj.powerModeLabel.Text = "Normal / Online";
                MainForm.FrmObj.powerModeLabel.ForeColor = Color.FromArgb(0, 0, 0);

                Application.Exit();
            }
            if (actualTrigger == "HDMISWITCH1")
            {
                // This function will trigger the HDMI Switch via RS232 (Python Script) to trigger a change of channel
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Change Channel on HDMI Switch - Channel #1");
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                var macStr = macAddr.Replace(":", "-");
                var uuidStr = MyIni.Read("deviceUUID", "Monitor");
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
                    var pingTest = GCMSSystem.Ping(networkURL);
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

                // Trigger the RS232 Serial Command
                using (var client = new WebClient())
                {
                    // Create the $_POST Data for the HTTP Request
                    var values = new NameValueCollection
                    {
                        ["clientID"] = MyIni.Read("clientID", "Monitor"),
                        ["deviceID"] = MyIni.Read("deviceName", "Monitor"),
                        ["deviceUUID"] = MyIni.Read("deviceUUID", "Monitor"),
                        ["theCtrlCode"] = "HDSwitch",
                    };

                    var responseString = "";
                    try
                    {
                        var response = client.UploadValues(networkURL + "/v2/getControlCodeData.php", values);
                        responseString = Encoding.Default.GetString(response);
                        responseString = GCMSSystem.RemoveWhitespace(responseString);
                    }
                    catch
                    {
                        responseString = "Error";
                    }

                    if (!File.Exists("logs/ctrlCodes/" + responseString + "_channel1.py"))
                    {
                        DownloadFileSingle(networkURL + "/monitorUpdate/scripts/controlcodes/" + responseString + "_channel1.py", "logs/ctrlCodes/" + responseString + "_channel1.py");
                    }

                    // Trigger RS232 Python Script
                    var pythonRun = Python.Run("logs/ctrlCodes/" + responseString + "_channel1.py");
                }
            }
            if (actualTrigger == "HDMISWITCH2")
            {
                // This function will trigger the HDMI Switch via RS232 (Python Script) to trigger a change of channel
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Change Channel on HDMI Switch - Channel #2");
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                var macStr = macAddr.Replace(":", "-");
                var uuidStr = MyIni.Read("deviceUUID", "Monitor");
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
                    var pingTest = GCMSSystem.Ping(networkURL);
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

                // Trigger the RS232 Serial Command
                using (var client = new WebClient())
                {
                    // Create the $_POST Data for the HTTP Request
                    var values = new NameValueCollection
                    {
                        ["clientID"] = MyIni.Read("clientID", "Monitor"),
                        ["deviceID"] = MyIni.Read("deviceName", "Monitor"),
                        ["deviceUUID"] = MyIni.Read("deviceUUID", "Monitor"),
                        ["theCtrlCode"] = "HDSwitch",
                    };

                    var responseString = "";
                    try
                    {
                        var response = client.UploadValues(networkURL + "/v2/getControlCodeData.php", values);
                        responseString = Encoding.Default.GetString(response);
                        responseString = GCMSSystem.RemoveWhitespace(responseString);
                    }
                    catch
                    {
                        responseString = "Error";
                    }

                    if (!File.Exists("logs/ctrlCodes/" + responseString + "_channel2.py"))
                    {
                        DownloadFileSingle(networkURL + "/monitorUpdate/scripts/controlcodes/" + responseString + "_channel2.py", "logs/ctrlCodes/" + responseString + "_channel2.py");
                    }

                    // Trigger RS232 Python Script
                    var pythonRun = Python.Run("logs/ctrlCodes/" + responseString + "_channel2.py");
                }
            }
            if (actualTrigger == "HDMISWITCH3")
            {
                // This function will trigger the HDMI Switch via RS232 (Python Script) to trigger a change of channel
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Change Channel on HDMI Switch - Channel #3");
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                var macStr = macAddr.Replace(":", "-");
                var uuidStr = MyIni.Read("deviceUUID", "Monitor");
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
                    var pingTest = GCMSSystem.Ping(networkURL);
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

                // Trigger the RS232 Serial Command
                using (var client = new WebClient())
                {
                    // Create the $_POST Data for the HTTP Request
                    var values = new NameValueCollection
                    {
                        ["clientID"] = MyIni.Read("clientID", "Monitor"),
                        ["deviceID"] = MyIni.Read("deviceName", "Monitor"),
                        ["deviceUUID"] = MyIni.Read("deviceUUID", "Monitor"),
                        ["theCtrlCode"] = "HDSwitch",
                    };

                    var responseString = "";
                    try
                    {
                        var response = client.UploadValues(networkURL + "/v2/getControlCodeData.php", values);
                        responseString = Encoding.Default.GetString(response);
                        responseString = GCMSSystem.RemoveWhitespace(responseString);
                    }
                    catch
                    {
                        responseString = "Error";
                    }

                    if (!File.Exists("logs/ctrlCodes/" + responseString + "_channel3.py"))
                    {
                        DownloadFileSingle(networkURL + "/monitorUpdate/scripts/controlcodes/" + responseString + "_channel3.py", "logs/ctrlCodes/" + responseString + "_channel3.py");
                    }

                    // Trigger RS232 Python Script
                    var pythonRun = Python.Run("logs/ctrlCodes/" + responseString + "_channel3.py");
                }
            }
            if (actualTrigger == "HDMISWITCH4")
            {
                // This function will trigger the HDMI Switch via RS232 (Python Script) to trigger a change of channel
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Change Channel on HDMI Switch - Channel #4");
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                var macStr = macAddr.Replace(":", "-");
                var uuidStr = MyIni.Read("deviceUUID", "Monitor");
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
                    var pingTest = GCMSSystem.Ping(networkURL);
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

                // Trigger the RS232 Serial Command
                using (var client = new WebClient())
                {
                    // Create the $_POST Data for the HTTP Request
                    var values = new NameValueCollection
                    {
                        ["clientID"] = MyIni.Read("clientID", "Monitor"),
                        ["deviceID"] = MyIni.Read("deviceName", "Monitor"),
                        ["deviceUUID"] = MyIni.Read("deviceUUID", "Monitor"),
                        ["theCtrlCode"] = "HDSwitch",
                    };

                    var responseString = "";
                    try
                    {
                        var response = client.UploadValues(networkURL + "/v2/getControlCodeData.php", values);
                        responseString = Encoding.Default.GetString(response);
                        responseString = GCMSSystem.RemoveWhitespace(responseString);
                    }
                    catch
                    {
                        responseString = "Error";
                    }

                    if (!File.Exists("logs/ctrlCodes/" + responseString + "_channel4.py"))
                    {
                        DownloadFileSingle(networkURL + "/monitorUpdate/scripts/controlcodes/" + responseString + "_channel4.py", "logs/ctrlCodes/" + responseString + "_channel4.py");
                    }

                    // Trigger RS232 Python Script
                    var pythonRun = Python.Run("logs/ctrlCodes/" + responseString + "_channel4.py");
                }
            }
            if (actualTrigger == "SIGNDEBUGON")
            {
                // This function allowed to turn on the Debug Mode in Signage Browser
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Turn On Debug Mode - Signage");
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                Chrome.Unload();
                Chrome.UpdatePref();
                MyIni.Write("Debug", "On", "Browser");
                Chrome.Load();
            }
            if (actualTrigger == "SIGNDEBUGOFF")
            {
                // This function allowed to turn off the Debug Mode in Signage Browser
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Turn Off Debug Mode - Signage");
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                Chrome.Unload();
                Chrome.UpdatePref();
                MyIni.Write("Debug", "Off", "Browser");
                Chrome.Load();
            }
            if (actualTrigger == "UPDATESIGNSETTINGS")
            {
                // This function updates all the other settings for Signage
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Update Signage Settings");
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                Chrome.Unload();
                Chrome.UpdatePref();

                var uuidStr = MyIni.Read("deviceUUID", "Monitor");
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
                    var pingTest = GCMSSystem.Ping(networkURL);
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

                // Trigger the Remote Command to update Signage Config
                using (var client = new WebClient())
                {
                    // Create the $_POST Data for the HTTP Request
                    var values = new NameValueCollection
                    {
                        ["clientID"] = MyIni.Read("clientID", "Monitor"),
                        ["deviceID"] = MyIni.Read("deviceName", "Monitor"),
                        ["deviceUUID"] = MyIni.Read("deviceUUID", "Monitor"),
                        ["theCtrlCode"] = "HDSwitch",
                    };

                    var responseString = "";
                    try
                    {
                        var response = client.UploadValues(networkURL + "/v2/getSignSettings.php", values);
                        responseString = Encoding.Default.GetString(response);
                        responseString = GCMSSystem.RemoveWhitespace(responseString);
                        if (responseString == "")
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
                        dynamic json = JObject.Parse(responseString);
                        string signageURL = json.url;
                        string signageDebug = json.debug;
                        string signageXSS = json.xss;
                        string signageSSL = json.ssl;
                        string signageKeyboard = json.keyboard;

                        MyIni.Write("Debug", signageDebug, "Browser");
                        MyIni.Write("Keyboard", signageKeyboard, "Browser");
                        MyIni.Write("SSL", signageSSL, "Browser");

                        MyIni.Write("Referer", signageXSS, "Browser");
                        MyIni.Write("Load", signageURL, "Browser");
                    }
                }
                Chrome.Load();
            }
            if (actualTrigger == "ROLLBKSIGNAGE")
            {
                if (!MainForm.isUselessInternet)
                {
                    // This function is for triggering from the portal to Roll Back the Signage
                    // Update Signage
                    FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Roll Back Signage");

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
                        Chrome.Unload();
                        Chrome.UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear
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
                        DownloadFileSingle(networkURL + "/v2/signageUpdate/signageLast.zip", "signage.zip");
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
                        DownloadFileSingle(networkURL + "/v2/signageUpdate/websocket.js", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\websocket.js");
                    }
                    var WindowsMessageRelayEXE = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\WindowsMessageRelay.exe";
                    if (!File.Exists(WindowsMessageRelayEXE))
                    {
                        DownloadFileSingle(networkURL + "/v2/signageUpdate/WindowsMessageRelay.exe", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\WindowsMessageRelay.exe");
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

                    Chrome.UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear
                }
            }
            if (actualTrigger == "UPDATEENVCONF")
            {
                // This function is for the Omron Environmental Sensor, this will grab from API Server its config for timers and triggers
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Update Env Sensor Config");

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
                        ["hardwareMAC"] = MyIni.Read("hardwareMAC", "Monitor")
                    };

                    var responseString = "";
                    try
                    {
                        var response = client.UploadValues(networkURL + "/v2/outboundEnvConfig.php", values);
                        responseString = Encoding.UTF8.GetString(response);
                        responseString = GCMSSystem.RemoveWhitespace(responseString);
                    }
                    catch
                    {
                        responseString = "Error";
                        FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [ERR] Cannot Call API Server [" + networkNameShort + "] - Check Internet/Firewall");
                    }

                    if (responseString != "Error")
                    {
                        var data = (JObject)JsonConvert.DeserializeObject(responseString);
                        string CoolDown = data["Cooldown"].Value<string>();

                        double TempOver = data["TempOver"].Value<double>();
                        string TempOver_Enable = data["TempOver_Enable"].Value<string>();

                        double TempUnder = data["TempUnder"].Value<double>();
                        string TempUnder_Enable = data["TempUnder_Enable"].Value<string>();

                        string LightOver = data["LightOver"].Value<string>();
                        string LightOver_Enable = data["LightOver_Enable"].Value<string>();

                        string LightUnder = data["LightUnder"].Value<string>();
                        string LightUnder_Enable = data["LightUnder_Enable"].Value<string>();

                        double NoiseOver = data["NoiseOver"].Value<double>();
                        string NoiseOver_Enable = data["NoiseOver_Enable"].Value<string>();

                        // Write all the changes to the config.ini
                        MyIni.Write("TimerQuiet", CoolDown, "Environmental");
                        MyIni.Write("TempOver", TempOver.ToString(), "Environmental");
                        MyIni.Write("TempOver_Enable", TempOver_Enable, "Environmental");
                        MyIni.Write("TempUnder", TempUnder.ToString(), "Environmental");
                        MyIni.Write("TempUnder_Enable", TempUnder_Enable, "Environmental");
                        MyIni.Write("LightOver", LightOver.ToString(), "Environmental");
                        MyIni.Write("LightOver_Enable", LightOver_Enable, "Environmental");
                        MyIni.Write("LightUnder", LightUnder.ToString(), "Environmental");
                        MyIni.Write("LightUnder_Enable", TempUnder_Enable, "Environmental");
                        MyIni.Write("NoiseOver", NoiseOver.ToString(), "Environmental");
                        MyIni.Write("NoiseOver_Enable", NoiseOver_Enable, "Environmental");

                        // Set the Omron Global Triggers (To Enable/Disable)
                        OmronSensor.globalTempOverTrigger = TempOver_Enable;
                        OmronSensor.globalTempUnderTrigger = TempUnder_Enable;
                        OmronSensor.globalLightOverTrigger = LightOver_Enable;
                        OmronSensor.globalLightUnderTrigger = LightUnder_Enable;
                        OmronSensor.globalNoiseOverTrigger = NoiseOver_Enable;
                    }
                }
            }
            if (actualTrigger == "AIRSERVERINSTALL")
            {
                // This function is for activating/installing AirServer for (Mirroring)
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Install Mirroring");
                var osArch = GCMSSystem.GetOSArch();

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
                if (MainForm.NetworkOverride != "Auto" && MainForm.NetworkOverride == "Public") { networkURL = "https://api.globalcms.co.uk"; }
                if (MainForm.NetworkOverride != "Auto" && MainForm.NetworkOverride == "Secure") { networkURL = "http://172.16.0.2"; }

                var airServerCode = AirServer.GetCode(networkURL, networkNameShort);
                if (airServerCode != "")
                {
                    AirServer.Install(networkURL);
                    System.Threading.Thread.Sleep(5000);             // Wait for 5 seconds
                    AirServer.Activate(airServerCode);
                    System.Threading.Thread.Sleep(5000);             // Wait for 5 seconds
                    AirServer.SetPassword(0);
                    System.Threading.Thread.Sleep(5000);             // Wait for 5 seconds
                    AirServer.Start();
                }
            }
            if (actualTrigger == "AIRSERVERUNINSTALL")
            {
                // This function is for deactivating/uninstalling AirServer (Mirroring)
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Uninstall Mirroring");

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

                AirServer.Stop();
                System.Threading.Thread.Sleep(5000);             // Wait for 5 seconds
                AirServer.Deactivate();
                System.Threading.Thread.Sleep(5000);             // Wait for 5 seconds
                AirServer.Uninstall(networkURL);
            }
            if (actualTrigger == "AIRSERVERCONFIG")
            {
                // This function is for deactivating/uninstalling AirServer (Mirroring)
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Update Mirroring Config");

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
                if (MainForm.NetworkOverride != "Auto" && MainForm.NetworkOverride == "Public") { networkURL = "https://api.globalcms.co.uk"; }
                if (MainForm.NetworkOverride != "Auto" && MainForm.NetworkOverride == "Secure") { networkURL = "http://172.16.0.2"; }

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
                        ["hardwareMAC"] = MyIni.Read("hardwareMAC", "Monitor")
                    };

                    var responseString = "";
                    try
                    {
                        var response = client.UploadValues(networkURL + "/v2/outboundAirServerConfig.php", values);
                        responseString = Encoding.UTF8.GetString(response);
                        responseString = GCMSSystem.RemoveWhitespace(responseString);
                    }
                    catch
                    {
                        responseString = "Error";
                        FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [ERR] Cannot Call API Server [" + networkNameShort + "] - Check Internet/Firewall");
                    }

                    if (responseString != "Error")
                    {
                        string[] AirServerArr = responseString.Split(',');
                        string NewDeviceName = AirServerArr[0].ToString();
                        var NewPasscode = Convert.ToInt32(AirServerArr[1].ToString());

                        AirServer.Stop();            // AirServer Must Be Stopped Before Running Configs
                        AirServer.Kill();
                        AirServer.SetPassword(NewPasscode);
                        AirServer.SetName(NewDeviceName);
                        AirServer.Start();
                    }
                }
            }
            if (actualTrigger == "AIRSERVERSTART")
            {
                // This function is for deactivating/uninstalling AirServer (Mirroring)
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Start Mirroring");
                AirServer.Start();
            }
            if (actualTrigger == "AIRSERVERSTOP")
            {
                // This function is for deactivating/uninstalling AirServer (Mirroring)
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Stop Mirroring");
                AirServer.Stop();
            }
            if (actualTrigger == "AIRSERVERCHGPASSCODE")
            {
                // This function is for deactivating/uninstalling AirServer (Mirroring)
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] AirServer Change Passcode");

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

                AirServer.Stop();                               // Stop the AirServer
                System.Threading.Thread.Sleep(5000);            // Wait for 5 seconds

                // Get the new Passcode given from the Portal 
                int theCode = 0;

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
                        ["hardwareMAC"] = MyIni.Read("hardwareMAC", "Monitor")
                    };

                    var responseString = "";
                    try
                    {
                        var response = client.UploadValues(networkURL + "/v2/outboundAirNewPassword.php", values);
                        responseString = Encoding.UTF8.GetString(response);
                        responseString = GCMSSystem.RemoveWhitespace(responseString);
                    }
                    catch
                    {
                        responseString = "Error";
                        FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [ERR] Cannot Call API Server [" + networkNameShort + "] - Check Internet/Firewall");
                    }

                    if (responseString != "Error")
                    {

                        theCode = Convert.ToInt32(responseString);
                        AirServer.SetPassword(theCode);                  // Set the New AirServer Password for Connection (Must have a valid code)
                        System.Threading.Thread.Sleep(5000);             // Wait for 5 seconds
                    }
                }
                AirServer.Start();                            // Start AirServer back up after changing the password for connection
            }
            if (actualTrigger == "UPDATENEXUSCONF")
            {
                // This function always sits at the bottom, as a template for triggers
                // This function is for deactivating/uninstalling AirServer (Mirroring)
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Update Nexmosphere Config");

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
                        ["hardwareMAC"] = MyIni.Read("hardwareMAC", "Monitor")
                    };

                    var responseString = "";
                    try
                    {
                        var response = client.UploadValues(networkURL + "/v2/outboundNexusConfig.php", values);
                        responseString = Encoding.UTF8.GetString(response);
                    }
                    catch
                    {
                        responseString = "Error";
                        FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [ERR] Cannot Call API Server [" + networkNameShort + "] - Check Internet/Firewall");
                    }

                    if (responseString != "Error")
                    {
                        Nexmosphere.frmObj.Close();
                        string configFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "nexusConfig.ini");
                        try
                        {
                            System.Threading.Thread.Sleep(1000);
                            File.WriteAllText(configFile, responseString);
                        }
                        catch { }

                        if (!GCMSSystem.CheckOpened("Nexmosphere"))
                        {
                            Form Nexmosphere = new Nexmosphere();
                            Nexmosphere.Show();
                        }
                    }
                }
            }
            if (actualTrigger == "UPDATEKEYBOARDCONF")
            {
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Update Keyboard Config");
                // This function is for updating the configuration file for the Internal Keyboard Application
                GCMSSystem.InstallKeyboardConfig();
            }
            if (actualTrigger == "POORINETDISABLE")
            {
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Disable Poor Internet");
                // This function is for updating the configuration for Poor Internet
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                MyIni.Write("poorInternet", "Disabled", "Monitor");
                MainForm.isUselessInternet = false;
                MainForm.isUselessInternetTxt = "Disabled";
            }
            if (actualTrigger == "POORINETENABLE")
            {
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Enable Poor Internet");
                // This function is for updating the configuration for Poor Internet
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                MyIni.Write("poorInternet", "Enabled", "Monitor");
                MainForm.isUselessInternet = true;
                MainForm.isUselessInternetTxt = "Enabled";
            }
            if (actualTrigger == "RS232CONFUPDATE")
            {
                // This function is for RS232 Config for updating the ini
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] RS232 Config Update");
            }
            if (actualTrigger == "RS232CODE")
            {
                // This function is for RS232 Codes to be sent via Portal to RS232 Device (either Screen or iOT Device)
                var comPort = "COM1";
                var comCMD = "POWEROFF";
                FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] RS232 (" + comPort.ToString() + ") Command Received : " + comCMD);

                Rs232.RemoteSend(comPort, comCMD);
            }
            if (actualTrigger == "UPDATEAVACONF")
            {
                if (!lowPowerTrigger)
                {
                    FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Update AVA Config");
                }
                // This function is for updating the settings.conf of nodeJS Signage to update the variables for AVA Camera
                string signageIniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "settings.conf");
                var SignageIni = new IniFile(signageIniFile);

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
                        ["hardwareMAC"] = MyIni.Read("hardwareMAC", "Monitor")
                    };

                    var responseString = "";
                    try
                    {
                        var response = client.UploadValues(networkURL + "/v2/outboundAVAConfig.php", values);
                        responseString = Encoding.UTF8.GetString(response);
                    }
                    catch
                    {
                        responseString = "Error";
                        FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [ERR] Cannot Call API Server [" + networkNameShort + "] - Check Internet/Firewall");
                    }

                    if (responseString != "Error")
                    {
                        var data = (JObject)JsonConvert.DeserializeObject(responseString);
                        string Range_X = data["range_x"].Value<string>();
                        string Range_Y = data["range_y"].Value<string>();
                        string Size_Width = data["size_width"].Value<string>();
                        string Size_Height = data["size_height"].Value<string>();
                        string Config_Confidence = data["confidence"].Value<string>();
                        string Config_BodySize = data["bodysize"].Value<string>();
                        string Config_HandSize = data["handsize"].Value<string>();
                        string Config_FaceSize = data["facesize"].Value<string>();

                        if (!MainForm.isDebug)
                        {
                            Chrome.Unload();
                            Chrome.UpdatePref();
                        }
                        SignageIni.Write("avaRangeX", Range_X, "ava");
                        SignageIni.Write("avaRangeY", Range_Y, "ava");
                        SignageIni.Write("avaRangeWidth", Size_Width, "ava");
                        SignageIni.Write("avaRangeHeight", Size_Height, "ava");
                        SignageIni.Write("minConfidence", Config_Confidence, "ava");
                        SignageIni.Write("avaMinBodySize", Config_BodySize, "ava");
                        SignageIni.Write("avaMinHandSize", Config_HandSize, "ava");
                        SignageIni.Write("avaMinFaceSize", Config_FaceSize, "ava");
                        if (!MainForm.isDebug) { 
                            Chrome.Load();
                        }
                    }
                }


            }
            if (actualTrigger == "EXRAMENABLE")
            {
                // This function is for enabling ExRAM.exe (EOWP) for 64Bit Memory Allocation on Signage Browser
                if (!lowPowerTrigger)
                {
                    FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Enable exRAM");
                }
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                MyIni.Write("ExRAM", "On", "Browser");

                // Need to restart the application for the new changes to take effect
                System.Windows.Forms.Application.Restart();
                Environment.Exit(0);
            }
            if (actualTrigger == "EXRAMDISABLE")
            {
                // This function is for disabling ExRAM.exe (EOWP) for 64Bit Memory Allocation on Signage Browser
                if (!lowPowerTrigger)
                {
                    FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Disable exRAM");
                }
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                MyIni.Write("ExRAM", "Off", "Browser");

                // Need to restart the application for the new changes to take effect
                System.Windows.Forms.Application.Restart();
                Environment.Exit(0);
            }
            if (actualTrigger == "UPDATERNETWORK")
            {
                // This function is for the Updater Network - to force an Override
                if (!lowPowerTrigger)
                {
                    FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Chg Updater Network");
                }
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

                // Call Home on init, however the rest will be deligated over to a Timer
                using (var client = new WebClient())
                {
                    // Create the $_POST Data for the HTTP Request
                    var values = new NameValueCollection
                    {
                        ["devName"] = MyIni.Read("deviceName", "Monitor")
                    };

                    var responseString = "";
                    try
                    {
                        var response = client.UploadValues(networkURL + "/v2/checkForBlock.php", values);
                        responseString = RemoveWhitespace(Encoding.UTF8.GetString(response));
                    }
                    catch
                    {
                        responseString = "Error";
                        FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [ERR] Cannot Call API Server [" + networkNameShort + "] - Check Internet/Firewall");
                    }

                    if (responseString != "Error")
                    {
                        MyIni.Write("UpdateNetwork", responseString, "Monitor");
                        MainForm.NetworkOverride = responseString;
                    }
                }
            }
            if (actualTrigger == "UPDATEOFFLINEPWR")
            {
                // This function is for updating the Offline Power Settings for Screens.
                // 
                if (!lowPowerTrigger)
                {
                    FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Update Offline Power Settings");
                }
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

                // Call Home on init, however the rest will be deligated over to a Timer
                using (var client = new WebClient())
                {
                    // Create the $_POST Data for the HTTP Request
                    var values = new NameValueCollection
                    {
                        ["devName"] = MyIni.Read("deviceName", "Monitor")
                    };

                    var responseString = "";
                    try
                    {
                        var response = client.UploadValues(networkURL + "/v2/outboundOfflinePwr.php", values);
                        responseString = Encoding.UTF8.GetString(response);
                    }
                    catch
                    {
                        responseString = "Error";
                        FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [ERR] Cannot Call API Server [" + networkNameShort + "] - Check Internet/Firewall");
                    }

                    if ((responseString != "Error" && responseString == "Disabled") && (!responseString.Contains("MySQL")))
                    {
                        // If the system is Disabled - Reset to Defaults
                        string powerFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "powerConfig.ini");
                        IniFile PowerIni = new IniFile(powerFile);
                        PowerIni.Write("Status", "Off", "System");
                        PowerIni.Write("Type", "Virtual", "System");

                        PowerIni.Write("Mon", "0900,1700,NA", "Schedule");
                        PowerIni.Write("Tue", "0900,1700,NA", "Schedule");
                        PowerIni.Write("Wed", "0900,1700,NA", "Schedule");
                        PowerIni.Write("Thu", "0900,1700,NA", "Schedule");
                        PowerIni.Write("Fri", "0900,1700,NA", "Schedule");
                        PowerIni.Write("Sat", "0900,1700,NA", "Schedule");
                        PowerIni.Write("Sun", "0900,1700,NA", "Schedule");
                    }

                    if ((responseString != "Error" && responseString != "Disabled") && (!responseString.Contains("MySQL")))
                    {
                        string configFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "powerConfig.ini");
                        try
                        {
                            System.Threading.Thread.Sleep(1000);
                            File.WriteAllText(configFile, responseString);
                        }
                        catch { }
                    }
                }
                if (GCMSSystem.CheckOpened("EngineeringTools"))
                {
                    EngineerTools.FrmObj.PowerGrabPowerCnfLocalBTN.PerformClick();
                }
            }
            if (actualTrigger == "RESTARTAPP")
            {
                // Function for Remotely Restarting the Application
                System.Windows.Forms.Application.Restart();
                Environment.Exit(0);
            }
            if (actualTrigger == "UPDATEMULTISCREEN")
            {
                // Function for Remotely getting the Multiscreen Config
                if (!lowPowerTrigger)
                {
                    FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [CMD] Update Multiscreen");
                }
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

                // Call Home on init, however the rest will be deligated over to a Timer
                using (var client = new WebClient())
                {
                    // Create the $_POST Data for the HTTP Request
                    var values = new NameValueCollection
                    {
                        ["deviceID"] = MyIni.Read("deviceName", "Monitor"),
                        ["deviceUUID"] = MyIni.Read("deviceUUID", "Monitor")
                    };

                    var responseString = "";
                    try
                    {
                        var response = client.UploadValues(networkURL + "/v2/checkForMultiscreen.php", values);
                        responseString = RemoveWhitespace(Encoding.UTF8.GetString(response));
                    }
                    catch
                    {
                        responseString = "Error";
                        FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [ERR] Cannot Call API Server [" + networkNameShort + "] - Check Internet/Firewall");
                    }

                    if (responseString != "Error")
                    {
                        string[] multiscreenArr = responseString.Split(',');
                        var forceOrientation = MyIni.Read("Force", "Display");

                        var Screen1 = multiscreenArr[0];
                        var Screen2 = multiscreenArr[1];
                        var Screen3 = multiscreenArr[2];
                        var Screen4 = multiscreenArr[3];
                        var Screen5 = multiscreenArr[4];
                        var Screen6 = multiscreenArr[5];

                        MyIni.Write("Monitor1", Screen1, "Display");
                        MyIni.Write("Monitor2", Screen2, "Display");
                        MyIni.Write("Monitor3", Screen3, "Display");
                        MyIni.Write("Monitor4", Screen4, "Display");
                        MyIni.Write("Monitor5", Screen5, "Display");
                        MyIni.Write("Monitor6", Screen6, "Display");

                        if (forceOrientation == "On")
                        {
                            if (Screen1 != "Disabled")
                            {
                                if (Screen1 == "Landscape")
                                {
                                    try { ScreenRotation.SetOrientation(1, 0); } catch { }
                                }
                                if (Screen1 == "Landscape-Flip")
                                {
                                    try { ScreenRotation.SetOrientation(1, 90); } catch { }
                                }
                                if (Screen1 == "Portrait")
                                {
                                    try { ScreenRotation.SetOrientation(1, 45); } catch { }
                                }
                                if (Screen1 == "Portrait-Flip")
                                {
                                    try { ScreenRotation.SetOrientation(1, 135); } catch { }
                                }
                            }
                            if (Screen2 != "Disabled")
                            {
                                if (Screen2 == "Landscape")
                                {
                                    try { ScreenRotation.SetOrientation(2, 0); } catch { }
                                }
                                if (Screen2 == "Landscape-Flip")
                                {
                                    try { ScreenRotation.SetOrientation(2, 90); } catch { }
                                }
                                if (Screen2 == "Portrait")
                                {
                                    try { ScreenRotation.SetOrientation(2, 45); } catch { }
                                }
                                if (Screen2 == "Portrait-Flip")
                                {
                                    try { ScreenRotation.SetOrientation(2, 135); } catch { }
                                }
                            }
                            if (Screen3 != "Disabled")
                            {
                                if (Screen3 == "Landscape")
                                {
                                    try { ScreenRotation.SetOrientation(3, 0); } catch { }
                                }
                                if (Screen3 == "Landscape-Flip")
                                {
                                    try { ScreenRotation.SetOrientation(3, 90); } catch { }
                                }
                                if (Screen3 == "Portrait")
                                {
                                    try { ScreenRotation.SetOrientation(3, 45); } catch { }
                                }
                                if (Screen3 == "Portrait-Flip")
                                {
                                    try { ScreenRotation.SetOrientation(3, 135); } catch { }
                                }
                            }
                            if (Screen4 != "Disabled")
                            {
                                if (Screen4 == "Landscape")
                                {
                                    try { ScreenRotation.SetOrientation(4, 0); } catch { }
                                }
                                if (Screen4 == "Landscape-Flip")
                                {
                                    try { ScreenRotation.SetOrientation(4, 90); } catch { }
                                }
                                if (Screen4 == "Portrait")
                                {
                                    try { ScreenRotation.SetOrientation(4, 45); } catch { }
                                }
                                if (Screen4 == "Portrait-Flip")
                                {
                                    try { ScreenRotation.SetOrientation(4, 135); } catch { }
                                }
                            }
                            if (Screen5 != "Disabled")
                            {
                                if (Screen5 == "Landscape")
                                {
                                    try { ScreenRotation.SetOrientation(5, 0); } catch { }
                                }
                                if (Screen5 == "Landscape-Flip")
                                {
                                    try { ScreenRotation.SetOrientation(5, 90); } catch { }
                                }
                                if (Screen5 == "Portrait")
                                {
                                    try { ScreenRotation.SetOrientation(5, 45); } catch { }
                                }
                                if (Screen5 == "Portrait-Flip")
                                {
                                    try { ScreenRotation.SetOrientation(5, 135); } catch { }
                                }
                            }
                            if (Screen6 != "Disabled")
                            {
                                if (Screen6 == "Landscape")
                                {
                                    try { ScreenRotation.SetOrientation(6, 0); } catch { }
                                }
                                if (Screen6 == "Landscape-Flip")
                                {
                                    try { ScreenRotation.SetOrientation(6, 90); } catch { }
                                }
                                if (Screen6 == "Portrait")
                                {
                                    try { ScreenRotation.SetOrientation(6, 45); } catch { }
                                }
                                if (Screen6 == "Portrait-Flip")
                                {
                                    try { ScreenRotation.SetOrientation(6, 135); } catch { }
                                }
                            }
                        }
                    }
                }
            }
            if (actualTrigger == "BLANK")
            {
                // This function always sits at the bottom, as a template for triggers
            }

            if (actualTrigger != "NONE" && actualTrigger != "BLANK")
            {
                SendCompleteNotification();             // Send System a Complete Notification of the Trigger being completed
            }
            return "Triggered";
        }

        public static bool ProgramIsRunning(string FullPath)
        {
            string FilePath = Path.GetDirectoryName(FullPath);
            string FileName = Path.GetFileNameWithoutExtension(FullPath).ToLower();
            bool isRunning = false;

            Process[] pList = Process.GetProcessesByName(FileName);

            foreach (Process p in pList)
            {
                try {
                    var theVariable = p.MainModule.FileName.StartsWith(FilePath, StringComparison.InvariantCultureIgnoreCase);
                    if (theVariable)
                    {
                        isRunning = true;
                        break;
                    }
                }
                catch { }
            }

            return isRunning;
        }

        // Class for converting DateTime to Unixtimestamp Format
        public class UnixTime
        {
            public static long Convert(DateTime theDate)
            {
                long unixTimestamp = ((DateTimeOffset)theDate).ToUnixTimeSeconds();
                return unixTimestamp;
            }
        }

        // Other Parts of GCMS Class
        public class SysInternal
        {
            public static void Update()
            {
                // Version 1 Main EXE Files
                string checkRunningEXE = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\checkRunning.exe";
                string globalMonitorEXE = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\globalMonitor.exe";
                string globalToolsEXE = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\globalTools.exe";
                string globalUEXE = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\globalU.exe";
                string signageMonitorEXE = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signageMonitor.exe";

                // Remove checkRunning.exe
                if (File.Exists(checkRunningEXE))
                {
                    try
                    {
                        File.Delete(checkRunningEXE);
                    } catch { }
                }
                // Remove globalMonitor.exe
                if (File.Exists(globalMonitorEXE))
                {
                    try
                    {
                        File.Delete(globalMonitorEXE);
                    }
                    catch { }
                }
                // Remove globalTools.exe
                if (File.Exists(globalToolsEXE))
                {
                    try
                    {
                        File.Delete(globalToolsEXE);
                    }
                    catch { }
                }
                // Remove globalU.exe
                if (File.Exists(globalUEXE))
                {
                    try
                    {
                        File.Delete(globalUEXE);
                    }
                    catch { }
                }
                // Remove signageMonitorEXE.exe
                if (File.Exists(signageMonitorEXE))
                {
                    try
                    {
                        File.Delete(signageMonitorEXE);
                    }
                    catch { }
                }

                // Version 1 Log Files
                string controlCodeLog = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\controlcode.log";
                string debugCodeLog = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\debug.log";
                string deviceCodeLog = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\device.log";
                string inboundLog = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\inbound.log";
                string outboundLog = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\outbound.log";
                string setupLog = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\setup.log";
                string sysinfoLog = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\sysinfo.log";
                string version1Log = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\version.log";
                string version2Log = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\version2.log";
                string version3Log = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\version3.log";
                string vpnLog = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\vpnUUID.log";
                string wmicLog = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\wmic.log";

                // Fixes Files
                string firstrunFix = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\firstrun.lock";
                string secondrunFix = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\secondrun.lock";

                // Fixes For Removal of old GeckoFX Files
                string geckoFX_CoreDLL = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Geckofx-Core.dll";
                string geckoFX_WinFormDLL = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Geckofx-Winforms.dll";
                string geckoFX_FF_Folder = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\Firefox";

                // Remove controlcode.log
                if (File.Exists(controlCodeLog))
                {
                    try
                    {
                        File.Delete(controlCodeLog);
                    }
                    catch { }
                }
                // Remove debug.log
                if (File.Exists(debugCodeLog))
                {
                    try
                    {
                        File.Delete(debugCodeLog);
                    }
                    catch { }
                }
                // Remove device.log
                if (File.Exists(deviceCodeLog))
                {
                    try
                    {
                        File.Delete(deviceCodeLog);
                    }
                    catch { }
                }
                // Remove inbound.log
                if (File.Exists(inboundLog))
                {
                    try
                    {
                        File.Delete(inboundLog);
                    }
                    catch { }
                }
                // Remove outbound.log
                if (File.Exists(outboundLog))
                {
                    try
                    {
                        File.Delete(outboundLog);
                    }
                    catch { }
                }
                // Remove setup.log
                if (File.Exists(setupLog))
                {
                    try
                    {
                        File.Delete(setupLog);
                    }
                    catch { }
                }
                // Remove sysinfo.log
                if (File.Exists(sysinfoLog))
                {
                    try
                    {
                        File.Delete(sysinfoLog);
                    }
                    catch { }
                }
                // Remove version.log
                if (File.Exists(version1Log))
                {
                    try
                    {
                        File.Delete(version1Log);
                    }
                    catch { }
                }
                // Remove version2.log
                if (File.Exists(version2Log))
                {
                    try
                    {
                        File.Delete(version2Log);
                    }
                    catch { }
                }
                // Remove version3.log
                if (File.Exists(version3Log))
                {
                    try
                    {
                        File.Delete(version3Log);
                    }
                    catch { }
                }
                // Remove vpnUUID.log
                if (File.Exists(vpnLog))
                {
                    try
                    {
                        File.Delete(vpnLog);
                    }
                    catch { }
                }
                // Remove wmic.log
                if (File.Exists(wmicLog))
                {
                    try
                    {
                        File.Delete(wmicLog);
                    }
                    catch { }
                }

                // Remove wmic.log
                if (File.Exists(firstrunFix))
                {
                    try
                    {
                        File.Delete(firstrunFix);
                    }
                    catch { }
                }

                // Version 1 JS Folder can be totally removed
                string jsFolder = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\js";
                if (Directory.Exists(jsFolder))
                {
                    Directory.Delete(jsFolder, true);              // Recursive as we want to delete everything in the folder
                }

                // Version 1 bin Folder can be totally removed
                string binFolder = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\bin";
                if (Directory.Exists(binFolder))
                {
                    Directory.Delete(binFolder, true);              // Recursive as we want to delete everything in the folder
                }

                // Version 2 requires a folder to contain control code downloads
                string ctrlcodesFolder = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\ctrlCodes";
                if (!Directory.Exists(ctrlcodesFolder))
                {
                    Directory.CreateDirectory(ctrlcodesFolder);
                }

                // Remove the Old Gecko FX DLL Files
                if (File.Exists(geckoFX_CoreDLL))
                {
                    try
                    {
                        File.Delete(geckoFX_CoreDLL);
                    }
                    catch { }
                }
                if (File.Exists(geckoFX_WinFormDLL))
                {
                    try
                    {
                        File.Delete(geckoFX_WinFormDLL);
                    }
                    catch { }
                }

                // Remove the Old Gecko FX Firefox Folder
                if (Directory.Exists(geckoFX_FF_Folder))
                {
                    Directory.Delete(geckoFX_FF_Folder, true);
                }
            }
        }
        public class FileLogger
        {
            public static string Log(string message)
            {
                string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\system.log";
                if (!File.Exists(filePath))
                {
                    try
                    {
                        File.Create(filePath).Dispose();
                    }
                    catch { }
                }

                FileInfo file = new FileInfo(filePath);
                if (file.Length > 1048576)
                {
                    try
                    {
                        File.Delete(filePath);
                    }
                    catch { }
                    try
                    {
                        File.Create(filePath).Dispose();
                    }
                    catch { }
                }

                using (StreamWriter streamWriter = File.AppendText(filePath))
                {
                    streamWriter.WriteLine(message);
                    streamWriter.Close();
                }

                try
                {
                    if (GCMSSystem.CheckOpened("MainForm")) {
                        MainForm.FrmObj.LastLogMsgOpt.Text = message;
                    }
                }
                catch { }

                return "Done";
            }
        }
        public class DataLogger
        {
            public static string Log(string message)
            {
                string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\logs\\data.log";
                if (!File.Exists(filePath))
                {
                    try
                    {
                        File.Create(filePath).Dispose();
                    } catch { }
                }

                FileInfo file = new FileInfo(filePath);
                if (file.Length > 1048576)
                {
                    try
                    {
                        File.Delete(filePath);
                    }
                    catch { }
                    try
                    {
                        File.Create(filePath).Dispose();
                    }
                    catch { }
                }

                using (StreamWriter streamWriter = File.AppendText(filePath))
                {
                    streamWriter.WriteLine(message);
                    streamWriter.Close();
                }
                return "Done";
            }
        }
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

                if (timeZoneId != "Error") {
                    //timeZoneId = Regex.Escape(Regex.Replace(timeZoneId, @"\t|\n|\r", ""));
                    //
                    timeZoneId = timeZoneId.Trim();
                    var process = Process.Start(new ProcessStartInfo
                    {
                        FileName = Path.Combine(Environment.SystemDirectory, "tzutil.exe"),
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
                    // Read INI File for Config.ini
                    string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                    IniFile MyIni = new IniFile(iniFile);
                    MyIni.Write("timezone", "", "Network");
                    MyIni.Write("timezone", timeZoneId, "Network");
                }
                return "Done";
            }
        }
        public class Python
        {
            public static string Run(string python_file)
            {
                var envpath = System.Environment.GetEnvironmentVariable("PATH");
                var paths = envpath.Split(';');
                var pythonPath = paths.Select(x => Path.Combine(x, "python.exe")).Where(x => File.Exists(x)).FirstOrDefault();
                if (string.IsNullOrWhiteSpace(pythonPath))
                {
                    // GCMSSystem.FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [ERR] Python Not Found");
                    pythonPath = "C:\\Python27\\python.exe";            // Python NOT found as an ENV so failover to hard coded location of Python
                }

                ProcessStartInfo python = new ProcessStartInfo
                {
                    //FileName = "C:\\Python27\\python.exe",
                    FileName = pythonPath,
                    Arguments = string.Format("{0}", python_file),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                // GCMSSystem.FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [OK] Python Running");

                try
                {
                    using (Process process = Process.Start(python))
                    {
                        // GCMSSystem.FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [OK] Python Code Run Successfully.");
                        return "Run Successfully";
                    }
                }
                catch (Exception ex)
                {
                    FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [ERR] Failed to run Python. StackTrace to follow:");
                    FileLogger.Log(ex.StackTrace.ToString());
                    GCMSSystem.FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [ERR] Failed to run Python. StackTrace to follow:");
                    GCMSSystem.FileLogger.Log(ex.StackTrace.ToString());
                    return "FAILED TO RUN PYTHON....";
                }

            }
        }
        public class Pritunl
        {
            public static string Load()
            {
                var pritunlEXE = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\Pritunl\\pritunl.exe";
                Process pritunl = new Process();
                pritunl.StartInfo.FileName = pritunlEXE;
                pritunl.StartInfo.Verb = "runas";
                pritunl.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                pritunl.Start();

                return "Loaded";
            }
            public static string Unload()
            {
                foreach (var process in Process.GetProcessesByName("pritunl"))
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

                return "Unloaded";
            }
            public static string Reload()
            {
                foreach (var process in Process.GetProcessesByName("pritunl"))
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

                var pritunlEXE = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\Pritunl\\pritunl.exe";
                Process pritunl = new Process();
                pritunl.StartInfo.FileName = pritunlEXE;
                pritunl.StartInfo.Verb = "runas";
                pritunl.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                pritunl.Start();

                return "Reloaded";
            }
        }
        public class VPNClient {
            public static void Load(string vpnConfig)
            {
                var osArch = GCMSSystem.GetOSArch();
                var openVPNArch = "32";
                if (osArch == "x64")
                {
                    openVPNArch = "64";
                }

                bool isVPNRunning = false;
                foreach (var process in Process.GetProcessesByName("openvpn"))
                {
                    isVPNRunning = true;
                }

                if (!isVPNRunning)
                {
                    var vpnEXE = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\Pritunl\\openvpn\\" + openVPNArch + "\\openvpn.exe";
                    Process vpn = new Process();
                    vpn.StartInfo.FileName = vpnEXE;
                    vpn.StartInfo.Arguments = "--config " + vpnConfig + " --nobind --persist-tun --verb 1";
                    vpn.StartInfo.Verb = "runas";
                    vpn.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    vpn.Start();
                }
            }
            public static void Unload()
            {
                foreach (var process in Process.GetProcessesByName("openvpn"))
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
            }
            public static void Reload(string vpnConfig)
            {
                Unload();
                Load(vpnConfig);
            }
        }
        public class Chrome
        {
            public static int whichVer = 2;
            public static bool signageEnabled = IsSignageEnabled();

            public static string Load()
            {
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                var EngineerMode = MyIni.Read("Network", "maintMode");
                var LowPowerMode1 = MyIni.Read("Network", "powersaveMode");
                var LowPowerMode2 = MyIni.Read("Network", "powersaveMode2");

                if (EngineerMode == "TRUE" || LowPowerMode1 == "TRUE" || LowPowerMode2 == "TRUE") { return "Loaded"; }
                if (SignageBrowser.isSignageLoaded) { return "Loaded"; }
                whichVer = MainForm.signageLoader;
                // Debug.WriteLine("Signage Loader : " + whichVer.ToString());

                if (!MainForm.isDebug && whichVer == 1)                // GOOGLE CHROME
                {
                    UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear
                    if (signageEnabled)
                    {
                        var chromeEXE = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\Google\\Chrome\\Application\\chrome.exe";
                        string extraSignageParams = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\config\\signageParams.ini");

                        var sslMode = MyIni.Read("SSL", "Browser").ToLower();
                        Process chromeKiosk = new Process();
                        chromeKiosk.StartInfo.FileName = chromeEXE;
                        chromeKiosk.StartInfo.Arguments = "--kiosk --disable-web-security --disable-pinch --overscroll-history-navigation=0 --disable-plugins --disable-session-crashed-bubble --disable-infobars --disable-session-restore --disable-usb-keyboard-detect --autoplay-policy=no-user-gesture-required " + extraSignageParams + " file://" + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace("#", "%23").Replace("\\", "/") + "/signage/preloader.html?ssl=" + sslMode;
                        chromeKiosk.StartInfo.Verb = "runas";
                        chromeKiosk.Start();
                    }
                }
                if (whichVer == 2)                // GLOBALCMS CORE
                {
                    if (signageEnabled)
                    {
                        if (System.Windows.Forms.Application.OpenForms["SignageBrowser"] == null)
                        {
                            Form Browser = new SignageBrowser();
                            Browser.Show();
                        }
                    }
                }
                if (whichVer == 4)                // GOOGLE CHROME CANARY
                {
                    UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear
                    if (signageEnabled)
                    {
                        string user = GetCurrentMachineUser();
                        string path = Path.Combine("C:\\", "Users", user, "AppData", "Local", "Google", "Chrome SxS", "Application");
                        var chromeEXE = path + "\\chrome.exe";
                        string extraSignageParams = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\config\\signageParams.ini");

                        // Debug.WriteLine("Loading : " + chromeEXE);

                        Process chromeKiosk = new Process();
                        chromeKiosk.StartInfo.FileName = chromeEXE;
                        chromeKiosk.StartInfo.Arguments = "--disable-web-security --user-data-dir=\"D:\\Cache\" --disable-features=CrossSiteDocumentBlockingAlways,CrossSiteDocumentBlockingIfIsolating " + extraSignageParams + " file://" + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location).Replace("#", "%23").Replace("\\", "/") + "/signage/preloader.html";
                        chromeKiosk.StartInfo.Verb = "runas";
                        chromeKiosk.Start();
                    }
                }
                if (!signageEnabled)
                {
                    try
                    {
                        Taskbar.Show();
                    }
                    catch { }
                }
                return "Loaded";
            }
            public static string Unload()
            {
                whichVer = MainForm.signageLoader;
                if (!MainForm.isDebug && whichVer == 1)
                {
                    foreach (var process in Process.GetProcessesByName("chrome"))
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
                    UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear
                }
                if (whichVer == 2)
                {
                    if (GCMSSystem.CheckOpened("SignageBrowser"))
                    {
                        SignageBrowser.CloseForm();
                    }
                }
                if (!signageEnabled)
                {
                    try
                    {
                        Taskbar.Show();
                    }
                    catch { }
                }
                return "Unloaded";
            }
            public static string Reload()
            {
                whichVer = MainForm.signageLoader;
                if (!MainForm.isDebug && whichVer == 1)
                {
                    Unload();
                    UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear
                    Load();
                }
                if (whichVer == 2)
                {
                    Unload();
                    Load();
                }
                return "Reloaded";
            }

            public static bool IsSignageEnabled()
            {
                bool signageEnabled = true;

                // Read INI File for Config.ini
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");

                IniFile MyIni = new IniFile(iniFile);

                // Setup which Network we should run over
                //var signEnabled = MyIni.Read("Signage", "Serv");
                string signEnabled = "";
                signEnabled = MyIni.Read("Signage", "Serv");


                if (signEnabled == "Enabled")
                {
                    signageEnabled = true;
                }
                else { 
                    signageEnabled = false;
                }
                return signageEnabled;
            }

            public static string Serialize(object data)
            {
                var serializer = new DataContractJsonSerializer(data.GetType());
                var ms = new MemoryStream();
                serializer.WriteObject(ms, data);

                return Encoding.UTF8.GetString(ms.ToArray());
            }

            public static string GetCurrentMachineUser()
            {
                string userName = "";
                Process process = Process.GetProcessesByName("explorer").FirstOrDefault();
                Process[] procs = Process.GetProcesses();
                foreach (Process proc in procs)
                {
                    if (proc.ProcessName != "Idle" && proc.Id == process.Id)
                    {
                        if (NativeMethods.WTSQuerySessionInformationW(WTS_CURRENT_SERVER_HANDLE,
                                                        proc.SessionId,
                                                        WTS_UserName,
                                                        out IntPtr AnswerBytes,
                                                        out IntPtr AnswerCount))
                        {
                            userName = Marshal.PtrToStringUni(AnswerBytes);
                            return userName;
                        }
                    }
                }
                return userName;
            }

            public static bool UpdatePref()
            {
                whichVer = MainForm.signageLoader;
                if (!MainForm.isDebug && whichVer == 1)
                {
                    // As the program is running from system, we need to get the result of the currently logged in user
                    // Location: C:\Users\<os logged in user>\AppData\Local\Google\Chrome\User Data\Default\Preferences
                    string user = GetCurrentMachineUser();

                    // Load the Perf file into a response string.
                    string path = Path.Combine("C:\\", "Users", user, "AppData", "Local", "Google", "Chrome", "User Data", "Default", "Preferences");

                    // Read Pref File
                    string retVal = System.IO.File.ReadAllText(path);

                    // Decode object to an array
                    dynamic retData = Newtonsoft.Json.JsonConvert.DeserializeObject(retVal);

                    // Change the Variables
                    string t = @"{""http://127.0.0.1:444,http://127.0.0.1:444"":{""last_modified"":""13179066539895471"",""setting"":1}}";
                    JObject test = JObject.Parse(t);
                    retData.profile.content_settings.exceptions.geolocation = test;
                    retData.profile.exit_type = "Normal";
                    retData.profile.exited_cleanly = true;
                    string output = Newtonsoft.Json.JsonConvert.SerializeObject(retData, Newtonsoft.Json.Formatting.None);

                    try
                    {
                        File.WriteAllText(path, output);
                    }
                    catch (Exception)
                    {
                        FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [PREF FILE] Unable to write to Chrome Preferences");
                        FileInfo fileInfo = new FileInfo(path)
                        {
                            IsReadOnly = false
                        };
                        return false;
                    }
                }
                return true;
            }

            public static void ClearCookies(bool showForm)
            {
                whichVer = MainForm.signageLoader;
                if (!MainForm.isDebug && whichVer == 1)
                {
                    // If this is being run on end of the Interactive, then trigger the Form to show on top
                    var CookieCleanerForm = new GDPR();
                    if (showForm)
                    {
                        CookieCleanerForm.Show();
                    }

                    // Start the Service Timers
                    MainForm.FrmObj.CheckSNAP.Stop();
                    MainForm.FrmObj.CheckSNAP.Interval = 60000 + (MainForm.snapDelay * 1000);
                    MainForm.FrmObj.CheckServicesTimer.Stop();
                    MainForm.FrmObj.CheckForNewSignage.Stop();

                    bool isSignageEnabled = IsSignageEnabled();
                    if (isSignageEnabled)
                    {
                        Unload();
                        UpdatePref();                            
                    }

                    // As the program is running from system, we need to get the result of the currently logged in user
                    // Location: C:\Users\<os logged in user>\AppData\Local\Google\Chrome\User Data\Default\Preferences
                    string user = GetCurrentMachineUser();

                    // Load the Perf file into a response string.
                    string path = Path.Combine("C:\\", "Users", user, "AppData", "Local", "Google", "Chrome", "User Data", "Default", "Local Storage", "leveldb");
                    DirectoryInfo di = new DirectoryInfo(path);
                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                    foreach (DirectoryInfo dir in di.GetDirectories())
                    {
                        dir.Delete(true);
                    }

                    string path2 = Path.Combine("C:\\", "Users", user, "AppData", "Local", "Google", "Chrome", "User Data", "Default", "Session Storage");
                    DirectoryInfo di2 = new DirectoryInfo(path2);
                    foreach (FileInfo file2 in di2.GetFiles())
                    {
                        file2.Delete();
                    }
                    foreach (DirectoryInfo dir2 in di2.GetDirectories())
                    {
                        dir2.Delete(true);
                    }

                    string path3 = Path.Combine("C:\\", "Users", user, "AppData", "Local", "Google", "Chrome", "User Data", "Default");
                    DirectoryInfo di3 = new DirectoryInfo(path3);
                    foreach (FileInfo file3 in di3.GetFiles())
                    {
                        if (file3.Name != "Preferences")
                        {
                            file3.Delete();
                        }
                    }

                    if (showForm && isSignageEnabled)
                    {
                        Load();
                    }
 
                    // Restart the Service Timers
                    MainForm.FrmObj.CheckSNAP.Start();
                    MainForm.FrmObj.CheckServicesTimer.Start();
                    MainForm.FrmObj.CheckForNewSignage.Start();
                }

                if (whichVer == 2)
                {
                    bool isSignageEnabled = IsSignageEnabled();

                    // Start the Service Timers
                    MainForm.FrmObj.CheckSNAP.Stop();
                    MainForm.FrmObj.CheckSNAP.Interval = 60000 + (MainForm.snapDelay * 1000);
                    MainForm.FrmObj.CheckServicesTimer.Stop();
                    MainForm.FrmObj.CheckForNewSignage.Stop();

                    if (isSignageEnabled)
                    {
                        Unload();

                        // If this is being run on end of the Interactive, then trigger the Form to show on top
                        var CookieCleanerForm = new GDPR();
                        if (showForm)
                        {
                            CookieCleanerForm.Show();
                        }

                        SignageBrowser.FrmObj.Close();
                        SignageBrowser.ClearCookies();
                    }

                    // Restart the Service Timers
                    MainForm.FrmObj.CheckSNAP.Start();
                    MainForm.FrmObj.CheckServicesTimer.Start();
                    MainForm.FrmObj.CheckForNewSignage.Start();
                }
            }
            public static void UpdateCallHome()
            {
                using (var client = new WebClient())
                {
                    string networkURL;                                                // Which Network URL to use

                    // Read INI File for Config.ini
                    string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                    string signageConfFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "signage", "settings.conf");

                    var MyIni = new IniFile(iniFile);
                    var SignageIni = new IniFile(signageConfFile);

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
                        var pingTest = GCMSSystem.Ping(networkURL);
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
                        ["hardwareMAC"] = MyIni.Read("hardwareMAC", "Monitor")
                    };

                    var responseString = "";
                    try
                    {
                        var response = client.UploadValues(networkURL + "/v2/getJSONTimer.php", values);
                        responseString = Encoding.Default.GetString(response);
                        responseString = GCMSSystem.RemoveWhitespace(responseString);
                        MyIni.Write("CH", responseString, "Sign");              // Set into the Config.ini for backup reasons and for checksums

                        var timerData = responseString.Split('_');
                        var timer1 = timerData[0];
                        var timer2 = timerData[1];

                        SignageIni.Write("timer1", timer1, "core");         // Split Timer into lower and upper parts (timer1 - lower)
                        SignageIni.Write("timer2", timer2, "core");         // Split Timer into lower and upper parts (timer2 - upper)
                    }
                    catch
                    {
                        
                    }
                }
            }
        }
        public class Network
        {
            public static string Restart(string whichNetwork)
            {
                var curOS = GetOS();
                var nicCard = "";

                Process processStop = new Process();
                processStop.StartInfo.FileName = "netsh";
                if (whichNetwork == "LAN")
                {
                    if (curOS == "Windows10" || curOS == "Windows8")
                    {
                        nicCard = "Ethernet";
                    }
                    if (curOS != "Windows8" && curOS != "Windows10")
                    {
                        nicCard = "Local Area Connection";
                    }
                    processStop.StartInfo.Arguments = "interface set interface " + nicCard + " DISABLED";
                }
                else
                {
                    if (curOS == "Windows10")
                    {
                        nicCard = "Wi-Fi";
                    }
                    if (curOS == "Windows8")
                    {
                        nicCard = "WiFi";
                    }
                    if (curOS != "Windows8" && curOS != "Windows10")
                    {
                        nicCard = "Wireless Network Connection";
                    }
                    processStop.StartInfo.Arguments = "interface set interface " + nicCard + " DISABLED";
                }
                processStop.StartInfo.Verb = "runas";
                processStop.Start();
                processStop.WaitForExit(60000);

                Process processStart = new Process();
                processStart.StartInfo.FileName = "netsh";
                if (whichNetwork == "LAN")
                {
                    if (curOS == "Windows10" || curOS == "Windows8")
                    {
                        nicCard = "Ethernet";
                    }
                    if (curOS != "Windows8" && curOS != "Windows10")
                    {
                        nicCard = "Local Area Connection";
                    }
                    processStart.StartInfo.Arguments = "interface set interface " + nicCard + " ENABLED";
                }
                else
                {
                    if (curOS == "Windows10")
                    {
                        nicCard = "Wi-Fi";
                    }
                    if (curOS == "Windows8")
                    {
                        nicCard = "WiFi";
                    }
                    if (curOS != "Windows8" && curOS != "Windows10")
                    {
                        nicCard = "Wireless Network Connection";
                    }
                    processStart.StartInfo.Arguments = "interface set interface " + nicCard + " ENABLED";
                }
                processStart.StartInfo.Verb = "runas";
                processStart.Start();
                processStart.WaitForExit(60000);

                return "Completed";
            }
        }
        public class RemoteLog
        {
            public static void Send(string dateTime, string whichCommand)
            {
                using (var client = new WebClient())
                {
                    string networkURL;
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
                        var pingTest = GCMSSystem.Ping(networkURL);
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
                        ["deviceUUID"] = MyIni.Read("deviceUUID", "Monitor"),
                        ["command"] = whichCommand
                    };

                    var responseString = "";
                    try
                    {
                        var response = client.UploadValues(networkURL + "/v2/submitLog.php", values);
                        responseString = Encoding.Default.GetString(response);
                    }
                    catch
                    {

                    }
                }
            }
        }
        public class SystemCleaner
        {
            public static void Run()
            {
                Task worker = Task.Run(() =>
                {
                    // cleanmgr.exe / d C / VERYLOWDISK
                    Process cleanmgr = new Process 
                    {
                        EnableRaisingEvents = true
                    };
                    cleanmgr.StartInfo.FileName = "cleanmgr";
                    cleanmgr.StartInfo.Arguments = "/d C /VERYLOWDISK /AUTOCLEAN /SETUP /SAGERUN: 1";
                    cleanmgr.StartInfo.Verb = "runas";
                    cleanmgr.StartInfo.CreateNoWindow = true;
                    cleanmgr.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    cleanmgr.Start();
                    cleanmgr.WaitForExit(60000);
                    cleanmgr.Close();
                });
            }
        }
        public class EDID
        {
            public static string Get()
            {
                List<ScreenInformation> sil = EDIDUtil.GetEDID();
                List<string> edidDetails = new List<string>();            // Blank List to store records in

                foreach (var item in sil)
                {
                    edidDetails.Add("Raw EDID Data Block");
                    edidDetails.Add(item.FullEDID);
                    edidDetails.Add("");
                    edidDetails.Add("Manufacturer \t\t\t\t " + item.Manufacturer);
                    edidDetails.Add("Model Number \t\t\t\t " + item.Model);
                    edidDetails.Add("Part Number \t\t\t\t " + item.Part);
                    edidDetails.Add("Serial Number \t\t\t\t " + item.Serial);
                    edidDetails.Add("");
                    edidDetails.Add("Build Date \t\t\t\t " + item.Date);
                    edidDetails.Add("Build Year \t\t\t\t " + item.Year);
                    edidDetails.Add("");
                    edidDetails.Add("Screen Size \t\t\t\t " + item.Size + " inch");
                    edidDetails.Add("Digital (HDMI) \t\t\t\t " + item.Digital);
                    edidDetails.Add("Screen Resolution \t\t\t " + item.Res);
                    edidDetails.Add("");
                    edidDetails.Add("EDID Version \t\t\t\t " + item.Version);
                    edidDetails.Add("==========================================");
                    edidDetails.Add("");
                }
                var edidMessage = string.Join(Environment.NewLine, edidDetails);                  // Convert the list to a message that MessageBox will understand
                return edidMessage;
            }

        }
        public class WinService
        {
            public static string CheckInstalled(string serviceName)
            {
                ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == serviceName);
                var outString = "";
                if (ctl == null)
                {
                    outString = "Not_Installed";
                }
                else
                {
                    outString = "Installed";
                }
                return outString;
            }
            public static string CheckRunning(string serviceName)
            {
                ServiceController sc = new ServiceController(serviceName);
                switch (sc.Status)
                {
                    case ServiceControllerStatus.Running:
                        return "Running";
                    case ServiceControllerStatus.Stopped:
                        return "Stopped";
                    case ServiceControllerStatus.Paused:
                        return "Paused";
                    case ServiceControllerStatus.StopPending:
                        return "Stopping";
                    case ServiceControllerStatus.StartPending:
                        return "Starting";
                    default:
                        return "Status Changing";
                }
            }
            public static void Start(string serviceName)
            {
                try
                {
                    ServiceController sc = new ServiceController(serviceName);
                    sc.Start();
                }
                catch { }
            }
            public static void Stop(string serviceName)
            {
                try
                {
                    ServiceController sc = new ServiceController(serviceName);
                    sc.Stop();
                }
                catch { }
            }
            public static void Install(string[] serviceFile, string serviceName)
            {
                ManagedInstallerClass.InstallHelper(serviceFile);
                int exitCode;
                using (var process = new Process())
                {
                    var startInfo = process.StartInfo;
                    startInfo.FileName = "sc";
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                    // tell Windows that the service should restart if it fails
                    startInfo.Arguments = string.Format("failure \"{0}\" reset= 0 actions= restart/60000", serviceName);

                    process.Start();
                    process.WaitForExit();

                    exitCode = process.ExitCode;
                }
                using (var process = new Process())
                {
                    var startInfo = process.StartInfo;
                    startInfo.FileName = "sc";
                    startInfo.WindowStyle = ProcessWindowStyle.Hidden;

                    // tell Windows that the service start on Delayed Start
                    startInfo.Arguments = string.Format("config \"{0}\" start= delayed-auto", serviceName);

                    process.Start();
                    process.WaitForExit();

                    exitCode = process.ExitCode;
                }
            }
        }
        public class Reg
        {
            public static void AddReg(RegistryHive regHive, string regLocation, string regKey)
            {
                var osArch = GCMSSystem.GetOSArch();
                string[] keys = regLocation.Split('\\');
                keys[0] = "";
                keys = keys.Where((source, index) => index != 0).ToArray();

                string key = string.Join("\\", keys);

                using (RegistryKey hklm = RegistryKey.OpenBaseKey(regHive, osArch == "x86" ? RegistryView.Registry32 : RegistryView.Registry64))
                {
                    using (var subKey = hklm.OpenSubKey(key, RegistryKeyPermissionCheck.ReadWriteSubTree, System.Security.AccessControl.RegistryRights.FullControl))
                    {
                        try
                        {
                            subKey.SetValue(regKey, 96, RegistryValueKind.DWord);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Failed : " + ex);
                        }
                        subKey.Close();
                    }
                }
            }
            public static string CheckReg(RegistryHive regHive, string regLocation, string regKey)
            {
                string regValue = null;
                object theKey;
                try
                {
                    string[] keys = regLocation.Split('\\');
                    keys[0] = "";
                    keys = keys.Where((source, index) => index != 0).ToArray();

                    string key = string.Join("\\", keys);

                    var osArch = GCMSSystem.GetOSArch();
                    using (RegistryKey hklm = RegistryKey.OpenBaseKey(regHive, osArch == "x86" ? RegistryView.Registry32 : RegistryView.Registry64))
                    {
                        using (var subKey = hklm.OpenSubKey(key))
                        {
                            theKey = subKey.GetValue(regKey);
                            subKey.Close();
                        }
                    }
                    regValue = theKey.ToString();
                }
                catch { }

                return regValue;
            }
            public static string UpdateReg(RegistryHive regHive, string regLocation, string regKey, int regValue)
            {
                string regValue2 = null;
                try
                {
                    string[] keys = regLocation.Split('\\');
                    keys[0] = "";
                    keys = keys.Where((source, index) => index != 0).ToArray();

                    string key = string.Join("\\", keys);

                    var osArch = GCMSSystem.GetOSArch();
                    using (RegistryKey hklm = RegistryKey.OpenBaseKey(regHive, osArch == "x86" ? RegistryView.Registry32 : RegistryView.Registry64))
                    {
                        using (var subKey = hklm.OpenSubKey(key, RegistryKeyPermissionCheck.ReadWriteSubTree, System.Security.AccessControl.RegistryRights.FullControl))
                        {
                            try
                            {
                                subKey.SetValue(regKey, regValue, RegistryValueKind.DWord);
                                regValue2 = "Complete";
                            } catch (Exception ex)
                            {
                                regValue2 = "Failed : " + ex;
                            }
                            subKey.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    regValue2 = "Failed : " + ex;
                }
                return regValue2;
            }
        }
        public class NodeSocket
        {
            public static string Send(string socketStr)
            {
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");  // Load INI File
                string responseString;                  // Blank to store the record in

                HttpResponseMessage response = null;
                try
                {
                    var MyIni = new IniFile(iniFile);
                    Random rnd = new Random();

                    int rand = rnd.Next(1000, 9999);                                // Random Number for the ID
                    string playerID = MyIni.Read("deviceName", "Monitor");          // PlayerID from config.ini
                    var vpnIP = GCMSSystem.GetIP("VPN");                                 // Get VPN IP

                    // Generate the JSON String to send
                    string myJson = "{ \"id\": " +rand+ ", \"playerID\": \"" + playerID + "\", \"vpnIP\": \"" + vpnIP + "\" , \"command\": \"" + socketStr + "\" }";
                    // Debug.WriteLine(myJson);

                    using (var client = new System.Net.Http.HttpClient())
                    {
                        // Send JSON String to internal NODEJS Socket
                        response = client.PostAsync("http://127.0.0.1:442/",  new StringContent(myJson, Encoding.UTF8, "application/json")).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            responseString = "Websocket Sent";
                        }
                        else
                        {
                            responseString = "Error Sending Websocket";
                        }
                    }
                }
                catch (Exception)
                {
                    responseString = "Error Sending Websocket";
                }
                return responseString;
            }
            public static string Check(string whichConnection)
            {
                string jsonString;
                string theOut;
                bool status = false;

                using (WebClient client = new WebClient())
                {
                    if (SignageBrowser.isSignageLoaded)
                    {
                        jsonString = client.DownloadString("http://127.0.0.1:444/status");
                        // Debug.WriteLine(jsonString);
                        JToken json = JObject.Parse(jsonString);
                        if (whichConnection == "SEN")
                        {
                            status = (bool)json.SelectToken("arduinoConnected");
                        }
                        if (whichConnection == "AVA")
                        {
                            status = (bool)json.SelectToken("avaConnected");
                        }
                        if (whichConnection == "WSK")
                        {
                            status = (bool)json.SelectToken("websocketConnected");
                        }
                    }
                }
                if (status == false) { theOut = "Disconnected"; } else { theOut = "Connected"; }
                return theOut;
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
        }
        public class Display
        {
            public static bool Check(string connectionType)
            {
                bool theOutput = false;
                if (connectionType != "5")
                {
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM WmiMonitorConnectionParams");

                    foreach (ManagementObject queryObj in searcher.Get())
                    {
                        foreach (PropertyData pd in queryObj.Properties)
                        {
                            string name = pd.Name;
                            string val = pd.Value.ToString();
                            if (name == "VideoOutputTechnology" && connectionType == val)
                            {
                                switch (val)
                                {
                                    case "0":
                                        theOutput = true;
                                        break;
                                    case "5":
                                        theOutput = true;
                                        break;
                                    default:
                                        theOutput = false;
                                        break;
                                }
                            }
                        }
                    }
                }
                return theOutput;
            }
            public enum MonitorTech
            {
                VGA = 0,
                SVIDEO = 1,
                COMPOSITE_VIDEO = 2,
                COMPONENT_VIDEO = 3,
                DVI = 4,
                HDMI = 5,
                LVDS = 6,
                D_JPN = 8,
                SDI = 9,
                DISPLAYPORT_EXTERNAL = 10,
                DISPLAYPORT_EMBEDDED = 11,
                UDI_EXTERNAL = 12,
                UDI_EMBEDDED = 13,
                SDTVDONGLE = 14,
                MIRACAST = 15,
            }
        }
        public class Skin
        {
            private static readonly string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
            public static bool Check()
            {
                bool returnString = false;                                        // Return String to place the returned Skin Data to (default to false, to use default skin)
                if (File.Exists(iniFile))
                {
                    var MyIni = new IniFile(iniFile);                   // If Config.ini exists then load up the Skin Data from it
                    var MySkin = MyIni.Read("SkinID", "Skin");
                    if (MySkin == "")
                    {
                        Backfill(MyIni.Read("clientID", "Monitor"), MyIni.Read("deviceName", "Monitor"), MyIni.Read("deviceMAC", "Monitor"), MyIni.Read("deviceUUID", "Monitor"), MyIni.Read("hardwareMAC", "Monitor"));
                    } 
                    else
                    {
                        returnString = true;
                    }
                }
                return returnString;
            }
            public static string GetSkinID()
            {
                var returnString = "Default";   
                if (File.Exists(iniFile))
                {
                    var MyIni = new IniFile(iniFile);                   // If Config.ini exists then load up the Skin Data from it
                    var MySkin = MyIni.Read("SkinID", "Skin");
                    if (MySkin != "")
                    {
                        returnString = MySkin;
                    }
                }
                return returnString;
            }
            public static void Update()
            {
                var MyIni = new IniFile(iniFile);                   // If Config.ini exists then load up the Skin Data from it
                Backfill(MyIni.Read("clientID", "Monitor"), MyIni.Read("deviceName", "Monitor"), MyIni.Read("deviceMAC", "Monitor"), MyIni.Read("deviceUUID", "Monitor"), MyIni.Read("hardwareMAC", "Monitor"));
            }
            private static void Backfill(string clientID, string deviceID, string deviceMAC, string deviceUUID, string hardwareMAC)
            {
                // Check Remote Server to get the details for the Skinning
                var responseString = "";
                using (var client = new WebClient())
                {
                    // Create the $_POST Data for the HTTP Request
                    var values = new NameValueCollection
                    {
                        ["clientID"] = clientID,
                        ["deviceID"] = deviceID,
                        ["deviceMAC"] = deviceMAC,
                        ["deviceUUID"] = deviceUUID,
                        ["hardwareMAC"] = hardwareMAC
                    };

                    try
                    {
                        var response = client.UploadValues("http://api.globalcms.co.uk/v2/getSkinID.php", values);
                        responseString = Encoding.Default.GetString(response);
                        responseString = GCMSSystem.RemoveWhitespace(responseString);
                    }
                    catch
                    {
                        responseString = "Default";
                    }
                }

                var MyIni = new IniFile(iniFile);                   // If Config.ini exists then load up the Skin Data from it
                MyIni.Write("SkinID", responseString, "Skin");
            }
        }
        public class DriverSystem
        {
            public static bool CheckInstalled(string driverName)
            {
                bool isInstalled = false;
                System.Management.SelectQuery query = new System.Management.SelectQuery("Win32_SystemDriver")
                {
                    Condition = "Name = '" + driverName + "'"
                };
                System.Management.ManagementObjectSearcher searcher = new System.Management.ManagementObjectSearcher(query);
                var drivers = searcher.Get();

                if (drivers.Count > 0) { isInstalled = true; }
                return isInstalled;
            }
            public static void Install(string driverName)
            {
                MessageBox.Show("Driver Installed", "Engineering Tools", MessageBoxButtons.OK);
            }
            public static void Uninstall(string driverName)
            {
                MessageBox.Show("Driver Uninstalled", "Engineering Tools", MessageBoxButtons.OK);
            }
        }
        public class InteractiveLog
        {
            public static void Send(string whichInteractiveType)
            {
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);

                // Call Home on init, however the rest will be deligated over to a Timer
                using (var client = new WebClient())
                {
                    // Create the $_POST Data for the HTTP Request
                    var values = new NameValueCollection
                    {
                        ["deviceUUID"] = MyIni.Read("deviceUUID", "Monitor"),
                        ["whichOption"] = whichInteractiveType
                    };

                    var responseString = "";
                    try
                    {
                        var response = client.UploadValues("http://api.globalcms.co.uk/v2/inboundInteractiveLog.php", values);
                        responseString = Encoding.UTF8.GetString(response);
                        responseString = RemoveWhitespace(responseString);
                        // Debug.WriteLine(responseString);
                    }
                    catch
                    {
                        // Debug.WriteLine("Error Sending Interactive Trigger Log");
                    }
                }
            }
        }
        public class Updater
        {
            async public static void DownloadMonitor(bool downloadBeta)
            {
                MainForm.FrmObj.CheckSNAP.Enabled = false;
                MainForm.FrmObj.CheckSNAP.Interval = 60000;
                MainForm.snapCounter = 0;

                string stableVersion = "https://api.globalcms.co.uk/v2/monitorUpdate/GlobalCMS.zip";
                string betaVersion = "https://api.globalcms.co.uk/v2/monitorUpdate/beta/GlobalCMS.zip";

                string filePath = GlobalCMS.DownloaderManager.tmpFolder;
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                string fileName = "GlobalCMS.zip";
                string fullFileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\downloadtmp\\" + fileName;
                string fileURL = stableVersion;
                if (downloadBeta) { fileURL = betaVersion; }
                await GlobalCMS.DownloaderManager.FrmObj.DownloadFile(GlobalCMS.DownloaderManager.FrmObj.downloadOpt, filePath, fileName, fileURL);
                try
                {
                    ZipFile.ExtractToDirectory(fullFileName, filePath);
                }
                catch { }
                try
                {
                    File.Delete(fullFileName);
                }
                catch { }

                var updateTool = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\downloadtmp\\updatetool.bat";
                if (File.Exists(updateTool))
                {
                    try
                    {
                        Process updateProc = new Process();
                        updateProc.StartInfo.FileName = updateTool;
                        updateProc.StartInfo.Verb = "runas";
                        updateProc.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
                        updateProc.Start();
                    }
                    catch { }
                }

                MainForm.FrmObj.CheckSNAP.Interval = 60000;
                MainForm.snapCounter = 0;
                MainForm.FrmObj.CheckSNAP.Enabled = false;
            }
            async public static void DownloadSignage(bool downloadBeta)
            {
                MainForm.FrmObj.CheckSNAP.Enabled = false;
                MainForm.FrmObj.CheckSNAP.Interval = 60000;
                MainForm.snapCounter = 0;

                string networkURL;                                                // Which Network URL to use
                string stableVersion = "https://api.globalcms.co.uk/v2/signageUpdate/signage.zip";
                string betaVersion = "https://api.globalcms.co.uk/v2/signageUpdate/beta/signage.zip";

                string filePath = GlobalCMS.DownloaderManager.tmpFolder;
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                string fileName = "signage.zip";
                string fullFileName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\downloadtmp\\" + fileName;
                string fileURL = stableVersion;
                if (downloadBeta) { fileURL = betaVersion; }
                await GlobalCMS.DownloaderManager.FrmObj.DownloadFile(GlobalCMS.DownloaderManager.FrmObj.downloadOpt, filePath, fileName, fileURL);

                bool isSignageEnabled = Chrome.IsSignageEnabled();
                if (isSignageEnabled)
                {
                    var updateForm = new Updating();
                    updateForm.Show();
                }

                var chromeRunning = "NO"; var nodeRunning = "NO";

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
                    Chrome.Unload();
                    Chrome.UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear
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
                        catch { }
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
                        catch { }
                    }
                }

                // Move all content from signage\public\content to the temp folder
                var sourceDIR = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\public\\content";
                var destDIR = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage_tmp";

                // Shutdown Timers associated with Signage and the NodeJS
                MainForm.FrmObj.CheckServicesTimer.Stop();
                MainForm.FrmObj.CheckSNAP.Stop();
                MainForm.FrmObj.CheckSNAP.Interval = 60000 + (MainForm.snapDelay * 1000);

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

                // Unzip Signage.zip to the Signage Folder inside monitor main root DIR
                var signageZipFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\downloadtmp\\signage.zip";
                var signageZipFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                try
                {
                    ZipFile.ExtractToDirectory(signageZipFile, signageZipFolder);
                }
                catch { }

                // Move all content from temp folder back to the signage\public\content
                try
                {
                    Directory.Delete(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage\\public\\content", true);
                    Directory.Move(destDIR, sourceDIR);
                    File.Delete(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\signage.zip");
                }
                catch { }

                // Restart NodeJS and then Chrome
                var osArch = GCMSSystem.GetOSArch();
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

                Chrome.UpdateCallHome();            // On Updating and Extracting the new signage we need to make sure we lock back onto which Timers to use

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

                Chrome.UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear
                Chrome.Load();
                if (isSignageEnabled)
                {
                    Updating.frmObj.Close();
                }
                try
                {
                    Directory.Delete(@Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "downloadtmp"), true);
                }
                catch { }

                MainForm.FrmObj.CheckSNAP.Interval = 60000;
                MainForm.snapCounter = 0;
                MainForm.FrmObj.CheckSNAP.Enabled = true;
            }
        }
        public class OSK
        {
            public static bool isKeyboardOpen = false;
            public static void StartOSK()
            {
                string root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\keyboard";
                string subdir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\keyboard\\Layouts";

                string rootEXE = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\keyboard\\OpenKeyboard.exe";
                string layoutXML = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\keyboard\\Layouts\\Default.xml";

                if (!Directory.Exists(root))
                {
                    Directory.CreateDirectory(root);
                }
                if (!Directory.Exists(subdir))
                {
                    Directory.CreateDirectory(subdir);
                }

                if (!File.Exists(rootEXE))
                {
                    object ob = Properties.Resources.ResourceManager.GetObject("OpenKeyboard");
                    byte[] myResBytes = (byte[])ob;
                    using (FileStream fsDst = new FileStream(rootEXE, FileMode.CreateNew, FileAccess.Write))
                    {
                        byte[] bytes = myResBytes;
                        fsDst.Write(bytes, 0, bytes.Length);
                        fsDst.Close();
                        fsDst.Dispose();
                    }
                }
                if (!File.Exists(layoutXML))
                {
                    try
                    {
                        File.WriteAllText(layoutXML, Properties.Resources.Default);
                    }
                    catch { }
                        
                }

                // Once the OSK has been extracted from the RESOURCES now we need to run it so that at any time we can fire it up
                if (File.Exists(rootEXE) && File.Exists(layoutXML) && !isKeyboardOpen)
                {
                    Process kioskOSK = new Process();
                    kioskOSK.StartInfo.FileName = rootEXE;
                    kioskOSK.Start();
                    isKeyboardOpen = true;
                }
            }

            public static void StopOSK()
            {
                if (isKeyboardOpen)
                {
                    foreach (var process in Process.GetProcessesByName("OpenKeyboard"))
                    {
                        try
                        {
                            process.StartInfo.Verb = "runas";
                            process.Kill();
                            process.WaitForExit(60000);
                        }
                        catch { }
                    }
                    isKeyboardOpen = false;

                    SignageBrowser.FrmObj.BringToFront();
                    SignageBrowser.FrmObj.Focus();
                    SignageBrowser.FrmObj.Activate();

                }
                else
                {
                    isKeyboardOpen = false;
                }
            }
        }

        public class TeamViewer
        {
            public static bool IsInstaled()
            {
                bool isInstalled = false;
                try
                {
                    var path = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\TeamViewer\", "Version", null); 
                    if (path != null)
                    {
                        isInstalled = true;
                    }
                    else
                    {
                        var path2 = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\TeamViewer\", "Version", null);
                        if (path2 != null)
                        {
                            isInstalled = true;
                        }
                    }
                }
                catch { }
                return isInstalled;
            }
            public static string Version()
            {
                string whichVersion = "N/A";
                try
                {
                    whichVersion = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\TeamViewer\", "Version", null).ToString();
                }
                catch
                {
                    try
                    {
                        whichVersion = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\TeamViewer\", "Version", null).ToString();
                    }
                    catch { }
                }
                return whichVersion;
            }
            public static string ID()
            {
                string whichID = "N/A";
                try
                {
                    whichID = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\TeamViewer\", "ClientID", null).ToString();
                }
                catch
                {
                    whichID = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\TeamViewer\", "ClientID", null).ToString();
                }
                return whichID;
            }
        }

        public class HexadecimalEncoding
        {
            public static string ToHexString(string str)
            {
                var sb = new StringBuilder();

                var bytes = Encoding.Unicode.GetBytes(str);
                foreach (var t in bytes)
                {
                    sb.Append(t.ToString("X2"));
                }

                return sb.ToString(); // returns: "48656C6C6F20776F726C64" for "Hello world"
            }

            public static string FromHexString(string hexString)
            {
                var bytes = new byte[hexString.Length / 2];
                for (var i = 0; i < bytes.Length; i++)
                {
                    bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
                }

                return Encoding.Unicode.GetString(bytes); // returns: "Hello world" for "48656C6C6F20776F726C64"
            }
        }
        public class AirServer
        {
            static readonly string osArch = GCMSSystem.GetOSArch();
            static string theEXE = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\App Dynamic\\AirServer\\AirServerConsole.exe";

            public static bool IsInstaled()
            {
                if (osArch == "x64")
                {
                    theEXE = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\App Dynamic\\AirServer\\AirServerConsole.exe";
                }
                bool isInstalled = false;
                try
                {
                    if (File.Exists(theEXE))
                    {
                        isInstalled = true;
                    }
                }
                catch { }
                return isInstalled;
            }
            public static bool IsRunning()
            {
                if (osArch == "x64")
                {
                    theEXE = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\App Dynamic\\AirServer\\AirServerConsole.exe";
                }
                bool isAppRunning = false;
                try
                {
                    isAppRunning = GCMSSystem.ProgramIsRunning(theEXE);
                }
                catch { }

                if (isAppRunning) { isAppRunning = true; }
                return isAppRunning;
            }
            public static string Version()
            {
                if (osArch == "x64")
                {
                    theEXE = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\App Dynamic\\AirServer\\AirServerConsole.exe";
                }
                var airServerVersion = "Unknown";
                try
                {
                    airServerVersion = FileVersionInfo.GetVersionInfo(theEXE).FileVersion.ToString();
                }
                catch { }
                return airServerVersion;
            }
            public static string Passcode()
            {
                if (osArch == "x64")
                {
                    theEXE = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\App Dynamic\\AirServer\\AirServerConsole.exe";
                }
                string whichID = "N/A";
                try
                {
                    whichID = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\App Dynamic\AirServer\", "Password", null).ToString();
                }
                catch
                {
                    try
                    {
                        whichID = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\App Dynamic\AirServer\", "Password", null).ToString();
                    }
                    catch { }
                }
                return whichID;
            }
            public static string DeviceName()
            {
                if (osArch == "x64")
                {
                    theEXE = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\App Dynamic\\AirServer\\AirServerConsole.exe";
                }
                string whichID = "N/A";
                try
                {
                    whichID = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\App Dynamic\AirServer\", "ComputerName", null).ToString();
                }
                catch
                {
                    try
                    {
                        whichID = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\App Dynamic\AirServer\", "ComputerName", null).ToString();
                    }
                    catch { }
                }
                return whichID;
            }
            public static string GetCode(string networkURL, string networkNameShort)
            {
                if (osArch == "x64")
                {
                    theEXE = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\App Dynamic\\AirServer\\AirServerConsole.exe";
                }
                var theCode = "";
                // Load the INI File
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);

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
                        ["hardwareMAC"] = MyIni.Read("hardwareMAC", "Monitor")
                    };

                    var responseString = "";
                    try
                    {
                        var response = client.UploadValues(networkURL + "/v2/outboundAirServerSerial.php", values);
                        responseString = Encoding.UTF8.GetString(response);
                        responseString = GCMSSystem.RemoveWhitespace(responseString);
                    }
                    catch
                    {
                        responseString = "Error";
                        FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [ERR] Cannot Call API Server [" + networkNameShort + "] - Check Internet/Firewall");
                    }

                    if (responseString != "Error")
                    {
                        // Write all the changes to the config.ini
                        MyIni.Write("AS", responseString, "Licence");
                        theCode = responseString;
                    }
                }
                return theCode;
            }
            public static void RemoveCode()
            {
                if (osArch == "x64")
                {
                    theEXE = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\App Dynamic\\AirServer\\AirServerConsole.exe";
                }
                // Load the INI File
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);

                MyIni.Write("AS", "NotInstalled", "Licence");
            }
            public static void SetName(string givenDeviceName)
            {
                if (osArch == "x64")
                {
                    theEXE = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\App Dynamic\\AirServer\\AirServerConsole.exe";
                }
                try
                {
                    Process airserver = new Process();
                    airserver.StartInfo.FileName = theEXE;
                    airserver.StartInfo.Arguments = "set name " + givenDeviceName;
                    airserver.StartInfo.Verb = "runas";
                    airserver.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    airserver.Start();
                    airserver.WaitForExit(60000);
                }
                catch { }
            }
            public static void SetPassword(int givenPass)
            {
                if (osArch == "x64")
                {
                    theEXE = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\App Dynamic\\AirServer\\AirServerConsole.exe";
                }
                // var osArch = GCMSSystem.GetOSArch();
                Random rnd = new Random();
                int passCode = rnd.Next(10000, 99999);  // creates a number between 10000 and 99999
                if (givenPass >= 1000)
                {
                    passCode = givenPass;
                }
                // Debug.WriteLine("New Pass: " + passCode);
                try
                {
                    Process airserver = new Process();
                    airserver.StartInfo.FileName = theEXE;
                    airserver.StartInfo.Arguments = "authenticate using password " + passCode;
                    airserver.StartInfo.Verb = "runas";
                    airserver.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    airserver.Start();
                    airserver.WaitForExit(60000);
                }
                catch { }

                try
                {
                    EngineerTools.FrmObj.AirServerPasscode.Text = "Code: " + passCode.ToString();
                }
                catch { }
            }
            public static void Activate(string activateCode)
            {
                // TODO: Found new Bug where AirServer Auth takes more than 1 line to reply now
                // Add in a checksum feature to make 100% sure that the unit is activated

                if (osArch == "x64")
                {
                    theEXE = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\App Dynamic\\AirServer\\AirServerConsole.exe";
                }
                try
                {
                    Process airserver = new Process();
                    airserver.StartInfo.FileName = theEXE;
                    airserver.StartInfo.Arguments = "activate " + activateCode;
                    airserver.StartInfo.Verb = "runas";
                    airserver.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    airserver.Start();
                    airserver.WaitForExit(60000);
                }
                catch { }
            }
            public static void Deactivate()
            {
                if (osArch == "x64")
                {
                    theEXE = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\App Dynamic\\AirServer\\AirServerConsole.exe";
                }
                AirServer.Kill();            // Make 100% sure that AirServer is stopped
                try
                {
                    Process airserver = new Process();
                    airserver.StartInfo.FileName = theEXE;
                    airserver.StartInfo.Arguments = "deactivate";
                    airserver.StartInfo.Verb = "runas";
                    airserver.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    airserver.Start();
                    airserver.WaitForExit(60000);
                } catch { }
            }
            public static void Start()
            {
                if (osArch == "x64")
                {
                    theEXE = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\App Dynamic\\AirServer\\AirServerConsole.exe";
                }
                try
                {
                    Process airserver = new Process();
                    airserver.StartInfo.FileName = theEXE;
                    airserver.StartInfo.Verb = "runas";
                    airserver.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    airserver.Start();
                }
                catch { }
            }
            public static void Stop()
            {
                if (osArch == "x64")
                {
                    theEXE = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\App Dynamic\\AirServer\\AirServerConsole.exe";
                }
                Process airserver = new Process();
                try
                {
                    airserver.StartInfo.FileName = theEXE;
                    airserver.StartInfo.Verb = "runas";
                    airserver.StartInfo.Arguments = "shutdown";
                    airserver.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    airserver.Start();
                }
                catch { }
            }
            public static void Kill()
            {
                if (osArch == "x64")
                {
                    theEXE = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\App Dynamic\\AirServer\\AirServerConsole.exe";
                }
                foreach (var process in Process.GetProcessesByName("AirServer"))
                {
                    try
                    {
                        GCMSSystem.KillProcessAndChildrens(process.Id); // Get PID of AirServer, and kill 'it' as well as any other children it was spawned
                        process.StartInfo.Verb = "runas";
                        try { process.Kill(); } catch { }
                    }
                    catch { }
                }
            }
            public static void Restart()
            {
                if (osArch == "x64")
                {
                    theEXE = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\App Dynamic\\AirServer\\AirServerConsole.exe";
                }
                Process airserver = new Process();
                try
                {
                    airserver.StartInfo.FileName = theEXE;
                    airserver.StartInfo.Verb = "runas";
                    airserver.StartInfo.Arguments = "restart";
                    airserver.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    airserver.Start();
                }
                catch { }
            }
            public static void Install(string networkURL)
            {
                var osArch = "x86";
                DownloadFileSingle(networkURL + "/v2/monitorUpdate/reqFiles/airserver_" + osArch + ".msi", "airserver_" + osArch + ".msi");
                try
                {
                    // Use msiexec to install AirServer from the Downloaded msi file
                    Process installPID = new Process();
                    installPID.StartInfo.FileName = "msiexec";
                    installPID.StartInfo.Arguments = "/quiet /i \"" + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\airserver_" + osArch + ".msi";
                    installPID.StartInfo.Verb = "runas";
                    installPID.Start();
                    installPID.WaitForExit(60000);
                } catch { }

                try
                {
                    File.Delete(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\airserver_" + osArch + ".msi");
                }
                catch { }
            }
            public static void Uninstall(string networkURL)
            {
                var osArch = "x86";
                DownloadFileSingle(networkURL + "/v2/monitorUpdate/reqFiles/airserver_" + osArch + ".msi", "airserver_" + osArch + ".msi");
                try
                {
                    // Use msiexec to install AirServer from the Downloaded msi file
                    Process installPID = new Process();
                    installPID.StartInfo.FileName = "msiexec";
                    installPID.StartInfo.Arguments = "/quiet /x \"" + Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\airserver_" + osArch + ".msi";
                    installPID.StartInfo.Verb = "runas";
                    installPID.Start();
                    installPID.WaitForExit(60000);
                } catch { }

                try
                {
                    File.Delete(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\airserver_" + osArch + ".msi");
                }
                catch { }
                AirServer.RemoveCode();         // Remove the code from config.ini
            }
            public static void Rename(string networkURL, string newAirServerName)
            {
                if (osArch == "x64")
                {
                    theEXE = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + "\\App Dynamic\\AirServer\\AirServerConsole.exe";
                }
                Process airserver = new Process();
                try
                {
                    airserver.StartInfo.FileName = theEXE;
                    airserver.StartInfo.Verb = "runas";
                    airserver.StartInfo.Arguments = "set name " + newAirServerName;
                    airserver.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    airserver.Start();
                }
                catch { }
                AirServer.Stop();
                AirServer.Kill();
                AirServer.Start();
            }
        }

        public class WindowsEnv
        {
            public static int CheckUpdates()
            {
                int pendingUpdates = 0;
                try
                {
                    var updateSession = new UpdateSession();
                    var updateSearcher = updateSession.CreateUpdateSearcher();
                    updateSearcher.Online = false;
                    try
                    {
                        var searchResult = updateSearcher.Search("IsInstalled=0 And IsHidden=0");
                        if (searchResult.Updates.Count > 0)
                        {
                            // Debug.WriteLine("There are updates pending");
                            pendingUpdates = searchResult.Updates.Count;
                        }
                    }
                    catch { }
                }
                catch { }
                return pendingUpdates;
            }
        }

        // Private Function Only for the GCMSTrigger Class
        public static string GetFileSize(string filename)
        {
            double filesize = new FileInfo(filename).Length;
            return Convert.ToString(filesize);
        }
        public static void DownloadFileMulti(string downloadFile, string savedFile)
        {
            int numberDownloads = 4;
            var destinationFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString());
            var cpuArch = GetOSArch();
            if (cpuArch == "x86") { numberDownloads = 2; }
            // Try the Download
            try
            {
                var downloadResult = Downloader.Download(downloadFile, destinationFolderPath, numberDownloads);
            }
            catch { }
        }
        public static string DownloadFileSingle(string downloadFile, string savedFile)
        {
            using (var client = new WebClient())
            {
                try
                {
                    client.DownloadFile(downloadFile, savedFile);
                }
                catch { }
            }
            return "Failed";
        }
        private static void SendCaptureFile(string networkURL, string filename, string deviceUUID)
        {
            CodeScales.Http.HttpClient client = new CodeScales.Http.HttpClient();
            HttpPost postMethod = new HttpPost(new Uri(networkURL + "/v2/screencap.php"));

            MultipartEntity multipartEntity = new MultipartEntity();
            postMethod.Entity = multipartEntity;

            StringBody stringBody = new StringBody(Encoding.UTF8, "devUUID", deviceUUID);
            multipartEntity.AddBody(stringBody);

            FileInfo fileInfo = new FileInfo(@Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + filename);
            FileBody fileBody = new FileBody("file", filename, fileInfo);
            multipartEntity.AddBody(fileBody);

            HttpResponse response = client.Execute(postMethod);
        }
        private static void SendLogFile(string networkURL, string filename, string deviceUUID, string whichLog)
        {
            var theLog = "/v2/inboundLog.php";
            CodeScales.Http.HttpClient client = new CodeScales.Http.HttpClient();
            if (whichLog == "AVA")
            {
                theLog = "/v2/inboundAVA.php";
            }

            // Try Uploading The Log File
            try
            {
                HttpPost postMethod = new HttpPost(new Uri(networkURL + theLog));
                MultipartEntity multipartEntity = new MultipartEntity();
                postMethod.Entity = multipartEntity;

                StringBody stringBody = new StringBody(Encoding.UTF8, "devUUID", deviceUUID);
                multipartEntity.AddBody(stringBody);

                FileInfo fileInfo = new FileInfo(@Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + filename);
                FileBody fileBody = new FileBody("file", filename, fileInfo);
                multipartEntity.AddBody(fileBody);

                HttpResponse response = client.Execute(postMethod);
            }
            catch { }
        }
        private bool PingURL(string url)
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
        private static string GetVNCPass(string networkURL, string clientID)
        {
            string responseString;
            using (var client = new WebClient())
            {
                // Create the $_POST Data for the HTTP Request
                var values = new System.Collections.Specialized.NameValueCollection
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
        private static string GetSignageParams(string networkURL, string macAddy)
        {
            string responseString;
            using (var client = new WebClient())
            {
                // Create the $_POST Data for the HTTP Request
                var values = new System.Collections.Specialized.NameValueCollection
                {
                    ["hardwareMAC"] = macAddy
                };

                try
                {
                    var response = client.UploadValues(networkURL + "/v2/getSignParams.php", values);
                    responseString = Encoding.UTF8.GetString(response);
                }
                catch
                {
                    responseString = "Error";
                }
            }
            return responseString;
        }
        private static string GetSignageConfig(string networkURL, string macAddy)
        {
            string responseString;
            using (var client = new WebClient())
            {
                // Create the $_POST Data for the HTTP Request
                var values = new System.Collections.Specialized.NameValueCollection
                {
                    ["hardwareMAC"] = macAddy
                };

                try
                {
                    var response = client.UploadValues(networkURL + "/v2/getSignConfig.php", values);
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
        private static string GetSyncID(string networkURL, string macAddy, string devUUID)
        {
            string responseString;
            using (var client = new WebClient())
            {
                // Create the $_POST Data for the HTTP Request
                var values = new System.Collections.Specialized.NameValueCollection
                {
                    ["hardwareMAC"] = macAddy,
                    ["devUUID"] = devUUID
                };

                try
                {
                    var response = client.UploadValues(networkURL + "/v2/getSyncID.php", values);
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
        private static void SendCompleteNotification()
        {
            // Load the INI File
            string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
            var MyIni = new IniFile(iniFile);

            string networkURL;                                                // Which Network URL to use
            string networkNameShort;                                          // Network Name Short - A Single Letter P (Public) / S (Secure)

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
                    ["hardwareMAC"] = MyIni.Read("hardwareMAC", "Monitor")
                };

                var responseString = "";
                try
                {
                    var response = client.UploadValues(networkURL + "/v2/outboundSendCompleteNotification.php", values);
                    responseString = Encoding.UTF8.GetString(response);
                }
                catch
                {
                    responseString = "Error";
                    FileLogger.Log(DateTime.Now.ToString("dd MMM HH:mm:ss") + " - [ERR] Cannot Call API Server [" + networkNameShort + "] - Check Internet/Firewall");
                }
            }
        }
        private static string FixVPN(string macAddr, string networkURL, string clientID)
        {
            string responseString;
            using (var client = new WebClient())
            {
                // Create the $_POST Data for the HTTP Request
                var values = new System.Collections.Specialized.NameValueCollection
                {
                    ["clientID"] = clientID,
                    ["hardwareMAC"] = macAddr
                };

                try
                {
                    var response = client.UploadValues(networkURL + "/v2/vpnSystem/fixVPN.php", values);
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
        public static string GetOS(bool whitespace = false)
        {
            //Get Operating system information.
            OperatingSystem os = Environment.OSVersion;
            //Get version information about the os.
            Version vs = os.Version;

            //Variable to hold our return value
            string operatingSystem = "";

            if (os.Platform == PlatformID.Win32Windows)
            {
                //This is a pre-NT version of Windows
                switch (vs.Minor)
                {
                    case 0:
                        operatingSystem = "95";
                        break;
                    case 10:
                        if (vs.Revision.ToString() == "2222A")
                            operatingSystem = "98SE";
                        else
                            operatingSystem = "98";
                        break;
                    case 90:
                        operatingSystem = "Me";
                        break;
                    default:
                        break;
                }
            }
            else if (os.Platform == PlatformID.Win32NT)
            {
                switch (vs.Major)
                {
                    case 3:
                        operatingSystem = "NT 3.51";
                        break;
                    case 4:
                        operatingSystem = "NT 4.0";
                        break;
                    case 5:
                        if (vs.Minor == 0)
                            operatingSystem = "2000";
                        else
                            operatingSystem = "XP";
                        break;
                    case 6:
                        if (vs.Minor == 0)
                            operatingSystem = "Vista";
                        else if (vs.Minor == 1)
                            operatingSystem = "7";
                        else if (vs.Minor == 2)
                            operatingSystem = "8";
                        else
                            operatingSystem = "8";          // This is Windows 8.1
                        break;
                    case 10:
                        operatingSystem = "10";
                        break;
                    default:
                        break;
                }
            }

            //Make sure we actually got something in our OS check
            //We don't want to just return " Service Pack 2" or " 32-bit"
            //That information is useless without the OS version.
            if (operatingSystem != "")
            {
                if (!whitespace)
                {
                    operatingSystem = "Windows" + operatingSystem;
                }
                else
                {
                    operatingSystem = "Windows " + operatingSystem;
                }
            }
            return operatingSystem;
        }

        public static bool DriveExists(string driveLetterWithColonAndSlash)
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            var theOut = "";
            foreach (DriveInfo d in allDrives)
            {
                if (d.Name == driveLetterWithColonAndSlash) {
                    System.IO.DriveType driveType = d.DriveType;
                    switch (driveType)
                    {
                        case DriveType.CDRom:
                            theOut = "CDROM";
                            break;

                        case DriveType.Fixed:
                            // Local Drive
                            theOut = "FIXED";
                            break;

                        case DriveType.Network:
                            // Mapped Drive
                            theOut = "NETWORK";
                            break;

                        case DriveType.NoRootDirectory:
                            theOut = "NOROOTDIR";
                            break;

                        case DriveType.Ram:
                            theOut = "RAMDRIVE";
                            break;

                        case DriveType.Removable:
                            // Usually a USB Drive
                            theOut = "USBDISK";
                            break;

                        case DriveType.Unknown:
                            theOut = "UNKNOWN";
                            break;
                    }
                }
            }

            if (theOut == "FIXED")
            {
                return DriveInfo.GetDrives().Any(x => x.Name == driveLetterWithColonAndSlash);
            }
            else {
                return false;
            }
        }

        public static void KillProcessAndChildrens(int pid)
        {
            ManagementObjectSearcher processSearcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection processCollection = processSearcher.Get();
            try
            {
                Process proc = Process.GetProcessById(pid);
                if (!proc.HasExited) proc.Kill();
            }
            catch (ArgumentException)
            {
                // Process already exited.
            }

            if (processCollection != null)
            {
                foreach (ManagementObject mo in processCollection)
                {
                    KillProcessAndChildrens(Convert.ToInt32(mo["ProcessID"])); //kill child processes(also kills childrens of childrens etc.)
                }
            }
        }
        public static void RegisterApplicationRecoveryAndRestart()
        {
            uint KeepAliveInterval = 5000;

            // register for Application Restart
            RestartSettings restartSettings = new RestartSettings(string.Empty, RestartRestrictions.None);
            ApplicationRestartRecoveryManager.RegisterForApplicationRestart(restartSettings);

            // register for Application Recovery
            RecoverySettings recoverySettings = new RecoverySettings(new RecoveryData(PerformRecovery, null), KeepAliveInterval);
            ApplicationRestartRecoveryManager.RegisterForApplicationRecovery(recoverySettings);
        }
        public static void UnregisterApplicationRecoveryAndRestart()
        {
            ApplicationRestartRecoveryManager.UnregisterApplicationRestart();
            ApplicationRestartRecoveryManager.UnregisterApplicationRecovery();
        }
        public static int PerformRecovery(object parameter)
        {
            try
            {
                ApplicationRestartRecoveryManager.ApplicationRecoveryInProgress();
                ApplicationRestartRecoveryManager.ApplicationRecoveryFinished(true);
            }
            catch
            {
                ApplicationRestartRecoveryManager.ApplicationRecoveryFinished(false);
            }
            return 0;
        }

        public static string GetIP(string whichNIC)
        {
            string externalIP;
            string localIP;
            string vpnIP;

            if (whichNIC == "LAN")
            {
                try
                {
                    using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                    {
                        socket.Connect("api.globalcms.co.uk", 80);
                        IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                        localIP = endPoint.Address.ToString();
                        if (!localIP.Contains("192.168.") && !localIP.Contains("10.") && !localIP.Contains("172.32.") && !localIP.Contains("172.16.8.") && !localIP.Contains("172.16.73.") && !localIP.Contains("172.30.") && !localIP.Contains("172.17.") && !localIP.Contains("172.20."))
                        {
                            localIP = "Not Connected";
                        }
                    }
                }
                catch
                {
                    localIP = "Not Connected";
                }
                return localIP;
            }
            if (whichNIC == "VPN")
            {
                try
                {
                    using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                    {
                        socket.Connect("172.16.0.2", 80);
                        IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                        vpnIP = endPoint.Address.ToString();
                        if (!vpnIP.Contains("172.16."))
                        {
                            vpnIP = "Not Connected";
                        }
                    }
                }
                catch
                {
                    vpnIP = "Not Connected";
                }
                return vpnIP;
            }
            if (whichNIC == "WAN")
            {
                try
                {
                    using (WebClient client = new WebClient())
                    {
                        externalIP = client.DownloadString("https://api.globalcms.co.uk/myIP.php");
                    }

                    if (externalIP == "")
                    {
                        externalIP = (new WebClient()).DownloadString("http://checkip.dyndns.org/");
                        externalIP = (new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}")).Matches(externalIP)[0].ToString();
                    }
                    return externalIP;
                }
                catch
                {
                    return "Not Connected";
                }
            }
            return "GetIP";
        }

        public static string GetMACAddressOLD()
        {
            string mac = "";
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration where IPEnabled=true"))
            {
                IEnumerable<ManagementObject> objects = searcher.Get().Cast<ManagementObject>();
                mac = (from o in objects orderby o["IPConnectionMetric"] select o["MACAddress"].ToString()).FirstOrDefault();
            }
            return mac;
        }
        public static string GetMACAddress()
        {
            string macAddress = string.Empty;
            using (ManagementObjectSearcher objMOS = new ManagementObjectSearcher("Select * FROM Win32_NetworkAdapterConfiguration where IPEnabled=true"))
            {
                using (ManagementObjectCollection objMOC = objMOS.Get())
                {
                    foreach (ManagementObject objMO in objMOC)
                    {
                        object tempMacAddrObj = objMO["MacAddress"];

                        if (tempMacAddrObj == null) //Skip objects without a MACAddress
                        {
                            continue;
                        }
                        if (!tempMacAddrObj.ToString().StartsWith("00:FF"))               // OO:FF = Prefix for VPN TUN Adapters
                        {
                            macAddress = tempMacAddrObj.ToString();
                            break;  // No need to iterate through any more objects
                        }
                        //                }
                        objMO.Dispose();
                    }
                }
            }
            return macAddress;
        }
        public static string HowManyContent()
        {
            string outputStr = "";
            using (var webClient = new System.Net.WebClient())
            {
                if (SignageBrowser.isSignageLoaded)
                {
                    var json = webClient.DownloadString(@"http://localhost:444/contentcount");
                    var jsonData = JsonConvert.DeserializeObject<dynamic>(json);
                    outputStr = jsonData.ToString();
                }
            }
            return outputStr;
        }
        public static string GetRAMsize(string whichType)
        {
            ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject item in moc)
            {
                if (whichType == "Formatted")
                {
                    return Convert.ToString(Math.Round(Convert.ToDouble(item.Properties["TotalPhysicalMemory"].Value) / 1048576000, 0)) + " GB";
                }
                else
                {
                    return Convert.ToString(Math.Round(Convert.ToDouble(item.Properties["TotalPhysicalMemory"].Value)));
                }
            }

            return "RAMsize";
        }
        public static string GetRamLoad()
        {
            var wmiObject = new ManagementObjectSearcher("select * from Win32_OperatingSystem");
            var memoryValues = wmiObject.Get().Cast<ManagementObject>().Select(mo => new
            {
                FreePhysicalMemory = double.Parse(mo["FreePhysicalMemory"].ToString()),
                TotalVisibleMemorySize = double.Parse(mo["TotalVisibleMemorySize"].ToString())
            }).FirstOrDefault();

            if (memoryValues != null)
            {
                return Convert.ToString(Math.Round(((memoryValues.TotalVisibleMemorySize - memoryValues.FreePhysicalMemory) / memoryValues.TotalVisibleMemorySize) * 100)) + "% Load";
            }
            return "RAMload";
        }
        public static string GetRamFree()
        {
            var wmi = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem").Get().Cast<ManagementObject>().First();
            return Convert.ToString(Math.Round(Convert.ToDouble(wmi["FreePhysicalMemory"]))) + "000";
        }
        public static string CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("https://api.globalcms.co.uk/myIP.php"))
                {
                    return "Online";
                }
            }
            catch
            {
                return "Offline";
            }
        }
        public static string CheckForVPNConnection(string networkURL, string networkIP)
        {
            var request = (HttpWebRequest)WebRequest.Create("http://172.16.0.2/v2/myIP.php");
            request.UserAgent = "Android";
            request.KeepAlive = false;
            request.Timeout = 1500;
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        //Connection to internet available
                        return "Online";
                    }
                    else
                    {
                        //Connection to internet not available
                        return "Offline";
                    }
                }
            }
            catch
            {
                return "Offline";
            }

        }
        public static string CheckForSignage()
        {
            var signageProcess = Process.GetProcesses().Any(p => p.ProcessName.Contains("node"));
            if (signageProcess)
            {
                return "Online";
            }
            else
            {
                return "Offline";
            }
        }
        public static string CheckForAVA()
        {
            var avaProcess = Process.GetProcesses().Any(p => p.ProcessName.Contains("ava"));
            if (avaProcess)
            {
                return "YES";
            }
            else
            {
                return "NO";
            }
        }
        public static string CheckChromeVersion()
        {
            string user = GCMSSystem.Chrome.GetCurrentMachineUser();
            string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
            var lockFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "decom.lock");

            // Read INI File for Config.ini
            var MyIni = new IniFile(iniFile);
            var signageLoader = MyIni.Read("SignageLoader", "Sign");

            var path = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe", "", null);
            if (signageLoader == "4")
            {
                string pathDIR = Path.Combine("C:\\", "Users", user, "AppData", "Local", "Google", "Chrome SxS", "Application");
                path = pathDIR + "\\chrome.exe";
            }

            if (path != null)
            {
                return FileVersionInfo.GetVersionInfo(path.ToString()).FileVersion;
            }
            else
            {
                return "Error";
            }
        }
        public static string RemoveWhitespace(string str)
        {
            return string.Join("", str.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
        }
        public static bool Ping(string url)
        {
            if (!url.Contains("http://")) { url = "http://" + url;  }
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
        public static string GetCpuArch()
        {
            ManagementScope scope = new ManagementScope();
            ObjectQuery query = new ObjectQuery("SELECT Architecture FROM Win32_Processor");
            ManagementObjectSearcher search = new ManagementObjectSearcher(scope, query);
            ManagementObjectCollection results = search.Get();

            ManagementObjectCollection.ManagementObjectEnumerator e = results.GetEnumerator();
            e.MoveNext();
            ushort arch = (ushort)e.Current["Architecture"];

            switch (arch)
            {
                case 0:
                    return "x86";
                case 1:
                    return "MIPS";
                case 2:
                    return "Alpha";
                case 3:
                    return "PowerPC";
                case 6:
                    return "Itanium";
                case 9:
                    return "x64";
                default:
                    return "Unknown Architecture (WMI ID " + arch.ToString() + ")";
            }
        }
        public static string GetOSArch()
        {
            if (Environment.Is64BitOperatingSystem)
            {
                return "x64";
            }
            else
            {
                return "x86";
            }
        }
        public static string GetCPUTemp()
        {
            var cpuWMI = new ManagementObjectSearcher("SELECT * FROM Win32_Processor").Get().Cast<ManagementObject>().First();
            var device_cpu = (string)cpuWMI["Name"];
            var curTemp = "0";
            if (!device_cpu.Contains("Xeon")) {
                try
                {
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\WMI", "SELECT * FROM MSAcpi_ThermalZoneTemperature");
                    ManagementObjectCollection collection = searcher.Get();

                    foreach (ManagementBaseObject tempObject in collection)
                    {
                        curTemp = (tempObject["CurrentTemperature"].ToString());
                    }
                }
                catch { }
            }
            return curTemp;
        }
        public static void CheckKeyboardInstall()
        {
            string root = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\keyboard";
            string subdir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\keyboard\\Layouts";

            string rootEXE = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\keyboard\\OpenKeyboard.exe";
            string layoutXML = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\keyboard\\Layouts\\Default.xml";

            if (!Directory.Exists(root))
            {
                try { Directory.CreateDirectory(root); } catch { }
            }
            if (!Directory.Exists(subdir))
            {
                try { Directory.CreateDirectory(subdir); } catch { }
            }

            if (File.Exists(rootEXE)) { try { File.Delete(rootEXE); } catch { } }

            if (!File.Exists(rootEXE))
            {
                object ob = Properties.Resources.ResourceManager.GetObject("OpenKeyboard");
                byte[] myResBytes = (byte[])ob;
                using (FileStream fsDst = new FileStream(rootEXE, FileMode.CreateNew, FileAccess.Write))
                {
                    byte[] bytes = myResBytes;
                    fsDst.Write(bytes, 0, bytes.Length);
                    fsDst.Close();
                    fsDst.Dispose();
                }
            }
            if (!File.Exists(layoutXML))
            {
                try
                {
                    File.WriteAllText(layoutXML, Properties.Resources.Default);
                }
                catch { }
            }

            // Once we have written the "Default" Keyboard Layout, we should now check the API Server for its Actual Keyboard Settings
            InstallKeyboardConfig();
        }
        public static void InstallKeyboardConfig()
        {
            string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
            string configFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "keyboard", "Layouts", "Default.xml");
            var MyIni = new IniFile(iniFile);

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
                    ["hardwareMAC"] = MyIni.Read("hardwareMAC", "Monitor")
                };

                var responseString = "";
                try
                {
                    var response = client.UploadValues("http://api.globalcms.co.uk/v2/outboundKeyboardConfig.php", values);
                    responseString = Encoding.UTF8.GetString(response);
                }
                catch
                {
                    responseString = "Error";
                }

                if (responseString != "Error")
                {
                    try
                    {
                        System.Threading.Thread.Sleep(1000);
                        File.WriteAllText(configFile, responseString);
                    }
                    catch { }
                }
            }
        }
        public static void PreStartup()
        {
            try
            {
                GCMSSystem.Chrome.UpdatePref();                                  // Update Google Chrome Pref File to make sure no error messages appear
            }
            catch { }

            try
            {
                string startGCMSBatch = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "startGCMS.bat");
                if (!File.Exists(startGCMSBatch))
                {
                    DownloadFileSingle("http://api.globalcms.co.uk/v2/monitorUpdate/service/startGCMS.bat", "startGCMS.bat");
                }
            }
            catch { }
            try
            {
                string stopGCMSBatch = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "stopGCMS.bat");
                if (!File.Exists(stopGCMSBatch))
                {
                    DownloadFileSingle("http://api.globalcms.co.uk/v2/monitorUpdate/service/stopGCMS.bat", "stopGCMS.bat");
                }
            }
            catch { }

            // if the env.json file doesnt exist then create a blank file so that it wont error when trying to load the URL Webserver
            try
            {
                string envJSON = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "logs", "env.json");
                if (!File.Exists(envJSON))
                {
                    File.Create(envJSON).Dispose();
                }
            }
            catch { }

            try
            {
                var pingTest = Ping("api.globalcms.co.uk");
                var pingTest2 = Ping("172.16.0.2");
                if (pingTest || pingTest2)
                {
                    // Clear the Keyboard Folder so that we can extract the most up to date version of the keyboard
                    GCMSSystem.ClearFolder(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\keyboard");
                }
            }
            catch { }
        }
        public static void ClearFolder(string FolderName)
        {
            DirectoryInfo dir = new DirectoryInfo(FolderName);

            foreach (FileInfo fi in dir.GetFiles())
            {
                try
                {
                    fi.Delete();
                } catch { }
            }

            foreach (DirectoryInfo di in dir.GetDirectories())
            {
                ClearFolder(di.FullName);
                try { di.Delete(); } catch { }
            }
        }
        public static void EOCheck()
        {
            var eoDLL = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\EO.Base.dll";
            var EOversionInfo = FileVersionInfo.GetVersionInfo(eoDLL).FileVersion;
            if (EOversionInfo != MainForm.eoVersion)
            {
                try
                {
                    File.Delete(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()) + "\\EO.Base.dll");
                }
                catch { }
                try
                {
                    File.Delete(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()) + "\\EO.WebBrowser.dll");
                }
                catch { }
                try
                {
                    File.Delete(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()) + "\\EO.WebBrowser.WinForm.dll");
                }
                catch { }
                try
                {
                    File.Delete(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()) + "\\EO.WebEngine.dll");
                }
                catch { }

                // Download the Newest EO DLL Files
                try
                {
                    DownloadFileSingle("http://api.globalcms.co.uk/v2/monitorUpdate/dll/EO.Base.dll", "EO.Base.dll");
                }
                catch { }
                try
                {
                    DownloadFileSingle("http://api.globalcms.co.uk/v2/monitorUpdate/dll/EO.WebBrowser.dll", "EO.WebBrowser.dll");
                }
                catch { }
                try
                {
                    DownloadFileSingle("http://api.globalcms.co.uk/v2/monitorUpdate/dll/EO.WebBrowser.WinForm.dll", "EO.WebBrowser.WinForm.dll");
                }
                catch { }
                try
                {
                    DownloadFileSingle("http://api.globalcms.co.uk/v2/monitorUpdate/dll/EO.WebEngine.dll", "EO.WebEngine.dll");
                }
                catch { }
            }
        }
        public static HttpWebRequest GetRequest(string url, string httpMethod = "GET", bool allowAutoRedirect = true)
        {
            Uri uri = new Uri(url);
            HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.UserAgent = @"Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";

            request.Timeout = Convert.ToInt32(new TimeSpan(0, 5, 0).TotalMilliseconds);
            request.Method = httpMethod;
            return request;
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

        public static string CheckForDecom(string macAddr, string clientID, string deviceID)
        {
            // Check Remote Server to get the details for the Skinning
            var responseString = "";
            using (var client = new WebClient())
            {
                // Create the $_POST Data for the HTTP Request
                var values = new NameValueCollection
                {
                    ["clientID"] = clientID,
                    ["deviceID"] = deviceID,
                    ["hardwareMAC"] = macAddr
                };

                try
                {
                    var response = client.UploadValues("http://api.globalcms.co.uk/v2/checkForDecom.php", values);
                    responseString = Encoding.Default.GetString(response);
                    responseString = GCMSSystem.RemoveWhitespace(responseString);
                }
                catch
                {
                    responseString = "Error";
                }
            }
            return responseString;
        }

    }

    public static class User32Interop
    {
        public static TimeSpan GetLastInput()
        {
            var plii = new LASTINPUTINFO();
            plii.cbSize = (uint)Marshal.SizeOf(plii);

            if (GetLastInputInfo(ref plii))
                return unchecked(TimeSpan.FromMilliseconds((int)Environment.TickCount - (int)plii.dwTime));
            else
                throw new Exception("Error");
        }

        public static uint GetLastInputTime()
        {
            uint idleTime = 0;
            LASTINPUTINFO lastInputInfo = new LASTINPUTINFO();
            lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            uint envTicks = (uint)Environment.TickCount;

            if (GetLastInputInfo(ref lastInputInfo))
            {
                uint lastInputTick = lastInputInfo.dwTime;

                idleTime = envTicks - lastInputTick;
            }

            var OutStr = ((idleTime > 0) ? (idleTime / 1000) : 0);
            return OutStr;
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        struct LASTINPUTINFO
        {
            public uint cbSize;
            public uint dwTime;
        }
    }

}