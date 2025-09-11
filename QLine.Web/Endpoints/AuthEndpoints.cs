using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using QLine.Domain.Abstractions;
using System.Security.Claims;

namespace QLine.Web.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/auth/login", [IgnoreAntiforgeryToken] async (
                HttpContext http,
                IAppUserRepository users) =>
            {
                var form = await http.Request.ReadFormAsync();
                var email = form["email"].ToString();
                var returnUrl = form["returnUrl"].ToString();

                var tenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");

                var user = await users.GetByEmailAsync(tenantId, email, http.RequestAborted);
                if (user is null)
                    return Results.Redirect("/login?error=notfound");

                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new("tenant", user.TenantId.ToString()),
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                    new(ClaimTypes.Role, user.Role.ToString())
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return Results.Redirect(string.IsNullOrWhiteSpace(returnUrl) ? "/staff/queue" : returnUrl);
            });

            app.MapPost("/auth/logout", [IgnoreAntiforgeryToken] async (HttpContext http) =>
            {
                await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Results.Redirect("/login");
            });
        }
    }
}
