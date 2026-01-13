using MediatR;
using QLine.Application.Abstractions;
using QLine.Application.DTO;
using QLine.Domain;
using QLine.Domain.Abstractions;
using QLine.Domain.Enums;
using QLine.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QLine.Application.Features.Queue.Queries
{
    public sealed class GetMyServicePointsQueryHandler : IRequestHandler<GetMyServicePointsQuery, IReadOnlyList<ServicePointDto>>
    {
        private readonly IServicePointRepository _servicePoints;
        private readonly IAppUserRepository _users;
        private readonly ICurrentUser _currentUser;

        public GetMyServicePointsQueryHandler(IServicePointRepository servicePoints, ICurrentUser currentUser, IAppUserRepository users)
        {
            _servicePoints = servicePoints;
            _currentUser = currentUser;
            _users = users;
        }

        public async Task<IReadOnlyList<ServicePointDto>> Handle(GetMyServicePointsQuery request, CancellationToken ct)
        {
            if (_currentUser.UserId is null)
                throw new DomainException("User is not authenticated.");

            var user = await _users.GetByIdAsync(_currentUser.UserId.Value, ct)
                ?? throw new DomainException("User not found.");

            IReadOnlyList<ServicePoint> list;

            if (user.Role == UserRole.Admin)
            {
                list = await _servicePoints.GetAllAsync(ct);
            }
            else
            {
                list = await _servicePoints.GetAssignedServicePointsAsync(user.Id, ct);
            }

            return list
                .Select(sp => new ServicePointDto
                {
                    Id = sp.Id,
                    Name = sp.Name,
                    Address = sp.Address,
                    OpenHoursJson = sp.OpenHoursJson
                })
                .OrderBy(x => x.Name)
                .ToList();
        }
    }
}
