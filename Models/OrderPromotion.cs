namespace THUD_TN408.Models
{
	public class OrderPromotion
	{
		public string Id { get; set; } = null!;
		public string Name { get; set; } = null!;
		public string Description { get; set; } = null!;
		public DateTime ApplyFrom { get; set; } = DateTime.UtcNow;
		public DateTime ValidTo { get; set; } = DateTime.UtcNow.AddDays(1);
		public int Stock { get; set; } = 1;
		public int? MaxDiscount { get; set; } = null;
		public int DiscountPercent { get; set; } = 10;
		public int? ApplyCondition { get; set; }
		public bool IsActive { get; set; } = true;

		public virtual ICollection<Order>? Orders { get; set; }
	}
}
