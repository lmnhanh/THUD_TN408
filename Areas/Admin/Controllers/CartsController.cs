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
    public class CartsController : Controller
    {
        private readonly TN408DbContext _context;

        public CartsController(TN408DbContext context)
        {
            _context = context;
        }

        // GET: Admin/Carts
        public async Task<IActionResult> Index()
        {
            var tN408DbContext = _context.Carts.Include(c => c.Detail).Include(c => c.Order).Include(c => c.User);
            return View(await tN408DbContext.ToListAsync());
        }

        // GET: Admin/Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Carts == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.Detail)
                .Include(c => c.Order)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Admin/Carts/Create
        public IActionResult Create()
        {
            ViewData["ProductDetailId"] = new SelectList(_context.Details, "Id", "Size");
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Admin/Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Quantity,IsDeleted,IsCheckedOut,ProductDetailId,OrderId,UserId")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductDetailId"] = new SelectList(_context.Details, "Id", "Size", cart.ProductDetailId);
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", cart.OrderId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", cart.UserId);
            return View(cart);
        }

        // GET: Admin/Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Carts == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["ProductDetailId"] = new SelectList(_context.Details, "Id", "Size", cart.ProductDetailId);
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", cart.OrderId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", cart.UserId);
            return View(cart);
        }

        // POST: Admin/Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Quantity,IsDeleted,IsCheckedOut,ProductDetailId,OrderId,UserId")] Cart cart)
        {
            if (id != cart.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.Id))
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
            ViewData["ProductDetailId"] = new SelectList(_context.Details, "Id", "Size", cart.ProductDetailId);
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", cart.OrderId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", cart.UserId);
            return View(cart);
        }

        // GET: Admin/Carts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Carts == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.Detail)
                .Include(c => c.Order)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Admin/Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Carts == null)
            {
                return Problem("Entity set 'TN408DbContext.Carts'  is null.");
            }
            var cart = await _context.Carts.FindAsync(id);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int id)
        {
          return _context.Carts.Any(e => e.Id == id);
        }
    }
}
