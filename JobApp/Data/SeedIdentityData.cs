using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace JobApp.Data;

public static class SeedIdentityData
{
    private const string adminUser = "admin@jobapp.local";
    private const string adminPassword = "AdminPassword123!";
    private const string employerUser = "employer@jobapp.local";
    private const string employeeUser = "employee@jobapp.local";
    private const string regularPassword = "UserPassword123$";
    private const string adminRole = "Admin";
    private const string employerRole = "Employer";
    private const string employeeRole = "Employee";

    
    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var prov = scope.ServiceProvider;

        var roleManager = prov.GetService<RoleManager<IdentityRole>>();
        var userManager = prov.GetService<UserManager<IdentityUser>>();

        if (roleManager == null || userManager == null)
        {
            
            return;
        }

        // Ensuring roles
        var roles = new[] { adminRole, employerRole, employeeRole };
        foreach (var r in roles)
        {
            if (!await roleManager.RoleExistsAsync(r))
            {
                await roleManager.CreateAsync(new IdentityRole(r));
            }
        }

        // Creating admin user
        var admin = await userManager.FindByEmailAsync(adminUser);
        if (admin == null)
        {
            admin = new IdentityUser { UserName = adminUser, Email = adminUser, EmailConfirmed = true };
            var result = await userManager.CreateAsync(admin, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, adminRole);
            }
        }

        // Create example employer user
        var emp = await userManager.FindByEmailAsync(employerUser);
        if (emp == null)
        {
            emp = new IdentityUser { UserName = "employer", Email = employerUser, EmailConfirmed = true };
            var result = await userManager.CreateAsync(emp, regularPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(emp, employerRole);
            }
        }

        // Create example employee user
        var user = await userManager.FindByEmailAsync(employeeUser);
        if (user == null)
        {
            user = new IdentityUser { UserName = "employee", Email = employeeUser, EmailConfirmed = true };
            var result = await userManager.CreateAsync(user, regularPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, employeeRole);
            }
        }
    }
}