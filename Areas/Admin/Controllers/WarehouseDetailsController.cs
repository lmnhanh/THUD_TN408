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
    public class WarehouseDetailsController : Controller
    {
        private readonly TN408DbContext _context;

        public WarehouseDetailsController(TN408DbContext context)
        {
            _context = context;
        }

        // GET: Admin/WarehouseDetails
        public async Task<IActionResult> Index()
        {
            var tN408DbContext = _context.WarehouseDetails.Include(w => w.ProductDetail).Include(w => w.Warehouse);
            return View(await tN408DbContext.ToListAsync());
        }

        // GET: Admin/WarehouseDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.WarehouseDetails == null)
            {
                return NotFound();
            }

            var warehouseDetail = await _context.WarehouseDetails
                .Include(w => w.ProductDetail)
                .Include(w => w.Warehouse)
                .FirstOrDefaultAsync(m => m.WarehouseId == id);
            if (warehouseDetail == null)
            {
                return NotFound();
            }

            return View(warehouseDetail);
        }

        // GET: Admin/WarehouseDetails/Create
        public IActionResult Create()
        {
            ViewData["ProductDetailId"] = new SelectList(_context.Details, "Id", "Id");
            ViewData["WarehouseId"] = new SelectList(_context.Warehouses, "Id", "Id");
            return View();
        }

        // POST: Admin/WarehouseDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WarehouseId,ProductDetailId,Stock")] WarehouseDetail warehouseDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(warehouseDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductDetailId"] = new SelectList(_context.Details, "Id", "Id", warehouseDetail.ProductDetailId);
            ViewData["WarehouseId"] = new SelectList(_context.Warehouses, "Id", "Id", warehouseDetail.WarehouseId);
            return View(warehouseDetail);
        }

        // GET: Admin/WarehouseDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.WarehouseDetails == null)
            {
                return NotFound();
            }

            var warehouseDetail = await _context.WarehouseDetails.FindAsync(id);
            if (warehouseDetail == null)
            {
                return NotFound();
            }
            ViewData["ProductDetailId"] = new SelectList(_context.Details, "Id", "Id", warehouseDetail.ProductDetailId);
            ViewData["WarehouseId"] = new SelectList(_context.Warehouses, "Id", "Id", warehouseDetail.WarehouseId);
            return View(warehouseDetail);
        }

        // POST: Admin/WarehouseDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("WarehouseId,ProductDetailId,Stock")] WarehouseDetail warehouseDetail)
        {
            if (id != warehouseDetail.WarehouseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(warehouseDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WarehouseDetailExists(warehouseDetail.WarehouseId))
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
            ViewData["ProductDetailId"] = new SelectList(_context.Details, "Id", "Id", warehouseDetail.ProductDetailId);
            ViewData["WarehouseId"] = new SelectList(_context.Warehouses, "Id", "Id", warehouseDetail.WarehouseId);
            return View(warehouseDetail);
        }

        // GET: Admin/WarehouseDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.WarehouseDetails == null)
            {
                return NotFound();
            }

            var warehouseDetail = await _context.WarehouseDetails
                .Include(w => w.ProductDetail)
                .Include(w => w.Warehouse)
                .FirstOrDefaultAsync(m => m.WarehouseId == id);
            if (warehouseDetail == null)
            {
                return NotFound();
            }

            return View(warehouseDetail);
        }

        // POST: Admin/WarehouseDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.WarehouseDetails == null)
            {
                return Problem("Entity set 'TN408DbContext.WarehouseDetails'  is null.");
            }
            var warehouseDetail = await _context.WarehouseDetails.FindAsync(id);
            if (warehouseDetail != null)
            {
                _context.WarehouseDetails.Remove(warehouseDetail);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WarehouseDetailExists(int id)
        {
          return _context.WarehouseDetails.Any(e => e.WarehouseId == id);
        }
    }
}
