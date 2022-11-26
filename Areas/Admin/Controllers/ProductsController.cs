using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using THUD_TN408.Data;
using THUD_TN408.Models;
using X.PagedList;

namespace THUD_TN408.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly TN408DbContext _context;

        public ProductsController(TN408DbContext context)
        {
            _context = context;
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.ToPagedListAsync(1, 8);
			ViewData["page"] = "products";
			return View(products);
        }

        //GET products with paging
        public async Task<IActionResult> Paging(int? categoryId, int page = 1, int size = 8)
        {
            page = (page < 1) ? 1 : page;
            size = (size < 2) ? 2 : size;
            IPagedList<Product> products;
            if(categoryId != null)
            {
                var category = await _context.Categories.Include(c => c.Products).FirstOrDefaultAsync(m => m.Id == categoryId);
                if(category != null)
                {
					products = await category.Products.ToPagedListAsync(page, size);
					return PartialView("_ListProducts", products);
				}
                return PartialView("_ListProducts", null);
            }
            
            products = await _context.Products.ToPagedListAsync(page, size);
            return PartialView("_ListProducts", products);
        }

        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

			var product = await _context.Products.Include(p => p.Details).Include(p => p.Category).Include(p => p.Promotions).FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

			ViewData["ListDetails"] = await product.Details.ToPagedListAsync(1, 8);
            ViewData["ListPromotions"] = await product.Promotions.ToPagedListAsync(1, 8);
			ViewData["page"] = "products";
			return View(product);
        }

        // GET: Admin/Products/Create
        public IActionResult Create(int? categoryId)
        {
            if(categoryId != null)
            {
				ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", categoryId);
            }
            else
            {
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            }
			ViewData["page"] = "products_create";
			return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Origin,Description,IsActive,CategoryId")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
			ViewData["page"] = "products";
			return View(product);
        }

        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
			ViewData["page"] = "products";
			return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Origin,Description,IsActive,CategoryId")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories.ToList(), "Id", "Name", product.CategoryId);
			
			ViewData["page"] = "products";
			return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.Include(p => p.Details).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                return NotFound();
            }

			if (product.Details!.Any())
			{
				_context.Update(product);
				product.IsActive = false;
				await _context.SaveChangesAsync();
				return PartialView("_Product", product);
			}
			_context.Products.Remove(product);
			_context.SaveChanges();
			return PartialView("_Product", null);
		}

        [HttpPost]
		public async Task<IActionResult> Recovery(int? id)
		{
			if (id == null || _context.Products == null)
			{
				return NotFound();
			}

			var product = await _context.Products.Where(c => c.Id == id).FirstOrDefaultAsync();
			if (product == null)
			{
				return NotFound();
			}

			_context.Update(product);
			product.IsActive = true;
			await _context.SaveChangesAsync();
			return PartialView("_Product",product);
		}

		private bool ProductExists(int id)
        {
          return _context.Products.Any(e => e.Id == id);
        }
    }
}
