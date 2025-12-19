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
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> b)
        {
            b.HasKey(x => x.Id);

            b.Property(x => x.Email).IsRequired().HasMaxLength(256);
            b.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
            b.Property(x => x.LastName).IsRequired().HasMaxLength(100);
            b.Property(x => x.PasswordHash).IsRequired().HasMaxLength(500);

            b.Property(x => x.Role)
                .IsRequired()
                .HasConversion<int>();

            b.HasIndex(x => x.Email).IsUnique();
        }
    }
}
