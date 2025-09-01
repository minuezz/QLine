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
    public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
    {
        public void Configure(EntityTypeBuilder<Tenant> b)
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired().HasMaxLength(200);
            b.Property(x => x.Slug).IsRequired().HasMaxLength(100);
            b.Property(x => x.Timezone).IsRequired().HasMaxLength(100);
            b.Property(x => x.Language).IsRequired().HasMaxLength(10);

            b.HasIndex(x => x.Slug).IsUnique();
        }
    }
}
