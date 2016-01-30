using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dova.Handlers
{
    public class StatusHandler : Dova.MessageQueue.MessageHandler
    {
        public override bool ExecuteMQ(string serviceName, WQMessage message)
        { 
            return true;
        }

        public override object ExecuteMessageRequest(string serviceName, WQMessage message)
        {
            //1. 按照Key主键更新本地的状态,返回下一个节点的地址
            //2. 判断下一个节点地址去空格后是否和上节点去空格后一致.
            //3. 如果地址不一致,则一请求下一个节点DAC服务
            return message.UserToken;
        }
    }
}
