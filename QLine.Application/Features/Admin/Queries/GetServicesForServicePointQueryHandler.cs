using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using QLine.Application.DTO;
using QLine.Domain.Abstractions;
namespace QLine.Application.Features.Admin.Queries
{
    public sealed class GetServicesForServicePointQueryHandler
        : IRequestHandler<GetServicesForServicePointQuery, IReadOnlyList<ServiceDto>>
    {
        private readonly IServiceRepository _services;

        public GetServicesForServicePointQueryHandler(IServiceRepository services) 
            => _services = services;

        public async Task<IReadOnlyList<ServiceDto>> Handle(
            GetServicesForServicePointQuery request, 
            CancellationToken ct)
        {
            var list = await _services.GetByServicePointAsync(request.ServicePointId, ct);
            return list
                .Select(s => new ServiceDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    DurationMin = s.DurationMin,
                    BufferMin = s.BufferMin
                })
                .ToList();
        }
    }
}
