using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using THUD_TN408.Areas.Admin.Service;
using THUD_TN408.Authorization;
using THUD_TN408.Data;
using THUD_TN408.Models;
using X.PagedList;

namespace THUD_TN408.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Saleman,SuperAdmin,WarehouseManager")]
    public class ProductDetailsController : Controller
    {
        private readonly Services _services;
		private readonly INotyfService _notyf;

		public ProductDetailsController(TN408DbContext context, UserManager<User> userManager, INotyfService notyf)
        {
			_services = new Services(context, userManager);
            _notyf = notyf;
		}

        // GET: Admin/ProductDetails
        public async Task<IActionResult> Index()
        {
            var details = _services.GetListProductDetails(DateTime.Now);
			ViewData["page"] = "products";
			return View(await details.ToPagedListAsync(1, 8));
        }

        [HttpGet]
		public async Task<IActionResult> Paging(int? productId, int page = 1, int size = 8)
		{
			page = (page < 1) ? 1 : page;
            size = (size < 2) ? 2 : size;

			ViewData["page"] = "products";
            if (productId != null)
            {
				var d = _services.GetListProductDetails((int)productId,DateTime.Now);
				ViewData["productId"] = productId;
				return PartialView("_ListProductDetails", await d.ToPagedListAsync(page, size));
			}
			var details = _services.GetListProductDetails(DateTime.Now);
			return PartialView("_ListProductDetails", await details.ToPagedListAsync(page, size));
		}

		// GET: Admin/ProductDetails/Details/5
		public async Task<IActionResult> Details(int? id)
        {
            if (id == null || !_services.HasAnyProductDetail())
            {
                return NotFound();
            }

            var productDetail = await _services.GetProductDetail(id, DateTime.Now);
            if (productDetail == null)
            {
                return NotFound();
            }
            ViewData["page"] = "products";
            return View(productDetail);
        }

        // GET: Admin/ProductDetails/Create
        [Authorize(policy: Permissions.Products.Create)]
        public IActionResult Create(int? productId)
        {
            if(productId != null)
            {
                ViewData["ProductId"] = new SelectList(_services.GetListProduct().ToList(), "Id", "Name", productId);
            }
            else
            {
				ViewData["ProductId"] = new SelectList(_services.GetListProduct().ToList(), "Id", "Name");
			}
			ViewData["page"] = "products";
			return View(new ProductDetail());
        }

        // POST: Admin/ProductDetails/Create
        [HttpPost,Authorize(policy: Permissions.Products.Create)]
        public async Task<IActionResult> Create([Bind("Id,Size,Color,Amount,Gender,ProductId")] ProductDetail productDetail, IFormFile? Image1, IFormFile? Image2)
        {
			ViewData["page"] = "products";
			try
            {
                if (ModelState.IsValid)
                {
                    if (Image1 != null && Image1.Length > 0)
                    {

                        productDetail.Image1 = await _services.UploadImage(Image1);
                    }
                    else productDetail.Image1 = null;

				    if (Image2 != null && Image2.Length > 0)
                    {
                        productDetail.Image2 = await _services.UploadImage(Image2);
                    }
				    else productDetail.Image2 = null;

                    await _services.AddProductDetail(productDetail);
                    await _services.AddPrice(productDetail.Id, productDetail.Amount, DateTime.Now);
                    await _services.AddHistory(User, "Thêm chi tiết \"" + productDetail.Id + "\" cho sản phẩm \""+ productDetail.ProductId +"\"", "/Admin/ProductDetails/Details/" + productDetail.Id);
					_notyf.Success("Đã thêm chi tiết sản phẩm " + productDetail.Id);
					return RedirectToAction("Details", "Products", new {id = productDetail.ProductId});
				}
				ViewData["ProductId"] = new SelectList(_services.GetListProduct().ToList(), "Id", "Name", productDetail.ProductId);
				return View(productDetail);

			}
            catch
            {
                ViewData["ProductId"] = new SelectList(_services.GetListProduct().ToList(), "Id", "Name", productDetail.ProductId);
				return View(productDetail);
			}
        }

		// GET: Admin/ProductDetails/Edit/5
		[Authorize(policy: Permissions.Products.Edit)]
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || !_services.HasAnyProductDetail())
            {
                return NotFound();
            }

            var productDetail = await _services.GetProductDetail(id, DateTime.Now);
            if (productDetail == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_services.GetListProduct().ToList(), "Id", "Name", productDetail.ProductId);
			ViewData["page"] = "products";
			return View(productDetail);
        }

        // POST: Admin/ProductDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, Authorize(policy: Permissions.Products.Edit)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Size,Color,Amount,Gender,ProductId")] ProductDetail productDetail, IFormFile? Image1, IFormFile? Image2)
        {
            if (id != productDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ProductDetail? detail = await _services.GetProductDetail(id, DateTime.Now);
					if (detail != null)
                    {
                        await _services.UpdateProductDetail(detail, productDetail, Image1, Image2);
						await _services.AddHistory(User, "Chỉnh sửa chi tiết sản phẩm \"" + productDetail.Id + "\"", "/Admin/ProductDetails/Details/" + productDetail.Id);
						_notyf.Success("Đã lưu chỉnh sửa!");
					}
                    else
                    {
						return NotFound();
					}
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_services.ProductDetailExists(productDetail.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
				return RedirectToAction("Details", "ProductDetails", new { id = productDetail.Id });
			}
            ViewData["ProductId"] = new SelectList(_services.GetListProduct().ToList(), "Id", "Name", productDetail.ProductId);
			ViewData["page"] = "products";
			return View(productDetail);
        }

		// GET: Admin/ProductDetails/Delete/5
		[Authorize(policy: Permissions.Products.Delete)]
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || !_services.HasAnyProductDetail())
            {
                return NotFound();
            }

            var productDetail = await _services.GetProductDetail(id, DateTime.Now);
            if (productDetail == null)
            {
                return NotFound();
            }
            ViewData["page"] = "products";
            return View(productDetail);
        }

        // POST: Admin/ProductDetails/Delete/5
        [HttpGet, ActionName("DeleteConfirmed")]
		[Authorize(policy: Permissions.Products.Delete)]
		public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!_services.HasAnyProductDetail())
            {
				_notyf.Error("Đã có lỗi xảy ra!");
			}
            var productDetail = await _services.GetProductDetail(id, DateTime.Now);

			if (productDetail != null)
			{
				if (productDetail.Carts == null)
                {
                    _services.DeleteImage(productDetail.Image1);
				    _services.DeleteImage(productDetail.Image2);
					_notyf.Success("Đã xóa chi tiết sản phẩm " + productDetail.Id);
					await _services.AddHistory(User, "Xóa chi tiết sản phẩm \"" + productDetail.Id + "\"", null);
					await _services.RemoveProductDetail(productDetail);
                }
			}
			return RedirectToAction(nameof(Index));
		}

		[HttpPost, ActionName("DeleteAsync")]
		[Authorize(policy: Permissions.Products.Delete)]
		public async Task<IActionResult> DeleteAsync(int id)
		{
			if (!_services.HasAnyProductDetail())
			{
				return Problem("Entity set 'TN408DbContext.Details'  is null.");
			}
            var productDetail = await _services.GetProductDetail(id, DateTime.Now);
            if (productDetail != null)
            {
                if (productDetail.Carts == null || productDetail.Carts.Any() == false)
				{
					_services.DeleteImage(productDetail.Image1);
					_services.DeleteImage(productDetail.Image2);
					await _services.RemoveProductDetail(productDetail);
					_notyf.Success("Đã xóa chi tiết sản phẩm " + productDetail.Id);
					return PartialView("_ProductDetail", null);
				}
				_notyf.Error("Không thể xóa chi tiết sản phẩm " + productDetail.Id);
				return PartialView("_ProductDetail", productDetail);

			}
			return PartialView("_ProductDetail", null);
		}
    }
}
