using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using QLine.Application.Features.Account.Commands;
using QLine.Domain.Abstractions;
using QLine.Domain.Entities;
using System.Security.Claims;

namespace QLine.Web.Endpoints
{
    public static class AccountEndpoints
    {
        public static void MapAccountEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/account/change-password", async (
                HttpContext http,
                IAppUserRepository users,
                IPasswordHasher<AppUser> passwordHasher) =>
            {
                var form = await http.Request.ReadFormAsync();
                var currentPassword = form["password"].ToString();
                var newPassword = form["newPassword"].ToString();
                var confirmPassword = form["confirmPassword"].ToString();

                if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(newPassword))
                {
                    return Results.Redirect("/profile?pwdError=invalid");
                }

                if (!string.Equals(newPassword, confirmPassword, StringComparison.Ordinal))
                {
                    return Results.Redirect("/profile?pwdError=confirmMismatch");
                }

                var userIdValue = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!Guid.TryParse(userIdValue, out var userId))
                    return Results.Redirect("/login");

                var user = await users.GetByIdAsync(userId, http.RequestAborted);
                if (user is null)
                    return Results.Redirect("/login");

                var verify = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword);
                if (verify is not PasswordVerificationResult.Success and not PasswordVerificationResult.SuccessRehashNeeded)
                    return Results.Redirect("/profile?pwdError=invalidCurrent");

                var newPasswordHash = passwordHasher.HashPassword(user, newPassword);
                user.UpdatePasswordHash(newPasswordHash);
                await users.UpdateAsync(user, http.RequestAborted);

                return Results.Redirect("/profile?pwdChanged=true");
            })
            .RequireAuthorization()
            .DisableAntiforgery();

            app.MapPost("/account/delete", async (
                HttpContext http,
                IMediator mediator) =>
            {
                var form = await http.Request.ReadFormAsync();
                var currentPassword = form["password"].ToString();

                if (string.IsNullOrWhiteSpace(currentPassword))
                {
                    return Results.Redirect("/profile?deleteError=missingPassword");
                }

                var userIdValue = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!Guid.TryParse(userIdValue, out var userId))
                    return Results.Redirect("/login");

                try
                {
                    await mediator.Send(new DeleteAccountCommand(userId, currentPassword), http.RequestAborted);
                }
                catch (QLine.Domain.DomainException)
                {
                    return Results.Redirect("/profile?deleteError=invalidPassword");
                }
                catch (Exception)
                {
                    return Results.Redirect("/profile?deleteError=serverError");
                }

                await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Results.Redirect("/login?accountRemoved=true");
            })
            .RequireAuthorization()
            .DisableAntiforgery();
        }
    }
}