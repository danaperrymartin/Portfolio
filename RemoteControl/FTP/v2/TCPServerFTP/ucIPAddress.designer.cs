namespace TCPServerFTP
{
    partial class ucIPAddress
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.tbxPart1 = new System.Windows.Forms.TextBox();
            this.tbxPart2 = new System.Windows.Forms.TextBox();
            this.tbxPart3 = new System.Windows.Forms.TextBox();
            this.tbxPart4 = new System.Windows.Forms.TextBox();
            this.tbxPart5 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(10, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = ".";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(77, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(10, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = ".";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(122, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(10, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = ".";
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // tbxPart1
            // 
            this.tbxPart1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbxPart1.Location = new System.Drawing.Point(3, 2);
            this.tbxPart1.MaxLength = 3;
            this.tbxPart1.Name = "tbxPart1";
            this.tbxPart1.Size = new System.Drawing.Size(29, 20);
            this.tbxPart1.TabIndex = 11;
            this.tbxPart1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbxPart1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyDown);
            this.tbxPart1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TextBox_MouseClick);
            this.tbxPart1.Enter += new System.EventHandler(this.TextBox_Enter);
            this.tbxPart1.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_Validating);
            // 
            // tbxPart2
            // 
            this.tbxPart2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbxPart2.Location = new System.Drawing.Point(48, 3);
            this.tbxPart2.MaxLength = 3;
            this.tbxPart2.Name = "tbxPart2";
            this.tbxPart2.Size = new System.Drawing.Size(29, 20);
            this.tbxPart2.TabIndex = 12;
            this.tbxPart2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbxPart2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyDown);
            this.tbxPart2.Enter += new System.EventHandler(this.TextBox_Enter);
            this.tbxPart2.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_Validating);
            // 
            // tbxPart3
            // 
            this.tbxPart3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbxPart3.Location = new System.Drawing.Point(93, 3);
            this.tbxPart3.MaxLength = 3;
            this.tbxPart3.Name = "tbxPart3";
            this.tbxPart3.Size = new System.Drawing.Size(29, 20);
            this.tbxPart3.TabIndex = 13;
            this.tbxPart3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbxPart3.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyDown);
            this.tbxPart3.Enter += new System.EventHandler(this.TextBox_Enter);
            this.tbxPart3.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_Validating);
            // 
            // tbxPart4
            // 
            this.tbxPart4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbxPart4.Location = new System.Drawing.Point(138, 3);
            this.tbxPart4.MaxLength = 3;
            this.tbxPart4.Name = "tbxPart4";
            this.tbxPart4.Size = new System.Drawing.Size(29, 20);
            this.tbxPart4.TabIndex = 14;
            this.tbxPart4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbxPart4.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyDown);
            this.tbxPart4.Enter += new System.EventHandler(this.TextBox_Enter);
            this.tbxPart4.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_Validating);
            // 
            // tbxPart5
            // 
            this.tbxPart5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbxPart5.Location = new System.Drawing.Point(186, 3);
            this.tbxPart5.MaxLength = 4;
            this.tbxPart5.Name = "tbxPart5";
            this.tbxPart5.Size = new System.Drawing.Size(34, 20);
            this.tbxPart5.TabIndex = 15;
            this.tbxPart5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbxPart5.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyDown);
            this.tbxPart5.Enter += new System.EventHandler(this.TextBox_Enter);
            this.tbxPart5.Validating += new System.ComponentModel.CancelEventHandler(this.TextBox_Validating);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(170, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(10, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = ":";
            // 
            // ucIPAddress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbxPart5);
            this.Controls.Add(this.tbxPart4);
            this.Controls.Add(this.tbxPart3);
            this.Controls.Add(this.tbxPart2);
            this.Controls.Add(this.tbxPart1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "ucIPAddress";
            this.Size = new System.Drawing.Size(238, 28);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.TextBox tbxPart1;
        private System.Windows.Forms.TextBox tbxPart4;
        private System.Windows.Forms.TextBox tbxPart3;
        private System.Windows.Forms.TextBox tbxPart2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tbxPart5;
    }
}
