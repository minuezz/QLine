using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MediatR;
using QLine.Application.DTO;

namespace QLine.Application.Features.Reservations.Queries
{
    public sealed record GetAvailableSlotsQuery(
        Guid ServicePointId,
        Guid ServiceId,
        DateOnly Date
    ) : IRequest<IReadOnlyList<SlotDto>>;
}
