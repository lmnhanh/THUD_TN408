using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace THUD_TN408.Areas.Admin.Models
{
    public class ChangePassword
    {
		[Required(ErrorMessage = "Mật khẩu hiện tại không được trống!")]
		[DataType(DataType.Password)]
		public string OldPassword { get; set; } = null!;
		[Required(ErrorMessage = "Mật khẩu mới không được trống!")]
		[StringLength(100, ErrorMessage = "Mật khẩu phải ít nhất {2} kí tự và tối đa {1} kí tự", MinimumLength = 6)]
		[DataType(DataType.Password)]
		public string NewPassword { get; set; } = null!;
		[DataType(DataType.Password)]
		[Required(ErrorMessage = "Xác nhận mật khẩu không được trống!")]
		[Compare("NewPassword", ErrorMessage = "Mật khẩu mới và xác nhận mật khẩu không trùng khớp")]
		public string ConfirmPassword { get; set; } = null!;
	}
}
