using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLine.Application.DTO
{
    public sealed class ServicePointDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = default!;
        public string Address { get; init; } = default!;
    }
}
