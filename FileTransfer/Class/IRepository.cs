using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FileTransfer.Class
{
    public interface IRepository
    {
         void Save(IEnumerable<TransferRule> items);
         IEnumerable<TransferRule> Load();
    }


    public class FileRepository : IRepository
    {
        private string  FilePath { get; set; }

        public FileRepository(string FilePath)
        {
            this.FilePath = FilePath;
        }
        public void Save(IEnumerable<TransferRule> items)
        {
            using (var st = new StreamWriter(FilePath,false))
            {
                var ser = new XmlSerializer(typeof(List<TransferRule>));
                ser.Serialize(st, items.ToList());
            }
        }

        public IEnumerable<TransferRule> Load()
        {
            using (var st = new StreamReader(FilePath))
            {
                var ser = new XmlSerializer(typeof(List<TransferRule>));
                return (List<TransferRule>)ser.Deserialize(st);
            }
        }
    }


}
