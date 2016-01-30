namespace Dova.Utility
{
    using System;
    using System.IO;
    using System.Text;
    using System.Linq;
    using System.Threading;
    using System.Collections;
    using System.Diagnostics;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using Dova.Interfaces;
    using Dova.Services;
    using System.ServiceModel;



    /// <summary>
    /// 日志
    /// 作者:陈亮
    /// 日期:2010-06-02
    /// </summary>
    [ServiceBehavior(MaxItemsInObjectGraph = 0x7fffffff, InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple, AddressFilterMode = AddressFilterMode.Any)]
    public class RemoteLog : Log, ILog
    {
        private static ServiceFactory<ILog> logFactory = null;

        public delegate void RemoteLogDelegate(List<string> actions, List<byte[]> msgs);

        static RemoteLog()
        {
            try
            {
                logFactory = new ServiceFactory<ILog>(ServiceConfigs.GetConfigs("LogService"));
            }
            catch (Exception e)
            {
                string msg = e.Message;
            } 
        }

        public RemoteLog()
        {
            Flush += RemoteLog_Flush;
        }

        void RemoteLog_Flush(object sender, EventArgs e)
        {
            try
            {
                LogEventArgs le = e as LogEventArgs;
                PushLogs(le.logs);
            }
            catch { }
        }

        static int failTimes = 0;
        static int failTimeLimit = 5;
        static int failoverTime = 3;

        static DateTime time = DateTime.Now; 


        //public void PushLogs(Queue<KeyValuePair<string, string>> logs)
        //{
        //    try
        //    {
        //        if (failTimes > failTimeLimit)
        //        {
        //            if (time < DateTime.Now.AddMinutes(-failoverTime))
        //            {
        //                failTimes = 0;
        //            }
        //            return;
        //        }
        //        RemoteLogDelegate rld = new RemoteLogDelegate(PushRemoteLogs);
        //        rld.BeginInvoke(logs, null, null);
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Error(e);
        //    }
        //}


        //public   void PushRemoteLogs(Queue<KeyValuePair<string, string>> msgs)
        //{
        //    ServiceProcess<ILog> server = null;
        //    try
        //    {
        //        using (IDisposable service = logFactory.GetServer().Instance() as IDisposable)
        //        {
        //            (service as ILog).PushLogs(msgs);
        //        }
        //    }
        //    catch (Exception e)
        //    { 
        //        failTimes++;
        //        time = DateTime.Now;
        //    } 
        //}
         
        public override void PushLogs(List<string> actions, List<StringBuilder> msgs)
        {
            ServiceProcess<ILog> server = null;
            try
            { 
                using (IDisposable service = logFactory.GetServer().Instance() as IDisposable)
                {
                    (service as ILog).PushLogs(actions, msgs);
                }
                msgs = null;
            }
            catch (Exception e)//自己出错不有再记自己
            {
                // Log.Write("远程日志出错:" + e.Message + ":休息:" + failoverTime + "分钟");
                time = DateTime.Now;
                failTimes++;
                time = DateTime.Now;
            }
        }


        public override void PushLogs(List<string> actions, List<byte[]> msgs)
        { 
            ServiceProcess<ILog> server = null;
            try
            {
                using (IDisposable service = logFactory.GetServer().Instance() as IDisposable)
                {
                    (service as ILog).PushLogs(actions, msgs);
                }
            }
            catch (Exception e)
            {
                time = DateTime.Now;
                failTimes++;
                time = DateTime.Now;
            }
            finally
            {
                actions.Clear();
                msgs.Clear();
                msgs = null; 
            }
        }

        public override void PushLogs(Dictionary<string, StringBuilder> logs)
        {
            try
            { 
                if (null == logs || logs.Count == 0) return;
                if (failTimes > failTimeLimit)
                {
                    if (time < DateTime.Now.AddMinutes(-failoverTime))
                    {
                        failTimes = 0;
                    }
                    return;
                }
                List<byte[]> msgs = new List<byte[]>();
                List<string> actions = new List<string>();
                StringBuilder sb = null;
                foreach (string action in logs.Keys)
                { 
                    sb = logs[action];
                    if (sb.Length > 0)
                    {
                        actions.Add(action);
                        msgs.Add(encode.GetBytes(sb.ToString()));
                    }
                } 
                RemoteLogDelegate rld = new RemoteLogDelegate(PushLogs);
                rld.BeginInvoke(actions, msgs, null, null);
            }
            catch (Exception e)
            {
                //Log.Error(e);
            }
            finally
            { 
            }
        }

    }

}

