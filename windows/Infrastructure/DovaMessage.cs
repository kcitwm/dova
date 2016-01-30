using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using Dova.Utility;

namespace Dova
{
    [Serializable]
    [DataContract]
    public class WQMessage
    { 
        string _appName = "MessageService";
        int _sourceID = 0;
        int _scenarioID = 0;
        long _transactionID = 0; 
        int _serviceID = 0;
        int _methodID = 0;
        int _version = 1;
        string _topic = string.Empty;
        long _keyID = 0;
        long _timeStamp = 0;
        string _routingKey = string.Empty;
        long _subRoutingKey =0;
        string _serviceName = string.Empty;
        object _body = new object();
        string _userToken = string.Empty;
        bool _async = false;
        string _targetID = string.Empty; 
        string _autho = string.Empty;
        string _format = "json";
        string _methodName = string.Empty;

        public string AppName { get { return _appName; } set { if (null != value) _appName = value; } }


        string _message = string.Empty;
        [DefaultValue("")]
        [DataMember]
        public string Message{get{return _message;} set{if(null!=value) _message=value;}}

        [DataMember]
        [DefaultValue(0)]
        public int Status = 0;
         
         

        public WQMessage() { }

        /// <summary>
        /// 基本构造函数
        /// </summary>
        /// <param name="serviceID">服务ＩＤ号</param>
        /// <param name="methodID">方法ＩＤ</param>
        /// <param name="body">传输数据</param>
        public WQMessage(int serviceID, int methodID, object body)
        {
            _serviceID = serviceID;
            _methodID = methodID;
            _body = body;
        }

       /// <summary>
       /// 全构造函数
       /// </summary>
        /// <param name="sourceID">来源:发面消息的标志如:业务名称,IP等</param>
        /// <param name="scenarioID">业务场景:每个系统全局定义业务场景,相当于业务模块的ID</param>
        /// <param name="timeStamp">时间戳</param>
        /// <param name="version">消息的版本号,用于控制并发.</param>
        /// <param name="transID">事务ID: 根据业务定义场景事务,独立系统事务,全局跨系统事务, 可用SequenceID</param>
        /// <param name="serviceID">服务ＩＤ号</param>
        /// <param name="methodID">方法ＩＤ</param>
        /// <param name="body">传输数据</param>
        public WQMessage(int sourceID,int scenarioID,long timeStamp,int version,int transID, int serviceID, int methodID, object body):this(serviceID,methodID,body)
        {
            _sourceID = sourceID;
            _scenarioID = scenarioID;
            _timeStamp = timeStamp;
            _version = version;
            _transactionID = transID;
            _serviceID = serviceID;
            _methodID = methodID; 
        }


        public string ThreadId
        {
            get
            { 
                return Config.ThreadId; 
            }
        }

        /// <summary>
        /// 来源业务ＩＤ
        /// </summary>
        [DataMember]
        [DefaultValue(0)]
        public int SourceID { get { return _sourceID; } set {_sourceID = value; } }

        /// <summary>
        /// 业务场景:每个系统全局定义业务场景,相当于业务模块的ID
        /// </summary>
        [DataMember]
        [DefaultValue(0)]
        public int ScenarioID { get { return _scenarioID; } set { _scenarioID = value; } }

        /// <summary>
        /// 当前的时间戳
        /// </summary>
        [DataMember]
        [DefaultValue(0)]
        public long TimeStamp
        {
            get
            {
                if (_timeStamp == 0)
                    _timeStamp= DateTime.Now.Ticks;
                return _timeStamp;
            }
            set { _timeStamp = value; }
        }

        /// <summary>
        /// 消息的版本号,用于控制并发.
        /// </summary>
        [DataMember]
        [DefaultValue(0)]
        public int Version { get { return _version; } set { _version = value; } }

        [DataMember]
        [DefaultValue(false)]
        public bool Async { get { return _async; } set { _async = value; } }

        /// <summary>
        /// 事务ID: 根据业务定义场景事务,独立系统事务,全局跨系统事务, 可用SequenceID
        /// 默认为随机生成的ＩＤ
        /// </summary>
        [DataMember] 
        public long TransactionID
        {
            get
            {
                if (_transactionID == 0)
                {
                    RNGCryptoServiceProvider csp = new RNGCryptoServiceProvider();
                    byte[] bs = new byte[16];
                    csp.GetBytes(bs);
                    _transactionID = BitConverter.ToInt64(bs, 0);
                }
                return _transactionID;
            }
            set { _transactionID = value; }
        }
         
        [DataMember]
        [DefaultValue("")]
        public string TargetID { get { return _targetID; } set { _targetID = value; } }

        [DataMember]
        public string Topic { get { return _topic; } set { _topic = value; } }


        /// <summary>
        ///  请求的服务ID:全公司统一分配的服务ＩＤ号
        /// </summary>
        [DataMember]
        [DefaultValue(0)]
        public int ServiceID { get { return _serviceID; } set { _serviceID = value; } }

        /// <summary>
        ///  请求的服务ID:全公司统一分配的服务名
        /// </summary>
        [DataMember]
        [DefaultValue("")]
        public string ServiceName { get { return _serviceName; } set { _serviceName = value; } }

        [DataMember]
        [DefaultValue("")]
        public string Autho { get { return _autho; } set { _autho = value; } }


        [DataMember]
        [DefaultValue("")]
        public string UserToken { get { return _userToken; } set { _userToken = value; } }


        /// <summary>
        ///   操作ＩＤ：操作类型的指示：就是消息类型用于指示接收端收到消息后怎么处理Body
        /// </summary>
        [DataMember]
        [DefaultValue(0)]
        public int MethodID { get { return _methodID; } set { _methodID = value; } }


        /// <summary>
        ///   操作ＩＤ：操作类型的指示：就是消息类型用于指示接收端收到消息后怎么处理Body
        /// </summary>
        [DataMember]
        [DefaultValue("")]
        public string MethodName { get { return _methodName; } set { _methodName = value; } }


        /// <summary>
        /// 业务数据的主健ＩＤ
        /// </summary>
        [DataMember]
        [DefaultValue(0)]
        public long KeyID { get { return _keyID; } set { _keyID = value; } }

        /// <summary>
        /// 路由关键字
        /// </summary>
        [DataMember]
        [DefaultValue("")]
        public string RoutingKey { get { return _routingKey; } set { if (null != value) _routingKey = value; } }


        /// <summary>
        /// 子路由关键字
        /// </summary>
        [DataMember]
        [DefaultValue(0)]
        public long SubRoutingKey { get { return _subRoutingKey; } set { _subRoutingKey = value; } }


        /// <summary>
        /// 序列化格式 JSON 　BIN XML
        /// </summary>
        [DataMember]
        [DefaultValue("json")]
        public string Format { get { return _format; } set { if (null != value) _format = value.ToLower(); } }



        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember] 
        public DateTime CreateTime { get { return DateTime.Now; } set { } }

        /// <summary>
        /// 消息体：注意所有消息内容，内嵌内容都要标记为可序列化
        /// </summary>
        [DataMember]
        [DefaultValue("")]
        public object Body { get { return _body; } set { _body = value; } }

        public override string ToString()
        {
            return "Header::TansID:"+_transactionID+",SourceID:" + _sourceID + ",ServiceID:" + _serviceID + ",ScenarioID:" + _scenarioID + ",MethodID:" + _methodID +",MethodName:"+_methodName+ ",KeyID:" + _keyID +",UserToken:"+UserToken+ ",TargetID="+_targetID+",RoutingKey:" + _routingKey + ",SubRoutingKey:"+_subRoutingKey+ ",Format:" + _format + "," + _timeStamp + "," + _version +",Message;"+_message+ ";Body::" + (null == _body ? "请求Body为空" : _body.ToString());
        }

        public virtual string ToKeyString()
        {
            return "Header::TansID:" + _transactionID +  ",RoutingKey:" + _routingKey + ",SubRoutingKey:"+_subRoutingKey+ ",SourceID:" + _sourceID + ",ServiceID:" + _serviceID + ",ScenarioID:" + _scenarioID + ",MethodID:" + _methodID + ",KeyID:" + _keyID+ ",Format:" + _format+",UserToken:"+UserToken+ ",TargetID="+_targetID+",RoutingKey:" + _routingKey+",Message;"+_message ;
        }

    } 

}
 