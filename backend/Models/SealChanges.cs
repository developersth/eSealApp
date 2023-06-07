namespace backend.Models
{

    public partial class SealChanges
    {
        public int Id { get; set; }
        public string? SealOutId { get; set; } //ref Sealout id
        public string? SealInId { get; set; } //ref Sealout id
        public int? SealIdOld { get; }
        public string? SealNoOld { get; set; }
        public int? SealIdNew { get; }
        public string? SealNoNew { get; set; }
        public string? Remarks { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}