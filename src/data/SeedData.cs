using Microsoft.AspNetCore.Identity;

namespace enova_academy.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        // 1. Criar roles
        string[] roleNames = ["ADMIN", "STUDENT"];

        foreach (var roleName in roleNames)
        {
            var roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // 2. Criar usuário ADMIN
        var adminEmail = "admin@enova.ac";
        var adminPassword = "Admin123!"; // trocar para senha segura em produção

        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(adminUser, adminPassword);

            // 3. Atribuir role ADMIN ao usuário
            await userManager.AddToRoleAsync(adminUser, "ADMIN");
        }
    }
}
