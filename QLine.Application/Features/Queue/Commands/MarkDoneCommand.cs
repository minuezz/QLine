using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace QLine.Application.Features.Queue.Commands
{
    public sealed record MarkDoneCommand(Guid QueueEntryId) : IRequest;
}
