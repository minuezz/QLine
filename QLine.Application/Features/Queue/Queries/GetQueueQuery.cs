using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using QLine.Application.DTO;

namespace QLine.Application.Features.Queue.Queries
{
    public sealed record GetQueueQuery(Guid ServicePointId) : IRequest<QueueSnapshotDto>;
}
