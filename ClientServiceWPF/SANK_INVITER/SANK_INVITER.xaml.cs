using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ClientServiceWPF.Class;
using Oracle.ManagedDataAccess.Client;
using ServiceLoaderMedpomData;
using  ServiceLoaderMedpomData;
using ServiceLoaderMedpomData.Annotations;
using ServiceLoaderMedpomData.EntityMP_V31;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace ClientServiceWPF.SANK_INVITER
{
    /// <summary>
    /// Логика взаимодействия для SANK_INVITER.xaml
    /// </summary>
    public partial class SANK_INVITER : Window, INotifyPropertyChanged
    {
      

        SchemaColection scoll = new SchemaColection();
        SchemaChecking sc = new SchemaChecking();
        public List<FileItemEx> Files { get; set; } = new List<FileItemEx>();
        private CollectionViewSource CVSFiles;
        private static string LocalFolder => AppDomain.CurrentDomain.BaseDirectory;
        public SANK_INVITER()
        {
            InitializeComponent();
            CVSFiles = (CollectionViewSource) FindResource("CVSFiles");
          
            if (File.Exists(Path.Combine(LocalFolder, "SANK_INVITER_SCHEMA.dat")))
                scoll.LoadFromFile(Path.Combine(LocalFolder, "SANK_INVITER_SCHEMA.dat"));
            else
                MessageBox.Show(@"Файл схем не найден. Нужно проверить параметры");
        }

        private void this_Loaded(object sender, RoutedEventArgs e)
        {
            LogFolder = Properties.Settings.Default.FOLDER_LOG_SANK;
            TextBoxPathLog.Text = LogFolder;
            DatePickerPeriod.SelectedDate = DateTime.Now.AddMonths(-1);
            ComboBoxSMO.Items.Add("ЗМС");
            ComboBoxSMO.Items.Add("СВ");

            SetActiveButton();
            SetCheckBox();
        }


        public string LogFolder { get; set; }

        OpenFileDialog openFileDialog1 = new OpenFileDialog() {Filter = "Файлы XML(*.xml)|*.xml", Multiselect = true};
        private void MenuItemAddFile_Click(object sender, RoutedEventArgs e)
        {
            if (!IsLogFolder)
            {
                MessageBox.Show(@"Укажите директорию логов");
                return;
            }

            if (openFileDialog1.ShowDialog() == true)
            {

                foreach (var path in openFileDialog1.FileNames)
                {
                    var name = System.IO.Path.GetFileName(path);
                    var file = ParseFileName.Parse(name);

                    var item = new FileItemEx
                    {
                        Tag = 2,
                        DateCreate = DateTime.Now,
                        FileName = name,
                        FilePach = path,
                        FileLog = new LogFile(Path.Combine(LogFolder, $"{Path.GetFileNameWithoutExtension(name)}.log"))
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
                    if (file.FILE_TYPE != null)
                        item.Type = file.FILE_TYPE.ToFileType();
                    item.FileLog.Close();

                }
                FindL();
            }
            SetActiveButton();
            CVSFiles.View.Refresh();


        }
        private bool IsLogFolder => !string.IsNullOrEmpty(LogFolder) && Directory.Exists(LogFolder);
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
                Rx++;


            }
            CloseAllLogs();
        }
        void openAllLogs()
        {
            foreach (var fi in Files)
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
        private void CreateErrorByComment(FileItemBase fi)
        {
            var ErrList = new List<ErrorProtocolXML>
            {
                new ErrorProtocolXML {BAS_EL = "FILENAME", Comment = fi.Comment, IM_POL = "FILENAME", OSHIB = 41}
            };
            var pathToXml = Path.Combine(Path.GetDirectoryName(fi.FileLog.FilePath), Path.GetFileNameWithoutExtension(fi.FileLog.FilePath) + "FLK.xml");
            SchemaChecking.XMLfileFLK(pathToXml, fi.FileName, ErrList);
        }
        private void SetActiveButton()
        {
            this.Dispatcher.Invoke(() =>
            {
                ButtonXSD.IsEnabled = Files.Count(x => x.Process.In(StepsProcess.ErrorXMLxsd, StepsProcess.Invite)) != 0 && Files.Count != 0;
                ButtonFIND.IsEnabled = Files.Count(x => x.Process.In(StepsProcess.XMLxsd)) != 0 && Files.Count != 0;
                ButtonFLK.IsEnabled = Files.Count(x => x.Process.In(StepsProcess.XMLxsd) && (x.ZGLV_ID.HasValue || x.DOP_REESTR == true)) != 0 && Files.Count != 0;
                ButtonToBase.IsEnabled = Files.Count(x => x.Process.In(StepsProcess.XMLxsd) && (x.ZGLV_ID.HasValue || x.DOP_REESTR == true)) == Files.Count && Files.Count != 0;
            });
        }
        private void ButtonXSD_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IsLogFolder)
                {
                    throw  new Exception(@"Укажите директорию логов");
                }
                var set = ReadSetting();
                var th = new Thread(CheckSchema) { IsBackground = true };
                th.Start(set);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public class  Param
        {
            public bool FLAG_MEE { get; set; }
            public  string SMO { get; set; }
            public  int MONTH { get; set; }
            public int YEAR { get; set; }
            public bool RewriteSum { get; set; }
            public bool MEE_SUM_VALIDATE { get; set; }
            public bool NOT_FINISH_SANK { get; set; }
            public bool EXT_FLK { get; set; }
        }
        private Param ReadSetting()
        {
            if(!DatePickerPeriod.SelectedDate.HasValue)
                throw  new Exception("Укажите период обработки");
            var Param = new Param
            {
                FLAG_MEE = radioButtonMEE.IsChecked == true,
                SMO = TextBoxSMO.Text,
                MONTH = DatePickerPeriod.SelectedDate.Value.Month,
                YEAR = DatePickerPeriod.SelectedDate.Value.Year,
                RewriteSum = CheckBoxSUMP.IsChecked == true && CheckBoxSUMP.IsEnabled,
                MEE_SUM_VALIDATE = CheckBoxValidateSumMee.IsChecked == true && CheckBoxValidateSumMee.IsEnabled,
                NOT_FINISH_SANK = CheckBoxNot_Finish.IsChecked == true && CheckBoxNot_Finish.IsEnabled,
                EXT_FLK = CheckBoxEXTFLK.IsChecked == true && CheckBoxEXTFLK.IsEnabled
            };
            if(string.IsNullOrEmpty(Param.SMO))
                throw new Exception("Укажите страховую медицинскую организацию");
            return Param;
        }
       
        string SetTextStatus
        {
            set
            {
                TextBlockTool.Dispatcher.Invoke(() =>
                {
                    TextBlockTool.Text = value;
                });
            }
        }
      

      

        double ProgressStatus
        {
            set
            {
                ProgressBarTool.Dispatcher.Invoke(() =>
                {
                    ProgressBarTool.Value = value;
                });
            }
           
        }

        double ProggressMAX
        {
            set
            {
                ProgressBarTool.Dispatcher.Invoke(() =>
                {
                    ProgressBarTool.Maximum = value;
                });
            }

        }


        public int CountXSDErr
        {
            set { this.Dispatcher.Invoke(() => { LabelXSD.Content = value; }); }
        }

        public int CountDOPErr
        {
            set { this.Dispatcher.Invoke(() => { LabelDop.Content = value; }); }
        }

        private void CheckSchema(object val)
        {
            var set = (Param) val;
            FileItemEx item = null;
            try
            {
                var countSchemaFail = 0;
                var countNOT_DOP = 0;
                CountXSDErr = countSchemaFail;
                CountDOPErr = countNOT_DOP;

                ProggressMAX = Files.Count;
                for (var i = 0; i < Files.Count; i++)
                {
                    bool isSchemaErr = false;
                    bool isDOPErr = false;
                    item = Files[i];
                    var ActivSCOLL = scoll;

                    ProgressStatus = i;
                    SetTextStatus = item.FileName;
                    if (item.Process.In(StepsProcess.NotInvite,StepsProcess.FlkErr, StepsProcess.FlkOk, StepsProcess.XMLxsd) && item.filel?.Process.In(StepsProcess.NotInvite, StepsProcess.FlkErr, StepsProcess.FlkOk, StepsProcess.XMLxsd)==true) continue;
                    item.FileLog.Append();
                    item.filel?.FileLog.Append();
                    var vers_file_l = SchemaChecking.GetCode_fromXML(item.filel.FilePach, "VERSION");
                    var vers_file = SchemaChecking.GetCode_fromXML(item.FilePach, "VERSION");
                    var year = SchemaChecking.GetCode_fromXML(item.FilePach, "YEAR");
                    var month = SchemaChecking.GetCode_fromXML(item.FilePach, "MONTH");
                    var dt_file = new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 1);



                    item.Version = VersionMP.NONE;

                    var sc_file =  ActivSCOLL.FindSchema(vers_file, dt_file, item.Type.Value);
                

                    var sc_filel = ActivSCOLL.FindSchema(vers_file_l, dt_file, item.filel.Type.Value);

                    var item1 = item;
                    Dispatcher.Invoke(() =>
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
                            var pathToXml = Path.Combine(Path.GetDirectoryName(item1.FileLog.FilePath),Path.GetFileNameWithoutExtension(item1.FileLog.FilePath) + "FLK.xml");
                            SchemaChecking.XMLfileFLK(pathToXml, item1.FileName,new List<ErrorProtocolXML> { new ErrorProtocolXML { BAS_EL = "VERSION", Comment = err } });
                            isSchemaErr = true;
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
                            var pathToXml = Path.Combine(Path.GetDirectoryName(item1.filel.FileLog.FilePath),Path.GetFileNameWithoutExtension(item1.filel.FileLog.FilePath) + "FLK.xml");
                            SchemaChecking.XMLfileFLK(pathToXml, item1.filel.FileName,new List<ErrorProtocolXML> { new ErrorProtocolXML { BAS_EL = "VERSION", Comment = err } });
                            isSchemaErr = true;
                        }
                    });

                    //проверка основного файла
                    if (item.Version != VersionMP.NONE)
                    {
                        var res = sc.CheckSchema(item, Path.Combine( LocalFolder,sc_file.Value.Value));
                        if (!item.DOP_REESTR.HasValue)
                        {
                            isDOPErr = true;
                        }
                        Dispatcher.Invoke(() =>
                        {
                            if (res)
                            {
                                item.Process = StepsProcess.XMLxsd;
                                item.Comment = "Схема правильная";
                            }
                            else
                            {
                                item.Comment = "Схема ошибочна";
                                item.Process = StepsProcess.ErrorXMLxsd;
                                isSchemaErr = true;
                            }
                        });
                    }

                    //Проверка файла перс
                    if (item.filel != null)
                    {
                        if (item.filel.Version != VersionMP.NONE)
                        {
                            var res = sc.CheckSchema(item.filel, Path.Combine(LocalFolder,sc_filel.Value.Value));
                            Dispatcher.Invoke(() =>
                            {
                                if (res)
                                {
                                    item.filel.Process = StepsProcess.XMLxsd;
                                    item.filel.Comment = "Схема правильная";
                                }
                                else
                                {
                                    item.filel.Comment = "Схема ошибочна";
                                    item.filel.Process = StepsProcess.ErrorXMLxsd;
                                    isSchemaErr = true;
                                    if (item.Process == StepsProcess.XMLxsd)
                                    {
                                        item.Comment = "Схема ошибочна в файле перс данных";
                                        item.Process = StepsProcess.ErrorXMLxsd;
                                    }
                                    else
                                    {
                                        item.Comment += " Схема ошибочна в файле перс данных";
                                        item.Process = StepsProcess.ErrorXMLxsd;
                                    }
                                }
                            });
                        }
                    }

                    Dispatcher.Invoke(() =>
                    {
                        if (isDOPErr)
                        {
                            countNOT_DOP++;
                            item.SANK_STATUS = enumSANK_STATUS.ERROR_DOP;
                        }
                        else
                        {
                            if (item.SANK_STATUS == enumSANK_STATUS.ERROR_DOP)
                                item.SANK_STATUS = enumSANK_STATUS.NONE;
                        }
                        if (isSchemaErr)
                        {
                            countSchemaFail++;
                            Dispatcher.Invoke(() => { item.SANK_STATUS = enumSANK_STATUS.ERROR_XSD; });
                        }
                        else
                        {
                            if (item.SANK_STATUS == enumSANK_STATUS.ERROR_XSD)
                                item.SANK_STATUS = enumSANK_STATUS.NONE;
                        }
                    });

                    CountXSDErr = countSchemaFail;
                    CountDOPErr = countNOT_DOP;

                    item.FileLog.Close();
                    item.filel?.FileLog.Close();
                }

                ProgressStatus = 0;;
                SetTextStatus = "Проверка на схему завершена";
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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ButtonFIND_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IsLogFolder)
                {
                    throw new Exception(@"Укажите директорию логов");
                }
                var set = ReadSetting();
                var th = new Thread(IdentySchet) {IsBackground = true};
                th.Start(set);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public int CountIDErr
        {
            set { this.Dispatcher.Invoke(() => { LabelNOT_ID.Content = value; }); }
        }
   
        private void IdentySchet(object par)
        {
            try
            {
                int CountNotZGLV_ID = 0;
                CountIDErr = CountNotZGLV_ID;
                var set = (Param) par;
                var str = AppConfig.Property.schemaOracle + (string.IsNullOrEmpty(AppConfig.Property.schemaOracle) ? "" : ".") + AppConfig.Property.xml_h_schet;
                var cmd = new OracleDataAdapter($@"select zglv_id from {str} t where t.code_mo = :code_mo and t.code = :code and t.year_base = :year and nvl(t.dop_flag,0) = :DOP and t.DSCHET = :DSCHET and  t.NSCHET = :NSCHET", new OracleConnection(AppConfig.Property.ConnectionString));

                cmd.SelectCommand.Parameters.Add("CODE_MO", OracleDbType.Varchar2);
                cmd.SelectCommand.Parameters.Add("CODE", OracleDbType.Decimal);
                cmd.SelectCommand.Parameters.Add("YEAR", OracleDbType.Decimal);
                cmd.SelectCommand.Parameters.Add("DOP", OracleDbType.Decimal);
                cmd.SelectCommand.Parameters.Add("DSCHET", OracleDbType.Date);
                cmd.SelectCommand.Parameters.Add("NSCHET", OracleDbType.Varchar2);

                cmd.SelectCommand.Connection.Open();
                ProggressMAX = Files.Count;

                for (var i = 0; i < Files.Count; i++)
                {
                    var item = Files[i];
                    ProgressStatus = i;
                    SetTextStatus = item.FileName;
                    if (item.Process != StepsProcess.XMLxsd) continue;

                    if (item.DOP_REESTR == true && !set.FLAG_MEE)
                    {
                        continue;
                    }


                    item.FileLog.Append();
                    var VALUE = SchemaChecking.GetCode_fromXML(item.FilePach, "CODE", "CODE_MO", "YEAR", "DSCHET", "NSCHET");

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
                        this.Dispatcher.Invoke(() => { item.ZGLV_ID = Convert.ToInt32(tbl.Rows[0][0]); });
                    }
                    else
                    {
                        CountNotZGLV_ID++;
                        Dispatcher.Invoke(() => { item.SANK_STATUS = enumSANK_STATUS.ERROR_ID; });
                        CountIDErr = CountNotZGLV_ID;
                        if (tbl.Rows.Count != 0)
                        {
                            this.Dispatcher.Invoke(() => { item.ZGLV_ID = -1; });
                        }
                    }
                    item.FileLog.Close();
                }
                ProgressStatus = 0;
                cmd.SelectCommand.Connection.Close();
                SetTextStatus = "Идентификация завершена";
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
        public int CountFLKErr
        {
            set { this.Dispatcher.Invoke(() => { LabelFLK.Content = value; }); }
        }
        private void ButtonFLK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IsLogFolder)
                {
                    throw new Exception(@"Укажите директорию логов");
                }
                var set = ReadSetting();
                var th = new Thread(CheckFLK_BASE) { IsBackground = true };
                th.Start(new object[] {set, GetF006(),GetF014(),GetEXPERTS()});
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
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
                        KOD = Convert.ToInt32(row["KOD"]),
                        DATEBEG = Convert.ToDateTime(row["DATEBEG"])
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
        class ExpertRow
        {
            public static List<ExpertRow> Get(IEnumerable<DataRow> rows)
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

            public static ExpertRow Get(DataRow row)
            {
                try
                {
                    var item = new ExpertRow();
                    item.N_EXPERT = Convert.ToString(row["N_EXPERT"]);
                    return item;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка получение ExpertRow:{ex.Message}", ex);
                }
            }

            public string N_EXPERT { get; set; }
        }
        private List<F006Row> GetF006()
        {
            var tbl = new DataTable();
            var oda = new OracleDataAdapter(@"select f6.idvid,dateend, datebeg from nsi.f006 f6", AppConfig.Property.ConnectionString);
            oda.Fill(tbl);
            return F006Row.Get(tbl.Select());
        }
        private List<F014Row> GetF014()
        {
            var tbl = new DataTable();
            var oda = new OracleDataAdapter(@"select KOD, dateend, datebeg from nsi.f014 f14", AppConfig.Property.ConnectionString);
            oda.Fill(tbl);
            return F014Row.Get(tbl.Select());
        }
        private List<ExpertRow> GetEXPERTS()
        {
            var tbl = new DataTable();
            var  oda = new OracleDataAdapter(@"select N_EXPERT from nsi.EXPERTS f14", AppConfig.Property.ConnectionString);
            oda.Fill(tbl);
            return ExpertRow.Get(tbl.Select());
        }


        MYBDOracleNEW CreateMyBD()
        {
            return new MYBDOracleNEW(
                                 AppConfig.Property.ConnectionString,
                                 new TableInfo { TableName = AppConfig.Property.xml_h_zglv, SchemaName = AppConfig.Property.schemaOracle, SeqName = AppConfig.Property.seq_ZGLV },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_schet, SchemaName = AppConfig.Property.schemaOracle, SeqName = AppConfig.Property.seq_SCHET },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_sank, SchemaName = AppConfig.Property.schemaOracle, SeqName = AppConfig.Property.seq_SANK },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_code_exp, SchemaName = AppConfig.Property.schemaOracle, SeqName = "" },
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

        private void CheckFLK_BASE(object obj)
        {
            FileItemEx item = null;
            try
            {
                var set = (Param) ((object[]) obj)[0];
                var F006 = (List<F006Row>)((object[])obj)[1];
                var F014 = (List<F014Row>)((object[])obj)[2];
                var Experts = (List<ExpertRow>)((object[])obj)[3];

                var CountNotFLK = 0;
                ProggressMAX = Files.Count;
                for (var i = 0; i < Files.Count; i++)
                {

                    item = Files[i];
                    ProgressStatus = i;
                    SetTextStatus = $"Проверка файла: {item.FileName}";
                    if (item.Process != StepsProcess.XMLxsd) continue;

                    item.FileLog.Append();

                    item.FileLog.WriteLn($"Чтение файла {item.FileName}");
                    var zl = ZL_LIST.GetZL_LIST(item.Version, item.FilePach);
                    zl.SetSUMP();
                    

                  /*var dicPers = new Dictionary<string, PERS>();
                  foreach (var p in PERS_LIST.LoadFromFile(item.filel.FilePach).PERS)
                  {
                      dicPers.Add(p.ID_PAC, p);
                  }*/

                  var mybd = CreateMyBD();

                    item.InvokeComm("Обработка пакета: Проверка ФЛК(BASE)", this);
                    var flk = CheckFLK(item, zl,  set,F006,F014, Experts);
                    if (set.EXT_FLK)
                    {
                        item.InvokeComm("Обработка пакета: Проверка ФЛК(EXT)", this);
                        flk.AddRange(CheckFLKEx(item, zl, mybd, set));
                    }

                    if (flk.Count != 0)
                    {
                        item.InvokeComm("Обработка пакета: Выгрузка ошибок", this);
                        item.Process = StepsProcess.ErrorXMLxsd;
                        CreateError(item, flk);
                        Dispatcher.Invoke(() => { item.SANK_STATUS = enumSANK_STATUS.ERROR_FLK; });
                        CountNotFLK++;
                    }
                    else
                    {
                        Dispatcher.Invoke(() =>
                        {
                            if (item.SANK_STATUS == enumSANK_STATUS.ERROR_FLK)
                            {
                                item.SANK_STATUS = enumSANK_STATUS.NONE;
                            }
                        });
                    }
                    item.InvokeComm("Обработка пакета: Проверка завершена", this);

                    item.FileLog.Close();

                    CountFLKErr = CountNotFLK;
                }

                ProgressStatus = 0;
                SetTextStatus = "";
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

        private int Age(DateTime sl, DateTime dr)
        {
            var age = sl.Year - dr.Year;
            if (dr > sl.AddYears(-age)) age--;
            return age;
        }
        private List<ErrorProtocolXML> CheckFLK(ServiceLoaderMedpomData.FileItem fi, ZL_LIST zl, /*Dictionary<string, PERS> DicPERS,  */Param set, List<F006Row> F006, List<F014Row> F014, List<ExpertRow> Experts)
        {
            var ErrList = new List<ErrorProtocolXML>();
            try
            {
                var dtSelect = new DateTime(set.YEAR, set.MONTH, 1).AddMonths(1);
                var dtNow = DateTime.Now;
                var dtFile = new DateTime(Convert.ToInt32(zl.SCHET.YEAR), Convert.ToInt32(zl.SCHET.MONTH), 1).AddMonths(1);

                decimal SUMMAP_S = 0;
                decimal SUMMAV_S = 0;
                decimal SANK_MEE_S = 0;
                decimal SANK_EKMP_S = 0;
                decimal SANK_MEK_S = 0;


                if (set.YEAR >= 2019 && zl.ZGLV.VERSION != "3.1" && !set.FLAG_MEE)
                {
                    ErrList.Add(new ErrorProtocolXML
                    {
                        BAS_EL = "VERSION",
                        Comment = "Версия взаимодействия для реестров 2019 года не 3.1",
                        IM_POL = "VERSION",
                        OSHIB = 41
                    });
                }

                foreach (var zap in zl.ZAP)
                {
                    var N_ZAP = zap.N_ZAP.ToString();
                    foreach (var z_sl in zap.Z_SL_list)
                    {
                       // var AGE = Age(z_sl.DATE_Z_2, DicPERS[zap.PACIENT.ID_PAC].DR);

                        SUMMAP_S += z_sl.SUMP ?? 0;
                        SUMMAV_S += z_sl.SUMV;
                        var sank_it = z_sl.SANK_IT ?? 0;
                        var oplata = z_sl.OPLATA ?? 0;
                        var sump = z_sl.SUMP ?? 0;

                        //Проверка сумм законченного случая
                        if (!set.FLAG_MEE)
                        {

                            if (z_sl.SUMV - sump - sank_it != 0)
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH",
                                    IDCASE = z_sl.IDCASE.ToString(),
                                    N_ZAP = N_ZAP,
                                    Comment = "Не соответствие SUMV-SUMP-SANK_IT = 0",
                                    IM_POL = "SANK_IT",
                                    OSHIB = 41
                                });
                            }

                            if (!(sank_it == 0 && oplata == 1 && z_sl.SANK.Count == 0 || sank_it == z_sl.SUMV && oplata == 2 || oplata == 3))
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH",
                                    IDCASE = z_sl.IDCASE.ToString(),
                                    N_ZAP = N_ZAP,
                                    Comment = "не верное заполнение OPLATA",
                                    IM_POL = "OPLATA",
                                    OSHIB = 41
                                });
                            }

                            if (z_sl.SANK.Sum(x => x.S_SUM) != sank_it)
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH",
                                    IDCASE = z_sl.IDCASE.ToString(),
                                    N_ZAP = N_ZAP,
                                    Comment = "Не соответствие SANK_IT сумме санкций",
                                    IM_POL = "SANK_IT",
                                    OSHIB = 41
                                });
                            }
                        }

                      //  var isCOVID = z_sl.SL.Any(x => x.DS1.In("U07.1", "U07.2"));
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
                                if (Experts.Count(x=>x.N_EXPERT == exp.VALUE) == 0)
                                {
                                    ErrList.Add(new ErrorProtocolXML
                                    {
                                        BAS_EL = "SLUCH",
                                        IDCASE = z_sl.IDCASE.ToString(),
                                        N_ZAP = N_ZAP,
                                        Comment = $"CODE_EXP = {exp.VALUE} не соответствует справочнику",
                                        IM_POL = "CODE_EXP",
                                        OSHIB = 41
                                    });
                                }
                            }

                            if (san.S_TIP > 30 && ce_count == 0 && san.S_OSN != 43 && set.YEAR >= 2019)
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH",
                                    IDCASE = z_sl.IDCASE.ToString(),
                                    N_ZAP = N_ZAP,
                                    Comment = "Для санкций ЭКМП поле CODE_EXP обязательно к заполнению, кроме S_OSN <> 43",
                                    IM_POL = "CODE_EXP",
                                    OSHIB = 41
                                });
                            }


                            if (san.S_TIP.In(43, 44, 45, 46, 47, 48, 49) && san.CODE_EXP.Count <= 1)
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH",
                                    IDCASE = z_sl.IDCASE.ToString(),
                                    N_ZAP = N_ZAP,
                                    Comment = $"Для санкций с S_TIP = {{43,44,45,46,47,48,49}} количество CODE_EXP должно быть более 1",
                                    IM_POL = "CODE_EXP",
                                    OSHIB = 41
                                });
                            }

                            if (F006.Count(x => x.IDVID == san.S_TIP && san.DATE_ACT >= x.DATEBEG && san.DATE_ACT <= (x.DATEEND ?? DateTime.Now)) == 0)
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH",
                                    IDCASE = z_sl.IDCASE.ToString(),
                                    N_ZAP = N_ZAP,
                                    Comment = "Неверный тип санкции",
                                    IM_POL = "S_TIP",
                                    OSHIB = 41
                                });
                            }


                            if (F014.Count(x => x.KOD == san.S_OSN && san.DATE_ACT >= x.DATEBEG && san.DATE_ACT <= (x.DATEEND ?? DateTime.Now)) == 0 && !(san.S_OSN == 0 && san.S_SUM == 0 && set.FLAG_MEE))
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH",
                                    IDCASE = z_sl.IDCASE.ToString(),
                                    N_ZAP = N_ZAP,
                                    Comment = $"Неверный код отказа санкции - {san.S_OSN}",
                                    IM_POL = "S_OSN",
                                    OSHIB = 41
                                });
                            }

                            if (san.S_IST.ToString() != "1")
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH",
                                    IDCASE = z_sl.IDCASE.ToString(),
                                    N_ZAP = N_ZAP,
                                    Comment = "Источник санкции не верный",
                                    IM_POL = "S_IST",
                                    OSHIB = 41
                                });
                            }

                            if (!set.FLAG_MEE && S_TIP_1 != "1")
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH",
                                    IDCASE = z_sl.IDCASE.ToString(),
                                    N_ZAP = N_ZAP,
                                    Comment = "Наличие санкций МЭЭ\\ЭКМП",
                                    IM_POL = "S_TIP",
                                    OSHIB = 41
                                });
                            }

                            if (set.FLAG_MEE && !S_TIP_1.In("2", "3", "4"))
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH",
                                    IDCASE = z_sl.IDCASE.ToString(),
                                    N_ZAP = N_ZAP,
                                    Comment = "Наличие санкций МЭК",
                                    IM_POL = "S_TIP",
                                    OSHIB = 41
                                });
                            }


                            if (S_TIP_1 == "1" && san.S_SUM == 0 && z_sl.SUMV != 0)
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH",
                                    IDCASE = z_sl.IDCASE.ToString(),
                                    N_ZAP = N_ZAP,
                                    Comment = "Санкции МЭК с нулевой стоимостью не допустимы",
                                    IM_POL = "S_TIP",
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

                            if (S_TIP_1.In("2", "3", "4") && !san.DATE_ACT.Between(dtFile, dtNow))
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

                            if (san.S_TIP.In(10, 11, 12, 24, 25, 26, 39, 40, 41))
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

                        ErrList.AddRange(z_sl.SANK.GroupBy(x => new { x.S_OSN, x.S_SUM, x.S_TIP }).Where(x => x.Count() > 1).Select(san => new ErrorProtocolXML
                        {
                            BAS_EL = "SLUCH",
                            IDCASE = z_sl.IDCASE.ToString(),
                            N_ZAP = N_ZAP,
                            Comment =$"Дублирование санкции по полям(S_OSN,S_SUM,S_TIP) S_CODE = {string.Join(",", san.Select(x => x.S_CODE))}",
                            IM_POL = "SANK",
                            OSHIB = 41
                        }));


                        ErrList.AddRange(z_sl.SANK.Where(x => x.S_TIP.In(z_sl.SANK.Where(y => y.S_OSN == 0).Select(y => y.S_TIP).Distinct().ToArray()) && x.S_OSN != 0).Select(san => new ErrorProtocolXML
                        {
                            BAS_EL = "SLUCH",
                            IDCASE = z_sl.IDCASE.ToString(),
                            N_ZAP = N_ZAP,
                            Comment = $"Конфликт S_OSN = 0 и S_OSN!=0 для одного S_TIP для S_CODE = {san.S_CODE}",
                            IM_POL = "SANK",
                            OSHIB = 41
                        }));
                       
                        if (z_sl.SANK.GroupBy(x => x.S_TIP.ToTypeExp()).Count(x => x.Count() > 1) != 0)
                            ErrList.Add(new ErrorProtocolXML
                            {
                                BAS_EL = "SLUCH",
                                IDCASE = z_sl.IDCASE.ToString(),
                                N_ZAP = N_ZAP,
                                Comment = $"Два и более снятия для случая",
                                IM_POL = "SANK",
                                OSHIB = 41
                            });

                        ErrList.AddRange(z_sl.SANK.GroupBy(x => new { x.S_TIP }).Where(x => x.Count() > 1).Select(san => new ErrorProtocolXML
                        {
                            BAS_EL = "SLUCH",
                            IDCASE = z_sl.IDCASE.ToString(),
                            N_ZAP = N_ZAP,
                            Comment = $"Дублирование санкции по полям(S_TIP) S_CODE = {string.Join(",", san.Select(x => x.S_CODE))}",
                            IM_POL = "SANK",
                            OSHIB = 41
                        }));


                        if (z_sl.SANK.Count == 0 && set.FLAG_MEE)
                        {
                            ErrList.Add(new ErrorProtocolXML
                            {
                                BAS_EL = "SLUCH",
                                IDCASE = z_sl.IDCASE.ToString(),
                                N_ZAP = N_ZAP,
                                Comment = "Отсутствует санкции МЭЭ\\ЭКМП",
                                IM_POL = "SANK",
                                OSHIB = 41
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

                        if (SUMP_USL != sump && !set.FLAG_MEE)
                        {
                            ErrList.Add(new ErrorProtocolXML
                            {
                                BAS_EL = "SLUCH",
                                IDCASE = z_sl.IDCASE.ToString(),
                                N_ZAP = N_ZAP,
                                Comment = "SUMP_USL!=sump",
                                IM_POL = "SUMP",
                                OSHIB = 41
                            });
                        }

                        if (SUMP_SL != sump && !set.FLAG_MEE)
                        {
                            ErrList.Add(new ErrorProtocolXML
                            {
                                BAS_EL = "SLUCH",
                                IDCASE = z_sl.IDCASE.ToString(),
                                N_ZAP = N_ZAP,
                                Comment = "SUMP_SL!=sump",
                                IM_POL = "SUMP",
                                OSHIB = 41
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
                        IM_POL = "SCHET",
                        BAS_EL = "SCHET",
                        Comment = $"Сумма принятая = {SUMMAP}, а файле {Math.Round(SUMMAP_S, 2)}",
                        OSHIB = 41
                    });
                }

                if (SUMMAV != Math.Round(SUMMAV_S, 2))
                {
                    ErrList.Add(new ErrorProtocolXML
                    {
                        IM_POL = "SCHET",
                        BAS_EL = "SCHET",
                        Comment = $"Сумма выставленная = {SUMMAV}, а файле {Math.Round(SUMMAV_S, 2)}",
                        OSHIB = 41
                    });
                }

                if (SANK_MEE != Math.Round(SANK_MEE_S, 2) && set.MEE_SUM_VALIDATE)
                {
                    ErrList.Add(new ErrorProtocolXML
                    {
                        IM_POL = "SCHET",
                        BAS_EL = "SCHET",
                        Comment = $"Сумма МЕЕ = {SANK_MEE}, а файле {Math.Round(SANK_MEE_S, 2)}",
                        OSHIB = 41
                    });
                }

                if (SANK_EKMP != Math.Round(SANK_EKMP_S, 2) && set.MEE_SUM_VALIDATE)
                {
                    ErrList.Add(new ErrorProtocolXML
                    {
                        IM_POL = "SCHET",
                        BAS_EL = "SCHET",
                        Comment = $"Сумма ЭКМП = {SANK_EKMP}, а файле {Math.Round(SANK_EKMP_S, 2)}",
                        OSHIB = 41
                    });
                }

                if (SANK_MEK != Math.Round(SANK_MEK_S, 2))
                {
                    ErrList.Add(new ErrorProtocolXML
                    {
                        IM_POL = "SCHET",
                        BAS_EL = "SCHET",
                        Comment = $"Сумма МЭК = {SANK_MEK}, а файле {Math.Round(SANK_MEK_S, 2)}",
                        OSHIB = 41
                    });
                }

                if (!set.FLAG_MEE && fi.DOP_REESTR == true)
                {
                    var db = CreateMyBD();
                    var t = db.GetZGLV_BYFileName(zl.ZGLV.FILENAME);
                    if (t.Rows.Count != 0)
                    {
                        ErrList.Add(new ErrorProtocolXML
                        {
                            IM_POL = "SCHET",
                            BAS_EL = "SCHET",
                            Comment = $"Имя файла присутствует в БД: {zl.ZGLV.FILENAME}",
                            OSHIB = 41
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                ErrList.Add(new ErrorProtocolXML { Comment = $"Ошибка при проверке файла: {ex.StackTrace} {ex.Message}", OSHIB = 41 });
                fi.InvokeComm($"Ошибка при проверке файла: {ex.Message}", this);

            }
            return ErrList;
        }




        private Dictionary<string, List<FindSluchItem>> IDENT_INFO { get; set; } = new Dictionary<string, List<FindSluchItem>>();

        private List<FindSluchItem> GetIDENT_INFO(ServiceLoaderMedpomData.FileItem fi, ZL_LIST zl, MYBDOracleNEW bd)
        {
            if (!IDENT_INFO.ContainsKey(fi.FileName))
                IDENT_INFO.Add(fi.FileName, bd.Get_IdentInfo(zl, fi, this));
            return IDENT_INFO[fi.FileName];
        }
        private List<ErrorProtocolXML> CheckFLKEx(ServiceLoaderMedpomData.FileItem fi, ZL_LIST zl, MYBDOracleNEW bd, Param set)
        {
            var ErrList = new List<ErrorProtocolXML>();
            try
            {
                if (fi.DOP_REESTR == true && !set.FLAG_MEE) return ErrList;
                if (bd.IdentySluch(zl, fi, this, GetIDENT_INFO(fi, zl, bd)))
                {
                    fi.InvokeComm("Обработка пакета: Запрос санкций", this);
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
                                var DBerr = sank_BD.Where(x => x.S_TIP == sank.S_TIP && x.NUM_ACT == sank.NUM_ACT && x.DATE_ACT == sank.DATE_ACT && x.S_SUM != 0).ToList();
                                var FileErr = zs_sl.SANK.Where(x => x.S_TIP == sank.S_TIP && x.NUM_ACT == sank.NUM_ACT && x.DATE_ACT == sank.DATE_ACT && x.S_SUM != 0 && x.S_CODE != sank.S_CODE).ToList();
                                if (DBerr.Count != 0 || FileErr.Count != 0)
                                {
                                    var error = $"Не допустимо 2 и более снятия на 1 экспертизе. Источник ошибки: {string.Join(Environment.NewLine, DBerr.Select(x => $"S_TIP={x.S_TIP}, S_OSN={x.S_OSN}, S_SUM=, NUM_ACT={x.NUM_ACT}, DATE_ACT{x.DATE_ACT:dd-MM-yyyy}"))}{(DBerr.Count != 0 ? Environment.NewLine : "")}{string.Join(Environment.NewLine, FileErr.Select(x => $"(ФАЙЛ)S_TIP={x.S_TIP}, S_OSN={x.S_OSN}, S_SUM=, NUM_ACT={x.NUM_ACT}, DATE_ACT{x.DATE_ACT:dd-MM-yyyy}"))}";
                                    ErrList.Add(new ErrorProtocolXML { BAS_EL = "Z_SL", IDCASE = zs_sl.IDCASE.ToString(), IM_POL = "SANK", Comment = error });
                                }
                            }


                            if (sank.S_TIP.Like("2", "3", "4") && sank.S_OSN != 0 && sank.DATE_ACT.HasValue)
                            {
                                var act = bd.FindACT(sank.NUM_ACT, sank.DATE_ACT.Value, set.SMO);
                                if (act.Count != 0)
                                {
                                    var error = $"Данный акт уже присутствует в БД: {string.Join(Environment.NewLine, act.Select(x => $"NUM_ACT = {x.NUM_ACT}, DATE_ACT = {x.DATE_ACT:dd-MM-yyyy}, дата загрузки {x.DATE_INVITE:dd-MM-yyyy}, имя файла = {x.FILENAME}"))}";
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

        private void CreateError(ServiceLoaderMedpomData.FileItem fi, List<ErrorProtocolXML> ErrList)
        {
            if (ErrList.Count != 0)
            {
                var pathToXml = Path.Combine(Path.GetDirectoryName(fi.FileLog.FilePath), Path.GetFileNameWithoutExtension(fi.FileLog.FilePath) + "FLK.xml");
                SchemaChecking.XMLfileFLK(pathToXml, fi.FileName, ErrList);
                foreach (var err in ErrList)
                {
                    fi.FileLog.WriteLn(string.IsNullOrEmpty(err.IDCASE) ? err.Comment : $"IDCASE ={err.IDCASE}: {err.Comment}");
                }
            }
        }

        private Thread threadInsert;
        private void ButtonToBase_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IsLogFolder)
                {
                    throw new Exception(@"Укажите директорию логов");
                }
                var set = ReadSetting();
                if (MessageBox.Show($@"Загрузить в {new DateTime(set.YEAR, set.MONTH, 1):MMMMMMMMMMMM yyyy}. ТИП: {(!set.FLAG_MEE ? "МЭК" : "МЭЭ\\ЭКМП")}", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    threadInsert = new Thread(Transfer) { IsBackground = true };
                    threadInsert.Start(set);
                  
                }
              
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public int CountInsertErr
        {
            set { this.Dispatcher.Invoke(() => { LabelInsertERR.Content = value; }); }
        }
        public int CountInsert
        {
            set { this.Dispatcher.Invoke(() => { LabelInsert.Content = value; }); }
        }


        private void Transfer(object obj)
        {

            try
            {
                var set = (Param) obj;
                this.Dispatcher.Invoke(() => { ButtonToBase.IsEnabled = false; });
                var ta = CreateMyBD();
                ProggressMAX = Files.Count;


                var countErrTransfer = 0;
                var countTransfer = 0;
                CountInsertErr = countErrTransfer;
                CountInsert = countTransfer;
                for (var i = 0; i < Files.Count; i++)
                {
                    ProgressStatus = i;
                    SetTextStatus = $"Перенос {Files[i].FileName}";

                    var fi = Files[i];
                    try
                    {
                        if (fi.Process != StepsProcess.XMLxsd) continue;
                        if ((!fi.ZGLV_ID.HasValue || fi.ZGLV_ID == -1) && fi.DOP_REESTR == false) continue;

                        fi.FileLog.Append();
                        fi.filel.FileLog.Append();


                        bool rez1;

                        if (fi.DOP_REESTR == true && !set.FLAG_MEE)
                        {
                            rez1 = ToBaseFile(fi, ta,set);
                        }
                        else
                        {
                            var zl = ZL_LIST.GetZL_LIST(fi.Version, fi.FilePach);
                            zl.SetSUMP();
                            fi.FileLog.WriteLn("Чтение файла " + fi.FileName);
                            rez1 = ToBaseFileSANK(fi, zl, ta, set.RewriteSum, set.NOT_FINISH_SANK, set);

                        }
                        if (rez1)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                fi.Process = StepsProcess.FlkOk;
                                fi.filel.Process = StepsProcess.FlkOk;
                                fi.SANK_STATUS = enumSANK_STATUS.INSERTED;
                            });

                            countTransfer++;
                        }
                        else
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                fi.Process = StepsProcess.FlkErr;
                                fi.filel.Process = StepsProcess.FlkErr;
                                fi.SANK_STATUS = enumSANK_STATUS.ERROR_FLK;
                            });

                            countErrTransfer++;
                        }


                        CountInsertErr = countErrTransfer;
                        CountInsert = countTransfer;

                        fi.FileLog.Close();
                        fi.filel.FileLog.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($@"Ошибка при переносе {fi.FileName}: {ex.Message}");
                        if (fi == null) continue;
                        fi.FileLog.Close();
                        fi.filel.FileLog.Close();
                    }
                }
                ProgressStatus = 0;
                SetTextStatus = "Перенос завершен";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                SetActiveButton();
            }
        }
        private bool ToBaseFile(ServiceLoaderMedpomData.FileItem fi, MYBDOracleNEW mybd, Param set)
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
                id = mybd.AddSankZGLV(FILENAME, Convert.ToInt32(CODE), Convert.ToInt32(CODE_MO), set.FLAG_MEE? 1: 0, Convert.ToInt32(YEAR), Convert.ToInt32(MONTH), set.YEAR, set.MONTH, -1, set.SMO, fi.DOP_REESTR ?? false, false);

                zl.SCHET.YEAR_BASE = zl.SCHET.YEAR;
                zl.SCHET.MONTH_BASE = zl.SCHET.MONTH;

                zl.SCHET.YEAR = set.YEAR;
                zl.SCHET.MONTH = set.MONTH;
                zl.SCHET.DOP_FLAG = 1;

                fi.FileLog.WriteLn("Подготовка");
                foreach (var z in zl.ZAP)
                {
                    z.PR_NOV = 1;
                }

                foreach (var p in zl.ZAP.Select(x => x.PACIENT))
                {
                    p.SMO_TFOMS = set.SMO;
                }
                var ZS = zl.ZAP.SelectMany(x => x.Z_SL_list);
                var sanks = ZS.SelectMany(x => x.SANK);
                foreach (var p in sanks)
                {
                    p.S_ZGLV_ID = id;
                }

                fi.FileLog.WriteLn("Загрузка в бд");
                mybd.InsertFile(zl, PERS_LIST.LoadFromFile(fi.filel.FilePach));
                var Z_SL = zl.ZAP.SelectMany(x => x.Z_SL_list).ToList();
                fi.FileLog.WriteLn("Установка заголовков санкций");
                var zsl_ZGLV_count = mybd.UpdateSLUCH_Z_SANK_ZGLV_ID(Z_SL, id);

                if (Z_SL.Count != zsl_ZGLV_count)
                {
                    fi.InvokeComm("Не полное внесение SANK_ZGLV_ID для случаев", this);
                    fi.FileLog.WriteLn($"Не полное внесение SANK_ZGLV_ID для случаев: внесено {zsl_ZGLV_count} из {Z_SL.Count}");
                    mybd.Rollback();
                    return false;
                }

                fi.FileLog.WriteLn("Установка указателя на счет в заголовке санкций");
                var zglv_id = zl.ZGLV.ZGLV_ID.Value;
                mybd.UpdateSankZGLV(id, Convert.ToInt32(zglv_id));

                mybd.Commit();
                fi.InvokeComm("Загрузка завершена", this);
                fi.FileLog.WriteLn("Загрузка завершена");
                return true;
            }
            catch (Exception ex)
            {
                mybd.Rollback();
                fi.FileLog.WriteLn("Ошибка при переносе в БД: " + ex.StackTrace + ex.Message);
                this.Dispatcher.Invoke(() =>
                {
                    fi.Comment = "Ошибка при переносе в БД: " + ex.Message;
                });
                return false;
            }

        }
        private bool ToBaseFileSANK(ServiceLoaderMedpomData.FileItem fi, ZL_LIST zl, MYBDOracleNEW mybd, bool IsRewrite, bool isNotFinish, Param set)
        {
            mybd.BeginTransaction();
            try
            {
                var EL = SchemaChecking.GetCode_fromXML(fi.FilePach, "FILENAME", "CODE", "CODE_MO", "YEAR", "MONTH");
                //Заголовок санкций
                var id = mybd.AddSankZGLV(EL["FILENAME"], Convert.ToInt32(EL["CODE"]), Convert.ToInt32(EL["CODE_MO"]), set.FLAG_MEE? 1: 0, Convert.ToInt32(EL["YEAR"]), Convert.ToInt32(EL["MONTH"]), set.YEAR, set.MONTH, fi.ZGLV_ID.Value, set.SMO, fi.DOP_REESTR ?? false, isNotFinish);


                var rez = mybd.LoadSANK(fi, zl, id, !set.FLAG_MEE, IsRewrite, this, GetIDENT_INFO(fi, zl, mybd));
                if (rez)
                {
                    fi.InvokeComm("Загрузка завершена", this);
                    fi.FileLog.WriteLn("Загрузка завершена");
                    mybd.Commit();
                }
                else
                {
                    fi.InvokeComm("Ошибка загрузки", this);
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

        private void MenuItemClear_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(@"Удалить файлы логов?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var files = Directory.GetFiles(LogFolder);
                foreach (var item in files)
                {
                    if (File.Exists(item))
                        File.Delete(item);
                }
            }

            IDENT_INFO.Clear();
            Files.Clear();
            CountDOPErr = 0;
            CountXSDErr = 0;
            CountFLKErr = 0;
            CountIDErr = 0;
            CountInsert = 0;
            CountInsertErr = 0;
            SetActiveButton();
            CVSFiles.View.Refresh();
        }

        private void ButtonBreakInsert_Click(object sender, RoutedEventArgs e)
        {
            if (threadInsert == null) return;
            if (!threadInsert.IsAlive) return;
            if (MessageBox.Show(@"Вы уверены что хотите остановить загрузку?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                threadInsert.Abort();
            }
        }

        private List<FileItemEx> selectedFileItems => DataGridFiles.SelectedCells.Select(x => (FileItemEx) x.Item).Distinct().ToList();


        private void MenuItemShowFile_Click(object sender, RoutedEventArgs e)
        {
            var selected = selectedFileItems;
            if (selected.Count != 0)
            {
                try
                {
                    ShowSelectedInExplorer.FilesOrFolders(selected.Select(x=>x.FilePach));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void MenuItemShowLog_Click(object sender, RoutedEventArgs e)
        {
            var selected = selectedFileItems.Where(x=>x.FileLog!=null).ToList();
            if (selected.Count != 0)
            {
                try
                {
                    ShowSelectedInExplorer.FilesOrFolders(selected.Select(x => x.FileLog.FilePath));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void MenuItemShowFileL_Click(object sender, RoutedEventArgs e)
        {
            var selected = selectedFileItems.Where(x=>x.filel!=null).ToList();
            if (selected.Count != 0)
            {
                try
                {
                    ShowSelectedInExplorer.FilesOrFolders(selected.Select(x => x.filel.FilePach));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void MenuItemShowLogL_Click(object sender, RoutedEventArgs e)
        {
            var selected = selectedFileItems.Where(x=>x.filel!=null).Where(x => x.filel.FileLog != null).ToList();
            if (selected.Count != 0)
            {
                try
                {
                    ShowSelectedInExplorer.FilesOrFolders(selected.Select(x => x.filel.FileLog.FilePath));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void MenuItemSetDOP_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var items = selectedFileItems;
                if (items.Count == 0) return;

                foreach (var item in items)
                {
                    item.DOP_REESTR = item.DOP_REESTR.HasValue ? !item.DOP_REESTR : true;
                    if (item.SANK_STATUS == enumSANK_STATUS.ERROR_DOP)
                        item.SANK_STATUS = enumSANK_STATUS.NONE;
                    if (item.DOP_REESTR == true)
                        item.ZGLV_ID = null;
                }
                

                CountDOPErr = Files.Count(x => !x.DOP_REESTR.HasValue);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MenuItemFindReestr_Click(object sender, RoutedEventArgs e)
        {
            var item = selectedFileItems.FirstOrDefault();
            if (item != null)
            {
                try
                {
                    var CODE = Convert.ToInt32(SchemaChecking.GetCode_fromXML(item.FilePach, "CODE"));
                    var CODE_MO = Convert.ToInt32(SchemaChecking.GetCode_fromXML(item.FilePach, "CODE_MO"));
                    var YEAR = Convert.ToInt32(SchemaChecking.GetCode_fromXML(item.FilePach, "YEAR"));
                    var f = new FindReestr(CODE_MO, CODE, YEAR);
                    if (f.ShowDialog() == true)
                    {
                        item.ZGLV_ID = f.ZGLV_ID;
                        item.DOP_REESTR = false;
                        if (item.SANK_STATUS == enumSANK_STATUS.ERROR_ID)
                            item.SANK_STATUS = enumSANK_STATUS.NONE;
                        CountIDErr = Files.Count(x => !x.ZGLV_ID.HasValue);
                        CountDOPErr = Files.Count(x => !x.DOP_REESTR.HasValue);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

     
        FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
        private void ButtonPathLogBrouse_Click(object sender, RoutedEventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LogFolder = folderBrowserDialog1.SelectedPath;
                Properties.Settings.Default.FOLDER_LOG_SANK = LogFolder;
                Properties.Settings.Default.Save();
                TextBoxPathLog.Text = LogFolder;
            }
        }


        void SetCheckBox()
        {
            if (radioButtonMEK.IsChecked==true)
            {
                CheckBoxSUMP.IsEnabled = true;
                CheckBoxNot_Finish.IsEnabled = false;
                CheckBoxValidateSumMee.IsEnabled = false;
            }
            else
            {
                CheckBoxSUMP.IsEnabled = false;
                CheckBoxNot_Finish.IsEnabled = true;
                CheckBoxValidateSumMee.IsEnabled = true;
            }
        }

     

        private void RadioButtonMEK_OnClick(object sender, RoutedEventArgs e)
        {
            SetCheckBox();
        }

        private void RadioButtonMEE_OnClick(object sender, RoutedEventArgs e)
        {
            SetCheckBox();
        }

        private void ComboBoxSMO_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextBoxSMO.Text = ComboBoxSMO.SelectedIndex == 0 ? "75001" : "75003";
        }
    }


    public enum enumSANK_STATUS
    {
        NONE,
        ERROR_XSD,
        ERROR_DOP,
        ERROR_ID,
        ERROR_FLK,
        ERROR_INSERT,
        INSERTED
    }

    public class FileItemEx : ServiceLoaderMedpomData.FileItem
    {
        private enumSANK_STATUS sank_status;
        public enumSANK_STATUS SANK_STATUS
        {
            get { return sank_status;}
            set { sank_status = value;PropChange("SANK_STATUS"); }
        }
    }
    public enum enumTypeEXP
    {
        MEK,
        MEE,
        EKMP
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

        public static void InvokeComm(this ServiceLoaderMedpomData.FileItem item, string COMM, Window win)
        {
            win.Dispatcher?.Invoke(()=>
            {
                item.Comment = COMM;
            });
        }


        public static enumTypeEXP ToTypeExp(this decimal val)
        {
            switch (val.ToString().Substring(0,1))
            {
                case "1": return enumTypeEXP.MEK;
                case "2": return enumTypeEXP.MEE;
                default:
                    return enumTypeEXP.EKMP;
            }
        }



    }
}
