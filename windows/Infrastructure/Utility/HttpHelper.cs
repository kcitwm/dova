using System; 
using System.Net;
using System.Text;
using System.IO;
using System.Collections.Specialized;

namespace Dova.Utility
{
   public  class HttpHelper
    { 
      
       public static  string Request(string url,string datas,string method,string contentType,string coding)
       {
           try
           {
               HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest;
               req.Proxy = null;
               req.Method = method;
               req.ContentType = contentType;
               Encoding enc = Encoding.GetEncoding(coding);
               if (method.ToUpper() == "POST" && datas != "")
               {
                   byte[] data = enc.GetBytes(datas);
                   req.ContentLength = data.Length;
                   using (Stream sr = req.GetRequestStream())
                   {
                       sr.Write(data, 0, data.Length);
                   }
               }
               HttpWebResponse res = req.GetResponse() as HttpWebResponse;
               using (Stream sr = res.GetResponseStream())
               {
                   using (StreamReader reader = new StreamReader(sr, enc))
                   {
                       return reader.ReadToEnd();
                   }
               }
           }
           catch(Exception e)
           {
               Log.Error(e);
           }
           return string.Empty;
       }
       public static string Request(string url, string datas, string method, string contentType, string coding, NameValueCollection headers)
       {
           HttpWebRequest req = WebRequest.Create(url) as HttpWebRequest; 
           req.Proxy = null;
           req.Method = method;
           req.ContentType = contentType;
           Encoding enc = Encoding.GetEncoding(coding);
           foreach (string name in headers)
           {
               if (name.ToUpper() == "USER-AGENT")
                   req.UserAgent = headers[name];
               else if (name.ToUpper() == "REFERRER")
                   req.Referer = headers[name];
               else
                   req.Headers.Add(name, headers[name]);
           }
           if (method.ToUpper() == "POST" && datas != "")
           {
               byte[] data = enc.GetBytes(datas);
               req.ContentLength = data.Length;
               using (Stream sr = req.GetRequestStream())
               { 
                   sr.Write(data, 0, data.Length);
               }
           }
           HttpWebResponse res = req.GetResponse() as HttpWebResponse;
           using (Stream sr = res.GetResponseStream())
           {
               using (StreamReader reader = new StreamReader(sr, enc))
               {  
                   return reader.ReadToEnd();
               }
           }
       }
    }

   public class HttpProxy: IDisposable
   {
       string url = "";

       public HttpProxy(string url)
       {
           this.url = url;
       }

       public string Request(string datas, string method, string contentType, string coding)
       {
           return HttpHelper.Request(url, datas, method, contentType, coding);
       }


       public void Dispose()
       {
           
       }
   }


}
