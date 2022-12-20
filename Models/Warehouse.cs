using System.ComponentModel.DataAnnotations;

namespace THUD_TN408.Models
{
	public class Warehouse
	{
		public int Id { get; set; }
		[Required(AllowEmptyStrings = false, ErrorMessage ="Tên kho không được trống!")]
		[MaxLength(30, ErrorMessage = "Tên kho không quá 30 kí tự!")]
		public string Name { get; set; } = null!;
		[Required(AllowEmptyStrings = false, ErrorMessage = "Địa chỉ kho không được trống!")]
		[MaxLength(100, ErrorMessage = "Địa chỉ không quá 100 kí tự!")]
		public string Address { get; set; } = null!;
		public virtual ICollection<WarehouseDetail>? Details { set; get; }
		public virtual ICollection<ImportNote>? ImportNotes { set; get; }
	}
}
