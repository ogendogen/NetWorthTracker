using System.Text;
using FluentValidation;
using Mediator;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetWorthTracker.Api.Middleware;
using NetWorthTracker.Application.AssemblyMarker;
using NetWorthTracker.Application.Authentication;
using NetWorthTracker.Application.Authentication.Interfaces;
using NetWorthTracker.Application.Authentication.Services;
using NetWorthTracker.Application.Common.Handlers;
using NetWorthTracker.Application.Common.Models;
using NetWorthTracker.Application.Common.Services;
using NetWorthTracker.Domain.Common.Interfaces;
using NetWorthTracker.Domain.User.Interfaces;
using NetWorthTracker.Infrastructure;
using NetWorthTracker.Infrastructure.Repositories;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile(
    $"appsettings.secrets.{builder.Environment.EnvironmentName}.json",
    optional: builder.Environment.IsEnvironment("Testing"),
    reloadOnChange: false);
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.AddCommandLine(args);

if (builder.Environment.IsProduction()
    && string.IsNullOrWhiteSpace(builder.Configuration[$"{JwtSettings.SectionName}:SigningKey"]))
{
    throw new InvalidOperationException("The JWT signing key is required in production.");
}

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddOptions<JwtSettings>()
    .BindConfiguration(JwtSettings.SectionName)
    .Validate(
        settings => !string.IsNullOrWhiteSpace(settings.Issuer)
                    && !string.IsNullOrWhiteSpace(settings.Audience)
                    && !string.IsNullOrWhiteSpace(settings.SigningKey)
                    && settings.LifetimeMinutes > 0,
        "JWT settings are required.")
    .ValidateOnStart();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IEmailService, EmailService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("SpaDevelopment", policy => policy
        .WithOrigins(
            "http://localhost:4200",
            "https://localhost:4200",
            "http://localhost:4201",
            "https://localhost:4201")
        .AllowAnyHeader()
        .AllowAnyMethod());
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();
builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IOptions<JwtSettings>>((options, jwtOptions) =>
    {
        var jwtSettings = jwtOptions.Value;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SigningKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddDbContext<NetWorthTrackerDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Scoped);
builder.Services.AddScoped(
    typeof(IPipelineBehavior<,>),
    typeof(RequestLoggingBehavior<,>));
builder.Services.AddValidatorsFromAssembly(typeof(ApplicationAssemblyMarker).Assembly);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.Configure<LoggerFactoryOptions>(options =>
{
    options.ActivityTrackingOptions = ActivityTrackingOptions.TraceId
                                     | ActivityTrackingOptions.SpanId
                                     | ActivityTrackingOptions.ParentId;
});
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));
builder.Logging.AddOpenTelemetry(x =>
{
    x.SetResourceBuilder(ResourceBuilder.CreateEmpty()
        .AddService("NetWorthTracker.Api")
        .AddTelemetrySdk()
        .AddEnvironmentVariableDetector()
        .AddAttributes(new Dictionary<string, object>
        {
            ["host.type"] = Environment.MachineName,
            ["deployment.environment"] = builder.Environment.EnvironmentName,
        }));

    x.IncludeScopes = true;
    x.IncludeFormattedMessage = true;

    x.AddOtlpExporter(y =>
    {
        y.Endpoint = new Uri(builder.Configuration.GetValue<string>("Seq:ApiUrl")!);
        y.Protocol = OtlpExportProtocol.HttpProtobuf;
        y.Headers = $"X-Seq-ApiKey={builder.Configuration.GetValue<string>("Seq:ApiKey")}";
    });
});

var app = builder.Build();

app.UseMiddleware<HttpRequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("SpaDevelopment");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

/// <summary>
/// Exposes the application entry point to integration tests.
/// </summary>
public partial class Program;
