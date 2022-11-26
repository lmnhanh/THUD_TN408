using NuGet.Protocol;
using System.ComponentModel.DataAnnotations;

namespace THUD_TN408.Models
{
	public class Product
	{
		public int Id { get; set; }
		[Required(AllowEmptyStrings = false, ErrorMessage = "Tên sản phẩm không được trống!")]
		[MaxLength(30,ErrorMessage ="Tên sản phẩm không quá 30 kí tự!")]
		public string Name { get; set; } = null!;
		[Required(AllowEmptyStrings = false, ErrorMessage = "Nơi sản xuất không được trống!")]
		[MaxLength(15, ErrorMessage = "Nơi sản xuất không quá 15 kí tự!")]
		public string Origin { get; set; } = null!;
		[Required(AllowEmptyStrings = false, ErrorMessage = "Mô tả về sản phẩm không được trống!")]
		[MaxLength(500,ErrorMessage = "Mô tả không quá 500 kí tự!")]
		public string Description { get; set; } = null!;
		public bool IsActive { get; set; } = true;

		public virtual ICollection<ProductDetail>? Details { get; set; }
		public int CategoryId { get; set; }
		public virtual Category? Category { get; set; } = null!;
		public virtual ICollection<ProductPromotion>? Promotions { get; set; }
	}
}
