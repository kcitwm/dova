using System;
using System.Data;
using System.Data.Common;
using System.ServiceModel;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using Dova.Data;
using Dova.Utility;
using Dova.Interfaces;
using System.Configuration;
using System.Xml;
using System.Collections.Generic;
using System.IO; 
using System.Web.Security;

namespace Dova.Data
{
    public class DAC
    {
        static string className = "DAC";
        static string routingGroupName = "DAC";
        protected string _providerName = "System.Data.SqlClient";
        protected string _connString = Config.DefaultConnectionName;

        public string ConnString
        {
            get { return _connString; }
            set { _connString = value; }
        }

        protected string _serviceName = Config.DefaultConnectionName;
        public string ServiceName
        {
            get { return _serviceName; }
            set { _serviceName = value; }
        }

        protected static int _model = 1;

        IClientDataAccess dac = null;

        #region 配置脚本

        static Dictionary<string, Dictionary<string, Scripter>> scripts = new Dictionary<string, Dictionary<string, Scripter>>();
        static Dictionary<string, IClientDataAccess> dacs = new Dictionary<string, IClientDataAccess>();
        static string scriptPath = "";

        static DAC()
        {
            long t = DateTime.Now.Ticks;
            try
            {
                scriptPath = Config.Get("DACScriptPath", Config.BasePath + "Scripts\\");
                _model = Config.Get("DACModel", "1").ToInt32();
                if (_model == 2)
                {
                    if (!Directory.Exists(scriptPath))
                        Directory.CreateDirectory(scriptPath);
                    ConnectionStringSettingsCollection cons = ConfigurationManager.ConnectionStrings;
                    if (null != cons && cons.Count > 0)
                    {
                        foreach (ConnectionStringSettings con in cons)
                        {
                            LoadScript(con.Name);
                        }
                    }
                }
                Log.Write(LogAction.Dac, className, "静态DAC", "info", DateTime.Now.Ticks - t, "初始化DAC：scriptPath:" + scriptPath + ";_model:" + _model);
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "静态DAC", "Error", DateTime.Now.Ticks - t, "初始化DAC出错：" + e.Message);
            }
        }

        static void LoadScript(string connStrings)
        {
            if (_model == 1 || scripts.ContainsKey(connStrings)) return;
            long t = DateTime.Now.Ticks;
            try
            {
                Dictionary<string, Scripter> ss = new Dictionary<string, Scripter>();
                XmlDocument doc = new XmlDocument();
                if (File.Exists(scriptPath + connStrings + ".xml"))
                {
                    doc.Load(scriptPath + connStrings + ".xml");
                    XmlNodeList nodeList;
                    XmlNode root = doc.DocumentElement;
                    nodeList = root.SelectNodes("//s");
                    string name = string.Empty; ;
                    string ctype = string.Empty;
                    string cacheKey = string.Empty;
                    string cacheParas = string.Empty;
                    string cacheSpan = string.Empty;
                    foreach (XmlNode n in nodeList)
                    {
                        ctype = n.Attributes["CType"] == null ? "1" : n.Attributes["CType"].Value;
                        cacheKey = n.Attributes["CacheKey"] == null ? string.Empty : n.Attributes["CacheKey"].Value;
                        cacheParas = n.Attributes["CacheParas"] == null ? string.Empty : n.Attributes["CacheParas"].Value;
                        cacheSpan = n.Attributes["CacheSpan"] == null ? string.Empty : n.Attributes["CacheSpan"].Value;
                        name = n.Attributes["CTxt"].Value;
                        ss[name] = new Scripter(name, n.InnerText, ctype.ToInt32(), cacheKey, cacheParas, cacheSpan);
                        Log.Write("加载了执行脚本：" + name + ":" + n.InnerText);
                    }
                    scripts[connStrings] = ss;
                }
                else
                    Log.Write(LogAction.Error, className, "LoadScript", connStrings, DateTime.Now.Ticks - t, "没有找到脚本文件:" + scriptPath + connStrings + ".xml");
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "LoadScript", connStrings, DateTime.Now.Ticks - t, "加载执行脚本connStrings出错:" + connStrings + ":" + e.Message);
            }
        }

        #endregion

        public DAC()
        {
            Init(_serviceName, _connString);
        }

        void Init(string serviceName, string connString)
        {
            if (dacs.ContainsKey(serviceName))
            {
                dac = dacs[serviceName];
                return;
            }
            long t = DateTime.Now.Ticks;
            Log.Write("创建本地访问实例:ServiceConfigs.Services.Count:" + ServiceConfigs.Services.Count);
            if (ServiceConfigs.Services.ContainsKey(serviceName))
            {
                dac = new RemoteDAC(serviceName, connString);
                dacs[serviceName] = dac;
                Log.Write("创建远程访问实例:serviceName:" + serviceName);
                return;
            }
            Log.Write("创建本地访问实例:connString:" + connString);
            LoadScript(connString);
            ConnectionStringSettings css = Config.GetConnection(connString);
            if (css.ProviderName != "")
                _providerName = css.ProviderName;
            dac = new LocalDAC(css.ConnectionString, _providerName);
            dacs[serviceName] = dac;
            Log.Write(LogAction.Dac, className, "Init", connString, DateTime.Now.Ticks - t, "初始化DAC完毕");
        }

        public DAC(string connectionString)
        {
            this._connString = connectionString;
            this._serviceName = connectionString;
            Init(_serviceName, _connString);
        }

        protected virtual string Routing(string groupName, string routingKey, string defaultValue)
        {
            string serviceName = defaultValue;
            try
            {
                Routing routing = ServiceConfigs.Routings[groupName][routingKey];
                serviceName = routing.GroupName;
                Log.Write(LogAction.Dac, className, "Routing", groupName, routingKey, -1, ",找到路由:" + groupName + "." + routingKey);
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "Routing", groupName, routingKey, -1, ",没有找到路由:" + groupName + "." + routingKey + ";" + e.ToString());
            }
            return serviceName;
        }

        public DAC(string routingKey, string connectionString)
        {
            this._connString = connectionString;
            if (string.IsNullOrEmpty(routingKey))
                _serviceName = connectionString;
            else
                this._serviceName = Routing(routingGroupName, routingKey, connectionString);
            Init(_serviceName, _connString);
        }


        public DAC(string groupName, string routingKey, string connectionString)
        {
            this._connString = connectionString;
            if (string.IsNullOrEmpty(routingKey))
                _serviceName = connectionString;
            else
                this._serviceName = Routing(groupName, routingKey, connectionString);
            Init(_serviceName, _connString);
        }

        public virtual LoginRes Login(LoginReq req)
        {
            long t = DateTime.Now.Ticks;
            string cmdText = "UserLogin";
            LoginRes info = new LoginRes();
            try
            {
                bool status = (this.CheckLogin(req.UserName) == 1);
                if (!status)
                    status = Membership.ValidateUser(req.UserName, req.Password);
                Log.Write(LogAction.Svc, className, "Login", "Membership", cmdText, DateTime.Now.Ticks - t, "执行成功:userName,loc,machineKey:" + req.UserName + "," + req.Location + "," + req.MachineKey + ";status=" + status + " pwd:" + req.Password);
                if (status)
                {
                    info.UserID = req.UserName;
                    info.MachineKey = req.MachineKey == null ? string.Empty : req.MachineKey;
                    info.Location = req.Location == null ? string.Empty : req.Location;
                    //info.Roles = new List<string>(Roles.GetRolesForUser(req.UserName));
                    //DAC dac = new DAC(Config.LoginConnectionName);
                    info.Rights = new List<string>();//根据用户和app获取权限列表．
                    Dova.Cache.CacheManager.Set(Config.LoginCacheServiceName, req.UserName, info, new TimeSpan(24, 0, 0));
                    info.Status = 1;
                }  

            }
            catch (Exception e)
            {
                Log.Write(LogAction.Svc, className, "Login", "Membership", cmdText, DateTime.Now.Ticks - t, "执行失败:userName,loc,machineKey:" + req.UserName + "," + req.Location + "," + req.MachineKey + ";" + e.Message);
            }
            return info;
            //return Newtonsoft.Json.JsonConvert.SerializeObject(info);
        }

        public virtual int Regist(LoginReq req)
        {
            long t = DateTime.Now.Ticks;
            if (string.IsNullOrEmpty(req.Email))
                req.Email = req.UserName + "@outlook.com";
            MembershipCreateStatus status = MembershipCreateStatus.UserRejected;
            try
            {
                MembershipUser user = Membership.CreateUser(req.UserName, req.Password, req.UserName + "@outlook.com", "qqqqqqqqqqqqqq", "qqqqqqqqqqqqqq", true, out status);

                Log.Write(LogAction.Svc, className, "Regist", "Regist", "CreateUser", DateTime.Now.Ticks - t, "执行成功:userName,loc,machineKey:" + req.UserName + "," + req.Location + "," + req.MachineKey + ";status=" + status);
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Svc, className, "Regist", "Regist", "CreateUser", DateTime.Now.Ticks - t, "执行失败:userName,loc,machineKey:" + req.UserName + "," + req.Location + "," + req.MachineKey + ";" + e.Message);
            }
            return (int)status;
        }

        public virtual int Logout(string userName)
        {
            try
            {
                Dova.Cache.CacheManager.Remove(Config.LoginCacheServiceName, userName);
            }
            catch
            {
                return -1;
            }
            return 1;
        }

        public virtual int CheckLogin(string userName)
        {
            try
            {
                if (null != Dova.Cache.CacheManager.Get(Config.LoginCacheServiceName, userName))
                    return 1;
                return 0;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "CheckLogin", "CheckLogin", "CheckLogin", 0, "执行失败:userName:" + userName + ";" + e.Message);
                return -1;
            }
        }


        public virtual IDataReader ExcuteDataReader(string cmdText, CommandType cmdType, params DbParameter[] parameters)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                string sql = cmdText;
                if (_model == 2)
                {
                    Scripter spr = scripts[_connString][cmdText];
                    cmdText = spr.CmdText;
                    cmdType = (CommandType)spr.CmdType;
                }
                IDataReader dr = dac.ExcuteDataReader(sql, cmdType, parameters);
                Log.Write(LogAction.Dac, className, "ExcuteDataReader", _connString, cmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行成功");
                return dr;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "ExcuteDataReader", _connString, cmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行出错:cmdText:" + cmdText + ";" + e.ToString());
                throw;
            }
        }

        public virtual int ExecuteNonQuery(string cmdText, CommandType cmdType, params DbParameter[] parameters)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                if (_model == 2)
                {
                    Scripter spr = scripts[_connString][cmdText];
                    cmdText = spr.CmdText;
                    cmdType = (CommandType)spr.CmdType;
                }
                int n = dac.ExecuteNonQuery(cmdText, cmdType, parameters);
                Log.Write(LogAction.Dac, className, "ExecuteNonQuery", _connString, cmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行成功");
                return n;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "ExecuteNonQuery", _connString, cmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行出错:cmdText:" + cmdText + ";" + e.Message);
                throw;
            }
        }

        public virtual void ExecuteNonQueryOneWay(string cmdText, CommandType cmdType, params DbParameter[] parameters)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                if (_model == 2)
                {
                    Scripter spr = scripts[_connString][cmdText];
                    cmdText = spr.CmdText;
                    cmdType = (CommandType)spr.CmdType;
                }
                ExecuteNonQuery(cmdText, cmdType, parameters);
                Log.Write(LogAction.Dac, className, "ExecuteNonQueryOneWay", _connString, cmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行成功");
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "ExecuteNonQueryOneWay", _connString, cmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行出错:cmdText:" + cmdText + ";" + e.Message);
                throw;
            }
        }

        public virtual object ExecuteScalar(string cmdText, CommandType cmdType, params DbParameter[] parameters)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                object o = null;
                string cacheKey = string.Empty;
                TimeSpan tsp = TimeSpan.FromMinutes(20); ;
                if (_model == 2)
                {
                    Scripter spr = scripts[_connString][cmdText];
                    cmdText = spr.CmdText;
                    cmdType = (CommandType)spr.CmdType;
                    cacheKey = spr.GetCachekey(parameters);
                    tsp = spr.CacheSpan;
                    o = Dova.Cache.CacheManager.Get(cacheKey);
                    if (null != o) return o;
                }
                o = dac.ExecuteScalar(cmdText, cmdType, parameters);
                if (cacheKey != string.Empty)
                    Dova.Cache.CacheManager.Set(cacheKey, tsp, o);
                Log.Write(LogAction.Dac, className, "ExecuteScalar", _connString, cmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行成功");
                return o;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "ExecuteScalar", _connString, cmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行出错:cmdText:" + cmdText + ";" + e.Message);
                throw;
            }
        }

        public virtual DataSet ExecuteDataSet(string tableName, string cmdText, CommandType cmdType, params DbParameter[] parameters)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                string cacheKey = string.Empty;
                TimeSpan tsp = TimeSpan.FromMinutes(20); ;
                object o = null;
                if (_model == 2)
                {
                    Scripter spr = scripts[_connString][cmdText];
                    cmdText = spr.CmdText;
                    cmdType = (CommandType)spr.CmdType;
                    cacheKey = spr.GetCachekey(parameters);
                    tsp = spr.CacheSpan;
                    o = Dova.Cache.CacheManager.Get(cacheKey);
                    if (null != o) return o as DataSet;
                }
                DataSet ds = dac.ExecuteDataSet(tableName, cmdText, cmdType, parameters);
                if (cacheKey != string.Empty)
                    Dova.Cache.CacheManager.Set(cacheKey, tsp, o);
                Log.Write(LogAction.Dac, className, "ExecuteDataSet", _connString, cmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行成功");
                return ds;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "ExecuteDataSet", _connString, cmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行出错:cmdText:" + cmdText + ";" + e.Message);
                throw;
            }
        }

        public virtual DataSet ExecutePagedDataSet(string dsTableName, string tableName, string fields, string orderBy, long pageIndex, long pageSize, long recordCount, string where)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                string cmdText = "PagedRecordSP";
                List<DbParameter> pars = new List<DbParameter>();
                pars.Add(CreateParameter("TableName", tableName));
                pars.Add(CreateParameter("Fields", fields));
                pars.Add(CreateParameter("OrderBy", orderBy));
                pars.Add(CreateParameter("PageIndex", pageIndex));
                pars.Add(CreateParameter("PageSize", pageSize));
                pars.Add(CreateParameter("RecordCount", recordCount));
                pars.Add(CreateParameter("Where", where));
                DataSet ds = dac.ExecuteDataSet(tableName, cmdText, CommandType.StoredProcedure, pars.ToArray());
                Log.Write(LogAction.Dac, className, "ExecutePagedDataSet", _connString, "PagedRecordSP", DateTime.Now.Ticks - t, "执行成功");
                return ds;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "ExecutePagedDataSet", _connString, "PagedRecordSP", DateTime.Now.Ticks - t, "执行出错:cmdText:PagedRecordSP;" + e.Message);
                throw;
            }
        }

         



        public virtual string ExecuteDataList(string cmdText, CommandType cmdType, params DbParameter[] parameters)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                string cacheKey = string.Empty;
                TimeSpan tsp = TimeSpan.FromMinutes(20); ;
                object o = null;
                if (_model == 2)
                {
                    Scripter spr = scripts[_connString][cmdText];
                    cmdText = spr.CmdText;
                    cmdType = (CommandType)spr.CmdType;
                    cacheKey = spr.GetCachekey(parameters);
                    tsp = spr.CacheSpan;
                    o = Dova.Cache.CacheManager.Get(cacheKey);
                    if (null != o) return o as string;
                }
                string list = dac.ExecuteDataList(cmdText, cmdType, parameters);
                if (cacheKey != string.Empty)
                    Dova.Cache.CacheManager.Set(cacheKey, tsp, list);
                Log.Write(LogAction.Dac, className, "ExecuteDataList", _connString, cmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行成功");
                return list;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "ExecuteDataList", _connString, cmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行出错:cmdText:" + cmdText + ";" + e.Message);
                throw;
            }
        }

        public virtual int Fill(DataSet ds, string cmdText, CommandType cmdType, params DbParameter[] parameters)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                if (_model == 2)
                {
                    Scripter spr = scripts[_connString][cmdText];
                    cmdText = spr.CmdText;
                    cmdType = (CommandType)spr.CmdType;
                }
                int n = dac.Fill(ds, cmdText, cmdType, parameters);
                Log.Write(LogAction.Dac, className, "Fill", _connString, cmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行成功");
                return n;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "Fill", _connString, cmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行出错:cmdText:" + cmdText + ";" + e.Message);
                throw;
            }
        }

        public virtual int AdapterSave(DataSet ds, string insertCmdText, string updateCmdText, string deleteCmdText, CommandType cmdType, params DbParameter[] parameters)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                if (_model == 2)
                {
                    insertCmdText = scripts[_connString][insertCmdText].CmdText;
                    updateCmdText = scripts[_connString][updateCmdText].CmdText;
                    deleteCmdText = scripts[_connString][deleteCmdText].CmdText;
                }
                int n = dac.AdapterSave(ds, insertCmdText, updateCmdText, deleteCmdText, cmdType, parameters);
                Log.Write(LogAction.Dac, className, "AdapterSave", _connString, updateCmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行成功");
                return n;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "AdapterSave", _connString, updateCmdText.TrimSql(128), DateTime.Now.Ticks - t, "执行出错:updateCmdText:" + updateCmdText + ";" + e.Message);
                throw;
            }
        }

        public virtual bool SqlBulkInsertDataReader(string targetTable, IDataReader dr)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                bool n = dac.SqlBulkInsertDataReader(targetTable, dr);
                Log.Write(LogAction.Dac, className, "SqlBulkInsertDataReader1", _connString, targetTable, DateTime.Now.Ticks - t, "执行成功");
                return n;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "SqlBulkInsertDataReader1", _connString, targetTable, DateTime.Now.Ticks - t, "执行出错:targetTable:" + targetTable + ";" + e.Message);
                throw;
            }
        }

        public virtual bool SqlBulkInsertDataTable(string targetTable, DataTable dt)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                bool n = dac.SqlBulkInsertDataTable(targetTable, dt);
                Log.Write(LogAction.Dac, className, "SqlBulkInsertDataTable1", _connString, targetTable, DateTime.Now.Ticks - t, "执行成功");
                return n;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "SqlBulkInsertDataTable1", _connString, targetTable, DateTime.Now.Ticks - t, "执行出错:targetTable:" + targetTable + ";" + e.Message);
                throw;
            }
        }

        public virtual bool SqlBulkInsertDataReader(string targetTable, IDataReader dr, SqlBulkCopyColumnMapping[] mappings)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                bool n = dac.SqlBulkInsertDataReader(targetTable, dr, mappings);
                Log.Write(LogAction.Dac, className, "SqlBulkInsertDataReader2", _connString, targetTable, DateTime.Now.Ticks - t, "执行成功");
                return n;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "SqlBulkInsertDataReader2", _connString, targetTable, DateTime.Now.Ticks - t, "执行出错:targetTable:" + targetTable + ";" + e.Message);
                throw;
            }
        }

        public virtual bool SqlBulkInsertDataTable(string targetTable, DataTable dt, SqlBulkCopyColumnMapping[] mappings)
        {
            long t = DateTime.Now.Ticks;
            try
            {
                bool n = dac.SqlBulkInsertDataTable(targetTable, dt, mappings);
                Log.Write(LogAction.Dac, className, "SqlBulkInsertDataTable2", _connString, targetTable, DateTime.Now.Ticks - t, "执行成功");
                return n;
            }
            catch (Exception e)
            {
                Log.Write(LogAction.Error, className, "SqlBulkInsertDataTable2", _connString, targetTable, DateTime.Now.Ticks - t, "执行出错:targetTable:" + targetTable + ";" + e.Message);
                throw;
            }
        }

        public DbParameter CreateParameter()
        {
            return DataHelper.CreateParameter(_providerName);
        }

        public DbParameter CreateParameter(string name, object value)
        {
            return DataHelper.CreateParameter(_providerName, name, value);
        }

        public DbParameter CreateParameter(ParameterDirection dirction, string name)
        {
            return DataHelper.CreateParameter(_providerName, name, dirction);
        }

        public DbParameter CreateParameter(string name, object value, DbType type)
        {
            return DataHelper.CreateParameter(_providerName, name, value, type);
        }

        public DbParameter CreateParameter(string name, DbType type, ParameterDirection direction)
        {
            return DataHelper.CreateParameter(_providerName, name, type, direction);
        }

        public DbParameter CreateParameter(string name, ParameterDirection direction, DbType type, int size)
        {
            return DataHelper.CreateParameter(_providerName, name, direction, type, size);
        }

        public DbParameter CreateParameter(string name, ParameterDirection direction, int size)
        {
            return DataHelper.CreateParameter(_providerName, name, direction, size);
        }

        public DbParameter CreateParameter(string name, DbType type, int size, object value)
        {
            return DataHelper.CreateParameter(_providerName, name, type, size, value);
        }

        public DbParameter CreateParameter(string name, DbType type, ParameterDirection direction, object value)
        {
            return DataHelper.CreateParameter(_providerName, name, type, direction, value);
        }

        public DbParameter CreateParameter(string name, DbType type, ParameterDirection direction, int size, object value)
        {
            return DataHelper.CreateParameter(_providerName, name, type, direction, size, value);
        }


        public byte[] GetFile(string name)
        {
            return dac.GetFile(name);
        }

        public string SaveFile(string name, byte[] img)
        {
            return dac.SaveFile(name, img);
        }

    }
}
