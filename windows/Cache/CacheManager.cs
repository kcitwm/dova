using Dova.Interfaces;
using Dova.Services;
using Dova.Utility;
using System;
using System.Collections.Generic;
using System.Linq; 
using System.Text;
using System.Web.Caching; 

namespace Dova.Cache
{
    public class CacheManager
    {
        static string className = "CacheManager";
        protected string apiCacheKeys = string.Empty;
        static string cacheServiceName = "CacheService";

        public static void Remove(string serviceName,string key)
        {
            ServiceProcess<ICache> sf = ServiceFactory<ICache>.GetServer(serviceName);
            using (IDisposable service = sf.Instance() as IDisposable)
            {
                long ts = DateTime.Now.Ticks;
                 (service as ICache).Remove(key);
                 Log.Write(LogAction.Set, string.Format(Config.LogFormat2, className, "Remove", DateTime.Now.Ticks - ts, key));
                 return;
            } 
        }

        public static void Remove( string key)
        {
            Remove(cacheServiceName, key);
        }

        public static void Set(string key, object value)
        {
            Set(cacheServiceName, key, value);
        }


        public static object Get(string serviceName, string key)
        {
            ServiceProcess<ICache> sf = ServiceFactory<ICache>.GetServer(serviceName);
            long ts = DateTime.Now.Ticks;
            using (IDisposable service = sf.Instance() as IDisposable)
            {
                APICache mcs = service as APICache; 
                object o = (service as ICache).Get(key); 
                if(null!=o)
                    o = SerializeHelper.DeSerializeFromBytes(o as byte[]); 
                Log.Write(LogAction.Get,Config.AppName,className,"Get",DateTime.Now.Ticks-ts,key);
                return o;
            } 
        }

        public static object Get( string key)
        {
            return Get(cacheServiceName, key);
        }


        public static void Set(string serviceName, string key, object value)
        {
            ServiceProcess<ICache> sf = ServiceFactory<ICache>.GetServer(serviceName);
            int max = sf.Config.MaxObjectSize;
            byte[] buffer = SerializeHelper.SerializeToBytes(value);
            if (buffer.Length < max)
            {
                using (IDisposable service = sf.Instance() as IDisposable)
                {
                    if (buffer.Length < max)
                    { 
                        long ts = DateTime.Now.Ticks; 
                        (service as ICache).Set(key, buffer);
                        long te = DateTime.Now.Ticks;
                        Log.Write(LogAction.Set, Config.AppName, className, "Set", DateTime.Now.Ticks - ts, key); 
                        return;
                    }
                }
            } 
        }


        public static void Set(string key,TimeSpan slidExpiration, object value )
        {
            Set(cacheServiceName, key, value, slidExpiration);
        }

        public static void Set(string serviceName, string key, object value, TimeSpan slidExpiration)
        {
            try
            {
                ServiceProcess<ICache> sf = ServiceFactory<ICache>.GetServer(serviceName);
                int max = sf.Config.MaxObjectSize;
                byte[] buffer = SerializeHelper.SerializeToBytes(value);
                if (buffer.Length < max)
                {
                    using (IDisposable service = sf.Instance() as IDisposable)
                    {
                        if (buffer.Length < max)
                        {
                            long ts = DateTime.Now.Ticks;

                            (service as ICache).Set(key, buffer, slidExpiration);
                            long te = DateTime.Now.Ticks;
                            Log.Write(LogAction.Set, Config.AppName, className, "Set", DateTime.Now.Ticks - ts, "serviceName:" + serviceName + ";Key:" + key);
                            return;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("CacheManager.Set:" + e.Message);
            }
        }

        public void Set(string serviceName, string key, object value, DateTime absoluteExpiration, TimeSpan slidExpiration, CacheItemPriority priority)
        {
            ServiceProcess<ICache> sf = ServiceFactory<ICache>.GetServer(serviceName);
            int max = sf.Config.MaxObjectSize;
            byte[] buffer = SerializeHelper.SerializeToBytes(value);
            if (buffer.Length < max)
            {
                using (IDisposable service = sf.Instance() as IDisposable)
                {
                    if (buffer.Length < max)
                    {
                        long ts = DateTime.Now.Ticks;

                        (service as ICache).Set(key, buffer, absoluteExpiration, slidExpiration, (int)priority);
                        long te = DateTime.Now.Ticks;
                        Log.Write(LogAction.Set, string.Format(Config.LogFormat2, className, "Set", te - ts, key));
                        return;
                    }
                }
            }
            Log.Warn(string.Format(Config.LogFormat2, className, "Set", 0, key));
        }

        public void Set(string key, object value, DateTime absoluteExpiration, TimeSpan slidExpiration, CacheItemPriority priority)
        {
            Set(cacheServiceName, key, value, absoluteExpiration, slidExpiration, priority);
        }

        public void Set(string serviceName, string key, object value, string dependencyFileName, DateTime absoluteExpiration, TimeSpan slidExpiration)
        {
            ServiceProcess<ICache> sf = ServiceFactory<ICache>.GetServer(serviceName);
            int max = sf.Config.MaxObjectSize;
            byte[] buffer = SerializeHelper.SerializeToBytes(value);
            if (buffer.Length < max)
            {
                using (IDisposable service = sf.Instance() as IDisposable)
                {
                    if (buffer.Length < max)
                    {
                        long ts = DateTime.Now.Ticks;
                        (service as ICache).Set(key, buffer, dependencyFileName, absoluteExpiration, slidExpiration);
                        long te = DateTime.Now.Ticks;
                        Log.Write(LogAction.Set, string.Format(Config.LogFormat2, className, "Set", te - ts, key));
                        return;
                    }
                }
            }
            Log.Warn(string.Format(Config.LogFormat2, className, "Set", 0, key));
        }


        public void Set(string key, object value, string dependencyFileName, DateTime absoluteExpiration, TimeSpan slidExpiration)
        {
            Set(cacheServiceName, key, value, dependencyFileName, absoluteExpiration, slidExpiration);
        }


        public void Set(string serviceName, string key, object value, string dependencyFileName, DateTime absoluteExpiration, TimeSpan slidExpiration, CacheItemPriority priority)
        {
            ServiceProcess<ICache> sf = ServiceFactory<ICache>.GetServer(serviceName);
            int max = sf.Config.MaxObjectSize;
            byte[] buffer = SerializeHelper.SerializeToBytes(value);
            if (buffer.Length < max)
            {
                using (IDisposable service = sf.Instance() as IDisposable)
                {
                    if (buffer.Length < max)
                    {
                        long ts = DateTime.Now.Ticks;
                        (service as ICache).Set(key, buffer, dependencyFileName, absoluteExpiration, slidExpiration, (int)priority);
                        long te = DateTime.Now.Ticks;
                        Log.Write(LogAction.Set, string.Format(Config.LogFormat2, className, "Set", te - ts, key));
                        return;
                    }
                }
            }
            Log.Warn(string.Format(Config.LogFormat2, className, "Set", 0, key));
        }
    }

}
