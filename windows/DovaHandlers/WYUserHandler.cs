
using Dova;
using Dova.Utility;
using PersistentSocketAsyncServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dova.Data;

namespace Dova.Handlers
{
    public class WYUserHandler : Dova.MessageQueue.MessageHandler
    {
        public override bool ExecuteMQ(string serviceName, WQMessage message)
        { 
            return true;
        }

        public override object ExecuteMessageRequest(string serviceName, WQMessage message)
        { 
            string msg = SerializeHelper.SerializeToJSon(message, true);
            Log.Info("WYUserHandler.ExecuteMessageRequest:" + msg);
            if (message.ServiceName == "LoginService")
            {
                //DAC dac = new DAC(Configs.WeiYunConnection);


                return null;
            }
            //else return null;
            return message.UserToken;
        }

    }
}
