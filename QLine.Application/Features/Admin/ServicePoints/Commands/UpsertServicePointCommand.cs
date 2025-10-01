using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using QLine.Application.DTO;

namespace QLine.Application.Features.Admin.ServicePoints.Commands
{
    public sealed record UpsertServicePointCommand(
        Guid? Id,
        Guid TenantId,
        string Name,
        string Address,
        string OpenHoursJson
    ) : IRequest<ServicePointDto>;
}
