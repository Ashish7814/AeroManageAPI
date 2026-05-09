using AeroManage.API.Middleware;
using AeroManage.BookingManagement.Application.Queries.Bookings;
using AeroManage.BookingManagement.Application.Services.Implementation;
using AeroManage.BookingManagement.Application.Services.Interfaces;
using AeroManage.BookingManagement.Infrastructure.Repositories.Implementation;
using AeroManage.BookingManagement.Infrastructure.Repositories.Interfaces;
using AeroManage.FlightManagement.Application.Handlers.Flights;
using AeroManage.FlightManagement.Application.Hubs;
using AeroManage.FlightManagement.Application.Queries.Flights;
using AeroManage.FlightManagement.Application.Services.Implementation;
using AeroManage.FlightManagement.Application.Services.Interfaces;
using AeroManage.FlightManagement.Infrastructure.Repositories.Implementation;
using AeroManage.FlightManagement.Infrastructure.Repositories.Interfaces;
using AeroManage.Shared.Service.Implementation;
using AeroManage.Shared.Service.Interfaces;
using AeroMange.Shared.Repositories;
using AeroMange.UserManagement.Application.Commands.Auth;
using AeroMange.UserManagement.Application.Service;
using AeroMange.UserManagement.Application.Service.Implementation;
using AeroMange.UserManagement.Infrastructure.Repositories.Implemention;
using AeroMange.UserManagement.Infrastructure.Repositories.Interfaces;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RabbitMQ.Client;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = builder.Configuration;

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(
        typeof(RegisterUserCommand).Assembly,
        typeof(GetBookingSummaryQuery).Assembly,
        typeof(GetFlightsQuery).Assembly
    );
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200"
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); // Required for SignalR
    });
});

// Register Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();

// Flight Management Repositories
builder.Services.AddScoped<IAirportRepository, AirportRepository>();
builder.Services.AddScoped<IAircraftRepository, AircraftRepository>();
builder.Services.AddScoped<IRouteRepository, RouteRepository>();
builder.Services.AddScoped<IFlightRepository, FlightRepository>();

// Register Services
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IBackgroundJobService, BackgroundJobService>();


// Enhanced Flight Management Repositories
builder.Services.AddScoped<IRouteLayoverRepository, RouteLayoverRepository>();
builder.Services.AddScoped<IFlightScheduleTemplateRepository, FlightScheduleTemplateRepository>();
builder.Services.AddScoped<IFlightNotificationRepository, FlightNotificationRepository>();
builder.Services.AddScoped<IWeatherAlertRepository, WeatherAlertRepository>();
builder.Services.AddScoped<IGateAssignmentRepository, GateAssignmentRepository>();
builder.Services.AddScoped<IFlightDelayRepository, FlightDelayRepository>();
builder.Services.AddScoped<IFlightNumberRepository, FlightNumberRepository>();
builder.Services.AddScoped<IFlightDashboardRepository, FlightDashboardRepository>();

builder.Services.AddScoped<IBookingPricingRepository, BookingPricingRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IPassengerRepository, PassengerRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IPromoCodeRepository, PromoCodeRepository>();
builder.Services.AddScoped<ISeatRepository, SeatRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();

builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddScoped<IQRCodeService, QRCodeService>();
builder.Services.AddScoped<IStripePaymentService, StripePaymentService>();
builder.Services.AddScoped<IBoardingPassRepository, BoardingPassRepository>();

// Configure JWT Authentication
/*var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"];*/

/*builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});*/


var jwtSecretKey = configuration["Jwt:SecretKey"];
var jwtIssuer = configuration["Jwt:Issuer"];
var jwtAudience = configuration["Jwt:Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey)),
        ClockSkew = TimeSpan.Zero
    };

    // Configure SignalR authentication
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.ClientTimeoutInterval = TimeSpan.FromMinutes(1);
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.HandshakeTimeout = TimeSpan.FromSeconds(15);
    options.MaximumReceiveMessageSize = 32 * 1024; // 32KB
});

var redisConnection = builder.Configuration.GetConnectionString("RedisConnection");
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = ConfigurationOptions.Parse(redisConnection);
    config.AbortOnConnectFail = false;
    config.ConnectTimeout = 5000;
    config.SyncTimeout = 5000;
    config.ReconnectRetryPolicy = new ExponentialRetry(5000);
    return ConnectionMultiplexer.Connect(config);
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnection;
    options.InstanceName = builder.Configuration["Redis:InstanceName"] ?? "AirlineManagement:";
});

builder.Services.AddSingleton<ICacheService, CacheService>();

builder.Services.AddHangfire(config =>
{
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
        {
            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
            QueuePollInterval = TimeSpan.Zero,
            UseRecommendedIsolationLevel = true,
            DisableGlobalLocks = true,
            SchemaName = "Hangfire"
        });
});

builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = int.Parse(builder.Configuration["Hangfire:WorkerCount"] ?? "5");
    options.Queues = new[] { "critical", "default", "low" };
});

builder.Services.AddSingleton<IMessageQueueService, RabbitMQService>();


// ==================== HEALTH CHECKS ====================
builder.Services.AddHealthChecks()
    .AddCheck("database", () =>
    {
        try
        {
            using var connection = new Microsoft.Data.SqlClient.SqlConnection(configuration.GetConnectionString("DefaultConnection"));
            connection.Open();
            return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Database is reachable");
        }
        catch (Exception ex)
        {
            return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("Database is unreachable", ex);
        }
    })
    .AddCheck("redis", () =>
    {
        try
        {
            var redis = builder.Services.BuildServiceProvider().GetRequiredService<IConnectionMultiplexer>();
            return redis.IsConnected
                ? Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Redis is connected")
                : Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("Redis is disconnected");
        }
        catch (Exception ex)
        {
            return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("Redis check failed", ex);
        }
    })
    //.AddCheck("rabbitmq", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("RabbitMQ is running"))
    .AddCheck("rabbitmq", () =>
    {
        try
        {
            var rabbitConfig = configuration.GetSection("RabbitMQ");
            var factory = new ConnectionFactory
            {
                HostName = rabbitConfig["HostName"],
                Port = int.Parse(rabbitConfig["Port"]),
                UserName = rabbitConfig["UserName"],
                Password = rabbitConfig["Password"],
                VirtualHost = rabbitConfig["VirtualHost"]
            };

            // Try to open a connection
            using var connection =  factory.CreateConnectionAsync().GetAwaiter().GetResult();
            using var channel = connection.CreateChannelAsync();

            return HealthCheckResult.Healthy("RabbitMQ is reachable");
        }
        catch (Exception ex)
        {
            return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("RabbitMQ is unreachable", ex);
        }
    });




// ==================== RESPONSE COMPRESSION ====================
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

// Configure Swagger
/*builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Airline Management System API - CQRS Pattern",
        Version = "v1",
        Description = "API for Airline Management System - User Management Module with CQRS and MediatR",
        Contact = new OpenApiContact
        {
            Name = "Your Name",
            Email = "your.email@example.com"
        }
    });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});*/

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
   /* app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Airline Management API V1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });*/
}

app.UseHttpsRedirection();

// ==================== HANGFIRE DASHBOARD ====================
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() },
    DashboardTitle = "Airline Management - Background Jobs"
});

// ==================== SIGNALR HUBS ====================
app.MapHub<FlightHub>("/hubs/flights");


app.UseCors("AllowAngular"); 

// ==================== CONTROLLERS ====================

// ==================== HEALTH CHECKS ====================
app.MapHealthChecks("/health");

// ==================== START RABBITMQ CONSUMER ====================
var messageQueue = app.Services.GetRequiredService<IMessageQueueService>();
messageQueue.StartConsuming();

// ==================== CONFIGURE RECURRING JOBS ====================
/*var backgroundJobService = app.Services.GetRequiredService<IBackgroundJobService>();
backgroundJobService.ScheduleRecurringFlightCleanup();*/

/*using (var scope = app.Services.CreateScope())
{
    var backgroundJobService =
        scope.ServiceProvider.GetRequiredService<IBackgroundJobService>();

    backgroundJobService.ScheduleRecurringFlightCleanup();
}*/


// Add more recurring jobs
//RecurringJob.AddOrUpdate(
//    "flight-status-sync",
//    () => Console.WriteLine("Syncing flight statuses..."),
//    "*/5 * * * *" // Every 5 minutes
//);

//RecurringJob.AddOrUpdate(
//    "weather-update",
//    () => Console.WriteLine("Updating weather data..."),
//    "*/15 * * * *" // Every 15 minutes
//);




app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Health Check Result
public class HealthCheckResult
{
    public static Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult Healthy(string description)
    {
        return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(description);
    }
}
