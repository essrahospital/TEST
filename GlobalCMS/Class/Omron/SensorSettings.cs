using System;
using System.IO;
using System.Xml.Serialization;

namespace GlobalCMS
{
    public class Setting
    {
        [NonSerialized]
        private static Setting _instance;

        public int DisplayResolution { get; set; }

        public int AvgNum { get; set; }

        public int RefeshInterval { get; set; }

        public bool AutoScale { get; set; }

        public double Chart_Temp_YMAX { get; set; }

        public double Chart_Temp_YMIN { get; set; }

        public double Chart_Humi_YMAX { get; set; }

        public double Chart_Humi_YMIN { get; set; }

        public double Chart_Light_YMAX { get; set; }

        public double Chart_Light_YMIN { get; set; }

        public double Chart_Pressure_YMAX { get; set; }

        public double Chart_Pressure_YMIN { get; set; }

        public double Chart_DI_YMAX { get; set; }

        public double Chart_DI_YMIN { get; set; }

        public double Chart_Heat_YMAX { get; set; }

        public double Chart_Heat_YMIN { get; set; }

        public double Chart_Noise_YMAX { get; set; }

        public double Chart_Noise_YMIN { get; set; }

        public double Chart_VOC_YMAX { get; set; }

        public double Chart_VOC_YMIN { get; set; }

        public double Chart_CO2_YMAX { get; set; }

        public double Chart_CO2_YMIN { get; set; }

        public double Chart_SI_YMAX { get; set; }

        public double Chart_SI_YMIN { get; set; }

        public double Chart_PGA_YMAX { get; set; }

        public double Chart_PGA_YMIN { get; set; }

        public double Chart_Accel_YMAX { get; set; }

        public double Chart_Accel_YMIN { get; set; }

        public Setting()
        {
            this.DisplayResolution = 0;
            this.AvgNum = 10;
            this.RefeshInterval = 100;
            this.AutoScale = true;
            this.Chart_Temp_YMIN = 18.0;
            this.Chart_Temp_YMAX = 32.0;
            this.Chart_Humi_YMIN = 20.0;
            this.Chart_Humi_YMAX = 90.0;
            this.Chart_Light_YMIN = 0.0;
            this.Chart_Light_YMAX = 2000.0;
            this.Chart_Pressure_YMIN = 970.0;
            this.Chart_Pressure_YMAX = 1100.0;
            this.Chart_Noise_YMIN = 30.0;
            this.Chart_Noise_YMAX = 90.0;
            this.Chart_Accel_YMIN = -1600.0;
            this.Chart_Accel_YMAX = 1600.0;
            this.Chart_DI_YMIN = 30.0;
            this.Chart_DI_YMAX = 95.0;
            this.Chart_Heat_YMIN = 10.0;
            this.Chart_Heat_YMAX = 50.0;
            this.Chart_VOC_YMIN = 0.0;
            this.Chart_VOC_YMAX = 1000.0;
            this.Chart_CO2_YMIN = 300.0;
            this.Chart_CO2_YMAX = 6000.0;
            this.Chart_SI_YMIN = 0.0;
            this.Chart_SI_YMAX = 150.0;
            this.Chart_PGA_YMIN = 0.0;
            this.Chart_PGA_YMAX = 4000.0;
        }

        [XmlIgnore]
        public static Setting Instance
        {
            get
            {
                if (Setting._instance == null)
                    Setting._instance = new Setting();
                return Setting._instance;
            }
            set
            {
                Setting._instance = value;
            }
        }

        public static void LoadFromXmlFile(string path)
        {
            FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            object obj = new XmlSerializer(typeof(Setting)).Deserialize((Stream)fileStream);
            fileStream.Close();
            Setting.Instance = (Setting)obj;
        }

        public static void SaveToXmlFile(string path)
        {
            FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write);
            new XmlSerializer(typeof(Setting)).Serialize((Stream)fileStream, (object)Setting.Instance);
            fileStream.Close();
        }
    }
}
