using System; 
using System.Net.Security;
using System.ServiceModel;
using System.Web.Caching; 

namespace Dova.Interfaces
{
   [ServiceContract(ProtectionLevel = ProtectionLevel.None)]
    public interface ICache
    {
         [OperationContract]
         object Get(string key);
        
         [OperationContract(IsOneWay = true)]
         void Remove(string key); 
         
         [OperationContract(IsOneWay = true, Name = "SetCache0")]
         void Set(string key, object value);
         [OperationContract(IsOneWay = true, Name = "SetCache1")]
         void Set(string key, object value, DateTime absoluteExpiration, TimeSpan solidExpiration);
         [OperationContract(IsOneWay = true, Name = "SetCache2")]
         void Set(string key, object value, DateTime absoluteExpiration, TimeSpan solidExpiration, int priority);
         [OperationContract(IsOneWay = true, Name = "SetCache3")]
         void Set(string key, object value, string dependencyFileName, DateTime absoluteExpiration, TimeSpan solidingExpiration);
         [OperationContract(IsOneWay = true, Name = "SetCache4")]
         void Set(string key, object value, string dependencyFileName, DateTime absoluteExpiration, TimeSpan solidingExpiration, int priority);

         [OperationContract(IsOneWay = true, Name = "SetCache5")]
         void Set(string key, object value, TimeSpan solidExpiration);


    }
}
