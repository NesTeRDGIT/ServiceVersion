using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceLoaderMedpomData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceLoaderMedpomData.EntityMP_V31;
using ClientServiceWPF.Class;

namespace ServiceLoaderMedpomDataTests
{
    [TestClass()]
    public class SchemaCheckingTests
    {
        const string H_VALID = @"D:\Project\ServiceVersion\ServiceLoaderMedpomDataTests\XMLSource\Valid\V3.1\H_VALID.XML";
        const string H_VALID_32 = @"D:\Project\ServiceVersion\ServiceLoaderMedpomDataTests\XMLSource\Valid\V3.1\H_VALID_3.2.XML";
        const string PATH_XSD_H31 = @"D:\Project\ServiceVersion\ServiceLoaderMedpomDataTests\XMLSource\Valid\SCHEMA\FROM_MO_V31\МП 3.1.xsd";
        const string PATH_XSD_H32 = @"D:\Project\ServiceVersion\ServiceLoaderMedpomDataTests\XMLSource\Valid\SCHEMA\FROM_MO_V31\МП 3.2.xsd";

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

        [TestMethod(), Description("Проверка файлов H(3.2) на схему - правильное выполнение")]
        public void CheckXML_H_VALID_32()
        {
            var sc = new SchemaChecking();

            var file = ZL_LIST.ReadFromFile(H_VALID_32);

            var res = sc.CheckXML(H_VALID_32, PATH_XSD_H32, new CheckXMLValidator(VersionMP.V3_1, false,true,GetPacientInfo(file)));
            Assert.IsTrue(res.Count == 0, $"Для правильной XML не верную схему пишет: {string.Join(";", res.Select(x => x.Comment))}");
        }


        [TestMethod(), Description("Проверка файлов H(3.2) на схему - ERR_SL_WEI_1")]
        public void CheckERR_SL_WEI_1_NOT()
        {
            var file = ZL_LIST.ReadFromFile(H_VALID_32);
            file.ZAP.Select(x => x.Z_SL).SelectMany(x => x.SL).ToList()
                .ForEach(x =>
                {
                    x.DS1 = "U07.1";
                    x.WEI = 10;
                });
            using (var ms = new MemoryStream())
            {
                file.WriteXml(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var sc = new SchemaChecking();
                var res = sc.CheckXML(ms, PATH_XSD_H32, new CheckXMLValidator(VersionMP.V3_1));
                Assert.IsTrue(res.Count(x=>x.ERR_CODE == "ERR_SL_WEI_1") == 0, $"Видит ошибку которой нет");
            }
        }
        [TestMethod(), Description("Проверка файлов H(3.2) на схему - ERR_SL_WEI_1")]
        public void CheckERR_SL_WEI_1_ERR()
        {
            var file = ZL_LIST.ReadFromFile(H_VALID_32);
            file.SCHET.YEAR = 2022;
            file.SCHET.MONTH = 12;

            file.ZAP.ForEach(z =>
            {
                z.Z_SL.USL_OK = 1;
                z.Z_SL.SL.ForEach(sl =>
                {

                    sl.DS1 = "U07.1";
                    sl.WEI = null;
                });
            });



            using (var ms = new MemoryStream())
            {
                file.WriteXml(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var sc = new SchemaChecking();
                var res = sc.CheckXML(ms, PATH_XSD_H32, new CheckXMLValidator(VersionMP.V3_1,false, true, GetPacientInfo(file)));
                Assert.IsTrue(res.Count(x => x.ERR_CODE == "ERR_SL_WEI_1") == file.ZAP.SelectMany(x=>x.Z_SL_list).Select(x=>x.SL).Count(), $"Не видит ошибку");
            }
        }


          [TestMethod(), Description("Проверка файлов H(3.2) на схему - ERR_SL_WEI_1")]
        public void CheckERR_SL_LEK_PR_1_NOT()
        {
            var file = ZL_LIST.ReadFromFile(H_VALID_32);
            file.ZAP.Select(x => x.Z_SL).SelectMany(x => x.SL).ToList()
                .ForEach(x =>
                {
                    x.DS1 = "U07.1";
                    x.LEK_PR = new List<LEK_PR_H>()
                    {
                        new LEK_PR_H()
                        {
                            CODE_SH = "1",
                            COD_MARK = "1",
                            REGNUM = "REG",
                            LEK_DOSE = new LEK_DOSE()
                            {
                                COL_INJ = 1,
                                DOSE_INJ = 1,
                                ED_IZM = "dsad",
                                METHOD_INJ = "asdasd"
                            }
                        }
                    };
                });
            using (var ms = new MemoryStream())
            {
                file.WriteXml(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var sc = new SchemaChecking();
                var res = sc.CheckXML(ms, PATH_XSD_H32, new CheckXMLValidator(VersionMP.V3_1));
                Assert.IsTrue(res.Count(x=>x.ERR_CODE == "ERR_SL_LEK_PR_1") == 0, $"Видит ошибку которой нет");
            }
        }
        [TestMethod(), Description("Проверка файлов H(3.2) на схему - ERR_SL_WEI_1")]
        public void CheckERR_SL_LEK_PR_1_ERR()
        {
            var file = ZL_LIST.ReadFromFile(H_VALID_32);
            file.SCHET.YEAR = 2022;
            file.SCHET.MONTH = 12;
            file.ZAP.ForEach(z =>
            {
                z.Z_SL.USL_OK = 1;
                z.Z_SL.SL.ForEach(sl =>
                {

                    sl.DS1 = "U07.1";
                    sl.LEK_PR = null;
                });
            });
            using (var ms = new MemoryStream())
            {
                file.WriteXml(ms);
                ms.Seek(0, SeekOrigin.Begin);
                var sc = new SchemaChecking();
                var res = sc.CheckXML(ms, PATH_XSD_H32, new CheckXMLValidator(VersionMP.V3_1, false,true, GetPacientInfo(file)));
               
                Assert.IsTrue(res.Count(x => x.ERR_CODE == "ERR_SL_LEK_PR_1") == file.ZAP.SelectMany(x=>x.Z_SL_list).Select(x=>x.SL).Count(), $"Не видит ошибку");
            }
        }

        private Dictionary<string,PacientInfo> GetPacientInfo(ZL_LIST file)
        {
            Dictionary<string, PacientInfo> dic = new Dictionary<string, PacientInfo>();
            foreach(var zap in file.ZAP)
            {
                if (!dic.ContainsKey(zap.PACIENT.ID_PAC))
                    dic.Add(zap.PACIENT.ID_PAC, new PacientInfo { DR = new DateTime(1991, 1, 1) });
            }
            return dic;
        }


        const string C_VALID = @"D:\Project\ServiceVersion\ServiceLoaderMedpomDataTests\XMLSource\Valid\V3.1\C_VALID.XML";
        const string PATH_XSD_C31 = @"D:\Project\ServiceVersion\ServiceLoaderMedpomDataTests\XMLSource\Valid\SCHEMA\FROM_MO_V31\ЗНО 3.1.xsd";

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
        const string D_VALID = @"D:\Project\ServiceVersion\ServiceLoaderMedpomDataTests\XMLSource\Valid\V3.1\D_VALID.XML";
        const string PATH_XSD_D31 = @"D:\Project\ServiceVersion\ServiceLoaderMedpomDataTests\XMLSource\Valid\SCHEMA\FROM_MO_V31\ДИСП 3.1_2 c 01.04.2020.xsd";

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

        const string T_VALID = @"D:\Project\ServiceVersion\ServiceLoaderMedpomDataTests\XMLSource\Valid\V3.1\T_VALID.XML";
        const string PATH_XSD_T31 = @"D:\Project\ServiceVersion\ServiceLoaderMedpomDataTests\XMLSource\Valid\SCHEMA\FROM_MO_V31\ВМП 3.1.xsd";
       
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

        private string C_EMPTY_ELEMENT = @"D:\Project\ServiceVersion\ServiceLoaderMedpomDataTests\XMLSource\Valid\V3.1\C_EMPTY_ELEMENT.XML";
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
    
     
        const string D_31_WITH_08_2021_VALID = @"D:\Project\ServiceVersion\ServiceLoaderMedpomDataTests\XMLSource\Valid\V3.1\D_VALID_08_2021.XML";
        const string PATH_XSD_D31_WITH_08_2021 = @"D:\Project\ServiceVersion\ServiceLoaderMedpomDataTests\XMLSource\Valid\SCHEMA\FROM_MO_V31\ДИСП 3.1 c 01.08.2021.xsd";
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

        public class SeparatorComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                var result = 0;
                var x_arr = x.Split('.').Where(z => !string.IsNullOrEmpty(z)).ToArray();
                var y_arr = y.Split('.').Where(z => !string.IsNullOrEmpty(z)).ToArray(); ;

                for (var i = 0; i < x_arr.Length; i++)
                {
                    if (i >= y_arr.Length) break;
                    var a = Convert.ToInt32(x_arr[i]);
                    var b = Convert.ToInt32(y_arr[i]);
                    result = a.CompareTo(b);
                    if (result != 0)
                        return result;
                }
                return x.Length.CompareTo(y.Length);
            }


        }

        [TestMethod(), Description("Проверка файла из каталога")]
        public void TestCustomFile()
        {
            var item = new ZL_LIST();
            string p = "C:\\1\\1.xml";
            using (var st = File.Create(p))
            {
                item.WriteXmlCustom(st);
            }
            ExtZLLIST.ChangeNamespace(p);
        }

    }
}