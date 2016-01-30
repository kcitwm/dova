using System; 
using System.Xml.Serialization;

namespace Dova
{
    [Serializable]
    public class ServiceConfigItem
    {
        [XmlElement]
        public bool Enable { get; set; }

        string _url = "Local";
        [XmlElement]
        public string Url { get { return _url; } set { if (null!=value) _url = value; } }

        string _endPoint = "";
        [XmlElement]
        public string EndPoint { get { return _endPoint; } set { if (null!=value) _endPoint = value; } } 

        public string RoutKey { get { return  _endPoint; } } 

        int _serviceType = 1;
        [XmlElement]
        public int ServiceType { get { return _serviceType; } set { _serviceType = value; } }

        string _defaultType = "";
        [XmlElement]
        public string DefaultType { get { return _defaultType; } set { _defaultType = value; } }


        int _failedTimes = 0;
        [XmlElement]
        public int FailedTimes { get { return _failedTimes; } set { _failedTimes = value; } }

        int _failOverTime = 1;
        [XmlElement]
        public int FailOverTime { get { return _failOverTime; } set { _failOverTime = value; } }

        int _failTimesLimt = 3;
        [XmlElement]
        public int FailTimesLimt { get { return _failTimesLimt; } set { _failTimesLimt = value; } }

        int _maxObjectSize = 1024*1024;
        [XmlElement]
        public int MaxObjectSize { get { return _maxObjectSize; } set { _maxObjectSize = value; } }

        public override string ToString()
        {
            return  _url + " " + _endPoint + " " + _serviceType;
        }
 

    }
}
