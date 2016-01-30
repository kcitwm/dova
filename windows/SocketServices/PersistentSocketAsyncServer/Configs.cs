using Dova;
using System;
using System.Net;
using System.Text;

namespace PersistentSocketAsyncServer
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
        public static string PersistAddress = "Any";
        /// <summary>
        /// 侦听端口
        /// </summary>
        public static int PersistPort = 19000;

        public static IPEndPoint MessageIPE = new IPEndPoint(IPAddress.Loopback, PersistPort);
        
        public static int MaxNumberOfConnections = 20000;
        public static int ExcessSaeaObjectsInPool = 20000;
        public static int Backlog = 1000;
        public static int MaxSimultaneousAcceptOps = 20000;
        public static int ReceivePrefixLength = 4;
        public static int ReceiveBufferSize = 1024;
        public static int sendPrefixLength = 4;
        public static int opsToPreAlloc = 2; 
        public static Encoding Charset = Encoding.UTF8;

        public static bool PersistentConnect = true;

        static Configs()
        {
            PersistPort = Config.Get("Port", PersistPort);
            PersistAddress = Config.Get("Any", PersistAddress);
            IPAddress ipd;
            if (PersistAddress.ToUpper() == "ANY")
                ipd = IPAddress.Any;
            else
                ipd = IPAddress.Parse(PersistAddress);
            MessageIPE = new IPEndPoint(ipd, PersistPort);
            MaxNumberOfConnections = Config.Get("MaxNumberOfConnections", MaxNumberOfConnections);
            ExcessSaeaObjectsInPool = Config.Get("ExcessSaeaObjectsInPool", ExcessSaeaObjectsInPool);
            Backlog = Config.Get("Backlog", Backlog);
            MaxSimultaneousAcceptOps = Config.Get("MaxSimultaneousAcceptOps", MaxSimultaneousAcceptOps);
            ReceivePrefixLength = Config.Get("ReceivePrefixLength", ReceivePrefixLength);
            ReceiveBufferSize = Config.Get("TestBufferSize", ReceiveBufferSize);
            sendPrefixLength = Config.Get("sendPrefixLength", sendPrefixLength);
            opsToPreAlloc = Config.Get("opsToPreAlloc", opsToPreAlloc);
            Charset = Encoding.GetEncoding(Config.Get("Charset", "UTF-8"));
        }



    }
}
