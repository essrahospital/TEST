using System;

namespace GlobalCMS
{
    public class Sensor
    {
        public SensorData data;
        public SensorFlag flag;
        public CSV csv;

        public Sensor.TYPE type { get; set; }

        public string serialString { get; set; }

        public byte sequenceNum { get; set; }

        public byte sequenceNum_sr { get; set; }

        public DateTime lastUpdated { get; set; }

        public short rssi { get; set; }

        public string deviceAddress { get; set; }

        public Sensor(Sensor sensor)
        {
            DateTime now = DateTime.Now;
            this.data = new SensorData();
            this.flag = new SensorFlag();
            this.csv = new CSV(this);
            this.data = sensor.data;
            this.flag = sensor.flag;
            this.serialString = sensor.serialString;
            this.sequenceNum = sensor.sequenceNum;
            this.sequenceNum_sr = sensor.sequenceNum_sr;
            this.lastUpdated = sensor.lastUpdated;
            this.type = sensor.type;
            this.rssi = sensor.rssi;
            this.deviceAddress = sensor.deviceAddress;
        }

        public Sensor()
        {
            DateTime now = DateTime.Now;
            this.data = new SensorData();
            this.flag = new SensorFlag();
            this.csv = new CSV(this);
            this.serialString = (string)null;
            this.sequenceNum = (byte)0;
            this.sequenceNum_sr = (byte)0;
            this.lastUpdated = now;
            this.rssi = (short)0;
            this.type = Sensor.TYPE.UNKNOWN;
            this.deviceAddress = (string)null;
            this.sequenceNum = (byte)0;
            this.lastUpdated = now;
        }

        public void SensorUpdate(Sensor sensor)
        {
            DateTime now = DateTime.Now;
            this.data = sensor.data;
            this.flag = sensor.flag;
            this.serialString = sensor.serialString;
            this.sequenceNum = sensor.sequenceNum;
            this.sequenceNum_sr = sensor.sequenceNum_sr;
            this.lastUpdated = sensor.lastUpdated;
            this.type = sensor.type;
            this.rssi = sensor.rssi;
            this.deviceAddress = sensor.deviceAddress;
            this.sequenceNum = sensor.sequenceNum;
            this.lastUpdated = sensor.lastUpdated;
        }

        public enum TYPE
        {
            UNKNOWN = 0,
            BLE_0x01 = 1,
            BLE_0x02 = 2,
            BLE_0x03 = 3,
            BLE_0x04 = 4,
            BLE_0x05 = 5,
            BLE_0x06 = 6,
            USB = 129, // 0x00000081
        }
    }
}
