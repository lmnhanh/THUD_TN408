using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using NuGet.Protocol;
using System.Text.Encodings.Web;
using System.Text;
using THUD_TN408.Areas.Admin.Models;
using THUD_TN408.Authorization;
using THUD_TN408.Data;
using THUD_TN408.Models;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace THUD_TN408.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Saleman,SuperAdmin,WarehouseManager")]
    public class UsersController : Controller
    {
		private readonly TN408DbContext _context;
		private readonly INotyfService _notyf;
        private readonly UserManager<User> _userManager;
		private readonly SignInManager<User> _signInManager;
		private readonly IEmailSender _emailSender;
		private readonly IUserStore<User> _userStore;
		private readonly IUserEmailStore<User> _emailStore;
		public UsersController(TN408DbContext context, UserManager<User> userManager, INotyfService notyf,
			SignInManager<User> signInManager, IUserStore<User> userStore, IEmailSender emailSender)
        {
            _context = context;
            _notyf = notyf;
            _userManager = userManager;
			_signInManager = signInManager;
			_emailSender = emailSender;
			_userStore = userStore;
			_emailStore = GetEmailStore();
		}

        public async Task<IActionResult> Info(string? id)
        {
			ViewData["page"] = "Info";
			if (id != null)
			{
				return View(await _userManager.FindByIdAsync(id));
			}
            User user = await _userManager.GetUserAsync(User);
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
                var myUser = _context.Users.FirstOrDefault(x => x.Id == user.Id);
                if(myUser != null)
                {
					myUser.FirstName = user.FirstName;
                    myUser.LastName = user.LastName;
                    myUser.Address = user.Address;
                    myUser.Gender = user.Gender;
                    myUser.DateOfBirth = user.DateOfBirth;
                    myUser.PhoneNumber = user.PhoneNumber;
					_context.Users.Update(myUser);
					await _context.SaveChangesAsync();
					_notyf.Success("Đã lưu chỉnh sửa!");
					return RedirectToAction("Info", "Users");
				}
            }
            return View("Info", user);
        }

		[HttpGet]
		public IActionResult Create()
		{
			ViewData["page"] = "accounts";
			return View(new Create());
		}

		[HttpPost]
		public async Task<IActionResult>Create(Create model)
		{
			if (ModelState.IsValid)
			{
				var user =  Activator.CreateInstance<User>();
				user.FirstName = model.FirstName;
				user.LastName = model.LastName;
				user.EmailConfirmed = true;

				await _userStore.SetUserNameAsync(user, model.Email, CancellationToken.None);
				await _emailStore.SetEmailAsync(user, model.Email, CancellationToken.None);
				var result = await _userManager.CreateAsync(user, model.Password);

				if (result.Succeeded)
				{
					//await _userManager.AddToRoleAsync(user, Roles.Customer.ToString());
					_notyf.Success("Thêm tài khoản thành công!");
					return RedirectToAction("Details", "ManageAccounts", new {userId = user.Id});
				}
			}
			_notyf.Error("Không thể thêm tài khoản!");
			ViewData["page"] = "accounts";
			return View(model);
		}

        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}
			ViewData["page"] = "accounts";
			return View();
        }

		[HttpPost]
		public async Task<IActionResult> SavePassword([Bind("OldPassword,NewPassword,ConfirmPassword")] ChangePassword model)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.GetUserAsync(User);
				if (user == null)
				{
					return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
				}

				var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
				if (!changePasswordResult.Succeeded)
				{
					_notyf.Error("Thay đổi mật khẩu không thành công!");
					//foreach (var error in changePasswordResult.Errors)
					//{
					//	ModelState.AddModelError(string.Empty, error.Description);
					//}
					return View("ChangePassword", model);
				}

				await _signInManager.RefreshSignInAsync(user);
				_notyf.Success("Thay đổi mật khẩu thành công!");

				return RedirectToAction("Info", "Users");
			}
			ViewData["page"] = "accounts";
			return View("ChangePassword", model);
		}
		private IUserEmailStore<User> GetEmailStore()
		{
			if (!_userManager.SupportsUserEmail)
			{
				throw new NotSupportedException("The default UI requires a user store with email support.");
			}
			return (IUserEmailStore<User>)_userStore;
		}
	}
}
