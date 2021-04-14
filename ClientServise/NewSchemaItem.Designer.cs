namespace ClientServise
{
    partial class NewSchemaItem
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
            this.textBoxPath = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkBoxEND = new System.Windows.Forms.CheckBox();
            this.dateTimePickerEND = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerBEG = new System.Windows.Forms.DateTimePicker();
            this.buttonBrouseFile = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxPath
            // 
            this.textBoxPath.Location = new System.Drawing.Point(101, 19);
            this.textBoxPath.Name = "textBoxPath";
            this.textBoxPath.ReadOnly = true;
            this.textBoxPath.Size = new System.Drawing.Size(432, 20);
            this.textBoxPath.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBoxEND);
            this.groupBox1.Controls.Add(this.dateTimePickerEND);
            this.groupBox1.Controls.Add(this.dateTimePickerBEG);
            this.groupBox1.Controls.Add(this.buttonBrouseFile);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBoxPath);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(637, 175);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Схема";
            // 
            // checkBoxEND
            // 
            this.checkBoxEND.AutoSize = true;
            this.checkBoxEND.Location = new System.Drawing.Point(307, 73);
            this.checkBoxEND.Name = "checkBoxEND";
            this.checkBoxEND.Size = new System.Drawing.Size(80, 17);
            this.checkBoxEND.TabIndex = 4;
            this.checkBoxEND.Text = "Бессрочно";
            this.checkBoxEND.UseVisualStyleBackColor = true;
            this.checkBoxEND.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // dateTimePickerEND
            // 
            this.dateTimePickerEND.Location = new System.Drawing.Point(101, 71);
            this.dateTimePickerEND.Name = "dateTimePickerEND";
            this.dateTimePickerEND.Size = new System.Drawing.Size(200, 20);
            this.dateTimePickerEND.TabIndex = 3;
            // 
            // dateTimePickerBEG
            // 
            this.dateTimePickerBEG.Location = new System.Drawing.Point(101, 45);
            this.dateTimePickerBEG.Name = "dateTimePickerBEG";
            this.dateTimePickerBEG.Size = new System.Drawing.Size(200, 20);
            this.dateTimePickerBEG.TabIndex = 3;
            // 
            // buttonBrouseFile
            // 
            this.buttonBrouseFile.Location = new System.Drawing.Point(539, 16);
            this.buttonBrouseFile.Name = "buttonBrouseFile";
            this.buttonBrouseFile.Size = new System.Drawing.Size(75, 23);
            this.buttonBrouseFile.TabIndex = 2;
            this.buttonBrouseFile.Text = "...";
            this.buttonBrouseFile.UseVisualStyleBackColor = true;
            this.buttonBrouseFile.Click += new System.EventHandler(this.buttonBrouseFile_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Дата окончания";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Дата начала";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Путь к схеме";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 193);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(637, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "ОК";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // NewSchemaItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(661, 225);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.Name = "NewSchemaItem";
            this.Text = "Добавить схему";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxPath;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBoxEND;
        private System.Windows.Forms.DateTimePicker dateTimePickerEND;
        private System.Windows.Forms.DateTimePicker dateTimePickerBEG;
        private System.Windows.Forms.Button buttonBrouseFile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
    }
}