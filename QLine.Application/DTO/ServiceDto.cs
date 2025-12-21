using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLine.Application.DTO
{
    public sealed class ServiceDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = default!;
        public int DurationMin { get; init; }
        public int BufferMin { get; init; }
    }
}
