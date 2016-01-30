using System;
using System.ServiceModel;

namespace Dova.Infrastructure.Utility
{
    public class SocketEventArgs: EventArgs
    {
        public int Sent { get; set; }
        public int Received { get; set; }
        public string Data { get; set; }

    }
}
