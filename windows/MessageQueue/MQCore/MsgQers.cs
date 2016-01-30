using System;
using System.Collections.Generic;

namespace WQFree.MessageQueue
{
    public class MsgQers
    {
        static Dictionary<string, c2cplatform.component.msgqv5.CMqProducer> producers = new Dictionary<string, c2cplatform.component.msgqv5.CMqProducer>();
        static Dictionary<string, c2cplatform.component.msgqv5.CMqConsumer> consumers = new Dictionary<string, c2cplatform.component.msgqv5.CMqConsumer>();

        public static c2cplatform.component.msgqv5.CMqProducer GetProcudcer(int topicID, string pwd, string encode = "utf-8", string admin = "")
        {
            string key = topicID + pwd + encode + admin;
            c2cplatform.component.msgqv5.CMqProducer producer = null;
            if (!producers.ContainsKey(key))
            {
                producer = c2cplatform.component.msgqv5.CMqFactory.GetProducer((uint)topicID);
                producer.Init(pwd, encode, admin);
                producers[key] = producer;

            }
            return producer;
        }


        public static c2cplatform.component.msgqv5.CMqConsumer GetConsumer(int topicID, string pwd, string encode = "utf-8", string admin = "")
        {
            string key = topicID + pwd + encode + admin;
            c2cplatform.component.msgqv5.CMqConsumer consumer = null;
            if (!producers.ContainsKey(key))
            {
                consumer = c2cplatform.component.msgqv5.CMqFactory.GetConsumer((uint)topicID);
                consumer.Init(pwd, encode, admin);
                consumers[key] = consumer;
            }
            return consumer;
        }



    }
}
