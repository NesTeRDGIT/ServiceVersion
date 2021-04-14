using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ServiceLoaderMedpomData;

namespace ClientServise
{
    public partial class EdditProc : Form
    {
        ChekingList list;
        TableName name;
        int index;

        OrclProcedure curr;

        OrclProcedure currbackup;
        string connection;
        IWcfInterface wcf
        {
            get
            {
                return MainForm.MyWcfConnection;
            }
        }
        public EdditProc( OrclProcedure _curr,string _connection)
        {
            InitializeComponent();
            connection = _connection;
            comboBoxTypeParam.Items.Clear();
            comboBoxTypeParam.Items.Add(Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2.ToString());
            comboBoxTypeParam.Items.Add(Oracle.ManagedDataAccess.Client.OracleDbType.NVarchar2.ToString());
            comboBoxTypeParam.Items.Add(Oracle.ManagedDataAccess.Client.OracleDbType.Int32.ToString());
            comboBoxTypeParam.Items.Add(Oracle.ManagedDataAccess.Client.OracleDbType.Date.ToString());

            curr = _curr;
            currbackup = new OrclProcedure(curr);



            textBoxNameErr.Text = curr.NAME_ERR;
            textBoxNAME_PROC.Text = curr.NAME_PROC;
            textBoxComm.Text = curr.Comment;
            comboBoxTypeValue.Items.Add(TypeParamValue.value);
            comboBoxTypeValue.Items.Add(TypeParamValue.TABLE_NAME_ZGLV);
            comboBoxTypeValue.Items.Add(TypeParamValue.TABLE_NAME_SCHET);
            comboBoxTypeValue.Items.Add(TypeParamValue.TABLE_NAME_ZAP);
            comboBoxTypeValue.Items.Add(TypeParamValue.TABLE_NAME_PACIENT);
            comboBoxTypeValue.Items.Add(TypeParamValue.TABLE_NAME_SLUCH);
            comboBoxTypeValue.Items.Add(TypeParamValue.TABLE_NAME_USL);
            comboBoxTypeValue.Items.Add(TypeParamValue.TABLE_NAME_SANK);
            comboBoxTypeValue.Items.Add(TypeParamValue.TABLE_NAME_L_ZGLV);
            comboBoxTypeValue.Items.Add(TypeParamValue.TABLE_NAME_L_PERS);
            comboBoxTypeValue.Items.Add(TypeParamValue.CurrYear);
            comboBoxTypeValue.Items.Add(TypeParamValue.CurrMonth);
            comboBoxTypeValue.SelectedIndex = 0;
            refreshLV();
        }
        void refreshLV()
        {
            int selectindex = 0;
            if(listView1.SelectedItems.Count!=0)
              selectindex = listView1.SelectedItems[0].Index;
            listView1.Items.Clear();
            foreach (OrclParam par in curr.listParam)
            {
                ListViewItem LVI = new ListViewItem(par.Name);
                LVI.SubItems.Add(par.Type.ToString());
                LVI.SubItems.Add(par.ValueType.ToString());
                LVI.SubItems.Add(par.value.ToString());
                LVI.SubItems.Add(par.Comment.ToString());
                listView1.Items.Add(LVI);
            }
            if (listView1.Items.Count != 0 && selectindex < listView1.Items.Count) listView1.SelectedIndices.Add(selectindex);
        }
        public EdditProc(ref ChekingList check, TableName _name, int _index)
        {
            InitializeComponent();
            list = check;
            name = _name;
            index = _index;
            curr = list[name, index];
        }

        private void EdditProc_Load(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {
                textBoxNameParam.Text = curr.listParam[listView1.SelectedItems[0].Index].Name;
                textBoxDefaultValue.Text = curr.listParam[listView1.SelectedItems[0].Index].value;
                comboBoxTypeParam.Text = curr.listParam[listView1.SelectedItems[0].Index].Type.ToString();
                textBoxComment.Text = curr.listParam[listView1.SelectedItems[0].Index].Comment;
                comboBoxTypeValue.Text = curr.listParam[listView1.SelectedItems[0].Index].ValueType.ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {
                curr.listParam[listView1.SelectedItems[0].Index].Comment = textBoxComment.Text;
                curr.listParam[listView1.SelectedItems[0].Index].Name = textBoxNameParam.Text;
                curr.listParam[listView1.SelectedItems[0].Index].Type = OrclProcedure.GetDataType(comboBoxTypeParam.Text);
                curr.listParam[listView1.SelectedItems[0].Index].value = textBoxDefaultValue.Text;
                curr.listParam[listView1.SelectedItems[0].Index].ValueType = OrclParam.TypeParamValueFromStr(comboBoxTypeValue.Text);
                refreshLV();
            }
        }

        private void buttonAddParam_Click(object sender, EventArgs e)
        {
            curr.listParam.Add(new OrclParam(textBoxNameParam.Text, OrclProcedure.GetDataType(comboBoxTypeParam.Text), OrclParam.TypeParamValueFromStr(comboBoxTypeValue.Text), textBoxDefaultValue.Text, textBoxComment.Text));
            refreshLV();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            curr.NAME_ERR = currbackup.NAME_ERR;
            curr.NAME_PROC = currbackup.NAME_PROC;
            curr.STATE = currbackup.STATE;
            curr.TYPE_ERR = currbackup.TYPE_ERR;
            curr.listParam = currbackup.listParam;
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void textBoxNameErr_TextChanged(object sender, EventArgs e)
        {
            curr.NAME_ERR = textBoxNameErr.Text;
        }

        private void textBoxNAME_PROC_TextChanged(object sender, EventArgs e)
        {
            curr.NAME_PROC = textBoxNAME_PROC.Text;
        }


        private void button4_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {
                
                int index = listView1.SelectedItems[0].Index;
                if (index == 0) return;
                OrclParam p = curr.listParam[index];
                curr.listParam[index] = curr.listParam[index - 1];
                curr.listParam[index - 1] = p;
                listView1.SelectedItems.Clear();
                listView1.Items[index - 1].Selected = true;
                refreshLV();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count != 0)
            {

                int index = listView1.SelectedItems[0].Index;
                if (index == curr.listParam.Count-1) return;
                OrclParam p = curr.listParam[index];
                curr.listParam[index] = curr.listParam[index + 1];
                curr.listParam[index + 1] = p;
                listView1.SelectedItems.Clear();
                listView1.Items[index + 1].Selected = true;
                refreshLV();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                curr.listParam.AddRange(wcf.GetParam(textBoxNAME_PROC.Text));
                refreshLV();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void обновитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            refreshLV();
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for(int i =listView1.SelectedIndices.Count-1; i>=0;i--)
            {
                curr.listParam.RemoveAt(listView1.SelectedIndices[i]);
            }
            refreshLV();
        }


    }
}
