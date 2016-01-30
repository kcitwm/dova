using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Collections.Specialized;
using Dova.Utility;
namespace Dova.Infrastructure.Utility
{
    public class TcpHelper
    {

        public static void BeginSend(Socket socket, byte[] buffer, StateObject so)
        {
            so.WorkSocket = socket;
            socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(SendCallBack), so);
        }


        public static void SendCallBack(IAsyncResult ar)
        {
            StateObject so = (StateObject)ar.AsyncState;
            Socket s = so.WorkSocket;
            int n = s.EndSend(ar);
            Log.Info("数据发送成功,字节长度:" + n);
        }

        public static void BeginReceive(Socket socket, StateObject so)
        {
            so.WorkSocket = socket;
            socket.BeginReceive(so.Buffer, 0, StateObject.BUFFER_SIZE, SocketFlags.None, new AsyncCallback(ReceiveCallBack), so);
        }

        public static void Receive(Socket socket, StateObject so)
        {
            try
            {
                int read = socket.Receive(so.Buffer, 0, StateObject.BUFFER_SIZE, SocketFlags.None);
                if (read > 0)
                {
                    string data = so.Encode.GetString(so.Buffer, 0, read);
                    Log.Info("数据接收成功:" + data);
                    so.SB.Append(data);
                    if (!string.IsNullOrEmpty(data) && !data.EndsWith(so.ETX))
                        socket.Receive(so.Buffer, 0, StateObject.BUFFER_SIZE, SocketFlags.None);
                    return;
                }
                Log.Info("数据接收字节长度:" + read);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }


        public static void ReceiveCallBack(IAsyncResult ar)
        {
            try
            {
                StateObject so = (StateObject)ar.AsyncState;
                Socket s = so.WorkSocket;
                int read = s.EndReceive(ar);
                if (read > 0)
                {
                    string data = so.Encode.GetString(so.Buffer, 0, read);
                    Log.Info("数据接收成功:" + data);
                    so.SB.Append(data);
                    if (!string.IsNullOrEmpty(data) && !data.EndsWith(so.ETX))
                        s.BeginReceive(so.Buffer, 0, StateObject.BUFFER_SIZE, 0, new AsyncCallback(ReceiveCallBack), so);
                    else
                    {
                        if (so.SB.Length > 1)
                        {
                            so.SB.Length = 0;
                        }
                    }
                    return;
                }
                Log.Info("数据接收字节长度:" + read);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }

        }


        public static int Send(Socket s, byte[] data)
        {
            int total = 0;
            int size = data.Length;
            Log.Info("准备发送数据长度:data.Length:" + size);
            int dataLeft = size;
            int sent;
            while (total < size)
            {
                sent = s.Send(data, total, dataLeft, SocketFlags.None);
                total += sent;
                dataLeft -= sent;
            }
            Log.Info("已经发送数据长度:data.Length:" + total);
            return total;
        }

        public static string Receive(Socket s, Encoding encode)
        {
            Log.Info("Begin Receive");
            int piceSize = 500 * 1024;
            byte[] pice = new byte[piceSize];
            int recv = s.Receive(pice, piceSize, 0);
            StringBuilder sb = new StringBuilder();
            sb.Append(encode.GetString(pice, 0, recv));
            //while (recv > 0)
            //{
            //    pice = new byte[piceSize];
            //    recv = s.Receive(pice, piceSize, 0);
            //    if (recv > 0)
            //        sb.Append(encode.GetString(pice, 0, recv));
            //    else
            //        break;
            //}
            Log.Info("End Receive");
            return sb.ToString();

        }

        public string ReceiveAll(Socket s, Encoding encode)
        {
            Log.Write("Begin Receive");
            int piceSize = 100 * 1024;
            byte[] pice = new byte[piceSize];
            int recv = s.Receive(pice, piceSize, 0);
            StringBuilder sb = new StringBuilder();
            sb.Append(encode.GetString(pice, 0, recv));
            while (recv > 0)
            {
                pice = new byte[piceSize];
                recv = s.Receive(pice, piceSize, 0);
                if (recv > 0)
                    sb.Append(encode.GetString(pice, 0, recv));
                else
                    break;
            }
            Log.Write("End Receive");
            return sb.ToString();

        }

        public static  byte[] Receive(Socket s, int size)
        {
            int total = 0;
            int dataLeft = size;
            byte[] data = new byte[size];
            int recv;
            while (total < size)
            {
                recv = s.Receive(data, total, dataLeft, 0);
                if (recv > 0)
                {
                    total += recv;
                    dataLeft -= recv;
                }
                else
                    break;
            }
            return data;

        }

        public static int SendVar(Socket s, byte[] data)
        {
            return SendVar(s, data, 8);
        }

        public static byte[] SendVar(IPEndPoint ipe, byte[] data, int lenHeader)
        {
            Socket s = null; 
            try
            {
                s = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                s.Connect(ipe);
                SendVar(s, data, lenHeader);
                return ReceiveVar(s, lenHeader);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
            finally
            {
                s.Close();
            }
            return null;
        }

        public static int SendVar(Socket s, byte[] data, int lenHeader)
        {
            int total = 0;
            int size = data.Length;
            int sent;

            byte[] dataSize = new byte[lenHeader];
            dataSize = BitConverter.GetBytes(size);

            byte[] all = new byte[dataSize.Length + data.Length];
            Buffer.BlockCopy(dataSize, 0, all, 0, dataSize.Length);
            Buffer.BlockCopy(data, 0, all, dataSize.Length, data.Length); 
            int len=all.Length;
            int dataLeft = len;
            //sent = s.Send(dataSize);
            Log.Info("Socket发送数据长度信息:" + dataSize.Length);
            while (total < len)
            {
                sent = s.Send(all, total, dataLeft, SocketFlags.None);
                total += sent;
                dataLeft -= sent;
            }
            Log.Info("Socket发送数据长度total:" + total);
            return total;
        }

        public static byte[] ReceiveVar(Socket s)
        {
            return ReceiveVar(s, 4);
        }

        public static byte[] ReceiveVar(Socket s, int lenHeader)
        {
            int total = 0;
            int recv;
            byte[] dataSize = new byte[lenHeader];
            recv = s.Receive(dataSize, 0, lenHeader, 0);
            int size = BitConverter.ToInt32(dataSize, 0);
            int dataLeft = size;
            Log.Info("收到数据长度信息:" + size);
            byte[] data = new byte[size];
            while (total < size)
            {
                recv = s.Receive(data, total, dataLeft, 0);
                if (recv > 0)
                {
                    total += recv;
                    dataLeft -= recv;
                }
                else break;
            }
            Log.Info("收到数据长度:" + total);
            return data;
        }


        public static byte[] ReceiveVar2(Socket s, int lenHeader)
        {
            int total = 0;
            int recv;
            byte[] dataSize = new byte[lenHeader];
            recv = s.Receive(dataSize, 0, lenHeader, 0);
            int size = (int)Encoding.UTF8.GetString(dataSize).Trim().ToInt32();
            int dataLeft = size;
            Log.Info("收到数据长度信息:" + size);
            byte[] data = new byte[size];
            while (total < size)
            {
                recv = s.Receive(data, total, dataLeft, 0);
                if (recv > 0)
                {
                    total += recv;
                    dataLeft -= recv;
                }
                else break;
            }
            Log.Info("收到数据长度:" + total);
            return data;
        }

        public static string ReceiveVar2(Socket s, Encoding encoding, int lenHeader)
        {
            return encoding.GetString(ReceiveVar2(s, lenHeader));
        }

        public static string ReceiveVar(Socket s, Encoding encoding, int lenHeader)
        {
            return encoding.GetString(ReceiveVar(s, lenHeader));
        }

    }

    public class StateObject
    {
        public Socket WorkSocket = null;
        public const int BUFFER_SIZE = 512;
        public byte[] Buffer = new byte[BUFFER_SIZE];
        public StringBuilder SB = new StringBuilder();
        public Encoding Encode = Encoding.UTF8;
        public string ETX = "\u0003";
    }

}
