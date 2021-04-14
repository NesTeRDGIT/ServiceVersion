using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClientServise
{
    public partial class ACT_MEK : Form
    {
        public ACT_MEK()
        {
            InitializeComponent();
        }

        public List<MO_ITEM> MO_LIST = new List<MO_ITEM>();

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                MO_LIST.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }


    public class MO_ITEM
    {
        public static List<MO_ITEM> Get(IEnumerable<DataRow> rows)
        {
            return rows.Select(Get).ToList();
        }
        public static MO_ITEM Get(DataRow row)
        {
            try
            {
                var item = new MO_ITEM();
                item.CODE_MO = row["CODE_MO"].ToString();
                return item;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения MO_ITEM: {ex.Message}",ex);
            }
        }
        public string CODE_MO { get; set; }
        public int YEAR { get; set; }
        public int MONTH { get; set; }
        public string N_ACT { get; set; }
        public DateTime D_ACT { get; set; }
    }
}
