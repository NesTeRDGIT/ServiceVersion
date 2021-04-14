using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using ServiceLoaderMedpomData;
using ClientServise.SANK_SMO.MTR2015;
using System.IO;

namespace ClientServise
{
    public partial class ShowFileItemForm : Form
    {
        BindingList<FilePacket> list;
        int index;
        public ShowFileItemForm(ref BindingList<FilePacket> _list, int _index)
        {
            InitializeComponent();
            try
            {
                list = _list;
                index = _index;
                bindingSource1.DataSource = list[index].Files;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }




        private void ShowFileItemForm_Load(object sender, EventArgs e)
        {

        }


        private void ShowFileItemForm_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {
            try
            {
                fileItemBaseBindingSource.DataSource = (bindingSource1.Current as FileItem).filel;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bindingSource1.DataSource = list[index].Files;
            bindingSource1.ResetBindings(false);
            
        }

        private void открытьПапкуСФайломToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count != 0)
            {
                try
                {
                    var path = ((FileItemBase)dataGridView1.SelectedRows[0].DataBoundItem).FilePach;
                    if (Properties.Settings.Default.ISVIRTUALPATH)
                    {
                        path = Path.Combine(Properties.Settings.Default.VIRTUALPATH, Path.GetFileName(Path.GetDirectoryName(path)), Path.GetFileName(path));
                    }
                    ShowSelectedInExplorer.FileOrFolder(path);
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count != 0)
            {
                try
                {
                    var path = ((FileItemBase)dataGridView2.SelectedRows[0].DataBoundItem).FilePach;
                    if (Properties.Settings.Default.ISVIRTUALPATH)
                    {
                        path = Path.Combine(Properties.Settings.Default.VIRTUALPATH, Path.GetFileName(Path.GetDirectoryName(path)), Path.GetFileName(path));
                    }
                    ShowSelectedInExplorer.FileOrFolder(path);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var th = new Thread(DownloadThread) {IsBackground = true};
                th.Start(folderBrowserDialog1.SelectedPath);
            }
        }


        public void DownloadThread(object _path)
        {
            try
            {
                var path = _path.ToString();   
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                progressBarMAIN.Invoke(new Action(() =>
                    {
                        progressBarMAIN.Value = 0;
                        progressBarMAIN.Maximum = 3;
                        labelMAIN.Text = @"Загрузка FILE_STAT и FLK_ERROR";

                    }));
                var PAC = list[index];
              


                if (!string.IsNullOrEmpty(PAC.PATH_STAT))
                {
                    DOWNLOADFILE(Path.Combine(path, Path.GetFileName(PAC.PATH_STAT)), PAC.codeMOstr, 0, TypeDOWLOAD.FILE_STAT);
                }

                progressBarMAIN.Invoke(new Action(() =>
                {
                    progressBarMAIN.Value = 1;
                    progressBarMAIN.Maximum = 3;
                    labelMAIN.Text = @"Загрузка XML";

                }));

                //ОСНОВНЫЕ ФАЙЛЫ
                for (int i = 0; i < list[index].Files.Count; i++)
                {
                    DOWNLOADFILE(Path.Combine(path, Path.GetFileName(PAC.Files[i].FilePach)), PAC.codeMOstr, i, TypeDOWLOAD.File);
                }


                //ФАЙЛЫ L
                for (int i = 0; i < list[index].Files.Count; i++)
                {
                    if (list[index].Files[i].filel != null)
                        DOWNLOADFILE(Path.Combine(path, Path.GetFileName(PAC.Files[i].filel.FilePach)), PAC.codeMOstr, i, TypeDOWLOAD.File);
                }


                progressBarMAIN.Invoke(new Action(() =>
                {
                    progressBarMAIN.Value = 2;
                    progressBarMAIN.Maximum = 3;
                    labelMAIN.Text = @"Загрузка LOG";

                }));
                //ФАЙЛЫ ЛОГОВ
                for (int i = 0; i < list[index].Files.Count; i++)
                {
                    if (list[index].Files[i].filel != null)
                        if (list[index].Files[i].filel.FileLog != null)
                            DOWNLOADFILE(Path.Combine(path, Path.GetFileName(PAC.Files[i].filel.FileLog.FilePath)), PAC.codeMOstr, i, TypeDOWLOAD.FILE_L_LOG);

                    if (list[index].Files[i].filel != null)
                        if (!string.IsNullOrEmpty(PAC.Files[i].filel.PATH_LOG_XML))
                            DOWNLOADFILE(Path.Combine(path, Path.GetFileName(PAC.Files[i].filel.PATH_LOG_XML)), PAC.codeMOstr, i, TypeDOWLOAD.XML_OTCHET_L);

                    if (list[index].Files[i].FileLog != null)
                        DOWNLOADFILE(Path.Combine(path, Path.GetFileName(PAC.Files[i].FileLog.FilePath)), PAC.codeMOstr, i, TypeDOWLOAD.FILE_LOG);

                    if (!string.IsNullOrEmpty(list[index].Files[i].PATH_LOG_XML))
                        DOWNLOADFILE(Path.Combine(path, Path.GetFileName(list[index].Files[i].PATH_LOG_XML)), PAC.codeMOstr, i, TypeDOWLOAD.XML_OTCHET_H);
                }

                progressBarMAIN.Invoke(new Action(() =>
                {
                    progressBarMAIN.Value = 0;
                    progressBarMAIN.Maximum = 3;
                    labelMAIN.Text = @"Завершено";
                    if (MessageBox.Show(@"Показать файлы?", "", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        ShowSelectedInExplorer.FileOrFolder(path);
                    }

                }));
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


        }


        void DOWNLOADFILE(string FileName,string CODE_MO,int indFile,  TypeDOWLOAD type)
        {
            var st = File.Create(FileName);
            var length = MainForm.MyWcfConnection.GetFileLength(CODE_MO, indFile, type);
            progressBarMAIN.Invoke(new Action(() =>
            {
                progressBarDOWNLOAD.Value = 0;
                progressBarDOWNLOAD.Maximum = Convert.ToInt32(length);
                labelDOWNLOAD.Text = $@"Загрузка: {Path.GetFileName(FileName)}";
                

            }));

            byte[] buff = MainForm.MyWcfConnection.GetFile(CODE_MO, indFile, type, 0);
            int countREAD = 0;
           


            while (buff.Length!= 0)
            {
                countREAD += buff.Length;
                progressBarMAIN.Invoke(new Action(() =>
                {
                    progressBarDOWNLOAD.Value = countREAD;
                    progressBarDOWNLOAD.Maximum = Convert.ToInt32(length);

                }));
                st.Write(buff, 0, buff.Length);
                buff = MainForm.MyWcfConnection.GetFile(CODE_MO, indFile, type, countREAD);
          
            }
            st.Close();

            progressBarMAIN.Invoke(new Action(() =>
            {
                progressBarDOWNLOAD.Value = 0;
                progressBarDOWNLOAD.Maximum = Convert.ToInt32(length);
                labelDOWNLOAD.Text = @"Завершено";

            }));
        }
    }
}
