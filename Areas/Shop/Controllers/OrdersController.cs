using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using THUD_TN408.Areas.Shop.Models;
using THUD_TN408.Areas.Shop.Service;
using THUD_TN408.Data;
using THUD_TN408.Models;
using static THUD_TN408.Authorization.Permissions;

namespace THUD_TN408.Areas.Shop.Controllers
{
	[Area("Shop"), Authorize(Roles = "Customer")]
	public class OrdersController : Controller
	{
		private readonly Services _services;
		private readonly INotyfService _notyf;
		public OrdersController(TN408DbContext context, UserManager<User> userManager, INotyfService notyf)
		{
			_services = new Services(context, userManager);
			_notyf = notyf;
		}

		public async Task<IActionResult> List()
		{
			var user = await _services.GetUser(User);
			return View(await _services.GetListOrderOf(user.Id));
		}

		public async Task<IActionResult> CheckOut()
		{
			var user = await _services.GetUser(User);
			if (user.PhoneNumber == null || user.Address == null)
			{
				return RedirectToAction("ChangeInfo", "Users");
			}
			ViewData["user"] = user;
			var carts = await _services.GetListCart(user.Id);
			long totalAll = 0L;
			foreach (var cart in carts)
			{
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
				totalAll += (long)cart.Total;
			}
			return View(new CartsModel() { Carts = carts, Total = totalAll, TotalFinal = totalAll });
		}

		[HttpGet]
		public async Task<IActionResult> GetPromotion(string id)
		{
			var user = await _services.GetUser(User);
			var carts = await _services.GetListCart(user.Id);
			long totalAll = 0L;
			foreach (var cart in carts)
			{
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
				totalAll += (long)cart.Total;
			}
			var promotion = await _services.GetPromotion(id);

			if (promotion == null)
			{
				_notyf.Information("Mã khuyến mãi không chính xác");
				return PartialView("_ApplyPromotion", new CartsModel() { Carts = carts, Total = totalAll, TotalFinal = totalAll });
			}
			if(promotion.ValidTo.CompareTo(DateTime.Now) < 0)
			{
				_notyf.Information("Khuyến mãi đã hết hạn");
				return PartialView("_ApplyPromotion", new CartsModel() { Carts = carts, Total = totalAll, TotalFinal = totalAll });
			}
			if (promotion.ApplyFrom.CompareTo(DateTime.Now) > 0 )
			{
				_notyf.Information("Khuyến mãi chưa áp dụng");
				return PartialView("_ApplyPromotion", new CartsModel() { Carts = carts, Total = totalAll, TotalFinal = totalAll });
			}
			if (promotion.Stock < 1)
			{
				_notyf.Information("Khuyến mãi đã hết lượt áp dụng");
				return PartialView("_ApplyPromotion", new CartsModel() { Carts = carts, Total = totalAll, TotalFinal = totalAll });
			}
			if (totalAll < promotion.ApplyCondition)
			{
				_notyf.Information("Không đủ điều kiện áp dụng khuyễn mãi " + id);
				return PartialView("_ApplyPromotion", new CartsModel() { Carts = carts, Total = totalAll, TotalFinal = totalAll });
			}

			long orderDiscount = totalAll * promotion.DiscountPercent / 100;
			orderDiscount = (orderDiscount > promotion.MaxDiscount) ? (long)promotion.MaxDiscount : orderDiscount;

			_notyf.Success("Đã áp dụng khuyễn mãi " + id);

			return PartialView("_ApplyPromotion", new CartsModel() { Carts = carts, Promotion = promotion, Total = totalAll, Discount = orderDiscount, TotalFinal = totalAll });
		}

		[HttpPost]
		public async Task<IActionResult> Confirm(string promotionId, int total, int paymentId = 1)
		{
			if(promotionId != null)
			{
				var promotion = await _services.GetPromotion(promotionId);
				if (promotion != null)
				{
					promotion.Stock--;
					await _services.UpdatePromotion(promotion);
				}
			}

			var user = await _services.GetUser(User);
			var carts = await _services.GetListCart(user.Id);
			foreach (var cart in carts)
			{
				var currentPPromotion = cart.Detail?.Product?.Promotions?
					.Where(x => x.ValidTo.CompareTo(DateTime.Now) >= 0 && x.ApplyFrom.CompareTo(DateTime.Now) <= 0)
					.OrderByDescending(x => x.DiscountPercent).FirstOrDefault();
				if (currentPPromotion != null)
				{
					currentPPromotion.Stock--;
					await _services.UpdatePPromotion(currentPPromotion);
				}
			}

			await _services.AddOrder(carts, user.Id,(long)total, paymentId);


			return View("Success");
		}
	}
}
