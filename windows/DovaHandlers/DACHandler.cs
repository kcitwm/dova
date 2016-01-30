using Dova;
using Dova.MessageQueue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dova.Data;
using Newtonsoft.Json;
using Dova.Utility;

namespace Dova.Handlers
{
    public class DACHandler : MessageHandler
    {
        static string className = "DACHandler";
        DACService dac = new DACService();
        public override bool ExecuteMQ(string serviceName, WQMessage msg)
        {
            Log.Info("DACHandler.ExecuteMQ:" + serviceName);
            string methodName = msg.MethodName;
            string paras = msg.Body.ToString();
            long t = DateTime.Now.Ticks;
            try
            {
                switch (methodName)
                {
                    case "ExecutePagedDataList":
                        dac.ExecutePagedDataList(JsonConvert.DeserializeObject<PagedRecordParameter>(paras));
                        break;
                    case "ExecuteDataList":
                        dac.ExecuteDataList(JsonConvert.DeserializeObject<WrapedDatabaseParameter>(paras));
                        break;
                    case "ExecuteNonQuery":
                        dac.ExecuteNonQuery(JsonConvert.DeserializeObject<WrapedDatabaseParameter>(paras));
                        break;
                    case "ExecuteScalar":
                        dac.ExecuteScalar(JsonConvert.DeserializeObject<WrapedDatabaseParameter>(paras));
                        break;
                    case "ExecuteDataTable":
                        dac.ExecuteDataTable(JsonConvert.DeserializeObject<WrapedDatabaseParameter>(paras));
                        break;
                    case "ExecuteDataSet":
                        dac.ExecuteDataSet(JsonConvert.DeserializeObject<WrapedDatabaseParameter>(paras)).GetXml();
                        break;
                    case "ExecutePagedDataSet":
                        dac.ExecutePagedDataSet(JsonConvert.DeserializeObject<PagedRecordParameter>(paras)).GetXml();
                        break;
                    case "Login":
                        dac.Login(JsonConvert.DeserializeObject<LoginReq>(paras));
                        break;
                    case "Logout":
                        dac.Logout(paras);
                        break;
                    case "CheckLogin":
                        dac.CheckLogin(paras);
                        break;
                    case "Regist":
                        dac.Regist(JsonConvert.DeserializeObject<LoginReq>(paras));
                        break;
                }
                Log.Write(LogAction.Info, className, "ExecuteMQ", "end", "end", DateTime.Now.Ticks - t, "Socket_DataReceived:接口方法:" + serviceName + "." + methodName + "处理完成");
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "ExecuteMQ", "end", "end", DateTime.Now.Ticks - t, "Socket_DataReceived:接口方法:" + serviceName + "." + methodName + e.Message);
                return false;
            }
            return true;
        }
        public override object ExecuteMessageRequest(string serviceName, WQMessage msg)
        {
            string methodName = msg.MethodName;
            string paras = msg.Body.ToString();
            long t = DateTime.Now.Ticks;
            try
            {
                switch (methodName)
                {
                    case "ExecutePagedDataList":
                        return dac.ExecutePagedDataList(JsonConvert.DeserializeObject<PagedRecordParameter>(paras));
                    case "ExecuteDataList":
                        return dac.ExecuteDataList(JsonConvert.DeserializeObject<WrapedDatabaseParameter>(paras));
                    case "ExecuteNonQuery":
                        return dac.ExecuteNonQuery(JsonConvert.DeserializeObject<WrapedDatabaseParameter>(paras));
                    case "ExecuteScalar":
                        return dac.ExecuteScalar(JsonConvert.DeserializeObject<WrapedDatabaseParameter>(paras));
                    case "ExecuteDataTable":
                        return dac.ExecuteDataTable(JsonConvert.DeserializeObject<WrapedDatabaseParameter>(paras));
                    case "ExecuteDataSet":
                        return dac.ExecuteDataSet(JsonConvert.DeserializeObject<WrapedDatabaseParameter>(paras)).GetXml();
                    case "ExecutePagedDataSet":
                        return dac.ExecutePagedDataSet(JsonConvert.DeserializeObject<PagedRecordParameter>(paras)).GetXml();
                    case "Login":
                        return dac.Login(JsonConvert.DeserializeObject<LoginReq>(paras));
                    case "Logout":
                        return dac.Logout(paras);
                    case "CheckLogin":
                        return dac.CheckLogin(paras);
                    case "Regist":
                        return dac.Regist(JsonConvert.DeserializeObject<LoginReq>(paras));
                }
                Log.Write(LogAction.Info, className, "ExecuteMessageRequest", "end", "end", DateTime.Now.Ticks - t, "Socket_DataReceived:接口方法:" + serviceName + "." + methodName + "处理完成");
            } 
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "ExecuteMessageRequest", "end", "end", DateTime.Now.Ticks - t, "Socket_DataReceived:接口方法:" + serviceName + "." + methodName + e.Message);
            }
            return "";
        }

    }
}
