namespace backend.Models
{
    public class Seals
    {
        public int Id { get; set; }
        public string? SealNo { get; set; }
        public string? Type { get; set; }
        public int? Status { get; set; }//1=ซีล
        public bool? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
    public class SealOutExtraItem
    {
        public int Id { get; set; }
        public string? SealOutId { get; set; }
        public int? SealId { get; set; }
        public string? SealNo { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
    public class SealStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class SealRemarks
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }

    public class SealChanges
    {
        public int Id { get; set; }
        public string? SealOutId { get; set; }
        public string? SealInId { get; set; }
        public int? SealIdOld { get; set; }
        public string? SealNoOld { get; set; }
        public int? SealIdNew { get; set; }
        public string? SealNoNew { get; set; }
        public int? RemarkId { get; set; }
        public string? Remarks { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }

    public class SealIn
    {
        public int Id { get; set; }
        public string? SealInId { get; set; }
        public string? SealBetween { get; set; }
        public int? Pack { get; set; }
        public bool? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }

    public class SealInItem
    {
        public int Id { get; set; }
        public string? SealInId { get; set; }
        public int? SealId { get; set; }
        public string? SealNo { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }

    public class SealOut
    {
        public int Id { get; set; }
        public string? SealOutId { get; set; }
        public int? SealTotal { get; set; }
        public int? SealTotalExtra { get; set; }
        public int? TruckId { get; set; }
        public string? TruckName { get; set; }
        public int? DriverId { get; set; }
        public string? DriverName { get; set; }
        public bool IsCancel { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }

    public class SealOutItem
    {
        public int Id { get; set; }
        public string? SealOutId { get; set; }
        public string? SealInId { get; set; }
        public string? SealBetween { get; set; }
        public int? Pack { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
    public class Users
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public bool? IsActive { get; set; }
        public int? RoleId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }

    public class Roles
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public partial class Trucks
    {
        public int TruckId { get; set; }
        public string? TruckHead { get; set; }

        public string? TruckTail { get; set; }

        public int? SealTotal { get; set; } // จำนวนซีลของรถ
        public bool? IsActive { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}