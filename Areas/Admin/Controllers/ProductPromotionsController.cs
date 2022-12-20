using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
    public class ProductPromotionsController : Controller
    {
		private readonly Services _services;
		private readonly INotyfService _notyf;

		public ProductPromotionsController(TN408DbContext context, UserManager<User> userManager, INotyfService notyf)
        {
			_services = new Services(context, userManager);
			_notyf = notyf;
		}

        // GET: Admin/ProductPromotions
        public async Task<IActionResult> Index(int? productId)
        {
			ViewData["page"] = "ppromotions";
            if(productId != null)
            {
				return View(await _services.GetListPPromotion((int)productId).Result.ToPagedListAsync(1, 8));
			}
			return View(await _services.GetListPPromotion().ToPagedListAsync(1, 8));
		}

		[HttpGet]
		public async Task<IActionResult> Paging(int? productId,int page = 1, int size = 8)
		{
			page = (page < 1) ? 1 : page;
			size = (size < 2) ? 2 : size;

			IPagedList<ProductPromotion> promotions;
			if (productId != null)
            {			
				promotions = await _services.GetListPPromotion((int)productId).Result.ToPagedListAsync(page, size);
				return PartialView("_ListProductPromotions", promotions);
			}

			promotions = await _services.GetListPPromotion().ToPagedListAsync(page, size);
			return PartialView("_ListProductPromotions", promotions);
		}

		// GET: Admin/ProductPromotions/Details/5
		public async Task<IActionResult> Details(string id)
        {
            if (id == null || !_services.HasAnyPPromotion())
            {
                return NotFound();
            }

            var productPromotion = await _services.GetProductPromotion(id);
            if (productPromotion == null)
            {
                return NotFound();
            }
			ViewData["page"] = "ppromotions";
			return View(productPromotion);
        }

		// GET: Admin/ProductPromotions/Create
		[Authorize(policy: Permissions.Promotions.Create)]
		public IActionResult Create(int? productId = null)
        {
            if(productId != null)
            {
				ViewData["ProductId"] = new SelectList(_services.GetListProduct(), "Id", "Name", productId);
			}
            else
            {
				ViewData["ProductId"] = new SelectList(_services.GetListProduct(), "Id", "Name");
			}
            ViewData["page"] = "ppromotions";
			return View(new ProductPromotion());
        }

        // POST: Admin/ProductPromotions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(policy: Permissions.Promotions.Create)]
		public async Task<IActionResult> Create([Bind("Id,Name,Description,ApplyFrom,ValidTo,Stock,DiscountPercent,IsActive,ProductId")] ProductPromotion productPromotion)
        {
			if (_services.ProductPromotionExists(productPromotion.Id))
			{
				ModelState.AddModelError("Id", "Mã khuyến mãi đã được sử dụng!");
			}
			if (productPromotion.ApplyFrom.CompareTo(productPromotion.ValidTo) > 0)
			{
				ModelState.AddModelError("ValidTo", "Ngày hết hạn phải sau ngày bắt đầu áp dụng khuyến mãi!");
			}
			if (productPromotion.ValidTo.CompareTo(DateTime.Now) < 0)
			{
				ModelState.AddModelError("ValidTo", "Ngày hết hạn phải sau ngày hiện tại!");
			}
			if (ModelState.IsValid)
            {
				await _services.AddProductPromotion(productPromotion);
                await _services.AddHistory(User, "Thêm khuyến mãi \"" + productPromotion.Id + "\"", "/Admin/ProductPromotions/Details/" + productPromotion.Id);
				_notyf.Success("Đã thêm khuyến mãi " + productPromotion.Id);
				return RedirectToAction("Details", "Products", new {id = productPromotion.ProductId});
            }
			ViewData["ProductId"] = new SelectList(_services.GetListProduct(), "Id", "Name", productPromotion.ProductId);
			return View(productPromotion);
        }

		// GET: Admin/ProductPromotions/Edit/5
		[Authorize(policy: Permissions.Promotions.Edit)]
		public async Task<IActionResult> Edit(string id)
        {
            if (id == null || !_services.HasAnyPPromotion())
            {
                return NotFound();
            }

            var productPromotion = await _services.GetProductPromotion(id);
            if (productPromotion == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_services.GetListProduct(), "Id", "Name", productPromotion.ProductId);
            return View(productPromotion);
        }

        // POST: Admin/ProductPromotions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(policy: Permissions.Promotions.Edit)]
		public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Description,ApplyFrom,ValidTo,Stock,DiscountPercent,IsActive,ProductId")] ProductPromotion productPromotion)
        {
            if (id != productPromotion.Id)
            {
                return NotFound();
            }

			var oldPromotion = _services.GetListPPromotion().Where(x => x.Id.Equals(productPromotion.Id)).AsNoTracking().FirstOrDefault();
			if (oldPromotion != null && oldPromotion.ValidTo.CompareTo(DateTime.Now) < 0)
			{
				_notyf.Warning("Không thể chỉnh sửa khuyến mãi đã kết thúc!");
				return RedirectToAction("Details", "ProductPromotions", new { id = productPromotion.Id });
			}

			if (ModelState.IsValid)
            {
                try
                {
                    await _services.UpdateProductPromotion(productPromotion);
					_notyf.Success("Đã lưu chỉnh sửa!");
					await _services.AddHistory(User, "Chỉnh sửa khuyến mãi \"" + productPromotion.Id + "\"", "/Admin/ProductPromotions/Details/" + productPromotion.Id);
				}
                catch (DbUpdateConcurrencyException)
                {
                    if (!_services.ProductPromotionExists(productPromotion.Id))
                    {
                        return NotFound();
                    }
                }
                return RedirectToAction("Details", "ProductPromotions", new {id = productPromotion.Id});
            }
            ViewData["ProductId"] = new SelectList(_services.GetListProduct(), "Id", "Name", productPromotion.ProductId);
            return View(productPromotion);
        }

		[HttpPost, ActionName("DeleteAsync")]
		[Authorize(policy: Permissions.Promotions.Delete)]
		public async Task<IActionResult> DeleteAsync(string id)
		{
			if (!_services.HasAnyPPromotion())
			{
                _notyf.Error("Đã có lỗi xảy ra!");
			}
			var productPromotion = await _services.GetProductPromotion(id);
			if (productPromotion != null)
			{
                try
                {
                    _notyf.Success("Đã xóa khuyến mãi " + productPromotion.Id);
					await _services.AddHistory(User, "Xóa khuyến mãi \"" + productPromotion.Id + "\"", null);
					await _services.RemoveProductPromotion(productPromotion);
					return PartialView("_ProductPromotion", null);
				}
                catch
                {
					_notyf.Error("Đã có lỗi xảy ra!");
					return PartialView("_ProductPromotion", productPromotion);
				}
			}
			return PartialView("_ProductPromotion", productPromotion);
		}
	}
}
