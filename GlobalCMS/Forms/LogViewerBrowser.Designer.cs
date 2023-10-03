namespace GlobalCMS
{
    partial class LogViewerBrowser
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
            this.listView1 = new System.Windows.Forms.ListView();
            this.LoadLogBTN = new System.Windows.Forms.Button();
            this.WhichLogDropDwn = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.BackColor = System.Drawing.Color.White;
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView1.CausesValidation = false;
            this.listView1.Enabled = false;
            this.listView1.ForeColor = System.Drawing.Color.Black;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(12, 43);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.ShowGroups = false;
            this.listView1.Size = new System.Drawing.Size(600, 330);
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // LoadLogBTN
            // 
            this.LoadLogBTN.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoadLogBTN.Location = new System.Drawing.Point(495, 5);
            this.LoadLogBTN.Name = "LoadLogBTN";
            this.LoadLogBTN.Size = new System.Drawing.Size(117, 32);
            this.LoadLogBTN.TabIndex = 2;
            this.LoadLogBTN.Text = "Load Log File";
            this.LoadLogBTN.UseVisualStyleBackColor = true;
            this.LoadLogBTN.Click += new System.EventHandler(this.LoadLogBTN_Click);
            // 
            // WhichLogDropDwn
            // 
            this.WhichLogDropDwn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.WhichLogDropDwn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WhichLogDropDwn.FormattingEnabled = true;
            this.WhichLogDropDwn.Items.AddRange(new object[] {
            "LoadFailed Log - When Browser Fails To Load URL",
            "Javascript Stack - When Browser Crashes Due To Rendering Problems",
            "Exception Log - When the Browser Crashes Due To Browser Code Error "});
            this.WhichLogDropDwn.Location = new System.Drawing.Point(12, 10);
            this.WhichLogDropDwn.Name = "WhichLogDropDwn";
            this.WhichLogDropDwn.Size = new System.Drawing.Size(477, 24);
            this.WhichLogDropDwn.TabIndex = 3;
            // 
            // LogViewerBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 385);
            this.Controls.Add(this.WhichLogDropDwn);
            this.Controls.Add(this.LoadLogBTN);
            this.Controls.Add(this.listView1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(640, 424);
            this.MinimumSize = new System.Drawing.Size(640, 424);
            this.Name = "LogViewerBrowser";
            this.ShowIcon = false;
            this.Text = "LogViewerBrowser";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button LoadLogBTN;
        private System.Windows.Forms.ComboBox WhichLogDropDwn;
    }
}