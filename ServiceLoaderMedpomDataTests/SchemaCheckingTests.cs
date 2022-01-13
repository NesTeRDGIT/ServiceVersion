using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceLoaderMedpomData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceLoaderMedpomData.EntityMP_V31;

namespace ServiceLoaderMedpomDataTests
{
    [TestClass()]
    public class SchemaCheckingTests
    {
        const string H_VALID = @"E:\XML Project\ServiceVersion\ServiceLoaderMedpomDataTests\XMLSource\Valid\V3.1\H_VALID.XML";
        const string PATH_XSD_H31 = @"E:\XML Project\ServiceVersion\ServiceLoaderMedpomDataTests\XMLSource\Valid\SCHEMA\FROM_MO_V31\МП 3.1.xsd";

        /// <summary>
        /// Тестирование проверки схемы файлов H
        /// </summary>
        [TestMethod(), Description("Проверка файлов H на схему - правильное выполнение")]
        public void CheckXML_H_VALID_31()
        {
            var sc = new SchemaChecking();
            var res = sc.CheckXML(H_VALID, PATH_XSD_H31, new CheckXMLValidator(VersionMP.V3_1));
            Assert.IsTrue(res.Count==0, $"Для правильной XML не верную схему пишет: {string.Join(";",res.Select(x=>x.Comment))}");
        }


        const string C_VALID = @"E:\XML Project\ServiceVersion\ServiceLoaderMedpomDataTests\XMLSource\Valid\V3.1\C_VALID.XML";
        const string PATH_XSD_C31 = @"E:\XML Project\ServiceVersion\ServiceLoaderMedpomDataTests\XMLSource\Valid\SCHEMA\FROM_MO_V31\ЗНО 3.1.xsd";

        /// <summary>
        /// Тестирование проверки схемы файлов H
        /// </summary>
        [TestMethod(), Description("Проверка файлов C на схему - правильное выполнение")]
        public void CheckXML_C_VALID_31()
        {
            var sc = new SchemaChecking();
            var res = sc.CheckXML(C_VALID, PATH_XSD_C31, new CheckXMLValidator(VersionMP.V3_1));
            Assert.IsTrue(res.Count == 0, $"Для правильной XML не верную схему пишет: {string.Join(";", res.Select(x => x.Comment))}");
        }
        const string D_VALID = @"E:\XML Project\ServiceVersion\ServiceLoaderMedpomDataTests\XMLSource\Valid\V3.1\D_VALID.XML";
        const string PATH_XSD_D31 = @"E:\XML Project\ServiceVersion\ServiceLoaderMedpomDataTests\XMLSource\Valid\SCHEMA\FROM_MO_V31\ДИСП 3.1_2 c 01.04.2020.xsd";

        /// <summary>
        /// Тестирование проверки схемы файлов H
        /// </summary>
        [TestMethod(), Description("Проверка файлов D на схему - правильное выполнение")]
        public void CheckXML_D_VALID_31()
        {
            var sc = new SchemaChecking();
            var res = sc.CheckXML(D_VALID, PATH_XSD_D31, new CheckXMLValidator(VersionMP.V3_1));
            Assert.IsTrue(res.Count == 0, $"Для правильной XML не верную схему пишет: {string.Join(";", res.Select(x => x.Comment))}");
        }

        const string T_VALID = @"E:\XML Project\ServiceVersion\ServiceLoaderMedpomDataTests\XMLSource\Valid\V3.1\T_VALID.XML";
        const string PATH_XSD_T31 = @"E:\XML Project\ServiceVersion\ServiceLoaderMedpomDataTests\XMLSource\Valid\SCHEMA\FROM_MO_V31\ВМП 3.1.xsd";
       
        [TestMethod(), Description("Проверка файлов T на схему - правильное выполнение")]
        public void CheckXML_T_VALID_31()
        {
            var sc = new SchemaChecking();
            var res = sc.CheckXML(T_VALID, PATH_XSD_T31, new CheckXMLValidator(VersionMP.V3_1));
            Assert.IsTrue(res.Count == 0, $"Для правильной XML не верную схему пишет: {string.Join(";", res.Select(x => x.Comment))}");
        }
        [TestMethod(), Description("Проверка файлов H на схему - правильное выполнение")]
        public void CheckXML_T_VALID_31_AND_KSLP()
        {
            var file = ZL_LIST.ReadFromFile(H_VALID);
            file.ZAP.Select(x=>x.Z_SL).SelectMany(x=>x.SL).ToList()
                .ForEach(x=>x.KSG_KPG= new KSG_KPG() {N_KSG = "KSG", SL_KOEF = new List<SL_KOEF> { new SL_KOEF()}, CRIT = new List<string> {"CRIT"} });
         
            using (var ms = new MemoryStream())
            {
                file.WriteXml(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var sc = new SchemaChecking();
                var res = sc.CheckXML(ms, PATH_XSD_H31, new CheckXMLValidator(VersionMP.V3_1));
                Assert.IsTrue(res.Count == 0, $"Видит ошибку которой нет");
            }

          
        }

        private string C_EMPTY_ELEMENT = @"E:\XML Project\ServiceVersion\ServiceLoaderMedpomDataTests\XMLSource\Valid\V3.1\C_EMPTY_ELEMENT.XML";
        [TestMethod(), Description("Проверка файлов C на схему - пустой элемент <SMO/>")]
        public void CheckXML_C_EMPTY_ELEMENT()
        {
            var file = ZL_LIST.ReadFromFile(C_EMPTY_ELEMENT);
          
            var t = GetXML(file);
            using (var ms = new MemoryStream())
            {
                file.WriteXml(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var sc = new SchemaChecking();
                var res = sc.CheckXML(ms, PATH_XSD_H31, new CheckXMLValidator(VersionMP.V3_1));
                Assert.IsTrue(res.Count == 1, $"Видит ошибку которой нет");
            }


        }

        [TestMethod(), Description("Проверка файлов на схему - ошибка суммы")]
        public void CheckXML_H_31_ERR_SUMMAV_NOT_EQ_SUMV()
        {
            var file = ZL_LIST.ReadFromFile(H_VALID);
            file.SCHET.SUMMAV = 0;
            using (var ms = new MemoryStream())
            {
                file.WriteXml(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var sc = new SchemaChecking();
                var res = sc.CheckXML(ms, PATH_XSD_H31, new CheckXMLValidator(VersionMP.V3_1));
                Assert.IsTrue(res.Count == 1, $"Ошибка Сумма случаев в реестре однако ошибку не видит");
            }
        }


        [TestMethod(), Description("Проверка файлов на схему - ошибка C_ZAB_EMPTY")]
        public void CheckXML_H_31_ERR_C_ZAB_EMPTY()
        {
            var file = ZL_LIST.ReadFromFile(H_VALID);
            var zSl = file.ZAP.Select(x => x.Z_SL).ToList();
            
            foreach (var z in zSl)
            {
                z.USL_OK = 3;
                foreach (var sl in z.SL)
                {
                    sl.DS1 = "U07.1";
                    sl.C_ZAB = null;
                }
            }

            using (var ms = new MemoryStream())
            {
                file.WriteXml(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var sc = new SchemaChecking();
                var res = sc.CheckXML(ms, PATH_XSD_H31, new CheckXMLValidator(VersionMP.V3_1));
                Assert.IsTrue(res.Count(x=>x.ERR_CODE == "C_ZAB_EMPTY") == zSl.SelectMany(x=>x.SL).Count(), $"Ошибку не видит");
            }
        }


       


        [TestMethod(), Description("Проверка файлов на схему - ошибка SD_Z")]
        public void CheckXML_H_31_ERR_SD_Z()
        {
            var file = ZL_LIST.ReadFromFile(H_VALID);
            file.ZGLV.SD_Z = 0;

            using (var ms = new MemoryStream())
            {
                file.WriteXml(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var sc = new SchemaChecking();
                var res = sc.CheckXML(ms, PATH_XSD_H31, new CheckXMLValidator(VersionMP.V3_1));
                Assert.IsTrue(res.Count == 1, $"Ошибку не видит");
            }
        }

        [TestMethod(), Description("Проверка файлов на схему - ошибка SUMV!=SUM(SUM_M)")]
        public void CheckXML_H_31_ERR_SUMV_NOT_EQ_SUM_M()
        {
            var file = ZL_LIST.ReadFromFile(H_VALID);
            file.ZAP.SelectMany(x => x.Z_SL_list).SelectMany(x => x.SL).ToList().ForEach(x => x.SUM_M = 0); ;
            using (var ms = new MemoryStream())
            {
                file.WriteXml(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var sc = new SchemaChecking();
                var res = sc.CheckXML(ms, PATH_XSD_H31, new CheckXMLValidator(VersionMP.V3_1));
                Assert.IsTrue(res.Count == 1, $"Ошибку не видит");
            }
        }

        [TestMethod(), Description("Проверка файлов на схему - ошибка SUMV!=SUM(SUM_USL)")]
        public void CheckXML_H_31_ERR_SUM_M_NOT_EQ_SUM_USL()
        {
            var file = ZL_LIST.ReadFromFile(H_VALID);
            file.ZAP.SelectMany(x=>x.Z_SL_list).SelectMany(x=>x.SL).SelectMany(x=>x.USL).ToList().ForEach(x => x.SUMV_USL = 0); ;
            using (var ms = new MemoryStream())
            {
                file.WriteXml(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var sc = new SchemaChecking();
                var res = sc.CheckXML(ms, PATH_XSD_H31, new CheckXMLValidator(VersionMP.V3_1));
                Assert.IsTrue(res.Count == 1, $"Ошибку не видит");
            }
        }


        [TestMethod(), Description("Поле FIRST_IDCASE не подлежит заполнению без указания тэга SCHET\\REF")]
        public void CheckXML_H_31_ERR_FIRST_IDCASE_NOT_REF()
        {
            var file = ZL_LIST.ReadFromFile(H_VALID);
            file.ZAP.SelectMany(x => x.Z_SL_list).ToList().ForEach(x=>x.FIRST_IDCASE = 0);
            using (var ms = new MemoryStream())
            {
                file.WriteXml(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var sc = new SchemaChecking();
                var res = sc.CheckXML(ms, PATH_XSD_H31, new CheckXMLValidator(VersionMP.V3_1)).Where(x => x.Comment == "Поле FIRST_IDCASE не подлежит заполнению без указания тэга SCHET\\REF").ToList();
                Assert.IsTrue(res.Count !=0, $"Ошибку не видит");
            }

        }

        [TestMethod(), Description("Поле FIRST_IDCASE обязательно к заполнению при указании тэга SCHET\\REF")]
        public void CheckXML_H_31_ERR_FIRST_IDCASE_REF()
        {
            var file = ZL_LIST.ReadFromFile(H_VALID);
            file.SCHET.REF = new REF();
            using (var ms = new MemoryStream())
            {
                file.WriteXml(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var sc = new SchemaChecking();
                var res = sc.CheckXML(ms, PATH_XSD_H31, new CheckXMLValidator(VersionMP.V3_1)).Where(x => x.Comment == "Поле FIRST_IDCASE обязательно к заполнению при указании тэга SCHET\\REF").ToList();
                Assert.IsTrue(res.Count != 0, $"Ошибку не видит");
            }


        }

        [TestMethod(), Description("Признак исправленной записи = 1 недопустим без указания тэга SCHET\\REF")]
        public void CheckXML_H_31_ERR_PR_NOV_NOT_REF()
        {
            var file = ZL_LIST.ReadFromFile(H_VALID);
            file.ZAP.ForEach(x=>x.PR_NOV = 1);
            using (var ms = new MemoryStream())
            {
                file.WriteXml(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var sc = new SchemaChecking();
                var res = sc.CheckXML(ms, PATH_XSD_H31, new CheckXMLValidator(VersionMP.V3_1)).Where(x=>x.Comment == "Признак исправленной записи = 1 недопустим без указания тэга SCHET\\REF").ToList();
                Assert.IsTrue(res.Count != 0, $"Ошибку не видит");
            }



        }
        [TestMethod(), Description("Признак исправленной записи = 0 недопустим при указании тэга SCHET\\REF")]
        public void CheckXML_H_31_ERR_PR_NOV_REF()
        {
            
            var file = ZL_LIST.ReadFromFile(H_VALID);
            file.SCHET.REF = new REF();
            using (var ms = new MemoryStream())
            {
                file.WriteXml(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var sc = new SchemaChecking();
                var res = sc.CheckXML(ms, PATH_XSD_H31, new CheckXMLValidator(VersionMP.V3_1)).Where(x => x.Comment == "Признак исправленной записи = 0 недопустим при указании тэга SCHET\\REF").ToList();
                Assert.IsTrue(res.Count != 0, $"Ошибку не видит");
            }

        }


        [TestMethod(), Description("Проверка файлов на схему - REF правильная XML")]
        public void CheckXML_H_31_ERR_FIRST_IDCASE_WITH_REF()
        {
            var file = ZL_LIST.ReadFromFile(H_VALID);
            file.SCHET.REF = new REF();
            file.ZAP.SelectMany(x => x.Z_SL_list).ToList().ForEach(x => x.FIRST_IDCASE = 0);
            file.ZAP.ForEach(x=>x.PR_NOV = 1);
            using (var ms = new MemoryStream())
            {
                file.WriteXml(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var sc = new SchemaChecking();
                var res = sc.CheckXML(ms, PATH_XSD_H31, new CheckXMLValidator(VersionMP.V3_1));
                Assert.IsTrue(res.Count == 0, $"Правильное заполнение");
            }
        }

        private string GetXML(ZL_LIST file)
        {
            using (var ms = new MemoryStream())
            {
                file.WriteXml(ms);
                ms.Seek(0, SeekOrigin.Begin);
                using (var sr = new StreamReader(ms))
                {

                    return sr.ReadToEnd();
                }
            }
        }
    
     
        const string D_31_WITH_08_2021_VALID = @"E:\XML Project\ServiceVersion\ServiceLoaderMedpomDataTests\XMLSource\Valid\V3.1\D_VALID_08_2021.XML";
        const string PATH_XSD_D31_WITH_08_2021 = @"E:\XML Project\ServiceVersion\ServiceLoaderMedpomDataTests\XMLSource\Valid\SCHEMA\FROM_MO_V31\ДИСП 3.1 c 01.08.2021.xsd";
        [TestMethod(), Description("Проверка файлов на схему D c 01.08.2021 правильная XML")]
        public void CheckXML_D_31_WITH_08_2021()
        {
            var sc = new SchemaChecking();
            var res = sc.CheckXML(D_31_WITH_08_2021_VALID, PATH_XSD_D31_WITH_08_2021, new CheckXMLValidator(VersionMP.V3_1));
            Assert.IsTrue(res.Count == 0, $"Для правильной XML не верную схему пишет: {string.Join(Environment.NewLine, res.Select(x => x.MessageOUT))}");
           
        }

        [TestMethod(), Description("Проверка ошибок ERR_PAC_1-ERR_PAC_3")]
        public void ERR_PAC_1_3()
        {
            var file = ZL_LIST.ReadFromFile(D_31_WITH_08_2021_VALID);
            var p = file.ZAP.Select(x => x.PACIENT).First();
            p.VPOLIS = 3;
            p.ENP = null;
            p.SPOLIS = "123";
            p.NPOLIS = "asdasd";
            using (var ms = new MemoryStream())
            {
                file.WriteXml(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var sc = new SchemaChecking();
                var res = sc.CheckXML(ms, PATH_XSD_D31_WITH_08_2021, new CheckXMLValidator(VersionMP.V3_1));
                Assert.IsTrue(res.Count(x=>x.ERR_CODE == "ERR_PAC_1") == 1 && res.Count(x => x.ERR_CODE == "ERR_PAC_2") == 1 && res.Count(x => x.ERR_CODE == "ERR_PAC_3") == 1 && res.Count==3, $"Не видит ошибок");
            }
        }

        [TestMethod(), Description("Проверка ошибок ERR_PAC_4-ERR_PAC_5")]
        public void ERR_PAC_4_5()
        {
            var file = ZL_LIST.ReadFromFile(D_31_WITH_08_2021_VALID);
            var p = file.ZAP.Select(x => x.PACIENT).First();
            p.VPOLIS = 2;
            p.ENP = null;
            p.SPOLIS = "123";
            p.NPOLIS = null;
            using (var ms = new MemoryStream())
            {
                file.WriteXml(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var sc = new SchemaChecking();
                var res = sc.CheckXML(ms, PATH_XSD_D31_WITH_08_2021, new CheckXMLValidator(VersionMP.V3_1));
                Assert.IsTrue(res.Count(x => x.ERR_CODE == "ERR_PAC_4") == 1 && res.Count(x => x.ERR_CODE == "ERR_PAC_5") == 1 && res.Count == 2, $"Не видит ошибок");
            }
        }

        [TestMethod(), Description("Проверка ошибок ERR_PAC_6-ERR_PAC_8")]
        public void ERR_PAC_6_8()
        {
            var file = ZL_LIST.ReadFromFile(D_31_WITH_08_2021_VALID);
            var p = file.ZAP.Select(x => x.PACIENT).First();
            p.VPOLIS = 1;
            p.ENP = "75";
            p.SPOLIS = null;
            p.NPOLIS = null;
            using (var ms = new MemoryStream())
            {
                file.WriteXml(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var sc = new SchemaChecking();
                var res = sc.CheckXML(ms, PATH_XSD_D31_WITH_08_2021, new CheckXMLValidator(VersionMP.V3_1));
                Assert.IsTrue(res.Count(x => x.ERR_CODE == "ERR_PAC_6") == 1 && res.Count(x => x.ERR_CODE == "ERR_PAC_7") == 1 && res.Count(x => x.ERR_CODE == "ERR_PAC_8") == 1 && res.Count == 3, $"Не видит ошибок");
            }
        }
        [TestMethod(), Description("Проверка ошибок ERR_MR_USL_N_1-ERR_MR_USL_N_4")]
        public void ERR_MR_USL_N_1_4()
        {
            var file = ZL_LIST.ReadFromFile(D_31_WITH_08_2021_VALID);
            var us = file.ZAP.SelectMany(x=>x.Z_SL_list).SelectMany(x=>x.SL).SelectMany(x=>x.USL).ToList();
            us[0].P_OTK = 0;
            us[0].MR_USL_N[0].PRVS = null;
            us[0].MR_USL_N[0].CODE_MD = null;

            us[1].P_OTK = 1;
            us[1].MR_USL_N[0].PRVS = 1;
            us[1].MR_USL_N[0].CODE_MD = "75";

            using (var ms = new MemoryStream())
            {
                file.WriteXml(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var sc = new SchemaChecking();
                var res = sc.CheckXML(ms, PATH_XSD_D31_WITH_08_2021, new CheckXMLValidator(VersionMP.V3_1));
                Assert.IsTrue(res.Count(x => x.ERR_CODE == "ERR_MR_USL_N_1") == 1 && res.Count(x => x.ERR_CODE == "ERR_MR_USL_N_2") == 1 && res.Count(x => x.ERR_CODE == "ERR_MR_USL_N_3") == 1 && res.Count(x => x.ERR_CODE == "ERR_MR_USL_N_4") == 1 && res.Count == 4, $"Не видит ошибок");
            }
        }


        [TestMethod(), Description("Проверка дублирования MR_N")]
        public void ERR_DOUBLE_MR_N()
        {
            var file = ZL_LIST.ReadFromFile(D_31_WITH_08_2021_VALID);
             file.ZAP.SelectMany(x => x.Z_SL_list).SelectMany(x => x.SL).SelectMany(x => x.USL).SelectMany(x=>x.MR_USL_N).ToList().ForEach(x=>
            {
                x.MR_N = 1;
            });

            using (var ms = new MemoryStream())
            {
                file.WriteXml(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var sc = new SchemaChecking();
                var res = sc.CheckXML(ms, PATH_XSD_D31_WITH_08_2021, new CheckXMLValidator(VersionMP.V3_1));
                Assert.IsTrue(res.Count() != 0, $"Не видит ошибок");
            }
        }



        [TestMethod(), Description("Проверка файла из каталога")]
        public void TestCustomFile()
        {
            /*var file = ZL_LIST.ReadFromFile(@"C:\TEMP\HM750004T75_211262.XML");
            file.SCHET.MONTH = 9;
            using (var ms = new MemoryStream())
            {
                file.WriteXml(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var sc = new SchemaChecking();
                var res = sc.CheckXML(ms, PATH_XSD_H31, new CheckXMLValidator(VersionMP.V3_1));
                Assert.IsTrue(res.Count == 1, $"Ошибку не видит");
            }*/

        }

    }
}