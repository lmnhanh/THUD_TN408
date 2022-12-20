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

namespace THUD_TN408.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "SuperAdmin,WarehouseManager")]
    public class ImportNotesController : Controller
    {
        private readonly TN408DbContext _context;
		private readonly Services _services;
		private readonly INotyfService _notyf;

		public ImportNotesController(TN408DbContext context, UserManager<User> userManager, INotyfService notyf)
		{
			_services = new Services(context, userManager);
			_notyf = notyf;
			_context = context;
        }

        // GET: Admin/ImportNotes
        public async Task<IActionResult> Index()
        {
            ViewData["page"] = "warehouses_imports";
            var tN408DbContext = _context.ImportNotes.Include(i => i.User).Include(i => i.Warehouse);
            return View(await tN408DbContext.ToListAsync());
        }

        // GET: Admin/ImportNotes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ImportNotes == null)
            {
                return NotFound();
            }

            var importNote = await _context.ImportNotes
                .Include(i => i.User)
                .Include(i => i.Warehouse)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (importNote == null)
            {
                return NotFound();
            }
            var detail = _services.GetListProductDetails(DateTime.Now).ToList();
            ViewData["ListImportDetails"] = await _context.ImportDetails.Where(x => x.ImportNoteId == importNote.Id).Include(x => x.ProductDetail).ToListAsync();
			ViewData["page"] = "warehouses_imports";
			return View(importNote);
        }

		// GET: Admin/ImportNotes/Create
		[Authorize(policy: Permissions.ImportNotes.Create)]
		public async Task<IActionResult> Create()
        {
			var list = new List<User>();
			list.Add(await _services.GetUser(User));
			ViewData["UserId"] = new SelectList(list, "Id", "Email");
            ViewData["WarehouseId"] = new SelectList(_context.Warehouses, "Id", "Name");
			ViewData["page"] = "warehouses_imports";
			return View(new ImportNote());
        }

        // POST: Admin/ImportNotes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(policy: Permissions.ImportNotes.Create)]
		public async Task<IActionResult> Create([Bind("Id,CreatedDate,Total,WarehouseId,UserId")] ImportNote importNote)
        {
            if (ModelState.IsValid)
            {
                _context.Add(importNote);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            var list = new List<User>();
            list.Add(await _services.GetUser(User));

            ViewData["UserId"] = new SelectList(list, "Id", "Email", importNote.UserId);
            ViewData["WarehouseId"] = new SelectList(_context.Warehouses, "Id", "Name", importNote.WarehouseId);
			ViewData["page"] = "warehouses_imports";
			return View(importNote);
        }

		// GET: Admin/ImportNotes/Edit/5
		[Authorize(policy: Permissions.ImportNotes.Edit)]
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ImportNotes == null)
            {
                return NotFound();
            }

            var importNote = await _context.ImportNotes.FindAsync(id);
            if (importNote == null)
            {
                return NotFound();
            }
			var list = new List<User>();
			list.Add(await _services.GetUser(User));
			ViewData["UserId"] = new SelectList(list, "Id", "Email", importNote.UserId);
            ViewData["WarehouseId"] = new SelectList(_context.Warehouses, "Id", "Name", importNote.WarehouseId);
			ViewData["page"] = "warehouses_imports";
			return View(importNote);
        }

        // POST: Admin/ImportNotes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(policy: Permissions.ImportNotes.Edit)]
		public async Task<IActionResult> Edit(int id, [Bind("Id,CreatedDate,Total,WarehouseId,UserId")] ImportNote importNote)
        {
            if (id != importNote.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(importNote);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImportNoteExists(importNote.Id))
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
			var list = new List<User>();
			list.Add(await _services.GetUser(User));
			ViewData["UserId"] = new SelectList(list, "Id", "Email", importNote.UserId);
            ViewData["WarehouseId"] = new SelectList(_context.Warehouses, "Id", "Name", importNote.WarehouseId);
			ViewData["page"] = "warehouses_imports";
			return View(importNote);
        }

		// GET: Admin/ImportNotes/Delete/5
		[Authorize(policy: Permissions.ImportNotes.Delete)]
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ImportNotes == null)
            {
                return NotFound();
            }

            var importNote = await _context.ImportNotes
                .Include(i => i.User)
                .Include(i => i.Warehouse)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (importNote == null)
            {
                return NotFound();
            }
			ViewData["page"] = "warehouses_imports";
			return View(importNote);
        }

        // POST: Admin/ImportNotes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
		[Authorize(policy: Permissions.Products.Delete)]
		public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ImportNotes == null)
            {
                return Problem("Entity set 'TN408DbContext.ImportNotes'  is null.");
            }
            var importNote = await _context.ImportNotes.FindAsync(id);
            if (importNote != null)
            {
                _context.ImportNotes.Remove(importNote);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ImportNoteExists(int id)
        {
          return _context.ImportNotes.Any(e => e.Id == id);
        }
    }
}
