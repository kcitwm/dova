using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Dova
{
    [Serializable]
    public class ServiceConfigGroup
    {
        [XmlAttribute]
        public string GroupName;
        [XmlAttribute]
        public long GroupIndex;
        [XmlAttribute]
        public long SubGroupIndex;
        [XmlAttribute]
        public long ParentGroupIndex;
        [XmlElement]
        public List<ServiceConfigItem> ServiceConfigItem;

    }
}
