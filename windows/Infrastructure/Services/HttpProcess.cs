using System;
using System.ServiceModel;
using Dova.Interfaces;
using Dova.Utility;
using System.ServiceModel.Activation;

namespace Dova.Services
{
    public class HttpProcess<T> : ServiceProcess<T>
    { 
        public HttpProcess(ServiceConfigItem config)
            : base(config)
        { 
        }
         

        protected override T CreateInstance()
        {
            if (null == t)
            {
                object o = new HttpProxy(Config.Url);
                t = (T)o; 
            }
            return t;
        }
         

    }
}
