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
    using System.ServiceModel;
    using System.ComponentModel;
    using System.ServiceModel.Channels;

    public delegate void DealDelegate(Dictionary<string, StringBuilder> logs);


    /// <summary>
    /// 日志
    /// 作者:陈亮
    /// 日期:2010-06-02
    /// </summary>
    [ServiceBehavior(MaxItemsInObjectGraph = 0x7fffffff, InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple, AddressFilterMode = AddressFilterMode.Any)]
    public class Log : MarshalByRefObject, ILog
    {
        protected static Encoding encode = Encoding.GetEncoding("GB2312");

        protected static DateTime bakTime = DateTime.Now;
        protected static DateTime clearTime = DateTime.Now;
        //static Dictionary<string, StringBuilder> logs = null;
        public  Queue<KeyValuePair<string, StringBuilder>> logCache = new Queue<KeyValuePair<string, StringBuilder>>();
        public  Queue<KeyValuePair<string, StringBuilder>> logStack = new Queue<KeyValuePair<string, StringBuilder>>();
        static readonly ReaderWriterLockSlim rwLock = null;
        static readonly ReaderWriterLockSlim frwLock = null;


        static DealDelegate del;
        static Mutex mut = null;

        static int bakMinutes = 5;
        static int clearMinutes = 10;

        static int limitLength = 80 * 1024;

        static int logDep = 0;

        public static string tempDirs = "";

        static Log log = new Log();

        static bool logGC = false;

        static string logPath = Config.BasePath + "Logs\\";
        static string backPath = "";

        static string appName = "";
        static string logPrex = "";

        static readonly string logEventKey = "LogingEventKey";
        static readonly string logFlushKey = "LogFlushKey";
        protected static EventHandlerList eventHandler = new EventHandlerList();
        public static event EventHandler Logging
        {
            add
            {
                eventHandler.AddHandler(logEventKey, value);
            }
            remove
            {
                eventHandler.RemoveHandler(logEventKey, value);
            }
        }



        public event EventHandler Flush
        {
            add
            {
                eventHandler.AddHandler(logFlushKey, value);
            }
            remove
            {
                eventHandler.RemoveHandler(logFlushKey, value);
            }
        }



        protected static void OnFlush(EventArgs args)
        {
            EventHandler handler = eventHandler[logFlushKey] as EventHandler;
            if (null != handler)
                handler(log, args);
        }


        protected static void OnLogging(LogEventArgs args)
        {
            EventHandler handler = eventHandler[logEventKey] as EventHandler;
            if (null != handler)
                handler(log, args);
        }

        static Log()
        {
            try
            {
                del = new DealDelegate(DealMessages);
                int type = LogConfig.LogType;
                appName = Config.AppName;
                logDep = LogConfig.LogCagDep;
                logGC = LogConfig.LogGC;
                logPrex = appName + "\\";
                if (type <= 0 && type != (int)LogAction.All) return;
                try
                {
                    rwLock = new ReaderWriterLockSlim();
                    frwLock = new ReaderWriterLockSlim();
                    if (LogConfig.IsMutex)
                        mut = new Mutex(false, "LogMutex");
                }
                catch
                {
                    try
                    {
                        mut = Mutex.OpenExisting("LogMutex");
                    }
                    catch { LogConfig.IsMutex = false; }
                }
                Init();
                if (LogConfig.LogMate == 2)
                {
                    try
                    {
                        log = new RemoteLog();
                    }
                    catch { }
                }
                if (null == log)
                    log = new Log();
            }
            catch (Exception e) { e = null; LogConfig.LogType = 0; }
        }

        private static void InitActions(string path, Array acs)
        {
            //foreach (LogAction ac in acs)
            //{
            //    if (((LogConfig.LogType & (int)ac) != 0) && !logs.ContainsKey(ac.ToString()))
            //    {
            //        logs.Add(ac.ToString(), new StringBuilder());
            //    }
            //    if ((LogConfig.LogType & (int)ac) != 0 && ac != LogAction.Write && ac != LogAction.All)
            //        Directory.CreateDirectory(path + ac.ToString());
            //}
        }

        static void Init()
        {
            logPath = LogConfig.LogPath;
            if (string.IsNullOrEmpty(LogConfig.BakPath))
                backPath = logPath;
            else
                backPath = LogConfig.BakPath;
            if (!logPath.EndsWith("\\") && !logPath.EndsWith("/"))
                logPath = logPath + "\\";
            if (!backPath.EndsWith("\\") && !backPath.EndsWith("/"))
                backPath = backPath + "\\";
            try
            {
                Directory.CreateDirectory(logPath);
                Directory.CreateDirectory(backPath);
            }
            catch
            {
            }
            Array acs = Enum.GetValues(typeof(LogAction));
            InitActions(logPath, acs);
            Thread thread = new Thread(WriteLog);
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.AboveNormal;
            thread.Start();
        }

        public static string GetConfigItem(string key, string defaultValue)
        {
            NameValueCollection appSettings = ConfigurationManager.AppSettings;
            if (null != appSettings && null != appSettings[key])
                return appSettings[key];
            return defaultValue;
        }

        public static void Write(string msg)
        {
            if (LogConfig.LogType != 0)
                PushLog(logPrex + LogAction.Write.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + msg);
        }

        public static void WriteNoTime(string msg)
        {
            if (LogConfig.LogType != 0)
                PushLog(logPrex + LogAction.Write.ToString(), msg);
        }

        public static void WriteNoTime(LogAction action, string dir, string msg)
        {
            if ((LogConfig.LogType & (int)action) != 0)
            {
                string key = action.ToString() + "\\" + dir;
                if (tempDirs.IndexOf(key + ",") == -1)
                {
                    tempDirs += key + ",";
                    //logs[key] = new StringBuilder(); ;
                }
                PushLog(key, msg);
            }
        }

        public static void WriteNoTime(string dir, string msg)
        {
            if (LogConfig.LogType != 0)
            {
                string key = dir;
                if (tempDirs.IndexOf(key + ",") == -1)
                {
                    tempDirs += key + ",";
                    //logs[key] = new StringBuilder();
                }
                PushLog(key, msg);
            }
        }

        public static void Write(Exception e)
        {
            if (LogConfig.LogType != 0)
                PushLog(logPrex + LogAction.Write.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + ":" + e.Message + " " + e.StackTrace);
        }

        public static void Set(string msg)
        {
            if ((LogConfig.LogType & (int)LogAction.Set) != 0)
                PushLog(logPrex + LogAction.Set.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + msg);
        }

        public static void Get(string msg)
        {
            if ((LogConfig.LogType & (int)LogAction.Get) != 0)
                PushLog(logPrex + LogAction.Get.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + msg);
        }

        public static void Error(Exception e)
        {
            if ((LogConfig.LogType & (int)LogAction.Error) != 0)
                PushLog(logPrex + LogAction.Error.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + ":" + e.Source + ":" + e.Message + " " + e.StackTrace);
        }

        public static void Write(LogAction action, string msg)
        {
            if ((LogConfig.LogType & (int)action) != 0)
                PushLog(logPrex + action.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + msg);
        }



        /// <summary>
        /// 记录格式化日志
        /// </summary>
        /// <param name="action">日志类型</param>
        /// <param name="firstCategory">类名</param>
        /// <param name="secondCategory">方法名</param>
        /// <param name="thirdCategory">日志标志</param>
        /// <param name="timeSpan">执行时间</param>
        /// <param name="msg">日志内容</param>
        public static void Write(LogAction action, string firstCategory, string secondCategory, string thirdCategory, long timeSpan, string msg)
        {
            if ((LogConfig.LogType & (int)action) != 0)
            {
                if (msg.Length > limitLength)
                    msg = msg.Substring(0, limitLength / 2);
                string s = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + string.Format(Config.LogFormat, firstCategory, secondCategory, thirdCategory, timeSpan, msg);
                string path = action.ToString();
                if (logDep == 2)
                    path = firstCategory + "\\" + path;
                else if (logDep == 3)
                    path = firstCategory + "\\" + secondCategory + "\\" + path;
                PushLog(logPrex + path, s);
                s = null;
            }
        }


        public static void Write(LogAction action, string firstCategory, string secondCategory, string thirdCategory,string forthCategory, long timeSpan, string msg)
        {
            if ((LogConfig.LogType & (int)action) != 0)
            {
                if (msg.Length > limitLength)
                    msg = msg.Substring(0, limitLength / 2);
                string s = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + string.Format(Config.LogFormat2, firstCategory, secondCategory, thirdCategory,forthCategory, timeSpan, msg);
                string path = action.ToString();
                if (logDep == 2)
                    path = firstCategory + "\\" + path;
                else if (logDep == 3)
                    path = firstCategory + "\\" + secondCategory + "\\" + path;
                PushLog(logPrex + path, s);
                s = null;
            }
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="action">日志类型</param>
        /// <param name="dep">记录日志目录深度,根据category 值 为 1 , 2</param>
        /// <param name="firstCategory">应用名</param>
        /// <param name="secondCategory">进程名</param>
        /// <param name="thirdCategory">类名</param>
        /// <param name="forthCategory">方法名</param>
        /// <param name="fithCategory">标志名</param>
        /// <param name="timeSpan"></param>
        /// <param name="msg"></param>
        public static void Write(LogAction action,int dep, string firstCategory, string secondCategory, string thirdCategory, string forthCategory, string fithCategory, long timeSpan, string msg)
        {
            if ((LogConfig.LogType & (int)action) != 0)
            {
                if (msg.Length > limitLength)
                    msg = msg.Substring(0, limitLength / 2);
                string s = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + string.Format(Config.LogFormat3, firstCategory, secondCategory, thirdCategory, forthCategory,fithCategory, timeSpan, msg);
                string path = action.ToString();
                if (dep == 1)
                    path = firstCategory + "\\" + path;
                else if (logDep == 2)
                    path = firstCategory + "\\" + secondCategory + "\\" + path;
                PushLog(logPrex + path, s);
                s = null;
            }
        }

        /// <summary>
        /// 记录自定义目录的格式化日志
        /// </summary> 
        /// <param name="customType">自定义的日志类型(会新建立一个文件夹)</param>
        /// <param name="dir"></param>
        /// <param name="firstCategory">应用名</param>
        /// <param name="secondCategory">进程名</param>
        /// <param name="thirdCategory">类名</param>
        /// <param name="forthCategory">方法名</param>
        /// <param name="fithCategory">标志名</param>
        /// <param name="timeSpan">执行时间</param>
        /// <param name="msg">日志内容</param>
        public static void Write(string customType,int dep, string firstCategory, string secondCategory, string thirdCategory, string forthCategory, string fithCategory, long timeSpan, string msg)
        {
            string s = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + string.Format(Config.LogFormat3, Config.AppName, firstCategory, secondCategory, thirdCategory, forthCategory, fithCategory, timeSpan, msg);
            if (msg.Length > limitLength)
                msg = msg.Substring(0, limitLength / 2);
            string path = customType;
            if (dep == 1)
                path = firstCategory + "\\" + path;
            else if (dep == 2)
                path = firstCategory + "\\" + secondCategory + "\\" + path;
            PushLog(logPrex + path, s);
        }


        /// <summary>
        /// 记录自定义目录的格式化日志
        /// </summary> 
        /// <param name="customType">自定义的日志类型(会新建立一个文件夹)</param>
        /// <param name="dir"></param>
        /// <param name="firstCategory">类名</param>
        /// <param name="secondCategory">方法名</param>
        /// <param name="thirdCategory">日志标志</param>
        /// <param name="timeSpan">执行时间</param>
        /// <param name="msg">日志内容</param>
        public static void Write(string customType, string firstCategory, string secondCategory, string thirdCategory, long timeSpan, string msg)
        {
            string s = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + string.Format(Config.LogFormat3, Config.AppName, Config.ProcessName, firstCategory, secondCategory, thirdCategory, timeSpan, msg);
            if (msg.Length > limitLength)
                msg = msg.Substring(0, limitLength / 2);
            string path = customType;
            if (logDep == 1)
                path = firstCategory + "\\" + path;
            else if (logDep == 2)
                path = firstCategory + "\\" + secondCategory + "\\" + path;
            PushLog(logPrex + path, s);
        }

        public static void Write(string dir, string msg)
        {
            if (LogConfig.LogType != 0)
            {
                string key = dir;
                if (tempDirs.IndexOf(key + ",") == -1)
                {
                    tempDirs += key + ",";
                    // logs[key] = new StringBuilder();
                }
                PushLog(key, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + msg);
            }
        }

        public static void Write(LogAction action, string dir, string msg)
        {
            if ((LogConfig.LogType & (int)action) != 0)
            {
                string key = logPrex + dir + "\\" + action.ToString();
                PushLog(key, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + msg);
            }
        }

        public static void Error(string msg)
        {
            if ((LogConfig.LogType & (int)LogAction.Error) != 0)
                PushLog(logPrex + LogAction.Error.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + msg);
        }

        public static void Info(string msg)
        {
            if ((LogConfig.LogType & (int)LogAction.Info) != 0)
                //PushLog(logPrex + LogAction.Info.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + msg);
				PushLog(logPrex + LogAction.Info.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + msg);
        }

        public static void Warn(string msg)
        {
            if ((LogConfig.LogType & (int)LogAction.Warn) != 0)
                PushLog(logPrex + LogAction.Warn.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + msg);
        }

        public static void Dac(string msg)
        {
            if ((LogConfig.LogType & (int)LogAction.Dac) != 0)
                PushLog(logPrex + LogAction.Dac.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + msg);
        }

        public static void Dao(string msg)
        {
            if ((LogConfig.LogType & (int)LogAction.Dao) != 0)
                PushLog(logPrex + LogAction.Dao.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + msg);

        }

        public static void Bll(string msg)
        {
            if ((LogConfig.LogType & (int)LogAction.BLL) != 0)
                PushLog(logPrex + LogAction.BLL.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + msg);
        }

        public static void Fcd(string msg)
        {
            if ((LogConfig.LogType & (int)LogAction.Fcd) != 0)
                PushLog(logPrex + LogAction.Fcd.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + msg);
        }

        public static void Svc(string msg)
        {
            if ((LogConfig.LogType & (int)LogAction.Svc) != 0)
            {
                PushLog(logPrex + LogAction.Svc.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + msg);
            }
        }

        public static void UI(string msg)
        {
            if ((LogConfig.LogType & (int)LogAction.UI) != 0)
                PushLog(logPrex + LogAction.UI.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" + msg);
        }

        public static void PushLog(string action, string msg)
        {
            log.PushLogs(action, msg);
        }


        public static void PushLog(string action, StringBuilder msg)
        {
            log.PushLogs(action, msg);
        }


        public virtual void PushLogs(List<string> actions, List<string> msgs)
        {
        }

        public virtual void PushLogs(List<string> actions, List<StringBuilder> msgs)
        {
        }


        public virtual void PushLogs(List<string> actions, List<byte[]> msgs)
        {
        }

        public virtual void PushLogs(Queue<KeyValuePair<string, StringBuilder>> msgs)
        {
            while (msgs.Count > 0)
                log.logStack.Enqueue(msgs.Dequeue());
        }


        public virtual void PushLogs(Dictionary<string, StringBuilder> logs)
        {
        }


        public virtual void PushLogs(string action, string msg)
        {
            try
            {
                if (null == log.logStack || log.logStack.Count > LogConfig.SecurityMemNumber)
                    return;
                KeyValuePair<string, StringBuilder> l = new KeyValuePair<string, StringBuilder>(action, new StringBuilder(msg + "\r\n"));
                rwLock.EnterWriteLock();
                log.logStack.Enqueue(l);
            }
            catch (Exception e) { e = null; }
            finally
            {
                if (null != rwLock && rwLock.IsWriteLockHeld)
                    rwLock.ExitWriteLock();
            }
        }

        public virtual void PushLogs(string action, StringBuilder msg)
        {
            try
            {
                if (null == log.logStack || log.logStack.Count > LogConfig.SecurityMemNumber)
                    return;
                rwLock.EnterWriteLock();
                KeyValuePair<string, StringBuilder> l = new KeyValuePair<string, StringBuilder>(action, msg);
                log.logStack.Enqueue(l);
            }
            catch (Exception e) { e = null; }
            finally
            {
                if (null != rwLock && rwLock.IsWriteLockHeld)
                    rwLock.ExitWriteLock();
            }
        }

        static Log log2 = new Log();
        static void WriteLog()
        {
            int cnt = 0;
            while (true)
            {
                int rt = LogConfig.RefreshTime;
                if (rt < 1) rt = 1;
                Thread.Sleep(rt * 1000);
                if (log.logStack.Count == 0)
                {
                    continue;
                }
                Queue<KeyValuePair<string, StringBuilder>> stack = null;
                try
                {
                    rwLock.EnterWriteLock();
                    //stack = log.logStack; 
                    //log.logStack = null;
                    //log = null;
                    //log = log2;
                    //log2 = new Log();
                    stack = log.logStack;
                    log.logStack = null;
                    log.logStack = log.logCache; 
                    log.logCache = new Queue<KeyValuePair<string, StringBuilder>>();
                }
                catch (Exception e) { e = null; }
                finally
                {
                    if (null != rwLock && rwLock.IsWriteLockHeld)
                        rwLock.ExitWriteLock();
                }
                try
                {
                    Dictionary<string, StringBuilder> logs = new Dictionary<string, StringBuilder>();
                    KeyValuePair<string, StringBuilder> lg;
                    while (stack.Count > 0)
                    {
                        lg = (KeyValuePair<string, StringBuilder>)stack.Dequeue();
                        if (null != lg.Key)
                        {
                            if (!logs.ContainsKey(lg.Key))
                            {
                                logs[lg.Key] = new StringBuilder();
                            }
                            logs[lg.Key].Append(lg.Value); 
                        }
                    }
                    stack.Clear();
                    stack = null; 
                    //log.logCache = null;
                    //log.logCache = new Queue<KeyValuePair<string, StringBuilder>>();
                    del.BeginInvoke(logs, null, null);
                }
                catch (Exception e)
                {
                    e = null;
                    stack.Clear();
                    stack = null;
                    log.logCache = null;
                    log.logCache = new Queue<KeyValuePair<string, StringBuilder>>();
                }
                if (logGC)
                    GC.Collect();
            }
        }


        public static void WriteMsg(StringBuilder msg, string logPath, string bakDir, bool isMutex)
        {
            if (!isMutex)
            {
                try
                {
                    frwLock.EnterWriteLock();
                    WriteMsg(msg, logPath, bakDir);
                }
                catch (Exception e) { e = null; }
                finally
                {
                    if (frwLock.IsWriteLockHeld)
                        frwLock.ExitWriteLock();
                }
                return;
            }
            try
            {
                mut.WaitOne();
                WriteMsg(msg, logPath, bakDir);
            }
            catch (Exception e) { e = null; }
            finally
            {
                mut.ReleaseMutex();
            }
        }

        static void DealMessages(Dictionary<string, StringBuilder> logs)
        {
            try
            {
                DateTime now = DateTime.Now;
                int spanBak = (now - bakTime).Minutes;
                int spanClear = (now - clearTime).Minutes;
                if (spanBak > bakMinutes) bakTime = now;
                if (spanClear > clearMinutes) clearTime = now;
                string act = "";
                string timeFormat = now.ToString("yyyy-MM-dd HH-00-00") + ".txt";
                if (LogConfig.TimedLog == 2)
                    timeFormat = now.ToString("yyyy-MM-dd 00-00-00") + ".txt";
                string w = LogAction.Write.ToString();

                foreach (string action in logs.Keys)
                {
                    try
                    {
                        act = (action + "\\").Replace(w + "\\", "");
                        if (logDep == 0)
                            act = act.Replace(logPrex, "");
                        Directory.CreateDirectory(logPath + act);
                        WriteMsg(logs[action], logPath + act + timeFormat, backPath + act, LogConfig.IsMutex);
                    }
                    catch (Exception e) { e = null; logs.Clear(); }
                }
                if (spanClear > clearMinutes)
                {
                    ClearLog(logPath, backPath);
                    GC.Collect();
                }
            }
            catch (Exception e)
            {
                e = null;
                logs.Clear();
            }
            LogEventArgs arg = new LogEventArgs(logs);
            OnFlush(arg);
            //logs.Clear();
            //logs = null;
        }



        static void WriteCallback(IAsyncResult asyncResult)
        {
            try
            {
                FileStream fs = (FileStream)asyncResult.AsyncState;
                fs.EndWrite(asyncResult);
                fs.Flush();
                fs.Close();
                fs.Dispose();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public static void WriteMsg(StringBuilder msg, string logPath, string bakDir)
        {
            try
            {
                FileStream fs = null;
                long len = 0;
                //using (fs = new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.Delete | FileShare.ReadWrite, msg.Length, FileOptions.WriteThrough | FileOptions.Asynchronous))
                fs = new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.Delete | FileShare.ReadWrite, 8192, FileOptions.WriteThrough | FileOptions.Asynchronous);
                {
                    len = fs.Length + msg.Length;
                    byte[] bs = encode.GetBytes(msg.ToString());
                    IAsyncResult ir = fs.BeginWrite(encode.GetBytes(msg.ToString()), 0, bs.Length, WriteCallback, fs);
                }
                //using (StreamWriter sw = new StreamWriter(logPath, true, encode))
                //{
                //    sw.Write(msg);
                //    sw.Flush();
                //}

                //FileInfo ff = new FileInfo(logPath);
                //len = ff.Length;
                if (len > LogConfig.BakLength)
                {
                    string path = bakDir + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + ".txt";
                    FileInfo ff = new FileInfo(logPath);
                    ff.MoveTo(path);
                }
            }
            catch (Exception e)
            {
                int idx = logPath.LastIndexOf("/");
                if (idx == -1)
                    idx = logPath.LastIndexOf("\\");
                string dir = logPath.Remove(idx);
                Directory.CreateDirectory(dir);
                Log.Error(e);
                e = null;
            }
            // GC.Collect();
        }



        public static void WriteMsg(object msg, string path)
        {
            using (StreamWriter sw = new StreamWriter(path, true, encode))
            {
                sw.Write(msg);
            }
        }

        public static void DeleteExpirateFiles(string dir, DateTime expDate)
        {
            string[] dirs = Directory.GetDirectories(dir);
            List<string> files = Directory.GetFiles(dir, "*.txt").ToList().Where(x => ((File.GetLastWriteTime(x) < expDate))).ToList();
            foreach (string f in files)
            {
                File.Delete(f);
            }
            foreach (string d in dirs)
            {
                DeleteExpirateFiles(d, expDate);
            }
        }

        static void BakLog(string logPath, string backDir)
        {
            try
            {
                DateTime now = DateTime.Now;
                FileInfo f = new FileInfo(logPath);
                if (f.Length > LogConfig.BakLength)
                {
                    string path = backDir + now.ToString("yyyy-MM-dd HH-mm-ss") + ".txt";
                    f.MoveTo(path);
                }
            }
            catch (Exception e) { e = null; }
        }

        static void ClearLog(string logPath, string backDir)
        {
            try
            {
                DateTime now = DateTime.Now;
                DirectoryInfo dirInfo = new DirectoryInfo(backDir);
                DateTime expDate = now.AddHours(-LogConfig.ClearSpan);
                if (LogConfig.TimedLog == 2)
                    expDate = now.AddDays(-LogConfig.ClearSpan);
                DeleteExpirateFiles(backDir, expDate);
            }
            catch (Exception e) { e = null; }
        }



    }

    /// <summary>
    /// 根据需要扩展日志类型
    /// </summary>
    [Flags]
    public enum LogAction
    {
        All = ~0,
        None = 0,
        Write = 1,
        Info = 2,
        Error = 4,
        Warn = 8,
        Dac = 16,
        Dao = 32,
        Get = 64,
        Set = 128,
        BLL = 256,
        Fcd = 512,
        Svc = 1024,
        UI = 2048,
        Fatal = 4096
    }

}

