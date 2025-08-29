using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLine.Domain;

namespace QLine.Domain.Entities
{
    public class Service
    {
        public Guid Id { get; private set; }
        public Guid TenantId { get; private set; }
        public Guid ServicePointId { get; private set; }
        public string Name { get; private set; } = null!;
        public int DurationMin { get; private set; }
        public int BufferMin {  get; private set; }
        public int MaxPerDay { get; private set; }

        public Service() { }

        private Service(Guid id, Guid tenantId, Guid servicePointId, string name, int durationMin, int bufferMin, int maxPerDay)
        {
            if (id == Guid.Empty) throw new DomainException("Service Id cannot be empty.");
            if (tenantId == Guid.Empty) throw new DomainException("Service TenantId cannot be empty.");
            if (servicePointId == Guid.Empty) throw new DomainException("Service ServicePointId cannot be empty.");
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Service Name is required.");
            if (durationMin <= 0) throw new DomainException("Service Duration must be positive.");
            if (bufferMin < 0) throw new DomainException("Service Buffer cannot be negative.");
            if (maxPerDay < 0) throw new DomainException("Service MaxPerDay cannot be negative.");


            Id = id;
            TenantId = tenantId;
            ServicePointId = servicePointId;
            Name = name.Trim();
            DurationMin = durationMin;
            BufferMin = bufferMin;
            MaxPerDay = maxPerDay;
        }

        public static Service Create(Guid id, Guid tenantId, Guid servicePointId, string name, int durationMin, int bufferMin, int maxPerDay)
            => new(id, tenantId, servicePointId, name, durationMin, bufferMin, maxPerDay);

        public void Update(string name, int durationMin, int bufferMin, int maxPerDay)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Service Name is required.");
            if (durationMin <= 0) throw new DomainException("Service Duration must be positive.");
            if (bufferMin < 0) throw new DomainException("Service Buffer cannot be negative.");
            if (maxPerDay < 0) throw new DomainException("Service MaxPerDay cannot be negative.");

            Name = name.Trim();
            DurationMin = durationMin;
            BufferMin = bufferMin;
            MaxPerDay = maxPerDay;
        }
    }
}
