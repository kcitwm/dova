using System; 

namespace Dova
{
    [Flags]
    public enum ChannelType
    {
        Local=1,
        Socket=2,
        Remoting=4,
        WCF=8,
        WebService=16,
        REST=32,
        UDP=64,
        AsyncTcp=128,
        WCFRest=256,
        Http=512,
        PersistAsyncTcp=1024,
        MessageService=2048
    }
}
