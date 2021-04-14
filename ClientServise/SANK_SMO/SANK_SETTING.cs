using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;
using System.IO;
using ServiceLoaderMedpomData;

namespace ClientServise.SANK_SMO
{
    public partial class SANK_SETTING : Form
    {
        public SANK_SETTING()
        {
            InitializeComponent();
        }


        OracleConnectionStringBuilder conn;
        private void groupBox10_Enter(object sender, EventArgs e)
        {

        }
        
        private void SANK_SETTING_Load(object sender, EventArgs e)
        {
            SchemasDir = Properties.Settings.Default.SCHEMA_DIR;
     



            pictureBoxH_PACIENT_NEW.BackgroundImage = null;
            pictureBoxH_SANK_NEW.BackgroundImage = null;
            pictureBoxH_SCHET_NEW.BackgroundImage = null;
            pictureBoxH_SLUCH_NEW.BackgroundImage = null;
            pictureBoxH_USL_NEW.BackgroundImage = null;
            pictureBoxH_ZAP_NEW.BackgroundImage = null;
            pictureBoxH_ZGL_NEW.BackgroundImage = null;
            pictureBoxL_ZGLV_NEW.BackgroundImage = null;
            pictureBoxL_PERS_NEW.BackgroundImage = null;
            pictureBoxH_NAZR_NEW.BackgroundImage = null;
            pictureBoxH_DS2_N_NEW.BackgroundImage = null;
            pictureBoxH_KOEF.BackgroundImage = null;
            pictureBoxZ_SLUCH.BackgroundImage = null;

           

            pictureBox_S_PACIENT_NEW.BackgroundImage = null;
            pictureBox_S_SANK_NEW.BackgroundImage = null;
            pictureBox_S_SCHET_NEW.BackgroundImage = null;
            pictureBox_S_SLUCH_NEW.BackgroundImage = null;
            pictureBox_S_USL_NEW.BackgroundImage = null;
            pictureBox_S_ZAP_NEW.BackgroundImage = null;
            pictureBox_S_ZGLV_NEW.BackgroundImage = null;
            pictureBox_S_L_ZGLV_NEW.BackgroundImage = null;
            pictureBox_S_L_PERS_NEW.BackgroundImage = null;
            pictureBoxZSLUCH.BackgroundImage = null;
            textBoxSeqLEK_PR.BackgroundImage = null;
            textBoxSeqONK_USL.BackgroundImage = null;


            textBoxORA_SCHEMA_NEW.Text = AppConfig.Property.schemaOracle;

            textBoxH_PACIENT_NEW.Text = AppConfig.Property.xml_h_pacient;
            textBoxH_SANK_NEW.Text = AppConfig.Property.xml_h_sank;
            textBoxH_SCHET_NEW.Text = AppConfig.Property.xml_h_schet;
            textBoxH_SLUCH_NEW.Text = AppConfig.Property.xml_h_sluch;
            textBoxH_USL_NEW.Text = AppConfig.Property.xml_h_usl;
            textBoxH_ZAP_NEW.Text = AppConfig.Property.xml_h_zap;
            textBoxH_ZGLV_NEW.Text = AppConfig.Property.xml_h_zglv;

            textBoxH_NAZR_NEW.Text = AppConfig.Property.xml_h_nazr;
            textBoxH_DS2_N_NEW.Text = AppConfig.Property.xml_h_ds2_n;
            textBoxL_ZGLV_NEW.Text = AppConfig.Property.xml_l_zglv;
            textBoxL_PERS_NEW.Text = AppConfig.Property.xml_l_pers;
            textBoxZ_SLUCH.Text = AppConfig.Property.xml_h_z_sluch;
            textBoxH_KOEF.Text = AppConfig.Property.xml_h_kslp;

      
            textBoxB_DIAG.Text = AppConfig.Property.xml_h_b_diag;
            textBoxB_PROT.Text = AppConfig.Property.xml_h_b_prot;
            textBoxNAPR.Text = AppConfig.Property.xml_h_napr;
            textBoxCRIT.Text = AppConfig.Property.xml_h_crit;



            textBox_S_ZGLV_NEW.Text = AppConfig.Property.seq_ZGLV;
            textBox_S_SANK_NEW.Text = AppConfig.Property.seq_SANK;
            textBox_S_SCHET_NEW.Text = AppConfig.Property.seq_SCHET;
            textBox_S_SLUCH_NEW.Text = AppConfig.Property.seq_SLUCH;
            textBox_S_USL_NEW.Text = AppConfig.Property.seq_USL;
            textBox_S_ZAP_NEW.Text = AppConfig.Property.seq_ZAP;
            textBox_S_PACIENT_NEW.Text = AppConfig.Property.seq_PACIENT;

            textBox_S_L_ZGLV_NEW.Text = AppConfig.Property.seq_L_ZGLV;
  
            textBox_S_L_PERS_NEW.Text = AppConfig.Property.seq_L_pers;
            textBoxSeq_Z_SLUCH.Text = AppConfig.Property.seq_z_sluch;

            textBoxSeqONK_USL.Text = AppConfig.Property.seq_xml_h_onk_usl;
            textBoxSeqLEK_PR.Text = AppConfig.Property.seq_xml_h_lek_pr;
            textBoxDS2.Text = AppConfig.Property.xml_h_ds2;
            textBoxDS3.Text = AppConfig.Property.xml_h_ds3;



            textBoxORA_SCHEMA_NEW.Text = AppConfig.Property.schemaOracle;



            textBoxH_CONSto.Text = AppConfig.Property.xml_h_cons;
            textBoxH_ONK_USLto.Text = AppConfig.Property.xml_h_onk_usl;
            textBoxH_LEK_PRto.Text = AppConfig.Property.xml_h_lek_pr;
            textBoxDATE_INJto.Text = AppConfig.Property.xml_h_date_inj;
            textBoxCODE_EXPto.Text = AppConfig.Property.xml_h_code_exp;



            textBoxSEQ_ORA_SCHEMA_NEW.Text = AppConfig.Property.schemaOracle;

            string str = AppConfig.Property.ConnectionString;
            try
            {
                conn = new OracleConnectionStringBuilder(str);
                ConnSet();
            }
            catch
            {
                conn = new OracleConnectionStringBuilder();
            }
            
            sc = new SchemaColection();
            if (File.Exists(Path.Combine(Application.StartupPath,"SANK_INVITER_SCHEMA.dat")))
            {
                sc.LoadFromFile(Path.Combine(Application.StartupPath,"SANK_INVITER_SCHEMA.dat"));
            }
            comboBox1.Items.Clear();
            foreach (VersionMP vmp in sc.Versions)
            {
                comboBox1.Items.Add(vmp);
            }

            foreach (FileType ft in (FileType[])Enum.GetValues(typeof(FileType)))
            {
                listBoxTypeSchema.Items.Add(ft);
            }

      
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

     



        public DataTable GetTableServer_NEW()
        {
            string cmdstr = @"SELECT TABLE_NAME
                            FROM ALL_TABLES
                            WHERE (TABLE_NAME = '" + AppConfig.Property.xml_h_pacient.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_sank.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_schet.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_sluch.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_usl.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_zap.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_zglv.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_l_zglv.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_nazr.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_ds2_n.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_z_sluch.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_kslp.ToUpper() + "' or " +

                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_cons.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_onk_usl.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_lek_pr.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_date_inj.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_code_exp.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_crit.ToUpper() + "' or " +

                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_b_diag.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_b_prot.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_napr.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_ds2.ToUpper() + "' or " +
                                  "TABLE_NAME = '" + AppConfig.Property.xml_h_ds3.ToUpper() + "' or " +

                                  "TABLE_NAME = '" + AppConfig.Property.xml_l_pers.ToUpper() + "'" +
                                   ") and OWNER = '" + AppConfig.Property.schemaOracle.ToUpper() + "'";
            DataTable tbl;
            DataTable tbl1 = new DataTable();
            DataTable tr = new DataTable();
            try
            {
                OracleDataAdapter oda = new OracleDataAdapter(cmdstr, new OracleConnection(AppConfig.Property.ConnectionString));
                tbl = new DataTable();
                oda.Fill(tbl);
                tr = tbl;
            }
            catch (Exception)
            {
                tr = null;
            }
            return tr;
        }

    
        public DataTable GetSeqServer_NEW()
        {
            string cmdstr = @"SELECT t.SEQUENCE_NAME
                            FROM ALL_SEQUENCES t
                            WHERE (SEQUENCE_NAME = '" + AppConfig.Property.seq_PACIENT.ToUpper() + "' or " +
                                  "SEQUENCE_NAME = '" + AppConfig.Property.seq_SANK.ToUpper() + "' or " +
                                  "SEQUENCE_NAME = '" + AppConfig.Property.seq_SCHET.ToUpper() + "' or " +
                                  "SEQUENCE_NAME = '" + AppConfig.Property.seq_SLUCH.ToUpper() + "' or " +
                                  "SEQUENCE_NAME = '" + AppConfig.Property.seq_USL.ToUpper() + "' or " +
                                  "SEQUENCE_NAME = '" + AppConfig.Property.seq_ZAP.ToUpper() + "' or " +
                                  "SEQUENCE_NAME = '" + AppConfig.Property.seq_ZGLV.ToUpper() + "' or " +
                                  "SEQUENCE_NAME = '" + AppConfig.Property.seq_L_ZGLV.ToUpper() + "' or " +
                                  "SEQUENCE_NAME = '" + AppConfig.Property.seq_z_sluch.ToUpper() + "' or " +
                                  "SEQUENCE_NAME = '" + AppConfig.Property.seq_xml_h_onk_usl.ToUpper() + "' or " +
                                  "SEQUENCE_NAME = '" + AppConfig.Property.seq_xml_h_lek_pr.ToUpper() + "' or " +

                                  "SEQUENCE_NAME = '" + AppConfig.Property.seq_L_pers.ToUpper() + "'" +
                                   ") and SEQUENCE_OWNER = '" + AppConfig.Property.seq_schemaOracle.ToUpper() + "'";
            DataTable tbl;
            DataTable tbl1 = new DataTable();
            DataTable tr = new DataTable();
            try
            {
                OracleDataAdapter oda = new OracleDataAdapter(cmdstr, new OracleConnection(AppConfig.Property.ConnectionString));
                tbl = new DataTable();
                oda.Fill(tbl);
                tr = tbl;
            }
            catch (Exception)
            {
                tr = null;
            }
            return tr;
        }
     


        void readTableName_NEW()
        {
            AppConfig.Property.xml_h_pacient = textBoxH_PACIENT_NEW.Text;
            AppConfig.Property.xml_h_sank = textBoxH_SANK_NEW.Text;
            AppConfig.Property.xml_h_schet = textBoxH_SCHET_NEW.Text;
            AppConfig.Property.xml_h_sluch = textBoxH_SLUCH_NEW.Text;
            AppConfig.Property.xml_h_usl = textBoxH_USL_NEW.Text;
            AppConfig.Property.xml_h_zap = textBoxH_ZAP_NEW.Text;
            AppConfig.Property.xml_h_zglv = textBoxH_ZGLV_NEW.Text;
            AppConfig.Property.xml_h_nazr = textBoxH_NAZR_NEW.Text;
            AppConfig.Property.xml_h_ds2_n = textBoxH_DS2_N_NEW.Text;
            AppConfig.Property.xml_l_pers = textBoxL_PERS_NEW.Text;
            AppConfig.Property.xml_l_zglv = textBoxL_ZGLV_NEW.Text;
            AppConfig.Property.xml_l_zglv = textBoxL_ZGLV_NEW.Text;
            AppConfig.Property.xml_h_z_sluch = textBoxZ_SLUCH.Text;
            AppConfig.Property.xml_h_kslp = textBoxH_KOEF.Text;
            AppConfig.Property.schemaOracle = textBoxORA_SCHEMA_NEW.Text;


            AppConfig.Property.xml_h_b_diag = textBoxB_DIAG.Text;
            AppConfig.Property.xml_h_b_prot = textBoxB_PROT.Text;
            AppConfig.Property.xml_h_napr = textBoxNAPR.Text;

            AppConfig.Property.xml_h_cons = textBoxH_CONSto.Text;
            AppConfig.Property.xml_h_onk_usl = textBoxH_ONK_USLto.Text;
            AppConfig.Property.xml_h_lek_pr = textBoxH_LEK_PRto.Text;
            AppConfig.Property.xml_h_date_inj = textBoxDATE_INJto.Text;
            AppConfig.Property.xml_h_code_exp = textBoxCODE_EXPto.Text;

            AppConfig.Property.xml_h_ds2 = textBoxDS2.Text;
            AppConfig.Property.xml_h_ds3 = textBoxDS3.Text;
            AppConfig.Property.xml_h_crit = textBoxCRIT.Text;

        }




        void readSeqName_NEW()
        {
            AppConfig.Property.seq_ZGLV = textBox_S_ZGLV_NEW.Text;
            AppConfig.Property.seq_SANK = textBox_S_SANK_NEW.Text;
            AppConfig.Property.seq_SCHET = textBox_S_SCHET_NEW.Text;
            AppConfig.Property.seq_SLUCH = textBox_S_SLUCH_NEW.Text;
            AppConfig.Property.seq_USL = textBox_S_USL_NEW.Text;
            AppConfig.Property.seq_ZAP = textBox_S_ZAP_NEW.Text;
            AppConfig.Property.seq_PACIENT = textBox_S_PACIENT_NEW.Text;
  

            AppConfig.Property.seq_L_ZGLV = textBox_S_L_ZGLV_NEW.Text;
            AppConfig.Property.seq_L_pers = textBox_S_L_PERS_NEW.Text;
            AppConfig.Property.seq_schemaOracle = textBoxSEQ_ORA_SCHEMA_NEW.Text;
            AppConfig.Property.seq_z_sluch = textBoxSeq_Z_SLUCH.Text;
            AppConfig.Property.seq_xml_h_onk_usl = textBoxSeqONK_USL.Text;
            AppConfig.Property.seq_xml_h_lek_pr = textBoxSeqLEK_PR.Text;

        }

        private void button10_Click(object sender, EventArgs e)
        {
            OracleConnectionStringBuilder tmpcon = new OracleConnectionStringBuilder(conn.ConnectionString);
            tmpcon.DataSource = textBoxHOST.Text + ":" + numericUpDownPort.Value.ToString() + "/" + textBoxSID.Text;
            tmpcon.UserID = textBoxLogin.Text;
            tmpcon.Password = textBoxPass.Text;
            if (comboBoxPriv.Text == "SYSOPER" || comboBoxPriv.Text == "SYSDBA")
                tmpcon.DBAPrivilege = comboBoxPriv.Text;
            else
                tmpcon.DBAPrivilege = "";


            string rez = isConnenct(tmpcon.ConnectionString);
            if (rez=="")
            {
                pictureBox1.BackgroundImage = Properties.Resources.button_ok;
                label10.Text = "Подключение успешно";
                label10.ForeColor = Color.LimeGreen;
                System.Media.SystemSounds.Asterisk.Play();
            }
            else
            {
                pictureBox1.BackgroundImage = Properties.Resources.error;
                label10.Text = "Ошибка подключения" + Environment.NewLine + rez;
                label10.ForeColor = Color.Red;
                System.Media.SystemSounds.Exclamation.Play();
            }
        }

        public string isConnenct(string connectionstring)
        {
            string rez = "";
            try
            {
                OracleConnection con = new OracleConnection(connectionstring);
                con.Open();
                con.Close();
            }
            catch (Exception ex)
            {
                rez = ex.Message;
            }
            return rez;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            conn.DataSource = textBoxHOST.Text + ":" + numericUpDownPort.Value.ToString() + "/" + textBoxSID.Text;
            conn.UserID = textBoxLogin.Text;
            conn.Password = textBoxPass.Text;
            if (comboBoxPriv.Text == "SYSOPER" || comboBoxPriv.Text == "SYSDBA")
                conn.DBAPrivilege = comboBoxPriv.Text;
            else
                conn.DBAPrivilege = "";
            AppConfig.Property.ConnectionString = conn.ConnectionString;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            ConnSet();
        }

        private void SANK_SETTING_FormClosing(object sender, FormClosingEventArgs e)
        {
           

            System.Windows.Forms.DialogResult dr = MessageBox.Show("Сохранить внесеные изменения??", "", MessageBoxButtons.YesNoCancel);

            switch (dr)
            {
                case System.Windows.Forms.DialogResult.Yes:
                    Properties.Settings.Default.SCHEMA_DIR = SchemasDir;
                    Properties.Settings.Default.Save();
                         sc.SaveToFile(Path.Combine(Application.StartupPath,"SANK_INVITER_SCHEMA.dat"));
                    AppConfig.Save();
                    
                    break;
                case System.Windows.Forms.DialogResult.No:
                    AppConfig.Load(); break;
                case System.Windows.Forms.DialogResult.Cancel:
                    e.Cancel = true; break;

            }            
        }
        SchemaColection sc;
 
        string SchemasDir;
     
        private void listBoxTypeSchema_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (version != VersionMP.NONE)
            {
                var s = sc[version, (FileType) listBoxTypeSchema.SelectedIndex];
                SetSC(s);
            }
        }

        private void SetSC(List<SchemaElementValue> s)
        {
            schemaElementValueBindingSource.DataSource = s;
        }
      
     

      

    
     

  
        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            readTableName_NEW();

            DataTable tbl;
            DataTable tblrez = GetTableServer_NEW();
            if (tblrez == null)
            {
                MessageBox.Show("Не удалось получить список таблиц");
                return;
            }

            tbl = tblrez;


            pictureBoxH_PACIENT_NEW.BackgroundImage = Properties.Resources.error;
            pictureBoxH_SANK_NEW.BackgroundImage = Properties.Resources.error;
            pictureBoxH_SCHET_NEW.BackgroundImage = Properties.Resources.error;
            pictureBoxH_SLUCH_NEW.BackgroundImage = Properties.Resources.error;
            pictureBoxH_USL_NEW.BackgroundImage = Properties.Resources.error;
            pictureBoxH_ZAP_NEW.BackgroundImage = Properties.Resources.error;
            pictureBoxH_ZGL_NEW.BackgroundImage = Properties.Resources.error;
            pictureBoxL_PERS_NEW.BackgroundImage = Properties.Resources.error;
            pictureBoxL_ZGLV_NEW.BackgroundImage = Properties.Resources.error;
            pictureBoxH_NAZR_NEW.BackgroundImage = Properties.Resources.error;
            pictureBoxH_DS2_N_NEW.BackgroundImage = Properties.Resources.error;
            pictureBoxZ_SLUCH.BackgroundImage = Properties.Resources.error;
            pictureBoxH_KOEF.BackgroundImage = Properties.Resources.error;


            
            pictureBoxB_DIAG.BackgroundImage = Properties.Resources.error;
            pictureBoxB_PROT.BackgroundImage = Properties.Resources.error;
            pictureBoxNAPR.BackgroundImage = Properties.Resources.error;

            pictureBoxH_ONK_USLto.BackgroundImage = Properties.Resources.error;
            pictureBoxH_CONSto.BackgroundImage = Properties.Resources.error;
            pictureBoxH_LEK_PRto.BackgroundImage = Properties.Resources.error;
            pictureBoxDATE_INJto.BackgroundImage = Properties.Resources.error;
            pictureBoxCODE_EXPto.BackgroundImage = Properties.Resources.error;

            pictureBoxDS2.BackgroundImage = Properties.Resources.error;
            pictureBoxDS3.BackgroundImage = Properties.Resources.error;
            pictureBoxCRIT.BackgroundImage = Properties.Resources.error;


            for (int i = 0; i < tbl.Rows.Count; i++)
            {

                string value = tbl.Rows[i]["TABLE_NAME"].ToString();
                if (value == AppConfig.Property.xml_h_ds2.ToUpper()) { pictureBoxDS2.BackgroundImage = Properties.Resources.button_ok; continue; }
                if (value == AppConfig.Property.xml_h_ds3.ToUpper()) { pictureBoxDS3.BackgroundImage = Properties.Resources.button_ok; continue; }

                if (value == AppConfig.Property.xml_h_pacient.ToUpper()) { pictureBoxH_PACIENT_NEW.BackgroundImage = Properties.Resources.button_ok; continue; }
                if (value == AppConfig.Property.xml_h_sank.ToUpper()) { pictureBoxH_SANK_NEW.BackgroundImage = Properties.Resources.button_ok; continue; }
                if (value == AppConfig.Property.xml_h_schet.ToUpper()) { pictureBoxH_SCHET_NEW.BackgroundImage = Properties.Resources.button_ok; continue; }
                if (value == AppConfig.Property.xml_h_sluch.ToUpper()) { pictureBoxH_SLUCH_NEW.BackgroundImage = Properties.Resources.button_ok; continue; }
                if (value == AppConfig.Property.xml_h_usl.ToUpper()) { pictureBoxH_USL_NEW.BackgroundImage = Properties.Resources.button_ok; continue; }
                if (value == AppConfig.Property.xml_h_zap.ToUpper()) { pictureBoxH_ZAP_NEW.BackgroundImage = Properties.Resources.button_ok; continue; }
                if (value == AppConfig.Property.xml_h_zglv.ToUpper()) { pictureBoxH_ZGL_NEW.BackgroundImage = Properties.Resources.button_ok; continue; }
                if (value == AppConfig.Property.xml_l_zglv.ToUpper()) { pictureBoxL_ZGLV_NEW.BackgroundImage = Properties.Resources.button_ok; continue; }
                if (value == AppConfig.Property.xml_l_pers.ToUpper()) { pictureBoxL_PERS_NEW.BackgroundImage = Properties.Resources.button_ok; continue; }
                if (value == AppConfig.Property.xml_h_nazr.ToUpper()) { pictureBoxH_NAZR_NEW.BackgroundImage = Properties.Resources.button_ok; continue; }
                if (value == AppConfig.Property.xml_h_ds2_n.ToUpper()) { pictureBoxH_DS2_N_NEW.BackgroundImage = Properties.Resources.button_ok; continue; }
                if (value == AppConfig.Property.xml_h_z_sluch.ToUpper()) { pictureBoxZ_SLUCH.BackgroundImage = Properties.Resources.button_ok; continue; }
                if (value == AppConfig.Property.xml_h_kslp.ToUpper()) { pictureBoxH_KOEF.BackgroundImage = Properties.Resources.button_ok; continue; }

               
                if (value == AppConfig.Property.xml_h_b_diag.ToUpper()) { pictureBoxB_DIAG.BackgroundImage = Properties.Resources.button_ok; continue; }
                if (value == AppConfig.Property.xml_h_b_prot.ToUpper()) { pictureBoxB_PROT.BackgroundImage = Properties.Resources.button_ok; continue; }
                if (value == AppConfig.Property.xml_h_napr.ToUpper()) { pictureBoxNAPR.BackgroundImage = Properties.Resources.button_ok; continue; }

                if (value == AppConfig.Property.xml_h_cons.ToUpper()) { pictureBoxH_CONSto.BackgroundImage = Properties.Resources.button_ok; continue; }
                if (value == AppConfig.Property.xml_h_onk_usl.ToUpper()) { pictureBoxH_ONK_USLto.BackgroundImage = Properties.Resources.button_ok; continue; }
                if (value == AppConfig.Property.xml_h_lek_pr.ToUpper()) { pictureBoxH_LEK_PRto.BackgroundImage = Properties.Resources.button_ok; continue; }
                if (value == AppConfig.Property.xml_h_date_inj.ToUpper()) { pictureBoxDATE_INJto.BackgroundImage = Properties.Resources.button_ok; continue; }
                if (value == AppConfig.Property.xml_h_code_exp.ToUpper()) { pictureBoxCODE_EXPto.BackgroundImage = Properties.Resources.button_ok; continue; }
                if (value == AppConfig.Property.xml_h_crit.ToUpper()) { pictureBoxCRIT.BackgroundImage = Properties.Resources.button_ok; continue; }
                

            }
            System.Media.SystemSounds.Asterisk.Play();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            readSeqName_NEW();
            DataTable tbl;
            DataTable tblrez = GetSeqServer_NEW();
            if (tblrez == null)
            {
                MessageBox.Show("Не удалось получить список Seq");
                return;
            }

            tbl = tblrez;


            pictureBox_S_PACIENT_NEW.BackgroundImage = Properties.Resources.error;
            pictureBox_S_SANK_NEW.BackgroundImage = Properties.Resources.error;
            pictureBox_S_SCHET_NEW.BackgroundImage = Properties.Resources.error;
            pictureBox_S_SLUCH_NEW.BackgroundImage = Properties.Resources.error;
            pictureBox_S_USL_NEW.BackgroundImage = Properties.Resources.error;
            pictureBox_S_ZAP_NEW.BackgroundImage = Properties.Resources.error;
            pictureBox_S_ZGLV_NEW.BackgroundImage = Properties.Resources.error;
            pictureBox_S_L_ZGLV_NEW.BackgroundImage = Properties.Resources.error;
            pictureBox_S_L_PERS_NEW.BackgroundImage = Properties.Resources.error;
            pictureBoxZSLUCH.BackgroundImage = Properties.Resources.error;
            
            pictureBoxS_LEK_PR.BackgroundImage = Properties.Resources.error;
            pictureBoxS_ONK_USL.BackgroundImage = Properties.Resources.error;
            for (int i = 0; i < tbl.Rows.Count; i++)
            {

                string value = tbl.Rows[i]["SEQUENCE_NAME"].ToString();
                if (value == AppConfig.Property.seq_PACIENT.ToUpper()) { pictureBox_S_PACIENT_NEW.BackgroundImage = Properties.Resources.button_ok; }
                if (value == AppConfig.Property.seq_SANK.ToUpper()) { pictureBox_S_SANK_NEW.BackgroundImage = Properties.Resources.button_ok; }
                if (value == AppConfig.Property.seq_SCHET.ToUpper()) { pictureBox_S_SCHET_NEW.BackgroundImage = Properties.Resources.button_ok; }
                if (value == AppConfig.Property.seq_SLUCH.ToUpper()) { pictureBox_S_SLUCH_NEW.BackgroundImage = Properties.Resources.button_ok; }
                if (value == AppConfig.Property.seq_USL.ToUpper()) { pictureBox_S_USL_NEW.BackgroundImage = Properties.Resources.button_ok; }
                if (value == AppConfig.Property.seq_ZAP.ToUpper()) { pictureBox_S_ZAP_NEW.BackgroundImage = Properties.Resources.button_ok; }
                if (value == AppConfig.Property.seq_ZGLV.ToUpper()) { pictureBox_S_ZGLV_NEW.BackgroundImage = Properties.Resources.button_ok; }
              

                if (value == AppConfig.Property.seq_L_ZGLV.ToUpper()) { pictureBox_S_L_ZGLV_NEW.BackgroundImage = Properties.Resources.button_ok; }
                if (value == AppConfig.Property.seq_L_pers.ToUpper()) { pictureBox_S_L_PERS_NEW.BackgroundImage = Properties.Resources.button_ok; }
                if (value == AppConfig.Property.seq_z_sluch.ToUpper()) { pictureBoxZSLUCH.BackgroundImage = Properties.Resources.button_ok; }

                if (value == AppConfig.Property.seq_xml_h_onk_usl.ToUpper()) { pictureBoxS_ONK_USL.BackgroundImage = Properties.Resources.button_ok; }
                if (value == AppConfig.Property.seq_xml_h_lek_pr.ToUpper()) { pictureBoxS_LEK_PR.BackgroundImage = Properties.Resources.button_ok; }

            }
            System.Media.SystemSounds.Asterisk.Play();
        }

        VersionMP version =  VersionMP.NONE;
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                version = (VersionMP)comboBox1.SelectedItem;
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

        private void button13_Click(object sender, EventArgs e)
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
        private List<FileType> selectedTypeSchema => listBoxTypeSchema.SelectedItems.Cast<FileType>().ToList();
        private void добавитьНовуюToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (selectedTypeSchema.Count != 0)
            {
                var win = new NewSchemaItem(true);
                if (win.ShowDialog() == DialogResult.OK)
                {
                    foreach (var item in selectedTypeSchema)
                    {
                        try
                        {
                            sc[version].AddAndCheck(item,
                                new SchemaElementValue() { DATE_B = win.DATE_B, DATE_E = win.DATE_E, Value = win.PATH });
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
                        MessageBox.Show($@"Не удалось очистить схему к файлу {item} версии {version} по причине: {ex.Message}");
                    }
                }

                schemaElementValueBindingSource.ResetBindings(false);
            }
        }

        private void textBoxH_ZGLV_NEW_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
