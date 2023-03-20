
namespace TCPServerFTP
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
            this.btnStartServer = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.imageListLED = new System.Windows.Forms.ImageList(this.components);
            this.pbxStatus = new System.Windows.Forms.PictureBox();
            this.lbMsg = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSendMsg = new System.Windows.Forms.Button();
            this.ucIPAddress1 = new TCPServerFTP.ucIPAddress();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.lbl_FileTransferName = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pbxStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // btnStartServer
            // 
            this.btnStartServer.Location = new System.Drawing.Point(16, 44);
            this.btnStartServer.Margin = new System.Windows.Forms.Padding(4);
            this.btnStartServer.Name = "btnStartServer";
            this.btnStartServer.Size = new System.Drawing.Size(100, 28);
            this.btnStartServer.TabIndex = 0;
            this.btnStartServer.Text = "Start Server";
            this.btnStartServer.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(247, 50);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 17);
            this.label1.TabIndex = 1;
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
            // pbxStatus
            // 
            this.pbxStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbxStatus.Image = ((System.Drawing.Image)(resources.GetObject("pbxStatus.Image")));
            this.pbxStatus.Location = new System.Drawing.Point(384, 44);
            this.pbxStatus.Margin = new System.Windows.Forms.Padding(4);
            this.pbxStatus.Name = "pbxStatus";
            this.pbxStatus.Size = new System.Drawing.Size(43, 40);
            this.pbxStatus.TabIndex = 2;
            this.pbxStatus.TabStop = false;
            // 
            // lbMsg
            // 
            this.lbMsg.FormattingEnabled = true;
            this.lbMsg.ItemHeight = 16;
            this.lbMsg.Location = new System.Drawing.Point(16, 168);
            this.lbMsg.Margin = new System.Windows.Forms.Padding(4);
            this.lbMsg.Name = "lbMsg";
            this.lbMsg.ScrollAlwaysVisible = true;
            this.lbMsg.Size = new System.Drawing.Size(1015, 52);
            this.lbMsg.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 137);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "Messages:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(637, 25);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 17);
            this.label3.TabIndex = 6;
            this.label3.Text = "Listen Address:";
            // 
            // btnSendMsg
            // 
            this.btnSendMsg.Location = new System.Drawing.Point(16, 99);
            this.btnSendMsg.Margin = new System.Windows.Forms.Padding(4);
            this.btnSendMsg.Name = "btnSendMsg";
            this.btnSendMsg.Size = new System.Drawing.Size(126, 34);
            this.btnSendMsg.TabIndex = 15;
            this.btnSendMsg.Text = "Send Message";
            this.btnSendMsg.UseVisualStyleBackColor = true;
            this.btnSendMsg.Click += new System.EventHandler(this.btnSendMsg_Click);
            // 
            // ucIPAddress1
            // 
            this.ucIPAddress1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ucIPAddress1.IPAddress = "192.168.0.3:6001";
            this.ucIPAddress1.Location = new System.Drawing.Point(641, 44);
            this.ucIPAddress1.Margin = new System.Windows.Forms.Padding(5);
            this.ucIPAddress1.Name = "ucIPAddress1";
            this.ucIPAddress1.Size = new System.Drawing.Size(317, 34);
            this.ucIPAddress1.TabIndex = 5;
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(19, 529);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(314, 13);
            this.progressBar1.TabIndex = 16;
            // 
            // lbl_FileTransferName
            // 
            this.lbl_FileTransferName.AutoSize = true;
            this.lbl_FileTransferName.Location = new System.Drawing.Point(363, 525);
            this.lbl_FileTransferName.Name = "lbl_FileTransferName";
            this.lbl_FileTransferName.Size = new System.Drawing.Size(121, 17);
            this.lbl_FileTransferName.TabIndex = 17;
            this.lbl_FileTransferName.Text = "FileTransferName";
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(23, 239);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1007, 112);
            this.panel1.TabIndex = 18;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1067, 554);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lbl_FileTransferName);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnSendMsg);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.ucIPAddress1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbMsg);
            this.Controls.Add(this.pbxStatus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnStartServer);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmMain";
            this.Text = "Listener";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbxStatus)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnStartServer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ImageList imageListLED;
        private System.Windows.Forms.PictureBox pbxStatus;
        private System.Windows.Forms.ListBox lbMsg;
        private System.Windows.Forms.Label label2;
        private ucIPAddress ucIPAddress1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnSendMsg;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lbl_FileTransferName;
        private System.Windows.Forms.Panel panel1;
    }
}

