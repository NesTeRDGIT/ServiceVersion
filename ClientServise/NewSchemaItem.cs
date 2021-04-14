using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientServise
{
    public partial class NewSchemaItem : Form
    {
        private bool isLocalFind;
        public NewSchemaItem(bool _isLocalFind)
        {
            isLocalFind = _isLocalFind;
            InitializeComponent();
        }
       
        private void buttonBrouseFile_Click(object sender, EventArgs e)
        {
            if (isLocalFind)
            {
                var ofd = new OpenFileDialog();
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    textBoxPath.Text = ofd.FileName;
                }
            }
            else
            {
                var fd = new FolderDialog("", false);
                if (fd.ShowDialog() == DialogResult.OK)
                {
                    textBoxPath.Text = fd.FileNames[0];
                }
            }
            
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            dateTimePickerEND.Enabled = !checkBoxEND.Checked;
        }

        public string PATH => textBoxPath.Text;
        public DateTime DATE_B => dateTimePickerBEG.Value.Date;
        public DateTime? DATE_E => checkBoxEND.Checked ? null : (DateTime?)dateTimePickerEND.Value.Date;

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
