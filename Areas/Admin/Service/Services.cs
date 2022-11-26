using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
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

		public async Task<String> getUserId(ClaimsPrincipal user)
		{
			User currentUser = await _userManager.GetUserAsync(user);
			return currentUser.Id;
		}

		public async Task<IPagedList<Category>>PagingCategories(int page, int size)
        {
            return await _context.Categories.ToPagedListAsync(page, size);
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
        /// Add histori item with provided message and user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="msg"></param>
        /// <param name="url"></param>
        /// <returns>void</returns>
        public async Task addHistory(ClaimsPrincipal user, string msg, string? url)
        {
            History history = new History();
            history.UserId = await getUserId(user);
            history.Message = msg;
            history.TargetUrl = url;
            _context.Histories.Add(history);
            await _context.SaveChangesAsync();
            return;
        }

        /// <summary>
        /// Get IQueryable of product details with full name
        /// </summary>
        /// <returns></returns>
        public IQueryable<ProductDetail> getListProductDetails()
        {
			var details = _context.Details.Include(x => x.Product);
			foreach (var detail in details)
			{
				detail.FullName = detail.Product?.Name + ", " + ((detail.Gender == true) ? "Nam" : "Nữ") + ", " + detail.Size + ", " + detail.Color;
			}
            return details;
		}
    }
}
