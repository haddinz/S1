using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Support.Auth.Id.Models.Enum;
using Support.Auth.Id.Repository;
using Support.Auth.Id.Repository.Data;
using Support.Auth.Id.Middleware;
using Support.Auth.Id.Domain.ValueObject;
using Support.Auth.Id;
using Support.Notification.Id;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Connection InMemory Database
// Console.WriteLine("--> Using InMemory Database");
// builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("AuthMemory"));

// Add Connection PostgresSQL Database
Console.WriteLine("--> Support Auth Using PostgresSql Database");
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConn"))
);

// Add Option a substitute from Configuration
// builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
builder.Services
    .AddOptions<JwtSettings>()
    .Bind(builder.Configuration.GetSection(JwtSettings.SectionName))
    .ValidateOnStart();

builder.Services
    .AddOptions<FrontendSettings>()
    .Bind(builder.Configuration.GetSection(FrontendSettings.SectionName))
    .ValidateOnStart();

// Add disable ModelState Behaviour
// builder.Services.Configure<ApiBehaviorOptions>(opt => opt.SuppressModelStateInvalidFilter = true);

JwtSettings jwtSettings =
    builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
    ?? throw new InvalidOperationException("JwtSettings configuration missing.");

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecurityKey)
            ),
            RoleClaimType = ClaimTypes.Role,
            NameClaimType = JwtRegisteredClaimNames.Email,

            ClockSkew = TimeSpan.Zero,
        };
    });

builder
    .Services.AddAuthorizationBuilder()
    .AddPolicy("Admin", policy => policy.RequireRole(RoleEnum.Admin.ToString()))
    .AddPolicy(
        "User",
        policy => policy.RequireRole(RoleEnum.Admin.ToString(), RoleEnum.User.ToString())
    );

// Depedenci Injection Auth
builder.Services.AddAuthModule(builder.Configuration);
builder.Services.AddNotificationModule(builder.Configuration);

// Add Controller
builder.Services.AddControllers();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

await SeedData.SeedDatabase(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
