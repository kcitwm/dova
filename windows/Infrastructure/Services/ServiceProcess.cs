using System;

using Dova.Utility; 
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.ComponentModel;

namespace Dova.Services
{
    public class ServiceProcess<T> : IDisposable
    {
        protected ServiceConfigItem _config = null;

        DateTime now = DateTime.Now;

        protected string faultEventKey = "FaultEventKey";
        protected string closeEventKey = "closeEventKey";
        protected EventHandlerList events = new EventHandlerList();
        public event EventHandler Fault
        {
            add
            {
                events.AddHandler(faultEventKey, value);
            }
            remove
            {
                events.RemoveHandler(faultEventKey, value);
            }
        }

        public event EventHandler Closes
        {
            add
            {
                events.AddHandler(closeEventKey, value);
            }
            remove
            {
                events.RemoveHandler(closeEventKey, value);
            }
        }

        protected void OnFault(ServiceEventArgs args)
        {
            EventHandler handler = events[faultEventKey] as EventHandler;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        protected void OnClose(ServiceEventArgs args)
        {
            EventHandler handler = events[closeEventKey] as EventHandler;
            if (null != handler)
            {
                handler(this, args);
            }
        }


        public ServiceConfigItem Config
        {
            get { return _config; }
            set { _config = value; }
        }

        protected string _id = string.Empty;


        public string ID
        {
            get { return _id; }
        }
         

        public ServiceProcess()
        { 
        }

        public ServiceProcess(ServiceConfigItem config)
            : this()
        {
            this._config = config;
        }

        protected T t = default(T);

        protected virtual T CreateInstance()
        {
            if (null == t)
            {
                Type type = Type.GetType(Config.Url);
                t = (T)Activator.CreateInstance(type);
                Log.Info("创建缓存类:" + type);
            }
            return t;
        }

        static string key = "ServerProcessKey";

        public virtual T Instance()
        {
            T t = default(T);
            try
            {
                t = CreateInstance(); 
            }
            catch (Exception e)
            {
                Close(true);
                t = CreateInstance(); 
            }
            return t;
        }

        public virtual void Close()
        {
            Close(true);
        }

        public virtual void Close(bool force)
        {
        }

        public void Dispose()
        {
            Close(true);
        }
    }
}
