namespace ClientServise.Roles
{
    partial class USER_EDIT
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
            this.dataGridViewUser = new System.Windows.Forms.DataGridView();
            this.iDDataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nAMEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pASSDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStripUser = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.добавитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.удалитьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mEDPOMCLIENTUSERSBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dataSetROLES_EDIT = new ClientServise.Roles.DataSetROLES_EDIT();
            this.dataGridViewRoles = new System.Windows.Forms.DataGridView();
            this.rOLENAMEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rOLECOMMENTDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.iDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStripRoles = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.выбратьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mEDPOMCLIENTROLESBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dataGridViewUS_ROLES = new System.Windows.Forms.DataGridView();
            this.ROLE_NAME = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ROLE_COMMENT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.uSERIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rOLEIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mEDPOMCLIENTUSROLBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUser)).BeginInit();
            this.contextMenuStripUser.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mEDPOMCLIENTUSERSBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetROLES_EDIT)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRoles)).BeginInit();
            this.contextMenuStripRoles.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mEDPOMCLIENTROLESBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUS_ROLES)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mEDPOMCLIENTUSROLBindingSource)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewUser
            // 
            this.dataGridViewUser.AllowUserToAddRows = false;
            this.dataGridViewUser.AllowUserToDeleteRows = false;
            this.dataGridViewUser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewUser.AutoGenerateColumns = false;
            this.dataGridViewUser.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewUser.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.iDDataGridViewTextBoxColumn1,
            this.nAMEDataGridViewTextBoxColumn,
            this.pASSDataGridViewTextBoxColumn});
            this.dataGridViewUser.ContextMenuStrip = this.contextMenuStripUser;
            this.dataGridViewUser.DataSource = this.mEDPOMCLIENTUSERSBindingSource;
            this.dataGridViewUser.Location = new System.Drawing.Point(6, 19);
            this.dataGridViewUser.Name = "dataGridViewUser";
            this.dataGridViewUser.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewUser.Size = new System.Drawing.Size(350, 407);
            this.dataGridViewUser.TabIndex = 0;
            this.dataGridViewUser.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewUser_CellContentClick);
            this.dataGridViewUser.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dataGridView1_EditingControlShowing);
            this.dataGridViewUser.SelectionChanged += new System.EventHandler(this.dataGridViewUser_SelectionChanged);
            // 
            // iDDataGridViewTextBoxColumn1
            // 
            this.iDDataGridViewTextBoxColumn1.DataPropertyName = "ID";
            this.iDDataGridViewTextBoxColumn1.HeaderText = "ИД";
            this.iDDataGridViewTextBoxColumn1.Name = "iDDataGridViewTextBoxColumn1";
            // 
            // nAMEDataGridViewTextBoxColumn
            // 
            this.nAMEDataGridViewTextBoxColumn.DataPropertyName = "NAME";
            this.nAMEDataGridViewTextBoxColumn.HeaderText = "Имя";
            this.nAMEDataGridViewTextBoxColumn.Name = "nAMEDataGridViewTextBoxColumn";
            // 
            // pASSDataGridViewTextBoxColumn
            // 
            this.pASSDataGridViewTextBoxColumn.DataPropertyName = "PASS";
            this.pASSDataGridViewTextBoxColumn.HeaderText = "Пароль";
            this.pASSDataGridViewTextBoxColumn.Name = "pASSDataGridViewTextBoxColumn";
            // 
            // contextMenuStripUser
            // 
            this.contextMenuStripUser.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.добавитьToolStripMenuItem,
            this.удалитьToolStripMenuItem});
            this.contextMenuStripUser.Name = "contextMenuStripRole";
            this.contextMenuStripUser.Size = new System.Drawing.Size(153, 70);
            // 
            // добавитьToolStripMenuItem
            // 
            this.добавитьToolStripMenuItem.Name = "добавитьToolStripMenuItem";
            this.добавитьToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.добавитьToolStripMenuItem.Text = "Добавить";
            this.добавитьToolStripMenuItem.Click += new System.EventHandler(this.добавитьToolStripMenuItem_Click);
            // 
            // удалитьToolStripMenuItem
            // 
            this.удалитьToolStripMenuItem.Name = "удалитьToolStripMenuItem";
            this.удалитьToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.удалитьToolStripMenuItem.Text = "Удалить";
            this.удалитьToolStripMenuItem.Click += new System.EventHandler(this.удалитьToolStripMenuItem_Click);
            // 
            // mEDPOMCLIENTUSERSBindingSource
            // 
            this.mEDPOMCLIENTUSERSBindingSource.DataMember = "MEDPOM_CLIENT_USERS";
            this.mEDPOMCLIENTUSERSBindingSource.DataSource = this.dataSetROLES_EDIT;
            // 
            // dataSetROLES_EDIT
            // 
            this.dataSetROLES_EDIT.DataSetName = "DataSetROLES_EDIT";
            this.dataSetROLES_EDIT.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // dataGridViewRoles
            // 
            this.dataGridViewRoles.AllowUserToAddRows = false;
            this.dataGridViewRoles.AllowUserToDeleteRows = false;
            this.dataGridViewRoles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewRoles.AutoGenerateColumns = false;
            this.dataGridViewRoles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewRoles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.rOLENAMEDataGridViewTextBoxColumn,
            this.rOLECOMMENTDataGridViewTextBoxColumn,
            this.iDDataGridViewTextBoxColumn});
            this.dataGridViewRoles.ContextMenuStrip = this.contextMenuStripRoles;
            this.dataGridViewRoles.DataSource = this.mEDPOMCLIENTROLESBindingSource;
            this.dataGridViewRoles.Location = new System.Drawing.Point(6, 19);
            this.dataGridViewRoles.Name = "dataGridViewRoles";
            this.dataGridViewRoles.ReadOnly = true;
            this.dataGridViewRoles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewRoles.Size = new System.Drawing.Size(277, 407);
            this.dataGridViewRoles.TabIndex = 1;
            // 
            // rOLENAMEDataGridViewTextBoxColumn
            // 
            this.rOLENAMEDataGridViewTextBoxColumn.DataPropertyName = "ROLE_NAME";
            this.rOLENAMEDataGridViewTextBoxColumn.HeaderText = "Имя роли";
            this.rOLENAMEDataGridViewTextBoxColumn.Name = "rOLENAMEDataGridViewTextBoxColumn";
            this.rOLENAMEDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // rOLECOMMENTDataGridViewTextBoxColumn
            // 
            this.rOLECOMMENTDataGridViewTextBoxColumn.DataPropertyName = "ROLE_COMMENT";
            this.rOLECOMMENTDataGridViewTextBoxColumn.HeaderText = "Описание";
            this.rOLECOMMENTDataGridViewTextBoxColumn.Name = "rOLECOMMENTDataGridViewTextBoxColumn";
            this.rOLECOMMENTDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // iDDataGridViewTextBoxColumn
            // 
            this.iDDataGridViewTextBoxColumn.DataPropertyName = "ID";
            this.iDDataGridViewTextBoxColumn.HeaderText = "ID";
            this.iDDataGridViewTextBoxColumn.Name = "iDDataGridViewTextBoxColumn";
            this.iDDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // contextMenuStripRoles
            // 
            this.contextMenuStripRoles.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.выбратьToolStripMenuItem});
            this.contextMenuStripRoles.Name = "contextMenuStripRole";
            this.contextMenuStripRoles.Size = new System.Drawing.Size(130, 26);
            // 
            // выбратьToolStripMenuItem
            // 
            this.выбратьToolStripMenuItem.Name = "выбратьToolStripMenuItem";
            this.выбратьToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.выбратьToolStripMenuItem.Text = "Выбрать";
            this.выбратьToolStripMenuItem.Click += new System.EventHandler(this.выбратьToolStripMenuItem_Click);
            // 
            // mEDPOMCLIENTROLESBindingSource
            // 
            this.mEDPOMCLIENTROLESBindingSource.DataMember = "MEDPOM_CLIENT_ROLES";
            this.mEDPOMCLIENTROLESBindingSource.DataSource = this.dataSetROLES_EDIT;
            // 
            // dataGridViewUS_ROLES
            // 
            this.dataGridViewUS_ROLES.AllowUserToAddRows = false;
            this.dataGridViewUS_ROLES.AllowUserToDeleteRows = false;
            this.dataGridViewUS_ROLES.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewUS_ROLES.AutoGenerateColumns = false;
            this.dataGridViewUS_ROLES.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewUS_ROLES.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ROLE_NAME,
            this.ROLE_COMMENT,
            this.uSERIDDataGridViewTextBoxColumn,
            this.rOLEIDDataGridViewTextBoxColumn});
            this.dataGridViewUS_ROLES.DataSource = this.mEDPOMCLIENTUSROLBindingSource;
            this.dataGridViewUS_ROLES.Location = new System.Drawing.Point(6, 19);
            this.dataGridViewUS_ROLES.Name = "dataGridViewUS_ROLES";
            this.dataGridViewUS_ROLES.ReadOnly = true;
            this.dataGridViewUS_ROLES.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewUS_ROLES.Size = new System.Drawing.Size(319, 407);
            this.dataGridViewUS_ROLES.TabIndex = 2;
            // 
            // ROLE_NAME
            // 
            this.ROLE_NAME.DataPropertyName = "ROLE_NAME";
            this.ROLE_NAME.HeaderText = "Роль";
            this.ROLE_NAME.Name = "ROLE_NAME";
            this.ROLE_NAME.ReadOnly = true;
            // 
            // ROLE_COMMENT
            // 
            this.ROLE_COMMENT.DataPropertyName = "ROLE_COMMENT";
            this.ROLE_COMMENT.HeaderText = "Описание";
            this.ROLE_COMMENT.Name = "ROLE_COMMENT";
            this.ROLE_COMMENT.ReadOnly = true;
            // 
            // uSERIDDataGridViewTextBoxColumn
            // 
            this.uSERIDDataGridViewTextBoxColumn.DataPropertyName = "USER_ID";
            this.uSERIDDataGridViewTextBoxColumn.HeaderText = "ID пользователя";
            this.uSERIDDataGridViewTextBoxColumn.Name = "uSERIDDataGridViewTextBoxColumn";
            this.uSERIDDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // rOLEIDDataGridViewTextBoxColumn
            // 
            this.rOLEIDDataGridViewTextBoxColumn.DataPropertyName = "ROLE_ID";
            this.rOLEIDDataGridViewTextBoxColumn.HeaderText = "ID роли";
            this.rOLEIDDataGridViewTextBoxColumn.Name = "rOLEIDDataGridViewTextBoxColumn";
            this.rOLEIDDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // mEDPOMCLIENTUSROLBindingSource
            // 
            this.mEDPOMCLIENTUSROLBindingSource.DataMember = "MEDPOM_CLIENT_US_ROL";
            this.mEDPOMCLIENTUSROLBindingSource.DataSource = this.dataSetROLES_EDIT;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(807, 441);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(206, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Редактор ролей";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.Location = new System.Drawing.Point(12, 441);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(206, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Принять изменения";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click_1);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.dataGridViewUser);
            this.groupBox1.Location = new System.Drawing.Point(12, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(364, 432);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Пользователи";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.dataGridViewUS_ROLES);
            this.groupBox2.Location = new System.Drawing.Point(382, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(334, 432);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Роли пользователя";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.dataGridViewRoles);
            this.groupBox3.Location = new System.Drawing.Point(722, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(291, 432);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Доступные роли";
            // 
            // USER_EDIT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1025, 476);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "USER_EDIT";
            this.Text = "Редактор пользователей";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.USER_EDIT_FormClosing);
            this.Load += new System.EventHandler(this.USER_EDIT_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUser)).EndInit();
            this.contextMenuStripUser.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mEDPOMCLIENTUSERSBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataSetROLES_EDIT)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRoles)).EndInit();
            this.contextMenuStripRoles.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mEDPOMCLIENTROLESBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewUS_ROLES)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mEDPOMCLIENTUSROLBindingSource)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewUser;
        private System.Windows.Forms.DataGridView dataGridViewRoles;
        private Roles.DataSetROLES_EDIT dataSetROLES_EDIT;
        private System.Windows.Forms.BindingSource mEDPOMCLIENTROLESBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDDataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn nAMEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn pASSDataGridViewTextBoxColumn;
        private System.Windows.Forms.BindingSource mEDPOMCLIENTUSERSBindingSource;
        private System.Windows.Forms.DataGridView dataGridViewUS_ROLES;
        private System.Windows.Forms.BindingSource mEDPOMCLIENTUSROLBindingSource;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripUser;
        private System.Windows.Forms.ToolStripMenuItem добавитьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem удалитьToolStripMenuItem;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripRoles;
        private System.Windows.Forms.ToolStripMenuItem выбратьToolStripMenuItem;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ROLE_NAME;
        private System.Windows.Forms.DataGridViewTextBoxColumn ROLE_COMMENT;
        private System.Windows.Forms.DataGridViewTextBoxColumn uSERIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn rOLEIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.DataGridViewTextBoxColumn rOLENAMEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn rOLECOMMENTDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDDataGridViewTextBoxColumn;
    }
}