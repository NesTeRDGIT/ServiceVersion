using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.Office.Interop.Excel;
using ServiceLoaderMedpomData;
using ServiceLoaderMedpomData.EntityMP_V31;

namespace ClientServiceWPF.SANK_INVITER
{
  

    public interface ICheckFLK
    {
        List<ErrorProtocolXML> CheckFLK(FileItem item, ZL_LIST zl, int YEAR, int MONTH, bool FLAG_MEE, bool EXT_FLK, string SMO, List<F006Row> F006, List<F014Row> F014, List<ExpertRow> Experts, List<FindSluchItem> IDENT_INFO);
    }

    public class CheckerFLK : ICheckFLK
    {
        private IRepository repository;
        private Dispatcher dispatcher;
     

        public CheckerFLK(Dispatcher dispatcher, IRepository repository)
        {
            this.dispatcher = dispatcher;
            this.repository = repository;
        }

        public List<ErrorProtocolXML> CheckFLK(FileItem item, ZL_LIST zl, int YEAR, int MONTH, bool FLAG_MEE, bool EXT_FLK, string SMO, List<F006Row> F006, List<F014Row> F014, List<ExpertRow> Experts, List<FindSluchItem> IDENT_INFO)
        {
            item.InvokeComm("Обработка пакета: Проверка ФЛК(BASE)", dispatcher);
            var flk = CheckFLKbase(item, zl, YEAR, MONTH, FLAG_MEE, F006, F014, Experts);
            if (EXT_FLK)
            {
                item.InvokeComm("Обработка пакета: Проверка ФЛК(EXT)", dispatcher);
                if (IDENT_INFO == null)
                    IDENT_INFO = repository.Get_IdentInfo(zl, item, dispatcher);
                flk.AddRange(CheckFLKEx(item, zl, FLAG_MEE, SMO, IDENT_INFO));
            }
            return flk;
        }

        private List<ErrorProtocolXML> CheckFLKbase(FileItem fi, ZL_LIST zl, int YEAR, int MONTH,bool FLAG_MEE,  List<F006Row> F006, List<F014Row> F014, List<ExpertRow> Experts)
        {
            var ErrList = new List<ErrorProtocolXML>();
            try
            {
                var dtSelect = new DateTime(YEAR, MONTH, 1).AddMonths(1);
                var dtNow = DateTime.Now;
                var dtFile = new DateTime(Convert.ToInt32(zl.SCHET.YEAR), Convert.ToInt32(zl.SCHET.MONTH), 1).AddMonths(1);

                decimal SUMMAP_S = 0;
                decimal SUMMAV_S = 0;
                decimal SANK_MEE_S = 0;
                decimal SANK_EKMP_S = 0;
                decimal SANK_MEK_S = 0;


                if (YEAR >= 2019 && zl.ZGLV.VERSION != "3.1" && !FLAG_MEE)
                {
                    ErrList.Add(new ErrorProtocolXML
                    {
                        BAS_EL = "VERSION",
                        Comment = "Версия взаимодействия для реестров 2019 года не 3.1",
                        IM_POL = "VERSION",
                        OSHIB = 41
                    });
                }

                foreach (var zap in zl.ZAP)
                {
                    var N_ZAP = zap.N_ZAP.ToString();
                    foreach (var z_sl in zap.Z_SL_list)
                    {
                        SUMMAP_S += z_sl.SUMP ?? 0;
                        SUMMAV_S += z_sl.SUMV;
                        var sank_it = z_sl.SANK_IT ?? 0;
                        var oplata = z_sl.OPLATA ?? 0;
                        var sump = z_sl.SUMP ?? 0;

                        //Проверка сумм законченного случая
                        if (!FLAG_MEE)
                        {

                            if (z_sl.SUMV - sump - sank_it != 0)
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH",
                                    IDCASE = z_sl.IDCASE.ToString(),
                                    N_ZAP = N_ZAP,
                                    Comment = "Не соответствие SUMV-SUMP-SANK_IT = 0",
                                    IM_POL = "SANK_IT",
                                    OSHIB = 41
                                });
                            }

                            if (!(sank_it == 0 && oplata == 1 && z_sl.SANK.Count == 0 || sank_it == z_sl.SUMV && oplata == 2 || oplata == 3))
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH",
                                    IDCASE = z_sl.IDCASE.ToString(),
                                    N_ZAP = N_ZAP,
                                    Comment = "не верное заполнение OPLATA",
                                    IM_POL = "OPLATA",
                                    OSHIB = 41
                                });
                            }

                            if (z_sl.SANK.Sum(x => x.S_SUM) != sank_it)
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH",
                                    IDCASE = z_sl.IDCASE.ToString(),
                                    N_ZAP = N_ZAP,
                                    Comment = "Не соответствие SANK_IT сумме санкций",
                                    IM_POL = "SANK_IT",
                                    OSHIB = 41
                                });
                            }
                        }

                        //  var isCOVID = z_sl.SL.Any(x => x.DS1.In("U07.1", "U07.2"));
                        foreach (var san in z_sl.SANK)
                        {
                            var isMEK = san.S_TIP.IsMEK();
                            var IsMEE = san.S_TIP.IsMEE();
                            var IsEKMP = san.S_TIP.IsEKMP();

                            if (isMEK)
                                SANK_MEK_S += san.S_SUM;
                            if (IsMEE)
                                SANK_MEE_S += san.S_SUM;
                            if (IsEKMP)
                                SANK_EKMP_S += san.S_SUM;

                            var ce = san.CODE_EXP ?? new List<CODE_EXP>();
                            var ce_count = 0;
                            foreach (var exp in ce.Where(x => !string.IsNullOrEmpty(x.VALUE)))
                            {
                                
                                ce_count++;
                                if (Experts.Count(x => x.N_EXPERT == exp.VALUE) == 0)
                                {
                                    ErrList.Add(new ErrorProtocolXML
                                    {
                                        BAS_EL = "SLUCH",
                                        IDCASE = z_sl.IDCASE.ToString(),
                                        N_ZAP = N_ZAP,
                                        Comment = $"CODE_EXP = {exp.VALUE} не соответствует справочнику",
                                        IM_POL = "CODE_EXP",
                                        OSHIB = 41
                                    });
                                }
                            }

                            if (IsEKMP && ce_count == 0 && !san.S_OSN.IsNotDOC() && YEAR >= 2019)
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH",
                                    IDCASE = z_sl.IDCASE.ToString(),
                                    N_ZAP = N_ZAP,
                                    Comment = "Для санкций ЭКМП поле CODE_EXP обязательно к заполнению, кроме S_OSN=43,242",
                                    IM_POL = "CODE_EXP",
                                    OSHIB = 41
                                });
                            }


                            if (san.S_TIP.IsMultiDisp() && san.CODE_EXP.Count <= 1 && !san.S_OSN.IsNotDOC())
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH",
                                    IDCASE = z_sl.IDCASE.ToString(),
                                    N_ZAP = N_ZAP,
                                    Comment = "Для санкций мультидисциплинарных экспертиз количество CODE_EXP должно быть более 1, кроме S_OSN=43,242",
                                    IM_POL = "CODE_EXP",
                                    OSHIB = 41
                                });
                            }

                            if (F006.Count(x => x.IDVID == san.S_TIP && san.DATE_ACT >= x.DATEBEG && san.DATE_ACT <= (x.DATEEND ?? DateTime.Now)) == 0)
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH",
                                    IDCASE = z_sl.IDCASE.ToString(),
                                    N_ZAP = N_ZAP,
                                    Comment = $"Неверный тип санкции(F006) - {san.S_TIP}",
                                    IM_POL = "S_TIP",
                                    OSHIB = 41
                                });
                            }


                            if (F014.Count(x => x.KOD == san.S_OSN && san.DATE_ACT >= x.DATEBEG && san.DATE_ACT <= (x.DATEEND ?? DateTime.Now)) == 0 && !(san.S_OSN == 0 && san.S_SUM == 0 && FLAG_MEE))
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH",
                                    IDCASE = z_sl.IDCASE.ToString(),
                                    N_ZAP = N_ZAP,
                                    Comment = $"Неверный код отказа санкции(F014) - {san.S_OSN}",
                                    IM_POL = "S_OSN",
                                    OSHIB = 41
                                });
                            }

                            if (san.S_IST != 1)
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH",
                                    IDCASE = z_sl.IDCASE.ToString(),
                                    N_ZAP = N_ZAP,
                                    Comment = "Источник санкции не верный",
                                    IM_POL = "S_IST",
                                    OSHIB = 41
                                });
                            }

                            if (FLAG_MEE)
                            {
                                if (isMEK)
                                {
                                    ErrList.Add(new ErrorProtocolXML
                                    {
                                        BAS_EL = "SLUCH",
                                        IDCASE = z_sl.IDCASE.ToString(),
                                        N_ZAP = N_ZAP,
                                        Comment = "Наличие санкций МЭК",
                                        IM_POL = "S_TIP",
                                        OSHIB = 41
                                    });
                                }
                            }
                            else
                            {
                                if (!isMEK)
                                {
                                    ErrList.Add(new ErrorProtocolXML
                                    {
                                        BAS_EL = "SLUCH",
                                        IDCASE = z_sl.IDCASE.ToString(),
                                        N_ZAP = N_ZAP,
                                        Comment = "Наличие санкций МЭЭ\\ЭКМП",
                                        IM_POL = "S_TIP",
                                        OSHIB = 41
                                    });
                                }
                            }


                            if (isMEK && san.S_SUM == 0 && z_sl.SUMV != 0)
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH",
                                    IDCASE = z_sl.IDCASE.ToString(),
                                    N_ZAP = N_ZAP,
                                    Comment = "Санкции МЭК с нулевой стоимостью не допустимы",
                                    IM_POL = "S_TIP",
                                    OSHIB = 41
                                });
                            }

                            if (isMEK && !san.DATE_ACT.Between(dtSelect, dtNow))
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH",
                                    IDCASE = z_sl.IDCASE.ToString(),
                                    N_ZAP = N_ZAP,
                                    Comment = $"Некорректная дата акта = {san.DATE_ACT:dd-MM-yyyy} ожидается с {dtSelect:dd-MM-yyyy} по {dtNow:dd-MM-yyyy}",
                                    IM_POL = "DATE_ACT",
                                    OSHIB = 41
                                });
                            }

                            if (!isMEK && !san.DATE_ACT.Between(dtFile, dtNow))
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH",
                                    IDCASE = z_sl.IDCASE.ToString(),
                                    N_ZAP = N_ZAP,
                                    Comment = $"Некорректная дата акта = {san.DATE_ACT:dd-MM-yyyy} ожидается с {dtFile:dd-MM-yyyy} по {dtNow:dd-MM-yyyy}",
                                    IM_POL = "DATE_ACT",
                                    OSHIB = 41
                                });
                            }

                            if (san.S_TIP.IsOnlyTFOMS())
                            {
                                ErrList.Add(new ErrorProtocolXML
                                {
                                    BAS_EL = "SLUCH",
                                    IDCASE = z_sl.IDCASE.ToString(),
                                    N_ZAP = N_ZAP,
                                    Comment = "S_TIP{10,11,12,24,25,26,39,40,41} не предназначены для использования СМО",
                                    IM_POL = "S_OSN",
                                    OSHIB = 41
                                });
                            }

                        }

                        ErrList.AddRange(z_sl.SANK.GroupBy(x => new { x.S_OSN, x.S_SUM, x.S_TIP }).Where(x => x.Count() > 1).Select(san => new ErrorProtocolXML
                        {
                            BAS_EL = "SLUCH",
                            IDCASE = z_sl.IDCASE.ToString(),
                            N_ZAP = N_ZAP,
                            Comment = $"Дублирование санкции по полям(S_OSN,S_SUM,S_TIP) S_CODE = {string.Join(",", san.Select(x => x.S_CODE))}",
                            IM_POL = "SANK",
                            OSHIB = 41
                        }));


                        ErrList.AddRange(z_sl.SANK.Where(x => x.S_TIP.In(z_sl.SANK.Where(y => y.S_OSN == 0).Select(y => y.S_TIP).Distinct().ToArray()) && x.S_OSN != 0).Select(san => new ErrorProtocolXML
                        {
                            BAS_EL = "SLUCH",
                            IDCASE = z_sl.IDCASE.ToString(),
                            N_ZAP = N_ZAP,
                            Comment = $"Конфликт S_OSN = 0 и S_OSN!=0 для одного S_TIP для S_CODE = {san.S_CODE}",
                            IM_POL = "SANK",
                            OSHIB = 41
                        }));

                        if (z_sl.SANK.GroupBy(x => x.S_TIP.ToTypeExp()).Count(x => x.Count() > 1) != 0)
                            ErrList.Add(new ErrorProtocolXML
                            {
                                BAS_EL = "SLUCH",
                                IDCASE = z_sl.IDCASE.ToString(),
                                N_ZAP = N_ZAP,
                                Comment = $"Два и более снятия для случая",
                                IM_POL = "SANK",
                                OSHIB = 41
                            });

                        ErrList.AddRange(z_sl.SANK.GroupBy(x => new { x.S_TIP }).Where(x => x.Count() > 1).Select(san => new ErrorProtocolXML
                        {
                            BAS_EL = "SLUCH",
                            IDCASE = z_sl.IDCASE.ToString(),
                            N_ZAP = N_ZAP,
                            Comment = $"Дублирование санкции по полям(S_TIP) S_CODE = {string.Join(",", san.Select(x => x.S_CODE))}",
                            IM_POL = "SANK",
                            OSHIB = 41
                        }));


                        if (z_sl.SANK.Count == 0 && FLAG_MEE)
                        {
                            ErrList.Add(new ErrorProtocolXML
                            {
                                BAS_EL = "SLUCH",
                                IDCASE = z_sl.IDCASE.ToString(),
                                N_ZAP = N_ZAP,
                                Comment = "Отсутствует санкции МЭЭ\\ЭКМП",
                                IM_POL = "SANK",
                                OSHIB = 41
                            });
                        }



                        decimal SUMP_USL = 0;
                        decimal SUMP_SL = 0;

                        //Проверка случаев

                        foreach (var sl in z_sl.SL)
                        {
                            SUMP_SL += sl.SUM_MP ?? 0;
                            SUMP_USL += sl.USL.Sum(usl => usl.SUMP_USL ?? 0);
                        }

                        if (SUMP_USL != sump && !FLAG_MEE)
                        {
                            ErrList.Add(new ErrorProtocolXML
                            {
                                BAS_EL = "SLUCH",
                                IDCASE = z_sl.IDCASE.ToString(),
                                N_ZAP = N_ZAP,
                                Comment = "SUMP_USL!=sump",
                                IM_POL = "SUMP",
                                OSHIB = 41
                            });
                        }

                        if (SUMP_SL != sump && !FLAG_MEE)
                        {
                            ErrList.Add(new ErrorProtocolXML
                            {
                                BAS_EL = "SLUCH",
                                IDCASE = z_sl.IDCASE.ToString(),
                                N_ZAP = N_ZAP,
                                Comment = "SUMP_SL!=sump",
                                IM_POL = "SUMP",
                                OSHIB = 41
                            });
                        }

                    }
                }

                var SUMMAP = zl.SCHET.SUMMAP ?? 0;
                var SUMMAV = zl.SCHET.SUMMAV;
                var SANK_MEE = zl.SCHET.SANK_MEE ?? 0;
                var SANK_EKMP = zl.SCHET.SANK_EKMP ?? 0;
                var SANK_MEK = zl.SCHET.SANK_MEK ?? 0;


                if (SUMMAP != Math.Round(SUMMAP_S, 2))
                {
                    ErrList.Add(new ErrorProtocolXML
                    {
                        IM_POL = "SCHET",
                        BAS_EL = "SCHET",
                        Comment = $"Сумма принятая = {SUMMAP}, а файле {Math.Round(SUMMAP_S, 2)}",
                        OSHIB = 41
                    });
                }

                if (SUMMAV != Math.Round(SUMMAV_S, 2))
                {
                    ErrList.Add(new ErrorProtocolXML
                    {
                        IM_POL = "SCHET",
                        BAS_EL = "SCHET",
                        Comment = $"Сумма выставленная = {SUMMAV}, а файле {Math.Round(SUMMAV_S, 2)}",
                        OSHIB = 41
                    });
                }

                if (SANK_MEE != Math.Round(SANK_MEE_S, 2))
                {
                    ErrList.Add(new ErrorProtocolXML
                    {
                        IM_POL = "SCHET",
                        BAS_EL = "SCHET",
                        Comment = $"Сумма МЕЕ = {SANK_MEE}, а файле {Math.Round(SANK_MEE_S, 2)}",
                        OSHIB = 41
                    });
                }

                if (SANK_EKMP != Math.Round(SANK_EKMP_S, 2))
                {
                    ErrList.Add(new ErrorProtocolXML
                    {
                        IM_POL = "SCHET",
                        BAS_EL = "SCHET",
                        Comment = $"Сумма ЭКМП = {SANK_EKMP}, а файле {Math.Round(SANK_EKMP_S, 2)}",
                        OSHIB = 41
                    });
                }

                if (SANK_MEK != Math.Round(SANK_MEK_S, 2))
                {
                    ErrList.Add(new ErrorProtocolXML
                    {
                        IM_POL = "SCHET",
                        BAS_EL = "SCHET",
                        Comment = $"Сумма МЭК = {SANK_MEK}, а файле {Math.Round(SANK_MEK_S, 2)}",
                        OSHIB = 41
                    });
                }

                if (!FLAG_MEE && fi.DOP_REESTR == true)
                {
                    var t = repository.GetZGLV_BYFileName(zl.ZGLV.FILENAME);
                    if (t.Count != 0)
                    {
                        ErrList.Add(new ErrorProtocolXML
                        {
                            IM_POL = "SCHET",
                            BAS_EL = "SCHET",
                            Comment = $"Имя файла присутствует в БД: {zl.ZGLV.FILENAME}",
                            OSHIB = 41
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                ErrList.Add(new ErrorProtocolXML { Comment = $"Ошибка при проверке файла: {ex.StackTrace} {ex.Message}", OSHIB = 41 });
                fi.InvokeComm($"Ошибка при проверке файла: {ex.Message}", dispatcher);
            }
            return ErrList;
        }
        private List<ErrorProtocolXML> CheckFLKEx(FileItem fi, ZL_LIST zl, bool FLAG_MEE, string SMO, List<FindSluchItem> IDENT_INFO)
        {
            var ErrList = new List<ErrorProtocolXML>();
            try
            {
                if (fi.DOP_REESTR==true && !FLAG_MEE) return ErrList;
                if (IdentySluch(zl, fi, IDENT_INFO))
                {
                    fi.InvokeComm("Обработка пакета: Запрос санкций", dispatcher);
                    var SANK = repository.GetSank(zl, fi, dispatcher);


                    foreach (var zs_sl in zl.ZAP.SelectMany(x => x.Z_SL_list))
                    {
                        if (!zs_sl.SLUCH_Z_ID.HasValue)
                            throw new Exception($"Случай IDCASE={zs_sl.IDCASE} не найден в БД");

                        //var sank_sluch = SANK[zs_sl.SLUCH_Z_ID.Value].Where(x=>!(x.YEAR_SANK==2021 && x.MONTH_SANK ==4)).ToList();
                        var sank_sluch = SANK[zs_sl.SLUCH_Z_ID.Value];
                        var sank_main = sank_sluch.Where(x=>x.Type == TFindSANKItem.Main).ToList();
                        var sank_parent = sank_sluch.Where(x => x.Type == TFindSANKItem.Parent).ToList();
                        var sank_child = sank_sluch.Where(x => x.Type == TFindSANKItem.Child).ToList();
                        var sank_brother = sank_sluch.Where(x => x.Type == TFindSANKItem.Brother).ToList();

                        var isMEK = sank_main.Count(x => x.S_TIP.ToString().StartsWith("1")) != 0;

                        foreach (var sank in zs_sl.SANK)
                        {
                            var doubleSANK = sank_sluch.Where(x => x.S_SUM == sank.S_SUM && x.DATE_ACT == sank.DATE_ACT && x.NUM_ACT == sank.NUM_ACT && x.S_TIP == sank.S_TIP).ToList();
                            if (doubleSANK.Count != 0)
                            {
                                var error = $"Санкция была загружена ранее: {string.Join(Environment.NewLine, doubleSANK.Select(x => $"TYPE={x.Type},S_TIP={x.S_TIP}, S_SUM={x.S_SUM}, DATE_ACT={x.DATE_ACT:dd-MM-yyyy}, NUM_ACT={x.NUM_ACT} загружен {x.DATE_INVITE:dd-MM-yyyy} отчетный период {x.MONTH_SANK} {x.YEAR_SANK}"))}";
                                ErrList.Add(new ErrorProtocolXML { BAS_EL = "Z_SL", IDCASE = zs_sl.IDCASE.ToString(), IM_POL = "SANK", Comment = error, SLUCH_Z_ID = zs_sl.SLUCH_Z_ID.Value });
                            }
                            /*
                             Причина в незавершенных санкциях
                            if (sank.S_TIP.In(42,75) && zl.SCHET.YEAR >= 2021)
                            {
                                if (sank_sluch.Count(x => x.S_TIP.IsMEE() && x.S_OSN != 0) == 0 && zs_sl.SANK.Count(x => x.S_TIP.IsMEE() && x.S_OSN != 0) == 0)
                                {
                                    var error = $"Случай c экспертизой S_TIP={{42,75}} не содержит МЭЭ с дефектами";
                                    ErrList.Add(new ErrorProtocolXML { BAS_EL = "Z_SL", IDCASE = zs_sl.IDCASE.ToString(), IM_POL = "SANK", Comment = error, SLUCH_Z_ID = zs_sl.SLUCH_Z_ID.Value });
                                }
                            }*/
                            /*
                               Причина в незавершенных санкциях
                            if (sank.S_TIP.In(37,73) && zl.SCHET.YEAR >=2021)
                            {
                                if (sank_sluch.Count(x => x.S_TIP.IsMEE()) == 0 && zs_sl.SANK.Count(x => x.S_TIP.IsMEE()) == 0)
                                {
                                    var error = "Случай c экспертизой S_TIP={37,73} не содержит МЭЭ";
                                    ErrList.Add(new ErrorProtocolXML { BAS_EL = "Z_SL", IDCASE = zs_sl.IDCASE.ToString(), IM_POL = "SANK", Comment = error, SLUCH_Z_ID = zs_sl.SLUCH_Z_ID.Value });
                                }
                            }*/
                            if (sank.S_TIP.In(20, 21, 30, 31, 43, 44, 45, 46) && isMEK)
                            {
                                var error = "S_TIP{20, 21, 30, 31, 43, 44, 45, 46} не подлежит применению, если случай снят на МЭК";
                                ErrList.Add(new ErrorProtocolXML { BAS_EL = "Z_SL", IDCASE = zs_sl.IDCASE.ToString(), IM_POL = "SANK", Comment = error, SLUCH_Z_ID = zs_sl.SLUCH_Z_ID.Value });
                            }

                            if (sank.S_SUM != 0)
                            {
                                var DBerr = sank_sluch.Where(x => x.S_TIP == sank.S_TIP && x.NUM_ACT == sank.NUM_ACT && x.DATE_ACT == sank.DATE_ACT && x.S_SUM != 0).ToList();
                                var FileErr = zs_sl.SANK.Where(x => x.S_TIP == sank.S_TIP && x.NUM_ACT == sank.NUM_ACT && x.DATE_ACT == sank.DATE_ACT && x.S_SUM != 0 && x.S_CODE != sank.S_CODE).ToList();
                                if (DBerr.Count != 0 || FileErr.Count != 0)
                                {
                                    var error = $"Не допустимо 2 и более снятия на 1 экспертизе. Источник ошибки: {string.Join(Environment.NewLine, DBerr.Select(x => $"S_TIP={x.S_TIP}, S_OSN={x.S_OSN}, S_SUM=, NUM_ACT={x.NUM_ACT}, DATE_ACT{x.DATE_ACT:dd-MM-yyyy}"))}{(DBerr.Count != 0 ? Environment.NewLine : "")}{string.Join(Environment.NewLine, FileErr.Select(x => $"(ФАЙЛ)S_TIP={x.S_TIP}, S_OSN={x.S_OSN}, S_SUM=, NUM_ACT={x.NUM_ACT}, DATE_ACT{x.DATE_ACT:dd-MM-yyyy}"))}";
                                    ErrList.Add(new ErrorProtocolXML { BAS_EL = "Z_SL", IDCASE = zs_sl.IDCASE.ToString(), IM_POL = "SANK", Comment = error, SLUCH_Z_ID = zs_sl.SLUCH_Z_ID.Value });
                                }
                            }

                            /*
                            if (sank.S_TIP.Like("2", "3", "4") && sank.S_OSN != 0 && sank.DATE_ACT.HasValue)
                            {
                                var act = repository.FindACT(sank.NUM_ACT, sank.DATE_ACT.Value, SMO);
                                if (act.Count != 0)
                                {
                                    var error = $"Данный акт уже присутствует в БД: {string.Join(Environment.NewLine, act.Select(x => $"NUM_ACT = {x.NUM_ACT}, DATE_ACT = {x.DATE_ACT:dd-MM-yyyy}, дата загрузки {x.DATE_INVITE:dd-MM-yyyy}, имя файла = {x.FILENAME}"))}";
                                    //ErrList.Add(new ErrorProtocolXML { BAS_EL = "Z_SL", IDCASE = zs_sl.IDCASE.ToString(), IM_POL = "SANK", Comment = error });
                                }
                            }*/
                        }




                        /*var SUMV = zs_sl.SUMV;
                        var S_SUM = zs_sl.SANK.Where(x=>x.).Sum(x => x.S_SUM);
                        var S_SUM_BD = sank_BD.Sum(x => x.S_SUM);
                        var SUMP = SUMV - S_SUM - S_SUM_BD;
                        if (SUMP < 0 && S_SUM!=0)
                        {
                            var error = $"Снятие более суммы случая: сумма случая={SUMV}, сумма санкций в файле={S_SUM}, сумма санкций в базе={S_SUM_BD}, итоговая сумма(после вычета санкций)={SUMP}";
                            ErrList.Add(new ErrorProtocolXML { BAS_EL = "Z_SL", IDCASE = zs_sl.IDCASE.ToString(), IM_POL = "SANK", Comment = error });
                        }*/
                    }
                }
                else
                {
                    ErrList.Add(new ErrorProtocolXML { Comment = "Не полная идентификация случаев" });
                    fi.WriteLnFull("Не полная идентификация случаев");
                }
            }
            catch (Exception ex)
            {
                ErrList.Add(new ErrorProtocolXML { Comment = $"Ошибка при проверке файла: {ex.StackTrace} {ex.Message}" });
            }
            return ErrList;
        }
        private bool IdentySluch(ZL_LIST ZL, FileItem fi, List<FindSluchItem> IdentInfo = null)
        {
            try
            {
                if (IdentInfo == null)
                    IdentInfo = repository.Get_IdentInfo(ZL, fi, dispatcher);
                SetID(IdentInfo, ZL.GetHashTable());
                var t = ZL.ZAP.SelectMany(x => x.Z_SL_list).Where(x => x.SL.Count(y => !y.SLUCH_ID.HasValue) != 0).ToList();
                foreach (var row in t)
                {
                    foreach (var sl in row.SL)
                    {
                        fi.FileLog.WriteLn($"Не удалось найти случай IDCASE={row.IDCASE} SL_ID {sl.SL_ID}: {row.TagComment}{sl.TagComment}");
                    }
                }
                return !t.Any();
            }
            catch (Exception ex)
            {
                fi.FileLog.WriteLn($"Ошибка при идентификации случаев: {ex.StackTrace}{ex.Message}");
                fi.InvokeComm($"Ошибка при идентификации случаев: {ex.Message}", dispatcher);
                return false;
            }
        }
        private void SetID(List<FindSluchItem> IDCASEs, Dictionary<decimal, Z_SL> tab)
        {
            foreach (var IDCASE in IDCASEs)
            {
                var dr_ZSL = tab[IDCASE.IDCASE];

                var dr = dr_ZSL.SL.Where(x => x.SL_ID == IDCASE.SL_ID).ToList();
                var isErr = false;
                if (dr.Count > 1)
                {
                    dr_ZSL.TagComment = $"В случае IDCASE={IDCASE.IDCASE} и SL_ID={IDCASE.SL_ID} найдено более 1го случая. Доступны: {string.Join(",", dr_ZSL.SL.Select(x => x.SL_ID))}";
                    continue;
                }
                if (dr.Count == 0)
                {
                    dr_ZSL.TagComment = $"В случае IDCASE={IDCASE.IDCASE} и SL_ID={IDCASE.SL_ID} не найдено случая.  Доступны: {string.Join(",", dr_ZSL.SL.Select(x => x.SL_ID))}";
                    continue;
                }

                dr_ZSL.SLUCH_Z_ID = IDCASE.SLUCH_Z_ID;
                dr_ZSL.SLUCH_Z_ID_MAIN = IDCASE.SLUCH_Z_ID_MAIN;
                var dr_sl = dr[0];
                if (dr_sl.DATE_1 != IDCASE.DATE_1)
                {
                    isErr = true;
                    dr_sl.TagComment += $"В случае IDCASE={IDCASE.IDCASE} и SL_ID={IDCASE.SL_ID} DATE_1({dr_sl.DATE_1:MM.dd.yyyy}) не соответствует DATE_1({IDCASE.DATE_1:MM.dd.yyyy}) в БД";
                }
                if (dr_sl.DATE_2 != IDCASE.DATE_2)
                {
                    isErr = true;
                    dr_sl.TagComment += $"В случае IDCASE={IDCASE.IDCASE} и SL_ID={IDCASE.SL_ID} DATE_2({dr_sl.DATE_2:MM.dd.yyyy}) не соответствует DATE_2({IDCASE.DATE_2:MM.dd.yyyy}) в БД";
                }
                if (dr_sl.NHISTORY != IDCASE.NHISTORY)
                {
                    isErr = true;
                    dr_sl.TagComment += $"В случае IDCASE={IDCASE.IDCASE} и SL_ID={IDCASE.SL_ID} NHISTORY({dr_sl.NHISTORY}) не соответствует NHISTORY({IDCASE.NHISTORY}) в БД";
                }
                if (dr_sl.SUM_M != IDCASE.SUM_M)
                {
                    isErr = true;
                    dr_sl.TagComment += $"В случае IDCASE={IDCASE.IDCASE} и SL_ID={IDCASE.SL_ID} SUM_M({dr_sl.SUM_M}) не соответствует SUM_M({IDCASE.SUM_M}) в БД";
                }


                if (dr_sl.DS1 != IDCASE.DS1 && dr_sl.DS1 != null)
                {
                    isErr = true;
                    dr_sl.TagComment += $"В случае IDCASE={IDCASE.IDCASE} и SL_ID={IDCASE.SL_ID} DS1({dr_sl.DS1}) не соответствует DS1({IDCASE.DS1}) в БД";
                }
                if (dr_ZSL.RSLT != IDCASE.RSLT && dr_ZSL.RSLT.HasValue)
                {
                    isErr = true;
                    dr_sl.TagComment += $"В случае IDCASE={IDCASE.IDCASE} RSLT({dr_ZSL.RSLT}) не соответствует RSLT({IDCASE.RSLT}) в БД";
                }

                if (isErr) continue;

                dr_sl.SLUCH_ID = IDCASE.SLUCH_ID;
                dr_sl.SLUCH_Z_ID = IDCASE.SLUCH_Z_ID;
                foreach (var urow in dr_sl.USL)
                {
                    urow.SLUCH_ID = IDCASE.SLUCH_ID;
                }
                foreach (var urow in dr_ZSL.SANK)
                {
                    urow.SLUCH_Z_ID = IDCASE.SLUCH_Z_ID;
                }
            }
        }
    }


    public enum enumTypeEXP
    {
        MEK,
        MEE,
        EKMP
    }
    public static class Ext
    {
        /// <summary>
        /// Проверить находится ли значение в списке значений
        /// </summary>
        /// <param name="value">Значение</param>
        /// <param name="valuesArray">Список значений</param>
        /// <returns></returns>
        public static bool In(this decimal value, params decimal[] valuesArray)
        {
            return valuesArray.Contains(value);
        }
        public static bool In(this int value, params int[] valuesArray)
        {
            return valuesArray.Contains(value);
        }
        /// <summary>
        /// Проверить находится ли значение в списке значений
        /// </summary>
        /// <param name="value">Значение</param>
        /// <param name="valuesArray">Список значений</param>
        /// <returns></returns>
        public static bool In(this string value, params string[] valuesArray)
        {
            return valuesArray.Contains(value);
        }



        public static bool Like(this string val, params string[] vals)
        {
            return vals.Any(s => val.StartsWith(s));
        }

        public static bool Like(this decimal val, params string[] vals)
        {
            return Like(val.ToString(), vals);
        }


        public static bool Between(this DateTime? value, DateTime dt1, DateTime dt2)
        {
            if (!value.HasValue) return false;
            return value >= dt1 && value <= dt2;
        }


        public static enumTypeEXP ToTypeExp(this int val)
        {
            if (val.IsMEK())
                return enumTypeEXP.MEK;
            return val.IsMEE() ? enumTypeEXP.MEE : enumTypeEXP.EKMP;
        }

        public static bool IsMEK(this int val)
        {
            return val.ToString().StartsWith("1");
        }
        public static bool IsMEE(this int val)
        {
            return val.ToString().StartsWith("2") || val.ToString().StartsWith("5");
        }
        public static bool IsEKMP(this int val)
        {
            return val.ToString().StartsWith("3") || val.ToString().StartsWith("4") || val.ToString().StartsWith("7") || val.ToString().StartsWith("8"); 
        }
        public static bool IsMultiDisp(this int val)
        {
            return val.In(43, 44, 45, 46, 47, 48, 49, 79, 80, 81, 82, 83, 84);
        }

        public static bool IsNotDOC(this decimal val)
        {
            return val.In(43, 242);
        }


        public static bool IsOnlyTFOMS(this int val)
        {
            return val.In(10, 11, 12, 24, 25, 26, 39, 40, 41,59,86);
        }
    }
}
