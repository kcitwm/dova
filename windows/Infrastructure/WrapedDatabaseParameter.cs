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
    public class WrapedDatabaseParameter
    { 
        protected string _connectionString = Config.DefaultConnectionName;
        [DataMember]
        public string ConnectionString
        {
            get {

                return _connectionString;
            }
            set { _connectionString = value; }
        }
        protected string _tableName = "DataSet";
        [DataMember(IsRequired = false)]
        public string TableName { get { return _tableName; } set { if(null!=value) _tableName = value; } }
        [DataMember]
        public string CmdText { get; set; }
        [DataMember(IsRequired = false)]
        public int CmdType { get; set; }
        [DataMember(IsRequired = false)]
        public DatabaseParameter[] DatabaseParameters{get;set;}

        string _routingKey = string.Empty; 
        [DataMember(IsRequired = false)]
        public string RoutingKey { get { return _routingKey??string.Empty; } set { if (null != value) _routingKey = value; } }

        public override string ToString()
        { 
            return "ConnectionString:" + ConnectionString + ";TableName:" + _tableName + ";CmdText:" + CmdText + ";CmdType:" + CmdType +";RoutingKey:"+RoutingKey+";DatabaseParameters:" +(DatabaseParameters==null?"": DatabaseParameters.ToKeyString());
        }

    }
}
