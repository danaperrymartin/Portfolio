
using System.Windows.Forms;

namespace BoundingBoxCoordFinder
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.txtboxClassName = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.btn3_DisplayMask = new System.Windows.Forms.Button();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.txtBoxHorizontalDist = new System.Windows.Forms.TextBox();
            this.txtBoxVerticalDist = new System.Windows.Forms.TextBox();
            this.btn_LoadImgfromDB = new System.Windows.Forms.Button();
            this.btn_next = new System.Windows.Forms.Button();
            this.btn_DisplayMaskNext = new System.Windows.Forms.Button();
            this.btn_DisplayMaskPrevious = new System.Windows.Forms.Button();
            this.btn_ScaleMaskForOriginalImageSize = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(207, 92);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(715, 358);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(54, 92);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(101, 32);
            this.button1.TabIndex = 1;
            this.button1.Text = "Clear";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.ClearBtn_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(54, 145);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(101, 33);
            this.button2.TabIndex = 2;
            this.button2.Text = "Scale Down";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // txtboxClassName
            // 
            this.txtboxClassName.Location = new System.Drawing.Point(108, 51);
            this.txtboxClassName.Name = "txtboxClassName";
            this.txtboxClassName.Size = new System.Drawing.Size(203, 22);
            this.txtboxClassName.TabIndex = 3;
            this.txtboxClassName.Tag = "";
            this.txtboxClassName.Text = "Enter Class Name";
            this.txtboxClassName.MouseUp += new System.Windows.Forms.MouseEventHandler(this.txtboxClassName_TextChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(337, 51);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(134, 21);
            this.checkBox1.TabIndex = 4;
            this.checkBox1.Text = "SquareSelection";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(525, 51);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(126, 21);
            this.checkBox2.TabIndex = 5;
            this.checkBox2.Text = "LassoSelection";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(54, 20);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(173, 21);
            this.radioButton1.TabIndex = 6;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "GetBoundingBoxCoord";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(264, 20);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(146, 21);
            this.radioButton2.TabIndex = 7;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "CreateImage Mask";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // btn3_DisplayMask
            // 
            this.btn3_DisplayMask.Location = new System.Drawing.Point(54, 206);
            this.btn3_DisplayMask.Name = "btn3_DisplayMask";
            this.btn3_DisplayMask.Size = new System.Drawing.Size(101, 31);
            this.btn3_DisplayMask.TabIndex = 8;
            this.btn3_DisplayMask.Text = "DisplayMask";
            this.btn3_DisplayMask.UseVisualStyleBackColor = true;
            this.btn3_DisplayMask.Click += new System.EventHandler(this.btn3_Click);
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(447, 20);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(144, 21);
            this.radioButton3.TabIndex = 9;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "Calibrate Distance";
            this.radioButton3.UseVisualStyleBackColor = true;
            this.radioButton3.CheckedChanged += new System.EventHandler(this.radioButton3_CheckedChanged);
            // 
            // txtBoxHorizontalDist
            // 
            this.txtBoxHorizontalDist.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.txtBoxHorizontalDist.Location = new System.Drawing.Point(618, 20);
            this.txtBoxHorizontalDist.Name = "txtBoxHorizontalDist";
            this.txtBoxHorizontalDist.Size = new System.Drawing.Size(154, 22);
            this.txtBoxHorizontalDist.TabIndex = 10;
            this.txtBoxHorizontalDist.Text = "Horizontal Distance";
            // 
            // txtBoxVerticalDist
            // 
            this.txtBoxVerticalDist.Location = new System.Drawing.Point(798, 20);
            this.txtBoxVerticalDist.Name = "txtBoxVerticalDist";
            this.txtBoxVerticalDist.Size = new System.Drawing.Size(133, 22);
            this.txtBoxVerticalDist.TabIndex = 11;
            this.txtBoxVerticalDist.Text = "Vertical Distance";
            // 
            // btn_LoadImgfromDB
            // 
            this.btn_LoadImgfromDB.Location = new System.Drawing.Point(54, 329);
            this.btn_LoadImgfromDB.Name = "btn_LoadImgfromDB";
            this.btn_LoadImgfromDB.Size = new System.Drawing.Size(101, 84);
            this.btn_LoadImgfromDB.TabIndex = 12;
            this.btn_LoadImgfromDB.Text = "Load Image from Database";
            this.btn_LoadImgfromDB.UseVisualStyleBackColor = true;
            this.btn_LoadImgfromDB.Click += new System.EventHandler(this.btn_LoadImgfromDB_Click);
            // 
            // btn_next
            // 
            this.btn_next.Location = new System.Drawing.Point(54, 419);
            this.btn_next.Name = "btn_next";
            this.btn_next.Size = new System.Drawing.Size(101, 31);
            this.btn_next.TabIndex = 13;
            this.btn_next.Text = "next";
            this.btn_next.UseVisualStyleBackColor = true;
            this.btn_next.Click += new System.EventHandler(this.btn_next_Click);
            // 
            // btn_DisplayMaskNext
            // 
            this.btn_DisplayMaskNext.Location = new System.Drawing.Point(172, 206);
            this.btn_DisplayMaskNext.Name = "btn_DisplayMaskNext";
            this.btn_DisplayMaskNext.Size = new System.Drawing.Size(29, 31);
            this.btn_DisplayMaskNext.TabIndex = 14;
            this.btn_DisplayMaskNext.Text = ">";
            this.btn_DisplayMaskNext.UseVisualStyleBackColor = true;
            this.btn_DisplayMaskNext.Click += new System.EventHandler(this.btn_DisplayMaskNext_Click);
            // 
            // btn_DisplayMaskPrevious
            // 
            this.btn_DisplayMaskPrevious.Location = new System.Drawing.Point(1, 206);
            this.btn_DisplayMaskPrevious.Name = "btn_DisplayMaskPrevious";
            this.btn_DisplayMaskPrevious.Size = new System.Drawing.Size(29, 31);
            this.btn_DisplayMaskPrevious.TabIndex = 15;
            this.btn_DisplayMaskPrevious.Text = "<";
            this.btn_DisplayMaskPrevious.UseVisualStyleBackColor = true;
            this.btn_DisplayMaskPrevious.Click += new System.EventHandler(this.btn_DisplayMaskPrevious_Click);
            // 
            // btn_ScaleMaskForOriginalImageSize
            // 
            this.btn_ScaleMaskForOriginalImageSize.Location = new System.Drawing.Point(1, 243);
            this.btn_ScaleMaskForOriginalImageSize.Name = "btn_ScaleMaskForOriginalImageSize";
            this.btn_ScaleMaskForOriginalImageSize.Size = new System.Drawing.Size(200, 31);
            this.btn_ScaleMaskForOriginalImageSize.TabIndex = 16;
            this.btn_ScaleMaskForOriginalImageSize.Text = "ScaleMaskForOriginalImage";
            this.btn_ScaleMaskForOriginalImageSize.UseVisualStyleBackColor = true;
            this.btn_ScaleMaskForOriginalImageSize.Click += new System.EventHandler(this.btn_ScaleMaskForOriginalImageSize_Click);
            // 
            // Form1
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.ClientSize = new System.Drawing.Size(1097, 491);
            this.Controls.Add(this.btn_ScaleMaskForOriginalImageSize);
            this.Controls.Add(this.btn_DisplayMaskPrevious);
            this.Controls.Add(this.btn_DisplayMaskNext);
            this.Controls.Add(this.btn_next);
            this.Controls.Add(this.btn_LoadImgfromDB);
            this.Controls.Add(this.txtBoxVerticalDist);
            this.Controls.Add(this.txtBoxHorizontalDist);
            this.Controls.Add(this.radioButton3);
            this.Controls.Add(this.btn3_DisplayMask);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.txtboxClassName);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.Form1_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.Form1_DragEnter);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.txtboxClassName_TextChanged);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private Button button1;
        private Button button2;
        private TextBox txtboxClassName;
        private CheckBox checkBox1;
        private CheckBox checkBox2;
        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private Button btn3_DisplayMask;
        private RadioButton radioButton3;
        private TextBox txtBoxHorizontalDist;
        private TextBox txtBoxVerticalDist;
        private Button btn_LoadImgfromDB;
        private Button btn_next;
        private Button btn_DisplayMaskNext;
        private Button btn_DisplayMaskPrevious;
        private Button btn_ScaleMaskForOriginalImageSize;
    }
}

