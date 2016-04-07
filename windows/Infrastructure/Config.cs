namespace Dova
{
    using System;
    using System.Configuration;
    using System.Collections.Specialized;

    using Dova.Utility;
    using System.Net;
    using System.Management;
    using System.Net.Sockets;
    using System.Reflection;
    using System.IO;
    using System.Diagnostics;
    using System.Web;
using System.Text;

    /// <summary>
    /// 所有配置都可以在appSettings里面设置与属性名相同的键.
    /// </summary>
    public class Config
    {


        public static string AppName = Process.GetCurrentProcess().MainModule.ModuleName;
        public static string ProcessName = Process.GetCurrentProcess().ProcessName;

        /// <summary>
        /// {0} 应用名｛1｝　类名　｛2｝　方法名　｛3｝　执行时间　｛4｝关键参数值和名
        /// </summary>
        public static string LogFormat = "{0}\t{1}\t{2}\t{3}\t{4}";
        public static string LogFormat2 = "{0}\t{1}\t{2}\t{3}\t{4}\t{5}";
        public static string LogFormat3 = "{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}";

        public static string[] HostIPMAC = null;

        /// <summary>
        /// 获取设置默认根目录,默认值:AppDomain.CurrentDomain.BaseDirectory
        /// 可在appSettings设置"BasePath"
        /// </summary>        
        public static string BasePath = AppDomain.CurrentDomain.BaseDirectory;
        public static int ServerModel = 8;
        public static int CacheType = 1;
        public static string CacheConfigName = "CacheConfig";

        public static string AppDomainPath = "";
        public static string LoginCacheServiceName = "LoginCacheService";

        public static Encoding Encode = Encoding.UTF8;

        //public static string DefaultConnectionString = "DefaultConnection";
        //public static string DefaultProviderName = "System.Data.SqlClient";

        //public static string LogConnectionString = "LogConnection";
        //public static string LogProviderName = "System.Data.SqlClient";


        //public static string DACConnectionString = "LogConnection";
        //public static string DACProviderName = "System.Data.SqlClient";

        //public static string SequenceConnectionString = "SequenceConnection";
        //public static string SequenceProviderName = "System.Data.SqlClient";


        public static string DefaultConnectionName = "DefaultConnection";
        public static string LoginConnectionName = "LoginConnection";
        public static string LogConnectionName = "LogConnection";
        public static string DACConnectionName = "DACConnection";
        public static string SequenceConnectionName = "SequenceConnection";


        public static string FileConnectionName = "DefaultConnection";



        //public static ConnectionStringSettings DefaultConnection = null;
        //public static ConnectionStringSettings DACConnection = null;
        //public static ConnectionStringSettings LogConnection = null;
        //public static ConnectionStringSettings SequenceConnection = null;

        public static int DefaultLocalWRType = (int)RemoteWRType.Local;

        public static string ServiceConfigPath = BasePath + "Config/Dova.Services.config";

	    public static string logPrexs = "";

        public static int AuthoType = 1;// 0 不需要登录验证  1 需要登录验证.

        public static int IsEndPoint = 1;

        static string threadId = string.Empty;
        public static  string ThreadId
        {
            get
            {
                try
                {
                    if (string.IsNullOrEmpty(threadId))
                    {
                        string path=Path.Combine(Config.BasePath, "ThreadInfo.txt");
                        using (StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8))
                        {
                            string s = Guid.NewGuid().ToString("N");
                            sw.WriteLine(s);
                            sw.Close();
                            threadId = s;
                        }
                        
                    }
                }
                catch (Exception e)
                {
                }
                return threadId;

            }
        }



        public static string Get(string key, string defaultValue)
        {
            try
            {
                NameValueCollection appSettings = ConfigurationManager.AppSettings;
                if (null != appSettings && null != appSettings[key])
                    return appSettings[key].Trim();
                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        public static int Get(string key, int defaultValue)
        {
            try
            {
                NameValueCollection appSettings = ConfigurationManager.AppSettings;
                if (null != appSettings && null != appSettings[key])
                    return appSettings[key].Trim().ToInt32();
                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }


        public static long Get(string key, long defaultValue)
        {
            try
            {
                NameValueCollection appSettings = ConfigurationManager.AppSettings;
                if (null != appSettings && null != appSettings[key])
                    return appSettings[key].Trim().ToInt64();
                return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        public static ConnectionStringSettings GetConnection(string key)
        {
            ConnectionStringSettingsCollection css = ConfigurationManager.ConnectionStrings;
            if (null != css[key])
                return css[key];
            return null;
        }

        public void SaveConfig(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            int appStgCnt = ConfigurationManager.AppSettings.Count;
            KeyValueConfigurationElement kve = config.AppSettings.Settings[key];
            if (null != kve)
            {
                kve.Value = value;
                config.Save(ConfigurationSaveMode.Modified);
            }
            ConfigurationManager.RefreshSection("appSettings");
            Init();
        }


        public static string SequenceServiceName = "";
        public static int SequenceServiceType = 1;

        protected virtual void Init()
        {
            AuthoType = Config.Get("AuthoType", AuthoType);
            HostIPMAC = GetIPHostNameAndMAC();
            AppDomainPath = AppDomain.CurrentDomain.BaseDirectory;
            BasePath = Config.Get("BasePath", BasePath);
            string defaultAppName = AppName;
            AppName = Config.Get("AppName", AppName);
            ProcessName = GetProcessName(true);
            if (null != HttpContext.Current && AppName == defaultAppName)
                AppName = System.Web.Hosting.HostingEnvironment.ApplicationHost.GetSiteName();
            ServiceConfigPath = Config.Get("ServiceConfigPath", ServiceConfigPath);
            ServerModel = Config.Get("ServerModel", ServerModel);
            CacheType = Config.Get("CacheType", CacheType.ToString()).ToInt32();
            DefaultLocalWRType = Config.Get("DefaultLocalWRType", DefaultLocalWRType);
            SequenceServiceName = Config.Get("SequenceServiceName", SequenceServiceName);
            SequenceServiceType = Config.Get("SequenceServiceType", SequenceServiceType);   
            CacheConfigName = Config.Get("CacheConfigName", CacheConfigName);
            logPrexs = HostIPMAC[0] + "\t" + HostIPMAC[1] + "\t" + HostIPMAC[2] + "\t" + AppDomainPath + "\t";
            IsEndPoint = Config.Get("IsEndPoint", IsEndPoint);
            LogFormat = logPrexs + AppName + "\t" + ProcessName+"\t"+ LogFormat;
			LogFormat2 = logPrexs + AppName + "\t" + ProcessName + "\t"  + LogFormat2;
			LogFormat3 = logPrexs + AppName + "\t" + ProcessName + "\t" + LogFormat3;
        }

        static Config()
        {
            try
            {
                new Config().Init();
            }
            catch (Exception e)
            {
                Log.Write("static Config():" + e.Message);
            }
        }

        /// <summary>
        /// 获取主机名 IP地址 MAC地址
        /// </summary>
        /// <returns></returns>
        public static string[] GetIPHostNameAndMAC()
        {
            string[] infos = new string[3];
            try
            {
                string hostname = Dns.GetHostName();//得到本机名     
                IPHostEntry localhost = Dns.GetHostEntry(hostname);
                IPAddress[] ips = localhost.AddressList;
                string ipv4 = "";
                foreach (IPAddress ip in ips)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ipv4 = ip.ToString(); break;
                    }
                }
                string mac = GetMAC();
                infos[0] = hostname;
                infos[1] = ipv4;
                infos[2] = mac;
            }
            catch { }
            return infos;
        }


        static string GetMAC()
        {
            string mac = "";
            ManagementClass managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection managementObjectCollection = managementClass.GetInstances();
            foreach (ManagementObject managementObject in managementObjectCollection)
            {
                if ((bool)managementObject["IPEnabled"] == true)
                {
                    mac += managementObject["MACAddress"].ToString();
                }
            }
            return mac;
        }


        static string GetProcessName(bool changeShit3)
        {
            string name = "";
            try
            {
                Process p = Process.GetCurrentProcess();
                name = p.ProcessName;
                if (changeShit3 && name.IndexOf("#") > -1) return name;
                int pid = p.Id;
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT DisplayName FROM Win32_Service WHERE ProcessId = " + pid);
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    name = queryObj["DisplayName"].ToString();
                    break;
                }
            }
            catch { }
            return name;
        }

    }
}

