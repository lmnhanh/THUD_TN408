using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using THUD_TN408.Areas.Shop.Service;
using THUD_TN408.Data;
using THUD_TN408.Models;

namespace THUD_TN408.Areas.Shop.Controllers
{
	[Area("Shop")]
	public class HomePageController : Controller
	{
		private readonly Services _services;
		private readonly INotyfService _notyf;

		public HomePageController(TN408DbContext context, UserManager<User> userManager, INotyfService notyf)
		{
			_services = new Services(context, userManager);
			_notyf = notyf;
		}
		public async Task<IActionResult> Index()
		{
			return View( (await _services.GetListProductDetailsForShop(DateTime.Now)).Take(8));
		}
	}
}
