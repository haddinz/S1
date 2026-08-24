using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Support.Auth.Id.Models.Entity;
using Support.Auth.Id.Models.Enum;
using Support.Auth.Id.Repository.Data;
using Support.Auth.Id.Services.Interfaces;
using Support.Auth.Id.Domain.Entity;

namespace Support.Auth.Id.Repository;

public class SeedData
{
    public static async Task SeedDatabase(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        IAppHasher AppHasher =
            scope.ServiceProvider.GetRequiredService<IAppHasher>();

        ILogger<SeedData> logger = scope.ServiceProvider.GetRequiredService<ILogger<SeedData>>();

        try
        {
            logger.LogInformation("--> Migrate database is creating");
            await context.Database.MigrateAsync();

            // Add roles
            logger.LogInformation("--> Add and sending roles");
            if (!await context.Roles.AnyAsync())
            {
                Role adminRole = Role.Create(
                    RoleEnum.Admin.ToString(),
                    RoleEnum.Admin.ToString(),
                    "Administator"
                );
                Role userRole = Role.Create(
                    RoleEnum.User.ToString(),
                    RoleEnum.User.ToString(),
                    "User"
                );

                await context.Roles.AddRangeAsync(adminRole, userRole);
                await context.SaveChangesAsync();
            }

            // Add admin user
            logger.LogInformation("--> Sending admin user");

            string adminEmail = "admin@support.id";
            string password = "admin123";

            bool isAdminEmailExists = await context.Users.AnyAsync(user =>
                user.Email == adminEmail
            );

            if (!isAdminEmailExists)
            {
                Role adminRole = await context.Roles.FirstAsync(role =>
                    role.RoleName == RoleEnum.Admin.ToString()
                );

                string passwordHash = AppHasher.Hash(password);

                User adminUser = User.Register(
                    email: adminEmail,
                    fullName: "Administator",
                    userName: RoleEnum.Admin.ToString(),
                    passwordHash: passwordHash
                );

                adminUser.AssignRole(adminRole);

                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();
            }

            // Add normal user
            logger.LogInformation("--> Sending normal user");

            string userEmail = "user@support.id";

            bool isUserEmailExist = await context.Users.AnyAsync(user => user.Email == userEmail);

            if (!isUserEmailExist)
            {
                Role userRole = await context.Roles.FirstAsync(role =>
                    role.RoleName == RoleEnum.User.ToString()
                );

                string passwordHash = AppHasher.Hash(password);

                User normalUser = User.Register(
                    email: userEmail,
                    fullName: "User",
                    userName: RoleEnum.User.ToString(),
                    passwordHash: passwordHash
                );

                normalUser.AssignRole(userRole);

                await context.Users.AddAsync(normalUser);
                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            logger.LogError("--> An Error Occured while creating database {ex}", ex.Message);
        }
    }
}
