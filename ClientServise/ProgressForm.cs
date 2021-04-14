using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ServiceLoaderMedpomData;
using System.Threading;

namespace ClientServise
{
    public partial class ProgressForm : Form
    {
        public ProgressForm()
        {
            InitializeComponent();
        }

        IWcfInterface wcf
        {
            get
            {
                return MainForm.MyWcfConnection;
            }
        }
        private void ProgressForm_Load(object sender, EventArgs e)
        {
            Thread th = new Thread(new ThreadStart(ProgressArch));
            th.Start();
        }

        public void ProgressArch()
        {
            try
            {
                ProgressClass t = new ProgressClass() { Active = true };
                while (t.Active)
                {
                    t = wcf.GetProgressClassProcessArch();
                    if (t != null)
                    {
                        progressBar1.Invoke(new Action(() =>
                        {
                            progressBar1.Maximum = t.Max;
                            progressBar1.Value = t.Value;
                            if (t.Active)
                                label1.Text = "Активно: " + t.TXT;
                            else
                                label1.Text = "Не активно: " + t.TXT;
                        }));
                    }
                    Thread.Sleep(200);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
