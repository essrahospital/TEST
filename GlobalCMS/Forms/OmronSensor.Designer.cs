namespace GlobalCMS
{
    partial class OmronSensor
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ConnectBTN = new System.Windows.Forms.Button();
            this.TemperatureLabel = new System.Windows.Forms.Label();
            this.HumidityLabel = new System.Windows.Forms.Label();
            this.LightLabel = new System.Windows.Forms.Label();
            this.BarometricPressureLabel = new System.Windows.Forms.Label();
            this.label_USB_Mon_Temp = new System.Windows.Forms.Label();
            this.label_USB_Mon_Humi = new System.Windows.Forms.Label();
            this.label_USB_Mon_Light = new System.Windows.Forms.Label();
            this.label_USB_Mon_Press = new System.Windows.Forms.Label();
            this.label_USB_Mon_CO2 = new System.Windows.Forms.Label();
            this.label_USB_Mon_VOC = new System.Windows.Forms.Label();
            this.label_USB_Mon_Noise = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label_USB_Mon_SI = new System.Windows.Forms.Label();
            this.label_USB_Mon_PGA = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label_USB_Mon_Shindo = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label_USB_Mon_AccelZ = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label_USB_Mon_AccelY = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.label_USB_Mon_AccelX = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.ConnectStatus = new System.Windows.Forms.Label();
            this.timer_USB = new System.Windows.Forms.Timer(this.components);
            this.timer_Monitor_USB = new System.Windows.Forms.Timer(this.components);
            this.serialPort = new System.IO.Ports.SerialPort(this.components);
            this.label_USB_SerialNumber = new System.Windows.Forms.Label();
            this.label_USB_ModelNumber = new System.Windows.Forms.Label();
            this.TempOverTimer = new System.Windows.Forms.Timer(this.components);
            this.TempOverTimerCooldown = new System.Windows.Forms.Timer(this.components);
            this.LightOverTimer = new System.Windows.Forms.Timer(this.components);
            this.LightOverTimerCooldown = new System.Windows.Forms.Timer(this.components);
            this.NoiseOverTimer = new System.Windows.Forms.Timer(this.components);
            this.NoiseOverTimerCooldown = new System.Windows.Forms.Timer(this.components);
            this.LightUnderTimer = new System.Windows.Forms.Timer(this.components);
            this.LightUnderTimerCooldown = new System.Windows.Forms.Timer(this.components);
            this.TempUnderTimer = new System.Windows.Forms.Timer(this.components);
            this.TempUnderTimerCooldown = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label_USB_Mon_HSI = new System.Windows.Forms.Label();
            this.label_USB_Mon_DI = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.HSILabelTXT = new System.Windows.Forms.Label();
            this.DILabelTXT = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.NlLabelTxt = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.LlLabelTxt = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.eTVOCLabelTxt = new System.Windows.Forms.Label();
            this.eCO2LabelTxt = new System.Windows.Forms.Label();
            this.ConnectStatusUSB = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // ConnectBTN
            // 
            this.ConnectBTN.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.ConnectBTN.Location = new System.Drawing.Point(6, 19);
            this.ConnectBTN.Name = "ConnectBTN";
            this.ConnectBTN.Size = new System.Drawing.Size(183, 26);
            this.ConnectBTN.TabIndex = 0;
            this.ConnectBTN.Text = "Connect";
            this.ConnectBTN.UseVisualStyleBackColor = true;
            this.ConnectBTN.Click += new System.EventHandler(this.ConnectBTN_Click);
            // 
            // TemperatureLabel
            // 
            this.TemperatureLabel.AutoSize = true;
            this.TemperatureLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.TemperatureLabel.Location = new System.Drawing.Point(10, 16);
            this.TemperatureLabel.Name = "TemperatureLabel";
            this.TemperatureLabel.Size = new System.Drawing.Size(97, 16);
            this.TemperatureLabel.TabIndex = 1;
            this.TemperatureLabel.Text = "Temperature";
            // 
            // HumidityLabel
            // 
            this.HumidityLabel.AutoSize = true;
            this.HumidityLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.HumidityLabel.Location = new System.Drawing.Point(150, 16);
            this.HumidityLabel.Name = "HumidityLabel";
            this.HumidityLabel.Size = new System.Drawing.Size(68, 16);
            this.HumidityLabel.TabIndex = 2;
            this.HumidityLabel.Text = "Humidity";
            // 
            // LightLabel
            // 
            this.LightLabel.AutoSize = true;
            this.LightLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.LightLabel.Location = new System.Drawing.Point(262, 16);
            this.LightLabel.Name = "LightLabel";
            this.LightLabel.Size = new System.Drawing.Size(41, 16);
            this.LightLabel.TabIndex = 3;
            this.LightLabel.Text = "Light";
            // 
            // BarometricPressureLabel
            // 
            this.BarometricPressureLabel.AutoSize = true;
            this.BarometricPressureLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.BarometricPressureLabel.Location = new System.Drawing.Point(10, 63);
            this.BarometricPressureLabel.Name = "BarometricPressureLabel";
            this.BarometricPressureLabel.Size = new System.Drawing.Size(70, 16);
            this.BarometricPressureLabel.TabIndex = 4;
            this.BarometricPressureLabel.Text = "Pressure";
            // 
            // label_USB_Mon_Temp
            // 
            this.label_USB_Mon_Temp.AutoSize = true;
            this.label_USB_Mon_Temp.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.label_USB_Mon_Temp.Location = new System.Drawing.Point(11, 34);
            this.label_USB_Mon_Temp.Name = "label_USB_Mon_Temp";
            this.label_USB_Mon_Temp.Size = new System.Drawing.Size(49, 16);
            this.label_USB_Mon_Temp.TabIndex = 5;
            this.label_USB_Mon_Temp.Text = "00.00 c";
            this.label_USB_Mon_Temp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label_USB_Mon_Humi
            // 
            this.label_USB_Mon_Humi.AutoSize = true;
            this.label_USB_Mon_Humi.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.label_USB_Mon_Humi.Location = new System.Drawing.Point(150, 34);
            this.label_USB_Mon_Humi.Name = "label_USB_Mon_Humi";
            this.label_USB_Mon_Humi.Size = new System.Drawing.Size(53, 16);
            this.label_USB_Mon_Humi.TabIndex = 6;
            this.label_USB_Mon_Humi.Text = "00.00 rh";
            // 
            // label_USB_Mon_Light
            // 
            this.label_USB_Mon_Light.AutoSize = true;
            this.label_USB_Mon_Light.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.label_USB_Mon_Light.Location = new System.Drawing.Point(262, 34);
            this.label_USB_Mon_Light.Name = "label_USB_Mon_Light";
            this.label_USB_Mon_Light.Size = new System.Drawing.Size(27, 16);
            this.label_USB_Mon_Light.TabIndex = 7;
            this.label_USB_Mon_Light.Text = "0 lx";
            // 
            // label_USB_Mon_Press
            // 
            this.label_USB_Mon_Press.AutoSize = true;
            this.label_USB_Mon_Press.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.label_USB_Mon_Press.Location = new System.Drawing.Point(10, 81);
            this.label_USB_Mon_Press.Name = "label_USB_Mon_Press";
            this.label_USB_Mon_Press.Size = new System.Drawing.Size(87, 16);
            this.label_USB_Mon_Press.TabIndex = 8;
            this.label_USB_Mon_Press.Text = "0000.000 hPa";
            // 
            // label_USB_Mon_CO2
            // 
            this.label_USB_Mon_CO2.AutoSize = true;
            this.label_USB_Mon_CO2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.label_USB_Mon_CO2.Location = new System.Drawing.Point(14, 81);
            this.label_USB_Mon_CO2.Name = "label_USB_Mon_CO2";
            this.label_USB_Mon_CO2.Size = new System.Drawing.Size(45, 16);
            this.label_USB_Mon_CO2.TabIndex = 15;
            this.label_USB_Mon_CO2.Text = "0 ppm";
            // 
            // label_USB_Mon_VOC
            // 
            this.label_USB_Mon_VOC.AutoSize = true;
            this.label_USB_Mon_VOC.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.label_USB_Mon_VOC.Location = new System.Drawing.Point(13, 34);
            this.label_USB_Mon_VOC.Name = "label_USB_Mon_VOC";
            this.label_USB_Mon_VOC.Size = new System.Drawing.Size(42, 16);
            this.label_USB_Mon_VOC.TabIndex = 14;
            this.label_USB_Mon_VOC.Text = "0 ppb";
            // 
            // label_USB_Mon_Noise
            // 
            this.label_USB_Mon_Noise.AutoSize = true;
            this.label_USB_Mon_Noise.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.label_USB_Mon_Noise.Location = new System.Drawing.Point(150, 81);
            this.label_USB_Mon_Noise.Name = "label_USB_Mon_Noise";
            this.label_USB_Mon_Noise.Size = new System.Drawing.Size(59, 16);
            this.label_USB_Mon_Noise.TabIndex = 13;
            this.label_USB_Mon_Noise.Text = "00.00 dB";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.label10.Location = new System.Drawing.Point(13, 63);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(46, 16);
            this.label10.TabIndex = 11;
            this.label10.Text = "eCO2";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.label11.Location = new System.Drawing.Point(13, 16);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(58, 16);
            this.label11.TabIndex = 10;
            this.label11.Text = "eTVOC";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.label12.Location = new System.Drawing.Point(150, 63);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(49, 16);
            this.label12.TabIndex = 9;
            this.label12.Text = "Noise";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(10, 23);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(66, 16);
            this.label16.TabIndex = 21;
            this.label16.Text = "SI Value";
            // 
            // label_USB_Mon_SI
            // 
            this.label_USB_Mon_SI.AutoSize = true;
            this.label_USB_Mon_SI.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_USB_Mon_SI.Location = new System.Drawing.Point(10, 41);
            this.label_USB_Mon_SI.Name = "label_USB_Mon_SI";
            this.label_USB_Mon_SI.Size = new System.Drawing.Size(53, 16);
            this.label_USB_Mon_SI.TabIndex = 25;
            this.label_USB_Mon_SI.Text = "0.0 kine";
            // 
            // label_USB_Mon_PGA
            // 
            this.label_USB_Mon_PGA.AutoSize = true;
            this.label_USB_Mon_PGA.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_USB_Mon_PGA.Location = new System.Drawing.Point(220, 41);
            this.label_USB_Mon_PGA.Name = "label_USB_Mon_PGA";
            this.label_USB_Mon_PGA.Size = new System.Drawing.Size(47, 16);
            this.label_USB_Mon_PGA.TabIndex = 27;
            this.label_USB_Mon_PGA.Text = "0.0 gal";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(220, 23);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(39, 16);
            this.label18.TabIndex = 26;
            this.label18.Text = "PGA";
            // 
            // label_USB_Mon_Shindo
            // 
            this.label_USB_Mon_Shindo.AutoSize = true;
            this.label_USB_Mon_Shindo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_USB_Mon_Shindo.Location = new System.Drawing.Point(106, 41);
            this.label_USB_Mon_Shindo.Name = "label_USB_Mon_Shindo";
            this.label_USB_Mon_Shindo.Size = new System.Drawing.Size(39, 16);
            this.label_USB_Mon_Shindo.TabIndex = 29;
            this.label_USB_Mon_Shindo.Text = "0.000";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.Location = new System.Drawing.Point(105, 23);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(83, 16);
            this.label21.TabIndex = 28;
            this.label21.Text = "Seismic Int";
            // 
            // label_USB_Mon_AccelZ
            // 
            this.label_USB_Mon_AccelZ.AutoSize = true;
            this.label_USB_Mon_AccelZ.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_USB_Mon_AccelZ.Location = new System.Drawing.Point(489, 41);
            this.label_USB_Mon_AccelZ.Name = "label_USB_Mon_AccelZ";
            this.label_USB_Mon_AccelZ.Size = new System.Drawing.Size(47, 16);
            this.label_USB_Mon_AccelZ.TabIndex = 35;
            this.label_USB_Mon_AccelZ.Text = "0.0 gal";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.Location = new System.Drawing.Point(488, 23);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(55, 16);
            this.label23.TabIndex = 34;
            this.label23.Text = "Acc : Z";
            // 
            // label_USB_Mon_AccelY
            // 
            this.label_USB_Mon_AccelY.AutoSize = true;
            this.label_USB_Mon_AccelY.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_USB_Mon_AccelY.Location = new System.Drawing.Point(401, 41);
            this.label_USB_Mon_AccelY.Name = "label_USB_Mon_AccelY";
            this.label_USB_Mon_AccelY.Size = new System.Drawing.Size(47, 16);
            this.label_USB_Mon_AccelY.TabIndex = 33;
            this.label_USB_Mon_AccelY.Text = "0.0 gal";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(401, 23);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(56, 16);
            this.label25.TabIndex = 32;
            this.label25.Text = "Acc : Y";
            // 
            // label_USB_Mon_AccelX
            // 
            this.label_USB_Mon_AccelX.AutoSize = true;
            this.label_USB_Mon_AccelX.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_USB_Mon_AccelX.Location = new System.Drawing.Point(309, 41);
            this.label_USB_Mon_AccelX.Name = "label_USB_Mon_AccelX";
            this.label_USB_Mon_AccelX.Size = new System.Drawing.Size(47, 16);
            this.label_USB_Mon_AccelX.TabIndex = 31;
            this.label_USB_Mon_AccelX.Text = "0.0 gal";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label27.Location = new System.Drawing.Point(309, 23);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(55, 16);
            this.label27.TabIndex = 30;
            this.label27.Text = "Acc : X";
            // 
            // ConnectStatus
            // 
            this.ConnectStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ConnectStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.ConnectStatus.Location = new System.Drawing.Point(218, 22);
            this.ConnectStatus.Name = "ConnectStatus";
            this.ConnectStatus.Size = new System.Drawing.Size(112, 21);
            this.ConnectStatus.TabIndex = 36;
            this.ConnectStatus.Text = "Disconnected";
            this.ConnectStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timer_USB
            // 
            this.timer_USB.Tick += new System.EventHandler(this.Timer_USB_Tick);
            // 
            // serialPort
            // 
            this.serialPort.BaudRate = 115200;
            this.serialPort.PortName = "COM10";
            // 
            // label_USB_SerialNumber
            // 
            this.label_USB_SerialNumber.Location = new System.Drawing.Point(39, 47);
            this.label_USB_SerialNumber.Name = "label_USB_SerialNumber";
            this.label_USB_SerialNumber.Size = new System.Drawing.Size(86, 21);
            this.label_USB_SerialNumber.TabIndex = 37;
            this.label_USB_SerialNumber.Text = "Serial";
            this.label_USB_SerialNumber.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_USB_ModelNumber
            // 
            this.label_USB_ModelNumber.Location = new System.Drawing.Point(191, 47);
            this.label_USB_ModelNumber.Name = "label_USB_ModelNumber";
            this.label_USB_ModelNumber.Size = new System.Drawing.Size(86, 21);
            this.label_USB_ModelNumber.TabIndex = 38;
            this.label_USB_ModelNumber.Text = "Model";
            this.label_USB_ModelNumber.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // TempOverTimer
            // 
            this.TempOverTimer.Interval = 15000;
            this.TempOverTimer.Tick += new System.EventHandler(this.TempOverTimer_Tick);
            // 
            // TempOverTimerCooldown
            // 
            this.TempOverTimerCooldown.Interval = 60000;
            this.TempOverTimerCooldown.Tick += new System.EventHandler(this.TempOverTimerCooldown_Tick);
            // 
            // LightOverTimer
            // 
            this.LightOverTimer.Interval = 15000;
            this.LightOverTimer.Tick += new System.EventHandler(this.LightOverTimer_Tick);
            // 
            // LightOverTimerCooldown
            // 
            this.LightOverTimerCooldown.Interval = 60000;
            this.LightOverTimerCooldown.Tick += new System.EventHandler(this.LightTimerOverCooldown_Tick);
            // 
            // NoiseOverTimer
            // 
            this.NoiseOverTimer.Interval = 15000;
            this.NoiseOverTimer.Tick += new System.EventHandler(this.NoiseTimer_Tick);
            // 
            // NoiseOverTimerCooldown
            // 
            this.NoiseOverTimerCooldown.Interval = 60000;
            this.NoiseOverTimerCooldown.Tick += new System.EventHandler(this.NoiseOverTimerCooldown_Tick);
            // 
            // LightUnderTimer
            // 
            this.LightUnderTimer.Interval = 15000;
            this.LightUnderTimer.Tick += new System.EventHandler(this.LightUnderTimer_Tick);
            // 
            // LightUnderTimerCooldown
            // 
            this.LightUnderTimerCooldown.Interval = 60000;
            this.LightUnderTimerCooldown.Tick += new System.EventHandler(this.LightUnderTimerCooldown_Tick);
            // 
            // TempUnderTimer
            // 
            this.TempUnderTimer.Interval = 15000;
            this.TempUnderTimer.Tick += new System.EventHandler(this.TempUnderTimer_Tick);
            // 
            // TempUnderTimerCooldown
            // 
            this.TempUnderTimerCooldown.Interval = 60000;
            this.TempUnderTimerCooldown.Tick += new System.EventHandler(this.TempUnderTimerCooldown_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 16);
            this.label1.TabIndex = 39;
            this.label1.Text = "Heat Stroke Risk";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(6, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(123, 16);
            this.label2.TabIndex = 40;
            this.label2.Text = "Discomfort Index";
            // 
            // label_USB_Mon_HSI
            // 
            this.label_USB_Mon_HSI.AutoSize = true;
            this.label_USB_Mon_HSI.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.label_USB_Mon_HSI.Location = new System.Drawing.Point(171, 16);
            this.label_USB_Mon_HSI.Name = "label_USB_Mon_HSI";
            this.label_USB_Mon_HSI.Size = new System.Drawing.Size(32, 16);
            this.label_USB_Mon_HSI.TabIndex = 41;
            this.label_USB_Mon_HSI.Text = "0.00";
            // 
            // label_USB_Mon_DI
            // 
            this.label_USB_Mon_DI.AutoSize = true;
            this.label_USB_Mon_DI.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.label_USB_Mon_DI.Location = new System.Drawing.Point(171, 36);
            this.label_USB_Mon_DI.Name = "label_USB_Mon_DI";
            this.label_USB_Mon_DI.Size = new System.Drawing.Size(32, 16);
            this.label_USB_Mon_DI.TabIndex = 42;
            this.label_USB_Mon_DI.Text = "0.00";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(6, 51);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 13);
            this.label3.TabIndex = 43;
            this.label3.Text = "S/N";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(157, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 44;
            this.label4.Text = "M/N";
            // 
            // HSILabelTXT
            // 
            this.HSILabelTXT.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.HSILabelTXT.Location = new System.Drawing.Point(244, 14);
            this.HSILabelTXT.Name = "HSILabelTXT";
            this.HSILabelTXT.Size = new System.Drawing.Size(100, 18);
            this.HSILabelTXT.TabIndex = 45;
            this.HSILabelTXT.Text = "N/A";
            this.HSILabelTXT.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DILabelTXT
            // 
            this.DILabelTXT.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DILabelTXT.Location = new System.Drawing.Point(244, 34);
            this.DILabelTXT.Name = "DILabelTXT";
            this.DILabelTXT.Size = new System.Drawing.Size(100, 18);
            this.DILabelTXT.TabIndex = 46;
            this.DILabelTXT.Text = "N/A";
            this.DILabelTXT.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.NlLabelTxt);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.LlLabelTxt);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.DILabelTXT);
            this.groupBox1.Controls.Add(this.label_USB_Mon_HSI);
            this.groupBox1.Controls.Add(this.HSILabelTXT);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label_USB_Mon_DI);
            this.groupBox1.Location = new System.Drawing.Point(12, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(350, 106);
            this.groupBox1.TabIndex = 47;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Dynamic Data";
            // 
            // NlLabelTxt
            // 
            this.NlLabelTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NlLabelTxt.Location = new System.Drawing.Point(244, 55);
            this.NlLabelTxt.Name = "NlLabelTxt";
            this.NlLabelTxt.Size = new System.Drawing.Size(100, 18);
            this.NlLabelTxt.TabIndex = 52;
            this.NlLabelTxt.Text = "N/A";
            this.NlLabelTxt.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.label9.Location = new System.Drawing.Point(6, 59);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(91, 16);
            this.label9.TabIndex = 50;
            this.label9.Text = "Noise Level";
            // 
            // LlLabelTxt
            // 
            this.LlLabelTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LlLabelTxt.Location = new System.Drawing.Point(244, 77);
            this.LlLabelTxt.Name = "LlLabelTxt";
            this.LlLabelTxt.Size = new System.Drawing.Size(100, 18);
            this.LlLabelTxt.TabIndex = 49;
            this.LlLabelTxt.Text = "N/A";
            this.LlLabelTxt.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.label6.Location = new System.Drawing.Point(6, 81);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(104, 16);
            this.label6.TabIndex = 47;
            this.label6.Text = "Lighting Level";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.label_USB_Mon_SI);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.label_USB_Mon_PGA);
            this.groupBox2.Controls.Add(this.label21);
            this.groupBox2.Controls.Add(this.label_USB_Mon_Shindo);
            this.groupBox2.Controls.Add(this.label27);
            this.groupBox2.Controls.Add(this.label_USB_Mon_AccelZ);
            this.groupBox2.Controls.Add(this.label_USB_Mon_AccelX);
            this.groupBox2.Controls.Add(this.label23);
            this.groupBox2.Controls.Add(this.label25);
            this.groupBox2.Controls.Add(this.label_USB_Mon_AccelY);
            this.groupBox2.Location = new System.Drawing.Point(368, 121);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(581, 70);
            this.groupBox2.TabIndex = 48;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Vibration Information";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.ConnectStatusUSB);
            this.groupBox3.Controls.Add(this.ConnectBTN);
            this.groupBox3.Controls.Add(this.ConnectStatus);
            this.groupBox3.Controls.Add(this.label_USB_SerialNumber);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.label_USB_ModelNumber);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Location = new System.Drawing.Point(12, 121);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(350, 70);
            this.groupBox3.TabIndex = 49;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Controls";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.TemperatureLabel);
            this.groupBox4.Controls.Add(this.label_USB_Mon_Temp);
            this.groupBox4.Controls.Add(this.HumidityLabel);
            this.groupBox4.Controls.Add(this.label_USB_Mon_Humi);
            this.groupBox4.Controls.Add(this.BarometricPressureLabel);
            this.groupBox4.Controls.Add(this.label_USB_Mon_Press);
            this.groupBox4.Controls.Add(this.label_USB_Mon_Noise);
            this.groupBox4.Controls.Add(this.LightLabel);
            this.groupBox4.Controls.Add(this.label_USB_Mon_Light);
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Location = new System.Drawing.Point(368, 9);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(348, 106);
            this.groupBox4.TabIndex = 50;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Sensor Data";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.eCO2LabelTxt);
            this.groupBox5.Controls.Add(this.eTVOCLabelTxt);
            this.groupBox5.Controls.Add(this.label11);
            this.groupBox5.Controls.Add(this.label_USB_Mon_VOC);
            this.groupBox5.Controls.Add(this.label10);
            this.groupBox5.Controls.Add(this.label_USB_Mon_CO2);
            this.groupBox5.Location = new System.Drawing.Point(722, 9);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(227, 106);
            this.groupBox5.TabIndex = 51;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Air Quality";
            // 
            // eTVOCLabelTxt
            // 
            this.eTVOCLabelTxt.AutoSize = true;
            this.eTVOCLabelTxt.Location = new System.Drawing.Point(152, 36);
            this.eTVOCLabelTxt.Name = "eTVOCLabelTxt";
            this.eTVOCLabelTxt.Size = new System.Drawing.Size(27, 13);
            this.eTVOCLabelTxt.TabIndex = 16;
            this.eTVOCLabelTxt.Text = "N/A";
            // 
            // eCO2LabelTxt
            // 
            this.eCO2LabelTxt.AutoSize = true;
            this.eCO2LabelTxt.Location = new System.Drawing.Point(152, 83);
            this.eCO2LabelTxt.Name = "eCO2LabelTxt";
            this.eCO2LabelTxt.Size = new System.Drawing.Size(27, 13);
            this.eCO2LabelTxt.TabIndex = 17;
            this.eCO2LabelTxt.Text = "N/A";
            // 
            // ConnectStatusUSB
            // 
            this.ConnectStatusUSB.AutoSize = true;
            this.ConnectStatusUSB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ConnectStatusUSB.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.ConnectStatusUSB.Location = new System.Drawing.Point(313, 51);
            this.ConnectStatusUSB.Name = "ConnectStatusUSB";
            this.ConnectStatusUSB.Size = new System.Drawing.Size(32, 13);
            this.ConnectStatusUSB.TabIndex = 45;
            this.ConnectStatusUSB.Text = "USB";
            // 
            // OmronSensor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(959, 200);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.Name = "OmronSensor";
            this.ShowIcon = false;
            this.Text = "OmronSensor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OmronClose);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ConnectBTN;
        private System.Windows.Forms.Label TemperatureLabel;
        private System.Windows.Forms.Label HumidityLabel;
        private System.Windows.Forms.Label LightLabel;
        private System.Windows.Forms.Label BarometricPressureLabel;
        private System.Windows.Forms.Label label_USB_Mon_Temp;
        private System.Windows.Forms.Label label_USB_Mon_Humi;
        private System.Windows.Forms.Label label_USB_Mon_Light;
        private System.Windows.Forms.Label label_USB_Mon_Press;
        private System.Windows.Forms.Label label_USB_Mon_CO2;
        private System.Windows.Forms.Label label_USB_Mon_VOC;
        private System.Windows.Forms.Label label_USB_Mon_Noise;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label_USB_Mon_SI;
        private System.Windows.Forms.Label label_USB_Mon_PGA;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label_USB_Mon_Shindo;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label_USB_Mon_AccelZ;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label_USB_Mon_AccelY;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.Label label_USB_Mon_AccelX;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.Label ConnectStatus;
        private System.Windows.Forms.Timer timer_USB;
        private System.Windows.Forms.Timer timer_Monitor_USB;
        private System.IO.Ports.SerialPort serialPort;
        private System.Windows.Forms.Label label_USB_SerialNumber;
        private System.Windows.Forms.Label label_USB_ModelNumber;
        private System.Windows.Forms.Timer TempOverTimer;
        private System.Windows.Forms.Timer TempOverTimerCooldown;
        private System.Windows.Forms.Timer LightOverTimer;
        private System.Windows.Forms.Timer LightOverTimerCooldown;
        private System.Windows.Forms.Timer NoiseOverTimer;
        private System.Windows.Forms.Timer NoiseOverTimerCooldown;
        private System.Windows.Forms.Timer LightUnderTimer;
        private System.Windows.Forms.Timer LightUnderTimerCooldown;
        private System.Windows.Forms.Timer TempUnderTimer;
        private System.Windows.Forms.Timer TempUnderTimerCooldown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label_USB_Mon_HSI;
        private System.Windows.Forms.Label label_USB_Mon_DI;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label HSILabelTXT;
        private System.Windows.Forms.Label DILabelTXT;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label NlLabelTxt;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label LlLabelTxt;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label eCO2LabelTxt;
        private System.Windows.Forms.Label eTVOCLabelTxt;
        private System.Windows.Forms.Label ConnectStatusUSB;
    }
}