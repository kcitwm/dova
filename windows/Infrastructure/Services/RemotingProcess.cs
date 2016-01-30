using System;
using System.ServiceModel;

using Dova.Utility;
using Dova.Interfaces;

namespace Dova.Services
{

    public class RemotingProcess<T> : ServiceProcess<T>  
    { 

        public RemotingProcess(ServiceConfigItem config) : base(config)
        { 
        } 

        protected override T CreateInstance()
        {
            try
            {
                Log.Write("RemotingProcess:" + _config.Url);
                return (T)Activator.GetObject(typeof(T), _config.Url);
            }
            catch (Exception e)
            {
                Log.Error("RemotingProcess:" + e.Message); 
            }
            return default(T);
        } 

    }
}
