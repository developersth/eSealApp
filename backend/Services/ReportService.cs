using backend.Database;
using backend.Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace backend.Services
{
    public class ReportService
    {
        public DatabaseContext Context { get; }
        private readonly ILogger<ReportService> _logger;
        public ReportService(DatabaseContext dBContext, ILogger<ReportService> _logger)
        {
            this.Context = dBContext;
            this._logger = _logger;
        }

        public async Task<DataTable> GetSealOutReceipt(string SealOutId)
        {
            var result = from so in Context.SealOut
                         join soi in Context.SealOutItem on so.SealOutId equals soi.SealOutId
                         join sim in Context.SealInItem on soi.SealInId equals sim.SealInId
                         join s in Context.Seals on sim.SealId equals s.Id
                         where so.SealOutId == SealOutId
                         select new ReportSealOutReceipt
                         {
                             SealOutId = so.SealOutId,
                             Created = so.Created,
                             DriverName = so.DriverName,
                             TruckId = so.TruckId,
                             TruckName = so.TruckName,
                             SealTotal = so.SealTotal,
                             SealTotalExtra = so.SealTotalExtra,
                             SealId = sim.SealId,
                             SealNo = sim.SealNo,
                             Pack = soi.Pack,
                             SealBetween = soi.SealBetween,
                             SealInId = soi.SealInId,
                             Type = s.Type
                         };

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("SealOutId", typeof(string));
            dataTable.Columns.Add("Created", typeof(DateTime));
            dataTable.Columns.Add("DriverName", typeof(string));
            dataTable.Columns.Add("TruckId", typeof(int));
            dataTable.Columns.Add("TruckName", typeof(string));
            dataTable.Columns.Add("SealTotal", typeof(int));
            dataTable.Columns.Add("SealTotalExtra", typeof(int));
            dataTable.Columns.Add("SealId", typeof(string));
            dataTable.Columns.Add("SealNo", typeof(string));
            dataTable.Columns.Add("Pack", typeof(int));
            dataTable.Columns.Add("SealBetween", typeof(string));
            dataTable.Columns.Add("SealInId", typeof(string));
            dataTable.Columns.Add("Type", typeof(string));

            foreach (var item in result)
            {
                dataTable.Rows.Add(
                    item.SealOutId,
                    item.Created,
                    item.DriverName,
                    item.TruckId,
                    item.TruckName,
                    item.SealTotal,
                    item.SealTotalExtra,
                    item.SealId,
                    item.SealNo,
                    item.Pack,
                    item.SealBetween,
                    item.SealInId,
                    item.Type
                );
            }
          
            return dataTable;
        }
        public async Task<DataTable> GetSealOutReceiptExtra(string SealOutId)
        {
            var result = Context.SealOutExtraItem.Where(a=>a.SealOutId==SealOutId);

            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("SealOutId", typeof(string));
            dataTable.Columns.Add("Created", typeof(DateTime));
            dataTable.Columns.Add("SealId", typeof(string));
            dataTable.Columns.Add("SealNo", typeof(string));

            foreach (var item in result)
            {
                dataTable.Rows.Add(
                    item.SealOutId,
                    item.Created,
                    item.SealId,
                    item.SealNo
                );
            }

            return dataTable;
        }
        public async Task<List<ReportSealChange>> GetSealChanges(DateTime startDate, DateTime endDate)
        {
            var query = from sc in Context.SealChanges
                        join so in Context.SealOut
                        on sc.SealOutId equals so.SealOutId into joinedSealchangesSealout
                        from ju in joinedSealchangesSealout.DefaultIfEmpty()
                        select new ReportSealChange
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