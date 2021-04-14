namespace ClientService.SchemaEditor
{
    partial class XMLschemaEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XMLschemaEditor));
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.копироватьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.вставитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.копироватьОтмеченныеToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.buttonCreateNode = new System.Windows.Forms.Button();
            this.buttonCreateBranch = new System.Windows.Forms.Button();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonChange = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.checkBoxIndexNum = new System.Windows.Forms.CheckBox();
            this.checkBoxIndexGlobalNum = new System.Windows.Forms.CheckBox();
            this.button5 = new System.Windows.Forms.Button();
            this.textBoxEnumItem = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.listBoxDigitEnum = new System.Windows.Forms.ListBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.numericUpDownDigitZap = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownTotalDigit = new System.Windows.Forms.NumericUpDown();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.checkBoxIndexSTR = new System.Windows.Forms.CheckBox();
            this.checkBoxIndexGlobalStr = new System.Windows.Forms.CheckBox();
            this.button6 = new System.Windows.Forms.Button();
            this.textBoxEnumString = new System.Windows.Forms.TextBox();
            this.button7 = new System.Windows.Forms.Button();
            this.listBoxEnumString = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDownLength = new System.Windows.Forms.NumericUpDown();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBoxType = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxFormat = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.saveFileDialog2 = new System.Windows.Forms.SaveFileDialog();
            this.buttonLoad = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.buttonDeleteAll = new System.Windows.Forms.Button();
            this.buttonDelete = new System.Windows.Forms.Button();
            this.buttonDownElement = new System.Windows.Forms.Button();
            this.buttonUpElement = new System.Windows.Forms.Button();
            this.buttonRefresh = new System.Windows.Forms.Button();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.файлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.создатьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.открытьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.ъToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.сохранитьКакToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.компиляцияToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.выходToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxFIND_ITEM = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDigitZap)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTotalDigit)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLength)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.AllowDrop = true;
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.CheckBoxes = true;
            this.treeView1.ContextMenuStrip = this.contextMenuStrip1;
            this.treeView1.HideSelection = false;
            this.treeView1.ImageIndex = 0;
            this.treeView1.ImageList = this.imageList1;
            this.treeView1.Location = new System.Drawing.Point(12, 41);
            this.treeView1.Name = "treeView1";
            this.treeView1.PathSeparator = "|";
            this.treeView1.SelectedImageIndex = 0;
            this.treeView1.Size = new System.Drawing.Size(476, 446);
            this.treeView1.TabIndex = 0;
            this.treeView1.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.treeView1_ItemDrag);
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView1.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView1_DragDrop);
            this.treeView1.DragOver += new System.Windows.Forms.DragEventHandler(this.treeView1_DragOver);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.копироватьToolStripMenuItem,
            this.вставитьToolStripMenuItem,
            this.копироватьОтмеченныеToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(212, 70);
            // 
            // копироватьToolStripMenuItem
            // 
            this.копироватьToolStripMenuItem.Name = "копироватьToolStripMenuItem";
            this.копироватьToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.копироватьToolStripMenuItem.Text = "Копировать";
            this.копироватьToolStripMenuItem.Click += new System.EventHandler(this.копироватьToolStripMenuItem_Click);
            // 
            // вставитьToolStripMenuItem
            // 
            this.вставитьToolStripMenuItem.Name = "вставитьToolStripMenuItem";
            this.вставитьToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.вставитьToolStripMenuItem.Text = "Вставить";
            this.вставитьToolStripMenuItem.Click += new System.EventHandler(this.вставитьToolStripMenuItem_Click);
            // 
            // копироватьОтмеченныеToolStripMenuItem
            // 
            this.копироватьОтмеченныеToolStripMenuItem.Name = "копироватьОтмеченныеToolStripMenuItem";
            this.копироватьОтмеченныеToolStripMenuItem.Size = new System.Drawing.Size(211, 22);
            this.копироватьОтмеченныеToolStripMenuItem.Text = "Копировать отмеченные";
            this.копироватьОтмеченныеToolStripMenuItem.Click += new System.EventHandler(this.копироватьОтмеченныеToolStripMenuItem_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "DataTypeDate.png");
            this.imageList1.Images.SetKeyName(1, "DataTypeDigit.png");
            this.imageList1.Images.SetKeyName(2, "DataTypeText.png");
            this.imageList1.Images.SetKeyName(3, "Узел.png");
            // 
            // buttonCreateNode
            // 
            this.buttonCreateNode.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonCreateNode.Location = new System.Drawing.Point(107, 14);
            this.buttonCreateNode.Name = "buttonCreateNode";
            this.buttonCreateNode.Size = new System.Drawing.Size(75, 23);
            this.buttonCreateNode.TabIndex = 1;
            this.buttonCreateNode.Text = "Узел";
            this.buttonCreateNode.UseVisualStyleBackColor = true;
            this.buttonCreateNode.Click += new System.EventHandler(this.button1_Click);
            // 
            // buttonCreateBranch
            // 
            this.buttonCreateBranch.Enabled = false;
            this.buttonCreateBranch.Location = new System.Drawing.Point(107, 43);
            this.buttonCreateBranch.Name = "buttonCreateBranch";
            this.buttonCreateBranch.Size = new System.Drawing.Size(75, 23);
            this.buttonCreateBranch.TabIndex = 2;
            this.buttonCreateBranch.Text = "Ветвь";
            this.buttonCreateBranch.UseVisualStyleBackColor = true;
            this.buttonCreateBranch.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(114, 19);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(161, 20);
            this.textBoxName.TabIndex = 3;
            this.textBoxName.TextChanged += new System.EventHandler(this.textBoxName_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.comboBoxType);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.textBoxFormat);
            this.groupBox1.Controls.Add(this.textBoxName);
            this.groupBox1.Location = new System.Drawing.Point(543, 137);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(302, 488);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Редактор элемента";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.buttonChange);
            this.groupBox3.Controls.Add(this.tabControl1);
            this.groupBox3.Controls.Add(this.comboBox1);
            this.groupBox3.Location = new System.Drawing.Point(9, 129);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(281, 357);
            this.groupBox3.TabIndex = 17;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Редактор типа";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 22);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(66, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "Тип данных";
            // 
            // buttonChange
            // 
            this.buttonChange.Location = new System.Drawing.Point(44, 317);
            this.buttonChange.Name = "buttonChange";
            this.buttonChange.Size = new System.Drawing.Size(205, 23);
            this.buttonChange.TabIndex = 10;
            this.buttonChange.Text = "Изменить";
            this.buttonChange.UseVisualStyleBackColor = true;
            this.buttonChange.Click += new System.EventHandler(this.buttonChange_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.ItemSize = new System.Drawing.Size(20, 20);
            this.tabControl1.Location = new System.Drawing.Point(6, 46);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(264, 254);
            this.tabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl1.TabIndex = 15;
            this.tabControl1.TabStop = false;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.checkBoxIndexNum);
            this.tabPage1.Controls.Add(this.checkBoxIndexGlobalNum);
            this.tabPage1.Controls.Add(this.button5);
            this.tabPage1.Controls.Add(this.textBoxEnumItem);
            this.tabPage1.Controls.Add(this.button2);
            this.tabPage1.Controls.Add(this.listBoxDigitEnum);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.numericUpDownDigitZap);
            this.tabPage1.Controls.Add(this.numericUpDownTotalDigit);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(256, 226);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // checkBoxIndexNum
            // 
            this.checkBoxIndexNum.AutoSize = true;
            this.checkBoxIndexNum.Location = new System.Drawing.Point(9, 59);
            this.checkBoxIndexNum.Name = "checkBoxIndexNum";
            this.checkBoxIndexNum.Size = new System.Drawing.Size(64, 17);
            this.checkBoxIndexNum.TabIndex = 20;
            this.checkBoxIndexNum.Text = "Индекс";
            this.checkBoxIndexNum.UseVisualStyleBackColor = true;
            // 
            // checkBoxIndexGlobalNum
            // 
            this.checkBoxIndexGlobalNum.AutoSize = true;
            this.checkBoxIndexGlobalNum.Location = new System.Drawing.Point(79, 59);
            this.checkBoxIndexGlobalNum.Name = "checkBoxIndexGlobalNum";
            this.checkBoxIndexGlobalNum.Size = new System.Drawing.Size(88, 17);
            this.checkBoxIndexGlobalNum.TabIndex = 19;
            this.checkBoxIndexGlobalNum.Text = "Глобальный";
            this.checkBoxIndexGlobalNum.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(88, 108);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(22, 23);
            this.button5.TabIndex = 18;
            this.button5.Text = "-";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // textBoxEnumItem
            // 
            this.textBoxEnumItem.Location = new System.Drawing.Point(6, 82);
            this.textBoxEnumItem.Name = "textBoxEnumItem";
            this.textBoxEnumItem.Size = new System.Drawing.Size(76, 20);
            this.textBoxEnumItem.TabIndex = 17;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(88, 80);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(22, 23);
            this.button2.TabIndex = 16;
            this.button2.Text = "+";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // listBoxDigitEnum
            // 
            this.listBoxDigitEnum.FormattingEnabled = true;
            this.listBoxDigitEnum.Location = new System.Drawing.Point(119, 82);
            this.listBoxDigitEnum.Name = "listBoxDigitEnum";
            this.listBoxDigitEnum.Size = new System.Drawing.Size(120, 95);
            this.listBoxDigitEnum.TabIndex = 15;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 38);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(121, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Знаков после запятой";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Всего знаков";
            // 
            // numericUpDownDigitZap
            // 
            this.numericUpDownDigitZap.Location = new System.Drawing.Point(170, 36);
            this.numericUpDownDigitZap.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.numericUpDownDigitZap.Name = "numericUpDownDigitZap";
            this.numericUpDownDigitZap.Size = new System.Drawing.Size(46, 20);
            this.numericUpDownDigitZap.TabIndex = 1;
            this.numericUpDownDigitZap.TabStop = false;
            this.numericUpDownDigitZap.ValueChanged += new System.EventHandler(this.numericUpDownDigitZap_ValueChanged);
            // 
            // numericUpDownTotalDigit
            // 
            this.numericUpDownTotalDigit.Location = new System.Drawing.Point(170, 10);
            this.numericUpDownTotalDigit.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numericUpDownTotalDigit.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownTotalDigit.Name = "numericUpDownTotalDigit";
            this.numericUpDownTotalDigit.Size = new System.Drawing.Size(46, 20);
            this.numericUpDownTotalDigit.TabIndex = 0;
            this.numericUpDownTotalDigit.TabStop = false;
            this.numericUpDownTotalDigit.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownTotalDigit.ValueChanged += new System.EventHandler(this.numericUpDownTotalDigit_ValueChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.checkBoxIndexSTR);
            this.tabPage2.Controls.Add(this.checkBoxIndexGlobalStr);
            this.tabPage2.Controls.Add(this.button6);
            this.tabPage2.Controls.Add(this.textBoxEnumString);
            this.tabPage2.Controls.Add(this.button7);
            this.tabPage2.Controls.Add(this.listBoxEnumString);
            this.tabPage2.Controls.Add(this.label4);
            this.tabPage2.Controls.Add(this.numericUpDownLength);
            this.tabPage2.Location = new System.Drawing.Point(4, 24);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(256, 226);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // checkBoxIndexSTR
            // 
            this.checkBoxIndexSTR.AutoSize = true;
            this.checkBoxIndexSTR.Location = new System.Drawing.Point(9, 43);
            this.checkBoxIndexSTR.Name = "checkBoxIndexSTR";
            this.checkBoxIndexSTR.Size = new System.Drawing.Size(64, 17);
            this.checkBoxIndexSTR.TabIndex = 24;
            this.checkBoxIndexSTR.Text = "Индекс";
            this.checkBoxIndexSTR.UseVisualStyleBackColor = true;
            // 
            // checkBoxIndexGlobalStr
            // 
            this.checkBoxIndexGlobalStr.AutoSize = true;
            this.checkBoxIndexGlobalStr.Location = new System.Drawing.Point(79, 43);
            this.checkBoxIndexGlobalStr.Name = "checkBoxIndexGlobalStr";
            this.checkBoxIndexGlobalStr.Size = new System.Drawing.Size(88, 17);
            this.checkBoxIndexGlobalStr.TabIndex = 23;
            this.checkBoxIndexGlobalStr.Text = "Глобальный";
            this.checkBoxIndexGlobalStr.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(88, 92);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(22, 23);
            this.button6.TabIndex = 22;
            this.button6.Text = "-";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // textBoxEnumString
            // 
            this.textBoxEnumString.Location = new System.Drawing.Point(6, 66);
            this.textBoxEnumString.Name = "textBoxEnumString";
            this.textBoxEnumString.Size = new System.Drawing.Size(76, 20);
            this.textBoxEnumString.TabIndex = 21;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(88, 64);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(22, 23);
            this.button7.TabIndex = 20;
            this.button7.Text = "+";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // listBoxEnumString
            // 
            this.listBoxEnumString.FormattingEnabled = true;
            this.listBoxEnumString.Location = new System.Drawing.Point(119, 66);
            this.listBoxEnumString.Name = "listBoxEnumString";
            this.listBoxEnumString.Size = new System.Drawing.Size(120, 95);
            this.listBoxEnumString.TabIndex = 19;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(169, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Максимальное число символов";
            // 
            // numericUpDownLength
            // 
            this.numericUpDownLength.Location = new System.Drawing.Point(181, 10);
            this.numericUpDownLength.Maximum = new decimal(new int[] {
            3000,
            0,
            0,
            0});
            this.numericUpDownLength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownLength.Name = "numericUpDownLength";
            this.numericUpDownLength.Size = new System.Drawing.Size(44, 20);
            this.numericUpDownLength.TabIndex = 1;
            this.numericUpDownLength.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownLength.ValueChanged += new System.EventHandler(this.numericUpDownLength_ValueChanged);
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 24);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(256, 226);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Location = new System.Drawing.Point(4, 24);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(256, 226);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "tabPage4";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Строка",
            "Число",
            "Дата(ГГГГ-ММ-ДД)",
            "Комплекстный",
            "Время(HH:MM:SS)"});
            this.comboBox1.Location = new System.Drawing.Point(133, 19);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(133, 21);
            this.comboBox1.TabIndex = 16;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // comboBoxType
            // 
            this.comboBoxType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxType.FormattingEnabled = true;
            this.comboBoxType.Items.AddRange(new object[] {
            "Обязательный",
            "Обязательный множественый",
            "Не обязательный множественый",
            "Не обязательный",
            "Условно-обязательный множественый",
            "Условно-обязательный"});
            this.comboBoxType.Location = new System.Drawing.Point(114, 44);
            this.comboBoxType.Name = "comboBoxType";
            this.comboBoxType.Size = new System.Drawing.Size(161, 21);
            this.comboBoxType.TabIndex = 9;
            this.comboBoxType.SelectedIndexChanged += new System.EventHandler(this.comboBoxType_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Формат";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Тип";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Наименование";
            // 
            // textBoxFormat
            // 
            this.textBoxFormat.Location = new System.Drawing.Point(114, 71);
            this.textBoxFormat.Multiline = true;
            this.textBoxFormat.Name = "textBoxFormat";
            this.textBoxFormat.ReadOnly = true;
            this.textBoxFormat.Size = new System.Drawing.Size(161, 52);
            this.textBoxFormat.TabIndex = 5;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.buttonCreateBranch);
            this.groupBox2.Controls.Add(this.buttonCreateNode);
            this.groupBox2.Location = new System.Drawing.Point(543, 41);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(302, 82);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Новый";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "Файлы проекта схемы(*.pxsd)|*.pxsd";
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "Файлы проекта схемы(*.pxsd)|*.pxsd";
            // 
            // saveFileDialog2
            // 
            this.saveFileDialog2.Filter = "Файлы схемы(*.xsd)|*.xsd";
            // 
            // buttonLoad
            // 
            this.buttonLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonLoad.BackgroundImage = global::ClientServise.Properties.Resources.Загрузить;
            this.buttonLoad.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonLoad.Location = new System.Drawing.Point(76, 562);
            this.buttonLoad.Name = "buttonLoad";
            this.buttonLoad.Size = new System.Drawing.Size(56, 56);
            this.buttonLoad.TabIndex = 16;
            this.buttonLoad.UseVisualStyleBackColor = true;
            this.buttonLoad.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonSave.BackgroundImage = global::ClientServise.Properties.Resources.Сохранить;
            this.buttonSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonSave.Location = new System.Drawing.Point(14, 562);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(56, 56);
            this.buttonSave.TabIndex = 15;
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.BackgroundImage = global::ClientServise.Properties.Resources.Runs;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button1.Location = new System.Drawing.Point(447, 554);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(41, 41);
            this.button1.TabIndex = 14;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // buttonDeleteAll
            // 
            this.buttonDeleteAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonDeleteAll.BackgroundImage = global::ClientServise.Properties.Resources.Удаление_всего;
            this.buttonDeleteAll.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonDeleteAll.Location = new System.Drawing.Point(44, 494);
            this.buttonDeleteAll.Name = "buttonDeleteAll";
            this.buttonDeleteAll.Size = new System.Drawing.Size(26, 26);
            this.buttonDeleteAll.TabIndex = 13;
            this.buttonDeleteAll.UseVisualStyleBackColor = true;
            this.buttonDeleteAll.Click += new System.EventHandler(this.buttonDeleteAll_Click);
            // 
            // buttonDelete
            // 
            this.buttonDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonDelete.BackgroundImage = global::ClientServise.Properties.Resources.Удаление;
            this.buttonDelete.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonDelete.Location = new System.Drawing.Point(12, 494);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(26, 26);
            this.buttonDelete.TabIndex = 12;
            this.buttonDelete.UseVisualStyleBackColor = true;
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonDownElement
            // 
            this.buttonDownElement.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonDownElement.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("buttonDownElement.BackgroundImage")));
            this.buttonDownElement.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonDownElement.Location = new System.Drawing.Point(494, 70);
            this.buttonDownElement.Name = "buttonDownElement";
            this.buttonDownElement.Size = new System.Drawing.Size(26, 26);
            this.buttonDownElement.TabIndex = 8;
            this.buttonDownElement.UseVisualStyleBackColor = true;
            this.buttonDownElement.Click += new System.EventHandler(this.buttonDownElement_Click);
            // 
            // buttonUpElement
            // 
            this.buttonUpElement.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonUpElement.BackgroundImage = global::ClientServise.Properties.Resources.Стрелка_вверх;
            this.buttonUpElement.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonUpElement.Location = new System.Drawing.Point(494, 41);
            this.buttonUpElement.Name = "buttonUpElement";
            this.buttonUpElement.Size = new System.Drawing.Size(26, 26);
            this.buttonUpElement.TabIndex = 7;
            this.buttonUpElement.UseVisualStyleBackColor = true;
            this.buttonUpElement.Click += new System.EventHandler(this.buttonUpElement_Click);
            // 
            // buttonRefresh
            // 
            this.buttonRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRefresh.BackgroundImage = global::ClientServise.Properties.Resources.Обновить;
            this.buttonRefresh.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.buttonRefresh.Location = new System.Drawing.Point(462, 494);
            this.buttonRefresh.Name = "buttonRefresh";
            this.buttonRefresh.Size = new System.Drawing.Size(26, 26);
            this.buttonRefresh.TabIndex = 4;
            this.buttonRefresh.UseVisualStyleBackColor = true;
            this.buttonRefresh.Click += new System.EventHandler(this.button3_Click);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.файлToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(857, 24);
            this.menuStrip1.TabIndex = 17;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // файлToolStripMenuItem
            // 
            this.файлToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.создатьToolStripMenuItem,
            this.toolStripMenuItem3,
            this.открытьToolStripMenuItem,
            this.toolStripMenuItem2,
            this.ъToolStripMenuItem,
            this.сохранитьКакToolStripMenuItem,
            this.компиляцияToolStripMenuItem,
            this.toolStripMenuItem1,
            this.выходToolStripMenuItem});
            this.файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            this.файлToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.файлToolStripMenuItem.Text = "Файл";
            // 
            // создатьToolStripMenuItem
            // 
            this.создатьToolStripMenuItem.Name = "создатьToolStripMenuItem";
            this.создатьToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.создатьToolStripMenuItem.Text = "Создать";
            this.создатьToolStripMenuItem.Click += new System.EventHandler(this.создатьToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(159, 6);
            // 
            // открытьToolStripMenuItem
            // 
            this.открытьToolStripMenuItem.Name = "открытьToolStripMenuItem";
            this.открытьToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.открытьToolStripMenuItem.Text = "Открыть";
            this.открытьToolStripMenuItem.Click += new System.EventHandler(this.buttonLoad_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(159, 6);
            // 
            // ъToolStripMenuItem
            // 
            this.ъToolStripMenuItem.Name = "ъToolStripMenuItem";
            this.ъToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.ъToolStripMenuItem.Text = "Сохранить";
            this.ъToolStripMenuItem.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // сохранитьКакToolStripMenuItem
            // 
            this.сохранитьКакToolStripMenuItem.Name = "сохранитьКакToolStripMenuItem";
            this.сохранитьКакToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.сохранитьКакToolStripMenuItem.Text = "Сохранить как...";
            this.сохранитьКакToolStripMenuItem.Click += new System.EventHandler(this.сохранитьКакToolStripMenuItem_Click);
            // 
            // компиляцияToolStripMenuItem
            // 
            this.компиляцияToolStripMenuItem.Name = "компиляцияToolStripMenuItem";
            this.компиляцияToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.компиляцияToolStripMenuItem.Text = "Компиляция";
            this.компиляцияToolStripMenuItem.Click += new System.EventHandler(this.компиляцияToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(159, 6);
            // 
            // выходToolStripMenuItem
            // 
            this.выходToolStripMenuItem.Name = "выходToolStripMenuItem";
            this.выходToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
            this.выходToolStripMenuItem.Text = "Выход";
            this.выходToolStripMenuItem.Click += new System.EventHandler(this.выходToolStripMenuItem_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 25);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(35, 13);
            this.label8.TabIndex = 18;
            this.label8.Text = "label8";
            // 
            // textBoxFIND_ITEM
            // 
            this.textBoxFIND_ITEM.Location = new System.Drawing.Point(76, 500);
            this.textBoxFIND_ITEM.Name = "textBoxFIND_ITEM";
            this.textBoxFIND_ITEM.Size = new System.Drawing.Size(111, 20);
            this.textBoxFIND_ITEM.TabIndex = 19;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(193, 498);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 20;
            this.button3.Text = "Найти";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click_1);
            // 
            // XMLschemaEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(857, 631);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.textBoxFIND_ITEM);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.buttonLoad);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.buttonDeleteAll);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.buttonDownElement);
            this.Controls.Add(this.buttonUpElement);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonRefresh);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "XMLschemaEditor";
            this.Text = "XMLshema";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.XMLshema_FormClosing);
            this.Load += new System.EventHandler(this.XMLshema_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDigitZap)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownTotalDigit)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLength)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Button buttonCreateNode;
        private System.Windows.Forms.Button buttonCreateBranch;
        private System.Windows.Forms.TextBox textBoxName;
        private System.Windows.Forms.Button buttonRefresh;       
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBoxType;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxFormat;
        private System.Windows.Forms.Button buttonChange;
        private System.Windows.Forms.Button buttonDownElement;
        private System.Windows.Forms.Button buttonUpElement;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonDelete;
        private System.Windows.Forms.Button buttonDeleteAll;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numericUpDownLength;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown numericUpDownDigitZap;
        private System.Windows.Forms.NumericUpDown numericUpDownTotalDigit;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonLoad;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog2;
        private System.Windows.Forms.TextBox textBoxEnumItem;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ListBox listBoxDigitEnum;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.TextBox textBoxEnumString;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.ListBox listBoxEnumString;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem файлToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem открытьToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem ъToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem сохранитьКакToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem компиляцияToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem выходToolStripMenuItem;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ToolStripMenuItem создатьToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem копироватьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem вставитьToolStripMenuItem;
        private System.Windows.Forms.CheckBox checkBoxIndexNum;
        private System.Windows.Forms.CheckBox checkBoxIndexGlobalNum;
        private System.Windows.Forms.CheckBox checkBoxIndexSTR;
        private System.Windows.Forms.CheckBox checkBoxIndexGlobalStr;
        private System.Windows.Forms.ToolStripMenuItem копироватьОтмеченныеToolStripMenuItem;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox textBoxFIND_ITEM;
    }
}