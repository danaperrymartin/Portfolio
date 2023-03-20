namespace FileArchiverUI
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.txtbox_destdrive = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtbox_srcdrive = new System.Windows.Forms.TextBox();
            this.btn_run = new System.Windows.Forms.Button();
            this.btn_browsesrcdrive = new System.Windows.Forms.Button();
            this.btn_browsedestdrive = new System.Windows.Forms.Button();
            this.dateTimePicker_start = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker_end = new System.Windows.Forms.DateTimePicker();
            this.pBar1 = new System.Windows.Forms.ProgressBar();
            this.lbl_currentfile = new System.Windows.Forms.Label();
            this.lbl_progress = new System.Windows.Forms.Label();
            this.comboBox_delay = new System.Windows.Forms.ComboBox();
            this.lbl_delay = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtbox_destdrive
            // 
            this.txtbox_destdrive.Location = new System.Drawing.Point(300, 181);
            this.txtbox_destdrive.Name = "txtbox_destdrive";
            this.txtbox_destdrive.Size = new System.Drawing.Size(197, 22);
            this.txtbox_destdrive.TabIndex = 2;
            this.txtbox_destdrive.Text = "E:\\";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "StartDate";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(42, 162);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 16);
            this.label2.TabIndex = 6;
            this.label2.Text = "EndDate";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(297, 162);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 16);
            this.label3.TabIndex = 7;
            this.label3.Text = "DestinationDrive";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(297, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 16);
            this.label4.TabIndex = 9;
            this.label4.Text = "SourceDrive";
            // 
            // txtbox_srcdrive
            // 
            this.txtbox_srcdrive.Location = new System.Drawing.Point(300, 86);
            this.txtbox_srcdrive.Name = "txtbox_srcdrive";
            this.txtbox_srcdrive.Size = new System.Drawing.Size(197, 22);
            this.txtbox_srcdrive.TabIndex = 8;
            this.txtbox_srcdrive.Text = "L:\\";
            // 
            // btn_run
            // 
            this.btn_run.Location = new System.Drawing.Point(42, 276);
            this.btn_run.Name = "btn_run";
            this.btn_run.Size = new System.Drawing.Size(88, 50);
            this.btn_run.TabIndex = 13;
            this.btn_run.Text = "Run";
            this.btn_run.UseVisualStyleBackColor = true;
            this.btn_run.Click += new System.EventHandler(this.btn_run_Click);
            // 
            // btn_browsesrcdrive
            // 
            this.btn_browsesrcdrive.Location = new System.Drawing.Point(512, 82);
            this.btn_browsesrcdrive.Name = "btn_browsesrcdrive";
            this.btn_browsesrcdrive.Size = new System.Drawing.Size(96, 31);
            this.btn_browsesrcdrive.TabIndex = 14;
            this.btn_browsesrcdrive.Text = "Browse";
            this.btn_browsesrcdrive.UseVisualStyleBackColor = true;
            this.btn_browsesrcdrive.Click += new System.EventHandler(this.btn_browsesrcdrive_Click);
            // 
            // btn_browsedestdrive
            // 
            this.btn_browsedestdrive.Location = new System.Drawing.Point(512, 172);
            this.btn_browsedestdrive.Name = "btn_browsedestdrive";
            this.btn_browsedestdrive.Size = new System.Drawing.Size(96, 31);
            this.btn_browsedestdrive.TabIndex = 15;
            this.btn_browsedestdrive.Text = "Browse";
            this.btn_browsedestdrive.UseVisualStyleBackColor = true;
            this.btn_browsedestdrive.Click += new System.EventHandler(this.btn_browsedestdrive_Click);
            // 
            // dateTimePicker_start
            // 
            this.dateTimePicker_start.Location = new System.Drawing.Point(45, 86);
            this.dateTimePicker_start.Name = "dateTimePicker_start";
            this.dateTimePicker_start.Size = new System.Drawing.Size(189, 22);
            this.dateTimePicker_start.TabIndex = 16;
            // 
            // dateTimePicker_end
            // 
            this.dateTimePicker_end.Location = new System.Drawing.Point(42, 181);
            this.dateTimePicker_end.Name = "dateTimePicker_end";
            this.dateTimePicker_end.Size = new System.Drawing.Size(189, 22);
            this.dateTimePicker_end.TabIndex = 17;
            // 
            // pBar1
            // 
            this.pBar1.Location = new System.Drawing.Point(12, 424);
            this.pBar1.Name = "pBar1";
            this.pBar1.Size = new System.Drawing.Size(507, 14);
            this.pBar1.TabIndex = 18;
            // 
            // lbl_currentfile
            // 
            this.lbl_currentfile.AutoSize = true;
            this.lbl_currentfile.Location = new System.Drawing.Point(26, 395);
            this.lbl_currentfile.Name = "lbl_currentfile";
            this.lbl_currentfile.Size = new System.Drawing.Size(71, 16);
            this.lbl_currentfile.TabIndex = 19;
            this.lbl_currentfile.Text = "CurrentFile";
            // 
            // lbl_progress
            // 
            this.lbl_progress.AutoSize = true;
            this.lbl_progress.Location = new System.Drawing.Point(542, 418);
            this.lbl_progress.Name = "lbl_progress";
            this.lbl_progress.Size = new System.Drawing.Size(74, 16);
            this.lbl_progress.TabIndex = 20;
            this.lbl_progress.Text = "%Progress";
            // 
            // comboBox_delay
            // 
            this.comboBox_delay.FormattingEnabled = true;
            this.comboBox_delay.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5"});
            this.comboBox_delay.Location = new System.Drawing.Point(280, 281);
            this.comboBox_delay.Name = "comboBox_delay";
            this.comboBox_delay.Size = new System.Drawing.Size(122, 24);
            this.comboBox_delay.TabIndex = 21;
            // 
            // lbl_delay
            // 
            this.lbl_delay.AutoSize = true;
            this.lbl_delay.Location = new System.Drawing.Point(282, 261);
            this.lbl_delay.Name = "lbl_delay";
            this.lbl_delay.Size = new System.Drawing.Size(76, 16);
            this.lbl_delay.TabIndex = 22;
            this.lbl_delay.Text = "Delay (sec)";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.Top;
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 0);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(800, 26);
            this.statusStrip1.Stretch = false;
            this.statusStrip1.TabIndex = 23;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.BackColor = System.Drawing.Color.Green;
            this.toolStripStatusLabel1.Enabled = false;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(96, 20);
            this.toolStripStatusLabel1.Text = "GettingFiles...";
            this.toolStripStatusLabel1.Visible = false;
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.BackColor = System.Drawing.Color.Orange;
            this.toolStripStatusLabel2.Enabled = false;
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(88, 20);
            this.toolStripStatusLabel2.Text = "MovingFiles";
            this.toolStripStatusLabel2.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.lbl_delay);
            this.Controls.Add(this.comboBox_delay);
            this.Controls.Add(this.lbl_progress);
            this.Controls.Add(this.lbl_currentfile);
            this.Controls.Add(this.pBar1);
            this.Controls.Add(this.dateTimePicker_end);
            this.Controls.Add(this.dateTimePicker_start);
            this.Controls.Add(this.btn_browsedestdrive);
            this.Controls.Add(this.btn_browsesrcdrive);
            this.Controls.Add(this.btn_run);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtbox_srcdrive);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtbox_destdrive);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "FileArchiver";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtbox_destdrive;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtbox_srcdrive;
        private System.Windows.Forms.Button btn_BeginArchive;
        private System.Windows.Forms.Button btn_run;
        private System.Windows.Forms.Button btn_browsesrcdrive;
        private System.Windows.Forms.Button btn_browsedestdrive;
        private System.Windows.Forms.DateTimePicker dateTimePicker_start;
        private System.Windows.Forms.DateTimePicker dateTimePicker_end;
        private System.Windows.Forms.ProgressBar pBar1;
        private System.Windows.Forms.Label lbl_currentfile;
        private System.Windows.Forms.Label lbl_progress;
        private System.Windows.Forms.ComboBox comboBox_delay;
        private System.Windows.Forms.Label lbl_delay;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
    }
}

