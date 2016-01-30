using Dova;
using System;
using System.Text;

namespace SocketAsyncServer
{
    /// <summary>
    /// 配置
    /// carlchen
    /// </summary>
    public class Configs : Config
    {
        public static long MainTransMissionId = 0;
        public static int maxSimultaneousClientsThatWereConnected = 0;
        public static int MainSessionId = 0;
        public static int MSDelayAfterGettingMessage = 0;
        /// <summary>
        /// 侦听IP地址
        /// </summary>
        public static string Address = "Any";
        /// <summary>
        /// 侦听端口
        /// </summary>
        public static int Port = 18000;
        public static int MaxNumberOfConnections = 10000;
        public static int ExcessSaeaObjectsInPool = 10000;
        public static int Backlog = 10000;
        public static int MaxSimultaneousAcceptOps = 10;
        public static int ReceivePrefixLength = 4;
        public static int ReceiveBufferSize = 256;//每次数据的大小.
        public static int sendPrefixLength = 4;
        public static int opsToPreAlloc = 2; 
        public static Encoding Charset = Encoding.UTF8;

        static Configs()
        {
            Port = Config.Get("Port", Port);
            Address = Config.Get("Any", Address);
            MaxNumberOfConnections = Config.Get("MaxNumberOfConnections", MaxNumberOfConnections);
            ExcessSaeaObjectsInPool = Config.Get("ExcessSaeaObjectsInPool", ExcessSaeaObjectsInPool);
            Backlog = Config.Get("Backlog", Backlog);
            MaxSimultaneousAcceptOps = Config.Get("MaxSimultaneousAcceptOps", MaxSimultaneousAcceptOps);
            ReceivePrefixLength = Config.Get("ReceivePrefixLength", ReceivePrefixLength);
            ReceiveBufferSize = Config.Get("ReceiveBufferSize", ReceiveBufferSize);
            sendPrefixLength = Config.Get("SendPrefixLength", sendPrefixLength);
            opsToPreAlloc = Config.Get("opsToPreAlloc", opsToPreAlloc);
            Charset = Encoding.GetEncoding(Config.Get("Charset", "UTF-8"));
        }



    }
}
