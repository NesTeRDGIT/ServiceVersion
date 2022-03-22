using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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
using ClientServiceWPF.MEK_RESULT;
using ClientServiceWPF.MEK_RESULT.ACTMEK;
using Oracle.ManagedDataAccess.Client;
using ServiceLoaderMedpomData;
using ServiceLoaderMedpomData.Annotations;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace ClientServiceWPF
{
    /// <summary>
    /// Логика взаимодействия для Setting.xaml
    /// </summary>
    public partial class Setting : Window,INotifyPropertyChanged
    {
        public SettingVM VM { get; set; } = new SettingVM(LoginForm.wcf);
        private IWcfInterface wcf => LoginForm.wcf;
        private bool OnlyLocal { get; set; }
        public Setting(bool OnlyLocal)
        {
            InitializeComponent();
            this.OnlyLocal = OnlyLocal;

        }

        void BlockTabs()
        {
            foreach (TabItem tab in TabControl.Items)
            {
                if (tab.IsEnabled)
                {
                    tab.IsSelected = true;
                    break;
                }

            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            VM.LoadParam(OnlyLocal);
            VM.OnClosed += VM_OnClosed;
            BlockTabs();
        }

        private void VM_OnClosed(bool DialogResult)
        {
            AppConfig.Load();
            Properties.Settings.Default.Reload();
            if (!OnlyLocal)
                wcf.LoadProperty();
            this.DialogResult = DialogResult;
            Close();
        }

     
     
        private void ListBoxServerVersionZGLV_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VM.SchemaServerParam.SelectedVersionZGLV = ListBoxServerVersionZGLV.SelectedItems.Cast<string>().ToList();
        }
        private void ListBoxLocalVersionZGLV_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VM.SchemaLocalParam.SelectedVersionZGLV = ListBoxLocalVersionZGLV.SelectedItems.Cast<string>().ToList();
        }


        private void ListBoxServerFileType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VM.SchemaServerParam.SelectedFileType = ListBoxServerFileType.SelectedItems.Cast<FileType>().ToList();
        }
     
      
        private void ListBoxLocalFileType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VM.SchemaLocalParam.SelectedFileType = ListBoxLocalFileType.SelectedItems.Cast<FileType>().ToList(); 
        }
        private List<OrclProcedure> _SelectCheck = new List<OrclProcedure>();
        public List<OrclProcedure> SelectCheck
        {
            get => _SelectCheck;
            set
            {
                _SelectCheck = value;
                RaisePropertyChanged();
            }
        }
        private void ListViewCheck_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectCheck = ListViewCheck.SelectedItems.Cast<OrclProcedure>().ToList();
        }
      
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        private void DataGridServerSchema_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            VM.SchemaServerParam.SelectedSchemaElement = DataGridServerSchema.SelectedCells.Select(x => (SchemaElementValue)x.Item).Distinct().ToList();
        }

        private void DataGridLocalSchema_OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            VM.SchemaLocalParam.SelectedSchemaElement = DataGridLocalSchema.SelectedCells.Select(x => (SchemaElementValue)x.Item).Distinct().ToList();
        }
    }

    public enum TableItemType
    {
        XML_H_ZGLV,
        XML_H_SCHET,
        XML_H_ZAP,
        XML_H_PACIENT,
        XML_H_Z_SLUCH,
        XML_H_SANK_SMO,
        XML_H_SANK_CODE_EXP,
        XML_H_SLUCH,
        XML_H_KSLP,
        XML_H_CRIT,
        XML_H_NAPR,
        XML_H_B_DIAG,
        XML_H_B_PROT,
        XML_H_CONS,
        XML_H_DS2,
        XML_H_DS3,
        XML_H_DS2_N,
        XML_H_NAZR,
        XML_H_ONK_USL,
        XML_H_LEK_PR,
        XML_H_LEK_PR_DATE_INJ,
        XML_H_USL,
        XML_L_ZGLV,
        XML_L_PERS,
        V_XML_ERROR,
        seq_ZGLV,
        seq_SCHET,
        seq_SANK,
        seq_PACIENT,
        seq_ZAP,
        seq_USL,
        seq_SLUCH,
        seq_z_sluch,
        seq_L_ZGLV,
        seq_xml_h_onk_usl,
        seq_L_pers,
        seq_xml_h_lek_pr,
        XML_H_MR_USL_N,
        XML_H_SL_LEK_PR,
        XML_H_MED_DEV
    }
    public class TableItem : INotifyPropertyChanged
    {
        private TableItemType _Type { get; set; }
        public TableItemType Type
        {
            get => _Type;
            set
            {
                _Type = value;
                RaisePropertyChanged();
            }
        }
        private string _TableName { get; set; }
        public string TableName
        {
            get => _TableName ?? "";
            set
            {
                _TableName = value;
                RaisePropertyChanged();
            }
        }
        private bool? _Check { get; set; }
        public bool? Check
        {
            get => _Check;
            set
            {
                _Check = value;
                RaisePropertyChanged();
            }
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


    public class SettingVM : INotifyPropertyChanged
    {
        public delegate void SettingVMClose(bool DialogResult);

        public event SettingVMClose OnClosed;

        private string LocalFolder => AppDomain.CurrentDomain.BaseDirectory;
        private IWcfInterface wcf { get; }
        public SettingParamFolder ParamFolder { get; } = new SettingParamFolder();

        public SettingVM(IWcfInterface wcf)
        {
            this.wcf = wcf;
        }


      
        public List<FileType> SelectedLocalFileType { get; set; } = new List<FileType>();
        #region LocalParam
        private bool _ISVIRTUALPATH;
        public bool ISVIRTUALPATH
        {
            get => _ISVIRTUALPATH;
            set
            {
                _ISVIRTUALPATH = value;
                RaisePropertyChanged();
            }
        }
        private string _VIRTUALPATH;
        public string VIRTUALPATH
        {
            get => _VIRTUALPATH;
            set
            {
                _VIRTUALPATH = value;
                RaisePropertyChanged();
            }
        }
        public ICommand SelectVIRTUALPATH => new Command(o =>
        {
            try
            {
                var fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    VIRTUALPATH = fbd.SelectedPath;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        });


        #endregion
        #region LoadParam
        private bool _OnlyLocal;
        public bool OnlyLocal
        {
            get => _OnlyLocal;
            set
            {
                _OnlyLocal = value;
                RaisePropertyChanged();
            }
        }
        public void LoadParam(bool OnlyLocal)
        {
            this.OnlyLocal = OnlyLocal;
            if (!OnlyLocal)
            {
                var set = wcf.GetSettingsFolder();
                ParamFolder.SetFolder(set);
                var sc = wcf.GetSchemaCollection();
                SchemaServerParam.SetSchemaCollection(sc);

                var conn = wcf.GetSettingConnect();
                ConnectionServer.SetConnectionString(conn.ConnectingString);
                TableServerParam.CreateTableItemsServer(conn);

                TableTransferParam.CreateTableItemsTransfer(wcf.GetSettingTransfer());
                LoadCheckFromBDCommand.Execute(null);
            }

            var sc_local = new SchemaCollection();
            if (File.Exists(System.IO.Path.Combine(LocalFolder, "SANK_INVITER_SCHEMA.dat")))
                sc_local.LoadFromFile(System.IO.Path.Combine(LocalFolder, "SANK_INVITER_SCHEMA.dat"));
            SchemaLocalParam.SetSchemaCollection(sc_local);
            ConnectionLocal.SetConnectionString(AppConfig.Property.ConnectionString);

            TableLocalParam.CreateTableItemsLocal(AppConfig.Property);


           ISVIRTUALPATH = Properties.Settings.Default.ISVIRTUALPATH;
           VIRTUALPATH = Properties.Settings.Default.VIRTUALPATH; 

        }


        #endregion LoadParam
        #region SelectFolderCommand

        public ICommand SelectIncomingDir => new Command(o =>
        {
            try
            {
                var di = new RemoteFolderDialog(ParamFolder.IncomingDir, true);
                if (di.ShowDialog() == true)
                {
                    ParamFolder.IncomingDir = di.selectpath;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        });

        public ICommand SelectProcessDir => new Command(o =>
        {
            try
            {
                var di = new RemoteFolderDialog(ParamFolder.ProcessDir, true);
                if (di.ShowDialog() == true)
                {
                    ParamFolder.ProcessDir = di.selectpath;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        });

        public ICommand SelectInputDir => new Command(o =>
        {
            try
            {
                var di = new RemoteFolderDialog(ParamFolder.InputDir, true);
                if (di.ShowDialog() == true)
                {
                    ParamFolder.InputDir = di.selectpath;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        });

        public ICommand SelectErrorMessageFile => new Command(o =>
        {
            try
            {
                var di = new RemoteFolderDialog(ParamFolder.ErrorMessageFile, true);
                if (di.ShowDialog() == true)
                {
                    ParamFolder.ErrorMessageFile = di.selectpath;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        });

        public ICommand SelectErrorDir => new Command(o =>
        {
            try
            {
                var di = new RemoteFolderDialog(ParamFolder.ErrorDir, true);
                if (di.ShowDialog() == true)
                {
                    ParamFolder.ErrorDir = di.selectpath;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        });

        public ICommand SelectAddDIRInERROR => new Command(o =>
        {
            try
            {
                var di = new RemoteFolderDialog(ParamFolder.AddDIRInERROR, true);
                if (di.ShowDialog() == true)
                {
                    ParamFolder.AddDIRInERROR = di.selectpath;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        });

        #endregion

        public SchemaParamVM SchemaServerParam { get; } = new SchemaParamVM(false);
        public SchemaParamVM SchemaLocalParam { get; } = new SchemaParamVM(true);

        #region ConnectionParam
        public ConnectionParamVM ConnectionLocal { get; } = new ConnectionParamVM();
        public ICommand SaveConnectionLocal => new Command(o =>
        {
            try
            {
                AppConfig.Property.ConnectionString = ConnectionLocal.ConnectionString;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        });
        public ICommand ResetConnectionLocal => new Command(o =>
        {
            try
            {
                ConnectionLocal.SetConnectionString(AppConfig.Property.ConnectionString);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        });
        public ICommand CheckConnectionLocal => new Command(o =>
        {
            try
            {
                using (var con = new OracleConnection(ConnectionLocal.ConnectionString))
                {
                    con.Open();
                    con.Close();
                    ConnectionLocal.IsTestingOK = true;
                    ConnectionLocal.IsTestingMessage = "Подключение успешно";
                    System.Media.SystemSounds.Asterisk.Play();
                }
            }
            catch (Exception ex)
            {
                ConnectionLocal.IsTestingOK = false;
                ConnectionLocal.IsTestingMessage = $"Ошибка подключения{Environment.NewLine}{ex.Message}";
                System.Media.SystemSounds.Exclamation.Play();
            }
        });

        public ConnectionParamVM ConnectionServer { get; } = new ConnectionParamVM();
        public ICommand SaveConnectionServer => new Command(o =>
        {
            try
            {
                var setCon = new SettingConnect {ConnectingString = ConnectionServer.ConnectionString};
                wcf.SettingConnect(setCon);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        });
        public ICommand ResetConnectionServer => new Command(o =>
        {
            try
            {
                ConnectionServer.SetConnectionString(wcf.GetSettingConnect().ConnectingString);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        });
        public ICommand CheckConnectionServer => new Command(o =>
        {
            try
            {
                var rez = wcf.isConnect(ConnectionServer.ConnectionString);
                if (rez.Result)
                {
                    ConnectionServer.IsTestingOK = true;
                    ConnectionServer.IsTestingMessage = "Подключение успешно";
                    System.Media.SystemSounds.Asterisk.Play();
                }
                else
                {
                    ConnectionServer.IsTestingOK = false;
                    ConnectionServer.IsTestingMessage = $"Ошибка подключения{Environment.NewLine}{rez.Exception}";
                    System.Media.SystemSounds.Exclamation.Play();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        #endregion
        #region TableParam
        public TableParamVM TableServerParam { get; } = new TableParamVM();
        public ICommand CheckTableServerCommand => new Command(o =>
        {
            try
            {
                var tblrez = wcf.GetTableServer(TableServerParam.Owner);
                if (tblrez.Result == null)
                {
                    throw new Exception(tblrez.Exception);
                }

                foreach (var tbl in TableServerParam.TableItems)
                {
                    tbl.Check = tblrez.Result.Select($"TABLE_NAME = '{tbl.TableName.ToUpper()}'").Length != 0;
                }

                System.Media.SystemSounds.Asterisk.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });

        public TableParamVM TableLocalParam { get; } = new TableParamVM();
        public ICommand CheckTableLocalCommand => new Command(o =>
        {
            try
            {
                var TBL = GetTableLOCAL();
                var SEQ = GetSeqLOCAL();

                foreach (var tbl in TableLocalParam.TableItems)
                {
                    tbl.Check = TBL.Select($"TABLE_NAME = '{tbl.TableName.ToUpper()}'").Length != 0;
                }

                foreach (var tbl in TableLocalParam.SeqItems)
                {
                    tbl.Check = SEQ.Select($"SEQUENCE_NAME = '{tbl.TableName.ToUpper()}'").Length != 0;
                }

                System.Media.SystemSounds.Asterisk.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        });
        private DataTable GetTableLOCAL()
        {
            using (var con = new OracleConnection(AppConfig.Property.ConnectionString))
            {
                using (var oda = new OracleDataAdapter($@"SELECT TABLE_NAME FROM ALL_TABLES where  OWNER = '{AppConfig.Property.schemaOracle.ToUpper()}'", con))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return tbl;
                }
            }
        }
        private DataTable GetSeqLOCAL()
        {
            using (var con = new OracleConnection(AppConfig.Property.ConnectionString))
            {
                using (var oda = new OracleDataAdapter($@"SELECT SEQUENCE_NAME FROM ALL_SEQUENCES where  SEQUENCE_OWNER = '{AppConfig.Property.schemaOracle.ToUpper()}'", con))
                {
                    var tbl = new DataTable();
                    oda.Fill(tbl);
                    return tbl;
                }
            }
        }

        public TableParamVM TableTransferParam { get; } = new TableParamVM();
        public ICommand CheckTableTransferCommand => new Command(o =>
        {
            try
            {
                var set = TableTransferParam.ReadTableItemsTransfer();
                var tblrez = wcf.GetTableServer(set.schemaOracle);
                if (tblrez.Result == null)
                {
                    throw new Exception(tblrez.Exception);
                }

                foreach (var tbl in TableTransferParam.TableItems)
                {
                    tbl.Check = tblrez.Result.Select($"TABLE_NAME = '{tbl.TableName.ToUpper()}'").Length != 0;
                }

                System.Media.SystemSounds.Asterisk.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        #endregion

        public ICommand SaveParamCommand => new Command(o =>
        {
            try
            {
                if (!OnlyLocal)
                {
                    wcf.SettingsFolder(ParamFolder.GetFolder());
                    wcf.SettingSchemaCollection(SchemaServerParam.sc);
                    var set = TableServerParam.GetSettingConnect();
               
                    set.ConnectingString = ConnectionServer.ConnectionString;
                    wcf.SettingConnect(set);
                  
                    wcf.SetSettingTransfer(TableTransferParam.ReadTableItemsTransfer());
                    //wcf.SetCheckingList(checList);
                    wcf.SaveProperty();
                  
                }

                Properties.Settings.Default.ISVIRTUALPATH = ISVIRTUALPATH;
                Properties.Settings.Default.VIRTUALPATH = VIRTUALPATH;


                TableLocalParam.ReadTableItemsLocal();
                SchemaLocalParam.sc.SaveToFile(Path.Combine(LocalFolder, "SANK_INVITER_SCHEMA.dat"));
                AppConfig.Save();
                Properties.Settings.Default.Save();
                OnClosed?.Invoke(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });

        public ICommand CancelParamCommand => new Command(o =>
        {
            try
            {
                OnClosed?.Invoke(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        #region CheckParam

        public CheckParamVM CheckParam { get; } = new CheckParamVM();

        public ICommand LoadCheckFromBDCommand => new Command(o =>
        {
            try
            {
                var br = wcf.LoadCheckListFromBD();
                if (br.Result == false)
                {
                    MessageBox.Show(br.Exception);
                    return;
                }
                CheckParam.checkList =  wcf.GetCheckingList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        public ICommand SaveCheckInBDCommand => new Command(o =>
        {
            try
            {
                var res = wcf.SetCheckingList(CheckParam.checkList);
                MessageBox.Show(res.Result ? "Передача настроек успешна!" : $"Ошибка при передаче настроек: {res.Exception}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        });
        public ICommand CheckCheckInBDCommand => new Command(o =>
        {
            try
            {
                var result = wcf.ExecuteCheckAv(CheckParam.checkList);
                if (result == null)
                {
                    System.Windows.Forms.MessageBox.Show(@"Ошибка при выполнении см. лог сервера", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                CheckParam.checkList = result;
                MessageBox.Show(@"Выполнено", "", MessageBoxButton.OK, MessageBoxImage.Asterisk);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
           

        });
        private SaveFileDialog saveFileDialog1 = new SaveFileDialog { Filter = @"Файл проверок(CheckListFile(*.clf))|*.clv" };
        public ICommand SaveCheckInFileCommand => new Command(o =>
        {
            try
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    CheckParam.checkList.SaveToFile(saveFileDialog1.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        });
        private OpenFileDialog openFileDialog1 = new OpenFileDialog { Filter = @"Файл проверок(CheckListFile(*.clf))|*.clv" };
        public ICommand LoadCheckFromFileCommand => new Command(o =>
        {
            try
            {
                if (openFileDialog1.ShowDialog() ==DialogResult.OK)
                {
                    var checList = new CheckingList();
                    var res = checList.LoadToFile(openFileDialog1.FileName);
                    if (res == false) MessageBox.Show("Не удалось загрузить файл!");
                    CheckParam.checkList = checList;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        });


        public ICommand AddPackageCheckCommand => new Command(o =>
        {
            try
            {
                var AddPack = (string) o;
                if (!string.IsNullOrEmpty(AddPack))
                {
                    CheckParam.AddListProcedure( wcf.GetProcedureFromPack(AddPack));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        });


        public ICommand AddProcedureCheckCommand => new Command(o =>
        {
            try
            {
                var proc = new OrclProcedure();
                var win = new EdditProc(proc, AppConfig.Property.ConnectionString);
                if (win.ShowDialog() == true)
                {
                    CheckParam.AddProcedure(proc);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        });

        public ICommand EditProcedureCheckCommand => new Command(o =>
        {
            try
            {
                var orclselect = (List<OrclProcedure>)o;
                var proc = orclselect.FirstOrDefault();
                if (proc != null)
                {
                    var win = new EdditProc(new OrclProcedure(proc), AppConfig.Property.ConnectionString);
                    if (win.ShowDialog() == true)
                    {
                        proc.CopyFrom(win.curr);
                        CheckParam.RefreshProcedure();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        });

        public ICommand DuplicateProcedureCheckCommand => new Command(o =>
        {
            try
            {
                var orclselect = (List<OrclProcedure>)o;
                var proc = orclselect.FirstOrDefault();
                if (proc != null)
                {
                    var newproc = new OrclProcedure(proc);
                    CheckParam.AddProcedure(newproc);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        });


        public ICommand DeleteProcedureCheckCommand => new Command(o =>
        {
            try
            {
                var orclselect = (List<OrclProcedure>)o;
                if (orclselect.Count != 0)
                {
                    if (MessageBox.Show($"Вы уверены что хотите удалить проверк{(orclselect.Count == 1 ? "у" : "и")}?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        foreach (var item in orclselect)
                        {
                            CheckParam.RemoveProcedure(item);
                        }
                     
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        });

        /*
            try
            {
                var select = CurrentCheckTableName;
                if (select.HasValue)
                {
                    var proc = currOrclProcedure;
                    if (proc.Count != 0)
                    {
                        var newproc = new OrclProcedure(proc.First());
                        checList[select.Value].Add(newproc);
                        refreshListViewChek_ALL();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }*/


        /*
            var select = CurrentCheckTableName;
            if (select.HasValue)
            {
                var proc = new OrclProcedure();
                var win = new EdditProc(proc, AppConfig.Property.ConnectionString) { Owner = this };
                if (win.ShowDialog() == true)
                {
                    checList.Add(select.Value, win.curr);
                    refreshListViewChek_ALL();

                }
            }*/
        /*
           try
           {
               var select = CurrentCheckTableName;
               if (select.HasValue)
               {
                   checList.AddList(select.Value, wcf.GetProcedureFromPack(TextBoxAddPack.Text));
                   refreshListViewChek_ALL();
               }
           }
           catch (Exception ex)
           {
               MessageBox.Show(ex.Message);
           }*/

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


    public class SettingParamFolder : INotifyPropertyChanged
    {
        public string _IncomingDir;
        public string IncomingDir
        {
            get => _IncomingDir;
            set
            {
                _IncomingDir = value;
                RaisePropertyChanged();
            }
        }

        public string _InputDir;
        public string InputDir
        {
            get => _InputDir;
            set
            {
                _InputDir = value;
                RaisePropertyChanged();
            }
        }

        public string _ErrorDir;
        public string ErrorDir
        {
            get => _ErrorDir;
            set
            {
                _ErrorDir = value;
                RaisePropertyChanged();
            }
        }
        public string _ErrorMessageFile;
        public string ErrorMessageFile
        {
            get => _ErrorMessageFile;
            set
            {
                _ErrorMessageFile = value;
                RaisePropertyChanged();
            }
        }
        public string _ProcessDir;
        public string ProcessDir
        {
            get => _ProcessDir;
            set
            {
                _ProcessDir = value;
                RaisePropertyChanged();
            }
        }
        public string _AddDIRInERROR;
        public string AddDIRInERROR
        {
            get => _AddDIRInERROR;
            set
            {
                _AddDIRInERROR = value;
                RaisePropertyChanged();
            }
        }

        public int _TimePacketOpen;
        public int TimePacketOpen
        {
            get => _TimePacketOpen;
            set
            {
                _TimePacketOpen = value;
                RaisePropertyChanged();
            }
        }

        public string _ISP;
        public string ISP
        {
            get => _ISP;
            set
            {
                _ISP = value;
                RaisePropertyChanged();
            }
        }

        public void SetFolder(SettingsFolder set)
        {
            IncomingDir = set.IncomingDir;
            InputDir = set.InputDir;
            ErrorDir = set.ErrorDir;
            ProcessDir = set.ProcessDir;
            ErrorMessageFile = set.ErrorMessageFile;
            AddDIRInERROR = set.AddDIRInERROR;
            TimePacketOpen = set.TimePacketOpen;
            ISP = set.ISP;
        }

        public SettingsFolder GetFolder( )
        {
            var set = new SettingsFolder
            {
                IncomingDir = IncomingDir,
                InputDir = InputDir,
                ErrorDir = ErrorDir,
                ProcessDir = ProcessDir,
                ErrorMessageFile = ErrorMessageFile,
                AddDIRInERROR = AddDIRInERROR,
                TimePacketOpen = TimePacketOpen,
                ISP = ISP
            };
            return set;
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
    public class SchemaParamVM:INotifyPropertyChanged
    {
        private bool isLocalFind{ get; set; }
        public SchemaParamVM(bool isLocalFind)
        {
            this.isLocalFind = isLocalFind;
        }
        public void SetSchemaCollection(SchemaCollection sc)
        {
            this.sc = sc;
            RaisePropertyChanged(nameof(Versions));
            RaisePropertyChanged(nameof(Elements));
            RaisePropertyChanged(nameof(VersionZGLV));
        }
        public List<FileType> SelectedFileType { get; set; } = new List<FileType>();

        public List<string> SelectedVersionZGLV = new List<string>();
        public List<SchemaElementValue> SelectedSchemaElement { get; set; } = new List<SchemaElementValue>();


        public SchemaCollection sc { get; private set; } = new SchemaCollection();
        public IEnumerable<FileType> FileTypes => (FileType[])Enum.GetValues(typeof(FileType));
        private FileType _CurrentFileType;
        public FileType CurrentFileType
        {
            get => _CurrentFileType;
            set
            {
                _CurrentFileType = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Elements));
            }
        }

        public List<VersionMP> Versions => sc.Versions;
        private VersionMP _CurrentVersion;
        public VersionMP CurrentVersion
        {
            get => _CurrentVersion;
            set
            {
                _CurrentVersion = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Elements));
                RaisePropertyChanged(nameof(VersionZGLV));
            }
        }

        public IEnumerable<SchemaElementValue> Elements
        {
            get
            {
                foreach (var item in sc[CurrentVersion, CurrentFileType])
                {
                    yield return item;
                }
            }
        }

        public IEnumerable<string> VersionZGLV
        {
            get
            {
                foreach (var item in sc[CurrentVersion].VersionsZGLV)
                {
                    yield return item;
                }
            }
        }


        public ICommand AddVersionZglvCommand => new Command(o =>
        {
            try
            {
                var value = o.ToString();
                if (!string.IsNullOrEmpty(value))
                    sc[CurrentVersion].VersionsZGLV.Add(o.ToString());
                RaisePropertyChanged(nameof(VersionZGLV));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        });
        public ICommand RemoveVersionZglvCommand => new Command(o =>
        {
            try
            {
                if (SelectedVersionZGLV.Count != 0)
                {
                    foreach (var item in SelectedVersionZGLV)
                    {
                        sc[CurrentVersion].VersionsZGLV.Remove(item);
                    }
                    RaisePropertyChanged(nameof(VersionZGLV));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        });

        public ICommand AddSchemaElementAllCommand => new Command(o =>
        {
            try
            {
                if (SelectedFileType.Count != 0)
                {
                    var win = new NewSchemaItem(isLocalFind);
                    if (win.ShowDialog() == true)
                    {
                        foreach (var item in SelectedFileType)
                        {
                            try
                            {
                                sc[CurrentVersion].AddAndCheck(item, new SchemaElementValue { DATE_B = win.DATE_B, DATE_E = win.DATE_E, Value = win.PATH });
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($@"Не удалось добавить схему к файлу {item} версии {CurrentVersion} по причине: {ex.Message}");
                            }
                        }
                        RaisePropertyChanged(nameof(Elements));
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        });


        public ICommand AddSchemaElementCommand => new Command(o =>
        {
            try
            {
                if (SelectedFileType.Count != 0)
                {
                    var win = new NewSchemaItem(isLocalFind);
                    if (win.ShowDialog() == true)
                    {
                        var item = SelectedFileType.First();
                        sc[CurrentVersion].AddAndCheck(item, new SchemaElementValue { DATE_B = win.DATE_B, DATE_E = win.DATE_E, Value = win.PATH });
                        RaisePropertyChanged(nameof(Elements));
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        });

        public ICommand EditSchemaElementCommand => new Command(o =>
        {
            try
            {
                var item = SelectedSchemaElement.FirstOrDefault();
                if (item!=null)
                {
                    var win = new NewSchemaItem(isLocalFind, item);
                    if (win.ShowDialog() == true)
                    {
                        item.DATE_B = win.DATE_B;
                        item.DATE_E = win.DATE_E;
                        item.Value = win.PATH;
                        RaisePropertyChanged(nameof(Elements));
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        });


        public ICommand RemoveSchemaElementCommand => new Command(o =>
        {
            try
            {
              
                if (SelectedSchemaElement.Count!=0 && SelectedFileType.Count != 0)
                {
                    var ft = SelectedFileType.FirstOrDefault();
                    foreach (var item in SelectedSchemaElement)
                    {
                        sc[CurrentVersion].SchemaElements[ft].Remove(item);
                    }
                    RaisePropertyChanged(nameof(Elements));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        });


        private string LocalFolder => AppDomain.CurrentDomain.BaseDirectory;

        public ICommand ClearSchemaElementAllCommand => new Command(o =>
        {
            try
            {
                if (SelectedFileType.Count != 0)
                {
                    foreach (var item in SelectedFileType)
                    {
                        try
                        {
                            sc[CurrentVersion].ClearSchema(item);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($@"Не удалось добавить схему к файлу {item} версии {CurrentVersion} по причине: {ex.Message}");
                        }
                    }
                    RaisePropertyChanged(nameof(Elements));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        });
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
    public class ConnectionParamVM : INotifyPropertyChanged
    {
        public List<string> DBAPrivilegeList { get; } = new List<string> {"NORMAL", "SYSDBA", "SYSOPER"};

        private string _HOST;
        public string HOST
        {
            get => _HOST;
            set
            {
                _HOST = value;
                RaisePropertyChanged();
            }
        }
        private string _User;
        public string User
        {
            get => _User;
            set
            {
                _User = value;
                RaisePropertyChanged();
            }
        }
        private string _Password;
        public string Password
        {
            get => _Password;
            set
            {
                _Password = value;
                RaisePropertyChanged();
            }
        }
        private string _DataBase;
        public string DataBase
        {
            get => _DataBase;
            set
            {
                _DataBase = value;
                RaisePropertyChanged();
            }
        }
        private string _Port;
        public string Port
        {
            get => _Port;
            set
            {
                _Port = value;
                RaisePropertyChanged();
            }
        }

        private string _DBAPrivilege;
        public string DBAPrivilege
        {
            get => _DBAPrivilege;
            set
            {
                _DBAPrivilege = value;
                RaisePropertyChanged();
            }
        }


        private bool? _IsTestingOK;
        public bool? IsTestingOK
        {
            get => _IsTestingOK;
            set
            {
                _IsTestingOK = value;
                RaisePropertyChanged();
            }
        }

        private string _IsTestingMessage;
        public string IsTestingMessage
        {
            get => _IsTestingMessage;
            set
            {
                _IsTestingMessage = value;
                RaisePropertyChanged();
            }
        }

        public void SetConnectionString(string ConnectionString)
        {
            var conn = ServerRef.ParseDataSource(ConnectionString);
            HOST = conn.HOST;
            User = conn.UserID;
            Password = conn.Password;
            DataBase = conn.BD;
            Port = conn.PORT;
            DBAPrivilege = conn.DBAPrivilege;
        }

        public string ConnectionString
        {
            get
            {
                var conn = new OracleConnectionStringBuilder {DataSource = $"{HOST}:{Port}/{DataBase}", UserID = User, Password = Password, DBAPrivilege = DBAPrivilege.In("SYSOPER", "SYSDBA") ? DBAPrivilege : ""};
                return conn.ConnectionString;
            }
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
    public class TableParamVM : INotifyPropertyChanged
    {
        public ObservableCollection<TableItem> TableItems { get; } = new ObservableCollection<TableItem>();
        public ObservableCollection<TableItem> SeqItems { get; } = new ObservableCollection<TableItem>();
        
        private string _Owner;
        public string Owner
        {
            get => _Owner;
            set
            {
                _Owner = value;
                RaisePropertyChanged();
            }
        }

        private bool _Enabled;
        public bool Enabled
        {
            get => _Enabled;
            set
            {
                _Enabled = value;
                RaisePropertyChanged();
            }
        }

        public void CreateTableItemsServer(SettingConnect setCon)
        {
            TableItems.Clear();
            TableItems.Add(new TableItem { TableName = setCon.xml_h_zglv, Type = TableItemType.XML_H_ZGLV });
            TableItems.Add(new TableItem { TableName = setCon.xml_h_schet, Type = TableItemType.XML_H_SCHET });
            TableItems.Add(new TableItem { TableName = setCon.xml_h_zap, Type = TableItemType.XML_H_ZAP });
            TableItems.Add(new TableItem { TableName = setCon.xml_h_pacient, Type = TableItemType.XML_H_PACIENT });
            TableItems.Add(new TableItem { TableName = setCon.xml_h_z_sluch, Type = TableItemType.XML_H_Z_SLUCH });
            TableItems.Add(new TableItem { TableName = setCon.xml_h_sank_smo, Type = TableItemType.XML_H_SANK_SMO });
            TableItems.Add(new TableItem { TableName = setCon.xml_h_sank_code_exp, Type = TableItemType.XML_H_SANK_CODE_EXP });
            TableItems.Add(new TableItem { TableName = setCon.xml_h_sluch, Type = TableItemType.XML_H_SLUCH });
            TableItems.Add(new TableItem { TableName = setCon.xml_h_kslp, Type = TableItemType.XML_H_KSLP });
            TableItems.Add(new TableItem { TableName = setCon.xml_h_crit, Type = TableItemType.XML_H_CRIT });
            TableItems.Add(new TableItem { TableName = setCon.xml_h_napr, Type = TableItemType.XML_H_NAPR });
            TableItems.Add(new TableItem { TableName = setCon.xml_h_b_diag, Type = TableItemType.XML_H_B_DIAG });
            TableItems.Add(new TableItem { TableName = setCon.xml_h_b_prot, Type = TableItemType.XML_H_B_PROT });
            TableItems.Add(new TableItem { TableName = setCon.xml_h_cons, Type = TableItemType.XML_H_CONS });
            TableItems.Add(new TableItem { TableName = setCon.xml_h_ds2, Type = TableItemType.XML_H_DS2 });
            TableItems.Add(new TableItem { TableName = setCon.xml_h_ds3, Type = TableItemType.XML_H_DS3 });
            TableItems.Add(new TableItem { TableName = setCon.xml_h_ds2_n, Type = TableItemType.XML_H_DS2_N });
            TableItems.Add(new TableItem { TableName = setCon.xml_h_nazr, Type = TableItemType.XML_H_NAZR });
            TableItems.Add(new TableItem { TableName = setCon.xml_h_onk_usl, Type = TableItemType.XML_H_ONK_USL });
            TableItems.Add(new TableItem { TableName = setCon.xml_h_lek_pr, Type = TableItemType.XML_H_LEK_PR });
            TableItems.Add(new TableItem { TableName = setCon.xml_h_lek_pr_date_inj, Type = TableItemType.XML_H_LEK_PR_DATE_INJ });
            TableItems.Add(new TableItem { TableName = setCon.xml_h_usl, Type = TableItemType.XML_H_USL });
            TableItems.Add(new TableItem { TableName = setCon.xml_h_mr_usl_n, Type = TableItemType.XML_H_MR_USL_N });

            TableItems.Add(new TableItem { TableName = setCon.xml_h_sl_lek_pr, Type = TableItemType.XML_H_SL_LEK_PR });
            TableItems.Add(new TableItem { TableName = setCon.xml_h_med_dev, Type = TableItemType.XML_H_MED_DEV });


            TableItems.Add(new TableItem { TableName = setCon.xml_l_zglv, Type = TableItemType.XML_L_ZGLV });
            TableItems.Add(new TableItem { TableName = setCon.xml_l_pers, Type = TableItemType.XML_L_PERS });
            TableItems.Add(new TableItem { TableName = setCon.v_xml_error, Type = TableItemType.V_XML_ERROR });
            Owner = setCon.schemaOracle;
        }
        public void CreateTableItemsLocal(AppProperties prop)
        {
            TableItems.Clear();
            TableItems.Add(new TableItem { TableName = prop.xml_h_zglv, Type = TableItemType.XML_H_ZGLV });
            TableItems.Add(new TableItem { TableName = prop.xml_h_schet, Type = TableItemType.XML_H_SCHET });
            TableItems.Add(new TableItem { TableName = prop.xml_h_zap, Type = TableItemType.XML_H_ZAP });
            TableItems.Add(new TableItem { TableName = prop.xml_h_pacient, Type = TableItemType.XML_H_PACIENT });
            TableItems.Add(new TableItem { TableName = prop.xml_h_z_sluch, Type = TableItemType.XML_H_Z_SLUCH });
            TableItems.Add(new TableItem { TableName = prop.xml_h_sank, Type = TableItemType.XML_H_SANK_SMO });
            TableItems.Add(new TableItem { TableName = prop.xml_h_code_exp, Type = TableItemType.XML_H_SANK_CODE_EXP });
            TableItems.Add(new TableItem { TableName = prop.xml_h_sluch, Type = TableItemType.XML_H_SLUCH });
            TableItems.Add(new TableItem { TableName = prop.xml_h_kslp, Type = TableItemType.XML_H_KSLP });
            TableItems.Add(new TableItem { TableName = prop.xml_h_crit, Type = TableItemType.XML_H_CRIT });
            TableItems.Add(new TableItem { TableName = prop.xml_h_napr, Type = TableItemType.XML_H_NAPR });
            TableItems.Add(new TableItem { TableName = prop.xml_h_b_diag, Type = TableItemType.XML_H_B_DIAG });
            TableItems.Add(new TableItem { TableName = prop.xml_h_b_prot, Type = TableItemType.XML_H_B_PROT });
            TableItems.Add(new TableItem { TableName = prop.xml_h_cons, Type = TableItemType.XML_H_CONS });
            TableItems.Add(new TableItem { TableName = prop.xml_h_ds2, Type = TableItemType.XML_H_DS2 });
            TableItems.Add(new TableItem { TableName = prop.xml_h_ds3, Type = TableItemType.XML_H_DS3 });
            TableItems.Add(new TableItem { TableName = prop.xml_h_ds2_n, Type = TableItemType.XML_H_DS2_N });
            TableItems.Add(new TableItem { TableName = prop.xml_h_nazr, Type = TableItemType.XML_H_NAZR });
            TableItems.Add(new TableItem { TableName = prop.xml_h_onk_usl, Type = TableItemType.XML_H_ONK_USL });
            TableItems.Add(new TableItem { TableName = prop.xml_h_lek_pr, Type = TableItemType.XML_H_LEK_PR });
            TableItems.Add(new TableItem { TableName = prop.xml_h_date_inj, Type = TableItemType.XML_H_LEK_PR_DATE_INJ });
            TableItems.Add(new TableItem { TableName = prop.xml_h_usl, Type = TableItemType.XML_H_USL });
            TableItems.Add(new TableItem { TableName = prop.xml_h_mr_usl_n, Type = TableItemType.XML_H_MR_USL_N });
            TableItems.Add(new TableItem { TableName = prop.xml_h_sl_lek_pr, Type = TableItemType.XML_H_SL_LEK_PR });
            TableItems.Add(new TableItem { TableName = prop.xml_h_med_dev, Type = TableItemType.XML_H_MED_DEV });
            TableItems.Add(new TableItem { TableName = prop.xml_l_zglv, Type = TableItemType.XML_L_ZGLV });
            TableItems.Add(new TableItem { TableName = prop.xml_l_pers, Type = TableItemType.XML_L_PERS });


            Owner = prop.schemaOracle;


            SeqItems.Clear();
            SeqItems.Add(new TableItem { TableName = prop.seq_ZGLV, Type = TableItemType.seq_ZGLV });
            SeqItems.Add(new TableItem { TableName = prop.seq_SCHET, Type = TableItemType.seq_SCHET });
            SeqItems.Add(new TableItem { TableName = prop.seq_ZAP, Type = TableItemType.seq_ZAP });
            SeqItems.Add(new TableItem { TableName = prop.seq_PACIENT, Type = TableItemType.seq_PACIENT });
            SeqItems.Add(new TableItem { TableName = prop.seq_z_sluch, Type = TableItemType.seq_z_sluch });
            SeqItems.Add(new TableItem { TableName = prop.seq_SANK, Type = TableItemType.seq_SANK });
            SeqItems.Add(new TableItem { TableName = prop.seq_SLUCH, Type = TableItemType.seq_SLUCH });
            SeqItems.Add(new TableItem { TableName = prop.seq_USL, Type = TableItemType.seq_USL });
            SeqItems.Add(new TableItem { TableName = prop.seq_L_ZGLV, Type = TableItemType.seq_L_ZGLV });
            SeqItems.Add(new TableItem { TableName = prop.seq_L_pers, Type = TableItemType.seq_L_pers });
            SeqItems.Add(new TableItem { TableName = prop.seq_xml_h_onk_usl, Type = TableItemType.seq_xml_h_onk_usl });
            SeqItems.Add(new TableItem { TableName = prop.seq_xml_h_lek_pr, Type = TableItemType.seq_xml_h_lek_pr });
        }
        public void CreateTableItemsTransfer(SettingTransfer setTrans)
        {
            TableItems.Clear();
            TableItems.Add(new TableItem { TableName = setTrans.xml_h_zglv, Type = TableItemType.XML_H_ZGLV });
            TableItems.Add(new TableItem { TableName = setTrans.xml_h_schet, Type = TableItemType.XML_H_SCHET });
            TableItems.Add(new TableItem { TableName = setTrans.xml_h_zap, Type = TableItemType.XML_H_ZAP });
            TableItems.Add(new TableItem { TableName = setTrans.xml_h_pacient, Type = TableItemType.XML_H_PACIENT });
            TableItems.Add(new TableItem { TableName = setTrans.xml_h_z_sluch, Type = TableItemType.XML_H_Z_SLUCH });
            TableItems.Add(new TableItem { TableName = setTrans.xml_h_sank_smo, Type = TableItemType.XML_H_SANK_SMO });
            TableItems.Add(new TableItem { TableName = setTrans.xml_h_sank_code_exp, Type = TableItemType.XML_H_SANK_CODE_EXP });
            TableItems.Add(new TableItem { TableName = setTrans.xml_h_sluch, Type = TableItemType.XML_H_SLUCH });
            TableItems.Add(new TableItem { TableName = setTrans.xml_h_kslp, Type = TableItemType.XML_H_KSLP });
            TableItems.Add(new TableItem { TableName = setTrans.xml_h_crit, Type = TableItemType.XML_H_CRIT });
            TableItems.Add(new TableItem { TableName = setTrans.xml_h_napr, Type = TableItemType.XML_H_NAPR });
            TableItems.Add(new TableItem { TableName = setTrans.xml_h_b_diag, Type = TableItemType.XML_H_B_DIAG });
            TableItems.Add(new TableItem { TableName = setTrans.xml_h_b_prot, Type = TableItemType.XML_H_B_PROT });
            TableItems.Add(new TableItem { TableName = setTrans.xml_h_cons, Type = TableItemType.XML_H_CONS });
            TableItems.Add(new TableItem { TableName = setTrans.xml_h_ds2, Type = TableItemType.XML_H_DS2 });
            TableItems.Add(new TableItem { TableName = setTrans.xml_h_ds3, Type = TableItemType.XML_H_DS3 });
            TableItems.Add(new TableItem { TableName = setTrans.xml_h_ds2_n_transfer, Type = TableItemType.XML_H_DS2_N });
            TableItems.Add(new TableItem { TableName = setTrans.xml_h_nazr_transfer, Type = TableItemType.XML_H_NAZR });
            TableItems.Add(new TableItem { TableName = setTrans.xml_h_onk_usl, Type = TableItemType.XML_H_ONK_USL });
            TableItems.Add(new TableItem { TableName = setTrans.xml_h_lek_pr, Type = TableItemType.XML_H_LEK_PR });
            TableItems.Add(new TableItem { TableName = setTrans.xml_h_lek_pr_date_inj, Type = TableItemType.XML_H_LEK_PR_DATE_INJ });
            TableItems.Add(new TableItem { TableName = setTrans.xml_h_usl, Type = TableItemType.XML_H_USL });
            TableItems.Add(new TableItem { TableName = setTrans.xml_h_mr_usl_n, Type = TableItemType.XML_H_MR_USL_N });
            TableItems.Add(new TableItem { TableName = setTrans.xml_h_sl_lek_pr, Type = TableItemType.XML_H_SL_LEK_PR });
            TableItems.Add(new TableItem { TableName = setTrans.xml_h_med_dev, Type = TableItemType.XML_H_MED_DEV });
            TableItems.Add(new TableItem { TableName = setTrans.xml_l_zglv, Type = TableItemType.XML_L_ZGLV });
            TableItems.Add(new TableItem { TableName = setTrans.xml_l_pers, Type = TableItemType.XML_L_PERS });


            Owner = setTrans.schemaOracle;
            Enabled = setTrans.Transfer;
        }

        public SettingConnect GetSettingConnect()
        {
            var setCon = new SettingConnect
            {
                schemaOracle = Owner,
                xml_h_zglv = GetTableItems(TableItemType.XML_H_ZGLV),
                xml_h_schet = GetTableItems(TableItemType.XML_H_SCHET),
                xml_h_zap = GetTableItems(TableItemType.XML_H_ZAP),
                xml_h_pacient = GetTableItems(TableItemType.XML_H_PACIENT),
                xml_h_z_sluch = GetTableItems(TableItemType.XML_H_Z_SLUCH),
                xml_h_sank_smo = GetTableItems(TableItemType.XML_H_SANK_SMO),
                xml_h_sank_code_exp = GetTableItems(TableItemType.XML_H_SANK_CODE_EXP),
                xml_h_sluch = GetTableItems(TableItemType.XML_H_SLUCH),
                xml_h_kslp = GetTableItems(TableItemType.XML_H_KSLP),
                xml_h_crit = GetTableItems(TableItemType.XML_H_CRIT),
                xml_h_napr = GetTableItems(TableItemType.XML_H_NAPR),
                xml_h_b_diag = GetTableItems(TableItemType.XML_H_B_DIAG),
                xml_h_b_prot = GetTableItems(TableItemType.XML_H_B_PROT),
                xml_h_cons = GetTableItems(TableItemType.XML_H_CONS),
                xml_h_ds2 = GetTableItems(TableItemType.XML_H_DS2),
                xml_h_ds3 = GetTableItems(TableItemType.XML_H_DS3),
                xml_h_ds2_n = GetTableItems(TableItemType.XML_H_DS2_N),
                xml_h_nazr = GetTableItems(TableItemType.XML_H_NAZR),
                xml_h_onk_usl = GetTableItems(TableItemType.XML_H_ONK_USL),
                xml_h_lek_pr = GetTableItems(TableItemType.XML_H_LEK_PR),
                xml_h_lek_pr_date_inj = GetTableItems(TableItemType.XML_H_LEK_PR_DATE_INJ),
                xml_h_usl = GetTableItems(TableItemType.XML_H_USL),
                xml_h_mr_usl_n = GetTableItems(TableItemType.XML_H_MR_USL_N),
                xml_l_zglv = GetTableItems(TableItemType.XML_L_ZGLV),
                xml_l_pers = GetTableItems(TableItemType.XML_L_PERS),
                v_xml_error = GetTableItems(TableItemType.V_XML_ERROR),
                xml_h_sl_lek_pr  = GetTableItems(TableItemType.XML_H_SL_LEK_PR),
                xml_h_med_dev = GetTableItems(TableItemType.XML_H_MED_DEV)
        };
            return setCon;
        }
        public void ReadTableItemsLocal()
        {
            AppConfig.Property.xml_h_zglv = GetTableItems(TableItemType.XML_H_ZGLV);
            AppConfig.Property.xml_h_schet = GetTableItems(TableItemType.XML_H_SCHET);
            AppConfig.Property.xml_h_zap = GetTableItems(TableItemType.XML_H_ZAP);
            AppConfig.Property.xml_h_pacient = GetTableItems(TableItemType.XML_H_PACIENT);
            AppConfig.Property.xml_h_z_sluch = GetTableItems(TableItemType.XML_H_Z_SLUCH);
            AppConfig.Property.xml_h_sank = GetTableItems(TableItemType.XML_H_SANK_SMO);
            AppConfig.Property.xml_h_code_exp = GetTableItems(TableItemType.XML_H_SANK_CODE_EXP);
            AppConfig.Property.xml_h_sluch = GetTableItems(TableItemType.XML_H_SLUCH);
            AppConfig.Property.xml_h_kslp = GetTableItems(TableItemType.XML_H_KSLP);
            AppConfig.Property.xml_h_crit = GetTableItems(TableItemType.XML_H_CRIT);
            AppConfig.Property.xml_h_napr = GetTableItems(TableItemType.XML_H_NAPR);
            AppConfig.Property.xml_h_b_diag = GetTableItems(TableItemType.XML_H_B_DIAG);
            AppConfig.Property.xml_h_b_prot = GetTableItems(TableItemType.XML_H_B_PROT);
            AppConfig.Property.xml_h_cons = GetTableItems(TableItemType.XML_H_CONS);
            AppConfig.Property.xml_h_ds2 = GetTableItems(TableItemType.XML_H_DS2);
            AppConfig.Property.xml_h_ds3 = GetTableItems(TableItemType.XML_H_DS3);
            AppConfig.Property.xml_h_ds2_n = GetTableItems(TableItemType.XML_H_DS2_N);
            AppConfig.Property.xml_h_nazr = GetTableItems(TableItemType.XML_H_NAZR);
            AppConfig.Property.xml_h_onk_usl = GetTableItems(TableItemType.XML_H_ONK_USL);
            AppConfig.Property.xml_h_lek_pr = GetTableItems(TableItemType.XML_H_LEK_PR);
            AppConfig.Property.xml_h_date_inj = GetTableItems(TableItemType.XML_H_LEK_PR_DATE_INJ);
            AppConfig.Property.xml_h_usl = GetTableItems(TableItemType.XML_H_USL);
            AppConfig.Property.xml_h_mr_usl_n= GetTableItems(TableItemType.XML_H_MR_USL_N);
            AppConfig.Property.xml_l_zglv = GetTableItems(TableItemType.XML_L_ZGLV);
            AppConfig.Property.xml_l_pers = GetTableItems(TableItemType.XML_L_PERS);
            AppConfig.Property.xml_h_sl_lek_pr = GetTableItems(TableItemType.XML_H_SL_LEK_PR);
            AppConfig.Property.xml_h_med_dev = GetTableItems(TableItemType.XML_H_MED_DEV);

            AppConfig.Property.schemaOracle = Owner;


            AppConfig.Property.seq_ZGLV = GetSeqItems(TableItemType.seq_ZGLV);
            AppConfig.Property.seq_SCHET = GetSeqItems(TableItemType.seq_SCHET);
            AppConfig.Property.seq_ZAP = GetSeqItems(TableItemType.seq_ZAP);
            AppConfig.Property.seq_PACIENT = GetSeqItems(TableItemType.seq_PACIENT);
            AppConfig.Property.seq_z_sluch = GetSeqItems(TableItemType.seq_z_sluch);
            AppConfig.Property.seq_SANK = GetSeqItems(TableItemType.seq_SANK);
            AppConfig.Property.seq_SLUCH = GetSeqItems(TableItemType.seq_SLUCH);
            AppConfig.Property.seq_USL = GetSeqItems(TableItemType.seq_USL);
            AppConfig.Property.seq_L_ZGLV = GetSeqItems(TableItemType.seq_L_ZGLV);
            AppConfig.Property.seq_L_pers = GetSeqItems(TableItemType.seq_L_pers);
            AppConfig.Property.seq_xml_h_onk_usl = GetSeqItems(TableItemType.seq_xml_h_onk_usl);
            AppConfig.Property.seq_xml_h_lek_pr = GetSeqItems(TableItemType.seq_xml_h_lek_pr);
        }

        public SettingTransfer ReadTableItemsTransfer()
        {
            var setTrans = new SettingTransfer
            {
                schemaOracle = Owner,
                Transfer = Enabled,
                xml_h_zglv = GetTableItems(TableItemType.XML_H_ZGLV),
                xml_h_schet = GetTableItems(TableItemType.XML_H_SCHET),
                xml_h_zap = GetTableItems(TableItemType.XML_H_ZAP),
                xml_h_pacient = GetTableItems(TableItemType.XML_H_PACIENT),
                xml_h_z_sluch = GetTableItems(TableItemType.XML_H_Z_SLUCH),
                xml_h_sank_smo = GetTableItems(TableItemType.XML_H_SANK_SMO),
                xml_h_sank_code_exp = GetTableItems(TableItemType.XML_H_SANK_CODE_EXP),
                xml_h_sluch = GetTableItems(TableItemType.XML_H_SLUCH),
                xml_h_kslp = GetTableItems(TableItemType.XML_H_KSLP),
                xml_h_crit = GetTableItems(TableItemType.XML_H_CRIT),
                xml_h_napr = GetTableItems(TableItemType.XML_H_NAPR),
                xml_h_b_diag = GetTableItems(TableItemType.XML_H_B_DIAG),
                xml_h_b_prot = GetTableItems(TableItemType.XML_H_B_PROT),
                xml_h_cons = GetTableItems(TableItemType.XML_H_CONS),
                xml_h_ds2 = GetTableItems(TableItemType.XML_H_DS2),
                xml_h_ds3 = GetTableItems(TableItemType.XML_H_DS3),
                xml_h_ds2_n_transfer = GetTableItems(TableItemType.XML_H_DS2_N),
                xml_h_nazr_transfer = GetTableItems(TableItemType.XML_H_NAZR),
                xml_h_onk_usl = GetTableItems(TableItemType.XML_H_ONK_USL),
                xml_h_lek_pr = GetTableItems(TableItemType.XML_H_LEK_PR),
                xml_h_lek_pr_date_inj = GetTableItems(TableItemType.XML_H_LEK_PR_DATE_INJ),
                xml_h_usl = GetTableItems(TableItemType.XML_H_USL),
                xml_h_mr_usl_n = GetTableItems(TableItemType.XML_H_MR_USL_N),
                xml_l_zglv = GetTableItems(TableItemType.XML_L_ZGLV),
                xml_l_pers = GetTableItems(TableItemType.XML_L_PERS),
                xml_h_sl_lek_pr = GetTableItems(TableItemType.XML_H_SL_LEK_PR),
                xml_h_med_dev = GetTableItems(TableItemType.XML_H_MED_DEV)
            };
            return setTrans;
        }




        private string GetTableItems(TableItemType type)
        {
            return TableItems.FirstOrDefault(x => x.Type == type)?.TableName ?? "";
        }
        private string GetSeqItems(TableItemType type)
        {
            return SeqItems.FirstOrDefault(x => x.Type == type)?.TableName ?? "";
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
    public class CheckParamVM : INotifyPropertyChanged
    {
        private CheckingList _checkList= new CheckingList();

        public CheckingList checkList
        {
            get => _checkList;
            set
            {
                _checkList = value;
                RaisePropertyChanged();
                WriteCheckList();
            }
        }

     
        private void WriteCheckList()
        {
            CheckTableName.Clear();
            CheckTableName.AddRange(checkList.GeTableNames());
        }

        public ObservableCollection<TableName> CheckTableName { get; set; } = new ObservableCollection<TableName>();

        private TableName _CurrentTableName;
        public TableName CurrentTableName
        {
            get => _CurrentTableName;
            set
            {
                _CurrentTableName = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(Procedures));
            }
        }
        public IEnumerable<OrclProcedure> Procedures
        {
            get
            {
                foreach (var item in checkList[CurrentTableName])
                {
                    yield return item;
                }
            }
        }

        public void AddProcedure(OrclProcedure proc)
        {
            checkList.Add(CurrentTableName, proc);
            RaisePropertyChanged(nameof(Procedures));
        }
        public void AddListProcedure(List<OrclProcedure> procs)
        {
            checkList.AddList(CurrentTableName, procs);
            RaisePropertyChanged(nameof(Procedures));
        }

        public void RemoveProcedure(OrclProcedure proc)
        {
            checkList[CurrentTableName].Remove(proc);
            RaisePropertyChanged(nameof(Procedures));
        }
        public void RefreshProcedure()
        {
            RaisePropertyChanged(nameof(Procedures));
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

}
