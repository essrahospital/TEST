using System;
using System.IO;
using System.Text;

namespace GlobalCMS
{
    public class CSV
    {
        private string csvPath;
        private Sensor sensor;

        public CSV(Sensor sensor_dev)
        {
            this.sensor = sensor_dev;
            this.csvPath = (string)null;
        }

        public bool SetPath()
        {
            if (!Directory.Exists(Environment.CurrentDirectory + "\\Data"))
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\Data");
            string file_head = "";
            string id = "";
            this.get_devInfo(ref file_head, ref id);
            this.csvPath = Environment.CurrentDirectory + "\\Data\\" + file_head + "_" + id + "_" + DateTime.Now.ToString("yyMMdd_HHmmss") + ".csv";
            return true;
        }

        private void get_devInfo(ref string file_head, ref string id)
        {
            switch (this.sensor.type)
            {
                case Sensor.TYPE.UNKNOWN:
                case Sensor.TYPE.USB:
                    file_head = "USB_MONINTOR";
                    id = this.sensor.serialString;
                    break;
                case Sensor.TYPE.BLE_0x01:
                case Sensor.TYPE.BLE_0x02:
                case Sensor.TYPE.BLE_0x03:
                case Sensor.TYPE.BLE_0x04:
                case Sensor.TYPE.BLE_0x05:
                case Sensor.TYPE.BLE_0x06:
                    file_head = "BLE_BEACON";
                    id = this.sensor.deviceAddress;
                    break;
            }
        }

        public bool SaveInit()
        {
            bool flag = true;
            try
            {
                if (this.csvPath != null)
                {
                    if (!Directory.Exists(Environment.CurrentDirectory + "\\Data"))
                        Directory.CreateDirectory(Environment.CurrentDirectory + "\\Data");
                    TextWriter textWriter = (TextWriter)new StreamWriter((Stream)new BufferedStream((Stream)new FileStream(this.csvPath, FileMode.Append)));
                    StringBuilder stringBuilder = new StringBuilder();
                    string file_head = "";
                    string id = "";
                    this.get_devInfo(ref file_head, ref id);
                    stringBuilder.Append("Date," + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "\r\n");
                    stringBuilder.Append("Sensor ID," + id + "\r\n");
                    stringBuilder.Append("\r\n");
                    // for (int index = 2; index < 34; ++index)
                        // stringBuilder.Append(MainForm.dgvColumnName[index] + ",");
                    textWriter.WriteLine(stringBuilder.ToString());
                    textWriter.Close();
                }
            }
            catch
            {
                return false;
            }
            return flag;
        }

        public bool Save()
        {
            bool flag = true;
            try
            {
                if (this.csvPath == null)
                {
                    flag = this.SetPath();
                    if (flag)
                        this.SaveInit();
                }
                if (flag)
                {
                    if (!File.Exists(this.csvPath))
                        this.SaveInit();
                    TextWriter textWriter = (TextWriter)new StreamWriter((Stream)new BufferedStream((Stream)new FileStream(this.csvPath, FileMode.Append)));
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.Append(this.sensor.lastUpdated.ToString("yyyy/MM/dd HH:mm:ss") + ",");
                    stringBuilder.Append(this.sensor.type.ToString() + ",");
                    stringBuilder.Append(this.sensor.rssi.ToString("D") + ",");
                    stringBuilder.Append(this.sensor.sequenceNum.ToString());
                    for (int index = 0; index < 15; ++index)
                        stringBuilder.Append("," + this.sensor.data.value[index].ToString());
                    stringBuilder.Append("," + this.sensor.data.vibrationState.ToString());
                    for (int index = 0; index < 12; ++index)
                        stringBuilder.Append("," + this.sensor.flag.value[index].ToString());
                    textWriter.WriteLine(stringBuilder.ToString());
                    textWriter.Close();
                }
            }
            catch
            {
                flag = false;
            }
            return flag;
        }
    }
}
