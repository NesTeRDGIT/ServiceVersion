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
using Oracle.ManagedDataAccess.Client;
using System.IO;

namespace ClientServise
{
    public partial class PrepareReestr : Form
    {
        IWcfInterface WcfInterface
        {
            get
            {
                return MainForm.MyWcfConnection;
            }
        }
        List<OrclProcedure> list;

        public PrepareReestr()
        {
            InitializeComponent();
            BlockInterfice(LoginForm.SecureCard);
            if (groupBoxPrepare.Enabled)
            {
                list = WcfInterface.GetListTransfer();
                RefreshListBox();
            }
        }


        void BlockInterfice(List<string> card)
        {
            groupBoxPrepare.Enabled = card.Contains("GetListTransfer") && card.Contains("RunProcListTransfer");
           groupBoxFAKT.Enabled = card.Contains("GetID_SPOSOB") && card.Contains("GetVIDMP") &&
               card.Contains("GetMUR_FIN") && card.Contains("GetMUR_FIN_SMP") &&
               card.Contains("Getf003") && card.Contains("Getf002") &&
               card.Contains("GetV_XML_H_FAKTURA") ;        
        }
        void RefreshListBox()
        {
            checkedListBox1.Items.Clear();

            foreach (OrclProcedure op in list)
            {
                checkedListBox1.Items.Add(op.NAME_ERR);
            }
            if(DateTime.Now.Month == 1)
            {
                comboBoxMonthFACT.SelectedIndex  = 11;
            }
            else
            {
                comboBoxMonthFACT.SelectedIndex = DateTime.Now.Month - 2;
            }
            
        }

        private void PrepareReestr_Load(object sender, EventArgs e)
        {

        }

        void Progress(int pos, int max)
        {
            progressBar1.Invoke(new Action(() =>
                {
                    progressBar1.Maximum = max;
                    progressBar1.Value = pos;
                }));
        }

        void Label(string str)
        {
            progressBar1.Invoke(new Action(() =>
            {
                label1.Text = str;
            }));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (OrclProcedure op in list)
            {
                op.STATE = false;
            }
            foreach (int i in checkedListBox1.CheckedIndices)
            {
                list[i].STATE = true;
            }

            button1.Enabled = false;
            Thread th = new Thread(new ThreadStart(ThreadF));
            th.Start();
        }

        void ThreadF()
        {
            try
            {
                for (int i = 0; i < list.Count; i++)
                {
                    Progress(i + 1, list.Count);
                    if (list[i].STATE)
                    {
                        Label("Выполнение " + list[i].NAME_ERR);
                        WcfInterface.RunProcListTransfer(i,1);
                    }
                }
                button1.Invoke(new Action(() =>
                    {
                        button1.Enabled = true;
                    }));
                Label("Завершено");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                button1.Invoke(new Action(() =>
                    {
                        button1.Enabled = true;
                    }));
            }
        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
                checkedListBox1.SetItemChecked(i, checkBox1.Checked);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(WcfInterface.CheckTableTemp1());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBox.Show(WcfInterface.CheckTableTemp100());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                WcfInterface.ClearBaseTemp1();
                MessageBox.Show("Завершено");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                WcfInterface.ClearBaseTemp100();
                MessageBox.Show("Завершено");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        void SetProgress(int value, int Max)
        {
            progressBar2.Invoke(new Action(() =>
                {
                    progressBar2.Maximum = Max;
                    progressBar2.Value = value;
                }));
        }

        void SetLabel2(string TEXT)
        {
            progressBar2.Invoke(new Action(() =>
            {
                label2.Text = TEXT;
            }));
        }
        string MonthFact = "";
        private void button6_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Thread th = new Thread(new ParameterizedThreadStart(Fact));
                th.IsBackground = true;
                MonthFact = comboBoxMonthFACT.Text;
                th.Start(folderBrowserDialog1.SelectedPath);
            }
        }

        void Fact(object obj)
        {
            try
            {
                string path = obj as string;
                SetLabel2("Запрос справочников: Способ оплаты");
                SetProgress(0, 5);
                DataTable SPOSOB_OPL = WcfInterface.GetID_SPOSOB();
                SetLabel2("Запрос справочников: Виды помощи");
                SetProgress(0, 5);
                DataTable VIDMP = WcfInterface.GetVIDMP();            

                SetLabel2("Запрос справочников: MUR_FIN");
                SetProgress(1, 5);
                DataTable MUR_FIN = WcfInterface.GetMUR_FIN();
            

                SetLabel2("Запрос справочников: MUR_FIN_SMP");
                SetProgress(1, 5);
                DataTable MUR_FIN_SMP = WcfInterface.GetMUR_FIN_SMP();
             

                SetLabel2("Запрос справочников: МО");
                SetProgress(2, 5);
                DataTable code_mo_1 = WcfInterface.Getf003();
     

                SetLabel2("Запрос справочников: СМО");
                SetProgress(2, 5);
                DataTable smo = WcfInterface.Getf002();

                SetLabel2("Запрос данных");
                SetProgress(3, 5);
                DataTable V_FAKTURA_2015 = WcfInterface.GetV_XML_H_FAKTURA();
                SetProgress(4, 5);
                SetLabel2("Определение списка МО для выгрузки");

                //Определение списка МО для выгрузки
                DataView viewV = new DataView(V_FAKTURA_2015);
                DataTable distinctValuesV = new DataTable();
                distinctValuesV = viewV.ToTable(true, "mcod");

                DataTable code_mo = new DataTable();
                foreach (DataColumn col in code_mo_1.Columns)
                {
                    code_mo.Columns.Add(col.ColumnName, col.DataType);
                }
                foreach (DataRow row in distinctValuesV.Rows)
                {
                    var t = code_mo_1.Select("mcod = '" + row["mcod"] + "'");
                    if (t.Length == 1)
                        code_mo.Rows.Add(t[0].ItemArray);
                    else
                    MessageBox.Show("Код МО = "+ row["mcod"].ToString()+" не найден в справочнике(F003)");
                }
                SetProgress(5, 5);

                int Row = 0;
                int Col = 0;

                SetProgress(0, code_mo.Rows.Count);
                ///Для каждой МО   
                int index = 0;
                foreach (DataRow row_mo in code_mo.Rows)
                {
                    SetLabel2("Выгрузка " + row_mo["nam_mok"].ToString() + "(" + row_mo["mcod"].ToString() + ")");
                    index++;
                    SetProgress(index, code_mo.Rows.Count);
                    
                    ExcelFileManager efm = new ExcelFileManager(false);
                    var styleName = efm.GetStyle(false, CellAlignment.CENTER, "", true, true);
                    styleName.SetFont(efm.GetFont(12, "Times New Roman", true));

                    var styleZGLV = efm.GetStyle(true, CellAlignment.CENTER, "", true, true);
                    styleZGLV.SetFont(efm.GetFont(12, "Times New Roman", true));

                    var styleSchet = efm.GetStyle(false, CellAlignment.RIGHT, "", false, true);
                    styleSchet.SetFont(efm.GetFont(12, "Times New Roman", false));
                    var styleOka = efm.GetStyle(false, CellAlignment.CENTER, "", false, true);
                    styleOka.SetFont(efm.GetFont(12, "Times New Roman", false));

                    var styleLeft = efm.GetStyle(true, CellAlignment.LEFT, "", false, true);
                    styleLeft.SetFont(efm.GetFont(12, "Times New Roman", false));

                    var styleLeftNB = efm.GetStyle(false, CellAlignment.LEFT, "", false, true);
                    styleLeftNB.SetFont(efm.GetFont(12, "Times New Roman", false));

                    var styleRight = efm.GetStyle(true, CellAlignment.RIGHT, "", false, true);
                    styleRight.SetFont(efm.GetFont(12, "Times New Roman", false));

                    var styleRightN0 = efm.GetStyle(true, CellAlignment.RIGHT, ExcelFileManager.N0, false, true);
                    styleRightN0.SetFont(efm.GetFont(12, "Times New Roman", false));

                    var styleRightN2 = efm.GetStyle(true, CellAlignment.RIGHT, ExcelFileManager.N2, false, true);
                    styleRightN2.SetFont(efm.GetFont(12, "Times New Roman", false));

                    var styleRightN3 = efm.GetStyle(true, CellAlignment.RIGHT, ExcelFileManager.N3, false, true);
                    styleRightN3.SetFont(efm.GetFont(12, "Times New Roman", false));


                    var styleLeftBold = efm.GetStyle(true, CellAlignment.LEFT, "", true, true);
                    styleLeftBold.SetFont(efm.GetFont(12, "Times New Roman", true));

                    var styleRigthBoldN2 = efm.GetStyle(true, CellAlignment.RIGHT, ExcelFileManager.N2, true, true);
                    styleRigthBoldN2.SetFont(efm.GetFont(12, "Times New Roman", true));
                    var styleRigthBoldN0 = efm.GetStyle(true, CellAlignment.RIGHT, ExcelFileManager.N0, true, true);
                    styleRigthBoldN0.SetFont(efm.GetFont(12, "Times New Roman", true));

                    // Суммы для Word
                  

                    double SUM_K;
                    //ДЛЯ каждой страховой                        
                    foreach (DataRow row_smo in smo.Rows)
                    {
                        double POL = 0;
                        double STAC = 0;
                        double STAC_NSZ = 0;
                        double DS = 0;
                        double DOP = 0;
                        double SMP = 0;
                        double HMP = 0;
                        double NET = 0;
                        double FIN_S = 0;
                        double FIN_SMP = 0;
                        SUM_K = 0;
                        //Имя листа
                        efm.addScheet(row_smo["nam_smok"].ToString());
                        Row = 0;
                        Col = 0;
                        //Размер колонок
                        efm.SetColumnWidth(0, 14 * 256);
                        efm.SetColumnWidth(1, 73 * 256);
                        efm.SetColumnWidth(2, 14 * 256);
                        efm.SetColumnWidth(3, 14 * 256);
                        efm.SetColumnWidth(4, 14 * 256);
                        efm.SetColumnWidth(5, 14 * 256);
                        efm.SetColumnWidth(6, 14 * 256);
                        //Имя мо
                        efm.PrintCell(Row, Col, row_mo["nam_mok"].ToString(), styleName);
                        efm.AddMergedRegion(Row, 0, Row, 4, styleName);
                        Row++;
                        //Строка счет фактура
                        efm.AddMergedRegion(Row, 0, Row, 1, styleSchet);
                        efm.PrintCell(Row, Col, "СЧЕТ-ФАКТУРА от", styleSchet);
                        Row++;
                        //Строка за оказанные 
                        efm.AddMergedRegion(Row, 0, Row, 4, styleOka);
                        efm.PrintCell(Row, Col, "за оказанные медицинские услуги по территориальной программе ОМС пациентам своей территории", styleOka);
                        Row++;
                        //Строка месяц
                        efm.PrintCell(Row, Col, "за сентябрь", styleOka);
                        efm.AddMergedRegion(Row, 0, Row, 4, styleOka);
                        Row++;
                        //Строка Страховая
                        efm.PrintCell(Row, Col, "по договору", styleOka);
                        efm.PrintCell(Row, 1, row_smo["nam_smok"].ToString(), styleOka);
                        Row++;
                        Row++;
                        //Проходим по способам финансирования
                        foreach (DataRow row_opl in SPOSOB_OPL.Rows)
                        {

                            string str1 = "Q = '" + row_smo["smocod"].ToString() + "' and mcod = '" + row_mo["mcod"].ToString() + "' and FOND = " + row_opl["ID_SPOSOB"].ToString();
                            if (V_FAKTURA_2015.Select(str1).Length == 0)
                                continue;

                            string ZGLV = "-";
                            int Type = Convert.ToInt32(row_opl["ID_SPOSOB"]);
                            switch (Type)
                            {
                                case 1: ZGLV = "Оплата услуг в рамках подушевого финансирования на сумму:"; break;
                                case 2: ZGLV = "Оплата услуг вне рамок подушевого финансирования:"; break;
                                case 3: ZGLV = "Оплата услуг из нормированного страхового запаса ФФОМС:"; break;
                            }

                            if (Type == 1)
                            {
                                efm.PrintCell(Row, 0, ZGLV, styleZGLV);
                                efm.AddMergedRegion(Row, 0, Row, 3, styleZGLV);
                                var m = MUR_FIN.Select("code_mo = '" + row_mo["mcod"].ToString() + "' and smo = '" + row_smo["smocod"].ToString() + "'");
                                if (m.Length == 1)
                                {
                                    efm.PrintCell(Row, 4, Convert.ToDouble(m[0]["sum"]), styleRigthBoldN2);
                                    FIN_S += Convert.ToDouble(m[0]["sum"]);
                                    SUM_K += Convert.ToDouble(m[0]["sum"]);
                                }
                                else
                                {
                                    MessageBox.Show("Не корректные данные в MUR_FIN для " + row_mo["mcod"].ToString() + "||" + row_smo["smocod"].ToString());
                                }

                                Row++;


                                efm.PrintCell(Row, 0, "Оплата скорой медицинской помощи в рамках подушевого финансирования на сумму:", styleZGLV);
                                efm.AddMergedRegion(Row, 0, Row, 3, styleZGLV);
                                m = MUR_FIN_SMP.Select("code_mo = '" + row_mo["mcod"].ToString() + "' and smo = '" + row_smo["smocod"].ToString() + "'");
                                if (m.Length == 1)
                                {
                                    efm.PrintCell(Row, 4, Convert.ToDouble(m[0]["sum"]), styleRigthBoldN2);
                                    FIN_SMP += Convert.ToDouble(m[0]["sum"]); 
                                    SUM_K += Convert.ToDouble(m[0]["sum"]);
                                }
                                else
                                {
                                    efm.PrintCell(Row, 4,0, styleRigthBoldN2);                                  
                                }
                            }
                            else
                            {
                                efm.PrintCell(Row, 0, ZGLV, styleZGLV);
                                efm.AddMergedRegion(Row, 0, Row, 6, styleZGLV);
                            }

                            Row++;

                            Row++;

                            efm.PrintCell(Row, 0, "Код услуги", styleZGLV);
                            efm.PrintCell(Row, 1, "Наименование услуги", styleZGLV);
                            efm.PrintCell(Row, 2, "Кол-во случаев", styleZGLV);
                            efm.PrintCell(Row, 3, "Кол-во услуг(к/дней, п/дней, посещений, УЕТ)", styleZGLV);
                            if (Type != 1)
                            {
                                efm.PrintCell(Row, 4, "Тариф", styleZGLV);
                                efm.PrintCell(Row, 5, "Поправочный коэффициент", styleZGLV);
                                efm.PrintCell(Row, 6, "Стоимость", styleZGLV);
                            }
                            Row++;

                            foreach (DataRow rowVIDMP in VIDMP.Rows)
                            {
                                string str = "Q = '" + row_smo["smocod"].ToString() + "' and mcod = '" + row_mo["mcod"].ToString() + "' and FOND = " + row_opl["ID_SPOSOB"].ToString() + " and TIP = '" + rowVIDMP["id_vidmp"].ToString() + "'";
                                var data = V_FAKTURA_2015.Select(str, "COD ASC");
                                if (data.Length == 0)
                                {
                                    continue;
                                }



                                efm.PrintCell(Row, 0, rowVIDMP["NAME_VIDMP"].ToString(), styleZGLV);
                                if (Type != 1)
                                {
                                    efm.AddMergedRegion(Row, 0, Row, 6, styleZGLV);
                                }
                                else
                                {
                                    efm.AddMergedRegion(Row, 0, Row, 3, styleZGLV);
                                }
                                Row++;

                                double sum_sl = 0;
                                double sum_ku = 0;
                                double sum_s_all = 0;
                               
                                foreach (DataRow row_fact in data)
                                {
                                    //Cуммы для Word
                                    if (Type != 1)
                                    {
                                        switch (row_fact["TIP"].ToString().Substring(0, 3))
                                        {
                                            case "AMB":
                                            case "DIS":
                                            case "OSM":
                                            case "EXT":
                                                POL += Convert.ToDouble(row_fact["s_all"]);
                                                break;
                                            case "STC":
                                                STAC += Convert.ToDouble(row_fact["s_all"]);
                                                if (row_fact["FOND"].ToString() == "3")
                                                {
                                                    STAC_NSZ += Convert.ToDouble(row_fact["s_all"]);
                                                }
                                                break;
                                            case "DST":
                                                DS += Convert.ToDouble(row_fact["s_all"]);
                                                break;
                                            case "OMT":
                                                DOP += Convert.ToDouble(row_fact["s_all"]);
                                                break;
                                            case "SMP":
                                                SMP += Convert.ToDouble(row_fact["s_all"]);
                                                break;
                                            case "HMP":
                                                HMP += Convert.ToDouble(row_fact["s_all"]);
                                                break;
                                            case "NET":
                                            case "НЕТ":
                                                NET += Convert.ToDouble(row_fact["s_all"]);
                                                break;
                                        }
                                    }

                                    efm.PrintCell(Row, 0, row_fact["COD"].ToString(), styleRight);
                                    efm.PrintCell(Row, 1, row_fact["NAME_TARIF"].ToString(), styleLeft);
                                    efm.PrintCell(Row, 2, Convert.ToDouble(row_fact["SLUCH"]), styleRightN0);
                                    efm.PrintCell(Row, 3, Convert.ToDouble(row_fact["K_U"]), styleRightN2);
                                    if (Type != 1)
                                    {
                                        efm.PrintCell(Row, 4, Convert.ToDouble(row_fact["SUMMA"]), styleRightN2);
                                        efm.PrintCell(Row, 5, Convert.ToDouble(row_fact["SK"]), styleRightN3);
                                        efm.PrintCell(Row, 6, Convert.ToDouble(row_fact["S_ALL"]), styleRightN2);
                                        sum_s_all += Convert.ToDouble(row_fact["S_ALL"]);
                                    }
                                    sum_sl += Convert.ToDouble(row_fact["SLUCH"]);
                                    sum_ku += Convert.ToDouble(row_fact["K_U"]);
                                    

                                    Row++;
                                }

                                SUM_K += sum_s_all;

                                efm.PrintCell(Row, 0, "Всего по группе:", styleLeftBold);
                                efm.PrintCell(Row, 1, "", styleRigthBoldN2);
                                efm.PrintCell(Row, 2, sum_sl, styleRigthBoldN0);
                                efm.PrintCell(Row, 3, sum_ku, styleRigthBoldN2);
                                if (Type != 1)
                                {
                                    efm.PrintCell(Row, 4, "", styleRigthBoldN2);
                                    efm.PrintCell(Row, 5, "", styleRigthBoldN2);
                                    efm.PrintCell(Row, 6, sum_s_all, styleRigthBoldN2);
                                }
                                efm.AddMergedRegion(Row, 0, Row, 1);
                                Row++;
                                Row++;
                            }
                        }
                        Row--;
                        efm.PrintCell(Row, 0, "Итого по компании", styleLeftBold);
                        efm.AddMergedRegion(Row, 0, Row, 1, styleLeftBold);
                        efm.PrintCell(Row, 4, SUM_K, styleRigthBoldN2);
                        Row++;
                        Row++;
                        Row++;
                        efm.PrintCell(Row, 1, "Руководитель медицинского учреждения", styleLeftNB);
                        Row++;
                        efm.PrintCell(Row, 1, "Главный бухгалтер медицинского учреждения", styleLeftNB);
                        //Файл WORD
                        string PathTemplateWORD = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "TEMPLATE\\ITOG_REESTR.docx");
                        string PathNewNameWORD = Path.Combine(path, row_mo["mcod"].ToString() + " Итоговый реестр " + row_smo["smocod"].ToString() + ".docx");
                        File.Copy(PathTemplateWORD, PathNewNameWORD, true);
                        Stream FSt = File.Open(PathNewNameWORD, FileMode.Open, FileAccess.ReadWrite);
                        DocumentFormat.OpenXml.Packaging.WordprocessingDocument file = DocumentFormat.OpenXml.Packaging.WordprocessingDocument.Open(FSt, true);



                        WordManager.ReplaceDocBookMark(file, "b_lpu", row_mo["nam_mok"].ToString());
                        WordManager.ReplaceDocBookMark(file, "b_smo", row_smo["nam_smok"].ToString());
                        WordManager.ReplaceDocBookMark(file, "b_soul", NumberToStr(FIN_S));
                        WordManager.ReplaceDocBookMark(file, "b_smp", NumberToStr(FIN_SMP));
                        WordManager.ReplaceDocBookMark(file, "b_pol", NumberToStr(POL));
                        WordManager.ReplaceDocBookMark(file, "b_month", MonthFact.ToLower());
                        
                        if (STAC_NSZ != 0)
                        {

                            WordManager.ReplaceDocBookMark(file, "b_stac", NumberToStr(STAC) + ", в том числе в рамках НСЗ ФФ " + NumberToStr(STAC_NSZ));
                        }
                        else
                        {
                            WordManager.ReplaceDocBookMark(file, "b_stac", NumberToStr(STAC));
                        }
                        WordManager.ReplaceDocBookMark(file, "b_ds", NumberToStr(DS));
                        WordManager.ReplaceDocBookMark(file, "b_dop", NumberToStr(DOP));
                        WordManager.ReplaceDocBookMark(file, "b_hmp", NumberToStr(HMP));
                        WordManager.ReplaceDocBookMark(file, "b_net", NumberToStr(NET));
                        WordManager.ReplaceDocBookMark(file, "b_itog", NumberToStr((STAC+FIN_S+FIN_SMP+POL+DS+DOP+HMP+NET)));
                        file.MainDocumentPart.Document.Save();
                        file.Close();
                        FSt.Close();
                    }

                    efm.SaveToFile(Path.Combine(path, row_mo["mcod"].ToString() + "Счет-фактура.xls"));
                   


                }

                SetLabel2("Завершено");
                if (MessageBox.Show("Завершено. Показать файлы?", "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    ClientServise.SANK_SMO.MTR2015.ShowSelectedInExplorer.FileOrFolder(path);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        string NumberToStr(double value)
        {
            return value.ToString("N2")+" ("+RusCurrency.Str(value)+")";
        }
        
    }
}
