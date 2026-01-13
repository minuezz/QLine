using System;
using System.Collections.Generic;
using MediatR;
using QLine.Application.DTO;

namespace QLine.Application.Features.Admin.ServicePoints.Queries
{
    public sealed record GetUnassignedStaffQuery(Guid ServicePointId) : IRequest<IReadOnlyList<StaffDto>>;
}
