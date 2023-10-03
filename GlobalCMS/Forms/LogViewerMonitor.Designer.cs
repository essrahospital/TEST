namespace GlobalCMS
{
    partial class LogViewerMonitor
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
            this.LogSizeLabel = new System.Windows.Forms.Label();
            this.LastModLabel = new System.Windows.Forms.Label();
            this.LogSizeValue = new System.Windows.Forms.Label();
            this.LastModValue = new System.Windows.Forms.Label();
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
            this.LoadLogBTN.Location = new System.Drawing.Point(12, 5);
            this.LoadLogBTN.Name = "LoadLogBTN";
            this.LoadLogBTN.Size = new System.Drawing.Size(174, 32);
            this.LoadLogBTN.TabIndex = 2;
            this.LoadLogBTN.Text = "Load Log File";
            this.LoadLogBTN.UseVisualStyleBackColor = true;
            this.LoadLogBTN.Click += new System.EventHandler(this.LoadLogBTN_Click);
            // 
            // LogSizeLabel
            // 
            this.LogSizeLabel.AutoSize = true;
            this.LogSizeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LogSizeLabel.Location = new System.Drawing.Point(202, 6);
            this.LogSizeLabel.Name = "LogSizeLabel";
            this.LogSizeLabel.Size = new System.Drawing.Size(56, 13);
            this.LogSizeLabel.TabIndex = 3;
            this.LogSizeLabel.Text = "Log Size";
            // 
            // LastModLabel
            // 
            this.LastModLabel.AutoSize = true;
            this.LastModLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LastModLabel.Location = new System.Drawing.Point(202, 23);
            this.LastModLabel.Name = "LastModLabel";
            this.LastModLabel.Size = new System.Drawing.Size(83, 13);
            this.LastModLabel.TabIndex = 4;
            this.LastModLabel.Text = "Last Modified";
            // 
            // LogSizeValue
            // 
            this.LogSizeValue.AutoSize = true;
            this.LogSizeValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LogSizeValue.Location = new System.Drawing.Point(299, 6);
            this.LogSizeValue.Name = "LogSizeValue";
            this.LogSizeValue.Size = new System.Drawing.Size(13, 13);
            this.LogSizeValue.TabIndex = 5;
            this.LogSizeValue.Text = "0";
            // 
            // LastModValue
            // 
            this.LastModValue.AutoSize = true;
            this.LastModValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LastModValue.Location = new System.Drawing.Point(299, 23);
            this.LastModValue.Name = "LastModValue";
            this.LastModValue.Size = new System.Drawing.Size(30, 13);
            this.LastModValue.TabIndex = 6;
            this.LastModValue.Text = "Date";
            // 
            // LogViewerMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 385);
            this.Controls.Add(this.LastModValue);
            this.Controls.Add(this.LogSizeValue);
            this.Controls.Add(this.LastModLabel);
            this.Controls.Add(this.LogSizeLabel);
            this.Controls.Add(this.LoadLogBTN);
            this.Controls.Add(this.listView1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(640, 424);
            this.MinimumSize = new System.Drawing.Size(640, 424);
            this.Name = "LogViewerMonitor";
            this.ShowIcon = false;
            this.Text = "LogViewerMonitor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Button LoadLogBTN;
        private System.Windows.Forms.Label LogSizeLabel;
        private System.Windows.Forms.Label LastModLabel;
        private System.Windows.Forms.Label LogSizeValue;
        private System.Windows.Forms.Label LastModValue;
    }
}