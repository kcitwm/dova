namespace Dova.Cache
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Runtime.Remoting.Messaging;
    using System.ServiceModel;
    using System.Web;
    using System.Web.Caching;
    using Dova.Utility;
    using System.Text;
    using System.IO;
    using System.Xml;
    using System.Net.Sockets;
    using Dova.Interfaces;

    [ServiceBehavior(MaxItemsInObjectGraph = 0x7fffffff, InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class APICache : MarshalByRefObject, ICache
    {
        private static string dependencyFilePath = "";

        static APICache()
        {
            new APICache().Init();
            Log.Write("初始化了APICache");
        }

        public void Init()
        {
        }


        [OneWay]
        void ICache.Remove(string key)
        { 
            HttpRuntime.Cache.Remove(key);
        }

        object ICache.Get(string key)
        {
            TimeSpan ts = new TimeSpan(DateTime.Now.Ticks);
            object obj2 = HttpRuntime.Cache.Get(key);
            TimeSpan span2 = new TimeSpan(DateTime.Now.Ticks);
            Log.Write(LogAction.Get, string.Concat(new object[] { "APICache.Get:t2-t1=", span2.Subtract(ts).Milliseconds, " :key=", key }));
            return obj2;
        }

        [OneWay]
        void ICache.Set(string key, object value)
        {
            if (value == null)
            {
                Log.Write("Set Key:为空值");
            }
            else
            {
                TimeSpan ts = new TimeSpan(DateTime.Now.Ticks);
                HttpRuntime.Cache.Insert(key, value);
                TimeSpan span2 = new TimeSpan(DateTime.Now.Ticks);
                Log.Write(LogAction.Set, string.Concat(new object[] { "APICache.Set:t2-t1=", span2.Subtract(ts).Milliseconds, " :key=", key }));
            }
        }

        [OneWay]
        void ICache.Set(string key, object value, DateTime absoluteExpiration, TimeSpan solidingExpiration)
        {
            if (value == null)
            {
                Log.Write("Set Key:为空值");
            }
            else
            {
                TimeSpan ts = new TimeSpan(DateTime.Now.Ticks);
                HttpRuntime.Cache.Insert(key, value, null, absoluteExpiration, solidingExpiration);
                TimeSpan span2 = new TimeSpan(DateTime.Now.Ticks);
                Log.Write(LogAction.Set, string.Concat(new object[] { "APICache.Set:t2-t1=", span2.Subtract(ts).Milliseconds, " :key=", key }));
            }
        }

        [OneWay]
        void  Set(string key, object value, DateTime absoluteExpiration, TimeSpan solidingExpiration, CacheItemPriority priority)
        {
            if (value == null)
            {
                Log.Write("Set Key:为空值");
            }
            else
            {
                TimeSpan ts = new TimeSpan(DateTime.Now.Ticks);
                HttpRuntime.Cache.Insert(key, value, null, absoluteExpiration, solidingExpiration, priority, null);
                TimeSpan span2 = new TimeSpan(DateTime.Now.Ticks);
                Log.Write(LogAction.Set, string.Concat(new object[] { "APICache.Set:t2-t1=", span2.Subtract(ts).Milliseconds, " :key=", key }));
            }
        }

        [OneWay]
        void ICache.Set(string key, object value, string dependencyFileName, DateTime absoluteExpiration, TimeSpan solidingExpiration)
        {
            if (value == null)
            {
                Log.Write("Set Key:为空值");
            }
            else
            {
                TimeSpan ts = new TimeSpan(DateTime.Now.Ticks);
                HttpRuntime.Cache.Insert(key, value, new CacheDependency(dependencyFilePath + dependencyFileName), absoluteExpiration, solidingExpiration);
                TimeSpan span2 = new TimeSpan(DateTime.Now.Ticks);
                Log.Write(LogAction.Set, string.Concat(new object[] { "APICache.Set:t2-t1=", span2.Subtract(ts).Milliseconds, " :key=", key }));
            }
        }

        [OneWay]
        void  Set(string key, object value, string dependencyFileName, DateTime absoluteExpiration, TimeSpan solidingExpiration, CacheItemPriority priority)
        {
            if (value == null)
            {
                Log.Write("Set Key:为空值");
            }
            else
            {
                TimeSpan ts = new TimeSpan(DateTime.Now.Ticks);
                HttpRuntime.Cache.Insert(key, value, new CacheDependency(dependencyFilePath + dependencyFileName), absoluteExpiration, solidingExpiration, priority, null);
                TimeSpan span2 = new TimeSpan(DateTime.Now.Ticks);
                Log.Write(LogAction.Set, string.Concat(new object[] { "APICache.Set:t2-t1=", span2.Subtract(ts).Milliseconds, " :key=", key }));
            }
        }


        [Conditional("Test")]
        private void TestSync()
        {
            int num = 0;
            while (num < 5)
            {
                num++;
                new WebClient().DownloadData("http://localhost:3348/InitData.aspx");
            }
        }

        [Conditional("Test")]
        private void WriteLog(string msg)
        {
            Log.Write(msg);
        }


        public void Set(string key, object value, DateTime absoluteExpiration, TimeSpan solidExpiration, int priority)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, object value, string dependencyFileName, DateTime absoluteExpiration, TimeSpan solidingExpiration, int priority)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, object value, TimeSpan solidExpiration)
        {
            throw new NotImplementedException();
        }
    }
}

