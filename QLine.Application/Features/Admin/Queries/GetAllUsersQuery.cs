using System.Collections.Generic;
using MediatR;
using QLine.Application.DTO;

namespace QLine.Application.Features.Admin.Queries
{
    public sealed record GetAllUsersQuery() : IRequest<IEnumerable<UserDto>>;
}
