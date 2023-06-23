using backend.Database;
using backend.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace backend.Services
{
    public class ReportService
    {
        public  DatabaseContext Context { get; }
        private  readonly ILogger<ReportService> _logger;
        public ReportService(DatabaseContext dBContext, ILogger<ReportService> _logger)
        {
            this.Context = dBContext;
            this._logger = _logger;
        }
        public  async Task<List<ReportฺSealChange>> GetSealChanges(DateTime startDate, DateTime endDate)
        {
            var query = from sc in Context.SealChanges
                        join so in Context.SealOut
                        on sc.SealOutId equals so.SealOutId into joinedSealchangesSealout
                        from ju in joinedSealchangesSealout.DefaultIfEmpty()
                        select new ReportฺSealChange
                        {
                            Id = sc.Id,
                            TruckName = ju.TruckName,
                            SealIdOld = sc.SealIdOld,
                            SealNoOld = sc.SealNoOld,
                            SealIdNew = sc.SealIdNew,
                            SealNoNew = sc.SealNoNew,
                            Remarks = sc.Remarks,
                            Created = sc.Created,
                            Updated = sc.Updated,
                            CreatedBy = sc.CreatedBy,
                            UpdatedBy = sc.UpdatedBy
                        };

            query = query.Where(u => u.Created >= startDate && u.Created <= endDate);

            var result = await query.ToListAsync();
           return result;
        }


    }
}