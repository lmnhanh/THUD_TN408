using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Xaml.Permissions;
using THUD_TN408.Areas.Shop.Models;
using THUD_TN408.Areas.Shop.Service;
using THUD_TN408.Data;
using THUD_TN408.Models;
using static THUD_TN408.Authorization.Permissions;

namespace THUD_TN408.Areas.Shop.Controllers
{
	[Area("Shop"), Authorize(Roles = "Customer")]
    public class CartsController : Controller
    {
		private readonly Services _services;
		private readonly INotyfService _notyf;

		public CartsController(TN408DbContext context, UserManager<User> userManager, INotyfService notyf)
		{
			_services = new Services(context, userManager);
			_notyf = notyf;
		}

		[HttpGet]
        public async Task<IActionResult> MyCart()
        {
			var userId = await _services.GetUserId(User);
			if (userId == null) return NotFound();
			var carts = await _services.GetListCart(userId);
			long totalAll = 0L;
			foreach(var cart in carts)
			{
				long discount = 0L;
				var currentPPromotion = cart.Detail?.Product?.Promotions?.Where(x => x.ValidTo.CompareTo(DateTime.Now) >= 0 && x.ApplyFrom.CompareTo(DateTime.Now) <= 0).OrderByDescending(x => x.DiscountPercent).FirstOrDefault();
				if (currentPPromotion != null)
				{
					discount = cart.Detail!.Amount * currentPPromotion.DiscountPercent / 100;
					cart.Total = (cart.Detail!.Amount - discount) * cart.Quantity;
				}
				else{
					cart.Total = cart.Detail!.Amount * cart.Quantity;
				}
				totalAll += (long)cart.Total;
			}
            return View(new CartsModel() { Carts = carts, Total= totalAll, TotalFinal = totalAll});
        }

		[HttpPost]
		public async Task<IActionResult> GetCart(int cartId, int quantity)
		{
			var currentUserId = await _services.GetUserId(User);
			if (currentUserId == null) return NotFound();
			var cart = await _services.GetCart(currentUserId, cartId);
			if(cart == null) return NotFound();

			if(quantity == 0)
			{
				await _services.RemoveCart(cart);
				return PartialView("_Cart", null);
			}

			if(cart.Detail?.Stock >= quantity)
			{
				cart.Quantity = quantity;
				await _services.UpdateCart(cart);
			}
			else
			{
				_notyf.Warning("Đã đạt số lượng tối đa của sản phẩm");
			}

			long discount = 0L;
			var currentPPromotion = cart.Detail?.Product?.Promotions?.Where(x => x.ValidTo.CompareTo(DateTime.Now) >= 0 && x.ApplyFrom.CompareTo(DateTime.Now) <= 0).OrderByDescending(x => x.DiscountPercent).FirstOrDefault();
			if (currentPPromotion != null)
			{
				discount = cart.Detail!.Amount * currentPPromotion.DiscountPercent / 100;
				cart.Total = (cart.Detail!.Amount - discount) * cart.Quantity;
			}
			else
			{
				cart.Total = cart.Detail!.Amount * cart.Quantity;
			}
			
			return PartialView("_Cart", cart);
		}

		[HttpPost]
		public async Task<IActionResult> RemoveCart(int cartId)
		{
			var currentUserId = await _services.GetUserId(User);
			if (currentUserId == null) return NotFound();
			var cart = await _services.GetCart(currentUserId, cartId);
			if (cart == null) return NotFound();
			await _services.RemoveCart(cart);
			return PartialView("_Cart", null);
		}

		[HttpPost]
		public async Task AddCart(int detailId, int quantity)
		{
			var currentUserId = await _services.GetUserId(User);
			if (currentUserId == null)
			{
				return;
			}

			var cart = await _services.GetCartOfDetail(currentUserId, detailId);
			if (cart == null)
			{
				var detail = await _services.GetProductDetail(detailId, DateTime.Now);
				if(detail != null && detail.Stock >= quantity)
				{
					await _services.AddCart(new Cart()
					{
						ProductDetailId = detail.Id,
						Quantity = quantity,
						UserId = currentUserId
					});
					_notyf.Success("Đã thêm sản phẩm vào giỏ hàng!");
				}
				else
				{
					_notyf.Error("Số lượng yêu cầu lớn hơn số lượng sản phẩm!");
				}
			}
			else
			{
				int newQuantity = cart.Quantity + quantity;
				if (cart.Detail?.Stock >= newQuantity)
				{

					cart.Quantity = newQuantity;
					await _services.UpdateCart(cart);
					_notyf.Success("Đã thêm "+quantity+" sản phẩm vào giỏ hàng!");
				}
				else
				{
					_notyf.Error("Số lượng yêu cầu lớn hơn số lượng sản phẩm !" + cart.Quantity + "." + cart.Detail.Stock);
				}
			}
		}
	}
}
