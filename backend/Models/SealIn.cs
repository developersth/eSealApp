using System.Globalization;

namespace backend.Models
{

    public class SealIn
    {
        public int Id { get; set; }
        public string? SealInId { get; set; } //รหัสซีล Genarations
        public string? SealBetween { get; set; }
        public int? Pack { get; set; }
        public bool? IsActive { get; set; }  //เอาไว้ใข้กร
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }

}
