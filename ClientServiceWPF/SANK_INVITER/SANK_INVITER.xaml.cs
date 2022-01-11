using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Threading;
using ClientServiceWPF.Class;
using Oracle.ManagedDataAccess.Client;
using ServiceLoaderMedpomData;
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
        public SANK_INVITERVM VM { get; set; } 
        public List<SMO_ITEM> SMO_LIST { get; set; } = new List<SMO_ITEM>
        {
            new SMO_ITEM{NAME = "ЗМС", SMO_COD = "75001"},
            new SMO_ITEM{NAME = "СВ", SMO_COD = "75003"}
        };
        private static string LocalFolder => AppDomain.CurrentDomain.BaseDirectory;
        public SANK_INVITER()
        {
            CreateVM();
            VM.Param.FLAG_MEE = true;
            VM.Param.EXT_FLK = true;
            InitializeComponent();
           
        }


        private void CreateVM()
        {
            var scoll = new SchemaCollection();
            if (File.Exists(Path.Combine(LocalFolder, "SANK_INVITER_SCHEMA.dat")))
                scoll.LoadFromFile(Path.Combine(LocalFolder, "SANK_INVITER_SCHEMA.dat"));
            else
                MessageBox.Show(@"Файл схем не найден. Нужно проверить параметры");

            var dispatcher = Dispatcher.CurrentDispatcher;
            var sANK_INVITERRepository = new SANK_INVITERRepository(AppConfig.Property.ConnectionString, $"{AppConfig.Property.schemaOracle}{(string.IsNullOrEmpty(AppConfig.Property.schemaOracle) ? "" : ".")}{AppConfig.Property.xml_h_schet}");
            var repository = CreateMyBD();
            var checkerFLK = new CheckerFLK(dispatcher, repository);
            var checkSchema =  new CheckSchema(Dispatcher.CurrentDispatcher, scoll);
            var toBaseFile = new ToBaseFileSank(dispatcher, repository);
            VM = new SANK_INVITERVM(dispatcher, sANK_INVITERRepository, checkerFLK, repository, checkSchema, toBaseFile);
        }

        private void this_Loaded(object sender, RoutedEventArgs e)
        {
            VM.Param.LogFolder = Properties.Settings.Default.FOLDER_LOG_SANK;
            VM.Param.PERIOD = DateTime.Now.AddMonths(-1);
            VM.Param.SMO = SMO_LIST.FirstOrDefault()?.SMO_COD;
        }

        IRepository CreateMyBD()
        {
            return new MYBDOracle.MYBDOracle(
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
                                 new TableInfo { TableName = AppConfig.Property.xml_h_mr_usl_n, SchemaName = AppConfig.Property.schemaOracle, SeqName = "" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_sl_lek_pr, SchemaName = AppConfig.Property.schemaOracle, SeqName = "" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_med_dev, SchemaName = AppConfig.Property.schemaOracle, SeqName = "" },


                                 new TableInfo { TableName = "", SchemaName = AppConfig.Property.schemaOracle, SeqName = " " },
                                 DateTime.Now);
        }


     
        public List<FileItemVM> selectedFileItems => DataGridFiles.SelectedCells.Select(x => (FileItemVM)x.Item).Distinct().ToList();
        private void DataGridFiles_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(selectedFileItems));
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }



    public class SANK_INVITERVM : INotifyPropertyChanged
    {
        private ICheckSchema checkSchema;
        private ICheckFLK checkFlk;
        private IRepository repository;
        private ISANK_INVITERRepository sankInviterRepository;
        private Dispatcher dispatcher;
        private IToBaseFile toBaseFile;
        public SANK_INVITERVM(Dispatcher dispatcher, ISANK_INVITERRepository sankInviterRepository, ICheckFLK checkFlk, IRepository repository, ICheckSchema checkSchema, IToBaseFile toBaseFile)
        {
            this.dispatcher = dispatcher;
            this.sankInviterRepository = sankInviterRepository;
            this.checkFlk = checkFlk;
            this.repository = repository;
            this.checkSchema = checkSchema;
            this.toBaseFile = toBaseFile;
        }
     
        public SANK_INVITERParam Param { get; } = new SANK_INVITERParam();
        public ObservableCollection<FileItemVM> FileItems { get; set; } = new ObservableCollection<FileItemVM>();

        #region  Стастистика


        public ProcessActive Processing { get;} = new ProcessActive();

        private CancellationToken StartProcessing()
        {
            Processing.Active = true;
            Processing.cts = new CancellationTokenSource();
            CommandManager.InvalidateRequerySuggested();
            return  Processing.cts.Token;
        }
        private void StopProcessing()
        {
            Processing.Active = false;
            Processing.cts?.Cancel();
            CommandManager.InvalidateRequerySuggested();
        }

        public ICommand BreakCommand => new Command(o =>
        {
            StopProcessing();
        }, o=> Processing.Active);

        public SANK_INVITER_STAT STAT { get; set; } = new SANK_INVITER_STAT();
        private void RefreshStat()
        {
            var ERR_INVITE = 0;
            var ERR_ID = 0;
            var ERR_XSD = 0;
            var ErrDOP = 0;
            var FLK_ERR = 0;
            var ERR_INSERT = 0;
            var OK_INSERT = 0;

            var IsFLKCheck = false;
            var IsInsertCheck = false;
            var IsXSDCheck = false;


            foreach (var item in FileItems)
            {
                if (item.Flag.Invite == false)
                    ERR_INVITE++;
                if (item.Flag.IDErr == true)
                    ERR_ID++;
                if (item.Flag.XSD == false)
                    ERR_XSD++;
                if (item.Flag.ErrDOP == true)
                    ErrDOP++;
                if (item.Flag.FLK_OK == false)
                    FLK_ERR++;
                switch (item.Flag.Inserted)
                {
                    case false:
                        ERR_INSERT++;
                        break;
                    case true:
                        OK_INSERT++;
                        break;
                }
                if (item.Flag.IsFLKCheck)
                    IsFLKCheck = true;
                if (item.Flag.IsInsertCheck)
                    IsInsertCheck = true;
                if (item.Flag.IsXSDCheck)
                    IsXSDCheck = true;

            }
            dispatcher.Invoke(() =>
            {
                STAT.ERR_INVITE = ERR_INVITE;
                STAT.NOT_ID = ERR_ID;
                STAT.ERR_XSD = ERR_XSD;
                STAT.ERR_DOP = ErrDOP;
                STAT.ERR_FLK = FLK_ERR;
                STAT.ERR_INSERT = ERR_INSERT;
                STAT.Inserted = OK_INSERT;
                STAT.IsFLKCheck = IsFLKCheck;
                STAT.IsInsertCheck = IsInsertCheck;
                STAT.IsXSDCheck = IsXSDCheck;

            });
        }
        #endregion




        FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
        public ICommand SelectLogFolderCommand => new Command(o =>
        {
            try
            {
                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    Param.LogFolder = folderBrowserDialog1.SelectedPath;
                    Properties.Settings.Default.FOLDER_LOG_SANK = Param.LogFolder;
                    Properties.Settings.Default.Save();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        });
        private void throwIfNotLogFolder()
        {
            if (!Param.IsLogFolder)
            {
                throw new Exception(@"Укажите директорию логов");
            }
            if (string.IsNullOrEmpty(Param.SMO))
            {
                throw new Exception(@"Укажите СМО");
            }
        }



        #region Добавление файлов
        OpenFileDialog openFileDialog1 = new OpenFileDialog { Filter = "Файлы XML(*.xml)|*.xml", Multiselect = true };
        public ICommand AddFilesCommand => new Command(o =>
        {
            try
            {
                if (!Param.IsLogFolder)
                {
                    MessageBox.Show(@"Укажите директорию логов");
                    return;
                }

                if (openFileDialog1.ShowDialog() == true)
                {
                    var Files = new List<string>();
                    Files.AddRange(openFileDialog1.FileNames);
                    var DelList = Files.Where(path => FileItems.Count(x => x.Item.FilePach == path.ToUpper() || x.Item.filel?.FilePach == path.ToUpper()) != 0).ToList();
                    if (DelList.Count != 0)
                    {
                        MessageBox.Show($"Следующие файлы уже присутствую в выборке и были удалены из списка добавления:{Environment.NewLine}{string.Join(Environment.NewLine, DelList)}");
                        foreach (var del in DelList)
                        {
                            Files.Remove(del);
                        }
                    }

                foreach (var path in Files)
                    {
                        var name = Path.GetFileName(path).ToUpper();
                        var file = ParseFileName.Parse(name);

                        var item = new FileItemVM(new FileItem
                        {
                            DateCreate = DateTime.Now,
                            FileName = name,
                            FilePach = path.ToUpper(),
                            FileLog = new LogFile(Path.Combine(Param.LogFolder, $"{Path.GetFileNameWithoutExtension(name)}.log"))
                        });
                        FileItems.Add(item);

                        if (file.IsNull)
                        {
                            item.Flag.Invite = false;
                            item.Item.CommentAndLog = "Имя файла не корректно";
                            CreateErrorByComment(item.Item);
                        }
                        else
                        {
                            item.Flag.Invite = true;
                            item.Item.CommentAndLog = "Имя файла корректно";
                        }

                        if (file.FILE_TYPE != null)
                            item.Item.Type = file.FILE_TYPE.ToFileType();
                        item.Item.FileLog.Close();
                    }

                    FindL();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                RefreshStat();
            }
        });

        /// <summary>
        /// Открыть все файлы логов
        /// </summary>
        private void openAllLogs()
        {
            foreach (var fi in FileItems)
            {
                fi.Item.FileLog.Append();
                fi.Item.filel?.FileLog.Append();
            }
        }
        /// <summary>
        /// Закрыть все файлы логов
        /// </summary>
        private void CloseAllLogs()
        {
            foreach (var fi in FileItems)
            {
                fi.Item.FileLog.Close();
                fi.Item.filel?.FileLog.Close();
            }
        }
        /// <summary>
        /// Поиск файлов L в коллекции
        /// </summary>
        private void FindL()
        {
            //проверка на файл L 
            openAllLogs();
            for (var i = 0; i < FileItems.Count; i++)
            {
                var fiVM = FileItems[i];
                var fi = fiVM.Item;
                if(fi.filel!=null) continue;
                
                var findfile = fi.FileName;
                switch (fi.Type)
                {
                    case FileType.DA:
                    case FileType.DB:
                    case FileType.DD:
                    case FileType.DF:
                    case FileType.DO:
                    case FileType.DP:
                    case FileType.DR:
                    case FileType.DS:
                    case FileType.DU:
                    case FileType.DV:
                    case FileType.H:
                        findfile = $"L{findfile.Remove(0, 1)}";
                        break;
                    case FileType.T:
                    case FileType.C:
                        findfile = $"L{findfile}";
                        break;
                    default:
                        continue;
                }

                var LFile = FileItems.FirstOrDefault(F => F.Item.FileName == findfile);

                if (LFile != null)
                {
                    fi.FileLog.WriteLn("Контроль: Файл персональных данных присутствует");
                    var l = new FileL
                    {
                        Process = LFile.Item.Process,
                        FileLog = LFile.Item.FileLog,
                        FileName = LFile.Item.FileName,
                        FilePach = LFile.Item.FilePach,
                        DateCreate = LFile.Item.DateCreate,
                        Type = LFile.Item.Type,
                        Comment = LFile.Item.Comment
                    };
                    fi.filel = l;
                    fi.filel.FileLog.WriteLn($"Контроль: Файл владелец присутствует ({fi.FileName})");
                    if (FileItems.IndexOf(LFile) < i)
                        i--;
                    FileItems.Remove(LFile);
                    fiVM.Flag.Invite = true;
                }
                else
                {
                    fiVM.Flag.Invite = false;
                    fi.CommentAndLog = "Ошибка: Файл персональных данных отсутствует";
                    CreateErrorByComment(fi);
                }
            }

            foreach (var item in FileItems)
            {
                var F = item.Item;
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
                        item.Flag.Invite = false;
                        F.CommentAndLog = "Ошибка: Файл владелец данных отсутствует";
                        CreateErrorByComment(F);
                        break;
                    default: break;
                }
            }
            CloseAllLogs();
        }
        /// <summary>
        /// Создание сообщения о FILENAME
        /// </summary>
        /// <param name="fi"></param>
        private void CreateErrorByComment(FileItemBase fi)
        {
            var ErrList = new List<ErrorProtocolXML>
            {
                new ErrorProtocolXML {BAS_EL = "FILENAME", Comment = fi.Comment, IM_POL = "FILENAME", OSHIB = 41}
            };
            var pathToXml = Path.Combine(Path.GetDirectoryName(fi.FileLog.FilePath), Path.GetFileNameWithoutExtension(fi.FileLog.FilePath) + "FLK.xml");
            SchemaChecking.XMLfileFLK(pathToXml, fi.FileName, ErrList);
        }

        public ICommand ClearFilesCommand => new Command(o =>
        {
            try
            {
                CloseAllLogs();
                if (MessageBox.Show(@"Удалить файлы логов?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var files = Directory.GetFiles(Param.LogFolder);
                    foreach (var item in files)
                    {
                        if (File.Exists(item))
                            File.Delete(item);
                    }
                }

                FileItems.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                RefreshStat();
            }
        });


        #endregion
        public ProgressItem Progress1 { get; set; } = new ProgressItem();
        #region Проверка схемы файлов

        public ICommand CheckSchemaCommand => new Command(async o =>
        {
            try
            {
                var cancel = StartProcessing();
                throwIfNotLogFolder();
                var items = FileItems.Where(x => x.Flag.IsXSDCheck).ToList();
                Progress1.SetValues(items.Count, 0, "");
                await Task.Run(() =>
                {
                    for (var i = 0; i < items.Count; i++)
                    {
                        cancel.ThrowIfCancellationRequested();
                        var itemVM = items[i];
                        var item = itemVM.Item;
                        dispatcher.Invoke(() => { Progress1.SetTextValue(i, item.FileName); });
                        if (itemVM.Flag.XSD == true) continue;
                        var result = checkSchema.CheckSchemaFileItem(item);
                        var vm = itemVM;
                        dispatcher.Invoke(() =>
                        {
                            vm.Flag.ErrDOP = !item.DOP_REESTR.HasValue;
                            vm.Flag.XSD = result;
                        });
                        RefreshStat();
                    }
                }, cancel);
                Progress1.Clear("Проверка на схему завершена");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                RefreshStat();
                StopProcessing();
            }

        }, o => STAT.IsXSDCheck && !Processing.Active);
        #endregion
        #region Поиск реестров
        public ICommand FindReestrCommand => new Command(async o =>
        {
            try
            {
                var cancel = StartProcessing();
                throwIfNotLogFolder();
                Progress1.SetValues(FileItems.Count, 0, "");
                await Task.Run(() =>
                {
                    try
                    {
                        sankInviterRepository.OpenConnection();
                        for (var i = 0; i < FileItems.Count; i++)
                        {
                            cancel.ThrowIfCancellationRequested();
                            var itemVM = FileItems[i];
                            var item = itemVM.Item;
                            dispatcher.Invoke(() => { Progress1.SetTextValue(i,item.FileName); });

                            if (itemVM.Flag.XSD == false || item.DOP_REESTR == true && !Param.FLAG_MEE) continue;

                            var VALUE = SchemaChecking.GetELEMENTs(item.FilePach, "CODE", "CODE_MO", "YEAR", "DSCHET", "NSCHET");
                            var ID = sankInviterRepository.FindZGLV(VALUE["CODE_MO"], Convert.ToInt32(VALUE["CODE"]), Convert.ToInt32(VALUE["YEAR"]), item.DOP_REESTR == true, Convert.ToDateTime(VALUE["DSCHET"]), VALUE["NSCHET"]);
                            dispatcher.Invoke(() =>
                            {
                                item.ZGLV_ID = ID;
                                itemVM.Flag.IDErr = !ID.HasValue;
                            });
                            RefreshStat();
                        }
                    }
                    finally
                    {
                        sankInviterRepository.CloseConnection();
                    }
                }, cancel);
                Progress1.Clear("Идентификация завершена");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                RefreshStat();
                StopProcessing();
            }

        },o=> !Processing.Active);
        #endregion
        #region ФЛК
        public ICommand CheckFLKCommand => new Command(async o =>
        {
            try
            {
                var cancel = StartProcessing();
                throwIfNotLogFolder();
                Progress1.Text = "Запрос врачей экспертов";
                var Experts = await Task.Run(() => repository.GetEXPERTS(), cancel);

                Progress1.Text = "Запрос F006";
                var F006 = await Task.Run(() => repository.GetF006(), cancel);

                Progress1.Text = "Запрос F014";
                var F014 = await Task.Run(() => repository.GetF014(), cancel);

                var items = FileItems.Where(x => x.Flag.IsFLKCheck).ToList();
                Progress1.SetValues(items.Count, 0, "");
                await Task.Run(() => { CheckFLKFileItems(items,F006, F014, Experts, cancel); }, cancel);
                Progress1.Clear("Проверка ФЛК завершена");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                RefreshStat();
                StopProcessing();
            }

        }, o => STAT.IsFLKCheck && !Processing.Active);

      

        private void CheckFLKFileItems(List<FileItemVM> items,List<F006Row> F006, List<F014Row> F014, List<ExpertRow> Experts,CancellationToken cancel)
        {
            for (var i = 0; i < items.Count; i++)
            {
                cancel.ThrowIfCancellationRequested();
                var itemVM = items[i];
                var item = itemVM.Item;
                dispatcher.Invoke(() => { Progress1.SetTextValue(i, $"Проверка файла: {item.FileName}"); });
                if (itemVM.Flag.XSD != true) continue;

                item.FileLog.Append();
                try
                {
                    
                    item.FileLog.WriteLn($"Чтение файла {item.FileName}");
                    item.InvokeComm("Обработка пакета: Чтение файла", dispatcher);
                    var zl = ZL_LIST.GetZL_LIST(item.Version, item.FilePach);
                    zl.SetSUMP();

                    item.InvokeComm("Обработка пакета: Поиск случаев", dispatcher);
                    itemVM.IDENT_INFO = repository.Get_IdentInfo(zl, item, dispatcher);
                    var flk = checkFlk.CheckFLK(item, zl, Param.YEAR, Param.MONTH, Param.FLAG_MEE, Param.EXT_FLK, Param.SMO, F006, F014, Experts, itemVM.IDENT_INFO);
                    if (flk.Count != 0)
                    {
                        item.InvokeComm("Обработка пакета: Выгрузка ошибок", dispatcher);
                        dispatcher.Invoke(() => { itemVM.Flag.FLK_OK = false; });
                        CreateError(item, flk);
                    }
                    else
                    {
                        dispatcher.Invoke(() => { itemVM.Flag.FLK_OK = true; });
                    }

                    item.InvokeComm("Обработка пакета: Проверка ФЛК завершена", dispatcher);
                    RefreshStat();
                }
                finally
                {
                    item.FileLog.Close();
                }
            }
        }
        private void CreateError(FileItem fi, List<ErrorProtocolXML> ErrList)
        {
            if (ErrList.Count != 0)
            {
                var pathToXml = Path.Combine(Path.GetDirectoryName(fi.FileLog.FilePath), Path.GetFileNameWithoutExtension(fi.FileLog.FilePath) + "FLK.xml");
                SchemaChecking.XMLfileFLK(pathToXml, fi.FileName, ErrList);
                foreach (var err in ErrList)
                {
                    var IDCASE = !string.IsNullOrEmpty(err.IDCASE) ? $"IDCASE={err.IDCASE}" : "";
                    var SLUCH_Z_ID = err.SLUCH_Z_ID.HasValue ? $"SLUCH_Z_ID={err.SLUCH_Z_ID.Value}" : "";
                    var prefix = $"{IDCASE} {SLUCH_Z_ID}".Trim();
                    fi.FileLog.WriteLn($"{prefix}{(string.IsNullOrEmpty(prefix)? "" : ": ")}{err.Comment}");
                }
            }
        }
        #endregion
        #region  Перенос в БД
        public ICommand InsertCommand => new Command(async o =>
        {
            try
            {
                var cancel = StartProcessing();
                throwIfNotLogFolder();
                if (MessageBox.Show($@"Загрузить в {Param.PERIOD:MMMMMMMMMMMM yyyy}. ТИП: {(!Param.FLAG_MEE ? "МЭК" : "МЭЭ\\ЭКМП")}", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    var items = FileItems.Where(x => x.Flag.IsInsertCheck).ToList();
                    Progress1.SetValues(items.Count, 0, "");
                    await Task.Run(()=>
                    {
                        Transfer(items, cancel);
                    }, cancel);
                    Progress1.Clear("Перенос завершен");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                RefreshStat();
                StopProcessing();
            }
        }, o=> STAT.IsInsertCheck && !Processing.Active);
        private void Transfer(List<FileItemVM> items,CancellationToken cancel)
        {
            for (var i = 0; i < items.Count; i++)
            {
                cancel.ThrowIfCancellationRequested();
                var fiVM = items[i];
                var fi = fiVM.Item;
                dispatcher.Invoke(() => { Progress1.SetTextValue(i, $"Перенос {fi.FileName}"); });

                try
                {
                    if (fi.Process != StepsProcess.XMLxsd) continue;
                    if ((!fi.ZGLV_ID.HasValue || fi.ZGLV_ID == -1) && fi.DOP_REESTR == false) continue;

                    fi.FileLog.Append();
                    fi.filel.FileLog.Append();
                    ToBaseFileResult rezult;

                    if (fi.DOP_REESTR == true && !Param.FLAG_MEE)
                    {
                        rezult = toBaseFile.ToBaseFile(fi, Param.FLAG_MEE, Param.YEAR, Param.MONTH, Param.SMO, fi.DOP_REESTR ?? false, Param.NOT_FINISH_SANK);
                    }
                    else
                    {
                        rezult = toBaseFile.ToBaseFileSANK(fi, Param.FLAG_MEE, Param.YEAR, Param.MONTH, Param.SMO, fi.DOP_REESTR ?? false, Param.NOT_FINISH_SANK, Param.RewriteSum, fiVM.IDENT_INFO);
                    }


                    dispatcher.Invoke(() =>
                    {
                        fi.CommentAndLog = rezult.Result ? "Перенос завершен" : $"Ошибка переноса: {rezult.Message}";
                        fiVM.Flag.Inserted = rezult.Result;
                    });
                    fi.FileLog.Close();
                    fi.filel.FileLog.Close();
                    RefreshStat();
                }
                catch (Exception ex)
                {

                    fi.FileLog.Close();
                    fi.filel.FileLog.Close();
                    throw new Exception($@"Ошибка при переносе {fi.FileName}: {ex.Message}", ex);
                }
            }
        }
        #endregion
        #region ContextAction
        public ICommand ShowFileHCommand => new Command(o =>
        {
            try
            {
                var selected = ((List<FileItemVM>)o).Where(x => x.Item.FileLog != null).ToList();
                if (selected.Count != 0)
                {
                    try
                    {
                        ShowSelectedInExplorer.FilesOrFolders(selected.Select(x => x.Item.FilePach));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        public ICommand ShowFileHLogCommand => new Command(o =>
        {
            try
            {
                var selected = (List<FileItemVM>)o;
                if (selected.Count != 0)
                {
                    try
                    {
                        ShowSelectedInExplorer.FilesOrFolders(selected.Select(x => x.Item.FileLog.FilePath));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        public ICommand ShowFileLCommand => new Command(o =>
        {
            try
            {
                var selected = ((List<FileItemVM>)o).Where(x => x.Item.filel != null).Where(x => x.Item.filel.FileLog != null).ToList();
                if (selected.Count != 0)
                {
                    try
                    {
                        ShowSelectedInExplorer.FilesOrFolders(selected.Select(x => x.Item.filel.FilePach));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        public ICommand ShowFileLLogCommand => new Command(o =>
        {
            try
            {
                var selected = ((List<FileItemVM>)o).Where(x => x.Item.filel != null).ToList();
                if (selected.Count != 0)
                {
                    try
                    {
                        ShowSelectedInExplorer.FilesOrFolders(selected.Select(x => x.Item.filel.FileLog.FilePath));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });

        public ICommand FindItemCommand => new Command(o =>
        {
            try
            {
                var item = ((List<FileItemVM>) o).FirstOrDefault();
                if (item != null)
                {

                    var el = SchemaChecking.GetELEMENTs(item.Item.FilePach, "CODE", "CODE_MO", "YEAR");
                    var CODE = Convert.ToInt32(el["CODE"]);
                    var CODE_MO = Convert.ToInt32(el["CODE_MO"]);
                    var YEAR = Convert.ToInt32(el["YEAR"]);
                    var f = new FindReestr(CODE_MO, CODE, YEAR);
                    if (f.ShowDialog() == true)
                    {
                        item.Item.ZGLV_ID = f.ZGLV_ID;
                        item.Flag.IDErr = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                RefreshStat();
            }
        });

        public ICommand SetDOP_REESTRCommand => new Command(o =>
        {
            try
            {
                var items = (List<FileItemVM>)o;
                foreach (var item in items)
                {
                    item.Item.DOP_REESTR = item.Item.DOP_REESTR.HasValue ? !item.Item.DOP_REESTR : true;
                    item.Flag.ErrDOP = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                RefreshStat();
            }
        });
        public ICommand SetValidXSDCommand => new Command(o =>
        {
            try
            {
                var items = (List<FileItemVM>)o;
                foreach (var item in items)
                {
                    item.Flag.XSD = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                RefreshStat();
            }
        });
        public ICommand SetValidFLKCommand => new Command(o =>
        {
            try
            {
                var items = (List<FileItemVM>)o;
                foreach (var item in items)
                {
                    item.Flag.FLK_OK = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                RefreshStat();
            }
        });

        #endregion
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
    public class SANK_INVITER_STAT : INotifyPropertyChanged
    {
        private int _ERR_INVITE;
        public int ERR_INVITE
        {
            get => _ERR_INVITE;
            set { _ERR_INVITE = value; RaisePropertyChanged(); }
        }
        private int _ERR_XSD;
        public int ERR_XSD
        {
            get => _ERR_XSD;
            set { _ERR_XSD = value; RaisePropertyChanged(); }
        }
        private int _ERR_DOP;
        public int ERR_DOP
        {
            get => _ERR_DOP;
            set { _ERR_DOP = value; RaisePropertyChanged(); }
        }
        private int _NOT_ID;
        public int NOT_ID
        {
            get => _NOT_ID;
            set { _NOT_ID = value; RaisePropertyChanged(); }
        }
        private int _ERR_FLK;
        public int ERR_FLK
        {
            get => _ERR_FLK;
            set { _ERR_FLK = value; RaisePropertyChanged(); }
        }
        private int _ERR_INSERT;
        public int ERR_INSERT
        {
            get => _ERR_INSERT;
            set { _ERR_INSERT = value; RaisePropertyChanged(); }
        }
        private int _Inserted;
        public int Inserted
        {
            get => _Inserted;
            set { _Inserted = value; RaisePropertyChanged(); }
        }
        private bool _IsFLKCheck;
        public bool IsFLKCheck
        {
            get => _IsFLKCheck;
            set { _IsFLKCheck = value; RaisePropertyChanged(); }
        }

        private bool _IsInsertCheck;
        public bool IsInsertCheck
        {
            get => _IsInsertCheck;
            set { _IsInsertCheck = value; RaisePropertyChanged(); }
        }

        private bool _IsXSDCheck;
        public bool IsXSDCheck
        {
            get => _IsXSDCheck;
            set { _IsXSDCheck = value; RaisePropertyChanged(); }
        }
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
    public class FileItemVM : INotifyPropertyChanged
    {
        public FileItemVM(FileItem item)
        {
            this._Item = item;
        }
        private FileItem _Item;
        public FileItem Item
        {
            get => _Item;
            set { _Item = value;RaisePropertyChanged(); }
        }

        private Flag _Flag = new Flag();
        public Flag Flag
        {
            get => _Flag;
            set { _Flag = value; RaisePropertyChanged(); }
        }


        public List<FindSluchItem> IDENT_INFO { get;  set; }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
    public class Flag : INotifyPropertyChanged
    {
        private bool? _Invite;
        /// <summary>
        /// Пройдена первичная проверка
        /// </summary>
        public bool? Invite
        {
            get => _Invite;
            set { _Invite = value; RaisePropertyChanged(); }
        }
        private bool? _ErrDOP;
        /// <summary>
        /// Файл не неопределенный(доп или нет доп)
        /// </summary>
        public bool? ErrDOP
        {
            get => _ErrDOP;
            set { _ErrDOP = value; RaisePropertyChanged(); }
        }
        private bool? _XSD;
        /// <summary>
        /// Проверка схемы успешно пройдена
        /// </summary>
        public bool? XSD
        {
            get => _XSD;
            set { _XSD = value; RaisePropertyChanged(); }
        }
        private bool? _IDErr;
        /// <summary>
        /// Не найден ID
        /// </summary>
        public bool? IDErr
        {
            get => _IDErr;
            set { _IDErr = value; RaisePropertyChanged(); }
        }
        private bool? _FLK_OK;
        /// <summary>
        /// Пройдена ли проверка ФЛК
        /// </summary>
        public bool? FLK_OK
        {
            get => _FLK_OK;
            set { _FLK_OK = value; RaisePropertyChanged(); }
        }
        private bool? _Inserted;
        /// <summary>
        /// Внесено ли в БД
        /// </summary>
        public bool? Inserted
        {
            get => _Inserted;
            set { _Inserted = value; RaisePropertyChanged(); }
        }
        /// <summary>
        /// Можно ли проверять на схему
        /// </summary>
        public bool IsXSDCheck => Invite == true && XSD != true;
        /// <summary>
        /// Можно ли проверять ФЛК
        /// </summary>
        public bool IsFLKCheck => XSD == true && IDErr == false && FLK_OK!=true;
        /// <summary>
        /// Можно ли вставлять в БД
        /// </summary>
        public bool IsInsertCheck => XSD == true && IDErr == false && FLK_OK == true && Inserted != true;

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
    public class SANK_INVITERParam : INotifyPropertyChanged
    {
        private bool _FLAG_MEE;
        /// <summary>
        /// Признак МЭЭ\ЭКМП
        /// </summary>
        public bool FLAG_MEE
        {
            get => _FLAG_MEE;
            set
            {
                _FLAG_MEE = value;
                RaisePropertyChanged();
            }
        }
        private string _SMO;
        /// <summary>
        /// СМО
        /// </summary>
        public string SMO
        {
            get => _SMO;
            set
            {
                _SMO = value;
                RaisePropertyChanged();
            }
        }
        private DateTime _PERIOD;
        public DateTime PERIOD
        {
            get => _PERIOD;
            set
            {
                _PERIOD = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(MONTH));
                RaisePropertyChanged(nameof(YEAR));
            }
        }
        public int MONTH => _PERIOD.Month;
        public int YEAR => _PERIOD.Year;
        private bool _RewriteSum = false;
        /// <summary>
        /// Перезаписывать сумму принятую
        /// </summary>
        public bool RewriteSum
        {
            get => _RewriteSum;
            set
            {
                _RewriteSum = value;
                RaisePropertyChanged();
            }
        }
        private bool _NOT_FINISH_SANK;
        /// <summary>
        /// Незавершенные санкции
        /// </summary>
        public bool NOT_FINISH_SANK
        {
            get => _NOT_FINISH_SANK;
            set
            {
                _NOT_FINISH_SANK = value;
                RaisePropertyChanged();
            }
        }
        private bool _EXT_FLK;
        /// <summary>
        /// Расширенный ФЛК
        /// </summary>
        public bool EXT_FLK
        {
            get => _EXT_FLK;
            set
            {
                _EXT_FLK = value;
                RaisePropertyChanged();
            }
        }
        private string _LogFolder;
        /// <summary>
        /// Расширенный ФЛК
        /// </summary>
        public string LogFolder
        {
            get => _LogFolder;
            set
            {
                _LogFolder = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsLogFolder));
            }
        }

        public bool IsLogFolder => !string.IsNullOrEmpty(LogFolder);

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
    public class ProcessActive : INotifyPropertyChanged
    {
        private bool _Active;

        public bool Active
        {
            get => _Active;
            set
            {
                _Active = value;
                RaisePropertyChanged();
            }
        }

        public CancellationTokenSource cts { get; set; }
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }


    public class SMO_ITEM
    {
        public string SMO_COD { get; set; }
        public string NAME { get; set; }
    }


}
