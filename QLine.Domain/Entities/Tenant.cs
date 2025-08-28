using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLine.Domain;

namespace QLine.Infrastructure.Entities
{
    public class Tenant
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; } = null!;
        public string Slug { get; private set; } = null!;
        public string Timezone { get; private set; } = null!;
        public string Language { get; private set; } = null!;

        private Tenant() { }

        private Tenant(Guid id, string name, string slug, string timezone, string language)
        {
            if (id == Guid.Empty) throw new DomainException("Tenant Id cannot be empty.");
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Tenant Name is required.");
            if (string.IsNullOrWhiteSpace(slug)) throw new DomainException("Tenant Slug is required.");
            if (string.IsNullOrWhiteSpace(timezone)) throw new DomainException("Tenant TimeZone is required.");
            if (string.IsNullOrWhiteSpace(language)) throw new DomainException("Tenant Language is required.");


            Id = id;
            Name = name.Trim();
            Slug = slug.Trim();
            Timezone = timezone.Trim();
            Language = language.Trim();
        }

        public static Tenant Create(Guid id, string name, string slug, string timezone, string language) 
            => new(id, name, slug, timezone, language);

        public void UpdateProfile(string name, string timezone, string language)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new DomainException("Tenant Name is required.");
            if (string.IsNullOrWhiteSpace(timezone)) throw new DomainException("Tenant Time Zone is required.");
            if (string.IsNullOrWhiteSpace(language)) throw new DomainException("Tenant Language is required.");

            Name = name.Trim();
            Timezone = timezone.Trim();
            Language = language.Trim();
        }
    }
}
