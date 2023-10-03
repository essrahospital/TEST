namespace GlobalCMS
{
    partial class LiveSupport
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
            this.WebView = new EO.WebBrowser.WebView();
            this.wBrowser = new EO.WinForm.WebControl();
            this.SuspendLayout();
            // 
            // WebView
            // 
            this.WebView.AllowDropLoad = false;
            this.WebView.InputMsgFilter = null;
            this.WebView.ObjectForScripting = null;
            this.WebView.Title = null;
            // 
            // wBrowser
            // 
            this.wBrowser.BackColor = System.Drawing.Color.Black;
            this.wBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wBrowser.Location = new System.Drawing.Point(0, 0);
            this.wBrowser.Margin = new System.Windows.Forms.Padding(0);
            this.wBrowser.Name = "wBrowser";
            this.wBrowser.Size = new System.Drawing.Size(471, 352);
            this.wBrowser.TabIndex = 1;
            this.wBrowser.Text = "wBrowser";
            this.wBrowser.WebView = this.WebView;
            // 
            // LiveSupport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(471, 352);
            this.Controls.Add(this.wBrowser);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(487, 391);
            this.MinimumSize = new System.Drawing.Size(487, 391);
            this.Name = "LiveSupport";
            this.ShowIcon = false;
            this.Text = "SupportChat";
            this.Load += new System.EventHandler(this.LiveSupport_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private EO.WebBrowser.WebView WebView;
        public EO.WinForm.WebControl wBrowser;
    }
}