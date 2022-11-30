using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using THUD_TN408.Areas.Admin.Service;
using THUD_TN408.Data;
using THUD_TN408.Models;
using X.PagedList;
using static THUD_TN408.Authorization.Permissions;

namespace THUD_TN408.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class WarehousesController : Controller
    {
        private readonly TN408DbContext _context;
		private readonly INotyfService _notyf;
		private readonly Services _services;

		public WarehousesController(TN408DbContext context, UserManager<User> userManager, INotyfService notyf)
        {
            _context = context;
			_notyf = notyf;
			_services = new Services(context, userManager);
		}

        // GET: Admin/Warehouses
        public async Task<IActionResult> Index()
        {
			ViewData["page"] = "warehouses";
			return View(await _context.Warehouses.ToPagedListAsync(1, 8));
        }

		public async Task<IActionResult> Paging(int page = 1, int size = 8)
		{
			page = (page < 1) ? 1 : page;
			size = (size < 2) ? 2 : size;
			return PartialView("_ListWarehouses", await _context.Warehouses.ToPagedListAsync(page, size));
		}

		[HttpGet]
        public async Task<IActionResult> DetailStockIndex()
        {
            var warehouses = await _services.GetListWareHouse().ToListAsync();
            var details = await _services.GetListProductDetails(DateTime.Now).ToListAsync();
            var products = await _services.GetListProduct().ToListAsync();

            ViewData["ProductDetail"] = null;
			ViewData["Product"] = products;
			ViewData["Warehouse"] = warehouses;
            ViewData["page"] = "warehouses";
			return View(await _context.WarehouseDetails.Include(w => w.ProductDetail).OrderBy(x => x.ProductDetailId).ToPagedListAsync(1,8));
        }

        [HttpGet]     
        public async Task<IActionResult> FilterPaging(int page = 1, int warehouseId = -1, int productId = -1, int detailId = -1)
        {
            page = (page < 1) ? 1 : page;
			ViewData["ProductDetail"] = await _services.GetListProductDetails(DateTime.Now).ToListAsync();
			ViewData["Product"] = await _services.GetListProduct().ToListAsync();
			ViewData["Warehouse"] = await _services.GetListWareHouse().ToListAsync();

            var result = (IQueryable<WarehouseDetail>)_context.WarehouseDetails;
			if (warehouseId != -1)
            {
                result = result.Where(x => x.WarehouseId == warehouseId);
				if (detailId != -1)
				{
					result = result.Where(x => x.ProductDetailId == detailId);
					if (productId != -1)
					{
						result = result.Where(x => x.ProductDetail!.ProductId == productId);
					}
                }
                else
                {
					if (productId != -1)
					{
						result = result.Where(x => x.ProductDetail!.ProductId == productId);
					}
				}
			}
            else
            {
                if(detailId != -1)
                {
					result = result.Where(x => x.ProductDetailId == detailId);
                    if(productId != -1)
                    {
						result = result.Where(x => x.ProductDetail!.ProductId == productId);
					}
				}
                else
                {
					if (productId != -1)
					{
						result = result.Where(x => x.ProductDetail!.ProductId == productId);
					}
				}
            }
			    
            return PartialView("_ListWarehouseDetails", await result
                .Include(w => w.ProductDetail)
                .OrderBy(x => x.ProductDetailId)
                .ToPagedListAsync(page, 8));
		}

        [HttpGet]
        public async Task<IActionResult> GetSelectListDetails(int productId)
        {
            return PartialView("_SelectListDetails", await _services.GetListProductDetails(productId, DateTime.Now).ToListAsync());
        }

		// GET: Admin/Warehouses/Details/5
		public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Warehouses == null)
            {
                return NotFound();
            }

            var warehouse = await _context.Warehouses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (warehouse == null)
            {
                return NotFound();
            }

			ViewData["page"] = "warehouses";
			return View(warehouse);
        }

        // GET: Admin/Warehouses/Create
        public IActionResult Create()
        {
			ViewData["page"] = "warehouses";
			return View();
        }

        // POST: Admin/Warehouses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Address")] Warehouse warehouse)
        {
            if (ModelState.IsValid)
            {
                _context.Add(warehouse);
                await _context.SaveChangesAsync();
				await _services.AddHistory(User, "Thêm kho \"" + warehouse.Name + "\"", null);
				_notyf.Success("Đã thêm kho " + warehouse.Name);
				return RedirectToAction(nameof(Index));
            }
			ViewData["page"] = "warehouses";
			return View(warehouse);
        }

        // GET: Admin/Warehouses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Warehouses == null)
            {
                return NotFound();
            }

            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null)
            {
                return NotFound();
            }
			ViewData["page"] = "warehouses";
			return View(warehouse);
        }

        // POST: Admin/Warehouses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Address")] Warehouse warehouse)
        {
            if (id != warehouse.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(warehouse);
                    await _context.SaveChangesAsync();
					await _services.AddHistory(User, "Chỉnh sửa thông tin kho \"" + warehouse.Name + "\"", null);
					_notyf.Success("Đã lưu thông tin chỉnh sửa");
				}
                catch (DbUpdateConcurrencyException)
                {
                    if (!WarehouseExists(warehouse.Id))
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
            return View(warehouse);
        }

        // GET: Admin/Warehouses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Warehouses == null)
            {
                return NotFound();
            }

            var warehouse = await _context.Warehouses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (warehouse == null)
            {
                return NotFound();
            }

			ViewData["page"] = "warehouses";
			return View(warehouse);
        }

        // POST: Admin/Warehouses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Warehouses == null)
            {
                return Problem("Entity set 'TN408DbContext.Warehouses'  is null.");
            }
            var warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse != null)
            {
                _context.Warehouses.Remove(warehouse);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WarehouseExists(int id)
        {
          return _context.Warehouses.Any(e => e.Id == id);
        }
    }
}
