using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QLine.Domain.Entities;

namespace QLine.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Tenant> Tenants => Set<Tenant>();
        public DbSet<ServicePoint> ServicePoints => Set<ServicePoint>();
        public DbSet<Service> Services => Set<Service>();
        public DbSet<Reservation> Reservations => Set<Reservation>();
        public DbSet<QueueEntry> QueueEntries => Set<QueueEntry>();
        public DbSet<AppUser> AppUsers => Set<AppUser>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
