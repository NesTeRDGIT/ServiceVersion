namespace ClientServise
{
    partial class FilesManagerView
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
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.priory = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.codeMOstrDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CaptionMO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Comment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IST = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.commentSite = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WARNNING = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.составФайловToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.повторПроверкиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.закончитьОжиданияToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.прерватьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.button4 = new System.Windows.Forms.Button();
            this.numericUpDownPriority = new System.Windows.Forms.NumericUpDown();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.labelUpdateData = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPriority)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.priory,
            this.codeMOstrDataGridViewTextBoxColumn,
            this.CaptionMO,
            this.Status,
            this.Date,
            this.Comment,
            this.IST,
            this.commentSite,
            this.WARNNING});
            this.dataGridView1.ContextMenuStrip = this.contextMenuStrip1;
            this.dataGridView1.DataSource = this.bindingSource1;
            this.dataGridView1.Location = new System.Drawing.Point(12, 41);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(1201, 547);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            this.dataGridView1.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentDoubleClick);
            this.dataGridView1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseDoubleClick);
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "ID";
            this.Column1.FillWeight = 30F;
            this.Column1.HeaderText = "ID";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 43;
            // 
            // priory
            // 
            this.priory.DataPropertyName = "Priory";
            this.priory.FillWeight = 20F;
            this.priory.HeaderText = "LVL";
            this.priory.MinimumWidth = 10;
            this.priory.Name = "priory";
            this.priory.ReadOnly = true;
            this.priory.Width = 30;
            // 
            // codeMOstrDataGridViewTextBoxColumn
            // 
            this.codeMOstrDataGridViewTextBoxColumn.DataPropertyName = "codeMOstr";
            this.codeMOstrDataGridViewTextBoxColumn.FillWeight = 49.08962F;
            this.codeMOstrDataGridViewTextBoxColumn.HeaderText = "Код МО";
            this.codeMOstrDataGridViewTextBoxColumn.Name = "codeMOstrDataGridViewTextBoxColumn";
            this.codeMOstrDataGridViewTextBoxColumn.ReadOnly = true;
            this.codeMOstrDataGridViewTextBoxColumn.Width = 70;
            // 
            // CaptionMO
            // 
            this.CaptionMO.DataPropertyName = "CaptionMO";
            this.CaptionMO.HeaderText = "Наименование МО";
            this.CaptionMO.Name = "CaptionMO";
            this.CaptionMO.ReadOnly = true;
            this.CaptionMO.Width = 225;
            // 
            // Status
            // 
            this.Status.DataPropertyName = "Status";
            this.Status.HeaderText = "Статус";
            this.Status.Name = "Status";
            this.Status.ReadOnly = true;
            this.Status.Width = 66;
            // 
            // Date
            // 
            this.Date.DataPropertyName = "Date";
            this.Date.HeaderText = "Дата";
            this.Date.Name = "Date";
            this.Date.ReadOnly = true;
            // 
            // Comment
            // 
            this.Comment.DataPropertyName = "Comment";
            this.Comment.HeaderText = "Комментарий";
            this.Comment.Name = "Comment";
            this.Comment.ReadOnly = true;
            this.Comment.Width = 102;
            // 
            // IST
            // 
            this.IST.DataPropertyName = "IST";
            this.IST.FillWeight = 102.635F;
            this.IST.HeaderText = "Источник";
            this.IST.Name = "IST";
            this.IST.ReadOnly = true;
            this.IST.Width = 80;
            // 
            // commentSite
            // 
            this.commentSite.DataPropertyName = "CommentSite";
            this.commentSite.FillWeight = 102.635F;
            this.commentSite.HeaderText = "Коммент(Сайт)";
            this.commentSite.Name = "commentSite";
            this.commentSite.ReadOnly = true;
            this.commentSite.Width = 231;
            // 
            // WARNNING
            // 
            this.WARNNING.DataPropertyName = "WARNNING";
            this.WARNNING.FillWeight = 102.635F;
            this.WARNNING.HeaderText = "Предупреждение(На сайт)";
            this.WARNNING.Name = "WARNNING";
            this.WARNNING.ReadOnly = true;
            this.WARNNING.Width = 231;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.составФайловToolStripMenuItem,
            this.toolStripMenuItem1,
            this.повторПроверкиToolStripMenuItem,
            this.toolStripMenuItem2,
            this.закончитьОжиданияToolStripMenuItem,
            this.toolStripMenuItem3,
            this.прерватьToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(191, 110);
            // 
            // составФайловToolStripMenuItem
            // 
            this.составФайловToolStripMenuItem.Name = "составФайловToolStripMenuItem";
            this.составФайловToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.составФайловToolStripMenuItem.Text = "Состав файлов";
            this.составФайловToolStripMenuItem.Click += new System.EventHandler(this.составФайловToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(187, 6);
            // 
            // повторПроверкиToolStripMenuItem
            // 
            this.повторПроверкиToolStripMenuItem.Name = "повторПроверкиToolStripMenuItem";
            this.повторПроверкиToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.повторПроверкиToolStripMenuItem.Text = "Повтор проверки";
            this.повторПроверкиToolStripMenuItem.Click += new System.EventHandler(this.повторПроверкиToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(187, 6);
            // 
            // закончитьОжиданияToolStripMenuItem
            // 
            this.закончитьОжиданияToolStripMenuItem.Name = "закончитьОжиданияToolStripMenuItem";
            this.закончитьОжиданияToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.закончитьОжиданияToolStripMenuItem.Text = "Закончить ожидания";
            this.закончитьОжиданияToolStripMenuItem.Click += new System.EventHandler(this.закончитьОжиданияToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(187, 6);
            // 
            // прерватьToolStripMenuItem
            // 
            this.прерватьToolStripMenuItem.Name = "прерватьToolStripMenuItem";
            this.прерватьToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.прерватьToolStripMenuItem.Text = "Прервать";
            this.прерватьToolStripMenuItem.Click += new System.EventHandler(this.прерватьToolStripMenuItem_Click);
            // 
            // bindingSource1
            // 
            this.bindingSource1.DataSource = typeof(ServiceLoaderMedpomData.FilePacket);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(1138, 8);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Обновить";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(1138, 591);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Очистка";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button3.Location = new System.Drawing.Point(12, 594);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 5;
            this.button3.Text = "Excel";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.FileName = "Прием_реестров.xls";
            this.saveFileDialog1.Filter = "Файлы Excel(*.xls)|*.xls";
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button4.Location = new System.Drawing.Point(285, 591);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(155, 23);
            this.button4.TabIndex = 6;
            this.button4.Text = "Установить приоритет";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // numericUpDownPriority
            // 
            this.numericUpDownPriority.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numericUpDownPriority.Location = new System.Drawing.Point(159, 594);
            this.numericUpDownPriority.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownPriority.Name = "numericUpDownPriority";
            this.numericUpDownPriority.Size = new System.Drawing.Size(120, 20);
            this.numericUpDownPriority.TabIndex = 7;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(13, 11);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 8;
            this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(119, 9);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(75, 23);
            this.button5.TabIndex = 9;
            this.button5.Text = "Найти Код МО";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(356, 8);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(159, 23);
            this.button6.TabIndex = 11;
            this.button6.Text = "Найти наименование МО";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(250, 10);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(100, 20);
            this.textBox2.TabIndex = 10;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            this.textBox2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox2_KeyPress);
            // 
            // button7
            // 
            this.button7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button7.Location = new System.Drawing.Point(1003, 591);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(129, 23);
            this.button7.TabIndex = 12;
            this.button7.Text = "Удалить пакет";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button8
            // 
            this.button8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button8.Location = new System.Drawing.Point(866, 591);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(131, 23);
            this.button8.TabIndex = 13;
            this.button8.Text = "Сохранить в архив";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "IST";
            this.dataGridViewTextBoxColumn1.HeaderText = "Источник";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Width = 255;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "IST";
            this.dataGridViewTextBoxColumn2.FillWeight = 102.635F;
            this.dataGridViewTextBoxColumn2.HeaderText = "Источник";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 228;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "IST";
            this.dataGridViewTextBoxColumn3.FillWeight = 102.635F;
            this.dataGridViewTextBoxColumn3.HeaderText = "Источник";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            this.dataGridViewTextBoxColumn3.Width = 228;
            // 
            // labelUpdateData
            // 
            this.labelUpdateData.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.labelUpdateData.AutoSize = true;
            this.labelUpdateData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelUpdateData.Location = new System.Drawing.Point(586, 598);
            this.labelUpdateData.Name = "labelUpdateData";
            this.labelUpdateData.Size = new System.Drawing.Size(97, 13);
            this.labelUpdateData.TabIndex = 14;
            this.labelUpdateData.Text = "Запрос данных";
            // 
            // FilesManagerView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1225, 620);
            this.Controls.Add(this.labelUpdateData);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.numericUpDownPriority);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.dataGridView1);
            this.Name = "FilesManagerView";
            this.Text = "Просмотр процесса обработки";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FilesManagerView_FormClosing);
            this.Load += new System.EventHandler(this.FilesManagerView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPriority)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem составФайловToolStripMenuItem;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.NumericUpDown numericUpDownPriority;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem повторПроверкиToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem закончитьОжиданияToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;

        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem прерватьToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn priory;
        private System.Windows.Forms.DataGridViewTextBoxColumn codeMOstrDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn CaptionMO;
        private System.Windows.Forms.DataGridViewTextBoxColumn Status;
        private System.Windows.Forms.DataGridViewTextBoxColumn Date;
        private System.Windows.Forms.DataGridViewTextBoxColumn Comment;
        private System.Windows.Forms.DataGridViewTextBoxColumn IST;
        private System.Windows.Forms.DataGridViewTextBoxColumn commentSite;
        private System.Windows.Forms.DataGridViewTextBoxColumn WARNNING;
        private System.Windows.Forms.Label labelUpdateData;
    }
}