namespace GlobalCMS
{
    partial class GDPR
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
            this.GDPRLoadingIMG = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.GDPRLoadingIMG)).BeginInit();
            this.SuspendLayout();
            // 
            // GDPRLoadingIMG
            // 
            this.GDPRLoadingIMG.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GDPRLoadingIMG.Image = global::GlobalCMS.Properties.Resources.SKIN_DEFAULT_GDPR;
            this.GDPRLoadingIMG.Location = new System.Drawing.Point(0, 0);
            this.GDPRLoadingIMG.Name = "GDPRLoadingIMG";
            this.GDPRLoadingIMG.Size = new System.Drawing.Size(800, 600);
            this.GDPRLoadingIMG.TabIndex = 0;
            this.GDPRLoadingIMG.TabStop = false;
            // 
            // GDPR
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.ControlBox = false;
            this.Controls.Add(this.GDPRLoadingIMG);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GDPR";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GDPR";
            this.TopMost = true;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.GDPR_Load);
            ((System.ComponentModel.ISupportInitialize)(this.GDPRLoadingIMG)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox GDPRLoadingIMG;
    }
}