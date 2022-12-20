namespace THUD_TN408.Models
{
    public class ImportNote
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public long Total { get; set; } = 0L;
        public int WarehouseId { get; set; }
        public string UserId { get; set; } = null!;
        public virtual Warehouse? Warehouse { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<ImportDetail>? Details { get; set; }
    }
}
