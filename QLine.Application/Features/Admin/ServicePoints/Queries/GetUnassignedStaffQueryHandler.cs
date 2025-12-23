using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QLine.Application.DTO;
using QLine.Domain.Abstractions;
using QLine.Domain.Enums;

namespace QLine.Application.Features.Admin.ServicePoints.Queries
{
    public sealed class GetUnassignedStaffQueryHandler : IRequestHandler<GetUnassignedStaffQuery, IReadOnlyList<StaffDto>>
    {
        private readonly IServicePointRepository _servicePoints;
        private readonly IAppUserRepository _users;

        public GetUnassignedStaffQueryHandler(IServicePointRepository servicePoints, IAppUserRepository users)
        {
            _servicePoints = servicePoints;
            _users = users;
        }

        public async Task<IReadOnlyList<StaffDto>> Handle(GetUnassignedStaffQuery request, CancellationToken ct)
        {
            var assigned = await _servicePoints.GetAssignedStaffAsync(request.ServicePointId, ct);
            var all = await _users.GetAllAsync(ct);

            var assignedIds = assigned.Select(a => a.Id).ToHashSet();

            return all
                .Where(u => u.Role == UserRole.Staff && !assignedIds.Contains(u.Id))
                .Select(u => new StaffDto
                {
                    Id = u.Id,
                    Name = u.FullName,
                    Email = u.Email
                })
                .OrderBy(u => u.Name)
                .ToList();
        }
    }
}
