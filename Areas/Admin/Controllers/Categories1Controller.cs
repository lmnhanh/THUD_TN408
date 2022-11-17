﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using THUD_TN408.Data;
using THUD_TN408.Models;
using X.PagedList;

namespace THUD_TN408.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize]
    public class Categories1Controller : Controller
    {
        private readonly TN408DbContext _context;

        public Categories1Controller(TN408DbContext context)
        {
            _context = context;
        }

		// GET: Admin/Categories
		public async Task<IActionResult> Index(int page = 1)
        {
            ViewData["page"] = "categories";
            page = (page < 1) ? 1 : page;
            int size = 8;

			var categories = await _context.Categories.ToPagedListAsync(page, size);
			return View(categories);
        }

		// GET: Admin/Categories/Details/5
		public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.Include(c => c.Products)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            ViewData["ListProducts"] = category.Products;
			ViewData["page"] = "categories";
			return View(category);
        }

        // GET: Admin/Categories/Create
        public IActionResult Create()
        {
			ViewData["page"] = "categories_create";
			return View();
        }

        // POST: Admin/Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,Name,IsActive")] Category category)
		{
			if (ModelState.IsValid)
			{
				_context.Add(category);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			ViewData["page"] = "categories_create";
			return View(category);
		}

		// GET: Admin/Categories/Edit/5
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
			ViewData["page"] = "categories";
			return View(category);
        }

        // POST: Admin/Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,Name,IsActive")] Category category)
		{
			if (id != category.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(category);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!CategoryExists(category.Id))
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
			ViewData["page"] = "categories";
			return View(category);
		}

		// GET: Admin/Categories/Delete/5
		public async Task<IActionResult> Delete(int? id)
        {
			if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.Where(c => c.Id == id).Include(c => c.Products).FirstOrDefaultAsync();
            if (category == null)
            {
                return NotFound();
            }
            if (category.Products.Any())
            {
                _context.Update(category);
                category.IsActive = false;
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
		    _context.Categories.Remove(category);
			await _context.SaveChangesAsync();
            
			return RedirectToAction(nameof(Index));
		}

        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'TN408DbContext.Categories'  is null.");
            }
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }
            
            await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
          return _context.Categories.Any(e => e.Id == id);
        }
    }
}