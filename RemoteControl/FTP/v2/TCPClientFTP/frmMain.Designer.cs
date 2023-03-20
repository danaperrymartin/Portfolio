
namespace TCPClientFTP
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lbMsg = new System.Windows.Forms.ListBox();
            this.pbxStatus = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.imageListLED = new System.Windows.Forms.ImageList(this.components);
            this.btnSendMsg = new System.Windows.Forms.Button();
            this.ucIPAddress1 = new TCPClientFTP.ucIPAddress();
            ((System.ComponentModel.ISupportInitialize)(this.pbxStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(637, 16);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(139, 17);
            this.label3.TabIndex = 12;
            this.label3.Text = "Connection Address:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 186);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 17);
            this.label2.TabIndex = 11;
            this.label2.Text = "Messages:";
            // 
            // lbMsg
            // 
            this.lbMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbMsg.FormattingEnabled = true;
            this.lbMsg.HorizontalScrollbar = true;
            this.lbMsg.ItemHeight = 16;
            this.lbMsg.Location = new System.Drawing.Point(16, 206);
            this.lbMsg.Margin = new System.Windows.Forms.Padding(4);
            this.lbMsg.Name = "lbMsg";
            this.lbMsg.ScrollAlwaysVisible = true;
            this.lbMsg.Size = new System.Drawing.Size(835, 276);
            this.lbMsg.TabIndex = 10;
            // 
            // pbxStatus
            // 
            this.pbxStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbxStatus.Image = ((System.Drawing.Image)(resources.GetObject("pbxStatus.Image")));
            this.pbxStatus.Location = new System.Drawing.Point(384, 36);
            this.pbxStatus.Margin = new System.Windows.Forms.Padding(4);
            this.pbxStatus.Name = "pbxStatus";
            this.pbxStatus.Size = new System.Drawing.Size(43, 40);
            this.pbxStatus.TabIndex = 9;
            this.pbxStatus.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(247, 42);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 17);
            this.label1.TabIndex = 8;
            this.label1.Text = "Connection Status:";
            // 
            // imageListLED
            // 
            this.imageListLED.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListLED.ImageStream")));
            this.imageListLED.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListLED.Images.SetKeyName(0, "GrayLED.ico");
            this.imageListLED.Images.SetKeyName(1, "GreenLED.ico");
            this.imageListLED.Images.SetKeyName(2, "YellowLED.ico");
            this.imageListLED.Images.SetKeyName(3, "RedLED.ico");
            // 
            // btnSendMsg
            // 
            this.btnSendMsg.Location = new System.Drawing.Point(251, 150);
            this.btnSendMsg.Margin = new System.Windows.Forms.Padding(4);
            this.btnSendMsg.Name = "btnSendMsg";
            this.btnSendMsg.Size = new System.Drawing.Size(136, 28);
            this.btnSendMsg.TabIndex = 14;
            this.btnSendMsg.Text = "Send Message";
            this.btnSendMsg.UseVisualStyleBackColor = true;
            this.btnSendMsg.Click += new System.EventHandler(this.btnSendMsg_Click);
            // 
            // ucIPAddress1
            // 
            this.ucIPAddress1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ucIPAddress1.IPAddress = "96.69.156.94:6001";
            //this.ucIPAddress1.IPAddress = "192.168.0.9:6000";
            this.ucIPAddress1.Location = new System.Drawing.Point(641, 42);
            this.ucIPAddress1.Margin = new System.Windows.Forms.Padding(5);
            this.ucIPAddress1.Name = "ucIPAddress1";
            this.ucIPAddress1.Size = new System.Drawing.Size(317, 34);
            this.ucIPAddress1.TabIndex = 13;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1063, 502);
            this.Controls.Add(this.btnSendMsg);
            this.Controls.Add(this.ucIPAddress1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbMsg);
            this.Controls.Add(this.pbxStatus);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmMain";
            this.Text = "TCP Client Test";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbxStatus)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox lbMsg;
        private System.Windows.Forms.PictureBox pbxStatus;
        private System.Windows.Forms.Label label1;
        private ucIPAddress ucIPAddress1;
        private System.Windows.Forms.ImageList imageListLED;
        private System.Windows.Forms.Button btnSendMsg;
    }
}

