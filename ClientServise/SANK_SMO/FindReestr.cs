using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace ClientServise.SANK_SMO
{
    public partial class FindReestr : Form
    {
        public FindReestr()
        {
            InitializeComponent();
        }
        public FindReestr(int code_mo,int CODE,int Year)
        {
            InitializeComponent();
            textBoxCODE.Text = CODE.ToString();
            textBoxCODE_MO.Text = code_mo.ToString();
            textBoxYEAR.Text = Year.ToString();
            button1_Click(button1, new EventArgs());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string code = textBoxCODE.Text.Trim();
                string code_mo = textBoxCODE_MO.Text.Trim();
                int? Year = null;
                if (textBoxYEAR.Text != "")
                {
                    Year = Convert.ToInt32(textBoxYEAR.Text);
                }

                dataGridView1.DataSource = GetSchetBase(code, code_mo, Year);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }
         public DataTable GetSchetBase(string CODE, string CODE_MO, int? YEAR)
        {
             
             string sql = "";
           if( AppConfig.Property.schemaOracle!="")
           {
               sql = "select z.filename,s.* from "+AppConfig.Property.schemaOracle+"."+ AppConfig.Property.xml_h_zglv+@"  z
               inner join "+AppConfig.Property.schemaOracle+"."+ AppConfig.Property.xml_h_schet+" s on (z.zglv_id = s.zglv_id)";
           }
           else
           {
                              sql = "select z.filename,s.* from "+AppConfig.Property.xml_h_zglv+@"  z
               inner join "+ AppConfig.Property.xml_h_schet+" s on (z.zglv_id = s.zglv_id)";
           }
          List<string> USL = new List<string>();
          if(CODE!="")
          {
              USL.Add("s.CODE = :CODE");
          }
          if (CODE_MO != "")
          {
              USL.Add("s.CODE_MO = :CODE_MO");
          }
          if (YEAR.HasValue)
          {
              USL.Add("s.YEAR_BASE = :YEAR");
          }

             string t ="";
          foreach (string s in USL)
          {
              if (s != USL.Last())
                  t += s + " and ";
              else
                  t += s;               
          }
          sql += " where " + t;
    
            OracleDataAdapter oda = new OracleDataAdapter(sql,  new OracleConnection(AppConfig.Property.ConnectionString ));
            if (CODE != "")
            {
                oda.SelectCommand.Parameters.Add("CODE", CODE);
            }
            if (CODE_MO != "")
            {
                oda.SelectCommand.Parameters.Add("CODE_MO", CODE_MO);
            }
            if (YEAR.HasValue)
            {
                oda.SelectCommand.Parameters.Add("YEAR", YEAR.Value);
            }
             DataTable tbl = new DataTable();
             oda.Fill(tbl);
             return tbl;
           
        }
         public int ZGLV_ID;
         private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
         {
             ZGLV_ID = Convert.ToInt32((dataGridView1.Rows[e.RowIndex].DataBoundItem as DataRowView).Row["ZGLV_ID"]);
             DialogResult = System.Windows.Forms.DialogResult.OK;
             this.Close();
         }
    }
}
