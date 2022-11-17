using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using THUD_TN408.Data;

namespace THUD_TN408.Areas.Admin.Controllers
{
	[Area("Admin"), Authorize]
	public class HomePageController : Controller
	{
		readonly TN408DbContext _context;

		public HomePageController(TN408DbContext _context)
		{
			this._context = _context;
		}
		[Authorize]
		public IActionResult Index()
		{
			ViewData["page"] = "dashboard";
			return View();
		}
	}
}
