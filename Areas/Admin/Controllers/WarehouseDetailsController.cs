﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using THUD_TN408.Authorization;
using THUD_TN408.Data;
using THUD_TN408.Models;
using X.PagedList;

namespace THUD_TN408.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Saleman,SuperAdmin,WarehouseManager")]
    public class WarehouseDetailsController : Controller
    {
        private readonly TN408DbContext _context;
		private readonly INotyfService _notyf;

		public WarehouseDetailsController(TN408DbContext context, INotyfService notyf)
        {
            _context = context;
			_notyf = notyf;
		}

        // GET: Admin/WarehouseDetails
        public async Task<IActionResult> Index()
        {
            var warehouseDetails = _context.WarehouseDetails.Include(w => w.Warehouse).Include(w => w.ProductDetail);
			return View(await warehouseDetails.ToPagedListAsync(1, 10));
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
		[Authorize(policy: Permissions.Warehouses.Create)]
		public IActionResult Create()
        {
			var details = _context.Details.Include(x => x.Product);
			foreach (var detail in details)
			{
				detail.FullName = detail.Product?.Name + ", " + ((detail.Gender == true) ? "Nam" : "Nữ") + ", " + detail.Size + ", " + detail.Color;
			}

			ViewData["ProductDetailId"] = new SelectList(details.ToList().OrderBy(d => d.FullName), "Id", "FullName");
            ViewData["WarehouseId"] = new SelectList(_context.Warehouses, "Id", "Name");
            return View(new WarehouseDetail());
        }

        // POST: Admin/WarehouseDetails/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(policy: Permissions.Warehouses.Create)]
		public async Task<IActionResult> Create([Bind("WarehouseId,ProductDetailId,Stock")] WarehouseDetail warehouseDetail)
        {
            if (ModelState.IsValid)
            {
                var oldDetail = await _context.WarehouseDetails.AsNoTracking().Include(w => w.Warehouse)
                    .FirstOrDefaultAsync(w =>
                    w.ProductDetailId == warehouseDetail.ProductDetailId &&
                    w.WarehouseId == warehouseDetail.WarehouseId);
                if(oldDetail == null)
                {
					 _context.Add(warehouseDetail);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("DetailStockIndex", "Warehouses");
				}
                _notyf.Error("Kho " + oldDetail.Warehouse?.Name + " đã chứa " + oldDetail.Stock +" sản phẩm này");
            }

            var details = _context.Details.Include(x => x.Product);
            foreach(var detail in details)
            {
                detail.FullName = detail.Product?.Name + ", " + ((detail.Gender == true)? "Nam" : "Nữ")+ ", " + detail.Size + ", " + detail.Color;
            }

			ViewData["ProductDetailId"] = new SelectList(details.ToList().OrderBy(x => x.FullName), "Id", "FullName", warehouseDetail.ProductDetailId);
            ViewData["WarehouseId"] = new SelectList(_context.Warehouses, "Id", "Name", warehouseDetail.WarehouseId);
            return View(warehouseDetail);
        }

		// GET: Admin/WarehouseDetails/Edit/5
		[Authorize(policy: Permissions.Warehouses.Edit)]
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
            ViewData["ProductDetailId"] = new SelectList(_context.Details.Include(x => x.Product), "Id", "Product.Name", warehouseDetail.ProductDetailId);
            ViewData["WarehouseId"] = new SelectList(_context.Warehouses, "Id", "Name", warehouseDetail.WarehouseId);
            return View(warehouseDetail);
        }

        // POST: Admin/WarehouseDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(policy: Permissions.Warehouses.Edit)]
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
            ViewData["ProductDetailId"] = new SelectList(_context.Details.Include(x => x.Product), "Id", "Product.Name", warehouseDetail.ProductDetailId);
            ViewData["WarehouseId"] = new SelectList(_context.Warehouses, "Id", "Name", warehouseDetail.WarehouseId);
            return View(warehouseDetail);
        }

		// GET: Admin/WarehouseDetails/Delete/5
		[Authorize(policy: Permissions.Warehouses.Delete)]
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
		[Authorize(policy: Permissions.Warehouses.Delete)]
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
