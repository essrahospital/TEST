namespace GlobalCMS
{
    partial class DownloaderManager
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
            this.DownloadView = new System.Windows.Forms.ListView();
            this.DateTimeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FileNameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SizeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SpeedHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.PercentComplete = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.TestDownloadBTN = new System.Windows.Forms.Button();
            this.CurrentDownloadID = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // DownloadView
            // 
            this.DownloadView.AutoArrange = false;
            this.DownloadView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DownloadView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.DateTimeHeader,
            this.FileNameHeader,
            this.SizeHeader,
            this.SpeedHeader,
            this.PercentComplete});
            this.DownloadView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DownloadView.Enabled = false;
            this.DownloadView.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DownloadView.FullRowSelect = true;
            this.DownloadView.GridLines = true;
            this.DownloadView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.DownloadView.HideSelection = false;
            this.DownloadView.Location = new System.Drawing.Point(0, 0);
            this.DownloadView.MultiSelect = false;
            this.DownloadView.Name = "DownloadView";
            this.DownloadView.ShowGroups = false;
            this.DownloadView.Size = new System.Drawing.Size(610, 141);
            this.DownloadView.TabIndex = 57;
            this.DownloadView.UseCompatibleStateImageBehavior = false;
            this.DownloadView.View = System.Windows.Forms.View.Details;
            // 
            // DateTimeHeader
            // 
            this.DateTimeHeader.Text = "Added";
            this.DateTimeHeader.Width = 100;
            // 
            // FileNameHeader
            // 
            this.FileNameHeader.Text = "File";
            this.FileNameHeader.Width = 211;
            // 
            // SizeHeader
            // 
            this.SizeHeader.Text = "Size";
            this.SizeHeader.Width = 132;
            // 
            // SpeedHeader
            // 
            this.SpeedHeader.Text = "Avg Speed";
            this.SpeedHeader.Width = 95;
            // 
            // PercentComplete
            // 
            this.PercentComplete.Text = "Status";
            this.PercentComplete.Width = 70;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.DownloadView);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.TestDownloadBTN);
            this.panel1.Controls.Add(this.CurrentDownloadID);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(610, 141);
            this.panel1.TabIndex = 58;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(518, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 13);
            this.label1.TabIndex = 60;
            this.label1.Text = "ID";
            // 
            // TestDownloadBTN
            // 
            this.TestDownloadBTN.Location = new System.Drawing.Point(41, 37);
            this.TestDownloadBTN.Name = "TestDownloadBTN";
            this.TestDownloadBTN.Size = new System.Drawing.Size(179, 24);
            this.TestDownloadBTN.TabIndex = 59;
            this.TestDownloadBTN.Text = "Download Test File (50MB)";
            this.TestDownloadBTN.UseVisualStyleBackColor = true;
            this.TestDownloadBTN.Click += new System.EventHandler(this.TestDownloadBTN_Click);
            // 
            // CurrentDownloadID
            // 
            this.CurrentDownloadID.AutoSize = true;
            this.CurrentDownloadID.Location = new System.Drawing.Point(285, 93);
            this.CurrentDownloadID.Name = "CurrentDownloadID";
            this.CurrentDownloadID.Size = new System.Drawing.Size(13, 13);
            this.CurrentDownloadID.TabIndex = 61;
            this.CurrentDownloadID.Text = "0";
            // 
            // DownloaderManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 161);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(650, 200);
            this.MinimumSize = new System.Drawing.Size(650, 200);
            this.Name = "DownloaderManager";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Updates Queue";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ColumnHeader FileNameHeader;
        private System.Windows.Forms.ColumnHeader SpeedHeader;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button TestDownloadBTN;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label CurrentDownloadID;
        private System.Windows.Forms.ColumnHeader PercentComplete;
        private System.Windows.Forms.ColumnHeader SizeHeader;
        public System.Windows.Forms.ListView DownloadView;
        private System.Windows.Forms.ColumnHeader DateTimeHeader;
    }
}