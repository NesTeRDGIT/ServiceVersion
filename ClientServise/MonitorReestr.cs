using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ServiceLoaderMedpomData;
using System.Threading;
namespace ClientServise
{
    public partial class MonitorReestr : Form
    {
        IWcfInterface wcf => MainForm.MyWcfConnection;

        List<string> Card => LoginForm.SecureCard;

        public MonitorReestr()
        {
            InitializeComponent();
            SetControlForm(Card);
        }



        void SetControlForm(List<string> card)
        {
            tabControl1.TabPages[0].Enabled = card.Contains("GetNotReestr") ;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var th = new Thread(GetTable) {IsBackground = true};
            th.Start();           

        }


        private void GetTable()
        {
            try
            {

                statusStrip1.Invoke(new Action(() =>
                    {
                        statusLabel.Text = @"Запрос не подавших...";
                        StatusProgress.Value = 0;
                        StatusProgress.Maximum = 1;                       
                        button1.Enabled = false;
                    }));
                var tbl1 = new DataTable();
                if (Card.Contains("GetNotReestr"))
                {
                    tbl1 = wcf.GetNotReestr();
                }
                statusStrip1.Invoke(new Action(() =>
                {
                    dataGridViewNot.DataSource = tbl1;
                    StatusProgress.Value = 1;
                    button1.Enabled = true;
                    statusLabel.Text = @"Завершено";
                }));
            }
            catch (Exception ex)
            {
                statusStrip1.Invoke(new Action(() =>
                {
                    MessageBox.Show(ex.Message);
                    button1.Enabled = true;
                }));
            }
        }


        private void button2_Click(object sender, EventArgs e)
        {
            TBL_TOEXCEL(dataGridViewNot.DataSource as DataTable,"Не подавшие.xls");
        }

        private void TBL_TOEXCEL(DataTable tbl,string FileNameXSL)
        {
            saveFileDialog1.FileName = FileNameXSL;
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    var efm = new ExcelFileManager();
                    var style1 = efm.GetStyle(true, CellAlignment.LEFT, "");
                    var style2 = efm.GetStyle(true, CellAlignment.CENTER, "", true, true);
                    var style3 = efm.GetStyle(false, CellAlignment.CENTER, "", true, true);
                    var indexRow = 3;

                    efm.PrintCell(0, 0, tbl.TableName, style3);
                    efm.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, 0, tbl.Columns.Count - 1));
                    

                    for (int i = 0; i < tbl.Columns.Count; i++)
                    {
                        efm.PrintCell(2, i, tbl.Columns[i].ColumnName, style2);
                    }


                    for (int i = 0; i < tbl.Rows.Count; i++)
                        for (int j = 0; j < tbl.Columns.Count; j++)
                        {
                            double value;
                            if (Double.TryParse(tbl.Rows[i][j].ToString(), out value))
                                efm.PrintCell(indexRow + i, j, value, style1);
                            else
                                efm.PrintCell(indexRow + i, j, tbl.Rows[i][j].ToString(), style1);
                        }
                    efm.AutoSizeColumn(0, tbl.Columns.Count + 1);
                    efm.SaveToFile(saveFileDialog1.FileName);
                    if (MessageBox.Show($@"Выполнено. Показать файл?", "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        SANK_SMO.MTR2015.ShowSelectedInExplorer.FileOrFolder(saveFileDialog1.FileName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


    }
}
