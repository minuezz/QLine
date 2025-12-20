using MediatR;
using QLine.Application.DTO;
using QLine.Application.Abstractions;
using QLine.Domain;
using QLine.Domain.Abstractions;
using QLine.Domain.Entities;
using System.Text.Json;

namespace QLine.Application.Features.Reservations.Queries
{
    public sealed class GetAvailableSlotsQueryHandler
        : IRequestHandler<GetAvailableSlotsQuery, IReadOnlyList<SlotDto>>
    {
        private readonly IServicePointRepository _servicePoints;
        private readonly IServiceRepository _services;
        private readonly IReservationRepository _reservations;
        private readonly IDateTimeProvider _clock;

        public GetAvailableSlotsQueryHandler(
            IServicePointRepository servicePoints,
            IServiceRepository services,
            IReservationRepository reservations,
            IDateTimeProvider clock)
        {
            _servicePoints = servicePoints;
            _services = services;
            _reservations = reservations;
            _clock = clock;
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

            var nowUtc = _clock.UtcNow;
            TimeSpan? minStartTime = null;

            if (DateOnly.FromDateTime(nowUtc.UtcDateTime) == request.Date)
            {
                minStartTime = nowUtc.TimeOfDay;
            }

            var duration = TimeSpan.FromMinutes(service.DurationMin);

            var reservations = await _reservations.GetByDayAsync(
                request.ServicePointId,
                request.Date,
                ct);

            var takenSlots = reservations
                .Select(r => {
                    var actualDuration = r.Service != null
                        ? TimeSpan.FromMinutes(r.Service.DurationMin)
                        : duration;

                    var localStartTime = r.StartTime.ToLocalTime();

                    return (Start: localStartTime.TimeOfDay, End: localStartTime.TimeOfDay + actualDuration);
                })
                .ToList();

            var slots = new List<SlotDto>();

            for (var start = openHours.Value.Opening; start + duration <= openHours.Value.Closing; start += duration)
            {
                var end = start + duration;

                if (minStartTime.HasValue && start < minStartTime.Value)
                    continue;

                var overlapsReservation = takenSlots.Any(t => start < t.End && t.Start < end);

                slots.Add(new SlotDto
                {
                    Start = start,
                    End = end,
                    IsAvailable = !overlapsReservation
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
                var dayKey = date.DayOfWeek.ToString().ToLower();

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