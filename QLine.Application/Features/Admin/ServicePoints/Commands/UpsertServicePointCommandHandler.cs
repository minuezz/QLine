using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using QLine.Application.DTO;
using QLine.Domain.Entities;
using QLine.Domain.Abstractions;
using QLine.Domain;

namespace QLine.Application.Features.Admin.ServicePoints.Commands
{
    public sealed class UpsertServicePointCommandHandler
        : IRequestHandler<UpsertServicePointCommand, ServicePointDto>
    {
        private readonly IServicePointRepository _repo;

        public UpsertServicePointCommandHandler(IServicePointRepository repo) => _repo = repo;

        public async Task<ServicePointDto> Handle(UpsertServicePointCommand request, CancellationToken ct)
        {
            if (request.Id == null || request.Id == Guid.Empty)
            {
                var sp = ServicePoint.Create(
                    Guid.NewGuid(),
                    request.Name,
                    request.Address,
                    request.OpenHoursJson
                );
                await _repo.AddAsync(sp, ct);
                return new ServicePointDto
                {
                    Id = sp.Id,
                    Name = sp.Name,
                    Address = sp.Address,
                };
            }
            else
            {
                var existing = await _repo.GetByIdAsync(request.Id!.Value, ct) 
                    ?? throw new DomainException("ServicePoint not found.");
                existing.Update(request.Name, request.Address, request.OpenHoursJson);
                await _repo.UpdateAsync(existing, ct);
                return new ServicePointDto
                {
                    Id = existing.Id,
                    Name = existing.Name,
                    Address = existing.Address,
                };
            }
        }
    }
}
