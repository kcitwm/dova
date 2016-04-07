using System;
using System.Data;
using System.Data.Common;
using System.ServiceModel;
using System.Data.SqlClient;
using System.Runtime.Serialization;

using Dova.Data;
using Dova.Utility;
using Dova.Interfaces;
using System.Data.OracleClient;
using System.Runtime.Remoting.Messaging;
using System.Diagnostics;
using System.Runtime.Remoting.Channels;
using System.Collections;
using System.Runtime.Remoting.Contexts;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;
using System.Web.Security;

namespace Dova.Data
{
    [ServiceBehavior(MaxItemsInObjectGraph = 0x7fffffff, InstanceContextMode = InstanceContextMode.Single, UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple, AddressFilterMode = AddressFilterMode.Any)]
    public class DACService : MarshalByRefObject, Dova.Data.IDataAccess
    {
        [System.Diagnostics.Conditional("DEBUG")]
        void WriteLine(string msg)
        {
            Console.WriteLine(msg + " ThreadId:" + System.Threading.Thread.CurrentThread.ManagedThreadId);
        }

        static string className = "DACService";
        static string loginCacheSvcName = Config.LoginCacheServiceName;

        public string Test(string msg)
        {
            IContextProperty prop = System.Runtime.Remoting.Contexts.Context.DefaultContext.GetProperty("UserData") as IContextProperty;
            if (null != prop)
                msg += prop.ToString();
            msg += "线程ID:" + System.Threading.Thread.CurrentThread.ManagedThreadId + " ";
            Log.Dac(msg);
            return "收到" + msg;
        }

        public string Test2(CommandType cmdType)
        {
            string msg = "CommandType:" + cmdType.ToString();
            IContextProperty prop = System.Runtime.Remoting.Contexts.Context.DefaultContext.GetProperty("UserData") as IContextProperty;
            if (null != prop)
                msg += prop.ToString();
            msg += "线程ID:" + System.Threading.Thread.CurrentThread.ManagedThreadId + " ";
            Log.Dac(msg);
            return "收到" + msg;
        }

        public virtual string Login(LoginReq req)
        {
            long t = DateTime.Now.Ticks;
            LoginRes res = new LoginRes();
            try
            {
                DAC dac = new DAC(Config.LoginConnectionName);
                res = dac.Login(req);
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Svc, className, "Login", "Login", "Login", DateTime.Now.Ticks - t, "执行失败:userName,loc,machineKey:" + req.UserName + "," + req.Location + "," + req.MachineKey + ";" + e.Message);
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(res);
        }

        public virtual int Regist(LoginReq req)
        {
            long t = DateTime.Now.Ticks;
            int res = -1;
            try
            {
                DAC dac = new DAC(Config.LoginConnectionName);
                res = dac.Regist(req);
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Svc, className, "Regist", "Regist", "Regist", DateTime.Now.Ticks - t, "执行失败:userName,loc,machineKey:" + req.UserName + "," + req.Location + "," + req.MachineKey + ";" + e.Message);
            }
            return res;
        }

        public virtual int Logout(string userName)
        {
            long t = DateTime.Now.Ticks;
            int res = -1;
            try
            {
                DAC dac = new DAC(Config.LoginConnectionName);
                res = dac.Logout(userName);
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Svc, className, "Logout", "Logout", "Logout", DateTime.Now.Ticks - t, "执行失败:userName:" + userName + ";" + e.Message);
            }
            return res;
        }

        public virtual int CheckLogin(string userName)
        {
            long t = DateTime.Now.Ticks;
            int res = -1;
            try
            {
                DAC dac = new DAC(Config.LoginConnectionName);
                res = dac.CheckLogin(userName);
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "CheckLogin", "CheckLogin", "CheckLogin", DateTime.Now.Ticks - t, "执行失败:userName:" + userName + ";" + e.Message);
            }
            Log.Write(LogAction.Info, className, "CheckLogin", "CheckLogin", "CheckLogin", DateTime.Now.Ticks - t, "检查登录:userName:" + userName + ";res:" + res);
            return res;
        }

        public int Authentication(string token)
        {
            int rtn = -1;

            return rtn;
        }

        public virtual IDataReader ExecuteDataReader(string connString, string cmdText, CommandType cmdType, params DatabaseParameter[] parameters)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                DbParameter[] ps = ConvertParameter(parameters);
                DAC dac = new DAC(connString);
                IDataReader dr = dac.ExcuteDataReader(cmdText, cmdType, ps);
                Log.Write(LogAction.Svc, className, "ExcuteDataReader", connString, cmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行成功:" + parameters.ToKeyString());
                return dr;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "ExcuteDataReader", connString, cmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行出错:cmdText:" + cmdText + ";" + parameters.ToKeyString() + ";" + e.ToString());
            }
            return null;
        }

        public int ExecuteNonQuery(string connString, string cmdText, CommandType cmdType, params DatabaseParameter[] parameters)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                DbParameter[] ps = ConvertParameter(parameters);
                DAC dac = new DAC(connString);
                int rtn = dac.ExecuteNonQuery(cmdText, cmdType, ps);
                Log.Write(LogAction.Svc, className, "ExecuteNonQuery", connString, cmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行成功:" + parameters.ToKeyString());
                return rtn;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "ExecuteNonQuery", connString, cmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行出错:cmdText:" + cmdText + ";" + parameters.ToKeyString() + ";" + e.ToString());
            }
            return -1;
        }

        public void ExecuteNonQueryOneWay(string connString, string cmdText, CommandType cmdType, params DatabaseParameter[] parameters)
        {
            ExecuteNonQuery(connString, cmdText, cmdType, parameters);
        }

        public object ExecuteScalar(string connString, string cmdText, CommandType cmdType, params DatabaseParameter[] parameters)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                DbParameter[] ps = ConvertParameter(parameters);
                DAC dac = new DAC(connString);
                object o = dac.ExecuteScalar(cmdText, cmdType, ps);
                Log.Write(LogAction.Svc, className, "ExecuteScalar", connString, cmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行成功:" + parameters.ToKeyString());
                return o;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "ExecuteScalar", connString, cmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行出错:cmdText:" + cmdText + ";" + parameters.ToKeyString() + ";" + e.ToString());
            }
            return new object();
        }

        public DataSet ExecuteDataSet(string tableName, string connString, string cmdText, CommandType cmdType, params DatabaseParameter[] parameters)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                DbParameter[] ps = ConvertParameter(parameters);
                DAC dac = new DAC(connString);
                DataSet ds = dac.ExecuteDataSet(tableName, cmdText, cmdType, ps);
                Log.Write(LogAction.Svc, className, "ExecuteDataSet", connString, cmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行成功:" + parameters.ToKeyString());
                return ds;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "ExecuteDataSet", connString, cmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行出错:cmdText:" + cmdText + ";" + parameters.ToKeyString() + ";" + e.ToString());
            }
            return new DataSet();
        }

        public DataSet ExecutePagedDataSet(string connString, string dsTableName, string tableName, string fields, string orderBy, long pageIndex, long pageSize, long recordCount, string where)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                DAC dac = new DAC(connString);
                List<DbParameter> pars = new List<DbParameter>();
                pars.Add(dac.CreateParameter("TableName", tableName));
                pars.Add(dac.CreateParameter("Fields", fields));
                pars.Add(dac.CreateParameter("OrderBy", orderBy));
                pars.Add(dac.CreateParameter("PageIndex", pageIndex));
                pars.Add(dac.CreateParameter("PageSize", pageSize));
                pars.Add(dac.CreateParameter("RecordCount", recordCount));
                pars.Add(dac.CreateParameter("Where", where));
                DataSet ds = dac.ExecuteDataSet(tableName, "PagedRecordSP", CommandType.StoredProcedure, pars.ToArray());
                Log.Write(LogAction.Dac, className, "ExecutePagedDataSet", connString, "PagedRecordSP", DateTime.Now.Ticks - t, "执行成功");
                return ds;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "ExecutePagedDataSet", connString, "PagedRecordSP", DateTime.Now.Ticks - t, "执行出错:cmdText:PagedRecordSP;" + e.Message);
            }
            return new DataSet();
        }

        public DataSet ExecutePagedDataSet(PagedRecordParameter prp)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                DAC dac = new DAC(prp.ConnectionString);
                List<DbParameter> pars = new List<DbParameter>();
                pars.Add(dac.CreateParameter("TableName", prp.TableName));
                pars.Add(dac.CreateParameter("Fields", prp.Fields));
                pars.Add(dac.CreateParameter("OrderBy", prp.OrderBy));
                pars.Add(dac.CreateParameter("PageIndex", prp.PageIndex));
                pars.Add(dac.CreateParameter("PageSize", prp.PageSize));
                pars.Add(dac.CreateParameter("RecordCount", prp.RecordCount));
                pars.Add(dac.CreateParameter("Where", prp.Where));
                DataSet ds = dac.ExecuteDataSet(prp.DSTableName, "PagedRecordSP", CommandType.StoredProcedure, pars.ToArray());
                Log.Write(LogAction.Dac, className, "ExecutePagedDataSetWraped", prp.ConnectionString, "PagedRecordSP", DateTime.Now.Ticks - t, "执行成功");
                return ds;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "ExecutePagedDataSetWraped", prp.ConnectionString, "PagedRecordSP", DateTime.Now.Ticks - t, "执行出错:cmdText:PagedRecordSP;" + e.Message);
            }
            return new DataSet();
        }

        public string ExecutePagedDataList(PagedRecordParameter prp)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                DAC dac = new DAC(prp.ConnectionString);
                List<DbParameter> pars = new List<DbParameter>();
                pars.Add(dac.CreateParameter("TableName", prp.TableName));
                pars.Add(dac.CreateParameter("Fields", prp.Fields));
                pars.Add(dac.CreateParameter("OrderBy", prp.OrderBy));
                pars.Add(dac.CreateParameter("PageIndex", prp.PageIndex));
                pars.Add(dac.CreateParameter("PageSize", prp.PageSize));
                pars.Add(dac.CreateParameter("RecordCount", prp.RecordCount));
                pars.Add(dac.CreateParameter("Where", prp.Where));
                string s = dac.ExecuteDataList("PagedRecordSP", CommandType.StoredProcedure, pars.ToArray());
                Log.Write(LogAction.Dac, className, "ExecutePagedDataSetWraped", prp.ConnectionString, "PagedRecordSP", DateTime.Now.Ticks - t, "执行成功");
                return s;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "ExecutePagedDataSetWraped", prp.ConnectionString, "PagedRecordSP", DateTime.Now.Ticks - t, "执行出错:cmdText:PagedRecordSP;" + e.Message);
            }
            return string.Empty;
        }

        public string ExecuteDataList(string connString, string cmdText, CommandType cmdType, params DatabaseParameter[] parameters)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                DbParameter[] ps = ConvertParameter(parameters);
                DAC dac = new DAC(connString);
                string list = dac.ExecuteDataList(cmdText, cmdType, ps);
                Log.Write(LogAction.Svc, className, "ExecuteDataList", connString, cmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行成功:" + parameters.ToKeyString() + ";数据:" + list);
                return list;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "ExecuteDataList", connString, cmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行出错:cmdText:" + cmdText + ";" + parameters.ToKeyString() + ";" + e.ToString());
            }
            return "[]";
        }

        public int Fill(string connString, DataSet ds, string cmdText, CommandType cmdType, params DatabaseParameter[] parameters)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                DbParameter[] ps = ConvertParameter(parameters);
                DAC dac = new DAC(connString);
                int rtn = dac.Fill(ds, cmdText, cmdType, ps);
                Log.Write(LogAction.Svc, className, "Fill", connString, cmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行成功:" + parameters.ToKeyString());
                return rtn;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "Fill", connString, cmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行出错:cmdText:" + cmdText + ";" + parameters.ToKeyString() + ";" + e.ToString());
            }
            return -1;
        }

        public int AdapterSave(string connString, DataSet ds, string insertCmdText, string updateCmdText, string deleteCmdText, CommandType cmdType, params DatabaseParameter[] parameters)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                DbParameter[] ps = ConvertParameter(parameters);
                DAC dac = new DAC(connString);
                int rtn = dac.AdapterSave(ds, insertCmdText, updateCmdText, deleteCmdText, cmdType, ps);
                Log.Write(LogAction.Svc, className, "AdapterSave", connString, updateCmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行成功:" + parameters.ToKeyString());
                return rtn;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "AdapterSave", connString, updateCmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行出错:updateCmdText:" + updateCmdText + ";" + e.ToString());
            }
            return -1;
        }

        public bool SqlBulkInsertDataReader(string connString, string targetTable, IDataReader dr)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                DAC dac = new DAC(connString);
                bool rtn = dac.SqlBulkInsertDataReader(targetTable, dr);
                Log.Write(LogAction.Svc, className, "SqlBulkInsertDataReader1", connString, targetTable, DateTime.Now.Ticks - t, "执行成功:");
                return rtn;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "SqlBulkInsertDataReader1", connString, targetTable, DateTime.Now.Ticks - t, "执行出错targetTable:" + targetTable + ";" + e.Message);
            }
            return false;
        }

        public bool SqlBulkInsertDataTable(string connString, string targetTable, DataTable dt)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                DAC dac = new DAC(connString);
                bool rtn = dac.SqlBulkInsertDataTable(targetTable, dt);
                Log.Write(LogAction.Svc, className, "SqlBulkInsertDataTable1", connString, targetTable, DateTime.Now.Ticks - t, "执行成功:");
                return rtn;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "SqlBulkInsertDataTable1", connString, targetTable, DateTime.Now.Ticks - t, "执行出错targetTable:" + targetTable + ";" + e.Message);
            }
            return false;
        }

        public bool SqlBulkInsertDataReader(string connString, string targetTable, IDataReader dr, SqlBulkCopyColumnMapping[] mappings)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                DAC dac = new DAC(connString);
                bool rtn = dac.SqlBulkInsertDataReader(targetTable, dr, mappings);
                Log.Write(LogAction.Svc, className, "SqlBulkInsertDataReader2", connString, targetTable, DateTime.Now.Ticks - t, "执行成功:");
                return rtn;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "SqlBulkInsertDataReader2", connString, targetTable, DateTime.Now.Ticks - t, "执行出错targetTable:" + targetTable + ";" + e.Message);
            }
            return false;
        }

        public bool SqlBulkInsertDataTable(string connString, string targetTable, DataTable dt, SqlBulkCopyColumnMapping[] mappings)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                DAC dac = new DAC(connString);
                bool rtn = dac.SqlBulkInsertDataTable(targetTable, dt, mappings);
                Log.Write(LogAction.Svc, className, "SqlBulkInsertDataTabl2", connString, targetTable, DateTime.Now.Ticks - t, "执行成功:");
                return rtn;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "SqlBulkInsertDataTable2", connString, targetTable, DateTime.Now.Ticks - t, "执行出错targetTable:" + targetTable + ";" + e.Message);
            }
            return false;
        }

        public int ExecuteNonQuery(params WrapedDatabaseParameter[] parmss)
        {
            long t = DateTime.Now.Ticks;
            int n = 0;
            foreach (WrapedDatabaseParameter parms in parmss)
            {
                try
                {
                    DatabaseParameter[] pars = parms.DatabaseParameters ?? new DatabaseParameter[] { };
                    DbParameter[] ps = ConvertParameter(pars);
                    DAC dac = new DAC(parms.RoutingKey, parms.ConnectionString);
                    n += dac.ExecuteNonQuery(parms.CmdText, (CommandType)parms.CmdType, ps);
                    Log.Write(LogAction.Svc, className, "ExecuteNonQueryWraped", parms.ConnectionString, parms.CmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行成功:" + pars.ToKeyString());
                } 
                catch (Exception e)
                {
                    Log.Write(LogAction.Error, className, "ExecuteNonQueryWraped", parms.ConnectionString, parms.CmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行出错:" + parms + ";" + e.ToString());
                    return -1;
                }
            }
            return n;
        }

        public object ExecuteScalar(WrapedDatabaseParameter parms)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                DatabaseParameter[] pars = parms.DatabaseParameters ?? new DatabaseParameter[] { };
                DbParameter[] ps = ConvertParameter(pars);
                DAC dac = new DAC(parms.RoutingKey, parms.ConnectionString);
                object o = dac.ExecuteScalar(parms.CmdText, (CommandType)parms.CmdType, ps);
                Log.Write(LogAction.Svc, className, "ExecuteScalarWraped", parms.ConnectionString, parms.CmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行成功:" + pars.ToKeyString());
                return o;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "ExecuteScalarWraped", parms.ConnectionString, parms.CmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行出错:" + parms + ";" + e.ToString());
            }
            return new object();
        }

        public DataSet ExecuteDataSet(WrapedDatabaseParameter parms)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                DatabaseParameter[] pars = parms.DatabaseParameters ?? new DatabaseParameter[] { };
                DbParameter[] ps = ConvertParameter(pars);
                DAC dac = new DAC(parms.RoutingKey, parms.ConnectionString);
                DataSet ds = dac.ExecuteDataSet(parms.TableName ?? "DataSet", parms.CmdText, (CommandType)parms.CmdType, ps);
                Log.Write(LogAction.Svc, className, "ExecuteDataSetWraped", parms.ConnectionString, parms.CmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行成功:" + (CommandType)parms.CmdType + " " + pars.ToKeyString());
                return ds;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "ExecuteDataSetWraped", parms.ConnectionString, parms.CmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行出错:" + (CommandType)parms.CmdType + " " + parms + ";" + e.ToString());
            }
            return new DataSet();
        }

        public string ExecuteDataTable(WrapedDatabaseParameter parms)
        {
            return ExecuteDataSet(parms).GetXml().Replace("\r\n", "").Replace(" ", "");
        }

        public string ExecuteDataList(WrapedDatabaseParameter parms)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                DatabaseParameter[] pars = parms.DatabaseParameters ?? new DatabaseParameter[] { };
                DbParameter[] ps = ConvertParameter(pars);
                DAC dac = new DAC(parms.RoutingKey, parms.ConnectionString);
                string list = dac.ExecuteDataList(parms.CmdText, (CommandType)parms.CmdType, ps);
                Log.Write(LogAction.Svc, className, "ExecuteDataListWraped", parms.ConnectionString, parms.CmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行成功:" + (CommandType)parms.CmdType + " " + pars.ToKeyString());
                return list;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "ExecuteDataListWraped", parms.ConnectionString, parms.CmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行出错:" + (CommandType)parms.CmdType + " " + parms + ";" + e.ToString());
            }
            return "[]";
        }

        public DbParameter[] ConvertParameter(DatabaseParameter[] parameters)
        {
            int len = parameters.Length;
            DbParameter[] paras = new DbParameter[len];
            DbParameter p1 = null;
            int i = 0;
            foreach (DatabaseParameter p in parameters)
            {
                if (p.ProviderName == DatabaseProviders.SqlProvider)
                    p1 = new SqlParameter();
                else// if (p.ProviderName == DatabaseProviders.OracleProvider)
                    p1 = new OracleParameter();
                p1.DbType = p.DbType;
                p1.Direction = p.Direction;
                p1.ParameterName = p.ParameterName;
                if (p.Size != 0)
                    p1.Size = p.Size;
                p1.SourceColumn = p.SourceColumn;
                p1.SourceColumnNullMapping = p.SourceColumnNullMapping;
                p.SourceVersion = DataRowVersion.Current;
                p1.SourceVersion = p.SourceVersion;
                p1.Value = p.Value;
                paras[i] = p1;
                i++;
            }
            return paras;
        }


        public string SaveFile(string name, byte[] img)
        {
            try
            {
                DAC dac = new DAC(Config.FileConnectionName);
                return dac.SaveFile(name, img);
            }
            catch (Exception e)
            {
                Log.Error("DACService.SaveFile:" + e.Message);
                return name;
            }
        }

        public byte[] GetFile(string name)
        {
            try
            {
                DAC dac = new DAC(Config.FileConnectionName);
                return dac.GetFile(name);
            }
            catch (Exception e)
            {
                Log.Error("DACService.GetFile:" + e.Message);
                return new byte[1] { 0 };
            }
        }

    }
}
