namespace backend.Models
{

    public partial class SealOutInfo
    {
        public int id { get; set; }
        public int SealOutId { get; set; } //ref Sealout id
        public string? SealInId { get; set; } //ref Sealout id
        public string? SealBetween { get; set; }
        public int? Pack { get; set; }
        public int? SealType { get; set; }
        public string? SealTypeName { get; set; }
        public string? SealList { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}