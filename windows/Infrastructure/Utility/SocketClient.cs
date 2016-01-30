using Dova.Services;
using Dova.Utility;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Dova.Infrastructure.Utility
{
    /// <summary>
    /// 如果是长连接,在关闭程序或者异常的时候注意调用Dispose方法.
    /// 如果调用异步方法,在DealData里面的控件要异步操作.
    /// </summary>
    public class SocketClient : IDisposable
    {
        static string className = "SocketClient";
        Socket socket = null;
        Encoding encode = Encoding.ASCII;
        StateObject so = new StateObject();
        public SocketClient(IPEndPoint ipe)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipe);
        }

        public SocketClient(IPEndPoint ipe, Encoding encode)
            : this(ipe)
        {
            this.encode = encode;
        }


        public SocketClient(string ip, int port)
        {
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(ip), port);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipe);
        }

        public SocketClient(string ip, int port, Encoding encode)
            : this(ip, port)
        {
            this.encode = encode;
        }


        public SocketClient(IPEndPoint ipe, Encoding encode, StateObject so)
            : this(ipe, encode)
        {
            this.so = so;
            this.encode = encode;
        }

        static string sentKey = "Sent";
        static string receiveKey = "Received";

        protected EventHandlerList events = new EventHandlerList();
        public event EventHandler Sent
        {
            add
            {
                events.AddHandler(sentKey, value);
            }
            remove
            {
                events.RemoveHandler(sentKey, value);
            }
        }

        public event EventHandler Received
        {
            add
            {
                events.AddHandler(receiveKey, value);
            }
            remove
            {
                events.RemoveHandler(receiveKey, value);
            }
        }


        protected void OnSent(SocketEventArgs args)
        {
            EventHandler handler = events[sentKey] as EventHandler;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        protected void OnReceive(SocketEventArgs args)
        {
            EventHandler handler = events[receiveKey] as EventHandler;
            if (null != handler)
            {
                handler(this, args);
            }
        }

        public  void BeginSend(byte[] buffer)
        {
            so.WorkSocket = socket;
            Log.Info("开始异步发送数据,字节长度:" + buffer.Length);
            socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallBack), so);
        }

        public  void Send(byte[] buffer)
        {
            int n = TcpHelper.Send(socket, buffer);
        }

        public void SendCallBack(IAsyncResult ar)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                Log.Info("开始异步发送数据");
                StateObject so = (StateObject)ar.AsyncState;
                Socket s = so.WorkSocket;
                int n = s.EndSend(ar);
                Log.Write(LogAction.Info, Config.AppName, className, "SendCallBack", DateTime.Now.Ticks - t, "异步发送数据成功,数据长度:" + n);
                SocketEventArgs se = new SocketEventArgs();
                se.Sent = n;
                OnSent(se);
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Info, Config.AppName, className, "SendCallBack", DateTime.Now.Ticks - t, "异步发送数据失败:" + e.ToString());
            }

        }


        public IAsyncResult BeginReceive()
        {
            Log.Info("开始异步接收数据");
            so.WorkSocket = socket;
            IAsyncResult ar = socket.BeginReceive(so.Buffer, 0, StateObject.BUFFER_SIZE, SocketFlags.None, new AsyncCallback(ReceiveCallBack), so);
            return ar; 
        }


        public string Receive()
        {
            long t = DateTime.Now.Ticks;
            try
            {
                Log.Info("开始同步接收数据"); 
                int read = socket.Receive(so.Buffer, 0, StateObject.BUFFER_SIZE, SocketFlags.None);
                if (read > 0)
                {
                    string data = encode.GetString(so.Buffer, 0, read);
                    Log.Write(LogAction.Info, Config.AppName, className, "Receive", DateTime.Now.Ticks - t, "同步接收到数据:" + data);
                    so.SB.Append(data);
                    if (!string.IsNullOrEmpty(data) && !data.EndsWith(so.ETX))
                        socket.Receive(so.Buffer, 0, StateObject.BUFFER_SIZE, SocketFlags.None);
                    if (so.SB.Length > 1)
                    {
                        so.SB.Length = 0;
                        SocketEventArgs se = new SocketEventArgs();
                        se.Received = read;
                        se.Data = data;
                        OnReceive(se);
                        return data;
                    }
                }
                Log.Info("同步数据接收字节长度:" + read);
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, Config.AppName, className, "Receive", DateTime.Now.Ticks - t, "同步接收数据异步:" + e.ToString());
                throw;
            }
            return "";
        }


        public void ReceiveCallBack(IAsyncResult ar)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                StateObject so = (StateObject)ar.AsyncState;
                Socket s = so.WorkSocket;
                int read = s.EndReceive(ar);
                if (read > 0)
                {
                    string data = encode.GetString(so.Buffer, 0, read);
                    Log.Write(LogAction.Info, Config.AppName, className, "ReceiveCallBack", DateTime.Now.Ticks - t, "异步接收到数据:" + data); 
                    so.SB.Append(data);
                    if (!string.IsNullOrEmpty(data) && !data.EndsWith(so.ETX))
                        s.BeginReceive(so.Buffer, 0, StateObject.BUFFER_SIZE, 0, new AsyncCallback(ReceiveCallBack), so);
                    else
                    {
                        if (so.SB.Length > 1)
                        {
                            so.SB.Length = 0;
                            SocketEventArgs se = new SocketEventArgs();
                            se.Received = read;
                            se.Data = data;
                            OnReceive(se);
                        }
                    }
                    return;
                }
                Log.Info("异步数据接收字节长度:" + read);
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, Config.AppName, className, "ReceiveCallBack", DateTime.Now.Ticks - t, "异步接收数据异常:" + e.ToString()); 
            }

        }


        public void Dispose()
        {
            try
            {
                if (null != socket)
                {
                    socket.Disconnect(true);
                    socket.Close();
                }
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }



}
