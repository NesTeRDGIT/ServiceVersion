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


    }
}