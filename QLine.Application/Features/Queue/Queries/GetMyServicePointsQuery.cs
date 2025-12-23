using System.Collections.Generic;
using MediatR;
using QLine.Application.DTO;

namespace QLine.Application.Features.Queue.Queries
{
    public sealed record GetMyServicePointsQuery() : IRequest<IReadOnlyList<ServicePointDto>>;
}
