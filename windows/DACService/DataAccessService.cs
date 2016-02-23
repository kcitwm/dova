using System;
using System.IO;
using System.Diagnostics;
using System.ServiceModel;
using System.Configuration;
using System.ComponentModel;
using System.ServiceProcess;
using System.Runtime.Remoting;
using System.ServiceModel.Configuration;

using Dova.Utility;
using Newtonsoft.Json;
using Dova.Data;
using System.Text;
using System.Net;
using SocketAsyncServer;

namespace Dova.Services
{
    public partial class DataAccessService : ServiceBase
    {
        private IContainer components;
        public ServiceHost serviceHost;

        public DataAccessService()
        {
            this.InitializeComponent();
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.CurrentDomain_UnhandledException);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error("UnhandledException:" + e.ExceptionObject);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new Container();
            base.ServiceName = "DACService";
        }

        protected override void OnStart(string[] args)
        {
            if (args.Length > 0)
            {
                base.ServiceName = args[0];  
            }
            Log.Write("开始启动服务,ServerModle=" + Config.ServerModel.ToString());
            ChannelType model = (ChannelType)Config.ServerModel;  
            if ((model & ChannelType.Remoting) > 0)
                this.StartRemotingService();
            if ((model & ChannelType.WCF) > 0)
                this.StartWCFService();
            if ((model & ChannelType.AsyncTcp) > 0)
                this.StartAsyncTcpService(); 
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


        void Socket_DataReceived(object sender, ReceiveArgs e)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                Log.Info("Socket_DataReceived:开始触发接收事件处理数据的时间点:" + DateTime.Now.Ticks);
                DataHoldingUserToken token = e.Token;
                int len = token.lengthOfCurrentIncomingMessage;
                string userToken = string.Empty;
                int authoLen = 0;
                int headStart = 0;
                int headDataStart = headerLength;
                DACService dac = new DACService();
                int status = 1;
                if (Config.AuthoType == 1)
                {
                    Log.Info("Socket_DataReceived:开始认证:" + DateTime.Now.Ticks);

                    authoLen = BitConverter.ToInt32(token.theDataHolder.dataMessageReceived, 0);
                    userToken = Encoding.UTF8.GetString(token.theDataHolder.dataMessageReceived, headerLength, authoLen);
                    //检查是否登录 
                    status = dac.CheckLogin(userToken);

                    //结束检查登录
                    headStart = headerLength + authoLen;
                    headDataStart = headerLength + headerLength + authoLen;
                    Log.Info("Socket_DataReceived: 认证结束:" + DateTime.Now.Ticks);

                }
                int headDataLen = BitConverter.ToInt32(token.theDataHolder.dataMessageReceived, headStart);
                string methodName = Encoding.ASCII.GetString(token.theDataHolder.dataMessageReceived, headDataStart, headDataLen);
                if (status != 1 && methodName != "Login" && methodName != "Regist")
                {
                    e.DataToSend = encode.GetBytes("-99");
                    return;
                }
                int dataStart = headDataStart + headDataLen;
                len = len - dataStart;
                Log.Info("Socket_DataReceived:解析处理收到数据执行方法:" + methodName + " 时间点:" + DateTime.Now.Ticks);
                int fileNameLen = 0;
                string fileName = string.Empty;
                string data = "";
                switch (methodName)
                {
                    case "ExecutePagedDataList":
                        e.DataToSend = encode.GetBytes(dac.ExecutePagedDataList(JsonConvert.DeserializeObject<PagedRecordParameter>(encode.GetString(token.theDataHolder.dataMessageReceived, dataStart, len))));
                        break;
                    case "ExecuteDataList":
                        data = encode.GetString(token.theDataHolder.dataMessageReceived, dataStart, len);
                        Log.Write("收到数据:" + data);
                        e.DataToSend = encode.GetBytes(dac.ExecuteDataList(JsonConvert.DeserializeObject<WrapedDatabaseParameter>(data)));
                        break;
                    case "ExecuteNonQuery":
                        e.DataToSend = BitConverter.GetBytes(dac.ExecuteNonQuery(JsonConvert.DeserializeObject<WrapedDatabaseParameter>(encode.GetString(token.theDataHolder.dataMessageReceived, dataStart, len))));
                        break;
                    case "ExecuteScalar":
                        e.DataToSend = encode.GetBytes(dac.ExecuteScalar(JsonConvert.DeserializeObject<WrapedDatabaseParameter>(encode.GetString(token.theDataHolder.dataMessageReceived, dataStart, len))).ToString());
                        break;
                    case "ExecuteDataTable":
                        e.DataToSend = encode.GetBytes(dac.ExecuteDataTable(JsonConvert.DeserializeObject<WrapedDatabaseParameter>(encode.GetString(token.theDataHolder.dataMessageReceived, dataStart, len))));
                        break;
                    case "ExecuteDataSet":
                        e.DataToSend = encode.GetBytes(dac.ExecuteDataSet(JsonConvert.DeserializeObject<WrapedDatabaseParameter>(encode.GetString(token.theDataHolder.dataMessageReceived, dataStart, len))).GetXml());
                        break;
                    case "ExecutePagedDataSet":
                        e.DataToSend = encode.GetBytes(dac.ExecutePagedDataSet(JsonConvert.DeserializeObject<PagedRecordParameter>(encode.GetString(token.theDataHolder.dataMessageReceived, dataStart, len))).GetXml());
                        break;
                    case "Login":
                        e.DataToSend = encode.GetBytes(dac.Login(JsonConvert.DeserializeObject<LoginReq>(encode.GetString(token.theDataHolder.dataMessageReceived, dataStart, len))));
                        break;
                    case "Logout":
                        e.DataToSend = BitConverter.GetBytes(dac.Logout(encode.GetString(token.theDataHolder.dataMessageReceived, dataStart, len)));
                        break;
                    case "CheckLogin":
                        e.DataToSend = BitConverter.GetBytes(dac.CheckLogin(encode.GetString(token.theDataHolder.dataMessageReceived, dataStart, len)));
                        break;
                    case "Regist":
                        e.DataToSend = BitConverter.GetBytes(dac.Regist(JsonConvert.DeserializeObject<LoginReq>(encode.GetString(token.theDataHolder.dataMessageReceived, dataStart, len))));
                        break;
                    case "GetFile":
                        fileNameLen = BitConverter.ToInt32(token.theDataHolder.dataMessageReceived, dataStart);
                        fileName = Encoding.UTF8.GetString(token.theDataHolder.dataMessageReceived, headerLength + dataStart, fileNameLen);
                        e.DataToSend = dac.GetFile(fileName);
                        break;
                    case "SaveFile":
                        fileNameLen = BitConverter.ToInt32(token.theDataHolder.dataMessageReceived, dataStart);
                        fileName = Encoding.UTF8.GetString(token.theDataHolder.dataMessageReceived, headerLength + dataStart, fileNameLen);
                        int fileStart = dataStart + headerLength + fileNameLen;
                        len = len - headerLength - fileNameLen;
                        byte[] fileData = new byte[len];
                        Log.Write("收到文件长度:" + fileData.Length);
                        Buffer.BlockCopy(token.theDataHolder.dataMessageReceived, fileStart, fileData, 0, len);
                        e.DataToSend = encode.GetBytes(dac.SaveFile(fileName, fileData));

                        break;
                }
                Log.Write(LogAction.Info, className, "Socket_DataReceived", "end", "end", DateTime.Now.Ticks - t, "Socket_DataReceived:接口方法:" + methodName + "处理完成");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

        }
         

        //PersistentSocketAsyncServer.SocketListener presistentConnectionServer;


        static Encoding encode = Configs.Charset;
        const int headerLength = 4;
        static string className = "DataAccessService";
         


         


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

        private void StartWCFService()
        {
            try
            {
            Log.Write("开始启动WCF服务:");
            if (this.serviceHost != null)
            {
                this.serviceHost.Close();
            }
            ServicesSection section = ConfigurationManager.GetSection("system.serviceModel/services") as ServicesSection;
            string typeName = section.Services[0].Name + ",Dova.DataAccess";
            Log.Write("WCF服务启动:typeName:" + typeName);
            this.serviceHost = new ServiceHost(Type.GetType(typeName, true, false), new Uri[0]);
            this.serviceHost.Open();
            Log.Write("WCF服务启动成功:"); 
            }
            catch (Exception e)
            {
                Log.Error("WCF服务启动失败：" + e.Message);

            }
        }

    }
}
