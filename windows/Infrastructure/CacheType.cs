using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace Dova
{
    [Flags]
    public enum CacheType
    { 
        InMemory=1,
        IPC=2,
        Remote=4,
        Memcached=8,
        Redis=16
    }
}
