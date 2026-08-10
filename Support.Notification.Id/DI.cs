using Support.Notification.Client.Contract;
using Support.Notification.Client.Id.Services;
using Support.Notification.Id.Domain.ValueObject;

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

        services.AddScoped<INotificationClient, EmailSenderServices>();

        return services;
    }
}