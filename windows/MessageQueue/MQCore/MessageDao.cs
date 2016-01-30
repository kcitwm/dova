using Dova.Data;
using Dova.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Dova.MessageQueue
{
    public class MessageDao
    {
        static string className = "MessageDao";
        static DAC dac = new DAC(MQConfigs.MQConnectionName);

        public static int Save(string serviceName, WQMessage msg, int status)
        {
            int rtn = -1;
            long t = DateTime.Now.Ticks;
            try
            {
                DbParameter[] pars = new DbParameter[4];
                DbParameter p = null;
                pars[0] = dac.CreateParameter("TransactionID", msg.TransactionID);
                pars[1] = dac.CreateParameter("ServiceName", serviceName);
                pars[2] = dac.CreateParameter("Status", status);
                pars[3] = dac.CreateParameter("MessageData", SerializeHelper.SerializeToBytes(msg), System.Data.DbType.Binary);
                rtn = dac.ExecuteNonQuery("SaveDovaMessageSP", System.Data.CommandType.StoredProcedure, pars);
                Log.Write(LogAction.Dao, className, "Save",serviceName, DateTime.Now.Ticks - t, "保存消息:" + msg.TransactionID + ";KeyID:" + msg.KeyID + ";status:" + status);
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "Save",serviceName, DateTime.Now.Ticks - t, "msg.TransactionID:" + msg.TransactionID + ";Error:" + e.Message);
            }
            return rtn;
        }


        public static int Save(string serviceName, WQMessage msg, int status, long appIndex, long serviceIndex)
        {
            int rtn = -1;
            long t = DateTime.Now.Ticks;
            try
            {
                DbParameter[] pars = new DbParameter[6];
                DbParameter p = null;
                pars[0] = dac.CreateParameter("TransactionID", msg.TransactionID);
                pars[1] = dac.CreateParameter("ServiceName", serviceName);
                pars[2] = dac.CreateParameter("Status", status);
                pars[3] = dac.CreateParameter("AppIndex", appIndex);
                pars[4] = dac.CreateParameter("ServiceIndex", serviceIndex);
                pars[5] = dac.CreateParameter("MessageData", SerializeHelper.SerializeToBytes(msg), System.Data.DbType.Binary);
                rtn = dac.ExecuteNonQuery("SaveDovaMessageSP", System.Data.CommandType.StoredProcedure, pars);
                Log.Write(LogAction.Dao, className, "Save", serviceName, DateTime.Now.Ticks - t, "保存消息:" + msg.TransactionID + ";KeyID:" + msg.KeyID + ";status:" + status + ";appIndex:" + appIndex + ";serviceIndex:" + serviceIndex);
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "Save", serviceName, DateTime.Now.Ticks - t, "msg.TransactionID:" + msg.TransactionID + ";Error:" + e.Message);
            }
            return rtn;
        }


        public static int SaveStatus(long transactionID, long dealing, long done)
        {
            int rtn = -1;
            long t = DateTime.Now.Ticks;
            try
            {
                DbParameter[] pars = new DbParameter[3];
                pars[0] = dac.CreateParameter("TransactionID", transactionID);
                pars[1] = dac.CreateParameter("DealingStatus", dealing);
                pars[2] = dac.CreateParameter("DoneStatus", done);
                rtn = dac.ExecuteNonQuery("SaveDovaMessageStatusSP", System.Data.CommandType.StoredProcedure, pars);
                Log.Write(LogAction.Info, className, "SaveStatus","SaveStatus", DateTime.Now.Ticks - t, "保存消息状态:" + transactionID + ";dealing:" + dealing + ";done:" + done);
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "SaveStatus", "SaveStatus", DateTime.Now.Ticks - t, "msg.TransactionID:" + transactionID + ";保存消息状态出错:" + e.Message);
            }
            return rtn;
        }

        public static int SaveDealStatus(long transactionID, long appIndex, long serviceIndex, int status)
        {
            int rtn = -1;
            long t = DateTime.Now.Ticks;
            try
            {
                DbParameter[] pars = new DbParameter[4];
                pars[0] = dac.CreateParameter("TransactionID", transactionID);
                pars[1] = dac.CreateParameter("AppIndex", appIndex);
                pars[2] = dac.CreateParameter("ServiceIndex", serviceIndex);
                pars[3] = dac.CreateParameter("Status", status);
                rtn = dac.ExecuteNonQuery("SaveDovaMessageDealStatusSP", System.Data.CommandType.StoredProcedure, pars);
                Log.Write(LogAction.Info, className, "SaveDealStatus", "SaveStatus", DateTime.Now.Ticks - t, "保存消息处理状态:" + transactionID + ";pidx:" + appIndex + ";serviceIndex:" + serviceIndex + ";Status:" + status + ",返回:" + rtn);
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "SaveDealStatus", "SaveStatus", DateTime.Now.Ticks - t, "msg.TransactionID:" + transactionID + ";保存消息处理状态出错:" + e.Message);
            }
            return rtn;
        }

        public static  IDataReader GetRetryMessages(int pageIndex,int pageSize )
        {  
            long t = DateTime.Now.Ticks;
            try
            {
                DbParameter[] pars = new DbParameter[2];
                pars[0] = dac.CreateParameter("PageIndex", pageIndex);
                pars[1] = dac.CreateParameter("PageSize", pageSize); 
                return dac.ExcuteDataReader("GetRetryDovaMessagesSP", System.Data.CommandType.StoredProcedure, pars); 
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "GetRetryMessages","Error", DateTime.Now.Ticks - t, ";Error:" + e.Message);
                return null;
            } 
        }

    }
}
