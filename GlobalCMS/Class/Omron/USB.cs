using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Management;
using System.Threading;

namespace GlobalCMS
{
    public class USB
    {
        public static Mutex mutex = new Mutex();
        private static readonly ushort[] crcTable = new ushort[256]
        {
      (ushort) 0,
      (ushort) 49345,
      (ushort) 49537,
      (ushort) 320,
      (ushort) 49921,
      (ushort) 960,
      (ushort) 640,
      (ushort) 49729,
      (ushort) 50689,
      (ushort) 1728,
      (ushort) 1920,
      (ushort) 51009,
      (ushort) 1280,
      (ushort) 50625,
      (ushort) 50305,
      (ushort) 1088,
      (ushort) 52225,
      (ushort) 3264,
      (ushort) 3456,
      (ushort) 52545,
      (ushort) 3840,
      (ushort) 53185,
      (ushort) 52865,
      (ushort) 3648,
      (ushort) 2560,
      (ushort) 51905,
      (ushort) 52097,
      (ushort) 2880,
      (ushort) 51457,
      (ushort) 2496,
      (ushort) 2176,
      (ushort) 51265,
      (ushort) 55297,
      (ushort) 6336,
      (ushort) 6528,
      (ushort) 55617,
      (ushort) 6912,
      (ushort) 56257,
      (ushort) 55937,
      (ushort) 6720,
      (ushort) 7680,
      (ushort) 57025,
      (ushort) 57217,
      (ushort) 8000,
      (ushort) 56577,
      (ushort) 7616,
      (ushort) 7296,
      (ushort) 56385,
      (ushort) 5120,
      (ushort) 54465,
      (ushort) 54657,
      (ushort) 5440,
      (ushort) 55041,
      (ushort) 6080,
      (ushort) 5760,
      (ushort) 54849,
      (ushort) 53761,
      (ushort) 4800,
      (ushort) 4992,
      (ushort) 54081,
      (ushort) 4352,
      (ushort) 53697,
      (ushort) 53377,
      (ushort) 4160,
      (ushort) 61441,
      (ushort) 12480,
      (ushort) 12672,
      (ushort) 61761,
      (ushort) 13056,
      (ushort) 62401,
      (ushort) 62081,
      (ushort) 12864,
      (ushort) 13824,
      (ushort) 63169,
      (ushort) 63361,
      (ushort) 14144,
      (ushort) 62721,
      (ushort) 13760,
      (ushort) 13440,
      (ushort) 62529,
      (ushort) 15360,
      (ushort) 64705,
      (ushort) 64897,
      (ushort) 15680,
      (ushort) 65281,
      (ushort) 16320,
      (ushort) 16000,
      (ushort) 65089,
      (ushort) 64001,
      (ushort) 15040,
      (ushort) 15232,
      (ushort) 64321,
      (ushort) 14592,
      (ushort) 63937,
      (ushort) 63617,
      (ushort) 14400,
      (ushort) 10240,
      (ushort) 59585,
      (ushort) 59777,
      (ushort) 10560,
      (ushort) 60161,
      (ushort) 11200,
      (ushort) 10880,
      (ushort) 59969,
      (ushort) 60929,
      (ushort) 11968,
      (ushort) 12160,
      (ushort) 61249,
      (ushort) 11520,
      (ushort) 60865,
      (ushort) 60545,
      (ushort) 11328,
      (ushort) 58369,
      (ushort) 9408,
      (ushort) 9600,
      (ushort) 58689,
      (ushort) 9984,
      (ushort) 59329,
      (ushort) 59009,
      (ushort) 9792,
      (ushort) 8704,
      (ushort) 58049,
      (ushort) 58241,
      (ushort) 9024,
      (ushort) 57601,
      (ushort) 8640,
      (ushort) 8320,
      (ushort) 57409,
      (ushort) 40961,
      (ushort) 24768,
      (ushort) 24960,
      (ushort) 41281,
      (ushort) 25344,
      (ushort) 41921,
      (ushort) 41601,
      (ushort) 25152,
      (ushort) 26112,
      (ushort) 42689,
      (ushort) 42881,
      (ushort) 26432,
      (ushort) 42241,
      (ushort) 26048,
      (ushort) 25728,
      (ushort) 42049,
      (ushort) 27648,
      (ushort) 44225,
      (ushort) 44417,
      (ushort) 27968,
      (ushort) 44801,
      (ushort) 28608,
      (ushort) 28288,
      (ushort) 44609,
      (ushort) 43521,
      (ushort) 27328,
      (ushort) 27520,
      (ushort) 43841,
      (ushort) 26880,
      (ushort) 43457,
      (ushort) 43137,
      (ushort) 26688,
      (ushort) 30720,
      (ushort) 47297,
      (ushort) 47489,
      (ushort) 31040,
      (ushort) 47873,
      (ushort) 31680,
      (ushort) 31360,
      (ushort) 47681,
      (ushort) 48641,
      (ushort) 32448,
      (ushort) 32640,
      (ushort) 48961,
      (ushort) 32000,
      (ushort) 48577,
      (ushort) 48257,
      (ushort) 31808,
      (ushort) 46081,
      (ushort) 29888,
      (ushort) 30080,
      (ushort) 46401,
      (ushort) 30464,
      (ushort) 47041,
      (ushort) 46721,
      (ushort) 30272,
      (ushort) 29184,
      (ushort) 45761,
      (ushort) 45953,
      (ushort) 29504,
      (ushort) 45313,
      (ushort) 29120,
      (ushort) 28800,
      (ushort) 45121,
      (ushort) 20480,
      (ushort) 37057,
      (ushort) 37249,
      (ushort) 20800,
      (ushort) 37633,
      (ushort) 21440,
      (ushort) 21120,
      (ushort) 37441,
      (ushort) 38401,
      (ushort) 22208,
      (ushort) 22400,
      (ushort) 38721,
      (ushort) 21760,
      (ushort) 38337,
      (ushort) 38017,
      (ushort) 21568,
      (ushort) 39937,
      (ushort) 23744,
      (ushort) 23936,
      (ushort) 40257,
      (ushort) 24320,
      (ushort) 40897,
      (ushort) 40577,
      (ushort) 24128,
      (ushort) 23040,
      (ushort) 39617,
      (ushort) 39809,
      (ushort) 23360,
      (ushort) 39169,
      (ushort) 22976,
      (ushort) 22656,
      (ushort) 38977,
      (ushort) 34817,
      (ushort) 18624,
      (ushort) 18816,
      (ushort) 35137,
      (ushort) 19200,
      (ushort) 35777,
      (ushort) 35457,
      (ushort) 19008,
      (ushort) 19968,
      (ushort) 36545,
      (ushort) 36737,
      (ushort) 20288,
      (ushort) 36097,
      (ushort) 19904,
      (ushort) 19584,
      (ushort) 35905,
      (ushort) 17408,
      (ushort) 33985,
      (ushort) 34177,
      (ushort) 17728,
      (ushort) 34561,
      (ushort) 18368,
      (ushort) 18048,
      (ushort) 34369,
      (ushort) 33281,
      (ushort) 17088,
      (ushort) 17280,
      (ushort) 33601,
      (ushort) 16640,
      (ushort) 33217,
      (ushort) 32897,
      (ushort) 16448
        };
        public const byte HEADER0 = 82;
        public const byte HEADER1 = 66;
        private SerialPort serialPort;

        public USB(SerialPort sp)
        {
            this.serialPort = sp;
        }

        public List<USB.VComPortInfo> FindVirtualComPorts()
        {
            List<USB.VComPortInfo> vcomPortInfoList = new List<USB.VComPortInfo>();
            ConnectionOptions connectionOptions = new ConnectionOptions();
            connectionOptions.Impersonation = ImpersonationLevel.Impersonate;
            connectionOptions.Authentication = AuthenticationLevel.Default;
            connectionOptions.EnablePrivileges = true;
            ManagementScope scope = new ManagementScope();
            scope.Path = new ManagementPath("\\\\" + Environment.MachineName + "\\root\\CIMV2");
            scope.Options = connectionOptions;
            scope.Connect();
            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(scope, new ObjectQuery("SELECT * FROM Win32_PnPEntity WHERE ConfigManagerErrorCode = 0"));
            using (managementObjectSearcher)
            {
                foreach (ManagementObject managementObject in managementObjectSearcher.Get())
                {
                    if (managementObject != null)
                    {
                        object obj1 = managementObject["Caption"];
                        if (obj1 != null)
                        {
                            string description = obj1.ToString();
                            if (description.Contains("(COM"))
                            {
                                string portName = description.Substring(description.LastIndexOf("(COM")).Replace("(", string.Empty).Replace(")", string.Empty);
                                object obj2 = managementObject["DeviceID"];
                                if (obj2 != null)
                                {
                                    string str = obj2.ToString();
                                    if (str.Contains("VID_") && str.Contains("PID_"))
                                    {
                                        ushort uint16_1 = Convert.ToUInt16(str.Substring(str.LastIndexOf("VID_"), 8).Replace("VID_", string.Empty), 16);
                                        ushort uint16_2 = Convert.ToUInt16(str.Substring(str.LastIndexOf("PID_"), 8).Replace("PID_", string.Empty), 16);
                                        object obj3 = managementObject["Manufacturer"];
                                        string manufacturer = obj3 == null ? "Unknown" : obj3.ToString();
                                        USB.VComPortInfo vcomPortInfo = new USB.VComPortInfo(portName, description, manufacturer, uint16_1, uint16_2);
                                        vcomPortInfoList.Add(vcomPortInfo);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return vcomPortInfoList;
        }

        public bool PortOpen(string port)
        {
            try
            {
                if (this.serialPort.IsOpen)
                    this.serialPort.Close();
                this.serialPort.PortName = port;
                this.serialPort.ReadBufferSize = 4096;
                this.serialPort.DataBits = 8;
                this.serialPort.DtrEnable = false;
                this.serialPort.RtsEnable = false;
                this.serialPort.BaudRate = 115200;
                this.serialPort.Handshake = Handshake.None;
                this.serialPort.StopBits = StopBits.One;
                this.serialPort.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool PortClose()
        {
            try
            {
                if (!this.serialPort.IsOpen)
                    return false;
                this.serialPort.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Receive(ref byte[] rxBuff, ref ushort len, int timeout_ms)
        {
            this.serialPort.ReadTimeout = timeout_ms;
            try
            {
                long tickCount1 = (long)Environment.TickCount;
                while (this.serialPort.BytesToRead < 4)
                {
                    Thread.Sleep(1);
                    if ((long)Environment.TickCount - tickCount1 > 3000L)
                        return false;
                }
                this.serialPort.Read(rxBuff, 0, 4);
                if (rxBuff[0] != (byte)82 || rxBuff[1] != (byte)66)
                    return false;
                len = (ushort)((uint)rxBuff[3] << 8 | (uint)rxBuff[2]);
                long tickCount2 = (long)Environment.TickCount;
                while (this.serialPort.BytesToRead < (int)len)
                {
                    Thread.Sleep(1);
                    if ((long)Environment.TickCount - tickCount2 > 3000L)
                        return false;
                }
                this.serialPort.Read(rxBuff, 4, (int)len);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Send(byte[] data)
        {
            try
            {
                USB.mutex.WaitOne();
                this.ClearBuffer();
                this.serialPort.Write(data, 0, data.Length);
            }
            catch
            {
            }
            finally
            {
                USB.mutex.ReleaseMutex();
            }
        }

        private void ClearBuffer()
        {
            if (!this.serialPort.IsOpen)
                return;
            this.serialPort.DiscardInBuffer();
            this.serialPort.DiscardOutBuffer();
        }

        public enum ADDRESS : ushort
        {
            COMMON_DEVICE_INFO = 6154, // 0x180A
            COMMON_LATEST_MEMORY_INFO = 20484, // 0x5004
            USB_ORG_MEMORY_DATA_LONG = 20494, // 0x500E
            USB_ORG_MEMORY_DATA_SHORT = 20495, // 0x500F
            COMMON_LATEST_SENSING_DATA = 20498, // 0x5012
            COMMON_LATEST_CALC_DATA = 20499, // 0x5013
            COMMON_LATEST_SENSING_FLAG = 20500, // 0x5014
            COMMON_LATEST_CALC_FLAG = 20501, // 0x5015
            COMMON_LATEST_ACCEL_STATUS = 20502, // 0x5016
            USB_ORG_LATEST_DATA_LONG = 20513, // 0x5021
            USB_ORG_LATEST_DATA_SHORT = 20514, // 0x5022
            COMMON_VIBRATION_COUNT = 20529, // 0x5031
            USB_ORG_ACCEL_MEMORY_DATA_HEADER = 20542, // 0x503E
            USB_ORG_ACCEL_MEMORY_DATA = 20543, // 0x503F
            COMMON_LED_SETTING_NORMAL = 20753, // 0x5111
            COMMON_LED_SETTING_EVENT = 20754, // 0x5112
            COMMON_LED_SETTING_OPERATION = 20755, // 0x5113
            COMMON_INSTALLATION_OFFSET = 20756, // 0x5114
            COMMON_ADVERTISE_SETTING = 20757, // 0x5115
            COMMON_MEMORY_RESET = 20758, // 0x5116
            COMMON_MODE_CHANGE = 20759, // 0x5117
            COMMON_ACCEL_LOGGER_CONTROL = 20760, // 0x5118
            COMMON_ACCEL_LOGGER_STATUS = 20761, // 0x5119
            COMMON_DFU_MODE = 20762, // 0x511A
            COMMON_LATEST_TIME_COUNTER = 20993, // 0x5201
            COMMON_TIME_SETTING = 20994, // 0x5202
            COMMON_MEMORY_STORAGE_INTERVAL = 20995, // 0x5203
            COMMON_EVENT_PATTERN_SENSOR_1 = 21009, // 0x5211
            COMMON_EVENT_PATTERN_SENSOR_2 = 21010, // 0x5212
            COMMON_EVENT_PATTERN_ACCEL = 21030, // 0x5226
            COMMON_ERROR_STATUS = 21505, // 0x5401
            COMMON_MOUNTING_ORIENTAION = 21506, // 0x5402
        }

        public enum COMMAND : byte
        {
            NONE = 0,
            READ = 1,
            WRITE = 2,
            READ_ERROR = 129, // 0x81
            WRITE_ERROR = 130, // 0x82
            UNKNOWN = 255, // 0xFF
        }

        public enum ERRCODE : byte
        {
            SUCCESS,
            CRC,
            COMMAND,
            ADDRESS,
            LENGTH,
            DATA,
            BUSY,
        }

        public class VComPortInfo
        {
            public VComPortInfo()
            {
            }

            public VComPortInfo(
              string portName,
              string description,
              string manufacturer,
              ushort vid,
              ushort pid)
            {
                this.PortName = portName;
                this.Description = description;
                this.Manufacturer = manufacturer;
                this.Vid = vid;
                this.Pid = pid;
            }

            public string PortName { get; private set; }

            public string Description { get; private set; }

            public string Manufacturer { get; private set; }

            public ushort Vid { get; private set; }

            public ushort Pid { get; private set; }
        }

        public class Frame
        {
            public byte[] frame;
            public ushort length;
            public ushort payloadLength;
            public byte[] payload;
            public ushort crc;
            public USB.COMMAND command;
            public ushort address;
            public byte[] data;
            public ushort dataLength;
            public bool errorFlag;
            public USB.ERRCODE errorCode;

            public Frame()
            {
                this.errorFlag = false;
                this.errorCode = USB.ERRCODE.SUCCESS;
            }

            public void SetFrame(USB.COMMAND cmd, USB.ADDRESS address, byte[] data, ushort dataLength)
            {
                this.payloadLength = (ushort)((uint)dataLength + 3U);
                this.length = (ushort)((uint)this.payloadLength + 2U);
                this.payload = new byte[(int)this.payloadLength];
                this.payload[0] = (byte)cmd;
                this.payload[1] = (byte)(address & (USB.ADDRESS)255);
                this.payload[2] = (byte)((uint)(address & (USB.ADDRESS)65280) >> 8);
                Buffer.BlockCopy((Array)data, 0, (Array)this.payload, 3, (int)dataLength);
                this.frame = new byte[(int)this.length + 4];
                this.frame[0] = (byte)82;
                this.frame[1] = (byte)66;
                this.frame[2] = (byte)((uint)this.length & (uint)byte.MaxValue);
                this.frame[3] = (byte)(((int)this.length & 65280) >> 8);
                Buffer.BlockCopy((Array)this.payload, 0, (Array)this.frame, 4, (int)this.payloadLength);
                this.crc = this.CalcCrc(this.frame, (ushort)((uint)this.payloadLength + 4U));
                this.frame[4 + (int)this.payloadLength] = (byte)((uint)this.crc & (uint)byte.MaxValue);
                this.frame[5 + (int)this.payloadLength] = (byte)(((int)this.crc & 65280) >> 8);
            }

            public void SetFrame(USB.COMMAND cmd, USB.ADDRESS address)
            {
                this.payloadLength = (ushort)3;
                this.length = (ushort)((uint)this.payloadLength + 2U);
                this.payload = new byte[(int)this.payloadLength];
                this.payload[0] = (byte)cmd;
                this.payload[1] = (byte)(address & (USB.ADDRESS)255);
                this.payload[2] = (byte)((uint)(address & (USB.ADDRESS)65280) >> 8);
                this.frame = new byte[(int)this.length + 4];
                this.frame[0] = (byte)82;
                this.frame[1] = (byte)66;
                this.frame[2] = (byte)((uint)this.length & (uint)byte.MaxValue);
                this.frame[3] = (byte)(((int)this.length & 65280) >> 8);
                Buffer.BlockCopy((Array)this.payload, 0, (Array)this.frame, 4, (int)this.payloadLength);
                this.crc = this.CalcCrc(this.frame, (ushort)((uint)this.payloadLength + 4U));
                this.frame[4 + (int)this.payloadLength] = (byte)((uint)this.crc & (uint)byte.MaxValue);
                this.frame[5 + (int)this.payloadLength] = (byte)(((int)this.crc & 65280) >> 8);
            }

            public void ParsePacket(byte[] packet_with_header, ushort frameLength)
            {
                this.frame = new byte[(int)frameLength + 4];
                Buffer.BlockCopy((Array)packet_with_header, 0, (Array)this.frame, 0, this.frame.Length);
                this.length = frameLength;
                this.payloadLength = (ushort)((uint)this.length - 2U);
                this.crc = (ushort)((uint)this.frame[(int)this.payloadLength + 5] << 8 | (uint)this.frame[(int)this.payloadLength + 4]);
                this.payload = new byte[(int)this.payloadLength];
                ushort num = this.CalcCrc(this.frame, (ushort)((uint)this.payloadLength + 4U));
                Buffer.BlockCopy((Array)this.frame, 4, (Array)this.payload, 0, (int)this.payloadLength);
                this.command = (USB.COMMAND)this.payload[0];
                this.address = (ushort)((uint)this.payload[2] << 8 | (uint)this.payload[1]);
                this.data = new byte[(int)this.payloadLength - 3];
                Buffer.BlockCopy((Array)this.payload, 3, (Array)this.data, 0, (int)this.payloadLength - 3);
                if (((int)this.payload[0] & 128) == 128)
                {
                    this.errorCode = (USB.ERRCODE)this.data[0];
                    this.errorFlag = true;
                }
                if ((int)this.crc == (int)num)
                    return;
                this.errorCode = USB.ERRCODE.CRC;
                this.errorFlag = true;
            }

            private ushort CalcCrc(byte[] bytes, ushort length)
            {
                ushort num = ushort.MaxValue;
                byte[] numArray = new byte[2];
                for (int index = 0; index < (int)length; ++index)
                {
                    byte[] bytes1 = BitConverter.GetBytes((ushort)((uint)num ^ (uint)bytes[index]));
                    num = (ushort)((uint)USB.crcTable[(int)bytes1[0]] ^ (uint)bytes1[1]);
                }
                return num;
            }
        }
    }
}
