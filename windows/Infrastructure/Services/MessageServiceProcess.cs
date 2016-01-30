using System;

using Dova.Utility;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.ComponentModel;
using Dova.Interfaces;
using System.Net.Sockets;
using System.Net;
using Dova.Infrastructure.Utility;
using System.Text;

namespace Dova.Services
{
    public class MessageServiceProcess<T> : ServiceProcess<T>, IMessageService, IDisposable
    {
        DateTime now = DateTime.Now;
        static Encoding encode = Dova.Config.Encode;
        public MessageServiceProcess(ServiceConfigItem config)
            : base(config)
        {
            this._config = config;
            if (null == consumer)
            {
                try
                {
                    consumer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    consumer.Connect(new IPEndPoint(IPAddress.Parse(config.Url), config.EndPoint.ToInt32()));
                }
                catch (Exception e)
                {
                    Log.Error("MessageServiceProcess:" + e.Message);
                }
            }
        }
        Socket consumer = null;

        public void ReConnect()
        {
            if (null == consumer)
            {
                try
                {
                    consumer.Close();
                }
                catch { }
                try
                {
                    consumer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    consumer.Connect(new IPEndPoint(IPAddress.Parse(_config.Url), _config.EndPoint.ToInt32()));
                }
                catch (Exception e)
                {
                    Log.Error("MessageServiceProcess.ReConnect:" + e.Message);
                }
            }
        }


        protected override T CreateInstance()
        {
            Object o = this;
            return (T)(o);
        }

        public object Request(string serviceName, object msg)
        {
            byte[] req = SerializeHelper.SerializeToBytes(msg);
            int sent = TcpHelper.SendVar(consumer, req, 4);
            if (sent > 0)
            {
                byte[] res = TcpHelper.ReceiveVar(consumer, 4);
                return SerializeHelper.DeSerializeFromBytes(res);
            }
            return null;
        }

        public object RequestMessage(string serviceName, WQMessage msg)
        {
            byte[] req = SerializeHelper.SerializeToBytes(msg.Format, msg);
            int sent = TcpHelper.SendVar(consumer, req, 4);
            if (sent > 0)
            {
                byte[] res = TcpHelper.ReceiveVar(consumer, 4);
                return SerializeHelper.DeSerializeFromBytes(msg.Format, res);
            }
            return null;
        }

        public bool AsyncRequest(string serviceName, object msg)
        {
            byte[] req = SerializeHelper.SerializeToBytes(msg);
            int sent = TcpHelper.SendVar(consumer, req, 4);
            return sent > 0;
        }

        public bool AsyncRequestMessage(string serviceName, WQMessage msg)
        {
            throw new NotImplementedException();
        }

        public bool AsyncRequestMessages(string serviceName, WQMessage[] msgs)
        {
            bool status = false;
            foreach (WQMessage msg in msgs)
            {
                status = AsyncRequestMessage(serviceName, msg);
            }
            return status;
        }

        public DovaResponse RequestMessage(WQMessage msg)
        {
            byte[] req = SerializeHelper.SerializeToBytes(msg.Format, msg);
            int sent = TcpHelper.SendVar(consumer, req, 4);
            if (sent > 0)
            {
                byte[] res = TcpHelper.ReceiveVar(consumer, 4);
                return SerializeHelper.DeSerializeFromBytes<DovaResponse>(res);
            }
            return null;
        }

        public bool AsyncRequestMessage(WQMessage msg)
        {
            byte[] req = SerializeHelper.SerializeToBytes(msg.Format, msg);
            int sent = TcpHelper.SendVar(consumer, req, 4);
            return sent > 0;
        }

        public string Request(string msg)
        {
            byte[] req = encode.GetBytes(msg);
            int sent = TcpHelper.SendVar(consumer, req, 4);
            if (sent > 0)
                return encode.GetString(TcpHelper.ReceiveVar(consumer, 4));
            return string.Empty;
        }

        int n = 0;
        public object Send(WQMessage msg)
        {
            try
            {
                byte[] req = SerializeHelper.SerializeToBytes(msg.Format, msg);
                int sent = TcpHelper.SendVar(consumer, req, 4);
                if (sent > 0)
                {
                    byte[] res = TcpHelper.ReceiveVar(consumer, 4);
                    return SerializeHelper.DeSerializeFromBytes(msg.Format, res);
                }
            }
            catch (SocketException e)
            {
                n++;
                if (n > 1)
                {
                    Log.Error("发送1次重新连接失败");
                    return null;
                }
                if (n > 1)
                    Thread.Sleep(1000);
                Log.Error("出现错误，准备重连接.MessageServiceProcess.Send:" + e.Message);
                ReConnect();
                Send(msg);  
            }
            return null;
        }

        public void SendOneWay(WQMessage msg)
        {
            int n = 0;
            try
            {
                byte[] req = SerializeHelper.SerializeToBytes(msg.Format, msg);
                int sent = TcpHelper.SendVar(consumer, req, 4); 
            }
            catch (SocketException e)
            {
                n++;
                if (n > 1)
                {
                    Log.Error("发送1次重新连接失败"); 
                }
                if (n > 1)
                    Thread.Sleep(1000);
                Log.Error("出现错误，准备重连接.MessageServiceProcess.Send:" + e.Message);
                ReConnect();
                SendOneWay(msg);
            } 
        }


        public object Receive(string format)
        {
             int n = 0;
            try
            {
            byte[] res = TcpHelper.ReceiveVar(consumer, 4);
            return SerializeHelper.DeSerializeFromBytes(format, res);
            }
            catch (SocketException e)
            {
                n++;
                if (n > 1)
                {
                    Log.Error("接收1次重新连接失败");
                    return null;
                }
                if (n > 1)
                    Thread.Sleep(1000);
                Log.Error("出现错误，准备重连接.MessageServiceProcess.Receive:" + e.Message);
                ReConnect();
                Receive(format);
            }
            return null;
        }

        public DovaResponse Test(WQMessage msg)
        {
            return null;
        }
    }
}
