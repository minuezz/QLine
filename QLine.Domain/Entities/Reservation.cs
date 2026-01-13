using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLine.Domain;
using QLine.Domain.Enums;

namespace QLine.Domain.Entities
{
    public class Reservation
    {
        public Guid Id { get; private set; }
        public Guid ServicePointId { get; private set; }
        public Guid ServiceId { get; private set; }
        public Guid UserId { get; private set; }

        public DateTimeOffset StartTime { get; private set; }
        public ReservationStatus Status { get; private set; }
        public DateTimeOffset CreatedAt {  get; private set; }

        public Service? Service { get; private set; }

        private Reservation() { }

        private Reservation(Guid id, Guid servicePointId, Guid serviceId, Guid userId, DateTimeOffset startTime, DateTimeOffset createdAt)
        {
            if (id == Guid.Empty) throw new DomainException("Reservation Id cannot be empty.");
            if (servicePointId == Guid.Empty) throw new DomainException("Reservation ServicePointId cannot be empty.");
            if (serviceId == Guid.Empty) throw new DomainException("Reservation ServiceId cannot be empty.");
            if (userId == Guid.Empty) throw new DomainException("Reservation UserId cannot be empty.");
            if (startTime == default) throw new DomainException("Reservation StartTime is required.");
            if (createdAt == default) throw new DomainException("Reservation CreatedAt is required.");

            Id = id;
            ServicePointId = servicePointId;
            ServiceId = serviceId;
            UserId = userId;
            StartTime = startTime;
            CreatedAt = createdAt;
            Status = ReservationStatus.Active;
        }

        public static Reservation Create(Guid id, Guid servicePointId, Guid serviceId, Guid userId, DateTimeOffset startTime, DateTimeOffset createdAt)
            => new(id, servicePointId, serviceId, userId, startTime, createdAt);

        public void CheckIn()
        {
            if (Status != ReservationStatus.Active)
            {
                throw new DomainException("Check-in is allowed only for Active reservations.");
            }
            Status = ReservationStatus.Waiting;
        }

        public void StartService()
        {
            if (Status != ReservationStatus.Waiting && Status != ReservationStatus.Active)
            {
                throw new DomainException("Can only start service for Waiting reservations.");
            }
            Status = ReservationStatus.InService;
        }

        public void MarkAsNoShow()
        {
            Status = ReservationStatus.NoShow;
        }

        public void Cancel()
        {
            if (Status != ReservationStatus.Active && Status != ReservationStatus.Waiting)
                throw new DomainException("Reservation cannot be cancelled in current state.");
            Status = ReservationStatus.Cancelled;
        }

        public void Complete()
        {
            if (Status == ReservationStatus.Cancelled || Status == ReservationStatus.Completed)
                throw new DomainException("Reservation is already closed.");

            Status = ReservationStatus.Completed;
        }

        public void Reschedule(DateTimeOffset newStartTime)
        {
            if (Status != ReservationStatus.Active)
                throw new DomainException("Only active reservation can be rescheduled.");
            if (newStartTime == default)
                throw new DomainException("New StartTime is required.");
            StartTime = newStartTime; 
        }
    }
}
