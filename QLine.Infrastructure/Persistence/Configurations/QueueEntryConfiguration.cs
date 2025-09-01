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
    public class QueueEntryConfiguration : IEntityTypeConfiguration<QueueEntry>
    {
        public void Configure(EntityTypeBuilder<QueueEntry> b)
        {
            b.HasKey(x => x.Id);

            b.Property(x => x.TenantId).IsRequired();
            b.Property(x => x.ServicePointId).IsRequired();
            b.Property(x => x.ReservationId).IsRequired();

            b.Property(x => x.TicketNo).IsRequired().HasMaxLength(32);
            b.Property(x => x.Priority).IsRequired();
            b.Property(x => x.CreatedAt).IsRequired();

            b.Property(x => x.Status)
                .IsRequired()
                .HasConversion<int>();

            b.HasIndex(x => new { x.TenantId, x.TicketNo })
                .IsUnique()
                .HasDatabaseName("UX_QueueEntry_Tenant_TicketNo");

            b.HasIndex(x => new { x.ServicePointId, x.Status })
                .HasDatabaseName("IX_QueueEntry_Point_Status");
        }
    }
}
