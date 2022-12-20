using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using THUD_TN408.Areas.Shop.Service;
using THUD_TN408.Data;
using THUD_TN408.Models;

namespace THUD_TN408.Areas.Shop.Controllers
{
	[Area("Shop")]
    public class ProductsController : Controller
    {
		private readonly Services _services;
		private readonly INotyfService _notyf;
		public ProductsController(TN408DbContext context, UserManager<User> userManager, INotyfService notyf)
		{
			_services = new Services(context, userManager);
			_notyf = notyf;
		}

		[HttpGet, ActionName("All")]
		public async Task<IActionResult> AllProducts(int? category)
		{
			if(category != null)
			{
				var model = _services.GetListProductDetailsForShop(DateTime.Now).Result
					.Where(x => x.Product?.CategoryId == category);
				return PartialView("_ListProducts", model);
			}
			ViewData["categories"] = _services.GetListCategories();
			var products = await _services.GetListProductDetailsForShop(DateTime.Now);
			return View("AllProducts",products);
		}

		[HttpGet]
        public async Task<IActionResult> Details(int id)
        {
			var detail = await _services.GetProductDetail(id, DateTime.Now);
			if(detail == null)
			{
				return NotFound();
			}
			ViewData["ListOfGroup"] = await _services.GetListProducDetailsInGroupSameColor(detail.Id, DateTime.Now);
            return View(detail);
        }

		[HttpGet]
		public async Task<IActionResult> GetDetail(int detailId)
		{
			var detail = await _services.GetProductDetail(detailId, DateTime.Now);
			if (detail == null)
			{
				return NotFound();
			}
			ViewData["ListOfGroup"] = await _services.GetListProducDetailsInGroupSameColor(detail.Id, DateTime.Now);
			return View("_ProductDetail",detail);
		}
	}
}
