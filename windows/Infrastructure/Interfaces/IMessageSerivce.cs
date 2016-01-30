using System;
using System.ServiceModel;
using System.Net.Security;
using System.ServiceModel.Web;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;

namespace Dova.Interfaces
{
    [ServiceContract(ProtectionLevel = ProtectionLevel.None)] 
    public interface IMessageService
    {

        /// <summary>
        /// 同步发送数据
        /// </summary>
        /// <param name="topicID">主题ID</param>
        /// <param name="obj">发送的数据</param>
        /// <param name="keyID">业务主键</param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/Request/{serviceName}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        object Request(string serviceName, object msg);

        /// <summary>
        /// 同步根据主题发送自定义消息
        /// </summary>
        /// <param name="topicID"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        [OperationContract]
        //Rest 不支持返回object 对象
       [WebInvoke(Method = "POST", UriTemplate = "/{serviceName}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        object RequestMessage(string serviceName, WQMessage msg);

        /// <summary>
        /// 同步发送数据
        /// </summary>
        /// <param name="topicID">主题ID</param>
        /// <param name="obj">发送的数据</param>
        /// <param name="keyID">业务主键</param>
        /// <returns></returns>
        [OperationContract]
      [WebInvoke(Method = "POST", UriTemplate = "/AsyncRequest/{serviceName}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        bool AsyncRequest(string serviceName, object msg);

        /// <summary>
        /// 同步根据主题发送自定义消息
        /// </summary>
        /// <param name="topicID"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/async/{serviceName}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        bool AsyncRequestMessage(string serviceName, WQMessage msg);
        
        /// <summary>
        /// 同步根据主题发送自定义消息
        /// </summary>
        /// <param name="topicID"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/asyncs/{serviceName}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        bool AsyncRequestMessages(string serviceName, WQMessage[] msg);
             


        /// <summary>
        /// 同步根据主题发送自定义消息
        /// </summary>
        /// <param name="topicID"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        [OperationContract(Name = "RequestMessage2")]
        [WebInvoke(Method = "POST", UriTemplate = "/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        DovaResponse RequestMessage(WQMessage msg);


        /// <summary>
        /// 同步根据主题发送自定义消息
        /// </summary>
        /// <param name="topicID"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        [OperationContract(Name = "SendMessage")]
        [WebInvoke(Method = "POST", UriTemplate = "/send", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        Object Send(WQMessage msg);

        [OperationContract(Name = "SendOneWayMessage")]
        [WebInvoke(Method = "POST", UriTemplate = "/send", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        void SendOneWay(WQMessage msg);

        [OperationContract(Name = "ReceiveMessage")]
        [WebInvoke(Method = "POST", UriTemplate = "/receive", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        object Receive(string format);
         
        /// <summary>
        /// 同步根据主题发送自定义消息
        /// </summary>
        /// <param name="topicID"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        [OperationContract(Name = "AsyncRequestMessage2")]
        [WebInvoke(Method = "POST", UriTemplate = "/async", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        bool AsyncRequestMessage(WQMessage msg);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [OperationContract(Name = "RequestString")]
       // [WebInvoke(Method = "POST", UriTemplate = "/RequestString", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        string Request(string msg);


        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/Test", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        DovaResponse Test(WQMessage msg);
        
    }
}
