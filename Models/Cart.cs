using System.ComponentModel.DataAnnotations.Schema;

namespace THUD_TN408.Models
{
	public class Cart
	{
		public int Id { get; set; }
		public int Quantity { set; get; } = 1;
		public bool IsCheckedOut { get; set; } = false;
		public bool IsCheckOutable { get; set; } = true;
		public int ProductDetailId { set; get; }
		[NotMapped]
		public long? Total { set; get; } = 0L;
		public int? OrderId { set; get; }
		[NotMapped]
		public int? WarehouseId { set; get; }
		public string UserId { set; get; } = null!;
		public virtual User? User { get; set; }
		public virtual ProductDetail? Detail { get; set; }
		public virtual Order? Order { get; set; }
	}
}
