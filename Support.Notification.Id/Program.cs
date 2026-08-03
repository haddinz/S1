using Support.Notification.Client.Contract;
using Support.Notification.Client.Id.Services;
using Support.Notification.Id.src.Domain.ValueObject;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Option a substitute from Configuration
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection(SmtpOptions.SectionName));

// Add Repository
// builder.Services.AddScoped<IAuthRepositories,AuthRepositories>();

// Add Services
builder.Services.AddScoped<INotificationClient, EmailSenderServices>();

// Add Controller
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();

