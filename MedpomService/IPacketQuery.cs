using ServiceLoaderMedpomData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedpomService
{

    public interface IPacketQuery
    {
        event FilePacket.ChangeSiteStatus changeSiteStatus;
        event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Получить список пакетов
        /// </summary>
        /// <returns></returns>
        List<FilePacket> Get();

        FilePacket FindPack(string CODE_MO);
        FilePacket FindPack(Guid guid);
        void AddItem(FileItem item, string CODE_MO);
        void LoadFromFile(string path);
        void SaveToFile(string path);
        FilePacket GetHighPriority();
        FilePacket this[int index] { get; }
        void Clear();
        void DeletePack(FilePacket filePacket);
        int Order(FilePacket filePacket);
        FilePacket AddPacket(FilePacket pack);
        void SetProcessReestr(IProcessReestr ProcessReestr);
    }

    public class PacketQuery : IPacketQuery
    {
        private FilesManager FM { get; set; } = new FilesManager();
        private IPacketCreator PC { get; set; }
        private IProcessReestr ProcessReestr { get; set; }
        private ISchemaCheck SchemaCheck { get; set; }
        FilePacket IPacketQuery.this[int index] => FM[index];
        public void Clear()
        {
            foreach (var item in FM.Get())
            {
                RemovePropertyChanged(item);
            }
            FM.Clear();
        }
        public int Order(FilePacket filePacket)
        {
            return FM.Order(filePacket);
        }
        public PacketQuery(IPacketCreator PC,  ISchemaCheck SchemaCheck)
        {
            this.PC = PC;
            this.ProcessReestr = ProcessReestr;
            this.SchemaCheck = SchemaCheck;
        }
        public void SetProcessReestr(IProcessReestr ProcessReestr)
        {
            this.ProcessReestr = ProcessReestr;
        }
        public event FilePacket.ChangeSiteStatus changeSiteStatus;
        public event PropertyChangedEventHandler PropertyChanged;
        public void LoadFromFile(string path)
        {
            if (File.Exists(path))
            {
                FM.LoadToFile(path);
            }
            foreach (var item in FM.Get())
            {
                AddPropertyChanged(item);
            }
        }
        private void AddPropertyChanged(FilePacket pack)
        {
            pack.changeSiteStatus += CurrentPack_changeSiteStatus;
            pack.PropertyChanged += CurrentPack_PropertyChanged;
            foreach (var fi in pack.Files)
            {
                AddPropertyChanged(fi);
            }
        }
        private void AddPropertyChanged(FileItem item)
        {
            item.PropertyChanged += Item_PropertyChanged;
            if (item.filel != null)
                item.filel.PropertyChanged += Item_PropertyChanged;
        }
        private void RemovePropertyChanged(FilePacket pack)
        {
            pack.changeSiteStatus -= CurrentPack_changeSiteStatus;
            pack.PropertyChanged -= CurrentPack_PropertyChanged;
            foreach (var fi in pack.Files)
            {
                RemovePropertyChanged(fi);
            }
        }
        private void RemovePropertyChanged(FileItem item)
        {
            item.PropertyChanged -= Item_PropertyChanged;
            if (item.filel != null)
                item.filel.PropertyChanged -= Item_PropertyChanged;
        }
        public void SaveToFile(string path)
        {
            FM.SaveToFile(path);
        }
        public FilePacket GetHighPriority()
        {
            return FM.GetIndexHighPriority();
        }
        public List<FilePacket> Get()
        {
            return FM.Get();
        }
        public FilePacket FindPack(string CODE_MO)
        {
            return FM.FindPacket(CODE_MO);
        }

        public FilePacket FindPack(Guid guid)
        {
            return FM.FindPacket(guid);
        }

        private FilePacket NewPacket(string codeMO, DateTime DateCreate)
        {
            var currentPack = new FilePacket()
            {
                CodeMO = codeMO,
                CaptionMO = "[наименование]",
                Date = DateCreate,
                Status = StatusFilePack.Open,
                Files = new List<FileItem>()
            };
            return AddPacket(currentPack);
        }

        public FilePacket AddPacket(FilePacket pack)
        {
            var currentPack = FindPack(pack.CodeMO);
            if (currentPack != null)
            {
                DeletePack(currentPack);
            }
            FM.Add(pack);
            AddPropertyChanged(pack);
            PC.StartAway(pack);
            return pack;
        }

        public void AddItem(FileItem item, string CODE_MO)
        {
            var currentPack = FindPack(CODE_MO) ?? NewPacket(CODE_MO, item.DateCreate);
            if (currentPack.Status != StatusFilePack.Open)
            {
                DeletePack(currentPack);
                AddItem(item, CODE_MO);
            }
            else
            {
                currentPack.Files.Add(item);
                AddPropertyChanged(item);
                currentPack.OnPropertyChanged("Files");
            }
        }
        public void DeletePack(FilePacket filePacket)
        {
            ProcessReestr?.Break(filePacket);
            SchemaCheck.Break(filePacket);
            filePacket.CloserLogFiles();
            FM.Remove(filePacket);
            RemovePropertyChanged(filePacket);
        }
        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }
        private void CurrentPack_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }
        private void CurrentPack_changeSiteStatus(FilePacket fp)
        {
            changeSiteStatus?.Invoke(fp);
        }
    }

}
