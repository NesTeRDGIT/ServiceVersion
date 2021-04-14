using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ClientServiceWPF.Class;
using Oracle.ManagedDataAccess.Client;

namespace ClientServiceWPF.SANK_INVITER
{
    /// <summary>
    /// Логика взаимодействия для FindReestr.xaml
    /// </summary>
    public partial class FindReestr : Window
    {
      
        public FindReestr(int code_mo, int CODE, int Year)
        {
            InitializeComponent();
            TextBoxCODE.Text = CODE.ToString();
            TextBoxCODE_MO.Text = code_mo.ToString();
            TextBoxYEAR.Text = Year.ToString();
            buttonFind_Click(buttonFind, new RoutedEventArgs());
        }

        private void buttonFind_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var code = TextBoxCODE.Text.Trim();
                var code_mo = TextBoxCODE_MO.Text.Trim();
                int? Year = null;
                if (TextBoxYEAR.Text != "")
                {
                    Year = Convert.ToInt32(TextBoxYEAR.Text);
                }

                GetSchetBase(code, code_mo, Year);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


      
        public List<SCHET_ROW> ListSchet { get; set; } = new List<SCHET_ROW>();
        public void GetSchetBase(string CODE, string CODE_MO, int? YEAR)
        {
            ListSchet.Clear();
            var owner = "";
            if (!string.IsNullOrEmpty(AppConfig.Property.schemaOracle))
            {
                owner = $"{AppConfig.Property.schemaOracle}.";
            }
            var oda = new OracleDataAdapter("", new OracleConnection(AppConfig.Property.ConnectionString));
            var USL = new List<string>();
            if (!string.IsNullOrEmpty(CODE))
            {
                oda.SelectCommand.Parameters.Add("CODE", CODE);
                USL.Add("s.CODE = :CODE");
            }
            if (!string.IsNullOrEmpty(CODE_MO))
            {
                oda.SelectCommand.Parameters.Add("CODE_MO", CODE_MO);
                USL.Add("s.CODE_MO = :CODE_MO");
            }
            if (YEAR.HasValue)
            {
                oda.SelectCommand.Parameters.Add("YEAR", YEAR.Value);
                USL.Add("s.YEAR_BASE = :YEAR");
            }
            oda.SelectCommand.CommandText = $"select z.filename,s.* from {owner}{AppConfig.Property.xml_h_zglv} z inner join {owner}{AppConfig.Property.xml_h_schet} s on (z.zglv_id = s.zglv_id) where {string.Join(" and ", USL)}";
            var tbl = new DataTable();
            oda.Fill(tbl);
            ListSchet.AddRange(SCHET_ROW.Get(tbl.Select()));

        }

        public int ZGLV_ID { get; set; }

        private void TextBoxCODE_MO_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                buttonFind_Click(buttonFind, new RoutedEventArgs());
            }
        }

        private List<SCHET_ROW> selectedSchetRows => dataGrid.SelectedCells.Select(x => (SCHET_ROW) x.Item).Distinct().ToList();

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var item = selectedSchetRows.FirstOrDefault();
                if (item != null)
                {
                    ZGLV_ID = item.ZGLV_ID;
                    DialogResult = true;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
    public class SCHET_ROW
    {
        public static List<SCHET_ROW> Get(IEnumerable<DataRow> rows)
        {
            return rows.Select(Get).ToList();
        }

        public static SCHET_ROW Get(DataRow row)
        {
            try
            {
                var item = new SCHET_ROW();
                item.FILENAME = Convert.ToString(row["FILENAME"]);
                item.ZGLV_ID = Convert.ToInt32(row["ZGLV_ID"]);
                item.SCHET_ID = Convert.ToInt32(row["SCHET_ID"]);
                item.CODE_MO = Convert.ToString(row["CODE_MO"]);
                item.YEAR = Convert.ToInt32(row["YEAR"]);
                item.MONTH = Convert.ToInt32(row["MONTH"]);
                item.NSCHET = Convert.ToString(row["NSCHET"]);
                item.DSCHET = Convert.ToDateTime(row["DSCHET"]);
                item.PLAT = Convert.ToString(row["PLAT"]);
                if (row["SUMMAV"] != DBNull.Value)
                    item.SUMMAV = Convert.ToDecimal(row["SUMMAV"]);
                item.COMENTS = Convert.ToString(row["COMENTS"]);
                if (row["SUMMAP"] != DBNull.Value)
                    item.SUMMAP = Convert.ToDecimal(row["SUMMAP"]);
                if (row["SANK_MEK"] != DBNull.Value)
                    item.SANK_MEK = Convert.ToDecimal(row["SANK_MEK"]);
                if (row["SANK_MEE"] != DBNull.Value)
                    item.SANK_MEE = Convert.ToDecimal(row["SANK_MEE"]);
                if (row["SANK_EKMP"] != DBNull.Value)
                    item.SANK_EKMP = Convert.ToDecimal(row["SANK_EKMP"]);
                item.DISP = Convert.ToString(row["DISP"]);
                item.DOP_FLAG = Convert.ToBoolean(row["DOP_FLAG"]);
                item.YEAR_BASE = Convert.ToInt32(row["YEAR_BASE"]);
                item.MONTH_BASE = Convert.ToInt32(row["MONTH_BASE"]);

                return item;
            }
            catch (Exception e)
            {
                throw new Exception($"Ошибка формирования SCHET_ROW:{e.Message}", e);
            }
        }

        public string FILENAME { get; set; }
        public int ZGLV_ID { get; set; }
        public int SCHET_ID { get; set; }
        public string CODE_MO { get; set; }
        public int YEAR { get; set; }
        public int MONTH { get; set; }
        public string NSCHET { get; set; }
        public DateTime DSCHET { get; set; }
        public string PLAT { get; set; }
        public decimal? SUMMAV { get; set; }
        public string COMENTS { get; set; }
        public decimal? SUMMAP { get; set; }
        public decimal? SANK_MEK { get; set; }
        public decimal? SANK_MEE { get; set; }
        public decimal? SANK_EKMP { get; set; }
        public string DISP { get; set; }
        public bool DOP_FLAG { get; set; }
        public int YEAR_BASE { get; set; }
        public int MONTH_BASE { get; set; }

    }
}
