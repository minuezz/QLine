using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using QLine.Domain.Abstractions;

namespace QLine.Application.Features.Admin.ServicePoints.Commands
{
    public sealed class DeleteServicePointCommandHandler : IRequestHandler<DeleteServicePointCommand>
    {
        private readonly IServicePointRepository _repo;
        public DeleteServicePointCommandHandler(IServicePointRepository repo) => _repo = repo;

        public async Task Handle(DeleteServicePointCommand request, CancellationToken ct)
            => await _repo.DeleteAsync(request.ServicePointId, ct);
    }
}
