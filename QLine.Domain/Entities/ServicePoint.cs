using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using QLine.Domain;

namespace QLine.Domain.Entities
{
    public class ServicePoint
    {
        public Guid Id { get; private set; }
        public Guid TenantId { get; private set; }
        public string Name { get; private set; } = null!;
        public string Address { get; private set; } = null!;
        public string OpenHoursJson { get; private set; } = null!;

        private ServicePoint() { }

        private ServicePoint(Guid id, Guid tenantId, string name, string address, string openHoursJson)
        {
            if (id == Guid.Empty) throw new DomainException("ServicePoint Id cannot be empty.");
            if (id == Guid.Empty) throw new DomainException("ServicePoint TenantId cannot be empty.");
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("ServicePoint Name is required.");
            if (string.IsNullOrWhiteSpace(address)) throw new DomainException("ServicePoint Address is required.");
            if (openHoursJson is null) throw new DomainException("ServicePoint OpenHoursJson cannot be null.");


            Id = id;
            TenantId = tenantId;
            Name = name.Trim();
            Address = address.Trim();
            OpenHoursJson = openHoursJson.Trim();
        }

        public static ServicePoint Create(Guid id, Guid tenantId, string name, string address, string openHoursJson)
            => new(id, tenantId, name, address, openHoursJson);

        public void Update(string name, string address, string openHoursJson)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("ServicePoint Name is required.");
            if (string.IsNullOrWhiteSpace(address)) throw new DomainException("ServicePoint Address is required.");
            if (openHoursJson is null) throw new DomainException("ServicePoint OpenHoursJson is required.");

            Name = name.Trim();
            Address = address.Trim();
            OpenHoursJson = openHoursJson.Trim();
        }
    }
}
