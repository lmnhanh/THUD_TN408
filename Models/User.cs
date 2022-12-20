using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace THUD_TN408.Models
{
    public partial class User : IdentityUser
    {
        [Required(AllowEmptyStrings =false, ErrorMessage ="Họ lót không được trống!")]
        [MaxLength(25, ErrorMessage ="Họt lót không quá 25 kí tự!")]
        public string FirstName { get; set; } = null!;
		[Required(AllowEmptyStrings = false, ErrorMessage = "Tên không được trống!")]
		[MaxLength(20, ErrorMessage = "Tên không quá 20 kí tự!")]
		public string LastName { get; set; } = null!;
        public bool Gender { get; set; } = true;
        public DateTime? DateOfBirth { get; set; } = null;
		[MaxLength(100, ErrorMessage = "Địa chỉ không quá 100 kí tự!")]
		public string? Address { get; set; }
		public virtual ICollection<Cart>? Carts { get; set; }
		public virtual ICollection<Order>? Orders { get; set; }
        public virtual ICollection<History>? Histories { get; set; }
        public virtual ICollection<ImportNote>? ImportNotes { get; set; }
	}
}
