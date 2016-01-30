using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Net.Security;
using System.ServiceModel; 

namespace Dova.Interfaces
{
    [ServiceContract(ProtectionLevel = ProtectionLevel.None)]
    public interface IClientDataAccess
    { 
        [OperationContract]
        IDataReader ExcuteDataReader(string connString,string providerName, string cmdText, CommandType cmdType, params DbParameter[] parameters);
        [OperationContract]
        int  ExecuteNonQuery(string connString,string providerName,  string cmdText, CommandType cmdType, params DbParameter[] parameters);
        [OperationContract]
        object ExecuteScalar(string connString,string providerName,  string cmdText, CommandType cmdType, params DbParameter[] parameters);
        [OperationContract]
        DataSet ExecuteDataSet(string connString,string providerName,  string tableName, string cmdText, CommandType cmdType, params DbParameter[] parameters);
        [OperationContract]
        int Fill(string connString,string providerName,  DataSet ds, string cmdText, CommandType cmdType, params DbParameter[] parameters);

        [OperationContract(IsOneWay=true)] 
        void  ExecuteNonQueryOneWay(string connString,string providerName,  string cmdText, CommandType cmdType, params DbParameter[] parameters);


        [OperationContract]
        int AdapterSave(string connString,string providerName,  DataSet ds, string insertCmdText, string updateCmdText, string deleteCmdText, CommandType cmdType, params DbParameter[] parameters);

        [OperationContract]
        bool SqlBulkInsertDataReader(string connString,string providerName,  string targetTable, IDataReader dr);

        [OperationContract]
        bool SqlBulkInsertDataTable(string connString,string providerName,  string targetTable, DataTable dt);


        [OperationContract(Name = "SqlBulkInsertDataReaderWithMapping")]
        bool SqlBulkInsertDataReader(string connString,string providerName,  string targetTable, IDataReader dr, SqlBulkCopyColumnMapping[] mappings);

        [OperationContract(Name = "SqlBulkInsertDataTableWithMapping")]
        bool SqlBulkInsertDataTable(string connString,string providerName,  string targetTable, DataTable dt, SqlBulkCopyColumnMapping[] mappings);


        [OperationContract]
        string Test(string msg);


    }
}
