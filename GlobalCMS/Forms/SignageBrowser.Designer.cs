namespace GlobalCMS
{
    partial class SignageBrowser
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
            this.wBrowser = new EO.WinForm.WebControl();
            this.WebView = new EO.WebBrowser.WebView();
            this.AlwaysOnTop = new System.Windows.Forms.Timer(this.components);
            this.CheckForAirServerClient = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // wBrowser
            // 
            this.wBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.wBrowser.BackColor = System.Drawing.Color.Black;
            this.wBrowser.ForeColor = System.Drawing.Color.White;
            this.wBrowser.Location = new System.Drawing.Point(0, 0);
            this.wBrowser.Margin = new System.Windows.Forms.Padding(0);
            this.wBrowser.Name = "wBrowser";
            this.wBrowser.Size = new System.Drawing.Size(800, 450);
            this.wBrowser.TabIndex = 0;
            this.wBrowser.Text = "wBrowser";
            this.wBrowser.WebView = this.WebView;
            // 
            // WebView
            // 
            this.WebView.AllowDropLoad = false;
            this.WebView.InputMsgFilter = null;
            this.WebView.ObjectForScripting = null;
            this.WebView.Title = null;
            this.WebView.IsLoadingChanged += new System.EventHandler(this.WebView_IsLoadingChanged);
            this.WebView.CanGoBackChanged += new System.EventHandler(this.WebView_CanGoBackChanged);
            this.WebView.CanGoForwardChanged += new System.EventHandler(this.WebView_CanGoForwardChanged);
            this.WebView.UrlChanged += new System.EventHandler(this.WebView_UrlChanged);
            this.WebView.TitleChanged += new System.EventHandler(this.WebView_TitleChanged);
            this.WebView.StatusMessageChanged += new System.EventHandler(this.WebView_StatusMessageChanged);
            this.WebView.ConsoleMessage += new EO.WebBrowser.ConsoleMessageHandler(this.WebView_Console);
            this.WebView.BeforeContextMenu += new EO.WebBrowser.BeforeContextMenuHandler(this.WebView_BeforeContextMenu);
            this.WebView.Command += new EO.WebBrowser.CommandHandler(this.WebView_Command);
            this.WebView.ShouldForceDownload += new EO.WebBrowser.ShouldForceDownloadHandler(this.WebView_ShouldForceDownload);
            this.WebView.BeforeDownload += new EO.WebBrowser.BeforeDownloadHandler(this.WebView_BeforeDownload);
            this.WebView.NewWindow += new EO.WebBrowser.NewWindowHandler(this.WebView_NewWindow);
            this.WebView.CertificateError += new EO.WebBrowser.CertificateErrorHandler(this.WebView_CertError);
            this.WebView.BeforePrint += new EO.WebBrowser.BeforePrintHandler(this.WebView_BeforePrint);
            this.WebView.FocusedNodeChanged += new EO.WebBrowser.DOMNodeEventHandler(this.WebView_FocusNodeChanged);
            // 
            // AlwaysOnTop
            // 
            this.AlwaysOnTop.Enabled = true;
            this.AlwaysOnTop.Interval = 60000;
            this.AlwaysOnTop.Tick += new System.EventHandler(this.AlwaysOnTop_Tick);
            // 
            // CheckForAirServerClient
            // 
            this.CheckForAirServerClient.Interval = 1000;
            this.CheckForAirServerClient.Tick += new System.EventHandler(this.CheckForAirServerClient_Tick);
            // 
            // SignageBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.wBrowser);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SignageBrowser";
            this.ShowIcon = false;
            this.Text = "SignageBrowser";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.SignageBrowser_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private EO.WebBrowser.WebView WebView;
        public EO.WinForm.WebControl wBrowser;
        private System.Windows.Forms.Timer CheckForAirServerClient;
        private System.Windows.Forms.Timer AlwaysOnTop;
    }
}