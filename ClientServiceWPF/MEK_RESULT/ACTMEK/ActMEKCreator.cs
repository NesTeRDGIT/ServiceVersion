using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Threading;
using ClientServiceWPF.Class;
using ExcelManager;

namespace ClientServiceWPF.MEK_RESULT.ACTMEK
{
    public class MO_INFO
    {
        public MO_ITEM MO_ITEM { get; set; }
        public MO_FOND_INFO FOND { get; set; }
        public MEK_PARAM PARAM { get; set; }
        public List<MP_DEFECT_ITEM> DEFECT { get; set; }

        public MO_INFO(MO_ITEM MO_ITEM, MO_FOND_INFO FOND, MEK_PARAM PARAM, List<MP_DEFECT_ITEM> DEFECT)
        {
            this.MO_ITEM = MO_ITEM;
            this.FOND = FOND;
            this.PARAM = PARAM;
            this.DEFECT = DEFECT;
        }
    }
    public class SVOD_MEK
    {
        List<MO_INFO> SVOD_LIST { get; set; } = new List<MO_INFO>();

        public void Add(MO_INFO item)
        {
            SVOD_LIST.Add(item);
        }

        public MO_FOND_INFO FOND => SVOD_LIST.Select(x => x.FOND).Aggregate((x1, x2) => x1 + x2);

        public MEK_PARAM PARAM => SVOD_LIST.Select(x => x.PARAM).Aggregate((x1, x2) => x1 + x2);

        public List<MP_DEFECT_ITEM> DEFECT => SVOD_LIST.SelectMany(x => x.DEFECT).ToList();

        public MO_ITEM MO_ITEM => new MO_ITEM()
        {
            CODE_MO = "000000",
            D_ACT = DateTime.Now,
            DATE_INVITE = DateTime.Now,
            IsSelect = true,
            MONTH = SVOD_LIST.First().MO_ITEM.MONTH,
            YEAR = SVOD_LIST.First().MO_ITEM.YEAR,
            N_ACT = $"На {SVOD_LIST.Select(x => x.MO_ITEM).Count()} актов",
            NAME_MOK = "Сводный акт"
        };
    }
    public class MEK_PARAM
    {
        public MEK_ITEM ALL_PRED => NAPR + MP_NOT_VR + OTHER + SCOR + FOND + FAP;
        public MEK_ITEM POS_FOND { get; set; } = new MEK_ITEM();
        public MEK_ITEM OBR_FOND { get; set; } = new MEK_ITEM();
        public MEK_ITEM PROF_DET_FOND { get; set; } = new MEK_ITEM();
        public MEK_ITEM PROF_VZR_FOND { get; set; } = new MEK_ITEM();
        public MEK_ITEM DISP_DET_FOND { get; set; } = new MEK_ITEM();
        public MEK_ITEM DISP_VZR_FOND { get; set; } = new MEK_ITEM();

        public MEK_ITEM FOND => OBR_FOND + POS_FOND + PROF_DET_FOND + PROF_VZR_FOND + DISP_DET_FOND + DISP_VZR_FOND;
        public MEK_ITEM POS_FAP { get; set; } = new MEK_ITEM();
        public MEK_ITEM OBR_FAP { get; set; } = new MEK_ITEM();
        public MEK_ITEM FAP => POS_FAP + OBR_FAP;

        public MEK_ITEM FULL_SCOR => SCOR + TROMB;
        public MEK_ITEM SCOR { get; set; } = new MEK_ITEM();
        public MEK_ITEM POS_NAPR { get; set; } = new MEK_ITEM();
        public MEK_ITEM OBR_NAPR { get; set; } = new MEK_ITEM();
        public MEK_ITEM NAPR => OBR_NAPR + POS_NAPR;
        public MEK_ITEM POS_AMB { get; set; } = new MEK_ITEM();
        public MEK_ITEM OBR_AMB { get; set; } = new MEK_ITEM();
        public MEK_ITEM AMB => POS_AMB + OBR_AMB;

        public MEK_ITEM MP_NOT_VR => DISP2 + DISP_VZR + DISP_DET + PROF_DET + PROF_VZR + VMP + DSTAC + DIAL_STAC + STAC + NEOT + KORON + MOL + GIST + ENDO + USI + MRT + KT + DIAL_AMB + AMB + TROMB + KORON_1213+ UG_DISP2+ UG_DISP1;

        public MEK_ITEM TROMB { get; set; } = new MEK_ITEM();
        public MEK_ITEM DIAL_AMB { get; set; } = new MEK_ITEM();
        public MEK_ITEM KT { get; set; } = new MEK_ITEM();
        public MEK_ITEM MRT { get; set; } = new MEK_ITEM();
        public MEK_ITEM USI { get; set; } = new MEK_ITEM();
        public MEK_ITEM ENDO { get; set; } = new MEK_ITEM();
        public MEK_ITEM GIST { get; set; } = new MEK_ITEM();
        public MEK_ITEM MOL { get; set; } = new MEK_ITEM();
        public MEK_ITEM KORON { get; set; } = new MEK_ITEM();
        public MEK_ITEM KORON_1213 { get; set; } = new MEK_ITEM();
        public MEK_ITEM NEOT { get; set; } = new MEK_ITEM();
        public MEK_ITEM STAC { get; set; } = new MEK_ITEM();
        public MEK_ITEM STAC_1213 { get; set; } = new MEK_ITEM();
        public MEK_ITEM FULL_STAC => STAC + VMP + DIAL_STAC + STAC_1213;
        public MEK_ITEM DIAL_STAC { get; set; } = new MEK_ITEM();

        public MEK_ITEM DSTAC => DSTAC_WITHOUT_EKO + EKO;

        public MEK_ITEM DSTAC_WITHOUT_EKO { get; set; } = new MEK_ITEM();
        public MEK_ITEM EKO { get; set; } = new MEK_ITEM();
        public MEK_ITEM VMP { get; set; } = new MEK_ITEM();
        public MEK_ITEM PROF_DET { get; set; } = new MEK_ITEM();
        public MEK_ITEM PROF_VZR { get; set; } = new MEK_ITEM();
        public MEK_ITEM DISP_DET { get; set; } = new MEK_ITEM();
        public MEK_ITEM DISP_VZR { get; set; } = new MEK_ITEM();
        public MEK_ITEM DISP2 { get; set; } = new MEK_ITEM();

        public MEK_ITEM UG_DISP1 { get; set; } = new MEK_ITEM();
        public MEK_ITEM UG_DISP2 { get; set; } = new MEK_ITEM();

        public MEK_ITEM OTHER { get; set; } = new MEK_ITEM();


        public MEK_ITEM FULL_AMB => AMB + FOND + NAPR + FAP + PROF_DET + PROF_VZR + DISP_VZR + DISP_DET + DISP2 + OTHER + DIAL_AMB + KT + MRT + USI + ENDO + GIST + MOL + KORON + NEOT + KORON_1213+ UG_DISP1+ UG_DISP2;

        public decimal NAPR_FROM_MO { get; set; }


        public static MEK_PARAM operator +(MEK_PARAM x1, MEK_PARAM x2)
        {
            return new MEK_PARAM()
            {
                POS_FOND = x1.POS_FOND + x2.POS_FOND,
                OBR_FOND = x1.OBR_FOND + x2.OBR_FOND,
                POS_FAP = x1.POS_FAP + x2.POS_FAP,
                OBR_FAP = x1.OBR_FAP + x2.OBR_FAP,
                SCOR = x1.SCOR + x2.SCOR,
                POS_NAPR = x1.POS_NAPR + x2.POS_NAPR,
                OBR_NAPR = x1.OBR_NAPR + x2.OBR_NAPR,
                POS_AMB = x1.POS_AMB + x2.POS_AMB,
                OBR_AMB = x1.OBR_AMB + x2.OBR_AMB,
                TROMB = x1.TROMB + x2.TROMB,
                DIAL_AMB = x1.DIAL_AMB + x2.DIAL_AMB,
                KT = x1.KT + x2.KT,
                MRT = x1.MRT + x2.MRT,
                USI = x1.USI + x2.USI,
                ENDO = x1.ENDO + x2.ENDO,
                GIST = x1.GIST + x2.GIST,
                MOL = x1.MOL + x2.MOL,
                KORON = x1.KORON + x2.KORON,
                NEOT = x1.NEOT + x2.NEOT,
                STAC = x1.STAC + x2.STAC,
                DIAL_STAC = x1.DIAL_STAC + x2.DIAL_STAC,
                DSTAC_WITHOUT_EKO = x1.DSTAC_WITHOUT_EKO + x2.DSTAC_WITHOUT_EKO,
                EKO = x1.EKO + x2.EKO,
                VMP = x1.VMP + x2.VMP,
                PROF_DET = x1.PROF_DET + x2.PROF_DET,
                PROF_VZR = x1.PROF_VZR + x2.PROF_VZR,
                DISP_DET = x1.DISP_DET + x2.DISP_DET,
                DISP_VZR = x1.DISP_VZR + x2.DISP_VZR,
                UG_DISP2 = x1.UG_DISP2+x2.UG_DISP2,
                UG_DISP1 = x2.UG_DISP1 +x2.UG_DISP1,
                DISP2 = x1.DISP2 + x2.DISP2,
                OTHER = x1.OTHER + x2.OTHER,
                STAC_1213 = x1.STAC_1213 + x2.STAC_1213,
                NAPR_FROM_MO = x1.NAPR_FROM_MO + x2.NAPR_FROM_MO,
                KORON_1213 = x1.KORON_1213 + x2.KORON_1213,
                PROF_DET_FOND = x1.PROF_DET_FOND + x2.PROF_DET_FOND,
                PROF_VZR_FOND = x1.PROF_VZR_FOND + x2.PROF_VZR_FOND,
                DISP_DET_FOND = x1.DISP_DET_FOND + x2.DISP_DET_FOND,
                DISP_VZR_FOND = x1.DISP_VZR_FOND + x2.DISP_VZR_FOND
            };
        }
    }
    public class MEK_PROFIL
    {
        public int? PROFIL { get; set; }
        public string NAME { get; set; }
    }
    public class MEK_ITEM
    {
        public MEK_ITEM()
        {
        }


        public Dictionary<MEK_PROFIL, MEK_ITEM_VALUE> MEK_PROFIL { get; set; } = new Dictionary<MEK_PROFIL, MEK_ITEM_VALUE>();

        public decimal KOL => MEK_PROFIL.Values.Sum(x => x.KOL);
        public decimal SUM => MEK_PROFIL.Values.Sum(x => x.SUM);
        public decimal KOL_P => MEK_PROFIL.Values.Sum(x => x.KOL_P);
        public decimal SUM_P => MEK_PROFIL.Values.Sum(x => x.SUM_P);
        public decimal KOL_MEK => MEK_PROFIL.Values.Sum(x => x.KOL_MEK);
        public decimal SUM_MEK => MEK_PROFIL.Values.Sum(x => x.SUM_MEK);

        public void Add(MP_VOLUME_ITEM item)
        {
            FindMEK_ITEM_VALUE(item).Add(item);
        }

        private MEK_ITEM_VALUE FindMEK_ITEM_VALUE(MP_VOLUME_ITEM MVI)
        {
            var key = MEK_PROFIL.Keys.FirstOrDefault(x => x.PROFIL == MVI.PROFIL);
            if (key == null)
            {
                key = new MEK_PROFIL { NAME = MVI.PROFIL_NAME, PROFIL = MVI.PROFIL };
                MEK_PROFIL.Add(key, new MEK_ITEM_VALUE());
            }

            return MEK_PROFIL[key];
        }




        public void AddSUM(MP_VOLUME_ITEM item)
        {
            FindMEK_ITEM_VALUE(item).AddSUM(item);
        }



        public static MEK_ITEM operator +(MEK_ITEM A, MEK_ITEM B)
        {
            var newItem = new MEK_ITEM();
            foreach (var mp in A.MEK_PROFIL)
            {
                newItem.MEK_PROFIL.Add(mp.Key, mp.Value.Copy());
            }

            foreach (var mp in B.MEK_PROFIL)
            {
                var key = newItem.MEK_PROFIL.Keys.FirstOrDefault(x => x.PROFIL == mp.Key.PROFIL);
                if (key == null)
                {
                    key = new MEK_PROFIL { NAME = mp.Key.NAME, PROFIL = mp.Key.PROFIL };
                    newItem.MEK_PROFIL.Add(key, mp.Value.Copy());
                }
                else
                {
                    newItem.MEK_PROFIL[key].Add(mp.Value);
                }
            }

            return newItem;
        }

        public void AddKOL(MP_VOLUME_ITEM item)
        {
            FindMEK_ITEM_VALUE(item).AddKOL(item);
        }
    }
    public class MEK_ITEM_VALUE
    {
        public decimal KOL { get; set; }
        public decimal SUM { get; set; }
        public decimal KOL_P { get; set; }
        public decimal SUM_P { get; set; }
        public decimal KOL_MEK { get; set; }
        public decimal SUM_MEK { get; set; }

        public void Add(MP_VOLUME_ITEM item)
        {
            KOL += item.KOL;
            SUM += item.SUM;
            KOL_P += item.KOL_P;
            SUM_P += item.SUM_P;
            KOL_MEK += item.KOL_MEK;
            SUM_MEK += item.SUM_MEK;
        }

        public void Add(MEK_ITEM_VALUE item)
        {
            KOL += item.KOL;
            SUM += item.SUM;
            KOL_P += item.KOL_P;
            SUM_P += item.SUM_P;
            KOL_MEK += item.KOL_MEK;
            SUM_MEK += item.SUM_MEK;
        }

        public void AddSUM(MP_VOLUME_ITEM item)
        {
            SUM += item.SUM;
            SUM_P += item.SUM_P;
            SUM_MEK += item.SUM_MEK;
        }

        public MEK_ITEM_VALUE Copy()
        {
            return new MEK_ITEM_VALUE()
            {
                KOL = KOL,
                SUM = SUM,
                KOL_MEK = KOL_MEK,
                SUM_MEK = SUM_MEK,
                KOL_P = KOL_P,
                SUM_P = SUM_P
            };
        }

        public void AddKOL(MP_VOLUME_ITEM item)
        {
            KOL += item.KOL;
            KOL_P += item.KOL_P;
            KOL_MEK += item.KOL_MEK;
        }
    }
    public class ActMEKCreator
    {
        private string ACT_MEK_TEMPLATE { get; set; }
        private ProgressItem progress { get; set; }
        private Dispatcher dispatcher { get; set; }
        public ActMEKCreator(string ACT_MEK_TEMPLATE, ProgressItem progress, Dispatcher dispatcher)
        {
            this.ACT_MEK_TEMPLATE = ACT_MEK_TEMPLATE;
            this.progress = progress;
            this.dispatcher = dispatcher;
        }

        public void CreateActMEK(MO_ITEM item, string ACT_MEK_PATH,  MO_FOND_INFO FOND_INFO, List<MP_DEFECT_ITEM> DEFECT, MEK_PARAM par, PODPISANT ISP,PODPISANT RUK)
        {
            ExcelOpenXML efm = null;
            try
            {
                File.Copy(ACT_MEK_TEMPLATE, ACT_MEK_PATH, true);
                efm = new ExcelOpenXML();
                efm.OpenFile(ACT_MEK_PATH, 0);
                dispatcher.Invoke(() => { progress.Text = "Создание файла:(Акт МЭК)"; });
                CreateAct(efm, item, FOND_INFO, par, DEFECT,ISP,RUK);
                dispatcher.Invoke(() => { progress.Text = "Создание файла:(Раздел 1)"; });
                CreateRazdel1(efm, item, FOND_INFO, par, DEFECT, ISP, RUK);
                dispatcher.Invoke(() => { progress.Text = "Создание файла:(Раздел 2)"; });
                CreateRazdel2(efm, item, FOND_INFO, par,  ISP, RUK);
                dispatcher.Invoke(() => { progress.Text = "Создание файла:(Реестр актов)"; });
                CreateReestrAct(efm, item, FOND_INFO, par, DEFECT, ISP);
                efm.MarkAsFinal(true);
                efm.Save();
            }
            finally
            {
                efm?.Dispose();
            }
        }

        public void CreateActMEK_SHORT(MO_ITEM item, string ACT_MEK_PATH, MO_FOND_INFO FOND_INFO, List<MP_DEFECT_ITEM> DEFECT, MEK_PARAM par, PODPISANT ISP, PODPISANT RUK)
        {
            ExcelOpenXML efm = null;
            try
            {
                File.Copy(ACT_MEK_TEMPLATE, ACT_MEK_PATH, true);
                efm = new ExcelOpenXML();
                efm.OpenFile(ACT_MEK_PATH, 0);
                dispatcher.Invoke(() => { progress.Text = "Создание файла:(Акт МЭК)"; });
                CreateAct(efm, item, FOND_INFO, par, DEFECT, ISP, RUK);
                efm.MarkAsFinal(true);
                efm.Save();
            }
            finally
            {
                efm?.Dispose();
            }
        }
        private void CreateAct(ExcelOpenXML efm, MO_ITEM item, MO_FOND_INFO FOND_INFO, MEK_PARAM par, List<MP_DEFECT_ITEM> DEFECT, PODPISANT ISP, PODPISANT RUK)
        {
            efm.PrintCell(2, 1, $"№ {item.N_ACT} от {item.D_ACT:dd.MM.yyyy}", null);
            efm.PrintCell(7, 1, item.NAME_MOK, null);
            efm.PrintCell(9, 14, item.NAME_SMOK, null);
            efm.PrintCell(11, 6, (new DateTime(item.YEAR, item.MONTH, 1)).ToString("MMMMMMMMMMMM yyyy"), null);



            uint rowindex = 15;
            //Предъявлено по итоговому реестру на сумму:
            if (PrintVolume(efm, rowindex, par.ALL_PRED.KOL, FOND_INFO.AMB_S + FOND_INFO.FAP_S + FOND_INFO.SCOR_S + par.ALL_PRED.SUM, par.ALL_PRED.KOL_MEK, par.ALL_PRED.SUM_MEK + par.NAPR_FROM_MO, par.ALL_PRED.KOL_P, FOND_INFO.AMB_S + FOND_INFO.FAP_S + FOND_INFO.SCOR_S + par.ALL_PRED.SUM_P - par.NAPR_FROM_MO))
                rowindex++;

            //Медицинская помощь, оказанная прикрепленным гражданам:
            if (PrintVolume(efm, rowindex, null, FOND_INFO.AMB_S + FOND_INFO.FAP_S + FOND_INFO.SCOR_S, null, par.NAPR_FROM_MO, null, FOND_INFO.AMB_S + FOND_INFO.FAP_S + FOND_INFO.SCOR_S - par.NAPR_FROM_MO))
                rowindex++;

            //Амбулаторная помощь по дифференцированному подушевому нормативу
            if (PrintVolume(efm, rowindex, null, FOND_INFO.AMB_S, null, par.NAPR_FROM_MO, null, FOND_INFO.AMB_S - par.NAPR_FROM_MO))
                rowindex++;
            //Прикрепленно
            if (FOND_INFO.AMB_K != 0)
            {
                efm.PrintCell(rowindex, 8, FOND_INFO.AMB_K, null);
                rowindex++;
            }
            else
            {
                efm.RemoveRow(rowindex);
            }



            //Дифференцированный подушевой норматив по амбулаторно-поликлинической помощи(руб/чел):
            if (FOND_INFO.AMB_STANDART != 0)
            {
                efm.PrintCell(rowindex, 19, FOND_INFO.AMB_STANDART, null);
                rowindex++;
            }
            else
            {
                efm.RemoveRow(rowindex);
            }
            //посещений с профилактическими и иными целями
            if (PrintVolume(efm, rowindex, par.POS_FOND.KOL, null, par.POS_FOND.KOL_MEK, null, par.POS_FOND.KOL_P, null))
                rowindex++;
            //обращений по поводу заболевания
            if (PrintVolume(efm, rowindex, par.OBR_FOND.KOL, null, par.OBR_FOND.KOL_MEK, null, par.OBR_FOND.KOL_P, null))
                rowindex++;
            //профилактические осмотры несовершеннолетних 1 этап
            if (PrintVolume(efm, rowindex, par.PROF_DET_FOND.KOL, null, par.PROF_DET_FOND.KOL_MEK, null, par.PROF_DET_FOND.KOL_P, null))
                rowindex++;
            //профилактические осмотры взрослого населения 1 этап
            if (PrintVolume(efm, rowindex, par.PROF_VZR_FOND.KOL, null, par.PROF_VZR_FOND.KOL_MEK, null, par.PROF_VZR_FOND.KOL_P, null))
                rowindex++;
            //диспансеризация детского населения 1 этап
            if (PrintVolume(efm, rowindex, par.DISP_DET_FOND.KOL, null, par.DISP_DET_FOND.KOL_MEK, null, par.DISP_DET_FOND.KOL_P, null))
                rowindex++;
            //диспансеризация взрослого населения 1 этап
            if (PrintVolume(efm, rowindex, par.DISP_VZR_FOND.KOL, null, par.DISP_VZR_FOND.KOL_MEK, null, par.DISP_VZR_FOND.KOL_P, null))
                rowindex++;

            //Медицинская помощь, оказанная прикрепленным гражданам в других МО
            if (PrintVolume(efm, rowindex, null, null, null, par.NAPR_FROM_MO, null, null))
                rowindex++;


            //Финансовое обеспечение ФП/ФАП: 
            if (PrintVolume(efm, rowindex, null, FOND_INFO.FAP_S, null, 0, null, FOND_INFO.FAP_S))
                rowindex++;
            //посещений с профилактическими и иными целями
            if (PrintVolume(efm, rowindex, par.POS_FAP.KOL, null, par.POS_FAP.KOL_MEK, null, par.POS_FAP.KOL_P, null))
                rowindex++;
            //обращений по поводу заболевания
            if (PrintVolume(efm, rowindex, par.OBR_FAP.KOL, null, par.OBR_FAP.KOL_MEK, null, par.OBR_FAP.KOL_P, null))
                rowindex++;



            //Скорая помощь по дифференцированному подушевому нормативу: 
            if (PrintVolume(efm, rowindex, null, FOND_INFO.SCOR_S, null, 0, null, FOND_INFO.SCOR_S))
                rowindex++;
            //Прикрепленно:
            if (FOND_INFO.SCOR_K != 0)
            {
                efm.PrintCell(rowindex, 8, FOND_INFO.SCOR_K, null);
                rowindex++;
            }
            else
            {
                efm.RemoveRow(rowindex);
            }
            //Дифференцированный подушевой норматив по скорой помощи(руб/чел):
            if (FOND_INFO.SCOR_STANDART != 0)
            {
                efm.PrintCell(rowindex, 19, FOND_INFO.SCOR_STANDART, null);
                rowindex++;
            }
            else
            {
                efm.RemoveRow(rowindex);
            }
            //вызовов
            if (PrintVolume(efm, rowindex, par.SCOR.KOL, null, par.SCOR.KOL_MEK, null, par.SCOR.KOL_P, null))
                rowindex++;
            //Медицинская помощь, оказанная неприкрепленным гражданам по направлениям других МО или без направлений, оплачиваемые в бесспорном порядке (внешние услуги):
            //Амбулаторная помощь:
            if (PrintVolume(efm, rowindex, par.NAPR.KOL, par.NAPR.SUM, par.NAPR.KOL_MEK, par.NAPR.SUM_MEK, par.NAPR.KOL_P, par.NAPR.SUM_P))
                rowindex++;
            if (PrintVolume(efm, rowindex, par.NAPR.KOL, par.NAPR.SUM, par.NAPR.KOL_MEK, par.NAPR.SUM_MEK, par.NAPR.KOL_P, par.NAPR.SUM_P))
                rowindex++;
            //посещений с профилактическими и иными целями
            if (PrintVolume(efm, rowindex, par.POS_NAPR.KOL, par.POS_NAPR.SUM, par.POS_NAPR.KOL_MEK, par.POS_NAPR.SUM_MEK, par.POS_NAPR.KOL_P, par.POS_NAPR.SUM_P))
                rowindex++;
            //обращений по поводу заболевания
            if (PrintVolume(efm, rowindex, par.OBR_NAPR.KOL, par.OBR_NAPR.SUM, par.OBR_NAPR.KOL_MEK, par.OBR_NAPR.SUM_MEK, par.OBR_NAPR.KOL_P, par.OBR_NAPR.SUM_P))
                rowindex++;


            //Медицинская помощь, не включенная в систему взаиморасчетов
            if (PrintVolume(efm, rowindex, par.MP_NOT_VR.KOL, par.MP_NOT_VR.SUM, par.MP_NOT_VR.KOL_MEK, par.MP_NOT_VR.SUM_MEK, par.MP_NOT_VR.KOL_P, par.MP_NOT_VR.SUM_P))
                rowindex++;

            //Амбулаторная помощь:
            if (PrintVolume(efm, rowindex, par.AMB.KOL, par.AMB.SUM, par.AMB.KOL_MEK, par.AMB.SUM_MEK, par.AMB.KOL_P, par.AMB.SUM_P))
                rowindex++;
            //посещений с профилактическими и иными целями
            if (PrintVolume(efm, rowindex, par.POS_AMB.KOL, par.POS_AMB.SUM, par.POS_AMB.KOL_MEK, par.POS_AMB.SUM_MEK, par.POS_AMB.KOL_P, par.POS_AMB.SUM_P))
                rowindex++;
            //обращений по поводу заболевания
            if (PrintVolume(efm, rowindex, par.OBR_AMB.KOL, par.OBR_AMB.SUM, par.OBR_AMB.KOL_MEK, par.OBR_AMB.SUM_MEK, par.OBR_AMB.KOL_P, par.OBR_AMB.SUM_P))
                rowindex++;

            //Услуги диализа
            if (PrintVolume(efm, rowindex, par.DIAL_AMB.KOL, par.DIAL_AMB.SUM, par.DIAL_AMB.KOL_MEK, par.DIAL_AMB.SUM_MEK, par.DIAL_AMB.KOL_P, par.DIAL_AMB.SUM_P))
                rowindex++;
            //Компьютерная томография
            if (PrintVolume(efm, rowindex, par.KT.KOL, par.KT.SUM, par.KT.KOL_MEK, par.KT.SUM_MEK, par.KT.KOL_P, par.KT.SUM_P))
                rowindex++;
            //Магнитно-резонансная томография
            if (PrintVolume(efm, rowindex, par.MRT.KOL, par.MRT.SUM, par.MRT.KOL_MEK, par.MRT.SUM_MEK, par.MRT.KOL_P, par.MRT.SUM_P))
                rowindex++;
            //УЗИ сердечно сосудистой системы
            if (PrintVolume(efm, rowindex, par.USI.KOL, par.USI.SUM, par.USI.KOL_MEK, par.USI.SUM_MEK, par.USI.KOL_P, par.USI.SUM_P))
                rowindex++;
            //Эндоскопические диагностические исследования
            if (PrintVolume(efm, rowindex, par.ENDO.KOL, par.ENDO.SUM, par.ENDO.KOL_MEK, par.ENDO.SUM_MEK, par.ENDO.KOL_P, par.ENDO.SUM_P))
                rowindex++;
            //Гистологические исследования
            if (PrintVolume(efm, rowindex, par.GIST.KOL, par.GIST.SUM, par.GIST.KOL_MEK, par.GIST.SUM_MEK, par.GIST.KOL_P, par.GIST.SUM_P))
                rowindex++;
            //Молекулярно-диагностические исследования
            if (PrintVolume(efm, rowindex, par.MOL.KOL, par.MOL.SUM, par.MOL.KOL_MEK, par.MOL.SUM_MEK, par.MOL.KOL_P, par.MOL.SUM_P))
                rowindex++;
            //Определение РНК коронавирусов
            if (PrintVolume(efm, rowindex, par.KORON.KOL, par.KORON.SUM, par.KORON.KOL_MEK, par.KORON.SUM_MEK, par.KORON.KOL_P, par.KORON.SUM_P))
                rowindex++;
            //Определение РНК коронавирусов(1213)
            if (PrintVolume(efm, rowindex, par.KORON_1213.KOL, par.KORON_1213.SUM, par.KORON_1213.KOL_MEK, par.KORON_1213.SUM_MEK, par.KORON_1213.KOL_P, par.KORON_1213.SUM_P))
                rowindex++;
            //Неотложная МП
            if (PrintVolume(efm, rowindex, par.NEOT.KOL, par.NEOT.SUM, par.NEOT.KOL_MEK, par.NEOT.SUM_MEK, par.NEOT.KOL_P, par.NEOT.SUM_P))
                rowindex++;
            //Профилактические осмотры несовершеннолетних 1 этап
            if (PrintVolume(efm, rowindex, par.PROF_DET.KOL, par.PROF_DET.SUM, par.PROF_DET.KOL_MEK, par.PROF_DET.SUM_MEK, par.PROF_DET.KOL_P, par.PROF_DET.SUM_P))
                rowindex++;
            //Профилактические осмотры взрослого населения 1 этап
            if (PrintVolume(efm, rowindex, par.PROF_VZR.KOL, par.PROF_VZR.SUM, par.PROF_VZR.KOL_MEK, par.PROF_VZR.SUM_MEK, par.PROF_VZR.KOL_P, par.PROF_VZR.SUM_P))
                rowindex++;
            //Диспансеризация детского населения 1 этап
            if (PrintVolume(efm, rowindex, par.DISP_DET.KOL, par.DISP_DET.SUM, par.DISP_DET.KOL_MEK, par.DISP_DET.SUM_MEK, par.DISP_DET.KOL_P, par.DISP_DET.SUM_P))
                rowindex++;
            //Диспансеризация взрослого населения 1 этап
            if (PrintVolume(efm, rowindex, par.DISP_VZR.KOL, par.DISP_VZR.SUM, par.DISP_VZR.KOL_MEK, par.DISP_VZR.SUM_MEK, par.DISP_VZR.KOL_P, par.DISP_VZR.SUM_P))
                rowindex++;
            //Диспансеризация 2 этап
            if (PrintVolume(efm, rowindex, par.DISP2.KOL, par.DISP2.SUM, par.DISP2.KOL_MEK, par.DISP2.SUM_MEK, par.DISP2.KOL_P, par.DISP2.SUM_P))
                rowindex++;
            //Углубленная диспансеризация 1 этап																						
            if (PrintVolume(efm, rowindex, par.UG_DISP1.KOL, par.UG_DISP1.SUM, par.UG_DISP1.KOL_MEK, par.UG_DISP1.SUM_MEK, par.UG_DISP1.KOL_P, par.UG_DISP1.SUM_P))
                rowindex++;
            //Углубленная диспансеризация 2 этап
            if (PrintVolume(efm, rowindex, par.UG_DISP2.KOL, par.UG_DISP2.SUM, par.UG_DISP2.KOL_MEK, par.UG_DISP2.SUM_MEK, par.UG_DISP2.KOL_P, par.UG_DISP2.SUM_P))
                rowindex++;

            //Стационар
            if (PrintVolume(efm, rowindex, par.STAC.KOL, par.STAC.SUM, par.STAC.KOL_MEK, par.STAC.SUM_MEK, par.STAC.KOL_P, par.STAC.SUM_P))
                rowindex++;
            //Стационар 1213
            if (PrintVolume(efm, rowindex, par.STAC_1213.KOL, par.STAC_1213.SUM, par.STAC_1213.KOL_MEK, par.STAC_1213.SUM_MEK, par.STAC_1213.KOL_P, par.STAC_1213.SUM_P))
                rowindex++;
            //Услуги диализа (стационар)
            if (PrintVolume(efm, rowindex, par.DIAL_STAC.KOL, par.DIAL_STAC.SUM, par.DIAL_STAC.KOL_MEK, par.DIAL_STAC.SUM_MEK, par.DIAL_STAC.KOL_P, par.DIAL_STAC.SUM_P))
                rowindex++;
            //Дневной стационар
            if (PrintVolume(efm, rowindex, par.DSTAC.KOL, par.DSTAC.SUM, par.DSTAC.KOL_MEK, par.DSTAC.SUM_MEK, par.DSTAC.KOL_P, par.DSTAC.SUM_P))
                rowindex++;
            //   в т.ч. ЭКО
            if (PrintVolume(efm, rowindex, par.EKO.KOL, par.EKO.SUM, par.EKO.KOL_MEK, par.EKO.SUM_MEK, par.EKO.KOL_P, par.EKO.SUM_P))
                rowindex++;
            //ВМП
            if (PrintVolume(efm, rowindex, par.VMP.KOL, par.VMP.SUM, par.VMP.KOL_MEK, par.VMP.SUM_MEK, par.VMP.KOL_P, par.VMP.SUM_P))
                rowindex++;

            //Тромболизис
            if (PrintVolume(efm, rowindex, null, par.TROMB.SUM, null, par.TROMB.SUM_MEK, null, par.TROMB.SUM_P))
                rowindex++;

            //Прочее
            if (PrintVolume(efm, rowindex, par.OTHER.KOL, par.OTHER.SUM, par.OTHER.KOL_MEK, par.OTHER.SUM_MEK, par.OTHER.KOL_P, par.OTHER.SUM_P))
                rowindex++;

            rowindex += 4;
            efm.PrintCell(rowindex, 11, DEFECT.Count(x => x.IsTARIF), null); rowindex += 2;
            efm.PrintCell(rowindex, 11, DEFECT.Count(x => x.IsLIC), null);

            rowindex += 4;

            var GDEFECT = DEFECT.GroupBy(x => new { x.OSN, x.NAME }).OrderBy(x => x.Key.OSN).ToList();

            if (!GDEFECT.Any())
            {
                efm.RemoveRow(rowindex);

            }
            else
            {
                var lastDefect = GDEFECT.Last();
                foreach (var def in GDEFECT)
                {
                    if (def != lastDefect)
                    {
                        efm.CopyRow(rowindex, rowindex + 1);
                    }
                    var row = efm.GetRow(rowindex);
                    efm.PrintCell(row, 1, def.Key.OSN, null);
                    efm.PrintCell(row, 5, def.Key.NAME, null);
                    //var size1 = efm.Fit(rowindex, 5, 48, 15);
                    var size1 = efm.Fit(row, 5);
                    efm.PrintCell(row, 28, "", null);
                    //var size2 = efm.Fit(rowindex, 28, 48, 15);
                    var size2 = efm.Fit(row, 28);
                    efm.PrintCell(row, 51, def.Sum(x => x.S_SUM), null);
                    efm.SetRowHeigth(rowindex, Math.Max(size1, size2));

                    rowindex++;
                }
            }
            rowindex++;
            efm.PrintCell(rowindex, 1, ISP.DOLG, null);
            efm.PrintCell(rowindex, 26, ISP.FIO, null);
            rowindex++; rowindex++;
            efm.PrintCell(rowindex, 15, RUK.DOLG, null); rowindex++;
            efm.PrintCell(rowindex, 15, RUK.FIO, null);
            rowindex++;
            rowindex++;
            efm.PrintCell(rowindex, 20, $"/{item.D_ACT:dd.MM.yyyy}", null);
        }
        private void CreateRazdel2(ExcelOpenXML efm, MO_ITEM item, MO_FOND_INFO FOND_INFO, MEK_PARAM par,  PODPISANT ISP, PODPISANT RUK)
        {
            efm.SetCurrentSchet(2);
            efm.PrintCell(2, 1, $"№ {item.N_ACT} от {item.D_ACT:dd.MM.yyyy}", null);
            efm.PrintCell(7, 1, item.NAME_MOK, null);
            efm.PrintCell(9, 14, item.NAME_SMOK, null);
            efm.PrintCell(10, 6, (new DateTime(item.YEAR, item.MONTH, 1)).ToString("MMMMMMMMMMMM yyyy"), null);
            uint rowIndex = 14;


            //Предъявлено по итоговому реестру на сумму:
            if (PrintVolume(efm, rowIndex, par.ALL_PRED.KOL, FOND_INFO.AMB_S + FOND_INFO.FAP_S + FOND_INFO.SCOR_S + par.ALL_PRED.SUM, par.ALL_PRED.KOL_MEK, par.ALL_PRED.SUM_MEK + par.NAPR_FROM_MO, par.ALL_PRED.KOL_P, FOND_INFO.AMB_S + FOND_INFO.FAP_S + FOND_INFO.SCOR_S + par.ALL_PRED.SUM_P - par.NAPR_FROM_MO))
                rowIndex++;

            //Медицинская помощь, оказанная прикрепленным гражданам:
            if (PrintVolume(efm, rowIndex, null, FOND_INFO.AMB_S + FOND_INFO.FAP_S + FOND_INFO.SCOR_S, null, par.NAPR_FROM_MO, null, FOND_INFO.AMB_S + FOND_INFO.FAP_S + FOND_INFO.SCOR_S - par.NAPR_FROM_MO))
                rowIndex++;
            //Амбулаторная помощь по дифференцированному подушевому нормативу
            if (PrintVolume(efm, rowIndex, null, FOND_INFO.AMB_S, null, par.NAPR_FROM_MO, null, FOND_INFO.AMB_S - par.NAPR_FROM_MO))
                rowIndex++;

            //Финансовое обеспечение ФП/ФАП: 
            if (PrintVolume(efm, rowIndex, null, FOND_INFO.FAP_S, null, 0, null, FOND_INFO.FAP_S))
                rowIndex++;

            //Скорая помощь по дифференцированному подушевому нормативу: 
            if (PrintVolume(efm, rowIndex, null, FOND_INFO.SCOR_S, null, null, null, FOND_INFO.SCOR_S))
                rowIndex++;
            //Медицинская помощь, оказанная прикрепленным гражданам в других МО
            if (PrintVolume(efm, rowIndex, null, null, null, par.NAPR_FROM_MO, null, null))
                rowIndex++;
            //Медицинская помощь, оказанная неприкрепленным гражданам по направлениям других МО или без направлений, оплачиваемые в бесспорном порядке (внешние услуги):
            //Амбулаторная помощь:
            if (PrintVolume(efm, rowIndex, par.NAPR.KOL, par.NAPR.SUM, par.NAPR.KOL_MEK, par.NAPR.SUM_MEK, par.NAPR.KOL_P, par.NAPR.SUM_P))
                rowIndex++;
            if (PrintVolume(efm, rowIndex, par.NAPR.KOL, par.NAPR.SUM, par.NAPR.KOL_MEK, par.NAPR.SUM_MEK, par.NAPR.KOL_P, par.NAPR.SUM_P))
                rowIndex++;

            rowIndex = PrintRazdel2Profil(efm, rowIndex, par.NAPR);
            //Медицинская помощь, не включенная в систему взаиморасчетов
            if (PrintVolume(efm, rowIndex, par.MP_NOT_VR.KOL, par.MP_NOT_VR.SUM, par.MP_NOT_VR.KOL_MEK, par.MP_NOT_VR.SUM_MEK, par.MP_NOT_VR.KOL_P, par.MP_NOT_VR.SUM_P))
                rowIndex++;
            //Амбулаторная помощь:
            if (PrintVolume(efm, rowIndex, par.AMB.KOL, par.AMB.SUM, par.AMB.KOL_MEK, par.AMB.SUM_MEK, par.AMB.KOL_P, par.AMB.SUM_P))
                rowIndex++;
            rowIndex = PrintRazdel2Profil(efm, rowIndex, par.AMB);


            //Услуги диализа
            if (PrintVolume(efm, rowIndex, par.DIAL_AMB.KOL, par.DIAL_AMB.SUM, par.DIAL_AMB.KOL_MEK, par.DIAL_AMB.SUM_MEK, par.DIAL_AMB.KOL_P, par.DIAL_AMB.SUM_P))
                rowIndex++;
            rowIndex = PrintRazdel2Profil(efm, rowIndex, par.DIAL_AMB);
            //Компьютерная томография
            if (PrintVolume(efm, rowIndex, par.KT.KOL, par.KT.SUM, par.KT.KOL_MEK, par.KT.SUM_MEK, par.KT.KOL_P, par.KT.SUM_P))
                rowIndex++;
            rowIndex = PrintRazdel2Profil(efm, rowIndex, par.KT);
            //Магнитно-резонансная томография
            if (PrintVolume(efm, rowIndex, par.MRT.KOL, par.MRT.SUM, par.MRT.KOL_MEK, par.MRT.SUM_MEK, par.MRT.KOL_P, par.MRT.SUM_P))
                rowIndex++;
            rowIndex = PrintRazdel2Profil(efm, rowIndex, par.MRT);
            //УЗИ сердечно сосудистой системы
            if (PrintVolume(efm, rowIndex, par.USI.KOL, par.USI.SUM, par.USI.KOL_MEK, par.USI.SUM_MEK, par.USI.KOL_P, par.USI.SUM_P))
                rowIndex++;
            rowIndex = PrintRazdel2Profil(efm, rowIndex, par.USI);
            //Эндоскопические диагностические исследования
            if (PrintVolume(efm, rowIndex, par.ENDO.KOL, par.ENDO.SUM, par.ENDO.KOL_MEK, par.ENDO.SUM_MEK, par.ENDO.KOL_P, par.ENDO.SUM_P))
                rowIndex++;
            rowIndex = PrintRazdel2Profil(efm, rowIndex, par.ENDO);

            //Гистологические исследования
            if (PrintVolume(efm, rowIndex, par.GIST.KOL, par.GIST.SUM, par.GIST.KOL_MEK, par.GIST.SUM_MEK, par.GIST.KOL_P, par.GIST.SUM_P))
                rowIndex++;
            rowIndex = PrintRazdel2Profil(efm, rowIndex, par.GIST);
            //Молекулярно-диагностические исследования
            if (PrintVolume(efm, rowIndex, par.MOL.KOL, par.MOL.SUM, par.MOL.KOL_MEK, par.MOL.SUM_MEK, par.MOL.KOL_P, par.MOL.SUM_P))
                rowIndex++;
            rowIndex = PrintRazdel2Profil(efm, rowIndex, par.MOL);
            //Определение РНК коронавирусов
            if (PrintVolume(efm, rowIndex, par.KORON.KOL, par.KORON.SUM, par.KORON.KOL_MEK, par.KORON.SUM_MEK, par.KORON.KOL_P, par.KORON.SUM_P))
                rowIndex++;
            rowIndex = PrintRazdel2Profil(efm, rowIndex, par.KORON);
            //Определение РНК коронавирусов(1213)
            if (PrintVolume(efm, rowIndex, par.KORON_1213.KOL, par.KORON_1213.SUM, par.KORON_1213.KOL_MEK, par.KORON_1213.SUM_MEK, par.KORON_1213.KOL_P, par.KORON_1213.SUM_P))
                rowIndex++;
            rowIndex = PrintRazdel2Profil(efm, rowIndex, par.KORON_1213);

            //Неотложная МП
            if (PrintVolume(efm, rowIndex, par.NEOT.KOL, par.NEOT.SUM, par.NEOT.KOL_MEK, par.NEOT.SUM_MEK, par.NEOT.KOL_P, par.NEOT.SUM_P))
                rowIndex++;
            rowIndex = PrintRazdel2Profil(efm, rowIndex, par.NEOT);
            //Профилактические осмотры несовершеннолетних 1 этап
            if (PrintVolume(efm, rowIndex, par.PROF_DET.KOL, par.PROF_DET.SUM, par.PROF_DET.KOL_MEK, par.PROF_DET.SUM_MEK, par.PROF_DET.KOL_P, par.PROF_DET.SUM_P))
                rowIndex++;

            //Профилактические осмотры взрослого населения 1 этап
            if (PrintVolume(efm, rowIndex, par.PROF_VZR.KOL, par.PROF_VZR.SUM, par.PROF_VZR.KOL_MEK, par.PROF_VZR.SUM_MEK, par.PROF_VZR.KOL_P, par.PROF_VZR.SUM_P))
                rowIndex++;
            //Диспансеризация детского населения 1 этап
            if (PrintVolume(efm, rowIndex, par.DISP_DET.KOL, par.DISP_DET.SUM, par.DISP_DET.KOL_MEK, par.DISP_DET.SUM_MEK, par.DISP_DET.KOL_P, par.DISP_DET.SUM_P))
                rowIndex++;

            //Диспансеризация взрослого населения 1 этап
            if (PrintVolume(efm, rowIndex, par.DISP_VZR.KOL, par.DISP_VZR.SUM, par.DISP_VZR.KOL_MEK, par.DISP_VZR.SUM_MEK, par.DISP_VZR.KOL_P, par.DISP_VZR.SUM_P))
                rowIndex++;

            //Диспансеризация 2 этап
            if (PrintVolume(efm, rowIndex, par.DISP2.KOL, par.DISP2.SUM, par.DISP2.KOL_MEK, par.DISP2.SUM_MEK, par.DISP2.KOL_P, par.DISP2.SUM_P))
                rowIndex++;

            //Углубленная диспансеризация 1 этап
            if (PrintVolume(efm, rowIndex, par.UG_DISP1.KOL, par.UG_DISP1.SUM, par.UG_DISP1.KOL_MEK, par.UG_DISP1.SUM_MEK, par.UG_DISP1.KOL_P, par.UG_DISP1.SUM_P))
                rowIndex++;
            //Углубленная диспансеризация 2 этап
            if (PrintVolume(efm, rowIndex, par.UG_DISP2.KOL, par.UG_DISP2.SUM, par.UG_DISP2.KOL_MEK, par.UG_DISP2.SUM_MEK, par.UG_DISP2.KOL_P, par.UG_DISP2.SUM_P))
                rowIndex++;

            //Стационар
            if (PrintVolume(efm, rowIndex, par.STAC.KOL, par.STAC.SUM, par.STAC.KOL_MEK, par.STAC.SUM_MEK, par.STAC.KOL_P, par.STAC.SUM_P))
                rowIndex++;
            rowIndex = PrintRazdel2Profil(efm, rowIndex, par.STAC);

            //Стационар(1213)
            if (PrintVolume(efm, rowIndex, par.STAC_1213.KOL, par.STAC_1213.SUM, par.STAC_1213.KOL_MEK, par.STAC_1213.SUM_MEK, par.STAC_1213.KOL_P, par.STAC_1213.SUM_P))
                rowIndex++;
            rowIndex = PrintRazdel2Profil(efm, rowIndex, par.STAC_1213);

            //Услуги диализа (стационар)
            if (PrintVolume(efm, rowIndex, par.DIAL_STAC.KOL, par.DIAL_STAC.SUM, par.DIAL_STAC.KOL_MEK, par.DIAL_STAC.SUM_MEK, par.DIAL_STAC.KOL_P, par.DIAL_STAC.SUM_P))
                rowIndex++;
            rowIndex = PrintRazdel2Profil(efm, rowIndex, par.DIAL_STAC);

            //Дневной стационар
            if (PrintVolume(efm, rowIndex, par.DSTAC.KOL, par.DSTAC.SUM, par.DSTAC.KOL_MEK, par.DSTAC.SUM_MEK, par.DSTAC.KOL_P, par.DSTAC.SUM_P))
                rowIndex++;
            rowIndex = PrintRazdel2Profil(efm, rowIndex, par.DSTAC);

            //   в т.ч. ЭКО
            if (PrintVolume(efm, rowIndex, par.EKO.KOL, par.EKO.SUM, par.EKO.KOL_MEK, par.EKO.SUM_MEK, par.EKO.KOL_P, par.EKO.SUM_P))
                rowIndex++;
            rowIndex = PrintRazdel2Profil(efm, rowIndex, par.EKO);

            //ВМП
            if (PrintVolume(efm, rowIndex, par.VMP.KOL, par.VMP.SUM, par.VMP.KOL_MEK, par.VMP.SUM_MEK, par.VMP.KOL_P, par.VMP.SUM_P))
                rowIndex++;
            rowIndex = PrintRazdel2Profil(efm, rowIndex, par.VMP);
            //Тромболизис
            if (PrintVolume(efm, rowIndex, null, par.TROMB.SUM, null, par.TROMB.SUM_MEK, null, par.TROMB.SUM_P))
                rowIndex++;
            rowIndex = PrintRazdel2Profil(efm, rowIndex, par.TROMB, true);
            //Прочее
            if (PrintVolume(efm, rowIndex, par.OTHER.KOL, par.OTHER.SUM, par.OTHER.KOL_MEK, par.OTHER.SUM_MEK, par.OTHER.KOL_P, par.OTHER.SUM_P))
                rowIndex++;


            rowIndex++;
            efm.PrintCell(rowIndex, 1, ISP.DOLG, null);
            efm.PrintCell(rowIndex, 26, ISP.FIO, null);
            rowIndex++; rowIndex++;
            efm.PrintCell(rowIndex, 15, RUK.DOLG, null); rowIndex++;
            efm.PrintCell(rowIndex, 15, RUK.FIO, null);
            rowIndex++;
            rowIndex++;
            efm.PrintCell(rowIndex, 20, $"/{item.D_ACT:dd.MM.yyyy}", null);

        }
        private void CreateRazdel1(ExcelOpenXML efm, MO_ITEM item, MO_FOND_INFO FOND_INFO, MEK_PARAM par, List<MP_DEFECT_ITEM> DEFECT, PODPISANT ISP, PODPISANT RUK)
        {
            efm.SetCurrentSchet(1);
            efm.PrintCell(2, 1, $"№ {item.N_ACT} от {item.D_ACT:dd.MM.yyyy}", null);
            efm.PrintCell(7, 1, item.NAME_MOK, null);
            efm.PrintCell(10, 14, item.NAME_SMOK, null);
            efm.PrintCell(11, 6, (new DateTime(item.YEAR, item.MONTH, 1)).ToString("MMMMMMMMMMMM yyyy"), null);

            const uint KOD_INDEX = 14;
            const uint ZGLV_INDEX = 15;
            const uint ROWZGLV_INDEX = 16;
            uint rowIndex = 14;

            var StyleITOG_TEXT = efm.CreateType(new FontOpenXML() { VerticalAlignment = VerticalAlignmentV.Center, HorizontalAlignment = HorizontalAlignmentV.Right, Bold = true, fontname = "Times New Roman", size = 12 }, null, null);
            var StyleITOG_SUM = efm.CreateType(new FontOpenXML() { VerticalAlignment = VerticalAlignmentV.Center, HorizontalAlignment = HorizontalAlignmentV.Center, Bold = true, fontname = "Times New Roman", size = 12, Format = (uint)DefaultNumFormat.F4 }, null, null);

            if (DEFECT.Count == 0)
            {
                efm.PrintCell(rowIndex, 1, "Нет отказов", null);
                efm.RemoveRow(rowIndex + 2);
                rowIndex++;
                rowIndex++;
            }
            else
            {
                var GDEFECT = DEFECT.GroupBy(x => new { x.OSN, x.NAME, x.COMM }).OrderBy(x => x.Key.OSN).ToList();
                var first = GDEFECT.First();
                var last = GDEFECT.Last();
                foreach (var def in GDEFECT)
                {
                    //Копируем заголовок таблицы
                    if (first != def)
                    {
                        efm.CopyRow(KOD_INDEX, rowIndex);
                        efm.CopyRow(ZGLV_INDEX, rowIndex + 1);
                        efm.CopyRow(ROWZGLV_INDEX, rowIndex + 2);
                    }

                    efm.PrintCell(rowIndex, 1, $"{def.Key.OSN} {def.Key.NAME}({def.Key.COMM})", null);
                    //efm.Fit(rowIndex, 1, 76, 18);
                    efm.Fit(rowIndex, 1);
                    rowIndex++;
                    rowIndex++;
                    var items = def.OrderBy(x => x.S_SUM).ToList();
                    if (items.Count > 1)
                    {
                        efm.CopyRow(rowIndex, rowIndex + 1, items.Count - 1);
                    }

                    decimal SUM_S = 0;
                    foreach (var def_item in items)
                    {
                        var MRow = efm.GetRow(rowIndex);
                        efm.PrintCell(MRow, 1, def_item.IDCASE, null);
                        efm.PrintCell(MRow, 16, def_item.PODR, null);
                        efm.PrintCell(MRow, 22, def_item.POLIS, null);
                        efm.PrintCell(MRow, 32, def_item.MKB, null);
                        efm.PrintCell(MRow, 37, def_item.DATE_1, null);
                        efm.PrintCell(MRow, 43, def_item.DATE_2, null);
                        efm.PrintCell(MRow, 50, def_item.S_SUM, null);
                        SUM_S += def_item.S_SUM;
                        rowIndex++;
                    }

                    efm.InsertRow(rowIndex, 1);
                    efm.AddMergedRegion(new CellRangeAddress(rowIndex, 1, rowIndex, 42));
                    efm.AddMergedRegion(new CellRangeAddress(rowIndex, 43, rowIndex, 56));
                    efm.PrintCell(rowIndex, 1, $"Итого по коду дефекта/нарушения: ", StyleITOG_TEXT);
                    efm.PrintCell(rowIndex, 43, SUM_S, StyleITOG_SUM);
                    rowIndex++;
                    if (def != last)
                    {
                        efm.InsertRow(rowIndex, 1);
                        rowIndex++;
                    }


                }
            }

            rowIndex++;
            efm.PrintCell(rowIndex, 1, ISP.DOLG, null);
            efm.PrintCell(rowIndex, 26, ISP.FIO, null);
            rowIndex++; rowIndex++;
            efm.PrintCell(rowIndex, 15, RUK.DOLG, null); rowIndex++;
            efm.PrintCell(rowIndex, 15, RUK.FIO, null);
            rowIndex++;
            rowIndex++;
            efm.PrintCell(rowIndex, 20, $"/{item.D_ACT:dd.MM.yyyy}", null);

        }
        private void CreateReestrAct(ExcelOpenXML efm, MO_ITEM item, MO_FOND_INFO FOND_INFO, MEK_PARAM par, List<MP_DEFECT_ITEM> DEFECT, PODPISANT ISP)
        {
            efm.SetCurrentSchet(3);
            efm.PrintCell(2, 1, $"№ {item.N_ACT} от {item.D_ACT:dd.MM.yyyy}", null);
            efm.PrintCell(4, 14, item.NAME_SMOK, null);
            efm.PrintCell(5, 6, (new DateTime(item.YEAR, item.MONTH, 1)).ToString("MMMMMMMMMMMM yyyy"), null);
            efm.PrintCell(11, 1, $"{item.CODE_MO} {item.NAME_MOK}", null);
            uint RowIndex = 14;

            //Всего предоставлено счетов на сумму(руб.):
            efm.PrintCell(RowIndex, 10, FOND_INFO.AMB_S + FOND_INFO.FAP_S + FOND_INFO.SCOR_S + par.ALL_PRED.SUM, null);
            efm.PrintCell(RowIndex, 27, par.ALL_PRED.KOL, null);
            RowIndex += 4;
            //За медицинскую помощь, оказанную стационарно:
            efm.PrintCell(RowIndex, 10, par.FULL_STAC.SUM, null); efm.PrintCell(RowIndex, 27, par.FULL_STAC.KOL, null); RowIndex += 2;
            //За медицинскую помощь, оказанную в дневном стационаре:
            efm.PrintCell(RowIndex, 10, par.DSTAC.SUM, null); efm.PrintCell(RowIndex, 27, par.DSTAC.KOL, null); RowIndex += 2;
            //За медицинскую помощь, оказанную амбулаторно
            efm.PrintCell(RowIndex, 10, par.FULL_AMB.SUM + FOND_INFO.AMB_S + FOND_INFO.FAP_S, null); efm.PrintCell(RowIndex, 27, par.FULL_AMB.KOL, null); RowIndex += 2;
            //За медицинскую помощь, оказанную вне медицинской организации
            efm.PrintCell(RowIndex, 10, FOND_INFO.SCOR_S + par.TROMB.SUM, null); efm.PrintCell(RowIndex, 27, par.FULL_SCOR.KOL, null);

            RowIndex += 3;

            //1. Согласовано к оплате всего:
            efm.PrintCell(RowIndex, 10, FOND_INFO.AMB_S + FOND_INFO.FAP_S + FOND_INFO.SCOR_S + par.ALL_PRED.SUM_P - par.NAPR_FROM_MO, null); efm.PrintCell(RowIndex, 27, par.ALL_PRED.KOL_P, null); RowIndex += 3;
            //За медицинскую помощь, оказанную стационарно:
            efm.PrintCell(RowIndex, 10, par.FULL_STAC.SUM_P, null); efm.PrintCell(RowIndex, 27, par.FULL_STAC.KOL_P, null); RowIndex += 2;
            //За медицинскую помощь, оказанную в дневном стационаре:
            efm.PrintCell(RowIndex, 10, par.DSTAC.SUM_P, null); efm.PrintCell(RowIndex, 27, par.DSTAC.KOL_P, null); RowIndex += 2;
            //За медицинскую помощь, оказанную амбулаторно
            efm.PrintCell(RowIndex, 10, par.FULL_AMB.SUM_P + FOND_INFO.AMB_S + FOND_INFO.FAP_S - par.NAPR_FROM_MO, null); efm.PrintCell(RowIndex, 27, par.FULL_AMB.KOL_P, null); RowIndex += 2;
            //За медицинскую помощь, оказанную вне медицинской организации
            efm.PrintCell(RowIndex, 10, par.FULL_SCOR.SUM_P + FOND_INFO.SCOR_S, null); efm.PrintCell(RowIndex, 27, par.FULL_SCOR.KOL_P, null); RowIndex += 3;

            var sankSTAC = DEFECT.Where(x => x.USL_OK == 1 && !x.isPVOL).OrderBy(x => x.OSN).ThenBy(x => x.S_SUM).ToList();
            var sankDSTAC = DEFECT.Where(x => x.USL_OK == 2 && !x.isPVOL).OrderBy(x => x.OSN).ThenBy(x => x.S_SUM).ToList();
            var sankAMB = DEFECT.Where(x => x.USL_OK == 3 && !x.isPVOL).OrderBy(x => x.OSN).ThenBy(x => x.S_SUM).ToList();
            var sankSCOR = DEFECT.Where(x => x.USL_OK == 4 && !x.isPVOL).OrderBy(x => x.OSN).ThenBy(x => x.S_SUM).ToList();
            var sankVOLUME = DEFECT.Where(x => x.isPVOL).OrderBy(x => x.OSN).ThenBy(x => x.S_SUM).ToList();

            //2. Не согласовано к оплате реестров: 
            efm.PrintCell(RowIndex, 10, sankSTAC.Sum(x => x.S_SUM) + sankDSTAC.Sum(x => x.S_SUM) + sankAMB.Sum(x => x.S_SUM) + sankSCOR.Sum(x => x.S_SUM) + sankVOLUME.Sum(x => x.S_SUM) + par.NAPR_FROM_MO, null); efm.PrintCell(RowIndex, 27, sankSTAC.Count + sankDSTAC.Count + sankAMB.Count + sankSCOR.Count + sankVOLUME.Count, null); RowIndex += 3;
            //За медицинскую помощь, оказанную стационарно:
            efm.PrintCell(RowIndex, 10, sankSTAC.Sum(x => x.S_SUM), null); efm.PrintCell(RowIndex, 27, sankSTAC.Count, null); RowIndex += 2;
            //За медицинскую помощь, оказанную в дневном стационаре:
            efm.PrintCell(RowIndex, 10, sankDSTAC.Sum(x => x.S_SUM), null); efm.PrintCell(RowIndex, 27, sankDSTAC.Count, null); RowIndex += 2;
            //За медицинскую помощь, оказанную амбулаторно
            efm.PrintCell(RowIndex, 10, sankAMB.Sum(x => x.S_SUM), null); efm.PrintCell(RowIndex, 27, sankAMB.Count, null); RowIndex += 2;
            //За медицинскую помощь, оказанную вне медицинской организации
            efm.PrintCell(RowIndex, 10, sankSCOR.Sum(x => x.S_SUM), null); efm.PrintCell(RowIndex, 27, sankSCOR.Count, null); RowIndex += 2;
            //за превышение  согласованных объемов   медицинской помощи:
            efm.PrintCell(RowIndex, 10, sankVOLUME.Sum(x => x.S_SUM), null); efm.PrintCell(RowIndex, 27, sankVOLUME.Count, null); RowIndex += 2;
            //за медицинская помощь, отказанная прикрепленным гражданам в других МО:
            efm.PrintCell(RowIndex, 10, par.NAPR_FROM_MO, null); efm.PrintCell(RowIndex, 27, "", null);

            RowIndex += 3;
            efm.PrintCell(RowIndex, 10, DEFECT.Sum(x => x.S_SUM), null); efm.PrintCell(RowIndex, 27, sankSTAC.Count, null);
            RowIndex += 2;
            //2.1.1. за стационарную медицинскую помощь:
            efm.PrintCell(RowIndex, 10, sankSTAC.Sum(x => x.S_SUM), null); efm.PrintCell(RowIndex, 27, sankSTAC.Count, null);
            RowIndex += 3;
            if (sankSTAC.Count == 0)
            {
                efm.RemoveRow(RowIndex);
            }
            else
            {
                if (sankSTAC.Count > 1)
                    efm.CopyRow(RowIndex, RowIndex + 1, sankSTAC.Count - 1);

                foreach (var san in sankSTAC)
                {
                    efm.PrintCell(RowIndex, 1, san.PODR, null);
                    efm.PrintCell(RowIndex, 9, san.OTD, null);
                    efm.PrintCell(RowIndex, 15, san.IDCASE, null);
                    efm.PrintCell(RowIndex, 24, item.MONTH, null);
                    efm.PrintCell(RowIndex, 28, san.POLIS, null);
                    efm.PrintCell(RowIndex, 40, 75, null);
                    efm.PrintCell(RowIndex, 46, san.OSN, null);
                    efm.PrintCell(RowIndex, 51, san.S_SUM, null);
                    RowIndex++;
                }
            }
            RowIndex += 2;

            //2.1.2. за медицинскую помощь в дневном стационаре

            efm.PrintCell(RowIndex, 10, sankDSTAC.Sum(x => x.S_SUM), null); efm.PrintCell(RowIndex, 27, sankDSTAC.Count, null);
            RowIndex += 3;
            if (sankDSTAC.Count == 0)
            {
                efm.RemoveRow(RowIndex);
            }
            else
            {
                if (sankDSTAC.Count > 1)
                    efm.CopyRow(RowIndex, RowIndex + 1, sankDSTAC.Count - 1);

                foreach (var san in sankDSTAC)
                {
                    efm.PrintCell(RowIndex, 1, san.PODR, null);
                    efm.PrintCell(RowIndex, 9, san.OTD, null);
                    efm.PrintCell(RowIndex, 15, san.IDCASE, null);
                    efm.PrintCell(RowIndex, 24, item.MONTH, null);
                    efm.PrintCell(RowIndex, 28, san.POLIS, null);
                    efm.PrintCell(RowIndex, 40, 75, null);
                    efm.PrintCell(RowIndex, 46, san.OSN, null);
                    efm.PrintCell(RowIndex, 51, san.S_SUM, null);
                    RowIndex++;
                }
            }
            RowIndex += 2;

            //2.1.3. за амбулаторно-поликлиническую медицинскую помощь	

            efm.PrintCell(RowIndex, 10, sankAMB.Sum(x => x.S_SUM), null); efm.PrintCell(RowIndex, 27, sankAMB.Count, null);
            RowIndex += 3;
            if (sankAMB.Count == 0)
            {
                efm.RemoveRow(RowIndex);
            }
            else
            {
                if (sankAMB.Count > 1)
                    efm.CopyRow(RowIndex, RowIndex + 1, sankAMB.Count - 1);

                foreach (var san in sankAMB)
                {
                    efm.PrintCell(RowIndex, 1, san.PODR, null);
                    efm.PrintCell(RowIndex, 9, san.OTD, null);
                    efm.PrintCell(RowIndex, 15, san.IDCASE, null);
                    efm.PrintCell(RowIndex, 24, item.MONTH, null);
                    efm.PrintCell(RowIndex, 28, san.POLIS, null);
                    efm.PrintCell(RowIndex, 40, 75, null);
                    efm.PrintCell(RowIndex, 46, san.OSN, null);
                    efm.PrintCell(RowIndex, 51, san.S_SUM, null);
                    RowIndex++;
                }
            }
            RowIndex += 2;

            //2.1.4. медицинскую помощь, оказанную вне медицинской организации

            efm.PrintCell(RowIndex, 10, sankSCOR.Sum(x => x.S_SUM), null); efm.PrintCell(RowIndex, 27, sankSCOR.Count, null);
            RowIndex += 3;
            if (sankSCOR.Count == 0)
            {
                efm.RemoveRow(RowIndex);
            }
            else
            {
                if (sankSCOR.Count > 1)
                    efm.CopyRow(RowIndex, RowIndex + 1, sankSCOR.Count - 1);

                foreach (var san in sankSCOR)
                {
                    efm.PrintCell(RowIndex, 1, san.PODR, null);
                    efm.PrintCell(RowIndex, 9, san.OTD, null);
                    efm.PrintCell(RowIndex, 15, san.IDCASE, null);
                    efm.PrintCell(RowIndex, 24, item.MONTH, null);
                    efm.PrintCell(RowIndex, 28, san.POLIS, null);
                    efm.PrintCell(RowIndex, 40, 75, null);
                    efm.PrintCell(RowIndex, 46, san.OSN, null);
                    efm.PrintCell(RowIndex, 51, san.S_SUM, null);
                    RowIndex++;
                }
            }
            RowIndex += 2;
            //2. Не принято к оплате в связи с превышением установленных комиссией по разработке территориальной программы обязательного медицинского страхования объемов медицинской помощи:
            efm.PrintCell(RowIndex, 10, sankVOLUME.Sum(x => x.S_SUM), null); efm.PrintCell(RowIndex, 27, sankVOLUME.Count, null); RowIndex += 3;
            //За медицинскую помощь, оказанную стационарно:
            efm.PrintCell(RowIndex, 10, sankVOLUME.Where(x => x.USL_OK == 1).Sum(x => x.S_SUM), null); efm.PrintCell(RowIndex, 27, sankVOLUME.Count(x => x.USL_OK == 1), null); RowIndex += 2;
            //За медицинскую помощь, оказанную в дневном стационаре:
            efm.PrintCell(RowIndex, 10, sankVOLUME.Where(x => x.USL_OK == 2).Sum(x => x.S_SUM), null); efm.PrintCell(RowIndex, 27, sankVOLUME.Count(x => x.USL_OK == 2), null); RowIndex += 2;
            //За медицинскую помощь, оказанную амбулаторно:
            efm.PrintCell(RowIndex, 10, sankVOLUME.Where(x => x.USL_OK == 3).Sum(x => x.S_SUM), null); efm.PrintCell(RowIndex, 27, sankVOLUME.Count(x => x.USL_OK == 3), null); RowIndex += 2;
            //За медицинскую помощь, оказанную вне медицинской организации
            efm.PrintCell(RowIndex, 10, sankVOLUME.Where(x => x.USL_OK == 4).Sum(x => x.S_SUM), null); efm.PrintCell(RowIndex, 27, sankVOLUME.Count(x => x.USL_OK == 4), null); RowIndex += 3;

            if (sankVOLUME.Count == 0)
            {
                efm.RemoveRow(RowIndex);
            }
            else
            {
                if (sankVOLUME.Count > 1)
                    efm.CopyRow(RowIndex, RowIndex + 1, sankVOLUME.Count - 1);

                foreach (var san in sankVOLUME)
                {
                    efm.PrintCell(RowIndex, 1, san.PODR, null);
                    efm.PrintCell(RowIndex, 8, san.OTD, null);
                    efm.PrintCell(RowIndex, 14, item.MONTH, null);
                    efm.PrintCell(RowIndex, 21, san.S_SUM, null);
                    efm.PrintCell(RowIndex, 27, san.S_SUM, null);
                    efm.PrintCell(RowIndex, 33, san.S_SUM, null);
                    efm.PrintCell(RowIndex, 39, san.S_SUM, null);
                    efm.PrintCell(RowIndex, 45, san.S_SUM, null);
                    efm.PrintCell(RowIndex, 51, 0, null);
                    RowIndex++;
                }
            }

            RowIndex++;
            var Mrow = efm.GetRow(RowIndex);
            efm.PrintCell(Mrow, 33, item.DATE_INVITE, null);
            RowIndex++;
            Mrow = efm.GetRow(RowIndex);
            efm.PrintCell(Mrow, 14, item.D_ACT, null);
            RowIndex += 3;
            efm.PrintCell(RowIndex, 1, ISP.DOLG, null);
            efm.PrintCell(RowIndex, 22, ISP.FIO, null);
        }
        uint PrintRazdel2Profil(ExcelOpenXML efm, uint RowIndex, MEK_ITEM MEK, bool hideKOL = false)
        {
            if (MEK.MEK_PROFIL.Count == 0)
            {
                efm.RemoveRow(RowIndex);
            }
            else
            {
                var values = MEK.MEK_PROFIL.OrderBy(x => x.Key.PROFIL).ToList();
                if (values.Count > 1)
                    efm.CopyRow(RowIndex, RowIndex + 1, values.Count - 1);


                foreach (var mek in values)
                {
                    PrintProfil(efm, RowIndex, mek.Key.PROFIL, mek.Key.NAME, mek.Value.KOL, mek.Value.SUM, mek.Value.KOL_MEK, mek.Value.SUM_MEK, mek.Value.KOL_P, mek.Value.SUM_P, hideKOL);
                    RowIndex++;
                }
            }

            return RowIndex;
        }
        private bool PrintVolume(ExcelOpenXML efm, uint RowIndex, decimal? KOL, decimal? SUM, decimal? KOL_MEK, decimal? SUM_MEK, decimal? KOL_P, decimal? SUM_P)
        {
            var row = efm.GetRow(RowIndex);
            if ((KOL ?? 0) == 0 && (SUM ?? 0) == 0 && (KOL_MEK ?? 0) == 0 && (SUM_MEK ?? 0) == 0 && (KOL_P ?? 0) == 0 && (SUM_P ?? 0) == 0)
            {
                efm.RemoveRow(RowIndex);
                return false;
            }

            efm.PrintCell(row, 24, KOL, null);
            efm.PrintCell(row, 29, SUM, null);
            efm.PrintCell(row, 35, KOL_MEK, null);
            efm.PrintCell(row, 40, SUM_MEK, null);
            efm.PrintCell(row, 46, KOL_P, null);
            efm.PrintCell(row, 51, SUM_P, null);
            return true;
        }
        private void PrintProfil(ExcelOpenXML efm, uint RowIndex, int? PROFIL, string PROFIL_NAME, decimal? KOL, decimal? SUM, decimal? KOL_MEK, decimal? SUM_MEK, decimal? KOL_P, decimal? SUM_P, bool hideKOL)
        {
            var row = efm.GetRow(RowIndex);
            efm.PrintCell(row, 1, PROFIL, null);
            efm.PrintCell(row, 5, PROFIL_NAME, null);
            efm.PrintCell(row, 29, SUM, null);
            efm.PrintCell(row, 40, SUM_MEK, null);
            efm.PrintCell(row, 51, SUM_P, null);

            if (!hideKOL)
            {
                efm.PrintCell(row, 24, KOL, null);
                efm.PrintCell(row, 35, KOL_MEK, null);
                efm.PrintCell(row, 46, KOL_P, null);
            }
            //efm.Fit(row, 5, 35, 15);
            efm.Fit(row, 5);
        }
        public MEK_PARAM ConvertVOLUMEToMEK_PARAM(List<MP_VOLUME_ITEM> Volume, decimal NAPR_FROM_MO)
        {
            var par = new MEK_PARAM();
            var err = new List<MP_VOLUME_ITEM>();
            foreach (var vol in Volume)
            {
                switch (vol.VOLUME_VALUE_ID)
                {
                    //посещений с профилактическими и иными целями
                    case "3.1.1.1":
                        par.POS_FOND.AddKOL(vol);
                        break;
                    //обращений по поводу заболевания
                    case "3.1.2.1":
                        par.OBR_FOND.AddKOL(vol);
                        break;
                    //посещений с профилактическими и иными целями ФАП
                    case "3.1.3.1":
                        par.POS_FAP.AddKOL(vol);
                        break;
                    //обращений по поводу заболевания ФАП
                    case "3.1.4.1":
                        par.OBR_FAP.AddKOL(vol);
                        break;


                    //посещений с профилактическими и иными целями НАПРАВЛЕНИЯ
                    case "3.1.1.2":
                        par.POS_NAPR.Add(vol);
                        break;
                    //обращений по поводу заболевания НАПРАВЛЕНИЯ
                    case "3.1.2.2":
                        par.OBR_NAPR.Add(vol);
                        break;
                    //посещений с профилактическими и иными целями	 СОГЛАСОВАННЫЕ
                    case "3.1.1.3":
                        par.POS_AMB.Add(vol);
                        break;
                    //обращений по поводу заболевания	 СОГЛАСОВАННЫЕ
                    case "3.1.2.3":
                        par.OBR_AMB.Add(vol);
                        break;
                    //Услуги диализа АМБ		
                    case "5.2.1":
                        par.DIAL_AMB.Add(vol);
                        break;
                    //Компьютерная томография		
                    case "3.3.1.1":
                        par.KT.Add(vol);
                        break;
                    //Магнитно-резонансная томография	
                    case "3.3.2.1":
                        par.MRT.Add(vol);
                        break;
                    //УЗИ сердечно сосудистой системы				
                    case "3.3.3.1":
                        par.USI.Add(vol);
                        break;
                    //Эндоскопические диагностические исследования			
                    case "3.3.4.1":
                        par.ENDO.Add(vol);
                        break;
                    //Гистологические исследования	
                    case "3.3.5.1":
                        par.GIST.Add(vol);
                        break;
                    //Молекулярно-диагностические исследования			
                    case "3.3.6.1":
                        par.MOL.Add(vol);
                        break;
                    //Определение РНК коронавирусов			
                    case "3.3.7.1":
                        par.KORON.Add(vol);
                        break;
                    //Определение РНК коронавирусов	(Пост 1213)		
                    case "3.3.8.1":
                        par.KORON_1213.Add(vol);
                        break;
                    //Неотложная МП	
                    case "3.2.1.1":
                        par.NEOT.Add(vol);
                        break;
                    case "3.2.1.2":
                        par.NEOT.AddSUM(vol);
                        break;
                    //Стационар	
                    case "1.1.1":
                    case "1.2.1":
               
                        par.STAC.Add(vol);
                        break;
                    //Стационар МБТ	
                    case "1.1.1.2":
                    case "1.1.1.3":
                        par.STAC.Add(vol);
                        break;
                    //Услуги диализа (стационар)		
                    case "5.1.1":
                        par.DIAL_STAC.Add(vol);
                        break;
                    //Дневной стационар			
                    case "2.1.1":
                    case "2.2.1":
                        par.DSTAC_WITHOUT_EKO.Add(vol);
                        break;
                    //   в т.ч. ЭКО			
                    case "2.3.1":
                        par.EKO.Add(vol);
                        break;
                    //ВМП		
                    case "1.3.1":
                        par.VMP.Add(vol);
                        break;
                    //Профилактические осмотры несовершеннолетних 1 этап		
                    case "3.4.1.1":
                        par.PROF_DET.Add(vol);
                        break;
                    //Профилактические осмотры взрослого населения 1 этап		
                    case "3.4.2.1":
                        par.PROF_VZR.Add(vol);
                        break;
                    //Диспансеризация детского населения 1 этап
                    case "3.5.1.1":
                        par.DISP_DET.Add(vol);
                        break;
                    //Диспансеризация взрослого населения 1 этап	
                    case "3.5.2.1":
                        par.DISP_VZR.Add(vol);
                        break;
                    //Диспансеризация 2 этап
                    case "3.5.3.1":
                        par.DISP2.Add(vol);
                        break;
                    //Профилактические осмотры несовершеннолетних 1 этап(Фондодержание)		
                    case "3.6.1.1":
                        par.PROF_DET_FOND.AddKOL(vol);
                        break;
                    //Профилактические осмотры взрослого населения 1 этап(Фондодержание)
                    case "3.6.2.1":
                        par.PROF_VZR_FOND.AddKOL(vol);
                        break;
                    //Диспансеризация детского населения 1 этап(Фондодержание)
                    case "3.7.1.1":
                        par.DISP_DET_FOND.AddKOL(vol);
                        break;
                    //Диспансеризация взрослого населения 1 этап(Фондодержание)
                    case "3.7.2.1":
                        par.DISP_VZR_FOND.AddKOL(vol);
                        break;
                    //Углубленная диспансеризация 1 этап
                    case "3.8.1.1":
                        par.UG_DISP1.Add(vol);
                        break;
                    //Углубленная диспансеризация 2 этап
                    case "3.8.2.1":
                        par.UG_DISP2.Add(vol);
                        break;
                    //Параклиника
                    case "3.1.5.1": /*par.OBR_FOND.AddSUM(vol);*/ break;
                    //Параклиника(Направление)
                    case "3.1.5.2":
                        par.OBR_NAPR.AddSUM(vol);
                        break;
                    //Параклиника(Согласованные объемы)
                    case "3.1.5.3":
                        par.OBR_AMB.AddSUM(vol);
                        break;
                    //Параклиника фап
                    case "3.1.5.4":
                        // par.OBR_FAP.AddKOL(vol);
                        break;
                    //Скорая
                    case "4.1":
                        par.SCOR.AddKOL(vol);
                        break;
                    //Тромболизис
                    case "4.2":
                        par.TROMB.AddSUM(vol);
                        break;
                    case "":
                    case null:
                        par.OTHER.Add(vol);
                        break;

                    default:
                        err.Add(vol);
                        break;
                }
            }

            par.NAPR_FROM_MO = NAPR_FROM_MO;

            if (err.Count != 0)
                throw new Exception($"Показатели не участвуют в расчетах: {string.Join(",", err.Select(x => $"{x.VOLUME_VALUE_ID}({x.NAME})"))}");

            return par;
        }

        public void CreateFileСrossing(string path, List<CrossingRow> List)
        {
            using (var xls = new ExcelOpenXML(path, "Пересечения"))
            {
                var StyleHeader = xls.CreateType(new FontOpenXML() { Bold = true, HorizontalAlignment = HorizontalAlignmentV.Center, VerticalAlignment = VerticalAlignmentV.Center, fontname = "Times New Roman", size = 10 }, new BorderOpenXML(), null);
                var StyleText = xls.CreateType(new FontOpenXML() { HorizontalAlignment = HorizontalAlignmentV.Left, VerticalAlignment = VerticalAlignmentV.Center, fontname = "Times New Roman", size = 10 }, new BorderOpenXML(), null);
                var StyleDate = xls.CreateType(new FontOpenXML() { HorizontalAlignment = HorizontalAlignmentV.Left, VerticalAlignment = VerticalAlignmentV.Center, fontname = "Times New Roman", size = 10, Format = (uint)DefaultNumFormat.F14 }, new BorderOpenXML(), null);
                uint RowIndex = 1;
                xls.PrintCell(RowIndex, 1, "Пересечение случаев оказанной медицинской помощи", StyleHeader); xls.AddMergedRegion(new CellRangeAddress(RowIndex, 1, RowIndex, 19));
                RowIndex++;
                xls.PrintCell(RowIndex, 2, "Снятие", StyleHeader); xls.AddMergedRegion(new CellRangeAddress(RowIndex, 2, RowIndex, 14));
                xls.PrintCell(RowIndex, 15, "На основании случая", StyleHeader); xls.AddMergedRegion(new CellRangeAddress(RowIndex, 15, RowIndex, 19));
                RowIndex++;
                uint ColIndex = 1;
                xls.PrintCell(RowIndex, ColIndex, "№", StyleHeader); ColIndex++;
                xls.PrintCell(RowIndex, ColIndex, "Код МО", StyleHeader); ColIndex++;
                xls.PrintCell(RowIndex, ColIndex, "Наименование", StyleHeader); ColIndex++;
                xls.PrintCell(RowIndex, ColIndex, "Полис", StyleHeader); ColIndex++;
                xls.PrintCell(RowIndex, ColIndex, "Фамилия", StyleHeader); ColIndex++;
                xls.PrintCell(RowIndex, ColIndex, "Имя", StyleHeader); ColIndex++;
                xls.PrintCell(RowIndex, ColIndex, "Отчество", StyleHeader); ColIndex++;
                xls.PrintCell(RowIndex, ColIndex, "ДР", StyleHeader); ColIndex++;
                xls.PrintCell(RowIndex, ColIndex, "IDCASE", StyleHeader); ColIndex++;
                xls.PrintCell(RowIndex, ColIndex, "№ истории", StyleHeader); ColIndex++;
                xls.PrintCell(RowIndex, ColIndex, "Усл.ок.", StyleHeader); ColIndex++;
                xls.PrintCell(RowIndex, ColIndex, "Наименование", StyleHeader); ColIndex++;
                xls.PrintCell(RowIndex, ColIndex, "Дата начала", StyleHeader); ColIndex++;
                xls.PrintCell(RowIndex, ColIndex, "Дата окончания", StyleHeader); ColIndex++;
                xls.PrintCell(RowIndex, ColIndex, "Код МО", StyleHeader); ColIndex++;
                xls.PrintCell(RowIndex, ColIndex, "Наименование", StyleHeader); ColIndex++;
                xls.PrintCell(RowIndex, ColIndex, "Усл.ок.", StyleHeader); ColIndex++;
                xls.PrintCell(RowIndex, ColIndex, "Наименование", StyleHeader); ColIndex++;
                xls.PrintCell(RowIndex, ColIndex, "№ истории", StyleHeader);

                xls.SetColumnWidth(1, 4);
                xls.SetColumnWidth(2, 7);
                xls.SetColumnWidth(3, 37);
                xls.SetColumnWidth(4, 16);
                xls.SetColumnWidth(5, 17);
                xls.SetColumnWidth(6, 17);
                xls.SetColumnWidth(7, 17);
                xls.SetColumnWidth(8, 14);
                xls.SetColumnWidth(9, 9);
                xls.SetColumnWidth(10, 20);
                xls.SetColumnWidth(11, 7);
                xls.SetColumnWidth(12, 19);
                xls.SetColumnWidth(13, 14);
                xls.SetColumnWidth(14, 14);
                xls.SetColumnWidth(15, 7);
                xls.SetColumnWidth(16, 37);
                xls.SetColumnWidth(17, 7);
                xls.SetColumnWidth(18, 19);
                xls.SetColumnWidth(19, 20);


                var i = 0;
                foreach (var row in List)
                {
                    RowIndex++;
                    ColIndex = 1;
                    i++;
                    xls.PrintCell(RowIndex, ColIndex, i, StyleText); ColIndex++;
                    xls.PrintCell(RowIndex, ColIndex, row.CODE_MO, StyleText); ColIndex++;
                    xls.PrintCell(RowIndex, ColIndex, row.NAM_MOK, StyleText); ColIndex++;
                    xls.PrintCell(RowIndex, ColIndex, row.POLIS, StyleText); ColIndex++;
                    xls.PrintCell(RowIndex, ColIndex, row.FAM, StyleText); ColIndex++;
                    xls.PrintCell(RowIndex, ColIndex, row.IM, StyleText); ColIndex++;
                    xls.PrintCell(RowIndex, ColIndex, row.OT, StyleText); ColIndex++;
                    xls.PrintCell(RowIndex, ColIndex, row.DR, StyleDate); ColIndex++;
                    xls.PrintCell(RowIndex, ColIndex, (double)row.IDCASE, StyleText); ColIndex++;
                    xls.PrintCell(RowIndex, ColIndex, row.NHISTORY, StyleText); ColIndex++;
                    xls.PrintCell(RowIndex, ColIndex, row.USL_OK, StyleText); ColIndex++;
                    xls.PrintCell(RowIndex, ColIndex, row.USL_OK_NAME, StyleText); ColIndex++;
                    xls.PrintCell(RowIndex, ColIndex, row.DATE_1, StyleDate); ColIndex++;
                    xls.PrintCell(RowIndex, ColIndex, row.DATE_2, StyleDate); ColIndex++;
                    xls.PrintCell(RowIndex, ColIndex, row.CROS_CODE_MO, StyleText); ColIndex++;
                    xls.PrintCell(RowIndex, ColIndex, row.CROS_NAM_MOK, StyleText); ColIndex++;
                    xls.PrintCell(RowIndex, ColIndex, row.CROS_USL_OK, StyleText); ColIndex++;
                    xls.PrintCell(RowIndex, ColIndex, row.CROS_USL_OK_NAME, StyleText); ColIndex++;
                    xls.PrintCell(RowIndex, ColIndex, row.CROS_NHISTORY, StyleText);

                }
                xls.MarkAsFinal(true);
                xls.Save();
            }
        }
    }
}
