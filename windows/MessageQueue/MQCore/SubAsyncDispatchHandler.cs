using System;
using Dova.Services;
using Dova.Interfaces;
using System.Collections.Generic;
using Dova.Utility;

namespace Dova.MessageQueue
{

    /// <summary>
    /// 服务转发类
    /// </summary>
    public class SubAsyncDispatchHandler : AsyncDispatchHandler
    {
        static string className = "SubAsyncDispatchHandler"; 

        protected override void AsyncExecute(string serviceName, long pidx, long idx, WQMessage msg)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                PlugingItem service = services[serviceName];
                Log.Write(LogAction.Info, className, "AsyncExecute", serviceName, DateTime.Now.Ticks - t, "准备异步处理:serviceName:" + serviceName + ",service.RoutingGroupName:" + service.RoutingGroupName + ",msg.TransactionID:" + msg.TransactionID + ",pidx:" + pidx + ",idx:" + idx+",SubRoutingKey:"+msg.SubRoutingKey);
                //Dictionary<long, Dictionary<long, string>> groups = ServiceConfigs.ServiceGroups;
                Dictionary<long, string> group = null;
                long subIdx = msg.SubRoutingKey;
                foreach (long pgk in groups.Keys)
                {
                    if ((pidx & pgk) != 0)
                    {
                        group = groups[pgk];
                        foreach (long gk in group.Keys)
                        {
                            if ((subIdx & gk) != 0)
                                AsyncExecuteItem(serviceName, msg, pidx, subIdx, gk, group[gk]);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "AsyncExecute", serviceName, DateTime.Now.Ticks - t, string.Format(errorFormat, serviceName, msg.ToKeyString() + ",pidx:" + pidx + ",idx:" + idx, e.ToString()));
            }
        }


    }
}
