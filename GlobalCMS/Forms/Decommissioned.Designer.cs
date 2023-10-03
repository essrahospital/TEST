namespace GlobalCMS
{
    partial class Decommission
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
            this.macLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // macLabel
            // 
            this.macLabel.AutoSize = true;
            this.macLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 34F, System.Drawing.FontStyle.Bold);
            this.macLabel.Location = new System.Drawing.Point(96, 52);
            this.macLabel.Name = "macLabel";
            this.macLabel.Size = new System.Drawing.Size(417, 53);
            this.macLabel.TabIndex = 3;
            this.macLabel.Text = "00:00:00:00:00:00";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.label1.Location = new System.Drawing.Point(23, 156);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(564, 39);
            this.label1.TabIndex = 4;
            this.label1.Text = "Asset Currently Decommissioned";
            // 
            // Decommission
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(609, 264);
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.macLabel);
            this.MaximumSize = new System.Drawing.Size(625, 303);
            this.MinimumSize = new System.Drawing.Size(625, 303);
            this.Name = "Decommission";
            this.ShowIcon = false;
            this.Text = "Decommissioned";
            this.Load += new System.EventHandler(this.Decommission_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label macLabel;
        private System.Windows.Forms.Label label1;
    }
}