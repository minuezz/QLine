using System.Text.Json;
using MediatR;
using QLine.Application.DTO;
using QLine.Domain;
using QLine.Domain.Abstractions;
using QLine.Domain.Entities;

namespace QLine.Application.Features.Reservations.Queries
{
    public sealed class GetAvailableSlotsQueryHandler
        : IRequestHandler<GetAvailableSlotsQuery, IReadOnlyList<SlotDto>>
    {
        private readonly IServicePointRepository _servicePoints;
        private readonly IServiceRepository _services;
        private readonly IReservationRepository _reservations;

        public GetAvailableSlotsQueryHandler(
            IServicePointRepository servicePoints,
            IServiceRepository services,
            IReservationRepository reservations)
        {
            _servicePoints = servicePoints;
            _services = services;
            _reservations = reservations;
        }

        public async Task<IReadOnlyList<SlotDto>> Handle(GetAvailableSlotsQuery request, CancellationToken ct)
        {
            var servicePoint = await _servicePoints.GetByIdAsync(request.ServicePointId, ct)
                ?? throw new DomainException("Service Point not found.");

            var service = await _services.GetByIdAsync(request.ServiceId, ct)
                ?? throw new DomainException("Service not found.");

            var openHours = TryGetWorkingHours(servicePoint, request.Date);
            if (openHours is null)
                return Array.Empty<SlotDto>();

            var duration = TimeSpan.FromMinutes(service.DurationMin);
            var reservations = await _reservations.GetByDayAsync(
                request.ServicePointId,
                request.ServiceId,
                request.Date,
                ct);

            var takenSlots = reservations
                .Select(r =>
                {
                    var reservationDuration = r.Service is not null
                        ? TimeSpan.FromMinutes(r.Service.DurationMin)
                        : duration;

                    return (Start: r.StartTime.TimeOfDay, End: r.StartTime.TimeOfDay + reservationDuration);
                })
                .ToList();

            var slots = new List<SlotDto>();

            for (var start = openHours.Value.Opening; start + duration <= openHours.Value.Closing; start += duration)
            {
                var end = start + duration;
                var overlapsReservation = takenSlots.Any(t => start < t.End && t.Start < end);

                if (overlapsReservation)
                    continue;

                slots.Add(new SlotDto
                {
                    Start = start,
                    End = end,
                    IsAvailable = true
                });
            }

            return slots;
        }

        private static (TimeSpan Opening, TimeSpan Closing)? TryGetWorkingHours(ServicePoint servicePoint, DateOnly date)
        {
            if (string.IsNullOrWhiteSpace(servicePoint.OpenHoursJson))
                return null;

            try
            {
                using var doc = JsonDocument.Parse(servicePoint.OpenHoursJson);
                var dayKey = date.DayOfWeek switch
                {
                    DayOfWeek.Monday => "monday",
                    DayOfWeek.Tuesday => "tuesday",
                    DayOfWeek.Wednesday => "wednesday",
                    DayOfWeek.Thursday => "thursday",
                    DayOfWeek.Friday => "friday",
                    DayOfWeek.Saturday => "saturday",
                    DayOfWeek.Sunday => "sunday",
                    _ => string.Empty
                };

                if (string.IsNullOrWhiteSpace(dayKey))
                    return null;

                if (!doc.RootElement.TryGetProperty(dayKey, out var dayElement))
                    return null;

                if (dayElement.ValueKind != JsonValueKind.Object)
                    return null;

                var open = dayElement.TryGetProperty("open", out var openProp) && openProp.GetBoolean();
                if (!open)
                    return null;

                if (!dayElement.TryGetProperty("start", out var startProp) ||
                    !dayElement.TryGetProperty("end", out var endProp))
                    return null;

                if (!TimeSpan.TryParse(startProp.GetString(), out var opening) ||
                    !TimeSpan.TryParse(endProp.GetString(), out var closing))
                    return null;

                if (opening >= closing)
                    return null;

                return (opening, closing);
            }
            catch
            {
                return null;
            }
        }
    }
}
