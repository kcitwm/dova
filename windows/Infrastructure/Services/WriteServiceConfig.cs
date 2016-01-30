using System;
using System.Xml;
using System.Configuration;
using System.Xml.Serialization; 
using System.Collections.Generic;

using WQFree.Utility;

namespace WQFree.Services
{
    [Serializable, XmlRoot("DACWriteService")]
    public class WriteServiceConfig : IConfigurationSectionHandler
    { 

        public      List<ServiceConfigItem> ConfigItems;          

        public static List<ServiceConfigItem>  GetConfigs(string configGroup)
        {
            WriteServiceConfig instance = ConfigurationManager.GetSection(configGroup) as WriteServiceConfig;
            if (null == instance) return null;
            return instance.ConfigItems;
        }

        public object Create(object parent, object configContext, XmlNode section)
        {
            try
            {
                string typeName = (string) section.CreateNavigator().Evaluate("string(@type)");
                XmlSerializer serializer = new XmlSerializer(Type.GetType(typeName));
                return serializer.Deserialize(new XmlNodeReader(section));
            }
            catch(Exception e)
            {
                Log.Error("WriteServiceConfig.Create;" + e.Message);
            }
            return null;
        }


    }
}
