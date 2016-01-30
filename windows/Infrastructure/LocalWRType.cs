using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dova
{
    /// <summary>
    /// 读取操作是否为远程服务类型实例
    /// </summary> 
    [Flags]
    public enum RemoteWRType
    {
        Local = 1,
        RemoteRead = 2,
        RemoteWrite = 4,
        RemoteWR = 6
    }
}
