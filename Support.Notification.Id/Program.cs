using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Options;
using Support.Auth.Notification.Domain.ValueObject;
using Support.Notification.Id;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Connection Hangfire PostgresSQL Database
builder.Services.AddHangfire(config => config.UsePostgreSqlStorage(
    c => c.UseNpgsqlConnection(builder.Configuration.GetConnectionString("PostgresConn"))
));

// Add Option a substitute from Configuration
// builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection(SmtpOptions.SectionName));
builder 
    .Services.AddOptions<HangfireDashboard>()
    .Bind(builder.Configuration.GetSection(HangfireDashboard.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Add Repository
// builder.Services.AddScoped<IAuthRepositories,AuthRepositories>();

// Add Services
// builder.Services.AddScoped<INotificationClient, EmailSenderServices>();
builder.Services.AddNotificationModule(builder.Configuration);

// Add Hangfire and Background Worker
// builder.Services.AddHostedService<OutboxWorkerService>();
var workerCount = builder.Configuration.GetValue<int?>("Hangfire:WorkerCount") ?? (builder.Environment.IsProduction() ? 5 : 1);
builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = workerCount;
    options.Queues = new[] { "email", "default" };
});

// Add Controller
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Hangfire Dashboard UI
HangfireDashboard hangfireDashboard = app
    .Services.GetRequiredService<IOptions<HangfireDashboard>>().Value;

app.UseHangfireDashboard("/notification/hangfire", new DashboardOptions
{
    Authorization = new[]
    {
        new BasicAuthAuthorizationFilter(new BasicAuthAuthorizationFilterOptions
        {
            RequireSsl = hangfireDashboard.Ssl,
            SslRedirect = hangfireDashboard.SslRedirect,
            LoginCaseSensitive = true,
            Users = new[]
            {
                new BasicAuthAuthorizationUser
                {
                    Login = hangfireDashboard.Login,
                    PasswordClear = hangfireDashboard.Password
                }
            }
        })
    }
});


app.Run();

