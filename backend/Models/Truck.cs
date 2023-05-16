using System;
using System.Collections.Generic;

namespace backend.Models
{
    public partial class Truck
    {
        public Int32 TruckId { get; set; }
        public string? TruckHead { get; set; }

        public string? TruckTail { get; set; }

        public int? SealTotal { get; set; } // จำนวนซีลของรถ
        public bool? IsActive { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}
