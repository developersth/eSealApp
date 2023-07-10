using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace backend.Entity
{
    [Table("ReportBroken")]
    public class ReportBroken
    {
        public DateTime? report_date { get; set; }
        public string? truck_name { get; set; }
        public string? sealno_broken { get; }
        public string? sealno_new { get; set; }
        public string? remark { get; set; }
        public string? user_by { get; set; }
    }
        public class ReportRemaining
    {
        public string? ReportDate { get; set; }
        public int? SealStart { get; set; }
        public int? SealIsActive { get; set; }
        public int? SealAdditional { get; set; }
        public int? SealBroken { get; set; }
        public int? SealBalances { get; set; }
    }
    public class ReportSealChange
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
    public class ReportSealOutReceipt
    {
        public string SealOutId { get; set; }
        public DateTime Created { get; set; }
        public string DriverName { get; set; }
        public int? TruckId { get; set; }
        public string? TruckName { get; set; }
        public int? SealTotal { get; set; }
        public int? SealTotalExtra { get; set; }
        public int? SealId { get; set; }
        public string? SealNo { get; set; }
        public int? Pack { get; set; }
        public string? SealBetween { get; set; }
        public string? SealInId { get; set; }
        public string Type { get; set; }
    }
}