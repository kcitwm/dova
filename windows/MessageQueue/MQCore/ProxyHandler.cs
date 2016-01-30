using System;
using Dova.Services;
using Dova.Interfaces;
using Dova.Utility;

namespace Dova.MessageQueue
{

    /// <summary>
    /// 服务转发类
    /// </summary>
    public class ProxyHandler : MessageHandler
    {
        static string className = "ProxyHandler";

        /// <summary>
        /// 同步模式:处理
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public override object ExecuteRequest(string serviceName, object msg)
        {
            using (IDisposable service = ServiceFactory<IMessageService>.GetServer(services[serviceName].GroupName).Instance() as IDisposable)
            {
                long t = DateTime.Now.Ticks;
                try
                {
                    object o = (service as IMessageService).Request(serviceName, msg);
                    Log.Write(LogAction.Info, className, "ExecuteRequest", serviceName, DateTime.Now.Ticks - t, "msg:" + msg.ToString());
                    return o;
                }
                catch (Exception e)
                {
                    Log.Write(LogAction.Error, className, "ExecuteRequest", serviceName, DateTime.Now.Ticks - t,"msg:" + msg.ToString()+","+ e.ToString());
                    throw new Exception("消息处理失败:ExecuteRequest.serviceName:" + serviceName + ",TransactionID" + msg.ToString() + ",错误信息:" + e.Message);
                }
            }
        }

        /// <summary>
        /// 同步模式:处理
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public override object DoExecuteMessageRequest(string serviceName, WQMessage msg, bool async)
        {
            // Log.Write("ProxyHandler.DoExecuteMessageRequest:" + services[serviceName].GroupName);
            Log.Write(LogAction.Info, className, "DoExecuteMessageRequest" , serviceName, -1, string.Format(startFormat, serviceName, msg.TransactionID));
            long t = DateTime.Now.Ticks;
            string groupName = services[serviceName].GroupName;
            using (IDisposable service = ServiceFactory<IMessageService>.GetServer(groupName).Instance() as IDisposable)
            { 
                try
                {
                    Log.Write(LogAction.Info, className, "DoExecuteMessageRequest", serviceName, DateTime.Now.Ticks - t, "获取目标服务:"+groupName+",msg:" + msg.TransactionID);
                    t = DateTime.Now.Ticks;
                    object o = (service as IMessageService).RequestMessage(serviceName, msg);
                    Log.Write(LogAction.Info, className, "DoExecuteMessageRequest", serviceName, DateTime.Now.Ticks - t, "同步代理转发:"+groupName+":"+serviceName+",msg:" + msg.ToKeyString());
                    return o;
                }
                catch (Exception e)
                {
                    Log.Write(LogAction.Error, className, "DoExecuteMessageRequest", serviceName, DateTime.Now.Ticks - t, string.Format(errorFormat, serviceName, msg.ToKeyString(), e.ToString()));
                    throw new Exception("消息处理失败:DoExecuteMessageRequest.serviceName:" + serviceName + ",TransactionID:" + msg.TransactionID + ",错误信息:" + e.Message);
                }
            }
        }

    }
}
