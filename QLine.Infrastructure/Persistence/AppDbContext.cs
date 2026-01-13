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

        public DbSet<ServicePoint> ServicePoints => Set<ServicePoint>();
        public DbSet<Service> Services => Set<Service>();
        public DbSet<Reservation> Reservations => Set<Reservation>();
        public DbSet<QueueEntry> QueueEntries => Set<QueueEntry>();
        public DbSet<AppUser> AppUsers => Set<AppUser>();
        public DbSet<StaffAssignment> StaffAssignments => Set<StaffAssignment>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<DateTime>().HaveColumnType("timestamptz");
        }
    }
}
