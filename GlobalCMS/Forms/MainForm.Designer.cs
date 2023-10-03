using System.Drawing;

namespace GlobalCMS
{
    partial class MainForm
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
            this.CallHomeTimer = new System.Windows.Forms.Timer(this.components);
            this.systemDetailsGroupBox = new System.Windows.Forms.GroupBox();
            this.devOS = new System.Windows.Forms.Label();
            this.TrialLicTxt = new System.Windows.Forms.Label();
            this.devMAC = new System.Windows.Forms.Label();
            this.devUUID = new System.Windows.Forms.Label();
            this.devName = new System.Windows.Forms.Label();
            this.devMACLabel = new System.Windows.Forms.Label();
            this.devUUIDLabel = new System.Windows.Forms.Label();
            this.devNameLabel = new System.Windows.Forms.Label();
            TaskbarIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.TaskbarContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.taskbarAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.taskbarLANLabel = new System.Windows.Forms.ToolStripTextBox();
            this.taskbarWANLabel = new System.Windows.Forms.ToolStripTextBox();
            this.taskbarVPNLabel = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.taskbarShutdownService = new System.Windows.Forms.ToolStripMenuItem();
            this.IPDetailsGroupBox = new System.Windows.Forms.GroupBox();
            this.devWAN = new System.Windows.Forms.Label();
            this.devVPN = new System.Windows.Forms.Label();
            this.devLAN = new System.Windows.Forms.Label();
            this.devWANLabel = new System.Windows.Forms.Label();
            this.devVPNLabel = new System.Windows.Forms.Label();
            this.devLANLabel = new System.Windows.Forms.Label();
            this.hardwareDetailsGroupBox = new System.Windows.Forms.GroupBox();
            this.TestBrowserBTN = new System.Windows.Forms.Button();
            this.ComPortsBTN = new System.Windows.Forms.Button();
            this.SignageDebugBTN = new System.Windows.Forms.Button();
            this.MaintModeBTN = new System.Windows.Forms.Button();
            this.ramLoadLabel = new System.Windows.Forms.Label();
            this.cpuLoadLabel = new System.Windows.Forms.Label();
            this.hddAmounts = new System.Windows.Forms.Label();
            this.hddLabel = new System.Windows.Forms.Label();
            this.gfxCard = new System.Windows.Forms.Label();
            this.gfxCardLabel = new System.Windows.Forms.Label();
            this.CPUArch = new System.Windows.Forms.Label();
            this.ramAmount = new System.Windows.Forms.Label();
            this.ramLabel = new System.Windows.Forms.Label();
            this.cpuLabel = new System.Windows.Forms.Label();
            this.CheckStatsTimer = new System.Windows.Forms.Timer(this.components);
            this.SystemCheckGroupBox = new System.Windows.Forms.GroupBox();
            this.systemNetworkOpt = new System.Windows.Forms.Label();
            this.RunningModeLabel = new System.Windows.Forms.Label();
            this.systemNetworkLabel = new System.Windows.Forms.Label();
            this.SignageSystemOpt = new System.Windows.Forms.Label();
            this.SecureConnectionOpt = new System.Windows.Forms.Label();
            this.powerModeLabel = new System.Windows.Forms.Label();
            this.InternetConnectionOpt = new System.Windows.Forms.Label();
            this.SignageSystemLabel = new System.Windows.Forms.Label();
            this.SecureConnectionLabel = new System.Windows.Forms.Label();
            this.InternetConnectionLabel = new System.Windows.Forms.Label();
            this.LastHeartbeatLabel = new System.Windows.Forms.Label();
            this.CheckServicesTimer = new System.Windows.Forms.Timer(this.components);
            this.ReceivedTriggersTimer = new System.Windows.Forms.Timer(this.components);
            this.CheckSNAP = new System.Windows.Forms.Timer(this.components);
            this.DataLoggingTimer = new System.Windows.Forms.Timer(this.components);
            this.LowTimer = new System.Windows.Forms.Timer(this.components);
            this.PerfCheckGroupBox = new System.Windows.Forms.GroupBox();
            this.NEXOpt = new System.Windows.Forms.Label();
            this.NEXLabel = new System.Windows.Forms.Label();
            this.SCROpt = new System.Windows.Forms.Label();
            this.SCRLabel = new System.Windows.Forms.Label();
            this.WSKOpt = new System.Windows.Forms.Label();
            this.WSKLabel = new System.Windows.Forms.Label();
            this.AVAOpt = new System.Windows.Forms.Label();
            this.AVALabel = new System.Windows.Forms.Label();
            this.SENOpt = new System.Windows.Forms.Label();
            this.SENLabel = new System.Windows.Forms.Label();
            this.LastLogMsgLabel = new System.Windows.Forms.Label();
            this.LastLogMsgOpt = new System.Windows.Forms.Label();
            this.USBDevicesGroupBox = new System.Windows.Forms.GroupBox();
            this.SensorOpt = new System.Windows.Forms.Label();
            this.SensorLabel = new System.Windows.Forms.Label();
            this.USBSensorDebug = new System.Windows.Forms.PictureBox();
            this.RS232Opt = new System.Windows.Forms.Label();
            this.RS232Label = new System.Windows.Forms.Label();
            this.RS232Picture = new System.Windows.Forms.PictureBox();
            this.WiFiCardOpt = new System.Windows.Forms.Label();
            this.WiFiLabel = new System.Windows.Forms.Label();
            this.USBDriveOpt = new System.Windows.Forms.Label();
            this.USBDriveLabel = new System.Windows.Forms.Label();
            this.InternetOpt = new System.Windows.Forms.Label();
            this.InternetLabel = new System.Windows.Forms.Label();
            this.TouchscreenOpt = new System.Windows.Forms.Label();
            this.TouchscreenLabel = new System.Windows.Forms.Label();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.CheckForInteractive = new System.Windows.Forms.Timer(this.components);
            this.CheckForNewSignage = new System.Windows.Forms.Timer(this.components);
            LogoBottomCorner = new System.Windows.Forms.PictureBox();
            this.LauncherTimer = new System.Windows.Forms.Timer(this.components);
            this.CheckTrial = new System.Windows.Forms.Timer(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.LastWebsocketLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.CurRunningVerTxt = new System.Windows.Forms.Label();
            this.systemDetailsGroupBox.SuspendLayout();
            this.TaskbarContextMenu.SuspendLayout();
            this.IPDetailsGroupBox.SuspendLayout();
            this.hardwareDetailsGroupBox.SuspendLayout();
            this.SystemCheckGroupBox.SuspendLayout();
            this.PerfCheckGroupBox.SuspendLayout();
            this.USBDevicesGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.USBSensorDebug)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RS232Picture)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(LogoBottomCorner)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // CallHomeTimer
            // 
            this.CallHomeTimer.Enabled = true;
            this.CallHomeTimer.Interval = 60000;
            this.CallHomeTimer.Tick += new System.EventHandler(this.CallHomeTimer_Tick);
            // 
            // systemDetailsGroupBox
            // 
            this.systemDetailsGroupBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.systemDetailsGroupBox.Controls.Add(this.devOS);
            this.systemDetailsGroupBox.Controls.Add(this.TrialLicTxt);
            this.systemDetailsGroupBox.Controls.Add(this.devMAC);
            this.systemDetailsGroupBox.Controls.Add(this.devUUID);
            this.systemDetailsGroupBox.Controls.Add(this.devName);
            this.systemDetailsGroupBox.Controls.Add(this.devMACLabel);
            this.systemDetailsGroupBox.Controls.Add(this.devUUIDLabel);
            this.systemDetailsGroupBox.Controls.Add(this.devNameLabel);
            this.systemDetailsGroupBox.Location = new System.Drawing.Point(12, 9);
            this.systemDetailsGroupBox.Name = "systemDetailsGroupBox";
            this.systemDetailsGroupBox.Size = new System.Drawing.Size(309, 109);
            this.systemDetailsGroupBox.TabIndex = 0;
            this.systemDetailsGroupBox.TabStop = false;
            this.systemDetailsGroupBox.Text = "System Details";
            // 
            // devOS
            // 
            this.devOS.AutoSize = true;
            this.devOS.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.devOS.Location = new System.Drawing.Point(133, 38);
            this.devOS.Name = "devOS";
            this.devOS.Size = new System.Drawing.Size(10, 13);
            this.devOS.TabIndex = 12;
            this.devOS.Text = " ";
            // 
            // TrialLicTxt
            // 
            this.TrialLicTxt.AutoSize = true;
            this.TrialLicTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TrialLicTxt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.TrialLicTxt.Location = new System.Drawing.Point(-4, -4);
            this.TrialLicTxt.Name = "TrialLicTxt";
            this.TrialLicTxt.Size = new System.Drawing.Size(141, 20);
            this.TrialLicTxt.TabIndex = 24;
            this.TrialLicTxt.Text = "TRIAL LICENCE";
            this.TrialLicTxt.Visible = false;
            // 
            // devMAC
            // 
            this.devMAC.AutoSize = true;
            this.devMAC.Location = new System.Drawing.Point(133, 80);
            this.devMAC.Name = "devMAC";
            this.devMAC.Size = new System.Drawing.Size(10, 13);
            this.devMAC.TabIndex = 5;
            this.devMAC.Text = " ";
            // 
            // devUUID
            // 
            this.devUUID.AutoSize = true;
            this.devUUID.Location = new System.Drawing.Point(133, 59);
            this.devUUID.Name = "devUUID";
            this.devUUID.Size = new System.Drawing.Size(10, 13);
            this.devUUID.TabIndex = 5;
            this.devUUID.Text = " ";
            // 
            // devName
            // 
            this.devName.AutoSize = true;
            this.devName.Location = new System.Drawing.Point(133, 19);
            this.devName.Name = "devName";
            this.devName.Size = new System.Drawing.Size(10, 13);
            this.devName.TabIndex = 4;
            this.devName.Text = " ";
            // 
            // devMACLabel
            // 
            this.devMACLabel.AutoSize = true;
            this.devMACLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.devMACLabel.Location = new System.Drawing.Point(35, 80);
            this.devMACLabel.Name = "devMACLabel";
            this.devMACLabel.Size = new System.Drawing.Size(77, 13);
            this.devMACLabel.TabIndex = 3;
            this.devMACLabel.Text = "Device MAC";
            // 
            // devUUIDLabel
            // 
            this.devUUIDLabel.AutoSize = true;
            this.devUUIDLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.devUUIDLabel.Location = new System.Drawing.Point(30, 59);
            this.devUUIDLabel.Name = "devUUIDLabel";
            this.devUUIDLabel.Size = new System.Drawing.Size(82, 13);
            this.devUUIDLabel.TabIndex = 2;
            this.devUUIDLabel.Text = "Device UUID";
            // 
            // devNameLabel
            // 
            this.devNameLabel.AutoSize = true;
            this.devNameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.devNameLabel.Location = new System.Drawing.Point(29, 19);
            this.devNameLabel.Name = "devNameLabel";
            this.devNameLabel.Size = new System.Drawing.Size(83, 13);
            this.devNameLabel.TabIndex = 1;
            this.devNameLabel.Text = "Device Name";
            // 
            // TaskbarIcon
            // 
            TaskbarIcon.ContextMenuStrip = this.TaskbarContextMenu;
            TaskbarIcon.Text = "GlobalCMS Monitoring Solution";
            TaskbarIcon.Visible = true;
            // 
            // TaskbarContextMenu
            // 
            this.TaskbarContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.taskbarAbout,
            this.toolStripSeparator3,
            this.taskbarLANLabel,
            this.taskbarWANLabel,
            this.taskbarVPNLabel,
            this.toolStripSeparator2,
            this.taskbarShutdownService});
            this.TaskbarContextMenu.Name = "TaskbarContextMenu";
            this.TaskbarContextMenu.Size = new System.Drawing.Size(218, 114);
            // 
            // taskbarAbout
            // 
            this.taskbarAbout.Name = "taskbarAbout";
            this.taskbarAbout.Size = new System.Drawing.Size(217, 22);
            this.taskbarAbout.Text = "About Monitor";
            this.taskbarAbout.ToolTipText = "About GlobalCMS Monitoring Solution";
            this.taskbarAbout.Click += new System.EventHandler(this.TaskbarAbout_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(214, 6);
            // 
            // taskbarLANLabel
            // 
            this.taskbarLANLabel.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.taskbarLANLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.taskbarLANLabel.Enabled = false;
            this.taskbarLANLabel.Name = "taskbarLANLabel";
            this.taskbarLANLabel.ReadOnly = true;
            this.taskbarLANLabel.Size = new System.Drawing.Size(150, 16);
            // 
            // taskbarWANLabel
            // 
            this.taskbarWANLabel.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.taskbarWANLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.taskbarWANLabel.Enabled = false;
            this.taskbarWANLabel.Name = "taskbarWANLabel";
            this.taskbarWANLabel.ReadOnly = true;
            this.taskbarWANLabel.Size = new System.Drawing.Size(150, 16);
            // 
            // taskbarVPNLabel
            // 
            this.taskbarVPNLabel.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.taskbarVPNLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.taskbarVPNLabel.Enabled = false;
            this.taskbarVPNLabel.Name = "taskbarVPNLabel";
            this.taskbarVPNLabel.ReadOnly = true;
            this.taskbarVPNLabel.Size = new System.Drawing.Size(150, 16);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(214, 6);
            // 
            // taskbarShutdownService
            // 
            this.taskbarShutdownService.Name = "taskbarShutdownService";
            this.taskbarShutdownService.Size = new System.Drawing.Size(217, 22);
            this.taskbarShutdownService.Text = "Shutdown Service (30mins)";
            this.taskbarShutdownService.ToolTipText = "Stop Service & Shutdown Application";
            this.taskbarShutdownService.Click += new System.EventHandler(this.TaskbarShutdownService_Click);
            // 
            // IPDetailsGroupBox
            // 
            this.IPDetailsGroupBox.Controls.Add(this.devWAN);
            this.IPDetailsGroupBox.Controls.Add(this.devVPN);
            this.IPDetailsGroupBox.Controls.Add(this.devLAN);
            this.IPDetailsGroupBox.Controls.Add(this.devWANLabel);
            this.IPDetailsGroupBox.Controls.Add(this.devVPNLabel);
            this.IPDetailsGroupBox.Controls.Add(this.devLANLabel);
            this.IPDetailsGroupBox.Location = new System.Drawing.Point(328, 9);
            this.IPDetailsGroupBox.Name = "IPDetailsGroupBox";
            this.IPDetailsGroupBox.Size = new System.Drawing.Size(206, 109);
            this.IPDetailsGroupBox.TabIndex = 5;
            this.IPDetailsGroupBox.TabStop = false;
            this.IPDetailsGroupBox.Text = "IP Details";
            // 
            // devWAN
            // 
            this.devWAN.AutoSize = true;
            this.devWAN.Location = new System.Drawing.Point(94, 80);
            this.devWAN.Name = "devWAN";
            this.devWAN.Size = new System.Drawing.Size(10, 13);
            this.devWAN.TabIndex = 5;
            this.devWAN.Text = " ";
            // 
            // devVPN
            // 
            this.devVPN.AutoSize = true;
            this.devVPN.Location = new System.Drawing.Point(94, 53);
            this.devVPN.Name = "devVPN";
            this.devVPN.Size = new System.Drawing.Size(10, 13);
            this.devVPN.TabIndex = 5;
            this.devVPN.Text = " ";
            // 
            // devLAN
            // 
            this.devLAN.AutoSize = true;
            this.devLAN.Location = new System.Drawing.Point(94, 26);
            this.devLAN.Name = "devLAN";
            this.devLAN.Size = new System.Drawing.Size(10, 13);
            this.devLAN.TabIndex = 4;
            this.devLAN.Text = " ";
            // 
            // devWANLabel
            // 
            this.devWANLabel.AutoSize = true;
            this.devWANLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.devWANLabel.Location = new System.Drawing.Point(22, 80);
            this.devWANLabel.Name = "devWANLabel";
            this.devWANLabel.Size = new System.Drawing.Size(52, 13);
            this.devWANLabel.TabIndex = 3;
            this.devWANLabel.Text = "WAN IP";
            // 
            // devVPNLabel
            // 
            this.devVPNLabel.AutoSize = true;
            this.devVPNLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.devVPNLabel.Location = new System.Drawing.Point(26, 53);
            this.devVPNLabel.Name = "devVPNLabel";
            this.devVPNLabel.Size = new System.Drawing.Size(48, 13);
            this.devVPNLabel.TabIndex = 2;
            this.devVPNLabel.Text = "VPN IP";
            // 
            // devLANLabel
            // 
            this.devLANLabel.AutoSize = true;
            this.devLANLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.devLANLabel.Location = new System.Drawing.Point(27, 26);
            this.devLANLabel.Name = "devLANLabel";
            this.devLANLabel.Size = new System.Drawing.Size(47, 13);
            this.devLANLabel.TabIndex = 1;
            this.devLANLabel.Text = "LAN IP";
            // 
            // hardwareDetailsGroupBox
            // 
            this.hardwareDetailsGroupBox.Controls.Add(this.TestBrowserBTN);
            this.hardwareDetailsGroupBox.Controls.Add(this.ComPortsBTN);
            this.hardwareDetailsGroupBox.Controls.Add(this.SignageDebugBTN);
            this.hardwareDetailsGroupBox.Controls.Add(this.MaintModeBTN);
            this.hardwareDetailsGroupBox.Controls.Add(this.ramLoadLabel);
            this.hardwareDetailsGroupBox.Controls.Add(this.cpuLoadLabel);
            this.hardwareDetailsGroupBox.Controls.Add(this.hddAmounts);
            this.hardwareDetailsGroupBox.Controls.Add(this.hddLabel);
            this.hardwareDetailsGroupBox.Controls.Add(this.gfxCard);
            this.hardwareDetailsGroupBox.Controls.Add(this.gfxCardLabel);
            this.hardwareDetailsGroupBox.Controls.Add(this.CPUArch);
            this.hardwareDetailsGroupBox.Controls.Add(this.ramAmount);
            this.hardwareDetailsGroupBox.Controls.Add(this.ramLabel);
            this.hardwareDetailsGroupBox.Controls.Add(this.cpuLabel);
            this.hardwareDetailsGroupBox.Location = new System.Drawing.Point(12, 123);
            this.hardwareDetailsGroupBox.Name = "hardwareDetailsGroupBox";
            this.hardwareDetailsGroupBox.Size = new System.Drawing.Size(522, 110);
            this.hardwareDetailsGroupBox.TabIndex = 6;
            this.hardwareDetailsGroupBox.TabStop = false;
            this.hardwareDetailsGroupBox.Text = "Hardware Details";
            // 
            // TestBrowserBTN
            // 
            this.TestBrowserBTN.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.TestBrowserBTN.Location = new System.Drawing.Point(308, 75);
            this.TestBrowserBTN.Name = "TestBrowserBTN";
            this.TestBrowserBTN.Size = new System.Drawing.Size(39, 23);
            this.TestBrowserBTN.TabIndex = 14;
            this.TestBrowserBTN.Text = "EO";
            this.TestBrowserBTN.UseVisualStyleBackColor = true;
            this.TestBrowserBTN.Click += new System.EventHandler(this.TestBrowserBTN_Click);
            // 
            // ComPortsBTN
            // 
            this.ComPortsBTN.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ComPortsBTN.Location = new System.Drawing.Point(349, 75);
            this.ComPortsBTN.Name = "ComPortsBTN";
            this.ComPortsBTN.Size = new System.Drawing.Size(43, 23);
            this.ComPortsBTN.TabIndex = 13;
            this.ComPortsBTN.Text = "COM";
            this.ComPortsBTN.UseVisualStyleBackColor = true;
            this.ComPortsBTN.Click += new System.EventHandler(this.ComPortsBTN_Click);
            // 
            // SignageDebugBTN
            // 
            this.SignageDebugBTN.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SignageDebugBTN.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SignageDebugBTN.Location = new System.Drawing.Point(394, 75);
            this.SignageDebugBTN.Margin = new System.Windows.Forms.Padding(0);
            this.SignageDebugBTN.Name = "SignageDebugBTN";
            this.SignageDebugBTN.Size = new System.Drawing.Size(30, 23);
            this.SignageDebugBTN.TabIndex = 12;
            this.SignageDebugBTN.Text = "Inf";
            this.SignageDebugBTN.UseVisualStyleBackColor = true;
            this.SignageDebugBTN.Click += new System.EventHandler(this.SignageDebugBTN_Click);
            // 
            // MaintModeBTN
            // 
            this.MaintModeBTN.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.MaintModeBTN.Location = new System.Drawing.Point(426, 75);
            this.MaintModeBTN.Name = "MaintModeBTN";
            this.MaintModeBTN.Size = new System.Drawing.Size(86, 23);
            this.MaintModeBTN.TabIndex = 11;
            this.MaintModeBTN.Text = "Maintenance";
            this.MaintModeBTN.UseVisualStyleBackColor = true;
            this.MaintModeBTN.Click += new System.EventHandler(this.MaintModeBTN_Click);
            // 
            // ramLoadLabel
            // 
            this.ramLoadLabel.Location = new System.Drawing.Point(426, 43);
            this.ramLoadLabel.Name = "ramLoadLabel";
            this.ramLoadLabel.Size = new System.Drawing.Size(86, 13);
            this.ramLoadLabel.TabIndex = 10;
            this.ramLoadLabel.Text = "% Load";
            this.ramLoadLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cpuLoadLabel
            // 
            this.cpuLoadLabel.Location = new System.Drawing.Point(426, 21);
            this.cpuLoadLabel.Name = "cpuLoadLabel";
            this.cpuLoadLabel.Size = new System.Drawing.Size(86, 13);
            this.cpuLoadLabel.TabIndex = 9;
            this.cpuLoadLabel.Text = " % Load";
            this.cpuLoadLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // hddAmounts
            // 
            this.hddAmounts.AutoSize = true;
            this.hddAmounts.Location = new System.Drawing.Point(146, 85);
            this.hddAmounts.Name = "hddAmounts";
            this.hddAmounts.Size = new System.Drawing.Size(10, 13);
            this.hddAmounts.TabIndex = 8;
            this.hddAmounts.Text = " ";
            // 
            // hddLabel
            // 
            this.hddLabel.AutoSize = true;
            this.hddLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hddLabel.Location = new System.Drawing.Point(37, 86);
            this.hddLabel.Name = "hddLabel";
            this.hddLabel.Size = new System.Drawing.Size(74, 13);
            this.hddLabel.TabIndex = 7;
            this.hddLabel.Text = "Hard Drives";
            // 
            // gfxCard
            // 
            this.gfxCard.AutoSize = true;
            this.gfxCard.Location = new System.Drawing.Point(146, 65);
            this.gfxCard.Name = "gfxCard";
            this.gfxCard.Size = new System.Drawing.Size(10, 13);
            this.gfxCard.TabIndex = 7;
            this.gfxCard.Text = " ";
            // 
            // gfxCardLabel
            // 
            this.gfxCardLabel.AutoSize = true;
            this.gfxCardLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gfxCardLabel.Location = new System.Drawing.Point(25, 65);
            this.gfxCardLabel.Name = "gfxCardLabel";
            this.gfxCardLabel.Size = new System.Drawing.Size(87, 13);
            this.gfxCardLabel.TabIndex = 6;
            this.gfxCardLabel.Text = "Graphics Card";
            // 
            // CPUArch
            // 
            this.CPUArch.AutoSize = true;
            this.CPUArch.Location = new System.Drawing.Point(146, 21);
            this.CPUArch.Name = "CPUArch";
            this.CPUArch.Size = new System.Drawing.Size(10, 13);
            this.CPUArch.TabIndex = 5;
            this.CPUArch.Text = " ";
            // 
            // ramAmount
            // 
            this.ramAmount.AutoSize = true;
            this.ramAmount.Location = new System.Drawing.Point(146, 43);
            this.ramAmount.Name = "ramAmount";
            this.ramAmount.Size = new System.Drawing.Size(10, 13);
            this.ramAmount.TabIndex = 4;
            this.ramAmount.Text = " ";
            // 
            // ramLabel
            // 
            this.ramLabel.AutoSize = true;
            this.ramLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ramLabel.Location = new System.Drawing.Point(23, 43);
            this.ramLabel.Name = "ramLabel";
            this.ramLabel.Size = new System.Drawing.Size(89, 13);
            this.ramLabel.TabIndex = 1;
            this.ramLabel.Text = "RAM (Memory)";
            // 
            // cpuLabel
            // 
            this.cpuLabel.AutoSize = true;
            this.cpuLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cpuLabel.Location = new System.Drawing.Point(35, 21);
            this.cpuLabel.Name = "cpuLabel";
            this.cpuLabel.Size = new System.Drawing.Size(77, 13);
            this.cpuLabel.TabIndex = 2;
            this.cpuLabel.Text = "Processor(s)";
            // 
            // CheckStatsTimer
            // 
            this.CheckStatsTimer.Enabled = true;
            this.CheckStatsTimer.Interval = 60000;
            this.CheckStatsTimer.Tick += new System.EventHandler(this.CheckStatsTimer_Tick);
            // 
            // SystemCheckGroupBox
            // 
            this.SystemCheckGroupBox.Controls.Add(this.systemNetworkOpt);
            this.SystemCheckGroupBox.Controls.Add(this.RunningModeLabel);
            this.SystemCheckGroupBox.Controls.Add(this.systemNetworkLabel);
            this.SystemCheckGroupBox.Controls.Add(this.SignageSystemOpt);
            this.SystemCheckGroupBox.Controls.Add(this.SecureConnectionOpt);
            this.SystemCheckGroupBox.Controls.Add(this.powerModeLabel);
            this.SystemCheckGroupBox.Controls.Add(this.InternetConnectionOpt);
            this.SystemCheckGroupBox.Controls.Add(this.SignageSystemLabel);
            this.SystemCheckGroupBox.Controls.Add(this.SecureConnectionLabel);
            this.SystemCheckGroupBox.Controls.Add(this.InternetConnectionLabel);
            this.SystemCheckGroupBox.Location = new System.Drawing.Point(12, 235);
            this.SystemCheckGroupBox.Name = "SystemCheckGroupBox";
            this.SystemCheckGroupBox.Size = new System.Drawing.Size(295, 125);
            this.SystemCheckGroupBox.TabIndex = 7;
            this.SystemCheckGroupBox.TabStop = false;
            this.SystemCheckGroupBox.Text = "System Check";
            // 
            // systemNetworkOpt
            // 
            this.systemNetworkOpt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.systemNetworkOpt.ForeColor = System.Drawing.Color.Black;
            this.systemNetworkOpt.Location = new System.Drawing.Point(160, 39);
            this.systemNetworkOpt.Name = "systemNetworkOpt";
            this.systemNetworkOpt.Size = new System.Drawing.Size(121, 18);
            this.systemNetworkOpt.TabIndex = 8;
            this.systemNetworkOpt.Text = "S";
            this.systemNetworkOpt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // RunningModeLabel
            // 
            this.RunningModeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RunningModeLabel.Location = new System.Drawing.Point(17, 20);
            this.RunningModeLabel.Name = "RunningModeLabel";
            this.RunningModeLabel.Size = new System.Drawing.Size(121, 18);
            this.RunningModeLabel.TabIndex = 12;
            this.RunningModeLabel.Text = "Running Mode";
            this.RunningModeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // systemNetworkLabel
            // 
            this.systemNetworkLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.systemNetworkLabel.Location = new System.Drawing.Point(17, 39);
            this.systemNetworkLabel.Name = "systemNetworkLabel";
            this.systemNetworkLabel.Size = new System.Drawing.Size(121, 18);
            this.systemNetworkLabel.TabIndex = 7;
            this.systemNetworkLabel.Text = "System Network";
            this.systemNetworkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SignageSystemOpt
            // 
            this.SignageSystemOpt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SignageSystemOpt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.SignageSystemOpt.Location = new System.Drawing.Point(160, 99);
            this.SignageSystemOpt.Name = "SignageSystemOpt";
            this.SignageSystemOpt.Size = new System.Drawing.Size(121, 18);
            this.SignageSystemOpt.TabIndex = 6;
            this.SignageSystemOpt.Text = "Offline";
            this.SignageSystemOpt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SecureConnectionOpt
            // 
            this.SecureConnectionOpt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SecureConnectionOpt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.SecureConnectionOpt.Location = new System.Drawing.Point(160, 79);
            this.SecureConnectionOpt.Name = "SecureConnectionOpt";
            this.SecureConnectionOpt.Size = new System.Drawing.Size(121, 18);
            this.SecureConnectionOpt.TabIndex = 5;
            this.SecureConnectionOpt.Text = "Offline";
            this.SecureConnectionOpt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // powerModeLabel
            // 
            this.powerModeLabel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.powerModeLabel.BackColor = System.Drawing.Color.Transparent;
            this.powerModeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.powerModeLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.powerModeLabel.Location = new System.Drawing.Point(160, 20);
            this.powerModeLabel.Name = "powerModeLabel";
            this.powerModeLabel.Size = new System.Drawing.Size(121, 18);
            this.powerModeLabel.TabIndex = 11;
            this.powerModeLabel.Text = "Normal / Online";
            this.powerModeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // InternetConnectionOpt
            // 
            this.InternetConnectionOpt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InternetConnectionOpt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.InternetConnectionOpt.Location = new System.Drawing.Point(160, 59);
            this.InternetConnectionOpt.Name = "InternetConnectionOpt";
            this.InternetConnectionOpt.Size = new System.Drawing.Size(121, 18);
            this.InternetConnectionOpt.TabIndex = 4;
            this.InternetConnectionOpt.Text = "Offline";
            this.InternetConnectionOpt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SignageSystemLabel
            // 
            this.SignageSystemLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SignageSystemLabel.Location = new System.Drawing.Point(17, 99);
            this.SignageSystemLabel.Name = "SignageSystemLabel";
            this.SignageSystemLabel.Size = new System.Drawing.Size(121, 18);
            this.SignageSystemLabel.TabIndex = 3;
            this.SignageSystemLabel.Text = "Signage Connection";
            this.SignageSystemLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SecureConnectionLabel
            // 
            this.SecureConnectionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SecureConnectionLabel.Location = new System.Drawing.Point(17, 79);
            this.SecureConnectionLabel.Name = "SecureConnectionLabel";
            this.SecureConnectionLabel.Size = new System.Drawing.Size(121, 18);
            this.SecureConnectionLabel.TabIndex = 2;
            this.SecureConnectionLabel.Text = "Secure Connection";
            this.SecureConnectionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // InternetConnectionLabel
            // 
            this.InternetConnectionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InternetConnectionLabel.Location = new System.Drawing.Point(17, 59);
            this.InternetConnectionLabel.Name = "InternetConnectionLabel";
            this.InternetConnectionLabel.Size = new System.Drawing.Size(121, 18);
            this.InternetConnectionLabel.TabIndex = 1;
            this.InternetConnectionLabel.Text = "Internet Connection";
            this.InternetConnectionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // LastHeartbeatLabel
            // 
            this.LastHeartbeatLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LastHeartbeatLabel.Location = new System.Drawing.Point(16, 70);
            this.LastHeartbeatLabel.Name = "LastHeartbeatLabel";
            this.LastHeartbeatLabel.Size = new System.Drawing.Size(247, 17);
            this.LastHeartbeatLabel.TabIndex = 8;
            this.LastHeartbeatLabel.Text = "      ";
            // 
            // CheckServicesTimer
            // 
            this.CheckServicesTimer.Enabled = true;
            this.CheckServicesTimer.Interval = 60000;
            this.CheckServicesTimer.Tick += new System.EventHandler(this.CheckServicesTimer_Tick);
            // 
            // ReceivedTriggersTimer
            // 
            this.ReceivedTriggersTimer.Enabled = true;
            this.ReceivedTriggersTimer.Interval = 45000;
            this.ReceivedTriggersTimer.Tick += new System.EventHandler(this.ReceivedTriggersTimer_Tick);
            // 
            // CheckSNAP
            // 
            this.CheckSNAP.Interval = 60000;
            this.CheckSNAP.Tick += new System.EventHandler(this.CheckSNAP_Tick);
            // 
            // DataLoggingTimer
            // 
            this.DataLoggingTimer.Interval = 3600000;
            this.DataLoggingTimer.Tick += new System.EventHandler(this.DataLoggingTimer_Tick);
            // 
            // LowTimer
            // 
            this.LowTimer.Interval = 60000;
            this.LowTimer.Tick += new System.EventHandler(this.LowTimer_Tick);
            // 
            // PerfCheckGroupBox
            // 
            this.PerfCheckGroupBox.Controls.Add(this.NEXOpt);
            this.PerfCheckGroupBox.Controls.Add(this.NEXLabel);
            this.PerfCheckGroupBox.Controls.Add(this.SCROpt);
            this.PerfCheckGroupBox.Controls.Add(this.SCRLabel);
            this.PerfCheckGroupBox.Controls.Add(this.WSKOpt);
            this.PerfCheckGroupBox.Controls.Add(this.WSKLabel);
            this.PerfCheckGroupBox.Controls.Add(this.AVAOpt);
            this.PerfCheckGroupBox.Controls.Add(this.AVALabel);
            this.PerfCheckGroupBox.Controls.Add(this.SENOpt);
            this.PerfCheckGroupBox.Controls.Add(this.SENLabel);
            this.PerfCheckGroupBox.Location = new System.Drawing.Point(313, 235);
            this.PerfCheckGroupBox.Name = "PerfCheckGroupBox";
            this.PerfCheckGroupBox.Size = new System.Drawing.Size(221, 125);
            this.PerfCheckGroupBox.TabIndex = 20;
            this.PerfCheckGroupBox.TabStop = false;
            this.PerfCheckGroupBox.Text = "Module Check";
            // 
            // NEXOpt
            // 
            this.NEXOpt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NEXOpt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.NEXOpt.Location = new System.Drawing.Point(122, 99);
            this.NEXOpt.Name = "NEXOpt";
            this.NEXOpt.Size = new System.Drawing.Size(89, 13);
            this.NEXOpt.TabIndex = 26;
            this.NEXOpt.Text = " Disconnected";
            this.NEXOpt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NEXLabel
            // 
            this.NEXLabel.AutoSize = true;
            this.NEXLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NEXLabel.Location = new System.Drawing.Point(13, 99);
            this.NEXLabel.Name = "NEXLabel";
            this.NEXLabel.Size = new System.Drawing.Size(32, 13);
            this.NEXLabel.TabIndex = 25;
            this.NEXLabel.Text = "NEX";
            this.NEXLabel.Click += new System.EventHandler(this.NEXLabel_Click);
            // 
            // SCROpt
            // 
            this.SCROpt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SCROpt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.SCROpt.Location = new System.Drawing.Point(122, 20);
            this.SCROpt.Name = "SCROpt";
            this.SCROpt.Size = new System.Drawing.Size(89, 13);
            this.SCROpt.TabIndex = 24;
            this.SCROpt.Text = " Disconnected";
            this.SCROpt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SCRLabel
            // 
            this.SCRLabel.AutoSize = true;
            this.SCRLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SCRLabel.Location = new System.Drawing.Point(13, 20);
            this.SCRLabel.Name = "SCRLabel";
            this.SCRLabel.Size = new System.Drawing.Size(32, 13);
            this.SCRLabel.TabIndex = 9;
            this.SCRLabel.Text = "SCR";
            // 
            // WSKOpt
            // 
            this.WSKOpt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WSKOpt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.WSKOpt.Location = new System.Drawing.Point(122, 79);
            this.WSKOpt.Name = "WSKOpt";
            this.WSKOpt.Size = new System.Drawing.Size(89, 13);
            this.WSKOpt.TabIndex = 8;
            this.WSKOpt.Text = " Disconnected";
            this.WSKOpt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // WSKLabel
            // 
            this.WSKLabel.AutoSize = true;
            this.WSKLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WSKLabel.Location = new System.Drawing.Point(12, 79);
            this.WSKLabel.Name = "WSKLabel";
            this.WSKLabel.Size = new System.Drawing.Size(35, 13);
            this.WSKLabel.TabIndex = 7;
            this.WSKLabel.Text = "WSK";
            // 
            // AVAOpt
            // 
            this.AVAOpt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AVAOpt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.AVAOpt.Location = new System.Drawing.Point(122, 59);
            this.AVAOpt.Name = "AVAOpt";
            this.AVAOpt.Size = new System.Drawing.Size(89, 13);
            this.AVAOpt.TabIndex = 6;
            this.AVAOpt.Text = " Disconnected";
            this.AVAOpt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // AVALabel
            // 
            this.AVALabel.AutoSize = true;
            this.AVALabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AVALabel.Location = new System.Drawing.Point(13, 59);
            this.AVALabel.Name = "AVALabel";
            this.AVALabel.Size = new System.Drawing.Size(31, 13);
            this.AVALabel.TabIndex = 5;
            this.AVALabel.Text = "AVA";
            // 
            // SENOpt
            // 
            this.SENOpt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SENOpt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.SENOpt.Location = new System.Drawing.Point(122, 39);
            this.SENOpt.Name = "SENOpt";
            this.SENOpt.Size = new System.Drawing.Size(89, 13);
            this.SENOpt.TabIndex = 4;
            this.SENOpt.Text = " Disconnected";
            this.SENOpt.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SENLabel
            // 
            this.SENLabel.AutoSize = true;
            this.SENLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SENLabel.Location = new System.Drawing.Point(13, 39);
            this.SENLabel.Name = "SENLabel";
            this.SENLabel.Size = new System.Drawing.Size(32, 13);
            this.SENLabel.TabIndex = 1;
            this.SENLabel.Text = "SEN";
            // 
            // LastLogMsgLabel
            // 
            this.LastLogMsgLabel.AutoSize = true;
            this.LastLogMsgLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Bold);
            this.LastLogMsgLabel.Location = new System.Drawing.Point(14, 20);
            this.LastLogMsgLabel.Name = "LastLogMsgLabel";
            this.LastLogMsgLabel.Size = new System.Drawing.Size(69, 13);
            this.LastLogMsgLabel.TabIndex = 21;
            this.LastLogMsgLabel.Text = "Application";
            // 
            // LastLogMsgOpt
            // 
            this.LastLogMsgOpt.AutoSize = true;
            this.LastLogMsgOpt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LastLogMsgOpt.Location = new System.Drawing.Point(16, 36);
            this.LastLogMsgOpt.Name = "LastLogMsgOpt";
            this.LastLogMsgOpt.Size = new System.Drawing.Size(25, 13);
            this.LastLogMsgOpt.TabIndex = 22;
            this.LastLogMsgOpt.Text = "      ";
            // 
            // USBDevicesGroupBox
            // 
            this.USBDevicesGroupBox.Controls.Add(this.SensorOpt);
            this.USBDevicesGroupBox.Controls.Add(this.SensorLabel);
            this.USBDevicesGroupBox.Controls.Add(this.USBSensorDebug);
            this.USBDevicesGroupBox.Controls.Add(this.RS232Opt);
            this.USBDevicesGroupBox.Controls.Add(this.RS232Label);
            this.USBDevicesGroupBox.Controls.Add(this.RS232Picture);
            this.USBDevicesGroupBox.Controls.Add(this.WiFiCardOpt);
            this.USBDevicesGroupBox.Controls.Add(this.WiFiLabel);
            this.USBDevicesGroupBox.Controls.Add(this.USBDriveOpt);
            this.USBDevicesGroupBox.Controls.Add(this.USBDriveLabel);
            this.USBDevicesGroupBox.Controls.Add(this.InternetOpt);
            this.USBDevicesGroupBox.Controls.Add(this.InternetLabel);
            this.USBDevicesGroupBox.Controls.Add(this.TouchscreenOpt);
            this.USBDevicesGroupBox.Controls.Add(this.TouchscreenLabel);
            this.USBDevicesGroupBox.Controls.Add(this.pictureBox4);
            this.USBDevicesGroupBox.Controls.Add(this.pictureBox3);
            this.USBDevicesGroupBox.Controls.Add(this.pictureBox2);
            this.USBDevicesGroupBox.Controls.Add(this.pictureBox1);
            this.USBDevicesGroupBox.Location = new System.Drawing.Point(540, 9);
            this.USBDevicesGroupBox.Name = "USBDevicesGroupBox";
            this.USBDevicesGroupBox.Size = new System.Drawing.Size(145, 351);
            this.USBDevicesGroupBox.TabIndex = 23;
            this.USBDevicesGroupBox.TabStop = false;
            this.USBDevicesGroupBox.Text = "Devices";
            // 
            // SensorOpt
            // 
            this.SensorOpt.AutoSize = true;
            this.SensorOpt.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold);
            this.SensorOpt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.SensorOpt.Location = new System.Drawing.Point(65, 321);
            this.SensorOpt.Name = "SensorOpt";
            this.SensorOpt.Size = new System.Drawing.Size(74, 12);
            this.SensorOpt.TabIndex = 37;
            this.SensorOpt.Text = "Disconnected";
            // 
            // SensorLabel
            // 
            this.SensorLabel.AutoSize = true;
            this.SensorLabel.Location = new System.Drawing.Point(64, 305);
            this.SensorLabel.Name = "SensorLabel";
            this.SensorLabel.Size = new System.Drawing.Size(62, 13);
            this.SensorLabel.TabIndex = 36;
            this.SensorLabel.Text = "Env Sensor";
            // 
            // USBSensorDebug
            // 
            this.USBSensorDebug.Image = global::GlobalCMS.Properties.Resources.ICO_TEMP64;
            this.USBSensorDebug.Location = new System.Drawing.Point(15, 295);
            this.USBSensorDebug.Name = "USBSensorDebug";
            this.USBSensorDebug.Size = new System.Drawing.Size(45, 45);
            this.USBSensorDebug.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.USBSensorDebug.TabIndex = 35;
            this.USBSensorDebug.TabStop = false;
            this.USBSensorDebug.Click += new System.EventHandler(this.USBSensorDebug_Click);
            // 
            // RS232Opt
            // 
            this.RS232Opt.AutoSize = true;
            this.RS232Opt.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold);
            this.RS232Opt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.RS232Opt.Location = new System.Drawing.Point(65, 267);
            this.RS232Opt.Name = "RS232Opt";
            this.RS232Opt.Size = new System.Drawing.Size(74, 12);
            this.RS232Opt.TabIndex = 34;
            this.RS232Opt.Text = "Disconnected";
            // 
            // RS232Label
            // 
            this.RS232Label.AutoSize = true;
            this.RS232Label.Location = new System.Drawing.Point(64, 251);
            this.RS232Label.Name = "RS232Label";
            this.RS232Label.Size = new System.Drawing.Size(40, 13);
            this.RS232Label.TabIndex = 33;
            this.RS232Label.Text = "RS232";
            // 
            // RS232Picture
            // 
            this.RS232Picture.Image = global::GlobalCMS.Properties.Resources.ICO_SERIAL64;
            this.RS232Picture.Location = new System.Drawing.Point(15, 242);
            this.RS232Picture.Name = "RS232Picture";
            this.RS232Picture.Size = new System.Drawing.Size(45, 45);
            this.RS232Picture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.RS232Picture.TabIndex = 32;
            this.RS232Picture.TabStop = false;
            this.RS232Picture.Click += new System.EventHandler(this.RS232Picture_Click);
            // 
            // WiFiCardOpt
            // 
            this.WiFiCardOpt.AutoSize = true;
            this.WiFiCardOpt.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold);
            this.WiFiCardOpt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.WiFiCardOpt.Location = new System.Drawing.Point(65, 215);
            this.WiFiCardOpt.Name = "WiFiCardOpt";
            this.WiFiCardOpt.Size = new System.Drawing.Size(74, 12);
            this.WiFiCardOpt.TabIndex = 31;
            this.WiFiCardOpt.Text = "Disconnected";
            // 
            // WiFiLabel
            // 
            this.WiFiLabel.AutoSize = true;
            this.WiFiLabel.Location = new System.Drawing.Point(64, 199);
            this.WiFiLabel.Name = "WiFiLabel";
            this.WiFiLabel.Size = new System.Drawing.Size(68, 13);
            this.WiFiLabel.TabIndex = 30;
            this.WiFiLabel.Text = "WiFi Adapter";
            // 
            // USBDriveOpt
            // 
            this.USBDriveOpt.AutoSize = true;
            this.USBDriveOpt.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold);
            this.USBDriveOpt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.USBDriveOpt.Location = new System.Drawing.Point(65, 160);
            this.USBDriveOpt.Name = "USBDriveOpt";
            this.USBDriveOpt.Size = new System.Drawing.Size(74, 12);
            this.USBDriveOpt.TabIndex = 29;
            this.USBDriveOpt.Text = "Disconnected";
            // 
            // USBDriveLabel
            // 
            this.USBDriveLabel.AutoSize = true;
            this.USBDriveLabel.Location = new System.Drawing.Point(64, 144);
            this.USBDriveLabel.Name = "USBDriveLabel";
            this.USBDriveLabel.Size = new System.Drawing.Size(57, 13);
            this.USBDriveLabel.TabIndex = 28;
            this.USBDriveLabel.Text = "USB Drive";
            // 
            // InternetOpt
            // 
            this.InternetOpt.AutoSize = true;
            this.InternetOpt.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold);
            this.InternetOpt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.InternetOpt.Location = new System.Drawing.Point(65, 106);
            this.InternetOpt.Name = "InternetOpt";
            this.InternetOpt.Size = new System.Drawing.Size(74, 12);
            this.InternetOpt.TabIndex = 27;
            this.InternetOpt.Text = "Disconnected";
            // 
            // InternetLabel
            // 
            this.InternetLabel.AutoSize = true;
            this.InternetLabel.Location = new System.Drawing.Point(64, 90);
            this.InternetLabel.Name = "InternetLabel";
            this.InternetLabel.Size = new System.Drawing.Size(68, 13);
            this.InternetLabel.TabIndex = 26;
            this.InternetLabel.Text = "USB Internet";
            // 
            // TouchscreenOpt
            // 
            this.TouchscreenOpt.AutoSize = true;
            this.TouchscreenOpt.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.5F, System.Drawing.FontStyle.Bold);
            this.TouchscreenOpt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.TouchscreenOpt.Location = new System.Drawing.Point(65, 49);
            this.TouchscreenOpt.Name = "TouchscreenOpt";
            this.TouchscreenOpt.Size = new System.Drawing.Size(74, 12);
            this.TouchscreenOpt.TabIndex = 25;
            this.TouchscreenOpt.Text = "Disconnected";
            // 
            // TouchscreenLabel
            // 
            this.TouchscreenLabel.AutoSize = true;
            this.TouchscreenLabel.Location = new System.Drawing.Point(64, 33);
            this.TouchscreenLabel.Name = "TouchscreenLabel";
            this.TouchscreenLabel.Size = new System.Drawing.Size(70, 13);
            this.TouchscreenLabel.TabIndex = 4;
            this.TouchscreenLabel.Text = "Touchscreen";
            // 
            // pictureBox4
            // 
            this.pictureBox4.Image = global::GlobalCMS.Properties.Resources.ICO_WIFI64;
            this.pictureBox4.Location = new System.Drawing.Point(15, 189);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(45, 45);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox4.TabIndex = 3;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::GlobalCMS.Properties.Resources.ICO_USB64;
            this.pictureBox3.Location = new System.Drawing.Point(15, 135);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(45, 45);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 2;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::GlobalCMS.Properties.Resources.ICO_USB_MODEM64;
            this.pictureBox2.Location = new System.Drawing.Point(15, 81);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(45, 45);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::GlobalCMS.Properties.Resources.ICO_TOUCHSCREEN64;
            this.pictureBox1.Location = new System.Drawing.Point(15, 26);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(45, 45);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // CheckForInteractive
            // 
            this.CheckForInteractive.Enabled = true;
            this.CheckForInteractive.Interval = 30000;
            this.CheckForInteractive.Tick += new System.EventHandler(this.CheckForInteractive_Tick);
            // 
            // CheckForNewSignage
            // 
            this.CheckForNewSignage.Enabled = true;
            this.CheckForNewSignage.Interval = 3600000;
            this.CheckForNewSignage.Tick += new System.EventHandler(this.CheckForNewSignage_Tick);
            // 
            // LogoBottomCorner
            // 
            LogoBottomCorner.BackColor = System.Drawing.Color.Transparent;
            LogoBottomCorner.Image = global::GlobalCMS.Properties.Resources.SKIN_DEFAULT_LOGO;
            LogoBottomCorner.Location = new System.Drawing.Point(533, 360);
            LogoBottomCorner.Name = "LogoBottomCorner";
            LogoBottomCorner.Size = new System.Drawing.Size(155, 87);
            LogoBottomCorner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            LogoBottomCorner.TabIndex = 3;
            LogoBottomCorner.TabStop = false;
            LogoBottomCorner.Click += new System.EventHandler(this.LogoBottomCorner_Click);
            // 
            // LauncherTimer
            // 
            this.LauncherTimer.Interval = 60000;
            this.LauncherTimer.Tick += new System.EventHandler(this.LauncherTimer_Tick);
            // 
            // CheckTrial
            // 
            this.CheckTrial.Interval = 1800000;
            this.CheckTrial.Tick += new System.EventHandler(this.CheckTrial_Tick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.LastWebsocketLabel);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.LastHeartbeatLabel);
            this.groupBox1.Controls.Add(this.LastLogMsgOpt);
            this.groupBox1.Controls.Add(this.LastLogMsgLabel);
            this.groupBox1.Location = new System.Drawing.Point(12, 364);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(522, 90);
            this.groupBox1.TabIndex = 24;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "System";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(329, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 25;
            this.label2.Text = "Via Websocket";
            // 
            // LastWebsocketLabel
            // 
            this.LastWebsocketLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LastWebsocketLabel.Location = new System.Drawing.Point(331, 70);
            this.LastWebsocketLabel.Name = "LastWebsocketLabel";
            this.LastWebsocketLabel.Size = new System.Drawing.Size(181, 17);
            this.LastWebsocketLabel.TabIndex = 24;
            this.LastWebsocketLabel.Text = "      ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(14, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "From Server";
            // 
            // CurRunningVerTxt
            // 
            this.CurRunningVerTxt.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.CurRunningVerTxt.BackColor = System.Drawing.Color.Transparent;
            this.CurRunningVerTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CurRunningVerTxt.ForeColor = System.Drawing.SystemColors.ControlText;
            this.CurRunningVerTxt.Location = new System.Drawing.Point(540, 440);
            this.CurRunningVerTxt.Name = "CurRunningVerTxt";
            this.CurRunningVerTxt.Size = new System.Drawing.Size(145, 18);
            this.CurRunningVerTxt.TabIndex = 25;
            this.CurRunningVerTxt.Text = "0.0.0.0";
            this.CurRunningVerTxt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(694, 459);
            this.Controls.Add(this.CurRunningVerTxt);
            this.Controls.Add(this.USBDevicesGroupBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.SystemCheckGroupBox);
            this.Controls.Add(this.hardwareDetailsGroupBox);
            this.Controls.Add(this.systemDetailsGroupBox);
            this.Controls.Add(this.PerfCheckGroupBox);
            this.Controls.Add(this.IPDetailsGroupBox);
            this.Controls.Add(LogoBottomCorner);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(710, 498);
            this.MinimumSize = new System.Drawing.Size(710, 498);
            this.Name = "MainForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Monitoring Solution";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.systemDetailsGroupBox.ResumeLayout(false);
            this.systemDetailsGroupBox.PerformLayout();
            this.TaskbarContextMenu.ResumeLayout(false);
            this.TaskbarContextMenu.PerformLayout();
            this.IPDetailsGroupBox.ResumeLayout(false);
            this.IPDetailsGroupBox.PerformLayout();
            this.hardwareDetailsGroupBox.ResumeLayout(false);
            this.hardwareDetailsGroupBox.PerformLayout();
            this.SystemCheckGroupBox.ResumeLayout(false);
            this.PerfCheckGroupBox.ResumeLayout(false);
            this.PerfCheckGroupBox.PerformLayout();
            this.USBDevicesGroupBox.ResumeLayout(false);
            this.USBDevicesGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.USBSensorDebug)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RS232Picture)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(LogoBottomCorner)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox systemDetailsGroupBox;
        private System.Windows.Forms.Label devUUIDLabel;
        private System.Windows.Forms.Label devNameLabel;
        private System.Windows.Forms.Label devMAC;
        private System.Windows.Forms.Label devUUID;
        private System.Windows.Forms.Label devName;
        private System.Windows.Forms.Label devMACLabel;
        private System.Windows.Forms.GroupBox IPDetailsGroupBox;
        private System.Windows.Forms.Label devWAN;
        private System.Windows.Forms.Label devVPN;
        private System.Windows.Forms.Label devLAN;
        private System.Windows.Forms.Label devWANLabel;
        private System.Windows.Forms.Label devVPNLabel;
        private System.Windows.Forms.Label devLANLabel;
        private System.Windows.Forms.GroupBox hardwareDetailsGroupBox;
        private System.Windows.Forms.Label CPUArch;
        private System.Windows.Forms.Label ramAmount;
        private System.Windows.Forms.Label ramLabel;
        private System.Windows.Forms.Label cpuLabel;
        private System.Windows.Forms.Label gfxCard;
        private System.Windows.Forms.Label gfxCardLabel;
        private System.Windows.Forms.Label hddAmounts;
        private System.Windows.Forms.Label hddLabel;
        private System.Windows.Forms.Label ramLoadLabel;
        private System.Windows.Forms.Label cpuLoadLabel;
        private System.Windows.Forms.GroupBox SystemCheckGroupBox;
        private System.Windows.Forms.Label SecureConnectionOpt;
        private System.Windows.Forms.Label InternetConnectionOpt;
        private System.Windows.Forms.Label SignageSystemLabel;
        private System.Windows.Forms.Label SecureConnectionLabel;
        private System.Windows.Forms.Label InternetConnectionLabel;
        private System.Windows.Forms.Label SignageSystemOpt;
        private System.Windows.Forms.Label LastHeartbeatLabel;
        private System.Windows.Forms.Label devOS;
        private System.Windows.Forms.Label systemNetworkOpt;
        private System.Windows.Forms.Label systemNetworkLabel;
        public System.Windows.Forms.Timer CallHomeTimer;
        public System.Windows.Forms.Timer CheckStatsTimer;
        public System.Windows.Forms.Timer CheckServicesTimer;
        public System.Windows.Forms.Timer ReceivedTriggersTimer;
        public System.Windows.Forms.Label powerModeLabel;
        public System.Windows.Forms.Timer CheckSNAP;
        public System.Windows.Forms.Timer DataLoggingTimer;
        public System.Windows.Forms.Timer LowTimer;
        private System.Windows.Forms.ToolStripMenuItem taskbarShutdownService;
        private System.Windows.Forms.ToolStripMenuItem taskbarAbout;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripTextBox taskbarLANLabel;
        private System.Windows.Forms.ToolStripTextBox taskbarVPNLabel;
        private System.Windows.Forms.ToolStripTextBox taskbarWANLabel;
        public System.Windows.Forms.ContextMenuStrip TaskbarContextMenu;
        private System.Windows.Forms.GroupBox PerfCheckGroupBox;
        private System.Windows.Forms.Label SENOpt;
        private System.Windows.Forms.Label SENLabel;
        private System.Windows.Forms.Label AVAOpt;
        private System.Windows.Forms.Label AVALabel;
        private System.Windows.Forms.Label WSKOpt;
        private System.Windows.Forms.Label WSKLabel;
        private System.Windows.Forms.Label RunningModeLabel;
        private System.Windows.Forms.Label LastLogMsgLabel;
        public System.Windows.Forms.Label LastLogMsgOpt;
        private System.Windows.Forms.GroupBox USBDevicesGroupBox;
        private System.Windows.Forms.Label NEXLabel;
        private System.Windows.Forms.Label SCROpt;
        private System.Windows.Forms.Label SCRLabel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label TouchscreenLabel;
        private System.Windows.Forms.Label USBDriveOpt;
        private System.Windows.Forms.Label USBDriveLabel;
        private System.Windows.Forms.Label InternetOpt;
        private System.Windows.Forms.Label InternetLabel;
        private System.Windows.Forms.Label TouchscreenOpt;
        private System.Windows.Forms.Label RS232Opt;
        private System.Windows.Forms.Label RS232Label;
        private System.Windows.Forms.PictureBox RS232Picture;
        private System.Windows.Forms.Button MaintModeBTN;
        private System.Windows.Forms.Button SignageDebugBTN;
        public System.Windows.Forms.Timer CheckForInteractive;
        public System.Windows.Forms.Timer CheckForNewSignage;
        private System.Windows.Forms.Button ComPortsBTN;
        public System.Windows.Forms.Timer LauncherTimer;
        public System.Windows.Forms.Label TrialLicTxt;
        public System.Windows.Forms.Timer CheckTrial;
        private System.Windows.Forms.Label SensorLabel;
        private System.Windows.Forms.PictureBox USBSensorDebug;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label SensorOpt;
        private System.Windows.Forms.Button TestBrowserBTN;
        public System.Windows.Forms.Label NEXOpt;
        public System.Windows.Forms.Label CurRunningVerTxt;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.Label LastWebsocketLabel;
        public System.Windows.Forms.Label WiFiCardOpt;
        private System.Windows.Forms.Label WiFiLabel;
        private System.Windows.Forms.PictureBox pictureBox4;
        public static System.Windows.Forms.PictureBox LogoBottomCorner;
        public static System.Windows.Forms.NotifyIcon TaskbarIcon;
    }
}

