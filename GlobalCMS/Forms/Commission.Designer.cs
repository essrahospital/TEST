namespace GlobalCMS
{
    partial class Commission
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
            this.InstallationProgressBar = new System.Windows.Forms.ProgressBar();
            this.InstallationProgressLabel = new System.Windows.Forms.Label();
            this.macLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.IPDetailsGroupBox = new System.Windows.Forms.GroupBox();
            this.devWAN = new System.Windows.Forms.Label();
            this.devVPN = new System.Windows.Forms.Label();
            this.devLAN = new System.Windows.Forms.Label();
            this.devWANLabel = new System.Windows.Forms.Label();
            this.devVPNLabel = new System.Windows.Forms.Label();
            this.devLANLabel = new System.Windows.Forms.Label();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.triggerStatusTxt = new System.Windows.Forms.Label();
            this.ComTimer = new System.Windows.Forms.Timer(this.components);
            this.TheBarCode = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.VersionLabel = new System.Windows.Forms.Label();
            this.MacTimer = new System.Windows.Forms.Timer(this.components);
            this.IPDetailsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TheBarCode)).BeginInit();
            this.SuspendLayout();
            // 
            // InstallationProgressBar
            // 
            this.InstallationProgressBar.Location = new System.Drawing.Point(271, 24);
            this.InstallationProgressBar.Name = "InstallationProgressBar";
            this.InstallationProgressBar.Size = new System.Drawing.Size(218, 23);
            this.InstallationProgressBar.TabIndex = 0;
            // 
            // InstallationProgressLabel
            // 
            this.InstallationProgressLabel.AutoSize = true;
            this.InstallationProgressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InstallationProgressLabel.Location = new System.Drawing.Point(269, 5);
            this.InstallationProgressLabel.Name = "InstallationProgressLabel";
            this.InstallationProgressLabel.Size = new System.Drawing.Size(180, 16);
            this.InstallationProgressLabel.TabIndex = 1;
            this.InstallationProgressLabel.Text = "Commissioning Progress";
            // 
            // macLabel
            // 
            this.macLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 34F, System.Drawing.FontStyle.Bold);
            this.macLabel.Location = new System.Drawing.Point(22, 157);
            this.macLabel.MaximumSize = new System.Drawing.Size(473, 53);
            this.macLabel.MinimumSize = new System.Drawing.Size(473, 53);
            this.macLabel.Name = "macLabel";
            this.macLabel.Size = new System.Drawing.Size(473, 53);
            this.macLabel.TabIndex = 2;
            this.macLabel.Text = "00:00:00:00:00:00";
            this.macLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(28, 137);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(467, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = " MAC Address for Commissioning";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // IPDetailsGroupBox
            // 
            this.IPDetailsGroupBox.Controls.Add(this.devWAN);
            this.IPDetailsGroupBox.Controls.Add(this.devVPN);
            this.IPDetailsGroupBox.Controls.Add(this.devLAN);
            this.IPDetailsGroupBox.Controls.Add(this.devWANLabel);
            this.IPDetailsGroupBox.Controls.Add(this.devVPNLabel);
            this.IPDetailsGroupBox.Controls.Add(this.devLANLabel);
            this.IPDetailsGroupBox.Location = new System.Drawing.Point(32, 12);
            this.IPDetailsGroupBox.Name = "IPDetailsGroupBox";
            this.IPDetailsGroupBox.Size = new System.Drawing.Size(221, 118);
            this.IPDetailsGroupBox.TabIndex = 6;
            this.IPDetailsGroupBox.TabStop = false;
            this.IPDetailsGroupBox.Text = "IP Details";
            // 
            // devWAN
            // 
            this.devWAN.AutoSize = true;
            this.devWAN.Location = new System.Drawing.Point(94, 84);
            this.devWAN.Name = "devWAN";
            this.devWAN.Size = new System.Drawing.Size(10, 13);
            this.devWAN.TabIndex = 5;
            this.devWAN.Text = " ";
            // 
            // devVPN
            // 
            this.devVPN.AutoSize = true;
            this.devVPN.Location = new System.Drawing.Point(94, 57);
            this.devVPN.Name = "devVPN";
            this.devVPN.Size = new System.Drawing.Size(10, 13);
            this.devVPN.TabIndex = 5;
            this.devVPN.Text = " ";
            // 
            // devLAN
            // 
            this.devLAN.AutoSize = true;
            this.devLAN.Location = new System.Drawing.Point(94, 30);
            this.devLAN.Name = "devLAN";
            this.devLAN.Size = new System.Drawing.Size(10, 13);
            this.devLAN.TabIndex = 4;
            this.devLAN.Text = " ";
            // 
            // devWANLabel
            // 
            this.devWANLabel.AutoSize = true;
            this.devWANLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.devWANLabel.Location = new System.Drawing.Point(22, 84);
            this.devWANLabel.Name = "devWANLabel";
            this.devWANLabel.Size = new System.Drawing.Size(52, 13);
            this.devWANLabel.TabIndex = 3;
            this.devWANLabel.Text = "WAN IP";
            // 
            // devVPNLabel
            // 
            this.devVPNLabel.AutoSize = true;
            this.devVPNLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.devVPNLabel.Location = new System.Drawing.Point(26, 57);
            this.devVPNLabel.Name = "devVPNLabel";
            this.devVPNLabel.Size = new System.Drawing.Size(48, 13);
            this.devVPNLabel.TabIndex = 2;
            this.devVPNLabel.Text = "VPN IP";
            // 
            // devLANLabel
            // 
            this.devLANLabel.AutoSize = true;
            this.devLANLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.devLANLabel.Location = new System.Drawing.Point(27, 30);
            this.devLANLabel.Name = "devLANLabel";
            this.devLANLabel.Size = new System.Drawing.Size(47, 13);
            this.devLANLabel.TabIndex = 1;
            this.devLANLabel.Text = "LAN IP";
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.BackColor = System.Drawing.SystemColors.Control;
            this.checkedListBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.checkedListBox1.Enabled = false;
            this.checkedListBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Items.AddRange(new object[] {
            "Installing Remote Desktop",
            "Setting Up Device ID",
            "Installing Configuration File",
            "Installing Secure Network",
            "Submitting Bios and System Information to Server",
            "Creating Configuration Backup Failsafe",
            "Installing 3rd Party Applications"});
            this.checkedListBox1.Location = new System.Drawing.Point(33, 242);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(384, 133);
            this.checkedListBox1.TabIndex = 7;
            // 
            // triggerStatusTxt
            // 
            this.triggerStatusTxt.AutoSize = true;
            this.triggerStatusTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.triggerStatusTxt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.triggerStatusTxt.Location = new System.Drawing.Point(284, 50);
            this.triggerStatusTxt.Name = "triggerStatusTxt";
            this.triggerStatusTxt.Size = new System.Drawing.Size(192, 20);
            this.triggerStatusTxt.TabIndex = 8;
            this.triggerStatusTxt.Text = "!! Waiting For Trigger !!";
            // 
            // ComTimer
            // 
            this.ComTimer.Enabled = true;
            this.ComTimer.Interval = 30000;
            this.ComTimer.Tick += new System.EventHandler(this.ComTimer_Tick);
            // 
            // TheBarCode
            // 
            this.TheBarCode.BackColor = System.Drawing.Color.Transparent;
            this.TheBarCode.Location = new System.Drawing.Point(263, 73);
            this.TheBarCode.Name = "TheBarCode";
            this.TheBarCode.Size = new System.Drawing.Size(232, 57);
            this.TheBarCode.TabIndex = 9;
            this.TheBarCode.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(29, 217);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 20);
            this.label2.TabIndex = 10;
            this.label2.Text = "Status";
            // 
            // VersionLabel
            // 
            this.VersionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.VersionLabel.Location = new System.Drawing.Point(24, 395);
            this.VersionLabel.Name = "VersionLabel";
            this.VersionLabel.Size = new System.Drawing.Size(469, 15);
            this.VersionLabel.TabIndex = 11;
            this.VersionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MacTimer
            // 
            this.MacTimer.Enabled = true;
            this.MacTimer.Interval = 10000;
            this.MacTimer.Tick += new System.EventHandler(this.MacTimer_Tick);
            // 
            // Commission
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 425);
            this.ControlBox = false;
            this.Controls.Add(this.VersionLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TheBarCode);
            this.Controls.Add(this.triggerStatusTxt);
            this.Controls.Add(this.checkedListBox1);
            this.Controls.Add(this.IPDetailsGroupBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.macLabel);
            this.Controls.Add(this.InstallationProgressLabel);
            this.Controls.Add(this.InstallationProgressBar);
            this.Name = "Commission";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Commissioning System";
            this.Load += new System.EventHandler(this.Commission_Load);
            this.IPDetailsGroupBox.ResumeLayout(false);
            this.IPDetailsGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TheBarCode)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar InstallationProgressBar;
        private System.Windows.Forms.Label InstallationProgressLabel;
        private System.Windows.Forms.Label macLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox IPDetailsGroupBox;
        private System.Windows.Forms.Label devWAN;
        private System.Windows.Forms.Label devVPN;
        private System.Windows.Forms.Label devLAN;
        private System.Windows.Forms.Label devWANLabel;
        private System.Windows.Forms.Label devVPNLabel;
        private System.Windows.Forms.Label devLANLabel;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.Label triggerStatusTxt;
        private System.Windows.Forms.Timer ComTimer;
        private System.Windows.Forms.PictureBox TheBarCode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label VersionLabel;
        private System.Windows.Forms.Timer MacTimer;
    }
}