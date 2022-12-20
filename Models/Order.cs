using System.ComponentModel.DataAnnotations.Schema;

namespace THUD_TN408.Models
{
	public class Order
	{
		public int Id { get; set; }
		public DateTime CreatedDate { get; set; }
		public bool IsPaid { set; get; } = false;
		public bool IsTrans { set; get; } = false;
		public bool IsSuccess { set; get; } = false;
		public long Total { set; get; } = 0L;
		public int PaymentId { get; set; }
		public int? ShipmentId { get; set; }
		public string UserId { get; set; } = null!;

		public virtual User? User { get; set; }
		public virtual Payment? Payment { get; set; }
		public virtual Shipment? Shipment { get; set; }
		[NotMapped]
		public virtual OrderPromotion? Promotion { get; set; } = null;
		[NotMapped]
		public virtual ProductPromotion? ProductPromotion { get; set; } = null;
		public virtual ICollection<Cart>? Carts { get; set; }
	}
}
