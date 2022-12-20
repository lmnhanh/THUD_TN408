using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using THUD_TN408.Areas.Admin.Models;
using THUD_TN408.Authorization;
using THUD_TN408.Models;

namespace THUD_TN408.Areas.Admin.Controllers
{
	[Area("Admin"), Authorize(Roles = "SuperAdmin")]
	public class ManageAccountsController : Controller
	{
		private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly INotyfService _notyf;

		public ManageAccountsController(UserManager<User> userManager, INotyfService notyf, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_roleManager = roleManager;
			_notyf = notyf;
		}
		public async Task<IActionResult> Index()
		{
			var currentUser = await _userManager.GetUserAsync(HttpContext.User);
			var allUsersExceptCurrentUser = await _userManager.Users.Where(a => a.Id != currentUser.Id).ToListAsync();

			List<User> allUsersExceptCustomer = new List<User>();
			foreach(var user in allUsersExceptCurrentUser)
			{
				if(!_userManager.IsInRoleAsync(user, Roles.Customer.ToString()).Result)
				{
					allUsersExceptCustomer.Add(user);
				}
			}
			ViewData["page"] = "accounts";
			return View(allUsersExceptCustomer);
		}

		public async Task<IActionResult> Details(string userId)
		{
			var viewModel = new List<Role>();
			var user = await _userManager.FindByIdAsync(userId);
			foreach (var role in _roleManager.Roles.Where(r => r.Name != "Customer").ToList())
			{
				var userRolesViewModel = new Role() {Name = role.Name};
				if (await _userManager.IsInRoleAsync(user, role.Name))
				{
					userRolesViewModel.Selected = true;
				}
				else
				{
					userRolesViewModel.Selected = false;
				}
				viewModel.Add(userRolesViewModel);
			}
			var model = new UserRole() {UserId = userId, Roles = viewModel};
			ViewData["page"] = "accounts";
			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Update(string id, UserRole model)
		{
			var user = await _userManager.FindByIdAsync(id);
			var roles = await _userManager.GetRolesAsync(user);
			var result = await _userManager.RemoveFromRolesAsync(user, roles);

			result = await _userManager.AddToRolesAsync(user, model.Roles.Where(x => x.Selected).Select(y => y.Name));
			_notyf.Success("Cập nhật role thành công!");
			var currentUser = await _userManager.GetUserAsync(User);
			await _signInManager.RefreshSignInAsync(currentUser);
			await DefaultUsers.SeedSuperAdminAsync(_userManager, _roleManager);
			ViewData["page"] = "accounts";
			return RedirectToAction("Details", new { userId = id });
		}
	}
}
