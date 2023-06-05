namespace backend.Models
{

    public partial class SealOutInfoList
    {
        public int id { get; set; }
        public string? SealOutId { get; set; } //ref Sealout id
        public string? SealInId { get; set; } //ref Sealout id
        public int? sealId { get; set; }
        public string? SealNo { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}