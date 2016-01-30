using c2cplatform.component.msgqv5;
using WQFree.Interfaces;
using WQFree.Services;
using WQFree.Utility;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Xml;

namespace WQFree.MessageQueue
{
    /// <summary>
    /// 消息处理基类
    /// </summary>
    public class MessageHandler : IMessageHandler
    {

        static string className = "MessageHandler";

        protected static Dictionary<string, IMessageHandler> handlers = new Dictionary<string, IMessageHandler>();
        protected static Dictionary<string, IMessageHandler> emptyHandlers = new Dictionary<string, IMessageHandler>();
        protected static Dictionary<string, PlugingItem> services = new Dictionary<string, PlugingItem>();
        protected static Dictionary<string, PlugingItem> mqServices = new Dictionary<string, PlugingItem>();
        protected static Dictionary<string, Dictionary<string, Routing>> routings = new Dictionary<string, Dictionary<string, Routing>>();
        static string handlersKey = "ServiceHandlers";
        static string cfgPath = Config.ServiceConfigPath;

        static string eccMonitorKey = "";//"6094c672bfd59334b2242958d3b29620"; 

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

        static MessageHandler()
        {
            try
            {
                Init();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public static void StartRetryProcess()
        {
            eccMonitorKey = Config.Get("EccMonitorKey", eccMonitorKey);
            int inteval = ServiceConfigs.RetryInteval;
            Log.Write(LogAction.Write, "StartRetryProcess:准备开启补偿模式进程:RetryInteval:" + inteval);
            long t = DateTime.Now.Ticks;
            try
            {
                if (inteval > 0)
                {
                    Thread thread = new Thread(Retry);
                    thread.IsBackground = true;
                    thread.Priority = ThreadPriority.AboveNormal;
                    thread.Start();
                    Log.Write(LogAction.Write, "StartRetryProcess:开启补偿进程成功:");
                }
                else
                    Log.Write(LogAction.Write, "StartRetryProcess:没有设置为补偿模式进程:RetryInteval:" + inteval);

            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "StartRetryProcess", "Error", DateTime.Now.Ticks - t, "开启补偿进程出错:" + e.Message);
            }
        }

        public static void Retry()
        {
            MessageHandler retryHandler = new MessageHandler();
            while (true)
            {
                Thread.Sleep(ServiceConfigs.RetryInteval * 1000);
                retryHandler.RetryExecute();
            }
        }

        /// <summary>
        /// 初始化插件组
        /// </summary>
        public static void Init()
        {
            long t = DateTime.Now.Ticks;
            try
            {
                ServiceConfigs.Changed += ServiceConfigs_Changed;
                List<PlugingItem> plugs = ServiceConfigs.GetPlugings(handlersKey);
                List<Routing> routs = ServiceConfigs.GetRoutings(handlersKey);
                IMessageHandler handler = null;
                foreach (PlugingItem plug in plugs)
                {
                    if (!handlers.ContainsKey(plug.Name))
                    {
                        Log.Write(LogAction.Write, className, "Init", "Init1", DateTime.Now.Ticks - t, "plug:" + plug);
                        handler = CreateInstance<IMessageHandler>(plug.Type);
                        if (null != handler)
                        {
                            handlers[plug.Name] = handler;
                            services[plug.Name] = plug;
                            if (plug.ChannelType == 2)
                                mqServices[plug.Name] = plug;
                        }
                    }
                }
                foreach (Routing rout in routs)
                {
                    Log.Write(LogAction.Write, className, "Init", "Init2", DateTime.Now.Ticks - t, "rout:" + rout);
                    if (!routings.ContainsKey(rout.RoutingGroupName))
                        routings[rout.RoutingGroupName] = new Dictionary<string, Routing>();
                    routings[rout.RoutingGroupName][rout.Key] = rout;
                }
                Log.Write(LogAction.Write, className, "Init", "Init3", DateTime.Now.Ticks - t, "plugs.Count:" + plugs.Count + ",routs.Count:" + routs.Count + ",handlers.Count:" + handlers.Count + ",services.Count:" + services.Count);
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "Init", "Init4", DateTime.Now.Ticks - t, e.Message);
            }

        }

        static void ServiceConfigs_Changed(object sender, EventArgs e)
        {
            Log.Write(LogAction.Write, className, "ConfigChanged", "ConfigChanged1", -1, cfgPath + ":配置文件变更，重新初始化");
            Init();
        }

        /// <summary>
        /// 创建handler
        /// </summary>
        /// <typeparam name="T"></typeparam> 
        /// <param name="typeName"></param>
        /// <returns></returns>
        public static T1 CreateInstance<T1>(string typeName)
        {
            long tic = DateTime.Now.Ticks;
            try
            {
                Type type = Type.GetType(typeName);
                T1 t = (T1)Activator.CreateInstance(type);
                Log.Write(LogAction.Write, className, "CreateInstance", "CreateInstance1", DateTime.Now.Ticks - tic, typeName);
                return t;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "CreateInstance", "CreateInstance2", DateTime.Now.Ticks - tic, "初始化：" + typeName + " 出错:" + e.Message);
            }
            return default(T1);
        }

        public static IMessageHandler LoadHandler(string serviceName, object msg)
        {
            if (!handlers.ContainsKey(serviceName))
            {
                Log.Error("没有为服务:" + serviceName + "配置处理类,已经加载的处理类数量:handlers.Count:" + handlers.Count);
                throw new Exception("没有为服务:" + serviceName + "配置处理类");
            }
            return handlers[serviceName];
        }


        public static Dictionary<string, PlugingItem> MQServices
        {
            get { return mqServices; }
        }

        protected static MessageHandler GetHandler(int dealMode)
        {
            MessageHandler handler;
            //if (dealMode == 2)
            //    handler = new ClientCachedHandler();
            //else
            handler = new MessageHandler();
            return handler;
        }

        public static void DealMessage(int consumerID)
        {
            int ackedMode = services[consumerID.ToString()].DealMode;
            MessageHandler handler = GetHandler(ackedMode);
            List<CConsumerMsgInfo> msgs = handler.GetMessage(consumerID);
            IMessageHandler dealHandler = LoadHandler(consumerID.ToString(), msgs);
            ExecuteMQ(handler, consumerID, msgs);
        }



        protected static void ExecuteMQ(MessageHandler handler, int consumerID, List<CConsumerMsgInfo> msgs)
        {
            WQFreeMessage imsg = null;
            List<WQFreeMessage> imsgs = new List<WQFreeMessage>();
            IMessageHandler dealHandler = handlers[consumerID.ToString()];
            List<CConsumerMsgInfo> succMsgs = new List<CConsumerMsgInfo>();
            foreach (CConsumerMsgInfo msg in msgs)
            {
                imsg = SerializeHelper.DeSerializeFromBytes(msg.MsgData) as WQFreeMessage;
                if (null != imsg)
                {
                    imsgs.Add(imsg);
                    if (dealHandler.ExecuteMQ(consumerID.ToString(), imsg))
                        succMsgs.Add(msg);
                }
            }
            handler.ClearMessage(succMsgs);
        }

        /// <summary>
        /// 异步模式:处理多条统一格式消息
        /// </summary>
        /// <param name="msgs">消息集合</param>
        /// <returns>处理成功的条数</returns>
        public virtual int ExecuteMQ(string serviceName, List<WQFreeMessage> msgs)
        {
            int n = 0;
            foreach (WQFreeMessage msg in msgs)
            {
                if (ExecuteMQ(serviceName, msg))
                    n++;
            }
            return n;
        }

        /// <summary>
        /// 异步模式:单条处理统一格式消息
        /// </summary>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        public virtual bool ExecuteMQ(string serviceName, WQFreeMessage msg)
        {
            throw new Exception("没有实现ExecuteMQ");
        }

        /// <summary>
        /// 异步模式:单条处理统一格式消息
        /// </summary>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        public virtual bool ExecuteObjectMQ(string serviceName, object msg)
        {
            throw new Exception("没有实现ExecuteObjectMQ");
        }



        /// <summary>
        /// 同步模式:处理
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static object Execute(string serviceName, object msg, bool async)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                IMessageHandler handler = LoadHandler(serviceName, msg);
                object o = handler.DoExecuteRequest(serviceName, msg, async);
                Log.Write(LogAction.Info, className, "Execute", serviceName, DateTime.Now.Ticks - t, "serviceName:" + serviceName + "," + msg.ToString());
                return o;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "Execute", serviceName, DateTime.Now.Ticks - t, e.ToString());
                return null;
            }
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static object ExecuteMessage(string serviceName, WQFreeMessage msg, bool async)
        {
            Log.Write(LogAction.Info, className, "ExecuteMessage", serviceName, -1, string.Format(startFormat, serviceName, "开始处理消息:" + serviceName));

            Log.Write(LogAction.Info, className, "ExecuteMessage", serviceName, -1, string.Format(startFormat, serviceName, "开始处理消息:" + msg.ToKeyString()));
            long t = DateTime.Now.Ticks;
            long tid = msg.TransactionID;
            try
            {
                if (eccMonitorKey != "")
                {
                    ECCMonitorClient.ECCMonitorClient.ECCMonitorStart(eccMonitorKey, 1);
                    Log.Write("打点:" + (DateTime.Now.Ticks - t));
                    t = DateTime.Now.Ticks;
                }
                IMessageHandler handler = LoadHandler(serviceName, msg);
                object o = handler.DoExecuteMessageRequest(serviceName, msg, async);
                Log.Write(LogAction.Info, className, "ExecuteMessage", serviceName, DateTime.Now.Ticks - t, string.Format(endFormat, serviceName, "消息处理完毕:" + msg.TransactionID));
                if (eccMonitorKey != "")
                {   
                    t = DateTime.Now.Ticks;            
                    ECCMonitorClient.ECCMonitorClient.ECCMonitorEnd(eccMonitorKey, 1);
                    Log.Write("打点:" + (DateTime.Now.Ticks - t));
                }
                return o;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "ExecuteMessage", serviceName, DateTime.Now.Ticks - t, string.Format(errorFormat, serviceName, msg.ToKeyString(), e.ToString()));
                throw new Exception(string.Format(errorFormat, serviceName, msg.ToKeyString(), e.Message));
            }
        }


        /// <summary>
        /// 异步模式:处理
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool AsyncExecute(string serviceName, object msg)
        {
            return (bool)Execute(serviceName, msg, true);
        }

        public static bool AsyncExecuteMessage(string serviceName, WQFreeMessage msg)
        {
            return (bool)ExecuteMessage(serviceName, msg, true);
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual object DoExecuteRequest(string serviceName, object msg, bool async)
        {
            Log.Write(LogAction.Info, className, "DoExecuteRequest", serviceName, -1, "收到处理消息:serviceName:" + serviceName + ",async:" + async + "," + msg.ToString());
            if (async)
                return ExecuteObjectMQ(serviceName, msg);
            object obj = SerializeHelper.DeSerializeFromBytes(msg as byte[]);
            return SerializeHelper.SerializeToBytes(ExecuteRequest(serviceName, obj));
        }


        /// <summary>
        /// 同步模式:处理
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual object DoExecuteMessageRequest(string serviceName, WQFreeMessage msg, bool async)
        {
            long t = DateTime.Now.Ticks;
            object so = SerializeHelper.DeSerialize(msg.Format, msg.Body);
            msg.Body = so;
            long tid = msg.TransactionID;
            Log.Write(LogAction.Info, className, "DoExecuteMessageRequest", serviceName, DateTime.Now.Ticks - t, string.Format(startFormat, serviceName, "开始处理业务:" + (null == so ? "序列化返回空值:" : "序列化成功:") + msg.ToString()));
            t = DateTime.Now.Ticks;
            if (async)
            {
                bool rtn = ExecuteMQ(serviceName, msg);
                Log.Write(LogAction.Info, className, "DoExecuteMessageRequest", serviceName, DateTime.Now.Ticks - t, string.Format(startFormat, serviceName, "异步业务处理完毕:" + msg.TransactionID + ",返回状态:" + rtn));
                return rtn;
            }
            object result = ExecuteMessageRequest(serviceName, msg);
            object o = SerializeHelper.Serialize(msg.Format, result);
            Log.Write(LogAction.Info, className, "DoExecuteMessageRequest", serviceName, DateTime.Now.Ticks - t, string.Format(startFormat, serviceName, "同步业务处理完毕:" + (null == o ? "反序列化返回空值:" : "反序列化成功:") + msg.TransactionID + ";返回数据:" + o.ToString()));
            return o;
        }


        public void RetryExecute()
        {
            long t = DateTime.Now.Ticks;
            try
            {
                Log.Write(LogAction.Info, className, "RetryExecute", "Step1", -1, "开始补偿发送消息");
                int i = 0;
                using (IDataReader dr = MessageDao.GetRetryMessages(1, 10))
                {
                    if (null != dr)
                    {
                        int nidx = 0;
                        int aidx = 0;
                        int sidx = 0;
                        int didx = 0;
                        if (dr.Read())
                        {
                            i++;
                            nidx = dr.GetOrdinal("ServiceName");
                            aidx = dr.GetOrdinal("AppIndex");
                            sidx = dr.GetOrdinal("ServiceIndex");
                            didx = dr.GetOrdinal("MessageData");
                            AsyncExecute(dr.GetString(nidx), dr.GetInt64(aidx), dr.GetInt64(sidx), SerializeHelper.DeSerializeFromBytes<WQFreeMessage>((byte[])dr[didx]));
                        }
                        while (dr.Read())
                        {
                            i++;
                            AsyncExecute(dr.GetString(nidx), dr.GetInt64(aidx), dr.GetInt64(sidx), SerializeHelper.DeSerializeFromBytes<WQFreeMessage>((byte[])dr[didx]));
                        }
                    }
                }
                Log.Write(LogAction.Info, className, "RetryExecute", "Step2", DateTime.Now.Ticks - t, "补偿发送消息一共" + i + "条");
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "RetryExecute", "Error", DateTime.Now.Ticks - t, "重发消息出错:" + e.ToString());
            }
        }

        protected virtual void AsyncExecute(string serviceName, long pidx, long idx, WQFreeMessage msg)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                PlugingItem service = services[serviceName];
                Log.Write(LogAction.Info, className, "AsyncExecute", serviceName, DateTime.Now.Ticks - t, "准备异步处理:serviceName:" + serviceName + ",service.RoutingGroupName:" + service.RoutingGroupName + ",msg.TransactionID:" + msg.TransactionID + ",pidx:" + pidx + ",idx:" + idx);
                Dictionary<long, Dictionary<long, string>> groups = ServiceConfigs.ServiceGroups;
                Dictionary<long, string> group = null;
                foreach (long pgk in groups.Keys)
                {
                    if ((pidx & pgk) != 0)
                    {
                        group = groups[pgk];
                        foreach (long gk in group.Keys)
                        {
                            if ((idx & gk) != 0)
                                AsyncExecuteItem(serviceName, msg, pidx, idx, gk, group[gk]);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "AsyncExecute", serviceName, DateTime.Now.Ticks - t, string.Format(errorFormat, serviceName, msg.ToKeyString() + ",pidx:" + pidx + ",idx:" + idx, e.ToString()));
            }
        }

        protected void AsyncExecuteItem(string serviceName, WQFreeMessage msg, long pidx, long done, long dealing, string groupName)
        {
            bool ret = false;
            int sret = -1;
            long tid = msg.TransactionID;
            Log.Write(LogAction.Info, className, "AsyncExecuteItem", serviceName, -1, string.Format(startFormat, serviceName, tid));
            long t = DateTime.Now.Ticks;
            try
            {
                using (IDisposable service = ServiceFactory<IMessageService>.GetServer(groupName).Instance() as IDisposable)
                {
                    Log.Write(LogAction.Info, className, "AsyncExecuteItem", serviceName, DateTime.Now.Ticks - t, "获取路由目标服务:" + groupName + ",msg:" + tid + ",要求分发处理的Index集合:" + done + ",当前处理服务Index:" + dealing);
                    t = DateTime.Now.Ticks;
                    ret = (service as IMessageService).AsyncRequestMessage(serviceName, msg);
                    Log.Write(LogAction.Info, className, "AsyncExecuteItem", serviceName, DateTime.Now.Ticks - t, "异步路由代理转发:" + groupName + ":" + serviceName + ",返回值:" + ret + ",msg:" + msg.ToKeyString());
                }
                t = DateTime.Now.Ticks;
                if (ret)
                {
                    sret = MessageDao.SaveStatus(tid, dealing, done);
                    Log.Write(LogAction.Info, className, "AsyncExecuteItem", serviceName, DateTime.Now.Ticks - t, string.Format(startFormat, serviceName, msg.TransactionID + ",异步请求成功,保存状态返回值:" + sret));
                }
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "AsyncExecuteItem", serviceName, DateTime.Now.Ticks - t, string.Format(startFormat, serviceName, msg.TransactionID + ":" + e.Message));
                throw;
            }
            finally
            {
                if (!ret)
                {
                    sret = MessageDao.SaveDealStatus(tid, pidx, dealing, (int)DealStatus.Fail);
                    Log.Write(LogAction.Info, className, "AsyncExecuteItem", serviceName, DateTime.Now.Ticks - t, string.Format(startFormat, serviceName, msg.TransactionID + ",异步请求失败,保存处理状态返回值:" + sret));
                }
            }
        }

        /// <summary>
        /// 同步模式:处理
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual object ExecuteRequest(string serviceName, object msg)
        {
            return null;
        }

        /// <summary>
        /// 同步模式:处理
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual object ExecuteMessageRequest(string serviceName, WQFreeMessage msg)
        {
            return null;
        }




        ///// <summary>
        ///// 同步模式:处理
        ///// </summary>
        ///// <param name="msg"></param>
        ///// <returns></returns>
        //public virtual object ExecuteMessageRequest<T>(string serviceName, WQFreeMessage<T> msg)
        //{
        //    return null;
        //}

        protected virtual void ClearMessage(List<CConsumerMsgInfo> msgs)
        {
            //根据clientType,清理缓存Message;
        }


        protected virtual List<CConsumerMsgInfo> GetMessage(int consumerID)
        {
            return MessageClient.ReceiveConsumerMsg(consumerID);
        }



    }
}
