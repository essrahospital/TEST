namespace GlobalCMS
{
    partial class Rs232
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
            this.listBox1 = new System.Windows.Forms.ListView();
            this.COM = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.BaudRate = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Parity = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.DataBits = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.StopBits = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.isOpen = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Status = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Config = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SendClear = new System.Windows.Forms.Button();
            this.SendCMD = new System.Windows.Forms.Button();
            this.TxtToSend = new System.Windows.Forms.TextBox();
            this.NumberOfConnectedDevices = new System.Windows.Forms.Label();
            this.txtData1 = new System.Windows.Forms.TextBox();
            this.COMListDropdwn = new System.Windows.Forms.ComboBox();
            this.txtData2 = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.AutoArrange = false;
            this.listBox1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.COM,
            this.BaudRate,
            this.Parity,
            this.DataBits,
            this.StopBits,
            this.isOpen,
            this.Status,
            this.Config});
            this.listBox1.FullRowSelect = true;
            this.listBox1.GridLines = true;
            this.listBox1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listBox1.HideSelection = false;
            this.listBox1.Location = new System.Drawing.Point(357, 63);
            this.listBox1.MultiSelect = false;
            this.listBox1.Name = "listBox1";
            this.listBox1.ShowGroups = false;
            this.listBox1.Size = new System.Drawing.Size(421, 180);
            this.listBox1.TabIndex = 12;
            this.listBox1.UseCompatibleStateImageBehavior = false;
            this.listBox1.View = System.Windows.Forms.View.Details;
            // 
            // COM
            // 
            this.COM.Text = "COM";
            this.COM.Width = 46;
            // 
            // BaudRate
            // 
            this.BaudRate.Text = "Baud";
            this.BaudRate.Width = 44;
            // 
            // Parity
            // 
            this.Parity.Text = "Parity";
            this.Parity.Width = 47;
            // 
            // DataBits
            // 
            this.DataBits.Text = "Data";
            this.DataBits.Width = 54;
            // 
            // StopBits
            // 
            this.StopBits.Text = "Stop";
            this.StopBits.Width = 51;
            // 
            // isOpen
            // 
            this.isOpen.Text = "isOpen";
            // 
            // Status
            // 
            this.Status.Text = "Status";
            // 
            // Config
            // 
            this.Config.Text = "Config";
            this.Config.Width = 109;
            // 
            // SendClear
            // 
            this.SendClear.Enabled = false;
            this.SendClear.Location = new System.Drawing.Point(538, 34);
            this.SendClear.Name = "SendClear";
            this.SendClear.Size = new System.Drawing.Size(101, 23);
            this.SendClear.TabIndex = 18;
            this.SendClear.Text = "Clear Log";
            this.SendClear.UseVisualStyleBackColor = true;
            this.SendClear.Click += new System.EventHandler(this.SendClear_Click);
            // 
            // SendCMD
            // 
            this.SendCMD.Enabled = false;
            this.SendCMD.Location = new System.Drawing.Point(357, 34);
            this.SendCMD.Name = "SendCMD";
            this.SendCMD.Size = new System.Drawing.Size(175, 23);
            this.SendCMD.TabIndex = 15;
            this.SendCMD.Text = "Send Control Code String";
            this.SendCMD.UseVisualStyleBackColor = true;
            this.SendCMD.Click += new System.EventHandler(this.SendCMD_Click);
            // 
            // TxtToSend
            // 
            this.TxtToSend.Location = new System.Drawing.Point(359, 7);
            this.TxtToSend.Multiline = true;
            this.TxtToSend.Name = "TxtToSend";
            this.TxtToSend.Size = new System.Drawing.Size(336, 21);
            this.TxtToSend.TabIndex = 16;
            // 
            // NumberOfConnectedDevices
            // 
            this.NumberOfConnectedDevices.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NumberOfConnectedDevices.Location = new System.Drawing.Point(700, 34);
            this.NumberOfConnectedDevices.Margin = new System.Windows.Forms.Padding(0);
            this.NumberOfConnectedDevices.Name = "NumberOfConnectedDevices";
            this.NumberOfConnectedDevices.Size = new System.Drawing.Size(78, 23);
            this.NumberOfConnectedDevices.TabIndex = 19;
            this.NumberOfConnectedDevices.Text = "0 Devices";
            this.NumberOfConnectedDevices.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtData1
            // 
            this.txtData1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtData1.Location = new System.Drawing.Point(8, 16);
            this.txtData1.Margin = new System.Windows.Forms.Padding(8);
            this.txtData1.Multiline = true;
            this.txtData1.Name = "txtData1";
            this.txtData1.ReadOnly = true;
            this.txtData1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtData1.Size = new System.Drawing.Size(329, 99);
            this.txtData1.TabIndex = 20;
            // 
            // COMListDropdwn
            // 
            this.COMListDropdwn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.COMListDropdwn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.COMListDropdwn.FormattingEnabled = true;
            this.COMListDropdwn.Location = new System.Drawing.Point(701, 7);
            this.COMListDropdwn.Name = "COMListDropdwn";
            this.COMListDropdwn.Size = new System.Drawing.Size(77, 21);
            this.COMListDropdwn.TabIndex = 21;
            // 
            // txtData2
            // 
            this.txtData2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtData2.Location = new System.Drawing.Point(8, 16);
            this.txtData2.Margin = new System.Windows.Forms.Padding(8);
            this.txtData2.Multiline = true;
            this.txtData2.Name = "txtData2";
            this.txtData2.ReadOnly = true;
            this.txtData2.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtData2.Size = new System.Drawing.Size(329, 99);
            this.txtData2.TabIndex = 22;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtData1);
            this.groupBox1.Location = new System.Drawing.Point(9, 7);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(339, 118);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Incoming";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtData2);
            this.groupBox2.Location = new System.Drawing.Point(9, 128);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(339, 118);
            this.groupBox2.TabIndex = 24;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Outgoing";
            // 
            // Rs232
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 250);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.COMListDropdwn);
            this.Controls.Add(this.NumberOfConnectedDevices);
            this.Controls.Add(this.SendClear);
            this.Controls.Add(this.SendCMD);
            this.Controls.Add(this.TxtToSend);
            this.Controls.Add(this.listBox1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(800, 289);
            this.MinimumSize = new System.Drawing.Size(800, 289);
            this.Name = "Rs232";
            this.ShowIcon = false;
            this.Text = "RS232";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Rs232_FormClosing);
            this.Load += new System.EventHandler(this.Rs232_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListView listBox1;
        private System.Windows.Forms.ColumnHeader COM;
        private System.Windows.Forms.Button SendClear;
        private System.Windows.Forms.Button SendCMD;
        private System.Windows.Forms.TextBox TxtToSend;
        private System.Windows.Forms.Label NumberOfConnectedDevices;
        private System.Windows.Forms.TextBox txtData1;
        private System.Windows.Forms.ComboBox COMListDropdwn;
        private System.Windows.Forms.ColumnHeader BaudRate;
        private System.Windows.Forms.ColumnHeader Parity;
        private System.Windows.Forms.ColumnHeader StopBits;
        private System.Windows.Forms.ColumnHeader Config;
        private System.Windows.Forms.ColumnHeader DataBits;
        private System.Windows.Forms.ColumnHeader isOpen;
        private System.Windows.Forms.ColumnHeader Status;
        private System.Windows.Forms.TextBox txtData2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}