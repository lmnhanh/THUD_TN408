using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using THUD_TN408.Areas.Admin.Service;
using THUD_TN408.Authorization;
using THUD_TN408.Data;
using THUD_TN408.Models;
using X.PagedList;

namespace THUD_TN408.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Saleman,SuperAdmin,WarehouseManager")]
    public class ProductsController : Controller
    {
		private readonly Services _services;

		public ProductsController(TN408DbContext context, UserManager<User> userManager)
        {
            _services = new Services(context, userManager);
        }

        public async Task<IActionResult> Index()
        {
            var products = await _services.GetListProduct().ToPagedListAsync(1, 8);
			ViewData["page"] = "products";
			return View(products);
        }

        //GET products with paging
        public async Task<IActionResult> Paging(int? categoryId, int page = 1, int size = 8)
        {
            page = (page < 1) ? 1 : page;
            size = (size < 2) ? 2 : size;

            if(categoryId != null)
            {
                return PartialView("_ListProducts", await _services.GetListProduct((int)categoryId).ToPagedListAsync(page, size));
            }
			return PartialView("_ListProducts", await _services.GetListProduct().ToPagedListAsync(page, size));
        }

        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || !_services.HasAnyProduct())
            {
                return NotFound();
            }

            var product = await _services.GetProduct(id);
            if (product == null)
            {
                return NotFound();
            }

			ViewData["ListDetails"] = await _services.GetListProductDetails(product.Id, DateTime.Now).ToPagedListAsync(1, 8);
            ViewData["ListPromotions"] = await _services.GetListPPromotion(product.Id).Result.ToPagedListAsync(1, 8);
			ViewData["page"] = "products";
			return View(product);
        }

		// GET: Admin/Products/Create
		[Authorize(policy: Permissions.Products.Create)]
		public IActionResult Create(int? categoryId)
        {
            if(categoryId != null)
            {
				ViewData["CategoryId"] = new SelectList(_services.GetListCategory(), "Id", "Name", categoryId);
            }
            else
            {
                ViewData["CategoryId"] = new SelectList(_services.GetListCategory(), "Id", "Name");
            }
			ViewData["ProducerId"] = new SelectList(_services.GetListProducers(), "Id", "Name");
			ViewData["page"] = "products_create";
			return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(policy: Permissions.Products.Create)]
		public async Task<IActionResult> Create([Bind("Id,Name,Origin,Description,IsActive,CategoryId,ProducerId")] Product product)
        {
            if (ModelState.IsValid)
            {
                await _services.AddProduct(product);
                await _services.AddHistory(User, "Sản phẩm \"" + product.Id + "\" đã được thêm vào cơ sở dữ liệu", "/Admin/Products/Details/" + product.Id);
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_services.GetListCategory(), "Id", "Name", product.CategoryId);
			ViewData["ProducerId"] = new SelectList(_services.GetListProducers(), "Id", "Name");
			ViewData["page"] = "products";
			return View(product);
        }

		// GET: Admin/Products/Edit/5
		[Authorize(policy: Permissions.Products.Edit)]
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || !_services.HasAnyProduct())
            {
                return NotFound();
            }

            var product = await _services.GetProduct(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_services.GetListCategory(), "Id", "Name", product.CategoryId);
			ViewData["page"] = "products";
			return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(policy: Permissions.Products.Edit)]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Origin,Description,IsActive,CategoryId,ProducerId")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _services.UpdateProduct(product);
					await _services.AddHistory(User, "Sản phẩm \"" + product.Id + "\" đã được chỉnh sửa thông tin", "/Admin/Products/Details/" + product.Id);
				}
                catch (DbUpdateConcurrencyException)
                {
                    if (!_services.ProductExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Products", new {id = product.Id});
            }
            ViewData["CategoryId"] = new SelectList(_services.GetListCategory(), "Id", "Name", product.CategoryId);
			ViewData["ProducerId"] = new SelectList(_services.GetListProducers(), "Id", "Name");

			ViewData["page"] = "products";
			return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost]
		[Authorize(policy: Permissions.Products.Delete)]
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || !_services.HasAnyProduct())
            {
                return NotFound();
            }

            var product = await _services.GetProduct(id);
            if (product == null)
            {
                return NotFound();
            }

			if (product.Details!.Any() == true || product.Promotions?.Any() == true)
			{
				product.IsActive = false;
                await _services.UpdateProduct(product);
				return PartialView("_Product", product);
			}
			
			await _services.AddHistory(User, "Sản phẩm \"" + product.Id + "\" đã được xóa khỏi cơ sở dữ liệu", null);
            await _services.RemoveProduct(product);
			return PartialView("_Product", null);
		}

        [HttpPost]
		[Authorize(policy: Permissions.Products.Delete)]
		public async Task<IActionResult> Recovery(int? id)
		{
			if (id == null || !_services.HasAnyProduct())
			{
				return NotFound();
			}

            var product = await _services.GetProduct(id);
			if (product == null)
			{
				return NotFound();
			}

			product.IsActive = true;
            await _services.UpdateProduct(product);
			return PartialView("_Product",product);
		}
    }
}
