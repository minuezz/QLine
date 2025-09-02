using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QLine.Domain.Entities;
using QLine.Domain.Enums;

namespace QLine.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(AppDbContext db, CancellationToken ct = default)
        {
            await db.Database.MigrateAsync(ct);

            if (await db.Tenants.AnyAsync(ct))
                return;

            var tenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var spId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var svcId = Guid.Parse("33333333-3333-3333-3333-333333333333");
            var userId = Guid.Parse("44444444-4444-4444-4444-444444444444");

            var tenant = Tenant.Create(
                id: tenantId,
                name: "Demo Tenant",
                slug: "demo",
                timezone: "Europe/Warsaw",
                language: "pl"
            );

            var sp = ServicePoint.Create(
                id: spId,
                tenantId: tenantId,
                name: "CityDesk Centrum",
                address: "ul. Testowa 1, Warszawa",
                openHoursJson: "{}"
            );

            var svc = Service.Create(
                id: svcId,
                tenantId: tenantId,
                servicePointId: spId,
                name: "Rejestracja ogólna",
                durationMin: 15,
                bufferMin: 5,
                maxPerDay: 100
            );

            var user = AppUser.Create(
                id: userId,
                tenantId: tenantId,
                email: "test.client@qline.local",
                firstName: "Test",
                lastName: "Cleint",
                role: UserRole.Client
            );

            await db.Tenants.AddAsync(tenant, ct);
            await db.ServicePoints.AddAsync(sp, ct);
            await db.Services.AddAsync(svc, ct);
            await db.AppUsers.AddAsync(user, ct);

            await db.SaveChangesAsync(ct);
        }
    }
}
