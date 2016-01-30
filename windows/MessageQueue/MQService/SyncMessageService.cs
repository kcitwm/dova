using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.ServiceModel;
using System.Configuration;
using System.ServiceProcess;
using System.Runtime.Remoting;
using System.ServiceModel.Configuration;

using Dova;
using Dova.Utility;
using Dova.MessageQueue;
using System.ServiceModel.Web;
using System.ServiceModel.Activation;
using Dova.Interfaces;
using System.Net;
using System.Text;
using Dova.Data; 
using SocketAsyncServer;

namespace Dova.MQService
{
    public partial class SyncMessageService : ServiceBase
    {
        public ServiceHost serviceHost;
        // public WebServiceHost serviceHost2;


       

        public SyncMessageService()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.CurrentDomain_UnhandledException);
            try
            {
                MessageHandler.StartRetryProcess();
            }
            catch { }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error("UnhandledException:" + e.ExceptionObject);
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                Log.Write("开始启动服务,ServerModle=" + Config.ServerModel.ToString());
                switch (((ChannelType)Config.ServerModel))
                {
                    case ChannelType.AsyncTcp:
                        this.StartAsyncTcpService();
                        break;
                    case ChannelType.PersistAsyncTcp:
                        this.PersistStartAsyncTcpService();
                        break;
                    case ChannelType.Remoting:
                        this.StartRemotingService();
                        break;
                    case (ChannelType.WCF):
                    case (ChannelType.WCFRest):
                        this.StartWCFService();
                        break;

                    case (ChannelType.WCF | ChannelType.Remoting):
                        this.StartRemotingService();
                        this.StartWCFService();
                        break;
                }
                Log.Write("启动服务结束,ServerModle=" + Config.ServerModel.ToString());
            }
            catch (Exception e)
            {
                Log.Error("服务出错,ServerModle=" + Config.ServerModel.ToString() + ";" + e.Message);
            }
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

        private void StartRemotingService()
        {
            string filename = "";
            Log.Write("开始启动Remoting服务:");
            if (ConfigurationManager.AppSettings["ConfigPath"] != null)
            {
                filename = ConfigurationManager.AppSettings["ConfigPath"];
            }
            else
            {
                filename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Process.GetCurrentProcess().MainModule.FileName + ".config");
            }
            RemotingConfiguration.Configure(filename, false);
            Log.Write("Remoting服务启动成功:");
        }


        private Thread wcfThread;

        private void StartWCFService()
        {
            wcfThread = new Thread(StartWCF);
            wcfThread.Start();
        }

        private void StartWCF()
        {
            int svcType = Config.ServerModel;
            ServicesSection section = ConfigurationManager.GetSection("system.serviceModel/services") as ServicesSection;
            string typeName = section.Services[0].Name + ",Dova.MQCore";
            Log.Write("开始启动WCF服务:typeName:" + typeName);
            //if ((svcType & (int)ChannelType.WCF) != 0)
            //{
            if (this.serviceHost != null)
            {
                this.serviceHost.Close();
            }
            this.serviceHost = new ServiceHost(Type.GetType(typeName, true, true), new Uri[0]);
            this.serviceHost.Open();
            Log.Write("WCF服务启动成功:");
            //}
            //if ((svcType & (int)ChannelType.WCFRest) != 0)
            //{
            //      if (this.serviceHost2 != null)
            //    {
            //        this.serviceHost2.Close();
            //    }
            //    this.serviceHost2 = new WebServiceHost(Type.GetType(typeName, true, true), new Uri[0]);
            //    this.serviceHost2.Open();
            //    Log.Write("WCFRest服务启动成功:");
            //}
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

        private void PersistStartAsyncTcpService()
        {
            try
            {
                Log.Write("开始启动异步长连接TCP服务:" + PersistentSocketAsyncServer.Configs.MessageIPE);
                //This object holds a lot of settings that we pass from Main method
                //to the SocketListener. In a real app, you might want to read
                //these settings from a database or windows registry settings that
                //you would create.
                
                PersistentSocketAsyncServer.SocketListenerSettings theSocketListenerSettings = new PersistentSocketAsyncServer.SocketListenerSettings
                (
                    PersistentSocketAsyncServer.Configs.MaxNumberOfConnections, //100000
                    PersistentSocketAsyncServer.Configs.ExcessSaeaObjectsInPool, //20000
                    PersistentSocketAsyncServer.Configs.Backlog,//1000
                    PersistentSocketAsyncServer.Configs.MaxSimultaneousAcceptOps,
                    PersistentSocketAsyncServer.Configs.ReceivePrefixLength,
                    PersistentSocketAsyncServer.Configs.ReceiveBufferSize,
                    PersistentSocketAsyncServer.Configs.sendPrefixLength,
                    PersistentSocketAsyncServer.Configs.opsToPreAlloc,
                    PersistentSocketAsyncServer.Configs.MessageIPE
                );

                //instantiate the SocketListener.
                presistentConnectionServer = new PersistentSocketAsyncServer.SocketListener(theSocketListenerSettings);
                presistentConnectionServer.DataReceived += Socket_DataReceived;
                Log.Write("异步长连接TCP服务启动成功:");
            }
            catch (Exception e)
            {
                Log.Error(e.StackTrace);
            }
        }

        PersistentSocketAsyncServer.SocketListener presistentConnectionServer;

        static Encoding encode = Configs.Charset;
        const int headerLength = 4;
        static Type type = typeof(WQMessage);
        void Socket_DataReceived(object sender, ReceiveArgs e)
        {
            try
            {
                DataHoldingUserToken token = e.Token;
                int len = token.lengthOfCurrentIncomingMessage;
                string userToken = string.Empty;
                WQMessage msg = SerializeHelper.DeSerializeToJSon<WQMessage>(encode.GetString(token.theDataHolder.dataMessageReceived)) ;
                int status = -1; 
                if (Config.AuthoType == 1)
                {
                    DACService dac = new DACService();
                    status = dac.CheckLogin(userToken);
                    if (status != 1)
                    {
                        token.dataToSend = encode.GetBytes("-99");
                        return;
                    }
                }
                object o = MessageHandler.ExecuteMessage(msg.ServiceName, msg, msg.Async);
                token.dataToSend = encode.GetBytes(SerializeHelper.SerializeToJSon(o));
                //using (MemoryStream ms = new MemoryStream())
                //{
                //    RuntimeTypeModel.Default.Serialize(ms, o);
                //    token.dataToSend=ms.GetBuffer(); 
                //} 
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        void Socket_DataReceived(object sender , PersistentSocketAsyncServer.DataHolder e)
        {
            try
            {  
                int status = -1;
                string msg = encode.GetString(e.dataMessageReceived);
                WQMessage message = SerializeHelper.DeSerializeToJSon<WQMessage>(msg);
                string userToken = message.UserToken;
                if (Config.AuthoType == 1)
                {
                    DACService dac = new DACService();
                    status = dac.CheckLogin(userToken);
                    if (status != 1)
                    {
                        e.dataMessageSend= encode.GetBytes("-99");
                        return;
                    }
                }
                e.userTokenId = userToken;
                Log.Info("添加客户端注册表:" + userToken);
                //原版本
                //presistentConnectionServer.AddToken(userToken, token.theMediator.GiveBack());
                //原版本
                //新版本
                // presistentConnectionServer.AddToken(userToken,e.GiveBackSend());  称到里面去
                //新版本
                object o = MessageHandler.ExecuteMessage(message.ServiceName, message, message.Async);
                if (null != o)
                    e.dataMessageSend = encode.GetBytes(o.ToString());
                //using (MemoryStream ms = new MemoryStream())
                //{
                //    RuntimeTypeModel.Default.Serialize(ms, o);
                //    token.dataToSend=ms.GetBuffer();
                //}

            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

        }

        //原版本
        //void Socket_DataReceived(object sender, PersistentSocketAsyncServer.ReceiveArgs e)
        //{
        //    try
        //    {
        //        PersistentSocketAsyncServer.DataHoldingUserToken token = e.Token;
        //        int len = token.lengthOfCurrentIncomingMessage;
        //        int status = -1;
        //        string msg = encode.GetString(token.theDataHolder.dataMessageReceived);
        //        WQMessage message = SerializeHelper.DeSerializeToJSon<WQMessage>(msg);
        //        string userToken = message.UserToken;
        //        if (Config.AuthoType == 1)
        //        {
        //            DACService dac = new DACService();
        //            status = dac.CheckLogin(userToken);
        //            if (status != 1)
        //            {
        //                token.dataToSend = encode.GetBytes("-99");
        //                return;
        //            }
        //        }
        //        token.userTokenId = userToken;
        //        Log.Info("添加客户端注册表:" + userToken);
        //        //原版本
        //        //presistentConnectionServer.AddToken(userToken, token.theMediator.GiveBack());
        //        //原版本
        //        //新版本
        //        presistentConnectionServer.AddToken(userToken, token.theMediator.GiveBackSend());
        //        //新版本
        //        object o = MessageHandler.ExecuteMessage(message.ServiceName, message, message.Async);
        //        if(null!=o)
        //        e.DataToSend = encode.GetBytes(o as string);
        //        //using (MemoryStream ms = new MemoryStream())
        //        //{
        //        //    RuntimeTypeModel.Default.Serialize(ms, o);
        //        //    token.dataToSend=ms.GetBuffer();
        //        //}

        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex);
        //    }

        //}

        //原版本


    }
}
