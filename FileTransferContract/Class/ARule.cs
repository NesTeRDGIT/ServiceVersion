using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileTransferContract.Class;

namespace FileTransferContract.Class
{
    public abstract class ARule
    {
        public abstract TransferRule TransferRule { get; set; }
        public abstract bool isActive { get; }
        public abstract string GUID { get; set; }
    }

}
