using System;
using MediatR;

namespace QLine.Application.Features.Admin.ServicePoints.Commands
{
    public sealed record AssignStaffCommand(Guid ServicePointId, Guid UserId) : IRequest;
}
