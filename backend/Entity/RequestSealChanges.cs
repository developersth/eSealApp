namespace backend.Entity
{
    public partial class RequestSealChanges
    {
        public string? SealOutId { get; set; } //ref Sealout id
        public string? SealInId { get; set; } //ref Sealout id
        public int? SealIdOld { get; }
        public string? SealNoOld { get; set; }
        public int? SealIdNew { get; }
        public string? SealNoNew { get; set; }
        public int? RemarkId { get;set; }
        public string? Remarks { get; set; }
        public string? CreatedBy { get;set; }
        public string? UpdatedBy { get;set; }
    }
}