using Microsoft.AspNetCore.Mvc;

namespace THUD_TN408.Areas.Shop.Controllers
{
	[Area("Shop")]
	public class HomePageController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
