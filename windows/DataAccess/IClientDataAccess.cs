using System;
using System.Net.Security;
using System.ServiceModel;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.Common;

namespace Dova.Data
{
    public interface IClientDataAccess
    {
        IDataReader ExcuteDataReader(string cmdText, CommandType cmdType, params DbParameter[] parameters);
        int ExecuteNonQuery(string cmdText, CommandType cmdType, params DbParameter[] parameters);
        void ExecuteNonQueryOneWay(string cmdText, CommandType cmdType, params DbParameter[] parameters);
        object ExecuteScalar(string cmdText, CommandType cmdType, params DbParameter[] parameters);
        DataSet ExecuteDataSet(string tableName, string cmdText, CommandType cmdType, params DbParameter[] parameters);
        string ExecuteDataList(string cmdText, CommandType cmdType, params DbParameter[] parameters);
        int Fill(DataSet ds, string cmdText, CommandType cmdType, params DbParameter[] parameters);
        int AdapterSave(DataSet ds, string insertCmdText, string updateCmdText, string deleteCmdText, CommandType cmdType, params DbParameter[] parameters);
        bool SqlBulkInsertDataReader(string targetTable, IDataReader dr);
        bool SqlBulkInsertDataTable(string targetTable, DataTable dt);
        bool SqlBulkInsertDataReader(string targetTable, IDataReader dr, SqlBulkCopyColumnMapping[] mappings);
        bool SqlBulkInsertDataTable(string targetTable, DataTable dt, SqlBulkCopyColumnMapping[] mappings);
        string SaveFile(string name, byte[] img);
        byte[] GetFile(string name);
    }
}
