using ServiceLoaderMedpomData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedpomService
{
    public partial class WcfInterface
    {
        public FilePacketAndOrder GetPackForMO(string code_mo)
        {
            var t = GetFileManagerList().FirstOrDefault(x => x.CodeMO == code_mo);
            var res = new FilePacketAndOrder() { FP = t, ORDER = PacketQuery.Order(t) };
            return res;
        }

        public void AddFilePacketForMO(FilePacket fp)
        {
            fp.StopTime = true;
            fp.IST = IST.SITE;
            fp.Date = DateTime.Now;
            PacketQuery.AddPacket(fp);
            FileInviter.ToArchive(fp);
        }
    }
}
