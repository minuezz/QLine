using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using QLine.Application.DTO;

namespace QLine.Application.Features.Admin.Services.Commands
{
    public sealed record UpsertServiceCommand(
        Guid? Id,
        Guid TenantId,
        Guid ServicePointId,
        string Name,
        int DurationMin,
        int BufferMin,
        int MaxPerDay
    ) : IRequest<ServiceDto>;
}
