using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using THUD_TN408.Data;
using THUD_TN408.Models;

namespace THUD_TN408.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PricesController : Controller
    {
        private readonly TN408DbContext _context;

        public PricesController(TN408DbContext context)
        {
            _context = context;
        }

        // GET: Admin/Prices
        public async Task<IActionResult> Index(int? productDetailId)
        {
            var prices = _context.Prices.Include(p => p.Detail);
            if(productDetailId != null)
            {
				return View(await prices.Where(p => p.ProductDetailId == productDetailId).OrderByDescending(p => p.ApplyFrom).ToListAsync());
			}
            return View(await prices.OrderByDescending(p => p.ApplyFrom).ToListAsync());
        }

        // GET: Admin/Prices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Prices == null)
            {
                return NotFound();
            }

            var price = await _context.Prices
                .Include(p => p.Detail)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (price == null)
            {
                return NotFound();
            }

            return View(price);
        }

        // GET: Admin/Prices/Create
        public IActionResult Create()
        {
            ViewData["ProductDetailId"] = new SelectList(_context.Details, "Id", "ProductDetailId");
            return View();
        }

        // POST: Admin/Prices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Amount,ApplyFrom,ProductDetailId")] Price price)
        {
            if (ModelState.IsValid)
            {
                _context.Add(price);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductDetailId"] = new SelectList(_context.Details, "Id", "ProductDetailId", price.ProductDetailId);
            return View(price);
        }

        // GET: Admin/Prices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Prices == null)
            {
                return NotFound();
            }

            var price = await _context.Prices.FindAsync(id);
            if (price == null)
            {
                return NotFound();
            }
            ViewData["ProductDetailId"] = new SelectList(_context.Details, "Id", "ProductDetailId", price.ProductDetailId);
            return View(price);
        }

        // POST: Admin/Prices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Amount,ApplyFrom,ProductDetailId")] Price price)
        {
            if (id != price.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(price);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PriceExists(price.Id))
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
            ViewData["ProductDetailId"] = new SelectList(_context.Details, "Id", "ProductDetailId", price.ProductDetailId);
            return View(price);
        }

        // GET: Admin/Prices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Prices == null)
            {
                return NotFound();
            }

            var price = await _context.Prices
                .Include(p => p.Detail)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (price == null)
            {
                return NotFound();
            }

            return View(price);
        }

        // POST: Admin/Prices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Prices == null)
            {
                return Problem("Entity set 'TN408DbContext.Prices'  is null.");
            }
            var price = await _context.Prices.FindAsync(id);
            if (price != null)
            {
                _context.Prices.Remove(price);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PriceExists(int id)
        {
          return _context.Prices.Any(e => e.Id == id);
        }
    }
}
