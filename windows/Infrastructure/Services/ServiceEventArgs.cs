using System;
using System.ServiceModel;

namespace Dova.Services
{
    public class ServiceEventArgs: EventArgs
    {
        private string _routKey = "";
        public string RoutKey
        {
            get { return _routKey; }
            set { if (null != value) _routKey = value; }
        }

        public Object Channel { get; set; }

    }
}
