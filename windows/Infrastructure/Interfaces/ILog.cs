using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Security;
using System.ServiceModel;
using System.Text;

namespace Dova.Interfaces
{
    [ServiceContract(ProtectionLevel = ProtectionLevel.None)]  
    public interface ILog
    {
        [OperationContract(IsOneWay = true   )]
        void PushLogs(string action, string msg);

        [OperationContract(IsOneWay = true,  Name="PushLogsList")] 
        void PushLogs(List<string> actions, List<string> msgs);
         
        [OperationContract(IsOneWay = true,  Name = "PushLogsList2")] 
        void PushLogs(List<string> actions, List<byte[]> msgs);


        [OperationContract(IsOneWay = true,  Name = "PushLogsList3")]
        void PushLogs(Queue<KeyValuePair<string, StringBuilder>> msgs);

        [OperationContract(IsOneWay = true, Name = "PushLogsList4")]
        void PushLogs(List<string> actions, List<StringBuilder> msgs);
         
    }
}
