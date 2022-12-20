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
    public class ImportDetailsController : Controller
    {
        private readonly TN408DbContext _context;
		private readonly Services _services;
		private readonly INotyfService _notyf;

		public ImportDetailsController(TN408DbContext context, UserManager<User> userManager, INotyfService notyf)
        {
			_services = new Services(context, userManager);
			_notyf = notyf;
			_context = context;
        }

        // GET: Admin/ImportDetails
        public async Task<IActionResult> Index()
        {
            var tN408DbContext = _context.ImportDetails.Include(i => i.ImportNote).Include(i => i.ProductDetail);
            return View(await tN408DbContext.ToListAsync());
        }

        // GET: Admin/ImportDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ImportDetails == null)
            {
                return NotFound();
            }

            var importDetail = await _context.ImportDetails
                .Include(i => i.ImportNote)
                .Include(i => i.ProductDetail)
                .FirstOrDefaultAsync(m => m.ImportNoteId == id);
            if (importDetail == null)
            {
                return NotFound();
            }

            return View(importDetail);
        }

		// GET: Admin/ImportDetails/Create
		[Authorize(policy: Permissions.ImportNotes.Create)]
		public IActionResult Create(int? importNoteId)
        {
			if (importNoteId == null)
				ViewData["ImportNoteId"] = new SelectList(_context.ImportNotes, "Id", "Id");
            else
				ViewData["ImportNoteId"] = new SelectList(_context.ImportNotes, "Id", "Id", importNoteId);
			ViewData["ProductDetailId"] = new SelectList(_services.GetListProductDetails(DateTime.Now), "Id", "FullName");
            ViewData["page"] = "warehouses";

			return View();
        }

        // POST: Admin/ImportDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(policy: Permissions.ImportNotes.Create)]
		public async Task<IActionResult> Create([Bind("ImportNoteId,ProductDetailId,Quantity")] ImportDetail importDetail)
        {
            if (ModelState.IsValid && !ImportDetailExists(importDetail.ImportNoteId, importDetail.ProductDetailId))
            {
				_context.ImportDetails.Add(importDetail);
                await _context.SaveChangesAsync();

                var idetail = _context.ImportDetails.Include(x=>x.ImportNote).Where(x => x.ProductDetailId == importDetail.ProductDetailId && x.ImportNoteId == importDetail.ImportNoteId).FirstOrDefault();

                var oldDetail = await _context.WarehouseDetails
                    .FirstOrDefaultAsync(w => w.ProductDetailId == idetail.ProductDetailId &&
                    w.WarehouseId == idetail.ImportNote.WarehouseId);

                if (oldDetail == null)
                {
					var newDetail = new WarehouseDetail();
					newDetail.ProductDetailId = idetail.ProductDetailId;
					newDetail.WarehouseId = idetail.ImportNote.WarehouseId;
					newDetail.Stock = importDetail.Quantity;
					_context.WarehouseDetails.Add(newDetail);
                    await _context.SaveChangesAsync();
                    //return RedirectToAction("Index", "ImportDetails", new {importNoteId = idetail.ImportNoteId});
                }
                else
                {
					_context.WarehouseDetails.Update(oldDetail);
					oldDetail.Stock += importDetail.Quantity;
					await _context.SaveChangesAsync();

					return RedirectToAction(nameof(Index));
				}
                

            }
            
            ViewData["ImportNoteId"] = new SelectList(_context.ImportNotes, "Id", "Id", importDetail.ImportNoteId);
            ViewData["ProductDetailId"] = new SelectList(_services.GetListProductDetails(DateTime.Now), "Id", "FullName", importDetail.ProductDetailId);
            return View(importDetail);
        }

		// GET: Admin/ImportDetails/Edit/5
		[Authorize(policy: Permissions.ImportNotes.Edit)]
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ImportDetails == null)
            {
                return NotFound();
            }

            var importDetail = await _context.ImportDetails.FindAsync(id);
            if (importDetail == null)
            {
                return NotFound();
            }
            ViewData["ImportNoteId"] = new SelectList(_context.ImportNotes, "Id", "Id", importDetail.ImportNoteId);
            ViewData["ProductDetailId"] = new SelectList(_services.GetListProductDetails(DateTime.Now), "Id", "FullName", importDetail.ProductDetailId);
            return View(importDetail);
        }

        // POST: Admin/ImportDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(policy: Permissions.ImportNotes.Edit)]
		public async Task<IActionResult> Edit(int id, [Bind("ImportNoteId,ProductDetailId,Quantity")] ImportDetail importDetail)
        {
            if (id != importDetail.ImportNoteId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(importDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImportDetailExists(importDetail.ImportNoteId, importDetail.ProductDetailId))
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
            ViewData["ImportNoteId"] = new SelectList(_context.ImportNotes, "Id", "Id", importDetail.ImportNoteId);
            ViewData["ProductDetailId"] = new SelectList(_services.GetListProductDetails(DateTime.Now), "Id", "FullName", importDetail.ProductDetailId);
            return View(importDetail);
        }

		// GET: Admin/ImportDetails/Delete/5
		[Authorize(policy: Permissions.ImportNotes.Delete)]
		public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ImportDetails == null)
            {
                return NotFound();
            }

            var importDetail = await _context.ImportDetails
                .Include(i => i.ImportNote)
                .Include(i => i.ProductDetail)
                .FirstOrDefaultAsync(m => m.ImportNoteId == id);
            if (importDetail == null)
            {
                return NotFound();
            }

            return View(importDetail);
        }

        // POST: Admin/ImportDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
		[Authorize(policy: Permissions.ImportNotes.Delete)]
		public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ImportDetails == null)
            {
                return Problem("Entity set 'TN408DbContext.ImportDetails'  is null.");
            }
            var importDetail = await _context.ImportDetails.FindAsync(id);
            if (importDetail != null)
            {
                _context.ImportDetails.Remove(importDetail);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ImportDetailExists(int id, int detailId)
        {
          return _context.ImportDetails.Any(e => e.ImportNoteId == id && e.ProductDetailId == detailId);
        }
    }
}
