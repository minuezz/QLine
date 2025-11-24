using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using QLine.Application.DTO;

namespace QLine.Application.Features.Admin.Queries
{
    public sealed record GetServicePointsQuery() : IRequest<IReadOnlyList<ServicePointDto>>;
}
