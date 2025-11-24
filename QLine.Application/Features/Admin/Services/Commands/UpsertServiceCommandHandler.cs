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
using QLine.Application.Abstractions;

namespace QLine.Application.Features.Admin.Services.Commands
{
    public sealed class UpsertServiceCommandHandler
        : IRequestHandler<UpsertServiceCommand, ServiceDto>
    {
        private readonly IServiceRepository _repo;
        private readonly IRealtimeNotifier _realtime;

        public UpsertServiceCommandHandler(IServiceRepository repo, IRealtimeNotifier realtime)
        {
            _repo = repo;
            _realtime = realtime;
        }

        public async Task<ServiceDto> Handle(UpsertServiceCommand request, CancellationToken ct)
        {
            if (request.Id == null || request.Id == Guid.Empty)
            {
                var svc = Service.Create(
                    Guid.NewGuid(),
                    request.ServicePointId,
                    request.Name,
                    request.DurationMin,
                    request.BufferMin,
                    request.MaxPerDay
                );
                await _repo.AddAsync(svc, ct);

                await _realtime.QueueUpdated(request.ServicePointId, ct);

                return new ServiceDto
                {
                    Id = svc.Id,
                    Name = svc.Name,
                    DurationMin = svc.DurationMin,
                    BufferMin = svc.BufferMin,
                    MaxPerDay = svc.MaxPerDay,
                };
            }
            else
            {
                var existing = await _repo.GetByIdAsync(request.Id!.Value, ct)
                    ?? throw new DomainException("Service not found.");

                existing.Update(request.Name, request.DurationMin, request.BufferMin, request.MaxPerDay);

                await _repo.UpdateAsync(existing, ct);
                await _realtime.QueueUpdated(request.ServicePointId, ct);

                return new ServiceDto
                {
                    Id = existing.Id,
                    Name = existing.Name,
                    DurationMin = existing.DurationMin,
                    BufferMin = existing.BufferMin,
                    MaxPerDay = existing.MaxPerDay,
                };
            }
        }
    }
}
