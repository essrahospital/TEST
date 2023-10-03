using HardwareHelperLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace GlobalCMS
{
    public partial class Nexmosphere : Form
    {
        int theCNT = 0;
        public static int maxSensors = 79;
        public static bool startupComplete = false;
        public static string NexmosphereVersion = "0.0.2";
        private static SerialPort usb;                  // Serial Port Holder for global Access from Form and Functions
        delegate void SetTextCallback(string text);     // This delegate enables asynchronous calls for setting the text property on a TextBox control.

        private static string PreviousSensor;           // This is to work out the difference between Press and Release
        private static int globalMAX = 0;               // This is for the quiet system on Timer to stop it flooding and sending over and over again
        private static bool rfidSensor = false;         // To work out if this X000 Unit is actually a RFID Antenna

        private static string[] proximityArr = new string[0];       // Setup a Proximity Array - This is to store which Elements from Sensors are actually PRESENCE Sensors and Not Buttons/RFID
        private static string[] proximityCountArr = new string[0];  // Setup a Proximity Array - This is to store which Elements from Sensors are actually PRESENCE Sensors and Not Buttons/RFID
        private static string[] proximityTriggeredArr = new string[0];  // Setup a Proximity Array - This is to store which Elements from Sensors are actually PRESENCE Sensors and Not Buttons/RFID

        private static bool proximityEnabled = false;
        private static bool toTrigger = false;

        private static string configFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "nexusConfig.ini");

        private const int WM_DEVICECHANGE = 0x0219;                 // device change event 
        private const int DBT_DEVICEARRIVAL = 0x8000;               // system detected a new device 
        private const int DBT_DEVICEREMOVEPENDING = 0x8003;         // about to remove, still available 
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;        // device is gone 
        private const int DBT_DEVTYP_PORT = 0x00000003;      // serial, parallel 

        HH_Lib hwh = new HH_Lib();
        List<DEVICE_INFO> HardwareList;

        // This is to store the info about the Nexmosphere Hub
        public struct NexusUSB
        {
            public static string GUID = "";
            public static string DeviceID = "";
            public static string PnpDeviceID = "";
            public static string Description = "";
            public static string Caption = "";
        }

        public Nexmosphere()
        {
            // WindowState = FormWindowState.Minimized;
            MainForm.FrmObj.LastLogMsgOpt.Text = DateTime.Now.ToString("dd MMM HH:mm:ss") + " - Started Nexmosphere";
            WindowState = FormWindowState.Minimized;
            InitializeComponent();

            try
            {
                var usbDevices = GetUSBDevices();
                foreach (var usbDevice in usbDevices)
                {
                    if (usbDevice.Description.StartsWith("Prolific USB") && usbDevice.Caption.Contains("COM12"))
                    {
                        // Debug.WriteLine("GUID: {0}, Device ID: {1}, PNP Device ID: {2}, Description: {3}, Caption: {4}", usbDevice.GUID, usbDevice.DeviceID, usbDevice.PnpDeviceID, usbDevice.Description, usbDevice.Caption);
                        NexusUSB.GUID = usbDevice.GUID.ToString();
                        NexusUSB.DeviceID = usbDevice.DeviceID.ToString();
                        NexusUSB.PnpDeviceID = usbDevice.PnpDeviceID.ToString();
                        NexusUSB.Description = usbDevice.Description.ToString();
                        NexusUSB.Caption = usbDevice.Caption.ToString();
                        break;
                    }
                }
            }
            catch { }

            CurrentStatus.Text = "Nexmosphere Sensors Init";
            CurrentStatus.ForeColor = Color.FromArgb(0, 192, 0);

            try
            {
                if (!File.Exists(configFile))
                {
                    File.Create(configFile).Dispose();
                    BackFillINI();
                }
            }
            catch { }

            const int BufferSize = 128;
            using (var fileStream = File.OpenRead(configFile))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    if (line.StartsWith("[")) { continue; }

                    var SensorData = line.Split('=');
                    var lineItem = new ListViewItem(new[] { SensorData[0], SensorData[1], null, null, null, null, null });
                    if (SensorData[1] != "None")
                    {
                        var SensorConfig = SensorData[1].Split('?');
                        var SensorActions = SensorData[1].Split(',');

                        lineItem = new ListViewItem(new[] { SensorData[0], SensorConfig[0], SensorConfig[1], SensorConfig[2], SensorConfig[3], SensorConfig[4], SensorConfig[5] });
                    }
                    listBox1.Items.Add(lineItem);
                }

                listBox1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                listBox1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }

            usb = new SerialPort("COM12", 115200, Parity.None, 8, StopBits.One);
            bool nexmosphereBox = SerialPort.GetPortNames().Any(x => x == "COM12");
            if (nexmosphereBox)
            {
                NexmoStart();
            }
            else
            {
                CurrentStatus.Text = "Nexmosphere Sensor Error - Not Found";
                CurrentStatus.ForeColor = Color.FromArgb(192, 0, 0);
                COMLabel.ForeColor = Color.FromArgb(192, 0, 0);
                // Debug.WriteLine("Error no device found");
            }

            // CheckNexmoConnected.Enabled = true;
        }

        private void CheckNexmoConnected_Tick(object sender, EventArgs e)
        {
            bool nexmosphereBox = SerialPort.GetPortNames().Any(x => x == "COM12");
            if (!nexmosphereBox)
            {
                theCNT++;                // Add to Error CNT

                CurrentStatus.Text = "Nexmosphere Sensor Error - Not Found";
                CurrentStatus.ForeColor = Color.FromArgb(192, 0, 0);
                COMLabel.ForeColor = Color.FromArgb(192, 0, 0);

                if (theCNT == 5)
                {
                    try
                    {
                        usb.Close();
                        usb.DataReceived -= NexmoDataReceived;          // Unhook Event
                        usb.ErrorReceived -= NexmoDataError;            // Unhook Event

                        usb.DiscardOutBuffer();
                        usb.DiscardInBuffer();
                    }
                    catch { }

                    CurrentStatus.Text = "Nexmosphere Sensor Error - Not Found";
                    CurrentStatus.ForeColor = Color.FromArgb(192, 0, 0);
                    COMLabel.ForeColor = Color.FromArgb(192, 0, 0);

                    MainForm.FrmObj.NEXOpt.Text = "Disconnected";
                    MainForm.FrmObj.NEXOpt.ForeColor = Color.FromArgb(192, 0, 0);

                    Close();            // Close the Window as the Device cannot be found any longer
                }
            }
        }

        private void NexmoDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (!usb.IsOpen) return;            // This stops any IO Buffer Errors
            SetText(usb.ReadExisting());
        }

        private void NexmoDataError(object sender, SerialErrorReceivedEventArgs e)
        {
            if (!usb.IsOpen) return;            // This stops any IO Buffer Errors
        }

        private void NexmoStart()
        {
            Thread.Sleep(7000);             // Wait 7 seconds to allow the Nexmosphere Hub to startup (the Hub takes 5 seconds in order to startup
            try
            {
                usb = new SerialPort("COM12", 115200, Parity.None, 8, StopBits.One)
                {
                    RtsEnable = true,              // Request To Send, normally connected to CTS (Clear To Send) on the device
                    DtrEnable = true,              // Data Terminal Ready, normally connected to DSR (Data Set Ready) on the device

                    Encoding = Encoding.ASCII,
                    Handshake = Handshake.None,
                    ReadBufferSize = 4096,
                    ReadTimeout = -1,
                    ReceivedBytesThreshold = 1,
                    WriteBufferSize = 8000,
                    WriteTimeout = -1
                };

                usb.Open();

                CurrentStatus.Text = "Nexmosphere Sensors Connected";
                CurrentStatus.ForeColor = Color.FromArgb(0, 192, 0);

                COMLabel.ForeColor = Color.FromArgb(0, 192, 0);

                var ConfigINI = new IniFile(configFile);
                if (usb.IsOpen)
                {
                    // Init Config so that we do auto startup rules for LEDs etc
                    new System.Threading.Thread(() =>
                    {
                        System.Threading.Thread.CurrentThread.IsBackground = true;
                        for (int i = 1; i <= maxSensors; i++)
                        {
                            try
                            {
                                string sensorNumber = string.Format("{0:D3}", i);

                                // Read INI File for nexusConfig.ini
                                var sensorConfData = ConfigINI.Read("X" + sensorNumber, "Sensors");
                                if (sensorConfData != "None")
                                {
                                    var SensorConfig = sensorConfData.Split('?');
                                    var SensorType = SensorConfig[0];
                                    var SensorInit = SensorConfig[1].Split(',');
                                    var SensorActions = SensorConfig[3].Split(',');

                                    // Init Global Variables for PRESENCE Sensor
                                    if (SensorType == "PRESENCE")
                                    {
                                        // Debug.WriteLine("Found PRESENCE Sensor in Config // Sensor Number: " +sensorNumber);
                                        Array.Resize(ref proximityArr, proximityArr.Length + 1);
                                        Array.Resize(ref proximityCountArr, proximityCountArr.Length + 1);
                                        Array.Resize(ref proximityTriggeredArr, proximityTriggeredArr.Length + 1);

                                        proximityArr[proximityArr.GetUpperBound(0)] = sensorNumber;
                                        proximityCountArr[proximityArr.GetUpperBound(0)] = "0";
                                        proximityTriggeredArr[proximityArr.GetUpperBound(0)] = "No";
                                        proximityEnabled = true;
                                    }

                                    for (int t = 0; t <= (SensorInit.Length - 1); t++)
                                    {
                                        if (SensorInit[t] != "NONE")
                                        {
                                            usb.Write(string.Format("{0}\r\n", "X" + sensorNumber + SensorInit[t]));
                                            SetText2(string.Format("{0}\r\n", "X" + sensorNumber + SensorInit[t]));
                                            System.Threading.Thread.Sleep(100);         // Wait for 0.1 second
                                        }
                                    }
                                    System.Threading.Thread.Sleep(100);         // Wait for 0.2 second
                                }
                            }
                            catch { }
                        }
                        startupComplete = true;
                    }).Start();

                    // Hook Returns for Decoding
                    usb.DataReceived += NexmoDataReceived;
                    usb.ErrorReceived += NexmoDataError;

                    // Set the Connected String and Mark SendNexCMD = true;
                    CurrentStatus.Text = "Connected && Receiving Data";
                    SendNexCMD.Enabled = true;
                    SendClearCMD.Enabled = true;
                }
            }
            catch
            {
                CurrentStatus.Text = "Nexmosphere Sensor Error - Cannot Open Port";
                CurrentStatus.ForeColor = Color.FromArgb(192, 0, 0);
                COMLabel.ForeColor = Color.FromArgb(192, 0, 0);
                SendNexCMD.Enabled = false;
                SendClearCMD.Enabled = false;
            }
        }

        private void SetText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (txtData.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                Invoke(d, new object[] { text });
            }
            else
            {
                txtData.Text += text;
                txtData.SelectionStart = txtData.TextLength;
                txtData.ScrollToCaret();
            }
        }
        private void SetText2(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (txtData2.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText2);
                Invoke(d, new object[] { text });
            }
            else
            {
                txtData2.Text += text;
                txtData2.SelectionStart = txtData2.TextLength;
                txtData2.ScrollToCaret();
            }
        }

        public static void PowerCycleUSB(bool enable, string GuId, string devicePNP)
        {
            Guid mouseGuid = new Guid(GuId);
            string instancePath = @devicePNP;
        }

        private static string DecodeNexmosphereData(string text, string text2, bool rfid, bool showText)
        {
            var theResult = "";
            var theSensorRaw = "";
            var theSensorPort = "";
            var rfidId = "";

            string theSensorHub = text.Substring(0, 2);                         // Either XR OR X<number>
            if (!rfid)
            {
                theSensorHub = text.Substring(0, 1);                            // if rfid = false then its not XR meaning its a XT Unit/Sensor
            }
            if (theSensorHub == "X" || !rfid)
            {
                theSensorPort = text.Substring(1, 3);                           // XXX : where XXX is the Port Number on iOT Hub
            }
            if (theSensorHub == "XR" || rfid)
            {
                theSensorPort = text2.Substring(1, 3);                           // XXX : where XXX is the Port Number on iOT Hub
            }

            theSensorRaw = GetSubstringByString("[", "]", text);
            // Debug.WriteLine("theSensorHub: " + theSensorHub.ToString());
            // Debug.WriteLine("theSensorPort: " + theSensorPort.ToString());
            // Debug.WriteLine("theSensorRaw: " + theSensorRaw.ToString());
            if (theSensorHub == "XR" || rfid)
            {
                // If RFID then we need to work out if its PICK UP (PU) or PUT BACK (PB)
                string theSensorRaw2 = theSensorRaw.Substring(0, 2);                         // Either PU or PB
                // Debug.WriteLine("Sensor RAW: " + theSensorRaw2.ToString());
                rfidId = theSensorRaw.Substring(2, 3);
                // Debug.WriteLine("RFID TokenID: " + rfidId.ToString());
                theSensorRaw = theSensorRaw2;
            }

            if (theSensorRaw != "")
            {
                string theSensor = XTALK(theSensorHub, theSensorPort, theSensorRaw, rfidId, showText);             // This is for the Debugging Area to Convert XCODE to Text
                if (proximityEnabled && theSensor == "")
                {
                    // Becuase Presence Sensing is different (due to the way it fires ALOT of commands we need to change a few variables)
                    if (showText) { if (theSensorRaw == "1") { theSensor = "Presence Left"; } else { theSensor = "Presence Detected"; } frmObj.CurrentCodeInTxt.Visible = false; }
                    if (!showText) { if (theSensorRaw == "1") { theSensor = "PRESENCE_LEFT"; } else { theSensor = "PRESENCE_JOIN"; } frmObj.CurrentCodeInTxt.Visible = false; }
                }
                if (theSensor != "")
                {
                    frmObj.CurrentCodeIn.Text = theSensor;
                    theResult = theSensor;          // Update TextLabel to show Decoded XCODE
                    if (!showText)
                    {
                        var ConfigINI = new IniFile(configFile);
                        // Read INI File for nexusConfig.ini
                        var sensorConfData = ConfigINI.Read("X" + theSensorPort, "Sensors");
                        // Debug.WriteLine("theSensorPort: " + theSensorPort);
                        if (sensorConfData != "None")
                        {
                            var SensorConfig = sensorConfData.Split('?');
                            var SensorType = SensorConfig[0].ToUpper();
                            var SensorPwrDwn = SensorConfig[2].ToUpper();
                            var SensorActions = SensorConfig[3].Split(',');
                            var SensorTriggers = SensorConfig[4].Split(',');
                            // Debug.WriteLine("SensorConfig[4]: " + SensorConfig[4]);
                            var SensorXtraData = SensorConfig[5].Split(',');

                            if (SensorActions.Length > 0 && SensorTriggers.Length > 0 && SensorXtraData.Length > 0)
                            {
                                var convertedData = frmObj.CurrentCodeIn.Text;
                                if (convertedData == "RFID_PICKUP" || convertedData == "RFID_PUTBACK")
                                {
                                    SensorXtraData = SensorConfig[5].Split('#');
                                }

                                if (usb.IsOpen)
                                {
                                    int sensorArrKey = 0;               // By Default use the 1st Array Elem
                                    if (SensorType == "PRESENCE")
                                    {
                                        var ProximitySlot = theSensorRaw;
                                        int theNewCount = 0;
                                        int proxyID = 0;

                                        if (convertedData == "PRESENCE_JOIN") { sensorArrKey = 0; }
                                        if (convertedData == "PRESENCE_LEFT") { sensorArrKey = 1; }
                                        var convertedTriggers = SensorTriggers[sensorArrKey].ToUpper();
                                        var convertedXtraData = SensorXtraData[sensorArrKey].ToUpper();

                                        for (int proxy = 0; proxy <= (proximityArr.Length - 1); proxy++)
                                        {
                                            if (proximityArr[proxy] != theSensorPort) { continue; }         // Skip if the Sensor Doesnt Match
                                            int theCurrentCount = Convert.ToInt32(proximityCountArr[proxy]);
                                            theNewCount = (theCurrentCount + 1);
                                            if (ProximitySlot == "1")
                                            {
                                                theNewCount--;
                                                if (theNewCount < 0)
                                                {
                                                    theNewCount = 0;
                                                }
                                            }
                                            if (ProximitySlot == "1" && proximityTriggeredArr[proxyID] == "Yes")
                                            {
                                                // If ProximitySlot is 1 then that is the Sensor saying the person has "LEFT"
                                                if (proximityTriggeredArr[proxy] == "Yes")
                                                {
                                                    proximityTriggeredArr[proxyID] = "No";
                                                }
                                                toTrigger = true;
                                            }
                                            proximityCountArr[proxy] = theNewCount.ToString();
                                            proxyID = proxy;
                                        }

                                        // if the proximity counter hits 3 then we know the person is standing in front     !!False Positive Protection!!
                                        if (theNewCount > 3 && proximityTriggeredArr[proxyID] == "No")
                                        {
                                            theNewCount = 0;
                                            proximityCountArr[proxyID] = "0";
                                            proximityTriggeredArr[proxyID] = "Yes";
                                            toTrigger = true;
                                        }

                                        if ((SensorActions[sensorArrKey] == theSensor) && toTrigger)
                                        {
                                            if (convertedTriggers != "" && convertedTriggers != "NONE")
                                            {
                                                var SensorTriggersArr = convertedTriggers.Split('~');
                                                var SensorXtraDataArr = convertedXtraData.Split('~');
                                                for (int x = 0; x <= (SensorTriggersArr.Length - 1); x++)
                                                {
                                                    if (SensorTriggersArr[x] == "WEBSOCKET" && SensorType == "PRESENCE")
                                                    {
                                                        // If Websocket then we need to send an Internal Websocket to NodeJS Signage System
                                                        // Debug.WriteLine("Sensor : " + SensorTriggersArr[x] + " Sending Command: " + SensorXtraDataArr[x]);
                                                        GCMSSystem.NodeSocket.Send("custom:" + SensorXtraDataArr[x]);
                                                        frmObj.txtData2.Text += "Websocket: " + SensorXtraDataArr[x] + "\r\n";
                                                        frmObj.txtData2.SelectionStart = frmObj.txtData2.TextLength;
                                                        frmObj.txtData2.ScrollToCaret();
                                                        toTrigger = false;
                                                    }
                                                    if (SensorTriggersArr[x] != "WEBSOCKET" && SensorType == "PRESENCE")
                                                    {
                                                        var NexmosphereCMD = SensorTriggersArr[x] + SensorXtraDataArr[x];
                                                        // If Anythign other than Websocket then we are tellign the system to trigger on the Nexmosphere Unit
                                                        // Debug.WriteLine("Nexmosphere: " + SensorTriggersArr[x] + " Sending Comamnd : " + NexmosphereCMD);
                                                        usb.Write(string.Format("{0}\r\n", NexmosphereCMD));
                                                        frmObj.CurrentCodeOut.Text = NexmosphereCMD;
                                                        frmObj.SetText2(string.Format("{0}\r\n", NexmosphereCMD));
                                                        toTrigger = false;
                                                    }
                                                }
                                                theNewCount = 0;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        frmObj.CurrentCodeInTxt.Visible = false;
                                        toTrigger = false;
                                        var rfidKey = 0;
                                        if ((SensorType == "BUTTONBAR" || SensorType == "TOUCHBUTTON") && convertedData != "None")
                                        {
                                            if (convertedData == "BTN1_PRESS") { sensorArrKey = 0; }
                                            if (convertedData == "BTN2_PRESS") { sensorArrKey = 1; }
                                            if (convertedData == "BTN3_PRESS") { sensorArrKey = 2; }
                                            if (convertedData == "BTN4_PRESS") { sensorArrKey = 3; }
                                        }
                                        if (SensorType == "RFID" && convertedData != "None")
                                        {
                                            if (convertedData == "RFID_PICKUP") { sensorArrKey = 0; }
                                            if (convertedData == "RFID_PUTBACK") { sensorArrKey = 1; }
                                        }
                                        if (SensorType == "SECUREPICKUP" && convertedData != "None")
                                        {
                                            if (convertedData == "BTN1_PRESS") { sensorArrKey = 0; }
                                            if (convertedData == "BTN1_RELEASE") { sensorArrKey = 1; }
                                        }
                                        if (SensorType == "GENERALIO" && convertedData != "None")
                                        {
                                            if (convertedData == "BTN1_PRESS") { sensorArrKey = 0; }
                                            if (convertedData == "BTN1_RELEASE") { sensorArrKey = 1; }
                                        }

                                        var rfidToken = "";
                                        if (SensorActions[sensorArrKey] == theSensor)
                                        {
                                            var convertedTriggers = SensorTriggers[0].ToUpper();
                                            var convertedXtraData = SensorXtraData[0].ToUpper();
                                            if (SensorType == "RFID")
                                            {
                                                convertedTriggers = SensorTriggers[0].ToUpper();
                                                convertedXtraData = SensorXtraData[0].ToUpper();
                                                frmObj.CurrentCodeInTxt.Visible = true;

                                                if (convertedData == "RFID_PUTBACK")
                                                {
                                                    convertedTriggers = SensorTriggers[1].ToUpper();
                                                    convertedXtraData = SensorXtraData[1].ToUpper();
                                                }

                                                for (int a = 0; a <= (SensorTriggers.Length - 1); a++)
                                                {
                                                    if (rfidId == SensorTriggers[a])
                                                    {
                                                        rfidKey = a;
                                                        rfidToken = SensorTriggers[a];
                                                        // Debug.WriteLine("rfidKey: " + rfidKey);
                                                        break;
                                                    }
                                                }
                                                convertedTriggers = SensorTriggers[rfidKey].ToUpper();

                                                if (convertedData == "RFID_PICKUP")
                                                {
                                                    convertedXtraData = SensorXtraData[0].ToUpper();
                                                }
                                                if (convertedData == "RFID_PUTBACK")
                                                {
                                                    convertedXtraData = SensorXtraData[1].ToUpper();
                                                }
                                                // Debug.WriteLine("convertedXtraData: " + convertedXtraData);
                                            }

                                            // Debug.WriteLine("convertedTriggers: " + convertedTriggers);
                                            // Debug.WriteLine("convertedXtraData: " + convertedXtraData);
                                            if (SensorType != "RFID")
                                            {
                                                convertedTriggers = SensorTriggers[sensorArrKey].ToUpper();
                                                convertedXtraData = SensorXtraData[sensorArrKey].ToUpper();
                                            }

                                            if (convertedTriggers != "" && convertedTriggers != "NONE")
                                            {
                                                var SensorTriggersArr = convertedTriggers.Split('~');
                                                var SensorXtraDataArr = convertedXtraData.Split('~');
                                                // Debug.WriteLine("Triggered By: " + convertedData);
                                                // Debug.WriteLine("convertedTriggers: " + convertedTriggers);
                                                // Debug.WriteLine("convertedXtraData: " + convertedXtraData);

                                                if (SensorType == "RFID")
                                                {
                                                    SensorXtraDataArr = convertedXtraData.Split(',');
                                                }
                                                // Debug.WriteLine("SensorXtraDataArr Length: " + SensorTriggersArr.Length);

                                                if (SensorType == "RFID")
                                                {
                                                    if (rfidToken != rfidId) { frmObj.CurrentCodeInTxt.Visible = false; return ""; }
                                                    // Debug.WriteLine("rfidToken From Config: " + rfidToken);
                                                    // Debug.WriteLine("rfidId: " + rfidId);
                                                    for (int aa = 0; aa <= (SensorXtraDataArr.Length - 1); aa++)
                                                    {
                                                        var theTrig = "";
                                                        var theEngage = "";

                                                        if (aa == rfidKey)
                                                        {
                                                            // Debug.WriteLine("rfidTrigger: " + SensorXtraDataArr[aa]);
                                                            if (SensorXtraDataArr[aa] != "PB_NONE" && SensorXtraDataArr[aa] != "PU_NONE")
                                                            {
                                                                var rfidTriggerArr = SensorXtraDataArr[aa].Split(',');
                                                                for (int bb = 0; bb <= (rfidTriggerArr.Length - 1); bb++)
                                                                {
                                                                    theTrig = rfidTriggerArr[bb];
                                                                    var trigArr = theTrig.Split('~');
                                                                    for (int cc = 0; cc <= (trigArr.Length - 1); cc++)
                                                                    {
                                                                        theEngage = trigArr[cc];
                                                                        var theEngageArr = theEngage.Split('!');
                                                                        var EngageLeft = theEngageArr[0];
                                                                        var EngageRight = theEngageArr[1];

                                                                        var EngageLeftArr = EngageLeft.Split('_');
                                                                        var EngageLeftTrigger = EngageLeftArr[0];
                                                                        var EngageLeftOption = EngageLeftArr[1];

                                                                        // Debug.WriteLine("theTrig: " + theTrig);
                                                                        // Debug.WriteLine("----");
                                                                        // Debug.WriteLine("theEngage String: " + theEngage);

                                                                        // Debug.WriteLine("theEngage (L): " + EngageLeft);
                                                                        // Debug.WriteLine("theEngage Trigger (L): " + EngageLeftTrigger);
                                                                        // Debug.WriteLine("theEngage Option (L): " + EngageLeftOption);

                                                                        // Debug.WriteLine("theEngage (R): " + EngageRight);
                                                                        if (EngageLeftOption == "WEBSOCKET")
                                                                        {
                                                                            // If Websocket then we need to send an Internal Websocket to NodeJS Signage System
                                                                            // Debug.WriteLine("Sensor : " + rfidSensorConvertLeft + " Sending Command: " + rfidSensorConvertRight);
                                                                            GCMSSystem.NodeSocket.Send("custom:" + EngageRight);
                                                                            frmObj.txtData2.Text += "Websocket: " + EngageRight + "\r\n";
                                                                            frmObj.txtData2.SelectionStart = frmObj.txtData2.TextLength;
                                                                            frmObj.txtData2.ScrollToCaret();
                                                                        }
                                                                        if (EngageLeftOption != "WEBSOCKET")
                                                                        {
                                                                            var NexmosphereCMD = EngageLeftOption + EngageRight;
                                                                            // If Anythign other than Websocket then we are tellign the system to trigger on the Nexmosphere Unit
                                                                            // Debug.WriteLine("Nexmosphere: " + SensorTriggersArr[x] + " Sending Comamnd : " + NexmosphereCMD);
                                                                            usb.Write(string.Format("{0}\r\n", NexmosphereCMD));
                                                                            frmObj.CurrentCodeOut.Text = NexmosphereCMD;
                                                                            frmObj.SetText2(string.Format("{0}\r\n", NexmosphereCMD));
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }

                                                if (SensorType != "RFID")
                                                {
                                                    for (int x = 0; x <= (SensorTriggersArr.Length - 1); x++)
                                                    {
                                                        if (SensorTriggersArr[x] == "WEBSOCKET")
                                                        {
                                                            // If Websocket then we need to send an Internal Websocket to NodeJS Signage System
                                                            // Debug.WriteLine("Sensor : " + SensorTriggersArr[x] + " Sending Command: " + SensorXtraDataArr[x]);
                                                            GCMSSystem.NodeSocket.Send("custom:" + SensorXtraDataArr[x]);
                                                            frmObj.txtData2.Text += "Websocket: " + SensorXtraDataArr[x] + "\r\n";
                                                            frmObj.txtData2.SelectionStart = frmObj.txtData2.TextLength;
                                                            frmObj.txtData2.ScrollToCaret();
                                                        }
                                                        if (SensorTriggersArr[x] != "WEBSOCKET")
                                                        {
                                                            var NexmosphereCMD = SensorTriggersArr[x] + SensorXtraDataArr[x];
                                                            // If Anythign other than Websocket then we are tellign the system to trigger on the Nexmosphere Unit
                                                            // Debug.WriteLine("Nexmosphere: " + SensorTriggersArr[x] + " Sending Comamnd : " + NexmosphereCMD);
                                                            usb.Write(string.Format("{0}\r\n", NexmosphereCMD));
                                                            frmObj.CurrentCodeOut.Text = NexmosphereCMD;
                                                            frmObj.SetText2(string.Format("{0}\r\n", NexmosphereCMD));
                                                        }
                                                    }
                                                }
                                                // Debug.WriteLine("-------------------------");
                                            }
                                            else
                                            {
                                                // Debug.WriteLine(theSensor + " has no assigned Trigger");
                                            }
                                        }
                                    }
                                }
                            }
                            System.Threading.Thread.Sleep(500);         // Wait for 0.5 second
                        }
                    }
                }
            }
            return theResult;
        }

        private static string XTALK(string theSensorHub, string theSensorPort, string XCODE, string rfidId, bool showText)
        {
            string returnValue = "";
            if (XCODE == "0")
            {
                if (PreviousSensor == "1")
                {
                    returnValue = "BTN1_RELEASE";
                    if (showText)
                    {
                        returnValue = "Button #1 Released";
                    }
                }
                if (PreviousSensor == "3")
                {
                    returnValue = "BTN1_RELEASE";
                    if (showText)
                    {
                        returnValue = "Button #1 Released";
                    }
                }
                if (PreviousSensor == "5")
                {
                    returnValue = "BTN2_RELEASE";
                    if (showText)
                    {
                        returnValue = "Button #2 Released";
                    }
                }
                if (PreviousSensor == "9")
                {
                    returnValue = "BTN3_RELEASE";
                    if (showText)
                    {
                        returnValue = "Button #3 Released";
                    }
                }
                if (PreviousSensor == "17")
                {
                    returnValue = "BTN4_RELEASE";
                    if (showText)
                    {
                        returnValue = "Button #4 Released";
                    }
                }
            }
            if (XCODE == "1")
            {
                if (PreviousSensor == "0")
                {
                    returnValue = "BTN1_PRESS";
                    if (showText)
                    {
                        returnValue = "Button #1 Pressed";
                    }
                }
            }
            if (XCODE == "3")
            {
                returnValue = "BTN1_PRESS";
                if (showText)
                {
                    returnValue = "Button #1 Pressed";
                }
            }
            if (XCODE == "5")
            {
                returnValue = "BTN2_PRESS";
                if (showText)
                {
                    returnValue = "Button #2 Pressed";
                }
            }
            if (XCODE == "9")
            {
                returnValue = "BTN3_PRESS";
                if (showText)
                {
                    returnValue = "Button #3 Pressed";
                }
            }
            if (XCODE == "17")
            {
                returnValue = "BTN4_PRESS";
                if (showText)
                {
                    returnValue = "Button #4 Pressed";
                }
            }

            // RFID Codes
            if (XCODE == "PU")
            {
                returnValue = "RFID_PICKUP";
                if (showText)
                {
                    returnValue = "RFID [Token: " + rfidId.ToString() + "]";
                }
            }
            if (XCODE == "PB")
            {
                returnValue = "RFID_PUTBACK";
                if (showText)
                {
                    returnValue = "RFID [Token: " + rfidId.ToString() + "]";
                }
            }
            PreviousSensor = XCODE;
            // Debug.WriteLine("PreviousSensor: " + PreviousSensor.ToString());
            return returnValue;
        }

        public static string GetSubstringByString(string a, string b, string c)
        {
            return c.Substring((c.IndexOf(a) + a.Length), (c.IndexOf(b) - c.IndexOf(a) - a.Length));
        }

        private void BackFillINI()
        {
            var ConfigINI = new IniFile(configFile);

            // Backfill for SensorsXT (Buttons and LEDs etc)
            for (int i = 1; i <= maxSensors; i++)
            {
                string sensorNumber = string.Format("{0:D3}", i);

                // Read INI File for nexusConfig.ini
                var sensorData = ConfigINI.Read("X" + sensorNumber, "Sensors");
                if (sensorData == "")
                {
                    ConfigINI.Write("X" + sensorNumber, "None", "Sensors");
                }
            }
        }

        private void CheckSensorsTimer_Tick(object sender, EventArgs e)
        {
            // If Proximity Sensor Found then Mark as Active
            if (proximityEnabled)
            {
                frmObj.CurrentCodeInPresence.Text = "ACTIVE"; frmObj.CurrentCodeInPresence.ForeColor = Color.FromArgb(0, 192, 0);
            }
            else
            {
                frmObj.CurrentCodeInPresence.Text = "INACTIVE"; frmObj.CurrentCodeInPresence.ForeColor = Color.FromArgb(192, 0, 0);
            }

            // RunTime Checker
            int maxLines = txtData.Lines.Length;
            if ((maxLines - 3) != globalMAX && startupComplete)                // Lockdown so the timer doesnt keep firing
            {
                var lastLine2 = "";
                if (maxLines > 0)
                {
                    string lastLine = txtData.Lines[maxLines - 1];
                    rfidSensor = false;
                    if (maxLines >= 1)
                    {
                        rfidSensor = false;
                        lastLine = txtData.Lines[maxLines - 2];
                        string theSensorHub = lastLine.Substring(0, 1);                         // Either X or XR

                        if (theSensorHub == "X")
                        {
                            // if it has detected a X command we need to check the previous line to make sure its not an RFID
                            lastLine2 = txtData.Lines[maxLines - 3];
                            if (lastLine2.StartsWith("XR"))
                            {
                                // Detected that RFID was used and not a button (RFID Shoots 2 CMD's 1 for the RFID and One to say No Button was Pushed)
                                rfidSensor = true;
                                lastLine = lastLine2;
                                lastLine2 = txtData.Lines[maxLines - 2];
                            }
                        }
                    }

                    var decodedData = DecodeNexmosphereData(lastLine, lastLine2, rfidSensor, false);
                    if (decodedData != "")
                    {
                        var decodedDataTxt = DecodeNexmosphereData(lastLine, lastLine2, rfidSensor, true);
                        CurrentCodeIn.Text = decodedData;
                        CurrentCodeInTxt.Text = decodedDataTxt;
                        globalMAX = (txtData.Lines.Length - 3);           // The 2 Init Codes Plus the Blank End Line
                    }
                }
            }
        }

        static Nexmosphere _frmObj;
        public static Nexmosphere frmObj
        {
            get { return _frmObj; }
            set { _frmObj = value; }
        }
        private void NexmosphereLoad()
        {
            frmObj = this;
            frmObj.CurrentCodeIn.Text = "BTN1_RELEASE";             // This is becuase we add some default initiations on startup
        }

        private void NexmosphereClosed()
        {
            MainForm.FrmObj.LastLogMsgOpt.Text = DateTime.Now.ToString("dd MMM HH:mm:ss") + " - Stopped Nexmosphere";
            var ConfigINI = new IniFile(configFile);

            // Init Config so that we do auto startup rules for LEDs etc
            new System.Threading.Thread(() =>
            {
                System.Threading.Thread.CurrentThread.IsBackground = true;
                for (int i = 1; i <= maxSensors; i++)
                {
                    try
                    {
                        string sensorNumber = string.Format("{0:D3}", i);

                        // Read INI File for nexusConfig.ini
                        var sensorConfData = ConfigINI.Read("X" + sensorNumber, "Sensors");
                        if (sensorConfData != "None")
                        {
                            var SensorConfig = sensorConfData.Split('?');
                            var SensorType = SensorConfig[0];
                            var SensorPwrDwn = SensorConfig[2].Split(',');
                            var SensorActions = SensorConfig[3].Split(',');

                            if (usb.IsOpen)
                            {
                                for (int t = 0; t <= (SensorPwrDwn.Length - 1); t++)
                                {
                                    if (SensorPwrDwn[t] != "NONE")
                                    {
                                        usb.Write(string.Format("{0}\r\n", "X" + sensorNumber + SensorPwrDwn[t]));
                                        System.Threading.Thread.Sleep(100);         // Wait for 0.1 second
                                    }
                                }
                            }
                            System.Threading.Thread.Sleep(100);         // Wait for 0.1 second
                        }
                    }
                    catch { }
                }

                // Shutdown the Port correctly to stop any errors
                try
                {
                    if (usb.IsOpen)
                    {
                        usb.Close();
                        usb.DataReceived -= NexmoDataReceived;          // Unhook Event
                        usb.ErrorReceived -= NexmoDataError;            // Unhook Event

                        usb.DiscardOutBuffer();
                        usb.DiscardInBuffer();
                    }
                }
                catch { }
            }).Start();
        }

        // USB Handlers
        class USBDeviceInfo
        {
            public USBDeviceInfo(string classGuid, string deviceID, string pnpDeviceID, string description, string caption)
            {
                GUID = classGuid;
                DeviceID = deviceID;
                PnpDeviceID = pnpDeviceID;
                Description = description;
                Caption = caption;
            }
            public string GUID { get; private set; }
            public string DeviceID { get; private set; }
            public string PnpDeviceID { get; private set; }
            public string Description { get; private set; }
            public string Caption { get; private set; }
        }
        static List<USBDeviceInfo> GetUSBDevices()
        {
            List<USBDeviceInfo> devices = new List<USBDeviceInfo>();

            ManagementObjectCollection collection;
            using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_PnPEntity"))
                collection = searcher.Get();

            foreach (var device in collection)
            {
                devices.Add(new USBDeviceInfo(
                    (string)device.GetPropertyValue("ClassGuid"),
                    (string)device.GetPropertyValue("DeviceID"),
                    (string)device.GetPropertyValue("PNPDeviceID"),
                    (string)device.GetPropertyValue("Description"),
                    (string)device.GetPropertyValue("Caption")
                ));

            }

            collection.Dispose();
            return devices;
        }

        // Hardware List Generator
        public void ReloadHardwareList()
        {
            int totalHardwareCNT = 0;
            HardwareList = hwh.GetAll();
            listdevices.Items.Clear();
            foreach (var device in HardwareList)
            {
                totalHardwareCNT++;
                if (device.name.StartsWith("Prolific USB") && device.friendlyName.Contains("COM12"))
                {
                    var deviceCOM = GetSubstringByString("(", ")", device.friendlyName);
                    ListViewItem lvi = new ListViewItem(new string[] { device.name, device.friendlyName, device.hardwareId, deviceCOM, device.status.ToString() });
                    listdevices.Items.Add(lvi);
                }
                else
                {
                    totalHardwareCNT--;
                }
            }
            NumberOfConnectedDevices.Text = totalHardwareCNT.ToString() + " Devices Attached";
        }

        // Nexmosphere Command (Manual)
        private void SendNexCMD_Click(object sender, EventArgs e)
        {
            var command = txtToSend.Text;
            if (usb.IsOpen)
            {
                usb.Write(string.Format("{0}\r\n", command));
                CurrentCodeOut.Text = command;
                SetText2(string.Format("{0}\r\n", command));
            }
        }

        protected override void WndProc(ref Message m)
        {
            int devType;
            base.WndProc(ref m);

            switch (m.WParam.ToInt32())
            {
                case DBT_DEVICEARRIVAL:
                    ReloadHardwareList();
                    devType = Marshal.ReadInt32(m.LParam, 4);

                    if (devType == DBT_DEVTYP_PORT)
                    {
                        // usb device inserted,  
                        // get the friendly name of the new device 
                        // if the friendly name matches my device name,  
                        // call my procedure for new device
                        // Debug.WriteLine("Connect: " + m.ToString());
                        CurrentStatus.Text = "";
                        CurrentStatus.ForeColor = Color.FromArgb(0, 192, 0);
                        COMLabel.ForeColor = Color.FromArgb(0, 192, 0);

                        NexmoStart();
                    }
                    break;

                case DBT_DEVICEREMOVECOMPLETE:
                    ReloadHardwareList();
                    devType = Marshal.ReadInt32(m.LParam, 4);

                    if (devType == DBT_DEVTYP_PORT)
                    {
                        // usb device removed,  
                        // get the friendly name of the device removed 
                        // if the friendly name matches my device name, 
                        // call my procedure for removed device 
                        // Debug.WriteLine("Disconnect: " + m.ToString());
                        CurrentStatus.Text = "Nexmosphere Sensor Error - Not Found";
                        CurrentStatus.ForeColor = Color.FromArgb(192, 0, 0);
                        COMLabel.ForeColor = Color.FromArgb(192, 0, 0);

                        usb.Close();

                        try
                        {
                           
                            usb.DataReceived -= NexmoDataReceived;          // Unhook Event
                            usb.ErrorReceived -= NexmoDataError;            // Unhook Event

                            usb.DiscardOutBuffer();
                            usb.DiscardInBuffer();
                        }
                        catch { }
                    }
                    break;
            }
        }

        // Functions for Start and Close
        private void NexmosphereClosed(object sender, FormClosingEventArgs e)
        {
            hwh.CutLooseHardwareNotifications(this.Handle);
            hwh = null;

            NexmosphereClosed();
        }

        private void NexmosphereLoad(object sender, EventArgs e)
        {
            ReloadHardwareList();
            hwh.HookHardwareNotifications(this.Handle, true);
            NexmosphereLoad();
        }

        private void SendClearCMD_Click(object sender, EventArgs e)
        {
            txtData.Text = "X001A[3]\r\nX001A[0]\r\n";
        }
    }
}