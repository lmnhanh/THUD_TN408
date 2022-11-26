using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using THUD_TN408.Data;
using THUD_TN408.Models;
using X.PagedList;

namespace THUD_TN408.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductPromotionsController : Controller
    {
        private readonly TN408DbContext _context;

        public ProductPromotionsController(TN408DbContext context)
        {
            _context = context;
        }

        // GET: Admin/ProductPromotions
        public async Task<IActionResult> Index()
        {
            var promotions = _context.PPromotions.Include(p => p.Product).ToPagedListAsync(1, 8);
			ViewData["page"] = "ppromotions";
			return View(await promotions);
        }

		[HttpGet]
		public async Task<IActionResult> Paging(int? productId,int page = 1, int size = 8)
		{
			page = (page < 1) ? 1 : page;
			size = (size < 2) ? 2 : size;

			IPagedList<ProductPromotion> promotions;
			if (productId != null)
			{
				var product = await _context.Products.Include(c => c.Promotions).FirstOrDefaultAsync(m => m.Id == productId);
				if (product != null)
				{
					promotions = await product.Promotions.ToPagedListAsync(page, size);
					return PartialView("_ListProductPromotions", promotions);
				}
				return PartialView("_ListProductPromotions", null);
			}

			promotions = await _context.PPromotions.ToPagedListAsync(page, size);
			return PartialView("_ListProductPromotions", promotions);
		}

		// GET: Admin/ProductPromotions/Details/5
		public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.PPromotions == null)
            {
                return NotFound();
            }

            var productPromotion = await _context.PPromotions
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
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
				ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", productId);
			}
            else
            {
				ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name");
			}
            ViewData["page"] = "ppromotions";
			return View();
        }

        // POST: Admin/ProductPromotions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,ApplyFrom,ValidTo,Stock,DiscountPercent,IsActive,ProductId")] ProductPromotion productPromotion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productPromotion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", productPromotion.ProductId);
            return View(productPromotion);
        }

        // GET: Admin/ProductPromotions/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.PPromotions == null)
            {
                return NotFound();
            }

            var productPromotion = await _context.PPromotions.FindAsync(id);
            if (productPromotion == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", productPromotion.ProductId);
            return View(productPromotion);
        }

        // POST: Admin/ProductPromotions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                    _context.Update(productPromotion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductPromotionExists(productPromotion.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Name", productPromotion.ProductId);
            return View(productPromotion);
        }

        // GET: Admin/ProductPromotions/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.PPromotions == null)
            {
                return NotFound();
            }

            var productPromotion = await _context.PPromotions
                .Include(p => p.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
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
            if (_context.PPromotions == null)
            {
                return Problem("Entity set 'TN408DbContext.PPromotions'  is null.");
            }
            var productPromotion = await _context.PPromotions.FindAsync(id);
            if (productPromotion != null)
            {
                _context.PPromotions.Remove(productPromotion);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductPromotionExists(string id)
        {
          return _context.PPromotions.Any(e => e.Id == id);
        }
    }
}
