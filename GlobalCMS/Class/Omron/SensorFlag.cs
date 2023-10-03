namespace GlobalCMS
{
    public class SensorFlag
    {
        public const int TYPE_NUM = 12;
        public ushort[] value;

        public SensorFlag()
        {
            this.value = new ushort[12];
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
        }
    }
}
