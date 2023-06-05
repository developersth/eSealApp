namespace backend.Models
{     
    public class SealStatus
    {
        public int Id { get; set; } //1=พร้อมใช้งาน,2=ซีลชำรุด,3=ซีลทดแทน
        public string? Name { get; set; }
    }

}