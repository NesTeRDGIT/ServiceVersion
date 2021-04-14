using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ServiceLoaderMedpomData;
namespace ClientServise.Roles
{
    public partial class RoleEdit : Form
    {
        IWcfInterface wcf;
        List<string> card;
        public RoleEdit(IWcfInterface wcf)
        {
            InitializeComponent();
            this.wcf = wcf;
            this.card = LoginForm.SecureCard;
        }

        private void RoleEdit_Load(object sender, EventArgs e)
        {
            SetControlForm(card);
            dataSetROLES_EDIT.MEDPOM_CLIENT_ROLES.Merge(wcf.Roles_GetRoles());
            dataSetROLES_EDIT.MEDPOM_CLIENT_CLAIMS.Merge(wcf.Roles_GetRolesClaims());
            dataSetROLES_EDIT.Method.Merge(wcf.Roles_GetMethod());
            
        }


        void SetControlForm(List<string> card)
        {
            button2.Enabled = card.Contains("Roles_AddRoles") && card.Contains("Roles_DeleteRoles") && card.Contains("Roles_UpdateRoles");
            button1.Enabled = card.Contains("Roles_GetMethod");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Roles.MethodEdit form = new Roles.MethodEdit(wcf);
            form.Owner = this;
            if (form.ShowDialog() == System.Windows.Forms.DialogResult.Retry)
            {
                dataSetROLES_EDIT.Method.Rows.Clear();
                dataSetROLES_EDIT.Method.Merge(
                wcf.Roles_GetMethod());
            }
        }

        private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {

                Roles.NewRoles f = new NewRoles(wcf);
                f.Owner = this;
                if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    dataSetROLES_EDIT.MEDPOM_CLIENT_ROLES.AddMEDPOM_CLIENT_ROLESRow(f.RoleName, f.RoleComment);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           

        }

        private void выбратьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridViewMethod.SelectedRows)
            {
                if (row.DataBoundItem == null) continue;

                foreach (DataGridViewRow rowRoles in dataGridViewRole.SelectedRows)
                {
                    if (rowRoles.DataBoundItem == null) continue;
                    var newrow = dataSetROLES_EDIT.MEDPOM_CLIENT_CLAIMS.NewMEDPOM_CLIENT_CLAIMSRow();
                    newrow.ROLE_ID = (decimal)(rowRoles.DataBoundItem as DataRowView).Row["ID"];
                    newrow.CLAIMS_ID = (decimal)(row.DataBoundItem as DataRowView).Row["ID"];   
                    newrow.NAME = (row.DataBoundItem as DataRowView).Row["NAME"].ToString();
                    newrow.Coment = (row.DataBoundItem as DataRowView).Row["Coment"].ToString();
                    dataSetROLES_EDIT.MEDPOM_CLIENT_CLAIMS.AddMEDPOM_CLIENT_CLAIMSRow(newrow);

                }

            }
            FillterMethod();
        }

        void FillterMethod()
        {
            string filter = "";
            if (dataGridViewClaims.Rows.Count != 0)
            {
                filter = "ID not in (";
                foreach (DataGridViewRow row in dataGridViewClaims.Rows)
                {
                    if (row != dataGridViewClaims.Rows[dataGridViewClaims.Rows.GetLastRow(DataGridViewElementStates.None)])
                        filter += "" + ((row.DataBoundItem as DataRowView).Row as DataSetROLES_EDIT.MEDPOM_CLIENT_CLAIMSRow).CLAIMS_ID + ",";
                    else
                        filter += "" + ((row.DataBoundItem as DataRowView).Row as DataSetROLES_EDIT.MEDPOM_CLIENT_CLAIMSRow).CLAIMS_ID + ")";

                }
            }
            methodBindingSource.Filter = filter;
        }

        private void dataGridViewRole_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewRole.SelectedRows.Count != 0)
            {
                mEDPOMCLIENTCLAIMSBindingSource.Filter = "ROLE_ID = " + dataGridViewRole.SelectedRows[0].Cells["iDDataGridViewTextBoxColumn"].Value.ToString();
                FillterMethod();
            }
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridViewRole.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Удаление роли приведет к лишений прав на эту роль у всех пользователей. Вы уверены что хотите удалить " + dataGridViewRole.SelectedRows.Count.ToString() + " ролей?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Yes)
                    foreach (DataGridViewRow row in dataGridViewRole.SelectedRows)
                    {
                        (row.DataBoundItem as DataRowView).Row.Delete();
                    }
            }
            

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {

            foreach (DataGridViewRow row in dataGridViewClaims.SelectedRows)
            {
                (row.DataBoundItem as DataRowView).Row.Delete();
            }
            FillterMethod();

        }

        private void mEDPOMCLIENTCLAIMSBindingSource_BindingComplete(object sender, BindingCompleteEventArgs e)
        {

        }

        private void mEDPOMCLIENTCLAIMSBindingSource_ListChanged(object sender, ListChangedEventArgs e)
        {
            

        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Вы уверены?", "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    foreach (DataSetROLES_EDIT.MEDPOM_CLIENT_ROLESRow row in dataSetROLES_EDIT.MEDPOM_CLIENT_ROLES)
                    {

                        switch (row.RowState)
                        {
                            case DataRowState.Added:
                                int id = wcf.Roles_AddRoles(row.ROLE_NAME, row.ROLE_COMMENT);
                                row.ID = id;
                                break;
                            case DataRowState.Deleted:
                                wcf.Roles_DeleteRoles(Convert.ToInt32(row.Field<decimal>("ID", DataRowVersion.Original)));
                                break;
                            case DataRowState.Modified:
                                wcf.Roles_UpdateRoles(row.ROLE_NAME, row.ROLE_COMMENT, (int)row.ID);
                                break;

                        }
                    }


                    
                    foreach (DataSetROLES_EDIT.MEDPOM_CLIENT_CLAIMSRow row in dataSetROLES_EDIT.MEDPOM_CLIENT_CLAIMS)
                    {

                        switch (row.RowState)
                        {
                            case DataRowState.Added:
                                wcf.Roles_AddClaims((int)row.ROLE_ID, (int)row.CLAIMS_ID);
                                break;
                            case DataRowState.Deleted:
                                wcf.Roles_DeleteClaims(Convert.ToInt32(row.Field<decimal>("ROLE_ID", DataRowVersion.Original)),Convert.ToInt32( row.Field<decimal>("CLAIMS_ID", DataRowVersion.Original)));
                                break;
                            case DataRowState.Modified:
                                wcf.Roles_UpdateClaims((int)row.ROLE_ID, (int)row.CLAIMS_ID,Convert.ToInt32( row.Field<decimal>("ROLE_ID", DataRowVersion.Original)), Convert.ToInt32(row.Field<decimal>("CLAIMS_ID", DataRowVersion.Original)));
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

        private void RoleEdit_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (Form form in this.OwnedForms)
            {

                form.Close();
            }
        }
    }
}
