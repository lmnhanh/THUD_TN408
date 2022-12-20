using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace THUD_TN408.Models
{
	public class ProductPromotion
	{
		[Required(AllowEmptyStrings = false, ErrorMessage = "Mã khuyến mãi không được trống!")]
		[MaxLength(12, ErrorMessage = "Mã khuyến mãi không quá 12 kí tự")]
		[Key]
		public string Id { get; set; } = null!;
		[Required(AllowEmptyStrings = false, ErrorMessage = "Tên khuyến mãi không được trống!")]
		[MaxLength(30, ErrorMessage = "Tên khuyến mãi không quá 30 kí tự")]
		public string Name { get; set; } = null!;
		[Required(AllowEmptyStrings = false, ErrorMessage = "Mô tả không được trống!")]
		[MaxLength(100, ErrorMessage = "Mô tả không quá 100 kí tự")]
		public string Description { set; get; } = null!;
		[Required(ErrorMessage = "Ngày áp dụng không được trống!")]
		public DateTime ApplyFrom { set; get; } = DateTime.Now;
		[Required(ErrorMessage = "Ngày kết thúc không được trống!")]
		public DateTime ValidTo { set; get; } =	DateTime.Now.AddDays(1);
		[Required(ErrorMessage = "Số lượng không được trống!")]
		[Range(minimum:1, maximum: 1000, ErrorMessage = "Số lượng phải lớn hơn 1 và nhỏ hơn 1000!")]
		public int Stock { get; set; } = 1;
		[Required(ErrorMessage = "Tỉ lệ giảm không được trống!")]
		[Range(minimum: 1, maximum: 100, ErrorMessage = "Tỉ lệ giảm phải lớn hơn 1% và nhỏ hơn 100%!")]
		public int DiscountPercent { set; get; } = 1;
		public bool IsActive { set; get; } = true;
		public int ProductId { set; get; }
		public virtual Product? Product { get; set; }
	}
}
