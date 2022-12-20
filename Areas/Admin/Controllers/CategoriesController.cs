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
using THUD_TN408.Authorization;
using THUD_TN408.Data;
using THUD_TN408.Models;
using X.PagedList;

namespace THUD_TN408.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Saleman,SuperAdmin,WarehouseManager")]
    public class CategoriesController : Controller
    {
		private readonly Services _services;

        public CategoriesController(TN408DbContext context, UserManager<User> userManager)
        {
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
		public async Task<IActionResult> Details(int? id)
        {
            if (id == null || !_services.HasAnyCategory())
            {
                return NotFound();
            }
			var category = await _services.GetCategory(id);
            if (category == null)
            {
                return NotFound();
            }
            ViewData["ListProducts"] = category.Products.ToPagedList(1, 8);
			ViewData["page"] = "categories";
			return View(category);
        }

		// GET: Admin/Categories/Create
		[Authorize(policy: Permissions.Categories.Create)]
		public IActionResult Create()
        {
			ViewData["page"] = "categories_create";
			return View();
        }

        // POST: Admin/Categories/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(policy: Permissions.Categories.Create)]
		public async Task<IActionResult> Create([Bind("Id,Name,IsActive")] Category category)
		{
			if (ModelState.IsValid)
			{
				await _services.AddCategory(category);
				await _services.AddHistory(User ,"Danh mục \"" + category.Name + "\" đã được thêm vào cơ sở dữ liệu!", "/Admin/Categories/Details/" + category.Id);
				return RedirectToAction(nameof(Index));
			}
			ViewData["page"] = "categories_create";
			return View(category);
		}

		// GET: Admin/Categories/Edit/5
		[Authorize(policy: Permissions.Categories.Edit)]
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || !_services.HasAnyCategory())
            {
                return NotFound();
            }

            var category = await _services.GetCategory(id);
            if (category == null)
            {
                return NotFound();
            }
			ViewData["page"] = "categories";
			return View(category);
        }

        // POST: Admin/Categories/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(policy: Permissions.Categories.Edit)]
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
					await _services.UpdateCategory(category);
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!_services.CategoryExists(category.Id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				await _services.AddHistory(User, "Danh mục \"" + category.Id + "\" đã được chỉnh sửa thông tin!", "/Admin/Categories/Details/" + category.Id);
				return RedirectToAction("Details", "Categories", new {id = category.Id});
			}
			ViewData["page"] = "categories";
			return View(category);
		}

		[HttpPost]
		[Authorize(policy: Permissions.Categories.Delete)]
		public async Task<IActionResult> Delete(int id)
		{
			if (!_services.HasAnyCategory())
			{
				return NotFound();
			}

			var category = await _services.GetCategory(id);
			if (category == null)
			{
				return NotFound();
			}
			if (category.Products!.Any())
			{
				category.IsActive = false;
				await _services.UpdateCategory(category);
				return PartialView("_Category", category);
			}
			await _services.AddHistory(User, "Danh mục \"" + category.Name + "\" đã bị xóa khỏi cơ sở dữ liệu!", null);
			await _services.RemoveCategory(category);
			return PartialView("_Category", null);
		}

		[HttpPost]
		[Authorize(policy: Permissions.Categories.Delete)]
		public async Task<IActionResult> Recovery(int id)
		{
			if (!_services.HasAnyCategory())
			{
				return NotFound();
			}

			var category = await _services.GetCategory(id);
			if (category == null)
			{
				return NotFound();
			}
			category.IsActive = true;
			await _services.UpdateCategory(category);
			return PartialView("_Category", category);
		}
    }
}
