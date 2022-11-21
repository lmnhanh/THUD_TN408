using System.ComponentModel.DataAnnotations;

namespace THUD_TN408.Models
{
	public class Category
	{
		public int Id { get; set; }
		[Required(AllowEmptyStrings = false, ErrorMessage ="Tên danh mục không được trống")]
		[MaxLength(20, ErrorMessage = "Tên danh mục không quá 20 kí tự!")]
		public string Name { get; set; } = null!;
		public bool IsActive { set; get; } = true;
		public virtual ICollection<Product>? Products { get; set; }
	}
}
