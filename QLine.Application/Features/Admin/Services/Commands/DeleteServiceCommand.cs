using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace QLine.Application.Features.Admin.Services.Commands
{
    public sealed record DeleteServiceCommand(Guid ServiceId) : IRequest;
}
