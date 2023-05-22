using System;
using System.Collections.Generic;

namespace backend.Models
{
    public partial class Drivers
    {
        public int DriverId { get; set; }
        public string? Prefix { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool? IsActive { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}
