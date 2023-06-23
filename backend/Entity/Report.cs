using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace backend.Entity
{
    [Table("ReportฺBroken")]
    public class ReportฺBroken
    {
        public DateTime? report_date { get; set; }
        public string? truck_name { get; set; }
        public string? sealno_broken { get; }
        public string? sealno_new { get; set; }
        public string? remark { get; set; }
        public string? user_by { get; set; }
    }
    public class ReportฺSealChange
    {
        public int? Id { get; set; }
        public string? TruckName { get; set; }
        public int? SealIdOld { get; set; }
        public string? SealNoOld { get; set; }
        public int? SealIdNew { get; set; }
        public string? SealNoNew { get; set; }
        public string? Remarks { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}