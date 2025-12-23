using Command.AsyncDataServices;
using Command.Data;
using Command.Data.Interfaces;
using Command.Data.Repository;
using Command.EventProcessing;
using Command.EventProcessing.Interfaces;
using Command.SyncDataServices;
using Command.SyncDataServices.Grpc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment;

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

//Add Automapper Service
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//Add Injetion Scoped
builder.Services.AddScoped<ICommandsRepo, CommandsRepo>();
builder.Services.AddScoped<IPlatformDataClient, PlatformDataClient>();

//Add Injetion Singleton
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();

//Add Injection Host and Baground Services
builder.Services.AddHostedService<MessageBusSubscriber>();

//Add Database
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));

//Add Swagger
builder.Services.AddControllers();
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

PrepDb.PrepPopulation(app, env.IsProduction());

app.MapControllers();

app.UseHttpsRedirection();

app.Run();
