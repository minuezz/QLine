using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLine.Application.DTO
{
    public sealed class QueueSnapshotDto
    {
        public QueueEntryDto? Current { get; init; }
        public IReadOnlyList<QueueEntryDto> Waiting { get; init; } = Array.Empty<QueueEntryDto>();
    }
}
