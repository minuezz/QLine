using QLine.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using QLine.Application.DTO;
namespace QLine.Application.Features.Reservations.Commands
{
    public sealed record CreateReservationCommand(
        Guid TenantId,
        Guid ServicePointId,
        Guid ServiceId,
        Guid UserId,
        DateTimeOffset StartTime
        ) : IRequest<ReservationDto>;

}
