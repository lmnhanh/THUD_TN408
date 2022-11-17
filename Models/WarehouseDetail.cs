namespace THUD_TN408.Models
{
	public class WarehouseDetail
	{
		public int WarehouseId { get; set; }
		public int ProductDetailId { get; set; }
		public int Stock { get; set; }

		public virtual Warehouse Warehouse { get; set; } = null!;
		public virtual ProductDetail ProductDetail { get; set; } = null!;
	}
}
