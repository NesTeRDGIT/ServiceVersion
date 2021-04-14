using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MedpomService
{
    public class AppProperties
    {
        public string IncomingDir { get; set; } = "";
        public string InputDir { get; set; } = "";
        ///public string SchemasDir { get; set; } = "";
        public string ProcessDir { get; set; } = "";
        public int TimePacketOpen { get; set; } = 100;
        public string ErrorDir { get; set; } = "";
        public string ConnectionString { get; set; } = "";
        public string ErrorMessageFile { get; set; } = "";
        public string AddDIRInERROR { get; set; } = "";

        public string schemaOracle { get; set; } = "asu12";
        public string xml_h_zglv { get; set; } = "xml_h_zglv_v3_temp99";
        public string xml_h_schet { get; set; } = "xml_h_schet_v3_temp99";
        public string xml_h_zap { get; set; } = "xml_h_zap_v3_temp99";
        public string xml_h_z_sluch { get; set; } = "xml_h_z_sluch_v3_temp99";
        public string xml_h_pacient { get; set; } = "xml_h_pacient_v3_temp99";
        public string xml_h_sluch { get; set; } = "xml_h_sluch_v3_temp99";
        public string xml_h_usl { get; set; } = "xml_h_usl_v3_temp99"; 
        public string xml_h_kslp { get; set; } = "xml_h_kslp_v3_temp99";
        public string XML_H_NAZR { get; set; } = "XML_H_NAZR_v3_temp99";
        public string XML_H_DS2_N { get; set; } = "XML_H_DS2_N_v3_temp99";
        public string xml_h_sank { get; set; } = "xml_h_sank_v3_temp99";
        public string xml_h_onk_sl { get; set; } = "xml_h_onk_sl_v3_temp99";
        public string xml_h_b_prot { get; set; } = "xml_h_b_prot_v3_temp99";
        public string xml_h_napr { get; set; } = "xml_h_napr_v3_temp99";
        public string xml_h_b_diag { get; set; } = "xml_h_b_diag_v3_temp99";
        public string xml_h_cons { get; set; } = "xml_h_cons_v3_temp99";
        public string xml_h_date_inj { get; set; } = "xml_h_date_inj_v3_temp99";
        public string xml_h_lek_pr { get; set; } = "xml_h_lek_pr_v3_temp99";
        public string xml_h_onk_usl { get; set; } = "xml_h_onk_usl_v3_temp99";
        public string xml_h_sank_code_exp { get; set; } = "xml_h_sank_code_exp_v3_temp99";
        public string xml_h_ds2 { get; set; } = "xml_h_ds2_v3_temp99";
        public string xml_h_ds3 { get; set; } = "xml_h_ds3_v3_temp99";
        public string xml_h_crit { get; set; } = "xml_h_crit_v3_temp99";
        public string xml_l_zglv { get; set; } = "xml_l_zglv_v3_temp99";
        public string xml_l_pers { get; set; } = "xml_l_pers_v3_temp99";
       
        
        public string xml_errors { get; set; } = "xml_errors";
        public bool TransferBD { get; set; } = false;

        public string schemaOracle_transfer { get; set; } = "asu12";
        public string xml_h_zglv_transfer { get; set; } = "xml_h_zglv_v3_temp100";
        public string xml_h_schet_transfer { get; set; } = "xml_h_schet_v3_temp100";
        public string xml_h_zap_transfer { get; set; } = "xml_h_zap_v3_temp100";
        public string xml_h_pacient_transfer { get; set; } = "xml_h_pacient_v3_temp100";
        public string xml_h_z_sluch_transfer { get; set; } = "xml_h_z_sluch_v3_temp100";
        public string xml_h_sluch_transfer { get; set; } = "xml_h_sluch_v3_temp100";
        public string xml_h_usl_transfer { get; set; } = "xml_h_usl_v3_temp100";
        public string xml_h_sank_smo_transfer { get; set; } = "xml_h_sank_v3_temp100";
        public string xml_h_nazr_transfer { get; set; } = "xml_h_nazr_v3_temp100";
        public string xml_l_zglv_transfer { get; set; } = "xml_l_zglv_v3_temp100";
        public string xml_l_pers_transfer { get; set; } = "xml_l_pers_v3_temp100";
        public string xml_h_ds2_n_transfer { get; set; } = "xml_h_ds2_n_v3_temp100";
        public string xml_h_kslp_transfer { get; set; } = "xml_h_kslp_v3_temp100";
        public string xml_h_napr_transfer { get; set; } = "xml_h_napr_v3_temp100";
        public string xml_h_b_prot_transfer { get; set; } = "xml_h_b_prot_v3_temp100";
        public string xml_h_b_diag_transfer { get; set; } = "xml_h_b_diag_v3_temp100";
        public string xml_h_onk_sl_transfer { get; set; } = "xml_h_onk_sl_v3_temp100";
        public string xml_h_lek_pr_transfer { get; set; } = "xml_h_lek_pr_v3_temp100";
        public string xml_h_date_inj_transfer { get; set; } = "xml_h_date_inj_v3_temp100";
        public string xml_h_cons_transfer { get; set; } = "xml_h_cons_v3_temp100";
        public string xml_h_onk_usl_transfer { get; set; } = "xml_h_onk_usl_v3_temp100";
        public string xml_h_sank_code_exp_transfer { get; set; } = "xml_h_sank_code_exp_v3_temp100";
        public string xml_h_ds3_transfer { get; set; } = "xml_h_ds3_v3_temp100";
        public string xml_h_ds2_transfer { get; set; } = "xml_h_ds2_v3_temp100";
        public string xml_h_crit_transfer { get; set; } = "xml_h_crit_v3_temp100";


        public bool MainTypePriem { get; set; } = false;
        public DateTime OtchetDate { get; set; } = DateTime.Now;
        public bool AUTO { get; set; } = false;
        public bool FILE_ON { get; set; } = false;

        public string PROC_CLEAR_TRANSFER { get; set; } = "";
        public string PROC_STATUS { get; set; } = "";
        public string PROC_STATUS_TRANSFER { get; set; } = "";
        public string PROC_CLEAR { get; set; } = "";
        public string ISP_NAME { get; set; } = "ФИО ДР";

        public string USER_PRIV { get; set; } = "";

    }





    public static class AppConfig
    {
        static public AppProperties Property = new AppProperties();
        static public void Save()
        {
            XmlSerializer ser = new XmlSerializer(typeof(AppProperties));
            Stream st = File.Create(Path.Combine(System.Windows.Forms.Application.StartupPath, "Setting.xml"));
            ser.Serialize(st, Property);
            st.Close();
        }
        public static void Load()
        {
            if (File.Exists(Path.Combine(System.Windows.Forms.Application.StartupPath, "Setting.xml")))
            {
                XmlSerializer ser = new XmlSerializer(typeof(AppProperties));
                Stream st = File.OpenRead(Path.Combine(System.Windows.Forms.Application.StartupPath, "Setting.xml"));
                Property = (AppProperties)ser.Deserialize(st);
                st.Close();
            }
            else
            {
                Save();
            }
        }

        static AppConfig()
        {
            Load();
        }
    }
}
