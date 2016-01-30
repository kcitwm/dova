using System;
using System.Collections.Generic;

using Dova.Utility;
using Dova.Services;
using System.Configuration;
using Dova.Interfaces;


namespace Dova.MessageQueue
{
    public class MessageClient
    {
        static string className = "MessageClient";

        static string MessageServiceName = "DovaMessageService";
        /// <summary>
        /// {0} 服务名   {1}消息
        /// </summary>
        protected const string startFormat = "开始:{0},消息:{1}";
        /// <summary>
        /// {0} 服务名   {1}消息
        /// </summary>
        protected const string endFormat = "结束:{0},消息:{1}";
        /// <summary>
        /// {0} 服务名 {1} TransactionID {2}消息
        /// </summary>
        protected const string errorFormat = "出错:{0},消息:{1},错误信息:{2}";

        ///// <summary>
        ///// 异步:发送数据
        ///// </summary>
        ///// <param name="topicID">
        ///// 主题ID:
        ///// 在MsgQ服务管理端添加一种主题，然后记下这个业务主题的ＩＤ，客户端针对自己业务主题发送消息
        ///// </param>
        ///// <param name="obj">
        ///// 发送的数据：
        ///// 任何类型数据，要求标记为可序列化．
        ///// </param>
        ///// <param name="keyID">
        ///// 业务主键：
        ///// 每个消息的业务主键，如果为新增的话填写０</param>
        ///// <returns></returns>
        //public static string Send(int topicID, long keyID, object msg)
        //{
        //    long t = DateTime.Now.Ticks;
        //    Log.Write(LogAction.Info, "发送消息开始:开始创建生产者:topicID=" + topicID);
        //    uint tid = (uint)topicID;
        //    uint kid = (uint)keyID;
        //    c2cplatform.component.msgqv5.CMqProducer prod = c2cplatform.component.msgqv5.CMqFactory.GetProducer(tid);
        //    Log.Write(LogAction.Info, className, "SendMessage", "SendMessage1", DateTime.Now.Ticks - t, prod == null ? "prod is null" : "创建生产者成功:" + topicID);
        //    t = DateTime.Now.Ticks;
        //    prod.Init("ddd", "utf-8", "carl");
        //    Log.Write(LogAction.Info, className, "SendMessage","SendMessage2", DateTime.Now.Ticks - t, "初始化生产者成功:" + topicID);
        //    byte[] data = SerializeHelper.SerializeToBytes(msg);
        //    string retKey = "";
        //    Log.Write(LogAction.Info, className, "SendMessage","SendMessage3", DateTime.Now.Ticks - t, "序列化数据成功:data.Length=" + data.Length);
        //    int sent = prod.SendMsg(data, kid, out retKey);
        //    Log.Write(LogAction.Info, className, "SendMessage","SendMessage4", DateTime.Now.Ticks - t, "发送数据成功:sent=" + sent + ";返回唯一标志:" + retKey);
        //    return retKey;
        //}

        /// <summary>
        /// 异步:发送数据
        /// </summary>
        /// <param name="topicID">
        /// 主题ID:
        /// 在MsgQ服务管理端添加一种主题，然后记下这个业务主题的ＩＤ，客户端针对自己业务主题发送消息
        /// </param>
        /// <param name="obj">
        /// 发送的数据：
        /// 任何类型数据，要求标记为可序列化．
        /// </param>
        /// <param name="keyID">
        /// 业务主键：
        /// 每个消息的业务主键，如果为新增的话填写０</param>
        /// <returns></returns>
        //public static string Send(int topicID, long keyID, DovaMessage obj)
        //{
        //    uint tid = (uint)topicID;
        //    uint kid = (uint)keyID;
        //    Log.Info("开始创建生产者");
        //    c2cplatform.component.msgqv5.CMqProducer prod = c2cplatform.component.msgqv5.CMqFactory.GetProducer(tid);
        //    Log.Info(prod == null ? "prod is null" : "创建生产者成功");
        //    prod.Init("ddd", "utf-8", "carl");
        //    Log.Info("初始化成功:");
        //    byte[] data = SerializeHelper.SerializeToBytes(obj);
        //    Console.WriteLine("准备发送数据:data.lenght=" + data.Length);
        //    Log.Info("准备发送数据:");
        //    string retKey = "";
        //    prod.SendMsg(data, kid, out retKey);
        //    Log.Info("发送数据完成:" + retKey);
        //    return retKey; 
        //}



        /// <summary>
        /// 异步:根据主题发送统一格式消息
        /// </summary>
        /// </summary>
        /// <param name="topicID">
        /// 主题ID:
        /// 在MsgQ服务管理端添加一种主题，然后记下这个业务主题的ＩＤ，客户端针对自己业务主题发送消息
        /// </param>
        /// <param name="obj">
        /// 发送的数据：
        /// 任何类型数据，要求标记为可序列化．
        /// </param>
        /// <param name="keyID">
        /// 业务主键：
        /// 每个消息的业务主键，如果为新增的话填写０</param>
        /// <returns>发送消息自动产生的seq</returns> 
        //public static string SendMessage(int topicID, DovaMessage msg)
        //{
        //    long t = DateTime.Now.Ticks;
        //    Log.Write(LogAction.Info,  "发送消息开始:开始创建生产者:topicID=" + topicID);
        //    uint tid = (uint)topicID;
        //    uint kid = (uint)msg.KeyID;
        //    c2cplatform.component.msgqv5.CMqProducer prod = c2cplatform.component.msgqv5.CMqFactory.GetProducer(tid);
        //    Log.Write(LogAction.Info, className, "SendMessage", "SendMessage1", DateTime.Now.Ticks - t, prod == null ? "prod is null" : "创建生产者成功:" + topicID);
        //    t = DateTime.Now.Ticks;
        //    prod.Init("ddd", "utf-8", "carl");
        //    Log.Write(LogAction.Info, className, "SendMessage", "SendMessage2", DateTime.Now.Ticks - t, "初始化生产者成功:" + topicID);
        //    byte[] data = SerializeHelper.SerializeToBytes(msg);
        //    string retKey = "";
        //    Log.Write(LogAction.Info, className, "SendMessage", "SendMessage3", DateTime.Now.Ticks - t, "序列化数据成功:data.Length=" + data.Length);
        //    int sent = prod.SendMsg(data, kid, out retKey);
        //    Log.Write(LogAction.Info, className, "SendMessage", "SendMessage4", DateTime.Now.Ticks - t, "发送数据成功:sent=" + sent + ";返回唯一标志:" + retKey);
        //    return retKey;
        //}



        /// <summary>
        /// 异步:根据服务消费ID获取数据
        /// </summary>
        /// <param name="consumerID">
        /// 消费ＩＤ：
        /// 在MsgQ服务管理端新增加一个消费ＩＤ，关联你要消费的主题ＩＤ．
        /// </param>
        /// <returns>
        /// 返回原始数据：
        /// 返回客户端发送的数据．自己负责进行强制转化．
        /// </returns>
        //public static object Receive(int consumerID)
        //{
        //    long t = DateTime.Now.Ticks;
        //    uint cid = (uint)consumerID;
        //    Log.Write(LogAction.Info, className, "Receive", "Receive1", -1, "接收消息开始:创建消费者:" + consumerID);
        //    c2cplatform.component.msgqv5.CMqConsumer cons = c2cplatform.component.msgqv5.CMqFactory.GetConsumer(cid);
        //    Log.Write(LogAction.Info, className, "Receive", "Receive2", DateTime.Now.Ticks - t, cons == null ? "cons is null" : "创建消费者成功");
        //    t = DateTime.Now.Ticks;
        //    cons.Init("ddd", "utf-8", "carl");
        //    Log.Write(LogAction.Info, className, "Receive", "Receive3", DateTime.Now.Ticks - t, cons == null ? "cons is null" : "消费者初始化成功");
        //    t = DateTime.Now.Ticks;
        //    c2cplatform.component.msgqv5.CConsumerMsgInfo msg = cons.RecvMsg();
        //    Log.Write(LogAction.Info, className, "Receive", "Receive4", DateTime.Now.Ticks - t, cons == null ? "cons is null" : "接收数据完成:" + msg == null ? " msg is null" : msg.TopicId + ":" + msg.ProducerId + ":" + msg.Seq + ":" + msg.SourceIp);
        //    byte[] data = msg.MsgData;
        //    Log.Write(LogAction.Info, className, "Receive", "Receive5", DateTime.Now.Ticks - t, "接收到数据长度:" + data.Length);
        //    return SerializeHelper.DeSerializeFromBytes(data);
        //}

        ///// <summary>
        ///// 异步:接收原始数据
        ///// </summary>
        ///// <param name="consumerID"></param>
        ///// <returns></returns>
        //public static List<c2cplatform.component.msgqv5.CConsumerMsgInfo> ReceiveConsumerMsg(int consumerID)
        //{
        //    long t = DateTime.Now.Ticks;
        //    uint cid = (uint)consumerID;
        //    Log.Write(LogAction.Info, className, "ReceiveConsumerMsg", "ReceiveConsumerMsg1", -1, "接收消息开始:创建消费者:" + consumerID);
        //    c2cplatform.component.msgqv5.CMqConsumer cons = c2cplatform.component.msgqv5.CMqFactory.GetConsumer(cid);
        //    Log.Write(LogAction.Info, className, "ReceiveConsumerMsg", "ReceiveConsumerMsg2", DateTime.Now.Ticks - t, cons == null ? "cons is null" : "创建消费者成功");
        //    t = DateTime.Now.Ticks;
        //    cons.Init("ddd", "utf-8", "carl");
        //    Log.Write(LogAction.Info, className, "ReceiveConsumerMsg", "ReceiveConsumerMsg3", DateTime.Now.Ticks - t, cons == null ? "cons is null" : "消费者初始化成功");
        //    t = DateTime.Now.Ticks;
        //    List<c2cplatform.component.msgqv5.CConsumerMsgInfo> msgs = cons.RecvMsgs();
        //    Log.Write(LogAction.Info, className, "ReceiveConsumerMsg", "ReceiveConsumerMsg4", DateTime.Now.Ticks - t, cons == null ? "cons is null" : "接收数据完成:msgs.Count=" + msgs.Count);
        //    return msgs;
        //}




        /// <summary>
        /// 异步:根据消费ID获取统一格式消息
        /// </summary>
        /// <param name="consumerID">
        /// 消费ＩＤ：
        /// 在MsgQ服务管理端新增加一个消费ＩＤ，关联你要消费的主题ＩＤ．
        /// </param>
        /// <returns>
        /// 统一格式数据：
        /// 返回客户端发送的数据．自己负责对Message的Body进行强制转化．
        /// </returns>
        //public static DovaMessage ReceiveMessage(int consumerID)
        //{
        //    uint cid = (uint)consumerID;
        //    c2cplatform.component.msgqv5.CMqConsumer cons = c2cplatform.component.msgqv5.CMqFactory.GetConsumer(cid);
        //    c2cplatform.component.msgqv5.CConsumerMsgInfo msg = cons.RecvMsg();
        //    byte[] data = msg.MsgData;
        //    return SerializeHelper.DeSerializeFromBytes(data) as DovaMessage;
        //}

        #region 同步请求
        /// <summary>
        /// 同步:发送数据
        /// </summary>
        /// <param name="serviceName">
        /// 服务名:
        /// 根据应用程序配置：config传递你需要请求的服务．
        /// </param>
        /// <param name="obj">
        /// 发送的数据：
        /// 任何类型数据，要求标记为可序列化．
        /// </param>　
        /// <returns>请求响应的数据</returns> 
        public static T Request<T>(string serviceName, object msg)
        {
            return Request<T>(MessageServiceName, serviceName, msg);
        }

        /// <summary>
        /// 同步:发送数据
        /// </summary>
        /// <param name="appName"
        /// 应用名
        /// </param>　
        /// <param name="serviceName">
        /// 服务名:
        /// 根据应用程序配置：config传递你需要请求的服务．
        /// </param>
        /// <param name="obj">
        /// 发送的数据：
        /// 任何类型数据，要求标记为可序列化．
        /// </param>　
        /// <returns>请求响应的数据</returns> 
        public static T Request<T>(string appName, string serviceName, object msg)
        {
            DateTime t = DateTime.Now;
            try
            {
                byte[] req = SerializeHelper.SerializeToBytes(msg);
                using (IDisposable service = ServiceFactory<IMessageService>.GetServer(appName).Instance() as IDisposable)
                {
                    byte[] res = (service as IMessageService).Request(serviceName, req) as byte[];
                    T obj = SerializeHelper.DeSerializeFromBytes<T>(res);
                    Log.Write(LogAction.Info, className, "RequestMessage", serviceName, (DateTime.Now - t).Ticks, "serviceName:" + serviceName + ",msg:" + msg.ToString());
                    return obj;
                }
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "RequestMessage", serviceName, (DateTime.Now - t).Ticks, "serviceName:" + serviceName + ",msg:" + msg.ToString() + "," + e.ToString());
                throw;
            }
        }


        ///// <summary>
        ///// 同步:发送数据
        ///// </summary>
        ///// <param name="topicID">服务名</param>
        ///// <param name="obj">发送的数据</param> 
        ///// <returns></returns>
        //public static void RequestOneWay(string serviceName, object obj)
        //{
        //    ServiceFactory<IMessageService>.GetInstance(MessageServiceName).RequestOneWay(serviceName, obj);
        //} 

        /// <summary>
        /// 同步:根据主题发送统一格式消息
        /// </summary>
        /// <param name="serviceName">
        /// 服务名:
        /// 根据应用程序配置：config传递你需要请求的服务．
        /// </param>
        /// <param name="obj">
        /// 发送的数据：
        /// 统一格式数据，要求标记为可序列化．
        /// </param>
        /// <returns>请求响应的数据</returns> 
        public static T RequestMessage<T>(string serviceName, WQMessage msg)
        {
            return RequestMessage<T>(MessageServiceName, serviceName, msg);
        }

        public static T Send<T>(WQMessage msg)
        {
            string appName = msg.AppName;
            string serviceName = msg.ServiceName;
            long t = DateTime.Now.Ticks;
            using (IDisposable service = ServiceFactory<IMessageService>.GetServer(appName).Instance() as IDisposable)
            {
                Log.Write(LogAction.Info, className, "Send<T>", serviceName, DateTime.Now.Ticks - t, "获取同步目标服务:" + appName + ",msg:" + msg.TransactionID);
                object res = (service as IMessageService).Send(msg);
                if (null == res) return default(T);
                if ( msg.Format == "json")
                    res = SerializeHelper.DeSerializeToJSon<T>((string)res);
                 Log.Write(LogAction.Info, className, "Send<T>", serviceName, DateTime.Now.Ticks - t, string.Format(startFormat, appName + ":" + serviceName, "DeSerialize:" + msg.TransactionID));
                return (T)res;
            }
        }

        public static void  SendOneWay<T>(WQMessage msg)
        {
            string appName = msg.AppName;
            string serviceName = msg.ServiceName;
            long t = DateTime.Now.Ticks;
            using (IDisposable service = ServiceFactory<IMessageService>.GetServer(appName).Instance() as IDisposable)
            {
                Log.Write(LogAction.Info, className, "Send<T>", serviceName, DateTime.Now.Ticks - t, "获取同步目标服务:" + appName + ",msg:" + msg.TransactionID);
                (service as IMessageService).SendOneWay(msg); 
            }
        }


        public static T Receive<T>(string appName, string format)
        {
            long t = DateTime.Now.Ticks;
            using (IDisposable service = ServiceFactory<IMessageService>.GetServer(appName).Instance() as IDisposable)
            {
                Log.Write(LogAction.Info, className, "Receive<T>", appName, DateTime.Now.Ticks - t, "获取同步目标服务:" + appName);
                object res = (service as IMessageService).Receive(format);
                if (format == "json")
                    res = SerializeHelper.DeSerializeToJSon<T>((string)res);
                Log.Write(LogAction.Info, className, "Receive<T>", appName, DateTime.Now.Ticks - t, "同步请求服务:" + appName);
                return (T)res;
            }
        }

        /// <summary>
        /// 异步:根据主题发送统一格式消息
        /// </summary>
        /// <param name="serviceName">
        /// 服务名:
        /// 根据应用程序配置：config传递你需要请求的服务．
        /// </param>
        /// <param name="obj">
        /// 发送的数据：
        /// 统一格式数据，要求标记为可序列化．
        /// </param>
        /// <returns>请求响应的数据</returns> 
        public static bool AsyncRequestMessage(string serviceName, WQMessage msg)
        {
            return AsyncRequestMessage(MessageServiceName, serviceName, msg);
        }


        /// <summary>
        /// 同步:根据主题发送统一格式消息
        /// </summary>　
        /// <param name="appName"
        /// 应用名
        /// </param>
        /// <param name="serviceName">
        /// 服务名:
        /// 根据应用程序配置：config传递你需要请求的服务．
        /// </param>
        /// <param name="obj">
        /// 发送的数据：
        /// 统一格式数据，要求标记为可序列化．
        /// </param>
        /// <returns>请求响应的数据</returns> 

        public static T RequestMessage<T>(string appName, string serviceName, WQMessage msg)
        {
            long t = DateTime.Now.Ticks;
            long tid = msg.TransactionID;
            try
            {
                msg.Body = SerializeHelper.Serialize(msg.Format, msg.Body);
                Log.Write(LogAction.Info, className, "RequestMessage<T>", serviceName, DateTime.Now.Ticks - t, string.Format(startFormat, appName + ":" + serviceName, "Serialize:" + msg));
                t = DateTime.Now.Ticks;
                using (IDisposable service = ServiceFactory<IMessageService>.GetServer(appName).Instance() as IDisposable)
                {
                    Log.Write(LogAction.Info, className, "RequestMessage<T>", serviceName, DateTime.Now.Ticks - t, "获取同步目标服务:" + appName + ",msg:" + msg.TransactionID);
                    object res = (service as IMessageService).RequestMessage(serviceName, msg);
                    Log.Write(LogAction.Info, className, "RequestMessage<T>", serviceName, DateTime.Now.Ticks - t, "同步请求服务:" + appName + ",msg:" + msg.ToKeyString());
                    t = DateTime.Now.Ticks;
                    T obj = SerializeHelper.DeSerialize<T>(msg.Format, res);
                    Log.Write(LogAction.Info, className, "RequestMessage<T>", serviceName, DateTime.Now.Ticks - t, string.Format(startFormat, appName + ":" + serviceName, "DeSerialize:" + msg.TransactionID));
                    return obj;
                }
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "RequestMessage<T>", serviceName, DateTime.Now.Ticks - t, string.Format(errorFormat, appName + ":" + serviceName, "msg:" + msg.ToKeyString(), e.ToString()));
                throw new Exception(string.Format(errorFormat, serviceName, msg.ToKeyString(), e.Message));

            }
        }

        public static bool AsyncRequestMessage(string appName, string serviceName, WQMessage msg)
        {
            long t = DateTime.Now.Ticks;
            long tid = msg.TransactionID;
            try
            {
                msg.Body = SerializeHelper.Serialize(msg.Format, msg.Body);
                Log.Write(LogAction.Info, className, "AsyncRequestMessage", serviceName, -1, string.Format(startFormat, appName + ":" + serviceName, "Serialize:" + msg.TransactionID));
                t = DateTime.Now.Ticks;
                using (IDisposable service = ServiceFactory<IMessageService>.GetServer(appName).Instance() as IDisposable)
                {
                    Log.Write(LogAction.Info, className, "AsyncRequestMessage", serviceName, DateTime.Now.Ticks - t, "获取异步目标服务:" + appName + ",msg:" + msg.TransactionID);
                    bool obj = (service as IMessageService).AsyncRequestMessage(serviceName, msg);
                    Log.Write(LogAction.Info, className, "AsyncRequestMessage", serviceName, DateTime.Now.Ticks - t, "异步请求服务:" + appName + ":" + serviceName + ",返回值:" + obj + "," + ",msg:" + msg.ToKeyString());
                    return obj;
                }
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "AsyncRequestMessage", serviceName, DateTime.Now.Ticks - t, string.Format(errorFormat, appName + ":" + serviceName, "msg:" + msg.ToKeyString(), e.ToString()));
                return false;
            }
        }

        #endregion

    }
}
