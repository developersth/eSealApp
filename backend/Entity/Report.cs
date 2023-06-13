using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace backend.Entity
{
      [Table("ReportฺBroken")]
    public class ReportฺBroken
    {
        public DateTime? report_date { get; set;}
        public string? truck_name { get; set; }
        public string? sealno_broken { get; }
        public string? sealno_new { get; set; }
        public string? remark { get; set; }
        public string?  user_by { get; set; }
    }
}