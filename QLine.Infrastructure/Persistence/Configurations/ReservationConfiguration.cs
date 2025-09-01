using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QLine.Domain.Entities;
using QLine.Domain.Enums;

namespace QLine.Infrastructure.Persistence.Configurations
{
    public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> b)
        {
            b.HasKey(x => x.Id);

            b.Property(x => x.TenantId).IsRequired();
            b.Property(x => x.ServicePointId).IsRequired();
            b.Property(x => x.ServiceId).IsRequired();
            b.Property(x => x.UserId).IsRequired();
            b.Property(x => x.StartTime).IsRequired();
            b.Property(x => x.CreatedAt).IsRequired();

            b.Property(x => x.Status)
                .IsRequired()
                .HasConversion<int>();

            b.HasIndex(x => new { x.TenantId, x.ServicePointId, x.StartTime })
                .IsUnique()
                .HasFilter("\"Status\" = 0")
                .HasDatabaseName("UX_Reservation_ActiveSlot");
        }
    }
}
