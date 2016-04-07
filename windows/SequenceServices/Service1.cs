using Dova.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.ServiceProcess;
using System.Text;

namespace SequenceServices
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
            Log.Error("SequenceServices.UnhandledException:" + e.ExceptionObject);
        }

        protected override void OnStart(string[] args)
        {
            StartWCFService();
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
                string typeName = section.Services[0].Name + ",Dova.Sequences";
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
    }
}
