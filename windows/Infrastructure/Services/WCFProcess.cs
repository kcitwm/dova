using System;
using System.ServiceModel;
using Dova.Interfaces;
using Dova.Utility;
using System.ServiceModel.Activation;

namespace Dova.Services
{
    public class WCFProcess<T> : ServiceProcess<T>
    {
        ChannelFactory<T> factory = null; 
       // WCFProcess<T> process = null;

        public WCFProcess(ServiceConfigItem config)
            : base(config)
        {
            if (null == factory)
            {
                factory = new ChannelFactory<T>(_config.EndPoint); 
            }
        }

        //T instance;
        protected override T CreateInstance()
        { 
            //if (null == instance)
            //{
            T t=  factory.CreateChannel();
            //factory.Faulted += Factory_Faulted;
            return t;
        }

        void Factory_Faulted(object sender, EventArgs e)
        {
            ServiceEventArgs args = new ServiceEventArgs();
            args.RoutKey = this._config.RoutKey;
            OnFault(args);
        }
         

        public override void Close(bool force)
        {
            //OnClose(this);
            //if (force)
            //{
            //    try
            //    {
            //        IClientChannel channel = instance as IClientChannel;
            //        channel.CloseConnection();
            //    }
            //    catch { }
            //}
        }

    }
}
