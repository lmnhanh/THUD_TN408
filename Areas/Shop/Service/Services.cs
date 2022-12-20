using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Security.Claims;
using THUD_TN408.Data;
using THUD_TN408.Models;

namespace THUD_TN408.Areas.Shop.Service
{
    public class Services
    {
		private readonly TN408DbContext _context;
		private readonly UserManager<User> _userManager;
		public Services(TN408DbContext context, UserManager<User> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		public async Task UpdateUser(User user)
		{
			_context.Users.Update(user);
			await _context.SaveChangesAsync();
		}

		public async Task<User> GetUser(ClaimsPrincipal user)
		{
			User currentUser = await _userManager.GetUserAsync(user);
			return currentUser;
		}

		public IQueryable<User> GetListUser()
		{
			return _context.Users;
		}

		public async Task<String> GetUserId(ClaimsPrincipal user)
		{
			User currentUser = await GetUser(user);
			return currentUser.Id;
		}

		public ProductDetail GetFullInfo(ProductDetail detail, DateTime? time)
		{
			var category = GetCategory(detail.Product?.CategoryId).Result?.Name;
			detail.FullName = category + " " + detail.Product?.Name + " " + ((detail.Gender == true) ? "Nam " : "Nữ ") + detail.Size + " (" + detail.Color + ")";
			detail.Name = detail.Product?.Name + " " + ((detail.Gender == true) ? "Nam " : "Nữ ") + " (" + detail.Color + ")";
			if (detail.StockDetails != null)
			{
				detail.Stock = detail.StockDetails.ToList().Sum(x => x.Stock);
			}
			var prices = _context.Prices.Where(pr => pr.ProductDetailId == detail.Id).ToList();
			if (prices.Any())
			{
				detail.Amount = GetPriceAt(prices, time ?? DateTime.Now) ?? 0L;
			}
			return detail;
		}

		public IQueryable<ProductDetail> GetListProductDetails(DateTime? time)
		{
			var details = _context.Details.Include(d => d.Prices)
				.Include(d => d.StockDetails).Include(d => d.Product).ThenInclude(p => p.Promotions);
			foreach (var detail in details.ToList())
			{
				GetFullInfo(detail, time ?? DateTime.Now);
			}
			return details;
		}

		public async Task<List<ProductDetail>>GetListProducDetailsInGroup(int productId, DateTime time)
		{
			var result = _context.Details.Where(d => d.ProductId == productId).ToListAsync();
			return await result;
		}

		public async Task<List<ProductDetail>> GetListSizesWith(int productId, string color)
		{
			var result = _context.Details.Where(d => d.ProductId == productId && d.Color == color).ToListAsync();
			return await result;
		}

		public async Task<List<ProductDetail>> GetListProducDetailsInGroupSameColor(int id, DateTime time)
		{
			var detail = await GetProductDetail(id, time);
			var result = _context.Details.Where(d => d.ProductId == detail.ProductId && d.Color == detail.Color).ToListAsync();
			return await result;
		}

		public async Task<List<ProductDetail>> GetListProductDetailsForShop(DateTime? time)
		{
			var details = _context.Details.Include(d => d.Prices)
				.Include(d => d.StockDetails).Include(d => d.Product).ThenInclude(p => p.Promotions)
				.Include(d => d.Product).ThenInclude(p => p.Category);
			foreach (var detail in details.ToList())
			{
				GetFullInfo(detail, time ?? DateTime.Now);
			}

			List<ProductDetail> result = new List<ProductDetail>();
			foreach (var detail in details.GroupBy(x => new { x.ProductId, x.Color }).Select(x => x.Key).ToList())
			{
				var id = GetProductDetailId(detail.ProductId, detail.Color);
				var d = await GetProductDetail(id, DateTime.Now);
				if (id != -1 && d != null)
				{
					result.Add(d);
				}
			}
			return result;		
		}

		public async Task<ProductDetail?> GetProductDetail(int? id, DateTime? time)
        {
			var detail = await _context.Details.Include(p=> p.StockDetails).Include(p => p.Product)
				.ThenInclude(p => p.Promotions)
				.FirstOrDefaultAsync(d => d.Id == id);
			if(detail != null)
            {
                return GetFullInfo(detail, time ?? DateTime.Now);
			}
            return null;
		}

		public async Task<ProductDetail?> GetProductDetail(string color, string size, DateTime? time)
		{
			var detail = await _context.Details.Include(p => p.StockDetails).Include(p => p.Product)
				.ThenInclude(p => p.Promotions)
				.FirstOrDefaultAsync(d => d.Color == color && d.Size == size);
			if (detail != null)
			{
				return GetFullInfo(detail, time ?? DateTime.Now);
			}
			return null;
		}

		private int GetProductDetailId(int productId, string color)
		{
			var detail = _context.Details.FirstOrDefault(d => d.ProductId == productId && d.Color == color);
			if (detail != null)
			{
				return detail.Id;
			}
			return -1;
		}

		public async Task<Category?> GetCategory(int? categoryId)
		{
			return await _context.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == categoryId);
		}

		public long? GetPriceAt(ICollection<Price> prices, DateTime time)
		{
			Price? price = prices.Where(p => p.ApplyFrom.CompareTo(time) <= 0).OrderByDescending(p => p.ApplyFrom).FirstOrDefault();
			if (price != null)
			{
				return price.Amount;
			}
			return null;
		}

		public async Task<List<Cart>> GetListCart(string userId)
		{
			var carts = await _context.Carts.Where(c => c.UserId == userId && !c.IsCheckedOut)
				.Include(c => c.Detail)
				.Include(c => c.Detail.StockDetails)
				.Include(c => c.Detail).ThenInclude(d => d.Product)
				.Include(c => c.Detail).ThenInclude(d => d.Product!.Promotions).ToListAsync();
			foreach(var cart in carts)
			{
				GetFullInfo(cart.Detail, DateTime.Now);
			}
			return carts;
		}

		public async Task<Cart?> GetCart(string userId, int cartId)
		{
			var cart = (await GetListCart(userId)).Where(c => c.Id == cartId).FirstOrDefault();
			if (cart != null)
				return cart;
			return null;
		}

		public async Task<Cart?> GetCartOfDetail(string userId, int detailId)
		{
			var cart = (await GetListCart(userId)).Where(c => c.ProductDetailId == detailId).FirstOrDefault();
			if (cart != null)
				return cart;
			return null;
		}

		public async Task UpdateCart(Cart cart)
		{
			_context.Carts.Update(cart);
			await _context.SaveChangesAsync();
		}

		public async Task AddCart(Cart cart)
		{
			_context.Carts.Add(cart);
			await _context.SaveChangesAsync();
		}
		public async Task AddCartWithOne(Cart cart)
		{
			cart.Quantity += 1;
			await UpdateCart(cart);
		}

		public async Task RemoveCart(Cart cart)
		{
			_context.Carts.Remove(cart);
			await _context.SaveChangesAsync();
		}

		public async Task<OrderPromotion?> GetPromotion(string id)
		{
			var opromotion = await _context.OPromotions.FirstOrDefaultAsync(o => o.Id.Equals(id));
			return opromotion;
		}

		public async Task UpdatePromotion(OrderPromotion promotion)
		{
			_context.OPromotions.Update(promotion);
			await _context.SaveChangesAsync();
		}
		public async Task UpdatePPromotion(ProductPromotion ppromotion)
		{
			_context.PPromotions.Update(ppromotion);
			await _context.SaveChangesAsync();
		}

		public async Task AddOrder(List<Cart> carts, string userId, long total, int paymentId)
		{
			foreach (var cart in carts)
			{
				cart.IsCheckedOut = true;
				cart.IsCheckOutable = false;
			}
			_context.Carts.UpdateRange(carts);

			Order order = new Order();
			order.UserId = userId;
			order.Total = total;
			order.PaymentId = paymentId;
			order.Carts = carts;
			order.CreatedDate = DateTime.Now;
			if(paymentId == 1 || paymentId == 2)
			{
				order.IsPaid = true;
			}
			_context.Orders.Add(order);
			await _context.SaveChangesAsync();

			await AddHistory("Đơn hàng mới \"" + order.Id +"\"", "/Admin/Orders/Details/" + order.Id);
		}

		public List<Category> GetListCategories()
		{
			return _context.Categories.ToList();
		}

		public async Task AddHistory(string msg, string? url)
		{
			History history = new History();
			history.UserId = null;
			history.Message = msg;
			history.TargetUrl = url;
			_context.Histories.Add(history);
			await _context.SaveChangesAsync();
			return;
		}

		public async Task<List<Order>>GetListOrderOf(string userId)
		{
			return await _context.Orders.Where(o => o.UserId.Equals(userId)).OrderByDescending(x => x.CreatedDate).ToListAsync();
		}

		public List<Producer> GetListProducers()
		{
			return _context.Producers.ToList();
		}

		public async Task<Producer?> GetProducer(int id)
		{
			return await _context.Producers.FirstOrDefaultAsync(x => x.Id == id);
		}

		public async Task<Shipment?> GetShipment(int id)
		{
			return await _context.Shipments.FirstOrDefaultAsync(x => x.Id == id);
		}

		public List<Shipment> GetListShipments()
		{
			return _context.Shipments.ToList();
		}
	}
}
