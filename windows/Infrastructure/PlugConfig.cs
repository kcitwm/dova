using System;
using System.Xml;
using System.Configuration;
using System.Xml.Serialization;
using System.Collections.Generic;

using Dova.Utility;

namespace Dova.Services
{
    [Serializable]
    public class PlugConfig : IConfigurationSectionHandler
    { 
        public List<PlugingItem> ConfigItems;
        public List<Routing> Routings;

        public static List<PlugingItem> GetConfigs(string section)
        {
            ConfigurationManager.RefreshSection(section);
            List<PlugingItem> list = new List<PlugingItem>();
            PlugConfig instance = ConfigurationManager.GetSection(section) as PlugConfig;
            if (null != instance) list = instance.ConfigItems;
            return list;
        }

        public static List<Routing> GetRoutings(string section)
        {
            ConfigurationManager.RefreshSection(section);
            List<Routing> list = new List<Routing>();
            PlugConfig instance = ConfigurationManager.GetSection(section) as PlugConfig;
            if (null != instance)
                list = instance.Routings;
            return list;
        }


        public static List<PlugingItem> GetConfigs(string group, string section)
        {
            ConfigurationManager.RefreshSection(section);
            List<PlugingItem> list = new List<PlugingItem>();
            PlugConfig instance = ConfigurationManager.GetSection(group + "/" + section) as PlugConfig;
            if (null != instance) list = instance.ConfigItems;
            return list;
        }





        public object Create(object parent, object configContext, XmlNode section)
        {
            try
            {
                string typeName = (string)section.CreateNavigator().Evaluate("string(@type)");
                string configSouce = (string)section.CreateNavigator().Evaluate("string(@configSouce)");
                XmlSerializer serializer = new XmlSerializer(Type.GetType(typeName), new XmlRootAttribute(section.Name));
                if (!string.IsNullOrEmpty(configSouce))
                {
                    using (XmlTextReader tr = new XmlTextReader(Config.BasePath + configSouce))
                    {
                        return serializer.Deserialize(tr);
                    }
                }
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
