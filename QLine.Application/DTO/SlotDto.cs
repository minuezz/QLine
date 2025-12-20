using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLine.Application.DTO
{
    public class SlotDto
    {
        public TimeSpan Start { get; init; }
        public TimeSpan End { get; init; }
        public bool IsAvailable { get; init; }
    }
}
