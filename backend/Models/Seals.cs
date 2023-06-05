namespace backend.Models
{
      public class Seals
    {
        public int Id { get; set; }
        public string? SealNo { get; set; }
        public int? Type { get; set; } //ref SealType 1=ปกติ, 2=พิเศษ
        public int? Status { get; set; } //1=ใช้งานได้ตามปกติ,2=ซีลชำรุด,3=ซีลทดแทน
         public bool? IsActive { get; set; }  //ใช้งาน
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

    }
}