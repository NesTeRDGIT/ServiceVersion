using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ServiceLoaderMedpomData;
using System.Reflection;

namespace ClientServise.Roles
{
    public partial class MethodEdit : Form
    {
        IWcfInterface wcf;
        List<string> card;
        public MethodEdit(IWcfInterface wcf)
        {
            InitializeComponent();
            this.wcf = wcf;
            this.card = LoginForm.SecureCard;
        }

        private void MethodEdit_Load(object sender, EventArgs e)
        {
            SetControlForm(card);
            DataTable tbl = new DataTable();

            tbl = wcf.Roles_GetMethod();
            dataSetROLES_EDIT1.Method.Merge(tbl);
            var Met = ReflectClass.MethodReflectInfo<IWcfInterface>();
            foreach (DataSetROLES_EDIT.MethodRow row in dataSetROLES_EDIT1.Method)
            {
                if (row.RowState == DataRowState.Deleted) continue;
                try
                {
                    Met.First(delegate(MethodInfo item)
                    {
                        return item.Name == row.NAME;
                    });
                    row.CHECKED = true;
                }
                catch (InvalidOperationException)
                {
                    row.CHECKED = false;
                }
            }
            Recolor();
            ResetMethod();
            
        }


        private void Recolor()
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["cHECKEDDataGridViewCheckBoxColumn"].Value == DBNull.Value) continue;
                row.DefaultCellStyle.BackColor = dataGridView1.RowsDefaultCellStyle.BackColor;
                switch (Convert.ToBoolean(row.Cells["cHECKEDDataGridViewCheckBoxColumn"].Value))
                {
                    case false: row.DefaultCellStyle.BackColor = Color.Tomato;
                        break;
                    default:
                     
                        break;
                }
            }
        }


        void SetControlForm(List<string> card)
        {      
            button2.Enabled = card.Contains("Roles_AddMethod") && card.Contains("Roles_DeleteMethod") && card.Contains("Roles_UpdateMethod");
        }

        void ResetMethod()
        {
            var Met = ReflectClass.MethodReflectInfo<IWcfInterface>();
            listBox1.Items.Clear();
            foreach (MethodInfo mi in Met)
            {               
                if (dataSetROLES_EDIT1.Method.Select("name = '"+mi.Name+"'").Count()==0)
                    listBox1.Items.Add(mi.Name);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                foreach (int i in listBox1.SelectedIndices)
                {
                    var t = dataSetROLES_EDIT1.Method.NewMethodRow();
                    t.NAME = listBox1.Items[i].ToString();
                    t.COMENT = textBoxComment.Text.ToUpper().Trim();
                    dataSetROLES_EDIT1.Method.AddMethodRow(t);

                }
                ResetMethod();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Вы уверены?", "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    foreach (DataSetROLES_EDIT.MethodRow row in dataSetROLES_EDIT1.Method)
                    {

                        switch (row.RowState)
                        {
                            case DataRowState.Added:
                                wcf.Roles_AddMethod(row.NAME, row.COMENT);
                                break;
                            case DataRowState.Deleted:
                                wcf.Roles_DeleteMethod((int)row.Field<decimal>("ID", DataRowVersion.Original));
                                break;
                            case DataRowState.Modified:
                                wcf.Roles_UpdateMethod(row.NAME, row.COMENT, (int)row.Field<decimal>("ID", DataRowVersion.Original));
                                break;

                        }
                    }

                    DialogResult = System.Windows.Forms.DialogResult.Retry;
                    Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            
        }


  
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

            if (dataGridView1.SelectedRows.Count>0)
            {
                if (MessageBox.Show("Удаление методов приведет к лишений прав на этот метод у всех ролей. Вы уверены что хотите удалить " + dataGridView1.SelectedRows.Count.ToString() + " методов?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Yes)
                {
                    List<DataSetROLES_EDIT.MethodRow> list = new List<DataSetROLES_EDIT.MethodRow>();
                    foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                    {
                        list.Add(((row.DataBoundItem as DataRowView).Row as DataSetROLES_EDIT.MethodRow));  
                    }

                    foreach (DataSetROLES_EDIT.MethodRow row in list)
                    {
                        row.Delete(); 
                    }
                ResetMethod();
                }
            }
        }

        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.Reset || e.ListChangedType == ListChangedType.ItemChanged)
            {
                Recolor();
            }
        }
    }
}
