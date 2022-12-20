using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using THUD_TN408.Areas.Admin.Service;
using THUD_TN408.Authorization;
using THUD_TN408.Data;
using THUD_TN408.Models;

namespace THUD_TN408.Areas.Admin.Controllers
{
	[Area("Admin"),Authorize(Roles = "SuperAdmin,Saleman,WarehouseManager")] //,Authorize(Roles = "SuperAdmin,Saleman,WarehouseManager")
	public class HomePageController : Controller
	{
		readonly RoleManager<IdentityRole> roleManager;
		readonly UserManager<User> userManager;
		readonly TN408DbContext _context;

		public HomePageController(TN408DbContext _context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
		{
			this.userManager = userManager;
			this.roleManager = roleManager;
			this._context = _context;
		}

		public IActionResult Index()
		{
			var orderToday = _context.Orders.Where(x => x.CreatedDate.Date > DateTime.UtcNow.AddDays(-1));
			ViewData["newOrder"] = orderToday.Count();
			ViewData["orderPending"] = _context.Orders.Where(x => !x.IsTrans).Count();
			ViewData["newCus"] = orderToday.Count();
			ViewData["profit"] = _context.Orders.Where(x => x.CreatedDate.Date > DateTime.UtcNow.AddDays(-30))
				.Where(x => x.IsSuccess && x.IsPaid)
				.Sum(x => x.Total);
			ViewData["page"] = "dashboard";
			return View();
		}

		//public async Task<IActionResult> Index()
		//{
		//	await DefaultRoles.SeedAsync(userManager, roleManager);
		//	await DefaultUsers.SeedBasicUserAsync(userManager, roleManager);
		//	await DefaultUsers.SeedWarehouseUserAsync(userManager, roleManager);
		//	await DefaultUsers.SeedSuperAdminAsync(userManager, roleManager);
		//	ViewData["page"] = "dashboard";
		//	return View();
		//}
	}
}
