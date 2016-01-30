using Dova;
using Dova.Utility;
using SocketAsyncServer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks; 

namespace CacheService
{
    public partial class Service1 : ServiceBase
    {  
        public ServiceHost serviceHost;

        public Service1()
        {
            InitializeComponent(); 
            base.ServiceName = "CacheService";
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.CurrentDomain_UnhandledException);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error("UnhandledException:" + e.ExceptionObject);
        }
         

        protected override void OnStart(string[] args)
        {
            Log.Write("开始启动WCF服务:");
            Log.Write("开始启动服务,ServerModle=" + Config.ServerModel.ToString());
            switch (((ChannelType)Config.ServerModel))
            {
                case ChannelType.Remoting:
                    this.StartRemotingService();
                    break;

                case ChannelType.WCF:
                    this.StartWCFService();
                    break;

                case ChannelType.AsyncTcp:
                    this.StartAsyncTcpService();
                    break;

                case (ChannelType.WCF | ChannelType.Remoting):
                    this.StartRemotingService();
                    this.StartWCFService();
                    break;
            }
            Log.Write("启动服务结束,ServerModle=" + Config.ServerModel.ToString());
        }

        protected override void OnStop()
        {
            try
            {
                if (this.serviceHost != null)
                {
                    this.serviceHost.Abort();
                    this.serviceHost.Close();
                    this.serviceHost = null;
                }
            }
            catch
            {
            }
        }

        private void StartAsyncTcpService()
        {
            Log.Write("开始启动异步TCP服务:");
            IPAddress ipd = IPAddress.Any;
            if (Configs.Address.ToUpper() != "ANY")
                ipd = IPAddress.Parse(Configs.Address);
            IPEndPoint localEndPoint = new IPEndPoint(ipd, Configs.Port);
            //This object holds a lot of settings that we pass from Main method
            //to the SocketListener. In a real app, you might want to read
            //these settings from a database or windows registry settings that
            //you would create.
            SocketListenerSettings theSocketListenerSettings = new SocketListenerSettings
            (Configs.MaxNumberOfConnections, Configs.ExcessSaeaObjectsInPool, Configs.Backlog, Configs.MaxSimultaneousAcceptOps
            , Configs.ReceivePrefixLength, Configs.ReceiveBufferSize, Configs.sendPrefixLength, Configs.opsToPreAlloc, localEndPoint);

            //instantiate the SocketListener.
            SocketListener sl = new SocketListener(theSocketListenerSettings);
            sl.DataReceived += Socket_DataReceived;
            Log.Write("异步TCP服务启动成功:");
        }
        static Encoding encode = Configs.Charset;
        const int headerLength = 4;
        void Socket_DataReceived(object sender, ReceiveArgs e)
        {
            try
            {

                DataHoldingUserToken token = e.Token;
                int len = token.theDataHolder.dataMessageReceived.Length;
                int headDataLen = BitConverter.ToInt32(token.theDataHolder.dataMessageReceived, 0);
                string methodName = Encoding.ASCII.GetString(token.theDataHolder.dataMessageReceived, headerLength, headDataLen);
                int dataStart = headerLength + headDataLen; ;
                len = len - dataStart;
                Log.Info("收到缓存请求:" + methodName);
                　
                switch (methodName)
                {
                    case "Get":
                           break;
                    case "Set":
                           break; 
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

        }

        private void StartRemotingService()
        {
            string filename = Config.Get("ConfigPath", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Process.GetCurrentProcess().MainModule.FileName + ".config"));
            Log.Write("开始启动Remoting服务:"); 
            RemotingConfiguration.Configure(filename, false);
            Log.Write("Remoting服务启动成功:");
        }

        private void StartWCFService()
        {
            Log.Write("开始启动WCF服务:");
            if (this.serviceHost != null)
            {
                this.serviceHost.Close();
            }
            ServicesSection section = ConfigurationManager.GetSection("system.serviceModel/services") as ServicesSection;
            string typeName = GetTypeName();
            Log.Write("WCF服务启动:typeName:" + typeName);
            this.serviceHost = new ServiceHost(Type.GetType(typeName, true, false), new Uri[0]);
            this.serviceHost.Open();
            Log.Write("WCF服务启动成功:");
        }

        private string GetTypeName()
        {
            int ct=Config.CacheType;
            string asb = "Dova.Cache";
            string t = "Dova.Cache.MemCacheService";
            switch (ct)
            {
                case (int)CacheType.InMemory:
                    t= "Dova.Cache.MemCacheService";
                    break;
            }
            return t + "," + asb;
        }

    }
}
