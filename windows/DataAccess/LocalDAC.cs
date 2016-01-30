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
using System.Collections.Generic;
using System.Xml;
using System.IO; 

namespace Dova.Data
{
    public class LocalDAC : Dova.Data.IClientDataAccess
    {
        [System.Diagnostics.Conditional("DEBUG")]
        void WriteLine(string msg)
        {
            Console.WriteLine(msg + " ThreadId:" + System.Threading.Thread.CurrentThread.ManagedThreadId);
        }

        static string className = "LocalDAC";
        static string appName = Config.AppName;

        protected string _connString;
        public string ConnString
        {
            get { return _connString; }
            set { _connString = value; }
        }

        protected string _providerName;
        public string ProviderName
        {
            get { return _providerName; }
            set { _providerName = value; }
        }

        public LocalDAC(string connectionString, string providerName)
        {
            _connString = connectionString;
            _providerName = providerName;
        }

        public IDataReader ExcuteDataReader(string cmdText, CommandType cmdType, params DbParameter[] parameters)
        {
            DataHelper hp = new DataHelper(_connString, _providerName);
            return hp.ExcuteDataReader(cmdText, cmdType, parameters); 
        }

        public int ExecuteNonQuery(string cmdText, CommandType cmdType, params DbParameter[] parameters)
        {
            DataHelper hp = new DataHelper(_connString, _providerName);
            int rtn = hp.ExecuteNonQuery(cmdText, cmdType, parameters);
            return rtn;
        }

        public void ExecuteNonQueryOneWay(string cmdText, CommandType cmdType, params DbParameter[] parameters)
        {
            ExecuteNonQuery(cmdText, cmdType, parameters);
        }

        public object ExecuteScalar(string cmdText, CommandType cmdType, params DbParameter[] parameters)
        {
            DataHelper hp = new DataHelper(_connString, _providerName);
            return hp.ExecuteScalar(cmdText, cmdType, parameters);

        }

        public DataSet ExecuteDataSet(string tableName, string cmdText, CommandType cmdType, params DbParameter[] parameters)
        {
            DataHelper hp = new DataHelper(_connString, _providerName);
            return hp.ExecuteDataSet(tableName, cmdText, cmdType, parameters);
        }


        public string ExecuteDataList(string cmdText, CommandType cmdType, params DbParameter[] parameters)
        {
            DataHelper hp = new DataHelper(_connString, _providerName);
            return hp.ExecuteDataList(cmdText, cmdType, parameters);
        }


        public int Fill(DataSet ds, string cmdText, CommandType cmdType, params DbParameter[] parameters)
        {
            DataHelper hp = new DataHelper(_connString, _providerName);
            return hp.Fill(ds, cmdText, cmdType, parameters); 
        }

        public int AdapterSave(DataSet ds, string insertCmdText, string updateCmdText, string deleteCmdText, CommandType cmdType, params DbParameter[] parameters)
        {
            DataHelper hp = new DataHelper(_connString, _providerName);
            return  hp.AdapterSave(ds, insertCmdText, updateCmdText, deleteCmdText, cmdType, parameters); 
        }

        public bool SqlBulkInsertDataReader(string targetTable, IDataReader dr)
        {
            DataHelper hp = new DataHelper(_connString, _providerName);
            return  hp.SqlBulkInsertDataReader(targetTable, dr); 
        }

        public bool SqlBulkInsertDataTable(string targetTable, DataTable dt)
        {
            DataHelper hp = new DataHelper(_connString, _providerName);
            return  hp.SqlBulkInsertDataTable(targetTable, dt); 
        }

        public bool SqlBulkInsertDataReader(string targetTable, IDataReader dr, SqlBulkCopyColumnMapping[] mappings)
        {
            DataHelper hp = new DataHelper(_connString, _providerName);
            return hp.SqlBulkInsertDataReader(targetTable, dr, mappings); 
        }

        public bool SqlBulkInsertDataTable(string targetTable, DataTable dt, SqlBulkCopyColumnMapping[] mappings)
        {
            DataHelper hp = new DataHelper(_connString, _providerName);
            return hp.SqlBulkInsertDataTable(targetTable, dt, mappings);
        }



        #region 图片处理
        public string SaveFile(  string name, byte[] img)
        {
            return Dova.Files.FileHelper.SaveFile(name, img);
        }

        public byte[] GetFile(string name)
        {
            return Dova.Files.FileHelper.GetFile(name);
        }

        #endregion


    }
}
