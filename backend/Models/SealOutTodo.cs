namespace backend.Models
{
    public  class SealOutTodo
    {
        public int? SealTotal { get; set; }
        public int? SealTotalExtra { get; set; }
        public int? TruckId { get; set; }
        public string? TruckName { get; set; }
        public List<SealOutInfo> SealOutInfo {get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}