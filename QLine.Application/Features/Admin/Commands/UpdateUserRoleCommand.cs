using System;
using MediatR;
using QLine.Domain.Enums;

namespace QLine.Application.Features.Admin.Commands
{
    public sealed record UpdateUserRoleCommand(Guid UserId, UserRole NewRole) : IRequest;
}
