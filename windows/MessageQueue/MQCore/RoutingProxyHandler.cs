using System;
using Dova.Services;
using Dova.Interfaces;
using Dova.Utility;

namespace Dova.MessageQueue
{

    /// <summary>
    /// 服务转发类
    /// </summary>
    public class RoutingProxyHandler : ProxyHandler
    {
        static string className = "RoutingProxyHandler";

        /// <summary>
        /// 根据Message路由接收端
        /// </summary>
        /// <param name="serviceName"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected virtual Routing Routing(string serviceName, object msg)
        {
            PlugingItem plug = services[serviceName];
            WQMessage imsg = msg as WQMessage; 
            try
            {
                Routing routing = null;
                if (!routings.ContainsKey(plug.RoutingGroupName) || !routings[plug.RoutingGroupName].ContainsKey(imsg.RoutingKey))
                {
                    if (plug.GroupName != "")
                    {
                        Log.Write(LogAction.Info, className, "Routing", serviceName, -1, "没有找到路由,用GroupName默认代替:plug.GroupName:" + plug.GroupName);
                        routing = new Routing();
                        routing.GroupName = plug.GroupName;
                        return routing;
                    }
                    return null;
                }
                routing = routings[plug.RoutingGroupName][imsg.RoutingKey];
                Log.Write(LogAction.Info, className, "Routing" , serviceName, -1, imsg.TransactionID+":找到路由:Routing:" + routing);
                return routing;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "Routing", serviceName, -1, "serviceName:" + serviceName + ",msg:" + imsg.TransactionID + ",没有找到路由:plug.RoutingGroupName:" + plug.RoutingGroupName + ",imsg.RoutingKey:" + imsg.RoutingKey + "," + e.ToString());
                return null;
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
                long t = DateTime.Now.Ticks;
                try
                {
                    object o = (service as IMessageService).Request(serviceName, msg);
                    Log.Write(LogAction.Info, className, "ExecuteRequest" , serviceName, DateTime.Now.Ticks - t, "serviceName:" + serviceName + ",msg:" + msg.ToString());
                    return o;
                }
                catch (Exception e)
                {
                    Log.Write(LogAction.Error, className, "ExecuteRequest" , serviceName, DateTime.Now.Ticks - t, "serviceName:" + serviceName + ",msg:" + msg.ToString() + ":" + e.ToString());
                    return null;
                }
            }
        }


        protected object EmptyRouting(string serviceName, WQMessage msg, bool async)
        {
            try
            {
                IMessageHandler handler = null;
                PlugingItem plug = services[serviceName];
                string emptyHandlerName = "";
                if (!routings.ContainsKey(plug.RoutingGroupName) || !routings[plug.RoutingGroupName].ContainsKey(msg.RoutingKey))
                {
                    Log.Write(LogAction.Info, className, "EmptyRouting" , serviceName, -1,string.Format(startFormat, serviceName, "没有找到路由,由服务默认空Handler处理:RoutingKey:" + msg.RoutingKey + ",msg:" + msg.ToKeyString()));
                    emptyHandlerName = services[serviceName].EmptyHandlerName;
                }
                else
                {
                    Routing routing = routings[plug.RoutingGroupName][msg.RoutingKey];
                    emptyHandlerName = routing.EmptyHandlerName;
                    Log.Write(LogAction.Info, className, "EmptyRouting" , serviceName, -1, string.Format(startFormat, serviceName,"路由配置为空处理,由名为 " + emptyHandlerName + " 的PlugItem对应处理类处理:routing:" + routing + ",msg:" + msg.ToKeyString()));
                }
                if (emptyHandlerName != string.Empty)
                {
                    handler = LoadHandler(emptyHandlerName, msg);
                    long t = DateTime.Now.Ticks;
                    object o = handler.DoExecuteMessageRequest(serviceName, msg, async);
                    Log.Write(LogAction.Info, className, "EmptyRouting" , serviceName, DateTime.Now.Ticks - t, string.Format(endFormat, serviceName, "msg:" + msg.ToKeyString()));
                    return o;
                }
                else
                {
                    Log.Write(LogAction.Info, className, "EmptyRouting", serviceName,-1, string.Format(errorFormat, serviceName, "msg:" + msg.ToKeyString(),"没有找到空处理类:"));
                    throw new Exception("消息处理失败:EmptyRouting.serviceName:" + serviceName + ",TransactionID:" + msg.TransactionID + ",错误信息: 消息没有正确的RoutingKey,并且没有指定空处理类");
                }
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "EmptyRouting", serviceName, -1, string.Format(errorFormat, serviceName, "msg:" + msg.ToKeyString(),e.ToString()));
                throw new Exception("消息处理失败:EmptyRouting.serviceName:" + serviceName + ",TransactionID:" + msg.TransactionID + ",错误信息:" + e.Message);
            }
        }

        /// <summary>
        /// 同步模式:处理
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public override object DoExecuteMessageRequest(string serviceName, WQMessage msg, bool async)
        {
            Log.Write(LogAction.Info, className, "DoExecuteMessageRequest" , serviceName, -1, string.Format(startFormat, serviceName,  msg.TransactionID));
            Routing routing = Routing(serviceName, msg);
            if (null == routing || routing.EmptyHandlerName.Length > 0)
            {
                return EmptyRouting(serviceName, msg, async);
            }
            string svcName = routing.GroupName;
           long t = DateTime.Now.Ticks;
           DebugLog("DoExecuteMessageRequest:svcName=" + svcName);
            using (IDisposable service = ServiceFactory<IMessageService>.GetServer(svcName).Instance() as IDisposable)
            {
                try
                {
                    Log.Write(LogAction.Info, className, "DoExecuteMessageRequest" , serviceName, DateTime.Now.Ticks - t, "获取路由目标服务:" + svcName + ",msg:" + msg.TransactionID);
                    t = DateTime.Now.Ticks;
                    object o = (service as IMessageService).RequestMessage(serviceName, msg);
                    Log.Write(LogAction.Info, className, "DoExecuteMessageRequest" , serviceName, DateTime.Now.Ticks - t, "同步路由代理转发:" + svcName +":"+serviceName+ ",msg:" + msg.ToKeyString());
                    return o;
                }
                catch (Exception e)
                {
                    Log.Write(LogAction.Error, className, "DoExecuteMessageRequest" , serviceName, DateTime.Now.Ticks - t, string.Format(errorFormat, serviceName,msg.ToKeyString(), e.ToString()));
                    throw new Exception("消息处理失败:DoExecuteMessageRequest.serviceName:" + serviceName + ",TransactionID:" + msg.TransactionID + ",错误信息:" + e.Message);
                }
            }
        } 

    }
}
