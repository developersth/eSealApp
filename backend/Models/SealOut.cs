using System;
using System.Collections.Generic;

namespace backend.Models
{
    public partial class SealOut
    {
        public Int32 Id { get; set; }
        public int? SealTotal { get; set; }

        public int? SealToTalExtra { get; set; }
        public int? TruckId { get; set; }
        public string? TruckName { get; set; }
        public int? DriverId { get; set; }
        public string? DriverName { get; set; }

        public bool? IsCancel { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
    public partial class SealOutInfo
    {
        public Int32 id { get; set; }
        public Int32 SealOutId { get; set; } //ref Sealout id
        public Int32? SealInId { get; set; } //ref Sealout id
        public string? SealBetween { get; set; }
        public int? Pack { get; set; }
        public int? SealType { get; set; }
        public string? SealTypeName { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
    
    public  class SealOutTodo
    {
        public Int32? SealInId { get; set; }
        public int? SealTotal { get; set; }
        public int? TruckId { get; set; }
        public string? TruckName { get; set; }
        public List<SealOutInfo> SealOutInfo {get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
