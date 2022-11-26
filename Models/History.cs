using System.Drawing.Printing;

namespace THUD_TN408.Models
{
	public class History
	{
		public int Id { get; set; }
		public string Message { get; set; } = null!;
		public string? TargetUrl { get; set; }
		public bool Status { get; set; } = true;
		public DateTime CreatedAt { set; get; } = DateTime.Now;

		public string? UserId {get; set; }
		public User? User { get; set; }
	}
}
