namespace THUD_TN408.Models
{
    public class ImportDetail
    {
        public int ImportNoteId { get; set; }
        public int ProductDetailId { get; set; }
        public int Quantity { get; set; } = 1;
        public virtual ImportNote? ImportNote { get; set; }
        public virtual ProductDetail? ProductDetail { get; set; }
    }
}
