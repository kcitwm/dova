using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

namespace Dova
{
    [Flags]
    public enum WRType
    { 
        Read=1,
        Write=2,
        WR=3
    }
}
