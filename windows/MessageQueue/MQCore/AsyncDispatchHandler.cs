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
    public class AsyncDispatchHandler : MessageHandler
    {
        static string className = "AsyncDispatchHandler";
        public delegate void AsyncDelegate(string serviceName, long pidx, long idx, WQMessage msg);

        /// <summary>
        /// 同步模式:处理
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public override object ExecuteRequest(string serviceName, object msg)
        {
             using (IDisposable service = ServiceFactory<IMessageService>.GetServer(services[serviceName].GroupName).Instance() as IDisposable)
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
            int rtn = -1;
            msg.ServiceName = serviceName;
            PlugingItem service = services[serviceName];
            long pidx = service.ParentGroupIndex;
            long idx = service.GroupIndex;
            rtn = MessageDao.Save(serviceName, msg, (int)DealStatus.Dealing,pidx,idx);
            Log.Write(LogAction.Info, className, "DoExecuteMessageRequest", serviceName, -1, msg.TransactionID + ",保存消息返回值:" + rtn);
            if (rtn > 0)
            {
                AsyncDelegate ad = new AsyncDelegate(AsyncExecute);
                ad.BeginInvoke(serviceName, pidx, idx, msg, null, null);
            }
            return (rtn > 0);

        }

    }
}
