using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
//using Ionic.Zip;
using ServiceLoaderMedpomData;
using ZipFile = System.IO.Compression.ZipFile;

namespace ClientServise
{
    class Class1
    {

        public static BoolResult FilesExtract(string From, string To)
        {
            var ArchiveName = Path.GetFileName(From);
            var tmppathMain = Path.Combine(To, Path.GetRandomFileName());
            try
            {
                using (var arc = ZipFile.Open(From, ZipArchiveMode.Read, Encoding.ASCII))
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
                    foreach (var entry in arc.Entries.Where(x=>x.CompressedLength!=0))
                    {
                        try
                        {
                            entry.ExtractToFile(Path.Combine(tmppathMain,entry.Name),true);
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
                    //WcfInterface.AddLog("Ошибка при переносе файлов архива " + file.Name + ": " + ex.Message, EventLogEntryType.Error);
                    Directory.Delete(tmppathMain, true);
                    //return false;
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
