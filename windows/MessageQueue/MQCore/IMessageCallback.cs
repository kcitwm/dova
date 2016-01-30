using System;
using System.ServiceModel;

namespace WQFree.MessageQueue
{
    public interface IMessageCallback 
    {
        [OperationContract(IsOneWay = true)]
        void RequestCallback(long keyID, object obj,object result);

        [OperationContract(IsOneWay = true)]
        void RequestMessageCallback(WQFreeMessage msg, object result); 

    }
}
