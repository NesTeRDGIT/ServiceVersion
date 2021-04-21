﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MedpomService;
using Oracle.ManagedDataAccess.Client;
using ServiceLoaderMedpomData;

namespace MedpomService
{
  


    public interface ISchemaCheck
    {
        void StartCheck(FilePacket fp);
        void StartReCheck(FilePacket fp);
        void Break(FilePacket fp);
        void SetSchemaCollection(SchemaCollection sc);
        void SaveSchemaCollection(string path);
        void LoadSchemaCollection(string path);
        SchemaCollection GetSchemaCollection();

    }

    public class SchemaCheck : ISchemaCheck
    {
        private IRepository mybd { get; set; }
        private IMessageMO messageMo { get; set; }
        private IExcelProtokol excelProtokol { get; set; }
        SchemaCollection SC { get; set; } = new SchemaCollection();
       
        string SvodFileNameXLS = "FileStat.xlsx";
        

        public SchemaCheck(IRepository mybd, IMessageMO messageMo, IExcelProtokol excelProtokol)
        {
            this.mybd = mybd;
            this.messageMo = messageMo;
            this.excelProtokol = excelProtokol;
        }


        private ThreadManager<CheckPackParam> thList = new ThreadManager<CheckPackParam>();

        public void StartCheck(FilePacket fp)
        {
           var th = new Thread(CheckPack) {IsBackground = true};
           var param = new CheckPackParam {isResetLog = false, pack = fp};
           thList.AddListTh(th, param);
           th.Start(param);
        }

        public void StartReCheck(FilePacket fp)
        {
            ClearCatalog(fp);
            var th = new Thread(CheckPack) { IsBackground = true };
            var param = new CheckPackParam { isResetLog = true, pack = fp };
            thList.AddListTh(th, param);
            th.Start(param);
        }

        private void ClearCatalog(FilePacket fp)
        {
            try
            {
                fp.Comment = "Очистка каталога перед проверкой";
                if (!string.IsNullOrEmpty(fp.PATH_STAT))
                {
                    if (File.Exists(fp.PATH_STAT))
                    {
                        File.Delete(fp.PATH_STAT);
                    }

                    fp.PATH_STAT = "";
                }

                if (!string.IsNullOrEmpty(fp.PATH_ZIP))
                {
                    if (File.Exists(fp.PATH_ZIP))
                    {
                        File.Delete(fp.PATH_ZIP);
                    }

                    fp.PATH_STAT = "";
                }

                var fileFLK = Directory.GetFiles(GetCatalogPath(fp), "*FLK.xml");
                foreach (var str in fileFLK)
                {
                    if (File.Exists(str))
                    {
                        File.Delete(str);
                    }
                }

                foreach (var f in fp.Files)
                {
                    f.Comment = "";
                    if (f.filel == null) continue;
                    if (f.Process != StepsProcess.NotInvite)
                    {
                        f.Process = StepsProcess.Invite;
                        f.filel.Process = StepsProcess.Invite;
                    }

                    f.filel.Comment = "";
                }
            }
            catch (Exception ex)
            {
                Logger.AddLog($"Ошибка в ClearCatalog: {ex.Message}", LogType.Error);
            }
        }

        public async void  Break(FilePacket fp)
        {
            await Task.Run(() =>
            {
                var item = thList.Get().FirstOrDefault(x => x.Param.pack == fp);
                item?.Th.Abort();
            });
        }

        public void SetSchemaCollection(SchemaCollection sc)
        {
            SC = sc;
        }

        public void SaveSchemaCollection(string path)
        {
            SC.SaveToFile(path);
        }

        public void LoadSchemaCollection(string path)
        {
            if (File.Exists(path))
            {
                if (!SC.LoadFromFile(path))
                {
                    Logger.AddLog($"Не удалось прочитать {path}", LogType.Error);
                }
                else
                {
                    Logger.AddLog($"Файл XML-схем загружен :{path}", LogType.Information);
                }
            }
            else
            {
                Logger.AddLog($"Файл {path} не найден. Создан новый экземпляр", LogType.Error);
            }
        }

        public SchemaCollection GetSchemaCollection()
        {
            return SC;
        }


        class CheckPackParam
        {
            public FilePacket pack { get; set; }
            public bool isResetLog { get; set; }
        }

        private void CheckPack(object _param)
        {
            var param = (CheckPackParam)_param;
            var pack = param.pack;
            var isResetLog = param.isResetLog;
            try
            {
                if (isResetLog)
                    pack.ResetLogFiles();
                else
                    pack.OpenLogFiles();
                //проверка на файл L
                FindFileL(pack);
                //Проверка на схему
                pack.Comment = "Обработка пакета: Проверка схемы";
                pack.CommentSite = "Проверка схемы документов";
                //Проверка на файл  H

                if (pack.Files.All(x => x.Type != FileType.H))
                    pack.WARNNING = "Отсутствует файл H";



                foreach (var item in pack.Files)
                {
                    if (item.Process == StepsProcess.NotInvite || item.Process == StepsProcess.FlkErr) continue;
                    var vers_file = ReadVersion(item);
                    var vers_file_l = ReadVersion(item.filel);
                    var dt_file = ReadDateFile(item);

                    item.Version = VersionMP.NONE;
                    item.filel.Version = VersionMP.NONE;


                    var FileXSD = FindXSD(SC, vers_file, dt_file, item);
                    var LFileXSD = FindXSD(SC, vers_file_l, dt_file, item.filel);


                    pack.Comment = $"Обработка пакета: Проверка схемы файла {item.FileName}";
                    CheckXSD(item, FileXSD);
                    pack.Comment = $"Обработка пакета: Проверка схемы файла {item.filel.FileName}";
                    CheckXSD(item.filel, LFileXSD);

                    if (item.Process == StepsProcess.ErrorXMLxsd && item.filel.Process == StepsProcess.XMLxsd)
                    {
                        item.filel.Process = StepsProcess.ErrorXMLxsd;
                        item.filel.CommentAndLog = $"Ошибка: Файл владелец({item.FileName}) содержит ошибки в дальнейшей обработке отказано";
                    }
                }

                //Проверка на уникальность кода внутри пакета
                try
                {
                    Check_code_FilePack(pack);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка проверки уникальности кода: {ex.Message}", ex);
                }

                //Проверка уникальности файла

                foreach (var fi in pack.Files)
                {
                    try
                    {
                        if (fi.Process != StepsProcess.XMLxsd) continue;
                        pack.Comment = "Обработка пакета: Проверка уникальности файла";
                        if (!CheckNameFile(fi, true, mybd))
                        {
                            fi.Process = StepsProcess.FlkErr;
                            fi.FileLog.WriteLn("Ошибка проверки имен файла. В приеме файла отказано полностью!!!");
                            fi.filel?.FileLog.WriteLn("Ошибка проверки имен файла. В приеме файла отказано полностью!!!");
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Ошибка при проверке уникальности файла: {ex.Message}", ex);
                    }
                }
                pack.Status = StatusFilePack.XMLSchemaOK;
                pack.Comment = "Обработка пакета: Ожидание ФЛК";
                pack.CommentSite = "Ожидание проверки";
                if (pack.Files.Count(x=>x.Process== StepsProcess.XMLxsd && x.filel.Process== StepsProcess.XMLxsd) == 0)
                {
                    //Закрываем все и фурмируем ошибки
                    pack.Status = StatusFilePack.FLKOK;
                    pack.CloserLogFiles();
                    //Формируем сводный файл
                    excelProtokol.CreateExcelSvod2(pack, Path.Combine(AppConfig.Property.ProcessDir, pack.CodeMO, SvodFileNameXLS), null, null, null);
                    messageMo.CreateErrorMessage(pack);
                    pack.Comment = "Обработка пакета: Завершено";
                    pack.CommentSite = "Завершено";
                }
            }
            catch (Exception ex)
            {
                pack.Status = StatusFilePack.FLKERR;
                pack.CommentSite = "Что то пошло не так...";
                pack.Comment = $"Ошибка проверки схемы: {ex.Message}";
            }
            finally
            {
                pack.CloserLogFiles();
                thList.RemoveTh(Thread.CurrentThread);
            }
        }

        private string ReadVersion(FileItemBase item)
        {
            try
            {
                return SchemaChecking.GetCode_fromXML(item.FilePach, "VERSION");
            }
            catch (Exception ex)
            {
                item.Process = StepsProcess.ErrorXMLxsd;
                item.FileLog.WriteLn($"Ошибка: Не удалось прочитать версию док-та:{ex.Message}");
                item.Comment = $"Ошибка: Не удалось прочитать версию док-та:{ex.Message}";
            }
            return null;
        }

        private DateTime ReadDateFile(FileItem item)
        {
            try
            {
                var year = SchemaChecking.GetCode_fromXML(item.FilePach, "YEAR");
                var month = SchemaChecking.GetCode_fromXML(item.FilePach, "MONTH");
                return new DateTime(Convert.ToInt32(year), Convert.ToInt32(month), 1);
            }
            catch (Exception ex)
            {
                item.Process = StepsProcess.ErrorXMLxsd;
                item.FileLog.WriteLn($"Ошибка: Не удалось прочитать отчетный период док-та:{ex.Message}");
                item.Comment = $"Ошибка: Не удалось прочитать отчетный период док-та:{ex.Message}";
            }
            return DateTime.Now;
        }

        private bool CheckXSD(FileItemBase item, string pathXSD)
        {
            if (!string.IsNullOrEmpty(pathXSD))
            {
                var scchek = new SchemaChecking();
                if (scchek.CheckSchema(item, pathXSD))
                {
                    item.Process = StepsProcess.XMLxsd;
                    return true;
                }
                item.Process = StepsProcess.ErrorXMLxsd;
                item.CommentAndLog = "Ошибка: Файл не соответствует схеме";
                return false;
            }
            return false;
        }

        private string FindXSD(SchemaCollection sc, string version, DateTime dt, FileItemBase item)
        {
            if (!item.Type.HasValue)
            {
                item.Process = StepsProcess.ErrorXMLxsd;
                item.CommentAndLog = $"Не указан тип файла";
                return null;
            }
              
            var sc_file = sc.FindSchema(version, dt, item.Type.Value);
            if (sc_file.Result)
            {
                item.Version = sc_file.Vers;
                return sc_file.Value.Value;
            }

            item.Process = StepsProcess.ErrorXMLxsd;
            item.CommentAndLog = $"Недопустимая версия документа: {sc_file.Exception}";
            return null;

        }
        private void FindFileL(FilePacket pack)
        {

            for (var i = 0; i < pack.Files.Count; i++)
            {
                var fi = pack.Files[i];
                if (fi.filel != null) continue;
                var findfile = fi.FileName;
                switch (fi.Type)
                {
                    case FileType.DD:
                    case FileType.DF:
                    case FileType.DO:
                    case FileType.DP:
                    case FileType.DR:
                    case FileType.DS:
                    case FileType.DU:
                    case FileType.DV:
                    case FileType.H:
                        findfile = "L" + findfile.Remove(0, 1);
                        break;
                    case FileType.T:
                    case FileType.C:
                        findfile = "L" + findfile;
                        break;
                    default:
                        continue;
                }

                var x = pack.Files.FindIndex(F => F.FileName == findfile);
                if (x != -1)
                {
                    fi.Process = StepsProcess.Invite;
                    fi.FileLog.WriteLn("Контроль: Файл персональных данных присутствует");
                    pack.Files[x].Process = StepsProcess.Invite;
                    var h = new FileL
                    {
                        Process = StepsProcess.Invite,
                        FileLog = pack.Files[x].FileLog,
                        FileName = pack.Files[x].FileName,
                        FilePach = pack.Files[x].FilePach,
                        DateCreate = pack.Files[x].DateCreate,
                        Type = pack.Files[x].Type,
                        Comment = pack.Files[x].Comment
                    };
                    fi.filel = h;
                    fi.filel.FileLog.WriteLn($"Контроль: Файл владелец присутствует ({fi.FileName})");
                    pack.Files.Remove(pack.Files[x]);
                    if (x < i) i--;
                }
                else
                {
                    fi.FileLog.WriteLn("Ошибка: Файл персональных данных отсутствует");
                    fi.Comment = ("Ошибка: Файл персональных данных отсутствует");
                }
            }


            foreach (var F in pack.Files)
            {
                if (F.Process == StepsProcess.NotInvite)
                    switch (F.Type)
                    {
                        case FileType.LD:
                        case FileType.LF:
                        case FileType.LO:
                        case FileType.LP:
                        case FileType.LR:
                        case FileType.LS:
                        case FileType.LU:
                        case FileType.LV:
                        case FileType.LH:
                        case FileType.LT:
                        case FileType.LC:
                            F.FileLog.WriteLn("Ошибка: Файл владелец данных отсутствует");
                            F.Comment = ("Ошибка: Файл владелец данных отсутствует");
                            break;
                        default: continue;
                    }

            }
        }

        public void CreateSIGN(FileItemBase item, string catalogSIGN)
        {
            if (!string.IsNullOrEmpty(item.SIGN_BUH))
            {
                if (!Directory.Exists(catalogSIGN))
                    Directory.CreateDirectory(catalogSIGN);
                using (var steam = new StreamWriter(Path.Combine(catalogSIGN, $"{Path.GetFileNameWithoutExtension(item.FilePach)}.BUH.SIG")))
                {
                    steam.Write(item.SIGN_BUH);
                    steam.Close();
                }
            }

            if (!string.IsNullOrEmpty(item.SIGN_ISP))
            {
                if (!Directory.Exists(catalogSIGN))
                    Directory.CreateDirectory(catalogSIGN);
                using (var steam = new StreamWriter(Path.Combine(catalogSIGN, $"{Path.GetFileNameWithoutExtension(item.FilePach)}.ISP.SIG")))
                {
                    steam.Write(item.SIGN_ISP);
                    steam.Close();
                }
            }

            if (!string.IsNullOrEmpty(item.SIGN_DIR))
            {
                if (!Directory.Exists(catalogSIGN))
                    Directory.CreateDirectory(catalogSIGN);
                using (var steam = new StreamWriter(Path.Combine(catalogSIGN, $"{Path.GetFileNameWithoutExtension(item.FilePach)}.DIR.SIG")))
                {
                    steam.Write(item.SIGN_DIR);
                    steam.Close();
                }
            }
        }

        private void Check_code_FilePack(FilePacket fp)
        {
            var ListDouble = new Dictionary<string, List<FileItem>>();
            foreach (var item in fp.Files)
            {
                if (item.Process != StepsProcess.XMLxsd)
                    continue;
                var CODE = SchemaChecking.GetCode_fromXML(item.FilePach, "CODE");

                if (ListDouble.ContainsKey(CODE))
                {
                    ListDouble[CODE].Add(item);
                }
                else
                {
                    ListDouble.Add(CODE, new List<FileItem>());
                    ListDouble[CODE].Add(item);
                }
            }

            foreach (var val in ListDouble)
            {
                if (val.Value.Count <= 1) continue;
                foreach (var fi in val.Value)
                {
                    fi.Comment = "Файл имеет не уникальный CODE";
                    fi.Process = StepsProcess.FlkErr;
                    fi.FileLog.Append();
                    fi.FileLog.WriteLn("Файл имеет не уникальный CODE = " + val.Key);
                    fi.FileLog.WriteLn("Файлы с повтором: " + string.Join(",", val.Value.Where(x => x != fi).Select(x => x.FileName).ToArray()));
                    fi.FileLog.Close();
                }
            }
        }

        private bool CheckNameFile(FileItem _fi, bool checkL, IRepository bd)
        {
            try
            {
                if (checkL)
                {
                    var fi = _fi.filel;


                    var FileName = SchemaChecking.GetCode_fromXML(fi.FilePach, "FILENAME");
                    var FileName1 = SchemaChecking.GetCode_fromXML(fi.FilePach, "FILENAME1");


                    if (FileName.ToUpper() != Path.GetFileNameWithoutExtension(fi.FileName).ToUpper())
                    {
                        fi.FileLog.WriteLn($"Файл {fi.FileName} имеет не корректный FILENAME = {FileName.ToUpper()}");
                        fi.Comment = $"Файл {fi.FileName} имеет не корректный FILENAME = {FileName.ToUpper()}";

                        return false;
                    }
                    if (FileName1.ToUpper() != Path.GetFileNameWithoutExtension(_fi.FileName).ToUpper())
                    {
                        fi.FileLog.WriteLn($"Файл {fi.FileName} имеет не корректный FILENAME1 = {FileName1.ToUpper()}");
                        fi.Comment = $"Файл {fi.FileName} имеет не корректный FILENAME1 = {FileName1.ToUpper()}";
                        return false;
                    }
                }

                var FILENAME = SchemaChecking.GetCode_fromXML(_fi.FilePach, "FILENAME");

                if (FILENAME.ToUpper() != Path.GetFileNameWithoutExtension(_fi.FileName).ToUpper())
                {
                    _fi.FileLog.WriteLn($"Файл {_fi.FileName} имеет не корректный FILENAME = {FILENAME.ToUpper()}");
                    _fi.Comment = $"Файл {_fi.FileName} имеет не корректный FILENAME = {FILENAME.ToUpper()}";
                    // dat.Dispose();
                    return false;
                }

                var tblnames = bd.GetZGLV_BYFileName(FILENAME);
                if (tblnames.Rows.Count != 0)
                {
                    _fi.FileLog.WriteLn($"Файл {_fi.FileName} FILENAME = {FILENAME.ToUpper()}, который присутствует в предыдущих периодах");
                    foreach (DataRow row in tblnames.Rows)
                    {
                        _fi.FileLog.WriteLn($"Файл {row["FileName"]} от {Convert.ToDateTime(row["DSCHET"]).ToShortDateString()}");
                    }
                    _fi.Comment = $"Файл {_fi.FileName} FILENAME = {FILENAME.ToUpper()}, который присутствует в предыдущих периодах";
                    return false;
                }


                var CODE = SchemaChecking.GetCode_fromXML(_fi.FilePach, "CODE");
                var CODE_MO = SchemaChecking.GetCode_fromXML(_fi.FilePach, "CODE_MO");
                var YEAR = SchemaChecking.GetCode_fromXML(_fi.FilePach, "YEAR");

                var tbl_schet = bd.GetSCHET_BYCODE_CODE_MO(Convert.ToInt32(CODE), CODE_MO, Convert.ToInt32(YEAR));
                if (tbl_schet.Rows.Count != 0)
                {
                    _fi.FileLog.WriteLn($"Файл {_fi.FileName} FILENAME = {FILENAME.ToUpper()}, имеет код счета который присутствует в предыдущих периодах code = {CODE}");
                    foreach (DataRow row in tbl_schet.Rows)
                    {
                        _fi.FileLog.WriteLn($"Файл {row["FileName"]} code = {row["code"]} от {Convert.ToDateTime(row["DSCHET"]).ToShortDateString()}");
                    }
                    _fi.Comment = $"Файл {_fi.FileName} FILENAME = {FILENAME.ToUpper()}, имеет код счета который присутствует в предыдущих периодах code = {CODE}";

                    return false;
                }

                var parse = ParseFileName.Parse(FILENAME);
                if (parse.Ni != CODE_MO)
                {
                    _fi.FileLog.WriteLn($"Код МО указанный в наименовании файла не совпадает с тэгом CODE_MO");
                    _fi.Comment = $"Код МО указанный в наименовании файла не совпадает с тэгом CODE_MO";
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.AddLog("Ошибка при проверке на соответствие имен файлов для " + _fi.FileName + ": " + ex.Message, LogType.Error);
                return false;
            }


        }

        string GetCatalogPath(FilePacket pack)
        {
            return Path.Combine(AppConfig.Property.ProcessDir, pack.CodeMO);
        }
    }
}
