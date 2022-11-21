namespace THUD_TN408.Models
{
	public class Cart
	{
		public int Id { get; set; }
		public int Quantity { set; get; } = 1;
		public bool IsDeleted { get; set; } = false;
		public bool IsCheckedOut { get; set; } = false;
		public int ProductDetailId { set; get; }
		public int? OrderId { set; get; }
		public string UserId { set; get; } = null!;
		public virtual User? User { get; set; }
		public virtual ProductDetail? Detail { get; set; }
		public virtual Order? Order { get; set; }
	}
}
