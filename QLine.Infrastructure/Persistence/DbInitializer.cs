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
                openHoursJson: "{\r\n  \"monday\": { \"open\": true, \"start\": \"09:00\", \"end\": \"17:00\" },\r\n  \"tuesday\": { \"open\": true, \"start\": \"09:00\", \"end\": \"17:00\" },\r\n  \"wednesday\": { \"open\": true, \"start\": \"09:00\", \"end\": \"17:00\" },\r\n  \"thursday\": { \"open\": true, \"start\": \"09:00\", \"end\": \"17:00\" },\r\n  \"friday\": { \"open\": true, \"start\": \"09:00\", \"end\": \"17:00\" },\r\n  \"saturday\": { \"open\": false, \"start\": \"10:00\", \"end\": \"14:00\" },\r\n  \"sunday\": { \"open\": false, \"start\": \"00:00\", \"end\": \"00:00\" }\r\n}"
            );

            var svc = Service.Create(
                id: svcId,
                servicePointId: spId,
                name: "Rejestracja ogólna",
                durationMin: 15,
                bufferMin: 5
            );

            var demoPassword = "Pass@word1";

            var user = AppUser.Create(
                id: userId,
                email: "test.client@qline.local",
                firstName: "Test",
                lastName: "Client",
                role: UserRole.Client
            );
            user.UpdatePasswordHash(passwordHasher.HashPassword(user, demoPassword));

            var staff = AppUser.Create(
                id: staffId,
                email: "staff@qline.local",
                firstName: "Demo",
                lastName: "Staff",
                role: UserRole.Staff
            );
            staff.UpdatePasswordHash(passwordHasher.HashPassword(staff, demoPassword));

            await db.ServicePoints.AddAsync(sp, ct);
            await db.Services.AddAsync(svc, ct);
            await db.AppUsers.AddAsync(user, ct);
            await db.AppUsers.AddAsync(staff, ct);

            await db.SaveChangesAsync(ct);
        }
    }
}
