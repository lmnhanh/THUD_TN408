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
    public class HistoriesController : Controller
    {
        private readonly TN408DbContext _context;

        public HistoriesController(TN408DbContext context)
        {
            _context = context;
        }

        // GET: Admin/Histories
        public async Task<IActionResult> Index(string userID)
        {
            var tN408DbContext = _context.Histories.Include(h => h.User).Where(h => h.UserId == null || h.UserId == userID).OrderByDescending(x => x.CreatedAt);
            return View(await tN408DbContext.ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> GetNotifications(string id)
        {
            return PartialView("_Notifications", await _context.Histories.Where(x => x.UserId == id)
                .Where(x => x.Status == true).OrderByDescending(x => x.CreatedAt).ToListAsync());
        }

        // GET: Admin/Histories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Histories == null)
            {
                return NotFound();
            }

            var history = await _context.Histories
                .Include(h => h.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (history == null)
            {
                return NotFound();
            }

            return View(history);
        }

        // GET: Admin/Histories/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Admin/Histories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Message,TargetUrl,Status,CreatedAt,UserId")] History history)
        {
            if (ModelState.IsValid)
            {
                _context.Add(history);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", history.UserId);
            return View(history);
        }

        // GET: Admin/Histories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Histories == null)
            {
                return NotFound();
            }

            var history = await _context.Histories.FindAsync(id);
            if (history == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", history.UserId);
            return View(history);
        }

        // POST: Admin/Histories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Message,TargetUrl,Status,CreatedAt,UserId")] History history)
        {
            if (id != history.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(history);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HistoryExists(history.Id))
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
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", history.UserId);
            return View(history);
        }

        // GET: Admin/Histories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Histories == null)
            {
                return NotFound();
            }

            var history = await _context.Histories
                .Include(h => h.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (history == null)
            {
                return NotFound();
            }

            return View(history);
        }

        // POST: Admin/Histories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Histories == null)
            {
                return Problem("Entity set 'TN408DbContext.Histories'  is null.");
            }
            var history = await _context.Histories.FindAsync(id);
            if (history != null)
            {
                _context.Histories.Remove(history);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HistoryExists(int id)
        {
          return _context.Histories.Any(e => e.Id == id);
        }
    }
}
