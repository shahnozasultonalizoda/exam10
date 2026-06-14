using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Seed;

public static class RoleSeeder
{
    public static async Task SeedAsync(
        RoleManager<IdentityRole> roleManager,
        UserManager<User> userManager)
    {
        foreach (var role in new[] { UserRoles.Admin, UserRoles.Manager, UserRoles.User })
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        var adminEmail = "admin@gmail.com";
        var admin = await userManager.FindByEmailAsync(adminEmail);

        if (admin == null)
        {
            var newAdmin = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Administrator"
            };

            await userManager.CreateAsync(newAdmin, "Admin123");
            await userManager.AddToRoleAsync(newAdmin, UserRoles.Admin);
        }
    }
}