using Support.Notification.Client.Contract;
using Support.Notification.Client.Id.Services;
using Support.Notification.Id.Domain.ValueObject;
using Support.Notification.Id.Services;
using Support.Notification.Id.Services.BackgroundServices;
using Support.Notification.Id.Services.Interfaces;

namespace Support.Notification.Id;

public static class DependencyInjection
{
    public static IServiceCollection AddNotificationModule(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<SmtpOptions>()
            .Bind(configuration.GetSection(SmtpOptions.SectionName))
            .Validate(
                settings => !string.IsNullOrWhiteSpace(settings.Host),
                "SMTP Host must be configured.")
            .Validate(
                settings => settings.Port > 0,
                "SMTP Port must be greater than zero.")
            .Validate(
                settings => !string.IsNullOrWhiteSpace(settings.SenderEmail),
                "SMTP SenderEmail must be configured.")
            .ValidateOnStart();

        // Low level email sender (SMTP)
        services.AddScoped<INotificationClient, EmailSenderServices>();

        // Email queue + Hangfire job (server-nya tetap di host Auth)
        services.AddScoped<IEmailBackgroundServices, EmailBackgroundServices>();
        services.AddScoped<IEmailQueueServiceClient, EmailQueueService>();

        return services;
    }
}
