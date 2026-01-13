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
    public class ServicePointConfiguration : IEntityTypeConfiguration<ServicePoint>
    {
        public void Configure(EntityTypeBuilder<ServicePoint> b)
        {
            b.HasKey(x => x.Id);

            b.Property(x => x.Name).IsRequired().HasMaxLength(200);
            b.Property(x => x.Address).IsRequired().HasMaxLength(500);
            b.Property(x => x.OpenHoursJson).IsRequired().HasColumnType("jsonb");

            b.HasIndex(x => x.Name);
        }
    }
}
