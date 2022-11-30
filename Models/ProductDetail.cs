using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace THUD_TN408.Models
{
	public class ProductDetail
	{
		public int Id { get; set; }
		[Required(AllowEmptyStrings = false, ErrorMessage = "Kích cở sản phẩm không được trống!")]
		[MaxLength(6, ErrorMessage = "Kích cỡ không quá 6 kí tự!")]
		public string Size { set; get; } = null!;

		[MaxLength(20, ErrorMessage = "Màu sắc không quá 20 kí tự!")]
		public string? Color { set; get; }
		public bool Gender { set; get; } = true;
		[NotMapped]
		[Required(ErrorMessage = "Giá không được trống!")]
		[Range(minimum: '1', maximum: long.MaxValue, ErrorMessage = "Giá phải lớn hơn 0!")]
		public long Amount { set; get; } = 100000;
		[NotMapped]
		public int Stock { set; get; }
		[NotMapped]
		public string? FullName { set; get; } 
		public string? Image1 { set; get; }
		public string? Image2 { set; get; }

		public int ProductId { get; set; }
		public virtual Product? Product { get; set; } = null!;
		public virtual ICollection<Cart>? Carts { get; set; }
		public virtual ICollection<WarehouseDetail>? StockDetails { get; set; }
		public virtual ICollection<Price>? Prices { get; set; }
	}
}
