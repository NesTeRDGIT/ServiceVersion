using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using ServiceLoaderMedpomData;

namespace ClientServiceWPF.SANK_INVITER
{
    public interface ICheckSchema
    {
        bool CheckSchemaFileItem(FileItem item);
    }

    public class CheckSchema: ICheckSchema
    {
        private Dispatcher dispatcher;
        private SchemaCollection scoll { get; set; }
        public string LocalFolder => AppDomain.CurrentDomain.BaseDirectory;
        public CheckSchema(Dispatcher dispatcher, SchemaCollection scoll)
        {
            this.dispatcher = dispatcher;
            this.scoll = scoll;
        }
     
        public bool CheckSchemaFileItem(FileItem item)
        {
            var result = true;
            try
            {
                item.FileLog.Append();
                item.filel?.FileLog.Append();

                var vers_file_l = SchemaChecking.GetELEMENT(item.filel.FilePach, "VERSION");
                var elements = SchemaChecking.GetELEMENTs(item.FilePach, "VERSION", "YEAR", "MONTH");
                var vers_file = elements["VERSION"];
                var dt_file = new DateTime(Convert.ToInt32(elements["YEAR"]), Convert.ToInt32(elements["MONTH"]), 1);

                item.Version = VersionMP.NONE;
                var sc_file = scoll.FindSchema(vers_file, dt_file, item.Type.Value);
                var sc_filel = scoll.FindSchema(vers_file_l, dt_file, item.filel.Type.Value);

                dispatcher.Invoke(() =>
                {
                    if (sc_file.Result)
                    {
                        item.Version = sc_file.Vers;
                    }
                    else
                    {
                        item.Process = StepsProcess.ErrorXMLxsd;
                        item.CommentAndLog = $"Недопустимая версия документа: {sc_file.Exception}";
                        CreateError(Path.Combine(Path.GetDirectoryName(item.FileLog.FilePath), $"{Path.GetFileNameWithoutExtension(item.FileLog.FilePath)}FLK.xml"), item.FileName, "VERSION", item.Comment);
                        result = false;
                    }

                    if (sc_filel.Result)
                    {
                        item.filel.Version = sc_filel.Vers;
                    }
                    else
                    {
                        item.filel.Process = StepsProcess.ErrorXMLxsd;
                        item.filel.CommentAndLog = $"Недопустимая версия документа: {sc_filel.Exception}";
                        item.CommentAndLog = item.filel.Comment;
                        CreateError(Path.Combine(Path.GetDirectoryName(item.filel.FileLog.FilePath), $"{Path.GetFileNameWithoutExtension(item.filel.FileLog.FilePath)}FLK.xml"), item.filel.FileName, "VERSION", item.Comment);
                        result = false;
                    }
                });
                var sc = new SchemaChecking();

                Dictionary<string, PacientInfo> PInfo = null;
                //Проверка файла перс
                if (item.filel != null)
                {
                    if (item.filel.Version != VersionMP.NONE)
                    {
                        var res = sc.CheckSchema(item.filel, Path.Combine(LocalFolder, sc_filel.Value.Value));
                        PInfo = sc.P_INFO;
                        dispatcher.Invoke(() =>
                        {
                            if (res)
                            {
                                item.filel.Process = StepsProcess.XMLxsd;
                                item.filel.Comment = "Схема правильная";
                            }
                            else
                            {
                                item.filel.Comment = "Схема ошибочна";
                                item.filel.Process = StepsProcess.ErrorXMLxsd;
                                result = false;
                                if (item.Process == StepsProcess.XMLxsd)
                                {
                                    item.Comment = "Схема ошибочна в файле перс данных";
                                    item.Process = StepsProcess.ErrorXMLxsd;
                                }
                                else
                                {
                                    item.Comment += " Схема ошибочна в файле перс данных";
                                    item.Process = StepsProcess.ErrorXMLxsd;
                                }
                            }
                        });
                    }
                }

                //проверка основного файла
                if (item.Version != VersionMP.NONE)
                {
                    var res = sc.CheckSchema(item, Path.Combine(LocalFolder, sc_file.Value.Value), false, PInfo);
                    var protot = sc.GetProtokol;
                    if (protot.Count(x => string.IsNullOrEmpty(x.ERR_CODE) || x.ERR_CODE != "C_ZAB_EMPTY") == 0)
                    {
                        res = true;
                    }
                    dispatcher.Invoke(() =>
                    {
                        if (res)
                        {
                            item.Process = StepsProcess.XMLxsd;
                            item.CommentAndLog = "Схема правильная";
                        }
                        else
                        {
                            item.Process = StepsProcess.ErrorXMLxsd;
                            item.CommentAndLog = "Схема ошибочна";
                            result = false;
                        }
                    });
                }

                return result;
            }
            finally
            {
                item.FileLog.Close();
                item.filel?.FileLog.Close();
            }
        }

        private void CreateError(string pathToXml, string FileName, string BAS_EL, string Comment)
        {
            SchemaChecking.XMLfileFLK(pathToXml, FileName, new List<ErrorProtocolXML> { new ErrorProtocolXML { BAS_EL = BAS_EL, Comment = Comment } });
        }

    }
}
