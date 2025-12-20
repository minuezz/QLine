using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QLine.Domain;
using QLine.Domain.Entities;
using QLine.Domain.Enums;
using QLine.Infrastructure.Persistence;

namespace QLine.Application.Features.Account.Commands
{
    public sealed class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand>
    {
        private readonly AppDbContext _db;
        private readonly IPasswordHasher<AppUser> _passwordHasher;

        public DeleteAccountCommandHandler(AppDbContext db, IPasswordHasher<AppUser> passwordHasher)
        {
            _db = db;
            _passwordHasher = passwordHasher;
        }

        public async Task Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            var user = await _db.AppUsers.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
            if (user is null)
            {
                throw new DomainException("User not found.");
            }

            var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (verify is not PasswordVerificationResult.Success and not PasswordVerificationResult.SuccessRehashNeeded)
            {
                throw new DomainException("Invalid password.");
            }

            var reservations = await _db.Reservations
                .Where(r => r.UserId == request.UserId)
                .ToListAsync(cancellationToken);

            foreach (var reservation in reservations.Where(r => r.Status == ReservationStatus.Active))
            {
                reservation.Cancel();
            }

            var reservationIds = reservations.Select(r => r.Id).ToList();
            var queueEntries = await _db.QueueEntries
                .Where(q => reservationIds.Contains(q.ReservationId))
                .ToListAsync(cancellationToken);

            _db.QueueEntries.RemoveRange(queueEntries);
            _db.Reservations.RemoveRange(reservations);
            _db.AppUsers.Remove(user);

            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
