namespace GlobalCMS
{
    partial class AirServerConfig
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
            this.UpdateBTN = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.DeviceAS = new System.Windows.Forms.TextBox();
            this.PasscodeAS = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // UpdateBTN
            // 
            this.UpdateBTN.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UpdateBTN.Location = new System.Drawing.Point(14, 210);
            this.UpdateBTN.Name = "UpdateBTN";
            this.UpdateBTN.Size = new System.Drawing.Size(311, 23);
            this.UpdateBTN.TabIndex = 12;
            this.UpdateBTN.Text = "Update AirServer Config";
            this.UpdateBTN.UseVisualStyleBackColor = true;
            this.UpdateBTN.Click += new System.EventHandler(this.UpdateBTN_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(11, 192);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(316, 13);
            this.label13.TabIndex = 50;
            this.label13.Text = "---------------------------------------------------------------------------------" +
    "----------------------";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 24);
            this.label1.TabIndex = 51;
            this.label1.Text = "Device Name";
            // 
            // DeviceAS
            // 
            this.DeviceAS.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DeviceAS.Location = new System.Drawing.Point(16, 45);
            this.DeviceAS.Name = "DeviceAS";
            this.DeviceAS.Size = new System.Drawing.Size(307, 29);
            this.DeviceAS.TabIndex = 53;
            // 
            // PasscodeAS
            // 
            this.PasscodeAS.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PasscodeAS.Location = new System.Drawing.Point(16, 135);
            this.PasscodeAS.Name = "PasscodeAS";
            this.PasscodeAS.Size = new System.Drawing.Size(307, 29);
            this.PasscodeAS.TabIndex = 55;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 99);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(156, 24);
            this.label2.TabIndex = 54;
            this.label2.Text = "Device Passcode";
            // 
            // AirServerConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(338, 247);
            this.Controls.Add(this.PasscodeAS);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.DeviceAS);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.UpdateBTN);
            this.MaximizeBox = false;
            this.Name = "AirServerConfig";
            this.ShowIcon = false;
            this.Text = "AirServer Config";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button UpdateBTN;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox DeviceAS;
        private System.Windows.Forms.TextBox PasscodeAS;
        private System.Windows.Forms.Label label2;
    }
}