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

//builder.Host.UseSerilog((ctx, lc) => lc
//    .ReadFrom.Configuration(ctx.Configuration)
//    .Enrich.FromLogContext()
//    .WriteTo.Console());

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddApplication();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//builder.Services.AddDbContext<AppDbContext>(opt =>
//    opt.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

//builder.Services.AddAuthentication().AddIdentityCookies();
//builder.Services.AddAuthorization();
//builder.Services
//    .AddIdentityCore<AppUser>(o => { /* пароли/требования */ })
//    .AddEntityFrameworkStores<AppDbContext>();

//builder.Services.AddSignalR();

//builder.Services.AddMudServices();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbInitializer.InitializeAsync(db);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    //app.UseDeveloperExceptionPage();
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseExceptionHandler("/Error");
    app.UseHsts();
    //using var scope = app.Services.CreateScope();
    //var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    //await db.Database.MigrateAsync();
    //await DbSeed.SeedAsync(db);
}

//app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

//app.UseAuthentication();
//app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

//app.MapHub<QueueHub>("/hubs/queue");

app.Run();