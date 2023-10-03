using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace GlobalCMS
{
    public partial class OmronSensor : Form
    {
        WebServer Webserver2 = new WebServer(150);          // Start WebServer that gives us a way to remotely check env data on port 150

        private static DateTime UNIX_EPOCH = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private long tickStart_USB;
        private Log log_USB;
        private USB usb;
        private ComboBox comboBox_USB_LED_setting_normal_rule;
        private Label label_USB_LED_Col_HEX;
        private ComboBox comboBox_BLE_LED_setting_normal_rule;
        private Label label_BLE_LED_Col_HEX;
        private Label label_USB_FWRevision;
        private Label label_USB_HWRevision;
        private Label label_USB_MFGName;
        private Label label_USB_Memory_index_Last;
        private Label label_USB_Memory_index_Latest;
        private Label[] USB_monitor_labels;
        private ColorWheel colorWheel_USB_LED;
        private DataGridView dgv_USB_DeviceList;
        private RichTextBox rtb_USB_Log;
        private TabControl tabCtrl;
        private Button button_USB_Scan;
        private Button button_USB_Open;
        private Button button_USB_Close;
        public Sensor sensor_usb = new Sensor();
        private GroupBox gb_USB_DeviceInfo;
        private GroupBox gb_USB_Acceleration_Service;
        private GroupBox gb_USB_Latest_Data_Service;
        private GroupBox gb_USB_Memory_Data_Service;
        private GroupBox gb_USB_Control_Service;
        private GroupBox gb_USB_Event_Setting_Service;
        private GroupBox gb_USB_Information_Service;
        private GroupBox gb_USB_Time_Setting_Service;
        private GroupBox gb_USB_USB_Only_Service;
        private TabPage tabPage_BLE_Config;
        private TabPage tabPage_USB_Config;
        private static AppStatus appStatus;
        private Chart chart_USB;
        internal ComboBox comboBox_USB_xrange;
        private Label label_USB_TargetDevice;
        private string targetUSBComPort = "";

        public static string globalTempOverTrigger = "Off";
        public static string globalTempUnderTrigger = "Off";
        public static string globalLightOverTrigger = "Off";
        public static string globalLightUnderTrigger = "Off";
        public static string globalNoiseOverTrigger = "Off";

        private int tempOverCNT = 0;
        private int tempUnderCNT = 0;
        private int lightOverCNT = 0;
        private int lightUnderCNT = 0;
        private int noiseOverCNT = 0;

        // Timers Quiet Variables
        private bool quietTempOverTrigger = false;
        private bool quietTempUnderTrigger = false;
        private bool quietLightOverTrigger = false;
        private bool quietLightUnderTrigger = false;
        private bool quietNoiseOverTrigger = false;

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(
          HandleRef hWnd,
          int msg,
          IntPtr wParam,
          IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern SafeFileHandle CreateFile(string lpFileName, int dwDesiredAccess, int dwShareMode, IntPtr securityAttrs, int dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

        public static void BeginControlUpdate(Control control)
        {
            OmronSensor.SendMessage(new HandleRef((object)control, control.Handle), 11, IntPtr.Zero, IntPtr.Zero);
        }

        public static void EndControlUpdate(Control control)
        {
            OmronSensor.SendMessage(new HandleRef((object)control, control.Handle), 11, new IntPtr(1), IntPtr.Zero);
            control.Invalidate();
        }

        public struct AppStatus
        {
            public bool USBConfig_Opened;
            public bool USBMonitoring;
            public bool USBTransmission;
            public bool USBScanningDevice;
            public bool BLEConfig_Connected;
            public bool BLEMonitoring;
            public bool BLETransmission;
            public bool BLEScanningDevice;
        }

        public struct OmronData
        {
            public static string Temp = "00.00 c";
            public static string Humidity = "00.00 rh";
            public static string Light = "0 lx";
            public static string Pressure = "0000.000 hPa";
            public static string Noise = "00.00 dB";
            public static string eTVOC = "0 ppb";
            public static string eCO2 = "0 ppm";
            public static string Vibrate_SI = "0.0 kine";
            public static string Vibrate_PGA = "0.0 gal";
            public static string Vibrate_SeismicIntensity = "0.000";
            public static string Vibrate_AccelerationX = "0.0 gal";
            public static string Vibrate_AccelerationY = "0.0 gal";
            public static string Vibrate_AccelerationZ = "0.0 gal";
            public static string HeatStrokeRisk = "0.00";
            public static string DiscomfortIndex = "0.00";
            public static string NlLabelTxt = "N/A";
            public static string DILabelTxt = "N/A";
            public static string HSILabelTxt = "N/A";
            public static string LlLabelTxt = "N/A";
            public static string eTVOCLabelTxt = "N/A";
            public static string eCO2LabelTxt = "N/A";
        }

        public OmronSensor()
        {
            InitializeComponent();

            // Start Trigger Timers to do auto websockets based on variables for each option
            StartTimers();
            usb = new USB(serialPort);
            comboBox_USB_LED_setting_normal_rule = new ComboBox();
            label_USB_LED_Col_HEX = new Label();
            comboBox_BLE_LED_setting_normal_rule = new ComboBox();
            label_BLE_LED_Col_HEX = new Label();
            label_USB_FWRevision = new Label();
            label_USB_HWRevision = new Label();
            label_USB_TargetDevice = new Label
            {
                Text = "COM10"
            };
            label_USB_MFGName = new Label();
            label_USB_Memory_index_Last = new Label();
            label_USB_Memory_index_Latest = new Label();
            USB_monitor_labels = new Label[15];
            colorWheel_USB_LED = new ColorWheel();
            dgv_USB_DeviceList = new DataGridView();
            rtb_USB_Log = new RichTextBox();
            tabCtrl = new TabControl();
            button_USB_Scan = new Button();
            button_USB_Open = new Button();
            button_USB_Close = new Button();

            gb_USB_DeviceInfo = new GroupBox();
            gb_USB_Acceleration_Service = new GroupBox();
            gb_USB_Latest_Data_Service = new GroupBox();
            gb_USB_Memory_Data_Service = new GroupBox();
            gb_USB_Control_Service = new GroupBox();
            gb_USB_Event_Setting_Service = new GroupBox();
            gb_USB_Information_Service = new GroupBox();
            gb_USB_Time_Setting_Service = new GroupBox();
            gb_USB_USB_Only_Service = new GroupBox();
            comboBox_USB_xrange = null;
            chart_USB = null; 

            tabPage_BLE_Config = new TabPage();
            tabPage_USB_Config = new TabPage();

            timer_Monitor_USB.Tick += new EventHandler(Timer_Monitor_USB_Tick);
            timer_USB.Tick += new EventHandler(Timer_USB_Tick);
            dgv_USB_DeviceList.RowEnter += new DataGridViewCellEventHandler(Dgv_USB_DeviceList_RowEnter);

            int num2 = USB_ScanDevices();
            if (num2 > 0)
            {
                try
                {
                    string port = label_USB_TargetDevice.Text.ToString().Trim();
                    if (usb.PortOpen(port))
                    {
                        appStatus.USBConfig_Opened = true;
                        ControlGUIComponents(appStatus);
                        USB_Device_information_Read();
                        Load += OmronSensor_Shown;
                    }
                }
                catch
                {
                    // Debug.WriteLine("Error opening port: {0}", ex.Message);
                }
            } else
            {
                // Debug.WriteLine("Error no device found");
            }
        }

        private void OmronSensor_Shown(object sender, EventArgs e)
        {
            // Trigger as soon as the form has opened, to click "Connect", and minimize the form itself
            ConnectBTN.PerformClick();
            MainForm.FrmObj.SensorOpt.Text = "Connected";
            WindowState = FormWindowState.Minimized;
        }

        private void ConnectBTN_Click(object sender, EventArgs e)
        {
            if (!OmronSensor.appStatus.USBMonitoring)
            {
                ConnectBTN.Text = "Stop";
                tickStart_USB = (long)Environment.TickCount;
                timer_USB.Interval = 1000;
                timer_USB.Start();
                timer_Monitor_USB.Interval = 100;
                timer_Monitor_USB.Start();
                OmronSensor.appStatus.USBMonitoring = true;
                ConnectStatus.Text = "Connected";
                ConnectStatus.ForeColor = Color.FromArgb(0, 192, 0);
                ConnectStatusUSB.ForeColor = Color.FromArgb(0, 192, 0);

                MainForm.FrmObj.SensorOpt.Text = "Connected";
            }
            else
            {
                timer_USB.Stop();
                timer_Monitor_USB.Stop();
                ConnectBTN.Text = "Connect";
                OmronSensor.appStatus.USBMonitoring = false;
                ConnectStatus.Text = "Disconnected";
                ConnectStatus.ForeColor = Color.FromArgb(192, 0, 0);
                ConnectStatusUSB.ForeColor = Color.FromArgb(192, 0, 0);

                label_USB_Mon_Temp.Text = "00.00 c";
                OmronData.Temp = "00.00 c";

                label_USB_Mon_Humi.Text = "00.00 rh";
                OmronData.Humidity = "00.00 rh";

                label_USB_Mon_Light.Text = "0 lx";
                OmronData.Light = "0 lx";

                label_USB_Mon_Press.Text = "0000.000 hPa";
                OmronData.Pressure = "0000.000 hPa";

                label_USB_Mon_Noise.Text = "00.00 dB";
                OmronData.Noise = "00.00 dB";

                label_USB_Mon_VOC.Text = "0 ppb";
                OmronData.eTVOC = "0 ppb";

                label_USB_Mon_CO2.Text = "0 ppm";
                OmronData.eCO2 = "0 ppm";

                label_USB_Mon_HSI.Text = "0.00";
                HSILabelTXT.Text = "N/A";
                label_USB_Mon_DI.Text = "0.00";
                DILabelTXT.Text = "N/A";

                label_USB_Mon_SI.Text = "0.0 kine";
                OmronData.Vibrate_SI = "0.0 kine";

                label_USB_Mon_PGA.Text = "0.0 gal";
                OmronData.Vibrate_PGA = "0.0 gal";

                label_USB_Mon_Shindo.Text = "0.000";
                OmronData.Vibrate_SeismicIntensity = "0.000";

                label_USB_Mon_AccelX.Text = "0.0 gal";
                OmronData.Vibrate_AccelerationX = "0.0 gal";

                label_USB_Mon_AccelY.Text = "0.0 gal";
                OmronData.Vibrate_AccelerationY = "0.0 gal";

                label_USB_Mon_AccelZ.Text = "0.0 gal";
                OmronData.Vibrate_AccelerationZ = "0.0 gal";

                NlLabelTxt.Text = "N/A";
                LlLabelTxt.Text = "N/A";
                eTVOCLabelTxt.Text = "N/A";
                eCO2LabelTxt.Text = "N/A";
                OmronData.eTVOCLabelTxt = "N/A";
                OmronData.eCO2LabelTxt = "N/A";
                OmronData.LlLabelTxt = "N/A";
                OmronData.NlLabelTxt = "N/A";
                OmronData.HSILabelTxt = "N/A";
                OmronData.DILabelTxt = "N/A";

                MainForm.FrmObj.SensorOpt.Text = "Inserted";
            }
        }

        private int USB_ScanDevices()
        {
            int index = 0;
            OmronSensor.appStatus.USBScanningDevice = true;
            this.USB_InitDeviceList();
            List<USB.VComPortInfo> virtualComPorts = this.usb.FindVirtualComPorts();
            foreach (USB.VComPortInfo vcomPortInfo in virtualComPorts)
            {
                this.dgv_USB_DeviceList.Rows.Add();
                this.dgv_USB_DeviceList[0, index].Value = (object)vcomPortInfo.PortName;
                this.dgv_USB_DeviceList[1, index].Value = (object)vcomPortInfo.Description.Substring(0, vcomPortInfo.Description.IndexOf("(COM"));
                this.dgv_USB_DeviceList[2, index].Value = (object)vcomPortInfo.Manufacturer;
                this.dgv_USB_DeviceList[4, index].Value = (object)("0x" + vcomPortInfo.Vid.ToString("X4"));
                this.dgv_USB_DeviceList[3, index].Value = (object)("0x" + vcomPortInfo.Pid.ToString("X4"));
                ++index;
            }
            OmronSensor.appStatus.USBScanningDevice = false;
            return virtualComPorts.Count;
        }

        private void USB_InitDeviceList()
        {
            this.dgv_USB_DeviceList.MultiSelect = false;
            this.dgv_USB_DeviceList.Rows.Clear();
            this.dgv_USB_DeviceList.Columns.Clear();
            this.dgv_USB_DeviceList.AllowUserToAddRows = false;
            this.dgv_USB_DeviceList.Columns.Add("C01", "Port");
            this.dgv_USB_DeviceList.Columns.Add("C02", "Description");
            this.dgv_USB_DeviceList.Columns.Add("C03", "Manufacurer");
            this.dgv_USB_DeviceList.Columns.Add("C04", "VID");
            this.dgv_USB_DeviceList.Columns.Add("C05", "PID");
            this.dgv_USB_DeviceList.AutoResizeColumnHeadersHeight();
            this.dgv_USB_DeviceList.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
            this.dgv_USB_DeviceList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dgv_USB_DeviceList.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgv_USB_DeviceList.RowHeadersVisible = false;
            this.dgv_USB_DeviceList.TopLeftHeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            foreach (DataGridViewColumn column in (BaseCollection)this.dgv_USB_DeviceList.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                column.ReadOnly = true;
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            this.dgv_USB_DeviceList.AllowUserToResizeColumns = false;
            this.dgv_USB_DeviceList.AllowUserToResizeRows = false;
        }

        public void USB_TxRx(USB.Frame requestFrame, ref USB.Frame responseFrame)
        {
            log_USB = null;
            int num = 0;
            byte[] rxBuff = new byte[512];
            ushort len = 0;
            this.usb.Send(requestFrame.frame);
            while (!this.usb.Receive(ref rxBuff, ref len, 3000))
            {
                this.usb.Send(requestFrame.frame);
                ++num;
                // this.log_USB.Write(Log.TYPE.TX, "Retry " + num.ToString(), true, true);
                if (num >= 3)
                {
                    // this.log_USB.Write(Log.TYPE.ERROR, "Retry Error", true, true);
                    responseFrame.errorFlag = true;
                    return;
                }
                Thread.Sleep(1);
            }
            responseFrame.ParsePacket(rxBuff, len);
        }

        public bool USB_TxRx_Memory(
          IProgress<int> progress,
          USB.Frame requestFrame,
          ref USB.Frame[] responseFrame,
          uint numIndex)
        {
            int num1 = 0;
            byte[] rxBuff = new byte[512];
            ushort len = 0;
            uint num2 = 0;
            this.usb.Send(requestFrame.frame);
            while (!this.usb.Receive(ref rxBuff, ref len, 3000))
            {
                this.usb.Send(requestFrame.frame);
                ++num1;
                // this.log_USB.Write(Log.TYPE.TX, "Retry " + num1.ToString(), true, true);
                if (num1 >= 3)
                {
                    // this.log_USB.Write(Log.TYPE.ERROR, "Retry Error", true, true);
                    return false;
                }
                Thread.Sleep(1);
            }
            USB.Frame[] frameArray = responseFrame;
            int index = (int)num2;
            uint num3 = (uint)(index + 1);
            frameArray[index].ParsePacket(rxBuff, len);
            while (num3 < numIndex)
            {
                if (!this.usb.Receive(ref rxBuff, ref len, 3000))
                {
                    // this.log_USB.Write(Log.TYPE.ERROR, "Data receive error", true, true);
                    return false;
                }
                responseFrame[(int)num3++].ParsePacket(rxBuff, len);
                progress.Report((int)((long)(num3 * 100U) / (long)responseFrame.Length));
            }
            progress.Report(100);
            return true;
        }

        private string CommonSetter_LED_setting_normal(
          ref byte[] data,
          OmronSensor.COMMUNICATION_INTERFACE comm_if)
        {
            string str = "";
            switch (comm_if)
            {
                case OmronSensor.COMMUNICATION_INTERFACE.USB:
                    data[0] = (byte)this.comboBox_USB_LED_setting_normal_rule.SelectedIndex;
                    byte[] numArray1 = data;
                    Color backColor1 = this.label_USB_LED_Col_HEX.BackColor;
                    int r1 = (int)backColor1.R;
                    numArray1[2] = (byte)r1;
                    byte[] numArray2 = data;
                    backColor1 = this.label_USB_LED_Col_HEX.BackColor;
                    int g1 = (int)backColor1.G;
                    numArray2[3] = (byte)g1;
                    byte[] numArray3 = data;
                    backColor1 = this.label_USB_LED_Col_HEX.BackColor;
                    int b1 = (int)backColor1.B;
                    numArray3[4] = (byte)b1;
                    str = this.comboBox_USB_LED_setting_normal_rule.Text;
                    break;
                case OmronSensor.COMMUNICATION_INTERFACE.BLE:
                    data[0] = (byte)this.comboBox_BLE_LED_setting_normal_rule.SelectedIndex;
                    byte[] numArray4 = data;
                    Color backColor2 = this.label_BLE_LED_Col_HEX.BackColor;
                    int r2 = (int)backColor2.R;
                    numArray4[2] = (byte)r2;
                    byte[] numArray5 = data;
                    backColor2 = this.label_BLE_LED_Col_HEX.BackColor;
                    int g2 = (int)backColor2.G;
                    numArray5[3] = (byte)g2;
                    byte[] numArray6 = data;
                    backColor2 = this.label_BLE_LED_Col_HEX.BackColor;
                    int b2 = (int)backColor2.B;
                    numArray6[4] = (byte)b2;
                    str = this.comboBox_BLE_LED_setting_normal_rule.Text;
                    break;
            }
            data[1] = (byte)0;
            return "" + " <LED setting (normal)>\n" + " Rule\t : 0x" + data[0].ToString("X4") + ": " + str + "\n" + " RGB\t : (" + data[2].ToString() + " : " + data[3].ToString() + " : " + data[4].ToString() + ")";
        }

        private void USB_LED_setting_normal_Write()
        {
            USB.Frame requestFrame = new USB.Frame();
            USB.Frame responseFrame = new USB.Frame();
            byte[] data = new byte[5];
            // this.log_USB.Write(Log.TYPE.TX, this.CommonSetter_LED_setting_normal(ref data, OmronSensor.COMMUNICATION_INTERFACE.USB), true, false);
            requestFrame.SetFrame(USB.COMMAND.WRITE, USB.ADDRESS.COMMON_LED_SETTING_NORMAL, data, (ushort)data.Length);
            this.USB_TxRx(requestFrame, ref responseFrame);
            // if (!responseFrame.errorFlag)
                // this.log_USB.Write(Log.TYPE.RX, this.CommonResultParser_LED_setting_normal(responseFrame.data, OmronSensor.COMMUNICATION_INTERFACE.USB), true, false);
            // else
                // this.log_USB.Write(Log.TYPE.ERROR, "Write error", true, false);
        }

        private void USB_LED_setting_normal_Read()
        {
            USB.Frame requestFrame = new USB.Frame();
            USB.Frame responseFrame = new USB.Frame();
            requestFrame.SetFrame(USB.COMMAND.READ, USB.ADDRESS.COMMON_LED_SETTING_NORMAL);
            this.USB_TxRx(requestFrame, ref responseFrame);
            byte[] data = responseFrame.data;
            // if (!responseFrame.errorFlag)
                // this.log_USB.Write(Log.TYPE.RX, this.CommonResultParser_LED_setting_normal(data, OmronSensor.COMMUNICATION_INTERFACE.USB), true, false);
            // else
                // this.log_USB.Write(Log.TYPE.ERROR, "Read error", true, false);
        }

        private void USB_Device_information_Read()
        {
            USB.Frame requestFrame = new USB.Frame();
            USB.Frame responseFrame = new USB.Frame();
            requestFrame.SetFrame(USB.COMMAND.READ, USB.ADDRESS.COMMON_DEVICE_INFO);
            this.USB_TxRx(requestFrame, ref responseFrame);
            this.label_USB_ModelNumber.Text = Encoding.UTF8.GetString(((IEnumerable<byte>)responseFrame.data).Skip<byte>(0).Take<byte>(10).ToArray<byte>());
            this.label_USB_SerialNumber.Text = Encoding.UTF8.GetString(((IEnumerable<byte>)responseFrame.data).Skip<byte>(10).Take<byte>(10).ToArray<byte>());
            this.label_USB_FWRevision.Text = Encoding.UTF8.GetString(((IEnumerable<byte>)responseFrame.data).Skip<byte>(20).Take<byte>(5).ToArray<byte>());
            this.label_USB_HWRevision.Text = Encoding.UTF8.GetString(((IEnumerable<byte>)responseFrame.data).Skip<byte>(25).Take<byte>(5).ToArray<byte>());
            this.label_USB_MFGName.Text = Encoding.UTF8.GetString(((IEnumerable<byte>)responseFrame.data).Skip<byte>(30).Take<byte>(5).ToArray<byte>());
            this.sensor_usb.serialString = this.label_USB_SerialNumber.Text;
        }

        private void USB_Latest_data_short_Read()
        {
            USB.Frame requestFrame = new USB.Frame();
            USB.Frame responseFrame = new USB.Frame();
            requestFrame.SetFrame(USB.COMMAND.READ, USB.ADDRESS.USB_ORG_LATEST_DATA_SHORT);
            this.USB_TxRx(requestFrame, ref responseFrame);
            if (!responseFrame.errorFlag)
            {
                byte[] data = responseFrame.data;
                // this.log_USB.Write(Log.TYPE.RX, "" + "<Latest data (short)>\n" + " Sequence No.\t\t : " + data[0].ToString("D") + "\n" + " Temperature\t\t : " + ((double)(short)((int)data[2] << 8 | (int)data[1]) / 100.0).ToString("f2") + " degC\n" + " Humidity\t\t : " + ((double)(short)((int)data[4] << 8 | (int)data[3]) / 100.0).ToString("f2") + " %RH\n" + " Light \t\t\t : " + ((short)((int)data[6] << 8 | (int)data[5])).ToString("D") + " lx\n" + " Pressure\t\t : " + ((double)((int)data[10] << 24 | (int)data[9] << 16 | (int)data[8] << 8 | (int)data[7]) / 1000.0).ToString("f3") + " hPa\n" + " Noise\t\t\t : " + ((double)(short)((int)data[12] << 8 | (int)data[11]) / 100.0).ToString("f2") + " dB\n" + " eTVOC\t\t\t : " + ((short)((int)data[14] << 8 | (int)data[13])).ToString("D") + " ppb\n" + " eCO2\t\t\t : " + ((short)((int)data[16] << 8 | (int)data[15])).ToString("D") + " ppm\n" + " Discomfort Index\t\t : " + ((double)(short)((int)data[18] << 8 | (int)data[17]) / 100.0).ToString("f2") + "\n" + " Heat Stroke Risk Factor\t : " + ((double)(short)((int)data[20] << 8 | (int)data[19]) / 100.0).ToString("f2"), true, false);
            }
            // else
                // this.log_USB.Write(Log.TYPE.ERROR, "Read error", true, false);
        }

        private void USB_Latest_data_long_Read()
        {
            USB.Frame requestFrame = new USB.Frame();
            USB.Frame responseFrame = new USB.Frame();
            requestFrame.SetFrame(USB.COMMAND.READ, USB.ADDRESS.USB_ORG_LATEST_DATA_LONG);
            this.USB_TxRx(requestFrame, ref responseFrame);
            if (!responseFrame.errorFlag)
            {
                byte[] data = responseFrame.data;
                byte num1 = data[0];
                double num2 = (double)(short)((int)data[2] << 8 | (int)data[1]) / 100.0;
                double num3 = (double)(short)((int)data[4] << 8 | (int)data[3]) / 100.0;
                short num4 = (short)((int)data[6] << 8 | (int)data[5]);
                double num5 = (double)((int)data[10] << 24 | (int)data[9] << 16 | (int)data[8] << 8 | (int)data[7]) / 1000.0;
                double num6 = (double)(short)((int)data[12] << 8 | (int)data[11]) / 100.0;
                short num7 = (short)((int)data[14] << 8 | (int)data[13]);
                short num8 = (short)((int)data[16] << 8 | (int)data[15]);
                double num9 = (double)(short)((int)data[18] << 8 | (int)data[17]) / 100.0;
                double num10 = (double)(short)((int)data[20] << 8 | (int)data[19]) / 100.0;
                string str1 = "";
                switch (data[21])
                {
                    case 0:
                        str1 = "None";
                        break;
                    case 1:
                        str1 = "Vibration";
                        break;
                    case 2:
                        str1 = "Earthquake";
                        break;
                }
                double num11 = (double)(ushort)((uint)data[23] << 8 | (uint)data[22]) / 10.0;
                double num12 = (double)(ushort)((uint)data[25] << 8 | (uint)data[24]) / 10.0;
                double num13 = (double)(ushort)((uint)data[27] << 8 | (uint)data[26]) / 1000.0;
                string message = "" + "<Latest data (Long)>\n" + " Sequence No.\t\t : " + num1.ToString("D") + "\n" + " Temperature\t\t : " + num2.ToString("f2") + " degC\n" + " Humidity\t\t : " + num3.ToString("f2") + " %RH\n" + " Light \t\t\t : " + num4.ToString("D") + " lx\n" + " Pressure\t\t : " + num5.ToString("f3") + " hPa\n" + " Noise\t\t\t : " + num6.ToString("f2") + " dB\n" + " eTVOC\t\t\t : " + num7.ToString("D") + " ppb\n" + " eCO2\t\t\t : " + num8.ToString("D") + " ppm\n" + " Discomfort Index\t\t : " + num9.ToString("f2") + "\n" + " Heat Stroke Risk Factor\t : " + num10.ToString("f2") + "\n" + "\n" + "<Vibration>\n" + " Status\t\t\t : " + str1 + "\n" + " SI\t\t\t : " + num11.ToString("f1") + " kine\n" + " PGA\t\t\t : " + num12.ToString("f1") + " gal\n" + " Seismic Intensity\t : " + num13.ToString("f3") + "\n";
                bool boolean;
                for (int index = 0; index < 9; ++index)
                {
                    ushort num14 = (ushort)((uint)data[29 + 2 * index] << 8 | (uint)data[28 + 2 * index]);
                    string str2 = message + "\n" + "<" + OmronSensor.sensors[index] + ">\n";
                    boolean = Convert.ToBoolean((int)num14 & 1);
                    string str3 = boolean.ToString();
                    string str4 = str2 + " Simple [upper limit] 1 \t: " + str3 + "\n";
                    boolean = Convert.ToBoolean((int)num14 & 2);
                    string str5 = boolean.ToString();
                    string str6 = str4 + " Simple [upper limit] 2 \t: " + str5 + "\n";
                    boolean = Convert.ToBoolean((int)num14 & 4);
                    string str7 = boolean.ToString();
                    string str8 = str6 + " Simple [lower limit] 1 \t: " + str7 + "\n";
                    boolean = Convert.ToBoolean((int)num14 & 8);
                    string str9 = boolean.ToString();
                    string str10 = str8 + " Simple [lower limit] 2 \t: " + str9 + "\n";
                    boolean = Convert.ToBoolean((int)num14 & 16);
                    string str11 = boolean.ToString();
                    string str12 = str10 + " Change [rise/prev.] 1 \t: " + str11 + "\n";
                    boolean = Convert.ToBoolean((int)num14 & 32);
                    string str13 = boolean.ToString();
                    string str14 = str12 + " Change [rise/prev.] 2 \t: " + str13 + "\n";
                    boolean = Convert.ToBoolean((int)num14 & 64);
                    string str15 = boolean.ToString();
                    string str16 = str14 + " Change [decline/prev.] 1 \t: " + str15 + "\n";
                    boolean = Convert.ToBoolean((int)num14 & 128);
                    string str17 = boolean.ToString();
                    string str18 = str16 + " Change [decline/prev.] 2 \t: " + str17 + "\n";
                    boolean = Convert.ToBoolean((int)num14 & 256);
                    string str19 = boolean.ToString();
                    string str20 = str18 + " Average [upper] \t\t: " + str19 + "\n";
                    boolean = Convert.ToBoolean((int)num14 & 512);
                    string str21 = boolean.ToString();
                    string str22 = str20 + " Average [lower] \t\t: " + str21 + "\n";
                    boolean = Convert.ToBoolean((int)num14 & 1024);
                    string str23 = boolean.ToString();
                    string str24 = str22 + " Peak-to-Peak [upper] \t: " + str23 + "\n";
                    boolean = Convert.ToBoolean((int)num14 & 2048);
                    string str25 = boolean.ToString();
                    string str26 = str24 + " Peak-to-Peak [lower] \t: " + str25 + "\n";
                    boolean = Convert.ToBoolean((int)num14 & 4096);
                    string str27 = boolean.ToString();
                    string str28 = str26 + " Interval [rise/term] \t: " + str27 + "\n";
                    boolean = Convert.ToBoolean((int)num14 & 8192);
                    string str29 = boolean.ToString();
                    string str30 = str28 + " Interval [decline/term] \t: " + str29 + "\n";
                    boolean = Convert.ToBoolean((int)num14 & 16384);
                    string str31 = boolean.ToString();
                    string str32 = str30 + " Base [upper] \t\t: " + str31 + "\n";
                    boolean = Convert.ToBoolean((int)num14 & 32768);
                    string str33 = boolean.ToString();
                    message = str32 + " Base [lower] \t\t: " + str33;
                }
                for (int index = 0; index < 3; ++index)
                {
                    string str2 = message + "\n\n" + "<" + OmronSensor.accel_sensors[index] + ">\n";
                    boolean = Convert.ToBoolean((int)data[46 + index] & 1);
                    string str3 = boolean.ToString();
                    string str4 = str2 + " Simple [upper limit] 1 \t: " + str3 + "\n";
                    boolean = Convert.ToBoolean((int)data[46 + index] & 2);
                    string str5 = boolean.ToString();
                    string str6 = str4 + " Simple [upper limit] 2 \t: " + str5 + "\n";
                    boolean = Convert.ToBoolean((int)data[46 + index] & 16);
                    string str7 = boolean.ToString();
                    string str8 = str6 + " Change [rise/prev.] 1 \t: " + str7 + "\n";
                    boolean = Convert.ToBoolean((int)data[46 + index] & 32);
                    string str9 = boolean.ToString();
                    message = str8 + " Change [rise/prev.] 2 \t: " + str9;
                }
                // this.log_USB.Write(Log.TYPE.RX, message, true, false);
            }
            // else
                // this.log_USB.Write(Log.TYPE.ERROR, "Read error", true, false);
        }

        private string CommonResultParser_Latest_sensing_data(byte[] data)
        {
            return "" + "<Latest sensing data>\n" + " Sequence No.\t\t : " + data[0].ToString("D") + "\n" + " Temperature\t\t : " + ((double)(short)((int)data[2] << 8 | (int)data[1]) / 100.0).ToString("f2") + " degC\n" + " Humidity\t\t : " + ((double)(short)((int)data[4] << 8 | (int)data[3]) / 100.0).ToString("f2") + " %RH\n" + " Light \t\t\t : " + ((short)((int)data[6] << 8 | (int)data[5])).ToString("D") + " lx\n" + " Pressure\t\t : " + ((double)((int)data[10] << 24 | (int)data[9] << 16 | (int)data[8] << 8 | (int)data[7]) / 1000.0).ToString("f3") + " hPa\n" + " Noise\t\t\t : " + ((double)(short)((int)data[12] << 8 | (int)data[11]) / 100.0).ToString("f2") + " dB\n" + " eTVOC\t\t\t : " + ((short)((int)data[14] << 8 | (int)data[13])).ToString("D") + " ppb\n" + " eCO2\t\t\t : " + ((short)((int)data[16] << 8 | (int)data[15])).ToString("D") + " ppm";
        }

        private void USB_Latest_memory_info_Read()
        {
            USB.Frame requestFrame = new USB.Frame();
            USB.Frame responseFrame = new USB.Frame();
            requestFrame.SetFrame(USB.COMMAND.READ, USB.ADDRESS.COMMON_LATEST_MEMORY_INFO);
            this.USB_TxRx(requestFrame, ref responseFrame);
            byte[] data = responseFrame.data;
            // if (!responseFrame.errorFlag)
                // this.log_USB.Write(Log.TYPE.RX, this.CommonResultParser_Latest_memory_info(data, OmronSensor.COMMUNICATION_INTERFACE.USB), true, false);
            // else
                // this.log_USB.Write(Log.TYPE.ERROR, "Read error", true, false);
        }

        private string CommonResultParser_Latest_calc_data(byte[] data)
        {
            byte num1 = data[0];
            double num2 = (double)(short)((int)data[2] << 8 | (int)data[1]) / 100.0;
            double num3 = (double)(short)((int)data[4] << 8 | (int)data[3]) / 100.0;
            string str = "";
            switch (data[5])
            {
                case 0:
                    str = "None";
                    break;
                case 1:
                    str = "Vibration";
                    break;
                case 2:
                    str = "Earthquake";
                    break;
            }
            double num4 = (double)(ushort)((uint)data[7] << 8 | (uint)data[6]) / 10.0;
            double num5 = (double)(ushort)((uint)data[9] << 8 | (uint)data[8]) / 10.0;
            double num6 = (double)(ushort)((uint)data[11] << 8 | (uint)data[10]) / 1000.0;
            double num7 = (double)(short)((int)data[13] << 8 | (int)data[12]) / 10.0;
            double num8 = (double)(short)((int)data[15] << 8 | (int)data[14]) / 10.0;
            double num9 = (double)(short)((int)data[17] << 8 | (int)data[16]) / 10.0;
            // Debug.WriteLine("" + "<Latest claculation data>\n" + " Sequence No.\t\t : " + num1.ToString("D") + "\n" + " Discomfort Index\t\t : " + num2.ToString("f2") + "\n" + " Heat Stroke Risk Factor\t : " + num3.ToString("f2") + "\n" + "\n" + "<Vibration>\n" + " Status\t\t\t : " + str + "\n" + " SI\t\t\t : " + num4.ToString("f1") + " kine\n" + " PGA\t\t\t : " + num5.ToString("f1") + " gal\n" + " Seismic Intensity\t : " + num6.ToString("f3") + "\n" + " Acceleration X\t\t : " + num7.ToString("f1") + "\n" + " Acceleration Y\t\t : " + num8.ToString("f1") + "\n" + " Acceleration Z\t\t : " + num9.ToString("f1"));
            return "" + "<Latest claculation data>\n" + " Sequence No.\t\t : " + num1.ToString("D") + "\n" + " Discomfort Index\t\t : " + num2.ToString("f2") + "\n" + " Heat Stroke Risk Factor\t : " + num3.ToString("f2") + "\n" + "\n" + "<Vibration>\n" + " Status\t\t\t : " + str + "\n" + " SI\t\t\t : " + num4.ToString("f1") + " kine\n" + " PGA\t\t\t : " + num5.ToString("f1") + " gal\n" + " Seismic Intensity\t : " + num6.ToString("f3") + "\n" + " Acceleration X\t\t : " + num7.ToString("f1") + "\n" + " Acceleration Y\t\t : " + num8.ToString("f1") + "\n" + " Acceleration Z\t\t : " + num9.ToString("f1");
        }

        private void CommonResultParser_HeatRatings(byte[] data)
        {
            byte num1 = data[0];
            double num2 = (double)(short)((int)data[2] << 8 | (int)data[1]) / 100.0;
            double num3 = (double)(short)((int)data[4] << 8 | (int)data[3]) / 100.0;
            switch (data[5])
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
            }
            double num4 = (double)(ushort)((uint)data[7] << 8 | (uint)data[6]) / 10.0;
            double num5 = (double)(ushort)((uint)data[9] << 8 | (uint)data[8]) / 10.0;
            double num6 = (double)(ushort)((uint)data[11] << 8 | (uint)data[10]) / 1000.0;
            double num7 = (double)(short)((int)data[13] << 8 | (int)data[12]) / 10.0;
            double num8 = (double)(short)((int)data[15] << 8 | (int)data[14]) / 10.0;
            double num9 = (double)(short)((int)data[17] << 8 | (int)data[16]) / 10.0;
            // Debug.WriteLine("" + " Sequence No.\t\t : " + num1.ToString("D") + "\n" + " Discomfort Index\t\t : " + num2.ToString("f2") + "\n" + " Heat Stroke Risk Factor\t : " + num3.ToString("f2"));

            label_USB_Mon_DI.Text = num2.ToString("f2");
            DILabelTXT.Text = Convert.ToDouble(num2.ToString("f2")) < 85.0 ? (Convert.ToDouble(num2.ToString("f2")) < 80.0 ? (Convert.ToDouble(num2.ToString("f2")) < 75.0 ? (Convert.ToDouble(num2.ToString("f2")) < 70.0 ? (Convert.ToDouble(num2.ToString("f2")) < 65.0 ? (Convert.ToDouble(num2.ToString("f2")) < 60.0 ? (Convert.ToDouble(num2.ToString("f2")) < 55.0 ? (string)"Extremely Cold" : (string)"Cold") : (string)"Neutral") : (string)"Comfortable") : (string)"Slightly Warm") : (string)"Warm") : (string)"Hot") :  (string)"Extremely Hot";
            OmronData.DiscomfortIndex = num2.ToString("f2");
            OmronData.DILabelTxt = Convert.ToDouble(num2.ToString("f2")) < 85.0 ? (Convert.ToDouble(num2.ToString("f2")) < 80.0 ? (Convert.ToDouble(num2.ToString("f2")) < 75.0 ? (Convert.ToDouble(num2.ToString("f2")) < 70.0 ? (Convert.ToDouble(num2.ToString("f2")) < 65.0 ? (Convert.ToDouble(num2.ToString("f2")) < 60.0 ? (Convert.ToDouble(num2.ToString("f2")) < 55.0 ? (string)"Extremely Cold" : (string)"Cold") : (string)"Neutral") : (string)"Comfortable") : (string)"Slightly Warm") : (string)"Warm") : (string)"Hot") : (string)"Extremely Hot";

            label_USB_Mon_HSI.Text = num3.ToString("f2");
            HSILabelTXT.Text = Convert.ToDouble(num3.ToString("f2")) < 31.0 ? (Convert.ToDouble(num3.ToString("f2")) < 28.0 ? (Convert.ToDouble(num3.ToString("f2")) < 25.0 ? (Convert.ToDouble(num3.ToString("f2")) < 22.0 ? (string)"Safe" : (string)"Caution") : (string)"Warning") : (string)"Severe Warning") : (string)"Danger";
            OmronData.HeatStrokeRisk = num3.ToString("f2");
            OmronData.HSILabelTxt = Convert.ToDouble(num3.ToString("f2")) < 31.0 ? (Convert.ToDouble(num3.ToString("f2")) < 28.0 ? (Convert.ToDouble(num3.ToString("f2")) < 25.0 ? (Convert.ToDouble(num3.ToString("f2")) < 22.0 ? (string)"Safe" : (string)"Caution") : (string)"Warning") : (string)"Severe Warning") : (string)"Danger";
        }

        private string CommonResultParser_Latest_sensing_flag(byte[] data)
        {
            string str1 = "" + "<Latest sensing flag>\n" + " Sequence No.\t\t : " + data[0].ToString("D");
            for (int index = 0; index < 7; ++index)
            {
                ushort num = (ushort)((uint)data[2 + 2 * index] << 8 | (uint)data[1 + 2 * index]);
                string str2 = str1 + "\n\n" + "<" + OmronSensor.sensors[index] + ">\n";
                bool boolean = Convert.ToBoolean((int)num & 1);
                string str3 = boolean.ToString();
                string str4 = str2 + " Simple [upper limit] 1 \t: " + str3 + "\n";
                boolean = Convert.ToBoolean((int)num & 2);
                string str5 = boolean.ToString();
                string str6 = str4 + " Simple [upper limit] 2 \t: " + str5 + "\n";
                boolean = Convert.ToBoolean((int)num & 4);
                string str7 = boolean.ToString();
                string str8 = str6 + " Simple [lower limit] 1 \t: " + str7 + "\n";
                boolean = Convert.ToBoolean((int)num & 8);
                string str9 = boolean.ToString();
                string str10 = str8 + " Simple [lower limit] 2 \t: " + str9 + "\n";
                boolean = Convert.ToBoolean((int)num & 16);
                string str11 = boolean.ToString();
                string str12 = str10 + " Change [rise/prev.] 1 \t: " + str11 + "\n";
                boolean = Convert.ToBoolean((int)num & 32);
                string str13 = boolean.ToString();
                string str14 = str12 + " Change [rise/prev.] 2 \t: " + str13 + "\n";
                boolean = Convert.ToBoolean((int)num & 64);
                string str15 = boolean.ToString();
                string str16 = str14 + " Change [decline/prev.] 1 \t: " + str15 + "\n";
                boolean = Convert.ToBoolean((int)num & 128);
                string str17 = boolean.ToString();
                string str18 = str16 + " Change [decline/prev.] 2 \t: " + str17 + "\n";
                boolean = Convert.ToBoolean((int)num & 256);
                string str19 = boolean.ToString();
                string str20 = str18 + " Average [upper] \t\t: " + str19 + "\n";
                boolean = Convert.ToBoolean((int)num & 512);
                string str21 = boolean.ToString();
                string str22 = str20 + " Average [lower] \t\t: " + str21 + "\n";
                boolean = Convert.ToBoolean((int)num & 1024);
                string str23 = boolean.ToString();
                string str24 = str22 + " Peak-to-Peak [upper] \t: " + str23 + "\n";
                boolean = Convert.ToBoolean((int)num & 2048);
                string str25 = boolean.ToString();
                string str26 = str24 + " Peak-to-Peak [lower] \t: " + str25 + "\n";
                boolean = Convert.ToBoolean((int)num & 4096);
                string str27 = boolean.ToString();
                string str28 = str26 + " Interval [rise/term] \t: " + str27 + "\n";
                boolean = Convert.ToBoolean((int)num & 8192);
                string str29 = boolean.ToString();
                string str30 = str28 + " Interval [decline/term] \t: " + str29 + "\n";
                boolean = Convert.ToBoolean((int)num & 16384);
                string str31 = boolean.ToString();
                string str32 = str30 + " Base [upper] \t\t: " + str31 + "\n";
                boolean = Convert.ToBoolean((int)num & 32768);
                string str33 = boolean.ToString();
                str1 = str32 + " Base [lower] \t\t: " + str33;
            }
            return str1;
        }

        private string CommonResultParser_Latest_calc_flag(byte[] data)
        {
            string str1 = "" + "<Latest calculation flag>\n" + " Sequence No.\t\t : " + data[0].ToString("D");
            bool boolean;
            for (int index = 0; index < 2; ++index)
            {
                ushort num = (ushort)((uint)data[2 + 2 * index] << 8 | (uint)data[1 + 2 * index]);
                string str2 = str1 + "\n\n<" + OmronSensor.sensors[index + 7] + ">\n";
                boolean = Convert.ToBoolean((int)num & 1);
                string str3 = boolean.ToString();
                string str4 = str2 + " Simple [upper limit] 1 \t: " + str3 + "\n";
                boolean = Convert.ToBoolean((int)num & 2);
                string str5 = boolean.ToString();
                string str6 = str4 + " Simple [upper limit] 2 \t: " + str5 + "\n";
                boolean = Convert.ToBoolean((int)num & 4);
                string str7 = boolean.ToString();
                string str8 = str6 + " Simple [lower limit] 1 \t: " + str7 + "\n";
                boolean = Convert.ToBoolean((int)num & 8);
                string str9 = boolean.ToString();
                string str10 = str8 + " Simple [lower limit] 2 \t: " + str9 + "\n";
                boolean = Convert.ToBoolean((int)num & 16);
                string str11 = boolean.ToString();
                string str12 = str10 + " Change [rise/prev.] 1 \t: " + str11 + "\n";
                boolean = Convert.ToBoolean((int)num & 32);
                string str13 = boolean.ToString();
                string str14 = str12 + " Change [rise/prev.] 2 \t: " + str13 + "\n";
                boolean = Convert.ToBoolean((int)num & 64);
                string str15 = boolean.ToString();
                string str16 = str14 + " Change [decline/prev.] 1 \t: " + str15 + "\n";
                boolean = Convert.ToBoolean((int)num & 128);
                string str17 = boolean.ToString();
                string str18 = str16 + " Change [decline/prev.] 2 \t: " + str17 + "\n";
                boolean = Convert.ToBoolean((int)num & 256);
                string str19 = boolean.ToString();
                string str20 = str18 + " Average [upper] \t\t: " + str19 + "\n";
                boolean = Convert.ToBoolean((int)num & 512);
                string str21 = boolean.ToString();
                string str22 = str20 + " Average [lower] \t\t: " + str21 + "\n";
                boolean = Convert.ToBoolean((int)num & 1024);
                string str23 = boolean.ToString();
                string str24 = str22 + " Peak-to-Peak [upper] \t: " + str23 + "\n";
                boolean = Convert.ToBoolean((int)num & 2048);
                string str25 = boolean.ToString();
                string str26 = str24 + " Peak-to-Peak [lower] \t: " + str25 + "\n";
                boolean = Convert.ToBoolean((int)num & 4096);
                string str27 = boolean.ToString();
                string str28 = str26 + " Interval [rise/term] \t: " + str27 + "\n";
                boolean = Convert.ToBoolean((int)num & 8192);
                string str29 = boolean.ToString();
                string str30 = str28 + " Interval [decline/term] \t: " + str29 + "\n";
                boolean = Convert.ToBoolean((int)num & 16384);
                string str31 = boolean.ToString();
                string str32 = str30 + " Base [upper] \t\t: " + str31 + "\n";
                boolean = Convert.ToBoolean((int)num & 32768);
                string str33 = boolean.ToString();
                str1 = str32 + " Base [lower] \t\t: " + str33;
            }
            for (int index = 0; index < 3; ++index)
            {
                string str2 = str1 + "\n\n<" + OmronSensor.accel_sensors[index] + ">\n";
                boolean = Convert.ToBoolean((int)data[5 + index] & 1);
                string str3 = boolean.ToString();
                string str4 = str2 + " Simple [upper limit] 1 \t: " + str3 + "\n";
                boolean = Convert.ToBoolean((int)data[5 + index] & 2);
                string str5 = boolean.ToString();
                string str6 = str4 + " Simple [upper limit] 2 \t: " + str5 + "\n";
                boolean = Convert.ToBoolean((int)data[5 + index] & 16);
                string str7 = boolean.ToString();
                string str8 = str6 + " Change [rise/prev.] 1 \t: " + str7 + "\n";
                boolean = Convert.ToBoolean((int)data[5 + index] & 32);
                string str9 = boolean.ToString();
                str1 = str8 + " Change [rise/prev.] 2 \t: " + str9;
            }
            return str1;
        }

        private string CommonResultParser_Latest_accel_status(byte[] data)
        {
            byte num1 = data[0];
            string str1 = "";
            switch (data[1])
            {
                case 0:
                    str1 = "None";
                    break;
                case 1:
                    str1 = "Vibration";
                    break;
                case 2:
                    str1 = "Earthquake";
                    break;
            }
            double num2 = (double)(short)((int)data[3] << 8 | (int)data[2]) / 10.0;
            double num3 = (double)(short)((int)data[5] << 8 | (int)data[4]) / 10.0;
            double num4 = (double)(short)((int)data[7] << 8 | (int)data[6]) / 10.0;
            string str2 = "";
            switch (data[8])
            {
                case 0:
                    str2 = "Y-Z";
                    break;
                case 1:
                    str2 = "X-Z";
                    break;
                case 2:
                    str2 = "X-Y";
                    break;
            }
            double num5 = (double)(short)((int)data[10] << 8 | (int)data[9]) / 10.0;
            double num6 = (double)(short)((int)data[12] << 8 | (int)data[11]) / 10.0;
            double num7 = (double)(short)((int)data[14] << 8 | (int)data[13]) / 10.0;
            return "" + "<Latest acceleration status>\n" + " Sequence No.\t\t : " + num1.ToString("D") + "\n" + " Status\t\t\t : " + str1 + "\n" + " Maximum acceleration X\t : " + num2.ToString("f1") + " gal\n" + " Maximum acceleration Y\t : " + num3.ToString("f1") + " gal\n" + " Maximum acceleration Z\t : " + num4.ToString("f1") + " gal\n" + " Plane for SI Calculation\t : " + str2 + "\n" + " Acceleration offset X\t : " + num5.ToString("f1") + " gal\n" + " Acceleration offset Y\t : " + num6.ToString("f1") + " gal\n" + " Acceleration offset Z\t : " + num7.ToString("f1") + " gal";
        }

        private string CommonResultParser_Vibration_count(
           byte[] data,
           OmronSensor.COMMUNICATION_INTERFACE comm_if)
        {
            return "" + "<Vibration count>\n" + " Earthquake count\t : " + ((uint)((int)data[3] << 24 | (int)data[2] << 16 | (int)data[1] << 8) | (uint)data[0]).ToString("D") + "\n" + " Vibration count\t\t : " + ((uint)((int)data[7] << 24 | (int)data[6] << 16 | (int)data[5] << 8) | (uint)data[4]).ToString("D");
        }

        private void USB_Latest_sensing_data_Read()
        {
            USB.Frame requestFrame = new USB.Frame();
            USB.Frame responseFrame = new USB.Frame();
            requestFrame.SetFrame(USB.COMMAND.READ, USB.ADDRESS.COMMON_LATEST_SENSING_DATA);
            this.USB_TxRx(requestFrame, ref responseFrame);
            byte[] data = responseFrame.data;
            // if (!responseFrame.errorFlag)
                // this.log_USB.Write(Log.TYPE.RX, this.CommonResultParser_Latest_sensing_data(data), true, false);
            // else
                // this.log_USB.Write(Log.TYPE.ERROR, "Read error", true, false);
        }

        private void USB_Latest_calc_data_Read()
        {
            USB.Frame requestFrame = new USB.Frame();
            USB.Frame responseFrame = new USB.Frame();
            requestFrame.SetFrame(USB.COMMAND.READ, USB.ADDRESS.COMMON_LATEST_CALC_DATA);
            this.USB_TxRx(requestFrame, ref responseFrame);
            byte[] data = responseFrame.data;
            if (!responseFrame.errorFlag) { this.log_USB.Write(Log.TYPE.RX, this.CommonResultParser_Latest_calc_data(data), true, false); }
        }

        private void CalculateHeatRatings()
        {
            USB.Frame requestFrame = new USB.Frame();
            USB.Frame responseFrame = new USB.Frame();
            requestFrame.SetFrame(USB.COMMAND.READ, USB.ADDRESS.COMMON_LATEST_CALC_DATA);
            this.USB_TxRx(requestFrame, ref responseFrame);
            byte[] data = responseFrame.data;
            if (!responseFrame.errorFlag) { CommonResultParser_HeatRatings(data); }
        }

        private void USB_Latest_sensing_flag_Read()
        {
            USB.Frame requestFrame = new USB.Frame();
            USB.Frame responseFrame = new USB.Frame();
            requestFrame.SetFrame(USB.COMMAND.READ, USB.ADDRESS.COMMON_LATEST_SENSING_FLAG);
            this.USB_TxRx(requestFrame, ref responseFrame);
            byte[] data = responseFrame.data;
            if (!responseFrame.errorFlag)
                this.log_USB.Write(Log.TYPE.RX, this.CommonResultParser_Latest_sensing_flag(data), true, false);
            else
                this.log_USB.Write(Log.TYPE.ERROR, "Read error", true, false);
        }

        private void USB_Latest_calculation_flag_Read()
        {
            USB.Frame requestFrame = new USB.Frame();
            USB.Frame responseFrame = new USB.Frame();
            requestFrame.SetFrame(USB.COMMAND.READ, USB.ADDRESS.COMMON_LATEST_CALC_FLAG);
            this.USB_TxRx(requestFrame, ref responseFrame);
            byte[] data = responseFrame.data;
            // if (!responseFrame.errorFlag)
                // this.log_USB.Write(Log.TYPE.RX, this.CommonResultParser_Latest_calc_flag(data), true, false);
            // else
                // this.log_USB.Write(Log.TYPE.ERROR, "Read error", true, false);
        }

        private void USB_Latest_acceleration_status_Read()
        {
            USB.Frame requestFrame = new USB.Frame();
            USB.Frame responseFrame = new USB.Frame();
            requestFrame.SetFrame(USB.COMMAND.READ, USB.ADDRESS.COMMON_LATEST_ACCEL_STATUS);
            this.USB_TxRx(requestFrame, ref responseFrame);
            byte[] data = responseFrame.data;
            if (!responseFrame.errorFlag)
                this.log_USB.Write(Log.TYPE.RX, this.CommonResultParser_Latest_accel_status(data), true, false);
            else
                this.log_USB.Write(Log.TYPE.ERROR, "Read error", true, false);
        }

        private void USB_Vibration_count_Read()
        {
            USB.Frame requestFrame = new USB.Frame();
            USB.Frame responseFrame = new USB.Frame();
            requestFrame.SetFrame(USB.COMMAND.READ, USB.ADDRESS.COMMON_VIBRATION_COUNT);
            this.USB_TxRx(requestFrame, ref responseFrame);
            byte[] data = responseFrame.data;
            // if (!responseFrame.errorFlag)
                // this.log_USB.Write(Log.TYPE.RX, this.CommonResultParser_Vibration_count(data, OmronSensor.COMMUNICATION_INTERFACE.USB), true, false);
            // else
                // this.log_USB.Write(Log.TYPE.ERROR, "Read error", true, false);
        }

        private void Dgv_USB_DeviceList_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (this.dgv_USB_DeviceList[0, e.RowIndex].Value == null)
                return;
            this.label_USB_TargetDevice.Text = this.dgv_USB_DeviceList[0, e.RowIndex].Value.ToString();
            this.targetUSBComPort = this.dgv_USB_DeviceList[0, e.RowIndex].Value.ToString();
            for (int index = 0; index < this.dgv_USB_DeviceList.Rows.Count; ++index)
            {
                if (index == e.RowIndex)
                    this.dgv_USB_DeviceList.Rows[index].DefaultCellStyle.BackColor = Color.LightSalmon;
                else
                    this.dgv_USB_DeviceList.Rows[index].DefaultCellStyle.BackColor = Color.White;
            }
        }

        private string CommonResultParser_LED_setting_normal(
          byte[] data,
          OmronSensor.COMMUNICATION_INTERFACE comm_if)
        {
            string str = "";
            ushort num1 = (ushort)((uint)data[1] << 8 | (uint)data[0]);
            byte num2 = data[2];
            byte num3 = data[3];
            byte num4 = data[4];
            Color color = Color.FromArgb((int)num2, (int)num3, (int)num4);
            int num5 = (int)data[1];
            int num6 = (int)data[0];
            switch (comm_if)
            {
                case OmronSensor.COMMUNICATION_INTERFACE.USB:
                    this.comboBox_USB_LED_setting_normal_rule.SelectedIndex = (int)num1;
                    this.colorWheel_USB_LED.Hue = ColorMath.RgbToHsl(color).H;
                    ColorWheel colorWheelUsbLed1 = this.colorWheel_USB_LED;
                    HslColor hsl1 = ColorMath.RgbToHsl(Color.FromArgb((int)num2, (int)num3, (int)num4));
                    int s1 = (int)hsl1.S;
                    colorWheelUsbLed1.Saturation = (byte)s1;
                    ColorWheel colorWheelUsbLed2 = this.colorWheel_USB_LED;
                    hsl1 = ColorMath.RgbToHsl(Color.FromArgb((int)num2, (int)num3, (int)num4));
                    int l1 = (int)hsl1.L;
                    colorWheelUsbLed2.Lightness = (byte)l1;
                    this.label_USB_LED_Col_HEX.BackColor = color;
                    this.label_USB_LED_Col_HEX.ForeColor = this.GetComplementaryColor(color);
                    this.label_USB_LED_Col_HEX.Text = ColorTranslator.ToHtml(color);
                    str = this.comboBox_USB_LED_setting_normal_rule.Text;
                    break;
                case OmronSensor.COMMUNICATION_INTERFACE.BLE:
                    break;
            }
            return "" + " <LED setting (normal)>\n" + " Rule\t : 0x" + num1.ToString("X4") + ": " + str + "\n" + " RGB\t : (" + num2.ToString() + " : " + num3.ToString() + " : " + num4.ToString() + ")";
        }

        private string CommonResultParser_Latest_memory_info(
          byte[] data,
          OmronSensor.COMMUNICATION_INTERFACE comm_if)
        {
            uint num1 = (uint)((int)data[3] << 24 | (int)data[2] << 16 | (int)data[1] << 8) | (uint)data[0];
            uint num2 = (uint)((int)data[7] << 24 | (int)data[6] << 16 | (int)data[5] << 8) | (uint)data[4];
            switch (comm_if)
            {
                case OmronSensor.COMMUNICATION_INTERFACE.USB:
                    this.label_USB_Memory_index_Latest.Text = num1.ToString("D");
                    this.label_USB_Memory_index_Last.Text = num2.ToString("D");
                    break;
                case OmronSensor.COMMUNICATION_INTERFACE.BLE:
                    break;
            }
            return "" + "<Latest memory information>\n" + " Memory Index (Latest)\t: " + num1.ToString("D") + "\n" + " Memory Index (Last)\t: " + num2.ToString("D");
        }

        private Color GetComplementaryColor(Color color)
        {
            int red = (int)~color.R;
            byte num1 = (byte)~color.G;
            byte num2 = (byte)~color.B;
            int green = (int)num1;
            int blue = (int)num2;
            return Color.FromArgb(red, green, blue);
        }

        public enum COMMUNICATION_INTERFACE
        {
            USB,
            BLE,
        }

        private static readonly string[] beaconMode = new string[8]
        {
      "0x00 : Event Beacon : Env",
      "0x01 : Standrd Beacon : Env",
      "0x02 : General Broadcaster 1 : IM",
      "0x03 : Limited Broadcaster 1 : IM",
      "0x04 : General Broadcaster 2 : EP",
      "0x05 : Limited Broadcaster 2 : EP",
      "0x07 : Alternate Beacon : Env",
      "0x08 : Event Beacon (ADV) : Env"
        };
        private static readonly string[] sensors = new string[9]
        {
      "Temperature",
      "Relative Humidity",
      "Ambient Light",
      "Barometric Pressure",
      "Sound noise",
      "eTVOC",
      "eCO2",
      "Discomfort Index",
      "Heatstroke risk factor"
        };
        private static readonly string[] sensors_unit = new string[9]
        {
      "degC",
      "%RH",
      "lx",
      "hPa",
      "dB",
      "ppb",
      "ppm",
      "-",
      "-"
        };
        private static readonly double[] sensors_order = new double[9]
        {
      0.01,
      0.01,
      1.0,
      0.001,
      0.01,
      1.0,
      1.0,
      0.01,
      0.01
        };
        private static readonly string[] sensors_order_type = new string[9]
        {
      "f2",
      "f2",
      "f0",
      "f3",
      "f2",
      "f0",
      "f0",
      "f2",
      "f2"
        };
        private static readonly double[] sensors_default_thresh_upper1 = new double[9]
        {
      35.0,
      85.0,
      300.0,
      1030.0,
      70.0,
      250.0,
      1500.0,
      75.0,
      28.0
        };
        private static readonly double[] sensors_default_thresh_upper2 = new double[9]
        {
      40.0,
      95.0,
      1000.0,
      1050.0,
      90.0,
      450.0,
      2500.0,
      80.0,
      31.0
        };
        private static readonly double[] sensors_default_thresh_lower1 = new double[9]
        {
      10.0,
      35.0,
      100.0,
      970.0,
      50.0,
      100.0,
      1000.0,
      60.0,
      25.0
        };
        private static readonly double[] sensors_default_thresh_lower2 = new double[9]
        {
      0.0,
      10.0,
      10.0,
      950.0,
      40.0,
      50.0,
      600.0,
      55.0,
      22.0
        };
        private static readonly double[] sensors_default_thresh_changing1 = new double[9]
        {
      1.0,
      1.0,
      100.0,
      0.1,
      10.0,
      50.0,
      100.0,
      2.0,
      1.0
        };
        private static readonly double[] sensors_default_thresh_changing2 = new double[9]
        {
      2.0,
      2.0,
      200.0,
      0.2,
      20.0,
      100.0,
      200.0,
      5.0,
      2.0
        };
        private static readonly double[] sensors_default_thresh_p2p_upper = new double[9]
        {
      1.0,
      1.0,
      100.0,
      0.1,
      10.0,
      50.0,
      100.0,
      2.0,
      1.0
        };
        private static readonly double[] sensors_default_thresh_p2p_lower = new double[9]
        {
      1.0,
      1.0,
      100.0,
      0.1,
      10.0,
      50.0,
      100.0,
      2.0,
      1.0
        };
        private static readonly double[] sensors_default_thresh_interval = new double[9]
        {
      1.0,
      1.0,
      100.0,
      0.1,
      10.0,
      50.0,
      100.0,
      2.0,
      1.0
        };
        private static readonly double[] sensors_default_thresh_base = new double[9]
        {
      1.0,
      1.0,
      100.0,
      0.1,
      10.0,
      50.0,
      100.0,
      2.0,
      1.0
        };
        private static readonly string[] accel_sensors = new string[3]
        {
      "SI",
      "PGA",
      "Seismic intensity"
        };
        private static readonly string[] accel_sensors_unit = new string[3]
        {
      "kine",
      "gal",
      "-"
        };
        private static readonly double[] accel_sensors_order = new double[3]
        {
      0.1,
      0.1,
      0.001
        };
        private static readonly string[] accel_sensors_order_type = new string[3]
        {
      "f1",
      "f1",
      "f3"
        };
        private static readonly double[] accel_sensors_default_thresh_upper1 = new double[3]
        {
      10.0,
      50.0,
      3.5
        };
        private static readonly double[] accel_sensors_default_thresh_upper2 = new double[3]
        {
      17.0,
      100.0,
      5.0
        };
        private static readonly double[] accel_sensors_default_thresh_changing1 = new double[3]
        {
      3.0,
      20.0,
      0.5
        };
        private static readonly double[] accel_sensors_default_thresh_changing2 = new double[3]
        {
      5.0,
      50.0,
      1.0
        };
        private static readonly string[] error_sensors = new string[8]
        {
      "Temperature",
      "Relative Humidity",
      "Ambient Light",
      "Barometric Pressure",
      "Sound noise",
      "Acceleration",
      "eTVOC",
      "eCO2"
        };
        public static readonly string[] dgvColumnName_short = new string[34]
        {
      "#",
      "ID",
      "Update",
      "Type",
      "RSSI",
      "Seq",
      "Temp",
      "Humi",
      "Light",
      "Press",
      "Noise",
      "eTVOC",
      "eCO2",
      "DI",
      "Heat",
      "SI",
      "PGA",
      "Shindo",
      "AccelX",
      "AccelY",
      "AccelZ",
      "State",
      "F_Temp",
      "F_Humi",
      "F_Light",
      "F_Press",
      "F_Noise",
      "F_eTVOC",
      "F_eCO2",
      "F_DI",
      "F_Heat",
      "F_SI",
      "F_PGA",
      "F_Shindo"
        };
        public static readonly string[] dgvColumnName = new string[34]
        {
      "#",
      "Device ID",
      "Last Update",
      "Type",
      "RSSI [dBm]",
      "Sequence No.",
      "Temperature [degC]",
      "Humidity [%RH]",
      "Light [lx]",
      "Pressure [hPa]",
      "Noise [dB]",
      "eTVOC [ppb]",
      "eCO2 [ppm]",
      "Discomfort Index",
      "HeatStroke Risk Factor [degC]",
      "SI [kine]",
      "PGA [gal]",
      "Seismic Intensity",
      "Accel X [mg]",
      "Accel Y [mg]",
      "Accel Z [mg]",
      "Vibration State",
      "Flag - Temperature",
      "Flag - Humidity",
      "Flag - Light",
      "Flag - Pressure",
      "Flag - Noise",
      "Flag - eTVOC",
      "Flag - eCO2",
      "Flag - Discomfort Index",
      "Flag - HeatStroke Risk Factor",
      "Flag - SI",
      "Flag - PGA",
      "Flag - Seismic Intensity"
        };
        public static readonly string[] dgvColumnNameBLE_short = new string[13]
        {
      "#",
      "Address",
      "Update",
      "Type",
      "RSSI",
      "Seq",
      "Temp",
      "Humi",
      "Light",
      "Press",
      "Noise",
      "eTVOC",
      "eCO2"
        };
        public static readonly string[] dgvColumnNameBLE = new string[13]
        {
      "#",
      "Device Address",
      "Last Update",
      "Beacon Type",
      "RSSI [dBm]",
      "Sequence No.",
      "Temperature [degC]",
      "Humidity [%RH]",
      "Light [lx]",
      "Pressure [hPa]",
      "Noise [dB]",
      "eTVOC [ppb]",
      "eCO2 [ppm]"
        };
        public static readonly string[] ble_chart_items = new string[8]
        {
      "RSSI [dBm]",
      "Temperature [degC]",
      "Humidity [%RH]",
      "Light [lx]",
      "Pressure [hPa]",
      "Noise [dB]",
      "eTVOC [ppb]",
      "eCO2 [ppm]"
        };
        public static readonly string[] usb_chart_items = new string[15]
        {
      "Temperature [degC]",
      "Humidity [%RH]",
      "Light [lx]",
      "Pressure [hPa]",
      "Noise [dB]",
      "eTVOC [ppb]",
      "eCO2 [ppm]",
      "Discomfort",
      "Heatstroke [degC]",
      "SI [kine]",
      "PGA [gal]",
      "Shindo",
      "Accel X [mg]",
      "Accel Y [mg]",
      "Accel Z [mg]"
        };
        public static readonly string[] accel_data_type = new string[3]
        {
      "0x00: Earthquake",
      "0x01: Vibration",
      "0x02: Logger"
        };
        public static readonly string[] memory_data_type = new string[4]
        {
      "0x00: Sensing data",
      "0x01: Calculation data",
      "0x02: Sensing flag",
      "0x03: Calculation flag"
        };

        private void ControlGUIComponents(OmronSensor.AppStatus stat)
        {
            if (this.tabCtrl.SelectedTab == this.tabPage_BLE_Config)
                OmronSensor.BeginControlUpdate((Control)this.tabPage_BLE_Config);
            if (this.tabCtrl.SelectedTab == this.tabPage_USB_Config)
                OmronSensor.BeginControlUpdate((Control)this.tabPage_USB_Config);
            if (stat.USBConfig_Opened)
            {
                this.dgv_USB_DeviceList.Enabled = false;
                this.button_USB_Scan.Enabled = false;
                this.button_USB_Open.Enabled = false;
                this.button_USB_Close.Enabled = true;
                this.gb_USB_DeviceInfo.Enabled = true;
                if (stat.USBTransmission)
                {
                    this.gb_USB_Acceleration_Service.Enabled = false;
                    this.gb_USB_Control_Service.Enabled = false;
                    this.gb_USB_Event_Setting_Service.Enabled = false;
                    this.gb_USB_Information_Service.Enabled = false;
                    this.gb_USB_Latest_Data_Service.Enabled = false;
                    this.gb_USB_Memory_Data_Service.Enabled = false;
                    this.gb_USB_Time_Setting_Service.Enabled = false;
                    this.ConnectBTN.Enabled = false;
                    this.gb_USB_USB_Only_Service.Enabled = false;
                }
                else
                {
                    this.gb_USB_Acceleration_Service.Enabled = true;
                    this.gb_USB_Control_Service.Enabled = true;
                    this.gb_USB_Event_Setting_Service.Enabled = true;
                    this.gb_USB_Information_Service.Enabled = true;
                    this.gb_USB_Latest_Data_Service.Enabled = true;
                    this.gb_USB_Memory_Data_Service.Enabled = true;
                    this.gb_USB_Time_Setting_Service.Enabled = true;
                    this.ConnectBTN.Enabled = true;
                    this.gb_USB_USB_Only_Service.Enabled = true;
                }
            }
            else
            {
                this.dgv_USB_DeviceList.Enabled = true;
                this.button_USB_Scan.Enabled = true;
                this.button_USB_Open.Enabled = true;
                this.button_USB_Close.Enabled = false;
                this.gb_USB_DeviceInfo.Enabled = false;
                this.gb_USB_Acceleration_Service.Enabled = false;
                this.gb_USB_Control_Service.Enabled = false;
                this.gb_USB_Event_Setting_Service.Enabled = false;
                this.gb_USB_Information_Service.Enabled = false;
                this.gb_USB_Latest_Data_Service.Enabled = false;
                this.gb_USB_Memory_Data_Service.Enabled = false;
                this.gb_USB_Time_Setting_Service.Enabled = false;
                this.ConnectBTN.Enabled = false;
                this.gb_USB_USB_Only_Service.Enabled = false;
            }

            if (this.tabCtrl.SelectedTab == this.tabPage_BLE_Config)
                OmronSensor.EndControlUpdate((Control)this.tabPage_BLE_Config);
            if (this.tabCtrl.SelectedTab != this.tabPage_USB_Config)
                return;
            OmronSensor.EndControlUpdate((Control)this.tabPage_USB_Config);
        }

        private void Timer_Monitor_USB_Tick(object sender, EventArgs e)
        {
            double num1 = (double)((long)Environment.TickCount - this.tickStart_USB) / 1000.0;
            if (num1 > this.chart_USB.ChartAreas[0].AxisX.Maximum - (double)((int)Convert.ToUInt16(this.comboBox_USB_xrange.Text) * 60) * 0.05)
            {
                if (this.chart_USB.Series[0].Points.Count > 0 && this.chart_USB.Series[0].Points[0].XValue < this.chart_USB.ChartAreas[0].AxisX.Minimum - (double)((int)Convert.ToUInt16(this.comboBox_USB_xrange.Text) * 60) * 0.05)
                {
                    for (int index = 0; index < this.chart_USB.Series.Count; ++index)
                        this.chart_USB.Series[index].Points.RemoveAt(0);
                }
                for (int index = 0; index < this.chart_USB.ChartAreas.Count; ++index)
                {
                    this.chart_USB.ChartAreas[index].AxisX.Maximum = num1 + (double)((int)Convert.ToUInt16(this.comboBox_USB_xrange.Text) * 60) * 0.05;
                    this.chart_USB.ChartAreas[index].AxisX.Minimum = this.chart_USB.ChartAreas[index].AxisX.Maximum - (double)((int)Convert.ToUInt16(this.comboBox_USB_xrange.Text) * 60);
                }
            }
            double num2 = 9999999999.0;
            double num3 = -999999999.0;
            for (int index = 0; index < this.chart_USB.Series.Count; ++index)
            {
                foreach (DataPoint point in (Collection<DataPoint>)this.chart_USB.Series[index].Points)
                {
                    double yvalue = point.YValues[0];
                    if (num3 < yvalue)
                        num3 = yvalue;
                    if (num2 > yvalue)
                        num2 = yvalue;
                }
            }
            double num4 = num3 - num2;
            if (num4 > 0.001)
            {
                this.chart_USB.ChartAreas[0].AxisY.Maximum = num3 + num4 * 0.1;
                this.chart_USB.ChartAreas[0].AxisY.Minimum = num2 - num4 * 0.1;
            }
            else if (num4 >= 0.0)
            {
                this.chart_USB.ChartAreas[0].AxisY.Maximum = num3 + 0.1;
                this.chart_USB.ChartAreas[0].AxisY.Minimum = num2 - 0.1;
            }
            this.chart_USB.Invalidate();
        }

        private void Timer_USB_Tick(object sender, EventArgs e)
        {
            USB.Frame requestFrame1 = new USB.Frame();
            USB.Frame responseFrame = new USB.Frame();
            requestFrame1.SetFrame(USB.COMMAND.READ, USB.ADDRESS.COMMON_LATEST_SENSING_DATA);
            this.USB_TxRx(requestFrame1, ref responseFrame);
            byte[] data1 = responseFrame.data;
            USB.Frame requestFrame2 = new USB.Frame();
            responseFrame = new USB.Frame();
            requestFrame2.SetFrame(USB.COMMAND.READ, USB.ADDRESS.COMMON_LATEST_CALC_DATA);
            this.USB_TxRx(requestFrame2, ref responseFrame);
            byte[] data2 = responseFrame.data;
            USB.Frame requestFrame3 = new USB.Frame();
            responseFrame = new USB.Frame();
            requestFrame3.SetFrame(USB.COMMAND.READ, USB.ADDRESS.COMMON_LATEST_SENSING_FLAG);
            this.USB_TxRx(requestFrame3, ref responseFrame);
            byte[] data3 = responseFrame.data;
            USB.Frame requestFrame4 = new USB.Frame();
            responseFrame = new USB.Frame();
            requestFrame4.SetFrame(USB.COMMAND.READ, USB.ADDRESS.COMMON_LATEST_CALC_FLAG);
            this.USB_TxRx(requestFrame4, ref responseFrame);
            byte[] data4 = responseFrame.data;

            Sensor sensor = new Sensor()
            {
                serialString = label_USB_SerialNumber.Text,
                lastUpdated = DateTime.Now,
                sequenceNum = data1[0]
            };
            sensor.type = Sensor.TYPE.USB;
            sensor.data.value[0] = (double)(short)((int)data1[2] << 8 | (int)data1[1]) / 100.0;
            sensor.data.value[1] = (double)(short)((int)data1[4] << 8 | (int)data1[3]) / 100.0;
            sensor.data.value[2] = (double)(short)((int)data1[6] << 8 | (int)data1[5]);
            sensor.data.value[3] = (double)((int)data1[10] << 24 | (int)data1[9] << 16 | (int)data1[8] << 8 | (int)data1[7]) / 1000.0;
            sensor.data.value[4] = (double)(short)((int)data1[12] << 8 | (int)data1[11]) / 100.0;
            sensor.data.value[5] = (double)(short)((int)data1[14] << 8 | (int)data1[13]);
            sensor.data.value[6] = (double)(short)((int)data1[16] << 8 | (int)data1[15]);
            sensor.data.value[7] = (double)(short)((int)data2[2] << 8 | (int)data2[1]) / 100.0;
            sensor.data.value[8] = (double)(short)((int)data2[4] << 8 | (int)data2[3]) / 100.0;
            sensor.data.value[9] = (double)(ushort)((uint)data2[7] << 8 | (uint)data2[6]) / 10.0;
            sensor.data.value[10] = (double)(ushort)((uint)data2[9] << 8 | (uint)data2[8]) / 10.0;
            sensor.data.value[11] = (double)(ushort)((uint)data2[11] << 8 | (uint)data2[10]) / 1000.0;
            sensor.data.value[12] = (double)(short)((int)data2[13] << 8 | (int)data2[12]) / 10.0;
            sensor.data.value[13] = (double)(short)((int)data2[15] << 8 | (int)data2[14]) / 10.0;
            sensor.data.value[14] = (double)(short)((int)data2[17] << 8 | (int)data2[16]) / 10.0;
            sensor.data.vibrationState = (SensorData.VIB_STATE)data2[5];
            sensor.flag.value[0] = (ushort)((uint)data3[2] << 8 | (uint)data3[1]);
            sensor.flag.value[1] = (ushort)((uint)data3[4] << 8 | (uint)data3[3]);
            sensor.flag.value[2] = (ushort)((uint)data3[6] << 8 | (uint)data3[5]);
            sensor.flag.value[3] = (ushort)((uint)data3[8] << 8 | (uint)data3[7]);
            sensor.flag.value[4] = (ushort)((uint)data3[10] << 8 | (uint)data3[9]);
            sensor.flag.value[5] = (ushort)((uint)data3[12] << 8 | (uint)data3[11]);
            sensor.flag.value[6] = (ushort)((uint)data3[14] << 8 | (uint)data3[13]);
            sensor.flag.value[7] = (ushort)((uint)data4[2] << 8 | (uint)data4[1]);
            sensor.flag.value[8] = (ushort)((uint)data4[4] << 8 | (uint)data4[3]);
            sensor.flag.value[9] = (ushort)data4[5];
            sensor.flag.value[10] = (ushort)data4[6];
            sensor.flag.value[11] = (ushort)data4[7];
            if ((int)this.sensor_usb.sequenceNum != (int)sensor.sequenceNum)
            {
                this.sensor_usb.SensorUpdate(sensor);
            }

            label_USB_Mon_Temp.Text = this.sensor_usb.data.value[0].ToString(SensorData.order[0]) + " c";
            OmronData.Temp = this.sensor_usb.data.value[0].ToString(SensorData.order[0]) + " c";

            label_USB_Mon_Humi.Text = this.sensor_usb.data.value[1].ToString(SensorData.order[1]) + " rh";
            OmronData.Humidity = this.sensor_usb.data.value[1].ToString(SensorData.order[1]) + " rh";

            // Calculate the Dynamic Data
            CalculateHeatRatings();         // Heat Rating and Discomfort Index

            eTVOCLabelTxt.Text = this.sensor_usb.data.value[5] < 450.0 ? (this.sensor_usb.data.value[5] < 250.0 ? (string)"Good" : (string)"Moderate") : (string)"Poor";
            OmronData.eTVOCLabelTxt = this.sensor_usb.data.value[5] < 450.0 ? (this.sensor_usb.data.value[5] < 250.0 ? (string)"Good" : (string)"Moderate") : (string)"Poor";

            eCO2LabelTxt.Text = this.sensor_usb.data.value[6] < 2500.0 ? (this.sensor_usb.data.value[6] < 1500.0 ? (string)"Good" : (string)"Moderate") : (string)"Poor";
            OmronData.eCO2LabelTxt = this.sensor_usb.data.value[6] < 2500.0 ? (this.sensor_usb.data.value[6] < 1500.0 ? (string)"Good" : (string)"Moderate") : (string)"Poor";

            NlLabelTxt.Text = this.sensor_usb.data.value[4] < 80.0 ? (this.sensor_usb.data.value[4] < 70.0 ? (this.sensor_usb.data.value[4] < 60.0 ? (this.sensor_usb.data.value[4] < 50.0 ? (string)"Quiet" : (string)"Intrusive") : (string)"Noisy") : (string)"Annoying") : (string)"Loud";
            OmronData.NlLabelTxt = this.sensor_usb.data.value[4] < 80.0 ? (this.sensor_usb.data.value[4] < 70.0 ? (this.sensor_usb.data.value[4] < 60.0 ? (this.sensor_usb.data.value[4] < 50.0 ? (string)"Quiet" : (string)"Intrusive") : (string)"Noisy") : (string)"Annoying") : (string)"Loud";

            label_USB_Mon_Light.Text = this.sensor_usb.data.value[2].ToString(SensorData.order[2]) + " lx";
            LlLabelTxt.Text = this.sensor_usb.data.value[2] >= 5 ? (this.sensor_usb.data.value[4] > 5 && this.sensor_usb.data.value[4] < 100 ? (string)"Moderate" : (string)"Good") : (string)"Poor";
            OmronData.Light = this.sensor_usb.data.value[2].ToString(SensorData.order[2]) + " lx";
            OmronData.LlLabelTxt = this.sensor_usb.data.value[2] >= 5 ? (this.sensor_usb.data.value[4] > 5 && this.sensor_usb.data.value[4] < 100 ? (string)"Moderate" : (string)"Good") : (string)"Poor";

            label_USB_Mon_Press.Text = this.sensor_usb.data.value[3].ToString(SensorData.order[3]) + " hPa";
            OmronData.Pressure = this.sensor_usb.data.value[3].ToString(SensorData.order[3]) + " hPa";

            label_USB_Mon_Noise.Text = this.sensor_usb.data.value[4].ToString(SensorData.order[4]) + " dB";
            OmronData.Noise = this.sensor_usb.data.value[4].ToString(SensorData.order[4]) + " dB";

            label_USB_Mon_VOC.Text = this.sensor_usb.data.value[5].ToString(SensorData.order[5]) + " ppb";
            OmronData.eTVOC = this.sensor_usb.data.value[5].ToString(SensorData.order[5]) + " ppb";

            label_USB_Mon_CO2.Text = this.sensor_usb.data.value[6].ToString(SensorData.order[6]) + " ppm";
            OmronData.eCO2 = this.sensor_usb.data.value[6].ToString(SensorData.order[6]) + " ppm";

            // Vibration Variables
            label_USB_Mon_SI.Text = this.sensor_usb.data.value[9].ToString(SensorData.order[9]) + " kine";
            OmronData.Vibrate_SI = this.sensor_usb.data.value[9].ToString(SensorData.order[9]) + " kine";

            label_USB_Mon_PGA.Text = this.sensor_usb.data.value[10].ToString(SensorData.order[10]) + " gal";
            OmronData.Vibrate_PGA = this.sensor_usb.data.value[10].ToString(SensorData.order[10]) + " gal";

            label_USB_Mon_Shindo.Text = this.sensor_usb.data.value[11].ToString(SensorData.order[11]) ?? "";
            OmronData.Vibrate_SeismicIntensity = this.sensor_usb.data.value[11].ToString(SensorData.order[11]) ?? "";

            label_USB_Mon_AccelX.Text = this.sensor_usb.data.value[12].ToString(SensorData.order[12]) + " gal";
            OmronData.Vibrate_AccelerationX = this.sensor_usb.data.value[12].ToString(SensorData.order[12]) + " gal";

            label_USB_Mon_AccelY.Text = this.sensor_usb.data.value[13].ToString(SensorData.order[13]) + " gal";
            OmronData.Vibrate_AccelerationY = this.sensor_usb.data.value[13].ToString(SensorData.order[13]) + " gal";

            label_USB_Mon_AccelZ.Text = this.sensor_usb.data.value[14].ToString(SensorData.order[14]) + " gal";
            OmronData.Vibrate_AccelerationZ = this.sensor_usb.data.value[14].ToString(SensorData.order[14]) + " gal";

            MainForm.FrmObj.SensorOpt.Text = "Connected";           // Make sure that the MainForm SensorOpt Label is always set to "Connected" for logging

            // Create a JSON Object of the OmronSensor so that it can drop this json into a flat file that could be read by signage
            dynamic jsonObject = new JObject();
            jsonObject.Status = MainForm.FrmObj.SensorOpt.Text;
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
            catch { }

            this.USB_monitor_labels[0].Text = this.sensor_usb.data.value[0].ToString(SensorData.order[0]) + " degC";
            this.USB_monitor_labels[1].Text = this.sensor_usb.data.value[1].ToString(SensorData.order[1]) + " %RH";
            this.USB_monitor_labels[2].Text = this.sensor_usb.data.value[2].ToString(SensorData.order[2]) + " lx";
            this.USB_monitor_labels[3].Text = this.sensor_usb.data.value[3].ToString(SensorData.order[3]) + " hPa";
            this.USB_monitor_labels[4].Text = this.sensor_usb.data.value[4].ToString(SensorData.order[4]) + " dB";
            this.USB_monitor_labels[5].Text = this.sensor_usb.data.value[5].ToString(SensorData.order[5]) + " ppb";
            this.USB_monitor_labels[6].Text = this.sensor_usb.data.value[6].ToString(SensorData.order[6]) + " ppm";
            this.USB_monitor_labels[7].Text = this.sensor_usb.data.value[7].ToString(SensorData.order[7]) ?? "";
            this.USB_monitor_labels[8].Text = this.sensor_usb.data.value[8].ToString(SensorData.order[8]) + " degC";
            this.USB_monitor_labels[9].Text = this.sensor_usb.data.value[9].ToString(SensorData.order[9]) + " kine";
            this.USB_monitor_labels[10].Text = this.sensor_usb.data.value[10].ToString(SensorData.order[10]) + " gal";
            this.USB_monitor_labels[11].Text = this.sensor_usb.data.value[11].ToString(SensorData.order[11]) ?? "";
            this.USB_monitor_labels[12].Text = this.sensor_usb.data.value[12].ToString(SensorData.order[12]) + " gal";
            this.USB_monitor_labels[13].Text = this.sensor_usb.data.value[13].ToString(SensorData.order[13]) + " gal";
            this.USB_monitor_labels[14].Text = this.sensor_usb.data.value[14].ToString(SensorData.order[14]) + " gal";

            switch (this.sensor_usb.data.vibrationState)
            {
                case SensorData.VIB_STATE.NONE:

                    break;
                case SensorData.VIB_STATE.VIBRATION:

                    break;
                case SensorData.VIB_STATE.QUAKE:

                    break;
            }
            this.sensor_usb.csv.Save();
        }

        private void StartTimers()
        {
            string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
            var MyIni = new IniFile(iniFile);
            // Backfill Checks incase this is the 1st time running the USB Environmental Sensor
            var TimerQuiet = MyIni.Read("TimerQuiet", "Environmental");
            if (TimerQuiet == "")
            {
                MyIni.Write("TimerQuiet", "2", "Environmental");
            }

            var TempOver = MyIni.Read("TempOver", "Environmental");
            if (TempOver == "")
            {
                MyIni.Write("TempOver", "50.00", "Environmental");
            }
            var TempOver_Enable = MyIni.Read("TempOver_Enable", "Environmental");
            if (TempOver_Enable == "")
            {
                MyIni.Write("TempOver_Enable", "Off", "Environmental");
                TempOver_Enable = "Off";
            }
            var TempUnder = MyIni.Read("TempUnder", "Environmental");
            if (TempUnder == "")
            {
                MyIni.Write("TempUnder", "5.00", "Environmental");
            }
            var TempUnder_Enable = MyIni.Read("TempUnder_Enable", "Environmental");
            if (TempUnder_Enable == "")
            {
                MyIni.Write("TempUnder_Enable", "Off", "Environmental");
                TempUnder_Enable = "Off";
            }

            var LightOver = MyIni.Read("LightOver", "Environmental");
            if (LightOver == "")
            {
                MyIni.Write("LightOver", "500", "Environmental");
            }
            var LightOver_Enable = MyIni.Read("LightOver_Enable", "Environmental");
            if (LightOver_Enable == "")
            {
                MyIni.Write("LightOver_Enable", "Off", "Environmental");
                LightOver_Enable = "Off";
            }
            var LightUnder = MyIni.Read("LightUnder", "Environmental");
            if (LightUnder == "")
            {
                MyIni.Write("LightUnder", "5", "Environmental");
            }
            var LightUnder_Enable = MyIni.Read("LightUnder_Enable", "Environmental");
            if (LightUnder_Enable == "")
            {
                MyIni.Write("LightUnder_Enable", "Off", "Environmental");
                LightUnder_Enable = "Off";
            }

            var NoiseOver = MyIni.Read("NoiseOver", "Environmental");
            if (NoiseOver == "")
            {
                MyIni.Write("NoiseOver", "100.00", "Environmental");
            }
            var NoiseOver_Enable = MyIni.Read("NoiseOver_Enable", "Environmental");
            if (NoiseOver_Enable == "")
            {
                MyIni.Write("NoiseOver_Enable", "Off", "Environmental");
                NoiseOver_Enable = "Off";
            }

            globalTempOverTrigger = TempOver_Enable;                // Enable or Disable Global Trigger/Timers (Temp - HIGH)
            globalTempUnderTrigger = TempUnder_Enable;              // Enable or Disable Global Trigger/Timers (Temp - LOW)
            globalLightOverTrigger = LightOver_Enable;              // Enable or Disable Global Trigger/Timers (Light - HIGH)
            globalLightUnderTrigger = LightUnder_Enable;            // Enable or Disable Global Trigger/Timers (Light - LOW)
            globalNoiseOverTrigger = NoiseOver_Enable;              // Enable or Disable Global Trigger/Timers (Noise - HIGH)

            TempOverTimer.Enabled = true;           // Temperature Timer (HIGH) Enabled
            TempOverTimer.Interval = 5000;         // Temperature Timer (HIGH) Interval (5.0 seconds)
            TempUnderTimer.Enabled = true;           // Temperature Timer (LOW) Enabled
            TempUnderTimer.Interval = 5000;         // Temperature Timer (LOW) Interval (5.0 seconds)

            LightOverTimer.Enabled = true;           // Light Timer (HIGH) Enabled
            LightOverTimer.Interval = 5100;         // Light Timer (HIGH) Interval (5.1 seconds)
            LightUnderTimer.Enabled = true;           // Light Timer (LOW) Enabled
            LightUnderTimer.Interval = 5100;         // Light Timer (LOW) Interval (5.1 seconds)

            NoiseOverTimer.Enabled = true;           // Noise Timer (HIGH) Enabled
            NoiseOverTimer.Interval = 5200;         // Noise Timer Interval (5.2 seconds)
        }

        private void TempOverTrigger(string TimerQuiet)
        {
            if (!quietTempOverTrigger && globalTempOverTrigger == "On")
            {
                int numberOfMins = int.Parse(TimerQuiet);          // Number of Mins
                int TimerShutOff = numberOfMins * 60000;           // numberOfMins * 60 seconds = Total Number of Mins

                TempOverTimerCooldown.Interval = TimerShutOff;         // Set the Interval for the Cooldown Timer to new Variable
                TempOverTimerCooldown.Enabled = true;                  // Enable and Set off the Temp Cooldown

                quietTempOverTrigger = true;                           // Set the Quiet Trigger to true to hold the checker for X Mins

                // Send the Websocket Trigger to Digital Signage
                GCMSSystem.NodeSocket.Send("custom:OMOVERTEMP");                  // Hardcoded Over Temperature Websocket
                tempOverCNT = 0;                                           // Reset the TempOverCNT to 0 for Checker
            }
            
        }
        private void TempUnderTrigger(string TimerQuiet)
        {
            if (!quietTempUnderTrigger && globalTempUnderTrigger == "On")
            {
                int numberOfMins = int.Parse(TimerQuiet);          // Number of Mins
                int TimerShutOff = numberOfMins * 60000;           // numberOfMins * 60 seconds = Total Number of Mins

                TempUnderTimerCooldown.Interval = TimerShutOff;         // Set the Interval for the Cooldown Timer to new Variable
                TempUnderTimerCooldown.Enabled = true;                  // Enable and Set off the Temp Cooldown

                quietTempUnderTrigger = true;                           // Set the Quiet Trigger to true to hold the checker for X Mins

                // Send the Websocket Trigger to Digital Signage
                GCMSSystem.NodeSocket.Send("custom:OMUNDERTEMP");                 // Hardcoded Under Temperature Websocket
                tempUnderCNT = 0;                                          // Reset the tempUnderCNT to 0 for Checker
            }
            
        }
        private void LightOverTrigger(string TimerQuiet)
        {
            if (!quietLightOverTrigger && globalLightOverTrigger == "On")
            {
                int numberOfMins = int.Parse(TimerQuiet);          // Number of Mins
                int TimerShutOff = numberOfMins * 60000;           // numberOfMins * 60 seconds = Total Number of Mins

                LightOverTimerCooldown.Interval = TimerShutOff;         // Set the Interval for the Cooldown Timer to new Variable
                LightOverTimerCooldown.Enabled = true;                  // Enable and Set off the Temp Cooldown

                quietLightOverTrigger = true;                           // Set the Quiet Trigger to true to hold the checker for X Mins

                // Send the Websocket Trigger to Digital Signage
                GCMSSystem.NodeSocket.Send("custom:OMOVERLIGHT");       // Hardcoded Over Light Websocket
                lightOverCNT = 0;                                          // Reset the LightOverCNT to 0 for Checker
            }
            
        }
        private void LightUnderTrigger(string TimerQuiet)
        {
            if (!quietLightUnderTrigger && globalLightUnderTrigger == "On")
            {
                int numberOfMins = int.Parse(TimerQuiet);          // Number of Mins
                int TimerShutOff = numberOfMins * 60000;           // numberOfMins * 60 seconds = Total Number of Mins

                LightUnderTimerCooldown.Interval = TimerShutOff;         // Set the Interval for the Cooldown Timer to new Variable
                LightUnderTimerCooldown.Enabled = true;                  // Enable and Set off the Temp Cooldown

                quietLightUnderTrigger = true;                           // Set the Quiet Trigger to true to hold the checker for X Mins

                // Send the Websocket Trigger to Digital Signage
                GCMSSystem.NodeSocket.Send("custom:OMUNDERLIGHT");              // Hardcoded Under Light Websocket
                lightUnderCNT = 0;                                        // Reset the lightUnderCNT to 0 for Checker
            }
            
        }
        private void NoiseOverTrigger(string TimerQuiet)
        {
            if (!quietNoiseOverTrigger && globalNoiseOverTrigger == "On")
            {
                int numberOfMins = int.Parse(TimerQuiet);          // Number of Mins
                int TimerShutOff = numberOfMins * 60000;           // numberOfMins * 60 seconds = Total Number of Mins

                NoiseOverTimerCooldown.Interval = TimerShutOff;         // Set the Interval for the Cooldown Timer to new Variable
                NoiseOverTimerCooldown.Enabled = true;                  // Enable and Set off the Temp Cooldown

                quietNoiseOverTrigger = true;                           // Set the Quiet Trigger to true to hold the checker for X Mins

                // Send the Websocket Trigger to Digital Signage
                GCMSSystem.NodeSocket.Send("custom:OMOVERNOISE");                 // Hardcoded Over Noise Websocket
                noiseOverCNT = 0;                                          // Reset the noiseOverCNT to 0 for Checker
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        private void TempOverTimer_Tick(object sender, EventArgs e)
        {
            string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
            var MyIni = new IniFile(iniFile);
            var TimerQuiet = MyIni.Read("TimerQuiet", "Environmental");
            if (int.Parse(TimerQuiet) < 1)
            {
                TimerQuiet = "2";
            };

            var TempThresholdOver = MyIni.Read("TempOver", "Environmental");
            var TempThresholdUnder = MyIni.Read("TempUnder", "Environmental");

            double ActualTempThresholdOver = double.Parse(TempThresholdOver);
            double ActualTempThresholdUnder = double.Parse(TempThresholdUnder);

            // Debug.WriteLine("Quiet Temp Trigger?: " + quietTempTrigger.ToString());
            if (!quietTempOverTrigger && globalTempOverTrigger == "On")
            {
                // Debug.WriteLine("Temp Count: " + tempOverCNT.ToString());
                string currentTempRegex = Regex.Replace(label_USB_Mon_Temp.Text, @"[^0-9\.]", "");
                double currentTempINT = double.Parse(currentTempRegex);
                // Debug.WriteLine("Current Temp: " + currentTempINT.ToString());

                if (currentTempINT > ActualTempThresholdOver)
                {
                    tempOverCNT++;
                } else
                {
                    tempOverCNT = 0;
                }

                // Over Threshold and the Counter has reached 5
                if (tempOverCNT > 1) { TempOverTrigger(TimerQuiet); }
            }
            
        }

        private void TempUnderTimer_Tick(object sender, EventArgs e)
        {
            string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
            var MyIni = new IniFile(iniFile);
            var TimerQuiet = MyIni.Read("TimerQuiet", "Environmental");
            if (int.Parse(TimerQuiet) < 1)
            {
                TimerQuiet = "2";
            };

            var TempThresholdOver = MyIni.Read("TempOver", "Environmental");
            var TempThresholdUnder = MyIni.Read("TempUnder", "Environmental");

            double ActualTempThresholdOver = double.Parse(TempThresholdOver);
            double ActualTempThresholdUnder = double.Parse(TempThresholdUnder);

            // Debug.WriteLine("Quiet Temp Trigger?: " + quietTempTrigger.ToString());
            if (!quietTempUnderTrigger && globalTempUnderTrigger == "On")
            {
                // Debug.WriteLine("Temp Count: " + tempOverCNT.ToString());
                string currentTempRegex = Regex.Replace(label_USB_Mon_Temp.Text, @"[^0-9\.]", "");
                double currentTempINT = double.Parse(currentTempRegex);
                // Debug.WriteLine("Current Temp: " + currentTempINT.ToString());

                if (currentTempINT < ActualTempThresholdUnder)
                {
                    tempUnderCNT++;
                }
                else
                {
                    tempUnderCNT = 0;
                }

                // Over Threshold and the Counter has reached 5
                if (tempUnderCNT > 1) { TempUnderTrigger(TimerQuiet); }
            }
            
        }

        private void TempOverTimerCooldown_Tick(object sender, EventArgs e)
        {
            if (quietTempOverTrigger && globalTempOverTrigger == "On")       // Only if quietTempTrigger is set to true, to run this function set
            {
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                var TimerQuiet = MyIni.Read("TimerQuiet", "Environmental");
                if (int.Parse(TimerQuiet) < 2)
                {
                    TimerQuiet = "2";
                };

                // Reset the Timer Details
                int numberOfMins = int.Parse(TimerQuiet);          // Number of Mins
                int TimerShutOff = numberOfMins * 60000;           // numberOfMins * 60 seconds = Total Number of Mins

                // Debug.WriteLine("Temp Cooldown Complete");
                tempOverCNT = 0;                                   // Reset the TempOverCNT to 0 for Checker
                quietTempOverTrigger = false;                          // Reset the quietTempTrigger to false, so it will check again

                TempOverTimerCooldown.Interval = TimerShutOff;         // Set the Interval for the Cooldown Timer to new Variable
                TempOverTimerCooldown.Enabled = false;                 // Disable Temp Cooldown
            }
            
        }
        private void TempUnderTimerCooldown_Tick(object sender, EventArgs e)
        {
            if (quietTempUnderTrigger && globalTempUnderTrigger == "On")       // Only if quietTempTrigger is set to true, to run this function set
            {
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                var TimerQuiet = MyIni.Read("TimerQuiet", "Environmental");
                if (int.Parse(TimerQuiet) < 2)
                {
                    TimerQuiet = "2";
                };

                // Reset the Timer Details
                int numberOfMins = int.Parse(TimerQuiet);          // Number of Mins
                int TimerShutOff = numberOfMins * 60000;           // numberOfMins * 60 seconds = Total Number of Mins

                // Debug.WriteLine("Temp Cooldown Complete");
                tempUnderCNT = 0;                                   // Reset the TempOverCNT to 0 for Checker
                quietTempUnderTrigger = false;                          // Reset the quietTempTrigger to false, so it will check again

                TempUnderTimerCooldown.Interval = TimerShutOff;         // Set the Interval for the Cooldown Timer to new Variable
                TempUnderTimerCooldown.Enabled = false;                 // Disable Temp Cooldown
            }
            
        }

        /// <summary>
        /// This area is for the Lights Timers and Cooldowns
        /// </summary>
        private void LightOverTimer_Tick(object sender, EventArgs e)
        {
            string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
            var MyIni = new IniFile(iniFile);
            var TimerQuiet = MyIni.Read("TimerQuiet", "Environmental");
            if (int.Parse(TimerQuiet) < 2)
            {
                TimerQuiet = "2";
            };

            var LightThresholdOver = MyIni.Read("LightOver", "Environmental");
            var LightThresholdUnder = MyIni.Read("LightUnder", "Environmental");

            double ActualLightThresholdOver = int.Parse(LightThresholdOver);
            double ActualLightThresholdUnder = int.Parse(LightThresholdUnder);

            // Debug.WriteLine("Quiet Light Trigger?: " + quietLightTrigger.ToString());
            if (!quietLightOverTrigger && globalLightOverTrigger == "On")
            {
                // Debug.WriteLine("Light Count: " + lightOverCNT.ToString());
                string currentLightRegex = Regex.Replace(label_USB_Mon_Light.Text, @"[^0-9\.]", "");
                double currentLightINT = double.Parse(currentLightRegex);
                // Debug.WriteLine("Current Light: " + currentLightINT.ToString());

                if (currentLightINT > ActualLightThresholdOver)
                {
                    lightOverCNT++;
                } else
                {
                    lightOverCNT = 0;
                }

                // Over Threshold and the Counter has reached 5
                if (lightOverCNT > 1) { LightOverTrigger(TimerQuiet); }
            }
            
        }

        private void LightUnderTimer_Tick(object sender, EventArgs e)
        {
            string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
            var MyIni = new IniFile(iniFile);
            var TimerQuiet = MyIni.Read("TimerQuiet", "Environmental");
            if (int.Parse(TimerQuiet) < 2)
            {
                TimerQuiet = "2";
            };

            var LightThresholdOver = MyIni.Read("LightOver", "Environmental");
            var LightThresholdUnder = MyIni.Read("LightUnder", "Environmental");

            double ActualLightThresholdOver = int.Parse(LightThresholdOver);
            double ActualLightThresholdUnder = int.Parse(LightThresholdUnder);

            // Debug.WriteLine("Quiet Light Trigger?: " + quietLightUnderTrigger.ToString());
            if (!quietLightUnderTrigger && globalLightUnderTrigger == "On")
            {
                // Debug.WriteLine("Light Count: " + lightUnderCNT.ToString());
                string currentLightRegex = Regex.Replace(label_USB_Mon_Light.Text, @"[^0-9\.]", "");
                double currentLightINT = double.Parse(currentLightRegex);
                // Debug.WriteLine("Current Light: " + currentLightINT.ToString());

                if (currentLightINT < ActualLightThresholdUnder)
                {
                    lightUnderCNT++;
                }
                else
                {
                    lightUnderCNT = 0;
                }

                // Over Threshold and the Counter has reached 5
                if (lightUnderCNT > 1) { LightUnderTrigger(TimerQuiet); }
            }
            
        }

        private void LightTimerOverCooldown_Tick(object sender, EventArgs e)
        {
            if (quietLightOverTrigger && globalLightOverTrigger == "On" )       // Only if quietLightTrigger is set to true, to run this function set
            {
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                var TimerQuiet = MyIni.Read("TimerQuiet", "Environmental");
                if (int.Parse(TimerQuiet) < 1) {
                    TimerQuiet = "2";
                };

                // Reset the Timer Details
                int numberOfMins = int.Parse(TimerQuiet);          // Number of Mins
                int TimerShutOff = numberOfMins * 60000;           // numberOfMins * 60 seconds = Total Number of Mins

                // Debug.WriteLine("Light Cooldown Complete");
                lightOverCNT = 0;                                   // Reset the lightOverCNT to 0 for Checker
                quietLightOverTrigger = false;                      // Reset the quietLightTrigger to false, so it will check again

                LightOverTimerCooldown.Interval = TimerShutOff;         // Set the Interval for the Cooldown Timer to new Variable
                LightOverTimerCooldown.Enabled = false;                 // Disable Temp Cooldown
            }
            
        }

        private void LightUnderTimerCooldown_Tick(object sender, EventArgs e)
        {
            if (quietLightUnderTrigger && globalLightUnderTrigger == "On")       // Only if quietLightTrigger is set to true, to run this function set
            {
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                var TimerQuiet = MyIni.Read("TimerQuiet", "Environmental");
                if (int.Parse(TimerQuiet) < 1)
                {
                    TimerQuiet = "2";
                };

                // Reset the Timer Details
                int numberOfMins = int.Parse(TimerQuiet);          // Number of Mins
                int TimerShutOff = numberOfMins * 60000;           // numberOfMins * 60 seconds = Total Number of Mins

                // Debug.WriteLine("Light Cooldown Complete");
                lightUnderCNT = 0;                                   // Reset the lightOverCNT to 0 for Checker
                quietLightUnderTrigger = false;                      // Reset the quietLightTrigger to false, so it will check again

                LightUnderTimerCooldown.Interval = TimerShutOff;         // Set the Interval for the Cooldown Timer to new Variable
                LightUnderTimerCooldown.Enabled = false;                 // Disable Temp Cooldown
            }
            
        }

        /// <summary>
        /// This area for for the Noise Timers and Cooldowns
        /// </summary>
        private void NoiseTimer_Tick(object sender, EventArgs e)
        {
            string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
            var MyIni = new IniFile(iniFile);
            var TimerQuiet = MyIni.Read("TimerQuiet", "Environmental");
            if (int.Parse(TimerQuiet) < 2)
            {
                TimerQuiet = "2";
            };

            var NoiseThresholdOver = MyIni.Read("NoiseOver", "Environmental");
            double ActualNoiseThresholdOver = int.Parse(NoiseThresholdOver);

            // Debug.WriteLine("Quiet Noise Trigger?: " + quietNoiseTrigger.ToString());
            if (!quietNoiseOverTrigger && globalNoiseOverTrigger == "On")
            {
                // Debug.WriteLine("Light Count: " + lightOverCNT.ToString());
                string currentNoiseRegex = Regex.Replace(label_USB_Mon_Noise.Text, @"[^0-9\.]", "");
                double currentNoiseINT = double.Parse(currentNoiseRegex);
                // Debug.WriteLine("Current Light: " + currentLightINT.ToString());

                if (currentNoiseINT > ActualNoiseThresholdOver)
                {
                    noiseOverCNT++;
                }
                else
                {
                    noiseOverCNT = 0;
                }

                // Over Threshold and the Counter has reached 5
                if (noiseOverCNT > 1) { NoiseOverTrigger(TimerQuiet); }
            }
            
        }

        private void NoiseOverTimerCooldown_Tick(object sender, EventArgs e)
        {
            if (quietNoiseOverTrigger && globalNoiseOverTrigger == "On")       // Only if quietNoiseTrigger is set to true, to run this function set
            {
                string iniFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()), "config", "config.ini");
                var MyIni = new IniFile(iniFile);
                var TimerQuiet = MyIni.Read("TimerQuiet", "Environmental");
                if (int.Parse(TimerQuiet) < 1)
                {
                    TimerQuiet = "2";
                };

                // Reset the Timer Details
                int numberOfMins = int.Parse(TimerQuiet);          // Number of Mins
                int TimerShutOff = numberOfMins * 60000;           // numberOfMins * 60 seconds = Total Number of Mins

                // Debug.WriteLine("Noise Cooldown Complete");
                noiseOverCNT = 0;                                   // Reset the noiseOverCNT to 0 for Checker
                quietNoiseOverTrigger = false;                          // Reset the quietNoiseTrigger to false, so it will check again

                NoiseOverTimerCooldown.Interval = TimerShutOff;         // Set the Interval for the Cooldown Timer to new Variable
                NoiseOverTimerCooldown.Enabled = false;                 // Disable Temp Cooldown
            }
            
        }

        private void OmronClose(object sender, FormClosingEventArgs e)
        {
            Webserver2.Stop();
        }
    }
}
