namespace ClientServise.Roles
{
    partial class RoleEdit
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
            this.dataGridViewMethod = new System.Windows.Forms.DataGridView();
            this.nAMEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cOMENTDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStripMethod = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.выбратьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.methodBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dataSetROLES_EDIT = new ClientServise.Roles.DataSetROLES_EDIT();
            this.dataGridViewClaims = new System.Windows.Forms.DataGridView();
            this.NAME = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Coment = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rOLEIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CLAIMS_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStripClaims = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mEDPOMCLIENTCLAIMSBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dataGridViewRole = new System.Windows.Forms.DataGridView();
            this.iDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rOLENAMEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rOLECOMMENTDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStripRole = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.добавитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.удалитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mEDPOMCLIENTROLESBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMethod)).BeginInit();
            this.contextMenuStripMethod.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.methodBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetROLES_EDIT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewClaims)).BeginInit();
            this.contextMenuStripClaims.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mEDPOMCLIENTCLAIMSBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRole)).BeginInit();
            this.contextMenuStripRole.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mEDPOMCLIENTROLESBindingSource)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewMethod
            // 
            this.dataGridViewMethod.AllowUserToAddRows = false;
            this.dataGridViewMethod.AllowUserToDeleteRows = false;
            this.dataGridViewMethod.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewMethod.AutoGenerateColumns = false;
            this.dataGridViewMethod.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewMethod.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nAMEDataGridViewTextBoxColumn,
            this.cOMENTDataGridViewTextBoxColumn});
            this.dataGridViewMethod.ContextMenuStrip = this.contextMenuStripMethod;
            this.dataGridViewMethod.DataSource = this.methodBindingSource;
            this.dataGridViewMethod.Location = new System.Drawing.Point(6, 19);
            this.dataGridViewMethod.Name = "dataGridViewMethod";
            this.dataGridViewMethod.ReadOnly = true;
            this.dataGridViewMethod.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewMethod.Size = new System.Drawing.Size(319, 362);
            this.dataGridViewMethod.TabIndex = 0;
            // 
            // nAMEDataGridViewTextBoxColumn
            // 
            this.nAMEDataGridViewTextBoxColumn.DataPropertyName = "NAME";
            this.nAMEDataGridViewTextBoxColumn.HeaderText = "Наименование";
            this.nAMEDataGridViewTextBoxColumn.Name = "nAMEDataGridViewTextBoxColumn";
            this.nAMEDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // cOMENTDataGridViewTextBoxColumn
            // 
            this.cOMENTDataGridViewTextBoxColumn.DataPropertyName = "COMENT";
            this.cOMENTDataGridViewTextBoxColumn.HeaderText = "Описание";
            this.cOMENTDataGridViewTextBoxColumn.Name = "cOMENTDataGridViewTextBoxColumn";
            this.cOMENTDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // contextMenuStripMethod
            // 
            this.contextMenuStripMethod.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.выбратьToolStripMenuItem});
            this.contextMenuStripMethod.Name = "contextMenuStripRole";
            this.contextMenuStripMethod.Size = new System.Drawing.Size(130, 26);
            // 
            // выбратьToolStripMenuItem
            // 
            this.выбратьToolStripMenuItem.Name = "выбратьToolStripMenuItem";
            this.выбратьToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.выбратьToolStripMenuItem.Text = "Выбрать";
            this.выбратьToolStripMenuItem.Click += new System.EventHandler(this.выбратьToolStripMenuItem_Click);
            // 
            // methodBindingSource
            // 
            this.methodBindingSource.DataMember = "Method";
            this.methodBindingSource.DataSource = this.dataSetROLES_EDIT;
            // 
            // dataSetROLES_EDIT
            // 
            this.dataSetROLES_EDIT.DataSetName = "DataSetROLES_EDIT";
            this.dataSetROLES_EDIT.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // dataGridViewClaims
            // 
            this.dataGridViewClaims.AllowUserToAddRows = false;
            this.dataGridViewClaims.AllowUserToDeleteRows = false;
            this.dataGridViewClaims.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewClaims.AutoGenerateColumns = false;
            this.dataGridViewClaims.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewClaims.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NAME,
            this.Coment,
            this.rOLEIDDataGridViewTextBoxColumn,
            this.CLAIMS_ID});
            this.dataGridViewClaims.ContextMenuStrip = this.contextMenuStripClaims;
            this.dataGridViewClaims.DataSource = this.mEDPOMCLIENTCLAIMSBindingSource;
            this.dataGridViewClaims.Location = new System.Drawing.Point(6, 19);
            this.dataGridViewClaims.Name = "dataGridViewClaims";
            this.dataGridViewClaims.ReadOnly = true;
            this.dataGridViewClaims.Size = new System.Drawing.Size(440, 361);
            this.dataGridViewClaims.TabIndex = 1;
            // 
            // NAME
            // 
            this.NAME.DataPropertyName = "NAME";
            this.NAME.HeaderText = "Наименование";
            this.NAME.Name = "NAME";
            this.NAME.ReadOnly = true;
            // 
            // Coment
            // 
            this.Coment.DataPropertyName = "Coment";
            this.Coment.HeaderText = "Описание";
            this.Coment.Name = "Coment";
            this.Coment.ReadOnly = true;
            // 
            // rOLEIDDataGridViewTextBoxColumn
            // 
            this.rOLEIDDataGridViewTextBoxColumn.DataPropertyName = "ROLE_ID";
            this.rOLEIDDataGridViewTextBoxColumn.HeaderText = "Ид роли";
            this.rOLEIDDataGridViewTextBoxColumn.Name = "rOLEIDDataGridViewTextBoxColumn";
            this.rOLEIDDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // CLAIMS_ID
            // 
            this.CLAIMS_ID.DataPropertyName = "CLAIMS_ID";
            this.CLAIMS_ID.HeaderText = "Ид метода";
            this.CLAIMS_ID.Name = "CLAIMS_ID";
            this.CLAIMS_ID.ReadOnly = true;
            // 
            // contextMenuStripClaims
            // 
            this.contextMenuStripClaims.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.contextMenuStripClaims.Name = "contextMenuStripRole";
            this.contextMenuStripClaims.Size = new System.Drawing.Size(130, 26);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(129, 22);
            this.toolStripMenuItem1.Text = "Удалить";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // mEDPOMCLIENTCLAIMSBindingSource
            // 
            this.mEDPOMCLIENTCLAIMSBindingSource.DataMember = "MEDPOM_CLIENT_CLAIMS";
            this.mEDPOMCLIENTCLAIMSBindingSource.DataSource = this.dataSetROLES_EDIT;
            this.mEDPOMCLIENTCLAIMSBindingSource.BindingComplete += new System.Windows.Forms.BindingCompleteEventHandler(this.mEDPOMCLIENTCLAIMSBindingSource_BindingComplete);
            this.mEDPOMCLIENTCLAIMSBindingSource.ListChanged += new System.ComponentModel.ListChangedEventHandler(this.mEDPOMCLIENTCLAIMSBindingSource_ListChanged);
            // 
            // dataGridViewRole
            // 
            this.dataGridViewRole.AllowUserToAddRows = false;
            this.dataGridViewRole.AllowUserToDeleteRows = false;
            this.dataGridViewRole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewRole.AutoGenerateColumns = false;
            this.dataGridViewRole.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewRole.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.iDDataGridViewTextBoxColumn,
            this.rOLENAMEDataGridViewTextBoxColumn,
            this.rOLECOMMENTDataGridViewTextBoxColumn});
            this.dataGridViewRole.ContextMenuStrip = this.contextMenuStripRole;
            this.dataGridViewRole.DataSource = this.mEDPOMCLIENTROLESBindingSource;
            this.dataGridViewRole.Location = new System.Drawing.Point(6, 20);
            this.dataGridViewRole.Name = "dataGridViewRole";
            this.dataGridViewRole.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewRole.Size = new System.Drawing.Size(356, 359);
            this.dataGridViewRole.TabIndex = 2;
            this.dataGridViewRole.SelectionChanged += new System.EventHandler(this.dataGridViewRole_SelectionChanged);
            // 
            // iDDataGridViewTextBoxColumn
            // 
            this.iDDataGridViewTextBoxColumn.DataPropertyName = "ID";
            this.iDDataGridViewTextBoxColumn.HeaderText = "ИД роли";
            this.iDDataGridViewTextBoxColumn.Name = "iDDataGridViewTextBoxColumn";
            this.iDDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // rOLENAMEDataGridViewTextBoxColumn
            // 
            this.rOLENAMEDataGridViewTextBoxColumn.DataPropertyName = "ROLE_NAME";
            this.rOLENAMEDataGridViewTextBoxColumn.HeaderText = "Наименование";
            this.rOLENAMEDataGridViewTextBoxColumn.Name = "rOLENAMEDataGridViewTextBoxColumn";
            // 
            // rOLECOMMENTDataGridViewTextBoxColumn
            // 
            this.rOLECOMMENTDataGridViewTextBoxColumn.DataPropertyName = "ROLE_COMMENT";
            this.rOLECOMMENTDataGridViewTextBoxColumn.HeaderText = "Описание";
            this.rOLECOMMENTDataGridViewTextBoxColumn.Name = "rOLECOMMENTDataGridViewTextBoxColumn";
            // 
            // contextMenuStripRole
            // 
            this.contextMenuStripRole.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.добавитьToolStripMenuItem,
            this.удалитьToolStripMenuItem});
            this.contextMenuStripRole.Name = "contextMenuStripRole";
            this.contextMenuStripRole.Size = new System.Drawing.Size(136, 48);
            // 
            // добавитьToolStripMenuItem
            // 
            this.добавитьToolStripMenuItem.Name = "добавитьToolStripMenuItem";
            this.добавитьToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.добавитьToolStripMenuItem.Text = "Добавить";
            this.добавитьToolStripMenuItem.Click += new System.EventHandler(this.добавитьToolStripMenuItem_Click);
            // 
            // удалитьToolStripMenuItem
            // 
            this.удалитьToolStripMenuItem.Name = "удалитьToolStripMenuItem";
            this.удалитьToolStripMenuItem.Size = new System.Drawing.Size(135, 22);
            this.удалитьToolStripMenuItem.Text = "Удалить";
            this.удалитьToolStripMenuItem.Click += new System.EventHandler(this.удалитьToolStripMenuItem_Click);
            // 
            // mEDPOMCLIENTROLESBindingSource
            // 
            this.mEDPOMCLIENTROLESBindingSource.DataMember = "MEDPOM_CLIENT_ROLES";
            this.mEDPOMCLIENTROLESBindingSource.DataSource = this.dataSetROLES_EDIT;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(941, 404);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(234, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Редактировать методы";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.Location = new System.Drawing.Point(12, 404);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(234, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "Принять изменения";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.dataGridViewRole);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(368, 386);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Роли";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.dataGridViewClaims);
            this.groupBox2.Location = new System.Drawing.Point(386, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(452, 386);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Методы роли";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.dataGridViewMethod);
            this.groupBox3.Location = new System.Drawing.Point(844, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(331, 386);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Доступные методы";
            // 
            // RoleEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1185, 439);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Name = "RoleEdit";
            this.Text = "Редактор ролей";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RoleEdit_FormClosing);
            this.Load += new System.EventHandler(this.RoleEdit_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewMethod)).EndInit();
            this.contextMenuStripMethod.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.methodBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetROLES_EDIT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewClaims)).EndInit();
            this.contextMenuStripClaims.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mEDPOMCLIENTCLAIMSBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRole)).EndInit();
            this.contextMenuStripRole.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mEDPOMCLIENTROLESBindingSource)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewMethod;
        private System.Windows.Forms.BindingSource methodBindingSource;
        private Roles.DataSetROLES_EDIT dataSetROLES_EDIT;
        private System.Windows.Forms.DataGridView dataGridViewClaims;
        private System.Windows.Forms.DataGridView dataGridViewRole;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.BindingSource mEDPOMCLIENTROLESBindingSource;
        private System.Windows.Forms.BindingSource mEDPOMCLIENTCLAIMSBindingSource;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripRole;
        private System.Windows.Forms.ToolStripMenuItem добавитьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem удалитьToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripMethod;
        private System.Windows.Forms.ToolStripMenuItem выбратьToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripClaims;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DataGridViewTextBoxColumn nAMEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn cOMENTDataGridViewTextBoxColumn;

        private System.Windows.Forms.DataGridViewTextBoxColumn iDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn rOLENAMEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn rOLECOMMENTDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn NAME;
        private System.Windows.Forms.DataGridViewTextBoxColumn Coment;
        private System.Windows.Forms.DataGridViewTextBoxColumn rOLEIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn CLAIMS_ID;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;

    }
}