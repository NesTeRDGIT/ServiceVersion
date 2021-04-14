using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using ServiceLoaderMedpomData;

namespace ClientServise
{
    public partial class FilesManagerView : Form
    {
     
        IWcfInterface IWcfInterface => MainForm.MyWcfConnection;

        BindingList<FilePacket> list;
        public FilesManagerView(bool Activ, List<string> card)
        {
            InitializeComponent();
            list = new BindingList<FilePacket>();
            bindingSource1.DataSource = list;
            SetControlForm(card, Activ);
        }


        void SetControlForm(List<string> card, bool activ)
        {
            button4.Enabled = card.Contains("SetPriority");
            button2.Enabled = card.Contains("ClearFileManagerList") && activ;
            button7.Enabled = card.Contains("DelPack");
            button8.Enabled = card.Contains("SaveProcessArch");
            повторПроверкиToolStripMenuItem.Enabled = card.Contains("RepeatClosePac");
            закончитьОжиданияToolStripMenuItem.Enabled = card.Contains("StopTimeAway");
        }

        private void FilesManagerView_Load(object sender, EventArgs e)
        {
            UpdateList();
            IWcfInterface.RegisterNewFileManager();
        }

        private bool CancelUpdateList;
        public void UpdateList()
        {
            if (!CancelUpdateList)
            {
                var th = new Thread(UpdateListThread) { IsBackground = true };
                th.Start();
            }
        }

        public void UpdateListThread()
        {
            try
            {
                this.Invoke(new Action(() =>
                {
                    try
                    {
                        labelUpdateData.Text = @"Запрос данных...";
                        labelUpdateData.Visible = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }));

                var listtmp = IWcfInterface.GetFileManagerList();
                this.Invoke(new Action(() =>
                {
                    try
                    {
                        labelUpdateData.Text = @"Обновление списка...";
                        RefreshDate(ref list, listtmp);
                        labelUpdateData.Visible = false;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }


        private void button1_Click(object sender, EventArgs e)
        {
            UpdateList();
        }


        private void RefreshDate(ref BindingList<FilePacket> mainlist, List<FilePacket> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                if (i < mainlist.Count)
                    mainlist[i] = list[i];
                else
                    mainlist.Add(list[i]);
            }
            for (var i = list.Count; i < mainlist.Count; i++)
            {
                mainlist.RemoveAt(i);
            }
            if (list.Count == 0)
                mainlist.Clear();
        }
     
        private void FilesManagerView_FormClosing(object sender, FormClosingEventArgs e)
        {
            CancelUpdateList = true;
            IWcfInterface.UnRegisterNewFileManager();
            foreach (var form in this.OwnedForms)
            {
                form.Close();
            }
        }

        private void составФайловToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count != 0)
            {
                if (list != null)
                {
                    var form = new ShowFileItemForm(ref list, dataGridView1.SelectedRows[0].Index);
                    form.ShowDialog();
                }

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($@"Очистка пакетов приведет к отмене текущих операций вы уверены?{Environment.NewLine}Это также приведет к удалению всех связанных файлов", "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
            {
                IWcfInterface.ClearFileManagerList();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

    
        private void button3_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    PrintExcel(list, saveFileDialog1.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        void PrintExcel(BindingList<FilePacket> mainlist, string path)
        {
            try
            {
                var sort_list = mainlist.OrderBy(FilePacket => FilePacket);

                var efm = new ExcelFileManager();
                var StyleTop = efm.GetStyle(true, CellAlignment.CENTER, "", true, true);
                efm.PrintCell(0, 0, "№", StyleTop);
                efm.PrintCell(0, 1, "Код МО", StyleTop);
                efm.PrintCell(0, 2, "Наименование МО", StyleTop);
                efm.PrintCell(0, 3, "Состав файлов", StyleTop);
                efm.PrintCell(0, 4, "Файлы", StyleTop);
                efm.PrintCell(0, 5, "Результат", StyleTop);
                efm.PrintCell(0, 6, "Комментарий", StyleTop);
                var Rowindex = 1;
                var StyleValue = efm.GetStyle(true, CellAlignment.CENTER, "", false, true);
                var index = 1;
                foreach (var pack in sort_list)
                {
                    efm.PrintCell(Rowindex, 0, index.ToString(), StyleTop);
                    index++;
                    efm.PrintCell(Rowindex, 1, pack.codeMOstr, StyleTop);
                    efm.PrintCell(Rowindex, 2, pack.CaptionMO, StyleTop);
                    var countDD = 0;
                    var countDF = 0;
                    var countDO = 0;
                    var countDP = 0;
                    var countDR = 0;
                    var countDS = 0;
                    var countDU = 0;
                    var countDV = 0;
                    var countH = 0;
                    var countT = 0;
                    var countC = 0;
                    var CurrRow = Rowindex;
                    foreach (var fi in pack.Files)
                    {
                        switch (fi.Type)
                        {
                            case FileType.DD:
                                countDD++;
                                break;
                            case FileType.DF:
                                countDF++;
                                break;
                            case FileType.DO:
                                countDO++;
                                break;
                            case FileType.DP:
                                countDP++;
                                break;
                            case FileType.DR:
                                countDR++;
                                break;
                            case FileType.DS:
                                countDS++;
                                break;
                            case FileType.DU:
                                countDU++;
                                break;
                            case FileType.DV:
                                countDV++;
                                break;
                            case FileType.H:
                                countH++;
                                break;
                            case FileType.T:
                                countT++;
                                break;
                            case FileType.C:
                                countC++;
                                break;
                        }

                        var Style = efm.GetStyle(true, CellAlignment.CENTER, "", false, true);
                        if (fi.Process == StepsProcess.ErrorXMLxsd || fi.Process == StepsProcess.FlkErr ||
                            fi.Process == StepsProcess.Invite || fi.Process == StepsProcess.NotInvite
                           )
                            Style = efm.GetStyle(true, CellAlignment.CENTER, "", false, true, new NPOI.HSSF.Util.HSSFColor.Red());

                        efm.PrintCell(Rowindex, 4, fi.FileName, StyleValue);
                        efm.PrintCell(Rowindex, 5, fi.Process.ToString(), Style);
                        efm.PrintCell(Rowindex, 6, FilesItemStatus(fi.Process), Style);
                        efm.PrintCell(Rowindex, 7, fi.Comment, Style);
                        Rowindex++;
                    }
                    var tmp = "";
                    if (countH != 0)
                        tmp += "H(" + countH + ");";
                    if (countDD != 0)
                        tmp += "DD(" + countDD + ");";
                    if (countDF != 0)
                        tmp += "DF(" + countDF + ");";
                    if (countDO != 0)
                        tmp += "DO(" + countDO + ");";
                    if (countDP != 0)
                        tmp += "DP(" + countDP + ");";
                    if (countDR != 0)
                        tmp += "DR(" + countDR + ");";
                    if (countDS != 0)
                        tmp += "DS(" + countDS + ");";
                    if (countDU != 0)
                        tmp += "DU(" + countDU + ");";
                    if (countDV != 0)
                        tmp += "DV(" + countDV + ");";
                    if (countT != 0)
                        tmp += "T(" + countT + ");";
                    if (countC != 0)
                        tmp += "C(" + countC + ");";

                    var StyleRED = efm.GetStyle(true, CellAlignment.CENTER, "", false, true, new NPOI.HSSF.Util.HSSFColor.Red());
                    efm.PrintCell(CurrRow, 3, tmp, countH != 0 ? StyleValue : StyleRED);
                }
                efm.AutoSizeColumn(0, 8);
                efm.PrintCell(Rowindex, 1, "", StyleValue);
                efm.PrintCell(Rowindex, 0, "Всего: " + mainlist.Count + " пакетов", StyleValue);
                efm.AddMergedRegion(Rowindex, 0, Rowindex, 1);
                efm.SetBorder(0, 0, Rowindex, 6, StyleValue);
                efm.SaveToFile(path);
                if (MessageBox.Show(@"Выполнено. Показать файл?", "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    SANK_SMO.MTR2015.ShowSelectedInExplorer.FileOrFolder(path);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        string FilesItemStatus(StepsProcess value)
        {
            switch (value)
            {
                case StepsProcess.NotInvite: return "Файл не принят. Файл не корректен";
                case StepsProcess.Invite: return "Файл не принят. Файл корректен";
                case StepsProcess.ErrorXMLxsd: return "Файл не принят. Ошибка схемы документа";
                case StepsProcess.XMLxsd: return "Файл принят. Схема документа пройдена";
                case StepsProcess.FlkErr: return "Файл принят. Файл содержит ошибки ФЛК";
                case StepsProcess.FlkOk: return "Файл принят.";
                default: return "Неизвестно";
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count != 0)
            {
                IWcfInterface.SetPriority(dataGridView1.SelectedRows[0].Index, (int)numericUpDownPriority.Value);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var value = textBox1.Text.ToUpper();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                var item = (FilePacket)row.DataBoundItem;
                if (item.codeMOstr.ToUpper().IndexOf(value, StringComparison.Ordinal) != -1)
                {
                    dataGridView1.ClearSelection();
                    row.Selected = true;
                    dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows.IndexOf(row);
                    break;
                }
            }
        }

        int findindex = 0;
        private void button6_Click(object sender, EventArgs e)
        {
            if (findindex >= dataGridView1.Rows.Count)
            {
                findindex = 0;
                MessageBox.Show(@"Поиск достиг конца списка!");
                return;
            }
            var value = textBox2.Text.ToUpper();
            for (var i = findindex; i < dataGridView1.Rows.Count; i++)
            {
                var row = dataGridView1.Rows[i];
                var item = (FilePacket)row.DataBoundItem;
                if (item.CaptionMO.ToUpper().IndexOf(value, StringComparison.Ordinal) != -1)
                {
                    findindex = i + 1;
                    dataGridView1.ClearSelection();
                    row.Selected = true;
                    dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows.IndexOf(row);
                    return;
                }

            }
            MessageBox.Show(@"Поиск достиг конца списка!");
            findindex = 0;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            findindex = 0;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                button5_Click(button5, new EventArgs());
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                button6_Click(button6, new EventArgs());
            }
        }

        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.SelectedRows.Count != 0)
            {
                if (list != null)
                {
                    var form = new ShowFileItemForm(ref list, dataGridView1.SelectedRows[0].Index);
                    form.ShowDialog();
                }

            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count != 0)
            {
                if (MessageBox.Show($@"Вы уверены что хотите удалить пакет?", "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    IWcfInterface.DelPack(dataGridView1.SelectedRows[0].Index);
            }
        }




        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                var form = new ProgressForm();
                var th = new Thread(SaveProcessArch) {IsBackground = true};
                th.Start();
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void SaveProcessArch()
        {
            try
            {
                IWcfInterface.SaveProcessArch();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void повторПроверкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count != 0)
                {
                    var s = "Вы уверены что хотите повторить проверку пакета?";
                    if (dataGridView1.SelectedRows.Count != 1)
                        s = $"Вы уверены что хотите повторить проверку {dataGridView1.SelectedRows.Count} пакетов?";
                    if (MessageBox.Show(s, "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        IWcfInterface.RepeatClosePac(dataGridView1.SelectedRows.Cast<DataGridViewRow>().Select(x => x.Index).ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void закончитьОжиданияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count != 0)
                {
                    if (MessageBox.Show(@"Вы уверены что хотите закончить ожидание пакета?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        IWcfInterface.StopTimeAway(dataGridView1.SelectedRows[0].Index);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void прерватьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.SelectedRows.Count != 0)
                {
                    if (MessageBox.Show(@"Вы уверены что хотите прервать обработку пакета?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        IWcfInterface.BreackProcessPac(dataGridView1.SelectedRows[0].Index);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
