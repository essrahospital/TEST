namespace GlobalCMS
{
    public class SensorData
    {
        public static readonly string[] vibration_state = new string[3]
        {
      "None",
      "Vibration",
      "Earthquake"
        };
        public static readonly string[] order = new string[15]
        {
      "f2",
      "f2",
      "f0",
      "f3",
      "f2",
      "f0",
      "f0",
      "f2",
      "f2",
      "f1",
      "f1",
      "f3",
      "f1",
      "f1",
      "f1"
        };
        public const int TYPE_NUM = 15;
        public SensorData.VIB_STATE vibrationState;
        public double[] value;

        public SensorData()
        {
            this.value = new double[15];
            this.vibrationState = SensorData.VIB_STATE.NONE;
        }

        public enum VIB_STATE
        {
            NONE,
            VIBRATION,
            QUAKE,
        }

        public enum TYPE
        {
            TEMP,
            HUMI,
            LIGHT,
            PRESSURE,
            NOISE,
            VOC,
            CO2,
            DI,
            HEAT,
            SI,
            PGA,
            SHINDO,
            ACCELX,
            ACCELY,
            ACCELZ,
        }
    }
}
