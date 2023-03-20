
namespace BoundingBoxCoordFinder
{
    partial class DateTimeSearchForm
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
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_search = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.rdiobtn_CarSearch = new System.Windows.Forms.RadioButton();
            this.rdiobtn_TrainSearch = new System.Windows.Forms.RadioButton();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(30, 86);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(254, 22);
            this.dateTimePicker1.TabIndex = 0;
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.Location = new System.Drawing.Point(360, 84);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(251, 22);
            this.dateTimePicker2.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Start Date";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(362, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "End Date";
            // 
            // btn_search
            // 
            this.btn_search.Location = new System.Drawing.Point(487, 313);
            this.btn_search.Name = "btn_search";
            this.btn_search.Size = new System.Drawing.Size(93, 23);
            this.btn_search.TabIndex = 9;
            this.btn_search.Text = "Search";
            this.btn_search.Click += new System.EventHandler(this.btn_search_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(222, 129);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(198, 17);
            this.label3.TabIndex = 8;
            this.label3.Text = "Enter Car names to search for";
            // 
            // rdiobtn_CarSearch
            // 
            this.rdiobtn_CarSearch.AutoSize = true;
            this.rdiobtn_CarSearch.Location = new System.Drawing.Point(31, 15);
            this.rdiobtn_CarSearch.Name = "rdiobtn_CarSearch";
            this.rdiobtn_CarSearch.Size = new System.Drawing.Size(100, 21);
            this.rdiobtn_CarSearch.TabIndex = 10;
            this.rdiobtn_CarSearch.TabStop = true;
            this.rdiobtn_CarSearch.Text = "Car Search";
            this.rdiobtn_CarSearch.UseVisualStyleBackColor = true;
            // 
            // rdiobtn_TrainSearch
            // 
            this.rdiobtn_TrainSearch.AutoSize = true;
            this.rdiobtn_TrainSearch.Location = new System.Drawing.Point(195, 15);
            this.rdiobtn_TrainSearch.Name = "rdiobtn_TrainSearch";
            this.rdiobtn_TrainSearch.Size = new System.Drawing.Size(111, 21);
            this.rdiobtn_TrainSearch.TabIndex = 11;
            this.rdiobtn_TrainSearch.TabStop = true;
            this.rdiobtn_TrainSearch.Text = "Train Search";
            this.rdiobtn_TrainSearch.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(458, 137);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(121, 166);
            this.textBox1.TabIndex = 12;
            // 
            // DateTimeSearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(657, 357);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.rdiobtn_TrainSearch);
            this.Controls.Add(this.rdiobtn_CarSearch);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btn_search);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dateTimePicker2);
            this.Controls.Add(this.dateTimePicker1);
            this.Name = "DateTimeSearchForm";
            this.Text = "DateTimeSearchForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_search;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton rdiobtn_CarSearch;
        private System.Windows.Forms.RadioButton rdiobtn_TrainSearch;
        private System.Windows.Forms.TextBox textBox1;
    }
}