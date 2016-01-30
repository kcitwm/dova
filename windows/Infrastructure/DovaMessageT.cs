//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.Serialization;
//using System.Security.Cryptography;
//using System.Text;

//namespace Dova
//{
//    [Serializable]
//    [DataContract]
//    public class DovaMessage<T>
//    {

//        int _sourceID = 0;
//        int _scenarioID = 0;
//        long _transactionID = 0; 
//        int _serviceID = 0;
//        int _methodID = 0;
//        int _version = 1;
//        long _keyID = 0;
//        long _timeStamp = 0;
//        string _routingKey = "";
//        T _body;

//        string _format = "Bin";

//        public DovaMessage() { }

//        /// <summary>
//        /// 基本构造函数
//        /// </summary>
//        /// <param name="serviceID">服务ＩＤ号</param>
//        /// <param name="methodID">方法ＩＤ</param>
//        /// <param name="body">传输数据</param>
//        public DovaMessage(int serviceID, int methodID, T body)
//        {
//            _serviceID = serviceID;
//            _methodID = methodID;
//            _body = body;
//        }

//       /// <summary>
//       /// 全构造函数
//       /// </summary>
//        /// <param name="sourceID">来源:发面消息的标志如:业务名称,IP等</param>
//        /// <param name="scenarioID">业务场景:每个系统全局定义业务场景,相当于业务模块的ID</param>
//        /// <param name="timeStamp">时间戳</param>
//        /// <param name="version">消息的版本号,用于控制并发.</param>
//        /// <param name="transID">事务ID: 根据业务定义场景事务,独立系统事务,全局跨系统事务, 可用SequenceID</param>
//        /// <param name="serviceID">服务ＩＤ号</param>
//        /// <param name="methodID">方法ＩＤ</param>
//        /// <param name="body">传输数据</param>
//        public DovaMessage(string sourceID,int scenarioID,long timeStamp,int version,int transID, int serviceID, int methodID, T body):this(serviceID,methodID,body)
//        {
//            _serviceID = serviceID;
//            _methodID = methodID;
//            _body = body;
//        }

//        /// <summary>
//        /// 来源业务ＩＤ
//        /// </summary>
//        [DataMember]
//        public int SourceID { get { return _sourceID; } set {_sourceID = value; } }

//        /// <summary>
//        /// 业务场景:每个系统全局定义业务场景,相当于业务模块的ID
//        /// </summary>
//        [DataMember]
//        public int ScenarioID { get { return _scenarioID; } set { _scenarioID = value; } }

//        /// <summary>
//        /// 当前的时间戳
//        /// </summary>
//        [DataMember]
//        public long TimeStamp
//        {
//            get
//            {
//                if (_timeStamp == 0)
//                    _timeStamp= DateTime.Now.Ticks;
//                return _timeStamp;
//            }
//            set { _timeStamp = value; }
//        }

//        /// <summary>
//        /// 消息的版本号,用于控制并发.
//        /// </summary>
//        [DataMember]
//        public int Version { get { return _version; } set { _version = value; } }

//        /// <summary>
//        /// 事务ID: 根据业务定义场景事务,独立系统事务,全局跨系统事务, 可用SequenceID
//        /// 默认为随机生成的ＩＤ
//        /// </summary>
//        [DataMember]
//        public long TransactionID
//        {
//            get
//            {
//                if (_transactionID == 0)
//                {
//                    RNGCryptoServiceProvider csp = new RNGCryptoServiceProvider();
//                    byte[] bs = new byte[16];
//                    csp.GetBytes(bs);
//                    _transactionID = BitConverter.ToInt64(bs, 0);
//                }
//                return _transactionID;
//            }
//            set { _transactionID = value; }
//        }

//        /// <summary>
//        ///  请求的服务ID:全公司统一分配的服务ＩＤ号
//        /// </summary>
//        [DataMember]
//        public int ServiceID { get { return _serviceID; } set { _serviceID = value; } }

//        /// <summary>
//        ///   操作ＩＤ：操作类型的指示：就是消息类型用于指示接收端收到消息后怎么处理Body
//        /// </summary>
//        [DataMember]
//        public int MethodID { get { return _methodID; } set { _methodID = value; } }

//        /// <summary>
//        /// 业务数据的主健ＩＤ
//        /// </summary>
//        [DataMember]
//        public long KeyID { get { return _keyID; } set { _keyID = value; } }

//        /// <summary>
//        /// 路由关键字
//        /// </summary>
//        [DataMember]
//        public string RoutingKey { get { return _routingKey; } set { if (null != value) _routingKey = value; } }


//        /// <summary>
//        /// 序列化格式 JSON 　BIN XML
//        /// </summary>
//        [DataMember]
//        public string Format { get { return _format; } set { if (null != value) _format = value.ToLower(); } }



//        /// <summary>
//        /// 创建时间
//        /// </summary>
//        [DataMember]
//        public DateTime CreateTime { get { return DateTime.Now; } set{} }

//        /// <summary>
//        /// 消息体：注意所有消息内容，内嵌内容都要标记为可序列化
//        /// </summary>
//        [DataMember]
//        public T Body { get { return _body; } set { _body = value; } }

//        public override string ToString()
//        {
//            return  "Header:"+_sourceID+","+ _serviceID+","+ _scenarioID+","+ _methodID+","+ _keyID+","+_routingKey+","+_format+","+ _timeStamp+","+ _version+";Body:"+_body.ToString();
//        }


//    } 
//}
