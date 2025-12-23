using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QLine.Application.DTO;
using QLine.Domain.Abstractions;

namespace QLine.Application.Features.Admin.Queries
{
    public sealed class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IEnumerable<UserDto>>
    {
        private readonly IAppUserRepository _users;

        public GetAllUsersQueryHandler(IAppUserRepository users) => _users = users;

        public async Task<IEnumerable<UserDto>> Handle(GetAllUsersQuery request, CancellationToken ct)
        {
            var list = await _users.GetAllAsync(ct);
            return list
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.FullName,
                    Email = u.Email,
                    Role = u.Role.ToString()
                })
                .ToList();
        }
    }
}
