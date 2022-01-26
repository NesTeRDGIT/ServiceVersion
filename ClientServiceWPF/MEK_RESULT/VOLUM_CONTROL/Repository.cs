using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClientServiceWPF.Class;
using Oracle.ManagedDataAccess.Client;

namespace ClientServiceWPF.MEK_RESULT.VOLUM_CONTROL
{

    public interface IRepositoryVolumeControl
    {
        /// <summary>
        /// Получить результат контроля лимитов(текущий)
        /// </summary>
        /// <returns></returns>
        List<LIMIT_RESULTRow> GET_VR();
        /// <summary>
        /// Получить результат контроля лимитов(текущий)
        /// </summary>
        Task<List<LIMIT_RESULTRow>> GET_VRAsync();
        /// <summary>
        /// Получить результат контроля лимитов за отчетный период
        /// </summary>
        List<LIMIT_RESULTRow> GET_VR(int year, int month);
        /// <summary>
        /// Получить результат контроля лимитов за отчетный период
        /// </summary>
        Task<List<LIMIT_RESULTRow>> GET_VRAsync(int year, int month);
        /// <summary>
        /// Получить лимиты
        /// </summary>
        List<LIMITRow> GET_LIMITS(int year, int month);
        /// <summary>
        /// Получить лимиты
        /// </summary>
        Task<List<LIMITRow>> GET_LIMITSAsync(int year, int month);
        /// <summary>
        /// Получить процедуру контроля
        /// </summary>
        List<VOLUME_CONTROLRow> GET_VOLUME();
        /// <summary>
        /// Получить процедуру контроля
        /// </summary>
        Task<List<VOLUME_CONTROLRow>> GET_VOLUMEAsync();

        /// <summary>
        /// Получить даты счетов в текущем приема
        /// </summary>
        DateTime? GetSchetDT();
        /// <summary>
        /// Получить даты счетов в текущем приема
        /// </summary>
        Task<DateTime?> GetSchetDTAsync();
        /// <summary>
        /// Получить кол-во санкции контроля объема
        /// </summary>
        int GetCountSANK();
        /// <summary>
        /// Получить кол-во санкции контроля объема
        /// </summary>
        Task<int> GetCountSANKAsync();

        /// <summary>
        /// Синхронизирована ли основная и текущая БД
        /// </summary>
        bool GetIsSyncMainBD(Progress<string> progress);
        /// <summary>
        /// Синхронизирована ли основная и текущая БД
        /// </summary>
        Task<bool> GetIsSyncMainBDAsync(Progress<string> progress);
        /// <summary>
        /// Получить дату акта МЭК
        /// </summary>
        DateTime? GetActDt();
        /// <summary>
        /// Получить дату акта МЭК
        /// </summary>
        Task<DateTime?> GetActDtAsync();
        /// <summary>
        /// Провести контроль объемов
        /// </summary>
        /// <param name="progress"></param>
        void VolumeCheck(Progress<string> progress);
        /// <summary>
        /// Провести контроль объемов
        /// </summary>
        /// <param name="progress"></param>
        Task VolumeCheckAsync(Progress<string> progress);
        /// <summary>
        /// Синхронизация БД
        /// </summary>
        /// <param name="progress"></param>
        void SyncBD(Progress<string> progress);
        /// <summary>
        /// Синхронизация БД
        /// </summary>
        /// <param name="progress"></param>
        Task SyncBDAsync(Progress<string> progress);
        /// <summary>
        /// Установить дату акта
        /// </summary>
        /// <param name="dt"></param>
        void SetActDt(DateTime dt);
        /// <summary>
        /// Установить дату акта
        /// </summary>
        /// <param name="dt"></param>
        Task SetActDtAsync(DateTime dt);
        /// <summary>
        /// Получить статус лимитов
        /// </summary>
        LimitStatus GetLimitStatus(int year,int month);
        /// <summary>
        /// Получить статус лимитов
        /// </summary>
        /// <param name="dt"></param>
        Task<LimitStatus> GetLimitStatusAsync(int year, int month);

        /// <summary>
        /// Расчет лимитов  БД
        /// </summary>
        /// <param name="progress"></param>
        void CalcLimit(int year, int month, Progress<string> progress);
        /// <summary>
        /// Расчет лимитов  БД
        /// </summary>
        /// <param name="progress"></param>
        Task CalcLimitAsync(int year, int month, Progress<string> progress);


        /// <summary>
        /// Получить статус Мэка прошлого периода
        /// </summary>
        /// <param name="progress"></param>
        MekPPStatus GetStatusMekPP(int year, int month);
        /// <summary>
        /// Получить статус Мэка прошлого периода
        /// </summary>
        /// <param name="progress"></param>
        Task<MekPPStatus> GetStatusMekPPAsync(int year, int month);

        /// <summary>
        /// Провести МЭК прошлого периода
        /// </summary>
        /// <param name="progress"></param>
        void RaiseMekPP(int year, int month, Progress<string> progress);
        /// <summary>
        ///  Провести МЭК прошлого периода
        /// </summary>
        /// <param name="progress"></param>
        Task RaiseMekPPAsync(int year, int month, Progress<string> progress);

        /// <summary>
        /// Актировать МЭК прошлого периода
        /// </summary>
        /// <param name="progress"></param>
        void CreateActMekPP(int year, int month, Progress<string> progress);
        /// <summary>
        ///  Провести МЭК прошлого периода
        /// </summary>
        /// <param name="progress"></param>
        Task CreateActMekPPAsync(int year, int month, Progress<string> progress);

        /// <summary>
        /// Отменить МЭК прошлого периода
        /// </summary>
        /// <param name="progress"></param>
        void CancelActMekPP(int year, int month, Progress<string> progress);
        /// <summary>
        ///  Отменить МЭК прошлого периода
        /// </summary>
        /// <param name="progress"></param>
        Task CancelActMekPPAsync(int year, int month, Progress<string> progress);


        /// <summary>
        /// Синхронизировать акты МЭК прошлых периодов
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="progress"></param>
        void SyncDTMEK_P_P(int year, int month, Progress<string> progress);
        /// <summary>
        /// Синхронизировать акты МЭК прошлых периодов
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        Task SyncActMekPPAsync(int year, int month, Progress<string> progress);
    }

    public class RepositoryVolumeControl : IRepositoryVolumeControl
    {
        private  string connectionString { get; }

        public RepositoryVolumeControl(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public List<LIMIT_RESULTRow> GET_VR()
        {
            using (var conn = new OracleConnection(connectionString))
            {
                using (var cmd = new OracleCommand("select * from table(volum_control.GET_VR)", conn))
                {
                    conn.Open();
                    var items = LIMIT_RESULTRow.GetCollection(cmd.ExecuteReader()).ToList();
                    conn.Close();
                    return items;
                }
            }
        }

        public Task<List<LIMIT_RESULTRow>> GET_VRAsync()
        {
            return Task.Run(GET_VR);
        }

        public List<LIMIT_RESULTRow> GET_VR(int year, int month)
        {
            using (var conn = new OracleConnection(connectionString))
            {
                using (var cmd = new OracleCommand("select * from table(volum_control.GET_VRPeriod(:year,:month))", conn))
                {
                    cmd.Parameters.Add("year", year);
                    cmd.Parameters.Add("month", month);
                    conn.Open();
                    var items = LIMIT_RESULTRow.GetCollection(cmd.ExecuteReader()).ToList();
                    conn.Close();
                    return items;
                }
            }
        }

        public Task<List<LIMIT_RESULTRow>> GET_VRAsync(int year,int month)
        {
            return Task.Run(() => GET_VR(year,month));
        }


        public List<LIMITRow> GET_LIMITS(int year, int month)
        {
            using (var conn = new OracleConnection(connectionString))
            {
                using (var cmd = new OracleCommand("select * from table(volum_control.GET_LIMITS(:year,:month))", conn))
                {
                    cmd.Parameters.Add("year", year);
                    cmd.Parameters.Add("month", month);
                    conn.Open();
                    var items = LIMITRow.GetCollection(cmd.ExecuteReader()).ToList();
                    conn.Close();
                    return items;
                }
            }
        }

        public Task<List<LIMITRow>> GET_LIMITSAsync(int year, int month)
        {
            return Task.Run(() => GET_LIMITS(year, month));
        }


        public List<VOLUME_CONTROLRow> GET_VOLUME()
        {
            using (var conn = new OracleConnection(connectionString))
            {
                using (var cmd = new OracleCommand("select * from table(volum_control.GET_VOLUME)", conn))
                {
                    conn.Open();
                    var items = VOLUME_CONTROLRow.GetCollection(cmd.ExecuteReader()).ToList();
                    conn.Close();
                    return items;
                }
            }
        }

        public Task<List<VOLUME_CONTROLRow>> GET_VOLUMEAsync()
        {
            return Task.Run(GET_VOLUME);
        }



        public DateTime? GetSchetDT()
        {
            using (var CONN = new OracleConnection(this.connectionString))
            {
                CONN.Open();
                using (var cmd = new OracleCommand("select VOLUM_CONTROL.SchetDT() from dual", CONN))
                {
                    var obj = cmd.ExecuteScalar();
                    if(obj!=null && obj!=DBNull.Value)
                        return Convert.ToDateTime(cmd.ExecuteScalar());
                    return null;
                }
            }
        }
        public Task<DateTime?> GetSchetDTAsync()
        {
            return Task.Run(GetSchetDT);
        }


        public int GetCountSANK()
        {
            using (var CONN = new OracleConnection(this.connectionString))
            {
                CONN.Open();
                using (var cmd = new OracleCommand("select VOLUM_CONTROL.CountSANK() from dual", CONN))
                {
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        public Task<int> GetCountSANKAsync()
        {
            return Task.Run(()=>GetCountSANK());
        }


        public bool GetIsSyncMainBD(Progress<string> progress)
        {
            OracleCMDWatcher watcher = null;
            try
            {
                using (var con = new OracleConnection(this.connectionString))
                {
                    using (var cmd = new OracleCommand("select volum_control.IsSyncMainBD from dual", con))
                    {
                        con.Open();
                        if (progress != null)
                        {
                            watcher = new OracleCMDWatcher(con, this.connectionString);
                            watcher.StartWatch(500, progress);
                        }
                        return Convert.ToBoolean(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                watcher?.Dispose();
            }
        }
        public Task<bool> GetIsSyncMainBDAsync(Progress<string> progress)
        {
            return Task.Run(()=>GetIsSyncMainBD(progress));
        }



        public DateTime? GetActDt()
        {
            using (var CONN = new OracleConnection(this.connectionString))
            {
                CONN.Open();
                using (var cmd = new OracleCommand("select VOLUM_CONTROL.ACT_DT() from dual", CONN))
                {
                    var obj = cmd.ExecuteScalar();
                    if (obj != null && obj != DBNull.Value)
                    {
                        return Convert.ToDateTime(obj);
                    }
                    return null;
                }
            }
        }
        public Task<DateTime?> GetActDtAsync()
        {
            return Task.Run(GetActDt);
        }
        public void VolumeCheck(Progress<string> progress)
        {
            OracleCMDWatcher watcher = null;
            try
            {
                using (var con = new OracleConnection(this.connectionString))
                {
                    using (var cmd = new OracleCommand("VOLUM_CONTROL.VolumeCheck", con) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        con.Open();
                        if (progress != null)
                        {
                            watcher = new OracleCMDWatcher(con, this.connectionString);
                            watcher.StartWatch(500, progress);
                        }
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                watcher?.Dispose();
            }
        }
        public Task VolumeCheckAsync(Progress<string> progress)
        {
            return Task.Run(()=>VolumeCheck(progress));
        }


        public void SyncBD(Progress<string> progress)
        {
            OracleCMDWatcher watcher = null;
            try
            {
                using (var con = new OracleConnection(this.connectionString))
                {
                    using (var cmd = new OracleCommand("VOLUM_CONTROL.SyncMainBD", con) { CommandType = System.Data.CommandType.StoredProcedure })
                    {
                        con.Open();
                        if (progress != null)
                        {
                            watcher = new OracleCMDWatcher(con, this.connectionString);
                            watcher.StartWatch(500, progress);
                        }
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                watcher?.Dispose();
            }
        }

        public Task SyncBDAsync(Progress<string> progress)
        {
            return Task.Run(() => SyncBD(progress));
        }


        public void SetActDt(DateTime dt)
        {
            using (var con = new OracleConnection(this.connectionString))
            {
                using (var cmd = new OracleCommand($"begin VOLUM_CONTROL.SetActDT('{dt:dd.MM.yyyy}'); end;", con))
                {
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public Task SetActDtAsync(DateTime dt)
        {
            return Task.Run(() => SetActDt(dt));
        }

        public LimitStatus GetLimitStatus(int year, int month)
        {
            var result = new LimitStatus();
            using (var con = new OracleConnection(this.connectionString))
            {
                using (var cmd = new OracleCommand($"select * from table(VOLUM_CONTROL.GetLimitStatus(:year,:month))", con))
                {
                    cmd.Parameters.Add("year", year);
                    cmd.Parameters.Add("month", month);

                    con.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.HasLimit = Convert.ToBoolean(reader[nameof(LimitStatus.HasLimit)]);
                            result.IsBLOCK = Convert.ToBoolean(reader[nameof(LimitStatus.IsBLOCK)]);
                        }
                    }
                    con.Close();
                }
            }
            return result;
        }

        public Task<LimitStatus> GetLimitStatusAsync(int year, int month)
        {
            return Task.Run(() => GetLimitStatus(year, month));
        }

        public void CalcLimit(int year, int month, Progress<string> progress)
        {
            OracleCMDWatcher watcher = null;
            try
            {
                using (var con = new OracleConnection(this.connectionString))
                {
                    using (var cmd = new OracleCommand("begin VOLUM_CONTROL.CalcLimit(:year,:month); end;", con) )
                    {
                        cmd.Parameters.Add("year", year);
                        cmd.Parameters.Add("month", month);
                        con.Open();
                        if (progress != null)
                        {
                            watcher = new OracleCMDWatcher(con, this.connectionString);
                            watcher.StartWatch(500, progress);
                        }
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                watcher?.Dispose();
            }
        }

        public Task CalcLimitAsync(int year, int month, Progress<string> progress)
        {
            return Task.Run(() => CalcLimit(year, month, progress));
        }

        public MekPPStatus GetStatusMekPP(int year, int month)
        {
            var res = new MekPPStatus();
            using (var con = new OracleConnection(this.connectionString))
            {
                using (var cmd = new OracleCommand("select * from table(MEK_P_P.GetStatusMekPP(:year,:month))", con))
                {
                    cmd.Parameters.Add("year", year);
                    cmd.Parameters.Add("month", month);
                    con.Open();
                    var reader = cmd.ExecuteReader();
                    while(reader.Read())
                    {
                        res.ActMekPP = Convert.ToBoolean( reader[nameof(MekPPStatus.ActMekPP)]);
                        res.HasMekDefault = Convert.ToBoolean(reader[nameof(MekPPStatus.HasMekDefault)]);
                        res.HasMekPP = Convert.ToBoolean(reader[nameof(MekPPStatus.HasMekPP)]);
                        if(reader[nameof(MekPPStatus.CurrActPPDt)]!=DBNull.Value)
                            res.CurrActPPDt = Convert.ToDateTime(reader[nameof(MekPPStatus.CurrActPPDt)]);
                    }                   
                    con.Close();
                    return res;
                }
            }
        }

        public Task<MekPPStatus> GetStatusMekPPAsync(int year, int month)
        {
            return Task.Run(() => GetStatusMekPP(year, month));
        }

        public void RaiseMekPP(int year, int month, Progress<string> progress)
        {
            OracleCMDWatcher watcher = null;
            try
            {
                using (var con = new OracleConnection(this.connectionString))
                {
                    using (var cmd = new OracleCommand("begin MEK_P_P.RaiseMekPP; end;", con))
                    {
                        cmd.Parameters.Add("year", year);
                        cmd.Parameters.Add("month", month);
                        con.Open();
                        if (progress != null)
                        {
                            watcher = new OracleCMDWatcher(con, this.connectionString);
                            watcher.StartWatch(500, progress);
                        }
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                watcher?.Dispose();
            }
        }

        public Task RaiseMekPPAsync(int year, int month, Progress<string> progress)
        {
            return Task.Run(() => RaiseMekPPAsync(year, month, progress));
        }

        public void CreateActMekPP(int year, int month, Progress<string> progress)
        {
            OracleCMDWatcher watcher = null;
            try
            {
                using (var con = new OracleConnection(this.connectionString))
                {
                    using (var cmd = new OracleCommand("begin MEK_P_P.ActMek_PP(:year,:month); end;", con))
                    {
                        cmd.Parameters.Add("year", year);
                        cmd.Parameters.Add("month", month);
                        con.Open();
                        if (progress != null)
                        {
                            watcher = new OracleCMDWatcher(con, this.connectionString);
                            watcher.StartWatch(500, progress);
                        }
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                watcher?.Dispose();
            }
        }

        public Task CreateActMekPPAsync(int year, int month, Progress<string> progress)
        {
            return Task.Run(() => CreateActMekPPAsync(year, month, progress));
        }

        public void CancelActMekPP(int year, int month, Progress<string> progress)
        {
            OracleCMDWatcher watcher = null;
            try
            {
                using (var con = new OracleConnection(this.connectionString))
                {
                    using (var cmd = new OracleCommand("begin MEK_P_P.CancelMEK_PP(:year,:month); end;", con))
                    {
                        cmd.Parameters.Add("year", year);
                        cmd.Parameters.Add("month", month);
                        con.Open();
                        if (progress != null)
                        {
                            watcher = new OracleCMDWatcher(con, this.connectionString);
                            watcher.StartWatch(500, progress);
                        }
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                watcher?.Dispose();
            }
        }

        public Task CancelActMekPPAsync(int year, int month, Progress<string> progress)
        {
            return Task.Run(() => CancelActMekPP(year, month, progress));
        }

        public void SyncDTMEK_P_P(int year, int month, Progress<string> progress)
        {
            OracleCMDWatcher watcher = null;
            try
            {
                using (var con = new OracleConnection(this.connectionString))
                {
                    using (var cmd = new OracleCommand("begin MEK_P_P.SincDTMEK_P_P(:year,:month); end;", con))
                    {
                        cmd.Parameters.Add("year", year);
                        cmd.Parameters.Add("month", month);
                        con.Open();
                        if (progress != null)
                        {
                            watcher = new OracleCMDWatcher(con, this.connectionString);
                            watcher.StartWatch(500, progress);
                        }
                        cmd.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                watcher?.Dispose();
            }
        }

        public Task SyncActMekPPAsync(int year, int month, Progress<string> progress)
        {
            return Task.Run(() => SyncDTMEK_P_P(year, month, progress));
        }
    }


    public class LIMIT_RESULTRow
    {
        public static IEnumerable<LIMIT_RESULTRow> GetCollection(IDataReader reader)
        {
            while (reader.Read())
            {
                yield return Get(reader);
            }
        }

        public static LIMIT_RESULTRow Get(IDataReader reader)
        {
            try
            {
                var item = new LIMIT_RESULTRow();
                item.YEAR = Convert.ToInt32(reader[nameof(YEAR)]);
                item.MONTH = Convert.ToInt32(reader[nameof(MONTH)]);
                item.CODE_MO = Convert.ToString(reader[nameof(CODE_MO)]);
                item.NAM_MOK = Convert.ToString(reader[nameof(NAM_MOK)]);
                item.SMO = Convert.ToString(reader[nameof(SMO)]);
                item.NAM_SMOK = Convert.ToString(reader[nameof(NAM_SMOK)]);
                item.RUBRIC = Convert.ToString(reader[nameof(RUBRIC)]);
                item.RUBRIC_NAME = Convert.ToString(reader[nameof(RUBRIC_NAME)]);
                item.KOL = Convert.ToDecimal(reader[nameof(KOL)]);
                item.SUM = Convert.ToDecimal(reader[nameof(SUM)]);


                item.K_MEK_NOT_V = Convert.ToDecimal(reader[nameof(K_MEK_NOT_V)]);
                item.S_MEK_NOT_V = Convert.ToDecimal(reader[nameof(S_MEK_NOT_V)]);
                item.K_P_NOT_V = Convert.ToDecimal(reader[nameof(K_P_NOT_V)]);
                item.S_P_NOT_V = Convert.ToDecimal(reader[nameof(S_P_NOT_V)]);

                item.FOND = Convert.ToDecimal(reader[nameof(FOND)]);
                item.MUR = Convert.ToDecimal(reader[nameof(MUR)]);
                item.FAP = Convert.ToDecimal(reader[nameof(FAP)]);
                item.KOL_LIMIT = Convert.ToDecimal(reader[nameof(KOL_LIMIT)]);
                item.SUM_LIMIT = Convert.ToDecimal(reader[nameof(SUM_LIMIT)]);
                item.K_MEK_VK = Convert.ToDecimal(reader[nameof(K_MEK_VK)]);
                item.S_MEK_VK = Convert.ToDecimal(reader[nameof(S_MEK_VK)]);
                item.K_MEK_VK_RUB = Convert.ToDecimal(reader[nameof(K_MEK_VK_RUB)]);
                item.S_MEK_VK_RUB = Convert.ToDecimal(reader[nameof(S_MEK_VK_RUB)]);
                item.K_MEK_VS = Convert.ToDecimal(reader[nameof(K_MEK_VS)]);
                item.S_MEK_VS = Convert.ToDecimal(reader[nameof(S_MEK_VS)]);
                item.K_MEK_VS_RUB = Convert.ToDecimal(reader[nameof(K_MEK_VS_RUB)]);
                item.S_MEK_VS_RUB = Convert.ToDecimal(reader[nameof(S_MEK_VS_RUB)]);
                item.KOL_P = Convert.ToDecimal(reader[nameof(KOL_P)]);
                item.SUM_P = Convert.ToDecimal(reader[nameof(SUM_P)]);
                item.SUM_MEK_P = Convert.ToDecimal(reader[nameof(SUM_MEK_P)]);
                item.KOL_MEK_P = Convert.ToDecimal(reader[nameof(KOL_MEK_P)]);
                item.MUR_RETURN = Convert.ToDecimal(reader[nameof(MUR_RETURN)]);
                return item;
            }
            catch (Exception e)
            {
                throw new Exception($"Ошибка получения LIMIT_RESULTRow: {e.Message}");
            }
        }

        public bool IsSHOW { get; set; } = true;

        /// <summary>
        /// Год
        /// </summary>
        public int YEAR { get; set; }

        /// <summary>
        /// Месяц
        /// </summary>
        public int MONTH { get; set; }

        /// <summary>
        /// Код МО
        /// </summary>
        public string CODE_MO { get; set; }

        /// <summary>
        /// Наименование МО
        /// </summary>
        public string NAM_MOK { get; set; }

        /// <summary>
        /// СМО
        /// </summary>
        public string SMO { get; set; }

        /// <summary>
        /// Наименование СМО
        /// </summary>
        public string NAM_SMOK { get; set; }

        /// <summary>
        /// Рубрика
        /// </summary>
        public string RUBRIC { get; set; }

        /// <summary>
        /// Наименование рубрики
        /// </summary>
        public string RUBRIC_NAME { get; set; }

        /// <summary>
        /// Кол-во
        /// </summary>
        public decimal KOL { get; set; }

        /// <summary>
        /// Сумма
        /// </summary>
        public decimal SUM { get; set; }

        /// <summary>
        /// Кол-во снятых на МЭК без учета контроля объемов
        /// </summary>
        public decimal K_MEK_NOT_V { get; set; }

        /// <summary>
        /// Сумма снятых на МЭК без учета контроля объемов
        /// </summary>
        public decimal S_MEK_NOT_V { get; set; }

        /// <summary>
        /// Кол-во принятых к оплате без учета контроля объемов
        /// </summary>
        public decimal K_P_NOT_V { get; set; }

        /// <summary>
        /// Сумма принятых к оплате без учета контроля объемов
        /// </summary>
        public decimal S_P_NOT_V { get; set; }

        /// <summary>
        /// Фондодержание
        /// </summary>
        public decimal FOND { get; set; }

        /// <summary>
        /// Муры
        /// </summary>
        public decimal MUR { get; set; }

        /// <summary>
        /// Фондодержание фап
        /// </summary>
        public decimal FAP { get; set; }

        /// <summary>
        /// Количественный лимит
        /// </summary>
        public decimal KOL_LIMIT { get; set; }

        /// <summary>
        /// Стоимостной лимит
        /// </summary>
        public decimal SUM_LIMIT { get; set; }

        /// <summary>
        /// Количество снятых по превышению объемов(количественные показатели)
        /// </summary>
        public decimal K_MEK_VK { get; set; }

        /// <summary>
        /// Сумма снятых по превышению объемов(количественные показатели)
        /// </summary>
        public decimal S_MEK_VK { get; set; }

        /// <summary>
        /// Количество снятых по превышению объемов(количественные показатели)(в рамках рубрики)
        /// </summary>
        public decimal K_MEK_VK_RUB { get; set; }

        /// <summary>
        /// Сумма снятых по превышению объемов(количественные показатели)(в рамках рубрики)
        /// </summary>
        public decimal S_MEK_VK_RUB { get; set; }

        /// <summary>
        /// Количество снятых по превышению объемов(стоимостные показатели)
        /// </summary>
        public decimal K_MEK_VS { get; set; }

        /// <summary>
        /// Сумма снятых по превышению объемов(стоимостные показатели)
        /// </summary>
        public decimal S_MEK_VS { get; set; }

        /// <summary>
        /// Количество снятых по превышению объемов(стоимостные показатели)(в рамках рубрики)
        /// </summary>
        public decimal K_MEK_VS_RUB { get; set; }

        /// <summary>
        /// Сумма снятых по превышению объемов(стоимостные показатели)(в рамках рубрики)
        /// </summary>
        public decimal S_MEK_VS_RUB { get; set; }

        /// <summary>
        /// Принято кол-во
        /// </summary>
        public decimal KOL_P { get; set; }

        /// <summary>
        ///  Принято Сумма
        /// </summary>
        public decimal SUM_P { get; set; }

        /// <summary>
        /// МЭК прошлого периода сумма
        /// </summary>
        public decimal SUM_MEK_P { get; set; }

        /// <summary>
        ///  МЭК прошлого периода сумма количество
        /// </summary>
        public decimal KOL_MEK_P { get; set; }

        /// <summary>
        ///  Возврат муров
        /// </summary>
        public decimal MUR_RETURN { get; set; }

        /// <summary>
        /// Сумма всего
        /// </summary>
        public decimal SUM_ALL => SUM + FOND + FAP;

        /// <summary>
        /// Сумма принято всего
        /// </summary>
        public decimal SUM_P_ALL => SUM_P + FOND + FAP - MUR + MUR_RETURN;

        /// <summary>
        /// Признак формирование акта МЭК
        /// </summary>
        public bool IsACT_MEK { get; set; }

        public bool IsMEK_KOL => K_MEK_VK != 0 || S_MEK_VK != 0;
        public bool IsMEK_SUM => K_MEK_VS != 0 || S_MEK_VS != 0;

        public decimal ProcSUM_P => SUM_ALL == 0 ? 0 : Math.Round(SUM_P_ALL / SUM_ALL * 100, 2);
        public decimal ProcKOL_P => KOL == 0 ? 0 : Math.Round(KOL_P / KOL * 100, 2);
    }

    public class VOLUME_CONTROLRow
    {
        public static IEnumerable<VOLUME_CONTROLRow> GetCollection(IDataReader rows)
        {
            while (rows.Read())
            {
                yield return Get(rows);
            }
        }

        public static VOLUME_CONTROLRow Get(IDataReader row)
        {
            try
            {
                var item = new VOLUME_CONTROLRow();
                item.MEK_SUM = Convert.ToBoolean(row[nameof(MEK_SUM)]);
                item.SUM_ITOG = Convert.ToDecimal(row[nameof(SUM_ITOG)]);
                item.MEK_KOL = Convert.ToBoolean(row[nameof(MEK_KOL)]);
                item.KOL_ITOG = Convert.ToDecimal(row[nameof(KOL_ITOG)]);
                item.FOND = Convert.ToDecimal(row[nameof(FOND)]);
                item.MUR = Convert.ToDecimal(row[nameof(MUR)]);
                item.FOND_SCOR = Convert.ToDecimal(row[nameof(FOND_SCOR)]);
                item.KOL_LIMIT = Convert.ToDecimal(row[nameof(KOL_LIMIT)]);
                item.SUM_LIMIT = Convert.ToDecimal(row[nameof(SUM_LIMIT)]);
                item.CODE_MO = Convert.ToString(row[nameof(CODE_MO)]);
                item.NAM_MOK = Convert.ToString(row[nameof(NAM_MOK)]);
                item.SMO = Convert.ToString(row[nameof(SMO)]);
                item.NAM_SMOK = Convert.ToString(row[nameof(NAM_SMOK)]);
                item.SLUCH_Z_ID = Convert.ToInt32(row[nameof(SLUCH_Z_ID)]);
                item.RUBRIC_ID = Convert.ToString(row[nameof(RUBRIC_ID)]);
                item.NAME_RUB = Convert.ToString(row[nameof(NAME_RUB)]);
                item.RUBRIC_ID_NEW = Convert.ToString(row[nameof(RUBRIC_ID_NEW)]);
                item.NAME_RUB_NEW = Convert.ToString(row[nameof(NAME_RUB_NEW)]);
                item.KOL = Convert.ToDecimal(row[nameof(KOL)]);
                item.SUM = Convert.ToDecimal(row[nameof(SUM)]);
                item.DATE_Z_1 = Convert.ToDateTime(row[nameof(DATE_Z_1)]);
                item.DATE_Z_2 = Convert.ToDateTime(row[nameof(DATE_Z_2)]);
                item.IDCASE = Convert.ToString(row[nameof(IDCASE)]);
                return item;
            }
            catch (Exception e)
            {

                throw new Exception($"Ошибка получения VOLUME_CONTROL: {e.Message}");
            }
        }


        public bool IsSHOW { get; set; } = true;
        public bool MEK_SUM { get; set; }
        public decimal SUM_ITOG { get; set; }
        public bool MEK_KOL { get; set; }
        public decimal KOL_ITOG { get; set; }
        public decimal FOND { get; set; }
        public decimal MUR { get; set; }
        public decimal FOND_SCOR { get; set; }
        public decimal KOL_LIMIT { get; set; }
        public decimal SUM_LIMIT { get; set; }
        public string CODE_MO { get; set; }
        public string NAM_MOK { get; set; }

        public string SMO { get; set; }
        public string NAM_SMOK { get; set; }
        public int SLUCH_Z_ID { get; set; }
        public string RUBRIC_ID { get; set; }
        public string NAME_RUB { get; set; }
        public string RUBRIC_ID_NEW { get; set; }
        public string NAME_RUB_NEW { get; set; }
        public decimal KOL { get; set; }
        public decimal SUM { get; set; }
        public DateTime DATE_Z_1 { get; set; }
        public DateTime DATE_Z_2 { get; set; }
        public string IDCASE { get; set; }


    }
    
    public class LIMITRow
    {
        public static IEnumerable<LIMITRow> GetCollection(IDataReader rows)
        {
            while (rows.Read())
            {
                yield return Get(rows);
            }
        }

        public static LIMITRow Get(IDataReader row)
        {
            try
            {
                var item = new LIMITRow();
                item.YEAR = Convert.ToInt32(row[nameof(YEAR)]);
                item.MONTH = Convert.ToInt32(row[nameof(MONTH)]);
                item.CODE_MO = Convert.ToString(row[nameof(CODE_MO)]);
                item.NAM_MOK = Convert.ToString(row[nameof(NAM_MOK)]);
                item.SMO = Convert.ToString(row[nameof(SMO)]);
                item.NAM_SMOK = Convert.ToString(row[nameof(NAM_SMOK)]);
                item.VOLUM_RUBRIC_ID = Convert.ToString(row[nameof(VOLUM_RUBRIC_ID)]);
                item.NAME_RUB = Convert.ToString(row[nameof(NAME_RUB)]);
                item.KOL_ISP = Convert.ToDecimal(row[nameof(KOL_ISP)]);
                item.SUM_ISP = Convert.ToDecimal(row[nameof(SUM_ISP)]);
                item.KOL_M = Convert.ToDecimal(row[nameof(KOL_M)]);
                item.SUM_M = Convert.ToDecimal(row[nameof(SUM_M)]);
                item.KOL_Q1 = Convert.ToDecimal(row[nameof(KOL_Q1)]);
                item.SUM_Q1 = Convert.ToDecimal(row[nameof(SUM_Q1)]);
                item.KOL_Q2 = Convert.ToDecimal(row[nameof(KOL_Q2)]);
                item.SUM_Q2 = Convert.ToDecimal(row[nameof(SUM_Q2)]);
                item.KOL_Q3 = Convert.ToDecimal(row[nameof(KOL_Q3)]);
                item.SUM_Q3 = Convert.ToDecimal(row[nameof(SUM_Q3)]);
                item.KOL_Q4 = Convert.ToDecimal(row[nameof(KOL_Q4)]);
                item.SUM_Q4 = Convert.ToDecimal(row[nameof(SUM_Q4)]);
                item.KOL_Y = Convert.ToDecimal(row[nameof(KOL_Y)]);
                item.SUM_Y = Convert.ToDecimal(row[nameof(SUM_Y)]);
                item.IsISP = Convert.ToBoolean(row[nameof(IsISP)]);
                item.IsYEAR = Convert.ToBoolean(row[nameof(IsYEAR)]);
                item.IsQ = Convert.ToBoolean(row[nameof(IsQ)]);
                item.IsLast = Convert.ToBoolean(row[nameof(IsLast)]);
                return item;
            }
            catch (Exception e)
            {
                throw new Exception($"Ошибка получения LIMITRow: {e.Message}");
            }
        }

        public bool IsShow { get; set; } = true;
        public int YEAR { get; set; }
        public int MONTH { get; set; }
        public string CODE_MO { get; set; }
        public string NAM_MOK { get; set; }
        public string SMO { get; set; }
        public string NAM_SMOK { get; set; }
        public string VOLUM_RUBRIC_ID { get; set; }
        public string NAME_RUB { get; set; }

        public decimal KOL_ISP { get; set; }
        public decimal SUM_ISP { get; set; }

        public decimal KOL_M { get; set; }
        public decimal SUM_M { get; set; }

        public decimal KOL_Q1 { get; set; }
        public decimal SUM_Q1 { get; set; }

        public decimal KOL_Q2 { get; set; }
        public decimal SUM_Q2 { get; set; }

        public decimal KOL_Q3 { get; set; }
        public decimal SUM_Q3 { get; set; }

        public decimal KOL_Q4 { get; set; }
        public decimal SUM_Q4 { get; set; }

        public decimal KOL_Y { get; set; }
        public decimal SUM_Y { get; set; }

        public bool IsYEAR { get; set; }
        public bool IsQ { get; set; }
        public bool IsISP { get; set; }

        public bool IsLast { get; set; }


        public string VID_STR => IsYEAR ? "Годовой" : "Квартальный";

        public string ErrComment
        {
            get
            {
                var Err = new List<string>();
                if (NotVID)
                    Err.Add("2 вида контроля(годовой и квартальный)");
                if (IsLess0)
                    Err.Add("Числа меньше нуля");
                if (IsErrKV_KOL_Y)
                    Err.Add("Сумма квартальных лимитов(кол-во) не равна годовой сумме");
                if (IsErrKV_SUM_Y)
                    Err.Add("Сумма квартальных лимитов(сумма) не равна годовой сумме");
                if (IsKOL_Merr)
                    Err.Add("Месячный лимит(кол-во) превышает квартальный или годовой");
                if (IsSUM_Merr)
                    Err.Add("Месячный лимит(сумма) превышает квартальный лимит или годовой");
                if (IsISP_KOLerr)
                    Err.Add("Исполнение(кол-во) превышает квартальный лимит или годовой");
                if (IsISP_SUMerr)
                    Err.Add("Исполнение(сумма) превышает квартальный лимит или годовой");
                if (IsISP_LIM_SUMerr)
                    Err.Add("Лимит + исполнено (Сумма) не равно квартальному лимиту или годовому");
                if (IsISP_LIM_KOLerr)
                    Err.Add("Лимит + исполнено (Кол-во) не равно квартальному лимиту или годовому");

                return string.Join(Environment.NewLine, Err);
            }
        }



        public bool IsErr => IsErrKV_KOL_Y || IsErrKV_SUM_Y || IsKOL_Merr || IsSUM_Merr || IsLess0 || NotVID || IsISP_KOLerr || IsISP_SUMerr || IsISP_LIM_SUMerr || IsISP_LIM_KOLerr;

        /// <summary>
        /// 2 вида контроля(годовой и квартальный)
        /// </summary>
        public bool NotVID => IsYEAR == IsQ;

        /// <summary>
        /// Числа меньше нуля
        /// </summary>
        public bool IsLess0 => KOL_ISP < 0 || SUM_ISP < 0 || KOL_M < 0 || SUM_M < 0 || KOL_Q1 < 0 || SUM_Q1 < 0 || KOL_Q2 < 0 || SUM_Q2 < 0 || KOL_Q3 < 0 || SUM_Q3 < 0 || KOL_Q4 < 0 || SUM_Q4 < 0 || KOL_Y < 0 || SUM_Y < 0;

        /// <summary>
        /// Сумма квартальных лимитов(кол-во) не равна годовой сумме
        /// </summary>
        public bool IsErrKV_KOL_Y => Math.Round(KOL_Q1 + KOL_Q2 + KOL_Q3 + KOL_Q4, 2) != KOL_Y;

        /// <summary>
        /// Сумма квартальных лимитов(сумма) не равна годовой сумме
        /// </summary>
        public bool IsErrKV_SUM_Y => Math.Round(SUM_Q1 + SUM_Q2 + SUM_Q3 + SUM_Q4, 2) != SUM_Y;

        /// <summary>
        /// Месячный лимит(кол-во) превышает квартальный или годовой
        /// </summary>
        public bool IsKOL_Merr
        {
            get
            {
                if (IsQ && KOL_M > GetKV_KOL(MONTH))
                {
                    return true;
                }

                if (IsYEAR && KOL_M > KOL_Y && !IsISP) return true;
                return false;
            }
        }

        /// <summary>
        /// Месячный лимит(сумма) превышает квартальный лимит или годовой
        /// </summary>
        public bool IsSUM_Merr
        {
            get
            {
                if (IsQ && SUM_M > GetKV_SUM(MONTH))
                {
                    return true;
                }

                if (IsYEAR && SUM_M > SUM_Y && !IsISP) return true;
                return false;
            }
        }

        /// <summary>
        /// Исполнение(кол-во) превышает квартальный лимит или годовой
        /// </summary>
        public bool IsISP_KOLerr
        {
            get
            {
                if (IsQ && KOL_ISP > GetKV_KOL(MONTH))
                {
                    return true;
                }

                if (IsYEAR && KOL_ISP > KOL_Y) return true;
                return false;
            }
        }

        /// <summary>
        /// Исполнение(сумма) превышает квартальный лимит или годовой
        /// </summary>
        public bool IsISP_SUMerr
        {
            get
            {
                if (IsQ && SUM_ISP > GetKV_SUM(MONTH))
                {
                    return true;
                }

                if (IsYEAR && SUM_ISP > SUM_Y) return true;
                return false;
            }
        }

        /// <summary>
        /// Лимит + исполнено (сумма) не равно квартальному лимиту или годовому
        /// </summary>
        /// <returns></returns>
        public bool IsISP_LIM_SUMerr
        {
            get
            {
                if (IsLast)
                {
                    if (IsQ && Math.Round((GetKV_SUM(MONTH) - SUM_ISP) / Delimetr, 2, MidpointRounding.AwayFromZero) != SUM_M)
                    {
                        return true;
                    }

                    if (IsYEAR && decimal.Round(SUM_Y - SUM_ISP, 2, MidpointRounding.AwayFromZero) != SUM_M) return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Лимит + исполнено (Кол-во) не равно квартальному лимиту или годовому
        /// </summary>
        /// <returns></returns>
        public bool IsISP_LIM_KOLerr
        {
            get
            {
                if (IsLast)
                {
                    if (IsQ && Math.Round((GetKV_KOL(MONTH) - KOL_ISP) / Delimetr, 0, MidpointRounding.AwayFromZero) != KOL_M)
                    {
                        return true;
                    }

                    if (IsYEAR && Math.Round(KOL_Y - KOL_ISP, 0, MidpointRounding.AwayFromZero) != KOL_M) return true;
                }

                return false;
            }
        }

        private decimal GetKV_KOL(int Month)
        {
            switch (Month)
            {
                case 1:
                case 2:
                case 3: return KOL_Q1;

                case 4:
                case 5:
                case 6: return KOL_Q1 + KOL_Q2;

                case 7:
                case 8:
                case 9: return KOL_Q1 + KOL_Q2 + KOL_Q3;

                case 10:
                case 11:
                case 12: return KOL_Q1 + KOL_Q2 + KOL_Q3 + KOL_Q4;
                default: return 0;
            }

        }

        private decimal GetKV_SUM(int Month)
        {
            switch (Month)
            {
                case 1:
                case 2:
                case 3: return SUM_Q1;

                case 4:
                case 5:
                case 6: return SUM_Q1 + SUM_Q2;

                case 7:
                case 8:
                case 9: return SUM_Q1 + SUM_Q2 + SUM_Q3;

                case 10:
                case 11:
                case 12: return SUM_Q1 + SUM_Q2 + SUM_Q3 + SUM_Q4;
                default: return 0;
            }

        }

        private decimal Delimetr
        {
            get
            {
                switch (MONTH)
                {
                    case 1:
                    case 4:
                    case 7:
                    case 10:
                        return 3;
                    case 2:
                    case 5:
                    case 8:
                    case 11:
                        return 2;
                    case 3:
                    case 6:
                    case 9:
                    case 12:
                        return 1;
                    default: return 0;
                }
            }
        }
    }

    public class LimitStatus
    {
        public bool IsBLOCK { get; set; }
        public bool HasLimit { get; set; }
    }

    public class MekPPStatus
    {
        public bool HasMekPP { get; set; }
        public bool HasMekDefault { get; set; }
        public bool ActMekPP { get; set; }
        public DateTime? CurrActPPDt { get; set; }
    }
}
