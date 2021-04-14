namespace ClientServise
{
    partial class EdditProc
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
            this.textBoxNameErr = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBoxTypeParam = new System.Windows.Forms.ComboBox();
            this.textBoxNameParam = new System.Windows.Forms.TextBox();
            this.buttonAddParam = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxComm = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxNAME_PROC = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.обновитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.удалитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textBoxDefaultValue = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.comboBoxTypeValue = new System.Windows.Forms.ComboBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxComment = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.filePacketBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.groupBox1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.filePacketBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxNameErr
            // 
            this.textBoxNameErr.Location = new System.Drawing.Point(153, 19);
            this.textBoxNameErr.Name = "textBoxNameErr";
            this.textBoxNameErr.Size = new System.Drawing.Size(304, 20);
            this.textBoxNameErr.TabIndex = 0;
            this.textBoxNameErr.TextChanged += new System.EventHandler(this.textBoxNameErr_TextChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 585);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(190, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Принять";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Наименование ошибки";
            // 
            // comboBoxTypeParam
            // 
            this.comboBoxTypeParam.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTypeParam.FormattingEnabled = true;
            this.comboBoxTypeParam.Location = new System.Drawing.Point(94, 204);
            this.comboBoxTypeParam.Name = "comboBoxTypeParam";
            this.comboBoxTypeParam.Size = new System.Drawing.Size(166, 21);
            this.comboBoxTypeParam.TabIndex = 3;
            // 
            // textBoxNameParam
            // 
            this.textBoxNameParam.Location = new System.Drawing.Point(94, 178);
            this.textBoxNameParam.Name = "textBoxNameParam";
            this.textBoxNameParam.Size = new System.Drawing.Size(166, 20);
            this.textBoxNameParam.TabIndex = 4;
            // 
            // buttonAddParam
            // 
            this.buttonAddParam.Location = new System.Drawing.Point(14, 307);
            this.buttonAddParam.Name = "buttonAddParam";
            this.buttonAddParam.Size = new System.Drawing.Size(82, 23);
            this.buttonAddParam.TabIndex = 5;
            this.buttonAddParam.Text = "Добавить";
            this.buttonAddParam.UseVisualStyleBackColor = true;
            this.buttonAddParam.Click += new System.EventHandler(this.buttonAddParam_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxComm);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.textBoxNAME_PROC);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBoxNameErr);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(513, 226);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Наименование";
            // 
            // textBoxComm
            // 
            this.textBoxComm.AcceptsReturn = true;
            this.textBoxComm.AcceptsTab = true;
            this.textBoxComm.BackColor = System.Drawing.Color.MintCream;
            this.textBoxComm.Location = new System.Drawing.Point(153, 74);
            this.textBoxComm.Multiline = true;
            this.textBoxComm.Name = "textBoxComm";
            this.textBoxComm.ReadOnly = true;
            this.textBoxComm.Size = new System.Drawing.Size(304, 128);
            this.textBoxComm.TabIndex = 7;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 77);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(90, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "Системное поле";
            // 
            // textBoxNAME_PROC
            // 
            this.textBoxNAME_PROC.Location = new System.Drawing.Point(153, 48);
            this.textBoxNAME_PROC.Name = "textBoxNAME_PROC";
            this.textBoxNAME_PROC.Size = new System.Drawing.Size(304, 20);
            this.textBoxNAME_PROC.TabIndex = 3;
            this.textBoxNAME_PROC.TextChanged += new System.EventHandler(this.textBoxNAME_PROC_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(141, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Наименование процедуры";
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader5,
            this.columnHeader3,
            this.columnHeader4});
            this.listView1.ContextMenuStrip = this.contextMenuStrip1;
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(6, 19);
            this.listView1.Name = "listView1";
            this.listView1.ShowItemToolTips = true;
            this.listView1.Size = new System.Drawing.Size(448, 153);
            this.listView1.TabIndex = 7;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Имя";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Тип";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Тип значения";
            this.columnHeader5.Width = 86;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Значение";
            this.columnHeader3.Width = 140;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Комментарий";
            this.columnHeader4.Width = 219;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.обновитьToolStripMenuItem,
            this.toolStripMenuItem1,
            this.удалитьToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(129, 54);
            // 
            // обновитьToolStripMenuItem
            // 
            this.обновитьToolStripMenuItem.Name = "обновитьToolStripMenuItem";
            this.обновитьToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.обновитьToolStripMenuItem.Text = "Обновить";
            this.обновитьToolStripMenuItem.Click += new System.EventHandler(this.обновитьToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(125, 6);
            // 
            // удалитьToolStripMenuItem
            // 
            this.удалитьToolStripMenuItem.Name = "удалитьToolStripMenuItem";
            this.удалитьToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.удалитьToolStripMenuItem.Text = "Удалить";
            this.удалитьToolStripMenuItem.Click += new System.EventHandler(this.удалитьToolStripMenuItem_Click);
            // 
            // textBoxDefaultValue
            // 
            this.textBoxDefaultValue.Location = new System.Drawing.Point(94, 255);
            this.textBoxDefaultValue.Name = "textBoxDefaultValue";
            this.textBoxDefaultValue.Size = new System.Drawing.Size(166, 20);
            this.textBoxDefaultValue.TabIndex = 8;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.button6);
            this.groupBox2.Controls.Add(this.button5);
            this.groupBox2.Controls.Add(this.button4);
            this.groupBox2.Controls.Add(this.comboBoxTypeValue);
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.textBoxComment);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.listView1);
            this.groupBox2.Controls.Add(this.textBoxDefaultValue);
            this.groupBox2.Controls.Add(this.textBoxNameParam);
            this.groupBox2.Controls.Add(this.comboBoxTypeParam);
            this.groupBox2.Controls.Add(this.buttonAddParam);
            this.groupBox2.Location = new System.Drawing.Point(12, 244);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(513, 335);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Параметры";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(14, 234);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(76, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "Тип значения";
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(413, 234);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(94, 90);
            this.button6.TabIndex = 18;
            this.button6.Text = "Загрузить параметры из БД";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button5
            // 
            this.button5.BackgroundImage = global::ClientServise.Properties.Resources.Стрелка_вниз;
            this.button5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button5.Location = new System.Drawing.Point(460, 55);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(30, 30);
            this.button5.TabIndex = 17;
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button4
            // 
            this.button4.BackgroundImage = global::ClientServise.Properties.Resources.Стрелка_вверх;
            this.button4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button4.Location = new System.Drawing.Point(460, 19);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(30, 30);
            this.button4.TabIndex = 16;
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // comboBoxTypeValue
            // 
            this.comboBoxTypeValue.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTypeValue.FormattingEnabled = true;
            this.comboBoxTypeValue.Location = new System.Drawing.Point(94, 231);
            this.comboBoxTypeValue.Name = "comboBoxTypeValue";
            this.comboBoxTypeValue.Size = new System.Drawing.Size(166, 21);
            this.comboBoxTypeValue.TabIndex = 15;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(178, 307);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(82, 23);
            this.button2.TabIndex = 14;
            this.button2.Text = "Изменить";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 284);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Комментарий";
            // 
            // textBoxComment
            // 
            this.textBoxComment.Location = new System.Drawing.Point(94, 281);
            this.textBoxComment.Name = "textBoxComment";
            this.textBoxComment.Size = new System.Drawing.Size(166, 20);
            this.textBoxComment.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(11, 258);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Значение";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(11, 207);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Тип данных";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 185);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Имя";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(335, 585);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(190, 23);
            this.button3.TabIndex = 10;
            this.button3.Text = "Отмена";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // filePacketBindingSource
            // 
            this.filePacketBindingSource.DataSource = typeof(ServiceLoaderMedpomData.FilePacket);
            // 
            // EdditProc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(548, 620);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button1);
            this.Name = "EdditProc";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ы";
            this.Load += new System.EventHandler(this.EdditProc_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.filePacketBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxNameErr;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBoxTypeParam;
        private System.Windows.Forms.TextBox textBoxNameParam;
        private System.Windows.Forms.Button buttonAddParam;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.TextBox textBoxDefaultValue;
        private System.Windows.Forms.TextBox textBoxNAME_PROC;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxComment;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.BindingSource filePacketBindingSource;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboBoxTypeValue;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem обновитьToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem удалитьToolStripMenuItem;
        private System.Windows.Forms.TextBox textBoxComm;
        private System.Windows.Forms.Label label8;
    }
}