
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace Dova.MQService
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            int type = ServiceConfigs.ServiceType;
            ServiceBase[] ServicesToRun = null;
            if (type == 1)
            {
                ServicesToRun = new ServiceBase[] 
                { 
                    new SyncMessageService() 
                };
            }
            //else if (type == 2)
            //{
            //    ServicesToRun = new ServiceBase[] 
            //    { 
            //        new MQConsumerService() 
            //    };
            //}
            //else
            //{
            //    ServicesToRun = new ServiceBase[] 
            //    { 
            //        new SyncMessageService(),
            //        new MQConsumerService() 
            //    };
            //}
            ServiceBase.Run(ServicesToRun);
        }
    }
}



 
