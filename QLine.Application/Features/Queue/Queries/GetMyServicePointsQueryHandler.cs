using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using QLine.Application.Abstractions;
using QLine.Application.DTO;
using QLine.Domain;
using QLine.Domain.Abstractions;

namespace QLine.Application.Features.Queue.Queries
{
    public sealed class GetMyServicePointsQueryHandler : IRequestHandler<GetMyServicePointsQuery, IReadOnlyList<ServicePointDto>>
    {
        private readonly IServicePointRepository _servicePoints;
        private readonly ICurrentUser _currentUser;

        public GetMyServicePointsQueryHandler(IServicePointRepository servicePoints, ICurrentUser currentUser)
        {
            _servicePoints = servicePoints;
            _currentUser = currentUser;
        }

        public async Task<IReadOnlyList<ServicePointDto>> Handle(GetMyServicePointsQuery request, CancellationToken ct)
        {
            if (_currentUser.UserId is null)
                throw new DomainException("User is not authenticated.");

            var list = await _servicePoints.GetAssignedServicePointsAsync(_currentUser.UserId.Value, ct);

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
