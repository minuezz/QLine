using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QLine.Domain.Entities;
using QLine.Infrastructure.Persistence;
using QLine.Web.Components;
using QLine.Web.Hubs;
using Serilog;
using System;
using MudBlazor.Services;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console());

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddAuthentication().AddIdentityCookies();
builder.Services.AddAuthorization();
builder.Services
    .AddIdentityCore<AppUser>(o => { /* пароли/требования */ })
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddSignalR();

builder.Services.AddMudServices();

//builder.Services.AddMediatR(typeof(Program));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    //await db.Database.MigrateAsync();
    //await DbSeed.SeedAsync(db);
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapHub<QueueHub>("/hubs/queue");

app.Run();