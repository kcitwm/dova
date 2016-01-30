using System;
using System.Data;
using System.Reflection;
using System.Data.Common;
using System.ServiceModel;
using System.Configuration;
using System.Data.OracleClient;
using System.Collections.Generic;

using Dova.Utility;
using Dova.Services;
using System.Data.SqlClient;


namespace Dova.Data
{
    public class DataHelper
    {
        protected DbProviderFactory factory;
        protected bool autoClose = true;
        static Dictionary<string, DbProviderFactory> factories = new Dictionary<string, DbProviderFactory>();
        static DbProviderFactory GetFactory(string factoryName)
        {
            //if (factories.ContainsKey(factoryName))
            //    return factories[factoryName];
            DbProviderFactory fac = DbProviderFactories.GetFactory(factoryName);
            //factories[factoryName] = fac;
            return fac;
        }

        public bool AutoClose
        {
            get { return autoClose; }
            set { autoClose = value; }
        }
         
        string _connectionString;
        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        string _providerName = "System.Data.SqlClient";
        public string ProviderName
        {
            get { return _providerName; }
            set { _providerName = value; }
        }

        public DataHelper()
        {
        }

        public DataHelper(string providerName)
        {
            _providerName = providerName;
        } 

        public DataHelper(string connStrings, string providerName)
        {
            _connectionString = connStrings;
            _providerName = providerName;
            factory = GetFactory(providerName);
        }

        #region 

        public int ExecuteNonQuery(string cmdText, CommandType cmdType, params IDataParameter[] parameters)
        {
            int n = -1;
            using (DbConnection conn = factory.CreateConnection())
            {
                conn.ConnectionString = _connectionString;
                conn.Open();
                DbCommand cmd = conn.CreateCommand() as DbCommand;
                cmd.CommandText = cmdText;
                cmd.CommandType = cmdType;
                //cmd.Prepare();
                cmd.Parameters.AddRange(parameters);
                n = cmd.ExecuteNonQuery();
            }
            return n;
        } 

        public virtual IDataReader ExcuteDataReader(string cmdText, CommandType cmdType, params IDataParameter[] parameters)
        {
            DbConnection conn = factory.CreateConnection();
            conn.ConnectionString = _connectionString;
            conn.Open();
            DbCommand cmd = conn.CreateCommand() as DbCommand;
            cmd.CommandText = cmdText;
            cmd.CommandType = cmdType;
            cmd.Parameters.AddRange(parameters);
            //cmd.Prepare();
            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public object ExecuteScalar(string cmdText, CommandType cmdType, params IDataParameter[] parameters)
        {
            using (DbConnection conn = factory.CreateConnection())
            {
                conn.ConnectionString = _connectionString;
                conn.Open();
                DbCommand cmd = conn.CreateCommand() as DbCommand;
                cmd.CommandText = cmdText;
                cmd.CommandType = cmdType;
                cmd.Parameters.AddRange(parameters);
                //cmd.Prepare();
                object o = cmd.ExecuteScalar();
                return o;
            }
        }

        public DataSet ExecuteDataSet(string tableName, string cmdText, CommandType cmdType, params IDataParameter[] parameters)
        {
            DataSet ds = new DataSet(tableName);
            using (DbConnection conn = factory.CreateConnection())
            {
                conn.ConnectionString = _connectionString;
                conn.Open();
                DbCommand cmd = conn.CreateCommand() as DbCommand;
                cmd.CommandText = cmdText;
                cmd.CommandType = cmdType;
                //cmd.Prepare();
                cmd.Parameters.AddRange(parameters);
                DbDataAdapter da = factory.CreateDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(ds);
            }
            return ds;
        } 

        public string ExecuteDataList(string cmdText, CommandType cmdType, params IDataParameter[] parameters)
        {
            string list = "[]";
            using (DbConnection conn = factory.CreateConnection())
            {
                conn.ConnectionString = _connectionString;
                conn.Open();
                DbCommand cmd = conn.CreateCommand() as DbCommand;
                cmd.CommandText = cmdText;
                cmd.CommandType = cmdType;
                //cmd.Prepare();
                cmd.Parameters.AddRange(parameters);
                DbDataAdapter da = factory.CreateDataAdapter();
                da.SelectCommand = cmd;
                DataSet ds = new DataSet();
                da.Fill(ds);
               // if (ds.Tables.Count == 1)
                    list = Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables[0]);
                    Log.Dac("DataHelper.ExecuteDataList:" + list);
                //else
                  //  list = Newtonsoft.Json.JsonConvert.SerializeObject(ds.Tables);

            }
            return list;
        }
         
        public int Fill(DataSet ds, string cmdText, CommandType cmdType, params IDataParameter[] parameters)
        {
            int n = -1;
            using (DbConnection conn = factory.CreateConnection())
            {
                conn.ConnectionString = _connectionString;
                conn.Open();
                DbCommand cmd = conn.CreateCommand() as DbCommand;
                cmd.CommandText = cmdText;
                cmd.CommandType = cmdType;
                //cmd.Prepare();
                cmd.Parameters.AddRange(parameters);
                DbDataAdapter da = factory.CreateDataAdapter();
                da.SelectCommand = cmd;
                da.AcceptChangesDuringFill = false;
                n = da.Fill(ds);
            }
            return n;
        }
 
        public int AdapterSave(DataSet ds, string insertCmdText, string updateCmdText, string deleteCmdText, CommandType cmdType, params IDataParameter[] parameters)
        {
            int n = -1;
            using (DbConnection conn = factory.CreateConnection())
            {
                SqlConnection sconn = conn as SqlConnection;
                SqlDataAdapter da = new SqlDataAdapter();
                SqlCommand cmd = null;
                if (!string.IsNullOrEmpty(insertCmdText))
                {
                    cmd = sconn.CreateCommand();
                    cmd.CommandText = insertCmdText;
                    cmd.CommandType = cmdType;
                    da.InsertCommand = cmd;
                }
                if (!string.IsNullOrEmpty(updateCmdText))
                {
                    cmd = sconn.CreateCommand();
                    cmd.CommandText = updateCmdText;
                    cmd.CommandType = cmdType;
                    da.UpdateCommand = cmd;
                }
                if (!string.IsNullOrEmpty(deleteCmdText))
                {
                    cmd = sconn.CreateCommand();
                    cmd.CommandText = deleteCmdText;
                    cmd.CommandType = cmdType;
                    da.DeleteCommand = cmd;
                }
                da.UpdateBatchSize = 1000;
                n = da.Update(ds);
            }
            return n;
        }
 
        public bool SqlBulkInsertDataReader(string targetTable, IDataReader dr)
        {
            using (SqlBulkCopy sbc = new SqlBulkCopy(_connectionString))
            {
                sbc.DestinationTableName = targetTable;
                sbc.WriteToServer(dr);
                return true;
            }
        }

        public bool SqlBulkInsertDataTable(string targetTable, DataTable dt)
        {
            using (SqlBulkCopy sbc = new SqlBulkCopy(_connectionString))
            {
                sbc.DestinationTableName = targetTable;
                sbc.WriteToServer(dt);
                return true;
            }
        }

        public bool SqlBulkInsertDataReader(string targetTable, IDataReader dr, SqlBulkCopyColumnMapping[] mappings)
        {
            using (SqlBulkCopy sbc = new SqlBulkCopy(_connectionString))
            {
                sbc.DestinationTableName = targetTable;
                foreach (SqlBulkCopyColumnMapping map in mappings)
                    sbc.ColumnMappings.Add(map);
                sbc.WriteToServer(dr);
                return true;
            }
        }

        public bool SqlBulkInsertDataTable(string targetTable, DataTable dt, SqlBulkCopyColumnMapping[] mappings)
        {
            using (SqlBulkCopy sbc = new SqlBulkCopy(_connectionString))
            {
                sbc.DestinationTableName = targetTable;
                foreach (SqlBulkCopyColumnMapping map in mappings)
                    sbc.ColumnMappings.Add(map);
                sbc.WriteToServer(dt);
                return true;
            }
        }
         
        #endregion

        public static DbParameter CreateParameter(string providerName)
        {
            DbProviderFactory fac = GetFactory(providerName);
            DbParameter p = fac.CreateParameter();
            return p;
        }

        public static DbParameter CreateParameter(string providerName, string name, object value)
        {
            DbProviderFactory fac = GetFactory(providerName);
            DbParameter p = fac.CreateParameter();
            p.ParameterName = name;
            p.Value = value;
            return p;
        }

        public static DbParameter CreateParameter(string providerName, string name, ParameterDirection direction)
        {
            DbProviderFactory fac = GetFactory(providerName);
            DbParameter p = fac.CreateParameter();
            p.ParameterName = name;
            p.Direction = direction;
            return p;
        }

        public static DbParameter CreateParameter(string providerName, string name, object value, DbType type)
        {
            DbProviderFactory fac = GetFactory(providerName);
            DbParameter p = fac.CreateParameter();
            p.DbType = type;
            p.Value = value;
            p.ParameterName = name;
            return p;
        }

        public static DbParameter CreateParameter(string providerName, string name, DbType type, ParameterDirection direction)
        {
            DbProviderFactory fac = GetFactory(providerName);
            DbParameter p = fac.CreateParameter();
            p.DbType = type;
            p.Direction = direction;
            p.ParameterName = name;
            return p;
        }
        public static DbParameter CreateParameter(string providerName, string name, ParameterDirection direction, DbType type, int size)
        {
            DbProviderFactory fac = GetFactory(providerName);
            DbParameter p = fac.CreateParameter();
            p.DbType = type;
            p.Direction = direction;
            p.ParameterName = name;
            p.Size = size;
            return p;
        }
        public static DbParameter CreateParameter(string providerName, string name, ParameterDirection direction, int size)
        {
            DbProviderFactory fac = GetFactory(providerName);
            DbParameter p = fac.CreateParameter();
            p.Direction = direction;
            p.ParameterName = name;
            p.Size = size;
            return p;
        }
        public static DbParameter CreateParameter(string providerName, string name, DbType type, int size, object value)
        {
            DbProviderFactory fac = GetFactory(providerName);
            DbParameter p = fac.CreateParameter();
            p.ParameterName = name;
            p.Size = size;
            p.DbType = type;
            p.Value = value;
            return p;
        }
        public static DbParameter CreateParameter(string providerName, string name, DbType type, ParameterDirection direction, object value)
        {
            DbProviderFactory fac = GetFactory(providerName);
            DbParameter p = fac.CreateParameter();
            p.ParameterName = name;
            p.DbType = type;
            p.Direction = direction;
            p.Value = value;
            return p;
        }
        public static DbParameter CreateParameter(string providerName, string name, DbType type, ParameterDirection direction, int size, object value)
        {
            DbProviderFactory fac = GetFactory(providerName);
            DbParameter p = fac.CreateParameter();
            p.ParameterName = name;
            p.DbType = type;
            p.Direction = direction;
            p.Size = size;
            p.Value = value;
            return p;
        }
        
    }
}
