namespace GlobalCMS
{
    partial class Nexmosphere
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
            this.serialPort = new System.IO.Ports.SerialPort(this.components);
            this.txtData = new System.Windows.Forms.TextBox();
            this.CurrentCodeLabel = new System.Windows.Forms.Label();
            this.CheckSensorsTimer = new System.Windows.Forms.Timer(this.components);
            this.CurrentCodeIn = new System.Windows.Forms.Label();
            this.txtData2 = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.SendClearCMD = new System.Windows.Forms.Button();
            this.CurrentCodeInPresence = new System.Windows.Forms.Label();
            this.Presence = new System.Windows.Forms.Label();
            this.NumberOfConnectedDevices = new System.Windows.Forms.Label();
            this.SendNexCMD = new System.Windows.Forms.Button();
            this.listdevices = new System.Windows.Forms.ListView();
            this.CommonName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FriendlyName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.HardwareId = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ComPort = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Status = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.txtToSend = new System.Windows.Forms.TextBox();
            this.CurrentCodeInTxt = new System.Windows.Forms.Label();
            this.CurrentCodeOut = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListView();
            this.Socket = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Sensor = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Init = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PwrDwn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Actions = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Triggers = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Data = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.CurrentStatus = new System.Windows.Forms.Label();
            this.COMLabel = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.CheckNexmoConnected = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // serialPort
            // 
            this.serialPort.BaudRate = 115200;
            this.serialPort.PortName = "COM12";
            // 
            // txtData
            // 
            this.txtData.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtData.Location = new System.Drawing.Point(6, 19);
            this.txtData.Multiline = true;
            this.txtData.Name = "txtData";
            this.txtData.ReadOnly = true;
            this.txtData.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtData.Size = new System.Drawing.Size(249, 226);
            this.txtData.TabIndex = 1;
            this.txtData.Text = "X001A[3]\r\nX001A[0]\r\n";
            // 
            // CurrentCodeLabel
            // 
            this.CurrentCodeLabel.AutoSize = true;
            this.CurrentCodeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CurrentCodeLabel.Location = new System.Drawing.Point(6, 16);
            this.CurrentCodeLabel.Name = "CurrentCodeLabel";
            this.CurrentCodeLabel.Size = new System.Drawing.Size(82, 20);
            this.CurrentCodeLabel.TabIndex = 3;
            this.CurrentCodeLabel.Text = "Incoming";
            // 
            // CheckSensorsTimer
            // 
            this.CheckSensorsTimer.Enabled = true;
            this.CheckSensorsTimer.Tick += new System.EventHandler(this.CheckSensorsTimer_Tick);
            // 
            // CurrentCodeIn
            // 
            this.CurrentCodeIn.AutoSize = true;
            this.CurrentCodeIn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CurrentCodeIn.Location = new System.Drawing.Point(7, 40);
            this.CurrentCodeIn.Name = "CurrentCodeIn";
            this.CurrentCodeIn.Size = new System.Drawing.Size(37, 15);
            this.CurrentCodeIn.TabIndex = 4;
            this.CurrentCodeIn.Text = "None";
            // 
            // txtData2
            // 
            this.txtData2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtData2.Location = new System.Drawing.Point(6, 19);
            this.txtData2.Multiline = true;
            this.txtData2.Name = "txtData2";
            this.txtData2.ReadOnly = true;
            this.txtData2.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtData2.Size = new System.Drawing.Size(246, 226);
            this.txtData2.TabIndex = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.SendClearCMD);
            this.groupBox1.Controls.Add(this.CurrentCodeInPresence);
            this.groupBox1.Controls.Add(this.Presence);
            this.groupBox1.Controls.Add(this.NumberOfConnectedDevices);
            this.groupBox1.Controls.Add(this.SendNexCMD);
            this.groupBox1.Controls.Add(this.listdevices);
            this.groupBox1.Controls.Add(this.txtToSend);
            this.groupBox1.Controls.Add(this.CurrentCodeInTxt);
            this.groupBox1.Controls.Add(this.CurrentCodeOut);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.CurrentCodeIn);
            this.groupBox1.Controls.Add(this.CurrentCodeLabel);
            this.groupBox1.Location = new System.Drawing.Point(541, 9);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(290, 252);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            // 
            // SendClearCMD
            // 
            this.SendClearCMD.Enabled = false;
            this.SendClearCMD.Location = new System.Drawing.Point(230, 223);
            this.SendClearCMD.Name = "SendClearCMD";
            this.SendClearCMD.Size = new System.Drawing.Size(54, 23);
            this.SendClearCMD.TabIndex = 14;
            this.SendClearCMD.Text = "Clear";
            this.SendClearCMD.UseVisualStyleBackColor = true;
            this.SendClearCMD.Click += new System.EventHandler(this.SendClearCMD_Click);
            // 
            // CurrentCodeInPresence
            // 
            this.CurrentCodeInPresence.AutoSize = true;
            this.CurrentCodeInPresence.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CurrentCodeInPresence.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.CurrentCodeInPresence.Location = new System.Drawing.Point(169, 41);
            this.CurrentCodeInPresence.Name = "CurrentCodeInPresence";
            this.CurrentCodeInPresence.Size = new System.Drawing.Size(67, 15);
            this.CurrentCodeInPresence.TabIndex = 11;
            this.CurrentCodeInPresence.Text = "INACTIVE";
            // 
            // Presence
            // 
            this.Presence.AutoSize = true;
            this.Presence.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.Presence.Location = new System.Drawing.Point(169, 17);
            this.Presence.Name = "Presence";
            this.Presence.Size = new System.Drawing.Size(84, 20);
            this.Presence.TabIndex = 10;
            this.Presence.Text = "Presence";
            // 
            // NumberOfConnectedDevices
            // 
            this.NumberOfConnectedDevices.AutoSize = true;
            this.NumberOfConnectedDevices.Location = new System.Drawing.Point(169, 111);
            this.NumberOfConnectedDevices.Name = "NumberOfConnectedDevices";
            this.NumberOfConnectedDevices.Size = new System.Drawing.Size(101, 13);
            this.NumberOfConnectedDevices.TabIndex = 13;
            this.NumberOfConnectedDevices.Text = "0 Devices Attached";
            this.NumberOfConnectedDevices.Visible = false;
            // 
            // SendNexCMD
            // 
            this.SendNexCMD.Enabled = false;
            this.SendNexCMD.Location = new System.Drawing.Point(149, 223);
            this.SendNexCMD.Name = "SendNexCMD";
            this.SendNexCMD.Size = new System.Drawing.Size(75, 23);
            this.SendNexCMD.TabIndex = 8;
            this.SendNexCMD.Text = "Send CMD";
            this.SendNexCMD.UseVisualStyleBackColor = true;
            this.SendNexCMD.Click += new System.EventHandler(this.SendNexCMD_Click);
            // 
            // listdevices
            // 
            this.listdevices.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.CommonName,
            this.FriendlyName,
            this.HardwareId,
            this.ComPort,
            this.Status});
            this.listdevices.FullRowSelect = true;
            this.listdevices.HideSelection = false;
            this.listdevices.Location = new System.Drawing.Point(6, 137);
            this.listdevices.MultiSelect = false;
            this.listdevices.Name = "listdevices";
            this.listdevices.Size = new System.Drawing.Size(278, 77);
            this.listdevices.TabIndex = 12;
            this.listdevices.UseCompatibleStateImageBehavior = false;
            this.listdevices.View = System.Windows.Forms.View.Details;
            // 
            // CommonName
            // 
            this.CommonName.Text = "CommonName";
            this.CommonName.Width = 79;
            // 
            // FriendlyName
            // 
            this.FriendlyName.Text = "FriendlyName";
            this.FriendlyName.Width = 74;
            // 
            // HardwareId
            // 
            this.HardwareId.Text = "HardwareId";
            this.HardwareId.Width = 93;
            // 
            // ComPort
            // 
            this.ComPort.Text = "ComPort";
            // 
            // Status
            // 
            this.Status.Text = "Status";
            this.Status.Width = 101;
            // 
            // txtToSend
            // 
            this.txtToSend.Location = new System.Drawing.Point(6, 223);
            this.txtToSend.Multiline = true;
            this.txtToSend.Name = "txtToSend";
            this.txtToSend.Size = new System.Drawing.Size(137, 23);
            this.txtToSend.TabIndex = 9;
            // 
            // CurrentCodeInTxt
            // 
            this.CurrentCodeInTxt.AutoSize = true;
            this.CurrentCodeInTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CurrentCodeInTxt.Location = new System.Drawing.Point(7, 57);
            this.CurrentCodeInTxt.Name = "CurrentCodeInTxt";
            this.CurrentCodeInTxt.Size = new System.Drawing.Size(10, 15);
            this.CurrentCodeInTxt.TabIndex = 7;
            this.CurrentCodeInTxt.Text = " ";
            // 
            // CurrentCodeOut
            // 
            this.CurrentCodeOut.AutoSize = true;
            this.CurrentCodeOut.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.CurrentCodeOut.Location = new System.Drawing.Point(6, 109);
            this.CurrentCodeOut.Name = "CurrentCodeOut";
            this.CurrentCodeOut.Size = new System.Drawing.Size(40, 15);
            this.CurrentCodeOut.TabIndex = 6;
            this.CurrentCodeOut.Text = " None";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(6, 85);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 20);
            this.label4.TabIndex = 5;
            this.label4.Text = "Outgoing";
            // 
            // listBox1
            // 
            this.listBox1.AutoArrange = false;
            this.listBox1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Socket,
            this.Sensor,
            this.Init,
            this.PwrDwn,
            this.Actions,
            this.Triggers,
            this.Data});
            this.listBox1.FullRowSelect = true;
            this.listBox1.GridLines = true;
            this.listBox1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listBox1.HideSelection = false;
            this.listBox1.Location = new System.Drawing.Point(12, 267);
            this.listBox1.MultiSelect = false;
            this.listBox1.Name = "listBox1";
            this.listBox1.ShowGroups = false;
            this.listBox1.Size = new System.Drawing.Size(819, 244);
            this.listBox1.TabIndex = 8;
            this.listBox1.UseCompatibleStateImageBehavior = false;
            this.listBox1.View = System.Windows.Forms.View.Details;
            // 
            // Socket
            // 
            this.Socket.Text = "Socket";
            // 
            // Sensor
            // 
            this.Sensor.Text = "Sensor";
            this.Sensor.Width = 77;
            // 
            // Init
            // 
            this.Init.Text = "Init";
            // 
            // PwrDwn
            // 
            this.PwrDwn.Text = "PwrDwn";
            // 
            // Actions
            // 
            this.Actions.Text = "Action";
            this.Actions.Width = 114;
            // 
            // Triggers
            // 
            this.Triggers.Text = "Triggers";
            this.Triggers.Width = 117;
            // 
            // Data
            // 
            this.Data.Text = "XtraData";
            this.Data.Width = 148;
            // 
            // CurrentStatus
            // 
            this.CurrentStatus.AutoSize = true;
            this.CurrentStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CurrentStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.CurrentStatus.Location = new System.Drawing.Point(12, 514);
            this.CurrentStatus.Name = "CurrentStatus";
            this.CurrentStatus.Size = new System.Drawing.Size(98, 15);
            this.CurrentStatus.TabIndex = 9;
            this.CurrentStatus.Text = "Current Status";
            // 
            // COMLabel
            // 
            this.COMLabel.AutoSize = true;
            this.COMLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.COMLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.COMLabel.Location = new System.Drawing.Point(777, 514);
            this.COMLabel.Name = "COMLabel";
            this.COMLabel.Size = new System.Drawing.Size(54, 15);
            this.COMLabel.TabIndex = 10;
            this.COMLabel.Text = "COM12";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtData2);
            this.groupBox2.Location = new System.Drawing.Point(279, 9);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(258, 252);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Outgoing Data (RAW)";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtData);
            this.groupBox3.Location = new System.Drawing.Point(12, 9);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(261, 252);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Incoming Data (RAW)";
            // 
            // CheckNexmoConnected
            // 
            this.CheckNexmoConnected.Interval = 5000;
            this.CheckNexmoConnected.Tick += new System.EventHandler(this.CheckNexmoConnected_Tick);
            // 
            // Nexmosphere
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(842, 536);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.COMLabel);
            this.Controls.Add(this.CurrentStatus);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.listBox1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(858, 575);
            this.MinimumSize = new System.Drawing.Size(858, 575);
            this.Name = "Nexmosphere";
            this.ShowIcon = false;
            this.Text = "Nexmosphere";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NexmosphereClosed);
            this.Load += new System.EventHandler(this.NexmosphereLoad);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("");
        System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("");
        private System.IO.Ports.SerialPort serialPort;
        private System.Windows.Forms.TextBox txtData;
        private System.Windows.Forms.Label CurrentCodeLabel;
        private System.Windows.Forms.Timer CheckSensorsTimer;
        private System.Windows.Forms.TextBox txtData2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label CurrentCodeInTxt;
        private System.Windows.Forms.Button SendNexCMD;
        private System.Windows.Forms.TextBox txtToSend;
        private System.Windows.Forms.ListView listBox1;
        private System.Windows.Forms.ColumnHeader Socket;
        private System.Windows.Forms.ColumnHeader Sensor;
        private System.Windows.Forms.ColumnHeader Actions;
        private System.Windows.Forms.ColumnHeader Triggers;
        private System.Windows.Forms.ColumnHeader Data;
        private System.Windows.Forms.Label CurrentStatus;
        private System.Windows.Forms.ColumnHeader Init;
        private System.Windows.Forms.ColumnHeader PwrDwn;
        private System.Windows.Forms.Label COMLabel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        public System.Windows.Forms.Label CurrentCodeIn;
        private System.Windows.Forms.Label Presence;
        public System.Windows.Forms.Label CurrentCodeInPresence;
        public System.Windows.Forms.Label CurrentCodeOut;
        private System.Windows.Forms.Timer CheckNexmoConnected;
        private System.Windows.Forms.ListView listdevices;
        private System.Windows.Forms.ColumnHeader CommonName;
        private System.Windows.Forms.ColumnHeader FriendlyName;
        private System.Windows.Forms.ColumnHeader HardwareId;
        private System.Windows.Forms.ColumnHeader Status;
        private System.Windows.Forms.Label NumberOfConnectedDevices;
        private System.Windows.Forms.ColumnHeader ComPort;
        private System.Windows.Forms.Button SendClearCMD;
    }
}