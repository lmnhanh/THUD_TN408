using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Security.Claims;
using THUD_TN408.Models;

namespace THUD_TN408.Authorization
{
	public static class DefaultUsers
	{
		public static async Task SeedBasicUserAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
		{
			var defaultUser = new User
			{
				UserName = "anhle184561@gmail.com",
				FirstName = "Le Minh",
				LastName = "Anh",
				Email = "anhle184561@gmail.com",
				EmailConfirmed = true
			};
			if (userManager.Users.All(u => u.Id != defaultUser.Id))
			{
				var user = await userManager.FindByEmailAsync(defaultUser.Email);
				if (user == null)
				{
					await userManager.CreateAsync(defaultUser, "LMAlmaht2st@");
					await userManager.AddToRoleAsync(defaultUser, Roles.SaleManager.ToString());


					await roleManager.AddPermissionAsCustomer();
					await roleManager.AddPermissionAsSaleManager();
					await roleManager.AddPermissionAsWarehouseManager();
					await roleManager.AddPermissionAsSaleman();
				}
			}
		}

		public static async Task AddPermissionAsCustomer(this RoleManager<IdentityRole> roleManager)
		{
			var customerRole = await roleManager.FindByNameAsync("Customer");
			await roleManager.AddClaimAsync(customerRole, new Claim("Permission", Permissions.Products.View));
			await roleManager.AddClaimAsync(customerRole, new Claim("Permission", Permissions.Carts.View));
			await roleManager.AddClaimAsync(customerRole, new Claim("Permission", Permissions.Carts.Create));
			await roleManager.AddClaimAsync(customerRole, new Claim("Permission", Permissions.Carts.Delete));
			await roleManager.AddClaimAsync(customerRole, new Claim("Permission", Permissions.Carts.Edit));
			await roleManager.AddClaimAsync(customerRole, new Claim("Permission", Permissions.Orders.Create));
			await roleManager.AddClaimAsync(customerRole, new Claim("Permission", Permissions.CustomerAccounts.Edit));
			await roleManager.AddClaimAsync(customerRole, new Claim("Permission", Permissions.CustomerAccounts.View));
			await roleManager.AddClaimAsync(customerRole, new Claim("Permission", Permissions.CustomerAccounts.Delete));
		}

		public static async Task AddPermissionAsWarehouseManager(this RoleManager<IdentityRole> roleManager)
		{
			var warehouseRole = await roleManager.FindByNameAsync("WarehouseManager");
			await roleManager.AddClaimAsync(warehouseRole, new Claim("Permission", Permissions.Products.View));
			await roleManager.AddClaimAsync(warehouseRole, new Claim("Permission", Permissions.Products.Create));
			await roleManager.AddClaimAsync(warehouseRole, new Claim("Permission", Permissions.Categories.View));
			await roleManager.AddClaimAsync(warehouseRole, new Claim("Permission", Permissions.Categories.Create));
			await roleManager.AddClaimAsync(warehouseRole, new Claim("Permission", Permissions.Warehouses.View));
			await roleManager.AddClaimAsync(warehouseRole, new Claim("Permission", Permissions.Warehouses.Create));
			await roleManager.AddClaimAsync(warehouseRole, new Claim("Permission", Permissions.Warehouses.Delete));
			await roleManager.AddClaimAsync(warehouseRole, new Claim("Permission", Permissions.Warehouses.Edit));
			//Còn phieu nha va xuat
		}

		public static async Task AddPermissionAsSaleman(this RoleManager<IdentityRole> roleManager)
		{
			var managerRole = await roleManager.FindByNameAsync("Saleman");
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.Products.View));
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.Orders.View));
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.Warehouses.View));
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.Payments.View));
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.Promotions.View));
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.CustomerAccounts.View));
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.Categories.View));
		}

		public static async Task AddPermissionAsSaleManager(this RoleManager<IdentityRole> roleManager)
		{
			var managerRole = await roleManager.FindByNameAsync("SaleManager");
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.Products.View));
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.Products.Create));
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.Products.Delete));
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.Products.Edit));
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.Orders.View));
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.Warehouses.View));
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.Payments.View));
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.Payments.Create));
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.Payments.Edit));
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.Payments.Delete));
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.Promotions.View));
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.Promotions.Create));
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.Promotions.Delete));
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.Promotions.Edit));
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.CustomerAccounts.View));
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.Categories.View));
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.Categories.Create));
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.Categories.Edit));
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.Categories.Delete));
		}

		public static async Task SeedSuperAdminAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
		{
			var defaultUser = new User
			{
				UserName = "lmanh0408@gmail.com",
				FirstName = "Lê Minh",
				LastName= "Anh",
				Email = "lmanh0408@gmail.com",
				EmailConfirmed = true
			};
			if (userManager.Users.All(u => u.Id != defaultUser.Id))
			{
				var user = await userManager.FindByEmailAsync(defaultUser.Email);
				if (user == null)
				{
					await userManager.CreateAsync(defaultUser, "LMAlmaht2st@");
					await userManager.AddToRoleAsync(defaultUser, Roles.SuperAdmin.ToString());
				}
				await roleManager.SeedClaimsForSuperAdmin();
			}
		}
		private async static Task SeedClaimsForSuperAdmin(this RoleManager<IdentityRole> roleManager)
		{
			var adminRole = await roleManager.FindByNameAsync("SuperAdmin");
			await roleManager.AddPermissionClaim(adminRole, "Accounts");
		}
		public static async Task AddPermissionClaim(this RoleManager<IdentityRole> roleManager, IdentityRole role, string module)
		{
			var allClaims = await roleManager.GetClaimsAsync(role);
			var allPermissions = Permissions.GeneratePermissionsForModule(module);
			foreach (var permission in allPermissions)
			{
				if (!allClaims.Any(a => a.Type == "Permission" && a.Value == permission))
				{
					await roleManager.AddClaimAsync(role, new Claim("Permission", permission));
				}
			}
		}
	}
}
