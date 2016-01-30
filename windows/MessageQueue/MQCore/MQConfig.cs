using System;
using System.Configuration;

namespace Dova.MessageQueue
{
    public class MQConfigs : Config
    {

        public static ConnectionStringSettings MQConnection = null;
        public static string MQConnectionName = "MQConnection";
        public static string MQProviderName = "System.Data.SqlClient";
        static MQConfigs()
        {
            //MQConnectionName = Config.Get(MQConnectionName, MQConnectionName);
            //if (MQConnectionName == "")
            //    MQConnectionName = Config.Get(Config.DefaultConnectionName, "DefaultConnection");
        }
    }
}
