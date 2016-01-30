using Dova.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Dova
{
    public class LogConfig
    {
        [XmlElement]
        public string InnerLogPath = (AppDomain.CurrentDomain.BaseDirectory + @"Logs\");
        [XmlElement]
        public string InnerBakPath = "";
        [XmlElement]
        public string InnerLogName = "log.txt";
        [XmlElement]
        public double InnerBakLength = 10;
        [XmlElement]
        public int InnerTimedLog = 1;
        [XmlElement]
        public int InnerLogType = -1;// (int)(LogAction.Error|LogAction.Info|LogAction.Warn);
        [XmlElement]
        public int InnerRefreshTime = 1;
        [XmlElement]
        public double InnerClearSpan = 96;
        [XmlElement]
        public int InnerSecurityMemNumber = 100000;
        [XmlElement]
        public bool InnerIsMutex = false;
        [XmlElement]
        public int InnerFlushNumber = 100;
        [XmlElement]
        public int InnerFlushSpan = 1;
        [XmlElement]
        public int InnerLogMate = 1;
        [XmlElement]
        public int InnerBakMinutes = 5;
        [XmlElement]
        public int InnerClearMinutes = 10;
        [XmlElement]
        public bool InnerStoreDB = false;

        [XmlElement]
        public int InnerLogCagDep = 0;

        [XmlElement]
        public bool InnerLogGC = false;


        public static string LogPath
        {
            get { return ServiceConfigs.LogSetting.InnerLogPath; }
            set { 
                        ServiceConfigs.LogSetting.InnerLogPath = value; 
            }
        }
        public static string LogName { get { return ServiceConfigs.LogSetting.InnerLogName; } set { ServiceConfigs.LogSetting.InnerLogName = value; } }
        public static double BakLength { get { return ServiceConfigs.LogSetting.InnerBakLength * 1024 * 1024; } set { ServiceConfigs.LogSetting.InnerBakLength = value; } }
        public static int TimedLog { get { return ServiceConfigs.LogSetting.InnerTimedLog; } set { ServiceConfigs.LogSetting.InnerTimedLog = value; } }
        public static int LogType { get { return ServiceConfigs.LogSetting.InnerLogType; } set { ServiceConfigs.LogSetting.InnerLogType = value; } }
        public static int RefreshTime { get {  
            return ServiceConfigs.LogSetting.InnerRefreshTime; 
        } set { ServiceConfigs.LogSetting.InnerRefreshTime = value; } }
        public static double ClearSpan { get { return ServiceConfigs.LogSetting.InnerClearSpan; } set { ServiceConfigs.LogSetting.InnerClearSpan = value; } }
        public static int SecurityMemNumber { get { return ServiceConfigs.LogSetting.InnerSecurityMemNumber; } set { ServiceConfigs.LogSetting.InnerSecurityMemNumber = value; } }
        public static bool IsMutex { get { return ServiceConfigs.LogSetting.InnerIsMutex; } set { ServiceConfigs.LogSetting.InnerIsMutex = value; } }
        public static int FlushNumber { get { return ServiceConfigs.LogSetting.InnerFlushNumber; } set { ServiceConfigs.LogSetting.InnerFlushNumber = value; } }
        public static int FlushSpan { get { return ServiceConfigs.LogSetting.InnerFlushSpan; } set { ServiceConfigs.LogSetting.InnerFlushSpan = value; } }
        public static int LogMate { get { return ServiceConfigs.LogSetting.InnerLogMate; } set { ServiceConfigs.LogSetting.InnerLogMate = value; } }
        public static int BakMinutes { get { return ServiceConfigs.LogSetting.InnerBakMinutes; } set { ServiceConfigs.LogSetting.InnerBakMinutes = value; } }
        public static int ClearMinutes { get { return ServiceConfigs.LogSetting.InnerClearMinutes; } set { ServiceConfigs.LogSetting.InnerClearMinutes = value; } }
        public static bool StoreDB { get { return ServiceConfigs.LogSetting.InnerStoreDB; } set { ServiceConfigs.LogSetting.InnerStoreDB = value; } }
        public static string BakPath
        {
            get { return ServiceConfigs.LogSetting.InnerBakPath == "" ? ServiceConfigs.LogSetting.InnerLogPath : ServiceConfigs.LogSetting.InnerBakPath; }
            set
            { 
                        ServiceConfigs.LogSetting.InnerBakPath = value; 
            }
        }


        public static int LogCagDep { get { return ServiceConfigs.LogSetting.InnerLogCagDep; } set { ServiceConfigs.LogSetting.InnerLogCagDep = value; } }
        public static bool LogGC { get { return ServiceConfigs.LogSetting.InnerLogGC; } set { ServiceConfigs.LogSetting.InnerLogGC = value; } }


    }
}
