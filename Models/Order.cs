namespace THUD_TN408.Models
{
	public class Order
	{
		public int Id { get; set; }
		public DateTime CreatedDate { get; set; }
		public bool IsPaid { set; get; } = false;
		public bool IsTrans { set; get; } = false;
		public bool IsSuccess { set; get; } = false;
		public int PaymentId { get; set; }
		public string UserId { get; set; } = null!;
		public virtual User User { get; set; } = null!;
		public virtual Payment Payment { get; set; } = null!;
		public virtual ICollection<Cart>? Carts { get; set; }
	}
}
