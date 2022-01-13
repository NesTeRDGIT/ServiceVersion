using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ClientServiceWPF.Class
{
    public static class DispatcherHelper
    {
        private static Dispatcher instance = null;
        public static void Init()
        {
            if (instance == null)
                instance = Dispatcher.CurrentDispatcher;
        }
        public static Dispatcher Dispatcher => instance;

    }
}
