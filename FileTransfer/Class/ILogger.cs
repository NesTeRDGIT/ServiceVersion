using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileTransfer.Class
{
    public interface ILogger
    {
        void Add(string text, bool isError);
    }
    
}
