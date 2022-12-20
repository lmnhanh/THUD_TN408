using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using THUD_TN408.Areas.Admin.Service;
using THUD_TN408.Authorization;
using THUD_TN408.Data;
using THUD_TN408.Models;
using X.PagedList;

namespace THUD_TN408.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Saleman,SuperAdmin")]
    public class OrderPromotionsController : Controller
    {
		private readonly INotyfService _notyf;
		private readonly Services _services;

		public OrderPromotionsController(TN408DbContext context, UserManager<User> userManager, INotyfService notyf)
        {
			_services = new Services(context, userManager);
			_notyf = notyf;
		}

        // GET: Admin/OrderPromotions
        public async Task<IActionResult> Index()
        {
			ViewData["page"] = "opromotions";
			var promotions = await _services.GetListOPromotion().ToPagedListAsync(1, 8);
			return View(promotions);
        }

        public async Task<IActionResult> Paging(int page = 1, int size = 8)
        {
			page = (page < 1) ? 1 : page;
			size = (size < 2) ? 2 : size;
			var promotions = await _services.GetListOPromotion().ToPagedListAsync(page, size);
			return PartialView("_ListOrderPromotions", promotions);
		}

        // GET: Admin/OrderPromotions/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || !_services.HasAnyOrderPromotion())
            {
                return NotFound();
            }
            var orderPromotion = await _services.GetOrderPromotion(id);
            if (orderPromotion == null)
            {
                return NotFound();
            }

			ViewData["page"] = "opromotions";
			return View(orderPromotion);
        }

		// GET: Admin/OrderPromotions/Create
		[Authorize(policy: Permissions.Promotions.Create)]
		public IActionResult Create()
        {
			ViewData["page"] = "opromotions";
			return View(new OrderPromotion());
        }

        // POST: Admin/OrderPromotions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(policy: Permissions.Promotions.Create)]
		public async Task<IActionResult> Create([Bind("Id,Name,Description,ApplyFrom,ValidTo,Stock,MaxDiscount,DiscountPercent,ApplyCondition,IsActive")] OrderPromotion orderPromotion)
        {
            if (_services.ProductPromotionExists(orderPromotion.Id))
			{
				ModelState.AddModelError("Id", "Mã khuyến mãi đã được sử dụng!");
			}
			if (orderPromotion.ApplyFrom.CompareTo(orderPromotion.ValidTo) > 0)
			{
				ModelState.AddModelError("ValidTo", "Ngày hết hạn phải sau ngày bắt đầu áp dụng khuyến mãi!");
			}
			if (orderPromotion.ValidTo.CompareTo(DateTime.Now) < 0)
			{
				ModelState.AddModelError("ValidTo", "Ngày hết hạn phải sau ngày hiện tại!");
			}
            if (ModelState.IsValid)
            {
				await _services.AddOrderPromotion(orderPromotion);
				await _services.AddHistory(User, "Thêm khuyến mãi \"" + orderPromotion.Id + "\"", "/Admin/OrderPromotions/Details/" + orderPromotion.Id);
				_notyf.Success("Đã thêm khuyến mãi " + orderPromotion.Id);
				return RedirectToAction("Details", "OrderPromotions", new { id = orderPromotion.Id });
			}
			ViewData["page"] = "opromotions";
			return View(orderPromotion);
        }

		// GET: Admin/OrderPromotions/Edit/5
		[Authorize(policy: Permissions.Promotions.Edit)]
		public async Task<IActionResult> Edit(string id)
        {
            if (id == null || !_services.HasAnyOrderPromotion())
            {
                return NotFound();
            }

            var orderPromotion = await _services.GetOrderPromotion(id);
            if (orderPromotion == null)
            {
                return NotFound();
            }
			ViewData["page"] = "opromotions";
			return View(orderPromotion);
        }

        // POST: Admin/OrderPromotions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(policy: Permissions.Promotions.Edit)]
		public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Description,ApplyFrom,ValidTo,Stock,MaxDiscount,DiscountPercent,ApplyCondition,IsActive")] OrderPromotion orderPromotion)
        {
            if (id != orderPromotion.Id)
            {
                return NotFound();
            }

            var oldPromotion = _services.GetListOPromotion().Where(x => x.Id.Equals(orderPromotion.Id)).AsNoTracking().FirstOrDefault();
            if(oldPromotion != null && oldPromotion.ValidTo.CompareTo(DateTime.Now) < 0)
            {
				_notyf.Warning("Không thể chỉnh sửa khuyến mãi đã kết thúc!");
				return RedirectToAction("Details", "OrderPromotions", new { id = orderPromotion.Id });
			}

			if (ModelState.IsValid)
            {
                try
                {
					await _services.UpdateOrderPromotion(orderPromotion);
					_notyf.Success("Đã lưu chỉnh sửa!");
					await _services.AddHistory(User, "Chỉnh sửa khuyến mãi \"" + orderPromotion.Id + "\"", "/Admin/OrderPromotions/Details/" + orderPromotion.Id);

				}
                catch (DbUpdateConcurrencyException)
                {
                    if (!_services.OrderPromotionExists(orderPromotion.Id))
                    {
                        return NotFound();
                    }
                }
				return RedirectToAction("Details", "OrderPromotions", new { id = orderPromotion.Id });
			}
            return View(orderPromotion);
        }

		// POST: Admin/OrderPromotions/DeleteAsync/5
		[HttpPost, ActionName("DeleteAsync")]
		[Authorize(policy: Permissions.Promotions.Delete)]
		public async Task<IActionResult> DeleteAsync(string id)
        {
			if (!_services.HasAnyOrderPromotion())
            {
				_notyf.Error("Đã có lỗi xảy ra!");
			}
            var orderPromotion = await _services.GetOrderPromotion(id);
			try
            {
                if (orderPromotion != null)
                {
                    _notyf.Success("Đã xóa khuyến mãi " + orderPromotion.Id);
					await _services.AddHistory(User, "Xóa khuyến mãi \"" + orderPromotion.Id + "\"", null);
					await _services.RemoveOrderPromotion(orderPromotion);
                    return PartialView("_OrderPromotion", null);
                }
            }
            catch
            {
                _notyf.Error("Đã có lỗi xảy ra!");
			    return PartialView("_OrderPromotion", orderPromotion);
            }
			return PartialView("_OrderPromotion", orderPromotion);
		}

        
    }
}
