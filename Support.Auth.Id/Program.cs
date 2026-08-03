using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Support.Auth.Id.Models.Enum;
using Support.Auth.Id.Repositories;
using Support.Auth.Id.Repositories.Interfaces;
using Support.Auth.Id.Repository;
using Support.Auth.Id.Repository.Data;
using Support.Auth.Id.Services.Interfaces;
using Support.Auth.Id.Middleware;
using Support.Auth.Id.Models.DTOs.Response;
using Support.Auth.Id.Common.CommandQuery;
using Support.Auth.Id.Features.Handler.Users;
using Support.Auth.Id.Commons.Command;
using Support.Auth.Id.Features.Handler.Auth;
using Support.Auth.Id.Features.Security;
using Support.Auth.Id.Domain.ValueObject;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Connection Database
// Console.WriteLine("--> Using InMemory Database");
// builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("AuthMemory"));
Console.WriteLine("--> Support Auth Using PostgresSql Database");
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConn"))
);

// Add Option a substitute from Configuration
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));

// Add disable ModelState Behaviour
// builder.Services.Configure<ApiBehaviorOptions>(opt => opt.SuppressModelStateInvalidFilter = true);

// Add Authentication and Authorization User and Password (costum)
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();

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

// Add Repository
builder.Services.AddScoped<IAuthRepositories,AuthRepositories>();

// Add Security Services
builder.Services.AddScoped<IPasswordPolicyValidator, PasswordPolicyValidator>();

// Add Handler User
builder.Services.AddScoped<ICommandQueryHandler<GetActiveUserQuery, UserProfileResponse>, GetActiveUserQueryHandler>();
builder.Services.AddScoped<ICommandQueryHandler<GetUsersQuery, PagedResponse<UserProfileResponse>>, GetUsersQueryHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateUserCommand>, UpdateUserCommandHandler>();

// Add Handler Auth
builder.Services.AddScoped<ICommandHandler<LoginCommand, AuthResponse>, LoginCommandHandler>();
builder.Services.AddScoped<ICommandHandler<LogoutCommand>, LogoutCommandHandler>();
builder.Services.AddScoped<ICommandHandler<RegisterCommand>, RegisterCommandHandler>();
builder.Services.AddScoped<ICommandHandler<RefreshTokenCommand, AuthResponse>, RefreshTokenCommandHandler>();
builder.Services.AddScoped<ICommandHandler<ChangesPasswordCommand>, ChangesPasswordCommandHandler>();
builder.Services.AddScoped<ICommandHandler<VerifyEmailCommand>, VerifyEmailCommandHandler>();


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
