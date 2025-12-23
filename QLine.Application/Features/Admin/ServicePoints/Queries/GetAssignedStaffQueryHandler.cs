using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QLine.Application.DTO;
using QLine.Domain.Abstractions;
using QLine.Domain.Enums;

namespace QLine.Application.Features.Admin.ServicePoints.Queries
{
    public sealed class GetAssignedStaffQueryHandler : IRequestHandler<GetAssignedStaffQuery, IReadOnlyList<StaffDto>>
    {
        private readonly IServicePointRepository _servicePoints;

        public GetAssignedStaffQueryHandler(IServicePointRepository servicePoints)
        {
            _servicePoints = servicePoints;
        }

        public async Task<IReadOnlyList<StaffDto>> Handle(GetAssignedStaffQuery request, CancellationToken ct)
        {
            var assigned = await _servicePoints.GetAssignedStaffAsync(request.ServicePointId, ct);
            return assigned
                .Where(u => u.Role == UserRole.Staff)
                .Select(u => new StaffDto
                {
                    Id = u.Id,
                    Name = u.FullName,
                    Email = u.Email
                })
                .ToList();
        }
    }
}
