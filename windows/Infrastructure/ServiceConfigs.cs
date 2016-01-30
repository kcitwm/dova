using System;
using System.Xml;
using System.Configuration;
using System.Xml.Serialization;
using System.Collections.Generic;

using Dova.Utility;
using System.IO;
using System.Web;
using System.Web.Caching;
using System.ComponentModel;
using System.Text;
using System.Diagnostics;

namespace Dova
{
    [Serializable]
    public class ServiceConfigs
    {
        static string className = "ServiceConfigs";
        public List<ServiceConfigGroup> ServiceConfigGroups=new List<ServiceConfigGroup>();
        public List<PlugingItem> PlugingItems=new List<PlugingItem>();
        public List<Routing> RoutingItems = new List<Routing>(); 
        public LogConfig LogConfigs=new LogConfig();
        public MsgQConfig MsgQConfigs=new MsgQConfig();

        public static Dictionary<long, Dictionary<long, string>> ServiceGroups = new Dictionary<long, Dictionary<long, string>>();
        public static Dictionary<string, Dictionary<string, Routing>> Routings = new Dictionary<string, Dictionary<string, Routing>>();
       // public static Dictionary<long, Dictionary<long, string>> SubServiceGroups = new Dictionary<long, Dictionary<long, string>>();

        /// <summary>
        /// 1 走同步　2 异步　3 同步和异步
        /// </summary>
        public int InnerServiceType = 1;

        public string AppName = "MQService";

        public int InnerRetryInteval = -1;
        public static int RetryInteval = -1;

        public static int ServiceType = 1;
        public static bool OpenAsyncDispatchThread = false;

        public bool InnerOpenAsyncDispatchThread = false;

        public  static ServiceConfigs Instance = null;
        public static Dictionary<string, List<ServiceConfigItem>> Services = new Dictionary<string, List<ServiceConfigItem>>();

        protected static EventHandlerList eventHandler = new EventHandlerList();
        static readonly string configChangedKey = "configChangedKey";
        public static event EventHandler Changed
        {
            add
            {
                eventHandler.AddHandler(configChangedKey, value);
            }
            remove
            {
                eventHandler.RemoveHandler(configChangedKey, value);
            }
        }

        protected static void OnChanged(EventArgs args)
        {
            EventHandler handler = eventHandler[configChangedKey] as EventHandler;
            if (null != handler)
                handler(Instance, args);
        }

        static bool inited = false;
        static object initLock = new object();
        static ServiceConfigs()
        {
            if (inited) return;
            lock (initLock)
            {
                inited = true; 
                Init();
            }
        }

        static Encoding encode = Encoding.GetEncoding("GB2312");  

        static void Init()
        {
            try
            { 
                Instance = DeSerializeFromXml<ServiceConfigs>(Config.ServiceConfigPath);
                if (null == Instance)
                {
                    Instance = new ServiceConfigs(); 
                    Log.Write(LogAction.Write, className, "Init", "Init1", -1, "ServiceConfigs.Init:初始化配置失败:" + Config.ServiceConfigPath); 
                }
                if (null != Instance.ServiceConfigGroups)
                {
                    foreach (ServiceConfigGroup group in Instance.ServiceConfigGroups)
                    { 
                        Services[group.GroupName] = group.ServiceConfigItem;
                        if (group.GroupIndex > 0)
                        {
                            try
                            {
                                if (!ServiceGroups.ContainsKey(group.ParentGroupIndex))
                                {
                                    ServiceGroups[group.ParentGroupIndex] = new Dictionary<long, string>();
                                   // SubServiceGroups[group.ParentGroupIndex] = new Dictionary<long, string>();
                                }
                                ServiceGroups[group.ParentGroupIndex][group.GroupIndex] = group.GroupName;
                                //SubServiceGroups[group.ParentGroupIndex][group.SubGroupIndex] = group.GroupName;
                            }
                            catch(Exception ex)
                            {
                                Log.Write(LogAction.Error,  className, "Init","Init2", -1, "ServiceConfigs.Init:添加服务组错误:group.GroupName:"+group.GroupName+",group.ParentGroupIndex:" + group.ParentGroupIndex + ",group.GroupIndex:" + group.GroupIndex +", "+ ex.ToString());
                            }
                            //ServiceGroups[group.GroupIndex] = group.GroupName;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "Init", "Init3", -1, "ServiceConfigs.Init:初始化配置失败1:" + e.ToString());
            }
            finally
            {
                if(File.Exists(Config.ServiceConfigPath))
                HttpRuntime.Cache.Insert("Dova.Services.ConfingKey", new object(), new CacheDependency(Config.ServiceConfigPath), DateTime.Now.AddYears(1), Cache.NoSlidingExpiration, CacheItemPriority.High, new CacheItemRemovedCallback(ConfigChanged));
            }
            InitRoutings();
            try
            {

                if (null != Instance && null != Instance.LogConfigs)
                    _logSetting = Instance.LogConfigs;
                if (null != Instance && null != Instance.MsgQConfigs)
                    _msgQSetting = Instance.MsgQConfigs;
                ServiceType = Instance.InnerServiceType;
                RetryInteval = Instance.InnerRetryInteval;
                OpenAsyncDispatchThread = Instance.InnerOpenAsyncDispatchThread;
            }
            catch (Exception ex)
            {
                Log.Write(LogAction.Error, className, "Init", "Init4", -1, "ServiceConfigs.Init:初始化配置失败2:" + ex.ToString()); 
            }
            Log.Write(LogAction.Write, className, "Init", "Init5", -1, "ServiceConfigs.Init:初始化完毕:Services.Count=" + Services.Count + ",ServiceGroups.Count:" + ServiceGroups.Count);
        }

        protected static void ConfigChanged(string key, object value, CacheItemRemovedReason reason)
        {
            inited = false;
            Init();
            OnChanged(new EventArgs());

        }

        static void InitRoutings()
        {
            try
            {
                List<Routing> routings = Instance.RoutingItems;
                foreach (Routing r in routings)
                {
                    if (!Routings.ContainsKey(r.GroupName))
                    {
                        Routings[r.GroupName] = new Dictionary<string, Routing>();
                    }
                    Routings[r.GroupName][r.Key] = r;
                }
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "InitRoutings", "InitRoutings", -1, "ServiceConfigs.InitRoutings:初始化配置失败2:" + e.ToString());
            }

        }

        public static List<ServiceConfigItem> GetConfigs(string groupName)
        { 
            return Services[groupName];
        }

        protected static T DeSerializeFromXml<T>(string xmlFile)
        {
            try
            { 
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                using (StreamReader sr = new StreamReader(xmlFile))
                { 
                    T t= (T)serializer.Deserialize(sr);
                    sr.Close();
                    return t;
                }
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error,  className, "DeSerializeFromXml","DeSerializeFromXml1", -1, "ServiceConfigs.Init:初始化配置失败:" + e.ToString());  
                e = null;
            }
            return default(T);
        }


        public static List<PlugingItem> GetPlugings(string groupName)
        {
            return Instance.PlugingItems;
        }


        public static List<Routing> GetRoutings(string groupName)
        {
            return Instance.RoutingItems;
        }


        [XmlIgnore]
        static LogConfig _logSetting = new LogConfig();

        [XmlIgnore]
        public static LogConfig LogSetting
        {
            get { return _logSetting; }
            set { if (null != value) _logSetting = value; }
        }


        [XmlIgnore]
        static MsgQConfig _msgQSetting = new MsgQConfig();
        [XmlIgnore]
        public static MsgQConfig MsgQSetting
        {
            get { return _msgQSetting; }
            set { if (null != value) _msgQSetting = value; }
        }

    }
}
