using Microsoft.EntityFrameworkCore;
using Support.Platform.AsyncDataServices;
using Support.Platform.Data;
using Support.Platform.Interfaces;
using Support.Platform.Repository;
using Support.Platform.SycnDataServices.Grpc;
using Support.Platform.SycnDataServices.Http;
using Support.Platform.SycnDataServices.Http.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment;
var configuration = builder.Configuration;

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// builder.Services.AddDbContext<AppDBContext>(opt => opt.UseSqlServer(configuration.GetConnectionString("SupportConnection")));

if (env.IsProduction())
{
    Console.WriteLine($"--> Using In SQL Database");
    builder.Services.AddDbContext<AppDBContext>(opt =>
        opt.UseSqlServer(configuration.GetConnectionString("SupportConnection"))
    );
}
else
{
    Console.WriteLine($"--> Using In Memory Database");
    builder.Services.AddDbContext<AppDBContext>(opt => opt.UseInMemoryDatabase("InMem"));
}

// Hosted Service
builder.Services.AddHostedService<RabbitMqInitializer>();

// Interface Injection
builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();

builder.Services.AddHttpClient<ICommandDataCLient, HttpCommandDataClient>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

// Grpc Injection
builder.Services.AddGrpc();

// Automapper service
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Adding Controller
builder.Services.AddControllers();

// Adding Swagger 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.MapGrpcService<GrpcPlatformService>();
app.MapGet(
    "/protos/platform.proto",
    async context =>
    {
        await context.Response.WriteAsync(File.ReadAllText("Protos/platform.proto"));
    }
);

app.UseHttpsRedirection();

PrepDb.PrepPopulation(app, env.IsProduction());

app.Run();
