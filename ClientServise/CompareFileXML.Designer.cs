namespace ClientServise
{
    partial class CompareFileXML
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.fileNameFAKTBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dataSetCompareFileXML = new ClientServise.DataSetCompareFileXML();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.button2 = new System.Windows.Forms.Button();
            this.fileNameUSERBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.fileNameDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resultDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resultKeyDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.fileNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resultDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ResultKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button5 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileNameFAKTBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetCompareFileXML)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileNameUSERBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.fileNameDataGridViewTextBoxColumn,
            this.resultDataGridViewTextBoxColumn,
            this.ResultKey,
            this.dataGridViewTextBoxColumn1});
            this.dataGridView1.DataSource = this.fileNameFAKTBindingSource;
            this.dataGridView1.Location = new System.Drawing.Point(12, 12);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(429, 442);
            this.dataGridView1.TabIndex = 0;
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.AutoGenerateColumns = false;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.fileNameDataGridViewTextBoxColumn1,
            this.resultDataGridViewTextBoxColumn1,
            this.resultKeyDataGridViewTextBoxColumn});
            this.dataGridView2.DataSource = this.fileNameUSERBindingSource;
            this.dataGridView2.Location = new System.Drawing.Point(779, 98);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.ReadOnly = true;
            this.dataGridView2.Size = new System.Drawing.Size(394, 356);
            this.dataGridView2.TabIndex = 1;
            // 
            // fileNameFAKTBindingSource
            // 
            this.fileNameFAKTBindingSource.DataMember = "FileNameFAKT";
            this.fileNameFAKTBindingSource.DataSource = this.dataSetCompareFileXML;
            // 
            // dataSetCompareFileXML
            // 
            this.dataSetCompareFileXML.DataSetName = "DataSetCompareFileXML";
            this.dataSetCompareFileXML.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(470, 384);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(282, 70);
            this.textBox1.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(677, 355);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Внести>>";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Multiselect = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(447, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "...";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // fileNameUSERBindingSource
            // 
            this.fileNameUSERBindingSource.DataMember = "FileNameUSER";
            this.fileNameUSERBindingSource.DataSource = this.dataSetCompareFileXML;
            // 
            // fileNameDataGridViewTextBoxColumn1
            // 
            this.fileNameDataGridViewTextBoxColumn1.DataPropertyName = "FileName";
            this.fileNameDataGridViewTextBoxColumn1.HeaderText = "FileName";
            this.fileNameDataGridViewTextBoxColumn1.Name = "fileNameDataGridViewTextBoxColumn1";
            this.fileNameDataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // resultDataGridViewTextBoxColumn1
            // 
            this.resultDataGridViewTextBoxColumn1.DataPropertyName = "Result";
            this.resultDataGridViewTextBoxColumn1.HeaderText = "Result";
            this.resultDataGridViewTextBoxColumn1.Name = "resultDataGridViewTextBoxColumn1";
            this.resultDataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // resultKeyDataGridViewTextBoxColumn
            // 
            this.resultKeyDataGridViewTextBoxColumn.DataPropertyName = "ResultKey";
            this.resultKeyDataGridViewTextBoxColumn.HeaderText = "ResultKey";
            this.resultKeyDataGridViewTextBoxColumn.Name = "resultKeyDataGridViewTextBoxColumn";
            this.resultKeyDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(521, 109);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(182, 23);
            this.button3.TabIndex = 5;
            this.button3.Text = "Сравнить";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(521, 138);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(182, 23);
            this.button4.TabIndex = 6;
            this.button4.Text = "Удалить";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // fileNameDataGridViewTextBoxColumn
            // 
            this.fileNameDataGridViewTextBoxColumn.DataPropertyName = "FileName";
            this.fileNameDataGridViewTextBoxColumn.HeaderText = "FileName";
            this.fileNameDataGridViewTextBoxColumn.Name = "fileNameDataGridViewTextBoxColumn";
            this.fileNameDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // resultDataGridViewTextBoxColumn
            // 
            this.resultDataGridViewTextBoxColumn.DataPropertyName = "Result";
            this.resultDataGridViewTextBoxColumn.HeaderText = "Result";
            this.resultDataGridViewTextBoxColumn.Name = "resultDataGridViewTextBoxColumn";
            this.resultDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // ResultKey
            // 
            this.ResultKey.DataPropertyName = "ResultKey";
            this.ResultKey.HeaderText = "ResultKey";
            this.ResultKey.Name = "ResultKey";
            this.ResultKey.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "FilePath";
            this.dataGridViewTextBoxColumn1.HeaderText = "FilePath";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(1098, 12);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 56);
            this.button5.TabIndex = 7;
            this.button5.Text = "Очистка списков";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // CompareFileXML
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1185, 466);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.dataGridView2);
            this.Controls.Add(this.dataGridView1);
            this.Name = "CompareFileXML";
            this.Text = "CompareFileXML";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileNameFAKTBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetCompareFileXML)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fileNameUSERBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.BindingSource fileNameFAKTBindingSource;
        private DataSetCompareFileXML dataSetCompareFileXML;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.BindingSource fileNameUSERBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn fileNameDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn resultDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn resultKeyDataGridViewTextBoxColumn;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.DataGridViewTextBoxColumn fileNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn resultDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ResultKey;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.Button button5;
    }
}