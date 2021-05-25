using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using ServiceLoaderMedpomData;
using ServiceLoaderMedpomData.EntityMP_V31;

namespace ClientServiceWPF.SANK_INVITER
{
    public class ToBaseFileResult
    {
        public bool Result { get; set; }
        public string Message { get; set; }

        public static ToBaseFileResult Create(bool Result, string Message = null)
        {
            return new ToBaseFileResult {Result = Result, Message = Message};
        }
    }
    public interface IToBaseFile
    {
         ToBaseFileResult ToBaseFile(FileItem fi, bool FLAG_MEE, int YEAR, int MONTH, string SMO, bool DOP_REESTR, bool IsNotFinish);
         ToBaseFileResult ToBaseFileSANK(FileItem fi, bool FLAG_MEE, int YEAR, int MONTH, string SMO, bool DOP_REESTR, bool IsNotFinish, bool IsRewrite, List<FindSluchItem> IDENT_INFO);
    }


    public class ToBaseFileSank: IToBaseFile
    {
        private Dispatcher dispatcher;
        private IRepository repository;

        public ToBaseFileSank(Dispatcher dispatcher, IRepository repository)
        {
            this.dispatcher = dispatcher;
            this.repository = repository;
        }
        public ToBaseFileResult ToBaseFile(FileItem fi, bool FLAG_MEE, int YEAR, int MONTH, string SMO, bool DOP_REESTR, bool IsNotFinish)
        {
            repository.BeginTransaction();
            try
            {
                var zl = ZL_LIST.GetZL_LIST(fi.Version, fi.FilePach);
                zl.SetSUMP();
                int id;
                fi.FileLog.WriteLn("Заголовок санкций");
                id = repository.AddSankZGLV(zl.ZGLV.FILENAME.ToUpper(), Convert.ToInt32(zl.SCHET.CODE), Convert.ToInt32(zl.SCHET.CODE_MO), FLAG_MEE ? 1 : 0, Convert.ToInt32(zl.SCHET.YEAR), Convert.ToInt32(zl.SCHET.MONTH), YEAR, MONTH, -1, SMO, DOP_REESTR, IsNotFinish);

                zl.SCHET.YEAR_BASE = zl.SCHET.YEAR;
                zl.SCHET.MONTH_BASE = zl.SCHET.MONTH;

                zl.SCHET.YEAR = YEAR;
                zl.SCHET.MONTH = MONTH;
                zl.SCHET.DOP_FLAG = 1;

                fi.FileLog.WriteLn("Подготовка");
                foreach (var z in zl.ZAP)
                {
                    z.PR_NOV = 1;
                }

                foreach (var p in zl.ZAP.Select(x => x.PACIENT))
                {
                    p.SMO_TFOMS = SMO;
                }

                var ZS = zl.ZAP.SelectMany(x => x.Z_SL_list);
                var sanks = ZS.SelectMany(x => x.SANK);
                foreach (var p in sanks)
                {
                    p.S_ZGLV_ID = id;
                }

                fi.FileLog.WriteLn("Загрузка в бд");
                repository.InsertFile(zl, PERS_LIST.LoadFromFile(fi.filel.FilePach));
                var Z_SL = zl.ZAP.SelectMany(x => x.Z_SL_list).ToList();
                fi.FileLog.WriteLn("Установка заголовков санкций");
                var zsl_ZGLV_count = repository.UpdateSLUCH_Z_SANK_ZGLV_ID(Z_SL, id);

                if (Z_SL.Count != zsl_ZGLV_count)
                {
                    repository.Rollback();
                    fi.FileLog.WriteLn($"Не полное внесение SANK_ZGLV_ID для случаев: внесено {zsl_ZGLV_count} из {Z_SL.Count}");
                    return ToBaseFileResult.Create(false, "Не полное внесение SANK_ZGLV_ID для случаев");
                }

                fi.FileLog.WriteLn("Установка указателя на счет в заголовке санкций");
                var zglv_id = zl.ZGLV.ZGLV_ID.Value;
                repository.UpdateSankZGLV(id, Convert.ToInt32(zglv_id));
                repository.Commit();
                return ToBaseFileResult.Create(true);

            }
            catch (Exception ex)
            {
                repository.Rollback();
                fi.FileLog.WriteLn($"Ошибка при переносе в БД: {ex.StackTrace}{ex.Message}");
                return ToBaseFileResult.Create(false, $"Ошибка при переносе в БД: {ex.Message}");
            }

        }
        public ToBaseFileResult ToBaseFileSANK(FileItem fi, bool FLAG_MEE, int YEAR, int MONTH, string SMO, bool DOP_REESTR, bool IsNotFinish, bool IsRewrite, List<FindSluchItem> IDENT_INFO)
        {
            repository.BeginTransaction();
            try
            {
                var zl = ZL_LIST.GetZL_LIST(fi.Version, fi.FilePach);
                zl.SetSUMP();
                var EL = SchemaChecking.GetELEMENTs(fi.FilePach, "FILENAME", "CODE", "CODE_MO", "YEAR", "MONTH");
                //Заголовок санкций
                var id = repository.AddSankZGLV(EL["FILENAME"], Convert.ToInt32(EL["CODE"]), Convert.ToInt32(EL["CODE_MO"]), FLAG_MEE ? 1 : 0, Convert.ToInt32(EL["YEAR"]), Convert.ToInt32(EL["MONTH"]), YEAR, MONTH, fi.ZGLV_ID.Value, SMO, DOP_REESTR, IsNotFinish);

                var rez = repository.LoadSANK(fi, zl, id, !FLAG_MEE, IsRewrite, dispatcher, IDENT_INFO);
                if (rez)
                {
                    repository.Commit();
                    return ToBaseFileResult.Create(true);
                }
                repository.Rollback();
                return ToBaseFileResult.Create(false, "Ошибка загрузки");

            }
            catch (Exception ex)
            {
                repository.Rollback();
                fi.FileLog.WriteLn($"Ошибка при переносе в БД: {ex.Message} {ex.StackTrace}");
                return ToBaseFileResult.Create(false, $"Ошибка при переносе в БД: {ex.Message}");
            }

        }
    }
}
