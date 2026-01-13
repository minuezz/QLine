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

            var now = DateTimeOffset.UtcNow;
            const string demoPassword = "Pass@word1";

            const string oh = "{\r\n  \"monday\": { \"open\": true, \"start\": \"09:00\", \"end\": \"17:00\" },\r\n  \"tuesday\": { \"open\": true, \"start\": \"09:00\", \"end\": \"17:00\" },\r\n  \"wednesday\": { \"open\": true, \"start\": \"09:00\", \"end\": \"17:00\" },\r\n  \"thursday\": { \"open\": true, \"start\": \"09:00\", \"end\": \"17:00\" },\r\n  \"friday\": { \"open\": true, \"start\": \"09:00\", \"end\": \"17:00\" },\r\n  \"saturday\": { \"open\": false, \"start\": \"10:00\", \"end\": \"14:00\" },\r\n  \"sunday\": { \"open\": false, \"start\": \"00:00\", \"end\": \"00:00\" }\r\n}";

            var sp1Id = Guid.NewGuid();
            var sp2Id = Guid.NewGuid();
            var sp3Id = Guid.NewGuid();

            var sp1 = ServicePoint.Create(sp1Id, "CityDesk Centrum", "ul. Testowa 1, Warszawa", oh);
            var sp2 = ServicePoint.Create(sp2Id, "CityDesk Mokotów", "ul. Puławska 100, Warszawa", oh);
            var sp3 = ServicePoint.Create(sp3Id, "CityDesk Ursynów", "al. KEN 20, Warszawa", oh);

            await db.ServicePoints.AddRangeAsync([sp1, sp2, sp3], ct);

            var services = new List<Service>
            {
                Service.Create(Guid.NewGuid(), sp1Id, "Rejestracja ogólna", 15, 5),
                Service.Create(Guid.NewGuid(), sp1Id, "Odbiór dokumentów", 10, 5),
                Service.Create(Guid.NewGuid(), sp1Id, "Konsultacja", 20, 5),

                Service.Create(Guid.NewGuid(), sp2Id, "Informacja", 10, 5),
                Service.Create(Guid.NewGuid(), sp2Id, "Reklamacja", 25, 5),
                Service.Create(Guid.NewGuid(), sp2Id, "Wydanie zaświadczeń", 15, 5),

                Service.Create(Guid.NewGuid(), sp3Id, "Obsługa klienta", 15, 5),
                Service.Create(Guid.NewGuid(), sp3Id, "Zmiana danych", 20, 5),
                Service.Create(Guid.NewGuid(), sp3Id, "Kasa", 10, 5),
            };

            await db.Services.AddRangeAsync(services, ct);

            AppUser CreateUser(string email, string fn, string ln, UserRole role)
            {
                var u = AppUser.Create(Guid.NewGuid(), email, fn, ln, role);
                u.UpdatePasswordHash(passwordHasher.HashPassword(u, demoPassword));
                return u;
            }

            var admin = CreateUser("admin@qline.local", "Demo", "Admin", UserRole.Admin);
            var staff1 = CreateUser("anna.staff@qline.local", "Anna", "Kowalska", UserRole.Staff);
            var staff2 = CreateUser("marek.staff@qline.local", "Marek", "Nowak", UserRole.Staff);
            var staff3 = CreateUser("piotr.staff@qline.local", "Piotr", "Zieliński", UserRole.Staff);

            var client1 = CreateUser("ola.client@qline.local", "Ola", "Wiśniewska", UserRole.Client);
            var client2 = CreateUser("jan.client@qline.local", "Jan", "Wójcik", UserRole.Client);
            var client3 = CreateUser("kasia.client@qline.local", "Kasia", "Kamińska", UserRole.Client);

            await db.AppUsers.AddRangeAsync([admin, staff1, staff2, staff3, client1, client2, client3], ct);

            var assignments = new[]
            {
                StaffAssignment.Create(sp1Id, staff1.Id),
                StaffAssignment.Create(sp1Id, staff2.Id),
                StaffAssignment.Create(sp2Id, staff3.Id),
            };

            await db.StaffAssignments.AddRangeAsync(assignments, ct);

            await db.SaveChangesAsync(ct);

            var sp1Svc = services.First(s => s.ServicePointId == sp1Id);
            var sp2Svc = services.First(s => s.ServicePointId == sp2Id);

            var rInService = Reservation.Create(Guid.NewGuid(), sp1Id, sp1Svc.Id, client1.Id, now.AddMinutes(-5), now.AddHours(-1));
            rInService.CheckIn();
            rInService.StartService();

            var qInService = QueueEntry.Create(Guid.NewGuid(), sp1Id, rInService.Id, "A012", priority: 0, createdAt: now.AddMinutes(-20));
            qInService.MarkInService();

            var rWait1 = Reservation.Create(Guid.NewGuid(), sp1Id, sp1Svc.Id, client2.Id, now.AddMinutes(10), now.AddHours(-2));
            rWait1.CheckIn();
            var qWait1 = QueueEntry.Create(Guid.NewGuid(), sp1Id, rWait1.Id, "A013", 0, now.AddMinutes(-12));

            var rWait2 = Reservation.Create(Guid.NewGuid(), sp1Id, sp1Svc.Id, client3.Id, now.AddMinutes(20), now.AddHours(-2));
            rWait2.CheckIn();
            var qWait2 = QueueEntry.Create(Guid.NewGuid(), sp1Id, rWait2.Id, "A014", 0, now.AddMinutes(-8));

            var rSkipped = Reservation.Create(Guid.NewGuid(), sp1Id, sp1Svc.Id, client1.Id, now.AddMinutes(25), now.AddHours(-3));
            rSkipped.CheckIn();
            var qSkipped = QueueEntry.Create(Guid.NewGuid(), sp1Id, rSkipped.Id, "A015", 0, now.AddMinutes(-6));
            qSkipped.Skip();

            var rDone = Reservation.Create(Guid.NewGuid(), sp2Id, sp2Svc.Id, client2.Id, now.AddDays(-2), now.AddDays(-3));
            rDone.Complete();

            var rCancelled = Reservation.Create(Guid.NewGuid(), sp2Id, sp2Svc.Id, client3.Id, now.AddDays(-1), now.AddDays(-2));
            rCancelled.Cancel();

            var rNoShow = Reservation.Create(Guid.NewGuid(), sp2Id, sp2Svc.Id, client1.Id, now.AddDays(-1), now.AddDays(-2));
            rNoShow.MarkAsNoShow();

            await db.Reservations.AddRangeAsync(
                [rInService, rWait1, rWait2, rSkipped, rDone, rCancelled, rNoShow], ct);

            await db.QueueEntries.AddRangeAsync(
                [qInService, qWait1, qWait2, qSkipped], ct);

            await db.SaveChangesAsync(ct);
        }

    }
}
