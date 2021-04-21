using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using ServiceLoaderMedpomData;

namespace MedpomService
{
    public partial class WcfInterface
    {
        public ServiceLoaderMedpomData.Version GetVersion()
        {
            try
            {
                var ver = new ServiceLoaderMedpomData.Version();
                var curr_dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                if (File.Exists(Path.Combine(curr_dir, "CLIENT_UPDATE", "version.xml")))
                {

                    ver.LoadFromFile(Path.Combine(curr_dir, "CLIENT_UPDATE", "version.xml"));
                    return ver;
                }

                throw new FaultException("Файл версии не найден на сервере!");
            }
            catch (Exception ex)
            {
                AddLog("GetVersion: " + ex.Message, LogType.Error);
                throw new FaultException("Ошибка в GetVersion подробнее в логах сервиса");
            }
        }


        public byte[] LoadFileUpdate(FileAndMD5 file, int offset, int count)
        {
            try
            {
                var curr_dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                var FilePath = Path.Combine(curr_dir, "CLIENT_UPDATE", file.Name);
                if (File.Exists(FilePath))
                {
                    Stream st = File.OpenRead(FilePath);


                    var buffer = new byte[count];
                    st.Position = offset;
                    var readByte = st.Read(buffer, 0, count);

                    if (readByte < count)
                    {
                        return GetPartByte(buffer, 0, readByte);
                    }
                    st.Close();
                    return buffer;
                }
                else
                {
                    throw new FaultException("Файл не найден на сервере!");
                }

            }
            catch (Exception ex)
            {

                AddLog("LoadFileUpdate: " + ex.Message, LogType.Error);
                throw new FaultException("Ошибка в LoadFileUpdate подробнее в логах сервиса");
            }
        }

        public byte[] GetFile(string CODE_MO, int FILE, TypeDOWLOAD type, long offset)
        {
            try
            {
                var PAC = PacketQuery.FindPack(CODE_MO);
                string PATH;
                switch (type)
                {
                    case TypeDOWLOAD.File:
                        PATH = PAC.Files[FILE].FilePach; break;
                    case TypeDOWLOAD.FileL:
                        PATH = PAC.Files[FILE].filel.FileName; break;
                    case TypeDOWLOAD.FILE_STAT:
                        PATH = PAC.PATH_STAT; break;
                    case TypeDOWLOAD.FILE_LOG:
                        PATH = PAC.Files[FILE].FileLog.FilePath; break;
                    case TypeDOWLOAD.FILE_L_LOG:
                        PATH = PAC.Files[FILE].filel.FileLog.FilePath; break;
                    case TypeDOWLOAD.XML_OTCHET_H:
                        PATH = PAC.Files[FILE].PATH_LOG_XML; break;
                    case TypeDOWLOAD.XML_OTCHET_L:
                        PATH = PAC.Files[FILE].filel.PATH_LOG_XML; break;
                    case TypeDOWLOAD.ZIP_ARCHIVE:
                        PATH = PAC.PATH_ZIP; break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }

                var count = 1024 * 1024 * 3;
                var buff = new byte[count];
                using (Stream st = File.Open(PATH, FileMode.Open))
                {
                    st.Position = offset;
                    var readByte = st.Read(buff, 0, count);
                    st.Close();
                    return readByte < count ? GetPartByte(buff, 0, readByte) : buff;
                }
            }
            catch (Exception ex)
            {
                AddLog($"GetFile{CODE_MO}: {ex.Message}", LogType.Error);
                throw new FaultException("Ошибка передачи файла");
            }

        }
        public long GetFileLength(string CODE_MO, int FILE, TypeDOWLOAD type)
        {
            try
            {
                var PAC = PacketQuery.FindPack(CODE_MO);
                string PATH;
                switch (type)
                {
                    case TypeDOWLOAD.File:
                        PATH = PAC.Files[FILE].FilePach; break;
                    case TypeDOWLOAD.FileL:
                        PATH = PAC.Files[FILE].filel.FileName; break;
                    case TypeDOWLOAD.FILE_STAT:
                        PATH = PAC.PATH_STAT; break;
                    case TypeDOWLOAD.FILE_LOG:
                        PATH = PAC.Files[FILE].FileLog.FilePath; break;
                    case TypeDOWLOAD.FILE_L_LOG:
                        PATH = PAC.Files[FILE].filel.FileLog.FilePath; break;
                    case TypeDOWLOAD.XML_OTCHET_H:
                        PATH = PAC.Files[FILE].PATH_LOG_XML; break;
                    case TypeDOWLOAD.XML_OTCHET_L:
                        PATH = PAC.Files[FILE].filel.PATH_LOG_XML; break;
                    case TypeDOWLOAD.ZIP_ARCHIVE:
                        PATH = PAC.PATH_ZIP; break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(type), type, null);
                }

                Stream st = File.Open(PATH, FileMode.Open);
                var rsl = st.Length;
                st.Close();
                return rsl;
            }
            catch (Exception ex)
            {
                AddLog($"GetFileLength: {ex.Message}", LogType.Error);
                throw new FaultException("Ошибка при передачи файла");
            }

        }
        private byte[] GetPartByte(byte[] owner, int from, int to)
        {
            var list = new List<byte>();
            for (var i = from; i < to; i++)
                list.Add(owner[i]);
            return list.ToArray();
        }

    }
}
