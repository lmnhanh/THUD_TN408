namespace THUD_TN408.Models
{
	public class Warehouse
	{
		public int Id { get; set; }
		public string Name { get; set; } = null!;
		public string Address { get; set; } = null!;

		public virtual ICollection<WarehouseDetail>? Details { set; get; }
	}
}
