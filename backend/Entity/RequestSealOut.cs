using backend.Models;

namespace backend.Entity
{
    public  class RequestSealOut
    {
        public int? SealTotal { get; set; }
        public int? SealTotalExtra { get; set; }
        public int? TruckId { get; set; }
        public string? TruckName { get; set; }
        public string? SealExtraList {get; set; }
        public List<SealOutInfo> SealOutInfo {get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}