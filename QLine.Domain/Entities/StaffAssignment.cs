using System;
using QLine.Domain;

namespace QLine.Domain.Entities
{
    public class StaffAssignment
    {
        public Guid ServicePointId { get; private set; }
        public Guid UserId { get; private set; }

        private StaffAssignment() { }

        private StaffAssignment(Guid servicePointId, Guid userId)
        {
            if (servicePointId == Guid.Empty)
                throw new DomainException("ServicePointId cannot be empty.");
            if (userId == Guid.Empty)
                throw new DomainException("UserId cannot be empty.");

            ServicePointId = servicePointId;
            UserId = userId;
        }

        public static StaffAssignment Create(Guid servicePointId, Guid userId) =>
            new(servicePointId, userId);
    }
}
