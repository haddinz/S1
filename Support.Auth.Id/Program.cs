using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Support.Auth.Id.Repository;
using Support.Auth.Id.Repository.Data;
using Support.Auth.Id.Services.Interfaces;
using Support.Auth.Id.Services.Security;

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
builder.Services.Configure<ApiBehaviorOptions>(opt => opt.SuppressModelStateInvalidFilter = true);

// Add Authentication and Authorization User and Password (costum)
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// Add Controller
builder.Services.AddControllers();

var app = builder.Build();

await SeedData.SeedDatabase(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseHttpsRedirection();

app.Run();
