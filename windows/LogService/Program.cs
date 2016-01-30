using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace LogService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun;
            string serverName = "LogService1";
            if ((args != null) && (args.Length > 0))
            {
                serverName = args[0];
            }
            ServicesToRun = new ServiceBase[] 
            { 
                new Service1() { ServiceName=serverName }
            };
            ServiceBase.Run(ServicesToRun); 
        }
    }
}
