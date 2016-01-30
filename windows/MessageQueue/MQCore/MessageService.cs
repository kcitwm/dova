using Dova.Interfaces;
using Dova.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;

namespace Dova.MessageQueue
{
    [ServiceBehavior(MaxItemsInObjectGraph = 0x7fffffff, InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple, AddressFilterMode = AddressFilterMode.Any)]
    public class MessageService : MarshalByRefObject, IMessageService
    {
        //static MessageService()
        //{
        //    try
        //    {
        //        MessageHandler.StartRetryProcess();
        //    }
        //    catch { }
        //}

        /// <summary>h
        /// 同步发送数据
        /// </summary>
        /// <param name="topicID">主题ID</param>
        /// <param name="obj">发送的数据</param> 
        /// <param name="keyID">业务主键</param>
        /// <returns></returns> 
        public object Request(string serviceName, object msg)
        {
            return MessageHandler.Execute(serviceName, msg, false);
        }


        public object Send(WQMessage msg)
        {
            return MessageHandler.Execute(msg.ServiceName, msg, msg.Async);
        }


        public void SendOneWay(WQMessage msg)
        {
             MessageHandler.Execute(msg.ServiceName, msg, msg.Async);
        }

        public object Receive(string format)
        {
            return null;
        }

        /// <summary>
        /// 同步发送数据
        /// </summary>
        /// <param name="topicID">主题ID</param>
        /// <param name="obj">发送的数据</param> 
        /// <param name="keyID">业务主键</param>
        /// <returns></returns> 
        public string Request(string msg)
        {
            OperationContext context = OperationContext.Current; 
            //获取传进的消息属性
            MessageProperties properties = context.IncomingMessageProperties;
            //获取消息发送的远程终结点IP和端口 
            RemoteEndpointMessageProperty endpoint = properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            string edp = context.EndpointDispatcher.EndpointAddress.Uri.AbsolutePath.Trim('/');
            Log.Info("收到来自Endpoint消息:" + edp);
            return MessageHandler.Execute(edp, msg, false);
        }

 

        /// <summary>h
        /// 异步发送数据
        /// </summary>
        /// <param name="topicID">主题ID</param>
        /// <param name="obj">发送的数据</param> 
        /// <param name="keyID">业务主键</param>
        /// <returns></returns> 
        public bool AsyncRequest(string serviceName, object msg)
        {
            return MessageHandler.AsyncExecute(serviceName, msg);
        }

        /// <summary>
        /// 异步根据主题发送自定义消息
        /// </summary>
        /// <param name="topicID"></param>
        /// <param name="msg"></param>
        /// <returns></returns>   
        public bool AsyncRequestMessage(string serviceName, WQMessage msg)
        {
            return MessageHandler.AsyncExecuteMessage(serviceName, msg);
        }


        /// <summary>
        /// 异步根据主题发送自定义消息
        /// </summary>
        /// <param name="topicID"></param>
        /// <param name="msg"></param>
        /// <returns></returns>   
        public bool AsyncRequestMessages(string serviceName, WQMessage[] msg)
        {
            int n = msg.Length;
            int i = 0;
            foreach (WQMessage m in msg)
            {
                if (MessageHandler.AsyncExecuteMessage(serviceName, m))
                    i++;
            }
            return i == n; 
        }
        /// <summary>
        /// 同步根据主题发送自定义消息
        /// </summary>
        /// <param name="topicID"></param>
        /// <param name="msg"></param>
        /// <returns></returns>  
        public object RequestMessage(string serviceName, WQMessage msg)
        {
            return MessageHandler.ExecuteMessage(serviceName, msg, msg.Async);
        }


        /// <summary>
        /// 异步根据主题发送自定义消息
        /// </summary>
        /// <param name="topicID"></param>
        /// <param name="msg"></param>
        /// <returns></returns>   
        public bool AsyncRequestMessage(WQMessage msg)
        {
            return MessageHandler.AsyncExecuteMessage(msg.ServiceName, msg);
        }

      

        /// <summary>
        /// 同步根据主题发送自定义消息
        /// </summary>
        /// <param name="topicID"></param>
        /// <param name="msg"></param>
        /// <returns></returns>  
        public DovaResponse RequestMessage(WQMessage msg)
        {
            long t = DateTime.Now.Ticks;
            //if (msg.Format == "raw")
            msg.Body = SerializeHelper.Serialize(msg.Format, msg.Body);
            DovaResponse res = null;
            try
            {
                object o = MessageHandler.ExecuteMessage(msg.ServiceName, msg, false);
                //if (msg.Format == "raw")
                res = SerializeHelper.DeSerialize<DovaResponse>(msg.Format, o);
                // else
                //    res = o as DovaResponse;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, "MessageService", "RequestMessage", msg.ServiceName, DateTime.Now.Ticks - t, msg.ToKeyString() + ";" + e.ToString());
            }
            return res;
        }


        public DovaResponse Test(WQMessage msg)
        {
            DovaResponse res = new DovaResponse();
            if (msg.Body != null)
                res.Body = msg.Body.ToString();
            else
                res.Body = "请求的body为空";
            res.TransactionID = msg.TransactionID;
            res.Status = 1;
            res.Message = "测试响应消息:";
            return res;
        }

    }
}
