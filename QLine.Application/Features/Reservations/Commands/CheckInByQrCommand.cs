using System;
using MediatR;
using QLine.Application.DTO;

namespace QLine.Application.Features.Reservations.Commands
{
    public sealed record CheckInByQrCommand(Guid ReservationId) : IRequest<QueueSnapshotDto>;
}
