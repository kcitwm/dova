using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading; 
using System.Net;
using WQFree.Cust.Framework.Common;
using WQFree.Utility;

namespace RMA.Util
{
    public class ECCMonitorClient
    {
        //private ILog _log = log4net.LogManager.GetLogger("log");
        private bool _isReportLog = false;
        private delegate void AsycDelegate(string key);
        static private object _syncRoot = new object();
        static private object _syncRoot2 = new object();
        static private ECCMonitorClient _instance = null;
        private Dictionary<string, int> _kvList = new Dictionary<string, int>();
        private List<string> _keyList = new List<string>();

        private string _monitorUrl = //@"http://shyw.ecc.WQFree.com/data/ias_api/api/ias_api.php";
        @"http://10.205.48.69:53187/data/ias_api/api/ias_api.php";

        private HttpClient _eccMonitor = null;
        private Thread _thread;
        private bool _run = true;
        private string _localip;
        private int _interval = 10000;

        private ECCMonitorClient()
        {
            this._eccMonitor = new HttpClient(this._monitorUrl);
            this._eccMonitor.DefaultEncoding = Encoding.UTF8;
            this._eccMonitor.Verb = HttpVerb.POST;
            this._eccMonitor.KeepContext = true;

            this._localip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0].ToString();

            this._thread = new Thread(DoSendMessage);
            this._thread.IsBackground = true;
            this._thread.SetApartmentState(ApartmentState.STA);
            this._thread.Priority = ThreadPriority.AboveNormal;
            this._thread.Start();
        }

        public bool IsReportLog
        {
            set
            {
                this._isReportLog = value;
            }
        }


        /// <summary>
        /// 使用已有连接实例定义实例化单件
        /// </summary>
        /// <returns>类实例</returns>
        public static ECCMonitorClient GetInstance()
        {
            lock (_syncRoot)
            {
                if (null == _instance)
                    _instance = new ECCMonitorClient();
            }
            return _instance;
        }

        /// <summary>
        /// 上报监控锚点数据
        /// </summary>
        /// <param name="MonitorKey">在监控系统中申请的Key值</param>
        public static void Report(string MonitorKey)
        {
            try
            {
                long t = DateTime.Now.Ticks;
                Log.Write("ECCMonitorClient:" + 1);
                ECCMonitorClient ist = ECCMonitorClient.GetInstance();
                Log.Write("ECCMonitorClient:" + 2 + ",用时" + (DateTime.Now.Ticks-t));
                t = DateTime.Now.Ticks;
                AsycDelegate myAsycDelegate = new AsycDelegate(ist.keyCount);
                Log.Write("ECCMonitorClient:" + 3 + ",用时" + (DateTime.Now.Ticks - t));
                t = DateTime.Now.Ticks;
                myAsycDelegate.Invoke(MonitorKey);
                Log.Write("ECCMonitorClient:" + 4 + ",用时" + (DateTime.Now.Ticks - t));

                //return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void keyCount(string key)
        {
            lock (_syncRoot2)
            {
                try
                {
                    if (this._keyList.Contains(key))
                    {
                        this._kvList[key]++;
                    }
                    else
                    {
                        this._keyList.Add(key);
                        this._kvList.Add(key, 1);
                    }
                }
                catch (Exception e)
                {
                    //this._log.Error(e);
                }
            }
        }

        private void DoSendMessage()
        {
            List<content> msgs = new List<content>();
            while (_run)
            {
                int count = this._keyList.Count;
                int time = ConvertToTimestamp(DateTime.Now);
                if (count > 0)
                {
                    try
                    {
                        this._keyList.ForEach(delegate(string key)
                        {
                            if (this._kvList[key] > 0)
                            {
                                msgs.Add(new content(this._localip, key, this._kvList[key], time));
                                this._kvList[key] = 0;
                            }
                        });
                        if (msgs.Count > 0)
                        {
                            Thread.Sleep(30);
                            //this.Post(msgs);
                            msgs.Clear();
                        }

                    }
                    catch (Exception e)
                    {
                        //this._log.Error(e);
                    }
                }
                Thread.Sleep(this._interval);
            }
        }

        private void Post(List<content> msgs)
        {
            try
            {
                this._eccMonitor.Reset();
                this._eccMonitor.Verb = HttpVerb.POST;
                string content = Newtonsoft.Json.JsonConvert.SerializeObject(msgs);
                this._eccMonitor.PostingData.Add("content", content);
                string s = this._eccMonitor.GetString();
                //if (this._isReportLog)
                //this._log.Info("上报数据:" + content);
            }
            catch (Exception e)
            {
                //this._log.Error(e);
            }
        }

        private static int ConvertToTimestamp(DateTime value)
        {
            //create Timespan by subtracting the value provided from
            //the Unix Epoch
            TimeSpan span = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());

            //return the total seconds (which is a UNIX timestamp)
            return (int)span.TotalSeconds;
        }

        private class content
        {
            public string ip;
            public string key;
            public int count = 1;
            public double time;

            public content(string _ip, string _key, int _count, double _time)
            {
                this.ip = _ip;
                this.key = _key;
                this.count = _count;
                this.time = _time;
            }
        }

    }
}
