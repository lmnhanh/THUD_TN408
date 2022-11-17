namespace THUD_TN408.Models
{
	public class Cart
	{
		public int Id { get; set; }
		public int Quantity { set; get; }
		public bool IsDeleted { get; set; }
		public bool IsCheckedOut { get; set; }
		public int ProductDetailId { set; get; }
		public int? OrderId { set; get; }
		public virtual User User { get; set; } = null!;
		public virtual ProductDetail Detail { get; set; } = null!;
		public virtual Order? Order { get; set; }
	}
}
