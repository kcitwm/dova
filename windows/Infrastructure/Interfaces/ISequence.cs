using System;
using System.Net.Security;
using System.ServiceModel;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.ServiceModel.Web;

namespace Dova.Interfaces
{
    [ServiceContract(ProtectionLevel = ProtectionLevel.None)]
    public interface ISequence
    {
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "/{seqName}",  ResponseFormat = WebMessageFormat.Json)]
        long Get(string seqName);

        //[WebGet( UriTemplate = "/get2/seqName={seqName}&total={total}&getTime={getTime}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Bare)]
        //[OperationContract(Name="GetByTime")]
        //long[] Get(string seqName, int total, DateTime getTime);
    }
}
