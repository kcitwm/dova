using System;
using System.ServiceModel;
using Dova.Interfaces;
using Dova.Utility;
using System.Net.Sockets;

namespace Dova.Services
{
    public class NetProcess<T> : ServiceProcess<T>
    {
        object s = null;
        public NetProcess(ServiceConfigItem config)
            : base(config)
        {
            if (null == s)
            { 
                ProtocolType type = ProtocolType.Tcp;
                s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, type);
            }
        }

        //T instance;
        protected override T CreateInstance()
        {
            if (null == t)
            {
                t = (T)s;
            }
            return t;
        }

      
        
        public override void Close(bool force)
        {
            if (null != s)
                (s as Socket).Close();
        }

    }
}
