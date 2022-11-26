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
    public class OrderPromotionsController : Controller
    {
        private readonly TN408DbContext _context;

        public OrderPromotionsController(TN408DbContext context)
        {
            _context = context;
        }

        // GET: Admin/OrderPromotions
        public async Task<IActionResult> Index()
        {
              return View(await _context.OPromotions.ToListAsync());
        }

        // GET: Admin/OrderPromotions/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.OPromotions == null)
            {
                return NotFound();
            }

            var orderPromotion = await _context.OPromotions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderPromotion == null)
            {
                return NotFound();
            }

            return View(orderPromotion);
        }

        // GET: Admin/OrderPromotions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/OrderPromotions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,ApplyFrom,ValidTo,Stock,MaxDiscount,DiscountPercent,ApplyCondition,IsActive")] OrderPromotion orderPromotion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderPromotion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(orderPromotion);
        }

        // GET: Admin/OrderPromotions/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.OPromotions == null)
            {
                return NotFound();
            }

            var orderPromotion = await _context.OPromotions.FindAsync(id);
            if (orderPromotion == null)
            {
                return NotFound();
            }
            return View(orderPromotion);
        }

        // POST: Admin/OrderPromotions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Description,ApplyFrom,ValidTo,Stock,MaxDiscount,DiscountPercent,ApplyCondition,IsActive")] OrderPromotion orderPromotion)
        {
            if (id != orderPromotion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderPromotion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderPromotionExists(orderPromotion.Id))
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
            return View(orderPromotion);
        }

        // GET: Admin/OrderPromotions/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.OPromotions == null)
            {
                return NotFound();
            }

            var orderPromotion = await _context.OPromotions
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderPromotion == null)
            {
                return NotFound();
            }

            return View(orderPromotion);
        }

        // POST: Admin/OrderPromotions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.OPromotions == null)
            {
                return Problem("Entity set 'TN408DbContext.OPromotions'  is null.");
            }
            var orderPromotion = await _context.OPromotions.FindAsync(id);
            if (orderPromotion != null)
            {
                _context.OPromotions.Remove(orderPromotion);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderPromotionExists(string id)
        {
          return _context.OPromotions.Any(e => e.Id == id);
        }
    }
}
