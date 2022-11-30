using System.ComponentModel.DataAnnotations;

namespace THUD_TN408.Models
{
	public class OrderPromotion
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
		public string Description { get; set; } = null!;
		[Required(ErrorMessage = "Ngày áp dụng không được trống!")]
		public DateTime ApplyFrom { get; set; } = DateTime.Now;
		[Required(ErrorMessage = "Ngày kết thúc không được trống!")]
		public DateTime ValidTo { get; set; } = DateTime.Now.AddDays(1);
		[Required(ErrorMessage = "Số lượng không được trống!")]
		[Range(minimum: 1, maximum: int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 1!")]
		public int Stock { get; set; } = 1;
		[Range(minimum: 0, maximum: int.MaxValue, ErrorMessage = "Giá trị giảm tối đa không hợp lệ!")]
		public int? MaxDiscount { get; set; } = null;
		[Required(ErrorMessage = "Tỉ lệ giảm không được trống!")]
		[Range(minimum: 1, maximum: 100, ErrorMessage = "Tỉ lệ giảm phải lớn hơn 1% và nhỏ hơn 100%!")]
		public int DiscountPercent { get; set; } = 10;
		[Range(minimum: 0, maximum: int.MaxValue, ErrorMessage = "Điều kiện áp dụng không hợp lệ!")]
		public int? ApplyCondition { get; set; }
		public bool IsActive { get; set; } = true;

		public virtual ICollection<Order>? Orders { get; set; }
	}
}
