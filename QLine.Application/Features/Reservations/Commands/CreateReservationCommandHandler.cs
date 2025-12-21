using System;
using MediatR;
using QLine.Application.Abstractions;
using QLine.Application.DTO;
using QLine.Domain;
using QLine.Domain.Entities;
using QLine.Domain.Abstractions;

namespace QLine.Application.Features.Reservations.Commands
{
    public sealed class CreateReservationCommandHandler
        : IRequestHandler<CreateReservationCommand, ReservationDto>
    {
        private readonly IReservationRepository _reservations;
        private readonly IDateTimeProvider _clock;
        private readonly ICurrentUser _currentUser;

        public CreateReservationCommandHandler(
            IReservationRepository reservations,
            IDateTimeProvider clock,
            ICurrentUser currentUser)
        {
            _reservations = reservations;
            _clock = clock;
            _currentUser = currentUser;
        }

        public async Task<ReservationDto> Handle(CreateReservationCommand request, CancellationToken ct)
        {
            var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();

            var safeStartTime = request.StartTime.ToUniversalTime();

            if (safeStartTime < _clock.UtcNow.AddMinutes(-1))
            {
                throw new DomainException("Cannot create a reservation in the past.");
            }

            var slotFree = await _reservations.IsSlotAvailableAsync(
                request.ServicePointId, safeStartTime, ct);

            if (!slotFree)
                throw new DomainException("The selected slot is already occupied.");

            var reservation = Reservation.Create(
                id: Guid.NewGuid(),
                servicePointId: request.ServicePointId,
                serviceId: request.ServiceId,
                userId: userId,
                startTime: safeStartTime,
                createdAt: _clock.UtcNow
            );

            await _reservations.AddAsync(reservation, ct);

            return new ReservationDto
            {
                Id = reservation.Id,
                ServicePointId = reservation.ServicePointId,
                ServiceId = reservation.ServiceId,
                UserId = reservation.UserId,
                StartTime = reservation.StartTime,
                Status = reservation.Status.ToString()
            };
        }
    }
}
