using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using ServiceLoaderMedpomData;
using ServiceLoaderMedpomData.EntityMP_V3;

namespace ClientServise.SANK_SMO
{
    public partial class TEST : Form
    {
        public TEST()
        {
            InitializeComponent();
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
           

        }


       
        static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            switch (e.Severity)
            {
                case XmlSeverityType.Error:
                    Console.WriteLine("Error: {0}", e.Message);
                    break;
                case XmlSeverityType.Warning:
                    Console.WriteLine("Warning {0}", e.Message);
                    break;
            }

        }



    }
}
