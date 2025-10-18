using ASI.Basecode.Data;
using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Repositories;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Services;
using ASI.Basecode.WebApp;
using ASI.Basecode.WebApp.Extensions.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    ContentRootPath = Directory.GetCurrentDirectory(),
});

// Configure URLs
builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001");

// Configure Kestrel to use specific ports
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5000); // HTTP port
    serverOptions.ListenAnyIP(5001, listenOptions => // HTTPS port
    {
        listenOptions.UseHttps();
    });
});

// Load configuration
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

// Logging setup
builder.Logging
    .AddConfiguration(builder.Configuration.GetLoggingSection())
    .AddConsole()
    .AddDebug();

// Database connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MarshallContext>(options =>
    options.UseSqlServer(connectionString));

// Register UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Repository registrations
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoomRepository, RoomRepository>();
builder.Services.AddScoped<IRoomAmenityRepository, RoomAmenityRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// Service registrations
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IRoomAmenityService, RoomAmenityService>();
builder.Services.AddScoped<IBookingService, BookingService>();

// Authentication services
builder.Services.AddScoped<ASI.Basecode.WebApp.Authentication.SignInManager>();
builder.Services.AddScoped<ASI.Basecode.WebApp.Authentication.TokenProviderOptionsFactory>();
builder.Services.AddScoped<ASI.Basecode.WebApp.Authentication.TokenValidationParametersFactory>();

// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Allow frontend access
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.SetIsOriginAllowed(_ => true) // Allow any origin in development
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddControllers();

// Add Distributed Cache and Session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add HTTP Context Accessor
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseRouting();
app.UseCors("AllowReact"); // CORS must be before Auth
app.UseAuthentication();
app.UseAuthorization();
app.UseSession(); // Add session middleware
app.MapControllers().RequireCors("AllowReact");

app.Run();
