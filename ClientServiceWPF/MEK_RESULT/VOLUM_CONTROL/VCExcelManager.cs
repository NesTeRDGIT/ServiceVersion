using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServiceWPF.MEK_RESULT.ACTMEK;
using ExcelManager;

namespace ClientServiceWPF.MEK_RESULT.VOLUM_CONTROL
{
    public interface IVCExcelManager
    {
        void VR_TO_PRIL5(string path, string FAM_RUK, string DOLG_RUK, IEnumerable<LIMIT_RESULTRow> list);
        void VR_TO_XLS(string path, IEnumerable<LIMIT_RESULTRow> list);
    }
    public class VCExcelManager: IVCExcelManager
    {

        private PRIL5_Row GetPril5NameRow(string CODE)
        {
            switch (CODE)
            {
                case "4":
                case "4.3":
                    return new PRIL5_Row { CODE = 1, NAME = "Скорая МП" };
                case "3.1.1":
                case "3.1.2":
                case "3.4.1":
                case "3.4.2":
                case "3.5.1":
                case "3.5.2":
                case "3.6.1":
                case "3.6.2":
                case "3.7.1":
                case "3.7.2":
                    return new PRIL5_Row { CODE = 2, NAME = "Амбулаторная МП" };
                case "3.5.3":
                    return new PRIL5_Row { CODE = 3, NAME = "Диспансеризация 2 этап" };
                case "3.8.1":
                    return new PRIL5_Row { CODE = 4, NAME = "Углубленная диспансеризация" };
                case "3.8.2":
                    return new PRIL5_Row { CODE = 5, NAME = "Углубленная диспансеризация 2 этап" };
                case "5.2":
                    return new PRIL5_Row { CODE = 6, NAME = "Услуги диализа" };
                case "3.3.7":
                case "3.3.8":
                    return new PRIL5_Row { CODE = 7, NAME = "Определение РНК коронавирусов" };
                case "3.1.3":
                case "3.1.4":
                    return new PRIL5_Row { CODE = 8, NAME = "ФП/ФАП" };

                case "3.3.1":
                    return new PRIL5_Row { CODE = 9, NAME = "КТ" };
                case "3.3.2":
                    return new PRIL5_Row { CODE = 10, NAME = "МРТ" };
                case "3.3.3":
                    return new PRIL5_Row { CODE = 11, NAME = "УЗИ сердечно-сосуд.системы" };
                case "3.3.4":
                    return new PRIL5_Row { CODE = 12, NAME = "Эндоскопические диагн. исследования" };
                case "3.3.5":
                    return new PRIL5_Row { CODE = 13, NAME = "Патологоанатомические исследования" };
                case "3.3.6":
                    return new PRIL5_Row { CODE = 14, NAME = "Молекулярно - диагн.исследования" };
                case "3.2.1":
                case "3.2.2":
                    return new PRIL5_Row { CODE = 15, NAME = "Неотложная МП" };


                case "1.1": return new PRIL5_Row { CODE = 16, NAME = "Стационар без онкологии" };
                case "1.4": return new PRIL5_Row { CODE = 17, NAME = "Стационар без онкологии(МБТ ПП РФ 1213)" };
                case "1.5": return new PRIL5_Row { CODE = 18, NAME = "Стационар без онкологии(МБТ ПП РФ 1997-р)" };
                case "1.2": return new PRIL5_Row { CODE = 19, NAME = "Стационар онкология" };
                case "5.1": return new PRIL5_Row { CODE = 20, NAME = "Стационар диализ" };
                case "1.3": return new PRIL5_Row { CODE = 21, NAME = "ВМП" };
                case "2.1": return new PRIL5_Row { CODE = 22, NAME = "Дневной стационар без онкологии" };
                case "2.2": return new PRIL5_Row { CODE = 23, NAME = "Дневной стационар онкология" };
                case "2.3": return new PRIL5_Row { CODE = 24, NAME = "Дневной стационар ЭКО" };
                case "-": return new PRIL5_Row { CODE = 25, NAME = "Прочее" };
            }
            throw new Exception($"Не найдена строка в приложении 5 для: {CODE}");
        }
        Dictionary<string, PRIL5> ConvertToPril5(IEnumerable<LIMIT_RESULTRow> list)
        {
            var pril5 = new Dictionary<string, PRIL5>();
            foreach (var item in list)
            {
                if (!pril5.ContainsKey(item.CODE_MO))
                    pril5.Add(item.CODE_MO, new PRIL5() { CODE_MO = item.CODE_MO, NAME_MOK = item.NAM_MOK });
                var pr = pril5[item.CODE_MO];
                var st = GetPril5NameRow(item.RUBRIC);
                var pr_row = pr.Rows.FirstOrDefault(x => x.CODE == st.CODE);
                if (pr_row == null)
                {
                    pr.Rows.Add(st);
                    pr_row = st;
                }

                pr_row.SUMV += item.SUM_ALL;
                pr_row.SUM_MEK += item.S_MEK_NOT_V;
                pr_row.SUM_VOLUM += item.S_MEK_VS + item.S_MEK_VK;
                pr_row.SUM_MUR += item.MUR;
                pr_row.SUM_P += item.SUM_P_ALL;
                pr_row.SUM_MEK_P += item.SUM_MEK_P;
                pr_row.SUM_MUR_RETURN += item.MUR_RETURN;
            }
            return pril5;
        }

        public void VR_TO_PRIL5(string path, string FAM_RUK, string DOLG_RUK, IEnumerable<LIMIT_RESULTRow> list)
        {

            using (var efm = new ExcelOpenXML(path, "Приложение 5"))
            {
                efm.SetColumnWidth("A", 8.43);
                efm.SetColumnWidth("B", 30);
                efm.SetColumnWidth("C", 9);
                efm.SetColumnWidth("D", 30);
                efm.SetColumnWidth("E", 17);
                efm.SetColumnWidth("F", 17);
                efm.SetColumnWidth("G", 17);
                efm.SetColumnWidth("H", 17);
                efm.SetColumnWidth("I", 17);
                efm.SetColumnWidth("J", 17);
                efm.SetColumnWidth("K", 17);

                uint ColI = 5; uint RowI = 1;

                var stringPRIL5Style = efm.CreateType(new FontOpenXML() { Bold = false, fontname = "Times New Roman", size = 22, HorizontalAlignment = HorizontalAlignmentV.Right, VerticalAlignment = VerticalAlignmentV.Center, wordwrap = true }, null, null);
                var stringPERIODStyle = efm.CreateType(new FontOpenXML() { Bold = false, fontname = "Times New Roman", size = 22, HorizontalAlignment = HorizontalAlignmentV.Left, VerticalAlignment = VerticalAlignmentV.Center, wordwrap = false }, null, null);
                var mrow = efm.GetRow(RowI);
                mrow.Height = 66;
                efm.PrintCell(mrow, ColI, "Приложение № 5 к Положению о порядке оплаты медицинской помощи в системе ОМС Забайкальского края", stringPRIL5Style); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI, ColI + 6));
                RowI++;
                mrow = efm.GetRow(RowI);
                mrow.Height = 58;
                RowI++;
                ColI = 1;
                var fItem = list.First();
                var period = new DateTime(fItem.YEAR, fItem.MONTH, 1);
                efm.PrintCell(RowI, ColI, $"Отчетный период: {period:MMMMMMMMMMMMMMMM  yyyy}", stringPERIODStyle);
                RowI += 2;
                ColI = 1;
                var HeadStyle = efm.CreateType(new FontOpenXML() { Bold = true, fontname = "Times New Roman", size = 10, HorizontalAlignment = HorizontalAlignmentV.Center, VerticalAlignment = VerticalAlignmentV.Center, wordwrap = true }, new BorderOpenXML(), null);
                mrow = efm.GetRow(RowI);
                mrow.Height = 38.25;
                efm.PrintCell(mrow, ColI, "№", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "Наименование МО", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "Код МО", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "Вид мед.помощи", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "Представлено по реестру", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "МЭК без превышения объемов", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "МЭК превышение объемов", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "Снято по МУР", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "Принято реестров", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "МЭК прошлых периодов", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "Возврат МУР", HeadStyle); ColI++;

                RowI++;
                ColI = 1;
                mrow = efm.GetRow(RowI);

                efm.PrintCell(mrow, ColI, "1", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "2", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "3", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "4", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "5", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "6", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "7", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "8", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "9", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "10", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "11", HeadStyle); ColI++;




                var TextStyle = efm.CreateType(new FontOpenXML() { HorizontalAlignment = HorizontalAlignmentV.Left, fontname = "Times New Roman", size = 11, wordwrap = true}, new BorderOpenXML(), null);
                var TextStyleBOLD = efm.CreateType(new FontOpenXML() { HorizontalAlignment = HorizontalAlignmentV.Left, Bold = true, fontname = "Times New Roman", size = 11, wordwrap = true }, new BorderOpenXML(), null);
                var TextStyleBOLDRight = efm.CreateType(new FontOpenXML() { HorizontalAlignment = HorizontalAlignmentV.Right, Bold = true, fontname = "Times New Roman", size = 11 }, new BorderOpenXML(), null);

             
                var NumberStyle = efm.CreateType(new FontOpenXML() { HorizontalAlignment = HorizontalAlignmentV.Right, Format = (uint)DefaultNumFormat.F4, fontname = "Times New Roman", size = 11 }, new BorderOpenXML(), null);
                var NumberStyleBold = efm.CreateType(new FontOpenXML() { HorizontalAlignment = HorizontalAlignmentV.Right, Format = (uint)DefaultNumFormat.F4, fontname = "Times New Roman", size = 11, Bold = true }, new BorderOpenXML(), null);


                var PRIL5 = ConvertToPril5(list);
                int i = 0;

                decimal SUMV, SUMV_ALL = 0, SUM_MEK, SUM_MEK_ALL = 0, SUM_VOLUM, SUM_VOLUM_ALL = 0, SUM_MUR, SUM_MUR_ALL = 0, SUM_P, SUM_P_ALL = 0, SUM_MEK_P, SUM_MEK_P_ALL = 0, SUM_MUR_RETURN, SUM_MUR_RETURN_ALL = 0;

                foreach (var dic_item in PRIL5)
                {
                    RowI++;
                    var pr5 = dic_item.Value;
                    i++;
                    ColI = 1;
                    mrow = efm.GetRow(RowI);
                    SUMV = pr5.Rows.Sum(x => x.SUMV); SUMV_ALL += SUMV;
                    SUM_MEK = pr5.Rows.Sum(x => x.SUM_MEK); SUM_MEK_ALL += SUM_MEK;
                    SUM_VOLUM = pr5.Rows.Sum(x => x.SUM_VOLUM); SUM_VOLUM_ALL += SUM_VOLUM;
                    SUM_MUR = pr5.Rows.Sum(x => x.SUM_MUR); SUM_MUR_ALL += SUM_MUR;
                    SUM_P = pr5.Rows.Sum(x => x.SUM_P); SUM_P_ALL += SUM_P;
                    SUM_MEK_P = pr5.Rows.Sum(x => x.SUM_MEK_P); SUM_MEK_P_ALL += SUM_MEK_P;
                    SUM_MUR_RETURN = pr5.Rows.Sum(x => x.SUM_MUR_RETURN); SUM_MUR_RETURN_ALL += SUM_MUR_RETURN;



                    efm.PrintCell(mrow, ColI, i.ToString(), TextStyleBOLD); ColI++;
                    efm.PrintCell(mrow, ColI, pr5.NAME_MOK, TextStyleBOLD); ColI++;
                    efm.PrintCell(mrow, ColI, pr5.CODE_MO, TextStyleBOLD); ColI++;
                    efm.PrintCell(mrow, ColI, "Итого", TextStyleBOLD); ColI++;
                    efm.PrintCell(mrow, ColI, SUMV, NumberStyleBold); ColI++;
                    efm.PrintCell(mrow, ColI, SUM_MEK, NumberStyleBold); ColI++;
                    efm.PrintCell(mrow, ColI, SUM_VOLUM, NumberStyleBold); ColI++;
                    efm.PrintCell(mrow, ColI, SUM_MUR, NumberStyleBold); ColI++;
                    efm.PrintCell(mrow, ColI, SUM_P, NumberStyleBold); ColI++;
                    efm.PrintCell(mrow, ColI, SUM_MEK_P, NumberStyleBold); ColI++;
                    efm.PrintCell(mrow, ColI, SUM_MUR_RETURN, NumberStyleBold); ColI++;

                    foreach (var row in pr5.Rows.OrderBy(x => x.CODE))
                    {
                        RowI++;
                        ColI = 1;
                        mrow = efm.GetRow(RowI);
                        efm.PrintCell(mrow, ColI, "", TextStyleBOLD); ColI++;
                        efm.PrintCell(mrow, ColI, "", TextStyleBOLD); ColI++;
                        efm.PrintCell(mrow, ColI, "", TextStyleBOLD); ColI++;
                        efm.PrintCell(mrow, ColI, row.NAME, TextStyle); ColI++;
                        efm.PrintCell(mrow, ColI, row.SUMV, NumberStyle); ColI++;
                        efm.PrintCell(mrow, ColI, row.SUM_MEK, NumberStyle); ColI++;
                        efm.PrintCell(mrow, ColI, row.SUM_VOLUM, NumberStyle); ColI++;
                        efm.PrintCell(mrow, ColI, row.SUM_MUR, NumberStyle); ColI++;
                        efm.PrintCell(mrow, ColI, row.SUM_P, NumberStyle); ColI++;
                        efm.PrintCell(mrow, ColI, row.SUM_MEK_P, NumberStyle); ColI++;
                        efm.PrintCell(mrow, ColI, row.SUM_MUR_RETURN, NumberStyle); ColI++;
                    }
                }

                RowI++;
                ColI = 1;
                efm.PrintCell(RowI, 1, "Итого", TextStyleBOLDRight); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI, ColI + 3));
                ColI += 4;

                efm.PrintCell(RowI, ColI, SUMV_ALL, NumberStyleBold); ColI++;
                efm.PrintCell(RowI, ColI, SUM_MEK_ALL, NumberStyleBold); ColI++;
                efm.PrintCell(RowI, ColI, SUM_VOLUM_ALL, NumberStyleBold); ColI++;
                efm.PrintCell(RowI, ColI, SUM_MUR_ALL, NumberStyleBold); ColI++;
                efm.PrintCell(RowI, ColI, SUM_P_ALL, NumberStyleBold); ColI++;
                efm.PrintCell(RowI, ColI, SUM_MEK_P_ALL, NumberStyleBold); ColI++;
                efm.PrintCell(RowI, ColI, SUM_MUR_RETURN_ALL, NumberStyleBold); ColI++;

                RowI += 12;
                ColI = 1;

                var TextStyleNotBorder = efm.CreateType(new FontOpenXML() { HorizontalAlignment = HorizontalAlignmentV.Left, fontname = "Times New Roman", size = 22 }, null, null);
                var TextStyleButtomBorder = efm.CreateType(new FontOpenXML() { HorizontalAlignment = HorizontalAlignmentV.Left, fontname = "Times New Roman", size = 22 }, new BorderOpenXML { BottomBorder = BorderValues.Thin, LeftBorder = BorderValues.None, RightBorder = BorderValues.None, TopBorder = BorderValues.None }, null);
                
                efm.PrintCell(RowI, ColI,DOLG_RUK, TextStyleNotBorder); efm.AddMergedRegion(new CellRangeAddress(RowI,ColI, RowI,ColI+4)); ColI+=4;
                efm.PrintCell(RowI, ColI, "", TextStyleButtomBorder); ColI++;
                efm.PrintCell(RowI, ColI, "", TextStyleButtomBorder); ColI++;
                efm.PrintCell(RowI, ColI, "", TextStyleButtomBorder); ColI++;
                efm.PrintCell(RowI, ColI, "", TextStyleButtomBorder); ColI++;
                efm.PrintCell(RowI, ColI, FAM_RUK, TextStyleNotBorder); ColI++;
                

                
                efm.MarginMini();
                efm.FitToPage(1);
                efm.Save();
            }
        }

        public void VR_TO_XLS(string path, IEnumerable<LIMIT_RESULTRow> list)
        {
            using (var efm = new ExcelOpenXML(path, "Лист1"))
            {
                uint ColI = 1;
                uint RowI = 1;
                var mrow = efm.GetRow(RowI);
                mrow.Height = 50;
                var mrow2 = efm.GetRow(RowI + 1);
                var HeadStyle = efm.CreateType(new FontOpenXML() { Bold = true, HorizontalAlignment = HorizontalAlignmentV.Center, VerticalAlignment = VerticalAlignmentV.Center, wordwrap = true }, new BorderOpenXML(), null);
                var TextStyle = efm.CreateType(new FontOpenXML() { HorizontalAlignment = HorizontalAlignmentV.Left }, new BorderOpenXML(), null);
                var NumberStyle = efm.CreateType(new FontOpenXML() { HorizontalAlignment = HorizontalAlignmentV.Right, Format = (uint)DefaultNumFormat.F4 }, new BorderOpenXML(), null);

                efm.PrintCell(mrow, ColI, "СМО", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI + 1, ColI)); ColI++;
                efm.PrintCell(mrow, ColI, "Код МО", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI + 1, ColI)); ColI++;
                efm.PrintCell(mrow, ColI, "Наименование МО", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI + 1, ColI)); ColI++;
                efm.PrintCell(mrow, ColI, "Рубрика", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI + 1, ColI)); ColI++;
                efm.PrintCell(mrow, ColI, "Наименование", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI + 1, ColI)); ColI++;
                efm.PrintCell(mrow, ColI, "Предъявлено", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI, ColI + 1)); efm.PrintCell(mrow2, ColI, "Кол-во", HeadStyle); ColI++; efm.PrintCell(mrow2, ColI, "Сумма", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "Предъявлено(всего)", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI + 1, ColI)); ColI++;
                efm.PrintCell(mrow, ColI, "МЭК без учета контроля объемов", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI, ColI + 1)); efm.PrintCell(mrow2, ColI, "Кол-во", HeadStyle); ColI++; efm.PrintCell(mrow2, ColI, "Сумма", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "Принято без учета контроля объемов", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI, ColI + 1)); efm.PrintCell(mrow2, ColI, "Кол-во", HeadStyle); ColI++; efm.PrintCell(mrow2, ColI, "Сумма", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "Фондодержание", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI + 1, ColI)); ColI++;
                efm.PrintCell(mrow, ColI, "МУР", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI + 1, ColI)); ColI++;
                efm.PrintCell(mrow, ColI, "ФАП", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI + 1, ColI)); ColI++;
                efm.PrintCell(mrow, ColI, "Месячный лимит", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI, ColI + 1)); efm.PrintCell(mrow2, ColI, "Кол-во", HeadStyle); ColI++; efm.PrintCell(mrow2, ColI, "Сумма", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "Превышение объемов(количественные показатели)", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI, ColI + 1)); efm.PrintCell(mrow2, ColI, "Кол-во", HeadStyle); ColI++; efm.PrintCell(mrow2, ColI, "Сумма", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "Превышение объемов(количественные показатели)(в рамках рубрики)", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI, ColI + 1)); efm.PrintCell(mrow2, ColI, "Кол-во", HeadStyle); ColI++; efm.PrintCell(mrow2, ColI, "Сумма", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "Превышение объемов(стоимостные показатели)", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI, ColI + 1)); efm.PrintCell(mrow2, ColI, "Кол-во", HeadStyle); ColI++; efm.PrintCell(mrow2, ColI, "Сумма", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "Превышение объемов(стоимостные показатели)(в рамках рубрики)", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI, ColI + 1)); efm.PrintCell(mrow2, ColI, "Кол-во", HeadStyle); ColI++; efm.PrintCell(mrow2, ColI, "Сумма", HeadStyle); ColI++;

                efm.PrintCell(mrow, ColI, "Принято к оплате(реестры)", HeadStyle);
                efm.PrintCell(mrow2, ColI, "Кол-во", HeadStyle);
                efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI, ColI + 3)); ColI++;
                efm.PrintCell(mrow2, ColI, "%", HeadStyle); ColI++;
                efm.PrintCell(mrow2, ColI, "Сумма", HeadStyle); ColI++;
                efm.PrintCell(mrow2, ColI, "%", HeadStyle); ColI++;

                efm.PrintCell(mrow, ColI, "Принято к оплате(всего)", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI + 1, ColI)); ColI++;
                efm.PrintCell(mrow, ColI, "МЭК прошлых периодов", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI, ColI + 1)); efm.PrintCell(mrow2, ColI, "Кол-во", HeadStyle); ColI++; efm.PrintCell(mrow2, ColI, "Сумма", HeadStyle); ColI++;
                efm.PrintCell(mrow, ColI, "Возврат МУР", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI + 1, ColI)); ColI++;

                efm.PrintCell(mrow, ColI, "Наличие акта МЭК", HeadStyle); efm.AddMergedRegion(new CellRangeAddress(RowI, ColI, RowI + 1, ColI)); ColI++;
                RowI += 2;
                mrow = efm.GetRow(RowI);
                for (uint i = 1; i < ColI; i++)
                {
                    efm.PrintCell(mrow, i, i.ToString(), HeadStyle);
                    int width;
                    switch (i)
                    {
                        case 1:
                        case 2:
                            width = 8;
                            break;
                        case 3:
                            width = 37;
                            break;
                        case 4:
                            width = 9;
                            break;
                        case 5:
                            width = 62;
                            break;
                        default:
                            width = 14;
                            break;
                    }

                    efm.SetColumnWidth(i, width);
                }
                RowI++;
                foreach (var row in list)
                {
                    mrow = efm.GetRow(RowI);
                    RowI++;
                    ColI = 1;
                    efm.PrintCell(mrow, ColI, row.SMO, TextStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.CODE_MO, TextStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.NAM_MOK, TextStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.RUBRIC, TextStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.RUBRIC_NAME, TextStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.KOL, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.SUM, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.SUM_ALL, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.K_MEK_NOT_V, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.S_MEK_NOT_V, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.K_P_NOT_V, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.S_P_NOT_V, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.FOND, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.MUR, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.FAP, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.KOL_LIMIT, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.SUM_LIMIT, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.K_MEK_VK, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.S_MEK_VK, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.K_MEK_VK_RUB, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.S_MEK_VK_RUB, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.K_MEK_VS, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.S_MEK_VS, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.K_MEK_VS_RUB, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.S_MEK_VS_RUB, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.KOL_P, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.ProcKOL_P, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.SUM_P, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.ProcSUM_P, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.SUM_P_ALL, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.KOL_MEK_P, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.SUM_MEK_P, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.MUR_RETURN, NumberStyle); ColI++;
                    efm.PrintCell(mrow, ColI, row.IsACT_MEK ? "Да" : "Нет", TextStyle); ColI++;
                }
                efm.Save();
            }


        }

    }

    class PRIL5
    {
        public string CODE_MO { get; set; }
        public string NAME_MOK { get; set; }
        public List<PRIL5_Row> Rows { get; set; } = new List<PRIL5_Row>();
    }

    class PRIL5_Row
    {
        public int CODE { get; set; }
        public string NAME { get; set; }
        public decimal SUMV { get; set; }
        public decimal SUM_MEK { get; set; }
        public decimal SUM_VOLUM { get; set; }
        public decimal SUM_MUR { get; set; }
        public decimal SUM_P { get; set; }
        public decimal SUM_MEK_P { get; set; }
        public decimal SUM_MUR_RETURN { get; set; }

    }

}
