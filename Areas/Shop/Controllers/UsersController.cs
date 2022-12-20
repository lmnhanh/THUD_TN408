using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using THUD_TN408.Areas.Shop.Service;
using THUD_TN408.Data;
using THUD_TN408.Models;

namespace THUD_TN408.Areas.Shop.Controllers
{
	[Area("Shop"), Authorize(Roles = "Customer")]
    public class UsersController : Controller
    {
		private readonly INotyfService _notyf;
		private readonly SignInManager<User> _signInManager;
		private readonly Services _services;

		public UsersController(TN408DbContext context, UserManager<User> userManager, INotyfService notyf,
			SignInManager<User> signInManager)
		{
			_notyf = notyf;
			_services = new Services(context, userManager);
			_signInManager = signInManager;
		}

		public async Task<IActionResult> ChangeInfo()
        {
			User user = await _services.GetUser(User);
			return View(user);
		}

		[HttpPost]
		public async Task<IActionResult> Edit([Bind("Id,Email,FirstName,LastName,Gender,DateOfBirth,Address,PhoneNumber")] User user)
		{
			ViewData["page"] = "Edit";
			if (user.DateOfBirth.HasValue && user.DateOfBirth.Value.CompareTo(DateTime.Now) >= 0)
			{
				ModelState.AddModelError("DateOfBirth", "Ngày sinh không hợp lệ!");
			}
			if (ModelState.IsValid)
			{
				var myUser = _services.GetListUser().FirstOrDefault(x => x.Id == user.Id);
				if (myUser != null)
				{
					myUser.FirstName = user.FirstName;
					myUser.LastName = user.LastName;
					myUser.Address = user.Address;
					myUser.Gender = user.Gender;
					myUser.DateOfBirth = user.DateOfBirth;
					myUser.PhoneNumber = user.PhoneNumber;
					await _services.UpdateUser(myUser);
					_notyf.Success("Đã lưu chỉnh sửa!");
					return RedirectToAction("ChangeInfo", "Users");
				}
			}
			return View("ChangeInfo", user);
		}
	}
}
