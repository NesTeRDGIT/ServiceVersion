using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ClientServise.Roles
{
    public partial class EditUser : Form
    {
        public string name;
        public string Password;
        public EditUser(string name,string Password)
        {
            InitializeComponent();
            this.name = name;
            this.Password = Password;
        }

        private void EditUser_Load(object sender, EventArgs e)
        {
            textBox1.Text = name;
            textBox2.Text = Password;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Password = textBox2.Text;
            name = textBox1.Text;
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
