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
using System.Threading.Tasks;
using ClientServise.SANK_SMO.MTR2015;
using Oracle.ManagedDataAccess.Client;
using ServiceLoaderMedpomData.EntityMP_V31;

namespace ClientServise
{


    enum Type_Invite
    {
        Type1,
        Type1L,
        Type2,
        Type2L
    }
    public partial class SANK_INVITER2 : Form
    {
        SchemaChecking sc = new SchemaChecking();
        SchemaColection scoll = new SchemaColection();

        Type_Invite TI;

        public SANK_INVITER2()
        {
            InitializeComponent();
            try
            {
                scoll = new SchemaColection();
                if (File.Exists(Path.Combine(Application.StartupPath, "SANK_INVITER_SCHEMA.dat")))
                    scoll.LoadFromFile(Path.Combine(Application.StartupPath, "SANK_INVITER_SCHEMA.dat"));
                else
                    MessageBox.Show(@"Файл схем не найден. Нужно проверить параметры");


                panelERXSD.BackColor = cEXSD;
                panelFLK.BackColor = cFlkErr;
                panelLOAD.BackColor = cLoad;
                panelnotInvite.BackColor = cNotInvite;
                panelNOT_ZGLV_ID.BackColor = c_ID_NOT_DATA;
                panelNotValueDop.BackColor = c_DOP_NOT_DATA;
                panelCHEK_FLK.BackColor = c_CHEK_FLK;

            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Ошибка считывания параметров:" + ex.Message);
                this.Close();
            }
        }

        List<FileItem> Files;
        private void SANK_INVITER_Load(object sender, EventArgs e)
        {
            Files = new List<FileItem>();
            fileItemBindingSource.DataSource = Files;
            folder = Properties.Settings.Default.FOLDER_LOG_SANK;
            textBox1.Text = folder;
            dateTimePicker1.Value = DateTime.Now.AddMonths(-1);
            selectMonth = dateTimePicker1.Value.Month;
            selectYear = dateTimePicker1.Value.Year;
            comboBox1.SelectedItem = 0;
            SetActiveButton();
        }
        void SetTextStatus(string text)
        {
            statusStrip1.Invoke(new Action(() =>
                {
                    toolStripStatusLabel1.Text = text;
                }
            ));
        }

        void SetProggressStatus(int value)
        {
            statusStrip1.Invoke(new Action(() =>
            {
                toolStripProgressBar1.Value = value;
            }
            ));
        }

        void SetProggressMAXStatus(int MAX)
        {
            statusStrip1.Invoke(new Action(() =>
            {
                toolStripProgressBar1.Maximum = MAX;
            }
            ));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folder == "" || !Directory.Exists(folder))
            {
                MessageBox.Show(@"Укажите директорию логов");
                return;
            }
            ReadSetting();
            var th = new Thread(CheckSchema) {IsBackground = true};
            th.Start();
        }
        int _countSchemaFail =0;
        int countSchemaFail
        {
            get
            {
                return _countSchemaFail;
            }
            set
            {
                _countSchemaFail = value;
                labelSCHEMA_COUNT.Invoke(new Action(() =>
                {
                    labelSCHEMA_COUNT.Text = _countSchemaFail.ToString();
                }));
            }
        }
                int _countNOT_DOP = 0;
             int countNOT_DOP
        {
            get
            {
                return _countNOT_DOP;
            }
            set
            {
                _countNOT_DOP = value;
                labelSCHEMA_COUNT.Invoke(new Action(() =>
                {
                    labelNotValueDop.Text = _countNOT_DOP.ToString();
                }));
            }
        }
        private void CheckSchema()
        {
            FileItem item = null;
            try
            {
                countSchemaFail = 0;
                countNOT_DOP = 0;

                SetProggressMAXStatus(Files.Count);
                for (var i = 0; i < Files.Count; i++)
                {
                    item = Files[i];
                    var ActivSCOLL = scoll;

                    SetProggressStatus(i);
                    SetTextStatus(item.FileName);
                    if ((item.Process == StepsProcess.NotInvite || item.Process == StepsProcess.FlkErr ||
                         item.Process == StepsProcess.FlkOk || item.Process == StepsProcess.XMLxsd) &&
                        (item.filel?.Process == StepsProcess.NotInvite || item.filel?.Process == StepsProcess.FlkErr ||
                         item.filel?.Process == StepsProcess.FlkOk ||
                         item.filel?.Process == StepsProcess.XMLxsd)) continue;
                    item.FileLog.Append();
                    item.filel?.FileLog.Append();
                    var vers_file_l = SchemaChecking.GetCode_fromXML(item.filel.FilePach, "VERSION");
                    var vers_file = SchemaChecking.GetCode_fromXML(item.FilePach, "VERSION");
                    var year = SchemaChecking.GetCode_fromXML(item.FilePach, "YEAR");
                    var month = SchemaChecking.GetCode_fromXML(item.FilePach, "MONTH");
                    var dt_file = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 1);



                    item.Version = VersionMP.NONE;

                    var sc_file = ActivSCOLL.FindSchema(vers_file, dt_file, item.Type.Value);
                    //Костыль для ЗМС
                    if (vers_file == "3.0" && FLAG_MEE == 1 && item.Type == FileType.C && SMO == "75001")
                    {
                        //sc_file = ActivSCOLL.FindSchema(vers_file, dt_file, FileType.H);
                    }

                    var sc_filel = ActivSCOLL.FindSchema(vers_file_l, dt_file, item.filel.Type.Value);

                    var item1 = item;
                    Invoke(new Action(() =>
                    {
                        if (sc_file.Result)
                        {
                            item1.Version = sc_file.Vers;
                        }
                        else
                        {
                            item1.Process = StepsProcess.ErrorXMLxsd;
                            var err = $"Недопустимая версия документа: {sc_file.Exception}";

                            item1.CommentAndLog = err;
                            var pathToXml = Path.Combine(Path.GetDirectoryName(item1.FileLog.FilePath),
                                Path.GetFileNameWithoutExtension(item1.FileLog.FilePath) + "FLK.xml");
                            SchemaChecking.XMLfileFLK(pathToXml, item1.FileName,
                                new List<ErrorProtocolXML> {new ErrorProtocolXML {BAS_EL = "VERSION", Comment = err}});
                            countSchemaFail++;
                        }

                        if (sc_filel.Result)
                        {
                            item1.filel.Version = sc_filel.Vers;
                        }
                        else
                        {
                            item1.filel.Process = StepsProcess.ErrorXMLxsd;
                            var err = $"Недопустимая версия документа: {sc_filel.Exception}";
                            item1.filel.CommentAndLog = err;
                            item1.CommentAndLog = err;
                            var pathToXml = Path.Combine(Path.GetDirectoryName(item1.filel.FileLog.FilePath),
                                Path.GetFileNameWithoutExtension(item1.filel.FileLog.FilePath) + "FLK.xml");
                            SchemaChecking.XMLfileFLK(pathToXml, item1.filel.FileName,
                                new List<ErrorProtocolXML> {new ErrorProtocolXML {BAS_EL = "VERSION", Comment = err}});
                            countSchemaFail++;

                            countSchemaFail++;
                        }
                    }));

                    //проверка основного файла
                    if (item.Version != VersionMP.NONE)
                    {
                        var res = sc.CheckSchema(item, sc_file.Value.Value);
                        var item2 = item;
                        Invoke(new Action(() =>
                        {
                            if (res)
                            {
                                item2.Process = StepsProcess.XMLxsd;
                                item2.Comment = "Схема правильная";
                                if (!item2.DOP_REESTR.HasValue)
                                {
                                    countNOT_DOP++;
                                }
                            }
                            else
                            {
                                item2.Comment = "Схема ошибочна";
                                item2.Process = StepsProcess.ErrorXMLxsd;
                                countSchemaFail++;
                            }
                        }));
                    }

                    //Проверка файла перс
                    if (item.filel != null)
                    {
                        if (item.filel.Version != VersionMP.NONE)
                        {
                            var res = sc.CheckSchema(item.filel, sc_filel.Value.Value);
                            var item2 = item;
                            Invoke(new Action(() =>
                            {
                                if (res)
                                {
                                    item2.filel.Process = StepsProcess.XMLxsd;
                                    item2.filel.Comment = "Схема правильная";
                                }
                                else
                                {
                                    item2.filel.Comment = "Схема ошибочна";
                                    item2.filel.Process = StepsProcess.ErrorXMLxsd;
                                    countSchemaFail++;
                                    if (item2.Process == StepsProcess.XMLxsd)
                                    {
                                        item2.Comment = "Схема ошибочна в файле перс данных";
                                        item2.Process = StepsProcess.ErrorXMLxsd;
                                    }
                                    else
                                    {
                                        item2.Comment += " Схема ошибочна в файле перс данных";
                                        item2.Process = StepsProcess.ErrorXMLxsd;
                                    }
                                }
                            }));
                        }
                    }

                    item.FileLog.Close();
                    item.filel?.FileLog.Close();

                    DataGridRecolor(i);
                    dataGridView1.Invoke(new Action(() => { fileItemBindingSource.ResetItem(i); }));
                    Task.Run(() =>
                    {
                    });
                }

                SetProggressStatus(0);
                SetTextStatus("Проверка на схему завершена");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                if (item != null)
                {
                    item.FileLog.Close();
                    item.filel?.FileLog.Close();
                }
            }
            finally
            {
                SetActiveButton();
            }
        }
        string folder;
        private void button2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                folder = folderBrowserDialog1.SelectedPath;
              Properties.Settings.Default.FOLDER_LOG_SANK = folder;
                Properties.Settings.Default.Save();
                textBox1.Text = folder;
            }
        }

        int FLAG_MEE;
        Thread th;
        private bool isRewriteSum = false;
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                ReadSetting();
                if(SMO=="")
                {
                    MessageBox.Show("SMO");
                    return;
                }
                if (MessageBox.Show($@"Загрузить в {new DateTime(selectYear,selectMonth,1):MMMMMMMMMMMM yyyy}. ТИП: {(radioButtonMEK.Checked ? "МЭК" : "МЭЭ\\ЭКМП")}", "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    if (TI == Type_Invite.Type1L || TI == Type_Invite.Type2L)
                    {
                        th = new Thread(Transfer) {IsBackground = true};
                        th.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void SetActiveButton()
        {
            this.Invoke(new Action(() =>
            {
                buttonCHECK_SCHEMA.Enabled =Files.Count(x => x.Process.In(StepsProcess.ErrorXMLxsd, StepsProcess.Invite)) != 0 && Files.Count!=0;
                buttonFIND.Enabled = Files.Count(x => x.Process.In(StepsProcess.XMLxsd)) != 0 && Files.Count != 0;
                buttonFLK.Enabled = Files.Count(x =>x.Process.In(StepsProcess.XMLxsd) && (x.ZGLV_ID.HasValue || x.DOP_REESTR == true)) != 0 && Files.Count != 0;
                buttonToBase.Enabled = Files.Count(x => x.Process.In(StepsProcess.XMLxsd) && (x.ZGLV_ID.HasValue || x.DOP_REESTR == true)) == Files.Count && Files.Count != 0;
            }));
        }

        private bool not_finish_sank;
        private void ReadSetting()
        {
            FLAG_MEE = radioButtonMEK.Checked ? 0 : 1;
            SMO = textBoxSMO.Text;
            selectMonth = dateTimePicker1.Value.Month;
            selectYear = dateTimePicker1.Value.Year;
            isRewriteSum = checkBoxRewriteSum.Checked && checkBoxRewriteSum.Enabled;
            mee_sum_validate = checkBoxValidate.Checked && checkBoxValidate.Enabled;
            not_finish_sank = checkBoxNotFinish.Checked && checkBoxNotFinish.Enabled;
            isEXT_FLK = checkBoxIsExFLK.Checked && checkBoxIsExFLK.Enabled;
        }
        private void Transfer()
        {
            try
            {
                this.Invoke(new Action(() => { buttonToBase.Enabled = false; }));
                var ta = CreateMyBD();
                SetProggressMAXStatus(Files.Count);


                var countErrTransfer = 0;
                var countTransfer = 0;
                for (var i = 0; i < Files.Count; i++)
                {
                    SetProggressStatus(i);
                    SetTextStatus("Перенос " + Files[i].FileName);

                    var fi = Files[i];
                    try
                    {
                        if (fi.Process != StepsProcess.XMLxsd) continue;
                        if ((!fi.ZGLV_ID.HasValue || fi.ZGLV_ID == -1) && fi.DOP_REESTR == false) continue;

                        fi.FileLog.Append();
                        fi.filel.FileLog.Append();


                        bool rez1;
                       
                        if (fi.DOP_REESTR == true && FLAG_MEE == 0)
                        {
                            rez1 = ToBaseFile(fi, ta, 1);
                        }
                        else
                        {
                            var zl = ZL_LIST.GetZL_LIST(fi.Version, fi.FilePach);
                            zl.SetSUMP();
                            fi.FileLog.WriteLn("Чтение файла " + fi.FileName);
                            rez1 = ToBaseFileSANK(fi,zl, ta, isRewriteSum, not_finish_sank);
                           
                        }
                        if (rez1)
                        {
                            this.Invoke(new Action(() =>
                            {
                                fi.Process = StepsProcess.FlkOk;
                                fi.filel.Process = StepsProcess.FlkOk;
                            }));
                            
                            countTransfer++;
                        }
                        else
                        {
                            this.Invoke(new Action(() =>
                            {
                                fi.Process = StepsProcess.FlkErr;
                                fi.filel.Process = StepsProcess.FlkErr;
                            }));
                          
                            countErrTransfer++;
                        }


                        labelCountFLK.Invoke(new Action(() =>
                        {
                            labelCountFLK.Text = countErrTransfer.ToString();
                            labelCountInvite.Text = countTransfer.ToString();
                        }));
                        DataGridRecolor(i);
                        fi.FileLog.Close();
                        fi.filel.FileLog.Close();
                        dataGridView1.Invoke(new Action(() =>
                                    {
                                        fileItemBindingSource.ResetItem(i);
                                        DataGridRecolor(i);
                                    }));
                        //Files.ResetItem(i);                       
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($@"Ошибка при переносе {fi.FileName}: {ex.Message}");
                        if (fi == null) continue;
                        fi.FileLog.Close();
                        fi.filel.FileLog.Close();
                    }
                }
                SetProggressStatus(0);
                SetTextStatus("Перенос завершен");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                SetActiveButton();
            }
        }
        MYBDOracleNEW CreateMyBD()
        {
            return new MYBDOracleNEW(
                                 AppConfig.Property.ConnectionString,
                                 new TableInfo { TableName = AppConfig.Property.xml_h_zglv, SchemaName = AppConfig.Property.schemaOracle, SeqName = AppConfig.Property.seq_ZGLV },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_schet, SchemaName = AppConfig.Property.schemaOracle, SeqName = AppConfig.Property.seq_SCHET },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_sank, SchemaName = AppConfig.Property.schemaOracle, SeqName = AppConfig.Property.seq_SANK },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_code_exp, SchemaName = AppConfig.Property.schemaOracle, SeqName ="" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_pacient, SchemaName = AppConfig.Property.schemaOracle, SeqName = AppConfig.Property.seq_PACIENT },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_zap, SchemaName = AppConfig.Property.schemaOracle, SeqName = AppConfig.Property.seq_ZAP },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_usl, SchemaName = AppConfig.Property.schemaOracle, SeqName = AppConfig.Property.seq_USL },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_sluch, SchemaName = AppConfig.Property.schemaOracle, SeqName = AppConfig.Property.seq_SLUCH },

                                 new TableInfo { TableName = AppConfig.Property.xml_h_ds2, SchemaName = AppConfig.Property.schemaOracle, SeqName = AppConfig.Property.seq_SLUCH },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_ds3, SchemaName = AppConfig.Property.schemaOracle, SeqName = AppConfig.Property.seq_SLUCH },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_crit, SchemaName = AppConfig.Property.schemaOracle, SeqName = AppConfig.Property.seq_SLUCH },
                                 
                                 new TableInfo { TableName = AppConfig.Property.xml_h_z_sluch, SchemaName = AppConfig.Property.schemaOracle, SeqName = AppConfig.Property.seq_z_sluch },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_kslp, SchemaName = AppConfig.Property.schemaOracle, SeqName = "" },
                                 new TableInfo { TableName = AppConfig.Property.xml_l_zglv, SchemaName = AppConfig.Property.schemaOracle, SeqName = AppConfig.Property.seq_L_ZGLV },
                                 new TableInfo { TableName = AppConfig.Property.xml_l_pers, SchemaName = AppConfig.Property.schemaOracle, SeqName = AppConfig.Property.seq_L_pers },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_ds2_n, SchemaName = AppConfig.Property.schemaOracle, SeqName = "" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_nazr, SchemaName = AppConfig.Property.schemaOracle, SeqName = "" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_b_diag, SchemaName = AppConfig.Property.schemaOracle, SeqName = " " },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_b_prot, SchemaName = AppConfig.Property.schemaOracle, SeqName = " " },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_napr, SchemaName = AppConfig.Property.schemaOracle, SeqName = " " },

                                 new TableInfo { TableName = AppConfig.Property.xml_h_onk_usl, SchemaName = AppConfig.Property.schemaOracle, SeqName = AppConfig.Property.seq_xml_h_onk_usl },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_lek_pr, SchemaName = AppConfig.Property.schemaOracle, SeqName = AppConfig.Property.seq_xml_h_lek_pr },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_date_inj, SchemaName = AppConfig.Property.schemaOracle, SeqName = "" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_cons, SchemaName = AppConfig.Property.schemaOracle, SeqName = "" },
                                 
                                 
                                 new TableInfo { TableName = "", SchemaName = AppConfig.Property.schemaOracle, SeqName = " " },
                                 DateTime.Now);
        }

        int selectMonth;
        int selectYear;
        private bool isEXT_FLK;

       
        private bool ToBaseFile(FileItem fi, MYBDOracleNEW mybd, int flag_dop)
        {
            mybd.BeginTransaction();
           
            try
            {
                var zl = ZL_LIST.GetZL_LIST(fi.Version, fi.FilePach);
                zl.SetSUMP();
                int id;

                var FILENAME = zl.ZGLV.FILENAME.ToUpper();
                var CODE = zl.SCHET.CODE.ToString();
                var CODE_MO = zl.SCHET.CODE_MO.ToString();
                var YEAR = zl.SCHET.YEAR.ToString();
                var MONTH = zl.SCHET.MONTH.ToString();

                fi.FileLog.WriteLn("Заголовок санкций");
                id = mybd.AddSankZGLV(FILENAME, Convert.ToInt32(CODE), Convert.ToInt32(CODE_MO), FLAG_MEE,Convert.ToInt32(YEAR), Convert.ToInt32(MONTH), selectYear, selectMonth, -1, SMO, fi.DOP_REESTR??false, false);

                zl.SCHET.YEAR_BASE = zl.SCHET.YEAR;
                zl.SCHET.MONTH_BASE = zl.SCHET.MONTH;

                zl.SCHET.YEAR = selectYear;
                zl.SCHET.MONTH = selectMonth;
                zl.SCHET.DOP_FLAG = 1;

                fi.FileLog.WriteLn("Подготовка");
                foreach (var z in zl.ZAP)
                {
                    z.PR_NOV = 1;
                }

                foreach (var p in zl.ZAP.Select(x => x.PACIENT))
                {
                    p.SMO_TFOMS = SMO;
                }
                var ZS = zl.ZAP.SelectMany(x => x.Z_SL_list);
                var sanks = ZS.SelectMany(x=>x.SANK);
                foreach (var p in sanks)
                {
                    p.S_ZGLV_ID = id;
                }
            
                fi.FileLog.WriteLn("Загрузка в бд");
                mybd.InsertFile(zl,PERS_LIST.LoadFromFile(fi.filel.FilePach));
                var Z_SL = zl.ZAP.SelectMany(x => x.Z_SL_list).ToList();
                fi.FileLog.WriteLn("Установка заголовков санкций");
                var zsl_ZGLV_count = mybd.UpdateSLUCH_Z_SANK_ZGLV_ID(Z_SL, id);              
              
                if (Z_SL.Count != zsl_ZGLV_count)
                {
                    fi.InvokeComment("Не полное внесение SANK_ZGLV_ID для случаев", this);
                    fi.FileLog.WriteLn($"Не полное внесение SANK_ZGLV_ID для случаев: внесено {zsl_ZGLV_count} из {Z_SL.Count}");
                    mybd.Rollback();
                    return false;
                }

                fi.FileLog.WriteLn("Установка указателя на счет в заголовке санкций");
                var zglv_id = zl.ZGLV.ZGLV_ID.Value;              
                mybd.UpdateSankZGLV(id, Convert.ToInt32(zglv_id));
                
                mybd.Commit();
                fi.InvokeComment("Загрузка завершена", this);
                fi.FileLog.WriteLn("Загрузка завершена");
                return true;
            }
            catch (Exception ex)
            {
                mybd.Rollback();
                fi.FileLog.WriteLn("Ошибка при переносе в БД: " + ex.StackTrace + ex.Message);
                this.Invoke(new Action(() =>
                {
                    fi.Comment = "Ошибка при переносе в БД: " + ex.Message;
                }));
                return false;
            }

        }
        private bool ToBaseFileSANK(FileItem fi, ZL_LIST zl, MYBDOracleNEW mybd, bool IsRewrite, bool isNotFinish)
        {
            mybd.BeginTransaction();
            try
            {
                var EL =  SchemaChecking.GetCode_fromXML(fi.FilePach, "FILENAME", "CODE", "CODE_MO", "YEAR", "MONTH");
                //Заголовок санкций
                var id = mybd.AddSankZGLV(EL["FILENAME"], Convert.ToInt32(EL["CODE"]), Convert.ToInt32(EL["CODE_MO"]), FLAG_MEE,Convert.ToInt32(EL["YEAR"]), Convert.ToInt32(EL["MONTH"]), selectYear, selectMonth, fi.ZGLV_ID.Value, SMO, fi.DOP_REESTR ?? false, isNotFinish);

               
                var rez = mybd.LoadSANK(fi, zl, id, FLAG_MEE == 0, IsRewrite, this, GetIDENT_INFO(fi, zl, mybd));
                if (rez)
                {
                    fi.InvokeComment("Загрузка завершена", this);
                    fi.FileLog.WriteLn("Загрузка завершена");
                    mybd.Commit();
                }
                else
                {
                    fi.InvokeComment("Ошибка загрузки", this);
                    fi.FileLog.WriteLn("Ошибка загрузки");
                    mybd.Rollback();
                }
                return rez;
            }
            catch (Exception ex)
            {
                mybd.Rollback();
                MessageBox.Show(ex.Message);
                return false;
            }

        }


        private void CheckFLK_BASE()
        {
            
            FileItem item = null;
            try
            {
                var CountNotFLK = 0;
                SetProggressMAXStatus(Files.Count);
                for (var i = 0; i < Files.Count; i++)
                {

                    item = Files[i];
                    SetProggressStatus(i);
                    SetTextStatus("Проверка файла:" + item.FileName);
                    if (item.Process != StepsProcess.XMLxsd) continue;

                    item.FileLog.Append();

                    item.FileLog.WriteLn("Чтение файла " + item.FileName);
                    var zl = ZL_LIST.GetZL_LIST(item.Version, item.FilePach);
                    zl.SetSUMP();
                  

                    var mybd = CreateMyBD();
                    item.InvokeComment("Обработка пакета: Проверка ФЛК(BASE)", this);
                    var flk = CheckFLK(item, zl);
                    if (isEXT_FLK)
                    {
                        item.InvokeComment("Обработка пакета: Проверка ФЛК(EXT)", this);
                        flk.AddRange(CheckFLKEx(item, zl, mybd));
                    }
                
                    if (flk.Count != 0)
                    {
                        item.InvokeComment("Обработка пакета: Выгрузка ошибок", this);
                        item.Process = StepsProcess.ErrorXMLxsd;
                        CreateError(item, flk);
                        CountNotFLK++;
                    }
                    item.InvokeComment("Обработка пакета: Проверка завершена", this);

                    DataGridRecolor(i);
                    item.FileLog.Close();

                    dataGridView1.Invoke(new Action(() =>
                    {
                        labelCHEK_FLK.Text = CountNotFLK.ToString();
                        fileItemBindingSource.ResetItem(i);
                    }));
                }

                SetProggressStatus(0);
                SetTextStatus("");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                item?.FileLog.Close();
            }
            finally
            {
                SetActiveButton();
            }
        }

        private List<ErrorProtocolXML> CheckFLK(FileItem fi, ZL_LIST zl)
        {
            var ErrList = new List<ErrorProtocolXML>();
            try
            {
                var dtSelect = new DateTime(selectYear, selectMonth, 1).AddMonths(1);
                var dtNow = DateTime.Now;
                var dtFile = new DateTime(Convert.ToInt32(zl.SCHET.YEAR), Convert.ToInt32(zl.SCHET.MONTH), 1).AddMonths(1);

                decimal SUMMAP_S = 0;
                decimal SUMMAV_S = 0;
                decimal SANK_MEE_S = 0;
                decimal SANK_EKMP_S = 0;
                decimal SANK_MEK_S = 0;


                if (selectYear >= 2019 && zl.ZGLV.VERSION != "3.1" && FLAG_MEE == 0)
                {
                    ErrList.Add(new ErrorProtocolXML
                    {
                        BAS_EL = "VERSION", Comment = "Версия взаимодействия для реестров 2019 года не 3.1",
                        IM_POL = "VERSION", OSHIB = 41
                    });
                }

                foreach (var zap in zl.ZAP)
                {
                    var N_ZAP = zap.N_ZAP.ToString();
                    foreach (var z_sl in zap.Z_SL_list)
                    {
                        SUMMAP_S += z_sl.SUMP ?? 0;
                        SUMMAV_S += z_sl.SUMV;
                        var sank_it = z_sl.SANK_IT ?? 0;
                        var oplata = z_sl.OPLATA ?? 0;
                        var sump = z_sl.SUMP ?? 0;

                        //Проверка сумм законченного случая
                        if (FLAG_MEE == 0)
                        {

                            if (z_sl.SUMV - sump - sank_it != 0)
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH", IDCASE = z_sl.IDCASE.ToString(), N_ZAP = N_ZAP,
                                    Comment = "Не соответствие SUMV-SUMP-SANK_IT = 0", IM_POL = "SANK_IT", OSHIB = 41
                                });
                            }

                            if (!(sank_it == 0 && oplata == 1 && z_sl.SANK.Count==0 || sank_it == z_sl.SUMV && oplata == 2 || oplata == 3))
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH", IDCASE = z_sl.IDCASE.ToString(), N_ZAP = N_ZAP,
                                    Comment = "не верное заполнение OPLATA", IM_POL = "OPLATA", OSHIB = 41
                                });
                            }

                            if (z_sl.SANK.Sum(x => x.S_SUM) != sank_it)
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH", IDCASE = z_sl.IDCASE.ToString(), N_ZAP = N_ZAP,
                                    Comment = "Не соответствие SANK_IT сумме санкций", IM_POL = "SANK_IT", OSHIB = 41
                                });
                            }
                        }

                        foreach (var san in z_sl.SANK)
                        {
                            var S_TIP_1 = san.S_TIP.ToString().Substring(0, 1);
                            switch (S_TIP_1)
                            {
                                case "1":
                                    SANK_MEK_S += san.S_SUM;
                                    break;
                                case "2":
                                    SANK_MEE_S += san.S_SUM;
                                    break;
                                case "3":
                                case "4":
                                    SANK_EKMP_S += san.S_SUM;
                                    break;
                            }




                            var ce = san.CODE_EXP ?? new List<CODE_EXP>();
                            var ce_count = 0;
                            foreach (var exp in ce.Where(x => !string.IsNullOrEmpty(x.VALUE)))
                            {
                                ce_count++;
                                if (EXPERTS.Select($"N_EXPERT = '{exp.VALUE}'").Length == 0)
                                {
                                    ErrList.Add(new ErrorProtocolXML
                                    {
                                        BAS_EL = "SLUCH", IDCASE = z_sl.IDCASE.ToString(), N_ZAP = N_ZAP,
                                        Comment = $"CODE_EXP = {exp.VALUE} не соответствует справочнику",
                                        IM_POL = "CODE_EXP", OSHIB = 41
                                    });
                                }
                            }

                            if (san.S_TIP > 30 && ce_count == 0 && san.S_OSN != 43 && selectYear >= 2019)
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH", IDCASE = z_sl.IDCASE.ToString(), N_ZAP = N_ZAP,
                                    Comment ="Для санкций ЭКМП поле CODE_EXP обязательно к заполнению, кроме S_OSN <> 43",
                                    IM_POL = "CODE_EXP", OSHIB = 41
                                });
                            }


                            if (san.S_TIP.In(43, 44, 45, 46, 47, 48, 49) && san.CODE_EXP.Count <= 1)
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH", IDCASE = z_sl.IDCASE.ToString(), N_ZAP = N_ZAP,
                                    Comment =$"Для санкций с S_TIP = {{43,44,45,46,47,48,49}} количество CODE_EXP должно быть более 1",
                                    IM_POL = "CODE_EXP", OSHIB = 41
                                });
                            }

                            if (F006.Count(x =>x.IDVID == san.S_TIP && san.DATE_ACT >= x.DATEBEG &&san.DATE_ACT <= (x.DATEEND ?? DateTime.Now)) == 0)
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH", IDCASE = z_sl.IDCASE.ToString(), N_ZAP = N_ZAP,
                                    Comment = "Неверный тип санкции", IM_POL = "S_TIP", OSHIB = 41
                                });
                            }


                            if (F014.Count(x =>x.KOD == san.S_OSN && san.DATE_ACT >= x.DATEBEG &&san.DATE_ACT <= (x.DATEEND ?? DateTime.Now)) == 0 &&!(san.S_OSN == 0 && san.S_SUM == 0 && FLAG_MEE == 1))
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH", IDCASE = z_sl.IDCASE.ToString(), N_ZAP = N_ZAP,
                                    Comment = $"Неверный код отказа санкции - {san.S_OSN}", IM_POL = "S_OSN", OSHIB = 41
                                });
                            }

                            if (san.S_IST.ToString() != "1")
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH", IDCASE = z_sl.IDCASE.ToString(), N_ZAP = N_ZAP,
                                    Comment = "Источник санкции не верный", IM_POL = "S_IST", OSHIB = 41
                                });
                            }

                            if (FLAG_MEE == 0 && S_TIP_1 != "1" )
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH", IDCASE = z_sl.IDCASE.ToString(), N_ZAP = N_ZAP,
                                    Comment = "Наличие санкций МЭЭ\\ЭКМП", IM_POL = "S_TIP", OSHIB = 41
                                });
                            }

                            if (FLAG_MEE == 1 && !S_TIP_1.In("2", "3", "4"))
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH", IDCASE = z_sl.IDCASE.ToString(), N_ZAP = N_ZAP,
                                    Comment = "Наличие санкций МЭК", IM_POL = "S_TIP", OSHIB = 41
                                });
                            }


                            if (S_TIP_1 == "1" && san.S_SUM == 0 && z_sl.SUMV != 0)
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH", IDCASE = z_sl.IDCASE.ToString(), N_ZAP = N_ZAP,
                                    Comment = "Санкции МЭК с нулевой стоимостью не допустимы", IM_POL = "S_TIP",
                                    OSHIB = 41
                                });
                            }

                            if (S_TIP_1 == "1" && !san.DATE_ACT.Between(dtSelect, dtNow))
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH",
                                    IDCASE = z_sl.IDCASE.ToString(),
                                    N_ZAP = N_ZAP,
                                    Comment = $"Некорректная дата акта = {san.DATE_ACT:dd-MM-yyyy} ожидается с {dtSelect:dd-MM-yyyy} по {dtNow:dd-MM-yyyy}",
                                    IM_POL = "DATE_ACT",
                                    OSHIB = 41
                                });
                            }

                            if (S_TIP_1.In("2","3","4") && !san.DATE_ACT.Between(dtFile, dtNow))
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH",
                                    IDCASE = z_sl.IDCASE.ToString(),
                                    N_ZAP = N_ZAP,
                                    Comment = $"Некорректная дата акта = {san.DATE_ACT:dd-MM-yyyy} ожидается с {dtFile:dd-MM-yyyy} по {dtNow:dd-MM-yyyy}",
                                    IM_POL = "DATE_ACT",
                                    OSHIB = 41
                                });
                            }

                            if (san.S_TIP.In(10,11,12,24,25,26,39,40,41))
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH",
                                    IDCASE = z_sl.IDCASE.ToString(),
                                    N_ZAP = N_ZAP,
                                    Comment = "S_TIP{10,11,12,24,25,26,39,40,41} не предназначены для использования СМО",
                                    IM_POL = "S_OSN",
                                    OSHIB = 41
                                });
                            }

                        }

                        ErrList.AddRange(z_sl.SANK.GroupBy(x => new {x.S_OSN, x.S_SUM, x.S_TIP}).Where(x => x.Count() > 1).Select(san => new ErrorProtocolXML
                        {
                                BAS_EL = "SLUCH", IDCASE = z_sl.IDCASE.ToString(), N_ZAP = N_ZAP,
                                Comment =
                                    $"Дублирование санкции по полям(S_OSN,S_SUM,S_TIP) S_CODE = {string.Join(",", san.Select(x => x.S_CODE))}",
                                IM_POL = "SANK", OSHIB = 41
                            }));


                        ErrList.AddRange(z_sl.SANK.Where(x => x.S_TIP.In(z_sl.SANK.Where(y => y.S_OSN == 0).Select(y => y.S_TIP).Distinct().ToArray()) && x.S_OSN != 0).Select(san => new ErrorProtocolXML
                            {
                                BAS_EL = "SLUCH", IDCASE = z_sl.IDCASE.ToString(), N_ZAP = N_ZAP,
                                Comment = $"Конфликт S_OSN = 0 и S_OSN!=0 для одного S_TIP для S_CODE = {san.S_CODE}",
                                IM_POL = "SANK", OSHIB = 41
                            }));

                        if (z_sl.SANK.Count(x => x.S_SUM != 0) > 1)
                            ErrList.Add(new ErrorProtocolXML
                            {
                                BAS_EL = "SLUCH", IDCASE = z_sl.IDCASE.ToString(), N_ZAP = N_ZAP,
                                Comment = $"Два и более снятия для случая", IM_POL = "SANK", OSHIB = 41
                            });

                        ErrList.AddRange(z_sl.SANK.GroupBy(x => new {x.S_TIP}).Where(x => x.Count() > 1).Select(san => new ErrorProtocolXML
                            {
                                BAS_EL = "SLUCH", IDCASE = z_sl.IDCASE.ToString(), N_ZAP = N_ZAP,
                                Comment =$"Дублирование санкции по полям(S_TIP) S_CODE = {string.Join(",", san.Select(x => x.S_CODE))}",
                                IM_POL = "SANK", OSHIB = 41
                            }));


                        if (z_sl.SANK.Count == 0 && FLAG_MEE == 1)
                        {
                            ErrList.Add(new ErrorProtocolXML
                            {
                                BAS_EL = "SLUCH", IDCASE = z_sl.IDCASE.ToString(), N_ZAP = N_ZAP,
                                Comment = "Отсутствует санкции МЭЭ\\ЭКМП", IM_POL = "SANK", OSHIB = 41
                            });
                        }



                        decimal SUMP_USL = 0;
                        decimal SUMP_SL = 0;

                        //Проверка случаев

                        foreach (var sl in z_sl.SL)
                        {
                            SUMP_SL += sl.SUM_MP ?? 0;
                            foreach (var usl in sl.USL)
                            {
                                SUMP_USL += usl.SUMP_USL ?? 0;
                            }
                        }

                        if (SUMP_USL != sump && FLAG_MEE == 0)
                        {
                            ErrList.Add(new ErrorProtocolXML
                            {
                                BAS_EL = "SLUCH", IDCASE = z_sl.IDCASE.ToString(), N_ZAP = N_ZAP,
                                Comment = "SUMP_USL!=sump", IM_POL = "SUMP", OSHIB = 41
                            });
                        }

                        if (SUMP_SL != sump && FLAG_MEE == 0)
                        {
                            ErrList.Add(new ErrorProtocolXML
                            {
                                BAS_EL = "SLUCH", IDCASE = z_sl.IDCASE.ToString(), N_ZAP = N_ZAP,
                                Comment = "SUMP_SL!=sump", IM_POL = "SUMP", OSHIB = 41
                            });
                        }

                    }
                }

                var SUMMAP = zl.SCHET.SUMMAP ?? 0;
                var SUMMAV = zl.SCHET.SUMMAV;
                var SANK_MEE = zl.SCHET.SANK_MEE ?? 0;
                var SANK_EKMP = zl.SCHET.SANK_EKMP ?? 0;
                var SANK_MEK = zl.SCHET.SANK_MEK ?? 0;


                if (SUMMAP != Math.Round(SUMMAP_S, 2))
                {
                    ErrList.Add(new ErrorProtocolXML
                    {
                        IM_POL = "SCHET", BAS_EL = "SCHET",
                        Comment = $"Сумма принятая = {SUMMAP}, а файле {Math.Round(SUMMAP_S, 2)}", OSHIB = 41
                    });
                }

                if (SUMMAV != Math.Round(SUMMAV_S, 2))
                {
                    ErrList.Add(new ErrorProtocolXML
                    {
                        IM_POL = "SCHET", BAS_EL = "SCHET",
                        Comment = $"Сумма выставленная = {SUMMAV}, а файле {Math.Round(SUMMAV_S, 2)}", OSHIB = 41
                    });
                }

                if (SANK_MEE != Math.Round(SANK_MEE_S, 2) && mee_sum_validate)
                {
                    ErrList.Add(new ErrorProtocolXML
                    {
                        IM_POL = "SCHET", BAS_EL = "SCHET",
                        Comment = $"Сумма МЕЕ = {SANK_MEE}, а файле {Math.Round(SANK_MEE_S, 2)}", OSHIB = 41
                    });
                }

                if (SANK_EKMP != Math.Round(SANK_EKMP_S, 2) && mee_sum_validate)
                {
                    ErrList.Add(new ErrorProtocolXML
                    {
                        IM_POL = "SCHET", BAS_EL = "SCHET",
                        Comment = $"Сумма ЭКМП = {SANK_EKMP}, а файле {Math.Round(SANK_EKMP_S, 2)}", OSHIB = 41
                    });
                }

                if (SANK_MEK != Math.Round(SANK_MEK_S, 2))
                {
                    ErrList.Add(new ErrorProtocolXML
                    {
                        IM_POL = "SCHET", BAS_EL = "SCHET",
                        Comment = $"Сумма МЭК = {SANK_MEK}, а файле {Math.Round(SANK_MEK_S, 2)}", OSHIB = 41
                    });
                }

                if (FLAG_MEE == 0 && fi.DOP_REESTR == true)
                {
                    var db = CreateMyBD();
                    var t = db.GetZGLV_BYFileName(zl.ZGLV.FILENAME);
                    if (t.Rows.Count != 0)
                    {
                        ErrList.Add(new ErrorProtocolXML
                        {
                            IM_POL = "SCHET", BAS_EL = "SCHET",
                            Comment = $"Имя файла присутствует в БД: {zl.ZGLV.FILENAME}", OSHIB = 41
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                ErrList.Add(new ErrorProtocolXML{Comment = $"Ошибка при проверке файла: {ex.StackTrace} {ex.Message}", OSHIB = 41});
                fi.InvokeComment("Ошибка при проверке файла: " + ex.Message, this);
               
            }
            return ErrList;
        }

        private Dictionary<string, List<FindSluchItem>> IDENT_INFO { get; set; } = new Dictionary<string, List<FindSluchItem>>();

        private List<FindSluchItem> GetIDENT_INFO( FileItem fi,ZL_LIST zl, MYBDOracleNEW bd)
        {
            if (!IDENT_INFO.ContainsKey(fi.FileName))
                IDENT_INFO.Add(fi.FileName, bd.Get_IdentInfo(zl, fi, this));

            return IDENT_INFO[fi.FileName];
        }
        private List<ErrorProtocolXML> CheckFLKEx(FileItem fi, ZL_LIST zl, MYBDOracleNEW bd)
        {
            var ErrList = new List<ErrorProtocolXML>();
            try
            {
                if (fi.DOP_REESTR == true && FLAG_MEE==0) return ErrList;
                if (bd.IdentySluch(zl, fi, this, GetIDENT_INFO(fi,zl, bd)))
                {
                    fi.InvokeComment("Обработка пакета: Запрос санкций", this);
                    var SANK = bd.GetSank(zl, fi, this);
                    foreach (var zs_sl in zl.ZAP.SelectMany(x => x.Z_SL_list))
                    {
                        var sank_BD = SANK[Convert.ToInt32(zs_sl.SLUCH_Z_ID)];
                        var isMEK = sank_BD.Count(x => x.S_TIP.ToString().StartsWith("1")) != 0;
                        foreach (var sank in zs_sl.SANK)
                        {
                            var doubleSANK = sank_BD.Where(x => x.S_SUM == sank.S_SUM && x.DATE_ACT == sank.DATE_ACT && x.NUM_ACT == sank.NUM_ACT && x.S_TIP == sank.S_TIP).ToList();
                            if (doubleSANK.Count != 0)
                            {
                                var error = $"Санкция была загружена ранее: {Environment.NewLine} {string.Join(Environment.NewLine, doubleSANK.Select(x => $"S_TIP={x.S_TIP}, S_SUM={x.S_SUM}, DATE_ACT={x.DATE_ACT:dd-MM-yyyy}, NUM_ACT={x.NUM_ACT} загружен {x.DATE_INVITE:dd-MM-yyyy} отчетный период {x.MONTH_SANK} {x.YEAR_SANK}"))}";
                                ErrList.Add(new ErrorProtocolXML { BAS_EL = "Z_SL", IDCASE = zs_sl.IDCASE.ToString(), IM_POL = "SANK", Comment = error });
                            }
                            if (sank.S_TIP == 42)
                            {
                                if (sank_BD.Count(x => x.S_TIP >= 20 && x.S_TIP < 30 && x.S_OSN != 0) == 0 && zs_sl.SANK.Count(x => x.S_TIP >= 20 && x.S_TIP < 30 && x.S_OSN != 0) == 0)
                                {
                                    //var error = "Случай c экспертизой S_TIP=43 не содержит МЭЭ с дефектами";
                                    //ErrList.Add(new ErrorProtocolXML { BAS_EL = "Z_SL", IDCASE = zs_sl.IDCASE.ToString(), IM_POL = "SANK", Comment = error });
                                }
                            }

                            if (sank.S_TIP == 37)
                            {
                                if (sank_BD.Count(x => x.S_TIP >= 20 && x.S_TIP < 30) == 0 && zs_sl.SANK.Count(x => x.S_TIP >= 20 && x.S_TIP < 30) == 0)
                                {
                                   // var error = "Случай c экспертизой S_TIP=37 не содержит МЭЭ";
                                    //ErrList.Add(new ErrorProtocolXML { BAS_EL = "Z_SL", IDCASE = zs_sl.IDCASE.ToString(), IM_POL = "SANK", Comment = error });
                                }
                            }
                            if (sank.S_TIP.In(20, 21, 30, 31, 43, 44, 45, 46) && isMEK)
                            {
                                var error = "S_TIP{20, 21, 30, 31, 43, 44, 45, 46} не подлежит применению, если случай снят на МЭК";
                                ErrList.Add(new ErrorProtocolXML { BAS_EL = "Z_SL", IDCASE = zs_sl.IDCASE.ToString(), IM_POL = "SANK", Comment = error });
                            }

                            if (sank.S_SUM != 0)
                            {
                                var DBerr= sank_BD.Where(x => x.S_TIP == sank.S_TIP && x.NUM_ACT == sank.NUM_ACT && x.DATE_ACT == sank.DATE_ACT && x.S_SUM != 0).ToList();
                                var FileErr = zs_sl.SANK.Where(x => x.S_TIP == sank.S_TIP && x.NUM_ACT == sank.NUM_ACT && x.DATE_ACT == sank.DATE_ACT && x.S_SUM != 0 && x.S_CODE!=sank.S_CODE).ToList();
                                if (DBerr.Count != 0 || FileErr.Count != 0)
                                {
                                    var error = $"Не допустимо 2 и более снятия на 1 экспертизе. Источник ошибки: {string.Join(Environment.NewLine, DBerr.Select(x=> $"S_TIP={x.S_TIP}, S_OSN={x.S_OSN}, S_SUM=, NUM_ACT={x.NUM_ACT}, DATE_ACT{x.DATE_ACT:dd-MM-yyyy}"))}{(DBerr.Count!=0? Environment.NewLine:"")}{string.Join(Environment.NewLine, FileErr.Select(x => $"(ФАЙЛ)S_TIP={x.S_TIP}, S_OSN={x.S_OSN}, S_SUM=, NUM_ACT={x.NUM_ACT}, DATE_ACT{x.DATE_ACT:dd-MM-yyyy}"))}";
                                    ErrList.Add(new ErrorProtocolXML { BAS_EL = "Z_SL", IDCASE = zs_sl.IDCASE.ToString(), IM_POL = "SANK", Comment = error });
                                }
                            }


                            if (sank.S_TIP.Like("2", "3", "4") && sank.S_OSN != 0 && sank.DATE_ACT.HasValue)
                            {
                                var act = bd.FindACT(sank.NUM_ACT, sank.DATE_ACT.Value, SMO);
                                if (act.Count != 0)
                                {
                                    var error = $"Данный акт уже присутствует в БД: {string.Join(Environment.NewLine, act.Select(x=>$"NUM_ACT = {x.NUM_ACT}, DATE_ACT = {x.DATE_ACT:dd-MM-yyyy}, дата загрузки {x.DATE_INVITE:dd-MM-yyyy}, имя файла = {x.FILENAME}"))}";
                                    //ErrList.Add(new ErrorProtocolXML { BAS_EL = "Z_SL", IDCASE = zs_sl.IDCASE.ToString(), IM_POL = "SANK", Comment = error });
                                }
                            }
                        }




                        /*var SUMV = zs_sl.SUMV;
                        var S_SUM = zs_sl.SANK.Where(x=>x.).Sum(x => x.S_SUM);
                        var S_SUM_BD = sank_BD.Sum(x => x.S_SUM);
                        var SUMP = SUMV - S_SUM - S_SUM_BD;
                        if (SUMP < 0 && S_SUM!=0)
                        {
                            var error = $"Снятие более суммы случая: сумма случая={SUMV}, сумма санкций в файле={S_SUM}, сумма санкций в базе={S_SUM_BD}, итоговая сумма(после вычета санкций)={SUMP}";
                            ErrList.Add(new ErrorProtocolXML { BAS_EL = "Z_SL", IDCASE = zs_sl.IDCASE.ToString(), IM_POL = "SANK", Comment = error });
                        }*/
                    }
                }
                else
                {
                    ErrList.Add(new ErrorProtocolXML { Comment = "Не полная идентификация случаев" });
                    fi.WriteLnFull("Не полная идентификация случаев");
                }
            }
            catch (Exception ex)
            {
                ErrList.Add(new ErrorProtocolXML { Comment = $"Ошибка при проверке файла: {ex.StackTrace} {ex.Message}" });
            }

            return ErrList;
        }

        private void CreateError(FileItem fi, List<ErrorProtocolXML> ErrList)
        {
            if (ErrList.Count != 0)
            {
                var pathToXml = Path.Combine(Path.GetDirectoryName(fi.FileLog.FilePath), $"{Path.GetFileNameWithoutExtension(fi.FileLog.FilePath)}FLK.xml");
                SchemaChecking.XMLfileFLK(pathToXml, fi.FileName, ErrList);
                foreach (var err in ErrList)
                {
                    fi.FileLog.WriteLn(string.IsNullOrEmpty(err.IDCASE) ? err.Comment : $"IDCASE ={err.IDCASE}: {err.Comment}");
                }
            }
        }
    
        private void общиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new SANK_SMO.SANK_SETTING();
            form.ShowDialog();
            if (File.Exists(Path.Combine(Application.StartupPath, "SANK_INVITER_SCHEMA.dat")))
                scoll.LoadFromFile(Path.Combine(Application.StartupPath, "SANK_INVITER_SCHEMA.dat"));
        
        }

        private void показатьФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (contextMenuStrip1.SourceControl == dataGridView1)
            {
                if (fileItemBindingSource.Current != null)
                {
                    try
                    {
                        ShowSelectedInExplorer.FileOrFolder(((FileItem) fileItemBindingSource.Current).FilePach);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }
            }
            if (contextMenuStrip1.SourceControl == dataGridView2)
            {
                if (fileHBindingSource.Current != null)
                {
                    try
                    {
                        ShowSelectedInExplorer.FileOrFolder(((FileL) fileHBindingSource.Current).FilePach);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }
            }
        }

        private void показатьЛогToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (contextMenuStrip1.SourceControl == dataGridView1)
            {
                if (fileItemBindingSource.Current != null)
                {
                    try
                    {
                        ShowSelectedInExplorer.FileOrFolder(((FileItem) fileItemBindingSource.Current).FileLog.FilePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }
            }
            if (contextMenuStrip1.SourceControl == dataGridView2)
            {
                if (fileHBindingSource.Current != null)
                {
                    try
                    {
                        ShowSelectedInExplorer.FileOrFolder(((FileL) fileHBindingSource.Current).FileLog.FilePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(@"Удалить файлы логов?", "", MessageBoxButtons.YesNo) ==
                System.Windows.Forms.DialogResult.Yes)
            {
                var files = Directory.GetFiles(folder);
                foreach (var item in files)
                {
                    if (File.Exists(item))
                        File.Delete(item);
                }
            }
            IDENT_INFO.Clear();
            dataGridView1.DataSource = null;
            dataGridView2.DataSource = null;
            Files.Clear();
            dataGridView1.DataSource = fileItemBindingSource;
            dataGridView2.DataSource = fileHBindingSource;

            labelCountFLK.Text = @"0";
            labelCountInvite.Text = @"0";
            labelNotInvite.Text = @"0";
            labelSCHEMA_COUNT.Text = @"0";
            labelNOT_ZGLV_ID.Text = @"0";
            labelNotValueDop.Text = @"0";
            labelCHEK_FLK.Text = @"0";
            SetActiveButton();
        }

        private void fileItemBindingSource_ListChanged(object sender, ListChangedEventArgs e)
        {
        }

        Color cEXSD = Color.Yellow;
        Color cNotInvite = Color.LightGray;
        Color cFlkErr = Color.Red;
        Color cLoad = Color.GreenYellow;
        Color c_DOP_NOT_DATA = Color.Orange;
        Color c_ID_NOT_DATA = Color.LightSalmon;
        Color c_CHEK_FLK = Color.Yellow;

        void DataGridRecolor(int x)
        {
            dataGridView1.Invoke(new Action(() =>
                {
                    dataGridView1.Rows[x].DefaultCellStyle.BackColor = dataGridView1.DefaultCellStyle.BackColor;

                    if (Files[x].Process == StepsProcess.ErrorXMLxsd || Files[x].filel?.Process == StepsProcess.ErrorXMLxsd)
                        dataGridView1.Rows[x].DefaultCellStyle.BackColor = cEXSD;
                    if (Files[x].Process == StepsProcess.NotInvite || Files[x].filel?.Process == StepsProcess.NotInvite)
                        dataGridView1.Rows[x].DefaultCellStyle.BackColor = cNotInvite;
                    if (Files[x].Process == StepsProcess.FlkErr || Files[x].filel?.Process == StepsProcess.FlkErr)
                        dataGridView1.Rows[x].DefaultCellStyle.BackColor = cFlkErr;
             

                    if (Files[x].Process == StepsProcess.XMLxsd && Files[x].filel?.Process == StepsProcess.XMLxsd)
                    {
                        if ((!Files[x].ZGLV_ID.HasValue || Files[x].ZGLV_ID == -1) && (Files[x].DOP_REESTR !=true || FLAG_MEE==1))
                            dataGridView1.Rows[x].DefaultCellStyle.BackColor = c_ID_NOT_DATA;
                    }

                    if(Files[x].Process == StepsProcess.FlkOk)
                        dataGridView1.Rows[x].DefaultCellStyle.BackColor = cLoad;
                }));

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
           
        }


        void openAllLogs()
        {
            foreach(var fi  in Files)
            {
                fi.FileLog.Append();
                fi.filel?.FileLog.Append();
            }
        }


        void CloseAllLogs()
        {
            foreach (var fi in Files)
            {
                fi.FileLog.Close();
                fi.filel?.FileLog.Close();
            }
        }

     
        

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void fileItemBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            try
            {
                fileHBindingSource.DataSource = ((FileItem) fileItemBindingSource.Current).filel;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            if (th == null) return;
            if (!th.IsAlive) return;
            if (MessageBox.Show(@"Вы уверены что хотите остановить загрузку?", "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                th.Abort();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (folder == "" || !Directory.Exists(folder))
            {
                MessageBox.Show(@"Укажите директорию логов");
                return;
            }
            ReadSetting();
            var th = new Thread(IdentySchet) {IsBackground = true};
            th.Start();
        }

        int _CountNotZGLV_ID = 0;
        int CountNotZGLV_ID
        {
            get
            {
                return _CountNotZGLV_ID;
            }set
            {
                _CountNotZGLV_ID = value;
                dataGridView1.Invoke(new Action(() =>
                {
                    labelNOT_ZGLV_ID.Text = CountNotZGLV_ID.ToString();
                }));
            }
        }

        private void IdentySchet()
        {
            try
            {
                var str = AppConfig.Property.schemaOracle +(string.IsNullOrEmpty(AppConfig.Property.schemaOracle) ? "" : ".") +AppConfig.Property.xml_h_schet;
                var cmd = new OracleDataAdapter($@"select zglv_id from {str} t where t.code_mo = :code_mo and t.code = :code and t.year_base = :year and nvl(t.dop_flag,0) = :DOP and t.DSCHET = :DSCHET and  t.NSCHET = :NSCHET",new OracleConnection(AppConfig.Property.ConnectionString));

                cmd.SelectCommand.Parameters.Add("CODE_MO", OracleDbType.Varchar2);
                cmd.SelectCommand.Parameters.Add("CODE", OracleDbType.Decimal);
                cmd.SelectCommand.Parameters.Add("YEAR", OracleDbType.Decimal);
                cmd.SelectCommand.Parameters.Add("DOP", OracleDbType.Decimal);
                cmd.SelectCommand.Parameters.Add("DSCHET", OracleDbType.Date);
                cmd.SelectCommand.Parameters.Add("NSCHET", OracleDbType.Varchar2);

                cmd.SelectCommand.Connection.Open();
                SetProggressMAXStatus(Files.Count);

                for (var i = 0; i < Files.Count; i++)
                {
                    var item = Files[i];
                    SetProggressStatus(i);
                    SetTextStatus(item.FileName);
                    if (item.Process != StepsProcess.XMLxsd) continue;

                    if (item.DOP_REESTR == true && FLAG_MEE == 0)
                    {
                        continue;
                    }


                    item.FileLog.Append();
                    var VALUE = SchemaChecking.GetCode_fromXML(item.FilePach, "CODE", "CODE_MO", "YEAR", "DSCHET","NSCHET");

                    cmd.SelectCommand.Parameters["CODE"].Value = Convert.ToInt32(VALUE["CODE"]);
                    cmd.SelectCommand.Parameters["CODE_MO"].Value = VALUE["CODE_MO"];
                    cmd.SelectCommand.Parameters["YEAR"].Value = Convert.ToInt32(VALUE["YEAR"]);
                    cmd.SelectCommand.Parameters["DOP"].Value = item.DOP_REESTR == true ? 1 : 0;
                    cmd.SelectCommand.Parameters["DSCHET"].Value = Convert.ToDateTime(VALUE["DSCHET"]);
                    cmd.SelectCommand.Parameters["NSCHET"].Value = VALUE["NSCHET"];

                    var tbl = new DataTable();
                    cmd.Fill(tbl);

                    if (tbl.Rows.Count == 1)
                    {
                        this.Invoke(new Action(() => { item.ZGLV_ID = Convert.ToInt32(tbl.Rows[0][0]); }));
                    }
                    else
                    {
                        CountNotZGLV_ID++;
                        if (tbl.Rows.Count != 0)
                        {
                            this.Invoke(new Action(() => { item.ZGLV_ID = -1; }));
                        }
                    }
                    item.FileLog.Close();
                    DataGridRecolor(i);
                    dataGridView1.Invoke(new Action(() => { fileItemBindingSource.ResetItem(i); }));
                }
                SetProggressStatus(0);
                cmd.SelectCommand.Connection.Close();
                SetTextStatus("Идентификация завершена");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                SetActiveButton();
            }
        }


   

        private void поискРеестраToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileItemBindingSource.Current != null)
            {
                try
                {
                    var item = fileItemBindingSource.Current as FileItem;
                    var CODE = Convert.ToInt32(SchemaChecking.GetCode_fromXML(item.FilePach, "CODE"));
                    var CODE_MO = Convert.ToInt32(SchemaChecking.GetCode_fromXML(item.FilePach, "CODE_MO"));
                    var YEAR = Convert.ToInt32(SchemaChecking.GetCode_fromXML(item.FilePach, "YEAR"));
                    var f = new SANK_SMO.FindReestr(CODE_MO, CODE, YEAR);
                    if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        item.ZGLV_ID = f.ZGLV_ID;
                        CountNotZGLV_ID = Files.Count(x => !x.ZGLV_ID.HasValue || x.ZGLV_ID==-1);
                        var i = Files.IndexOf(item);
                        DataGridRecolor(i);
                        dataGridView1.Invoke(new Action(() =>
                        {
                            fileItemBindingSource.ResetItem(i);
                        }));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
          
        }

        class F014Row
        {
            public static List<F014Row> Get(IEnumerable<DataRow> rows)
            {
                try
                {
                    return rows.Select(Get).ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка получение F014Row:{ex.Message}", ex);
                }
            }

            private static F014Row Get(DataRow row)
            {
                try
                {
                    var item = new F014Row
                    {
                        KOD = Convert.ToInt32(row["KOD"]), DATEBEG = Convert.ToDateTime(row["DATEBEG"])
                    };
                    if (row["DATEEND"] != DBNull.Value)
                        item.DATEEND = Convert.ToDateTime(row["DATEEND"]);
                    return item;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка получение F014Row:{ex.Message}", ex);
                }
            }

            public int KOD { get; set; }
            public DateTime DATEBEG { get; set; }
            public DateTime? DATEEND { get; set; }
        }

        class F006Row
        {
            public static List<F006Row> Get(IEnumerable<DataRow> rows)
            {
                try
                {
                    return rows.Select(row => Get(row)).ToList();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка получение F006Row:{ex.Message}", ex);
                }
            }

            public static F006Row Get(DataRow row)
            {
                try
                {
                    var item = new F006Row();
                    item.IDVID = Convert.ToInt32(row["IDVID"]);
                    item.DATEBEG = Convert.ToDateTime(row["DATEBEG"]);
                    if (row["DATEEND"] != DBNull.Value)
                        item.DATEEND = Convert.ToDateTime(row["DATEEND"]);
                    return item;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка получение F006Row:{ex.Message}", ex);
                }
            }

            public int IDVID { get; set; }
            public DateTime DATEBEG { get; set; }
            public DateTime? DATEEND { get; set; }
        }

        List<F014Row> F014 = new List<F014Row>();
        List<F006Row> F006 = new List<F006Row>();
        DataTable EXPERTS = new DataTable();
        string SMO = "";
        bool mee_sum_validate = true;
        private void button6_Click(object sender, EventArgs e)
        {
            ReadSetting();

            if (SMO == "")
            {
                MessageBox.Show("SMO");
                return;
            }
            var tbl = new DataTable();
            var oda = new OracleDataAdapter(@" select f6.idvid,dateend, datebeg from nsi.f006 f6", AppConfig.Property.ConnectionString);
            oda.Fill(tbl);
            F006 = F006Row.Get(tbl.Select());

            tbl = new DataTable();
            oda = new OracleDataAdapter(@"   select KOD, dateend, datebeg from nsi.f014 f14", AppConfig.Property.ConnectionString);
            oda.Fill(tbl);
            F014 = F014Row.Get(tbl.Select());

            EXPERTS.Rows.Clear();
            oda = new OracleDataAdapter(@"   select N_EXPERT from nsi.EXPERTS f14", AppConfig.Property.ConnectionString);
            oda.Fill(EXPERTS);

            var th = new Thread(CheckFLK_BASE) { IsBackground = true };
            th.Start();

        }


        private void установитьснятьФлагДопРеестрToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var items = dataGridView1.SelectedCells.Cast<DataGridViewCell>().Select(x => x.OwningRow)
                    .Select(x => (FileItem)x.DataBoundItem).Distinct().ToList();
                if (items.Count == 0) return;

                foreach (var item in items)
                {
                    item.DOP_REESTR = item.DOP_REESTR.HasValue ? !item.DOP_REESTR : true;
                    var i = Files.IndexOf(item);
                    DataGridRecolor(i);
                    dataGridView1.Invoke(new Action(() => { fileItemBindingSource.ResetItem(i); }));
                }

                countNOT_DOP = Files.Count(x => !x.DOP_REESTR.HasValue);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBoxSMO.Text = comboBox1.SelectedIndex == 0 ? "75001" : "75003";
        }

        private void добавитьФайлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folder == "" || !Directory.Exists(folder))
            {
                MessageBox.Show(@"Укажите директорию логов");
                return;
            }
            TI = Type_Invite.Type2L;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {

                foreach (var path in openFileDialog1.FileNames)
                {
                    var name = Path.GetFileName(path);
                    var file = ParseFileName.Parse(name);

                    var item = new FileItem
                    {
                        Tag = 2,
                        DateCreate = DateTime.Now,
                        FileName = name,
                        FilePach = path,
                        FileLog = new LogFile(Path.Combine(folder, Path.GetFileNameWithoutExtension(name) + ".log"))
                    };
                    Files.Add(item);
                 
                    if (file.IsNull)
                    {
                        item.Process = StepsProcess.NotInvite;
                        item.Comment = "Имя файла не корректно";
                        item.FileLog.WriteLn("Имя файла не корректно");
                        CreateErrorByComment(item);
                    }
                    else
                    {
                        item.Process = StepsProcess.Invite;
                        item.Comment = "Имя файла корректно";
                        item.FileLog.WriteLn("Имя файла корректно");
                    }
                    fileItemBindingSource.ResetBindings(false);
                    DataGridRecolor(Files.Count - 1);
                    if (file.FILE_TYPE != null)
                        item.Type = file.FILE_TYPE.ToFileType();
                    item.FileLog.Close();

                }
                labelCount.Text = Files.Count.ToString();
                FindL();
            }
            SetActiveButton();
        }



        private void FindL()
        {
            //проверка на файл L 
            openAllLogs();
            for (var i = 0; i < Files.Count; i++)
            {
                var fi = Files[i];
                var findfile = fi.FileName;
                switch (fi.Type)
                {
                    case FileType.DD:
                    case FileType.DF:
                    case FileType.DO:
                    case FileType.DP:
                    case FileType.DR:
                    case FileType.DS:
                    case FileType.DU:
                    case FileType.DV:
                    case FileType.H:

                        findfile = "L" + findfile.Remove(0, 1);
                        break;
                    case FileType.T:
                    case FileType.C:
                        findfile = "L" + findfile;
                        break;
                    default:
                        continue;
                }

                var x = Files.FindIndex(F => F.FileName == findfile);
                if (x != -1)
                {
                    fi.FileLog.WriteLn("Контроль: Файл персональных данных присутствует");
                    var h = new FileL
                    {
                        Process = Files[x].Process,
                        FileLog = Files[x].FileLog,
                        FileName = Files[x].FileName,
                        FilePach = Files[x].FilePach,
                        DateCreate = Files[x].DateCreate,
                        Tag = Files[x].Tag,
                        Type = Files[x].Type,
                        Comment = Files[x].Comment
                    };
                    fi.filel = h;
                    fi.filel.FileLog.WriteLn("Контроль: Файл владелец присутствует (" + fi.FileName + ")");
                    Files.Remove(Files[x]);
                    if (x < i) i--;
                }
                else
                {
                    fi.FileLog.WriteLn("Ошибка: Файл персональных данных отсутствует");
                    fi.Process = StepsProcess.NotInvite;
                    fi.Comment = "Ошибка: Файл персональных данных отсутствует";
                    CreateErrorByComment(fi);
                }
            }
            var Rx = 0;
            fileItemBindingSource.ResetBindings(false);
            foreach (var F in Files)
            {
                switch (F.Type)
                {
                    case FileType.LD:
                    case FileType.LF:
                    case FileType.LO:
                    case FileType.LP:
                    case FileType.LR:
                    case FileType.LS:
                    case FileType.LU:
                    case FileType.LV:
                    case FileType.LH:
                    case FileType.LT:

                        F.Process = StepsProcess.NotInvite;
                        F.FileLog.WriteLn("Ошибка: Файл владелец данных отсутствует");
                        F.Comment = ("Ошибка: Файл владелец данных отсутствует");
                        CreateErrorByComment(F);
                        break;
                    default: break;
                }
                DataGridRecolor(Rx);
                Rx++;


            }
            CloseAllLogs();
        }

        private void CreateErrorByComment(FileItemBase fi)
        {
            var ErrList = new List<ErrorProtocolXML>
            {
                new ErrorProtocolXML {BAS_EL = "FILENAME", Comment = fi.Comment, IM_POL = "FILENAME", OSHIB = 41}
            };
            var pathToXml = Path.Combine(Path.GetDirectoryName(fi.FileLog.FilePath),Path.GetFileNameWithoutExtension(fi.FileLog.FilePath) + "FLK.xml");
            SchemaChecking.XMLfileFLK(pathToXml, fi.FileName, ErrList);
        }

        private void radioButton2_Click(object sender, EventArgs e)
        {
            if (radioButtonMEK.Checked)
            {
                checkBoxRewriteSum.Enabled = true;
                checkBoxNotFinish.Enabled = false;
                checkBoxValidate.Enabled = false;
            }
            else
            {
                checkBoxRewriteSum.Enabled = false;
                checkBoxNotFinish.Enabled = true;
                checkBoxValidate.Enabled = true;
            }
        }

   

       
    }


    public static class Ext
    {
        /// <summary>
        /// Проверить находится ли значение в списке значений
        /// </summary>
        /// <param name="value">Значение</param>
        /// <param name="valuesArray">Список значений</param>
        /// <returns></returns>
        public static bool In(this decimal value, params decimal[] valuesArray)
        {
            return valuesArray.Contains(value);
        }

        /// <summary>
        /// Проверить находится ли значение в списке значений
        /// </summary>
        /// <param name="value">Значение</param>
        /// <param name="valuesArray">Список значений</param>
        /// <returns></returns>
        public static bool In(this string value, params string[] valuesArray)
        {
            return valuesArray.Contains(value);
        }

        /// <summary>
        /// Проверить находится ли значение в списке значений
        /// </summary>
        /// <param name="value">Значение</param>
        /// <param name="valuesArray">Список значений</param>
        /// <returns></returns>
        public static bool In(this StepsProcess value, params StepsProcess[] valuesArray)
        {
            return valuesArray.Contains(value);
        }

        public static bool Like(this string val, params string[] vals)
        {
            return vals.Any(s => val.StartsWith(s));
        }

        public static bool Like(this decimal val, params string[] vals)
        {
            return Like(val.ToString(), vals);
        }


        public static bool Between(this DateTime? value, DateTime dt1, DateTime dt2)
        {
            if (!value.HasValue) return false;
            return value >= dt1 && value <= dt2;
        }
    }
}
