using System;
using System.Xml;
using System.Configuration;
using System.Xml.Serialization;
using System.Collections.Generic;

using Dova.Utility;

namespace Dova.Services
{
    [Serializable]
    public class ServiceConfig : IConfigurationSectionHandler
    {

        public List<ServiceConfigItem> ConfigItems;

        public static List<ServiceConfigItem> GetConfigs(string section)
        {
            ServiceConfig instance = ConfigurationManager.GetSection(section) as ServiceConfig;
            if (null == instance) return null;
            return instance.ConfigItems;
        }

        public static List<ServiceConfigItem> GetConfigs(string group, string section)
        {
            ServiceConfig instance = ConfigurationManager.GetSection(group + "/" + section) as ServiceConfig;
            if (null == instance) return null;
            return instance.ConfigItems;
        }

        



        public object Create(object parent, object configContext, XmlNode section)
        {
            try
            {
                string typeName = (string)section.CreateNavigator().Evaluate("string(@type)");
                XmlSerializer serializer = new XmlSerializer(Type.GetType(typeName), new XmlRootAttribute(section.Name));
                return serializer.Deserialize(new XmlNodeReader(section));
            }
            catch (Exception e)
            {
                Log.Error("ReadServiceConfig.Create;" + e.Message);
            }
            return null;
        }


    }
}
