using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLine.Domain.Entities;
using QLine.Domain.Abstractions;
using QLine.Infrastructure;
using QLine.Infrastructure.Persistence;
using QLine.Application;
using QLine.Application.Abstractions;
using QLine.Web.Components;
using QLine.Web.Hubs;
using FluentValidation;
using Serilog;
using System;
using System.Security.Claims;
using MudBlazor.Services;
using MediatR;
using QLine.Web.Services;
using QLine.Web.Endpoints;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddInfrastructure(builder.Configuration);
services.AddApplication();

services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();

services.AddAuthorization(options =>
{
    options.AddPolicy("StaffOnly", p => p.RequireRole("Staff", "Admin"));
});

services.AddHttpContextAccessor();
services.AddScoped<ICurrentUser, CurrentUserAccessor>();

services.AddScoped<QLine.Web.Services.BrowserTimeService>();

services.AddRazorComponents()
    .AddInteractiveServerComponents();

//builder.Services.AddMudServices();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbInitializer.InitializeAsync(db);
}

// Configure pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();