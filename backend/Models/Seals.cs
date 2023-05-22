namespace backend.Models
{
      public class Seals
    {
        public int Id { get; set; }
        public string? SealNo { get; set; }
        public int? Type { get; set; } //ref SealType 1=ปกติ, 2=พิเศษ
        public int? Status { get; set; } //1=ยังไม่ได้ใช้งาน,2=ใช้งานแล้ว,3=ซีลชำรุด,4=ซีลทดแทน
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

    }
}