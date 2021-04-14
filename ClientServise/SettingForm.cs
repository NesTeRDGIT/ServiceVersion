using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Oracle.ManagedDataAccess.Client;
using ServiceLoaderMedpomData;
using System.ServiceModel;

namespace ClientServise
{
    public partial class SettingForm : Form
    {
        SchemaColection sc;
        OracleConnectionStringBuilder conn;
        SettingConnect setCon;
        SettingsFolder set;
        IWcfInterface WcfInterface => MainForm.MyWcfConnection;

        SettingTransfer setTrans;

        public SettingForm()
        {
            InitializeComponent();
            try
            {


                set = WcfInterface.GetSettingsFolder();
                setCon = WcfInterface.GetSettingConnect();
                sc = WcfInterface.GetSchemaColection();
                setTrans = WcfInterface.GetSettingTransfer();

                listTransferProc = WcfInterface.GetListTransfer();
                RefreshlistTransferProc();
                textBoxISP.Text = set.ISP;
              
                textBoxProcessDir.Text = set.ProcessDir;
                textBoxInputDir.Text = set.InputDir;
                textBoxErrorDir.Text = set.ErrorDir;
                textBoxDir.Text = set.IncomingDir;
                numericUpDown1.Value = set.TimePacketOpen;
                textBoxErrorMesDir.Text = set.ErrorMessageFile;
                textBoxAddDIRInERROR.Text = set.AddDIRInERROR;

                textBoxUserPriv.Text = WcfInterface.GetUserPriv();
                // SchemaCollection sc = new SchemaCollection();


                foreach (var ft in (FileType[]) Enum.GetValues(typeof(FileType)))
                {
                    listBoxTypeSchema.Items.Add(ft);
                }


                foreach (var ver in sc.Versions)
                {
                    comboBoxVersionSchema.Items.Add(ver);
                }

                if (comboBoxVersionSchema.Items.Count != 0)
                    comboBoxVersionSchema.SelectedIndex = 0;
                listBoxTypeSchema.SelectedIndex = 0;

                try
                {
                    conn = new OracleConnectionStringBuilder(setCon.ConnectingString);
                    ConnSet();
                }
                catch
                {
                    conn = new OracleConnectionStringBuilder();
                }


                pictureBoxH_PAC.BackgroundImage = null;
                pictureBoxH_SANK.BackgroundImage = null;
                pictureBoxH_SCHET.BackgroundImage = null;
                pictureBoxH_SLUCH.BackgroundImage = null;
                pictureBoxH_USL.BackgroundImage = null;
                pictureBoxH_ZAP.BackgroundImage = null;
                pictureBoxH_ZGL.BackgroundImage = null;
                pictureBoxL_PERS.BackgroundImage = null;
                pictureBoxL_ZGLV.BackgroundImage = null;
                pictureBoxH_NAZR.BackgroundImage = null;
                pictureBoxH_DS2.BackgroundImage = null;
                pictureBoxXML_ERRORS.BackgroundImage = null;
                pictureBoxZ_SLUCH.BackgroundImage = null;
                pictureBoxH_KOEF.BackgroundImage = null;

                pictureBoxPACto.BackgroundImage = null;
                pictureBoxSANKto.BackgroundImage = null;
                pictureBoxSCHETto.BackgroundImage = null;
                pictureBoxSLUCHto.BackgroundImage = null;
                pictureBoxUSLto.BackgroundImage = null;
                pictureBoxZAPto.BackgroundImage = null;
                pictureBoxH_ZGLVto.BackgroundImage = null;
                pictureBoxL_PERSto.BackgroundImage = null;
                pictureBoxL_ZGLVto.BackgroundImage = null;
                pictureBoxH_NAZRto.BackgroundImage = null;
                pictureBoxH_DS2_Nto.BackgroundImage = null;
                pictureBoxH_Z_SLUCHto.BackgroundImage = null;
                pictureBoxH_KOEFto.BackgroundImage = null;

                pictureBoxH_CONS.BackgroundImage = null;
                pictureBoxH_ONK_USL.BackgroundImage = null;
                pictureBoxDATE_INJ.BackgroundImage = null;
                pictureBoxH_LEK_PR.BackgroundImage = null;
                pictureBoxCODE_EXP.BackgroundImage = null;

                pictureBoxH_CONSto.BackgroundImage = null;
                pictureBoxH_ONK_USLto.BackgroundImage = null;
                pictureBoxDATE_INJto.BackgroundImage = null;
                pictureBoxH_LEK_PRto.BackgroundImage = null;
                pictureBoxCODE_EXPto.BackgroundImage = null;

                setTableName();

                try
                {
                    list = WcfInterface.GetChekingList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(" Не удалось загрузить список проверок", ex.Message);
                }

                if (list == null) list = new ChekingList();
                //    list = WcfInterface.GetChekingList();
                toolStripComboBoxTypeViwe.SelectedIndex = 0;
                refreshListViewChek_ALL();
                SetTransfer();
                // var ii = WcfInterface.GetImpersonInfo();
                //textBoxImpersDomen.Text = ii.Domen;
                //textBoxImpersPasword.Text = ii.Password;
                //textBoxImpersUser.Text = ii.Login;




                var s = WcfInterface.GetCheckClearProc();

                textBoxClearTemp100.Text = s[0];
                textBoxClearTemp1.Text = s[1];
                textBoxCheckTemp100.Text = s[2];
                textBoxCheckTemp1.Text = s[3];


                checkBoxISVIRTUALPATH.Checked = Properties.Settings.Default.ISVIRTUALPATH;
                textBoxVIRTUALPATH.Text = Properties.Settings.Default.VIRTUALPATH;


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
            }
        }

        private List<FileType> selectedTypeSchema => listBoxTypeSchema.SelectedItems.Cast<FileType>().ToList();


        void refreshListViewChek_ALL()
        {
            refreshListViewChek(TableName.ZGLV, listViewZglv);
            refreshListViewChek(TableName.ZAP, listViewZap);
            refreshListViewChek(TableName.USL, listViewUsl);
            refreshListViewChek(TableName.SLUCH, listViewSluch);
            refreshListViewChek(TableName.SCHET, listViewSchet);
            refreshListViewChek(TableName.SANK, listViewSank);
            refreshListViewChek(TableName.PACIENT, listViewPac);
            refreshListViewChek(TableName.L_ZGLV, listViewL_Zglv);
            refreshListViewChek(TableName.L_PERS, listViewL_Pers);
        }

        void setTableName()
        {
            textBoxH_PAC.Text = setCon.xml_h_pacient;
            textBoxH_SANK.Text = setCon.xml_h_sank_smo;
            textBoxH_SCHET.Text = setCon.xml_h_schet;
            textBoxH_SLUCH.Text = setCon.xml_h_sluch;
            textBoxH_USL.Text = setCon.xml_h_usl;
            textBoxH_ZAP.Text = setCon.xml_h_zap;
            textBoxH_ZGLV.Text = setCon.xml_h_zglv;
            textBoxL_PERS.Text = setCon.xml_l_pers;
            textBoxL_ZGLV.Text = setCon.xml_l_zglv;
            textBoxH_NAZR.Text = setCon.xml_h_nazr;
            textBoxH_DS2.Text = setCon.xml_h_ds2_n;

            textBoxH_CONS.Text = setCon.xml_h_cons;
            textBoxH_ONK_USL.Text = setCon.xml_h_onk_usl;
            textBoxH_LEK_PR.Text = setCon.xml_h_lek_pr;
            textBoxDATE_INJ.Text = setCon.xml_h_lek_pr_date_inj;

            textBoxORCLSchema.Text = setCon.schemaOracle;
            textBoxXML_ERRORS.Text = setCon.v_xml_error;
            textBoxZ_SLUCH.Text = setCon.xml_h_z_sluch;
            ;
            textBoxH_KOEF.Text = setCon.xml_h_kslp;
            ;

            textBoxNAPR.Text = setCon.xml_h_napr;

            textBoxB_PROT.Text = setCon.xml_h_b_prot;
            textBoxB_DIAG.Text = setCon.xml_h_b_diag;
            textBoxCODE_EXP.Text = setCon.xml_h_sank_code_exp;

            textBoxDS2.Text = setCon.xml_h_ds2;
            textBoxDS3.Text = setCon.xml_h_ds3;
            textBoxH_CRIT.Text = setCon.xml_h_crit;
        }

        void readTableName()
        {
            setCon.xml_h_pacient = textBoxH_PAC.Text;
            setCon.xml_h_sank_smo = textBoxH_SANK.Text;
            setCon.xml_h_schet = textBoxH_SCHET.Text;
            setCon.xml_h_sluch = textBoxH_SLUCH.Text;
            setCon.xml_h_usl = textBoxH_USL.Text;
            setCon.xml_h_zap = textBoxH_ZAP.Text;
            setCon.xml_h_zglv = textBoxH_ZGLV.Text;
            setCon.xml_l_pers = textBoxL_PERS.Text;
            setCon.xml_l_zglv = textBoxL_ZGLV.Text;
            setCon.xml_h_ds2_n = textBoxH_DS2.Text;
            setCon.xml_h_nazr = textBoxH_NAZR.Text;
            setCon.schemaOracle = textBoxORCLSchema.Text;
            setCon.v_xml_error = textBoxXML_ERRORS.Text;
            setCon.xml_h_z_sluch = textBoxZ_SLUCH.Text;
            setCon.xml_h_kslp = textBoxH_KOEF.Text;
            setCon.xml_h_crit = textBoxH_CRIT.Text;

            setCon.xml_h_napr = textBoxNAPR.Text;

            setCon.xml_h_b_diag = textBoxB_DIAG.Text;
            setCon.xml_h_b_prot = textBoxB_PROT.Text;

            setCon.xml_h_cons = textBoxH_CONS.Text;
            setCon.xml_h_onk_usl = textBoxH_ONK_USL.Text;
            setCon.xml_h_lek_pr = textBoxH_LEK_PR.Text;
            setCon.xml_h_lek_pr_date_inj = textBoxDATE_INJ.Text;
            setCon.xml_h_sank_code_exp = textBoxCODE_EXP.Text;

            setCon.xml_h_ds2 = textBoxDS2.Text;
            setCon.xml_h_ds3 = textBoxDS3.Text;

            //////////////////////////////////////////

            setTrans.xml_h_pacient = textBoxPACto.Text;
            setTrans.xml_h_sank_smo = textBoxSANKto.Text;
            setTrans.xml_h_schet = textBoxH_SCHETto.Text;
            setTrans.xml_h_sluch = textBoxSLUCHto.Text;
            setTrans.xml_h_usl = textBoxUSLto.Text;
            setTrans.xml_h_zap = textBoxZAPto.Text;
            setTrans.xml_h_zglv = textBoxH_ZGLVto.Text;
            setTrans.xml_l_pers = textBoxL_PERSto.Text;
            setTrans.xml_l_zglv = textBoxL_ZGLVto.Text;

            setTrans.xml_h_b_diag = textBoxB_DIAGto.Text;
            setTrans.xml_h_b_prot = textBoxB_PROT_to.Text;

            setTrans.xml_h_napr = textBoxNAPRto.Text;


            setTrans.xml_h_ds2_n_transfer = textBoxH_DS2_Nto.Text;
            setTrans.xml_h_nazr_transfer = textBoxH_NAZRto.Text;
            setTrans.xml_h_z_sluch = textBoxH_Z_SLUCHto.Text;
            setTrans.xml_h_kslp = textBoxH_KOEFto.Text;


            setTrans.xml_h_cons = textBoxH_CONSto.Text;
            setTrans.xml_h_onk_usl = textBoxH_ONK_USLto.Text;
            setTrans.xml_h_lek_pr = textBoxH_LEK_PRto.Text;
            setTrans.xml_h_lek_pr_date_inj = textBoxDATE_INJto.Text;
            setTrans.xml_h_sank_code_exp = textBoxCODE_EXPto.Text;

            setTrans.xml_h_ds2 = textBoxDS2to.Text;
            setTrans.xml_h_ds3 = textBoxDS3to.Text;
            setTrans.xml_h_crit = textBoxCRITto.Text;

            setTrans.schemaOracle = textBoxOWNERto.Text;
            setTrans.Transfer = checkBox1.Checked;





        }

        void SetTransfer()
        {
            textBoxPACto.Text = setTrans.xml_h_pacient;
            textBoxSANKto.Text = setTrans.xml_h_sank_smo;
            textBoxH_SCHETto.Text = setTrans.xml_h_schet;
            textBoxSLUCHto.Text = setTrans.xml_h_sluch;
            textBoxUSLto.Text = setTrans.xml_h_usl;
            textBoxZAPto.Text = setTrans.xml_h_zap;
            textBoxH_ZGLVto.Text = setTrans.xml_h_zglv;
            textBoxL_PERSto.Text = setTrans.xml_l_pers;
            textBoxL_ZGLVto.Text = setTrans.xml_l_zglv;
            textBoxH_NAZRto.Text = setTrans.xml_h_nazr_transfer;
            textBoxH_DS2_Nto.Text = setTrans.xml_h_ds2_n_transfer;
            textBoxOWNERto.Text = setTrans.schemaOracle;
            textBoxH_Z_SLUCHto.Text = setTrans.xml_h_z_sluch;
            textBoxH_KOEFto.Text = setTrans.xml_h_kslp;
            checkBox1.Checked = setTrans.Transfer;
            textBoxB_DIAGto.Text = setTrans.xml_h_b_diag;
            textBoxB_PROT_to.Text = setTrans.xml_h_b_prot;
            textBoxNAPRto.Text = setTrans.xml_h_napr;
            textBoxH_CONSto.Text = setTrans.xml_h_cons;
            textBoxH_ONK_USLto.Text = setTrans.xml_h_onk_usl;
            textBoxH_LEK_PRto.Text = setTrans.xml_h_lek_pr;
            textBoxDATE_INJto.Text = setTrans.xml_h_lek_pr_date_inj;
            textBoxCODE_EXPto.Text = setTrans.xml_h_sank_code_exp;
            textBoxDS2to.Text = setTrans.xml_h_ds2;
            textBoxDS3to.Text = setTrans.xml_h_ds3;
            textBoxCRITto.Text = setTrans.xml_h_crit;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var di = new FolderDialog(set.IncomingDir, true);
            if (di.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBoxDir.Text = di.selectpath;
                set.IncomingDir = textBoxDir.Text;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FolderDialog di = new FolderDialog(set.InputDir, true);
            if (di.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBoxInputDir.Text = di.selectpath;
                set.InputDir = textBoxInputDir.Text;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FolderDialog di = new FolderDialog(set.ErrorDir, true);
            if (di.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBoxErrorDir.Text = di.selectpath;
                set.ErrorDir = textBoxErrorDir.Text;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FolderDialog di = new FolderDialog(set.ProcessDir, true);
            if (di.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBoxProcessDir.Text = di.selectpath;
                set.ProcessDir = textBoxProcessDir.Text;
            }
        }

      

        private void button7_Click(object sender, EventArgs e)
        {
            FolderDialog di = new FolderDialog(set.ErrorMessageFile, true);
            if (di.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBoxErrorMesDir.Text = di.selectpath;
                set.ErrorMessageFile = textBoxErrorMesDir.Text;
            }
        }


        private void SettingForm_Load(object sender, EventArgs e)
        {



        }




        private void listBoxTypeSchema_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sc.Versions.Contains(version))
            {
                var s = sc[version, (FileType) listBoxTypeSchema.SelectedIndex];
                SetSC(s);
            }
        }

        private void SetSC(List<SchemaElementValue> s)
        {
            schemaElementValueBindingSource.DataSource = s;
        }

        private void SettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch ((WcfInterface as ICommunicationObject).State)
            {
                case CommunicationState.Closed:
                case CommunicationState.Closing:
                case CommunicationState.Created:
                case CommunicationState.Faulted:
                case CommunicationState.Opening:
                    foreach (Form form in this.OwnedForms)
                    {
                        form.Close();
                    }

                    MessageBox.Show(@"Канал был разорван изменения не сохранены!!!");

                    break;
                case CommunicationState.Opened:


                    readTableName();
                    WcfInterface.SettingSchemaColection(sc);
                    WcfInterface.SettingConnect(setCon);
                    WcfInterface.SettingsFolder(set);
                    WcfInterface.SetSettingTransfer(setTrans);

                    System.Windows.Forms.DialogResult dr = MessageBox.Show(@"Сохранить внесенные изменения??", "",
                        MessageBoxButtons.YesNoCancel);

                    switch (dr)
                    {
                        case System.Windows.Forms.DialogResult.Yes:

                            WcfInterface.SetListTransfer(listTransferProc, textBoxClearTemp100.Text,
                                textBoxClearTemp1.Text,
                                textBoxCheckTemp100.Text, textBoxCheckTemp1.Text);
                            WcfInterface.SaveProperty();
                            Properties.Settings.Default.Save();
                            break;
                        case System.Windows.Forms.DialogResult.No:
                            WcfInterface.LoadProperty();
                            Properties.Settings.Default.Reload();
                            break;
                        case System.Windows.Forms.DialogResult.Cancel:
                            e.Cancel = true;
                            break;

                    }

                    break;
            }

        }


        private void button8_Click(object sender, EventArgs e)
        {
            ConnSet();
        }

        private void ConnSet()
        {
            textBoxLogin.Text = conn.UserID;
            textBoxPass.Text = conn.Password;
            if (conn.DBAPrivilege == "")
                comboBoxPriv.Text = "NORMAL";
            else
                comboBoxPriv.Text = conn.DBAPrivilege;
            int index = 0;
            string tmptxt = "";
            for (int i = 0; i < conn.DataSource.Length; i++)
            {
                if (conn.DataSource[i] == ':')
                {
                    index = i + 1;
                    textBoxHOST.Text = tmptxt;
                    break;
                }

                tmptxt += conn.DataSource[i];
                index = i;
            }

            tmptxt = "";
            for (int i = index; i < conn.DataSource.Length; i++)
            {
                if (conn.DataSource[i] == '/')
                {
                    index = i + 1;
                    int outn;
                    if (Int32.TryParse(tmptxt, out outn))
                        numericUpDownPort.Value = outn;
                    break;
                }

                tmptxt += conn.DataSource[i];
                index = i;
            }

            tmptxt = "";
            for (int i = index; i < conn.DataSource.Length; i++)
            {
                tmptxt += conn.DataSource[i];
            }

            textBoxSID.Text = tmptxt;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            conn.DataSource = $"{textBoxHOST.Text}:{numericUpDownPort.Value}/{textBoxSID.Text}";
            conn.UserID = textBoxLogin.Text;
            conn.Password = textBoxPass.Text;
            if (comboBoxPriv.Text == "SYSOPER" || comboBoxPriv.Text == "SYSDBA")
                conn.DBAPrivilege = comboBoxPriv.Text;
            else
                conn.DBAPrivilege = "";
            setCon.ConnectingString = conn.ConnectionString;
            WcfInterface.SettingConnect(setCon);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            OracleConnectionStringBuilder tmpcon = new OracleConnectionStringBuilder(conn.ConnectionString);
            tmpcon.DataSource = $"{textBoxHOST.Text}:{numericUpDownPort.Value}/{textBoxSID.Text}";
            tmpcon.UserID = textBoxLogin.Text;
            tmpcon.Password = textBoxPass.Text;
            if (comboBoxPriv.Text == "SYSOPER" || comboBoxPriv.Text == "SYSDBA")
                tmpcon.DBAPrivilege = comboBoxPriv.Text;
            else
                tmpcon.DBAPrivilege = "";


            BoolResult rez = WcfInterface.isConnect(tmpcon.ConnectionString);
            if (rez.Result)
            {
                pictureBox1.BackgroundImage = Properties.Resources.button_ok;
                label10.Text = @"Подключение успешно";
                label10.ForeColor = Color.LimeGreen;
                System.Media.SystemSounds.Asterisk.Play();
            }
            else
            {
                pictureBox1.BackgroundImage = Properties.Resources.error;
                label10.Text = "Ошибка подключения" + Environment.NewLine + rez.Exception;
                label10.ForeColor = Color.Red;
                System.Media.SystemSounds.Exclamation.Play();
            }


        }

        private void button11_Click(object sender, EventArgs e)
        {
            readTableName();
            DataTable tbl;
            WcfInterface.SettingConnect(setCon);
            var tblrez = WcfInterface.GetTableServer(setCon.schemaOracle);
            if (tblrez.Result == null)
            {
                MessageBox.Show(tblrez.Exception);
                return;
            }

            tbl = tblrez.Result;


            pictureBoxH_PAC.BackgroundImage = Properties.Resources.error;
            pictureBoxH_SANK.BackgroundImage = Properties.Resources.error;
            pictureBoxH_SCHET.BackgroundImage = Properties.Resources.error;
            pictureBoxH_SLUCH.BackgroundImage = Properties.Resources.error;
            pictureBoxH_USL.BackgroundImage = Properties.Resources.error;
            pictureBoxH_ZAP.BackgroundImage = Properties.Resources.error;
            pictureBoxH_ZGL.BackgroundImage = Properties.Resources.error;
            pictureBoxL_PERS.BackgroundImage = Properties.Resources.error;
            pictureBoxL_ZGLV.BackgroundImage = Properties.Resources.error;
            pictureBoxXML_ERRORS.BackgroundImage = Properties.Resources.error;
            pictureBoxH_NAZR.BackgroundImage = Properties.Resources.error;
            pictureBoxH_DS2.BackgroundImage = Properties.Resources.error;
            pictureBoxZ_SLUCH.BackgroundImage = Properties.Resources.error;
            pictureBoxH_KOEF.BackgroundImage = Properties.Resources.error;

            pictureBoxB_DIAG.BackgroundImage = Properties.Resources.error;
            pictureBoxB_PROT.BackgroundImage = Properties.Resources.error;

            pictureBoxNAPR.BackgroundImage = Properties.Resources.error;

            pictureBoxH_CONS.BackgroundImage = Properties.Resources.error;
            pictureBoxH_ONK_USL.BackgroundImage = Properties.Resources.error;
            pictureBoxDATE_INJ.BackgroundImage = Properties.Resources.error;
            pictureBoxH_LEK_PR.BackgroundImage = Properties.Resources.error;
            pictureBoxCODE_EXP.BackgroundImage = Properties.Resources.error;

            pictureBoxDS2.BackgroundImage = Properties.Resources.error;
            pictureBoxDS3.BackgroundImage = Properties.Resources.error;
            for (int i = 0; i < tbl.Rows.Count; i++)
            {

                string value = tbl.Rows[i]["TABLE_NAME"].ToString();
                if (value == setCon.xml_h_ds2.ToUpper())
                {
                    pictureBoxDS2.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setCon.xml_h_ds3.ToUpper())
                {
                    pictureBoxDS3.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setCon.xml_h_pacient.ToUpper())
                {
                    pictureBoxH_PAC.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setCon.xml_h_sank_smo.ToUpper())
                {
                    pictureBoxH_SANK.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setCon.xml_h_schet.ToUpper())
                {
                    pictureBoxH_SCHET.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setCon.xml_h_sluch.ToUpper())
                {
                    pictureBoxH_SLUCH.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setCon.xml_h_usl.ToUpper())
                {
                    pictureBoxH_USL.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setCon.xml_h_zap.ToUpper())
                {
                    pictureBoxH_ZAP.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setCon.xml_h_zglv.ToUpper())
                {
                    pictureBoxH_ZGL.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setCon.xml_l_pers.ToUpper())
                {
                    pictureBoxL_PERS.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setCon.xml_l_zglv.ToUpper())
                {
                    pictureBoxL_ZGLV.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setCon.v_xml_error.ToUpper())
                {
                    pictureBoxXML_ERRORS.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setCon.xml_h_nazr.ToUpper())
                {
                    pictureBoxH_NAZR.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setCon.xml_h_ds2_n.ToUpper())
                {
                    pictureBoxH_DS2.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setCon.xml_h_z_sluch.ToUpper())
                {
                    pictureBoxZ_SLUCH.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setCon.xml_h_kslp.ToUpper())
                {
                    pictureBoxH_KOEF.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setCon.xml_h_b_diag.ToUpper())
                {
                    pictureBoxB_DIAG.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setCon.xml_h_b_prot.ToUpper())
                {
                    pictureBoxB_PROT.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setCon.xml_h_napr.ToUpper())
                {
                    pictureBoxNAPR.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setCon.xml_h_cons.ToUpper())
                {
                    pictureBoxH_CONS.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setCon.xml_h_onk_usl.ToUpper())
                {
                    pictureBoxH_ONK_USL.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setCon.xml_h_lek_pr_date_inj.ToUpper())
                {
                    pictureBoxDATE_INJ.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setCon.xml_h_lek_pr.ToUpper())
                {
                    pictureBoxH_LEK_PR.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setCon.xml_h_sank_code_exp.ToUpper())
                {
                    pictureBoxCODE_EXP.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }
            }

            System.Media.SystemSounds.Asterisk.Play();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            set.TimePacketOpen = (int) numericUpDown1.Value;
        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }



        ChekingList list = new ChekingList();

        void refreshListViewChek(TableName name, ListView ListView)
        {
            
            int select = 0;
            if (ListView.SelectedItems.Count != 0)
                select = ListView.SelectedItems[0].Index;
            ListView.Items.Clear();
            int count = list.Count(name);
            for (int i = 0; i < count; i++)
            {

                ListViewItem lvi = new ListViewItem();
                switch (toolStripComboBoxTypeViwe.SelectedIndex)
                {
                    case 0:
                        lvi.Text = list[name, i].NAME_ERR;
                        break;
                    case 1:
                        lvi.Text = list[name, i].NAME_PROC;
                        break;
                    case 2:
                        lvi.Text = list[name, i].NAME_ERR + "(" + list[name, i].NAME_PROC + ")";
                        break;
                }

                //lvi.Text = list[name, i].NAME_ERR + "("+list[name, i].NAME_PROC+")";
                lvi.Checked = list[name, i].STATE;
                switch (list[name, i].Excist)
                {
                    case StateExistProcedure.Exist:
                        lvi.ImageIndex = 1;
                        lvi.ToolTipText = list[name, i].NAME_PROC;
                        break;
                    case StateExistProcedure.NotExcist:
                        lvi.ImageIndex = 2;
                        lvi.ToolTipText = list[name, i].Comment;
                        break;
                    case StateExistProcedure.Unknow:
                        lvi.ImageIndex = 0;
                        lvi.ToolTipText = list[name, i].NAME_PROC;
                        break;
                }

                ListView.Items.Add(lvi);
            }

            if (ListView.Items.Count != 0)
            {
                ListView.Focus();
                if (select < ListView.Items.Count)
                    ListView.Items[select].Selected = true;
                else
                    ListView.Items[0].Selected = true;
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            refreshListViewChek(TableName.L_PERS, listViewL_Pers);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            if (listViewL_Pers.SelectedItems[0].Index != -1)
            {
                OrclProcedure proc = list[TableName.L_PERS, listViewL_Pers.SelectedIndices[0]];
                EdditProc form = new EdditProc(proc, setCon.ConnectingString);
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    refreshListViewChek(TableName.L_PERS, listViewL_Pers);
                }
            }
        }

        private void listViewL_Pers_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listViewL_Pers_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            int tmp = Convert.ToInt32((sender as CheckedListBox).Tag);
            TableName nametbl = TableName.ZGLV;
            switch (tmp)
            {
                case 1:
                    nametbl = TableName.ZGLV;
                    break;
                case 2:
                    nametbl = TableName.SCHET;
                    break;
                case 3:
                    nametbl = TableName.ZAP;
                    break;
                case 4:
                    nametbl = TableName.PACIENT;
                    break;
                case 5:
                    nametbl = TableName.SLUCH;
                    break;
                case 6:
                    nametbl = TableName.SANK;
                    break;
                case 7:
                    nametbl = TableName.USL;
                    break;
                case 8:
                    nametbl = TableName.L_ZGLV;
                    break;
                case 9:
                    nametbl = TableName.L_PERS;
                    break;
            }


            if (e.Index < 0) return;
            if (e.NewValue == CheckState.Checked)
                list[nametbl, e.Index].STATE = true;
            else
                list[nametbl, e.Index].STATE = false;

        }

        private void button15_Click(object sender, EventArgs e)
        {
            OrclProcedure proc = new OrclProcedure();
            EdditProc form = new EdditProc(proc, setCon.ConnectingString);
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                list.Add(TableName.L_PERS, proc);
                refreshListViewChek(TableName.L_PERS, listViewL_Pers);
            }

        }

        private void button16_Click_1(object sender, EventArgs e)
        {
            OrclProcedure proc = new OrclProcedure();
            EdditProc form = new EdditProc(proc, setCon.ConnectingString);
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                list.Add(TableName.L_ZGLV, proc);
                refreshListViewChek(TableName.L_ZGLV, listViewL_Zglv);
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            OrclProcedure proc = new OrclProcedure();
            EdditProc form = new EdditProc(proc, setCon.ConnectingString);
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                list.Add(TableName.USL, proc);
                refreshListViewChek(TableName.USL, listViewUsl);
            }
        }

        private void button22_Click(object sender, EventArgs e)
        {
            OrclProcedure proc = new OrclProcedure();
            EdditProc form = new EdditProc(proc, setCon.ConnectingString);
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                list.Add(TableName.SANK, proc);
                refreshListViewChek(TableName.SANK, listViewSank);
            }
        }

        private void button25_Click(object sender, EventArgs e)
        {
            OrclProcedure proc = new OrclProcedure();
            EdditProc form = new EdditProc(proc, setCon.ConnectingString);
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                list.Add(TableName.SLUCH, proc);
                refreshListViewChek(TableName.SLUCH, listViewSluch);
            }
        }

        private void button28_Click(object sender, EventArgs e)
        {
            TableName nametbl = TableName.ZGLV;
            int tmp = Convert.ToInt32(tabControlL_Pers.SelectedTab.Tag);
            switch (tmp)
            {
                case 1:
                    nametbl = TableName.ZGLV;
                    break;
                case 2:
                    nametbl = TableName.SCHET;
                    break;
                case 3:
                    nametbl = TableName.ZAP;
                    break;
                case 4:
                    nametbl = TableName.PACIENT;
                    break;
                case 5:
                    nametbl = TableName.SLUCH;
                    break;
                case 6:
                    nametbl = TableName.SANK;
                    break;
                case 7:
                    nametbl = TableName.USL;
                    break;
                case 8:
                    nametbl = TableName.L_ZGLV;
                    break;
                case 9:
                    nametbl = TableName.L_PERS;
                    break;
            }

            OrclProcedure proc = new OrclProcedure();
            EdditProc form = new EdditProc(proc, setCon.ConnectingString);
            form.Owner = this;
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                list.Add(nametbl, proc);
                switch (nametbl)
                {
                    case TableName.ZGLV:
                        refreshListViewChek(nametbl, listViewZglv);
                        break;
                    case TableName.ZAP:
                        refreshListViewChek(nametbl, listViewZap);
                        break;
                    case TableName.USL:
                        refreshListViewChek(nametbl, listViewUsl);
                        break;
                    case TableName.SLUCH:
                        refreshListViewChek(nametbl, listViewSluch);
                        break;
                    case TableName.SCHET:
                        refreshListViewChek(nametbl, listViewSchet);
                        break;
                    case TableName.SANK:
                        refreshListViewChek(nametbl, listViewSank);
                        break;
                    case TableName.PACIENT:
                        refreshListViewChek(nametbl, listViewPac);
                        break;
                    case TableName.L_ZGLV:
                        refreshListViewChek(nametbl, listViewL_Zglv);
                        break;
                    case TableName.L_PERS:
                        refreshListViewChek(nametbl, listViewL_Pers);
                        break;
                }
            }
        }

        private void button31_Click(object sender, EventArgs e)
        {
            OrclProcedure proc = new OrclProcedure();
            EdditProc form = new EdditProc(proc, setCon.ConnectingString);
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                list.Add(TableName.ZAP, proc);
                refreshListViewChek(TableName.ZAP, listViewZap);
            }
        }

        private void button34_Click(object sender, EventArgs e)
        {
            OrclProcedure proc = new OrclProcedure();
            EdditProc form = new EdditProc(proc, setCon.ConnectingString);
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                list.Add(TableName.SCHET, proc);
                refreshListViewChek(TableName.SCHET, listViewSchet);
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            OrclProcedure proc = new OrclProcedure();
            EdditProc form = new EdditProc(proc, setCon.ConnectingString);
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                list.Add(TableName.ZGLV, proc);
                refreshListViewChek(TableName.ZGLV, listViewZglv);
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            if (listViewL_Zglv.SelectedIndices[0] != -1)
            {
                OrclProcedure proc = list[TableName.L_ZGLV, listViewL_Zglv.SelectedIndices[0]];
                EdditProc form = new EdditProc(proc, setCon.ConnectingString);
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    refreshListViewChek(TableName.L_ZGLV, listViewL_Zglv);
                }
            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            if (listViewUsl.SelectedIndices[0] != -1)
            {
                OrclProcedure proc = list[TableName.USL, listViewUsl.SelectedIndices[0]];
                EdditProc form = new EdditProc(proc, setCon.ConnectingString);
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    refreshListViewChek(TableName.USL, listViewUsl);
                }
            }
        }

        private void button23_Click(object sender, EventArgs e)
        {
            if (listViewSank.SelectedIndices[0] != -1)
            {
                OrclProcedure proc = list[TableName.SANK, listViewSank.SelectedIndices[0]];
                EdditProc form = new EdditProc(proc, setCon.ConnectingString);
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    refreshListViewChek(TableName.SANK, listViewSank);
                }
            }
        }

        private void button26_Click(object sender, EventArgs e)
        {
            if (listViewSluch.SelectedIndices[0] != -1)
            {
                OrclProcedure proc = list[TableName.SLUCH, listViewSluch.SelectedIndices[0]];
                EdditProc form = new EdditProc(proc, setCon.ConnectingString);
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    refreshListViewChek(TableName.SLUCH, listViewSluch);
                }
            }
        }

        private void button29_Click(object sender, EventArgs e)
        {
            TableName nametbl = TableName.ZGLV;
            ListView lv = listViewZglv;
            int tmp = Convert.ToInt32(tabControlL_Pers.SelectedTab.Tag);
            switch (tmp)
            {
                case 1:
                    nametbl = TableName.ZGLV;
                    lv = listViewZglv;
                    break;
                case 2:
                    nametbl = TableName.SCHET;
                    lv = listViewSchet;
                    break;
                case 3:
                    nametbl = TableName.ZAP;
                    lv = listViewZap;
                    break;
                case 4:
                    nametbl = TableName.PACIENT;
                    lv = listViewPac;
                    break;
                case 5:
                    nametbl = TableName.SLUCH;
                    lv = listViewSluch;
                    break;
                case 6:
                    nametbl = TableName.SANK;
                    lv = listViewSank;
                    break;
                case 7:
                    nametbl = TableName.USL;
                    lv = listViewUsl;
                    break;
                case 8:
                    nametbl = TableName.L_ZGLV;
                    lv = listViewL_Zglv;
                    break;
                case 9:
                    nametbl = TableName.L_PERS;
                    lv = listViewL_Pers;
                    break;
            }

            if (lv.SelectedIndices.Count != 0)
            {


                OrclProcedure proc = list[nametbl, lv.SelectedIndices[0]];
                EdditProc form = new EdditProc(proc, setCon.ConnectingString);
                form.Owner = this;
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    switch (nametbl)
                    {
                        case TableName.ZGLV:
                            refreshListViewChek(nametbl, listViewZglv);
                            break;
                        case TableName.ZAP:
                            refreshListViewChek(nametbl, listViewZap);
                            break;
                        case TableName.USL:
                            refreshListViewChek(nametbl, listViewUsl);
                            break;
                        case TableName.SLUCH:
                            refreshListViewChek(nametbl, listViewSluch);
                            break;
                        case TableName.SCHET:
                            refreshListViewChek(nametbl, listViewSchet);
                            break;
                        case TableName.SANK:
                            refreshListViewChek(nametbl, listViewSank);
                            break;
                        case TableName.PACIENT:
                            refreshListViewChek(nametbl, listViewPac);
                            break;
                        case TableName.L_ZGLV:
                            refreshListViewChek(nametbl, listViewL_Zglv);
                            break;
                        case TableName.L_PERS:
                            refreshListViewChek(nametbl, listViewL_Pers);
                            break;
                    }
                }
            }
        }

        private void button32_Click(object sender, EventArgs e)
        {
            if (listViewZap.SelectedIndices[0] != -1)
            {
                OrclProcedure proc = list[TableName.ZAP, listViewZap.SelectedIndices[0]];
                EdditProc form = new EdditProc(proc, setCon.ConnectingString);
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    refreshListViewChek(TableName.ZAP, listViewZap);
                }
            }
        }

        private void button35_Click(object sender, EventArgs e)
        {
            if (listViewSchet.SelectedIndices[0] != -1)
            {
                OrclProcedure proc = list[TableName.SCHET, listViewSchet.SelectedIndices[0]];
                EdditProc form = new EdditProc(proc, setCon.ConnectingString);
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    refreshListViewChek(TableName.SCHET, listViewSchet);
                }
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (listViewZglv.SelectedIndices[0] != -1)
            {
                OrclProcedure proc = list[TableName.ZGLV, listViewZglv.SelectedIndices[0]];
                EdditProc form = new EdditProc(proc, setCon.ConnectingString);
                if (form.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    refreshListViewChek(TableName.ZGLV, listViewZglv);
                }
            }
        }

        private void buttonRefreshZGLV_Click(object sender, EventArgs e)
        {
            refreshListViewChek(TableName.L_ZGLV, listViewL_Zglv);
        }

        private void button21_Click(object sender, EventArgs e)
        {
            refreshListViewChek(TableName.USL, listViewUsl);
        }

        private void button24_Click(object sender, EventArgs e)
        {
            refreshListViewChek(TableName.SANK, listViewSank);
        }

        private void button27_Click(object sender, EventArgs e)
        {
            refreshListViewChek(TableName.SLUCH, listViewSluch);
        }

        private void button30_Click(object sender, EventArgs e)
        {
            TableName nametbl = TableName.ZGLV;
            int tmp = Convert.ToInt32(tabControlL_Pers.SelectedTab.Tag);
            switch (tmp)
            {
                case 1:
                    nametbl = TableName.ZGLV;
                    break;
                case 2:
                    nametbl = TableName.SCHET;
                    break;
                case 3:
                    nametbl = TableName.ZAP;
                    break;
                case 4:
                    nametbl = TableName.PACIENT;
                    break;
                case 5:
                    nametbl = TableName.SLUCH;
                    break;
                case 6:
                    nametbl = TableName.SANK;
                    break;
                case 7:
                    nametbl = TableName.USL;
                    break;
                case 8:
                    nametbl = TableName.L_ZGLV;
                    break;
                case 9:
                    nametbl = TableName.L_PERS;
                    break;
            }

            switch (nametbl)
            {
                case TableName.ZGLV:
                    refreshListViewChek(nametbl, listViewZglv);
                    break;
                case TableName.ZAP:
                    refreshListViewChek(nametbl, listViewZap);
                    break;
                case TableName.USL:
                    refreshListViewChek(nametbl, listViewUsl);
                    break;
                case TableName.SLUCH:
                    refreshListViewChek(nametbl, listViewSluch);
                    break;
                case TableName.SCHET:
                    refreshListViewChek(nametbl, listViewSchet);
                    break;
                case TableName.SANK:
                    refreshListViewChek(nametbl, listViewSank);
                    break;
                case TableName.PACIENT:
                    refreshListViewChek(nametbl, listViewPac);
                    break;
                case TableName.L_ZGLV:
                    refreshListViewChek(nametbl, listViewL_Zglv);
                    break;
                case TableName.L_PERS:
                    refreshListViewChek(nametbl, listViewL_Pers);
                    break;
            }
        }

        private void button33_Click(object sender, EventArgs e)
        {
            refreshListViewChek(TableName.ZAP, listViewZap);
        }

        private void button36_Click(object sender, EventArgs e)
        {
            refreshListViewChek(TableName.SCHET, listViewSchet);
        }

        private void button37_Click(object sender, EventArgs e)
        {
            refreshListViewChek(TableName.ZGLV, listViewZglv);
        }



        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView control = contextMenuStrip1.SourceControl as ListView;
            TableName nametbl = TableName.ZGLV;
            int tmp = Convert.ToInt32(control.Tag);
            switch (tmp)
            {
                case 1:
                    nametbl = TableName.ZGLV;
                    break;
                case 2:
                    nametbl = TableName.SCHET;
                    break;
                case 3:
                    nametbl = TableName.ZAP;
                    break;
                case 4:
                    nametbl = TableName.PACIENT;
                    break;
                case 5:
                    nametbl = TableName.SLUCH;
                    break;
                case 6:
                    nametbl = TableName.SANK;
                    break;
                case 7:
                    nametbl = TableName.USL;
                    break;
                case 8:
                    nametbl = TableName.L_ZGLV;
                    break;
                case 9:
                    nametbl = TableName.L_PERS;
                    break;
            }

            if (control.SelectedIndices[0] >= 0)
            {
                for (int i = control.SelectedIndices.Count - 1; i >= 0; i--)
                {
                    list.RemoveAt(nametbl, control.SelectedIndices[i]);
                }

                refreshListViewChek(nametbl, control);

            }
        }

        private void дублироватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListView control = contextMenuStrip1.SourceControl as ListView;
            TableName nametbl = TableName.ZGLV;
            int tmp = Convert.ToInt32(control.Tag);
            switch (tmp)
            {
                case 1:
                    nametbl = TableName.ZGLV;
                    break;
                case 2:
                    nametbl = TableName.SCHET;
                    break;
                case 3:
                    nametbl = TableName.ZAP;
                    break;
                case 4:
                    nametbl = TableName.PACIENT;
                    break;
                case 5:
                    nametbl = TableName.SLUCH;
                    break;
                case 6:
                    nametbl = TableName.SANK;
                    break;
                case 7:
                    nametbl = TableName.USL;
                    break;
                case 8:
                    nametbl = TableName.L_ZGLV;
                    break;
                case 9:
                    nametbl = TableName.L_PERS;
                    break;
            }

            if (control.SelectedIndices[0] >= 0)
            {
                OrclProcedure proc = new OrclProcedure(list[nametbl, control.SelectedIndices[0]]);
                list.Add(nametbl, proc);
                refreshListViewChek(nametbl, control);

            }
        }

        private void button38_Click(object sender, EventArgs e)
        {
            try
            {
                BoolResult res = WcfInterface.SetChekingList(list);
                if (!res.Result)
                {
                    MessageBox.Show("Ошибка при передаче настроек: " + res.Exception);
                    return;
                }
                else
                {
                    MessageBox.Show("Передача настроек успешна!");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button39_Click(object sender, EventArgs e)
        {
            try
            {
                ChekingList tmp = WcfInterface.ExecuteCheckAv(list);
                if (tmp == null)
                {
                    MessageBox.Show("Ошибка при выполнении см. лог сервера", "", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                list = tmp;
                refreshListViewChek_ALL();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            MessageBox.Show("Выполнено", "", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void listViewZglv_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listViewZGLV_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            int tmp = Convert.ToInt32((sender as ListView).Tag);
            TableName nametbl = TableName.ZGLV;
            switch (tmp)
            {
                case 1:
                    nametbl = TableName.ZGLV;
                    break;
                case 2:
                    nametbl = TableName.SCHET;
                    break;
                case 3:
                    nametbl = TableName.ZAP;
                    break;
                case 4:
                    nametbl = TableName.PACIENT;
                    break;
                case 5:
                    nametbl = TableName.SLUCH;
                    break;
                case 6:
                    nametbl = TableName.SANK;
                    break;
                case 7:
                    nametbl = TableName.USL;
                    break;
                case 8:
                    nametbl = TableName.L_ZGLV;
                    break;
                case 9:
                    nametbl = TableName.L_PERS;
                    break;
            }


            if (e.Item.Index < 0) return;
            if (e.Item.Checked == true)
                list[nametbl, e.Item.Index].STATE = true;
            else
                list[nametbl, e.Item.Index].STATE = false;
        }

        private void button41_Click(object sender, EventArgs e)
        {
            TableName nametbl = TableName.ZGLV;
            ListView lv = listViewZglv;
            int tmp = Convert.ToInt32(tabControlL_Pers.SelectedTab.Tag);
            switch (tmp)
            {
                case 1:
                    nametbl = TableName.ZGLV;
                    lv = listViewZglv;
                    break;
                case 2:
                    nametbl = TableName.SCHET;
                    lv = listViewSchet;
                    break;
                case 3:
                    nametbl = TableName.ZAP;
                    lv = listViewZap;
                    break;
                case 4:
                    nametbl = TableName.PACIENT;
                    lv = listViewPac;
                    break;
                case 5:
                    nametbl = TableName.SLUCH;
                    lv = listViewSluch;
                    break;
                case 6:
                    nametbl = TableName.SANK;
                    lv = listViewSank;
                    break;
                case 7:
                    nametbl = TableName.USL;
                    lv = listViewUsl;
                    break;
                case 8:
                    nametbl = TableName.L_ZGLV;
                    lv = listViewL_Zglv;
                    break;
                case 9:
                    nametbl = TableName.L_PERS;
                    lv = listViewL_Pers;
                    break;
            }

            try
            {
                list.AddList(nametbl, WcfInterface.GetProcedureFromPack(textBoxPackName.Text));
                refreshListViewChek(nametbl, lv);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button12_Click_1(object sender, EventArgs e)
        {
            TableName nametbl = TableName.ZGLV;
            ListView lv = listViewZglv;
            int tmp = Convert.ToInt32(tabControlL_Pers.SelectedTab.Tag);
            switch (tmp)
            {
                case 1:
                    nametbl = TableName.ZGLV;
                    lv = listViewZglv;
                    break;
                case 2:
                    nametbl = TableName.SCHET;
                    lv = listViewSchet;
                    break;
                case 3:
                    nametbl = TableName.ZAP;
                    lv = listViewZap;
                    break;
                case 4:
                    nametbl = TableName.PACIENT;
                    lv = listViewPac;
                    break;
                case 5:
                    nametbl = TableName.SLUCH;
                    lv = listViewSluch;
                    break;
                case 6:
                    nametbl = TableName.SANK;
                    lv = listViewSank;
                    break;
                case 7:
                    nametbl = TableName.USL;
                    lv = listViewUsl;
                    break;
                case 8:
                    nametbl = TableName.L_ZGLV;
                    lv = listViewL_Zglv;
                    break;
                case 9:
                    nametbl = TableName.L_PERS;
                    lv = listViewL_Pers;
                    break;
            }

            if (lv.SelectedIndices.Count != 0)
            {
                if (lv.SelectedIndices[0] == 0) return;
                OrclProcedure proc = list[nametbl, lv.SelectedIndices[0]];
                list[nametbl, lv.SelectedIndices[0]] = list[nametbl, lv.SelectedIndices[0] - 1];
                list[nametbl, lv.SelectedIndices[0] - 1] = proc;
                lv.Items[lv.SelectedIndices[0] - 1].Selected = true;
                refreshListViewChek(nametbl, lv);
            }
        }

        private void button13_Click_1(object sender, EventArgs e)
        {
            TableName nametbl = TableName.ZGLV;
            ListView lv = listViewZglv;
            int tmp = Convert.ToInt32(tabControlL_Pers.SelectedTab.Tag);
            switch (tmp)
            {
                case 1:
                    nametbl = TableName.ZGLV;
                    lv = listViewZglv;
                    break;
                case 2:
                    nametbl = TableName.SCHET;
                    lv = listViewSchet;
                    break;
                case 3:
                    nametbl = TableName.ZAP;
                    lv = listViewZap;
                    break;
                case 4:
                    nametbl = TableName.PACIENT;
                    lv = listViewPac;
                    break;
                case 5:
                    nametbl = TableName.SLUCH;
                    lv = listViewSluch;
                    break;
                case 6:
                    nametbl = TableName.SANK;
                    lv = listViewSank;
                    break;
                case 7:
                    nametbl = TableName.USL;
                    lv = listViewUsl;
                    break;
                case 8:
                    nametbl = TableName.L_ZGLV;
                    lv = listViewL_Zglv;
                    break;
                case 9:
                    nametbl = TableName.L_PERS;
                    lv = listViewL_Pers;
                    break;
            }

            if (lv.SelectedIndices.Count != 0)
            {
                if (lv.SelectedIndices[0] == lv.Items.Count - 1) return;
                int index = lv.SelectedIndices[0];
                OrclProcedure proc = list[nametbl, index];
                list[nametbl, index] = list[nametbl, index + 1];
                list[nametbl, index + 1] = proc;
                lv.SelectedItems.Clear();
                lv.Items[index + 1].Selected = true;
                refreshListViewChek(nametbl, lv);
            }
        }

        private void button14_Click_1(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                list.SaveToFile(saveFileDialog1.FileName);
        }

        private void button15_Click_1(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                bool res = list.LoadToFile(openFileDialog1.FileName);
                if (res == false) MessageBox.Show("Не удалось загрузить файл!");
                refreshListViewChek_ALL();

            }
        }

        private void tabPage6_Click(object sender, EventArgs e)
        {

        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            refreshListViewChek_ALL();
        }

        private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button28_Click(button28, new EventArgs());
        }

        private void изменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            button29_Click(button28, new EventArgs());
        }

        private void button18_Click(object sender, EventArgs e)
        {
            BoolResult br = WcfInterface.LoadChekListFromBD();
            if (br.Result == false)
            {
                MessageBox.Show(br.Exception);
                return;
            }

            list = WcfInterface.GetChekingList();
            refreshListViewChek_ALL();
        }

        private void button21_Click_1(object sender, EventArgs e)
        {
            readTableName();
            WcfInterface.SetSettingTransfer(setTrans);

            DataTable tbl;
            TableResult tblrez = WcfInterface.GetTableTransfer();
            if (tblrez.Result == null)
            {
                MessageBox.Show(tblrez.Exception);
                return;
            }

            tbl = tblrez.Result;


            pictureBoxPACto.BackgroundImage = Properties.Resources.error;
            pictureBoxSANKto.BackgroundImage = Properties.Resources.error;
            pictureBoxSCHETto.BackgroundImage = Properties.Resources.error;
            pictureBoxSLUCHto.BackgroundImage = Properties.Resources.error;
            pictureBoxUSLto.BackgroundImage = Properties.Resources.error;
            pictureBoxZAPto.BackgroundImage = Properties.Resources.error;
            pictureBoxH_ZGLVto.BackgroundImage = Properties.Resources.error;
            pictureBoxL_PERSto.BackgroundImage = Properties.Resources.error;
            pictureBoxL_ZGLVto.BackgroundImage = Properties.Resources.error;
            pictureBoxH_NAZRto.BackgroundImage = Properties.Resources.error;
            pictureBoxH_DS2_Nto.BackgroundImage = Properties.Resources.error;
            pictureBoxH_Z_SLUCHto.BackgroundImage = Properties.Resources.error;
            pictureBoxH_KOEFto.BackgroundImage = Properties.Resources.error;


            pictureBoxB_DIAGto.BackgroundImage = Properties.Resources.error;
            pictureBoxB_PROTto.BackgroundImage = Properties.Resources.error;

            pictureBoxH_CONSto.BackgroundImage = Properties.Resources.error;
            pictureBoxH_ONK_USLto.BackgroundImage = Properties.Resources.error;
            pictureBoxDATE_INJto.BackgroundImage = Properties.Resources.error;
            pictureBoxH_LEK_PRto.BackgroundImage = Properties.Resources.error;

            pictureBoxNAPRto.BackgroundImage = Properties.Resources.error;
            pictureBoxCODE_EXPto.BackgroundImage = Properties.Resources.error;

            pictureBoxDS2to.BackgroundImage = Properties.Resources.error;
            pictureBoxDS3to.BackgroundImage = Properties.Resources.error;
            for (int i = 0; i < tbl.Rows.Count; i++)
            {
                string value = tbl.Rows[i]["TABLE_NAME"].ToString();
                if (value == setTrans.xml_h_pacient.ToUpper())
                {
                    pictureBoxPACto.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setTrans.xml_h_sank_smo.ToUpper())
                {
                    pictureBoxSANKto.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setTrans.xml_h_schet.ToUpper())
                {
                    pictureBoxSCHETto.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setTrans.xml_h_sluch.ToUpper())
                {
                    pictureBoxSLUCHto.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setTrans.xml_h_usl.ToUpper())
                {
                    pictureBoxUSLto.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setTrans.xml_h_zap.ToUpper())
                {
                    pictureBoxZAPto.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setTrans.xml_h_zglv.ToUpper())
                {
                    pictureBoxH_ZGLVto.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setTrans.xml_l_pers.ToUpper())
                {
                    pictureBoxL_PERSto.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setTrans.xml_l_zglv.ToUpper())
                {
                    pictureBoxL_ZGLVto.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setTrans.xml_h_nazr_transfer.ToUpper())
                {
                    pictureBoxH_NAZRto.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setTrans.xml_h_ds2_n_transfer.ToUpper())
                {
                    pictureBoxH_DS2_Nto.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setTrans.xml_h_z_sluch.ToUpper())
                {
                    pictureBoxH_Z_SLUCHto.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setTrans.xml_h_kslp.ToUpper())
                {
                    pictureBoxH_KOEFto.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setTrans.xml_h_b_diag.ToUpper())
                {
                    pictureBoxB_DIAGto.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setTrans.xml_h_b_prot.ToUpper())
                {
                    pictureBoxB_PROTto.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setTrans.xml_h_napr.ToUpper())
                {
                    pictureBoxNAPRto.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setTrans.xml_h_cons.ToUpper())
                {
                    pictureBoxH_CONSto.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setTrans.xml_h_onk_usl.ToUpper())
                {
                    pictureBoxH_ONK_USLto.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setTrans.xml_h_lek_pr_date_inj.ToUpper())
                {
                    pictureBoxDATE_INJto.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setTrans.xml_h_lek_pr.ToUpper())
                {
                    pictureBoxH_LEK_PRto.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setTrans.xml_h_sank_code_exp.ToUpper())
                {
                    pictureBoxCODE_EXPto.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setTrans.xml_h_ds2.ToUpper())
                {
                    pictureBoxDS2to.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }

                if (value == setTrans.xml_h_ds3.ToUpper())
                {
                    pictureBoxDS3to.BackgroundImage = Properties.Resources.button_ok;
                    continue;
                }


            }

            System.Media.SystemSounds.Asterisk.Play();
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {

            groupBox13.Enabled = checkBox1.Checked;
        }

        private void button19_Click_1(object sender, EventArgs e)
        {

        }

        private void button22_Click_1(object sender, EventArgs e)
        {
            if (WcfInterface.CheckUserPriv(textBoxUserPriv.Text))
            {
                MessageBox.Show("Имя корректно");
            }
            else
            {
                MessageBox.Show("Не допустимое имя");
            }
        }

        private void button20_Click_1(object sender, EventArgs e)
        {
            if (WcfInterface.CheckUserPriv(textBoxUserPriv.Text))
            {
                WcfInterface.SetUserPriv(textBoxUserPriv.Text);
            }
            else
            {
                MessageBox.Show("Не допустимое имя");
            }
        }

        private void button24_Click_1(object sender, EventArgs e)
        {
            ImpersonInfo ii = new ImpersonInfo();
            ii.Domen = textBoxImpersDomen.Text;
            ii.Login = textBoxImpersUser.Text;
            ii.Password = textBoxImpersPasword.Text;
            // WcfInterface.SetImpersonInfo(ii);
        }

        private void button23_Click_1(object sender, EventArgs e)
        {
            ImpersonInfo ii = new ImpersonInfo();
            ii.Domen = textBoxImpersDomen.Text;
            ii.Login = textBoxImpersUser.Text;
            ii.Password = textBoxImpersPasword.Text;
            /*if (WcfInterface.CheckImpersonInfo(ii))
            {
                MessageBox.Show("Аутентификация удалась!");
            }
            else
            {
                MessageBox.Show("Аутентификация не удалась");
            }*/
        }

        List<OrclProcedure> listTransferProc;

        void RefreshlistTransferProc()
        {
            listBoxTransfer.Items.Clear();
            foreach (OrclProcedure pr in listTransferProc)
            {
                listBoxTransfer.Items.Add(pr.NAME_ERR);
            }
        }

        private void button19_Click_2(object sender, EventArgs e)
        {
            var t = new OrclProcedure();
            t.NAME_ERR = textBoxNameTransfer.Text.ToUpper().Trim();
            t.NAME_PROC = textBoxNameProcTransfer.Text.ToUpper().Trim();
            listTransferProc.Add(t);
            RefreshlistTransferProc();

        }

        private void button25_Click_1(object sender, EventArgs e)
        {
            if (listBoxTransfer.SelectedIndex != -1)
            {
                var t = listTransferProc[listBoxTransfer.SelectedIndex];
                t.NAME_ERR = textBoxNameTransfer.Text.ToUpper().Trim();
                t.NAME_PROC = textBoxNameProcTransfer.Text.ToUpper().Trim();
                RefreshlistTransferProc();
            }
        }

        //DOWN
        private void button27_Click_1(object sender, EventArgs e)
        {
            if (listBoxTransfer.SelectedIndex != -1 && listBoxTransfer.SelectedIndex + 1 != listTransferProc.Count)
            {
                int index = listBoxTransfer.SelectedIndex + 1;
                var t = listTransferProc[listBoxTransfer.SelectedIndex];
                listTransferProc[listBoxTransfer.SelectedIndex] = listTransferProc[listBoxTransfer.SelectedIndex + 1];
                listTransferProc[listBoxTransfer.SelectedIndex + 1] = t;
                RefreshlistTransferProc();
                listBoxTransfer.SelectedIndex = index;
            }
        }

        //UP
        private void button26_Click_1(object sender, EventArgs e)
        {
            if (listBoxTransfer.SelectedIndex != -1 && listBoxTransfer.SelectedIndex != 0)
            {
                int index = listBoxTransfer.SelectedIndex - 1;
                var t = listTransferProc[listBoxTransfer.SelectedIndex];
                listTransferProc[listBoxTransfer.SelectedIndex] = listTransferProc[listBoxTransfer.SelectedIndex - 1];
                listTransferProc[listBoxTransfer.SelectedIndex - 1] = t;
                RefreshlistTransferProc();
                listBoxTransfer.SelectedIndex = index;
            }
        }

        private void listBoxTransfer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxTransfer.SelectedIndex != -1)
            {
                textBoxNameTransfer.Text = listTransferProc[listBoxTransfer.SelectedIndex].NAME_ERR;
                textBoxNameProcTransfer.Text = listTransferProc[listBoxTransfer.SelectedIndex].NAME_PROC;
            }
        }

        private void button31_Click_1(object sender, EventArgs e)
        {
            try
            {
                for (var i = 0; i < listTransferProc.Count; i++)
                {
                    WcfInterface.RunProcListTransfer(i, 0);
                }

                MessageBox.Show(@"Завершено");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonAddDIRInERROR_Click(object sender, EventArgs e)
        {
            FolderDialog di = new FolderDialog(set.AddDIRInERROR, true);
            if (di.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                textBoxAddDIRInERROR.Text = di.selectpath;
                set.AddDIRInERROR = textBoxAddDIRInERROR.Text;
            }
        }

        private void textBoxISP_TextChanged(object sender, EventArgs e)
        {
            set.ISP = textBoxISP.Text;
        }

        VersionMP version = VersionMP.V2_1;

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxVersionSchema.SelectedItem != null)
            {
                version = (VersionMP) comboBoxVersionSchema.SelectedItem;
                RefreshLBZglvVers();
            }
        }

        void RefreshLBZglvVers()
        {
            listBoxZGLV_VERS.Items.Clear();
            if (sc.ContainsVersion(version))
            {
                foreach (string val in sc[version].VersionsZGLV)
                {
                    listBoxZGLV_VERS.Items.Add(val);
                }
            }
        }

        private void button32_Click_1(object sender, EventArgs e)
        {

            if (sc.ContainsVersion(version))
            {
                sc[version].VersionsZGLV.Add(textBoxNewVersion.Text.ToUpper().Trim());
                RefreshLBZglvVers();
            }

        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (listBoxZGLV_VERS.SelectedItem != null)
            {
                sc[version].VersionsZGLV.Remove(listBoxZGLV_VERS.SelectedItem.ToString());
                RefreshLBZglvVers();
            }
        }

        private void button33_Click_1(object sender, EventArgs e)
        {
            var fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.VIRTUALPATH = fbd.SelectedPath;
                textBoxVIRTUALPATH.Text = Properties.Settings.Default.VIRTUALPATH;
            }

        }

        private void checkBoxISVIRTUALPATH_CheckedChanged(object sender, EventArgs e)
        {


        }

        private void checkBoxISVIRTUALPATH_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.ISVIRTUALPATH = checkBoxISVIRTUALPATH.Checked;
        }




        private void добавитьНовуюToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedTypeSchema.Count != 0)
            {
                var win = new NewSchemaItem(false);
                if (win.ShowDialog() == DialogResult.OK)
                {
                    foreach (var item in selectedTypeSchema)
                    {
                        try
                        {
                            sc[version].AddAndCheck(item,
                                new SchemaElementValue() {DATE_B = win.DATE_B, DATE_E = win.DATE_E, Value = win.PATH});
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(
                                $@"Не удалось добавить схему к файлу {item} версии {version} по причине: {ex.Message}");
                        }

                    }

                    schemaElementValueBindingSource.ResetBindings(false);
                }
            }

        }

        private void удалитьВсеСхемыToolStripMenuItem_Click(object sender, EventArgs e)
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

                schemaElementValueBindingSource.ResetBindings(false);
            }

        }

        private void button16_Click_2(object sender, EventArgs e)
        {

        }

        private void button17_Click_1(object sender, EventArgs e)
        {

        }
    }
}
