using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace QLine.Application.Features.Admin.ServicePoints.Commands
{
    public sealed record DeleteServicePointCommand(Guid ServicePointId) : IRequest;
}
