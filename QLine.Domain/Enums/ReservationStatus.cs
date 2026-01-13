using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLine.Domain.Enums
{
    public enum ReservationStatus
    {
        Active = 0,
        Waiting = 1,
        InService = 2, 
        Completed = 3,
        Cancelled = 4,
        NoShow = 5
    }
}
