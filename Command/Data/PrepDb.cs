using Command.Data.Interfaces;
using Command.Models;
using Command.SyncDataServices;

namespace Command.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder applicationBuilder, bool isProduction)
    {
        using (IServiceScope serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            IPlatformDataClient grpcClient =
                serviceScope.ServiceProvider.GetService<IPlatformDataClient>()!;
            IEnumerable<Platform> platforms = grpcClient.ReturnAllPlatforms();

            SeedData(
                serviceScope.ServiceProvider.GetService<ICommandsRepo>()!,
                platforms,
                isProduction
            );
        }
    }

    private static void SeedData(
        ICommandsRepo repo,
        IEnumerable<Platform> platforms,
        bool isProduction
    )
    {
        if (platforms == null || !platforms.Any())
        {
            Console.WriteLine("--> No platform data received from gRPC");
            return;
        }

        if (!isProduction)
        {
            try
            {
                Console.WriteLine("--> Sending new Data Platfomrs.....");

                foreach (Platform plat in platforms)
                {
                    if (!repo.ExternalPlatformExist(plat.ExternalId))
                    {
                        repo.CreatePlatform(plat);
                    }
                }

                repo.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Cant Sending new Data Platforms {ex.Message}");
            }
        }
    }
}
