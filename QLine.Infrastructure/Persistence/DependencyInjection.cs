using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QLine.Application.Abstractions;
using QLine.Domain.Abstractions;
using QLine.Infrastructure.Persistence.Repositories;
using QLine.Infrastructure.Time;

namespace QLine.Infrastructure.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("QLineDb")
                ?? throw new InvalidOperationException("ConnectionStrings: QLineDb is not configured.");

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(connectionString, npgsql =>
                {
                    npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
                });
            });

            services.AddSingleton<IDateTimeProvider, SystemDateTimeProvider>();

            services.AddScoped<ITenantRepository, TenantRepository>();
            services.AddScoped<IServicePointRepository, ServicePointRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IReservationRepository, ReservationRepository>();
            services.AddScoped<IQueueEntryRepository, QueueEntryRepository>();
            services.AddScoped<IAppUserRepository, AppUserRepository>();

            return services;
        }
    }
}
