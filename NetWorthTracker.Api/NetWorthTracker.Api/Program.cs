using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NetWorthTracker.Api.Configuration;
using NetWorthTracker.Api.Services;
using NetWorthTracker.Infrastructure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsProduction()
    && string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("Jwt__SigningKey")))
{
    throw new InvalidOperationException("The Jwt__SigningKey environment variable is required in production.");
}

var jwtSettings = builder.Configuration.GetRequiredSection(JwtSettings.SectionName).Get<JwtSettings>()
    ?? throw new InvalidOperationException("JWT settings are required.");

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetRequiredSection(JwtSettings.SectionName));
builder.Services.AddSingleton<TokenService>();
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
    .AddJwtBearer(options =>
    {
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

// todo: register mediatr - should automatically detect all handler in 'application' project
//builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

var app = builder.Build();

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
