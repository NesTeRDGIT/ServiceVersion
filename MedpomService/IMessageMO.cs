using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ServiceLoaderMedpomData;
using System.Linq;

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
            var fileToArchive = new List<ZipArchiverEntry>();
            
            var dir = new DirectoryInfo(Path.Combine(AppConfig.Property.ProcessDir, pack.CodeMO));
            fileToArchive.AddRange(dir.EnumerateFiles("*.log").Select(fi => new ZipArchiverEntry(fi.FullName)));
            fileToArchive.AddRange(dir.EnumerateFiles("*FLK.XML").Select(fi => new ZipArchiverEntry(fi.FullName)));
            fileToArchive.AddRange(dir.EnumerateFiles("*.XLS").Select(fi => new ZipArchiverEntry(fi.FullName)));

            if (!string.IsNullOrEmpty(AppConfig.Property.AddDIRInERROR) && Directory.Exists(AppConfig.Property.AddDIRInERROR))
                fileToArchive.Add(new ZipArchiverEntry(AppConfig.Property.AddDIRInERROR));

            var ZIP_NAME = $"{GetErrorName(pack)}.zip";
            pack.Comment = "Обработка пакета: Формирование ошибок";
             
            var pathZip = Path.Combine(AppConfig.Property.ProcessDir, pack.CodeMO, ZIP_NAME);
            FilesHelper.CreateArchive(pathZip, fileToArchive);
            pack.PATH_ZIP = pathZip;
            FilesHelper.CopyFileTo(pack.PATH_ZIP, Path.Combine(ErrorArchivePath, Path.GetFileName(pack.PATH_ZIP)));

            if (pack.IST == IST.MAIL)
            {
                var FilePath = Path.Combine(AppConfig.Property.ErrorMessageFile, ZIP_NAME);
                var checkfile = FilePath;
                var x = 1;
                while (File.Exists(checkfile))
                {
                    checkfile = Path.Combine(Path.GetDirectoryName(FilePath), $"{Path.GetFileNameWithoutExtension(FilePath)}({x}){Path.GetExtension(FilePath)}");
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
     
        private object Flag = new object();
        public void CreateMessage(string Message, string FilePath, params string[] Attachment)
        {
            Monitor.Enter(Flag);
            try
            {
                var x = 1;
                var checkfile = FilePath;
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
                FilesHelper.CreateMessageArchive(pathArchive, Message, Attachment.Select(at => new ZipArchiverEntry(at)));
                FilesHelper.CreateMessageArchive(FilePath, Message, Attachment.Select(at => new ZipArchiverEntry(at)));
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
