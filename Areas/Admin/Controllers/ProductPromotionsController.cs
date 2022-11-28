using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using THUD_TN408.Areas.Admin.Service;
using THUD_TN408.Data;
using THUD_TN408.Models;
using X.PagedList;

namespace THUD_TN408.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize]
    public class ProductPromotionsController : Controller
    {
		private readonly Services _services;

		public ProductPromotionsController(TN408DbContext context, UserManager<User> userManager)
        {
			_services = new Services(context, userManager);
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
                await _services.AddHistory(User, "Khuyến mãi \"" + productPromotion.Id + "\" đã được thêm thành vào hệ thống!", "/Admin/ProductPromotions/Details/" + productPromotion.Id);
                return RedirectToAction("Details", "Products", new {id = productPromotion.ProductId});
            }
			ViewData["ProductId"] = new SelectList(_services.GetListProduct(), "Id", "Name", productPromotion.ProductId);
			return View(productPromotion);
        }

        // GET: Admin/ProductPromotions/Edit/5
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
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Description,ApplyFrom,ValidTo,Stock,DiscountPercent,IsActive,ProductId")] ProductPromotion productPromotion)
        {
            if (id != productPromotion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _services.UpdateProductPromotion(productPromotion);
					await _services.AddHistory(User, "Khuyến mãi \"" + productPromotion.Id + "\" đã được chỉnh sửa thông tin!", "/Admin/ProductPromotions/Details/" + productPromotion.Id);
				}
                catch (DbUpdateConcurrencyException)
                {
                    if (!_services.ProductPromotionExists(productPromotion.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "ProductPromotions", new {id = productPromotion.Id});
            }
            ViewData["ProductId"] = new SelectList(_services.GetListProduct(), "Id", "Name", productPromotion.ProductId);
            return View(productPromotion);
        }

        // GET: Admin/ProductPromotions/Delete/5
        public async Task<IActionResult> Delete(string id)
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

            return View(productPromotion);
        }

        // POST: Admin/ProductPromotions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (!_services.HasAnyPPromotion())
            {
                return Problem("Entity set 'TN408DbContext.PPromotions'  is null.");
            }
            var productPromotion = await _services.GetProductPromotion(id);
			if (productPromotion != null)
            {
                await _services.RemoveProductPromotion(productPromotion);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
