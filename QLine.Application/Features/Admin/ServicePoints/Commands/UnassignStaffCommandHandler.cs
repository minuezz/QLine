using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QLine.Domain.Abstractions;

namespace QLine.Application.Features.Admin.ServicePoints.Commands
{
    public sealed class UnassignStaffCommandHandler : IRequestHandler<UnassignStaffCommand>
    {
        private readonly IServicePointRepository _servicePoints;

        public UnassignStaffCommandHandler(IServicePointRepository servicePoints)
        {
            _servicePoints = servicePoints;
        }

        public async Task Handle(UnassignStaffCommand request, CancellationToken ct)
        {
            await _servicePoints.RemoveStaffAssignmentAsync(request.ServicePointId, request.UserId, ct);
        }
    }
}
