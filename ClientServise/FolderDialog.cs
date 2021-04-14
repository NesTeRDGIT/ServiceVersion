using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ServiceLoaderMedpomData;
using System.IO;
using System.Threading;

namespace ClientServise
{
    public partial class FolderDialog : Form
    {
        IWcfInterface wcf => MainForm.MyWcfConnection;
        string path;
        public string selectpath = "";
        public List<string> FileNames = new List<string>();

        bool OnlyFolder;
        bool OneFolder = false;
        public FolderDialog(string _path,bool OnlyFolder = false, bool OneFolder = false)
        {
            InitializeComponent();
            this.OnlyFolder = OnlyFolder;
            this.OneFolder = OneFolder;


            try
            {
                setDrive();
                SetPath(_path);
                setlist(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        void setDrive()
        {
            var drive = wcf.GetLocalDisk();
            comboBox1.Items.Clear();
            foreach (var str in drive)
            {
                comboBox1.Items.Add(str);
            }
        }

        void SetPath(string _path)
        {
            if (string.IsNullOrEmpty(_path) && comboBox1.Items.Count!=0)
            {
                path = comboBox1.Items[1].ToString();
            }
            else
            {
                path = _path;
            }
        }
        void setlist(string selectPath)
        {
            textBox1.Text = path;
            comboBox1.Text = path;
            listView1.Items.Clear();
        
            var lvi1 = new ListViewItem {Text = Path.GetFileName("..."), Tag = "0", ImageIndex = 1};
            listView1.Items.Add(lvi1);

            var list = wcf.GetFolderLocal(selectPath);
            foreach(var str in list)
            {
                var lvi = new ListViewItem {Text = Path.GetFileName(str), Tag = "0", ImageIndex = 0};
                listView1.Items.Add(lvi);
            }
            
            if (!OnlyFolder)
            {
                list = wcf.GetFilesLocal(selectPath, "*.XML");
                foreach (var str in list)
                {
                    var lvi = new ListViewItem {Text = Path.GetFileName(str)};
                    lvi.SubItems.Add(str);
                    lvi.Tag = "1";
                    lvi.ImageIndex = 2;
                    listView1.Items.Add(lvi);
                }

                list = wcf.GetFilesLocal(selectPath, "*.ZIP");
                foreach (var str in list)
                {
                    var lvi = new ListViewItem {Text = Path.GetFileName(str)};
                    lvi.SubItems.Add(str);
                    lvi.Tag = "1";
                    lvi.ImageIndex = 3;
                    listView1.Items.Add(lvi);
                }

                list = wcf.GetFilesLocal(selectPath, "*.XSD");
                foreach (var str in list)
                {
                    var lvi = new ListViewItem { Text = Path.GetFileName(str) };
                    lvi.SubItems.Add(str);
                    lvi.Tag = "1";
                    lvi.ImageIndex = 4;
                    listView1.Items.Add(lvi);
                }
            }
        }

        
        private void FolderDialog_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (OnlyFolder)
            {
                selectpath = textBox1.Text;
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            else
            {
                foreach (ListViewItem lvi in listView1.SelectedItems)
                {
                    if (lvi.Tag.ToString() == "1")
                        FileNames.Add(Path.Combine(textBox1.Text, lvi.Text));
                 
                }

                if (FileNames.Count == 0)
                {
                    MessageBox.Show($@"Не выбрано не одного файла!");
                }
                else
                {
                    selectpath = FileNames[0];
                    this.DialogResult = System.Windows.Forms.DialogResult.OK;
                    this.Close();
                }
                   
            }
        }
        
        private void listView1_DoubleClick(object sender, EventArgs e)
        {

            if (listView1.SelectedItems.Count != 0 && !OneFolder)
            {
                if (listView1.SelectedItems[0].Tag.ToString() == "1") return;
                if (listView1.SelectedItems[0].Index == 0)
                {
                    if (Path.GetDirectoryName(path) == null) return;
                    path = Path.GetDirectoryName(path);
                    setlist(path);
                }
                else
                {

                    path = Path.Combine(path, listView1.SelectedItems[0].Text);
                    setlist(path);
                }
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (OneFolder) return;
                path = comboBox1.Text;
                setlist(comboBox1.Text);
                textBox1.Text = comboBox1.Text ;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13 && !OneFolder)
                try
                {
                    path = comboBox1.Text;
                    setlist(comboBox1.Text);
                    textBox1.Text = comboBox1.Text;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
        }
    }
}
