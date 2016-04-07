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
    using Dova.Data;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.ServiceModel;



    /// <summary>
    /// 日志
    /// 作者:陈亮
    /// 日期:2010-06-02
    /// 动态反射Storage类,对日志进行格式化存储. 这个类应该拆到新工程里面.
    /// </summary>
    [ServiceBehavior(MaxItemsInObjectGraph = 0x7fffffff, InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple, AddressFilterMode = AddressFilterMode.Any)]
    public class RemoteLogService : ILog
    {
        static DAC dac = null;
        static bool isDb = LogConfig.StoreDB;
        const string SaveLogSP = "SaveLogSP";
        static DateTime flushTime = DateTime.Now;
        static readonly ReaderWriterLockSlim srwLock = new ReaderWriterLockSlim();
        protected static Encoding encode = Encoding.GetEncoding("GB2312");

        static DataTable schema = new DataTable();
        static DataTable schemaCache = new DataTable();
        static SqlBulkCopyColumnMapping[] mappings = new SqlBulkCopyColumnMapping[13];

        static string eccMonitorKey = "6094c672bfd59334b2242958d3b29620";
        static RemoteLogService()
        {
            try
            {
                schema.Columns.Add("MonitorAction", typeof(string));
                schema.Columns.Add("MonitorTime", typeof(DateTime));
                schema.Columns.Add("Host", typeof(string));
                schema.Columns.Add("IP", typeof(string));
                schema.Columns.Add("MAC", typeof(string));
                schema.Columns.Add("AppPath", typeof(string));
                schema.Columns.Add("AppName", typeof(string));
                schema.Columns.Add("ProcessName", typeof(string));
                schema.Columns.Add("FirstCag", typeof(string));
                schema.Columns.Add("SecondCag", typeof(string));
                schema.Columns.Add("ThirdCag", typeof(string)); 
                schema.Columns.Add("MonitorValue", typeof(double));
                schema.Columns.Add("Msg", typeof(string));
                mappings[0] = new SqlBulkCopyColumnMapping("MonitorAction", "MonitorAction");
                mappings[1] = new SqlBulkCopyColumnMapping("MonitorTime", "MonitorTime");
                mappings[2] = new SqlBulkCopyColumnMapping("Host", "Host");
                mappings[3] = new SqlBulkCopyColumnMapping("IP", "IP");
                mappings[4] = new SqlBulkCopyColumnMapping("MAC", "MAC");
                mappings[5] = new SqlBulkCopyColumnMapping("AppPath", "AppPath");
                mappings[6] = new SqlBulkCopyColumnMapping("AppName", "AppName");
                mappings[7] = new SqlBulkCopyColumnMapping("ProcessName", "ProcessName"); 
                mappings[8] = new SqlBulkCopyColumnMapping("FirstCag", "FirstCag");
                mappings[9] = new SqlBulkCopyColumnMapping("SecondCag", "SecondCag");
                mappings[10] = new SqlBulkCopyColumnMapping("ThirdCag", "ThirdCag"); 
                mappings[11] = new SqlBulkCopyColumnMapping("MonitorValue", "MonitorValue");
                mappings[12] = new SqlBulkCopyColumnMapping("Msg", "Msg");
                if (isDb)
                    dac = new DAC(Config.LogConnectionName);
                schemaCache = schema.Clone();

            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }


        void RemoteLogService_Logging(object sender, EventArgs e)
        {
            // ECCMonitorClient.Report(eccMonitorKey);
        }

        static void Log_Flush(object sender, EventArgs e)
        {
            FlushLog(false);
        }

        [OperationBehavior(AutoDisposeParameters = true)]
        public void PushLogs(string action, string msg)
        {
            try
            {
                Log.PushLog(action, msg);
                PushStorage(action, msg);
            }
            catch { }
        }

        [OperationBehavior(AutoDisposeParameters = true)]
        public void PushLogs(List<string> actions, List<string> msgs)
        {
            int n = 0;
            foreach (string action in actions)
            {
                Log.PushLog(action, msgs[n]);
                PushStorage(action, msgs[n]);
                n++;
            }
        }

        [OperationBehavior(AutoDisposeParameters = true)]
        public void PushLogs(List<string> actions, List<StringBuilder> msgs)
        {
            int n = 0;
            string s = "";
            foreach (string action in actions)
            {
                s = msgs[n].ToString();
                Log.PushLog(action, s);
                PushStorage(action, s);
                n++;
            }
            msgs.Clear();
        }

        [OperationBehavior(AutoDisposeParameters = true)]
        public void PushLogs(Queue<KeyValuePair<string, StringBuilder>> msgs)
        {
            try
            {
                KeyValuePair<string, StringBuilder> kvp;
                while (msgs.Count > 0)
                {
                    kvp = msgs.Dequeue();
                    Log.PushLog(kvp.Key, kvp.Value);
                    PushStorage(kvp.Key, kvp.Value.ToKey());
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        [OperationBehavior(AutoDisposeParameters = true)]
        public void PushLogs(List<string> actions, List<byte[]> msgs)
        {
            int n = 0;
            byte[] msg = null;
            string act = "";
            int idx = -1;
            string s = "";
            foreach (string action in actions)
            {
                act = action;
                msg = msgs[n];
                n++;
                s = encode.GetString(msg);
                //Thread.Sleep(50);
                Log.PushLog(action, s);
                try
                {
                    idx = action.LastIndexOf("\\");
                    if (idx > -1)
                        act = action.Substring(idx + 1, action.Length - idx - 1);
                    PushStorage(act, s);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
            msg = null;
        }

        public void PushStorage(string action, string msg)
        {
            if (!isDb) return;
            DataTable dt = null;
            DataTable temp = schemaCache.Clone();
            try
            {
                srwLock.EnterWriteLock();
                using (StringReader sr = new StringReader(msg))
                {
                    string ms = "";
                    double mv = 0.0;
                    while (true)
                    {
                        DataRow row = schema.NewRow();
                        ms = sr.ReadLine(); if (null == ms) break;
                        string[] ss = ms.Split(new char[] { '\t' }, StringSplitOptions.None);
                        if (ss.Length > 11)
                        {
                            //2016-02-21 13:31:23.720	O5PZK6VZOEB29VZ	192.168.1.95	AC:FD:CE:6E:1E:87	E:\Projects\dova\windows\Samples\bin\Release\	测试应用	Dova.Samples	Program	Main	Test	3440196	测试日志服务 20
                            mv = ss[10].ToDouble();
                            if (mv < 0) continue;
                            row[0] = action;
                            row[1] = ss[0].ToDateTime();
                            row[2] = ss[1];//host
                            row[3] = ss[2];//ip
                            row[4] = ss[3];//mac
                            row[5] = ss[4];//apppath
                            row[6] = ss[5];// appName
                            row[7] = ss[6];//processName
                            row[8] = ss[7];//className
                            row[9] = ss[8];//methodName
                            row[10] = ss[9];//tagName
                            row[11] = mv;//value
                            row[12] = ss[11].Length > 256 ? ss[11].Substring(0, 255) : ss[11];
                            schema.Rows.Add(row);
                        }
                    }
                }
                if (schema.Rows.Count >= LogConfig.FlushNumber || (DateTime.Now - flushTime).Seconds > LogConfig.FlushSpan)
                {
                    dt = schema;
                    schema = temp;
                    flushTime = DateTime.Now;
                }
            }
            catch (Exception e)
            {
                dt.Clear();
                dt.Dispose();
                schema.Clear();
                schema.Dispose();
                Log.Error(e);
                e = null;
            }
            finally
            {
                if (srwLock.IsWriteLockHeld)
                    srwLock.ExitWriteLock();
            }
            if (null != dt && dt.Rows.Count > 0)
            {
                FlushLog(dt);
            }
        }

        static void ECCReport(string appName)
        {


        }


        static DataTable GetTempLogs(bool forceClear)
        {
            DataTable dt = null;
            try
            {
                srwLock.EnterWriteLock();
                if (forceClear || ((schema.Rows.Count > 0) && (DateTime.Now - flushTime).Seconds > LogConfig.FlushSpan))
                {
                    dt = schema.Copy();
                    schema.Rows.Clear();
                    flushTime = DateTime.Now;
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            finally
            {
                if (srwLock.IsWriteLockHeld)
                    srwLock.ExitWriteLock();
            }
            return dt;
        }

        public static void FlushLog(DataTable dt)
        {
            try
            {
                dac.SqlBulkInsertDataTable("Logs", dt, mappings);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                ex = null;
            }
            finally
            {
                dt.Clear();
                dt.Dispose();
                dt = null;
            }
        }

        //程序结束的时候要清理掉
        public static void FlushLog(bool forceClear)
        {
            try
            {
                //DataTable dt = GetTempLogs(forceClear);
                //if (null != dt)
                //    FlushLog(dt);
                FlushLog(schema);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                ex = null;
            }
        }




        public void Dispose()
        {

        }
    }

}


