using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using THUD_TN408.Authorization;
using THUD_TN408.Data;
using THUD_TN408.Models;

namespace THUD_TN408.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Saleman,SuperAdmin")]
    public class ShipmentsController : Controller
    {
        private readonly TN408DbContext _context;

        public ShipmentsController(TN408DbContext context)
        {
            _context = context;
        }

        // GET: Admin/Shipments
        public async Task<IActionResult> Index()
        {
			ViewData["page"] = "shipments";
			return View(await _context.Shipments.ToListAsync());
        }

        // GET: Admin/Shipments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Shipments == null)
            {
                return NotFound();
            }

            var shipment = await _context.Shipments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shipment == null)
            {
                return NotFound();
            }
			ViewData["page"] = "shipments";
			return View(shipment);
        }

		// GET: Admin/Shipments/Create
		[Authorize(policy: Permissions.Shipments.Create)]
		public IActionResult Create()
        {
			ViewData["page"] = "shipments_create";
			return View();
        }

        // POST: Admin/Shipments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(policy: Permissions.Shipments.Create)]
		public async Task<IActionResult> Create([Bind("Id,Name")] Shipment shipment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shipment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
			ViewData["page"] = "shipments_create";
			return View(shipment);
        }

		// GET: Admin/Shipments/Edit/5
		[Authorize(policy: Permissions.Shipments.Edit)]
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Shipments == null)
            {
                return NotFound();
            }

            var shipment = await _context.Shipments.FindAsync(id);
            if (shipment == null)
            {
                return NotFound();
            }
			ViewData["page"] = "shipments";
			return View(shipment);
        }

        // POST: Admin/Shipments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(policy: Permissions.Shipments.Edit)]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Shipment shipment)
        {
            if (id != shipment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shipment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShipmentExists(shipment.Id))
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
			ViewData["page"] = "shipments";
			return View(shipment);
        }

		// GET: Admin/Shipments/Delete/5
		[Authorize(policy: Permissions.Shipments.Delete)]
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Shipments == null)
            {
                return NotFound();
            }

            var shipment = await _context.Shipments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shipment == null)
            {
                return NotFound();
            }
			ViewData["page"] = "shipments";
			return View(shipment);
        }

        // POST: Admin/Shipments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
		[Authorize(policy: Permissions.Shipments.Delete)]
		public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Shipments == null)
            {
                return Problem("Entity set 'TN408DbContext.Shipments'  is null.");
            }
            var shipment = await _context.Shipments.FindAsync(id);
            if (shipment != null)
            {
                _context.Shipments.Remove(shipment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShipmentExists(int id)
        {
          return _context.Shipments.Any(e => e.Id == id);
        }
    }
}
