using backend.Models;
namespace backend.Entity
{
      public class RequestSealIn
    {
        public string? SealBetween { get; set; }
        public int? Pack { get; set; }

        public bool? IsActive { get; set; }
        public List<Seals>? SealList { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

    }
}