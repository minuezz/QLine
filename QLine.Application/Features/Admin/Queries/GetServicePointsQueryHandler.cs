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
    public sealed class GetServicePointsQueryHandler
        : IRequestHandler<GetServicePointsQuery, IReadOnlyList<ServicePointDto>>
    {
        private readonly IServicePointRepository _servicePoints;

        public GetServicePointsQueryHandler(IServicePointRepository servicePoints)
            => _servicePoints = servicePoints;
        public async Task<IReadOnlyList<ServicePointDto>> Handle(
            GetServicePointsQuery request, 
            CancellationToken ct)
        {
            var list = await _servicePoints.GetByTenantAsync(request.TenantId, ct);
            return list
                .Select(sp => new ServicePointDto
                {
                    Id = sp.Id,
                    Name = sp.Name,
                    Address = sp.Address,
                    OpenHoursJson = sp.OpenHoursJson
                })
                .ToList();
        }
    }
}
