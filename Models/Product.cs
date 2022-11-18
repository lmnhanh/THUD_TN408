using NuGet.Protocol;
using System.ComponentModel.DataAnnotations;

namespace THUD_TN408.Models
{
	public class Product
	{
		public int Id { get; set; }
		[Required(AllowEmptyStrings = false, ErrorMessage = "Tên sản phẩm không được trống!")]
		public string Name { get; set; } = null!;
		[Required(AllowEmptyStrings = false, ErrorMessage = "Nơi sản xuất không được trống!")]
		public string Origin { get; set; } = null!;
		[Required(AllowEmptyStrings = false, ErrorMessage = "Mô tả về sản phẩm không được trống!")]
		public string Description { get; set; } = null!;
		public bool IsActive { get; set; }

		public virtual ICollection<ProductDetail>? Details { get; set; }
		public int CategoryId { get; set; }
		public virtual Category? Category { get; set; } = null!;
	}
}
