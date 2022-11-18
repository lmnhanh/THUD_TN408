namespace THUD_TN408.Models
{
	public class ProductDetail
	{
		public int Id { get; set; }

		public string Size { set; get; } = null!;
		public string? Color { set; get; }
		public bool Gender { set; get; } = true;
		public string? Image1 { set; get; }
		public string? Image2 { set; get; }

		public int ProductId { get; set; }
		public virtual Product? Product { get; set; } = null!;
		public virtual ICollection<Cart>? Carts { get; set; }
		public virtual ICollection<WarehouseDetail>? StockDetails { get; set; }
		public virtual ICollection<Price>? Prices { get; set; }
	}
}
