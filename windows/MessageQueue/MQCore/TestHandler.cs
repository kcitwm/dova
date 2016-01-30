using System;
using Dova.Services;
using Dova.Utility;
using Dova.Interfaces;

namespace Dova.MessageQueue
{

    /// <summary>
    /// 服务转发类
    /// </summary>
    public class TestHandler : MessageHandler
    {
        static string className = "TestHandler";
        /// <summary>
        /// 同步模式:处理
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public override object ExecuteRequest(string serviceName, object msg)
        {
            return null;
        }

        /// <summary>
        /// 同步模式:处理
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public override object ExecuteMessageRequest(string serviceName, WQMessage msg)
        {
            Log.Write(LogAction.Info, serviceName, className, "ExecuteMessageRequest", 0, msg.ToString()); 
            DovaResponse res = new DovaResponse();
            if (msg.Body != null)
                res.Body = msg.Body.ToString();
            else
                res.Body = "请求的body为空";
            res.TransactionID = msg.TransactionID;
            res.Status = 1;
            res.Message = "这是一个对服务:" + serviceName + "的消息响应";
            return res;

        }

        public override bool ExecuteMQ(string serviceName, WQMessage msg)
        {
            Log.Write("执行异步的分发方法:" + serviceName + ";msg:" + msg);
            return true;
        }

    }
}
