using System;
using System.Collections.Generic;

namespace backend.Models
{
    public partial class SealOut
    {
        public int Id { get; set; }
        public int? SealTotal { get; set; }
        public int? SealTotalExtra { get; set; }
        public string? SealExtraList { get; set; }
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

}
