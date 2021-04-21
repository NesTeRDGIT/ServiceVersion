using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ionic.Zip;
using ServiceLoaderMedpomData;

namespace MedpomService
{
    public interface IMessageMO
    {
        void CreateErrorMessage(FilePacket pack);
        void CreateMessage(string Message, string FilePath, params string[] Attachment);
    }
    public class MessageMO: IMessageMO
    {
        private string ErrorArchivePath => Path.Combine(AppConfig.Property.ErrorDir, "ARCHIVE", DateTime.Now.ToString("yyyy_MM_dd"));
        private string ErrorPath => Path.Combine(AppConfig.Property.ErrorDir, DateTime.Now.ToString("yyyy_MM_dd"));
        private ILogger Logger;

        public MessageMO(ILogger Logger)
        {
            this.Logger = Logger;
        }

        public void CreateErrorMessage(FilePacket pack)
        {
            // формируем файл для страховых
            var zipF = new ZipFile(Encoding.GetEncoding("cp866"));
            zipF.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
            var dir = new DirectoryInfo(Path.Combine(AppConfig.Property.ProcessDir, pack.CodeMO));
            foreach (var fi in dir.EnumerateFiles("*.log"))
                zipF.AddFile(fi.FullName, "");
            foreach (var fi in dir.EnumerateFiles("*FLK.XML"))
                zipF.AddFile(fi.FullName, "");
            foreach (var fi in dir.EnumerateFiles("*.XLS"))
                zipF.AddFile(fi.FullName, "");

            if (AppConfig.Property.AddDIRInERROR != "")
                if (Directory.Exists(AppConfig.Property.AddDIRInERROR))
                    zipF.AddDirectory(AppConfig.Property.AddDIRInERROR, Path.GetFileNameWithoutExtension(AppConfig.Property.AddDIRInERROR));

            var ZIP_NAME = GetErrorName(pack) + ".zip";
            pack.Comment = "Обработка пакета: Формирование ошибок";


            zipF.Save(Path.Combine(AppConfig.Property.ProcessDir, pack.CodeMO, ZIP_NAME));
            zipF.Dispose();
            pack.PATH_ZIP = Path.Combine(AppConfig.Property.ProcessDir, pack.CodeMO, ZIP_NAME);
            FilesHelper.CopyFileTo(pack.PATH_ZIP, Path.Combine(ErrorArchivePath, Path.GetFileName(pack.PATH_ZIP)));

            if (pack.IST == IST.MAIL)
            {
                var FilePath = Path.Combine(AppConfig.Property.ErrorMessageFile, ZIP_NAME);
                var checkfile = FilePath;
                var x = 1;
                while (File.Exists(checkfile))
                {
                    checkfile = Path.Combine(Path.GetDirectoryName(FilePath), Path.GetFileNameWithoutExtension(FilePath) + "(" + x.ToString() + ")" + Path.GetExtension(FilePath));
                    x++;
                }
                FilePath = checkfile;

                FilesHelper.CopyFileTo(pack.PATH_ZIP, FilePath);
            }
            pack.Comment = "Обработка пакета: Формирование ошибок закончено";
        }

        private string GetErrorName(FilePacket FP)
        {
            return $"{FP.CodeMO} результат приема МП от {DateTime.Now:dd_MM_yyyy HH_mm}";
        }
        private string GetErrorName(MatchParseFileName FP)
        {
            return FP.Ni != null ? $"{FP.Ni} ошибки МП от {DateTime.Now:dd_MM_yyyy HH_mm}" : $"UNKNOWN ошибки МП от {DateTime.Now:dd_MM_yyyy HH_mm}";
        }

        private object Flag;
        public void CreateMessage(string Message, string FilePath, params string[] Attachment)
        {
            Monitor.Enter(Flag);
            try
            {
                var buf = Encoding.UTF8.GetBytes(Message);
                var zfile = new ZipFile(Encoding.GetEncoding("cp866"));
                zfile.AddFiles(Attachment, "");
                zfile.AddEntry("message.txt", buf);
                var checkfile = FilePath;
                var x = 1;
                while (File.Exists(checkfile))
                {
                    checkfile = Path.Combine(Path.GetDirectoryName(FilePath), $"{Path.GetFileNameWithoutExtension(FilePath)}({x}){Path.GetExtension(FilePath)}");
                    x++;
                }
                FilePath = checkfile;

                var pathArchive = Path.Combine(ErrorArchivePath, Path.GetFileName(FilePath));

                if (!Directory.Exists(Path.GetDirectoryName(pathArchive)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(pathArchive));
                }
                zfile.Save(pathArchive);
                zfile.Save(FilePath);
                zfile.Dispose();
            }
            catch (Exception ex)
            {
                Logger.AddLog($"Ошибка при создания сообщения ошибки({Message}){ex.Message}", LogType.Error);
            }
            finally
            {
                Monitor.Exit(Flag);
            }
        }

    }
}
