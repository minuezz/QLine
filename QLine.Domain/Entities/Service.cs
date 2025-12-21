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
        public Guid ServicePointId { get; private set; }
        public string Name { get; private set; } = null!;
        public int DurationMin { get; private set; }
        public int BufferMin {  get; private set; }

        public Service() { }

        private Service(Guid id, Guid servicePointId, string name, int durationMin, int bufferMin)
        {
            if (id == Guid.Empty) throw new DomainException("Service Id cannot be empty.");
            if (servicePointId == Guid.Empty) throw new DomainException("Service ServicePointId cannot be empty.");
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Service Name is required.");
            if (durationMin <= 0) throw new DomainException("Service Duration must be positive.");
            if (bufferMin < 0) throw new DomainException("Service Buffer cannot be negative.");


            Id = id;
            ServicePointId = servicePointId;
            Name = name.Trim();
            DurationMin = durationMin;
            BufferMin = bufferMin;
        }

        public static Service Create(Guid id, Guid servicePointId, string name, int durationMin, int bufferMin)
            => new(id, servicePointId, name, durationMin, bufferMin);

        public void Update(string name, int durationMin, int bufferMin)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Service Name is required.");
            if (durationMin <= 0) throw new DomainException("Service Duration must be positive.");
            if (bufferMin < 0) throw new DomainException("Service Buffer cannot be negative.");

            Name = name.Trim();
            DurationMin = durationMin;
            BufferMin = bufferMin;
        }
    }
}
