namespace backend.Models
{

    public partial class SealChanges
    {
        public int id { get; set; }
        public string? SealOutId { get; set; } //ref Sealout id
        public string? SealInId { get; set; } //ref Sealout id
        public string? SealNoOld { get; set; }
        public string? SealNoNew { get; set; }
        public string? Remarks { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}