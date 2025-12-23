using System;
using MediatR;

namespace QLine.Application.Features.Admin.ServicePoints.Commands
{
    public sealed record UnassignStaffCommand(Guid ServicePointId, Guid UserId) : IRequest;
}
