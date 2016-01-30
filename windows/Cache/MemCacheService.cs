using WQFree.Interfaces;
using WQFree.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
using System.Text; 

namespace WQFree.Cache
{
    [ServiceBehavior(MaxItemsInObjectGraph = 0x7fffffff, InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple, AddressFilterMode = AddressFilterMode.Any)]
    public class MemCacheService : ICache,IDisposable
    {
        static string className = "MemCacheService";

        private static string dependencyFilePath = "";
        MemoryCache cache = MemoryCache.Default;

        public void Init()
        {
        }


        
        public void Remove(string key)
        {
            cache.Remove(key);
        }

        public object Get(string key)
        {
            TimeSpan ts = new TimeSpan(DateTime.Now.Ticks);
            object obj2 = cache.Get(key);
            TimeSpan span2 = new TimeSpan(DateTime.Now.Ticks);
            Log.Write(LogAction.Get, string.Concat(new object[] { "APGet:t2-t1=", span2.Subtract(ts).Milliseconds, " :key=", key }));
            return obj2;
        }

        
        public void Set(string key, object value)
        {
            if (value == null)
            {
                Log.Write("Set Key:为空值");
            }
            else
            {
                TimeSpan ts = new TimeSpan(DateTime.Now.Ticks);
                cache.Set(key, value, null);
                TimeSpan span2 = new TimeSpan(DateTime.Now.Ticks);
                Log.Write(LogAction.Set, string.Concat(new object[] { "APSet:t2-t1=", span2.Subtract(ts).Milliseconds, " :key=", key }));
            }
        }

        
        public void Set(string key, object value, DateTime absoluteExpiration, TimeSpan solidingExpiration)
        {
            if (value != null)
            {
                CacheItemPolicy cip = new CacheItemPolicy();
                cip.AbsoluteExpiration = new DateTimeOffset(absoluteExpiration);
                cip.SlidingExpiration = solidingExpiration;
                cache.Set(key, value, cip);
                return;
            }
            Log.Write("Set Key:为空值");
        }

        
        public void Set(string key, object value, TimeSpan solidingExpiration)
        {
            if (value != null)
            {
                CacheItemPolicy cip = new CacheItemPolicy(); 
                cip.SlidingExpiration = solidingExpiration;
                cache.Set(key, value, cip);
                return;
            }
            Log.Write("Set Key:为空值");
        }


        
        public void Set(string key, object value, DateTime absoluteExpiration, TimeSpan solidingExpiration, int priority)
        {
            if (value != null)
            {
                CacheItemPolicy cip = new CacheItemPolicy();
                cip.Priority = (CacheItemPriority)priority;
                cip.AbsoluteExpiration = new DateTimeOffset(absoluteExpiration);
                cip.SlidingExpiration = solidingExpiration;
                cache.Set(key, value, cip);
                return;
            }
            Log.Write("Set Key:为空值");
        }

        
        public void Set(string key, object value, string dependencyFileName, DateTime absoluteExpiration, TimeSpan solidingExpiration)
        {
            if (value != null)
            {
                CacheItemPolicy cip = new CacheItemPolicy(); 
                cip.ChangeMonitors.Add(new HostFileChangeMonitor( dependencyFileName.Split(new char[]{ ';' }).ToList<string>()));
                cip.AbsoluteExpiration = new DateTimeOffset(absoluteExpiration);
                cip.SlidingExpiration = solidingExpiration;
                cache.Set(key, value, cip);
                return;
            }
            Log.Write("Set Key:为空值");
        }

        
        public void Set(string key, object value, string dependencyFileName, DateTime absoluteExpiration, TimeSpan solidingExpiration, int priority)
        {
            if (value != null)
            {
                CacheItemPolicy cip = new CacheItemPolicy(); 
                cip.Priority = (CacheItemPriority)priority;
                cip.ChangeMonitors.Add(new HostFileChangeMonitor( dependencyFileName.Split(new char[]{ ';' }).ToList<string>()));
                cip.AbsoluteExpiration = new DateTimeOffset(absoluteExpiration);
                cip.SlidingExpiration = solidingExpiration;
                cache.Set(key, value, cip);
                return;
            }
            Log.Write("Set Key:为空值");
        }




        [System.Diagnostics.Conditional("Test")]
        private void WriteLog(string msg)
        {
            Log.Write(msg);
        }

        public void Dispose()
        {
            
        }
    }

}
