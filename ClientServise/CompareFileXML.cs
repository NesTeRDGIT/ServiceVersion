using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ClientServise
{
    public partial class CompareFileXML : Form
    {
        public CompareFileXML()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var Names = textBox1.Text.Split('\r');
            foreach(string str in Names)
            {
                var row = dataSetCompareFileXML.FileNameUSER.NewFileNameUSERRow();
                row.FileName = str.Trim().ToUpper();
                dataSetCompareFileXML.FileNameUSER.AddFileNameUSERRow(row);
            }
            fileNameUSERBindingSource.ResetBindings(false);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                foreach (string str in openFileDialog1.FileNames)
                {
                    var row = dataSetCompareFileXML.FileNameFAKT.NewFileNameFAKTRow();
                    row.FileName =  Path.GetFileNameWithoutExtension(str).ToUpper();
                    row.FilePath = str;
                    dataSetCompareFileXML.FileNameFAKT.AddFileNameFAKTRow(row);
                }
                fileNameFAKTBindingSource.ResetBindings(false);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (DataSetCompareFileXML.FileNameUSERRow row in dataSetCompareFileXML.FileNameUSER)
            {
                row.ResultKey = -1;
                row.Result = "Файл не найден";
            }

            foreach (DataSetCompareFileXML.FileNameFAKTRow row in dataSetCompareFileXML.FileNameFAKT)
            {
                row.ResultKey = -1;
                row.Result = "Файл не содержиться в списке";
            }

            foreach (DataSetCompareFileXML.FileNameFAKTRow row in dataSetCompareFileXML.FileNameFAKT)
            {
                var select = dataSetCompareFileXML.FileNameUSER.Select("Filename = '" + row.FileName + "'");
                if (select.Count() != 0)
                {
                    row.ResultKey = 1;
                    row.Result = "Файл содержиться в списке";
                    foreach (DataRow r in select)
                    {
                        r["ResultKey"] = 1;
                        r["Result"] = "Файл найден";
                    }
                }
                
            }
            fileNameFAKTBindingSource.ResetBindings(false);
            fileNameUSERBindingSource.ResetBindings(false);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            foreach (DataSetCompareFileXML.FileNameFAKTRow row in dataSetCompareFileXML.FileNameFAKT)
            {
                if (row.ResultKey == -1)
                    File.Delete(row.FilePath);

            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            dataSetCompareFileXML.FileNameUSER.Rows.Clear();
            dataSetCompareFileXML.FileNameFAKT.Rows.Clear();
            fileNameFAKTBindingSource.ResetBindings(false);
            fileNameUSERBindingSource.ResetBindings(false);
        }
    }
}
