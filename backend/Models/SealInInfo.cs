namespace backend.Models
{
    public class SealInInfo
    {
        public int Id { get; set; }
        public string? SealInId { get; set; } //ref SealIn
        public int? SealId { get; set; }  //ref Seals
        public string? SealNo { get; set; }  
        public string? CreatedBy { get; set; }
        public string? UpdaetedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

    }
}