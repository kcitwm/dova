using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Dova
{
    [Serializable]
    public class MsgQConfig
    {
        /// <summary>
        /// 多少毫秒收取一次消息
        /// </summary> 
        public int InnerReceiveInteval = 100;
        public static int ReceiveInteval { get { return ServiceConfigs.MsgQSetting.InnerReceiveInteval; } set { ServiceConfigs.MsgQSetting.InnerReceiveInteval = value; } }
         
        public string InnerAppName = "MQService";
        public static string AppName { get { return ServiceConfigs.MsgQSetting.InnerAppName; } set { ServiceConfigs.MsgQSetting.InnerAppName = value; } }
         
        public int InnerMaxThreads = 50;
        public static int MaxThreads { get { return ServiceConfigs.MsgQSetting.InnerMaxThreads; } set { ServiceConfigs.MsgQSetting.InnerMaxThreads = value; } }
         
        public string   InnerRoutingServerAddressLeader  = "10.24.177.165:53101"; 
        public string   InnerRoutingServerAddressReplica = "10.24.177.165:53101"; 
        public string   InnerLogType                     = "None"; 
        public int      InnerHBTimeout                   = 10; 
        public int      InnerRoutUpdateTimeout           = 30; 
        public int      InnerSet                         = 0; 
        public string   InnerAddrSource                  = "ConfigCenter"; 
        public string   InnerMailSender                  = "deshengli@tencent.com"; 
        public string   InnerPhoneList                   = ""; 
        public string   InnerMailList                    = ""; 
        public string   InnerRtxList                     = "";


        public static string  RoutingServerAddressLeader { get { return ServiceConfigs.MsgQSetting.InnerRoutingServerAddressLeader; }   set { ServiceConfigs.MsgQSetting.InnerRoutingServerAddressLeader = value; } }
        public static string  RoutingServerAddressReplica{ get { return ServiceConfigs.MsgQSetting.InnerRoutingServerAddressReplica; }  set { ServiceConfigs.MsgQSetting.InnerRoutingServerAddressReplica = value; } }
        public static string  LogType                    { get { return ServiceConfigs.MsgQSetting.InnerLogType; }                      set { ServiceConfigs.MsgQSetting.InnerLogType = value; } }
        public static int     HBTimeout                  { get { return ServiceConfigs.MsgQSetting.InnerHBTimeout; }                    set { ServiceConfigs.MsgQSetting.InnerHBTimeout = value; } }
        public static int     RoutUpdateTimeout          { get { return ServiceConfigs.MsgQSetting.InnerRoutUpdateTimeout; }            set { ServiceConfigs.MsgQSetting.InnerRoutUpdateTimeout = value; } }
        public static int     Set                        { get { return ServiceConfigs.MsgQSetting.InnerSet; }                          set { ServiceConfigs.MsgQSetting.InnerSet = value; } }
        public static string  AddrSource                 { get { return ServiceConfigs.MsgQSetting.InnerAddrSource; }                   set { ServiceConfigs.MsgQSetting.InnerAddrSource = value; } }
        public static string  MailSender                 { get { return ServiceConfigs.MsgQSetting.InnerMailSender; }                   set { ServiceConfigs.MsgQSetting.InnerMailSender = value; } }
        public static string  PhoneList                  { get { return ServiceConfigs.MsgQSetting.InnerPhoneList; }                    set { ServiceConfigs.MsgQSetting.InnerPhoneList = value; } }
        public static string  MailList                   { get { return ServiceConfigs.MsgQSetting.InnerMailList; }                     set { ServiceConfigs.MsgQSetting.InnerMailList = value; } }
        public static string  RtxList                    { get { return ServiceConfigs.MsgQSetting.InnerRtxList; }                      set { ServiceConfigs.MsgQSetting.InnerRtxList = value; } }



    }
}
