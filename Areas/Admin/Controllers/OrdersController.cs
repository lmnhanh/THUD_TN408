using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.X509;
using SendGrid.Helpers.Mail;
using THUD_TN408.Areas.Admin.Service;
using THUD_TN408.Data;
using THUD_TN408.Models;
using X.PagedList;
using static THUD_TN408.Authorization.Permissions;

namespace THUD_TN408.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Saleman,SuperAdmin")]
    public class OrdersController : Controller
    {
        private readonly Services _services;
		private readonly INotyfService _notyf;

		public OrdersController(TN408DbContext context, UserManager<User> userManager, INotyfService notyf)
        {
            _services = new Services(context, userManager);
			_notyf = notyf;
        }

        // GET: Admin/Orders
        public async Task<IActionResult> Index(string? userId, int page = 1)
        {
			if (userId != null)
			{
				ViewData["user"] = await _services.GetUser(userId);
				return View("_ListOrders", await _services.GetListOrder(userId).ToPagedListAsync(page, 8));
			}

			ViewData["page"] = "orders";
			var orders = _services.GetListOrder().Result.ToPagedList(page, 8);
            return View(orders);
        }

		[HttpGet]
		public async Task<IActionResult> Paging(string? userId, int page = 1, int size = 8)
		{
			page = (page < 1) ? 1 : page;
			size = (size < 2) ? 2 : size;

			if (userId != null)
			{
				ViewData["user"] = await _services.GetUser(userId);
				return PartialView("_ListOrders", await _services.GetListOrder(userId).ToPagedListAsync(page, size));
			}
			
			return PartialView("_ListOrders", await _services.GetListOrder().Result.ToPagedListAsync(page, size));
		}

		[HttpPost]
		public async Task<IActionResult> SetProcessed(int id)
		{
			var order = await _services.GetOrder(id);
			if(order != null)
			{
				await _services.SetOrderProcessed(order);
				_notyf.Success("Cập nhật thành công!");
			}
			
			return PartialView("_Order", order);
		}

		// GET: Admin/Orders/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null || !_services.HasAnyOrder())
			{
				return NotFound();
			}

			var order = await _services.GetOrder(id);
			if (order == null)
			{
				return NotFound();
			}
			var list = _services.GetListCart(order.Id).Include(x => x.Detail).ThenInclude(x => x.Prices).ToList();
			foreach(var cart in list)
			{
				cart.Detail.Amount = (long)_services.GetPriceAt(cart.Detail?.Prices, order.CreatedDate);
			}
			ViewData["ListDetails"] = list;
			ViewData["page"] = "orders";
			return View(order);
		}

		// GET: Admin/Orders/Create
		public IActionResult Create()
		{
			ViewData["PaymentId"] = new SelectList(_services.GetListPayment().ToList(), "Id", "Name");
			ViewData["ProductPromotionId"] = new SelectList(_services.GetListPPromotion().ToList(), "Id", "Name");
			ViewData["OrderPromotionId"] = new SelectList(_services.GetListOPromotion().ToList(), "Id", "Name");
			ViewData["UserId"] = new SelectList(_services.GetListUser().ToList(), "Id", "Email");
			ViewData["page"] = "orders";
			return View();
		}

		// POST: Admin/Orders/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to.
		// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("Id,CreatedDate,IsPaid,IsTrans,IsSuccess,PaymentId,UserId")] Order order)
		{
			if (ModelState.IsValid)
			{
				await _services.AddOrder(order);
				return RedirectToAction(nameof(Index));
			}
			ViewData["PaymentId"] = new SelectList( _services.GetListPayment().ToList(), "Id", "Name", order.PaymentId);
			ViewData["UserId"] = new SelectList(_services.GetListUser().ToList(), "Id", "Email", order.UserId);
			ViewData["page"] = "orders";
			return View(order);
		}
	}
}
