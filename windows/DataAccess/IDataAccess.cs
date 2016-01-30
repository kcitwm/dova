using System;
using System.Net.Security;
using System.ServiceModel;
using System.Data;
using System.Data.SqlClient;
using System.ServiceModel.Web;
using System.Collections.Generic;

namespace Dova.Data
{
    [ServiceContract(ProtectionLevel = ProtectionLevel.None)]
    public interface IDataAccess
    {

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/Login", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string Login(LoginReq req);
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/Regist", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        int Regist(LoginReq req);
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/Logout", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        int Logout(string userName);
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/CheckLogin", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        int CheckLogin(string userName);


        [OperationContract(Name = "ExecuteDataReader2")]
        [WebInvoke(Method = "POST", UriTemplate = "/ExcuteDataReader2", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        IDataReader ExecuteDataReader(string connectionString, string cmdText, CommandType cmdType, params DatabaseParameter[] parameters);

        [OperationContract(Name = "ExecuteNonQuery2")]
        [WebInvoke(Method = "POST", UriTemplate = "/ExecuteNonQuery2", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        int ExecuteNonQuery(string connectionString, string cmdText, CommandType cmdType, params DatabaseParameter[] parameters);

        [OperationContract(Name = "ExecuteScalar2")]
        [WebInvoke(Method = "POST", UriTemplate = "/ExecuteScalar2", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        object ExecuteScalar(string connectionString, string cmdText, CommandType cmdType, params DatabaseParameter[] parameters);

        [OperationContract(Name = "ExecuteDataSet2")]
        [WebInvoke(Method = "POST", UriTemplate = "/ExecuteDataSet2", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        DataSet ExecuteDataSet(string tableName, string connectionString, string cmdText, CommandType cmdType, params DatabaseParameter[] parameters);

        [OperationContract(Name = "ExecuteDataList2")]
        [WebInvoke(Method = "POST", UriTemplate = "/ExecuteDataList2", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string ExecuteDataList(string connectionString, string cmdText, CommandType cmdType, params DatabaseParameter[] parameters);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/Fill2", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        int Fill(string connectionString, DataSet ds, string cmdText, CommandType cmdType, params DatabaseParameter[] parameters);

        [OperationContract(IsOneWay = true)]
        [WebInvoke(Method = "POST", UriTemplate = "/ExecuteNonQueryOneWay2", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        void ExecuteNonQueryOneWay(string connectionString, string cmdText, CommandType cmdType, params DatabaseParameter[] parameters);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AdapterSave2", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        int AdapterSave(string connectionString, DataSet ds, string insertCmdText, string updateCmdText, string deleteCmdText, CommandType cmdType, params DatabaseParameter[] parameters);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/SqlBulkInsertDataReader2", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        bool SqlBulkInsertDataReader(string connectionString, string targetTable, IDataReader dr);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/SqlBulkInsertDataTable2", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        bool SqlBulkInsertDataTable(string connectionString, string targetTable, DataTable dt);

        [OperationContract(Name = "SqlBulkInsertDataReaderWithMapping")]
        [WebInvoke(Method = "POST", UriTemplate = "/SqlBulkInsertDataReaderWithMapping2", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        bool SqlBulkInsertDataReader(string connectionString, string targetTable, IDataReader dr, SqlBulkCopyColumnMapping[] mappings);

        [OperationContract(Name = "SqlBulkInsertDataTableWithMapping")]
        [WebInvoke(Method = "POST", UriTemplate = "/SqlBulkInsertDataTableWithMapping2", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        bool SqlBulkInsertDataTable(string connectionString, string targetTable, DataTable dt, SqlBulkCopyColumnMapping[] mappings);


        [OperationContract(Name = "ExecuteNonQuery")]
        [WebInvoke(Method = "POST", UriTemplate = "/ExecuteNonQuery", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        int ExecuteNonQuery(params WrapedDatabaseParameter[] parms);


        [OperationContract(Name = "ExecuteScalar")]
        [WebInvoke(Method = "POST", UriTemplate = "/ExecuteScalar", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        object ExecuteScalar(WrapedDatabaseParameter parms);

        [OperationContract(Name = "ExecuteDataSet")]
        [WebInvoke(Method = "POST", UriTemplate = "/ExecuteDataSet", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        DataSet ExecuteDataSet(WrapedDatabaseParameter parms);

        [OperationContract(Name = "ExecuteDataTable")]
        [WebInvoke(Method = "POST", UriTemplate = "/ExecuteDataTable", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        string ExecuteDataTable(WrapedDatabaseParameter parms);

        [OperationContract(Name = "ExecuteDataList")]
        [WebInvoke(Method = "POST", UriTemplate = "/ExecuteDataList", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        string ExecuteDataList(WrapedDatabaseParameter parms);

        [OperationContract(Name = "ExecutePagedDataSet2")]
        [WebInvoke(Method = "POST", UriTemplate = "/ExecutePagedDataSet2", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        DataSet ExecutePagedDataSet(string connString, string dsTableName, string tableName, string fields, string orderBy, long pageIndex, long pageSize, long recordCount, string where);

        [OperationContract(Name = "ExecutePagedDataSet")]
        [WebInvoke(Method = "POST", UriTemplate = "/ExecutePagedDataSet", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        DataSet ExecutePagedDataSet(PagedRecordParameter prp);


        [OperationContract(Name = "ExecutePagedDataList")]
        [WebInvoke(Method = "POST", UriTemplate = "/ExecutePagedDataList", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        string ExecutePagedDataList(PagedRecordParameter prp);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/Test", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        string Test(string msg);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/Test2", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        string Test2(CommandType cmdType);

         [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/SaveFile", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        string SaveFile( string name, byte[] img);

         [OperationContract]
         [WebInvoke(Method = "POST", UriTemplate = "/GetFile", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
         byte[] GetFile( string name);



    }
}
