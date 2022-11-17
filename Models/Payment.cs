namespace THUD_TN408.Models
{
	public class Payment
	{
		public int Id { get; set; }
		public string Name { get; set; } = null!;

		public virtual ICollection<Order>? Orders { get; set; }
	}
}
