using System;
using System.Collections.Generic;
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
using MYBDOracle;
using ServiceLoaderMedpomData;

namespace ClientServiceWPF
{
    /// <summary>
    /// Логика взаимодействия для EdditProc.xaml
    /// </summary>
    public partial class EdditProc : Window
    {
        private IWcfInterface wcf => LoginForm.wcf;
        private CollectionViewSource CVSPARAM;

      
        public EdditProc(OrclProcedure _curr, string _connection)
        {
            curr = _curr;
           //currbackup = new OrclProcedure(curr);

            InitializeComponent();
            CVSPARAM = (CollectionViewSource) FindResource("CVSPARAM");
           // connection = _connection;

            ComboBoxPAR_DATATYPE.Items.Clear();
            ComboBoxPAR_DATATYPE.Items.Add(Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2.ToString());
            ComboBoxPAR_DATATYPE.Items.Add(Oracle.ManagedDataAccess.Client.OracleDbType.NVarchar2.ToString());
            ComboBoxPAR_DATATYPE.Items.Add(Oracle.ManagedDataAccess.Client.OracleDbType.Int32.ToString());
            ComboBoxPAR_DATATYPE.Items.Add(Oracle.ManagedDataAccess.Client.OracleDbType.Date.ToString());
            ComboBoxPAR_DATATYPE.Items.Add(Oracle.ManagedDataAccess.Client.OracleDbType.Decimal.ToString());

            ComboBoxPAR_VALTYPE.Items.Add(TypeParamValue.value);
            ComboBoxPAR_VALTYPE.Items.Add(TypeParamValue.TABLE_NAME_ZGLV);
            ComboBoxPAR_VALTYPE.Items.Add(TypeParamValue.TABLE_NAME_SCHET);
            ComboBoxPAR_VALTYPE.Items.Add(TypeParamValue.TABLE_NAME_ZAP);
            ComboBoxPAR_VALTYPE.Items.Add(TypeParamValue.TABLE_NAME_PACIENT);
            ComboBoxPAR_VALTYPE.Items.Add(TypeParamValue.TABLE_NAME_SLUCH);
            ComboBoxPAR_VALTYPE.Items.Add(TypeParamValue.TABLE_NAME_USL);
            ComboBoxPAR_VALTYPE.Items.Add(TypeParamValue.TABLE_NAME_SANK);
            ComboBoxPAR_VALTYPE.Items.Add(TypeParamValue.TABLE_NAME_L_ZGLV);
            ComboBoxPAR_VALTYPE.Items.Add(TypeParamValue.TABLE_NAME_L_PERS);
            ComboBoxPAR_VALTYPE.Items.Add(TypeParamValue.CurrYear);
            ComboBoxPAR_VALTYPE.Items.Add(TypeParamValue.CurrMonth);
            ComboBoxPAR_VALTYPE.SelectedIndex = 0;

            textBoxNAME.Text = curr.NAME_ERR;
            textBoxNAME_PROC.Text = curr.NAME_PROC;
            textBoxCOMM.Text = curr.Comment;
            refreshLV();
        }
        public OrclProcedure curr { get; set; }
     
        void refreshLV()
        {
            CVSPARAM.View.Refresh();
        }
        private void buttonLoadFromServer_Click(object sender, RoutedEventArgs e)
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
        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            curr.listParam.Add(new OrclParam(textBoxPAR_NAME.Text, OracleCheckingList.GetDbType(ComboBoxPAR_DATATYPE.Text), OrclParam.TypeParamValueFromStr(ComboBoxPAR_VALTYPE.Text), textBoxPAR_VALUE.Text, textBoxPAR_COMMENT.Text));
            refreshLV();
        }
        public List<OrclParam> SelectedParams => dataGridParam.SelectedCells.Select(x =>  x.Item as OrclParam).Where(x=>x!=null).Distinct().ToList();
        private void buttonChange_Click(object sender, RoutedEventArgs e)
        {
            var select = SelectedParams;
            if (select.Count != 0)
            {
                var item = select.First();
                item.Comment = textBoxPAR_COMMENT.Text;
                item.Name = textBoxPAR_NAME.Text;
                item.Type = OracleCheckingList.GetDbType(ComboBoxPAR_DATATYPE.Text);
                item.value = textBoxPAR_VALUE.Text;
                item.ValueType = OrclParam.TypeParamValueFromStr(ComboBoxPAR_VALTYPE.Text);
                refreshLV();
            }
        }
        private void buttonToUp_Click(object sender, RoutedEventArgs e)
        {
            var select = SelectedParams;
            if (select.Count != 0)
            {
                var item = select.First();
                int index = curr.listParam.IndexOf(item);
                if (index == 0) return;
               
                curr.listParam[index] = curr.listParam[index - 1];
                curr.listParam[index - 1] = item;
                dataGridParam.SelectedItems.Clear();
                dataGridParam.SelectedIndex = index - 1;
                refreshLV();
            }
        }
        private void buttonToDown_Click(object sender, RoutedEventArgs e)
        {
            var select = SelectedParams;
            if (select.Count != 0)
            {
                var item = select.First();
                int index = curr.listParam.IndexOf(item);
                if (index == curr.listParam.Count - 1) return;
                var p = curr.listParam[index];
                curr.listParam[index] = curr.listParam[index + 1];
                curr.listParam[index + 1] = p;
                dataGridParam.SelectedItems.Clear();
                dataGridParam.SelectedIndex = index + 1;
                refreshLV();
            }
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var select = SelectedParams;
            if (select.Count != 0 && textBoxPAR_COMMENT!=null)
            {
                var item = select.First();
                textBoxPAR_COMMENT.Text = item.Comment;
                textBoxPAR_NAME.Text = item.Name;
                ComboBoxPAR_DATATYPE.Text = item.Type.ToString();
                textBoxPAR_VALUE.Text = item.value  ;
                ComboBoxPAR_VALTYPE.Text = item.ValueType.ToString();

            }
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult =true;
            curr.NAME_ERR = textBoxNAME.Text;
            curr.NAME_PROC = textBoxNAME_PROC.Text;
            curr.Comment = textBoxCOMM.Text;
            Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
        private void MenuItemDeleteParam_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var select = SelectedParams;
                foreach (var s in select)
                {
                    curr.listParam.Remove(s);
                }
                refreshLV();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
