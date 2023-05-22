namespace backend.Models
{     
    public class SealStatus
    {
        public int Id { get; set; } //1=ยังไม่ได้ใช้งาน,2=ใช้งานแล้ว,3=ซีลชำรุด,4=ซีลทดแทน
        public string? Name { get; set; }
    }

}