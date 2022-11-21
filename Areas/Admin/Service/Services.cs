using THUD_TN408.Data;

namespace THUD_TN408.Areas.Admin.Service
{
    public class Services
    {
		//private readonly TN408DbContext _context;
        //public Services(TN408DbContext _context)
        //{
        //    this._context = _context;
        //}

		public async Task<string> UploadImage(IFormFile image, string? path = null)
        {
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
    }
}
