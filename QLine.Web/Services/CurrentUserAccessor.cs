using System.Security.Claims;
using QLine.Application.Abstractions;

namespace QLine.Web.Services
{
    public sealed class CurrentUserAccessor : ICurrentUser
    {
        private readonly IHttpContextAccessor _http;

        public CurrentUserAccessor(IHttpContextAccessor http) => _http = http;

        public Guid? UserId
            => Guid.TryParse(_http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

        public string? Email => _http.HttpContext?.User.FindFirstValue(ClaimTypes.Email);

        public string? Role => _http.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
    }
}
