using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NetWorthTracker.Api.Configuration;
using NetWorthTracker.Api.Services;

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
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
