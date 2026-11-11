using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Identity;
using Olympus.DATA.Identity;
namespace Olympus.Data.Identity;
public static class RoleSeeder
{
    public static async Task SeedRoleAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles =
        {
            UserRoles.Mortal,
            UserRoles.Hero,
            UserRoles.God,
            UserRoles.Hades,
            UserRoles.Oracle,
            UserRoles.Zeus
        };
        foreach(var role in roles)
        {
            if(!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(role));
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Failed to create role: {role}");
                }
            } 
        }
        
    }
}