using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QLine.Domain.Abstractions;
using QLine.Domain.Entities;
using QLine.Domain.Enums;
using System.Security.Claims;

namespace QLine.Web.Endpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/auth/login", [IgnoreAntiforgeryToken] async (
                HttpContext http,
                IAppUserRepository users,
                IPasswordHasher<AppUser> passwordHasher) =>
            {
                var form = await http.Request.ReadFormAsync();
                var email = form["email"].ToString();
                var password = form["password"].ToString();
                var returnUrl = form["returnUrl"].ToString();

                var user = await users.GetByEmailAsync(email, http.RequestAborted);
                if (user is null)
                    return Results.Redirect("/login?error=notfound");

                var verificationResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
                if (verificationResult is not PasswordVerificationResult.Success and not PasswordVerificationResult.SuccessRehashNeeded)
                    return Results.Redirect("/login?error=invalid");

                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                    new(ClaimTypes.Role, user.Role.ToString())
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return Results.Redirect(string.IsNullOrWhiteSpace(returnUrl) ? "/staff/queue" : returnUrl);
            });

            app.MapPost("/auth/register", [IgnoreAntiforgeryToken] async (
                HttpContext http,
                IAppUserRepository users,
                IPasswordHasher<AppUser> passwordHasher) =>
            {
                var form = await http.Request.ReadFormAsync();
                var firstName = form["firstName"].ToString();
                var lastName = form["lastName"].ToString();
                var email = form["email"].ToString();
                var password = form["password"].ToString();

                if (string.IsNullOrWhiteSpace(firstName)
                    || string.IsNullOrWhiteSpace(lastName)
                    || string.IsNullOrWhiteSpace(email)
                    || string.IsNullOrWhiteSpace(password))
                {
                    return Results.Redirect("/register?error=invalid");
                }

                var existingUser = await users.GetByEmailAsync(email, http.RequestAborted);
                if (existingUser is not null)
                    return Results.Redirect("/register?error=exists");

                var user = AppUser.Create(Guid.NewGuid(), email, firstName, lastName, UserRole.Client);
                var passwordHash = passwordHasher.HashPassword(user, password);
                user.UpdatePasswordHash(passwordHash);

                await users.AddAsync(user, http.RequestAborted);

                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.Name, user.FullName),
                    new(ClaimTypes.Role, user.Role.ToString())
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await http.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return Results.Redirect("/");
            });

            app.MapPost("/auth/logout", [IgnoreAntiforgeryToken] async (HttpContext http) =>
            {
                await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Results.Redirect("/login");
            });
        }
    }
}
