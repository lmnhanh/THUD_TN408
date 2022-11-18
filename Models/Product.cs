using NuGet.Protocol;

namespace THUD_TN408.Models
{
	public class Product
	{
		public int Id { get; set; }
		public string Name { get; set; } = null!;
		public string Origin { get; set; } = null!;
		public string Description { get; set; } = null!;
		public bool IsActive { get; set; }

		public virtual ICollection<ProductDetail>? Details { get; set; }
		public int CategoryId { get; set; }
		public virtual Category? Category { get; set; } = null!;
	}
}
