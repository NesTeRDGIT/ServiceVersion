using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelManager;
using ServiceLoaderMedpomData;

namespace MedpomService
{
    public interface IExcelProtokol
    {
        void ExportExcel2(DataTable TBL, string path);
        void CreateExcelSvod2(FilePacket pack, string path, DataTable SVOD, DataTable STAT_VID_MP, DataTable STAT_FULL);

    }
    public class ExcelProtokol: IExcelProtokol
    {
        class ExportExcelRow
        {
            public static ExportExcelRow Get(DataRow row)
            {
                try
                {
                    var item = new ExportExcelRow();
                    if (row["SLUCH_ID"] != DBNull.Value)
                        item.SLUCH_ID = Convert.ToDecimal(row["SLUCH_ID"]);
                    if (row["CODE_MO"] != DBNull.Value)
                        item.CODE_MO = Convert.ToString(row["CODE_MO"]);
                    if (row["FAM"] != DBNull.Value)
                        item.FAM = Convert.ToString(row["FAM"]);
                    if (row["IM"] != DBNull.Value)
                        item.IM = Convert.ToString(row["IM"]);
                    if (row["OT"] != DBNull.Value)
                        item.OT = Convert.ToString(row["OT"]);
                    if (row["DR"] != DBNull.Value)
                        item.DR = Convert.ToString(row["DR"]);
                    if (row["ID_PAC"] != DBNull.Value)
                        item.ID_PAC = Convert.ToString(row["ID_PAC"]);
                    if (row["VPOLIS"] != DBNull.Value)
                        item.VPOLIS = Convert.ToString(row["VPOLIS"]);
                    if (row["SPOLIS"] != DBNull.Value)
                        item.SPOLIS = Convert.ToString(row["SPOLIS"]);
                    if (row["NPOLIS"] != DBNull.Value)
                        item.NPOLIS = Convert.ToString(row["NPOLIS"]);
                    if (row["NHISTORY"] != DBNull.Value)
                        item.NHISTORY = Convert.ToString(row["NHISTORY"]);
                    if (row["USL_OK"] != DBNull.Value)
                        item.USL_OK = Convert.ToString(row["USL_OK"]);
                    if (row["LPU_1"] != DBNull.Value)
                        item.LPU_1 = Convert.ToString(row["LPU_1"]);
                    if (row["IDDOKT"] != DBNull.Value)
                        item.IDDOKT = Convert.ToString(row["IDDOKT"]);
                    if (row["DATE_1"] != DBNull.Value)
                        item.DATE_1 = Convert.ToDateTime(row["DATE_1"]);
                    if (row["DATE_2"] != DBNull.Value)
                        item.DATE_2 = Convert.ToDateTime(row["DATE_2"]);
                    if (row["ERR"] != DBNull.Value)
                        item.ERR = Convert.ToString(row["ERR"]);
                    if (row["ERR_TYPE"] != DBNull.Value)
                        item.ERR_TYPE = Convert.ToString(row["ERR_TYPE"]);

                    return item;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка при получение ExportExcelRow: {ex.Message}", ex);
                }
            }

            public decimal SLUCH_ID { get; set; } = 0;
            public string CODE_MO { get; set; }
            public string FAM { get; set; } = "";
            public string IM { get; set; } = "";
            public string OT { get; set; } = "";
            public string DR { get; set; }
            public string ID_PAC { get; set; } = "";
            public string VPOLIS { get; set; } = "";
            public string SPOLIS { get; set; } = "";
            public string NPOLIS { get; set; } = "";
            public string NHISTORY { get; set; } = "";
            public string USL_OK { get; set; } = "";
            public string LPU_1 { get; set; } = "";
            public string IDDOKT { get; set; } = "";
            public DateTime? DATE_1 { get; set; }
            public DateTime? DATE_2 { get; set; }
            public string ERR { get; set; } = "";
            public string ERR_TYPE { get; set; } = "0";

        }
        class ItemSvod
        {
            public int? NUM { get; set; }
            public FileItemBase File { get; set; }

            public string FileName => File == null ? "" : File.FileName;

            public string Result
            {
                get
                {
                    switch (File.Process)
                    {
                        case StepsProcess.NotInvite:
                            return "Файл не принят! (Файл не поступил на обработку) подробности в " + File.FileLog?.FileName;
                        case StepsProcess.ErrorXMLxsd:
                            return "Файл не принят! (Ошибка при проверке схемы документа) подробности в " + File.FileLog?.FileName;
                        case StepsProcess.FlkOk:
                            return $"Файл принят подробности в {File.FileLog?.FileName}";
                        case StepsProcess.Invite:
                            return
                                $"Файл не принят! Файл поступил в обработку (ФЛК не выполнялось) подробности в {File.FileLog?.FileName}";
                        case StepsProcess.XMLxsd:
                            return
                                $"Файл не принят! (Проверка на схему выполнена, ФЛК не выполнялось) подробности в {File.FileLog?.FileName}";
                        case StepsProcess.FlkErr:
                            return
                                $"Файл не принят! (Ошибка при выполнении ФЛК) подробности в {File.FileLog?.FileName}";
                        default:
                            return $"Не определенность подробности в {File.FileLog?.FileName}";
                    }
                }
            }

            public bool IsError => File.Process != StepsProcess.FlkOk;

            public decimal? SUM { get; set; }
            public decimal? SUM_MEK { get; set; }
            public int? CSLUCH { get; set; }
            public int? CUSL { get; set; }
            public DateTime DATE { get; set; } = DateTime.Now;
            public string REL => File.FileLog?.FileName;
            public string Comment { get; set; }
        }
        public void ExportExcel2(DataTable TBL, string path)
        {
            ExcelOpenXML efm = null;
            try
            {
                if (File.Exists(path))
                {
                    efm = new ExcelOpenXML();
                    efm.OpenFile(path, 0);
                    if (efm.CurrentSheetName != "Ошибки ФЛК")
                    {
                        efm.AddSheet("Ошибки ФЛК");
                    }
                }
                else
                {
                    efm = new ExcelOpenXML(path, "Ошибки ФЛК");
                }

                var styleCellDefault = efm.CreateType(new FontOpenXML { HorizontalAlignment = HorizontalAlignmentV.Center, wordwrap = true }, new BorderOpenXML(), new FillOpenXML());
                var styleCellYellow = efm.CreateType(new FontOpenXML { HorizontalAlignment = HorizontalAlignmentV.Left, wordwrap = true }, new BorderOpenXML(), new FillOpenXML { color = System.Windows.Media.Colors.Yellow });
                var styleCellRed = efm.CreateType(new FontOpenXML { HorizontalAlignment = HorizontalAlignmentV.Left, wordwrap = true }, new BorderOpenXML(), new FillOpenXML { color = System.Windows.Media.Colors.LightSalmon });
                var styleCellDefaultDate = efm.CreateType(new FontOpenXML { HorizontalAlignment = HorizontalAlignmentV.Center, wordwrap = true, Format = Convert.ToUInt32(DefaultNumFormat.F14) }, new BorderOpenXML(), new FillOpenXML());




                var styleColumn = efm.CreateType(new FontOpenXML { HorizontalAlignment = HorizontalAlignmentV.Center, wordwrap = true, Bold = true }, new BorderOpenXML(), new FillOpenXML());

                var r = efm.GetRow(1);
                r.Height = 48;
                efm.PrintCell(r, 1, "Внутренний номер ТФОМС", styleColumn);
                efm.PrintCell(r, 2, "Код МО", styleColumn);
                efm.PrintCell(r, 3, "Фамилия пациента/представителя", styleColumn);
                efm.PrintCell(r, 4, "Имя пациента/представителя", styleColumn);
                efm.PrintCell(r, 5, "Отчество пациента/представителя", styleColumn);
                efm.PrintCell(r, 6, "ДР пациента/представителя", styleColumn);
                efm.PrintCell(r, 7, "Код пациента", styleColumn);
                efm.PrintCell(r, 8, "Тип полиса", styleColumn);
                efm.PrintCell(r, 9, "Серия полиса", styleColumn);
                efm.PrintCell(r, 10, "Номер полиса", styleColumn);
                efm.PrintCell(r, 11, "Подразделение МО", styleColumn);
                efm.PrintCell(r, 12, "№ истории болезни/ амбулаторной карты", styleColumn);
                efm.PrintCell(r, 13, "Условие оказания МП", styleColumn);
                efm.PrintCell(r, 14, "Код врача, закрывшего случай", styleColumn);
                efm.PrintCell(r, 15, "Дата начала", styleColumn);
                efm.PrintCell(r, 16, "Дата окончания", styleColumn);
                efm.PrintCell(r, 17, "Ошибка", styleColumn);

                efm.SetColumnWidth(1, 11);
                efm.SetColumnWidth(2, 18);
                efm.SetColumnWidth(3, 18);
                efm.SetColumnWidth(4, 18);
                efm.SetColumnWidth(5, 18);
                efm.SetColumnWidth(6, 18);
                efm.SetColumnWidth(7, 17);
                efm.SetColumnWidth(8, 9);
                efm.SetColumnWidth(9, 9);
                efm.SetColumnWidth(10, 19);
                efm.SetColumnWidth(11, 16);
                efm.SetColumnWidth(12, 17);
                efm.SetColumnWidth(13, 9);
                efm.SetColumnWidth(14, 18);
                efm.SetColumnWidth(15, 12);
                efm.SetColumnWidth(16, 12);
                efm.SetColumnWidth(17, 64);



                uint RowIndexEx = 2;

                if (TBL.Rows.Count == 0)
                {
                    r = efm.GetRow(RowIndexEx, true);
                    efm.PrintCell(r, 1, "Ошибки отсутствуют", styleCellDefault);
                    efm.Save();
                    return;
                }
                var ERR_LIST = new Dictionary<string, List<ErrorProtocolXML>>();

                for (var i = 0; i < TBL.Rows.Count; i++)
                {
                    #region Формирование файлов FLK
                    var fi = TBL.Rows[i]["FILENAME"].ToString();
                    var err = new ErrorProtocolXML
                    {
                        BAS_EL = TBL.Rows[i]["BAS_EL"].ToString(),
                        IDCASE = TBL.Rows[i]["IDCASE"].ToString(),
                        ID_SERV = TBL.Rows[i]["ID_SERV"].ToString(),
                        SL_ID = TBL.Rows[i]["SL_ID"].ToString(),
                        N_ZAP = TBL.Rows[i]["N_ZAP"].ToString(),
                        OSHIB = Convert.ToInt32(TBL.Rows[i]["OSHIB"]),
                        Comment = TBL.Rows[i]["ERR"].ToString()
                    };
                    if (!ERR_LIST.ContainsKey(fi))
                    {
                        ERR_LIST.Add(fi, new List<ErrorProtocolXML>());
                    }
                    ERR_LIST[fi].Add(err);
                    #endregion
                    var row = ExportExcelRow.Get(TBL.Rows[i]);
                    r = efm.GetRow(RowIndexEx, true);

                    uint tERR;

                    switch (row.ERR_TYPE)
                    {
                        case "1": tERR = styleCellYellow; break;
                        case "2": tERR = styleCellRed; break;
                        default:
                            tERR = styleCellDefault;
                            break;
                    }

                    efm.PrintCell(r, 1, row.SLUCH_ID, styleCellDefault);
                    efm.PrintCell(r, 2, row.CODE_MO, styleCellDefault);
                    efm.PrintCell(r, 3, row.FAM, styleCellDefault);
                    efm.PrintCell(r, 4, row.IM, styleCellDefault);
                    efm.PrintCell(r, 5, row.OT, styleCellDefault);
                    efm.PrintCell(r, 6, row.DR, styleCellDefault);
                    efm.PrintCell(r, 7, row.ID_PAC, styleCellDefault);
                    efm.PrintCell(r, 8, row.VPOLIS, styleCellDefault);
                    efm.PrintCell(r, 9, row.SPOLIS, styleCellDefault);
                    efm.PrintCell(r, 10, row.NPOLIS, styleCellDefault);
                    efm.PrintCell(r, 11, row.LPU_1, styleCellDefault);
                    efm.PrintCell(r, 12, row.NHISTORY, styleCellDefault);
                    efm.PrintCell(r, 13, row.USL_OK, styleCellDefault);
                    efm.PrintCell(r, 14, row.IDDOKT, styleCellDefault);
                    efm.PrintCell(r, 15, row.DATE_1, styleCellDefaultDate);
                    efm.PrintCell(r, 16, row.DATE_2, styleCellDefaultDate);
                    efm.PrintCell(r, 17, row.ERR, tERR);

                    RowIndexEx++;
                }
                efm.Save();
                efm = null;
                GC.Collect();
                foreach (var T in ERR_LIST)
                {
                    var pathToXML = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(T.Key) + "FLK.xml");
                    SchemaChecking.XMLfileFLK(pathToXML, T.Key, T.Value);
                }

            }
            catch (Exception ex)
            {
                efm?.Dispose();
                GC.Collect();
                Logger.AddLog($"Ошибка при формировании Excel(ФЛК) файла({path}): {ex.Message}", LogType.Error);
            }
        }
        private int FindRowSVODTBL(DataTable tbl, string FileName)
        {
            var i = 0;
            if (tbl == null)
                return -1;
            if (!tbl.Columns.Contains("FILENAME"))
                return -1;
            foreach (DataRow row in tbl.Rows)
            {
                if (Path.GetFileNameWithoutExtension(row["FILENAME"].ToString().ToUpper()) == Path.GetFileNameWithoutExtension(FileName.ToUpper()))
                    return i;
                i++;
            }
            return -1;
        }
    
        public void CreateExcelSvod2(FilePacket pack, string path, DataTable SVOD, DataTable STAT_VID_MP, DataTable STAT_FULL)
        {
            ExcelOpenXML efm = null;
            try
            {
                var SvodList = new List<ItemSvod>();
                pack.PATH_STAT = path;

                for (var i = 0; i < pack.Files.Count; i++)
                {
                    var item = new ItemSvod { File = pack.Files[i], NUM = i + 1 };
                    SvodList.Add(item);
                    var index = FindRowSVODTBL(SVOD, item.FileName);
                    if (index != -1)
                    {
                        item.Comment = SVOD.Rows[index]["COM"].ToString();
                        item.SUM = Convert.ToDecimal(SVOD.Rows[index]["SUM"]);
                        item.SUM_MEK = Convert.ToDecimal(SVOD.Rows[index]["SUM_MEK"]);
                        item.CSLUCH = Convert.ToInt32(SVOD.Rows[index]["CSLUCH"]);
                        item.CUSL = Convert.ToInt32(SVOD.Rows[index]["CUSL"]);
                    }
                    else
                    {
                        item.SUM = 0;
                        item.SUM_MEK = 0;
                        item.CSLUCH = 0;
                        item.CUSL = 0;
                    }

                    if (pack.Files[i].filel != null)
                    {
                        var itemL = new ItemSvod { File = pack.Files[i].filel };
                        SvodList.Add(itemL);
                    }
                }

                if (File.Exists(path))
                {
                    efm = new ExcelOpenXML();
                    efm.OpenFile(path, 0);
                    if (efm.CurrentSheetName != "Свод")
                    {
                        efm.InsertSheet("Свод");
                    }
                }
                else
                {
                    efm = new ExcelOpenXML(path, "Свод");
                }
                var styleHeader = efm.CreateType(new FontOpenXML { HorizontalAlignment = HorizontalAlignmentV.Center, wordwrap = true, Bold = true }, new BorderOpenXML(), new FillOpenXML());

                var r = efm.GetRow(1);

                efm.PrintCell(r, 1, "№", styleHeader);
                efm.PrintCell(r, 2, "Имя файла", styleHeader);
                efm.PrintCell(r, 3, "Результат приема", styleHeader);
                efm.PrintCell(r, 4, "Сумма реестра", styleHeader);
                efm.PrintCell(r, 5, "Сумма снятия(предварительно)", styleHeader);
                efm.PrintCell(r, 6, "Кол-во случаев", styleHeader);
                efm.PrintCell(r, 7, "Кол-во услуг", styleHeader);
                efm.PrintCell(r, 8, "Дата приёма", styleHeader);
                efm.PrintCell(r, 9, "Дополнительно", styleHeader);

                efm.SetColumnWidth(1, 4);

                var Style = efm.CreateType(new FontOpenXML { HorizontalAlignment = HorizontalAlignmentV.Left }, new BorderOpenXML(), new FillOpenXML());
                var StyleWRAP = efm.CreateType(new FontOpenXML { wordwrap = true, HorizontalAlignment = HorizontalAlignmentV.Left }, new BorderOpenXML(), new FillOpenXML());
                var StyleBold = efm.CreateType(new FontOpenXML { HorizontalAlignment = HorizontalAlignmentV.Left, Bold = true }, new BorderOpenXML(), new FillOpenXML());
                var StyleRed = efm.CreateType(new FontOpenXML { HorizontalAlignment = HorizontalAlignmentV.Left }, new BorderOpenXML(), new FillOpenXML { color = System.Windows.Media.Colors.LightSalmon });
                var StyleRedWRAP = efm.CreateType(new FontOpenXML { wordwrap = true, HorizontalAlignment = HorizontalAlignmentV.Left }, new BorderOpenXML(), new FillOpenXML { color = System.Windows.Media.Colors.LightSalmon });


                var StyleResTrue = efm.CreateType(new FontOpenXML { color = System.Windows.Media.Colors.Blue, Underline = true, HorizontalAlignment = HorizontalAlignmentV.Left }, new BorderOpenXML(), new FillOpenXML { color = System.Windows.Media.Colors.YellowGreen });
                var StyleResFalse = efm.CreateType(new FontOpenXML { color = System.Windows.Media.Colors.Blue, Underline = true, HorizontalAlignment = HorizontalAlignmentV.Left }, new BorderOpenXML(), new FillOpenXML { color = System.Windows.Media.Colors.LightSalmon });

                var Style1 = efm.CreateType(new FontOpenXML { HorizontalAlignment = HorizontalAlignmentV.Right, Format = Convert.ToUInt32(DefaultNumFormat.F4) }, new BorderOpenXML(), new FillOpenXML());
                var Style1Red = efm.CreateType(new FontOpenXML { HorizontalAlignment = HorizontalAlignmentV.Right, Format = Convert.ToUInt32(DefaultNumFormat.F4) }, new BorderOpenXML(), new FillOpenXML { color = System.Windows.Media.Colors.LightSalmon });
                var Style1Bold = efm.CreateType(new FontOpenXML { Bold = true, HorizontalAlignment = HorizontalAlignmentV.Right, Format = Convert.ToUInt32(DefaultNumFormat.F4) }, new BorderOpenXML(), new FillOpenXML());

                var Style1ResFalse = efm.CreateType(new FontOpenXML { HorizontalAlignment = HorizontalAlignmentV.Right, Format = Convert.ToUInt32(DefaultNumFormat.F4) }, new BorderOpenXML(), new FillOpenXML { color = System.Windows.Media.Colors.LightSalmon });
                var Style2 = efm.CreateType(new FontOpenXML { HorizontalAlignment = HorizontalAlignmentV.Right, Format = Convert.ToUInt32(DefaultNumFormat.F3) }, new BorderOpenXML(), new FillOpenXML());
                var Style2Red = efm.CreateType(new FontOpenXML { HorizontalAlignment = HorizontalAlignmentV.Right, Format = Convert.ToUInt32(DefaultNumFormat.F3) }, new BorderOpenXML(), new FillOpenXML { color = System.Windows.Media.Colors.LightSalmon });
                var Style2Bold = efm.CreateType(new FontOpenXML { Bold = true, HorizontalAlignment = HorizontalAlignmentV.Right, Format = Convert.ToUInt32(DefaultNumFormat.F3) }, new BorderOpenXML(), new FillOpenXML());

                var stWARNNING = efm.CreateType(new FontOpenXML { HorizontalAlignment = HorizontalAlignmentV.Left, Bold = true }, null, new FillOpenXML { color = System.Windows.Media.Colors.Yellow });
                var stWARNNINGBorder = efm.CreateType(new FontOpenXML { HorizontalAlignment = HorizontalAlignmentV.Left, Bold = true }, new BorderOpenXML(), new FillOpenXML { color = System.Windows.Media.Colors.Yellow });

                uint indexRow = 2;
                foreach (var item in SvodList)
                {
                    r = efm.GetRow(indexRow);
                    efm.PrintCell(r, 1, item.NUM, Style);
                    efm.PrintCell(r, 2, item.FileName, Style);
                    efm.PrintCell(r, 3, item.Result, item.IsError ? StyleResFalse : StyleResTrue);
                    if (!string.IsNullOrEmpty(item.REL))
                        efm.PrintHyperlink(r, 3, item.REL);


                    if (item.SUM.HasValue && item.CSLUCH.HasValue && item.CUSL.HasValue)
                    {
                        efm.PrintCell(r, 4, item.SUM, Style1);
                        efm.PrintCell(r, 5, item.SUM_MEK, item.SUM_MEK == 0 ? Style1 : Style1ResFalse);
                        efm.PrintCell(r, 6, item.CSLUCH, Style2);
                        efm.PrintCell(r, 7, item.CUSL, Style2);
                    }
                    else
                    {
                        efm.PrintCell(r, 4, "", Style1);
                        efm.PrintCell(r, 5, "", Style1);
                        efm.PrintCell(r, 6, "", Style1);
                        efm.PrintCell(r, 7, "", Style1);
                    }
                    efm.PrintCell(r, 8, item.DATE.ToShortDateString() + " " + item.DATE.ToShortTimeString(), Style);
                    efm.PrintCell(r, 9, item.Comment, string.IsNullOrEmpty(item.Comment) ? Style : stWARNNINGBorder);
                    indexRow++;
                }

                indexRow++;
                //efm.AutoSizeColumn(0, 8);
                var t = efm.CreateType(new FontOpenXML { HorizontalAlignment = HorizontalAlignmentV.Left, Bold = true }, null, new FillOpenXML());
                r = efm.GetRow(indexRow);
                efm.PrintCell(r, 1, AppConfig.Property.MainTypePriem ? $"Тип приёма реестров: Основной" : "Тип приёма реестров: Предварительный", t);
                indexRow++;
                r = efm.GetRow(indexRow);
                efm.PrintCell(r, 1, $"Отчетный период: {AppConfig.Property.OtchetDate:MMMMMMMMMM yyyy} года.", t);
                indexRow++;
                r = efm.GetRow(indexRow);
                efm.PrintCell(r, 1, $"Ф.И.О. принимающего лица: {AppConfig.Property.ISP_NAME}", t);
                if (pack.WARNNING != "")
                {
                    indexRow++;
                    r = efm.GetRow(indexRow);
                    efm.PrintCell(r, 1, $"Предупреждение: {pack.WARNNING}", stWARNNING);
                    string str = null;
                    efm.PrintCell(r, 2, str, stWARNNING);
                    efm.PrintCell(r, 3, str, stWARNNING);
                }
                efm.AutoSizeColumns(2, 9);

                if (STAT_VID_MP != null)
                {
                    if (STAT_VID_MP.Rows.Count != 0)
                    {
                        efm.AddSheet("Статистика");
                        indexRow = 1;
                        r = efm.GetRow(indexRow);
                        efm.PrintCell(r, 1, "Всего предъявлено:", t);
                        indexRow++;
                        r = efm.GetRow(indexRow);
                        efm.PrintCell(r, 1, "№", styleHeader);
                        efm.PrintCell(r, 2, "Вид медицинской помощи", styleHeader);
                        efm.PrintCell(r, 3, "Кол-во законченных случаев", styleHeader);
                        efm.PrintCell(r, 4, "Кол-во случаев", styleHeader);
                        efm.PrintCell(r, 5, "Сумма", styleHeader);
                        efm.SetColumnWidth(1, 9);
                        efm.SetColumnWidth(2, 91);
                        efm.SetColumnWidth(3, 16);
                        efm.SetColumnWidth(4, 16);
                        efm.SetColumnWidth(5, 16);

                        indexRow++;
                        double C_ZS_S = 0;
                        double C_SL_S = 0;
                        double SUMV_S = 0;

                        double C_ZS = 0;
                        double C_SL = 0;
                        double SUMV = 0;

                        foreach (DataRow row in STAT_VID_MP.Rows)
                        {
                            r = efm.GetRow(indexRow);
                            C_ZS = Convert.ToDouble(row["C_ZS"]);
                            C_SL = Convert.ToDouble(row["C_SL"]);
                            SUMV = Convert.ToDouble(row["SUMV"]);
                            C_ZS_S += C_ZS;
                            C_SL_S += C_SL;
                            SUMV_S += SUMV;
                            if (row["PS"] == DBNull.Value)
                            {
                                efm.PrintCell(r, 1, row["PS"].ToString(), StyleRed);
                                efm.PrintCell(r, 2, row["NAME"].ToString(), StyleRedWRAP);
                                efm.PrintCell(r, 3, C_ZS, Style2Red);
                                efm.PrintCell(r, 4, C_SL, Style2Red);
                                efm.PrintCell(r, 5, SUMV, Style1Red);
                            }
                            else
                            {
                                efm.PrintCell(r, 1, row["PS"].ToString(), Style);
                                efm.PrintCell(r, 2, row["NAME"].ToString(), StyleWRAP);
                                efm.PrintCell(r, 3, C_ZS, Style2);
                                efm.PrintCell(r, 4, C_SL, Style2);
                                efm.PrintCell(r, 5, SUMV, Style1);
                            }
                            indexRow++;
                        }
                        r = efm.GetRow(indexRow);
                        efm.PrintCell(r, 2, "Итого", StyleBold);
                        efm.PrintCell(r, 3, Math.Round(C_ZS_S, 0), Style2Bold);
                        efm.PrintCell(r, 4, Math.Round(C_SL_S, 0), Style2Bold);
                        efm.PrintCell(r, 5, Math.Round(SUMV_S, 2), Style1Bold);

                    }
                }


                if (STAT_FULL != null)
                {
                    if (STAT_FULL.Rows.Count != 0)
                    {
                        efm.AddSheet("Статистика(Расширенная)");
                        indexRow = 1;
                        r = efm.GetRow(indexRow);
                        efm.PrintCell(r, 1, "Всего предъявлено:", t);
                        indexRow++;
                        r = efm.GetRow(indexRow);
                        efm.PrintCell(r, 1, "Условие оказания", styleHeader);
                        efm.PrintCell(r, 2, "КСГ/Услуга/Метод ВМП", styleHeader);
                        efm.PrintCell(r, 3, "Наименование", styleHeader);
                        efm.PrintCell(r, 4, "Кол-во случаев", styleHeader);
                        efm.PrintCell(r, 5, "Кол-во услуг/ует", styleHeader);
                        efm.PrintCell(r, 6, "Сумма", styleHeader);
                        efm.SetColumnWidth(1, 21);
                        efm.SetColumnWidth(2, 14);
                        efm.SetColumnWidth(3, 85);
                        efm.SetColumnWidth(4, 16);
                        efm.SetColumnWidth(5, 16);
                        efm.SetColumnWidth(6, 16);
                        indexRow++;
                        double SL;
                        double SUM_SL = 0;
                        double SUMV;
                        double SUM_SUMV = 0;
                        double KOL_USL = 0;
                        double SUM_KOL_USL = 0;
                        foreach (DataRow row in STAT_FULL.Rows)
                        {
                            r = efm.GetRow(indexRow);
                            SL = Convert.ToDouble(row["SL"]);
                            SUMV = Convert.ToDouble(row["SUMV"]);
                            KOL_USL = Convert.ToDouble(row["KOL_USL"]);
                            SUM_SL += SL;
                            SUM_SUMV += SUMV;
                            SUM_KOL_USL += KOL_USL;
                            efm.PrintCell(r, 1, row["USL_OK"].ToString(), Style);
                            efm.PrintCell(r, 2, row["N_KSG"].ToString(), Style);
                            efm.PrintCell(r, 3, row["KSG"].ToString(), StyleWRAP);
                            efm.PrintCell(r, 4, SL, Style2);
                            efm.PrintCell(r, 5, KOL_USL, Style1);
                            efm.PrintCell(r, 6, SUMV, Style1);
                            indexRow++;
                        }

                        r = efm.GetRow(indexRow);
                        efm.PrintCell(r, 3, "Итого", StyleBold);
                        efm.PrintCell(r, 4, Math.Round(SUM_SL, 0), Style2Bold);
                        efm.PrintCell(r, 5, Math.Round(SUM_KOL_USL, 0), Style2Bold);
                        efm.PrintCell(r, 6, Math.Round(SUM_SUMV, 2), Style1Bold);
                    }
                }

                efm.Save();

                efm = null;
                STAT_VID_MP = null;
                GC.Collect();

            }
            catch (Exception ex)
            {
                efm?.Dispose();
                Logger.AddLog($"Ошибка при формировании Excel(СВОД) файла({path}): {ex.Message}", LogType.Error);
            }
        }
    }







}
