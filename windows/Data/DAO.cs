using System;
using System.Data;
using System.Reflection;
using System.Data.Common;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.OracleClient;
using System.Collections.Generic;

using Dova.Utility;
using Dova.Services;
using Dova.Interfaces;
using System.Web.Security;
using System.Collections;
using Dova.Infrastructure;

namespace Dova.Data
{
    /// <summary>
    /// 作者:陈亮 kcitwm@gmail.com
    /// 设计原则:
    /// 底层DAO只负责横向逻辑,分布式,性能等.
    /// 外层决定纵向逻辑,如不同用户不同的连接串.至于不同的连接的NWR策略可以向下层移动比如到DAO层.因为外层的连接策略已经决定,DAO层根据不同的连接来决策不同的NWR策略.
    /// 1. 默认本地,可动态指定远程或者本地,由配置文件和数据库配置读取
    /// 2. NWR策略实现:每个dao类型对应多个结点执行,根据NWR配置原则判断成功失败
    /// 3. 读写分离.
    /// 4. 支持不同接口实现:多个结点的dao类型可能不一样,这个可以丢在服务端IOC实现,但客户端需要选择策略吗?
    /// </summary>
    public class DAO<T> where T : new()
    {
        string typeName = typeof(T).Name;
        protected string fullTypeName = typeof(T).FullName;
        protected string saveSP = "SaveSP";
        protected string getSP = "GetSP";
        protected string getByKeySP = "GetByKeySP";
        protected string listSP = "ListSP";
        protected string pagedSP = "GetPagedRecordSP";
        protected string searchSP = "SearchSP";

        static readonly string localDefault = "LocalDefault";

        static Dictionary<string, Dictionary<string, int>> dbProperties = new Dictionary<string, Dictionary<string, int>>();

        static object o = new object();

        DAC dac = null;

        static ConnectionStringSettingsCollection conns = ConfigurationManager.ConnectionStrings;

        public DAO()
        { 
            dac = new DAC(Config.DefaultConnectionName);
        }

        public DAO(ConnectionStringSettings connectionStrings)
        {
            _connectionString = connectionStrings.ConnectionString;
            _providerName = connectionStrings.ProviderName;
        }

        public DAO(string connString, string providerName)
        {
            _connectionString = connString;
            _providerName = providerName;
        }

        protected string _connectionString;
        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        protected string _providerName;
        public string ProviderName
        {
            get { return _providerName; }
            set { _providerName = value; }
        }

        protected DbParameter[] GetAllFieldsParas(T entity, string cmdText, CommandType cmdType)
        {
            Dictionary<string, int> pars = new Dictionary<string, int>();
            Type t = typeof(T);
            PropertyInfo[] ps = t.GetProperties();
            string key = FormsAuthentication.HashPasswordForStoringInConfigFile(cmdText, "MD5");
            if (dbProperties.ContainsKey(key))
                pars = dbProperties[key];
            else
            {
                lock (o)
                {
                    if (!dbProperties.ContainsKey(key))
                    {
                        pars = GetSPParameters(cmdText, cmdType);
                        dbProperties[key] = pars;
                    }
                }
                pars = dbProperties[key];
            }
            int len = pars.Count;//
            DbParameter[] parames = new DbParameter[len];
            int i = 0;
            DbParameter ip = null;
            PropertyInfo p = null;
            BindingFlags flag = BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.GetProperty;
            foreach (string s in pars.Keys)
            {
                p = t.GetProperty(s.TrimStart(new char[] { '@', ':' }), flag);
                if (null != p)
                    ip = dac.CreateParameter(s, p.GetValue(entity, null));
                else if (pars[s] == 2)
                {
                    ip = dac.CreateParameter(s, ParameterDirection.Output);
                }
                else if (pars[s] == 5)
                {
                    ip = dac.CreateParameter(s, ParameterDirection.ReturnValue, 16);
                }
                parames[i] = ip;
                i++;
            }
            return parames;
        }

        public virtual int Save(T entity)
        {
            DbParameter[] pars = GetAllFieldsParas(entity, typeName + saveSP, CommandType.StoredProcedure);
            return dac.ExecuteNonQuery(typeName + saveSP, CommandType.StoredProcedure, pars);
        }

        //public virtual int Save(T entity, string cmdText, CommandType cmdType)
        //{
        //    DbParameter[] pars = GetAllFieldsParas(entity, cmdText, cmdType);
        //    return dac.ExecuteNonQuery(cmdText, cmdType, pars);
        //}

        public virtual int Save(string cmdText, CommandType cmdType, params DbParameter[] pars)
        {
            return dac.ExecuteNonQuery( cmdText, cmdType, pars);
        }

        public virtual int Delete(string cmdText, CommandType cmdType, params DbParameter[] pars)
        {
            return dac.ExecuteNonQuery( cmdText, cmdType, pars);
        }

        //public virtual int SaveAsync(T entity)
        //{
        //    DbParameter[] pars = GetAllFieldsParas(entity, typeName + saveSP, CommandType.StoredProcedure);
        //    dac.ExecuteNonQueryOneWay( typeName + saveSP, CommandType.StoredProcedure, pars);
        //    return 1;
        //}

        //public virtual int SaveAsync(T entity, string cmdText, CommandType cmdType)
        //{
        //    DbParameter[] pars = GetAllFieldsParas(entity, cmdText, cmdType);
        //    dac.ExecuteNonQueryOneWay( cmdText, CommandType.StoredProcedure, pars);
        //    return 1;
        //}

        public virtual int SaveAsync(string cmdText, CommandType cmdType, params DbParameter[] pars)
        {
            dac.ExecuteNonQueryOneWay( cmdText, CommandType.StoredProcedure, pars);
            return 1;
        }

        //public virtual int Save(List<T> entities)
        //{
        //    int ret = 0;
        //    foreach (T ent in entities)
        //    {
        //        ret += Save(ent);
        //    }
        //    return ret;
        //}

        //public virtual int SaveAsync(List<T> entities)
        //{
        //    int ret = 0;
        //    foreach (T ent in entities)
        //    {
        //        ret += SaveAsync(ent);
        //    }
        //    return ret;
        //}

        /// <summary>
        /// 暂只支持SQL Server
        /// </summary>
        /// <param name="spName"></param>
        /// <returns></returns>
        public Dictionary<string, int> GetSPParameters(string cmdText, CommandType cmdType)
        {
            Dictionary<string, int> pars = new Dictionary<string, int>();
            if (cmdType == CommandType.StoredProcedure)
            {
                DbParameter p = dac.CreateParameter("procedure_name", cmdText);
                DataSet ds = dac.ExecuteDataSet( "ProcedurePar", "dbo.sp_sproc_columns_90", CommandType.StoredProcedure, p);
                if (null != ds && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    DataRowCollection rows = dt.Rows;
                    string pName = "";
                    foreach (DataRow row in rows)
                    {
                        pName = row[3].ToString();
                        if (!pars.ContainsKey(pName))
                            pars.Add(pName, int.Parse(row[4].ToString()));
                    }
                }
            }
            else
            {

                //基于语句的参数实现
            }
            //Log.Write("错误:没有找到对应的执行脚本:" + "->" + cmdText);
            return pars;
        }

        public virtual T Get(string cmdText, CommandType cmdType, string idName, long id)
        {
            DbParameter p = dac.CreateParameter(idName, id);
            using (IDataReader dr = dac.ExcuteDataReader( cmdText, cmdType, p))
            {
                return Build(dr);
            }
        }


        //public virtual T Get(string idName, long id)
        //{
        //    DbParameter p = dac.CreateParameter(idName, id);
        //    using (IDataReader dr = dac.ExcuteDataReader( getSP, CommandType.StoredProcedure, p))
        //    {
        //        return Build(dr);
        //    }
        //}

        //public virtual T Get(string keyName, object keyValue)
        //{
        //    DbParameter p = CreateParameter( keyName, keyValue);
        //    using (IDataReader dr = dac.ExcuteDataReader( typeName + getByKeySP, CommandType.StoredProcedure, p))
        //    {
        //        return Build(dr);
        //    }
        //}

        public virtual T GetByKey(string cmdText, CommandType cmdType, string keyName, object keyValue)
        {
            DbParameter p = CreateParameter(keyName, keyValue);
            using (IDataReader dr = dac.ExcuteDataReader( cmdText, cmdType, p))
            {
                return Build(dr);
            }
        }

        //public virtual List<T> List()
        //{
        //    List<T> list = new List<T>();
        //    using (DataSet ds = dac.ExecuteDataSet( typeName, typeName + getSP, CommandType.StoredProcedure))
        //    {
        //        if (ds.Tables.Count > 0)
        //            list = BuildList(ds.Tables[0]);
        //        return list;
        //    }
        //}

        //public  virtual List<T> List(long pageIndex, long pageSize, out long recordCount)
        //{
        //    DbParameter pidx =  CreateParameter("PageIndex",pageIndex, DbType.Int64);
        //    DbParameter pse = CreateParameter("PageSize", pageSize, DbType.Int64);
        //    DbParameter cnt =  CreateParameter("RecordCount", DbType.Int64, ParameterDirection.Output);
        //    List<T> list = new List<T>();
        //    using (DataSet ds = dac.ExecuteDataSet( typeName, typeName + pagedSP, CommandType.StoredProcedure, pidx, pse, cnt))
        //    {
        //        if (ds.Tables.Count > 0)
        //            list = BuildList(ds.Tables[0]);
        //        recordCount = (long)cnt.Value;
        //        return list;
        //    }
        //}



        public virtual List<T> List(string cmdText, CommandType cmdType, params DbParameter[] parameters)
        {
            using (IDataReader dr = dac.ExcuteDataReader( cmdText, cmdType, parameters))
            {
                if (null != dr)
                    return BuildList(dr);
                
            }
            return new List<T>();
        }



        public virtual List<T> List(string cmdText, CommandType cmdType, long pageIndex, long pageSize, params DbParameter[] parameters)
        {
            DbParameter pidx = dac.CreateParameter("PageIndex", pageIndex, DbType.Int64);
            DbParameter pse = dac.CreateParameter("PageSize", pageSize, DbType.Int64);
            //DbParameter cnt = dac.CreateParameter("RecordCount", DbType.Int64, ParameterDirection.Output);
            ArrayList ary = new ArrayList();
            ary.Add(pidx);
            ary.Add(pse);
            // ary.Add(cnt);
            if (null != parameters)
                ary.AddRange(parameters);
            List<T> list = new List<T>();
            using (DataSet ds = dac.ExecuteDataSet( typeName, cmdText, cmdType, (DbParameter[])ary.ToArray(typeof(DbParameter))))
            {
                if (ds.Tables.Count > 0)
                    list = BuildList(ds.Tables[0]);
                //recordCount = 0;
                return list;
            }
        }

        //public virtual List<T> Search(string cmdText, CommandType cmdType, long pageIndex, long pageSize, out long recordCount, params DbParameter[] parameters)
        //{

        //    DbParameter pidx = dac.CreateParameter("PageIndex", pageIndex, DbType.Int64);
        //    DbParameter pse = dac.CreateParameter("PageSize", pageSize, DbType.Int64);
        //    DbParameter cnt = dac.CreateParameter("RecordCount", DbType.Int64, ParameterDirection.Output);
        //    ArrayList ary = new ArrayList();
        //    ary.AddRange(parameters);
        //    ary.Add(pidx);
        //    ary.Add(pse);
        //    ary.Add(cnt);
        //    using (DataSet ds = dac.ExecuteDataSet( "Result",cmdText, cmdType, (DbParameter[])ary.ToArray()))
        //    {
        //        List<T> list = new List<T>();
        //        if (ds.Tables.Count > 0)
        //            list = BuildList(ds.Tables[0]);
        //        recordCount = (long)cnt.Value;
        //        return list;
        //    }
        //}

        //public virtual List<T> Search(long pageIndex, long pageSize, out long recordCount, params DbParameter[] parameters)
        //{
        //    DbParameter pidx =  CreateParameter("PageIndex", pageIndex, DbType.Int64);
        //    DbParameter pse =  CreateParameter("PageSize", pageSize, DbType.Int64);
        //    DbParameter cnt =  CreateParameter("RecordCount", DbType.Int64, ParameterDirection.Output);
        //    ArrayList ary = new ArrayList();
        //    ary.AddRange(parameters);
        //    ary.Add(pidx);
        //    ary.Add(pse);
        //    ary.Add(cnt);
        //    using (DataSet ds = dac.ExecuteDataSet( "Result", typeName+searchSP,CommandType.StoredProcedure, (DbParameter[])ary.ToArray()))
        //    {
        //        List<T> list = new List<T>();
        //        if (ds.Tables.Count > 0)
        //            list = BuildList(ds.Tables[0]);
        //        recordCount = (long)cnt.Value;
        //        return list;
        //    }
        //}



        protected List<T> BuildList(DataTable dt)
        {
            return EntityBuilder<T>.BuildList(dt);
        }

        protected T Build(DataTable dt)
        {
            return EntityBuilder<T>.Build(dt);
        }

        protected T Build(DataRow row)
        {
            return EntityBuilder<T>.Build(row);
        }

        protected List<T> BuildList(IDataReader dr)
        {
            return EntityBuilder<T>.BuildList(dr);
        }

        protected T Build(IDataReader dr)
        {
            return EntityBuilder<T>.Build(dr);
        }


        public DbParameter CreateParameter(string name, object value)
        {
            return DataHelper.CreateParameter(_providerName, name, value);
        }

        public DbParameter CreateParameter(string name, ParameterDirection dirction)
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


    }
}
