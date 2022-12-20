using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace THUD_TN408.Areas.Shop.Models
{
	public class Login
	{
		[Required(ErrorMessage = ("Email không được trống"))]
		[EmailAddress]
		public string Email { get; set; } = null!;
		[Required(ErrorMessage = ("Mật khẩu không được trống"))]
		[DataType(DataType.Password)]
		public string Password { get; set; } = null!;
		public bool RememberMe { get; set; }
	}
}
