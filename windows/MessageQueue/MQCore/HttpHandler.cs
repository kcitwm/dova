using System;
using Dova.Services;
using Dova.Interfaces;
using Dova.Utility;

namespace Dova.MessageQueue
{

    /// <summary>
    /// http服务转发类, 
    /// </summary>
    public class HttpHandler : MessageHandler
    {
        static string className = "HttpHandler"; 

        /// <summary>
        /// 同步模式:处理
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public override object DoExecuteMessageRequest(string serviceName, WQMessage msg, bool async)
        {
            // Log.Write("ProxyHandler.DoExecuteMessageRequest:" + services[serviceName].GroupName);
            Log.Write(LogAction.Info, className, "DoExecuteMessageRequest", serviceName, -1, string.Format(startFormat, serviceName, msg.TransactionID));
            long t = DateTime.Now.Ticks;
            string data = (string)SerializeHelper.Serialize(msg.Format, msg.Body);
            DovaHttpMessage hmsg=msg as DovaHttpMessage; 
            string contentType = hmsg.ContentType;
            string groupName = services[serviceName].GroupName;
            using (IDisposable service = ServiceFactory<HttpProxy>.GetServer(groupName).Instance() as IDisposable)
            {
                try
                {
                    Log.Write(LogAction.Info, className, "DoExecuteMessageRequest", serviceName, DateTime.Now.Ticks - t, "获取目标服务:" + groupName + ",msg:" + msg.TransactionID);
                    t = DateTime.Now.Ticks;
                    string res = (service as HttpProxy).Request(data, hmsg.Method, hmsg.ContentType, hmsg.Encoding);
                    Log.Write(LogAction.Info, className, "DoExecuteMessageRequest", serviceName, DateTime.Now.Ticks - t, "同步代理转发:" + groupName + ":" + serviceName + ",msg:" + msg.ToKeyString());
                    return SerializeHelper.DeSerialize<DovaResponse>(msg.Format, res);
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
