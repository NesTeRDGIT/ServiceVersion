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
        void ExportErrorView(List<V_ErrorViewRow> list, string path);
        void CreateExcelSvod(FilePacket pack, string path, List<SVOD_FILE_Row> SVOD, List<STAT_VIDMP_Row> STAT_VID_MP, List<STAT_FULL_Row> STAT_FULL);

    }
    public class ExcelProtokol: IExcelProtokol
    {
        
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

        private ILogger Logger;

        public ExcelProtokol(ILogger Logger)
        {
            this.Logger = Logger;
        }
        public void ExportErrorView(List<V_ErrorViewRow> list, string path)
        {
            try
            {
                using (var efm = new ExcelOpenXML())
                {
                    if (File.Exists(path))
                    {
                        efm.OpenFile(path, 0);
                        if (efm.CurrentSheetName != "Ошибки ФЛК")
                        {
                            efm.AddSheet("Ошибки ФЛК");
                        }
                    }
                    else
                    {
                        efm.Create(path, "Ошибки ФЛК");
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
                    if (list.Count == 0)
                    {
                        r = efm.GetRow(RowIndexEx, true);
                        efm.PrintCell(r, 1, "Ошибки отсутствуют", styleCellDefault);
                        efm.AddMergedRegion(new CellRangeAddress(RowIndexEx, 1, RowIndexEx, 17));
                        efm.Save();
                        return;
                    }
                    var ERR_LIST = new Dictionary<string, List<ErrorProtocolXML>>();
                    foreach (var item in list)
                    {
                        #region Формирование файлов FLK
                        var fi = item.FILENAME;
                        var err = new ErrorProtocolXML
                        {
                            BAS_EL = item.BAS_EL,
                            IDCASE = item.IDCASE,
                            ID_SERV = item.ID_SERV,
                            SL_ID = item.SL_ID,
                            N_ZAP = item.N_ZAP,
                            OSHIB = item.OSHIB,
                            Comment = item.ERR
                        };
                        if (!ERR_LIST.ContainsKey(fi))
                        {
                            ERR_LIST.Add(fi, new List<ErrorProtocolXML>());
                        }
                        ERR_LIST[fi].Add(err);
                        #endregion
                        r = efm.GetRow(RowIndexEx, true);
                        uint tERR;

                        switch (item.ERR_TYPE)
                        {
                            case 1: tERR = styleCellYellow; break;
                            case 2: tERR = styleCellRed; break;
                            default:
                                tERR = styleCellDefault;
                                break;
                        }

                        efm.PrintCell(r, 1, item.SLUCH_ID, styleCellDefault);
                        efm.PrintCell(r, 2, item.CODE_MO, styleCellDefault);
                        efm.PrintCell(r, 3, item.FAM, styleCellDefault);
                        efm.PrintCell(r, 4, item.IM, styleCellDefault);
                        efm.PrintCell(r, 5, item.OT, styleCellDefault);
                        efm.PrintCell(r, 6, item.DR, styleCellDefault);
                        efm.PrintCell(r, 7, item.ID_PAC, styleCellDefault);
                        efm.PrintCell(r, 8, item.VPOLIS, styleCellDefault);
                        efm.PrintCell(r, 9, item.SPOLIS, styleCellDefault);
                        efm.PrintCell(r, 10, item.NPOLIS, styleCellDefault);
                        efm.PrintCell(r, 11, item.LPU_1, styleCellDefault);
                        efm.PrintCell(r, 12, item.NHISTORY, styleCellDefault);
                        efm.PrintCell(r, 13, item.USL_OK, styleCellDefault);
                        efm.PrintCell(r, 14, item.IDDOKT, styleCellDefault);
                        efm.PrintCell(r, 15, item.DATE_1, styleCellDefaultDate);
                        efm.PrintCell(r, 16, item.DATE_2, styleCellDefaultDate);
                        efm.PrintCell(r, 17, item.ERR, tERR);
                        RowIndexEx++;
                    }
                    efm.Save();

                    foreach (var T in ERR_LIST)
                    {
                        var pathToXML = Path.Combine(Path.GetDirectoryName(path), $"{Path.GetFileNameWithoutExtension(T.Key)}FLK.xml");
                        SchemaChecking.XMLfileFLK(pathToXML, T.Key, T.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.AddLog($"Ошибка при формировании Excel(ФЛК) файла({path}): {ex.Message}", LogType.Error);
            }
        }
    
        public void CreateExcelSvod(FilePacket pack, string path, List<SVOD_FILE_Row> SVOD, List<STAT_VIDMP_Row> STAT_VID_MP, List<STAT_FULL_Row> STAT_FULL)
        {
            ;
            try
            {
                var SvodList = new List<ItemSvod>();
                pack.PATH_STAT = path;
                var i = 1;
                foreach (var file in pack.Files)
                {
                    var item = new ItemSvod { File = file, NUM = i };
                    SvodList.Add(item);
                    var sv = SVOD?.FirstOrDefault(x => x.FILENAME == Path.GetFileNameWithoutExtension(item.FileName));
                    item.Comment = sv?.COM;
                    item.SUM = sv?.SUM ?? 0;
                    item.SUM_MEK = sv?.SUM_MEK ?? 0;
                    item.CSLUCH = sv?.CSLUCH ?? 0;
                    item.CUSL = sv?.CUSL ?? 0;

                    if (file.filel != null)
                    {
                        var itemL = new ItemSvod { File = file.filel };
                        SvodList.Add(itemL);
                    }
                    i++;
                }


                using (var efm = new ExcelOpenXML())
                {
                    if (File.Exists(path))
                    {
                        efm.OpenFile(path, 0);
                        if (efm.CurrentSheetName != "Свод")
                        {
                            efm.InsertSheet("Свод");
                        }
                    }
                    else
                    {
                        efm.Create(path, "Свод");
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

                    #region Лист Статистика
                    if (STAT_VID_MP != null)
                    {
                        if (STAT_VID_MP.Count != 0)
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
                            int C_ZS_S = 0;
                            int C_SL_S = 0;
                            decimal SUMV_S = 0;

                            foreach (var row in STAT_VID_MP)
                            {
                                r = efm.GetRow(indexRow);
                              
                                C_ZS_S += row.C_ZS;
                                C_SL_S += row.C_SL;
                                SUMV_S += row.SUMV;

                                var stylePS = Style;
                                var styleNAME = StyleWRAP;
                                var styleINT = Style2;
                                var styleDEC = Style1;

                                if (string.IsNullOrEmpty(row.PS))
                                {
                                     stylePS = StyleRed;
                                     styleNAME = StyleRedWRAP;
                                     styleINT = Style2Red;
                                     styleDEC = Style1Red;
                                }

                                efm.PrintCell(r, 1, row.PS, stylePS);
                                efm.PrintCell(r, 2, row.NAME, styleNAME);
                                efm.PrintCell(r, 3, row.C_ZS, styleINT);
                                efm.PrintCell(r, 4, row.C_SL, styleINT);
                                efm.PrintCell(r, 5, row.SUMV, styleDEC);
                                indexRow++;
                            }
                            r = efm.GetRow(indexRow);
                            efm.PrintCell(r, 2, "Итого", StyleBold);
                            efm.PrintCell(r, 3, C_ZS_S, Style2Bold);
                            efm.PrintCell(r, 4, C_SL_S, Style2Bold);
                            efm.PrintCell(r, 5, Math.Round(SUMV_S, 2), Style1Bold);
                        }
                    }
                    #endregion

                    #region Лист Статистика(Расширенная)
                    if (STAT_FULL != null)
                    {
                        if (STAT_FULL.Count != 0)
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
                          
                            var SUM_SL = 0;
                            decimal SUM_SUMV = 0;
                            decimal SUM_KOL_USL = 0;
                            foreach (var row in STAT_FULL)
                            {
                                r = efm.GetRow(indexRow);
                                SUM_SL += row.SL;
                                SUM_SUMV += row.SUMV;
                                SUM_KOL_USL += row.KOL_USL;
                                efm.PrintCell(r, 1, row.USL_OK, Style);
                                efm.PrintCell(r, 2, row.N_KSG, Style);
                                efm.PrintCell(r, 3, row.KSG, StyleWRAP);
                                efm.PrintCell(r, 4, row.SL, Style2);
                                efm.PrintCell(r, 5, row.KOL_USL, Style1);
                                efm.PrintCell(r, 6, row.SUMV, Style1);
                                indexRow++;
                            }

                            r = efm.GetRow(indexRow);
                            efm.PrintCell(r, 3, "Итого", StyleBold);
                            efm.PrintCell(r, 4, SUM_SL, Style2Bold);
                            efm.PrintCell(r, 5, Math.Round(SUM_KOL_USL, 2), Style2Bold);
                            efm.PrintCell(r, 6, Math.Round(SUM_SUMV, 2), Style1Bold);
                        }
                    }
                    #endregion
                    efm.Save();
                }
            }
            catch (Exception ex)
            {
                Logger.AddLog($"Ошибка при формировании Excel(СВОД) файла({path}): {ex.Message}", LogType.Error);
            }
        }
    }







}
