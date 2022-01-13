using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServiceLoaderMedpomData;
using MedpomService;
using System.Windows.Controls;
using Moq;
using ServiceLoaderMedpomData.EntityMP_V31;


namespace ServiceLoaderMedpomDataTests
{
    [TestClass()]
    public class FileInviterTest
    {
        [TestMethod, Description("Проверка файлов H на схему - правильное выполнение")]
        public void CheckErrorArchiveName()
        {
            var lis = new ZL_LIST();
            lis.ZAP.Add(new ZAP(){Z_SL = new Z_SL()});
            var t = lis.ZAP.Sum(x => null);
        }


       
    }


 
}
