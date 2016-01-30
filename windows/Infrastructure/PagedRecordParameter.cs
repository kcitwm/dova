using System;
using System.Data;
using System.Reflection;
using System.Data.Common;
using System.ServiceModel;
using System.Configuration;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.Serialization;


namespace Dova
{
    [Serializable]
    [DataContract]
    public class PagedRecordParameter 
    {
        protected string _connectionString = Config.DefaultConnectionName;
        [DataMember]
        public string ConnectionString
        {
            get
            {

                return _connectionString;
            }
            set { _connectionString = value; }
        }

        protected string _tableName = "DataSet";
        [DataMember(IsRequired = false)]
        public string TableName { get { return _tableName; } set { if (null != value) _tableName = value; } }

        string _dsTableName = "DataTable";
        [DataMember]
        public string DSTableName { get { return _dsTableName; } set { if (null != value) _dsTableName = value; } }

        string fields = "*";
        [DataMember]
        public string Fields { get { return fields; } set { if (null != value) fields = value; } }

        string orderBy = string.Empty;
        [DataMember]
        public string OrderBy { get { return orderBy; } set { if (null != value) orderBy = value; } }

        string where = string.Empty;
        [DataMember]
        public string Where { get { return where; } set { if (null != value) where = value; } }

        [DataMember]
        public long PageIndex { get; set; }
        [DataMember]
        public long PageSize { get; set; }
        [DataMember]
        public long RecordCount { get; set; }

        string _routingKey = string.Empty;
        [DataMember(IsRequired = false)]
        public string RoutingKey { get { return _routingKey ?? string.Empty; } set { if (null != value) _routingKey = value; } }

        public override string ToString()
        {
            return "ConnectionString:" + ConnectionString + ";TableName:" + _tableName + ";CmdText:PagedRecordSP;CmdType:" + CommandType.StoredProcedure + ";RoutingKey:" + RoutingKey;
        }

    }
}
