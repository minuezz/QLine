using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QLine.Domain.Entities;

namespace QLine.Infrastructure.Persistence.Configurations
{
    public class StaffAssignmentConfiguration : IEntityTypeConfiguration<StaffAssignment>
    {
        public void Configure(EntityTypeBuilder<StaffAssignment> b)
        {
            b.HasKey(x => new { x.ServicePointId, x.UserId });

            b.HasIndex(x => x.UserId);

            b.HasOne<ServicePoint>()
                .WithMany()
                .HasForeignKey(x => x.ServicePointId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
