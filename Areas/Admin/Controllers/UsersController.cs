using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using THUD_TN408.Data;
using THUD_TN408.Models;

namespace THUD_TN408.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize]
    public class UsersController : Controller
    {
		private readonly TN408DbContext _context;
		private readonly INotyfService _notyf;
        private readonly UserManager<User> _userManager;
		public UsersController(TN408DbContext context, UserManager<User> userManager, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
            _userManager = userManager;
        }

        public async Task<IActionResult> Info()
        {
            User user = await _userManager.GetUserAsync(User);
			ViewData["page"] = "Info";
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
    }
}
