using QLine.Domain;
using QLine.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace QLine.Domain.Entities
{
    public class QueueEntry
    {
        public Guid Id {  get; private set; }
        public Guid TenantId { get; private set; }
        public Guid ServicePointId { get; private set; }
        public Guid ReservationId { get; private set; }

        public string TicketNo { get; private set; } = null!;
        public int Priority { get; private set; }
        public QueueStatus Status { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }

        private QueueEntry() { }

        private QueueEntry(Guid id, Guid tenantId, Guid servicePointId, Guid reservationId, string ticketNo, int priority, QueueStatus status, DateTimeOffset createdAt)
        {
            if (id == Guid.Empty) throw new DomainException("QueueEntry Id cannot be empty.");
            if (tenantId == Guid.Empty) throw new DomainException("QueueEntry TenantId cannot be empty.");
            if (servicePointId == Guid.Empty) throw new DomainException("QueueEntry ServicePointId cannot be empty.");
            if (reservationId == Guid.Empty) throw new DomainException("QueueEntry ReservationId cannot be empty.");
            if (string.IsNullOrWhiteSpace(ticketNo)) throw new DomainException("QueueEntry TicketNo is required.");
            if (priority < 0) throw new DomainException("QueueEntry Priority cannot be negative.");
            if (createdAt == default) throw new DomainException("QueueEntry CreatedAt is required.");

            Id = id;
            TenantId = tenantId;
            ServicePointId = servicePointId;
            ReservationId = reservationId;
            TicketNo = ticketNo.Trim();
            Priority = priority;
            Status = QueueStatus.Waiting;
            CreatedAt = createdAt;
        }

        public static QueueEntry Create(Guid id, Guid tenantId, Guid servicePointId, Guid reservationId, string ticketNo, int priority, QueueStatus status, DateTimeOffset createdAt) 
            => new(id, tenantId, servicePointId, reservationId, ticketNo, priority, status, createdAt);

        public void MarkInService()
        {
            if (Status != QueueStatus.Waiting && Status != QueueStatus.Skipped)
                throw new DomainException("Only waiting or skipped entries can move to InService.");
            Status = QueueStatus.InService;
        }

        public void MarkDone()
        {
            if (Status != QueueStatus.InService)
                throw new DomainException("Only entries InService can be marked as Done.");
            Status = QueueStatus.Done;
        }

        public void MarkNoShow()
        {
            if (Status != QueueStatus.Waiting && Status != QueueStatus.Skipped)
                throw new DomainException("Only waiting or skipped entries can be marked as NoShow.");
            Status = QueueStatus.NoShow;
        }

        public void Skip()
        {
            if (Status != QueueStatus.Waiting)
                throw new DomainException("Only waiting entries can be skipped.");
            Status = QueueStatus.Skipped;
        }
    }
}
