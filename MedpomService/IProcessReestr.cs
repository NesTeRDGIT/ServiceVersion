using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SqlServer.Server;
using NPOI.SS.Formula.Functions;
using ServiceLoaderMedpomData;
using ServiceLoaderMedpomData.Annotations;
using ServiceLoaderMedpomData.EntityMP_V31;

namespace MedpomService
{
    public interface IProcessReestr
    {
        bool IsBDInvite();
        void StartBDInvite();
        void StopBDInvite();
        void Break(FilePacket FP);
        CheckingList GetCheckingList();
        void SetCheckingList(CheckingList checkList);
        void SaveCheckingList();
        void LoadCheckingList();
    }


    class ProcessReestr : IProcessReestr
    {
        const string SvodFileNameXLS = "FileStat.xlsx";
        private  CheckingList CheckList { get; set; }= new CheckingList();
        private IPacketQuery PacketQuery { get; set; }
        private readonly IExcelProtokol excelProtokol;
        private readonly IMessageMO messageMo;
        private ILogger Logger;
        public ProcessReestr(IPacketQuery PacketQuery,  IExcelProtokol excelProtokol, IMessageMO messageMo, ILogger Logger)
        {
            this.PacketQuery = PacketQuery;
            this.excelProtokol = excelProtokol;
            this.messageMo = messageMo;
            this.Logger = Logger;
        }
        private CancellationTokenSource BDinviteCancellationTokenSource;
        private Task thWorkProcess;
        private FilePacket DBinvitePac;
        private IRepository mybd { get; set; }
        private  void WorkProcess(CancellationToken cancel)
        {
            try
            {
                while (!cancel.IsCancellationRequested && AppConfig.Property.FILE_ON)
                {
                    var currentpack = PacketQuery.GetHighPriority();
                    if (currentpack != null)
                    {
                        try
                        {
                            if (currentpack.Status == StatusFilePack.XMLSchemaOK)
                            {
                                DBinvitePac = currentpack;
                                currentpack.OpenLogFiles();
                                currentpack.Comment = "Обработка пакета: Перенос данных";
                                currentpack.CommentSite = "Загрузка данных";
                                using (mybd = CreateMyBD())
                                {
                                    try
                                    {
                                        cancel.ThrowIfCancellationRequested();
                                        mybd.TruncALL();
                                    }
                                    catch (CancelException)
                                    {
                                        throw;
                                    }
                                    catch (Exception ex)
                                    {
                                        currentpack.Status = StatusFilePack.FLKERR;
                                        currentpack.CommentSite = "Что то пошло не так...";
                                        Logger.AddLog($"Ошибка при очистки базы перед загрузкой для {currentpack.CodeMO}: {ex.Message}", LogType.Error);
                                        continue;
                                    }

                                    //Проверка на сбой(т.е. во время переноса что то случилось. И теперь считает что они в БД
                                    foreach (var fi in currentpack.Files)
                                    {
                                        if (fi.Process == StepsProcess.FlkOk)
                                        {
                                            fi.Process = StepsProcess.XMLxsd;
                                        }
                                    }

                                    //Загрузка данных в БД

                                    foreach (var fi in currentpack.Files)
                                    {
                                        try
                                        {
                                            cancel.ThrowIfCancellationRequested();
                                            if (fi.Process != StepsProcess.XMLxsd) continue;
                                            currentpack.Comment = $"Обработка пакета: Перенос данных в БД ({fi.FileName})";
                                            fi.Comment = "Перенос файла";
                                            var rez1 = ToBaseFileFULL(fi, mybd);
                                            if (rez1.Result)
                                            {
                                                fi.FullProcess = StepsProcess.FlkOk;
                                                fi.Comment = "Перенос завершен";

                                            }
                                            else
                                            {
                                                fi.FullProcess = StepsProcess.FlkErr;
                                                fi.Comment = "Ошибка переноса";
                                                fi.FileLog.WriteLn(rez1.Exception);
                                            }

                                            GC.Collect();
                                        }
                                        catch (CancelException)
                                        {
                                            throw;
                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.AddLog($"Ошибка при переносе {fi.FileName}: {ex.Message}", LogType.Error);
                                        }
                                    }

                                    Task clearTemp100Task = null;
                                    var ProcessList = currentpack.Files.Where(x => x.Process == StepsProcess.FlkOk && x.filel.Process == StepsProcess.FlkOk).ToList();
                                    cancel.ThrowIfCancellationRequested();
                                    //ОЧИСТКА БАЗЫ ПЕРЕНОСА
                                    if (AppConfig.Property.TransferBD)
                                    {
                                        try
                                        {
                                            clearTemp100Task = mybd.DeleteTemp100TASK(currentpack.CodeMO);

                                        }
                                        catch (Exception ex)
                                        {
                                            Logger.AddLog($"Ошибка при очистки базы переноса для {currentpack.CodeMO}: {ex.Message}", LogType.Error);
                                            throw;
                                        }
                                    }

                                    currentpack.CommentSite = "Проверка";
                                    currentpack.Comment = "Обработка пакета: ФЛК и сбор ошибок";

                                    //Проверяем флк и пишем ЛОГ  


                                    if (ProcessList.Count != 0)
                                    {
                                        foreach (var fi in ProcessList)
                                        {
                                            fi.WriteLnFull(":Начало ФЛК:");
                                        }

                                        cancel.ThrowIfCancellationRequested();
                                        CheckFLK_ALL(mybd, currentpack, cancel);
                                        foreach (var fi in ProcessList)
                                        {
                                            fi.WriteLnFull(":Конец ФЛК:");
                                            fi.WriteLnFull(":ФАЙЛ ПРИНЯТ:");
                                        }
                                    }

                                    currentpack.CommentSite = "Формирование ошибок";
                                    currentpack.Comment = "Обработка пакета: Формирование EXCEL";
                                    //Формируем Excel файл
                                    try
                                    {
                                        if (ProcessList.Count != 0)
                                        {
                                            var err = mybd.GetErrorView();
                                            currentpack.PATH_STAT = Path.Combine(AppConfig.Property.ProcessDir, currentpack.CodeMO, SvodFileNameXLS);
                                            excelProtokol.ExportErrorView(err, currentpack.PATH_STAT);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.AddLog("Ошибка при формировании EXCEL файла:" + ex.Message, LogType.Error);
                                    }

                                    currentpack.Comment = "Обработка пакета: Формирование Свода";

                                    //Формируем сводный файл

                                    excelProtokol.CreateExcelSvod(currentpack, Path.Combine(AppConfig.Property.ProcessDir, currentpack.CodeMO, SvodFileNameXLS), mybd.SVOD_FILE_TEMP99(), mybd.STAT_VIDMP_TEMP99(), mybd.STAT_FULL_TEMP99());
                                    currentpack.CommentSite = "Удаление предыдущей выгрузки";
                                    currentpack.Comment = $"Обработка пакета: Очистка базы {AppConfig.Property.xml_h_zglv_transfer} от {currentpack.CodeMO}";
                                    try
                                    {
                                        clearTemp100Task?.Wait(cancel);
                                    }
                                    catch (AggregateException ex)
                                    {
                                        currentpack.CommentSite = "Что то пошло не так...";
                                        foreach (var e in ex.InnerExceptions)
                                        {
                                            currentpack.Comment = $"При очистке базы {AppConfig.Property.xml_h_zglv_transfer} от {currentpack.CodeMO}: {e.Message}";
                                            Logger.AddLog($"При очистке базы {AppConfig.Property.xml_h_zglv_transfer} от {currentpack.CodeMO}: {e.Message}", LogType.Error);
                                        }

                                        throw new Exception("Ошибка при очистке TEMP100");
                                    }

                                    currentpack.CommentSite = "Сохранение данных";
                                    //Перенос в месячную БД
                                    if (AppConfig.Property.TransferBD)
                                    {
                                        cancel.ThrowIfCancellationRequested();
                                        try
                                        {
                                            foreach (var fi in currentpack.Files)
                                            {
                                                if (fi.Process == StepsProcess.FlkOk && fi.Process == StepsProcess.FlkOk)
                                                {
                                                    fi.WriteLnFull("Начало переноса");
                                                }
                                            }

                                            //L_ZGLV
                                            currentpack.Comment = "Обработка пакета: Перенос L_ZGLV";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_l_zglv, AppConfig.Property.schemaOracle, AppConfig.Property.xml_l_zglv_transfer, AppConfig.Property.schemaOracle_transfer));
                                            //L_PERS
                                            currentpack.Comment = "Обработка пакета: Перенос L_PERS";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_l_pers, AppConfig.Property.schemaOracle, AppConfig.Property.xml_l_pers_transfer, AppConfig.Property.schemaOracle_transfer));
                                            //ZGLV
                                            currentpack.Comment = "Обработка пакета: Перенос ZGLV";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_zglv, AppConfig.Property.schemaOracle, AppConfig.Property.xml_h_zglv_transfer, AppConfig.Property.schemaOracle_transfer));
                                            //SCHET
                                            currentpack.Comment = "Обработка пакета: Перенос SCHET";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_schet, AppConfig.Property.schemaOracle, AppConfig.Property.xml_h_schet_transfer, AppConfig.Property.schemaOracle_transfer));
                                            //ZAP
                                            currentpack.Comment = "Обработка пакета: Перенос ZAP";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_zap, AppConfig.Property.schemaOracle, AppConfig.Property.xml_h_zap_transfer, AppConfig.Property.schemaOracle_transfer));
                                            //PAC
                                            currentpack.Comment = "Обработка пакета: Перенос PACIENT";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_pacient, AppConfig.Property.schemaOracle, AppConfig.Property.xml_h_pacient_transfer, AppConfig.Property.schemaOracle_transfer));

                                            //Z_SLUCH
                                            currentpack.Comment = "Обработка пакета: Перенос Z_SLUCH";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_z_sluch, AppConfig.Property.schemaOracle, AppConfig.Property.xml_h_z_sluch_transfer, AppConfig.Property.schemaOracle_transfer));

                                            //SLUCH
                                            currentpack.Comment = "Обработка пакета: Перенос SLUCH";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_sluch, AppConfig.Property.schemaOracle, AppConfig.Property.xml_h_sluch_transfer, AppConfig.Property.schemaOracle_transfer));
                                            //DS2_N
                                            currentpack.Comment = "Обработка пакета: Перенос DS2_N";
                                            TransResult(mybd.TransferTable(AppConfig.Property.XML_H_DS2_N, AppConfig.Property.schemaOracle, AppConfig.Property.xml_h_ds2_n_transfer, AppConfig.Property.schemaOracle_transfer));
                                            //NAZR
                                            currentpack.Comment = "Обработка пакета: Перенос NAZR";
                                            TransResult(mybd.TransferTable(AppConfig.Property.XML_H_NAZR, AppConfig.Property.schemaOracle, AppConfig.Property.xml_h_nazr_transfer, AppConfig.Property.schemaOracle_transfer));
                                            //KSLP
                                            currentpack.Comment = "Обработка пакета: Перенос KOEF";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_kslp, AppConfig.Property.schemaOracle, AppConfig.Property.xml_h_kslp_transfer, AppConfig.Property.schemaOracle_transfer));

                                            //USL
                                            currentpack.Comment = "Обработка пакета: Перенос USL";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_usl, AppConfig.Property.schemaOracle, AppConfig.Property.xml_h_usl_transfer, AppConfig.Property.schemaOracle_transfer));

                                            //SANK
                                            currentpack.Comment = "Обработка пакета: Перенос SANK";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_sank, AppConfig.Property.schemaOracle, AppConfig.Property.xml_h_sank_smo_transfer, AppConfig.Property.schemaOracle_transfer));

                                            //B_DIAG
                                            currentpack.Comment = "Обработка пакета: Перенос B_DIAG";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_b_diag, AppConfig.Property.schemaOracle, AppConfig.Property.xml_h_b_diag_transfer, AppConfig.Property.schemaOracle_transfer));
                                            //B_PROT
                                            currentpack.Comment = "Обработка пакета: Перенос B_PROT";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_b_prot, AppConfig.Property.schemaOracle, AppConfig.Property.xml_h_b_prot_transfer, AppConfig.Property.schemaOracle_transfer));
                                            //NAPR
                                            currentpack.Comment = "Обработка пакета: Перенос NAPR";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_napr, AppConfig.Property.schemaOracle, AppConfig.Property.xml_h_napr_transfer, AppConfig.Property.schemaOracle_transfer));

                                            //ONK_USL
                                            currentpack.Comment = "Обработка пакета: Перенос ONK_USL";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_onk_usl, AppConfig.Property.schemaOracle, AppConfig.Property.xml_h_onk_usl_transfer, AppConfig.Property.schemaOracle_transfer));

                                            //LEK_PR
                                            currentpack.Comment = "Обработка пакета: Перенос LEK_PR";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_lek_pr, AppConfig.Property.schemaOracle, AppConfig.Property.xml_h_lek_pr_transfer, AppConfig.Property.schemaOracle_transfer));

                                            //LEK_PR_DATE_INJ
                                            currentpack.Comment = "Обработка пакета: Перенос LEK_PR_DATE_INJ";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_date_inj, AppConfig.Property.schemaOracle, AppConfig.Property.xml_h_date_inj_transfer, AppConfig.Property.schemaOracle_transfer));

                                            //CONS
                                            currentpack.Comment = "Обработка пакета: Перенос CONS";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_cons, AppConfig.Property.schemaOracle, AppConfig.Property.xml_h_cons_transfer, AppConfig.Property.schemaOracle_transfer));
                                            //ds2
                                            currentpack.Comment = "Обработка пакета: Перенос DS2";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_ds2, AppConfig.Property.schemaOracle, AppConfig.Property.xml_h_ds2_transfer, AppConfig.Property.schemaOracle_transfer));
                                            //ds3
                                            currentpack.Comment = "Обработка пакета: Перенос DS3";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_ds3, AppConfig.Property.schemaOracle, AppConfig.Property.xml_h_ds3_transfer, AppConfig.Property.schemaOracle_transfer));

                                            //CRIT
                                            currentpack.Comment = "Обработка пакета: Перенос CRIT";
                                            TransResult(mybd.TransferTable(AppConfig.Property.xml_h_crit, AppConfig.Property.schemaOracle, AppConfig.Property.xml_h_crit_transfer, AppConfig.Property.schemaOracle_transfer));

                                            foreach (var fi in ProcessList)
                                            {
                                                fi.WriteLnFull("Перенос завершен");
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            foreach (var fi in ProcessList)
                                            {
                                                fi.WriteLnFull("Ошибка переноса");
                                            }

                                            currentpack.CommentSite = "Что то пошло не так...";
                                            currentpack.Comment = $"Ошибка при переносе в месячную БД: {ex.Message}";
                                            Logger.AddLog($"Ошибка при переносе в месячную БД для {currentpack.CodeMO}: {ex.Message}", LogType.Error);
                                        }

                                    }
                                    else
                                    {
                                        foreach (var fi in ProcessList)
                                        {
                                            fi.WriteLnFull("Перенос не активен");
                                        }
                                    }


                                    GC.Collect();
                                    //Закрываем все и формируемым ошибки
                                    currentpack.CloserLogFiles();
                                    messageMo.CreateErrorMessage(currentpack);
                                    currentpack.Status = StatusFilePack.FLKOK;
                                    currentpack.Comment = "Обработка пакета: Завершено";
                                    currentpack.CommentSite = "Завершено";
                                    //Сохраняем файлы(журнала и работы) на всякий случай.

                                    DBinvitePac = null;
                                }
                            }
                        }
                        catch (CancelException)
                        {
                            currentpack.Status = StatusFilePack.FLKERR;
                            currentpack.CloserLogFiles();
                            messageMo.CreateErrorMessage(currentpack);
                            currentpack.CommentSite = "Обработка прервана пользователем";
                            currentpack.Comment = "Обработка прервана пользователем";
                            Logger.AddLog($"Прерывание потока выполнения {currentpack.CodeMO}", LogType.Error);
                            DBinvitePac = null;
                        }
                        catch (Exception ex)
                        {
                            currentpack.Status = StatusFilePack.FLKERR;
                            currentpack.CommentSite = "Что то пошло не так...";
                            currentpack.Comment = "Ошибка при переносе в БД";
                            Logger.AddLog($"Ошибка при переносе в БД {currentpack.CodeMO} ({ex.Source}:{ex.Message})", LogType.Error);
                            currentpack.CloserLogFiles();
                        }
                        finally
                        {
                            SaveFilesParam();
                        }
                    }

                    var delay = Task.Delay(1000, cancel);
                    delay.Wait(cancel);
                    DBinvitePac = null;
                }
            }
            catch (CancelException)
            {

            }
            catch (Exception ex)
            {
                Logger.AddLog($"Ошибка в потоке ФЛК ({ex.Source}:{ex.Message})", LogType.Error);
            }
            finally
            {
                DBinvitePac = null;
                SaveFilesParam();
            }
        }
        string PathEXE = Process.GetCurrentProcess().MainModule?.FileName;
        void SaveFilesParam()
        {
            var dir = Path.GetDirectoryName(PathEXE);
            try
            {
                PacketQuery.SaveToFile(Path.Combine(dir, "FM.dat"));
            }
            catch (Exception ex)
            {
                Logger.AddLog("Ошибка при сохранении списка пакетов: " + ex.Message, LogType.Error);
            }
        }

        private BoolResult ToBaseFileFULL(FileItem fi, IRepository mybd)
        {
            try
            {
                var zl = ZL_LIST.GetZL_LIST(fi.Version, fi.FilePach);
                zl.SCHET.YEAR_BASE = zl.SCHET.YEAR;
                zl.SCHET.MONTH_BASE = zl.SCHET.MONTH;
                zl.SCHET.YEAR = AppConfig.Property.OtchetDate.Year;
                zl.SCHET.MONTH = AppConfig.Property.OtchetDate.Month;
                mybd.BeginTransaction();
                mybd.InsertFile(zl, PERS_LIST.LoadFromFile(fi.filel.FilePach));
                mybd.Commit();
                return new BoolResult { Result = true };
            }
            catch (Exception ex)
            {
                mybd.Rollback();
                return new BoolResult { Result = false, Exception = $"Ошибка при переносе в БД:{ex.Message}"};
            }
        }

        IRepository CreateMyBD()
        {
            return new MYBDOracleNEW(
                                 AppConfig.Property.ConnectionString,
                                 new TableInfo { TableName = AppConfig.Property.xml_h_zglv, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_schet, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_sank, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_sank_code_exp, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },



                                 new TableInfo { TableName = AppConfig.Property.xml_h_pacient, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_zap, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_usl, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_sluch, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },

                                      new TableInfo { TableName = AppConfig.Property.xml_h_ds2, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_ds3, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_crit, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },

                                 new TableInfo { TableName = AppConfig.Property.xml_h_z_sluch, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_kslp, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.xml_l_zglv, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.xml_l_pers, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.XML_H_DS2_N, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.XML_H_NAZR, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_b_diag, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_b_prot, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_napr, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },

                                 new TableInfo { TableName = AppConfig.Property.xml_h_onk_usl, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_lek_pr, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_date_inj, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 new TableInfo { TableName = AppConfig.Property.xml_h_cons, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },

                                 new TableInfo { TableName = AppConfig.Property.xml_errors, SchemaName = AppConfig.Property.schemaOracle, SeqName = "PACIENT" },
                                 AppConfig.Property.OtchetDate);


        }
        private void TransResult(TransferTableRESULT res)
        {
            if (res == null) return;
            if (res.Colums.Count != 0)
                Logger.AddLog($"Данные таблицы {res.Table} сохраняются не в полном объеме. Потери в {string.Join(",", res.Colums)}", LogType.Error);
        }

        private void CheckFLK_ALL(IRepository bd, FilePacket pack, CancellationToken cancel)
        {
            var cList = new List<TableName> { TableName.ZGLV, TableName.SCHET, TableName.ZAP, TableName.PACIENT, TableName.SLUCH, TableName.USL, TableName.L_ZGLV, TableName.L_PERS, };
            foreach (var tn in cList)
            {
                cancel.ThrowIfCancellationRequested();
                try
                {
                    CheckFLK(bd, pack, tn, cancel);
                }
                catch (Exception ex)
                {
                    pack.WriteAllLog($"Не удалось провести ФЛК {tn}:" + ex.Message);
                }
            }
        }
       
        private void CheckFLK(IRepository bd, FilePacket pack, TableName TN, CancellationToken cancel)
        {
            bd.Checking(TN, CheckList, cancel, ref pack._Comment);
            //Вывод кританувших процедур
            var listEROR = CheckList[TN].FindAll(val => val.Excist == StateExistProcedure.NotExcist && val.STATE);
            foreach (var or in listEROR)
            {
                Logger.AddLog($"Ошибка при выполнении {or.NAME_PROC}({or.Comment}) для {pack.CodeMO}", LogType.Error);
            }
        }
       

        public CheckingList GetCheckingList()
        {
            return CheckList;
        }

        public void SetCheckingList(CheckingList checkList)
        {
            CheckList = checkList;
        }

        public void SaveCheckingList()
        {
            CheckList.SaveToBD(AppConfig.Property.ConnectionString);
        }

        public void LoadCheckingList()
        {
            CheckList.LoadFromBD(AppConfig.Property.ConnectionString);
        }

        public bool IsBDInvite()
        {
            return thWorkProcess != null && thWorkProcess.Status== TaskStatus.Running;
        }

        public void StartBDInvite()
        {
            LoadCheckingList();
            BDinviteCancellationTokenSource = new CancellationTokenSource();
            thWorkProcess = Task.Run(() => { WorkProcess(BDinviteCancellationTokenSource.Token);});
        }

        public void StopBDInvite()
        {
            BDinviteCancellationTokenSource.Cancel();
        }

        public void Break(FilePacket FP)
        {
            if (DBinvitePac == null) return;
            if (DBinvitePac != FP) return;
            Logger.AddLog($"Прерывание потока переноса в БД для {FP.CodeMO}", LogType.Error);
            BDinviteCancellationTokenSource?.Cancel();
            mybd?.Dispose();
            Logger.AddLog($"Ожидание прерывания потока переноса в БД для {FP.CodeMO}", LogType.Error);
            while (thWorkProcess.Status == TaskStatus.Running)
            {
                Task.Delay(1000).Wait();
            }

            DBinvitePac = null;
            StartBDInvite();
            Logger.AddLog("Прерывание успешно", LogType.Error);
        }
    }



}
