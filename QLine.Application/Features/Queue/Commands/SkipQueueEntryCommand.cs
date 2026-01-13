using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using QLine.Application.Abstractions;

namespace QLine.Application.Features.Queue.Commands
{
    public sealed record SkipQueueEntryCommand(Guid QueueEntryId) : IRequest;
}
