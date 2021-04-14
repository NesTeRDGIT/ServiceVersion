namespace ClientServise
{
    partial class LoginForm
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
            this.textBoxUserName = new System.Windows.Forms.TextBox();
            this.passwordBoxPass = new System.Windows.Forms.TextBox();
            this.textBoxHOST = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // textBoxUserName
            // 
            this.textBoxUserName.Location = new System.Drawing.Point(87, 38);
            this.textBoxUserName.Name = "textBoxUserName";
            this.textBoxUserName.Size = new System.Drawing.Size(193, 20);
            this.textBoxUserName.TabIndex = 2;
            this.textBoxUserName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxHOST_KeyUp);
            // 
            // passwordBoxPass
            // 
            this.passwordBoxPass.Location = new System.Drawing.Point(87, 64);
            this.passwordBoxPass.Name = "passwordBoxPass";
            this.passwordBoxPass.PasswordChar = '*';
            this.passwordBoxPass.Size = new System.Drawing.Size(193, 20);
            this.passwordBoxPass.TabIndex = 3;
            this.passwordBoxPass.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxHOST_KeyUp);
            // 
            // textBoxHOST
            // 
            this.textBoxHOST.Location = new System.Drawing.Point(87, 12);
            this.textBoxHOST.Name = "textBoxHOST";
            this.textBoxHOST.Size = new System.Drawing.Size(193, 20);
            this.textBoxHOST.TabIndex = 1;
            this.textBoxHOST.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBoxHOST_KeyUp);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(15, 116);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(265, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Подключить";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Адрес";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Логин";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 67);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Пароль";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(107, 90);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(162, 17);
            this.checkBox1.TabIndex = 7;
            this.checkBox1.TabStop = false;
            this.checkBox1.Text = "Запомнить логин и пароль";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 150);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBoxHOST);
            this.Controls.Add(this.passwordBoxPass);
            this.Controls.Add(this.textBoxUserName);
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LoginForm";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxUserName;
        private System.Windows.Forms.TextBox passwordBoxPass;
        private System.Windows.Forms.TextBox textBoxHOST;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}