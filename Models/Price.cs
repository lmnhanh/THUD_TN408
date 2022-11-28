namespace THUD_TN408.Models
{
	public class Price
	{
		public int Id { set; get; }
		public long Amount { set; get; }
		public DateTime ApplyFrom { set; get; } = DateTime.Now;
		public int ProductDetailId { set; get; }
		public virtual ProductDetail? Detail { set; get; }
	}
}
