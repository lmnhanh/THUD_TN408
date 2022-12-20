using Microsoft.AspNetCore.Identity;
using THUD_TN408.Models;

namespace THUD_TN408.Authorization
{
	public static class DefaultRoles
	{
		public static async Task SeedAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
		{
			await roleManager.CreateAsync(new IdentityRole(Roles.SuperAdmin.ToString()));
			await roleManager.CreateAsync(new IdentityRole(Roles.Saleman.ToString()));
			await roleManager.CreateAsync(new IdentityRole(Roles.WarehouseManager.ToString()));
			await roleManager.CreateAsync(new IdentityRole(Roles.Customer.ToString()));
		}
	}
}
