namespace THUD_TN408.Models
{
	public class Category
	{
		public int Id { get; set; }
		public string Name { get; set; } = null!;
		public bool? IsActive { set; get; } = true;
		public virtual ICollection<Product>? Products { get; set; }
	}
}
