using Dova.Interfaces;
using Dova.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dova.MessageQueue
{
    public interface IMessageHandler
    {
        /// <summary>
        /// 异步模式:处理多条统一格式消息
        /// </summary>
        /// <param name="msgs">消息集合</param>
        /// <returns>处理成功的条数</returns>
        int ExecuteMQ(string serviceName, List<WQMessage> msgs);

        /// <summary>
        /// 异步模式:单条处理统一格式消息
        /// </summary>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        bool ExecuteMQ(string serviceName, WQMessage msg);

        /// <summary>
        /// 异步模式:单条处理统一格式消息
        /// </summary>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        bool ExecuteObjectMQ(string serviceName, object msg);

        /// <summary>
        /// 同步模式:处理普通消息格式
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        object ExecuteRequest(string serviceName, object msg);


        string ExecuteRequest(string   msg); 

        /// <summary>
        /// 同步模式:处理统一消息模式
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        object DoExecuteRequest(string serviceName, object msg, bool async);


        /// <summary>
        /// 同步模式:处理统一消息模式
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        string DoExecuteRequest(string msg, bool async);
         

        /// <summary>
        /// 同步模式:处理统一消息模式
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        object ExecuteMessageRequest(string serviceName, WQMessage msg);

        ///// <summary>
        ///// 同步模式:处理统一消息模式
        ///// </summary>
        ///// <param name="msg"></param>
        ///// <returns></returns>
        //object ExecuteMessageRequest<T>(string serviceName, DovaMessage<T> msg);



        /// <summary>
        /// 同步模式:处理统一消息模式
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        object DoExecuteMessageRequest(string serviceName, WQMessage msg, bool async);

        ///// <summary>
        ///// 同步模式:处理统一消息模式
        ///// </summary>
        ///// <param name="msg"></param>
        ///// <returns></returns>
        //object DoExecuteMessageRequest<T>(string serviceName, DovaMessage<T> msg);


    }
}
