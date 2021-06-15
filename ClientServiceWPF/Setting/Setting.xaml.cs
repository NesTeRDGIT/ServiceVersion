using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace ClientServiceWPF
{
    /// <summary>
    /// Логика взаимодействия для Setting.xaml
    /// </summary>
    public partial class Setting : Window
    {
        private IWcfInterface wcf => LoginForm.wcf;

        private SchemaCollection sc;
        private SchemaCollection sc_local;
        
        private CollectionViewSource CVSSchemaElementValue;
        private CollectionViewSource CVSTableItems;
        private CollectionViewSource CVSTableItemsLOCAL;
        private CollectionViewSource CVSSeqItemsLOCAL;

        private CollectionViewSource CVSTableItemsTRANS;
        private CollectionViewSource CVSCheckTableName;
        private CollectionViewSource CVSOrclProcedure;
        private CollectionViewSource CVSSchemaElementValueLocal;
        private bool OnlyLocal;


        CheckingList checList = new CheckingList();

        public Setting(bool OnlyLocal)
        {
            InitializeComponent();
            this.OnlyLocal = OnlyLocal;
            CVSSchemaElementValue = (CollectionViewSource) FindResource("CVSSchemaElementValue");
            CVSTableItems = (CollectionViewSource) FindResource("CVSTableItems");
            CVSTableItemsLOCAL = (CollectionViewSource)FindResource("CVSTableItemsLOCAL");
            CVSSeqItemsLOCAL = (CollectionViewSource)FindResource("CVSSeqItemsLOCAL");

            CVSTableItemsTRANS = (CollectionViewSource) FindResource("CVSTableItemsTRANS");
            CVSOrclProcedure = (CollectionViewSource) FindResource("CVSOrclProcedure");
            CVSCheckTableName = (CollectionViewSource) FindResource("CVSCheckTableName");
            CVSSchemaElementValueLocal = (CollectionViewSource)FindResource("CVSSchemaElementValueLocal");
        }

        void BlockTabs()
        {
            TabItemFolder.IsEnabled = !OnlyLocal;
            TabItemSchemaServer.IsEnabled = !OnlyLocal;
            TabItemConnectServer.IsEnabled = !OnlyLocal;
            TabItemTableServer.IsEnabled = !OnlyLocal;
            TabItemTransfer.IsEnabled = !OnlyLocal;
            textBoxTimeAwait.IsEnabled = !OnlyLocal;
            textBoxISP.IsEnabled = !OnlyLocal;
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
            BlockTabs();
            if (!OnlyLocal)
            {
                WriteServerConnect();
                CreateTableItems();
                CreateTableItemsTRANS();
                WriteFolder();
                WriteSchema();
                WriteCheckList();
                ButtonLoadCheckFromServer_Click(ButtonLoadCheckFromServer, new RoutedEventArgs());
            }
            SetStaticParam();
            WriteLocalConnect();
            CreateTableItemsLOCAL();
            WriteSchemaLOCAL();
        }

        public List<TableName> CheckTableName { get; set; } = new List<TableName>();

        private void WriteCheckList()
        {
            CheckTableName.Clear();
            CheckTableName.AddRange(checList.GeTableNames());
            CVSCheckTableName.View.Refresh();
        }
        private void SetStaticParam()
        {
            checkBoxIsVirtualPath.IsChecked = Properties.Settings.Default.ISVIRTUALPATH;
            textBoxVirtualPath.Text = Properties.Settings.Default.VIRTUALPATH;
        }
        private void GetStaticParam()
        {
            Properties.Settings.Default.ISVIRTUALPATH = checkBoxIsVirtualPath.IsChecked == true;
            Properties.Settings.Default.VIRTUALPATH = textBoxVirtualPath.Text;
        }
        private void WriteSchema()
        {
            foreach (var ft in (FileType[]) Enum.GetValues(typeof(FileType)))
            {
                listBoxTypeSchema.Items.Add(ft);
            }

            sc = wcf.GetSchemaCollection();

            foreach (var ver in sc.Versions)
            {
                comboBoxVersionSc.Items.Add(ver);
            }

            if (comboBoxVersionSc.Items.Count != 0)
                comboBoxVersionSc.SelectedIndex = 0;
            listBoxTypeSchema.SelectedIndex = 0;
        }
        private static string LocalFolder => AppDomain.CurrentDomain.BaseDirectory;
        private void WriteSchemaLOCAL()
        {
            foreach (var ft in (FileType[])Enum.GetValues(typeof(FileType)))
            {
                listBoxTypeSchemaLOCAL.Items.Add(ft);
            }

            sc_local = new SchemaCollection();
            if (File.Exists(System.IO.Path.Combine(LocalFolder, "SANK_INVITER_SCHEMA.dat")))
                sc_local.LoadFromFile(System.IO.Path.Combine(LocalFolder, "SANK_INVITER_SCHEMA.dat"));

            foreach (var ver in sc_local.Versions)
            {
                comboBoxVersionScLOCAL.Items.Add(ver);
            }

            if (comboBoxVersionScLOCAL.Items.Count != 0)
                comboBoxVersionScLOCAL.SelectedIndex = 0;
            listBoxTypeSchemaLOCAL.SelectedIndex = 0;
        }
        private void WriteFolder()
        {
            var set = wcf.GetSettingsFolder();
            textBoxIncomingDir.Text = set.IncomingDir;
            textBoxInputDir.Text = set.InputDir;
            textBoxErrorDir.Text = set.ErrorDir;
            textBoxProcessDir.Text = set.ProcessDir;
            textBoxErrorMessageFile.Text = set.ErrorMessageFile;
            textBoxAddDIRInERROR.Text = set.AddDIRInERROR;
            textBoxTimeAwait.Text = set.TimePacketOpen.ToString();
            textBoxISP.Text = set.ISP;
        }
        private SettingsFolder ReadFolder()
        {
            var set = new SettingsFolder();
            set.IncomingDir = textBoxIncomingDir.Text;
            set.InputDir = textBoxInputDir.Text;
            set.ErrorDir = textBoxErrorDir.Text;
            set.ProcessDir = textBoxProcessDir.Text;
            set.ErrorMessageFile = textBoxErrorMessageFile.Text;
            set.AddDIRInERROR = textBoxAddDIRInERROR.Text;
            set.TimePacketOpen = Convert.ToInt32(textBoxTimeAwait.Text);
            set.ISP = textBoxISP.Text;
            return set;
        }
        string ReadLocalConnect()
        {
            var conn = new OracleConnectionStringBuilder();
            conn.DataSource = $"{textBoxHOSTORA.Text}:{textBoxPORTORA.Text}/{textBoxBDORA.Text}";
            conn.UserID = textBoxLOGORA.Text;
            conn.Password = passwordBoxPASSORA.Password;
            conn.DBAPrivilege = comboBoxPRIVORA.Text.In("SYSOPER", "SYSDBA") ? comboBoxPRIVORA.Text : "";
            return conn.ConnectionString;
        }
        string ReadServerConnect()
        {
            var conn = new OracleConnectionStringBuilder();
            conn.DataSource = $"{textBoxHOSTORA_SERVER.Text}:{textBoxPORTORA_SERVER.Text}/{textBoxBDORA_SERVER.Text}";
            conn.UserID = textBoxLOGORA_SERVER.Text;
            conn.Password = passwordBoxPASSORA_SERVER.Password;
            conn.DBAPrivilege = comboBoxPRIVORA_SERVER.Text.In("SYSOPER", "SYSDBA") ? comboBoxPRIVORA_SERVER.Text : "";
            return conn.ConnectionString;
        }
        void WriteLocalConnect()
        {
            var sr = ServerRef.ParseDataSource(AppConfig.Property.ConnectionString);
            textBoxLOGORA.Text = sr.UserID;
            passwordBoxPASSORA.Password = sr.Password;
            comboBoxPRIVORA.Text = sr.DBAPrivilege;
            textBoxHOSTORA.Text = sr.HOST;
            textBoxPORTORA.Text = sr.PORT;
            textBoxBDORA.Text = sr.BD;
        }
        void WriteServerConnect()
        {
            var set = wcf.GetSettingConnect();
            
            var sr = ServerRef.ParseDataSource(set.ConnectingString);
            textBoxLOGORA_SERVER.Text = sr.UserID;
            passwordBoxPASSORA_SERVER.Password = sr.Password;
            comboBoxPRIVORA_SERVER.Text = sr.DBAPrivilege;
            textBoxHOSTORA_SERVER.Text = sr.HOST;
            textBoxPORTORA_SERVER.Text = sr.PORT;
            textBoxBDORA_SERVER.Text = sr.BD;
        }







        private void buttonORA_OK_Click(object sender, RoutedEventArgs e)
        {
            AppConfig.Property.ConnectionString = ReadLocalConnect();
           
        }

        private void buttonORA_Return_Click(object sender, RoutedEventArgs e)
        {
            WriteLocalConnect();
        }

        private void buttonORATEST_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var con = new OracleConnection(ReadLocalConnect());
                con.Open();
                con.Close();
                imageORATEST.Source = new BitmapImage(new Uri("Image/button_ok.png", UriKind.Relative));
                textBlockORA.Text = "Подключение успешно";
                textBlockORA.Foreground = Brushes.LimeGreen;
                System.Media.SystemSounds.Asterisk.Play();
            }
            catch (Exception ex)
            {
                imageORATEST.Source = new BitmapImage(new Uri("Image/error.png", UriKind.Relative));
                textBlockORA.Text = "Ошибка подключения" + Environment.NewLine + ex.Message;
                textBlockORA.Foreground = Brushes.Red;
                System.Media.SystemSounds.Exclamation.Play();
            }
        }

       

        private void textBoxPORTORA_TextChanged(object sender, TextChangedEventArgs e)
        {
            int o;
            if (!int.TryParse(textBoxPORTORA.Text, out o))
            {
                e.Handled = true;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           
        }

        private void ButtonIncomingDir_Click(object sender, RoutedEventArgs e)
        {
            var di = new RemoteFolderDialog(textBoxIncomingDir.Text, true);
            if (di.ShowDialog() == true)
            {
                textBoxIncomingDir.Text =  di.selectpath;
            }
        }

        private void ButtonInputDir_Click(object sender, RoutedEventArgs e)
        {
            var di = new RemoteFolderDialog(textBoxInputDir.Text, true);
            if (di.ShowDialog() == true)
            {
                textBoxInputDir.Text =  di.selectpath;
            }
        }

        private void ButtonErrorDir_Click(object sender, RoutedEventArgs e)
        {
            var di = new RemoteFolderDialog(textBoxErrorDir.Text, true);
            if (di.ShowDialog() == true)
            {
                textBoxErrorDir.Text =  di.selectpath;
            }
        }

        private void ButtonProcessDir_Click(object sender, RoutedEventArgs e)
        {
            var di = new RemoteFolderDialog(textBoxProcessDir.Text, true);
            if (di.ShowDialog() == true)
            {
                textBoxProcessDir.Text = di.selectpath;
            }
        }

        private void ButtonErrorMessageFile_Click(object sender, RoutedEventArgs e)
        {
            var di = new RemoteFolderDialog(textBoxErrorMessageFile.Text, true);
            if (di.ShowDialog() == true)
            {
                textBoxErrorMessageFile.Text =  di.selectpath;
            }
        }

        private void ButtonAddDIRInERROR_Click(object sender, RoutedEventArgs e)
        {
            var di = new RemoteFolderDialog(textBoxAddDIRInERROR.Text, true);
            if (di.ShowDialog() == true)
            {
                textBoxAddDIRInERROR.Text = di.selectpath;
            }
            
        }
        VersionMP version = VersionMP.V2_1;
        private void comboBoxVersionSc_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxVersionSc.SelectedItem != null)
            {
                version = (VersionMP)comboBoxVersionSc.SelectedItem;
                RefreshLBZglvVers();
                UpdatedataGrid();
            }
        }

        void RefreshLBZglvVers()
        {
            listBoxVers.Items.Clear();
            if (sc.ContainsVersion(version))
            {
                foreach (string val in sc[version].VersionsZGLV)
                {
                    listBoxVers.Items.Add(val);
                }
            }
        }


       

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (sc.ContainsVersion(version))
            {
                sc[version].VersionsZGLV.Add(textBoxNewVers.Text.ToUpper().Trim());
                RefreshLBZglvVers();
            }
        }

        private void listBoxTypeSchema_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatedataGrid();
        }


        void UpdatedataGrid()
        {
            if (listBoxTypeSchema.SelectedIndex != -1)
                SetSchemaSet(sc[version, (FileType)listBoxTypeSchema.SelectedIndex]);
        }

        void SetSchemaSet(List<SchemaElementValue> val)
        {
            CVSSchemaElementValue.Source = val;
        }
        private List<FileType> selectedTypeSchema => listBoxTypeSchema.SelectedItems.Cast<FileType>().ToList();
        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            if (selectedTypeSchema.Count != 0)
            {
                var win = new NewSchemaItem(false);
                if (win.ShowDialog() == true)
                {
                    foreach (var item in selectedTypeSchema)
                    {
                        try
                        {
                            sc[version].AddAndCheck(item,new SchemaElementValue { DATE_B = win.DATE_B, DATE_E = win.DATE_E, Value = win.PATH });
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($@"Не удалось добавить схему к файлу {item} версии {version} по причине: {ex.Message}");
                        }

                    }

                    CVSSchemaElementValue.View.Refresh();
                }
            }
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            if (selectedTypeSchema.Count != 0)
            {
                foreach (var item in selectedTypeSchema)
                {
                    try
                    {
                        sc[version].ClearSchema(item);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $@"Не удалось добавить схему к файлу {item} версии {version} по причине: {ex.Message}");
                    }
                }
                CVSSchemaElementValue.View.Refresh();
            }
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            if (listBoxVers.SelectedItem != null)
            {
                sc[version].VersionsZGLV.Remove(listBoxVers.SelectedItem.ToString());
                RefreshLBZglvVers();
            }
        }


        private string GetTableItems(TableItemType type)
        {
           return TableItems.FirstOrDefault(x => x.Type == type)?.TableName??"";
        }
        public List<TableItem> TableItems { get; set; } = new List<TableItem>();

        private void CreateTableItems()
        {
            var setCon = wcf.GetSettingConnect();
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
            TableItems.Add(new TableItem { TableName = setCon.xml_l_zglv, Type = TableItemType.XML_L_ZGLV });
            TableItems.Add(new TableItem { TableName = setCon.xml_l_pers, Type = TableItemType.XML_L_PERS });
            TableItems.Add(new TableItem { TableName = setCon.v_xml_error, Type = TableItemType.V_XML_ERROR });
            CVSTableItems.View.Refresh();
            textBoxTableOwner.Text = setCon.schemaOracle;
        }
        private SettingConnect ReadTableItems()
        {
            var setCon = new SettingConnect();
            setCon.schemaOracle = textBoxTableOwner.Text;
            setCon.xml_h_zglv = GetTableItems(TableItemType.XML_H_ZGLV);
            setCon.xml_h_schet = GetTableItems(TableItemType.XML_H_SCHET);
            setCon.xml_h_zap = GetTableItems(TableItemType.XML_H_ZAP);
            setCon.xml_h_pacient = GetTableItems(TableItemType.XML_H_PACIENT);
            setCon.xml_h_z_sluch = GetTableItems(TableItemType.XML_H_Z_SLUCH);
            setCon.xml_h_sank_smo = GetTableItems(TableItemType.XML_H_SANK_SMO);
            setCon.xml_h_sank_code_exp = GetTableItems(TableItemType.XML_H_SANK_CODE_EXP);
            setCon.xml_h_sluch = GetTableItems(TableItemType.XML_H_SLUCH);
            setCon.xml_h_kslp = GetTableItems(TableItemType.XML_H_KSLP);
            setCon.xml_h_crit = GetTableItems(TableItemType.XML_H_CRIT);
            setCon.xml_h_napr = GetTableItems(TableItemType.XML_H_NAPR);
            setCon.xml_h_b_diag = GetTableItems(TableItemType.XML_H_B_DIAG);
            setCon.xml_h_b_prot = GetTableItems(TableItemType.XML_H_B_PROT);
            setCon.xml_h_cons = GetTableItems(TableItemType.XML_H_CONS);
            setCon.xml_h_ds2 = GetTableItems(TableItemType.XML_H_DS2);
            setCon.xml_h_ds3 = GetTableItems(TableItemType.XML_H_DS3);
            setCon.xml_h_ds2_n = GetTableItems(TableItemType.XML_H_DS2_N);
            setCon.xml_h_nazr = GetTableItems(TableItemType.XML_H_NAZR);
            setCon.xml_h_onk_usl = GetTableItems(TableItemType.XML_H_ONK_USL);
            setCon.xml_h_lek_pr = GetTableItems(TableItemType.XML_H_LEK_PR);
            setCon.xml_h_lek_pr_date_inj = GetTableItems(TableItemType.XML_H_LEK_PR_DATE_INJ);
            setCon.xml_h_usl = GetTableItems(TableItemType.XML_H_USL);
            setCon.xml_l_zglv = GetTableItems(TableItemType.XML_L_ZGLV);
            setCon.xml_l_pers = GetTableItems(TableItemType.XML_L_PERS);
            setCon.v_xml_error = GetTableItems(TableItemType.V_XML_ERROR);
            return setCon;
        }

     
        private string GetTableItemsLOCAL(TableItemType type)
        {
            return TableItemsLOCAL.FirstOrDefault(x => x.Type == type)?.TableName ?? "";
        }
        public List<TableItem> TableItemsLOCAL { get; set; } = new List<TableItem>();


        private string GetSeqItemsLOCAL(TableItemType type)
        {
            return SeqItemsLOCAL.FirstOrDefault(x => x.Type == type)?.TableName ?? "";
        }
        public List<TableItem> SeqItemsLOCAL { get; set; } = new List<TableItem>();

        private void CreateTableItemsLOCAL()
        {
           
            TableItemsLOCAL.Clear();
            TableItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.xml_h_zglv, Type = TableItemType.XML_H_ZGLV });
            TableItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.xml_h_schet, Type = TableItemType.XML_H_SCHET });
            TableItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.xml_h_zap, Type = TableItemType.XML_H_ZAP });
            TableItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.xml_h_pacient, Type = TableItemType.XML_H_PACIENT });
            TableItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.xml_h_z_sluch, Type = TableItemType.XML_H_Z_SLUCH });
            TableItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.xml_h_sank, Type = TableItemType.XML_H_SANK_SMO });
            TableItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.xml_h_code_exp, Type = TableItemType.XML_H_SANK_CODE_EXP });
            TableItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.xml_h_sluch, Type = TableItemType.XML_H_SLUCH });
            TableItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.xml_h_kslp, Type = TableItemType.XML_H_KSLP });
            TableItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.xml_h_crit, Type = TableItemType.XML_H_CRIT });
            TableItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.xml_h_napr, Type = TableItemType.XML_H_NAPR });
            TableItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.xml_h_b_diag, Type = TableItemType.XML_H_B_DIAG });
            TableItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.xml_h_b_prot, Type = TableItemType.XML_H_B_PROT });
            TableItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.xml_h_cons, Type = TableItemType.XML_H_CONS });
            TableItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.xml_h_ds2, Type = TableItemType.XML_H_DS2 });
            TableItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.xml_h_ds3, Type = TableItemType.XML_H_DS3 });
            TableItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.xml_h_ds2_n, Type = TableItemType.XML_H_DS2_N });
            TableItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.xml_h_nazr, Type = TableItemType.XML_H_NAZR });
            TableItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.xml_h_onk_usl, Type = TableItemType.XML_H_ONK_USL });
            TableItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.xml_h_lek_pr, Type = TableItemType.XML_H_LEK_PR });
            TableItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.xml_h_date_inj, Type = TableItemType.XML_H_LEK_PR_DATE_INJ });
            TableItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.xml_h_usl, Type = TableItemType.XML_H_USL });
            TableItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.xml_l_zglv, Type = TableItemType.XML_L_ZGLV });
            TableItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.xml_l_pers, Type = TableItemType.XML_L_PERS });
            textBoxTableOwnerLOCAL.Text = AppConfig.Property.schemaOracle;


            SeqItemsLOCAL.Clear();
            SeqItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.seq_ZGLV, Type = TableItemType.seq_ZGLV });
            SeqItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.seq_SCHET, Type = TableItemType.seq_SCHET });
            SeqItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.seq_ZAP, Type = TableItemType.seq_ZAP });
            SeqItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.seq_PACIENT, Type = TableItemType.seq_PACIENT });
            SeqItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.seq_z_sluch, Type = TableItemType.seq_z_sluch });
            SeqItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.seq_SANK, Type = TableItemType.seq_SANK });
            SeqItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.seq_SLUCH, Type = TableItemType.seq_SLUCH });
            SeqItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.seq_USL, Type = TableItemType.seq_USL });
            SeqItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.seq_L_ZGLV, Type = TableItemType.seq_L_ZGLV });
            SeqItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.seq_L_pers, Type = TableItemType.seq_L_pers });
            SeqItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.seq_xml_h_onk_usl, Type = TableItemType.seq_xml_h_onk_usl });
            SeqItemsLOCAL.Add(new TableItem { TableName = AppConfig.Property.seq_xml_h_lek_pr, Type = TableItemType.seq_xml_h_lek_pr });


            CVSTableItemsLOCAL.View.Refresh();
            CVSSeqItemsLOCAL.View.Refresh();

        }
        private void ReadTableItemsLOCAL()
        {
            AppConfig.Property.xml_h_zglv = GetTableItemsLOCAL(TableItemType.XML_H_ZGLV);
            AppConfig.Property.xml_h_schet = GetTableItemsLOCAL(TableItemType.XML_H_SCHET);
            AppConfig.Property.xml_h_zap = GetTableItemsLOCAL(TableItemType.XML_H_ZAP);
            AppConfig.Property.xml_h_pacient = GetTableItemsLOCAL(TableItemType.XML_H_PACIENT);
            AppConfig.Property.xml_h_z_sluch = GetTableItemsLOCAL(TableItemType.XML_H_Z_SLUCH);
            AppConfig.Property.xml_h_sank = GetTableItemsLOCAL(TableItemType.XML_H_SANK_SMO);
            AppConfig.Property.xml_h_code_exp = GetTableItemsLOCAL(TableItemType.XML_H_SANK_CODE_EXP);
            AppConfig.Property.xml_h_sluch = GetTableItemsLOCAL(TableItemType.XML_H_SLUCH);
            AppConfig.Property.xml_h_kslp = GetTableItemsLOCAL(TableItemType.XML_H_KSLP);
            AppConfig.Property.xml_h_crit = GetTableItemsLOCAL(TableItemType.XML_H_CRIT);
            AppConfig.Property.xml_h_napr = GetTableItemsLOCAL(TableItemType.XML_H_NAPR);
            AppConfig.Property.xml_h_b_diag = GetTableItemsLOCAL(TableItemType.XML_H_B_DIAG);
            AppConfig.Property.xml_h_b_prot = GetTableItemsLOCAL(TableItemType.XML_H_B_PROT);
            AppConfig.Property.xml_h_cons = GetTableItemsLOCAL(TableItemType.XML_H_CONS);
            AppConfig.Property.xml_h_ds2 = GetTableItemsLOCAL(TableItemType.XML_H_DS2);
            AppConfig.Property.xml_h_ds3 = GetTableItemsLOCAL(TableItemType.XML_H_DS3);
            AppConfig.Property.xml_h_ds2_n = GetTableItemsLOCAL(TableItemType.XML_H_DS2_N);
            AppConfig.Property.xml_h_nazr = GetTableItemsLOCAL(TableItemType.XML_H_NAZR);
            AppConfig.Property.xml_h_onk_usl = GetTableItemsLOCAL(TableItemType.XML_H_ONK_USL);
            AppConfig.Property.xml_h_lek_pr = GetTableItemsLOCAL(TableItemType.XML_H_LEK_PR);
            AppConfig.Property.xml_h_date_inj = GetTableItemsLOCAL(TableItemType.XML_H_LEK_PR_DATE_INJ);
            AppConfig.Property.xml_h_usl = GetTableItemsLOCAL(TableItemType.XML_H_USL);
            AppConfig.Property.xml_l_zglv = GetTableItemsLOCAL(TableItemType.XML_L_ZGLV);
            AppConfig.Property.xml_l_pers = GetTableItemsLOCAL(TableItemType.XML_L_PERS);
            AppConfig.Property.schemaOracle = textBoxTableOwnerLOCAL.Text;


            AppConfig.Property.seq_ZGLV = GetSeqItemsLOCAL(TableItemType.seq_ZGLV);
            AppConfig.Property.seq_SCHET = GetSeqItemsLOCAL(TableItemType.seq_SCHET);
            AppConfig.Property.seq_ZAP = GetSeqItemsLOCAL(TableItemType.seq_ZAP);
            AppConfig.Property.seq_PACIENT = GetSeqItemsLOCAL(TableItemType.seq_PACIENT);
            AppConfig.Property.seq_z_sluch = GetSeqItemsLOCAL(TableItemType.seq_z_sluch);
            AppConfig.Property.seq_SANK = GetSeqItemsLOCAL(TableItemType.seq_SANK);
            AppConfig.Property.seq_SLUCH = GetSeqItemsLOCAL(TableItemType.seq_SLUCH);
            AppConfig.Property.seq_USL = GetSeqItemsLOCAL(TableItemType.seq_USL);
            AppConfig.Property.seq_L_ZGLV = GetSeqItemsLOCAL(TableItemType.seq_L_ZGLV);
            AppConfig.Property.seq_L_pers = GetSeqItemsLOCAL(TableItemType.seq_L_pers);
            AppConfig.Property.seq_xml_h_onk_usl = GetSeqItemsLOCAL(TableItemType.seq_xml_h_onk_usl);
            AppConfig.Property.seq_xml_h_lek_pr = GetSeqItemsLOCAL(TableItemType.seq_xml_h_lek_pr);
        }
        private string GetTableItemsTRANS(TableItemType type)
        {
            return TableItemsTRANS.FirstOrDefault(x => x.Type == type)?.TableName ?? "";
        }
        public List<TableItem> TableItemsTRANS { get; set; } = new List<TableItem>();
        private void CreateTableItemsTRANS()
        {
            var setTrans = wcf.GetSettingTransfer();
            TableItemsTRANS.Clear();
            TableItemsTRANS.Add(new TableItem { TableName = setTrans.xml_h_zglv, Type = TableItemType.XML_H_ZGLV });
            TableItemsTRANS.Add(new TableItem { TableName = setTrans.xml_h_schet, Type = TableItemType.XML_H_SCHET });
            TableItemsTRANS.Add(new TableItem { TableName = setTrans.xml_h_zap, Type = TableItemType.XML_H_ZAP });
            TableItemsTRANS.Add(new TableItem { TableName = setTrans.xml_h_pacient, Type = TableItemType.XML_H_PACIENT });
            TableItemsTRANS.Add(new TableItem { TableName = setTrans.xml_h_z_sluch, Type = TableItemType.XML_H_Z_SLUCH });
            TableItemsTRANS.Add(new TableItem { TableName = setTrans.xml_h_sank_smo, Type = TableItemType.XML_H_SANK_SMO });
            TableItemsTRANS.Add(new TableItem { TableName = setTrans.xml_h_sank_code_exp, Type = TableItemType.XML_H_SANK_CODE_EXP });
            TableItemsTRANS.Add(new TableItem { TableName = setTrans.xml_h_sluch, Type = TableItemType.XML_H_SLUCH });
            TableItemsTRANS.Add(new TableItem { TableName = setTrans.xml_h_kslp, Type = TableItemType.XML_H_KSLP });
            TableItemsTRANS.Add(new TableItem { TableName = setTrans.xml_h_crit, Type = TableItemType.XML_H_CRIT });
            TableItemsTRANS.Add(new TableItem { TableName = setTrans.xml_h_napr, Type = TableItemType.XML_H_NAPR });
            TableItemsTRANS.Add(new TableItem { TableName = setTrans.xml_h_b_diag, Type = TableItemType.XML_H_B_DIAG });
            TableItemsTRANS.Add(new TableItem { TableName = setTrans.xml_h_b_prot, Type = TableItemType.XML_H_B_PROT });
            TableItemsTRANS.Add(new TableItem { TableName = setTrans.xml_h_cons, Type = TableItemType.XML_H_CONS });
            TableItemsTRANS.Add(new TableItem { TableName = setTrans.xml_h_ds2, Type = TableItemType.XML_H_DS2 });
            TableItemsTRANS.Add(new TableItem { TableName = setTrans.xml_h_ds3, Type = TableItemType.XML_H_DS3 });
            TableItemsTRANS.Add(new TableItem { TableName = setTrans.xml_h_ds2_n_transfer, Type = TableItemType.XML_H_DS2_N });
            TableItemsTRANS.Add(new TableItem { TableName = setTrans.xml_h_nazr_transfer, Type = TableItemType.XML_H_NAZR });
            TableItemsTRANS.Add(new TableItem { TableName = setTrans.xml_h_onk_usl, Type = TableItemType.XML_H_ONK_USL });
            TableItemsTRANS.Add(new TableItem { TableName = setTrans.xml_h_lek_pr, Type = TableItemType.XML_H_LEK_PR });
            TableItemsTRANS.Add(new TableItem { TableName = setTrans.xml_h_lek_pr_date_inj, Type = TableItemType.XML_H_LEK_PR_DATE_INJ });
            TableItemsTRANS.Add(new TableItem { TableName = setTrans.xml_h_usl, Type = TableItemType.XML_H_USL });
            TableItemsTRANS.Add(new TableItem { TableName = setTrans.xml_l_zglv, Type = TableItemType.XML_L_ZGLV });
            TableItemsTRANS.Add(new TableItem { TableName = setTrans.xml_l_pers, Type = TableItemType.XML_L_PERS });
         
            CVSTableItemsTRANS.View.Refresh();
            textBoxTRANSOwner.Text = setTrans.schemaOracle;
            CheckBoxEnableTRANS.IsChecked = setTrans.Transfer;
        }

        private SettingTransfer ReadTableItemsTRANS()
        {
            var setTrans = new SettingTransfer();
            setTrans.schemaOracle = textBoxTableOwner.Text;
            setTrans.Transfer = CheckBoxEnableTRANS.IsChecked == true;
            setTrans.xml_h_zglv = GetTableItemsTRANS(TableItemType.XML_H_ZGLV);
            setTrans.xml_h_schet = GetTableItemsTRANS(TableItemType.XML_H_SCHET);
            setTrans.xml_h_zap = GetTableItemsTRANS(TableItemType.XML_H_ZAP);
            setTrans.xml_h_pacient = GetTableItemsTRANS(TableItemType.XML_H_PACIENT);
            setTrans.xml_h_z_sluch = GetTableItemsTRANS(TableItemType.XML_H_Z_SLUCH);
            setTrans.xml_h_sank_smo = GetTableItemsTRANS(TableItemType.XML_H_SANK_SMO);
            setTrans.xml_h_sank_code_exp = GetTableItemsTRANS(TableItemType.XML_H_SANK_CODE_EXP);
            setTrans.xml_h_sluch = GetTableItemsTRANS(TableItemType.XML_H_SLUCH);
            setTrans.xml_h_kslp = GetTableItemsTRANS(TableItemType.XML_H_KSLP);
            setTrans.xml_h_crit = GetTableItemsTRANS(TableItemType.XML_H_CRIT);
            setTrans.xml_h_napr = GetTableItemsTRANS(TableItemType.XML_H_NAPR);
            setTrans.xml_h_b_diag = GetTableItemsTRANS(TableItemType.XML_H_B_DIAG);
            setTrans.xml_h_b_prot = GetTableItemsTRANS(TableItemType.XML_H_B_PROT);
            setTrans.xml_h_cons = GetTableItemsTRANS(TableItemType.XML_H_CONS);
            setTrans.xml_h_ds2 = GetTableItemsTRANS(TableItemType.XML_H_DS2);
            setTrans.xml_h_ds3 = GetTableItemsTRANS(TableItemType.XML_H_DS3);
            setTrans.xml_h_ds2_n_transfer = GetTableItemsTRANS(TableItemType.XML_H_DS2_N);
            setTrans.xml_h_nazr_transfer = GetTableItemsTRANS(TableItemType.XML_H_NAZR);
            setTrans.xml_h_onk_usl = GetTableItemsTRANS(TableItemType.XML_H_ONK_USL);
            setTrans.xml_h_lek_pr = GetTableItemsTRANS(TableItemType.XML_H_LEK_PR);
            setTrans.xml_h_lek_pr_date_inj = GetTableItemsTRANS(TableItemType.XML_H_LEK_PR_DATE_INJ);
            setTrans.xml_h_usl = GetTableItemsTRANS(TableItemType.XML_H_USL);
            setTrans.xml_l_zglv = GetTableItemsTRANS(TableItemType.XML_L_ZGLV);
            setTrans.xml_l_pers = GetTableItemsTRANS(TableItemType.XML_L_PERS);
            return setTrans;
        }
        private void ButtonCheckTable_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var set = ReadTableItems();
                var tblrez = wcf.GetTableServer(set.schemaOracle);
                if (tblrez.Result == null)
                {
                    throw new Exception(tblrez.Exception);
                }

                foreach (var tbl in TableItems)
                {
                    tbl.Check = tblrez.Result.Select($"TABLE_NAME = '{tbl.TableName.ToUpper()}'").Length != 0;
                }
                CVSTableItems.View.Refresh();
                System.Media.SystemSounds.Asterisk.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
          
        }

        private void ButtonCheckTRANS_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var set = ReadTableItemsTRANS();
                var tblrez = wcf.GetTableServer(set.schemaOracle);
                if (tblrez.Result == null)
                {
                    throw new Exception(tblrez.Exception);
                }

                foreach (var tbl in TableItemsTRANS)
                {
                    tbl.Check = tblrez.Result.Select($"TABLE_NAME = '{tbl.TableName.ToUpper()}'").Length != 0;
                }
                CVSTableItemsTRANS.View.Refresh();
                System.Media.SystemSounds.Asterisk.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void buttonBrouseVirtualPath_Click(object sender, RoutedEventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBoxVirtualPath.Text = fbd.SelectedPath;
                
            }
        }
        TableName? CurrentCheckTableName => (TableName?) ListBoxCheckTableName.SelectedItem;
        List<TableName> SelectedCheckTableName => ListBoxCheckTableName.SelectedItems.Cast<TableName>().ToList();
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            refreshListViewChek_ALL();
        }

        void refreshListViewChek_ALL()
        {
            var select = CurrentCheckTableName;
            if (select.HasValue)
            {
                CVSOrclProcedure.Source = checList[select.Value];
                CVSOrclProcedure.View.Refresh();
            }
        }
        private void ButtonAddPacket_Click(object sender, RoutedEventArgs e)
        {
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
            }
        }

        private OpenFileDialog openFileDialog1 = new OpenFileDialog{Filter = @"Файл проверок(CheckListFile(*.clf))|*.clv"}; 
        private void ButtonLoadCheckFromFile_Click(object sender, RoutedEventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var res = checList.LoadToFile(openFileDialog1.FileName);
                if (res == false) MessageBox.Show("Не удалось загрузить файл!");
                refreshListViewChek_ALL();
            }
        }
        private SaveFileDialog saveFileDialog1 = new SaveFileDialog { Filter = @"Файл проверок(CheckListFile(*.clf))|*.clv" };
        private void ButtonSaveCheckInFile_Click(object sender, RoutedEventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                checList.SaveToFile(saveFileDialog1.FileName);
        }

        private void ButtonCheckListCheck_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var tmp = wcf.ExecuteCheckAv(checList);
                if (tmp == null)
                {
                    System.Windows.Forms.MessageBox.Show($@"Ошибка при выполнении см. лог сервера", "", MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }
                checList = tmp;
                refreshListViewChek_ALL();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            System.Windows.Forms.MessageBox.Show(@"Выполнено", "", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void ButtonSaveCheckInServer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var res = wcf.SetCheckingList(checList);
                MessageBox.Show(res.Result? "Передача настроек успешна!" : $"Ошибка при передаче настроек: {res.Exception}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ButtonLoadCheckFromServer_Click(object sender, RoutedEventArgs e)
        {
            var br = wcf.LoadCheckListFromBD();
            if (br.Result == false)
            {
                MessageBox.Show(br.Exception);
                return;
            }
            checList = wcf.GetCheckingList();
            refreshListViewChek_ALL();
        }

        private List<OrclProcedure> currOrclProcedure => ListViewCheck.SelectedItems.Cast<OrclProcedure>().ToList();


        private void MenuItemCheckDouble_Click(object sender, RoutedEventArgs e)
        {
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
            }
        }

        private void MenuItemCheckDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var selected = SelectedCheckTableName;
                if (selected.Count!=0)
                {
                    if (MessageBox.Show($"Вы уверены что хотите удалить проверк{(selected.Count==1? "у" : "и")}?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        var proc = currOrclProcedure;
                        foreach (var item in selected)
                        {
                            checList[item].Remove(proc.First());
                        }
                        refreshListViewChek_ALL();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void MenuItemCheckAdd_Click(object sender, RoutedEventArgs e)
        {
            var select = CurrentCheckTableName;
            if (select.HasValue)
            {
                var proc = new OrclProcedure();
                var win = new EdditProc(proc, AppConfig.Property.ConnectionString) {Owner = this};
                if (win.ShowDialog() == true)
                {
                    checList.Add(select.Value, win.curr);
                    refreshListViewChek_ALL();

                }
            }
        }

        private void MenuItemCheckChange_Click(object sender, RoutedEventArgs e)
        {
            var orclselect = currOrclProcedure;
            if (orclselect.Count != 0)
            {
                var proc = orclselect.First();
                var win = new EdditProc(new OrclProcedure(proc), AppConfig.Property.ConnectionString) {Owner = this};
                if (win.ShowDialog() == true)
                {
                    proc.CopyFrom(win.curr);
                    refreshListViewChek_ALL();
                }
            }
        }

        private void buttonRestore_SERVER_Click(object sender, RoutedEventArgs e)
        {
            WriteServerConnect();
        }

        private void buttonOK_SERVER_Click(object sender, RoutedEventArgs e)
        {
           var setCon = wcf.GetSettingConnect();
            setCon.ConnectingString = ReadServerConnect();
            wcf.SettingConnect(setCon);
        }

        private void buttonTEST_SERVER_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var rez = wcf.isConnect(ReadServerConnect());
                if (rez.Result)
                {
                    imageTEST_SERVER.Source = new BitmapImage(new Uri("Image/button_ok.png", UriKind.Relative));
                    textBlockORA_TEST_SERVER.Text = "Подключение успешно";
                    textBlockORA_TEST_SERVER.Foreground = Brushes.LimeGreen;
                    System.Media.SystemSounds.Asterisk.Play();
                }
                else
                {
                    imageTEST_SERVER.Source = new BitmapImage(new Uri("Image/error.png", UriKind.Relative));
                    textBlockORA_TEST_SERVER.Text = "Ошибка подключения" + Environment.NewLine + rez.Exception;
                    textBlockORA_TEST_SERVER.Foreground = Brushes.Red;
                    System.Media.SystemSounds.Exclamation.Play();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!OnlyLocal)
                {
                    wcf.SettingsFolder(ReadFolder());
                    wcf.SettingSchemaCollection(sc);
                    var set = ReadTableItems();
                    set.ConnectingString = ReadServerConnect();
                    wcf.SettingConnect(set);
                    wcf.SetCheckingList(checList);
                    wcf.SetSettingTransfer(ReadTableItemsTRANS());
                    wcf.SaveProperty();
                }
                GetStaticParam();
                ReadTableItemsLOCAL();
                sc_local.SaveToFile(System.IO.Path.Combine(LocalFolder, "SANK_INVITER_SCHEMA.dat"));
                AppConfig.Save();
                Properties.Settings.Default.Save();
                DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
          

        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Setting_OnClosed(object sender, EventArgs e)
        {
            AppConfig.Load();
            Properties.Settings.Default.Reload();
            if (!OnlyLocal)
                wcf.LoadProperty();
        }
        private List<FileType> selectedTypeSchemaLocal => listBoxTypeSchemaLOCAL.SelectedItems.Cast<FileType>().ToList();
        private void MenuItemAddSchemaLocal_OnClick(object sender, RoutedEventArgs e)
        {
            if (selectedTypeSchemaLocal.Count != 0)
            {
                var win = new NewSchemaItem(true);
                if (win.ShowDialog() == true)
                {
                    if (!win.PATH.ToUpper().Contains(LocalFolder.ToUpper()))
                    {
                        MessageBox.Show("Выбранный файл вне каталога программы!");
                    }

                    var path = win.PATH.ToUpper().Replace(LocalFolder.ToUpper(), "");
                    foreach (var item in selectedTypeSchemaLocal)
                    {
                        try
                        {
                            sc_local[versionLOCAL].AddAndCheck(item, new SchemaElementValue { DATE_B = win.DATE_B, DATE_E = win.DATE_E, Value = path });
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($@"Не удалось добавить схему к файлу {item} версии {version} по причине: {ex.Message}");
                        }

                    }

                    CVSSchemaElementValueLocal.View.Refresh();
                }
            }

        }


        private void MenuItemClearSchemaLocal_OnClick(object sender, RoutedEventArgs e)
        {
            if (selectedTypeSchemaLocal.Count != 0)
            {
                foreach (var item in selectedTypeSchemaLocal)
                {
                    try
                    {
                        sc_local[versionLOCAL].ClearSchema(item);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($@"Не удалось добавить схему к файлу {item} версии {version} по причине: {ex.Message}");
                    }
                }
                CVSSchemaElementValueLocal.View.Refresh();
            }
            
        }
        VersionMP versionLOCAL = VersionMP.V2_1;
        private void ComboBoxVersionScLOCAL_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxVersionScLOCAL.SelectedItem != null)
            {
                versionLOCAL = (VersionMP)comboBoxVersionScLOCAL.SelectedItem;
                RefreshLBZglvVersLOCAL();
                UpdatedataGridLOCAL();
            }
        }


        void RefreshLBZglvVersLOCAL()
        {
            listBoxVersLOCAL.Items.Clear();
            if (sc_local.ContainsVersion(version))
            {
                foreach (string val in sc_local[versionLOCAL].VersionsZGLV)
                {
                    listBoxVersLOCAL.Items.Add(val);
                }
            }
        }


        private void MenuItemDeleteVersLocal_OnClick(object sender, RoutedEventArgs e)
        {
            if (listBoxVersLOCAL.SelectedItem != null)
            {
                sc_local[versionLOCAL].VersionsZGLV.Remove(listBoxVersLOCAL.SelectedItem.ToString());
                RefreshLBZglvVersLOCAL();
            }
        }

        private void ButtonLOCAL_OnClick(object sender, RoutedEventArgs e)
        {
            if (sc_local.ContainsVersion(versionLOCAL))
            {
                sc_local[versionLOCAL].VersionsZGLV.Add(textBoxNewVersLOCAL.Text.ToUpper().Trim());
                RefreshLBZglvVersLOCAL();
            }
        }


        public DataTable GetTableLOCAL()
        {
            var oda = new OracleDataAdapter($@"SELECT TABLE_NAME FROM ALL_TABLES where  OWNER = '{AppConfig.Property.schemaOracle.ToUpper()}'", new OracleConnection(AppConfig.Property.ConnectionString));
            var tbl = new DataTable();
            oda.Fill(tbl);
            return tbl;
        }


        public DataTable GetSeqLOCAL()
        {
            var oda = new OracleDataAdapter($@"SELECT SEQUENCE_NAME FROM ALL_SEQUENCES where  SEQUENCE_OWNER = '{AppConfig.Property.schemaOracle.ToUpper()}'", new OracleConnection(AppConfig.Property.ConnectionString));
            var tbl = new DataTable();
            oda.Fill(tbl);
            return tbl;
        }

        private void ButtonCheckTableLOCAL_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var TBL = GetTableLOCAL();
                var SEQ = GetSeqLOCAL();

                foreach (var tbl in TableItemsLOCAL)
                {
                    tbl.Check = TBL.Select($"TABLE_NAME = '{tbl.TableName.ToUpper()}'").Length != 0;
                }

                foreach (var tbl in SeqItemsLOCAL)
                {
                    tbl.Check = SEQ.Select($"SEQUENCE_NAME = '{tbl.TableName.ToUpper()}'").Length != 0;
                }
                CVSTableItemsLOCAL.View.Refresh();
                CVSSeqItemsLOCAL.View.Refresh();
                System.Media.SystemSounds.Asterisk.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ListBoxTypeSchemaLOCAL_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatedataGridLOCAL();
        }
        void UpdatedataGridLOCAL()
        {
            if (listBoxTypeSchemaLOCAL.SelectedIndex != -1)
            {
                SetSchemaSetLOCAL(sc_local[versionLOCAL, (FileType)listBoxTypeSchemaLOCAL.SelectedIndex]);
            }
        }

        void SetSchemaSetLOCAL(List<SchemaElementValue> val)
        {
            CVSSchemaElementValueLocal.Source = val;
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
        seq_xml_h_lek_pr
    }
    public class TableItem
    {
        public TableItemType Type { get; set; }
        public string TableName { get; set; }
        public bool? Check { get; set; }
    }

 

   
}
