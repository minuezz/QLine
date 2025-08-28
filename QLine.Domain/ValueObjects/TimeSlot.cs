using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLine.Domain.ValueObjects
{
    public readonly record struct TimeSlot(DateTimeOffset Start, TimeSpan Duration)
    {
        public DateTimeOffset End => Start + Duration;

        public static TimeSlot Create(DateTimeOffset start, TimeSpan duration)
        {
            if (duration <= TimeSpan.Zero)
            {
                throw new ArgumentException("Duration must be greater than zero.", nameof(duration));
            }
            return new TimeSlot(start, duration);
        }
    }
}
