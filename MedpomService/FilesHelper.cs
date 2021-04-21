using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServiceLoaderMedpomData;


namespace MedpomService
{
    public static class FilesHelper
    {
        public enum PrichinAv
        {
            EXEPT,
            NOT_FOUND
        }
        public static bool CheckFileAv(string path, int NOT_FOUND_COUNT = 1)
        {
            var NOT_FOUND = 0;
            PrichinAv? pr;
            while (!CheckFileAv(path, out pr))
            {
                if (pr == PrichinAv.NOT_FOUND)
                {
                    Logger.AddLog($"Не удалось найти файл {path}",LogType.Error);
                }
                NOT_FOUND++;
                if (NOT_FOUND >= NOT_FOUND_COUNT)
                {
                    return false;
                }
                Thread.Sleep(1500);
            }
            return true;

        }

        public static bool CheckFileAv(string path, out PrichinAv? Pr)
        {
            Stream stream = null;
            try
            {
                stream = File.Open(path, FileMode.Open, FileAccess.Read);
                stream.Close();
                stream.Dispose();
                Pr = null;
                return true;
            }
            catch (FileNotFoundException)
            {
                Pr = PrichinAv.NOT_FOUND;
                stream?.Dispose();
                return false;
            }
            catch (Exception)
            {
                Pr = PrichinAv.EXEPT;
                stream?.Dispose();
                return false;
            }
        }


     
        public static bool MoveFile(FileItemBase item, string catalog, int NOT_FOUND_COUNT = 1)
        {
            var NOT_FOUND = 0;
            while (true)
            {
                try
                {
                    File.SetAttributes(item.FilePach, FileAttributes.Normal);
                    File.Move(item.FilePach, Path.Combine(catalog, item.FileName));
                    break;
                }
                catch (Exception ex)
                {
                    NOT_FOUND++;
                    item.Comment = $"Обработка пакета: Перенос файла {item.FileName}: {ex.GetType()} {ex.FullError()}";
                    try
                    {
                        if (!Directory.Exists(catalog))
                            Directory.CreateDirectory(catalog);
                    }
                    catch (Exception ex1)
                    {
                        Logger.AddLog($"Ошибка создания директории: {ex1.Message} для {item.FileName}", LogType.Error);
                    }

                    if (NOT_FOUND == NOT_FOUND_COUNT)
                    {
                        Logger.AddLog($"Ошибка переноса файла {item.FileName}({item.FilePach}): {ex.GetType()} {ex.FullError()}", LogType.Error);
                        item.Comment = $"Обработка пакета: Перенос файла {item.FileName}: {ex.GetType()} {ex.FullError()}";
                        return false;
                    }
                    Thread.Sleep(5000);
                }
            }
            return true;
        }

        public static string MoveFileTo(string From, string Dist)
        {
            var prefix = "";
            var x = 1;
            var dir_dist = Path.GetDirectoryName(Dist);
            var filename_dist = Path.GetFileNameWithoutExtension(Dist);
            var ext_dist = Path.GetExtension(Dist);
            var path = $"{dir_dist}\\{filename_dist}{prefix}{ext_dist}";

            if (!Directory.Exists(Path.GetDirectoryName(Dist)))
                Directory.CreateDirectory(dir_dist);


            while (File.Exists(path))
            {
                prefix = $"({x})";
                x++;
                path = $"{dir_dist}\\{filename_dist}{prefix}{ext_dist}";
            }

            while (!SchemaChecking.CheckFileAv(From)) { };
            File.Move(From, path);
            return path;
        }

        public static string CopyFileTo(string From, string Dist)
        {
            if (!Directory.Exists(Path.GetDirectoryName(Dist)))
                Directory.CreateDirectory(Path.GetDirectoryName(Dist));
            var prefix = "";
            var x = 1;
            var dir_dist = Path.GetDirectoryName(Dist);
            var filename_dist = Path.GetFileNameWithoutExtension(Dist);
            var ext_dist = Path.GetExtension(Dist);
            var path = $"{dir_dist}\\{filename_dist}{prefix}{ext_dist}";
            while (File.Exists(path))
            {
                prefix = $"({x})";
                x++;
                path = $"{dir_dist}\\{filename_dist}{prefix}{ext_dist}";
            }
            while (!SchemaChecking.CheckFileAv(From)) { };
            File.Copy(From, path);
            return path;
        }

        public static string[] GetFileNameInArchive(string path)
        {
            try
            {
                using (var arc = ZipFile.Open(path, ZipArchiveMode.Read, Encoding.GetEncoding("cp866")))
                {
                    return arc.Entries.Select(x => x.Name).ToArray();
                }
            }
            catch (Exception)
            {
                return new string[0];
            }

        }

        public static BoolResult FilesExtract(string From, string To)
        {
            var ArchiveName = Path.GetFileName(From);
            var tmppathMain = Path.Combine(To, Path.GetRandomFileName());
            try
            {
                using (var arc = System.IO.Compression.ZipFile.Open(From, ZipArchiveMode.Read, Encoding.GetEncoding("cp866")))
                {
                    try
                    {
                        if (Directory.Exists(tmppathMain))
                        {
                            Directory.Delete(tmppathMain, true);
                        }

                        Directory.CreateDirectory(tmppathMain);
                    }
                    catch (Exception ex)
                    {
                        return new BoolResult
                        {
                            Result = false,
                            Exception = $"Ошибка при распаковке файла {ArchiveName}: {ex.Message}"
                        };
                    }
                    foreach (var entry in arc.Entries.Where(x => x.CompressedLength != 0))
                    {
                        try
                        {
                            entry.ExtractToFile(Path.Combine(tmppathMain, entry.Name), true);
                        }
                        catch (Exception ex)
                        {
                            return new BoolResult()
                            {
                                Result = false,
                                Exception = $"Ошибка при распаковке файла {ArchiveName}: {ex.Message}"
                            };
                        }
                    }
                }

                string[] files;
                while ((files = Directory.GetFiles(tmppathMain, "*.zip", SearchOption.TopDirectoryOnly)).Length != 0)
                {
                    foreach (var str in files)
                    {
                        var t = FilesExtract(str, tmppathMain);
                        if (t.Result == false)
                            return t;
                        File.Delete(str);
                    }
                }

                //Переносим обратно
                try
                {
                    var filestmp = Directory.GetFiles(tmppathMain, "*.*", SearchOption.TopDirectoryOnly);
                    foreach (var name in filestmp)
                    {
                        ServiceLoaderMedpomData.FilesManager.MoveFileTo(name, Path.Combine(To, Path.GetFileName(name)));
                    }
                    Directory.Delete(tmppathMain, true);
                }
                catch (Exception ex)
                {
                    Directory.Delete(tmppathMain, true);
                    return new BoolResult()
                    {
                        Result = false,
                        Exception = $"Ошибка при переносе файлов архива {ArchiveName}: {ex.Message}"
                    };
                }
                return new BoolResult() { Result = true };
            }
            catch (Exception ex)
            {
                // WcfInterface.AddLog("Ошибка при извлечении архива " + file.Name + ":" + ex.Message, EventLogEntryType.Error);
                //return false;
                return new BoolResult()
                { Result = false, Exception = $"Ошибка при извлечении архива {ArchiveName}: {ex.Message}" };
            }

        }
    }
}
