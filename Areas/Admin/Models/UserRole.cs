namespace THUD_TN408.Areas.Admin.Models
{
    public class UserRole
    {
		public string UserId { get; set; } = null!;
		public List<Role> Roles { get; set; } = null!;
	}
}
