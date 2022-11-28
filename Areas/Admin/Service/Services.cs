using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Drawing;
using System.Security.Claims;
using THUD_TN408.Data;
using THUD_TN408.Models;
using X.PagedList;
using static System.Net.Mime.MediaTypeNames;

namespace THUD_TN408.Areas.Admin.Service
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

        public async Task<String> GetUserId(ClaimsPrincipal user)
        {
            User currentUser = await _userManager.GetUserAsync(user);
            return currentUser.Id;
        }

		public async Task<string?> UploadImage(IFormFile image, string? path = null)
        {

			string[] permittedExtensions = { ".jpg", ".png" };
            var ext = Path.GetExtension("\\"+image.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                return null;
            }
			string randomName = Path.GetRandomFileName();
            string fileName = randomName.Substring(0, randomName.Length - 4) + image.FileName;

			if (path == null)
            {
                path = Path.Combine("wwwroot\\images\\products", fileName);
            }
            else { path = Path.Combine(path, fileName); }

			using (var stream = System.IO.File.Create(path))
			{
				await image.CopyToAsync(stream);
			}
            return fileName;
		}

        public void DeleteImage(string? fileName)
        {
            if(fileName != null)
                File.Delete(Path.Combine("wwwroot\\images\\products", fileName));
            return;
        }

        /// <summary>
        /// Add history item with provided message and user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="msg"></param>
        /// <param name="url"></param>
        /// <returns>void</returns>
        public async Task AddHistory(ClaimsPrincipal user, string msg, string? url)
        {
            History history = new History();
            history.UserId = await GetUserId(user);
            history.Message = msg;
            history.TargetUrl = url;
            _context.Histories.Add(history);
            await _context.SaveChangesAsync();
            return;
        }

		//======================Category===============

        /// <summary>
        /// Check if Categories is empty or not
        /// </summary>
        /// <returns></returns>
        public bool HasAnyCategory()
        {
            return _context.Categories != null && _context.Categories.Any();
        }

        /// <summary>
        /// Get a category with provided id
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public async Task<Category?> GetCategory(int? categoryId)
        {
            return await _context.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.Id == categoryId);
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        /// <returns></returns>
        public IQueryable<Category> GetListCategory()
        {
            return _context.Categories.Include(c => c.Products);
        }

        /// <summary>
        /// Get categories with paging
        /// </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
		public async Task<IPagedList<Category>> PagingCategories(int page, int size)
		{
			return await GetListCategory().ToPagedListAsync(page, size);
		}

        /// <summary>
        /// Add new category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public async Task AddCategory(Category category)
        {
            _context.Categories.Add(category);
            await SaveChangesAsync();
        }

        /// <summary>
        /// Update a category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
		public async Task UpdateCategory(Category category)
		{
			_context.Categories.Update(category);
			await SaveChangesAsync();
		}

		/// <summary>
		/// Remove a category
		/// </summary>
		/// <param name="category"></param>
		/// <returns></returns>
		public async Task RemoveCategory(Category category)
		{
			_context.Categories.Remove(category);
			await SaveChangesAsync();
		}

        /// <summary>
        /// Check if category with id is exist or not
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public bool CategoryExists(int id)
		{
			return _context.Categories.Any(e => e.Id == id);
		}

		//======================ProductDetail===============

		/// <summary>
		/// Check if ProductsDetails is empty or not
		/// </summary>
		/// <returns></returns>
		public bool HasAnyProductDetail()
        {
            return _context.Details != null && _context.Details.Any();
        }

        public ProductDetail GetFullInfo(ProductDetail detail, DateTime? time)
        {			
            detail.FullName = detail.Product?.Name + ", " + ((detail.Gender == true) ? "Nam" : "Nữ") + ", " + detail.Size + ", " + detail.Color;
			if (detail.StockDetails != null)
			{
				detail.Stock = detail.StockDetails.ToList().Sum(x => x.Stock);
			}
			if (detail.Prices != null)
			{
				detail.Amount = GetPriceAt(detail.Prices, time ?? DateTime.Now) ?? 0L;
			}
			return detail;
		}

        /// <summary>
        /// Get all infomation of a ProductDetail at the time provided 
        /// </summary>
        /// <param name="id">Id</param>
        /// <param name="time">The time price applied at</param>
        /// <returns></returns>
        public async Task<ProductDetail?> GetProductDetail(int? id, DateTime? time)
        {
			var detail = await _context.Details.Include(p => p.Product).Include(d => d.Prices)
                .Include(p => p.StockDetails).Include(p => p.Carts)
			   .FirstOrDefaultAsync(m => m.Id == id);
			if(detail != null)
            {
                return GetFullInfo(detail, time ?? DateTime.Now);
			}
            return null;
		}

        /// <summary>
        /// Get IQueryable of ProductDetail with full name
        /// </summary>
        /// <returns></returns>
        public IQueryable<ProductDetail> GetListProductDetails(DateTime? time)
        {
            var details = _context.Details.Include(d => d.Product).Include(d => d.Prices)
                .Include(d => d.StockDetails);
			foreach (var detail in details)
			{
				GetFullInfo(detail, time ?? DateTime.Now);
			}
            return details;
		}

        /// <summary>
        /// Get IQueryable ProductDetail of provided productId
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="time"></param>
        /// <returns></returns>
		public IQueryable<ProductDetail> GetListProductDetails(int productId,DateTime? time)
		{
            var details = GetListProductDetails(time ?? DateTime.Now).Where(d => d.ProductId == productId);
			return details;
		}

        /// <summary>
        /// Add a ProductDetail
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        public async Task AddProductDetail(ProductDetail detail)
        {
            _context.Details.Add(detail);
            await SaveChangesAsync();
        }

        /// <summary>
        /// Update infomation of ProductDetail
        /// </summary>
        /// <param name="detail">Old ProductDetail</param>
        /// <param name="newDetail">New information</param>
        /// <param name="Image1">New image</param>
        /// <param name="Image2">New image</param>
        /// <returns></returns>
        public async Task UpdateProductDetail(ProductDetail detail, ProductDetail newDetail, IFormFile? Image1, IFormFile? Image2) {
			_context.Details.Update(detail);
            if (Image1 != null && Image1.Length > 0)
            {
				string? fname = await UploadImage(Image1);
				if (fname != null)
				{
					DeleteImage(detail.Image1);
                    detail.Image1 = fname;
                }
            }
            if (Image2 != null && Image2.Length > 0)
			{
				string? fname = await UploadImage(Image2);
				if (fname != null)
				{
					DeleteImage(detail.Image2);
					detail.Image2 = fname;
				}
			}
			detail.Size = newDetail.Size;
			detail.Color = newDetail.Color;
			detail.Gender = newDetail.Gender;
			detail.ProductId = newDetail.ProductId;
			await SaveChangesAsync();
            if (newDetail.Amount != detail.Amount)
            {
                await AddPrice(detail.Id, newDetail.Amount, DateTime.Now);
            }
		}

        /// <summary>
        /// Remove a details of product
        /// </summary>
        /// <param name="detail"></param>
        /// <returns></returns>
        public async Task RemoveProductDetail(ProductDetail detail)
        {
			_context.Details.Remove(detail);
			await SaveChangesAsync();
		}

        /// <summary>
        /// Check if a detail of product exists or not
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public bool ProductDetailExists(int id)
		{
			return _context.Details.Any(e => e.Id == id);
		}

		//======================Product===============

		/// <summary>
		/// Check if Products is empty or not
		/// </summary>
		/// <returns></returns>
		public bool HasAnyProduct()
		{
			return _context.Products != null && _context.Products.Any();
		}

		/// <summary>
		/// Get a Product of provided id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public async Task<Product?> GetProduct(int? id)
		{
			var product = await _context.Products.Include(p => p.Details).Include(p => p.Category)
                .Include(p => p.Promotions).FirstOrDefaultAsync(m => m.Id == id);
            return product;
		}


        /// <summary>
        /// Get IQueryable of all products
        /// </summary>
        /// <returns></returns>
        public IQueryable<Product> GetListProduct()
        {
            var products = _context.Products.Include(p => p.Details).Include(p => p.Category)
                .Include(p => p.Promotions);
			return products;
		}

        /// <summary>
        /// Get IQueryable of products with provided categoryId
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public IQueryable<Product> GetListProduct(int? categoryId)
        {
            var products = GetListProduct().Where(p => p.CategoryId == categoryId);
            return products;
        }

        /// <summary>
        /// Add new product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public async Task AddProduct(Product product)
        {
			_context.Add(product);
			await SaveChangesAsync();
		}

        /// <summary>
        /// Update a product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
		public async Task UpdateProduct(Product product)
		{
			_context.Update(product);
			await SaveChangesAsync();
		}

        /// <summary>
        /// Remove a product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
		public async Task RemoveProduct(Product product)
		{
			_context.Remove(product);
			await SaveChangesAsync();
		}

        /// <summary>
        /// Check if product exists or not
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
		public bool ProductExists(int id)
		{
			return _context.Products.Any(e => e.Id == id);
		}

		//======================ProductPromotion===============

        /// <summary>
        /// Check if ProductPromotion is emty or not
        /// </summary>
        /// <returns></returns>
        public bool HasAnyPPromotion()
        {
            return _context.PPromotions != null && _context.PPromotions.Any();
        }


        /// <summary>
        /// Get a ProductPromotion with provided id
        /// </summary>
        /// <param name="promotionId"></param>
        /// <returns></returns>
        public async Task<ProductPromotion?> GetProductPromotion(string promotionId) {
            return await GetListPPromotion().FirstOrDefaultAsync(pp => pp.Id.Equals(promotionId));
        }

		/// <summary>
		/// Get all ProductPromotion
		/// </summary>
		/// <returns></returns>
		public IQueryable<ProductPromotion> GetListPPromotion()
        {
            return _context.PPromotions.Include(p => p.Product);
        }

        /// <summary>
        /// Get all ProductPromotion of provided productId
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
		public async Task<ICollection<ProductPromotion>?> GetListPPromotion(int productId)
		{
			return (await GetProduct(productId))?.Promotions;
		}

		/// <summary>
		/// Add new product promotion
		/// </summary>
		/// <param name="promotion"></param>
		/// <returns></returns>
		public async Task AddProductPromotion(ProductPromotion promotion)
		{
			_context.PPromotions.Add(promotion);
			await SaveChangesAsync();
		}

		/// <summary>
		/// Update a product promotion
		/// </summary>
		/// <param name="promotion"></param>
		/// <returns></returns>
		public async Task UpdateProductPromotion(ProductPromotion promotion)
		{
			_context.PPromotions.Update(promotion);
			await SaveChangesAsync();
		}

		/// <summary>
		/// Remove a category
		/// </summary>
		/// <param name="promotion"></param>
		/// <returns></returns>
		public async Task RemoveProductPromotion(ProductPromotion promotion)
		{
			_context.PPromotions.Remove(promotion);
			await SaveChangesAsync();
		}

		/// <summary>
		/// Check if ProductPromotion exists or not
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public bool ProductPromotionExists(string id)
		{
			return _context.PPromotions.Any(e => e.Id == id);
		}


		//======================Price===============

		/// <summary>
		/// Add the current price of specific ProductDetail
		/// </summary>
		/// <param name="ProductDetailId">Id of your ProductDetail</param>
		/// <param name="Amount">Amount of price in VND</param>
		/// <param name="ApplyFrom">The date this price will be applied</param>
		/// <returns></returns>
		public async Task AddPrice(int ProductDetailId, long Amount, DateTime ApplyFrom)
        {
            try
            {
                _context.Prices.Add(new Price() { Amount = Amount, ApplyFrom = ApplyFrom, ProductDetailId = ProductDetailId});
                await _context.SaveChangesAsync();
                return;
            }
            catch { }
        }

        /// <summary>
        /// Get the price applied at the provided time
        /// </summary>
        /// <param name="prices">ICollection of Price</param>
        /// <param name="time">The time you want to get the price at</param>
        /// <returns></returns>
        public long? GetPriceAt(ICollection<Price> prices, DateTime time)
        {
            Price? price = prices.Where(p => p.ApplyFrom.CompareTo(time) <= 0).OrderByDescending(p => p.ApplyFrom).FirstOrDefault();
            if (price != null)
            {
                return price.Amount;
            }
            return null;
        }
		


        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
