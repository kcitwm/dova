using System;
using System.Xml.Serialization;

namespace Dova
{
    public class PlugingItem
    {
        string _name = "";
        string _groupName = "";
        string _routingGroupName = "";
        long _groupIndex = 0;
        long _parentGroupIndex = 0;

        [XmlAttribute]
        public string Name
        {
            get { return _name; }
            set { if (null != value) _name = value; }
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


        int _dealMode = 1;
        /// <summary>
        /// 是否走确认 1 确认　２　不确认
        /// </summary>
        [XmlAttribute]
        public int DealMode
        {
            get { return _dealMode; }
            set { _dealMode = value; }
        }

        int _channelType = 1;
        /// <summary>
        /// 走同步还是异步　１　同步　２异步
        /// </summary>
        [XmlAttribute]
        public int ChannelType
        {
            get { return _channelType; }
            set { _channelType = value; }
        }

        string _type = "";
        [XmlAttribute]
        public string Type
        {
            get { return _type; }
            set { if (null != value) _type = value; }
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
            return "Name:" + _name + ",Type;" + _type + ",GroupName:" + _groupName + ",GroupIndex:" + _groupIndex + ",DealMode:" + _dealMode + ",ChannelType:" + _channelType + ",DefaultEmptyHandler:" + _emptyHandlerName; ;
        }


    }
}
