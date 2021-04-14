using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ServiceLoaderMedpomData;

namespace ClientServise.Roles
{
    public partial class NewRoles : Form
    {
        IWcfInterface wcf;
        public NewRoles(IWcfInterface wcf)
        {
            InitializeComponent();
            this.wcf = wcf;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() == "" || textBox2.Text.Trim() == "")
            {
                MessageBox.Show("Имя пользователя и пароль обязательные поля!");
                return;
            }
            RoleName =textBox1.Text;
            RoleComment = textBox2.Text;
            DialogResult =  DialogResult.OK;
            Close();
        }


        public string RoleName;
        public  string RoleComment;

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
