namespace GlobalCMS
{
    partial class PowerTimerWindow
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
            this.label1 = new System.Windows.Forms.Label();
            this.CurDay = new System.Windows.Forms.Label();
            this.CurTime = new System.Windows.Forms.Label();
            this.TimeLabel = new System.Windows.Forms.Label();
            this.DayLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ScrOn = new System.Windows.Forms.Label();
            this.ScrOff = new System.Windows.Forms.Label();
            this.PowerScheduleTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(232, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "Screen Schedule Timer";
            // 
            // CurDay
            // 
            this.CurDay.AutoSize = true;
            this.CurDay.Location = new System.Drawing.Point(53, 45);
            this.CurDay.Name = "CurDay";
            this.CurDay.Size = new System.Drawing.Size(42, 13);
            this.CurDay.TabIndex = 1;
            this.CurDay.Text = "CurDay";
            // 
            // CurTime
            // 
            this.CurTime.AutoSize = true;
            this.CurTime.Location = new System.Drawing.Point(54, 68);
            this.CurTime.Name = "CurTime";
            this.CurTime.Size = new System.Drawing.Size(46, 13);
            this.CurTime.TabIndex = 2;
            this.CurTime.Text = "CurTime";
            // 
            // TimeLabel
            // 
            this.TimeLabel.AutoSize = true;
            this.TimeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TimeLabel.Location = new System.Drawing.Point(15, 68);
            this.TimeLabel.Name = "TimeLabel";
            this.TimeLabel.Size = new System.Drawing.Size(34, 13);
            this.TimeLabel.TabIndex = 4;
            this.TimeLabel.Text = "Time";
            // 
            // DayLabel
            // 
            this.DayLabel.AutoSize = true;
            this.DayLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DayLabel.Location = new System.Drawing.Point(15, 45);
            this.DayLabel.Name = "DayLabel";
            this.DayLabel.Size = new System.Drawing.Size(29, 13);
            this.DayLabel.TabIndex = 3;
            this.DayLabel.Text = "Day";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(130, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Screen On";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(130, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Screen Off";
            // 
            // ScrOn
            // 
            this.ScrOn.AutoSize = true;
            this.ScrOn.Location = new System.Drawing.Point(202, 45);
            this.ScrOn.Name = "ScrOn";
            this.ScrOn.Size = new System.Drawing.Size(37, 13);
            this.ScrOn.TabIndex = 7;
            this.ScrOn.Text = "ScrOn";
            // 
            // ScrOff
            // 
            this.ScrOff.AutoSize = true;
            this.ScrOff.Location = new System.Drawing.Point(202, 68);
            this.ScrOff.Name = "ScrOff";
            this.ScrOff.Size = new System.Drawing.Size(37, 13);
            this.ScrOff.TabIndex = 8;
            this.ScrOff.Text = "ScrOff";
            // 
            // PowerScheduleTimer
            // 
            this.PowerScheduleTimer.Enabled = true;
            this.PowerScheduleTimer.Interval = 50000;
            this.PowerScheduleTimer.Tick += new System.EventHandler(this.PowerScheduleTimer_Tick);
            // 
            // PowerTimerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(254, 93);
            this.Controls.Add(this.ScrOff);
            this.Controls.Add(this.ScrOn);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TimeLabel);
            this.Controls.Add(this.DayLabel);
            this.Controls.Add(this.CurTime);
            this.Controls.Add(this.CurDay);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "PowerTimerWindow";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "PowerTimer";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label CurDay;
        private System.Windows.Forms.Label CurTime;
        private System.Windows.Forms.Label TimeLabel;
        private System.Windows.Forms.Label DayLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label ScrOn;
        private System.Windows.Forms.Label ScrOff;
        private System.Windows.Forms.Timer PowerScheduleTimer;
    }
}