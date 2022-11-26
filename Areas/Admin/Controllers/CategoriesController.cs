using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using THUD_TN408.Areas.Admin.Service;
using THUD_TN408.Data;
using THUD_TN408.Models;
using X.PagedList;

namespace THUD_TN408.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize]
    public class CategoriesController : Controller
    {
        private readonly TN408DbContext _context;
		private readonly Services _services;

        public CategoriesController(TN408DbContext context, UserManager<User> userManager)
        {
            _context = context;
			_services = new Services(context, userManager);
		}

		// GET: Admin/Categories
		public async Task<IActionResult> Index()
        {
            ViewData["page"] = "categories";

			var categories = await _services.PagingCategories(1, 8);
			return View(categories);
        }

		//GET categories with paging
		public async Task<IActionResult> Paging(int page = 1, int size = 8)
		{
			page = (page < 1) ? 1 : page;
			size = (size < 2) ? 2 : size;
			IPagedList<Category> categories = await _services.PagingCategories(page, size);

			return PartialView("_ListCategories", categories);
		}

		// GET: Admin/Categories/Details/5
		public async Task<IActionResult> Details(int? id, int page= 1)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }
			page = (page < 1) ? 1 : page;
			int size = 5;
			var category = await _context.Categories.Include(c => c.Products)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            ViewData["ListProducts"] = category.Products.ToPagedList(page, size);
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
				_context.Categories.Add(category);
				await _context.SaveChangesAsync();
				await _services.addHistory(User, "Danh mục \"" + category.Name + "\" đã được thêm vào hệ thống!", "/Admin/Categories/Details/" + category.Id);
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
				await _services.addHistory(User, "Danh mục \"" + category.Id + "\" đã được chỉnh sửa thông tin!", "/Admin/Categories/Details/" + category.Id);
				return RedirectToAction("Details", "Categories", new {id = category.Id});
			}
			ViewData["page"] = "categories";
			return View(category);
		}

		[HttpPost]
		public async Task<IActionResult> Delete(int id)
		{
			if (_context.Categories == null)
			{
				return NotFound();
			}

			var category = await _context.Categories.Where(c => c.Id == id).Include(c => c.Products).FirstOrDefaultAsync();
			if (category == null)
			{
				return NotFound();
			}
			if (category.Products!.Any())
			{
				_context.Update(category);
				category.IsActive = false;
				await _context.SaveChangesAsync();
				await _services.addHistory(User, "Danh mục \"" + category.Name + "\" đã bị xóa khỏi hệ thống!", null);
				return PartialView("_Category", category);
			}
			_context.Categories.Remove(category);
			await _context.SaveChangesAsync();
			return PartialView("_Category", null);
		}

		[HttpPost]
		public async Task<IActionResult> Recovery(int id)
		{
			if (_context.Categories == null)
			{
				return NotFound();
			}

			var category = await _context.Categories.Where(c => c.Id == id).FirstOrDefaultAsync();
			if (category == null)
			{
				return NotFound();
			}
		
			_context.Update(category);
			category.IsActive = true;
			await _context.SaveChangesAsync();
			return PartialView("_Category", category);
		}

        private bool CategoryExists(int id)
        {
          return _context.Categories.Any(e => e.Id == id);
        }
    }
}
