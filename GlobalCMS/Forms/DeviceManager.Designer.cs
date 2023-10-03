namespace GlobalCMS
{
    partial class DeviceManager
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
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.CollapseBTN = new System.Windows.Forms.Button();
            this.ExpandBTN = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.SensorBoardDriverResult = new System.Windows.Forms.Label();
            this.AVACameraDriverResult = new System.Windows.Forms.Label();
            this.ENVSensorDriverResult = new System.Windows.Forms.Label();
            this.NexusSensorsDriverResult = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(16, 41);
            this.treeView1.Name = "treeView1";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(345, 382);
            this.treeView1.TabIndex = 0;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // CollapseBTN
            // 
            this.CollapseBTN.Enabled = false;
            this.CollapseBTN.Location = new System.Drawing.Point(205, 12);
            this.CollapseBTN.Name = "CollapseBTN";
            this.CollapseBTN.Size = new System.Drawing.Size(75, 23);
            this.CollapseBTN.TabIndex = 1;
            this.CollapseBTN.Text = "Collapse All";
            this.CollapseBTN.UseVisualStyleBackColor = true;
            this.CollapseBTN.Click += new System.EventHandler(this.CollapseBTN_Click);
            // 
            // ExpandBTN
            // 
            this.ExpandBTN.Location = new System.Drawing.Point(286, 12);
            this.ExpandBTN.Name = "ExpandBTN";
            this.ExpandBTN.Size = new System.Drawing.Size(75, 23);
            this.ExpandBTN.TabIndex = 2;
            this.ExpandBTN.Text = "Expand All";
            this.ExpandBTN.UseVisualStyleBackColor = true;
            this.ExpandBTN.Click += new System.EventHandler(this.ExpandBTN_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 24);
            this.label1.TabIndex = 3;
            this.label1.Text = "Hardware";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(380, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 24);
            this.label2.TabIndex = 4;
            this.label2.Text = "Drivers";
            // 
            // listView1
            // 
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(384, 41);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(327, 397);
            this.listView1.TabIndex = 5;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.List;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 426);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 18);
            this.label3.TabIndex = 6;
            this.label3.Text = "Drivers";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(13, 448);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 16);
            this.label4.TabIndex = 7;
            this.label4.Text = "Sensor Board";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(13, 468);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(86, 16);
            this.label5.TabIndex = 8;
            this.label5.Text = "AVA Camera";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(13, 487);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 16);
            this.label6.TabIndex = 9;
            this.label6.Text = "Env Sensor";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(13, 505);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(99, 16);
            this.label7.TabIndex = 10;
            this.label7.Text = "Nexus Sensors";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SensorBoardDriverResult
            // 
            this.SensorBoardDriverResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SensorBoardDriverResult.Location = new System.Drawing.Point(204, 448);
            this.SensorBoardDriverResult.Name = "SensorBoardDriverResult";
            this.SensorBoardDriverResult.Size = new System.Drawing.Size(156, 16);
            this.SensorBoardDriverResult.TabIndex = 11;
            this.SensorBoardDriverResult.Text = "Not Installed";
            this.SensorBoardDriverResult.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // AVACameraDriverResult
            // 
            this.AVACameraDriverResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AVACameraDriverResult.Location = new System.Drawing.Point(204, 468);
            this.AVACameraDriverResult.Name = "AVACameraDriverResult";
            this.AVACameraDriverResult.Size = new System.Drawing.Size(156, 16);
            this.AVACameraDriverResult.TabIndex = 12;
            this.AVACameraDriverResult.Text = "Not Installed";
            this.AVACameraDriverResult.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ENVSensorDriverResult
            // 
            this.ENVSensorDriverResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ENVSensorDriverResult.Location = new System.Drawing.Point(204, 487);
            this.ENVSensorDriverResult.Name = "ENVSensorDriverResult";
            this.ENVSensorDriverResult.Size = new System.Drawing.Size(156, 16);
            this.ENVSensorDriverResult.TabIndex = 13;
            this.ENVSensorDriverResult.Text = "Not Installed";
            this.ENVSensorDriverResult.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // NexusSensorsDriverResult
            // 
            this.NexusSensorsDriverResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NexusSensorsDriverResult.Location = new System.Drawing.Point(204, 505);
            this.NexusSensorsDriverResult.Name = "NexusSensorsDriverResult";
            this.NexusSensorsDriverResult.Size = new System.Drawing.Size(156, 16);
            this.NexusSensorsDriverResult.TabIndex = 14;
            this.NexusSensorsDriverResult.Text = "Not Installed";
            this.NexusSensorsDriverResult.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // DeviceManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(376, 530);
            this.Controls.Add(this.NexusSensorsDriverResult);
            this.Controls.Add(this.ENVSensorDriverResult);
            this.Controls.Add(this.AVACameraDriverResult);
            this.Controls.Add(this.SensorBoardDriverResult);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ExpandBTN);
            this.Controls.Add(this.CollapseBTN);
            this.Controls.Add(this.treeView1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(392, 569);
            this.MinimumSize = new System.Drawing.Size(392, 569);
            this.Name = "DeviceManager";
            this.ShowIcon = false;
            this.Text = "Device Manager";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button CollapseBTN;
        private System.Windows.Forms.Button ExpandBTN;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label SensorBoardDriverResult;
        private System.Windows.Forms.Label AVACameraDriverResult;
        private System.Windows.Forms.Label ENVSensorDriverResult;
        private System.Windows.Forms.Label NexusSensorsDriverResult;
    }
}