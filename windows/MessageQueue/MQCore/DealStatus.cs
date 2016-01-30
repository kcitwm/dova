using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dova.MessageQueue
{
    public enum DealStatus
    {
        All=~0,
        None=0,
        Dealing=1,
        Succ=2,
        Fail=4
    }
}
