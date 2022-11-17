namespace THUD_TN408.Models
{
	public class Customer
	{
		public int Id { get; set; }
		public string FirstName { get; set; } = null!;
		public string LastName { get; set; } = null!;
		public DateTime DateOfBirth { get; set; }
		public bool Gender { get; set; }
		public string Email { get; set; } = null!;
		public string PhoneNumber { get; set; } = null!;
		public string? Address { get; set; }

		public virtual ICollection<Cart>? Carts { get; set; }
		public virtual ICollection<Order>? Orders { get; set; }
	}
}
