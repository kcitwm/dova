using System; 
using System.Xml.Serialization;

namespace Dova
{
    public class Routing
    {
        string _key= "";
        string _groupName = "";
        string _routingGroupName = "";
        long _groupIndex = 0;
        long _parentGroupIndex = 0;
        
        [XmlAttribute]
        public string Key
        {
            get { return _key; }
            set { if (null != value) _key = value; }
        }

        [XmlAttribute]
        public string GroupName
        {
            get { return _groupName; }
            set { if (null != value) _groupName = value; }
        }


        [XmlAttribute]
        public string RoutingGroupName
        {
            get { return _routingGroupName; }
            set { if (null != value) _routingGroupName = value; }
        }

        [XmlAttribute]
        public long ParentGroupIndex
        {
            get { return _parentGroupIndex; }
            set { _parentGroupIndex = value; }
        }
 

        [XmlAttribute]
        public long GroupIndex
        {
            get { return _groupIndex; }
            set { _groupIndex = value; }
        }

        string _emptyHandlerName = "";
        [XmlAttribute]
        public string EmptyHandlerName
        {
            get { return _emptyHandlerName.Trim(); }
            set { if (null != value) _emptyHandlerName = value; }
        } 

        public override string ToString()
        {
            return "Key:" + _key + ",GroupIndex:" + _groupIndex + ",GroupName:" + _groupName +"RoutingGroupName:"+_routingGroupName+ ",DefaultEmptyHandler:" + _emptyHandlerName;
        }
    }
}
