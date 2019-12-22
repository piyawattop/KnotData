using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace KnotBackgroundService
{

    static class Program
    {
#if DEBUG
        [STAThread]
#endif
        static void Main()
        {
#if DEBUG
            KnotService ser = new KnotService();
            ser.OnDebug();
            System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);
#else
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new KnotService()
            };
            ServiceBase.Run(ServicesToRun);
#endif
        }
    }
}
