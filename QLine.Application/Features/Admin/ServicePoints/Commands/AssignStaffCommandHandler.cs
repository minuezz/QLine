using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QLine.Domain;
using QLine.Domain.Abstractions;
using QLine.Domain.Entities;
using QLine.Domain.Enums;

namespace QLine.Application.Features.Admin.ServicePoints.Commands
{
    public sealed class AssignStaffCommandHandler : IRequestHandler<AssignStaffCommand>
    {
        private readonly IServicePointRepository _servicePoints;
        private readonly IAppUserRepository _users;

        public AssignStaffCommandHandler(IServicePointRepository servicePoints, IAppUserRepository users)
        {
            _servicePoints = servicePoints;
            _users = users;
        }

        public async Task Handle(AssignStaffCommand request, CancellationToken ct)
        {
            _ = await _servicePoints.GetByIdAsync(request.ServicePointId, ct)
                ?? throw new DomainException("Service point not found.");

            var user = await _users.GetByIdAsync(request.UserId, ct)
                ?? throw new DomainException("User not found.");

            if (user.Role != UserRole.Staff)
                throw new DomainException("Only staff users can be assigned to a service point.");

            var assignedStaff = await _servicePoints.GetAssignedStaffAsync(request.ServicePointId, ct);
            if (assignedStaff.Any(s => s.Id == request.UserId))
                return;

            var assignment = StaffAssignment.Create(request.ServicePointId, request.UserId);
            await _servicePoints.AddStaffAssignmentAsync(assignment, ct);
        }
    }
}
