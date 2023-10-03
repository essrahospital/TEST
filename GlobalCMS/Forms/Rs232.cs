using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace GlobalCMS
{
    public partial class Rs232 : Form
    {
        private static string configFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "serialConfig.ini");
        public static int NumberOfCom = 0;

        // Presetup all ComPort Serial Containers
        private static SerialPort COM1;                 // Serial Port Holder for global Access from Form and Functions [COM1]
        private static SerialPort COM2;                 // Serial Port Holder for global Access from Form and Functions [COM2]
        private static SerialPort COM3;                 // Serial Port Holder for global Access from Form and Functions [COM3]
        private static SerialPort COM4;                 // Serial Port Holder for global Access from Form and Functions [COM4]
        private static SerialPort COM5;                 // Serial Port Holder for global Access from Form and Functions [COM5]
        private static SerialPort COM6;                 // Serial Port Holder for global Access from Form and Functions [COM6]
        private static SerialPort COM7;                 // Serial Port Holder for global Access from Form and Functions [COM7]
        private static SerialPort COM8;                 // Serial Port Holder for global Access from Form and Functions [COM8]
        private static SerialPort COM9;                 // Serial Port Holder for global Access from Form and Functions [COM9]
        delegate void SetTextCallback(string text, string whichCOM);     // This delegate enables asynchronous calls for setting the text property on a TextBox control.
        delegate void SetTextCallback2(string text);                     // This delegate enables asynchronous calls for setting the text property on a TextBox control.

        static Rs232 _frmObj;
        public static Rs232 FrmObj
        {
            get { return _frmObj; }
            set { _frmObj = value; }
        }

        public Rs232()
        {
            InitializeComponent();
        }

        private void Rs232_Load(object sender, EventArgs e)
        {
            NumberOfCom = 0;            // This stops any bug with the counter
            NumberOfConnectedDevices.Text = "0 Devices";
            try
            {
                if (!File.Exists(configFile))
                {
                    File.Create(configFile).Dispose();
                    BackFillINI();
                }
            }
            catch { }

            string[] ports = SerialPort.GetPortNames();
            foreach (string comport in ports)
            {
                if (comport != "COM10" && comport != "COM11" && comport != "COM12" && comport != "COM13" && comport != "COM14")
                {
                    NumberOfCom++;
                    COMListDropdwn.Items.Add(comport);
                }
            }

            if (NumberOfCom == 0) { NumberOfConnectedDevices.Text = NumberOfCom + " Devices"; }
            if (NumberOfCom == 1) { NumberOfConnectedDevices.Text = NumberOfCom + " Device"; }
            if (NumberOfCom > 1) { NumberOfConnectedDevices.Text = NumberOfCom + " Devices"; }

            if (NumberOfCom > 0)
            {
                COMListDropdwn.SelectedIndex = 0;           // Set COM Dropdown to the 1st available option
                SendCMD.Enabled = true;
                SendClear.Enabled = true;
            }

            const int BufferSize = 128;
            using (var fileStream = File.OpenRead(configFile))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    if (line.StartsWith("[")) { continue; }
                    var ComStatus = "Closed";
                    var SensorData = line.Split('=');
                    var lineItem = new ListViewItem(new[] { SensorData[0], null, null, null, null, ComStatus, null, null });
                    if (SensorData[1] != "None")
                    {
                        var ComConfig = SensorData[1].Split(',');
                        var CtrlCodeConfig = ComConfig[4].Split('!');

                        int baudRate = Convert.ToInt32(ComConfig[0]);
                        Parity parityRate = (Parity)Enum.Parse(typeof(Parity), ComConfig[1]);
                        int dataRate = Convert.ToInt32(ComConfig[2]);
                        StopBits stopBits = (StopBits)Enum.Parse(typeof(StopBits), ComConfig[3]);

                        using (SerialPort port = new SerialPort(SensorData[0], baudRate, parityRate, dataRate, stopBits))
                        {
                            port.Handshake = Handshake.None;
                            port.ReadBufferSize = 4096;
                            port.ReadTimeout = -1;
                            port.ReceivedBytesThreshold = 1;
                            port.WriteBufferSize = 8000;
                            port.WriteTimeout = -1;

                            if (SensorData[0] == "COM1")
                            {
                                COM1 = port;
                                try
                                {
                                    COM1.Open();
                                } catch { }
                                if (COM1.IsOpen)
                                {
                                    COM1.DataReceived += Com1Received;
                                    COM1.ErrorReceived += Com1Error;
                                    ComStatus = "Open";
                                }
                            }
                            if (SensorData[0] == "COM2")
                            {
                                COM2 = port;
                                try
                                {
                                    COM2.Open();
                                }
                                catch { }
                                if (COM2.IsOpen)
                                {
                                    COM2.DataReceived += Com2Received;
                                    COM2.ErrorReceived += Com2Error;
                                    ComStatus = "Open";
                                }
                            }
                            if (SensorData[0] == "COM3")
                            {
                                COM3 = port;
                                try
                                {
                                    COM3.Open();
                                }
                                catch { }
                                if (COM3.IsOpen)
                                {
                                    COM3.DataReceived += Com3Received;
                                    COM3.ErrorReceived += Com3Error;
                                    ComStatus = "Open";
                                }
                            }
                            if (SensorData[0] == "COM4")
                            {
                                COM4 = port;
                                try
                                {
                                    COM4.Open();
                                }
                                catch { }
                                if (COM4.IsOpen)
                                {
                                    COM4.DataReceived += Com4Received;
                                    COM4.ErrorReceived += Com4Error;
                                    ComStatus = "Open";
                                }
                            }
                            if (SensorData[0] == "COM5")
                            {
                                COM5 = port;
                                try
                                {
                                    COM5.Open();
                                }
                                catch { }
                                if (COM5.IsOpen)
                                {
                                    COM5.DataReceived += Com5Received;
                                    COM5.ErrorReceived += Com5Error;
                                    ComStatus = "Open";
                                }
                            }
                            if (SensorData[0] == "COM6")
                            {
                                COM6 = port;
                                try
                                {
                                    COM6.Open();
                                }
                                catch { }
                                if (COM6.IsOpen)
                                {
                                    COM6.DataReceived += Com6Received;
                                    COM6.ErrorReceived += Com6Error;
                                    ComStatus = "Open";
                                }
                            }
                            if (SensorData[0] == "COM7")
                            {
                                COM7 = port;
                                try
                                {
                                    COM7.Open();
                                }
                                catch { }
                                if (COM7.IsOpen)
                                {
                                    COM7.DataReceived += Com7Received;
                                    COM7.ErrorReceived += Com7Error;
                                    ComStatus = "Open";
                                }
                            }
                            if (SensorData[0] == "COM8")
                            {
                                COM8 = port;
                                try
                                {
                                    COM8.Open();
                                }
                                catch { }
                                if (COM8.IsOpen)
                                {
                                    COM8.DataReceived += Com8Received;
                                    COM8.ErrorReceived += Com8Error;
                                    ComStatus = "Open";
                                }
                            }
                            if (SensorData[0] == "COM9")
                            {
                                COM9 = port;
                                try
                                {
                                    COM9.Open();
                                }
                                catch { }
                                if (COM9.IsOpen)
                                {
                                    COM9.DataReceived += Com9Received;
                                    COM9.ErrorReceived += Com9Error;
                                    ComStatus = "Open";
                                }
                            }
                        }
                        lineItem = new ListViewItem(new[] { SensorData[0], ComConfig[0], ComConfig[1], ComConfig[2], ComConfig[3], ComStatus, null, ComConfig[4] });
                    }
                    listBox1.Items.Add(lineItem);
                }
                listBox1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
                listBox1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
            FrmObj = this;
        }
        private void Rs232_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Make sure that all COM ports are shut correctly
            try
            {
                if (COM1.IsOpen)
                {
                    COM1.Close();
                    COM1.DataReceived -= Com1Received;          // Unhook Event
                    COM1.ErrorReceived -= Com1Error;            // Unhook Event
                    COM1.DiscardOutBuffer();
                    COM1.DiscardInBuffer();
                }
            }
            catch { }
            try
            {
                if (COM2.IsOpen)
                {
                    COM2.Close();
                    COM2.DataReceived -= Com2Received;          // Unhook Event
                    COM2.ErrorReceived -= Com2Error;            // Unhook Event
                    COM2.DiscardOutBuffer();
                    COM2.DiscardInBuffer();
                }
            }
            catch { }
            try
            {
                if (COM3.IsOpen)
                {
                    COM3.Close();
                    COM3.DataReceived -= Com3Received;          // Unhook Event
                    COM3.ErrorReceived -= Com3Error;            // Unhook Event
                    COM3.DiscardOutBuffer();
                    COM3.DiscardInBuffer();
                }
            }
            catch { }
            try
            {
                if (COM4.IsOpen)
                {
                    COM4.Close();
                    COM4.DataReceived -= Com4Received;          // Unhook Event
                    COM4.ErrorReceived -= Com4Error;            // Unhook Event
                    COM4.DiscardOutBuffer();
                    COM4.DiscardInBuffer();
                }
            }
            catch { }
            try
            {
                if (COM5.IsOpen)
                {
                    COM5.Close();
                    COM5.DataReceived -= Com5Received;          // Unhook Event
                    COM5.ErrorReceived -= Com5Error;            // Unhook Event
                    COM5.DiscardOutBuffer();
                    COM5.DiscardInBuffer();
                }
            }
            catch { }
            try
            {
                if (COM6.IsOpen)
                {
                    COM6.Close();
                    COM6.DataReceived -= Com6Received;          // Unhook Event
                    COM6.ErrorReceived -= Com6Error;            // Unhook Event
                    COM6.DiscardOutBuffer();
                    COM6.DiscardInBuffer();
                }
            }
            catch { }
            try
            {
                if (COM7.IsOpen)
                {
                    COM7.Close();
                    COM7.DataReceived -= Com7Received;          // Unhook Event
                    COM7.ErrorReceived -= Com7Error;            // Unhook Event
                    COM7.DiscardOutBuffer();
                    COM7.DiscardInBuffer();
                }
            }
            catch { }
            try
            {
                if (COM8.IsOpen)
                {
                    COM8.Close();
                    COM8.DataReceived -= Com8Received;          // Unhook Event
                    COM8.ErrorReceived -= Com8Error;            // Unhook Event
                    COM8.DiscardOutBuffer();
                    COM8.DiscardInBuffer();
                }
            }
            catch { }
            try
            {
                if (COM9.IsOpen)
                {
                    COM9.Close();
                    COM9.DataReceived -= Com9Received;          // Unhook Event
                    COM9.ErrorReceived -= Com9Error;            // Unhook Event
                    COM9.DiscardOutBuffer();
                    COM9.DiscardInBuffer();
                }
            }
            catch { }
        }
        private void SendClear_Click(object sender, EventArgs e)
        {
            txtData1.Text = "";
            txtData2.Text = "";
        }
        private void SendCMD_Click(object sender, EventArgs e)
        {
            var selectedCOM = COMListDropdwn.Text;
            if (selectedCOM != "COM1" && selectedCOM != "COM2" && selectedCOM != "COM3" && selectedCOM != "COM4" && selectedCOM != "COM5" && selectedCOM != "COM6" && selectedCOM != "COM7" && selectedCOM != "COM8" && selectedCOM != "COM9")
            {
                return;
            }
            //string selectedCMD = TxtToSend.Text + "\r";
            string selectedCMD = TxtToSend.Text;
            if (selectedCMD != "")
            {
                byte[] actualCMD = HexStringConverter.ToByteArray(selectedCMD);
                if (selectedCOM == "COM1") {
                    if (!COM1.IsOpen)
                    {
                        COM1.Open();
                        COM1.DataReceived += Com1Received;
                        COM1.ErrorReceived += Com1Error;
                    }
                    try {
                        COM1.Write(actualCMD, 0, actualCMD.Length);
                    }
                    catch (Exception error)
                    {
                        Debug.WriteLine("Error Sending String to COM1: " + error.ToString());
                    }
                }
                if (selectedCOM == "COM2")
                {
                    if (!COM2.IsOpen)
                    {
                        COM2.Open();
                        COM2.DataReceived += Com2Received;
                        COM2.ErrorReceived += Com2Error;
                    }
                    try
                    {
                        COM2.Write(actualCMD, 0, actualCMD.Length);
                    }
                    catch (Exception error)
                    {
                        Debug.WriteLine("Error Sending String to COM2: " + error.ToString());
                    }
                }
                if (selectedCOM == "COM3")
                {
                    if (!COM3.IsOpen)
                    {
                        COM3.Open();
                        COM3.DataReceived += Com3Received;
                        COM3.ErrorReceived += Com3Error;
                    }
                    try
                    {
                        COM3.Write(actualCMD, 0, actualCMD.Length);
                    }
                    catch (Exception error)
                    {
                        Debug.WriteLine("Error Sending String to COM3: " + error.ToString());
                    }
                }
                if (selectedCOM == "COM4")
                {
                    if (!COM4.IsOpen)
                    {
                        COM4.Open();
                        COM4.DataReceived += Com4Received;
                        COM4.ErrorReceived += Com4Error;
                    }
                    try
                    {
                        COM4.Write(actualCMD, 0, actualCMD.Length);
                    }
                    catch (Exception error)
                    {
                        Debug.WriteLine("Error Sending String to COM4: " + error.ToString());
                    }
                }
                if (selectedCOM == "COM5")
                {
                    if (!COM5.IsOpen)
                    {
                        COM5.Open();
                        COM5.DataReceived += Com5Received;
                        COM5.ErrorReceived += Com5Error;
                    }
                    try
                    {
                        COM5.Write(actualCMD, 0, actualCMD.Length);
                    }
                    catch (Exception error)
                    {
                        Debug.WriteLine("Error Sending String to COM5: " + error.ToString());
                    }
                }
                if (selectedCOM == "COM6")
                {
                    if (!COM6.IsOpen)
                    {
                        COM6.Open();
                        COM6.DataReceived += Com6Received;
                        COM6.ErrorReceived += Com6Error;
                    }
                    try
                    {
                        COM6.Write(actualCMD, 0, actualCMD.Length);
                    }
                    catch (Exception error)
                    {
                        Debug.WriteLine("Error Sending String to COM6: " + error.ToString());
                    }
                }
                if (selectedCOM == "COM7")
                {
                    if (!COM7.IsOpen)
                    {
                        COM7.Open();
                        COM7.DataReceived += Com7Received;
                        COM7.ErrorReceived += Com7Error;
                    }
                    try
                    {
                        COM7.Write(actualCMD, 0, actualCMD.Length);
                    }
                    catch (Exception error)
                    {
                        Debug.WriteLine("Error Sending String to COM7: " + error.ToString());
                    }
                }
                if (selectedCOM == "COM8")
                {
                    if (!COM8.IsOpen)
                    {
                        COM8.Open();
                        COM8.DataReceived += Com8Received;
                        COM8.ErrorReceived += Com8Error;
                    }
                    try
                    {
                        COM8.Write(actualCMD, 0, actualCMD.Length);
                    }
                    catch (Exception error)
                    {
                        Debug.WriteLine("Error Sending String to COM8: " + error.ToString());
                    }
                }
                if (selectedCOM == "COM9")
                {
                    if (!COM9.IsOpen)
                    {
                        COM9.Open();
                        COM9.DataReceived += Com9Received;
                        COM9.ErrorReceived += Com9Error;
                    }
                    try
                    {
                        COM9.Write(actualCMD, 0, actualCMD.Length);
                    }
                    catch (Exception error)
                    {
                        Debug.WriteLine("Error Sending String to COM9: " + error.ToString());
                    }
                }
                SetText2(string.Format("[{1}] {0}\r\n", selectedCMD, selectedCOM));
            }
        }

        // Function so that we can remotely send through Commands from the Application (from Server etc)
        public static void RemoteSend(string selectedCOM, string selectedCMD)
        {
            if (selectedCOM != "COM1" && selectedCOM != "COM2" && selectedCOM != "COM3" && selectedCOM != "COM4" && selectedCOM != "COM5" && selectedCOM != "COM6" && selectedCOM != "COM7" && selectedCOM != "COM8" && selectedCOM != "COM9") { 
                return;
            }
            int i = 0;
            int x = 0;

            if (selectedCOM == "COM1") { i = 0; }
            if (selectedCOM == "COM2") { i = 1; }
            if (selectedCOM == "COM3") { i = 2; }
            if (selectedCOM == "COM4") { i = 3; }
            if (selectedCOM == "COM5") { i = 4; }
            if (selectedCOM == "COM6") { i = 5; }
            if (selectedCOM == "COM7") { i = 6; }
            if (selectedCOM == "COM8") { i = 7; }
            if (selectedCOM == "COM9") { i = 8; }

            if (selectedCMD == "POWERON") { x = 0; }                    // Power On
            if (selectedCMD == "POWEROFF") { x = 1; }                   // Power Off
            if (selectedCMD == "POWERSTATUS") { x = 2; }                // Power Status
            if (selectedCMD == "VOLUP") { x = 3; }                      // Volume Up
            if (selectedCMD == "VOLDWN") { x = 4; }                     // Volume Down
            if (selectedCMD == "VOLSTATUS") { x = 5; }                  // Volume Status
            if (selectedCMD == "CHANHDMI1") { x = 6; }                  // Change Channel - HDMI1
            if (selectedCMD == "CHANHDMI2") { x = 7; }                  // Change Channel - HDMI2
            if (selectedCMD == "CHANSTATUS") { x = 8; }                 // Channel Status
            if (selectedCMD == "IRDISABLE") { x = 9; }                  // Infrared Disable
            if (selectedCMD == "IRENABLE") { x = 10; }                  // Infrared Enable
            if (selectedCMD == "IRSTATUS") { x = 11; }                  // Ingrared Status
            if (selectedCMD == "BRIGHTUP") { x = 12; }                  // Brightness Up
            if (selectedCMD == "BRIGHTDWN") { x = 13; }                 // Brightness Down
            if (selectedCMD == "BRIGHTSTATUS") { x = 14; }              // Brightness Status
            if (selectedCMD == "CONTRASTUP") { x = 15; }                // Contrast Up
            if (selectedCMD == "CONTRASTDWN") { x = 16; }               // Contrast Down
            if (selectedCMD == "CONTRASTSTATUS") { x = 17; }            // Contrast Status

            var selectedConfig = FrmObj.listBox1.Items[i].SubItems[7].Text;
            var configArr = selectedConfig.Split('!');

            var theActualCMD = configArr[x].ToString().ToUpper();
            if (selectedCMD != "" && theActualCMD != "NONE")
            {
                byte[] actualCMD = HexStringConverter.ToByteArray(theActualCMD);
                if (selectedCOM == "COM1")
                {
                    if (!COM1.IsOpen)
                    {
                        COM1.Open();
                        COM1.DataReceived += FrmObj.Com1Received;
                        COM1.ErrorReceived += FrmObj.Com1Error;
                    }
                    try
                    {
                        COM1.Write(actualCMD, 0, actualCMD.Length);
                    }
                    catch (Exception error)
                    {
                        Debug.WriteLine("Error Sending String to COM1: " + error.ToString());
                    }
                }
                if (selectedCOM == "COM2")
                {
                    if (!COM2.IsOpen)
                    {
                        COM2.Open();
                        COM2.DataReceived += FrmObj.Com2Received;
                        COM2.ErrorReceived += FrmObj.Com2Error;
                    }
                    try
                    {
                        COM2.Write(actualCMD, 0, actualCMD.Length);
                    }
                    catch (Exception error)
                    {
                        Debug.WriteLine("Error Sending String to COM2: " + error.ToString());
                    }
                }
                if (selectedCOM == "COM3")
                {
                    if (!COM3.IsOpen)
                    {
                        COM3.Open();
                        COM3.DataReceived += FrmObj.Com3Received;
                        COM3.ErrorReceived += FrmObj.Com3Error;
                    }
                    try
                    {
                        COM3.Write(actualCMD, 0, actualCMD.Length);
                    }
                    catch (Exception error)
                    {
                        Debug.WriteLine("Error Sending String to COM3: " + error.ToString());
                    }
                }
                if (selectedCOM == "COM4")
                {
                    if (!COM4.IsOpen)
                    {
                        COM4.Open();
                        COM4.DataReceived += FrmObj.Com4Received;
                        COM4.ErrorReceived += FrmObj.Com4Error;
                    }
                    try
                    {
                        COM4.Write(actualCMD, 0, actualCMD.Length);
                    }
                    catch (Exception error)
                    {
                        Debug.WriteLine("Error Sending String to COM4: " + error.ToString());
                    }
                }
                if (selectedCOM == "COM5")
                {
                    if (!COM5.IsOpen)
                    {
                        COM5.Open();
                        COM5.DataReceived += FrmObj.Com5Received;
                        COM5.ErrorReceived += FrmObj.Com5Error;
                    }
                    try
                    {
                        COM5.Write(actualCMD, 0, actualCMD.Length);
                    }
                    catch (Exception error)
                    {
                        Debug.WriteLine("Error Sending String to COM5: " + error.ToString());
                    }
                }
                if (selectedCOM == "COM6")
                {
                    if (!COM6.IsOpen)
                    {
                        COM6.Open();
                        COM6.DataReceived += FrmObj.Com6Received;
                        COM6.ErrorReceived += FrmObj.Com6Error;
                    }
                    try
                    {
                        COM6.Write(actualCMD, 0, actualCMD.Length);
                    }
                    catch (Exception error)
                    {
                        Debug.WriteLine("Error Sending String to COM6: " + error.ToString());
                    }
                }
                if (selectedCOM == "COM7")
                {
                    if (!COM7.IsOpen)
                    {
                        COM7.Open();
                        COM7.DataReceived += FrmObj.Com7Received;
                        COM7.ErrorReceived += FrmObj.Com7Error;
                    }
                    try
                    {
                        COM7.Write(actualCMD, 0, actualCMD.Length);
                    }
                    catch (Exception error)
                    {
                        Debug.WriteLine("Error Sending String to COM7: " + error.ToString());
                    }
                }
                if (selectedCOM == "COM8")
                {
                    if (!COM8.IsOpen)
                    {
                        COM8.Open();
                        COM8.DataReceived += FrmObj.Com8Received;
                        COM8.ErrorReceived += FrmObj.Com8Error;
                    }
                    try
                    {
                        COM8.Write(actualCMD, 0, actualCMD.Length);
                    }
                    catch (Exception error)
                    {
                        Debug.WriteLine("Error Sending String to COM8: " + error.ToString());
                    }
                }
                if (selectedCOM == "COM9")
                {
                    if (!COM9.IsOpen)
                    {
                        COM9.Open();
                        COM9.DataReceived += FrmObj.Com9Received;
                        COM9.ErrorReceived += FrmObj.Com9Error;
                    }
                    try
                    {
                        COM9.Write(actualCMD, 0, actualCMD.Length);
                    }
                    catch (Exception error)
                    {
                        Debug.WriteLine("Error Sending String to COM9: " + error.ToString());
                    }
                }
                FrmObj.SetText2(string.Format("[{1}] {2} - {0}\r\n", theActualCMD, selectedCOM, selectedCMD));
            }
        }

        // Functions
        private void Com1Received(object sender, SerialDataReceivedEventArgs e)
        {
            if (!COM1.IsOpen) return;            // This stops any IO Buffer Errors
            SetText(COM1.ReadExisting(), "COM1");
        }
        private void Com1Error(object sender, SerialErrorReceivedEventArgs e)
        {
            if (!COM1.IsOpen) return;            // This stops any IO Buffer Errors
        }
        private void Com2Received(object sender, SerialDataReceivedEventArgs e)
        {
            if (!COM2.IsOpen) return;            // This stops any IO Buffer Errors
            SetText(COM2.ReadExisting(), "COM2");
        }
        private void Com2Error(object sender, SerialErrorReceivedEventArgs e)
        {
            if (!COM2.IsOpen) return;            // This stops any IO Buffer Errors
        }
        private void Com3Received(object sender, SerialDataReceivedEventArgs e)
        {
            if (!COM3.IsOpen) return;            // This stops any IO Buffer Errors
            SetText(COM3.ReadExisting(), "COM3");
        }
        private void Com3Error(object sender, SerialErrorReceivedEventArgs e)
        {
            if (!COM3.IsOpen) return;            // This stops any IO Buffer Errors
        }
        private void Com4Received(object sender, SerialDataReceivedEventArgs e)
        {
            if (!COM4.IsOpen) return;            // This stops any IO Buffer Errors
            SetText(COM4.ReadExisting(), "COM4");
        }
        private void Com4Error(object sender, SerialErrorReceivedEventArgs e)
        {
            if (!COM4.IsOpen) return;            // This stops any IO Buffer Errors
        }
        private void Com5Received(object sender, SerialDataReceivedEventArgs e)
        {
            if (!COM5.IsOpen) return;            // This stops any IO Buffer Errors
            SetText(COM5.ReadExisting(), "COM5");
        }
        private void Com5Error(object sender, SerialErrorReceivedEventArgs e)
        {
            if (!COM5.IsOpen) return;            // This stops any IO Buffer Errors
        }
        private void Com6Received(object sender, SerialDataReceivedEventArgs e)
        {
            if (!COM6.IsOpen) return;            // This stops any IO Buffer Errors
            SetText(COM6.ReadExisting(), "COM6");
        }
        private void Com6Error(object sender, SerialErrorReceivedEventArgs e)
        {
            if (!COM6.IsOpen) return;            // This stops any IO Buffer Errors
        }
        private void Com7Received(object sender, SerialDataReceivedEventArgs e)
        {
            if (!COM7.IsOpen) return;            // This stops any IO Buffer Errors
            SetText(COM7.ReadExisting(), "COM7");
        }
        private void Com7Error(object sender, SerialErrorReceivedEventArgs e)
        {
            if (!COM7.IsOpen) return;            // This stops any IO Buffer Errors
        }
        private void Com8Received(object sender, SerialDataReceivedEventArgs e)
        {
            if (!COM8.IsOpen) return;            // This stops any IO Buffer Errors
            SetText(COM8.ReadExisting(), "COM8");
        }
        private void Com8Error(object sender, SerialErrorReceivedEventArgs e)
        {
            if (!COM8.IsOpen) return;            // This stops any IO Buffer Errors
        }
        private void Com9Received(object sender, SerialDataReceivedEventArgs e)
        {
            if (!COM9.IsOpen) return;            // This stops any IO Buffer Errors
            SetText(COM9.ReadExisting(), "COM9");
        }
        private void Com9Error(object sender, SerialErrorReceivedEventArgs e)
        {
            if (!COM9.IsOpen) return;            // This stops any IO Buffer Errors
        }

        private void SetText(string text, string whichCOM)
        {
            try
            {
                // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
                // If these threads are different, it returns true.
                if (txtData1.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(SetText);
                    Invoke(d, new object[] { text, whichCOM });
                }
                else
                {
                    txtData1.Text += text;
                    txtData1.SelectionStart = txtData1.TextLength;
                    txtData1.ScrollToCaret();
                }
            }
            catch { }
        }
        private void SetText2(string text)
        {
            try
            {
                // InvokeRequired required compares the thread ID of the calling thread to the thread ID of the creating thread.
                // If these threads are different, it returns true.
                if (txtData2.InvokeRequired)
                {
                    SetTextCallback2 d = new SetTextCallback2(SetText2);
                    Invoke(d, new object[] { text });
                }
                else
                {
                    txtData2.Text += text;
                    txtData2.SelectionStart = txtData2.TextLength;
                    txtData2.ScrollToCaret();
                }
            } catch { }
        }
        private void BackFillINI()
        {
            var ConfigINI = new IniFile(configFile);
            var maxSensors = 9;
            for (int i = 1; i <= maxSensors; i++)
            {
                string comPort = "COM" +i;
                var sensorData = ConfigINI.Read(i.ToString(), "Ports");
                if (sensorData == "")
                {
                    ConfigINI.Write("COM" + i.ToString(), "None", "Ports");
                }
            }
        }

        static class HexStringConverter
        {
            public static byte[] ToByteArray(string HexString)
            {
                int NumberChars = HexString.Length;
                byte[] bytes = new byte[NumberChars / 2];
                for (int i = 0; i < NumberChars; i += 2)
                {
                    bytes[i / 2] = Convert.ToByte(HexString.Substring(i, 2), 16);
                }
                return bytes;
            }
        }

    }
}
