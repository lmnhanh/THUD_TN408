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
    [Area("Admin"), Authorize(Roles = "Saleman,SuperAdmin,WarehouseManager")]
    public class ProducersController : Controller
    {
        private readonly TN408DbContext _context;

        public ProducersController(TN408DbContext context)
        {
			_context = context;
        }

        // GET: Admin/Producers
        public async Task<IActionResult> Index()
        {
			ViewData["page"] = "producers";
			return View(await _context.Producers.ToListAsync());
        }

        // GET: Admin/Producers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Producers == null)
            {
                return NotFound();
            }

            var producer = await _context.Producers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producer == null)
            {
                return NotFound();
            }
			ViewData["page"] = "producers";
			return View(producer);
        }

		// GET: Admin/Producers/Create
		[HttpGet, Authorize(policy: Permissions.Producers.Create)]
		public IActionResult Create()
        {
			ViewData["page"] = "producers_create";
			return View();
        }

        // POST: Admin/Producers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(policy: Permissions.Producers.Create)]
		public async Task<IActionResult> Create([Bind("Id,Name,PhoneNumber,Email")] Producer producer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(producer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
			ViewData["page"] = "producers_create";
			return View(producer);
        }

		// GET: Admin/Producers/Edit/5
		[HttpPost, Authorize(policy: Permissions.Producers.Edit)]
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Producers == null)
            {
                return NotFound();
            }

            var producer = await _context.Producers.FindAsync(id);
            if (producer == null)
            {
                return NotFound();
            }
			ViewData["page"] = "producers";
			return View(producer);
        }

        // POST: Admin/Producers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(policy: Permissions.Producers.Edit)]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Name,PhoneNumber,Email")] Producer producer)
        {
            if (id != producer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(producer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProducerExists(producer.Id))
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
			ViewData["page"] = "producers";
			return View(producer);
        }

		// GET: Admin/Producers/Delete/5
		[Authorize(policy: Permissions.Producers.Edit)]
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Producers == null)
            {
                return NotFound();
            }

            var producer = await _context.Producers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producer == null)
            {
                return NotFound();
            }
			ViewData["page"] = "producers";
			return View(producer);
        }

        // POST: Admin/Producers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
		[Authorize(policy: Permissions.Producers.Delete)]
		public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Producers == null)
            {
                return Problem("Entity set 'TN408DbContext.Producers'  is null.");
            }
            var producer = await _context.Producers.FindAsync(id);
            if (producer != null)
            {
                _context.Producers.Remove(producer);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProducerExists(int id)
        {
          return _context.Producers.Any(e => e.Id == id);
        }
    }
}
