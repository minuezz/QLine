using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QLine.Domain.Entities;
using QLine.Domain.Enums;

namespace QLine.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(
            AppDbContext db,
            IPasswordHasher<AppUser> passwordHasher,
            CancellationToken ct = default)
        {
            await db.Database.MigrateAsync(ct);

            if (await db.ServicePoints.AnyAsync(ct))
                return;

            var spId = Guid.NewGuid();
            var svcId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var staffId = Guid.NewGuid();

            var sp = ServicePoint.Create(
                id: spId,
                name: "CityDesk Centrum",
                address: "ul. Testowa 1, Warszawa",
                openHoursJson: "{}"
            );

            var svc = Service.Create(
                id: svcId,
                servicePointId: spId,
                name: "Rejestracja ogólna",
                durationMin: 15,
                bufferMin: 5,
                maxPerDay: 100
            );

            var demoPassword = "Pass@word1";

            var user = AppUser.Create(
                id: userId,
                email: "test.client@qline.local",
                firstName: "Test",
                lastName: "Client",
                passwordHash: passwordHasher.HashPassword(null!, demoPassword),
                role: UserRole.Client
            );

            var staff = AppUser.Create(
                id: staffId,
                email: "staff@qline.local",
                firstName: "Demo",
                lastName: "Staff",
                passwordHash: passwordHasher.HashPassword(null!, demoPassword),
                role: UserRole.Staff
            );

            await db.ServicePoints.AddAsync(sp, ct);
            await db.Services.AddAsync(svc, ct);
            await db.AppUsers.AddAsync(user, ct);
            await db.AppUsers.AddAsync(staff, ct);

            await db.SaveChangesAsync(ct);
        }
    }
}
