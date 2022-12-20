using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using THUD_TN408.Areas.Shop.Models;
using THUD_TN408.Authorization;
using THUD_TN408.Models;

namespace THUD_TN408.Areas.Shop.Controllers
{
	[Area("Shop")]
	public class LoginController : Controller
	{
		private readonly SignInManager<User> _signInManager;
		private readonly UserManager<User> _userManager;
		public LoginController(SignInManager<User> signInManager, UserManager<User> userManager)
		{
			_signInManager = signInManager;
			_userManager = userManager;
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpPost, ActionName("Login")]
		public async Task<IActionResult> Login([Bind("Email, Password, RememberMe")]Login loginModel)
		{
			if (ModelState.IsValid)
			{
				var result = await _signInManager.PasswordSignInAsync(loginModel.Email, loginModel.Password, loginModel.RememberMe, lockoutOnFailure: false);
				if (result.Succeeded)
				{
					var user = await _userManager.FindByEmailAsync(loginModel.Email);
					if (await _userManager.IsInRoleAsync(user, Roles.Customer.ToString())){
						return RedirectToAction("Index", "HomePage");
					}
					return LocalRedirect("/Admin/HomePage/Index");
				}
				if (result.IsLockedOut)
				{
					ModelState.AddModelError(string.Empty, "Tài khoản đã bị khóa");
					return View("Index", loginModel);
				}
				
			}
			ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không chính xác");
			return View("Index", loginModel);
		}

		[HttpGet]
		public async Task<IActionResult> Logout()
		{
			await _signInManager.SignOutAsync();
			return RedirectToAction("Index", "HomePage");
		}
	}
}
