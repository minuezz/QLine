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
        private readonly IRealtimeNotifier _realtime;
        private readonly IAppUserRepository _userRepo;
        private readonly IEmailSender _emailSender;
        private readonly IServicePointRepository _servicePoints;
        private readonly IServiceRepository _services;

        public CreateReservationCommandHandler(
            IReservationRepository reservations,
            IDateTimeProvider clock,
            ICurrentUser currentUser,
            IRealtimeNotifier realtime,
            IAppUserRepository userRepo,
            IEmailSender emailSender,
            IServicePointRepository servicePoints,
            IServiceRepository services)
        {
            _reservations = reservations;
            _clock = clock;
            _currentUser = currentUser;
            _realtime = realtime;
            _userRepo = userRepo;
            _emailSender = emailSender;
            _servicePoints = servicePoints;
            _services = services;
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

            await _realtime.UserReservationsUpdated(reservation.UserId, ct);

            try
            {
                var user = await _userRepo.GetByIdAsync(userId, ct);
                var servicePoint = await _servicePoints.GetByIdAsync(request.ServicePointId, ct);
                var service = await _services.GetByIdAsync(request.ServiceId, ct);

                if (user != null && !string.IsNullOrEmpty(user.Email) && servicePoint != null && service != null)
                {
                    var subject = "QLine Booking Confirmation";

                    var body = $@"
                        <div style='font-family: Arial, sans-serif; color: #333;'>
                            <h2 style='color: #1976d2;'>Booking Confirmed!</h2>
                            <p>Hello <strong>{user.FirstName}</strong>,</p>
                            <p>Your appointment has been successfully scheduled.</p>
                            
                            <div style='background-color: #f5f5f5; padding: 15px; border-radius: 8px; margin: 20px 0;'>
                                <p style='margin: 5px 0;'><strong>Service:</strong> {service.Name}</p>
                                <p style='margin: 5px 0;'><strong>Location:</strong> {servicePoint.Name}</p>
                                <p style='margin: 5px 0;'><strong>Address:</strong> {servicePoint.Address}</p>
                                <p style='margin: 5px 0;'><strong>Date & Time:</strong> {reservation.StartTime.ToLocalTime():g}</p>
                            </div>

                            <p>Please arrive 5 minutes early. Don't forget to present your QR code from the app.</p>
                            <br/>
                            <p style='color: #888; font-size: 12px;'>This is an automated message from QLine System.</p>
                        </div>";

                    await _emailSender.SendEmailAsync(user.Email, subject, body);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Email Error] Failed to send email to user {userId}: {ex.Message}");
            }

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
