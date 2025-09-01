using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QLine.Domain.Entities;

namespace QLine.Infrastructure.Persistence.Configurations
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> b)
        {
            b.HasKey(x => x.Id);

            b.Property(x => x.TenantId).IsRequired();
            b.Property(x => x.ServicePointId).IsRequired();
            b.Property(x => x.Name).IsRequired().HasMaxLength(200);
            b.Property(x => x.DurationMin).IsRequired();
            b.Property(x => x.BufferMin).IsRequired();
            b.Property(x => x.MaxPerDay).IsRequired();

            b.HasIndex(x => new { x.ServicePointId, x.Name });
        }
    }
}
