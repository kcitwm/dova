using System;
using System.Data;
using System.Data.Common;
using System.ServiceModel;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using Dova.Data;
using Dova.Utility;
using Dova.Interfaces;
using Dova.Services;
using System.Collections.Generic;

namespace Dova.Data
{
    public class RemoteDAC : Dova.Data.IClientDataAccess
    {
        protected string _connString;
        public string ConnString
        {
            get { return _connString; }
            set { _connString = value; }
        }

        protected string _serviceName;
        public string ServiceName
        {
            get { return _serviceName; }
            set { _serviceName = value; }
        } 

        public RemoteDAC(string connectionName)
        {
            _connString = connectionName;
            _serviceName = connectionName;
        }

        public RemoteDAC(string serviceName,string connectionName)
        {
            _connString = connectionName;
            _serviceName = serviceName;
        } 


        public  IDataReader ExcuteDataReader(string cmdText, CommandType cmdType, params DbParameter[] parameters)
        {
            using(IDisposable service=ServiceFactory<Dova.Data.IDataAccess>.GetServer(_serviceName).Instance() as IDisposable)
            { 
                DatabaseParameter[] ps = ConvertParameter(parameters);
                return (service as IDataAccess).ExecuteDataSet(_connString, "Table1", cmdText, cmdType, ps).CreateDataReader();
            }
        }

        public  int ExecuteNonQuery(string cmdText, CommandType cmdType, params DbParameter[] parameters)
        {
            using(IDisposable service=ServiceFactory<Dova.Data.IDataAccess>.GetServer(_serviceName).Instance() as IDisposable)
            {
                DatabaseParameter[] ps = ConvertParameter(parameters);
                return (service as IDataAccess).ExecuteNonQuery(_connString, cmdText, cmdType, ps);
            }
        }

        public  void ExecuteNonQueryOneWay(string cmdText, CommandType cmdType, params DbParameter[] parameters)
        {
            using(IDisposable service=ServiceFactory<Dova.Data.IDataAccess>.GetServer(_serviceName).Instance() as IDisposable)
            {
                DatabaseParameter[] ps = ConvertParameter(parameters);
                (service as IDataAccess).ExecuteNonQueryOneWay(_connString, cmdText, cmdType, ps);
            }
        }

        public  object ExecuteScalar(string cmdText, CommandType cmdType, params DbParameter[] parameters)
        {
            using(IDisposable service=ServiceFactory<Dova.Data.IDataAccess>.GetServer(_serviceName).Instance() as IDisposable)
            {
                DatabaseParameter[] ps = ConvertParameter(parameters);
                return (service as IDataAccess).ExecuteScalar(_connString, cmdText, cmdType, ps);
            }
        }

        public   DataSet ExecuteDataSet(string tableName, string cmdText, CommandType cmdType, params DbParameter[] parameters)
        {
            using(IDisposable service=ServiceFactory<Dova.Data.IDataAccess>.GetServer(_serviceName).Instance() as IDisposable)
            {
                DatabaseParameter[] ps = ConvertParameter(parameters);
                return (service as IDataAccess).ExecuteDataSet( tableName, _connString,cmdText, cmdType, ps);
            }
        }

        public string ExecuteDataList(string cmdText, CommandType cmdType, params DbParameter[] parameters)
        {
            using (IDisposable service = ServiceFactory<Dova.Data.IDataAccess>.GetServer(_serviceName).Instance() as IDisposable)
            {
                DatabaseParameter[] ps = ConvertParameter(parameters);
                return (service as IDataAccess).ExecuteDataList(_connString, cmdText, cmdType, ps);
            }
        }


        public  int Fill(DataSet ds, string cmdText, CommandType cmdType, params DbParameter[] parameters)
        {
            using(IDisposable service=ServiceFactory<Dova.Data.IDataAccess>.GetServer(_serviceName).Instance() as IDisposable)
            {
                DatabaseParameter[] ps = ConvertParameter(parameters);
                return (service as IDataAccess).Fill(_connString, ds, cmdText, cmdType, ps);
            }
        }

        public  int AdapterSave(DataSet ds, string insertCmdText, string updateCmdText, string deleteCmdText, CommandType cmdType, params DbParameter[] parameters)
        {
            using(IDisposable service=ServiceFactory<Dova.Data.IDataAccess>.GetServer(_serviceName).Instance() as IDisposable)
            {
                DatabaseParameter[] ps = ConvertParameter(parameters);
                return (service as IDataAccess).AdapterSave(_connString, ds, insertCmdText, updateCmdText, deleteCmdText, cmdType, ps);
            }
        }

        public  bool SqlBulkInsertDataReader(string targetTable, IDataReader dr)
        {
            using(IDisposable service=ServiceFactory<Dova.Data.IDataAccess>.GetServer(_serviceName).Instance() as IDisposable)
            {
                return (service as IDataAccess).SqlBulkInsertDataReader(_connString, targetTable, dr);
            }
        }

        public bool SqlBulkInsertDataTable(string targetTable, DataTable dt)
        {
            using(IDisposable service=ServiceFactory<Dova.Data.IDataAccess>.GetServer(_serviceName).Instance() as IDisposable)
            {
                return (service as IDataAccess).SqlBulkInsertDataTable(_connString, targetTable, dt);
            }
        }


        public  bool SqlBulkInsertDataReader(string targetTable, IDataReader dr, SqlBulkCopyColumnMapping[] mappings)
        {
            using(IDisposable service=ServiceFactory<Dova.Data.IDataAccess>.GetServer(_serviceName).Instance() as IDisposable)
            {
                return (service as IDataAccess).SqlBulkInsertDataReader(_connString, targetTable, dr, mappings);
            }
        }

        public  bool SqlBulkInsertDataTable(string targetTable, DataTable dt, SqlBulkCopyColumnMapping[] mappings)
        {
            using(IDisposable service=ServiceFactory<Dova.Data.IDataAccess>.GetServer(_serviceName).Instance() as IDisposable)
            {
                return (service as IDataAccess).SqlBulkInsertDataTable(_connString, targetTable, dt, mappings);
            }
        }

        protected   DatabaseParameter[] ConvertParameter(DbParameter[] parameters)
        {
            int len = parameters.Length;
            DatabaseParameter[] paras = new DatabaseParameter[len];
            //DatabaseParameter p = null;
            DatabaseParameter p1 = null;
            int i = 0;
            foreach (DbParameter p in parameters)
            {
                p1 = new DatabaseParameter(); 
                if (p is SqlParameter)
                    p1.ProviderName = DatabaseProviders.SqlProvider;
                p1.DbType = p.DbType;
                p1.Direction = p.Direction;
                p1.ParameterName = p.ParameterName;
                p1.Size = p.Size;
                p1.SourceColumn = p.SourceColumn;
                p1.SourceColumnNullMapping = p.SourceColumnNullMapping;
                p1.SourceVersion = p.SourceVersion;
                p1.Value = p.Value;
                paras[i] = p1;
                i++;
                Log.Dac("ConvertParameter:" + p1.ToKeyString());
            }
            return paras;
        }

        public string SaveFile(string name, byte[] img)
        {
            using (IDisposable service = ServiceFactory<Dova.Data.IDataAccess>.GetServer(_serviceName).Instance() as IDisposable)
            {
                return (service as IDataAccess).SaveFile(name, img);
            }
        }

        public byte[] GetFile(string name)
        {
            using (IDisposable service = ServiceFactory<Dova.Data.IDataAccess>.GetServer(_serviceName).Instance() as IDisposable)
            {
                return (service as IDataAccess).GetFile(name);
            }
        }

    }
}
