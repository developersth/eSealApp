using System.Globalization;

namespace backend.Models
{
    public class SealIn
    {

        public Int32 Id { get; set; }
        public string? SealBetween { get; set; }
        public int? Pack { get; set; }
        public bool? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
    public class SealItem
    {
        public Int32 Id { get; set; }
        public string? SealNo { get; set; }
        public Int32 SealInId { get; set; }  //ref SealIn.id
        public int? Type { get; set; } //ref SealType 1=ปกติ, 2=พิเศษ
        public bool? IsUsed { get; set; }
        public int? Status { get; set; } //1=ซีลใช้งานได้ปกติ,2=ซีลชำรุด,3=ซีลทดแทน
        public string? CreatedBy { get; set; }
        public string? UpdaetedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

    }

    //model for from body
    public class SealInTodo
    {
        public string? SealBetween { get; set; }
        public int? Pack { get; set; }

        public bool? IsActive { get; set; }

        public List<SealItem>? SealItem { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

    }
    public class SealType
    {
        public int Id { get; set; }
        public string? TypeName { get; set; }
    }
    public class SealStatus
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
