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
    public partial class SvodSMO : Form
    {
        IWcfInterface wcf
        {
            get
            {
                return MainForm.MyWcfConnection;
            }
        }
        public SvodSMO()
        {
            InitializeComponent();
            SetControlForm(LoginForm.SecureCard);
        }


        void SetControlForm(List<string> card)
        {

            tabControl1.TabPages[0].Enabled = card.Contains("GetSVOD_SMO_TEMP100") || card.Contains("GetSVOD_SMO_TEMP1");
            tabControl1.TabPages[1].Enabled = (card.Contains("GetSVOD_DISP_TEMP100 ") || card.Contains("GetSVOD_DISP_TEMP1")) ;
            tabControl1.TabPages[2].Enabled = (card.Contains("GetSVOD_VMP_TEMP100") || card.Contains("GetSVOD_VMP_TEMP1"))  ;
            tabControl1.TabPages[3].Enabled = (card.Contains("GetSVOD_SMP_TEMP100") || card.Contains("GetSVOD_SMP_TEMP1")) ;


            




            radioButtonSMP_TEMP1.Enabled = card.Contains("GetSVOD_SMP_TEMP1");
            if (!card.Contains("GetSVOD_SMP_TEMP100"))
                radioButtonSMP_TEMP1.Checked = true;
            radioButtonSMP_TEMP100.Enabled = card.Contains("GetSVOD_SMP_TEMP100");

            radioButtonTEMP1_DISP.Enabled = card.Contains("GetSVOD_DISP_TEMP1");
            if (!card.Contains("GetSVOD_DISP_TEMP100"))
                radioButtonTEMP1_DISP.Checked = true;
            radioButtonTEMP100_DISP.Enabled = card.Contains("GetSVOD_DISP_TEMP100");

            radioButtonVMP_TEMP1.Enabled = card.Contains("GetSVOD_VMP_TEMP1");
            if (!card.Contains("GetSVOD_VMP_TEMP100"))
                radioButtonVMP_TEMP1.Checked = true;
            radioButtonVMP_TEMP100.Enabled = card.Contains("GetSVOD_VMP_TEMP100");


            radioButtonTEMP1_SVOD.Enabled = card.Contains("GetSVOD_SMO_TEMP1");
            if (!card.Contains("GetSVOD_SMO_TEMP100"))
                radioButtonTEMP1_SVOD.Checked = true;
            radioButtonTEMP100_SVOD.Enabled = card.Contains("GetSVOD_SMO_TEMP100");


            if (!card.Contains("GetSVOD_DISP_ITOG"))
            {
                checkBoxDISP_ITOG.Checked = false;
                checkBoxDISP_ITOG.Enabled = false;
            }
            if (!card.Contains("GetSVOD_SMP_ITOG"))
            {
                checkBoxITOG_SMP.Checked = false;
                checkBoxITOG_SMP.Enabled = false;
            }
            if (!card.Contains("GetSVOD_VMP_ITOG"))
            {
                checkBoxITOG_VMP.Checked = false;
                checkBoxITOG_VMP.Enabled = false;
            }

               
        }


        class  SVOD_ROW
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {

            dataSet1.SVOD_SMO.Rows.Clear();
            label1.Visible = true;
            if (radioButtonTEMP100_SVOD.Checked)
            {
                var th = new Thread(getData_svod) {IsBackground = true};
                th.Start(true);
            }
            else
            {
                var th = new Thread(getData_svod) { IsBackground = true };
                th.Start(false);
            }
        }


        /// <summary>
        /// Своднгая
        /// </summary>
        /// <param name="temp100"></param>
        void getData_svod(object temp100)
        {
            try
            {
                if ((bool)temp100)
                {
                    dataGridViewSVOD.Invoke(new Action(() =>
                    {
                        sVODSMOBindingSource.DataSource = null;
                        dataGridViewSVOD.Enabled = false;

                    }));
                    var SVOD_SMO = new DataTable("SVOD_SMO");
                    foreach (DataColumn col in dataSet1.Tables["SVOD_SMO"].Columns)
                    {
                        SVOD_SMO.Columns.Add(col.ColumnName, col.DataType);
                    }

                    var tbl = wcf.GetSVOD_SMO_TEMP100(SVOD_SMO);

                    dataSet1.SVOD_SMO.Merge(tbl);
                    dataGridViewSVOD.Invoke(new Action(() =>
                        {
                            sVODSMOBindingSource.DataSource = dataSet1.SVOD_SMO;
                            dataGridViewSVOD.Enabled = true;
                            label1.Visible = false;
                        }));
                }
                else
                {
                    dataGridViewSVOD.Invoke(new Action(() =>
                    {
                        sVODSMOBindingSource.DataSource = null;
                        dataGridViewSVOD.Enabled = false;

                    }));
                    var SVOD_SMO = new DataTable("SVOD_SMO");

                    foreach (DataColumn col in dataSet1.Tables["SVOD_SMO"].Columns)
                    {
                        SVOD_SMO.Columns.Add(col.ColumnName, col.DataType);
                    }

                    var tbl = wcf.GetSVOD_SMO_TEMP1(SVOD_SMO);

                    dataSet1.SVOD_SMO.Merge(tbl);
                }

                dataGridViewSVOD.Invoke(new Action(() =>
                {
                    sVODSMOBindingSource.DataSource = dataSet1.SVOD_SMO;
                    dataGridViewSVOD.Enabled = true;
                    label1.Visible = false;
                }));
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        /// <summary>
        /// Отчет по диспансаризации
        /// </summary>
        /// <param name="temp100"></param>
        void getData_DISP(object temp100)
        {
            try
            {
                SetProgressBarParam(0, 2);
                if ((bool)temp100)
                {
                    SetStatusText("Запрос информации из TEMP100");
                    dataGridViewSVOD.Invoke(new Action(() =>
                    {
                        mEDPOMSERVISOTCHETDISPBindingSource.DataSource = null;
                        dataGridViewDISP.Enabled = false;

                    }));
                    DataTable DATATBL = new DataTable("MEDPOMSERVIS_DISP");
                    foreach (DataColumn col in dataSet1.Tables["MEDPOMSERVIS_DISP"].Columns)
                    {
                        DATATBL.Columns.Add(col.ColumnName, col.DataType);
                    }

                    var tbl = wcf.GetSVOD_DISP_TEMP100(DATATBL);

                    dataSet1.MEDPOMSERVIS_DISP.Merge(tbl);
                    dataGridViewSVOD.Invoke(new Action(() =>
                    {
                        mEDPOMSERVISOTCHETDISPBindingSource.DataSource = dataSet1.MEDPOMSERVIS_DISP;
                        dataGridViewDISP.Enabled = true;
                    }));
                    SetProgressBarValue(1);
                }
                else
                {
                    SetStatusText("Запрос информации из TEMP1");
                    dataGridViewSVOD.Invoke(new Action(() =>
                    {
                        mEDPOMSERVISOTCHETDISPBindingSource.DataSource = null;
                        dataGridViewDISP.Enabled = false;

                    }));
                    DataTable DATATBL = new DataTable("MEDPOMSERVIS_DISP");

                    foreach (DataColumn col in dataSet1.Tables["MEDPOMSERVIS_DISP"].Columns)
                    {
                        DATATBL.Columns.Add(col.ColumnName, col.DataType);
                    }

                    var tbl = wcf.GetSVOD_DISP_TEMP1(DATATBL);

                    dataSet1.MEDPOMSERVIS_DISP.Merge(tbl);

                    dataGridViewSVOD.Invoke(new Action(() =>
                    {
                        mEDPOMSERVISOTCHETDISPBindingSource.DataSource = dataSet1.MEDPOMSERVIS_DISP;
                        dataGridViewDISP.Enabled = true;

                    }));
                    SetProgressBarValue(1);
                }
                if (checkBoxDISP_ITOG.Checked)
                {

                    SetStatusText("Запрос информации из Основной базы");
                    int Year = 0;
                    dataGridViewSVOD.Invoke(new Action(() =>
                    {
                        dataGridViewDISP_ITOG.Enabled = false;
                        mEDPOMSERVISOTCHETDISPITOGBindingSource.DataSource = null;
                        Year = (int)numericUpDownDisp_YEAR.Value;

                    }));
                    DataTable tbl_itog = new DataTable("MEDPOMSERVIS_DISP_ITOG");
                    foreach (DataColumn col in dataSet1.Tables["MEDPOMSERVIS_DISP_ITOG"].Columns)
                    {
                        tbl_itog.Columns.Add(col.ColumnName, col.DataType);
                    }

                    var maintbl = wcf.GetSVOD_DISP_ITOG(tbl_itog, Year);
                    // var tbl1 = wcf.GetSVOD_SMP _TEMP100(DATATBL);

                    dataSet1.MEDPOMSERVIS_DISP_ITOG.Merge(maintbl);
                    dataGridViewSVOD.Invoke(new Action(() =>
                    {
                        mEDPOMSERVISOTCHETDISPITOGBindingSource.DataSource = dataSet1.MEDPOMSERVIS_DISP_ITOG;
                        dataGridViewDISP_ITOG.Enabled = true;
                        StopTimer();
                    }));
                }
                SetProgressBarValue(2);
                SetStatusText("Завершено");
                UnLokedPage();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                dataGridViewDISP_ITOG.Enabled = true;
                StopTimer();
                UnLokedPage();
            }

        }

        /// <summary>
        /// Отчет по VMP
        /// </summary>
        /// <param name="temp100"></param>
        void getData_VMP(object temp100)
        {
            try
            {
                SetProgressBarParam(0, 2);
                if ((bool)temp100)
                {
                    SetStatusText("Запрос информации из TEMP100");
                    dataGridViewSVOD.Invoke(new Action(() =>
                    {
                        mEPOMSERVISVMPBindingSource.DataSource = null;
                        dataGridViewVMP.Enabled = true;

                    }));
                    DataTable DATATBL = new DataTable("MEPOMSERVIS_VMP");
                    foreach (DataColumn col in dataSet1.Tables["MEPOMSERVIS_VMP"].Columns)
                    {
                        DATATBL.Columns.Add(col.ColumnName, col.DataType);
                    }

                    var tbl = wcf.GetSVOD_VMP_TEMP100(DATATBL);

                    dataSet1.MEPOMSERVIS_VMP.Merge(tbl);
                    dataGridViewSVOD.Invoke(new Action(() =>
                    {
                        mEPOMSERVISVMPBindingSource.DataSource = dataSet1.MEPOMSERVIS_VMP;
                        dataGridViewVMP.Enabled = false;
                    }));
                    SetProgressBarValue(1);
                }
                else
                {
                    SetStatusText("Запрос информации из TEMP1");
                    dataGridViewSVOD.Invoke(new Action(() =>
                    {
                        mEPOMSERVISVMPBindingSource.DataSource = null;
                        dataGridViewVMP.Enabled = true;

                    }));
                    DataTable DATATBL = new DataTable("MEPOMSERVIS_VMP");

                    foreach (DataColumn col in dataSet1.Tables["MEPOMSERVIS_VMP"].Columns)
                    {
                        DATATBL.Columns.Add(col.ColumnName, col.DataType);
                    }

                    var tbl = wcf.GetSVOD_VMP_TEMP1(DATATBL);

                    dataSet1.MEPOMSERVIS_VMP.Merge(tbl);

                    dataGridViewSVOD.Invoke(new Action(() =>
                    {
                        mEPOMSERVISVMPBindingSource.DataSource = dataSet1.MEPOMSERVIS_VMP;
                        dataGridViewVMP.Enabled = false;

                    }));
                    SetProgressBarValue(1);
                }
                if (checkBoxITOG_VMP.Checked)
                {

                    SetStatusText("Запрос информации из Основной базы");
                    int Year = 0;
                    dataGridViewSVOD.Invoke(new Action(() =>
                    {
                        dataGridViewVMP_ITOG.Enabled = false;
                        mEPOMSERVISVMPITOGBindingSource.DataSource = null;
                        Year = (int)numericUpDownVMP_YEAR.Value;

                    }));
                    DataTable tbl_itog = new DataTable("MEPOMSERVIS_VMP_ITOG");
                    foreach (DataColumn col in dataSet1.Tables["MEPOMSERVIS_VMP_ITOG"].Columns)
                    {
                        tbl_itog.Columns.Add(col.ColumnName, col.DataType);
                    }

                    var maintbl = wcf.GetSVOD_VMP_ITOG(tbl_itog, Year);
                    // var tbl1 = wcf.GetSVOD_SMP _TEMP100(DATATBL);

                    dataSet1.MEPOMSERVIS_VMP_ITOG.Merge(maintbl);
                }
                    dataGridViewSVOD.Invoke(new Action(() =>
                    {
                        mEPOMSERVISVMPITOGBindingSource.DataSource = dataSet1.MEPOMSERVIS_VMP_ITOG;
                        dataGridViewVMP_ITOG.Enabled = true;
                        StopTimer();
                    }));
                    SetStatusText("Завершено");
                SetProgressBarValue(2);
                UnLokedPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                dataGridViewVMP_ITOG.Enabled = true;
                StopTimer();
                UnLokedPage();
            }

        }


        /// <summary>
        /// Отчет по SMP
        /// </summary>
        /// <param name="temp100"></param>
        void getData_SMP(object temp100)
        {
            try
            {

                SetProgressBarParam(0, 2);
                if ((bool)temp100)
                {
                    SetStatusText("Запрос информации из TEMP100");
                    dataGridViewSVOD.Invoke(new Action(() =>
                    {
                        dataGridViewSMP.Enabled = false;
                        mEPOMSERVISSMPBindingSource.DataSource = null;

                    }));
                    DataTable DATATBL = new DataTable("MEPOMSERVIS_SMP");
                    foreach (DataColumn col in dataSet1.Tables["MEPOMSERVIS_SMP"].Columns)
                    {
                        DATATBL.Columns.Add(col.ColumnName, col.DataType);
                    }

                    var tbl = wcf.GetSVOD_SMP_TEMP100(DATATBL);
                   // var tbl1 = wcf.GetSVOD_SMP _TEMP100(DATATBL);

                    dataSet1.MEPOMSERVIS_SMP.Merge(tbl);
                    dataGridViewSVOD.Invoke(new Action(() =>
                    {
                        mEPOMSERVISSMPBindingSource.DataSource = dataSet1.MEPOMSERVIS_SMP;
                        dataGridViewSMP.Enabled = true;
                    }));
                    SetProgressBarValue(1);
                }
                else
                {
                    SetStatusText("Запрос информации из TEMP1");
                    dataGridViewSVOD.Invoke(new Action(() =>
                    {
                        mEPOMSERVISSMPBindingSource.DataSource = null;
                        dataGridViewSMP.Enabled = false;

                    }));
                    DataTable DATATBL = new DataTable("MEPOMSERVIS_SMP");

                    foreach (DataColumn col in dataSet1.Tables["MEPOMSERVIS_SMP"].Columns)
                    {
                        DATATBL.Columns.Add(col.ColumnName, col.DataType);
                    }

                    var tbl = wcf.GetSVOD_SMP_TEMP1(DATATBL);

                    dataSet1.MEPOMSERVIS_SMP.Merge(tbl);

                    dataGridViewSVOD.Invoke(new Action(() =>
                    {
                        mEPOMSERVISSMPBindingSource.DataSource = dataSet1.MEPOMSERVIS_SMP;
                        dataGridViewSMP.Enabled = true;
                        

                    }));
                    SetProgressBarValue(1);
                }

                if (checkBoxITOG_SMP.Checked)
                {
                    SetStatusText("Запрос информации из Основной базы");
                    int Year = 0;
                    dataGridViewSVOD.Invoke(new Action(() =>
                    {
                        dataGridViewSMP_ITOG.Enabled = false;
                        mEPOMSERVISSMPITOGBindingSource.DataSource = null;
                        Year = (int)numericUpDownItogSMPYear.Value;

                    }));
                    DataTable tbl_itog = new DataTable("MEPOMSERVIS_SMP_ITOG");
                    foreach (DataColumn col in dataSet1.Tables["MEPOMSERVIS_SMP_ITOG"].Columns)
                    {
                        tbl_itog.Columns.Add(col.ColumnName, col.DataType);
                    }

                    var maintbl = wcf.GetSVOD_SMP_ITOG(tbl_itog, Year);
                    // var tbl1 = wcf.GetSVOD_SMP _TEMP100(DATATBL);

                    dataSet1.MEPOMSERVIS_SMP_ITOG.Merge(maintbl);
                }
                dataGridViewSVOD.Invoke(new Action(() =>
                {
                    mEPOMSERVISSMPITOGBindingSource.DataSource = dataSet1.MEPOMSERVIS_SMP_ITOG;
                    dataGridViewSMP_ITOG.Enabled = true;
                    StopTimer();
                }));
                SetStatusText("Завершено");
                SetProgressBarValue(2);
                UnLokedPage();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                dataGridViewSMP_ITOG.Enabled = true;
                StopTimer();
                UnLokedPage();
            }

        }
        private void button2_Click(object sender, EventArgs e)
        {
            string CurrPath;
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CurrPath = saveFileDialog1.FileName;
            }
            else
            {
                return;
            }
            var SMO = new List<string>();
            foreach (var row in dataSet1.SVOD_SMO)
            {
                if (!SMO.Contains(row.СМО))
                {
                    SMO.Add(row.СМО);
                }
            }


            var efm = new ExcelFileManager();
            var sHeader = efm.GetStyle(true, CellAlignment.CENTER, "", true, false);
            var sString = efm.GetStyle(true, CellAlignment.CENTER, "");
            var sDouble = efm.GetStyle(true, CellAlignment.CENTER, "#,##0.00");
            int indexrow = 1;
            int x = 0;
            foreach (string s in SMO)
            {

                x = 0;
                foreach (DataColumn col in dataSet1.SVOD_SMO.Columns)
                {
                    efm.PrintCell(0, x, col.Caption, sHeader);
                    x++;
                }

                var rows = dataSet1.SVOD_SMO.Select("СМО = '" + s + "'");
                indexrow = 1;
                foreach (var row in rows)
                {
                    x = 0;
                    foreach (DataColumn col in dataSet1.SVOD_SMO.Columns)
                    {
                        if (!radioButton4.Checked && col.ColumnName == "SUMV")
                            continue;
                        if (col.DataType != typeof(System.Decimal))
                        {
                            efm.PrintCell(indexrow, x, row[col].ToString(), sString);
                        }
                        else
                        {
                            efm.PrintCell(indexrow, x, Convert.ToDouble(row[col]), sDouble);
                        }
                        x++;
                    }
                    indexrow++;

                }
                efm.AutoSizeColumn(0, dataSet1.SVOD_SMO.Columns.Count);
                efm.SaveToFile(Path.Combine(Path.GetDirectoryName(CurrPath), Path.GetFileNameWithoutExtension(CurrPath) + "_" + s + ".xls"));

            }


            efm = new ExcelFileManager();
            sHeader = efm.GetStyle(true, CellAlignment.CENTER, "");
            sString = efm.GetStyle(false, CellAlignment.CENTER, "");
            sDouble = efm.GetStyle(false, CellAlignment.CENTER, "");
            x = 0;
            foreach (DataColumn col in dataSet1.SVOD_SMO.Columns)
            {
                efm.PrintCell(0, x, col.ColumnName, sHeader);
                x++;
            }

            var rows1 = dataSet1.SVOD_SMO.Select();
            indexrow = 1;
            foreach (var row in rows1)
            {
                x = 0;
                foreach (DataColumn col in dataSet1.SVOD_SMO.Columns)
                {
                    if (col.DataType != typeof(System.Decimal))
                    {
                        efm.PrintCell(indexrow, x, row[col].ToString(), sString);
                    }
                    else
                    {
                        efm.PrintCell(indexrow, x, Convert.ToDouble(row[col]), sDouble);
                    }
                    x++;
                }
                indexrow++;
            }
            efm.AutoSizeColumn(0, dataSet1.SVOD_SMO.Columns.Count);
            efm.SaveToFile(Path.Combine(Path.GetDirectoryName(CurrPath), Path.GetFileNameWithoutExtension(CurrPath) + "_СВОД.xls"));

            if (MessageBox.Show(@"Завершено. Показать папку?", "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                SANK_SMO.MTR2015.ShowSelectedInExplorer.FileOrFolder(Path.GetDirectoryName(CurrPath));
            }

        }

        void LokedPage()
        {
            tabControl1.Invoke(new Action(() =>
                {
                    tabControl1.Enabled = false;
                }));
        }
        void UnLokedPage()
        {
            tabControl1.Invoke(new Action(() =>
            {
                tabControl1.Enabled = true;
            }));
        }

        void SetProgressBarParam(int value,int max)
        {
            statusStrip1.Invoke(new Action(() =>
                {
                    toolStripProgressBar1.Maximum = max;
                    toolStripProgressBar1.Value = value;
                }
            ));
        }

        void SetProgressBarValue(int value)
        {
            statusStrip1.Invoke(new Action(() =>
            {
                toolStripProgressBar1.Value = value;
            }
            ));
        }

        void SetStatusText(string value)
        {
            statusStrip1.Invoke(new Action(() =>
            {
                toolStripStatusLabel1.Text = value;
            }
            ));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            LokedPage();
            StartTimer();
            dataSet1.MEDPOMSERVIS_DISP.Rows.Clear();
            dataSet1.MEDPOMSERVIS_DISP_ITOG.Rows.Clear();


            if (radioButtonTEMP100_DISP.Checked)
            {
                Thread th = new Thread(new ParameterizedThreadStart(getData_DISP));
                th.Start(true);
            }
            else
            {
                Thread th = new Thread(new ParameterizedThreadStart(getData_DISP));
                th.Start(false);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LokedPage(); StartTimer();
            dataSet1.MEPOMSERVIS_VMP.Rows.Clear();
            dataSet1.MEPOMSERVIS_VMP_ITOG.Rows.Clear();
            if (radioButtonVMP_TEMP100.Checked)
            {
                Thread th = new Thread(new ParameterizedThreadStart(getData_VMP));
                th.Start(true);
            }
            else
            {
                Thread th = new Thread(new ParameterizedThreadStart(getData_VMP));
                th.Start(false);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            LokedPage(); StartTimer();
            dataSet1.MEPOMSERVIS_SMP.Rows.Clear();
            dataSet1.MEPOMSERVIS_SMP_ITOG.Rows.Clear();
            if (radioButtonSMP_TEMP100.Checked)
            {
                Thread th = new Thread(new ParameterizedThreadStart(getData_SMP));
                th.Start(true);
            }
            else
            {
                Thread th = new Thread(new ParameterizedThreadStart(getData_SMP));
                th.Start(false);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string CurrPath;
            saveFileDialog1.FileName = "Отчет по диспансеризации от " + DateTime.Now.ToShortDateString() + ".xls";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CurrPath = saveFileDialog1.FileName;
            }
            else
            {
                return;
            }


            ExcelFileManager efm = new ExcelFileManager();
            var sHeader = efm.GetStyle(true, CellAlignment.CENTER, "", true, false);
            var sString = efm.GetStyle(true, CellAlignment.CENTER, "");
            var sDouble = efm.GetStyle(true, CellAlignment.CENTER, "#,##0.00");
            var sInt = efm.GetStyle(true, CellAlignment.CENTER, "0");
            int indexrow = 1;
            int x = 0;

            foreach (DataColumn col in dataSet1.MEDPOMSERVIS_DISP.Columns)
            {
                efm.PrintCell(0, x, col.Caption, sHeader);
                x++;
            }

            foreach (DataRow row in dataSet1.MEDPOMSERVIS_DISP.Rows)
            {
                x = 0;
                foreach (DataColumn col in dataSet1.MEDPOMSERVIS_DISP.Columns)
                {
                    if (col.DataType != typeof(System.Decimal))
                    {
                        efm.PrintCell(indexrow, x, row[col].ToString(), sString);
                    }
                    else
                    {
                        efm.PrintCell(indexrow, x, Convert.ToDouble(row[col]), sDouble);
                    }
                    x++;
                }
                indexrow++;

            }
            efm.AutoSizeColumn(0, dataSet1.MEDPOMSERVIS_DISP.Columns.Count);



            indexrow = 0;
            x = 0;

            efm.SetActivSheet(efm.AddScheet("Итогом за предыдущий период"));

            efm.PrintCell(indexrow, x, "За " + numericUpDownDisp_YEAR.Value.ToString() + " год", sHeader);
            indexrow++;
            foreach (DataColumn col in dataSet1.MEDPOMSERVIS_DISP_ITOG.Columns)
            {
                efm.PrintCell(indexrow, x, col.Caption, sHeader);
                x++;
            }

            indexrow++;
            foreach (DataRow row in dataSet1.MEDPOMSERVIS_DISP_ITOG.Rows)
            {
                x = 0;
                foreach (DataColumn col in dataSet1.MEDPOMSERVIS_DISP_ITOG.Columns)
                {
                    if (col.DataType != typeof(System.Decimal))
                    {
                        if (col.DataType == typeof(System.Int32))
                        {
                            efm.PrintCell(indexrow, x, Convert.ToDouble(row[col]), sInt);
                        }
                        else
                            efm.PrintCell(indexrow, x, row[col].ToString(), sString);
                    }
                    else
                    {
                        efm.PrintCell(indexrow, x, Convert.ToDouble(row[col]), sDouble);
                    }
                    x++;
                }
                indexrow++;

            }
            efm.AutoSizeColumn(0, dataSet1.MEDPOMSERVIS_DISP_ITOG.Columns.Count);


            efm.SaveToFile(Path.Combine(Path.GetDirectoryName(CurrPath), Path.GetFileNameWithoutExtension(CurrPath) + ".xls"));
            if (MessageBox.Show("Показать файл?", "Формирование файла завершено", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                SANK_SMO.MTR2015.ShowSelectedInExplorer.FileOrFolder(Path.Combine(Path.GetDirectoryName(CurrPath), Path.GetFileNameWithoutExtension(CurrPath) + ".xls"));
            }
        }


        private void button7_Click(object sender, EventArgs e)
        {
            string CurrPath;
            saveFileDialog1.FileName = "Отчет по ВМП от " + DateTime.Now.ToShortDateString() + ".xls";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CurrPath = saveFileDialog1.FileName;
            }
            else
            {
                return;
            }


            ExcelFileManager efm = new ExcelFileManager();
            var sHeader = efm.GetStyle(true, CellAlignment.CENTER, "", true, false);
            var sString = efm.GetStyle(true, CellAlignment.CENTER, "");
            var sDouble = efm.GetStyle(true, CellAlignment.CENTER, "#,##0.00");
            var sInt = efm.GetStyle(true, CellAlignment.CENTER, "0");
            int indexrow = 1;
            int x = 0;

            foreach (DataColumn col in dataSet1.MEPOMSERVIS_VMP.Columns)
            {
                efm.PrintCell(0, x, col.Caption, sHeader);
                x++;
            }

            foreach (DataRow row in dataSet1.MEPOMSERVIS_VMP.Rows)
            {
                x = 0;
                foreach (DataColumn col in dataSet1.MEPOMSERVIS_VMP.Columns)
                {

                    if (col.DataType != typeof(System.Decimal))
                    {
                        if (col.DataType == typeof(System.Int32))
                        {
                            efm.PrintCell(indexrow, x, Convert.ToDouble(row[col]), sInt);
                        }
                        else
                            efm.PrintCell(indexrow, x, row[col].ToString(), sString);
                    }
                    else
                    {
                        efm.PrintCell(indexrow, x, Convert.ToDouble(row[col]), sDouble);
                    }
                    x++;
                }
                indexrow++;

            }
            efm.AutoSizeColumn(0, dataSet1.MEPOMSERVIS_VMP.Columns.Count);


            indexrow = 0;
            x = 0;

            efm.SetActivSheet(efm.AddScheet("Итогом за предыдущий период"));

            efm.PrintCell(indexrow, x, "За " + numericUpDownVMP_YEAR.Value.ToString() + " год", sHeader);
            indexrow++;
            foreach (DataColumn col in dataSet1.MEPOMSERVIS_VMP_ITOG.Columns)
            {
                efm.PrintCell(indexrow, x, col.Caption, sHeader);
                x++;
            }

            indexrow++;
            foreach (DataRow row in dataSet1.MEPOMSERVIS_VMP_ITOG.Rows)
            {
                x = 0;
                foreach (DataColumn col in dataSet1.MEPOMSERVIS_VMP_ITOG.Columns)
                {
                    if (col.DataType != typeof(System.Decimal))
                    {
                        if (col.DataType == typeof(System.Int32))
                        {
                            efm.PrintCell(indexrow, x, Convert.ToDouble(row[col]), sInt);
                        }
                        else
                            efm.PrintCell(indexrow, x, row[col].ToString(), sString);
                    }
                    else
                    {
                        efm.PrintCell(indexrow, x, Convert.ToDouble(row[col]), sDouble);
                    }
                    x++;
                }
                indexrow++;

            }
            efm.AutoSizeColumn(0, dataSet1.MEPOMSERVIS_VMP_ITOG.Columns.Count);



            efm.SaveToFile(Path.Combine(Path.GetDirectoryName(CurrPath), Path.GetFileNameWithoutExtension(CurrPath) + ".xls"));
            if (MessageBox.Show("Показать файл?", "Формирование файла завершено", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                SANK_SMO.MTR2015.ShowSelectedInExplorer.FileOrFolder(Path.Combine(Path.GetDirectoryName(CurrPath), Path.GetFileNameWithoutExtension(CurrPath) + ".xls"));
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string CurrPath;
            saveFileDialog1.FileName = "Отчет по СМП от " + DateTime.Now.ToShortDateString() + ".xls";
           
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                CurrPath = saveFileDialog1.FileName;
            }
            else
            {
                return;
            }


            ExcelFileManager efm = new ExcelFileManager();
            var sHeader = efm.GetStyle(true, CellAlignment.CENTER, "", true, false);
            var sString = efm.GetStyle(true, CellAlignment.CENTER, "");
            var sDouble = efm.GetStyle(true, CellAlignment.CENTER, "#,##0.00");
            var sInt = efm.GetStyle(true, CellAlignment.CENTER, "0");
            int indexrow = 1;
            int x = 0;
            efm.RenameActivScheet("Текущий период");
            foreach (DataColumn col in dataSet1.MEPOMSERVIS_SMP.Columns)
            {
                efm.PrintCell(0, x, col.Caption, sHeader);
                x++;
            }

            foreach (DataRow row in dataSet1.MEPOMSERVIS_SMP.Rows)
            {
                x = 0;
                foreach (DataColumn col in dataSet1.MEPOMSERVIS_SMP.Columns)
                {
                    if (col.DataType != typeof(System.Decimal))
                    {
                        if (col.DataType == typeof(System.Int32))
                        {
                            efm.PrintCell(indexrow, x, Convert.ToDouble(row[col]), sInt);
                        }
                        else
                            efm.PrintCell(indexrow, x, row[col].ToString(), sString);
                    }
                    else
                    {
                        efm.PrintCell(indexrow, x, Convert.ToDouble(row[col]), sDouble);
                    }
                    x++;
                }
                indexrow++;

            }
            efm.AutoSizeColumn(0, dataSet1.MEPOMSERVIS_SMP.Columns.Count);
            
            indexrow = 0;
            x = 0;

            efm.SetActivSheet(efm.AddScheet("Итогом за предыдущий период" ));
           
            efm.PrintCell(indexrow, x, "За "+numericUpDownItogSMPYear.Value.ToString() +" год", sHeader);
            indexrow++;
            foreach (DataColumn col in dataSet1.MEPOMSERVIS_SMP_ITOG.Columns)
            {
                efm.PrintCell(indexrow, x, col.Caption, sHeader);
                x++;
            }

            indexrow++;
            foreach (DataRow row in dataSet1.MEPOMSERVIS_SMP_ITOG.Rows)
            {
                x = 0;
                foreach (DataColumn col in dataSet1.MEPOMSERVIS_SMP_ITOG.Columns)
                {
                    if (col.DataType != typeof(System.Decimal))
                    {
                        if (col.DataType == typeof(System.Int32))
                        {
                            efm.PrintCell(indexrow, x, Convert.ToDouble(row[col]), sInt);
                        }
                        else
                            efm.PrintCell(indexrow, x, row[col].ToString(), sString);
                    }
                    else
                    {
                        efm.PrintCell(indexrow, x, Convert.ToDouble(row[col]), sDouble);
                    }
                    x++;
                }
                indexrow++;

            }
            efm.AutoSizeColumn(0, dataSet1.MEPOMSERVIS_SMP_ITOG.Columns.Count);


            efm.SaveToFile(Path.Combine(Path.GetDirectoryName(CurrPath), Path.GetFileNameWithoutExtension(CurrPath) + ".xls"));
            if (MessageBox.Show("Показать файл?", "Формирование файла завершено", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                SANK_SMO.MTR2015.ShowSelectedInExplorer.FileOrFolder(Path.Combine(Path.GetDirectoryName(CurrPath), Path.GetFileNameWithoutExtension(CurrPath) + ".xls"));
            }
        }

        private void SvodSMO_Load(object sender, EventArgs e)
        {
            numericUpDownItogSMPYear.Value = DateTime.Now.Year;
            numericUpDownVMP_YEAR.Value = DateTime.Now.Year;
            numericUpDownDisp_YEAR.Value = DateTime.Now.Year;
            SetStatusText("");
        }

        int second = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            second++;
            toolStripStatusTime.Text = ": "+second.ToString() +" сек";
        }

        void StartTimer()
        {
            second = 0;
            timer1.Enabled = true;
        }

        void StopTimer()
        {
            timer1.Enabled = false;
        }
    }
}
