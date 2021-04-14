using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;

namespace MedpomService
{
    static class Program
    {
        /// <summary>
        /// Главная точка а для приложения.
        /// </summary>
        static void Main()
        {
          
            var ServicesToRun = new ServiceBase[] 
            { 
                new MedpomService() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
