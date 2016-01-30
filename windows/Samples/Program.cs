
using Dova;
using Dova.Data;
using Dova.Infrastructure.Utility;
using Dova.MessageQueue;
using Dova.Services;
using Dova.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;



namespace MsgqTest
{
    class Program
    {

        private static void RegistProperties(object obj)
        {
            IDictionary dict = ChannelServices.GetChannelSinkProperties(obj);
            dict["UserData"] = "一定要通过啊";
        }


        static void Main(string[] args)
        {
            Console.WriteLine("请输入消息类型: 1 消息接收者  2. 消息生产者  3. 心跳-100");
            string type = Console.ReadLine();
            Console.WriteLine("请输入模拟用户数量");
            int usernum = Console.ReadLine().ToInt32();
            Thread mt;
            ParameterizedThreadStart ts;
            for (int i = 0; i < usernum; i++)
            {

                if (type == "1")
                    ts = new ParameterizedThreadStart(TestSocketConsumerMQService);
                else if(type=="2")
                    ts = new ParameterizedThreadStart(TestSocketProcuderMQService);
                else
                    ts = new ParameterizedThreadStart(TestSocktHeartBeat); 
                mt = new Thread(ts);
                mt.Start(i);
            }
            return;
            //for (int i = 0; i < 5; i++)
            //    Log.Error("133AAAAAAAAAAAAA");
            string sss = string.Format("{0}\t{1}\t{2}\t{2}\t{3}\t{4}", "0", "1", "2", "3");
            Console.WriteLine(sss);
            Console.ReadLine();
            return;

            //string url = "http://servicecs.Dova.com:53191/json/syncreply/SOPrintPackageRequestDTO";
            //HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
            //req.Method = "POST";
            //req.Host = "10.205.48.69";
            //HttpWebResponse rres = req.GetResponse() as HttpWebResponse;
            //using (Stream st = rres.GetResponseStream())
            //{
            //    using (StreamReader sr = new StreamReader(st))
            //    {
            //        Console.WriteLine("响应:" + sr.ReadToEnd());
            //    }
            //}
            //Console.WriteLine("请输入测试日志次数");
            //Console.ReadLine();

            //TestMQService();
            //return;

            //int tt = MsgQConfig.HBTimeout;
            Console.WriteLine("请输入测试日志次数");
            string s = Console.ReadLine();
            int n = s.ToInt32();
            for (int i = 0; i < n; i++)
                Log.Write(LogAction.Write, "Program", "Main", "", 0, "测试日志");
            string path = LogConfig.BakPath;
            try
            {
                Console.WriteLine("请输入测试的次数和请求");
                s = Console.ReadLine();

                n = s.ToInt32();

                //for(int i=0;i<n;i++)
                //    Log.Info(string.Format(Config.LogFormat,"test", "tt", "Login", 0, "sss:"));
                //return;


                Console.WriteLine("请输入测试类型: 1 同步/路由  2 异步 3 异步路由");
                s = Console.ReadLine();

                Console.WriteLine("请输入测试服务名");
                string name = Console.ReadLine();

                int t = s.ToInt32();
                string rk = "";
                if (t == 3 || t == 1)
                {
                    Console.WriteLine("请输入RoutingKey");
                    s = Console.ReadLine();
                    rk = s;
                }
                if (n < 0) n = 1;
                DovaResponse<string> res;

                for (int i = 0; i < n; i++)
                {

                    WQMessage msg = new WQMessage();
                    if (t == 3 || t == 1)
                        msg.RoutingKey = rk;
                    msg.TargetID = "18817815180";
                    name = "PushMessageService";
                    msg.Body = "给手机发消息";
                    bool ret = false;
                    //MessageService ms = new MessageService();
                    //ms.RequestMessage("ToWMSReceiptService", msg);
                    //DovaResponse<object> res = MessageClient.RequestMessage<DovaResponse<object>>("ToWMSReceiptService", msg);
                    long time = DateTime.Now.Ticks;
                    if (t == 1)
                    {
                        res = MessageClient.RequestMessage<DovaResponse<string>>(name, msg);
                        Console.WriteLine("用时:" + ((DateTime.Now.Ticks - time) / 10000000.000) + ";返回消息:" + res.Message);
                    }
                    else if (t == 2)
                    {
                        ret = MessageClient.AsyncRequestMessage(name, msg);
                        Console.WriteLine(ret.ToString());
                    }
                    else if (t == 3)
                    {
                        ret = MessageClient.AsyncRequestMessage(name, msg);
                        Console.WriteLine(ret.ToString());
                    }
                }
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Log.Write(ex.ToString());
            }
            Console.ReadLine();
        }

        delegate void PointDelegate();
        static string eccMonitorKey = "6094c672bfd59334b2242958d3b29620";

        static void MultiThreadPoint()
        {
            for (int i = 0; i < 100000; i++)
            {
                PointDelegate pd = new PointDelegate(DoPoint);
                pd.Invoke();
            }
        }

        static void DoPoint()
        {
            // ECCMonitorClient.Report(eccMonitorKey); 
        }

        static void TestMQService()
        {

            //int tt = MsgQConfig.HBTimeout;
            Console.WriteLine("请输入测试日志次数");
            string s = Console.ReadLine();
            int n = s.ToInt32();
            for (int i = 0; i < n; i++)
                Log.Write(LogAction.Write, "Program", "", "Main", 0, "测试日志");
            string path = LogConfig.BakPath;
            try
            {
                Console.WriteLine("请输入测试的次数和请求");
                s = Console.ReadLine();

                n = s.ToInt32();

                //for(int i=0;i<n;i++)
                //    Log.Info(string.Format(Config.LogFormat,"test", "tt", "Login", 0, "sss:"));
                //return;


                Console.WriteLine("请输入测试类型: 1 同步/路由  2 异步 3 异步路由");
                s = Console.ReadLine();

                Console.WriteLine("请输入测试服务名");
                string name = Console.ReadLine();

                int t = s.ToInt32();
                string rk = "";
                if (t == 3 || t == 1)
                {
                    Console.WriteLine("请输入RoutingKey");
                    s = Console.ReadLine();
                    rk = s;
                }
                if (n < 0) n = 1;
                object res;

                MessageService ms = new MessageService();
                for (int i = 0; i < n; i++)
                {

                    WQMessage msg = new WQMessage();
                    if (t == 3 || t == 1)
                        msg.RoutingKey = rk;
                    msg.Body = "111";
                    bool ret = false;
                    //MessageService ms = new MessageService();
                    //ms.RequestMessage("ToWMSReceiptService", msg);
                    //DovaResponse<object> res = MessageClient.RequestMessage<DovaResponse<object>>("ToWMSReceiptService", msg);
                    long time = DateTime.Now.Ticks;
                    if (t == 1)
                    {
                        res = ms.RequestMessage(name, msg);
                        Console.WriteLine("用时:" + ((DateTime.Now.Ticks - time) / 10000000.000) + ";返回消息:" + res.ToString());
                    }
                    else if (t == 2)
                    {
                        ret = ms.AsyncRequestMessage(name, msg);
                        Console.WriteLine(ret.ToString());
                    }
                    else if (t == 3)
                    {
                        ret = ms.AsyncRequestMessage(name, msg);
                        Console.WriteLine(ret.ToString());
                    }
                }
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Log.Write(ex.ToString());
            }
            Console.ReadLine();

        }

        //static Socket scproducer = null;
        // static Socket scconsumer = null;

        static void TestSocketConsumerMQService(object ti)
        {
            WQMessage ms = new WQMessage();
            ms.UserToken = "消息消费者" + ti;
            ms.ServiceName = "LoginService";
            ms.Body = "第一次连接注册消息者";
            byte[] bs = null;
            byte[] resbs = null;
            string res = "";
            res=MessageClient.Send<string>(ms);

            Console.WriteLine("第一次连接注册消息消费者:" + res);

            while (true)
            {
                try
                {
                    WQMessage rec = MessageClient.Receive<WQMessage>(ms.AppName, ms.Format);
                    if (null != rec)
                    { 
                        Console.WriteLine("消息消费者收到消息:" + rec);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("出错:" + e.Message);
                }
            }

            return;



            Socket scconsumer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            scconsumer.Connect(new IPEndPoint(IPAddress.Parse("114.215.197.135"), 19000));

            bs = Encoding.UTF8.GetBytes(SerializeHelper.SerializeToJSon(ms));
            TcpHelper.SendVar(scconsumer, bs, 4);
            resbs = null;// TcpHelper.ReceiveVar(scconsumer, 4);
            resbs = TcpHelper.ReceiveVar(scconsumer, 4);
            if (null != resbs)
            {
                res = Encoding.UTF8.GetString(resbs);
                Log.Info(res);
                Console.WriteLine("第一次连接注册消息消费者:" + res);
            }
            while (true)
            {
                try
                {
                    resbs = TcpHelper.ReceiveVar(scconsumer, 4);
                    if (null != resbs)
                    {
                        res = Encoding.UTF8.GetString(resbs);
                        Log.Info("消息消费者收到消息:" + res);
                        Console.WriteLine("消息消费者收到消息:" + res);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("出错:" + e.Message);
                }
            }
        }

        static void TestSocketProcuderMQService(object ti)
        {

            WQMessage ms = new WQMessage();
            ms.Async = false; //异步
            ms.UserToken = "消息生产者"+ti;//取一个唯一标志自己
            ms.ServiceName = "LoginService";//推送消息
            ms.Body = "json串";//json 对象串
            string reg = "";
            reg=MessageClient.Send<string>(ms);
             Console.WriteLine("成功发送且返回了刚才发送的消息:" + reg); 

            byte[] bs = null;
            byte[] recd = null;
            int i = 0;

            while (true)
            {
                //scconsumer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //scconsumer.Connect(new IPEndPoint(IPAddress.Parse("114.215.197.135"), 19000));
                i++;
                ms = new WQMessage();
                ms.Async = true;
                ms.UserToken = "消息生产者" + ti;
                ms.TargetID = "18817815180";
                //ms.TargetID = "A0000040A2153E"; 
                ms.ServiceName = "PushMessageService";
                ms.Body = "每隔次发送给消费者的消息,编号:" + (++i);
                ms.Message = "i";
                long t = DateTime.Now.Ticks;
                //MessageClient.SendOneWay<string>(ms);
                string rem = MessageClient.Send<string>(ms);
                Console.WriteLine("成功发送且返回了刚才发送的消息耗时:" + (DateTime.Now.Ticks - t) + "  " + rem);
                 
                //Console.WriteLine("回车再次发送");
                //Console.ReadLine();
                 if (i > 1) break;
            }

            return;

            string res = "";
            Socket scconsumer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            scconsumer.Connect(new IPEndPoint(IPAddress.Parse("114.215.197.135"), 19000));

            bs = Encoding.UTF8.GetBytes(SerializeHelper.SerializeToJSon(ms));
            TcpHelper.SendVar(scconsumer, bs, 4);
            recd = null;
            recd = TcpHelper.ReceiveVar(scconsumer, 4);
            if (null != recd)
            {
                res = Encoding.UTF8.GetString(recd);
                Console.WriteLine("成功发送且返回了刚才发送的消息:" + res);
            }
            //Console.WriteLine("请按任意键给消费者发送消息");
            //Console.ReadLine();
            while (true)
            {
                //scconsumer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //scconsumer.Connect(new IPEndPoint(IPAddress.Parse("114.215.197.135"), 19000));
                ms = new WQMessage();
                ms.UserToken = "消息生产者" + ti;
                //ms.TargetID = "消息消费者" + ti;
                ms.TargetID = "A0000040A2153E";

                ms.ServiceName = "PushMessageService";
                ms.Body = "每隔次发送给消费者的消息,编号:" + (++i);
                long t = DateTime.Now.Ticks;
                bs = Encoding.UTF8.GetBytes(SerializeHelper.SerializeToJSon(ms));
                TcpHelper.SendVar(scconsumer, bs, 4);
                Console.WriteLine("消息发送成功:" + ms.TransactionID);
                recd = TcpHelper.ReceiveVar(scconsumer, 4);
                if (null != recd)
                {
                      res = Encoding.UTF8.GetString(recd);
                    Log.Info("成功发送且返回了刚才发送的消息:" + res + " 耗时:" + (DateTime.Now.Ticks - t));
                    Console.WriteLine("成功发送且返回了刚才发送的消息:" + res);
                }
                //Thread.Sleep(100);
                //Console.WriteLine("回车再次发送");
                //Console.ReadLine();
            } 
        }

        static void TestSocktHeartBeat(object ti)
        {
            byte[] bs = BitConverter.GetBytes(-100);
            string res = "";
            Socket scconsumer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            scconsumer.Connect(new IPEndPoint(IPAddress.Parse("192.168.56.1"), 19000));
            TcpHelper.Send(scconsumer, bs);
            byte[] recd = TcpHelper.Receive(scconsumer, 4);
            if (null != recd)
            { 
                Console.WriteLine("成功发送且返回了刚才发送的消息:" + BitConverter.ToInt32(recd,0));
            }
            //Console.WriteLine("请按任意键给消费者发送消息");
            Console.ReadLine();
          
        } 


    }

    public class TestClass
    {
        public TestClass(string name)
        {
            this.name = name;
        }
        public string name = "";
    }

}

 








