using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QLine.Application.Abstractions;
using QLine.Domain;
using QLine.Domain.Abstractions;

namespace QLine.Application.Features.Admin.Commands
{
    public sealed class UpdateUserRoleCommandHandler : IRequestHandler<UpdateUserRoleCommand>
    {
        private readonly IAppUserRepository _users;
        private readonly ICurrentUser _currentUser;

        public UpdateUserRoleCommandHandler(IAppUserRepository users, ICurrentUser currentUser)
        {
            _users = users;
            _currentUser = currentUser;
        }

        public async Task Handle(UpdateUserRoleCommand request, CancellationToken ct)
        {
            if (_currentUser.UserId.HasValue && _currentUser.UserId.Value == request.UserId)
            {
                throw new DomainException("You cannot change your own role.");
            }

            var user = await _users.GetByIdAsync(request.UserId, ct)
                ?? throw new DomainException("User not found.");

            user.ChangeRole(request.NewRole);
            await _users.UpdateAsync(user, ct);
        }
    }
}
