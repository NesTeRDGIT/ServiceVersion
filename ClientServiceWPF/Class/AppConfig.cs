using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ClientServiceWPF.Class
{
    public class AppProperties
    {

        public string xml_h_zglv { get; set; }
        public string xml_h_schet { get; set; }
        public string xml_h_zap { get; set; }
        public string xml_h_pacient { get; set; }
        public string xml_h_z_sluch { get; set; }
        public string xml_h_sluch { get; set; }
        public string xml_h_napr { get; set; }
        public string xml_h_nazr { get; set; }
        public string xml_h_ds2_n { get; set; }
        public string xml_h_kslp { get; set; }
        public string xml_h_b_diag { get; set; }
        public string xml_h_b_prot { get; set; }
        public string xml_h_usl { get; set; }
        public string xml_h_sank { get; set; }
        public string xml_l_zglv { get; set; }
        public string xml_l_pers { get; set; }
        public string schemaOracle { get; set; }
        public string ConnectionString { get; set; }
        public string seq_ZGLV { get; set; }
        public string seq_SCHET { get; set; }
        public string seq_ZAP { get; set; }
        public string seq_PACIENT { get; set; }
        public string seq_z_sluch { get; set; }
        public string seq_SLUCH { get; set; }
        //public string seq_ONK_SL { get; set; }
        public string seq_USL { get; set; }
        public string seq_SANK { get; set; }
        public string seq_L_ZGLV { get; set; }
        public string seq_L_pers { get; set; }
        public string seq_schemaOracle { get; set; }
        public string seq_xml_h_lek_pr { get; set; }
        public string seq_xml_h_onk_usl { get; set; }
        public string xml_h_onk_usl { get; set; }
        public string xml_h_lek_pr { get; set; }
        public string xml_h_date_inj { get; set; }
        public string xml_h_cons { get; set; }
        public string xml_h_code_exp { get; set; }
        public string xml_h_ds2 { get; set; }
        public string xml_h_ds3 { get; set; }
        public string xml_h_crit { get; set; }
        public string xml_h_mr_usl_n { get;  set; }
    }


    public static class AppConfig
    {
        public static AppProperties Property = new AppProperties();

        private static string  LocalFolder => AppDomain.CurrentDomain.BaseDirectory;
        public static void Save()
        {
            var ser = new XmlSerializer(typeof(AppProperties));
            Stream st = File.Create(Path.Combine(LocalFolder, "Setting.xml"));
            ser.Serialize(st, Property);
            st.Close();
        }
        public static void Load()
        {
            if (File.Exists(Path.Combine(LocalFolder, "Setting.xml")))
            {
                var ser = new XmlSerializer(typeof(AppProperties));
                Stream st = File.OpenRead(Path.Combine(LocalFolder, "Setting.xml"));
                Property = (AppProperties)ser.Deserialize(st);
                st.Close();
            }
        }
        static AppConfig()
        {
            Load();
        }
    }
}
