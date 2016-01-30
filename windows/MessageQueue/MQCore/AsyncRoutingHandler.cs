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
    public class AsyncRoutingHandler : RoutingProxyHandler
    {
        static string className = "AsyncRoutingHandler";
        public delegate void AsyncDelegate(string serviceName, long pidx, long idx, WQMessage msg);
        static bool openAsyncDispatchThread = ServiceConfigs.OpenAsyncDispatchThread;
        static AsyncRoutingHandler()
        {
            if (openAsyncDispatchThread)//开启修补程序
            {

            }
        }


        /// <summary>
        /// 同步模式:处理
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public override object ExecuteRequest(string serviceName, object msg)
        {
            string srvName = Routing(serviceName, msg).GroupName;
            using (IDisposable service = ServiceFactory<IMessageService>.GetServer(srvName).Instance() as IDisposable)
            {
                return (service as IMessageService).AsyncRequest(serviceName, msg);
            }
        }

        /// <summary>
        /// 同步模式:处理
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public override object DoExecuteMessageRequest(string serviceName, WQMessage msg, bool async)
        {
            //1. 保存DB,返回msg.TranscactionID
            //2. 异步调用同步方法
            //3. 开启守护线程修补失败的
            Log.Write(LogAction.Info, className, "DoExecuteMessageRequest", serviceName, -1, string.Format(startFormat, serviceName, msg.TransactionID));
            Routing routing = Routing(serviceName, msg);
            if (null == routing || routing.EmptyHandlerName.Length > 0)
            {
                return EmptyRouting(serviceName, msg, async);
            }
            int rtn = -1;
            msg.ServiceName = serviceName;
            long pidx = routing.ParentGroupIndex;
            long idx = routing.GroupIndex;
            rtn = MessageDao.Save(serviceName, msg, (int)DealStatus.Dealing, pidx, idx);
            Log.Write(LogAction.Info, className, "DoExecuteMessageRequest", serviceName, -1, msg.TransactionID + ",保存消息返回值:" + rtn);
            if (rtn > 0)
            {
                AsyncDelegate ad = new AsyncDelegate(AsyncExecute);
                ad.BeginInvoke(serviceName, pidx, idx, msg, null, null);
            }
            return (rtn > 0);

        }

        //protected virtual void AsyncExecute( string serviceName, long pidx, long idx, DovaMessage msg)
        //{
        //    long t = DateTime.Now.Ticks; 
        //    try
        //    {
        //        PlugingItem service = services[serviceName]; 
        //        Log.Write(LogAction.Info, className, "AsyncExecute:"+serviceName, DateTime.Now.Ticks - t, "准备异步处理:serviceName:" + serviceName + ",service.RoutingGroupName:" + service.RoutingGroupName + ",msg.TransactionID:" + msg.TransactionID + ",pidx:" + pidx + ",idx:" + idx);
        //        Dictionary<long, Dictionary<long, string>> groups = ServiceConfigs.ServiceGroups;
        //        Dictionary<long, string> group = null;
        //        foreach (long pgk in groups.Keys)
        //        {
        //            if ((pidx & pgk) != 0)
        //            {
        //                group = groups[pgk];
        //                foreach (long gk in group.Keys)
        //                {
        //                    if((idx& gk)!=0)
        //                    AsyncExecuteItem(serviceName, msg, idx, gk, group[gk]);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Write(LogAction.Error, className, "AsyncExecute:Error:" + serviceName, DateTime.Now.Ticks - t, string.Format(errorFormat, serviceName, msg.ToKeyString() + ",pidx:" + pidx + ",idx:" + idx, e.ToString()));
        //    }
        //}

        //void AsyncExecuteItem(string serviceName, DovaMessage msg, long done, long dealing, string groupName)
        //{
        //    bool ret;
        //    long t = DateTime.Now.Ticks;
        //    Log.Write(LogAction.Info, className, "AsyncExecuteItem:Start:" + serviceName, DateTime.Now.Ticks - t, string.Format(startFormat, serviceName, msg.TransactionID));
        //    using (IDisposable service = ServiceFactory<IMessageService>.GetServer(groupName).Instance() as IDisposable)
        //    {
        //        Log.Write(LogAction.Info, className, "AsyncExecuteItem:" + serviceName, DateTime.Now.Ticks - t, "获取路由目标服务:" + groupName + ",msg:" + msg.TransactionID);
        //        ret = (service as IMessageService).AsyncRequestMessage(serviceName, msg);
        //        Log.Write(LogAction.Info, className, "AsyncExecuteItem:" + serviceName, DateTime.Now.Ticks - t, "异步路由代理转发:" + groupName + ":" + serviceName + ",返回值:" + ret + ",msg:" + msg.ToKeyString());
        //    }
        //    if (ret)
        //    {
        //        MessageDao.SaveStatus(msg.TransactionID, dealing, done);
        //    }
        //    Log.Write(LogAction.Info, className, "AsyncExecuteItem:End:" + serviceName, DateTime.Now.Ticks - t, string.Format(startFormat, serviceName, msg.TransactionID + ",要求分发处理的Index集合:" + done + ",当前处理服务Index:" + dealing));
        //} 

    }
}
