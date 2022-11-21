using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using THUD_TN408.Areas.Admin.Service;
using THUD_TN408.Data;
using THUD_TN408.Models;
using X.PagedList;

namespace THUD_TN408.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductDetailsController : Controller
    {
        private readonly TN408DbContext _context;
        private Services _services;

        public ProductDetailsController(TN408DbContext context, Services services)
        {
            _context = context;
            _services = services;
        }

        // GET: Admin/ProductDetails
        public async Task<IActionResult> Index(int page = 1)
        {
			page = (page < 1) ? 1 : page;
			int size = 8;

			var details = await _context.Details.Include(d => d.Product).ToPagedListAsync(page, size);
			ViewData["page"] = "products";
			return View(details);
        }

        // GET: Admin/ProductDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Details == null)
            {
                return NotFound();
            }

            var productDetail = await _context.Details
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productDetail == null)
            {
                return NotFound();
            }
            ViewData["page"] = "products";
            return View(productDetail);
        }

        // GET: Admin/ProductDetails/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
			ViewData["page"] = "products";
			return View();
        }

        // POST: Admin/ProductDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,Size,Color,Gender,ProductId")] ProductDetail productDetail, IFormFile? Image1, IFormFile? Image2)
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

				    _context.Add(productDetail);
                    await _context.SaveChangesAsync();
				    return RedirectToAction("Details", "Products", new {id = productDetail.ProductId, page = 1});
			    }
				ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", productDetail.ProductId);
				return View(productDetail);

			}
            catch
            {
                ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", productDetail.ProductId);
				return View(productDetail);
			}
        }

        // GET: Admin/ProductDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Details == null)
            {
                return NotFound();
            }

            var productDetail = await _context.Details.FindAsync(id);
            if (productDetail == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", productDetail.ProductId);
			ViewData["page"] = "products";
			return View(productDetail);
        }

        // POST: Admin/ProductDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Size,Color,Gender,ProductId")] ProductDetail productDetail, IFormFile? Image1, IFormFile? Image2)
        {
            if (id != productDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ProductDetail? details = await _context.Details.FindAsync(productDetail.Id);
                    if(details != null)
                    {
						_context.Details.Update(details);
						if (Image1 != null && Image1.Length > 0)
						{
                            string? fname = await _services.UploadImage(Image1);
                            if(fname != null)
                            {
								_services.DeleteImage(details.Image1);
								details.Image1 = fname;
							}
                            
						}
						if (Image2 != null && Image2.Length > 0)
						{
							string? fname = await _services.UploadImage(Image2);
							if (fname != null)
							{
								_services.DeleteImage(details.Image2);
								details.Image2 = fname;
                            }
						}
                        details.Size = productDetail.Size;
                        details.Color = productDetail.Color;
                        details.Gender = productDetail.Gender;
                        details.ProductId = productDetail.ProductId;
						await _context.SaveChangesAsync();
                    }
                    else
                    {
						return NotFound();
					}
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductDetailExists(productDetail.Id))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", productDetail.ProductId);
			ViewData["page"] = "products";
			return View(productDetail);
        }

        // GET: Admin/ProductDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Details == null)
            {
                return NotFound();
            }

            var productDetail = await _context.Details
                .Include(d => d.Product)
                .Include(d => d.Carts)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productDetail == null)
            {
                return NotFound();
            }
            ViewData["page"] = "products";
            return View(productDetail);
        }

        // POST: Admin/ProductDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Details == null)
            {
                return Problem("Entity set 'TN408DbContext.Details'  is null.");
            }
            var productDetail = await _context.Details.FindAsync(id);
            if (productDetail != null)
            {
                _services.DeleteImage(productDetail.Image1);
                _services.DeleteImage(productDetail.Image2);
                _context.Details.Remove(productDetail);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductDetailExists(int id)
        {
          return _context.Details.Any(e => e.Id == id);
        }
    }
}
