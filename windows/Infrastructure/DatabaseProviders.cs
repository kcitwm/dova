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
    public class DatabaseProviders  
    {

        public static string SqlProvider = "System.Data.SqlClient";
        public static string OracleProvider = "System.Data.OracleClient";
        public static string ODPProvider = "Oracle.DataAccess.OracleClient";


    }
}
