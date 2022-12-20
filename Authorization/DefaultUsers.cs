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
				UserName = "saleman@gmail.com",
				FirstName = "Le Minh",
				LastName = "Anh",
				Email = "saleman@gmail.com",
				EmailConfirmed = true
			};
			if (userManager.Users.All(u => u.Id != defaultUser.Id))
			{
				var user = await userManager.FindByEmailAsync(defaultUser.Email);
				if (user == null)
				{
					await userManager.CreateAsync(defaultUser, "LMAlmaht2st@");
					await userManager.AddToRoleAsync(defaultUser, Roles.Saleman.ToString());

					await roleManager.AddPermissionAsCustomer();
					await roleManager.AddPermissionAsWarehouseManager();
					await roleManager.AddPermissionAsSaleman();
				}
			}
		}
		public static async Task SeedWarehouseUserAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
		{
			var defaultUser = new User
			{
				UserName = "warehouse@gmail.com",
				FirstName = "Le Minh",
				LastName = "Anh",
				Email = "warehouse@gmail.com",
				EmailConfirmed = true
			};
			if (userManager.Users.All(u => u.Id != defaultUser.Id))
			{
				var user = await userManager.FindByEmailAsync(defaultUser.Email);
				if (user == null)
				{
					await userManager.CreateAsync(defaultUser, "LMAlmaht2st@");
					await userManager.AddToRoleAsync(defaultUser, Roles.WarehouseManager.ToString());
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
			await roleManager.AddClaimAsync(warehouseRole, new Claim("Permission", Permissions.Producers.View));
			await roleManager.AddClaimAsync(warehouseRole, new Claim("Permission", Permissions.Products.Create));
			await roleManager.AddClaimAsync(warehouseRole, new Claim("Permission", Permissions.Categories.View));
			await roleManager.AddClaimAsync(warehouseRole, new Claim("Permission", Permissions.Categories.Create));
			await roleManager.AddClaimAsync(warehouseRole, new Claim("Permission", Permissions.Warehouses.View));
			await roleManager.AddClaimAsync(warehouseRole, new Claim("Permission", Permissions.ImportNotes.View));
			await roleManager.AddClaimAsync(warehouseRole, new Claim("Permission", Permissions.ImportNotes.Create));
			await roleManager.AddClaimAsync(warehouseRole, new Claim("Permission", Permissions.ImportNotes.Delete));
			await roleManager.AddClaimAsync(warehouseRole, new Claim("Permission", Permissions.ImportNotes.Edit));
		}

		public static async Task AddPermissionAsSaleman(this RoleManager<IdentityRole> roleManager)
		{
			var managerRole = await roleManager.FindByNameAsync("Saleman");
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.Products.View));
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.Shipments.View));
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.Producers.View));
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.Orders.View));
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.Warehouses.View));
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.Payments.View));
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.Promotions.View));
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.CustomerAccounts.View));
			await roleManager.AddClaimAsync(managerRole, new Claim("Permission", Permissions.Categories.View));
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
			await roleManager.AddPermissionClaim(adminRole, "StaffAccounts");
			await roleManager.AddPermissionClaim(adminRole, "Products");
			await roleManager.AddPermissionClaim(adminRole, "Producers");
			await roleManager.AddPermissionClaim(adminRole, "Categories");
			await roleManager.AddPermissionClaim(adminRole, "Promotions");
			await roleManager.AddPermissionClaim(adminRole, "Orders");
			await roleManager.AddPermissionClaim(adminRole, "Warehouses");
			await roleManager.AddPermissionClaim(adminRole, "Payments");
			await roleManager.AddPermissionClaim(adminRole, "Shipments");
			await roleManager.AddPermissionClaim(adminRole, "ImportNotes");
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
