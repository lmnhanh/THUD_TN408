using Microsoft.AspNetCore.Identity;

namespace THUD_TN408.Models
{
    public partial class User : IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public bool Gender { get; set; } = true;
        public string? DateOfBirth { get; set; }
        public string? Address { get; set; }
		public virtual ICollection<Cart>? Carts { get; set; }
		public virtual ICollection<Order>? Orders { get; set; }
	}
}
