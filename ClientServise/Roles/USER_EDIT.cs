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
    public partial class USER_EDIT : Form
    {
        IWcfInterface wcf
        {
            get
            {
                return MainForm.MyWcfConnection;
            }
        }


        List<string> card
        {
            get
            {
                return LoginForm.SecureCard;
            }
        }
        public USER_EDIT()
        {
            InitializeComponent();
           
        }

        private void button1_Click(object sender, EventArgs e)
        {
           
        }

        private void USER_EDIT_Load(object sender, EventArgs e)
        {
            SetControlForm(card);
            dataSetROLES_EDIT.MEDPOM_CLIENT_ROLES.Merge(wcf.Roles_GetRoles());
            dataSetROLES_EDIT.MEDPOM_CLIENT_US_ROL.Merge(wcf.Roles_GetUsers_Roles());
            dataSetROLES_EDIT.MEDPOM_CLIENT_USERS.Merge(wcf.Roles_GetUsers());
        }



        void SetControlForm(List<string> card)
        {

            button2.Enabled = card.Contains("Roles_UpdateUsers") && card.Contains("Roles_DeleteUsers") &&  card.Contains("Roles_AddUsers");
            button1.Enabled = card.Contains("Roles_GetRoles") && card.Contains("Roles_GetRolesClaims") && card.Contains("Roles_GetMethod"); 

        }


        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dataGridViewUser.CurrentCell.ColumnIndex ==2)//select target column
            {
                TextBox textBox = e.Control as TextBox;
                if (textBox != null)
                {
                    textBox.UseSystemPasswordChar = true;
                }
            }
        }

        private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                EditUser f = new EditUser("", "");
                if (f.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var row = dataSetROLES_EDIT.MEDPOM_CLIENT_USERS.NewMEDPOM_CLIENT_USERSRow();
                    row.NAME = f.name;
                    row.PASS = f.Password;
                    dataSetROLES_EDIT.MEDPOM_CLIENT_USERS.AddMEDPOM_CLIENT_USERSRow(row);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {


            if (dataGridViewUser.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Вы уверены что хотите удалить " + dataGridViewUser.SelectedRows.Count.ToString() + " пользователей?", "Предупреждение", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == System.Windows.Forms.DialogResult.Yes)

                    foreach (DataGridViewRow row in dataGridViewUser.SelectedRows)
                    {
                        (row.DataBoundItem as DataRowView).Row.Delete();

                    }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            Roles.RoleEdit rol = new RoleEdit(wcf);
            rol.Owner = this;
            rol.ShowDialog();
        }

        private void dataGridViewUser_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        void FillterRoles()
        {
            string filter = "";
            if (dataGridViewUS_ROLES.Rows.Count != 0)
            {
                filter = "ID not in (";
                foreach (DataGridViewRow row in dataGridViewUS_ROLES.Rows)
                {
                    if (row != dataGridViewUS_ROLES.Rows[dataGridViewUS_ROLES.Rows.GetLastRow(DataGridViewElementStates.None)])
                        filter += "" + ((row.DataBoundItem as DataRowView).Row as DataSetROLES_EDIT.MEDPOM_CLIENT_US_ROLRow).ROLE_ID + ",";
                    else
                        filter += "" + ((row.DataBoundItem as DataRowView).Row as DataSetROLES_EDIT.MEDPOM_CLIENT_US_ROLRow).ROLE_ID + ")";

                }
            }
            mEDPOMCLIENTROLESBindingSource.Filter = filter;
        }

        private void dataGridViewUser_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewUser.SelectedRows.Count != 0)
            {
                mEDPOMCLIENTUSROLBindingSource.Filter = "USER_ID = " + dataGridViewUser.SelectedRows[0].Cells["iDDataGridViewTextBoxColumn1"].Value.ToString();
                FillterRoles();
            }
        }

        private void выбратьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridViewRoles.SelectedRows)
            {
                if (row.DataBoundItem == null) continue;

                foreach (DataGridViewRow rowUser in dataGridViewUser.SelectedRows)
                {
                    if (rowUser.DataBoundItem == null) continue;
                    var newrow = dataSetROLES_EDIT.MEDPOM_CLIENT_US_ROL.NewMEDPOM_CLIENT_US_ROLRow();
                    newrow.USER_ID = (decimal)(rowUser.DataBoundItem as DataRowView).Row["ID"];
                    newrow.ROLE_ID = (decimal)((row.DataBoundItem as DataRowView).Row as DataSetROLES_EDIT.MEDPOM_CLIENT_ROLESRow).ID;
                    newrow.ROLE_NAME = ((row.DataBoundItem as DataRowView).Row as DataSetROLES_EDIT.MEDPOM_CLIENT_ROLESRow).ROLE_NAME;
                    newrow.ROLE_COMMENT = ((row.DataBoundItem as DataRowView).Row as DataSetROLES_EDIT.MEDPOM_CLIENT_ROLESRow).ROLE_COMMENT;

                    dataSetROLES_EDIT.MEDPOM_CLIENT_US_ROL.AddMEDPOM_CLIENT_US_ROLRow(newrow);

                }

            }
            FillterRoles();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Вы уверены?", "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    foreach (DataSetROLES_EDIT.MEDPOM_CLIENT_USERSRow row in dataSetROLES_EDIT.MEDPOM_CLIENT_USERS)
                    {

                        switch (row.RowState)
                        {
                            case DataRowState.Added:
                                int id = wcf.Roles_AddUsers(row.NAME, row.PASS);
                                row.ID = id;
                                break;
                            case DataRowState.Deleted:
                                wcf.Roles_DeleteUsers((int)row.Field<decimal>("ID", DataRowVersion.Original));
                                break;
                            case DataRowState.Modified:
                                wcf.Roles_UpdateUsers(row.NAME, row.PASS, (int)row.ID);
                                break;

                        }
                    }



                    foreach (DataSetROLES_EDIT.MEDPOM_CLIENT_US_ROLRow row in dataSetROLES_EDIT.MEDPOM_CLIENT_US_ROL)
                    {

                        switch (row.RowState)
                        {
                            case DataRowState.Added:
                                wcf.Roles_AddUsers_Role((int)row.USER_ID, (int)row.ROLE_ID);
                                break;
                            case DataRowState.Deleted:
                                wcf.Roles_DeleteUsers_Role(row.Field<int>("USER_ID", DataRowVersion.Original), row.Field<int>("ROLE_ID", DataRowVersion.Original));
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

        private void USER_EDIT_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (Form form in this.OwnedForms)
            {
                form.Close();
            }
        }


    }





    

}
