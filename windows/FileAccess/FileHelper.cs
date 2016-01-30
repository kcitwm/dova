using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Dova.Utility;

namespace Dova.Files
{
    public class FileHelper
    {
        static ConsistentHash ch = null;
        static List<string> names = new List<string>(100);


        static Dictionary<string, string> paths = new Dictionary<string, string>(100);

        static FileHelper()
        {
            //factories = new Dictionary<string, ServiceFactory<T>>();  
            string root = Configs.FileStorageRoot;
            if (!(root.EndsWith("\\") || root.EndsWith("/")))
                root = root + "\\";
            string path = ""; 
            try
            {
                for (int i = 1; i <= 100; i++)
                {
                    path = root + i + "\\";
                    paths.Add("Files"+i,path);
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                }
            }
            catch (Exception e)
            {
                Log.Error("FileHelper:" + e.Message);
            }
            ch = new ConsistentHash(paths.Keys.ToList(), 300);
        }

        public static string SaveFile(string name, byte[] bs)
        {
            int rtn = -1;
            try
            {
                string path = paths[ch.GetPrimary(name)]  + name;
                Log.Write("保存文件路径:" + path);
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Delete | FileShare.ReadWrite, 8192, FileOptions.WriteThrough | FileOptions.Asynchronous);
                IAsyncResult ir = fs.BeginWrite(bs, 0, bs.Length, WriteCallback, fs);
            }
            catch (Exception e)
            {
                Log.Error("FileHelper.SaveFile:" + e.Message);
            }
            return name;
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

        public static byte[] GetFile(string name)
        {
            try
            {
                string path = paths[ch.GetPrimary(name)] + name;
                Log.Write("读取文件路径:" + path);
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 8192))
                {
                    byte[] bs = new byte[fs.Length];
                    fs.Read(bs, 0, bs.Length);
                    return bs;
                }
            }
            catch (Exception e)
            {
                Log.Error("FileHelper.GetFile:" + e.Message);
                return new byte[1]{0}; ;
            }
        }

    }
}
