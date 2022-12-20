using System.ComponentModel.DataAnnotations;

namespace THUD_TN408.Areas.Admin.Models
{
    public class Create
    {
		[Required(ErrorMessage = "Email không được trống!")]
		[EmailAddress]
		public string Email { get; set; } = null!;

		[Required(ErrorMessage = "Họ lót không được trống!")]
		[MaxLength(25)]
		public string FirstName { get; set; } = null!;

		[Required(ErrorMessage = "Tên không được trống!")]
		[MaxLength(20)]
		public string LastName { get; set; } = null!;
		[Required]
		[StringLength(100, ErrorMessage = "Phải chứa ít nhất {2} kí tự và tối đa {1} kí tự", MinimumLength = 6)]
		[DataType(DataType.Password)]
		public string Password { get; set; } = null!;
	}
}
