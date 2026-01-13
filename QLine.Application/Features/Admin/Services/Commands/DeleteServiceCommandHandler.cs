using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using QLine.Domain.Abstractions;

namespace QLine.Application.Features.Admin.Services.Commands
{
    public sealed class DeleteServiceCommandHandler : IRequestHandler<DeleteServiceCommand>
    {
        private readonly IServiceRepository _repo;
        public DeleteServiceCommandHandler(IServiceRepository repo) => _repo = repo;

        public async Task Handle(DeleteServiceCommand request, CancellationToken ct)
            => await _repo.DeleteAsync(request.ServiceId, ct);
    }
}
