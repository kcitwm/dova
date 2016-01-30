using System;
using System.Linq;
using System.ServiceModel;
using System.Collections.Generic;
using System.Collections;
using Dova.Utility;
using Dova.Interfaces;
using System.ComponentModel;
using System.Diagnostics;

namespace Dova.Services
{
    public class ServiceFactory<T> : IDisposable
    {
        //[ThreadStatic]
        private Dictionary<string, ServiceProcess<T>> svcList ;
        //[ThreadStatic]
        static Dictionary<string, ServiceFactory<T>> factories=new Dictionary<string,ServiceFactory<T>>();
        ConsistentHash ch;

        static string className = "ServiceFactory<T>";

        public ServiceFactory(List<ServiceConfigItem> configs)
        {
            Log.Write("ServiceFactory构造:configs.count=" + configs.Count);
            if (null != configs && configs.Count > 0)
            {
                Init(configs);
            }
        }

       


        public void Init(List<ServiceConfigItem> configs)
        {
            //factories = new Dictionary<string, ServiceFactory<T>>();
            List<string> urls = new List<string>(configs.Count);
            foreach (ServiceConfigItem config in configs)
            {
                if (config.Enable )
                    if (!urls.Contains(config.EndPoint))
                        urls.Add(config.EndPoint); 
            }
            InitService(configs);

            ch = new ConsistentHash(urls, 200);
        }

        void InitService(List<ServiceConfigItem> endpoints)
        {
            svcList = new Dictionary<string, ServiceProcess<T>>();
            ServiceProcess<T> srv;
            string serverKey = "";
            Log.Write("初始服务列表数量:" + endpoints.Count);
            foreach (ServiceConfigItem p in endpoints)
            {
                Log.Write("初始服务列表:" + p);
                if (!p.Enable) continue;
                serverKey = p.RoutKey;
                switch (p.ServiceType)
                {
                    case (int)ChannelType.WCF:
                    case (int)ChannelType.WCFRest: 
                        srv = new WCFProcess<T>(p);
                        break;
                    case (int)ChannelType.MessageService:
                        srv = new MessageServiceProcess<T>(p);
                        break;
                    case (int)ChannelType.Remoting:
                        srv = new RemotingProcess<T>(p);
                        break;
                    case (int)ChannelType.Http:
                        srv = new HttpProcess<T>(p);
                        break; 
                    default:
                        srv = new ServiceProcess<T>(p);
                        break;
                }
                if (!svcList.ContainsKey(serverKey))
                {
                    svcList.Add(serverKey, srv);
                    //srv.Fault += Srv_Fault;
                }
            }
           // Log.Write(LogAction.Write,  className, "InitService","InitService1", -1, "svcList.Count:" + svcList.Count);
        }

        void Srv_Fault(object sender, EventArgs e)
        {
            ServiceEventArgs sea = e as ServiceEventArgs;
            if (null != sea)
            {
                Log.Error("服务出错了:" + sea.RoutKey);
            }
        }


        public T Instance()
        {
            return Instance(new Random().Next(int.MaxValue).ToString());
        }

        /// <summary>
        /// 移除散列表中的服务容错，侦测到恢复后再添加回来．
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Instance(string key)
        {
            string k = ch.GetPrimary(key);
            ServiceProcess<T> server = svcList[k];
            try
            {
                //Log.Write("散列命中:" + k); 
                return server.Instance();
            }
            catch (Exception e)
            {
                //Log.Error("ServiceFactory.Instance:" + e.Message);
                //try
                //{
                //    k = svcList.GetCircleNextKey(k);
                //    return svcList[k].Instance();
                //}
                //catch { }
            }
            return server.Instance();
        }

        void Server_Fault(object sender, EventArgs e)
        {
            ServiceEventArgs args = e as ServiceEventArgs;
            string k = svcList.GetCircleNextKey(args.RoutKey);
            sender = svcList[k];
        }


        [Conditional("DEBUGLOG")]
        protected static void DebugLog(string msg)
        {
            Log.Warn(msg);
        }

        public static ServiceProcess<T> GetServer(string serviceName, string key)
        {
            ServiceFactory<T> factory = null;
            if (!factories.ContainsKey(serviceName))
            {
                factory = new ServiceFactory<T>(ServiceConfigs.GetConfigs(serviceName));
                factories.Add(serviceName, factory);
            }
            else factory = factories[serviceName];
            ServiceProcess<T> server = factory.GetServerByKey(key);
            return server;
        }

        public static ServiceProcess<T> GetServer(string serviceName)
        {
            ServiceFactory<T> factory = null;
            Log.Write("GetServer:创建服务工厂1:" + serviceName);
            if (!factories.ContainsKey(serviceName))
            { 
                factory = new ServiceFactory<T>(ServiceConfigs.GetConfigs(serviceName));
                try
                {
                    factories.Add(serviceName, factory);
                }
                catch { }
            }
            else factory = factories[serviceName];
            Log.Write("GetServer:创建服务工厂2:" + serviceName);
            ServiceProcess<T> server = factory.GetServer();
            return server;
        }


        public ServiceProcess<T> GetServer()
        {
            //Log.Write("GetServer:GetServerByKey1:");
            if (svcList.Count == 1) return svcList.GetValue(0);
            string k = ch.GetPrimary(new Random().Next(int.MaxValue).ToString());
            //Log.Write("GetServer:GetServerByKey:k=" + k);
            ServiceProcess<T> server = svcList[k];
            return server;
        }

        public ServiceProcess<T> GetServerByKey(string key)
        {
            //if (null == svcList || svcList.Count == 0)
            //    return new ServiceProcess<T>();
            if (svcList.Count == 1) return svcList.GetValue(0);
            string k = ch.GetPrimary(key);
            ServiceProcess<T> server = svcList[k];
            return server;
        }

        public static T GetInstance(string serviceName)
        {
            ServiceFactory<T> factory = null;
            if (!factories.ContainsKey(serviceName))
            {
                factory = new ServiceFactory<T>(ServiceConfigs.GetConfigs(serviceName));
                factories.Add(serviceName, factory);
            }
            else factory = factories[serviceName];
            return factory.Instance();
        }


        public void Dispose()
        {

        }
    }

}
