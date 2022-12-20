using THUD_TN408.Models;

namespace THUD_TN408.Areas.Shop.Models
{
    public class CartsModel
    {
        public List<Cart> Carts { get; set; } = new List<Cart>();
        public long Total { get; set; } = 0L;
		public OrderPromotion? Promotion { get; set; }
		public long Discount { get; set; } = 0L;
		public long TotalFinal { get; set; } = 0L;
	}

}
