using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using THUD_TN408.Authorization;
using THUD_TN408.Data;
using THUD_TN408.Models;

namespace THUD_TN408.Areas.Admin.Controllers
{
	[Area("Admin"), Authorize] //(Roles = "SuperAdmin,Saleman,WarehouseManager")
	public class HomePageController : Controller
	{
		readonly UserManager<User> userManager;
		readonly RoleManager<IdentityRole> roleManager;
		readonly TN408DbContext _context;

		public HomePageController(TN408DbContext _context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
		{
			this._context = _context;
			this.userManager = userManager;
			this.roleManager = roleManager;
		}

		public IActionResult Index()
		{
			ViewData["page"] = "dashboard";
			return View();
		}

		//public async Task<IActionResult> Index()
		//{
		//	await DefaultRoles.SeedAsync(userManager, roleManager);
		//	await DefaultUsers.SeedBasicUserAsync(userManager, roleManager);
		//	await DefaultUsers.SeedSuperAdminAsync(userManager, roleManager);
		//	ViewData["page"] = "dashboard";
		//	return View();
		//}
	}
}
