using System.ComponentModel.DataAnnotations;

namespace THUD_TN408.Models
{
	public class Shipment
	{
		public int Id { get; set; }
		[Required(AllowEmptyStrings = false, ErrorMessage = "Tên đơn vị vận chuyển không được trống")]
		public string Name { get; set; } = null!;

		public ICollection<Order>? Orders { get; set; }
	}
}
