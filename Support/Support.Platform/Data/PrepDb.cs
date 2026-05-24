using Microsoft.EntityFrameworkCore;
using Support.Platform.Models;

namespace Support.Platform.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder app, bool IsProduction)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        SeedData(serviceScope.ServiceProvider.GetService<AppDBContext>()!, IsProduction);
    }

    private static void SeedData(AppDBContext context, bool isProduction)
    {
        if (isProduction)
        {
            Console.WriteLine("--> Attemting to Aplly Migrations");
            try
            {
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could Bot Aplly Migrations {ex.Message}");
            }
        }

        if (!context.Platforms.Any())
        {
            Console.WriteLine("Seeding data....");

            context.Platforms.AddRange(
                new PlatformModel()
                {
                    Name = "Dot Net",
                    Publisher = "Microsoft",
                    Cost = "Free",
                },
                new PlatformModel()
                {
                    Name = "Postgre SQL",
                    Publisher = "Communitas",
                    Cost = "Free",
                },
                new PlatformModel()
                {
                    Name = "Kubernetes",
                    Publisher = "Cloud Computing",
                    Cost = "Free",
                }
            );

            context.SaveChanges();
        }
        else
        {
            Console.WriteLine("Data Already Mounted");
        }
    }
}
