namespace GlobalCMS
{
    partial class SystemDebug
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
            this.RefreshBTN = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.JSONBox = new System.Windows.Forms.RichTextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ClearHistoryBTN = new System.Windows.Forms.Button();
            this.WebsocketBox = new System.Windows.Forms.RichTextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.OfflinePwrStatus = new System.Windows.Forms.Label();
            this.OfflinePwrOnLabel = new System.Windows.Forms.Label();
            this.OfflinePwrOffLabel = new System.Windows.Forms.Label();
            this.OfflinePwrOn = new System.Windows.Forms.Label();
            this.OfflinePwrOff = new System.Windows.Forms.Label();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // RefreshBTN
            // 
            this.RefreshBTN.Location = new System.Drawing.Point(139, 7);
            this.RefreshBTN.Name = "RefreshBTN";
            this.RefreshBTN.Size = new System.Drawing.Size(174, 23);
            this.RefreshBTN.TabIndex = 0;
            this.RefreshBTN.Text = "Refresh Signage Info";
            this.RefreshBTN.UseVisualStyleBackColor = true;
            this.RefreshBTN.Click += new System.EventHandler(this.RefreshBTN_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.RefreshBTN);
            this.panel2.Location = new System.Drawing.Point(6, 344);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(454, 38);
            this.panel2.TabIndex = 2;
            // 
            // JSONBox
            // 
            this.JSONBox.BackColor = System.Drawing.SystemColors.Control;
            this.JSONBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.JSONBox.Location = new System.Drawing.Point(6, 19);
            this.JSONBox.Name = "JSONBox";
            this.JSONBox.ReadOnly = true;
            this.JSONBox.Size = new System.Drawing.Size(454, 319);
            this.JSONBox.TabIndex = 0;
            this.JSONBox.Text = "";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.JSONBox);
            this.groupBox1.Controls.Add(this.panel2);
            this.groupBox1.Location = new System.Drawing.Point(12, 79);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(466, 388);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Signage Info";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.panel1);
            this.groupBox2.Controls.Add(this.WebsocketBox);
            this.groupBox2.Location = new System.Drawing.Point(484, 79);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(259, 388);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Monitor Websockets";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ClearHistoryBTN);
            this.panel1.Location = new System.Drawing.Point(6, 344);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(247, 38);
            this.panel1.TabIndex = 3;
            // 
            // ClearHistoryBTN
            // 
            this.ClearHistoryBTN.Location = new System.Drawing.Point(71, 7);
            this.ClearHistoryBTN.Name = "ClearHistoryBTN";
            this.ClearHistoryBTN.Size = new System.Drawing.Size(116, 23);
            this.ClearHistoryBTN.TabIndex = 0;
            this.ClearHistoryBTN.Text = "Clear History";
            this.ClearHistoryBTN.UseVisualStyleBackColor = true;
            this.ClearHistoryBTN.Click += new System.EventHandler(this.ClearHistoryBTN_Click);
            // 
            // WebsocketBox
            // 
            this.WebsocketBox.BackColor = System.Drawing.SystemColors.Control;
            this.WebsocketBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.WebsocketBox.Location = new System.Drawing.Point(6, 19);
            this.WebsocketBox.Name = "WebsocketBox";
            this.WebsocketBox.ReadOnly = true;
            this.WebsocketBox.Size = new System.Drawing.Size(247, 319);
            this.WebsocketBox.TabIndex = 1;
            this.WebsocketBox.Text = "";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.OfflinePwrOff);
            this.groupBox3.Controls.Add(this.OfflinePwrOn);
            this.groupBox3.Controls.Add(this.OfflinePwrOffLabel);
            this.groupBox3.Controls.Add(this.OfflinePwrOnLabel);
            this.groupBox3.Controls.Add(this.OfflinePwrStatus);
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(731, 61);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Offline Power Scheduler";
            // 
            // OfflinePwrStatus
            // 
            this.OfflinePwrStatus.AutoSize = true;
            this.OfflinePwrStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OfflinePwrStatus.ForeColor = System.Drawing.Color.Red;
            this.OfflinePwrStatus.Location = new System.Drawing.Point(11, 21);
            this.OfflinePwrStatus.Name = "OfflinePwrStatus";
            this.OfflinePwrStatus.Size = new System.Drawing.Size(105, 24);
            this.OfflinePwrStatus.TabIndex = 0;
            this.OfflinePwrStatus.Text = "Not Active";
            // 
            // OfflinePwrOnLabel
            // 
            this.OfflinePwrOnLabel.AutoSize = true;
            this.OfflinePwrOnLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OfflinePwrOnLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.OfflinePwrOnLabel.Location = new System.Drawing.Point(243, 24);
            this.OfflinePwrOnLabel.Name = "OfflinePwrOnLabel";
            this.OfflinePwrOnLabel.Size = new System.Drawing.Size(94, 20);
            this.OfflinePwrOnLabel.TabIndex = 1;
            this.OfflinePwrOnLabel.Text = "Screen On";
            // 
            // OfflinePwrOffLabel
            // 
            this.OfflinePwrOffLabel.AutoSize = true;
            this.OfflinePwrOffLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OfflinePwrOffLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.OfflinePwrOffLabel.Location = new System.Drawing.Point(490, 24);
            this.OfflinePwrOffLabel.Name = "OfflinePwrOffLabel";
            this.OfflinePwrOffLabel.Size = new System.Drawing.Size(96, 20);
            this.OfflinePwrOffLabel.TabIndex = 2;
            this.OfflinePwrOffLabel.Text = "Screen Off";
            // 
            // OfflinePwrOn
            // 
            this.OfflinePwrOn.AutoSize = true;
            this.OfflinePwrOn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OfflinePwrOn.ForeColor = System.Drawing.SystemColors.ControlText;
            this.OfflinePwrOn.Location = new System.Drawing.Point(352, 24);
            this.OfflinePwrOn.Name = "OfflinePwrOn";
            this.OfflinePwrOn.Size = new System.Drawing.Size(85, 20);
            this.OfflinePwrOn.TabIndex = 3;
            this.OfflinePwrOn.Text = "Screen On";
            // 
            // OfflinePwrOff
            // 
            this.OfflinePwrOff.AutoSize = true;
            this.OfflinePwrOff.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OfflinePwrOff.ForeColor = System.Drawing.SystemColors.ControlText;
            this.OfflinePwrOff.Location = new System.Drawing.Point(603, 24);
            this.OfflinePwrOff.Name = "OfflinePwrOff";
            this.OfflinePwrOff.Size = new System.Drawing.Size(86, 20);
            this.OfflinePwrOff.TabIndex = 4;
            this.OfflinePwrOff.Text = "Screen Off";
            // 
            // SystemDebug
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(753, 476);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(769, 515);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(769, 515);
            this.Name = "SystemDebug";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SystemDebug";
            this.Load += new System.EventHandler(this.SignageBrowserDebug_Load);
            this.panel2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button RefreshBTN;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RichTextBox JSONBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button ClearHistoryBTN;
        public System.Windows.Forms.RichTextBox WebsocketBox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label OfflinePwrStatus;
        private System.Windows.Forms.Label OfflinePwrOffLabel;
        private System.Windows.Forms.Label OfflinePwrOnLabel;
        private System.Windows.Forms.Label OfflinePwrOff;
        private System.Windows.Forms.Label OfflinePwrOn;
    }
}