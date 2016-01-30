using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace Dova
{
    [Serializable]
    [DataContract]
    public class DovaHttpMessage : WQMessage
    { 

        string _contentType = "xml/text";

        public string ContentType
        {
            get { return _contentType; }
            set { if (null != _contentType) _contentType = value; }
        }

        string _encoding = "utf-8";

        public string Encoding
        {
            get { return _encoding; }
            set { if (null != _encoding) _encoding = value; }
        }

        string _method = "POST";

        public string Method
        {
            get { return _method; }
            set { if (null != _method) _method = value.ToUpper(); }
        }

        

        public override string ToKeyString()
        {
           return base.ToKeyString() + ";ContentType:" + _contentType + ";Encoding:" + _encoding + ";Method:" + _method;
        }

    }

   

}
