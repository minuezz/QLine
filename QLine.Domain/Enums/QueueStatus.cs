using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLine.Domain.Enums
{
    public enum QueueStatus
    {
        Waiting = 0,
        InService = 1,
        Skipped = 2,
        NoShow = 3,
        Done = 4
    }
}
