using Dova;
using Dova.Utility;
using PersistentSocketAsyncServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dova.Handlers
{
    public class PushMessageHandler : Dova.MessageQueue.MessageHandler
    {
        public override bool ExecuteMQ(string serviceName, WQMessage message)
        {
            string endpoint = Configs.MessageIPE.ToString();
            string msg = SerializeHelper.SerializeToJSon(message, true);
            Log.Info("PushMessageHandler.ExecuteMQ:" + msg + " endpoint:" + endpoint + ", message.TargetID:" + message.TargetID);
            byte[] bs = Config.Encode.GetBytes(msg); 
            return SocketListener.GetInstance(endpoint).Send(message.TargetID, bs); 
        }

        public override object ExecuteMessageRequest(string serviceName, WQMessage message)
        {
            string endpoint = Configs.MessageIPE.ToString();
            string msg = SerializeHelper.SerializeToJSon(message, true); 
            Log.Info("PushMessageHandler.ExecuteMessageRequest:" + msg + " endpoint:" + endpoint + ", message.TargetID:" + message.TargetID);
            if (message.ServiceName != "LoginService")
            {
                byte[] bs = Config.Encode.GetBytes(msg); 
                SocketListener.GetInstance(endpoint).Send(message.TargetID, bs);
                return null;
            }
            //else return null;
            return message.UserToken;
        }

    }
}
