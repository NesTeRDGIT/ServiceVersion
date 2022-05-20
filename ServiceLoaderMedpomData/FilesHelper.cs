using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace ServiceLoaderMedpomData
{
    public enum PrichinAv
    {
        EXEPT,
        NOT_FOUND
    }
    public static class FilesHelper
    {
        public static bool CheckFileAv(string path, int NOT_FOUND_COUNT = 1, ILogger Logger = null)
        {
            var NOT_FOUND = 0;
            PrichinAv? pr;
            while (!CheckFileAv(path, out pr))
            {
                if (pr == PrichinAv.NOT_FOUND)
                {
                    Logger?.AddLog($"Не удалось найти файл {path}",LogType.Error);
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
        public static bool MoveFile(FileItemBase item, string catalog, int NOT_FOUND_COUNT = 1, ILogger Logger = null)
        {
            var NOT_FOUND = 0;
            int step1 = 0;
            while (true)
            {
                try
                {
                    step1++;
                    File.SetAttributes(item.FilePach, FileAttributes.Normal);
                    step1++;
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
                        Logger?.AddLog($"Ошибка создания директории: {ex1.Message} для {item.FileName}", LogType.Error);
                    }

                    if (NOT_FOUND == NOT_FOUND_COUNT)
                    {
                        Logger?.AddLog($"Ошибка переноса файла {item.FileName}({item.FilePach}): {ex.GetType()} {ex.FullError()}|step={step1}", LogType.Error);
                        item.Comment = $"Обработка пакета: Перенос файла {item.FileName}: {ex.GetType()} {ex.FullError()}";
                        return false;
                    }
                    Task.Delay(5000).Wait();
                }
            }
            return true;
        }
        /// <summary>
        /// Перенос файла
        /// </summary>
        /// <param name="From">Источник</param>
        /// <param name="Dist">Направление</param>
        /// <param name="Renamed">Переименовать файл, если в направлении уже существует такой файл(FILENAME(n))</param>
        /// <returns></returns>
        public static string MoveFileTo(string From, string Dist, bool Renamed=true)
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
                if (!Renamed)
                    throw new Exception($"Ошибка переноса файла {From} в {path}. Файл уже присутствует");
                prefix = $"({x})";
                x++;
                path = $"{dir_dist}\\{filename_dist}{prefix}{ext_dist}";
            }

            while (!CheckFileAv(From)) { };
            File.Move(From, path);
            return path;
        }

        /// <summary>
        /// Удаление директории(с несколькими попытками)
        /// </summary>
        /// <param name="dir">Директория</param>
        /// <param name="Count">Кол-во попыток</param>
        /// <param name="TimeOut">Время между попытками(МС)</param>
        /// <returns></returns>
        public static async Task<bool> DeleteDirectoryAsync(string dir, int Count = 3, int TimeOut = 500)
        {
            if (!Directory.Exists(dir)) throw new Exception($"Директория {dir} не существует");
            var Counttry = 0;
            while (true)
                try
                {
                    Directory.Delete(dir, true);
                    return true;
                }
                catch (Exception)
                {
                    Counttry++;
                    if (Counttry >= Count)
                        throw;
                    await Task.Delay(TimeOut);
                }

            throw new Exception($"Директория {dir} не существует");
        }

        /// <summary>
        /// Копирование файла с проверкой на совпадение и если совпадает то будет имя_файла(1).. итд
        /// </summary>
        /// <param name="From">Откуда</param>
        /// <param name="Dist">Куда</param>        
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
            while (!CheckFileAv(From)) { };
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
        public static async Task<BoolResult> FilesExtract(string From, string To)
        {
            var ArchiveName = Path.GetFileName(From);
            var tmppathMain = Path.Combine(To, Path.GetRandomFileName());
            try
            {
                using (var arc = ZipFile.Open(From, ZipArchiveMode.Read, Encoding.GetEncoding("cp866")))
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
                            Exception = $"Ошибка создания временной директории при распаковке файла {ArchiveName}: {ex.Message}"
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
                            return new BoolResult
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
                        var t =await  FilesExtract(str, tmppathMain);
                        if (t.Result == false)
                            return t;
                        File.Delete(str);
                    }
                }

                //Переносим обратно
                try
                {
                    var filestmp = Directory.GetFiles(tmppathMain, "*.*", SearchOption.TopDirectoryOnly);
                    foreach (var name in filestmp.Where(x => Path.GetDirectoryName(x)?.ToUpper() != ".TMP"))
                    {
                        MoveFileTo(name, Path.Combine(To, Path.GetFileName(name)));
                    }
                }
                catch (Exception ex)
                {
                    return new BoolResult
                    {
                        Result = false,
                        Exception = $"Ошибка при переносе файлов архива {ArchiveName}: {ex.Message}"
                    };
                }
                finally
                {
                    await DeleteDirectoryAsync(tmppathMain);
                }
                return new BoolResult { Result = true };
            }
            catch (Exception ex)
            {
                return new BoolResult { Result = false, Exception = $"Ошибка при извлечении архива {ArchiveName}: {ex.Message}" };
            }

        }

        private static string NormalizePath(string path)
        {
            while (path.Length != 0 && path[path.Length - 1] == '\\')
            {
                path = path.Remove(path.Length - 1, 1);
            }
            return path.ToUpper();
        }
        /// <summary>
        /// Удалить файл и ветку его каталогов, если они пусты
        /// </summary>
        /// <param name="rootFolder">Каталог до которого происходит удаление</param>
        /// <param name="p">Имя файла или каталога</param>
        public static void RemoveFileAndDir(string rootFolder, params string[] paths)
        {
            rootFolder = NormalizePath(rootFolder);
            foreach (var p in paths)
            {
                RemoveFileAndDirInner(rootFolder, NormalizePath(p));
            }
        }

        public static Task RemoveFileAndDirAsync(string rootFolder, params string[] paths)
        {
            return Task.Run(() => RemoveFileAndDir(rootFolder, paths));

        }

        private static void RemoveFileAndDirInner(string rootFolder, string p)
        {
            if (!p.StartsWith(p, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Путь не в базовом каталоге");
            }
            if (File.Exists(p))
            {
                File.Delete(p);
            }
            var dir = Path.GetDirectoryName(p);
            if (dir != rootFolder)
            {
                if (Directory.Exists(dir))
                {
                    if (Directory.GetFiles(dir).Length == 0 && Directory.GetDirectories(dir).Length == 0)
                    {
                        Directory.Delete(dir);
                        RemoveFileAndDirInner(rootFolder, dir);
                    }
                }
            }
        }


    }
}
