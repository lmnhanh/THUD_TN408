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
    [Area("Admin") ,Authorize(Roles = "Saleman,SuperAdmin,WarehouseManager")]
    public class PricesController : Controller
    {
        private readonly TN408DbContext _context;

        public PricesController(TN408DbContext context)
        {
            _context = context;
        }

        // GET: Admin/Prices
        public async Task<IActionResult> Index(int? productDetailId)
        {
            var prices = _context.Prices.Include(p => p.Detail);
            if(productDetailId != null)
            {
				return View(await prices.Where(p => p.ProductDetailId == productDetailId).OrderByDescending(p => p.ApplyFrom).ToListAsync());
			}
            return View(await prices.OrderByDescending(p => p.ApplyFrom).ToListAsync());
        }
       
        private bool PriceExists(int id)
        {
          return _context.Prices.Any(e => e.Id == id);
        }
    }
}
