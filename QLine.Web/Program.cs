using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QLine.Domain.Entities;
using QLine.Infrastructure;
using QLine.Infrastructure.Persistence;
using QLine.Application;
using QLine.Web.Components;
using QLine.Web.Hubs;
using FluentValidation;
using Serilog;
using System;
using MudBlazor.Services;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

builder.Services.AddScoped<QLine.Web.Services.BrowserTimeService>();

builder.Services.AddRazorComponents()
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

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();