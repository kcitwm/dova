using System;
using System.Runtime.Serialization;

namespace Dova
{
    [Serializable]
    [DataContract]
    public class DovaResponse<T>
    {


        long _transactionID = 0;
        int _status = 0;
        string _message = "";
        long _keyID = 0;  
        T _body ;
         

        /// <summary>
        /// 事务ID: 根据业务定义场景事务,独立系统事务,全局跨系统事务, 可用SequenceID
        /// </summary>
        [DataMember]
        public long TransactionID
        {
            get { return _transactionID; }
            set { _transactionID = value; }
        }

        /// <summary>
        /// 业务数据的主健ＩＤ
        /// </summary>
        [DataMember]
        public long KeyID { get { return _keyID; } set { _keyID = value; } }

        /// <summary>
        /// 业务状态编码
        /// </summary>
        [DataMember]
        public int Status { get { return _status; } set { if (null != value) _status = value; } }


        /// <summary>
        /// 业务处理消息　
        /// </summary>
        [DataMember]
        public string Message { get { return _message; } set { if (null != value) _message = value; } }

        /// <summary>
        /// 消息体：注意所有消息内容，内嵌内容都要标记为可序列化
        /// </summary>
        [DataMember]
        public T Body { get { return _body; } set { _body = value; } }
       
        public override string ToString()
        {
            return "Header:TransactionID:" + _transactionID + ",KeyID:" + _keyID + ",Status:" + _status + ",Message:" + _message+";Body:"+_body.ToString();
        }

    }


}
