using System;
using System.Net.Security;
using System.ServiceModel;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Dova.Infrastructure
{
    [ServiceContract(ProtectionLevel = ProtectionLevel.None)]
    public interface ISequence
    {
        [OperationContract]
        long[] Get(string seqName, int total);

        [OperationContract(Name="GetByTime")]
        long[] Get(string seqName, int total,out DateTime getTime);
    }
}
