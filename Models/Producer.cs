using System.ComponentModel.DataAnnotations;

namespace THUD_TN408.Models
{
	public class Producer
	{
		public int Id { get; set; }
		[Required(AllowEmptyStrings = false, ErrorMessage = "Tên nhà sản xuất không được trống")]
		public string Name { get; set; } = null!;
		public string? PhoneNumber { get; set; }
		public string? Email { get; set; }

		public virtual ICollection<Product>? Products { get; set; }
	}
}
