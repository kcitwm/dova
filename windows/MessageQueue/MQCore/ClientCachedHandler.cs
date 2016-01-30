using c2cplatform.component.msgqv5;
using WQFree.Interfaces;
using WQFree.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WQFree.MessageQueue
{
    /// <summary>
    /// 数据库缓存接收端数据
    /// </summary>
    public class ClientCachedHandler:MessageHandler
    {
        protected override void ClearMessage(List<CConsumerMsgInfo> msg)
        {  
        }


        /// <summary>
        /// 从数据库里面获取数据.
        /// </summary>
        /// <param name="consumerID"></param>
        /// <param name="clientType"></param>
        /// <returns></returns>
        protected override List<CConsumerMsgInfo> GetMessage(int consumerID)
        {
            return base.GetMessage(consumerID);
        } 

         


    }
}
