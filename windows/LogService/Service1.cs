using Dova;
using Dova.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.ServiceProcess;
using System.Text;
using System.Threading;

namespace LogService
{
    public partial class Service1 : ServiceBase
    {
        public ServiceHost serviceHost;

        public Service1()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.CurrentDomain_UnhandledException);
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Error("UnhandledException:" + e.ExceptionObject);
        }

        protected override void OnStart(string[] args)
        {
            Log.Write("开始启动服务,ServerModle=" + Config.ServerModel.ToString());
            switch (((ChannelType)Config.ServerModel))
            {
                case ChannelType.Remoting:
                    this.StartRemotingService();
                    break;

                case ChannelType.WCF:
                    this.StartWCFService();
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
                RemoteLogService.FlushLog(true);
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
            Log.Write("开始启动WCF服务:");
            if (this.serviceHost != null)
            {
                this.serviceHost.Close();
            }
            ServicesSection section = ConfigurationManager.GetSection("system.serviceModel/services") as ServicesSection;
            string typeName = section.Services[0].Name + ",Dova.LogLib";
            Log.Write("typeName:" + typeName);
            this.serviceHost = new ServiceHost(Type.GetType(typeName, true, false), new Uri[0]);
            this.serviceHost.Open();
            Log.Write("WCF服务启动成功:");
        }

    }
}
