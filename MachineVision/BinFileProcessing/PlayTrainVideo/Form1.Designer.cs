
namespace PlayTrainVideo
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
            System.Windows.Forms.Button btn_DisplayFrame;
            this.button1 = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btn_PrevFrame = new System.Windows.Forms.Button();
            this.btn_NextFrame = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.btn_ProcessFrame = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            btn_DisplayFrame = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_DisplayFrame
            // 
            btn_DisplayFrame.Location = new System.Drawing.Point(194, 12);
            btn_DisplayFrame.Name = "btn_DisplayFrame";
            btn_DisplayFrame.Size = new System.Drawing.Size(135, 38);
            btn_DisplayFrame.TabIndex = 2;
            btn_DisplayFrame.Text = "DisplayFrame";
            btn_DisplayFrame.UseVisualStyleBackColor = true;
            btn_DisplayFrame.Click += new System.EventHandler(this.btn_DisplayFrame);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(32, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(145, 38);
            this.button1.TabIndex = 1;
            this.button1.Text = "SelectFile";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btn_LoadBinFile);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(32, 101);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(925, 248);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // btn_PrevFrame
            // 
            this.btn_PrevFrame.Location = new System.Drawing.Point(284, 401);
            this.btn_PrevFrame.Name = "btn_PrevFrame";
            this.btn_PrevFrame.Size = new System.Drawing.Size(68, 50);
            this.btn_PrevFrame.TabIndex = 4;
            this.btn_PrevFrame.Text = "previous frame";
            this.btn_PrevFrame.UseVisualStyleBackColor = true;
            this.btn_PrevFrame.Click += new System.EventHandler(this.btn_PreviousFrame);
            // 
            // btn_NextFrame
            // 
            this.btn_NextFrame.Location = new System.Drawing.Point(400, 401);
            this.btn_NextFrame.Name = "btn_NextFrame";
            this.btn_NextFrame.Size = new System.Drawing.Size(54, 50);
            this.btn_NextFrame.TabIndex = 5;
            this.btn_NextFrame.Text = "next frame";
            this.btn_NextFrame.UseVisualStyleBackColor = true;
            this.btn_NextFrame.Click += new System.EventHandler(this.btn_NextFrame_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(364, 25);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(135, 22);
            this.textBox1.TabIndex = 6;
            this.textBox1.Text = "1";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(558, 27);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(104, 20);
            this.checkBox1.TabIndex = 7;
            this.checkBox1.Text = "Mirror Image";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // btn_ProcessFrame
            // 
            this.btn_ProcessFrame.Location = new System.Drawing.Point(34, 57);
            this.btn_ProcessFrame.Name = "btn_ProcessFrame";
            this.btn_ProcessFrame.Size = new System.Drawing.Size(142, 44);
            this.btn_ProcessFrame.TabIndex = 8;
            this.btn_ProcessFrame.Text = "ProcessFrame";
            this.btn_ProcessFrame.UseVisualStyleBackColor = true;
            this.btn_ProcessFrame.Click += new System.EventHandler(this.btn_ProcessFrame_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(363, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 16);
            this.label1.TabIndex = 9;
            this.label1.Text = "ScaleFactor";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1056, 474);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_ProcessFrame);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btn_NextFrame);
            this.Controls.Add(this.btn_PrevFrame);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(btn_DisplayFrame);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "PlayTrainVideo";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btn_PrevFrame;
        private System.Windows.Forms.Button btn_NextFrame;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button btn_ProcessFrame;
        private System.Windows.Forms.Label label1;
    }
}

