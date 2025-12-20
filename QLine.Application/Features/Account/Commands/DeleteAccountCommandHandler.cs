using MediatR;
using Microsoft.AspNetCore.Identity;
using QLine.Domain;
using QLine.Domain.Abstractions;
using QLine.Domain.Entities;

namespace QLine.Application.Features.Account.Commands
{
    public sealed class DeleteAccountCommandHandler : IRequestHandler<DeleteAccountCommand>
    {
        private readonly IAppUserRepository _users;
        private readonly IPasswordHasher<AppUser> _passwordHasher;

        public DeleteAccountCommandHandler(IAppUserRepository users, IPasswordHasher<AppUser> passwordHasher)
        {
            _users = users;
            _passwordHasher = passwordHasher;
        }

        public async Task Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        {
            var user = await _users.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null)
            {
                throw new DomainException("User not found.");
            }

            var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (verify is not PasswordVerificationResult.Success and not PasswordVerificationResult.SuccessRehashNeeded)
            {
                throw new DomainException("Invalid password.");
            }

            await _users.DeleteWithRelatedDataAsync(user, cancellationToken);
        }
    }
}