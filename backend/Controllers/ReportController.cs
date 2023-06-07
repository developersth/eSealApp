using System.Globalization;
using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using backend.Database;
using backend.Models;
using backend.Database;
using backend.Models;
using CryptoHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Table;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {

        ILogger<ReportController> _logger;
        public DatabaseContext Context { get; }
        public IConfiguration Configuration { get; }

        public ReportController(DatabaseContext context,
        ILogger<ReportController> logger,
        IConfiguration Configuration)
        {
            Context = context;
            _logger = logger;
            this.Configuration = Configuration;
        }
        [HttpGet("ExportSealChanges")]
        public IActionResult ExportSealChanges([FromQuery] string pStartDate = "", [FromQuery] string pEndDate = "")
        {

            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;
            var query = from sc in Context.SealChanges
                        join so in Context.SealOut
                        on sc.SealOutId equals so.SealOutId into joinedSealchangesSealout
                        from ju in joinedSealchangesSealout.DefaultIfEmpty()
                        select new
                        {
                            ReportDate = sc.Created,
                            TruckName = ju.TruckName,
                            SealNoOld = sc.SealNoOld,
                            SealNoNew = sc.SealNoNew,
                            Remarks = sc.Remarks,
                            Created = sc.Created,
                            CreatedBy = sc.CreatedBy,
                        };
            if (!string.IsNullOrEmpty(pStartDate))
            {
                string[] p = pStartDate.Split('-');
                startDate = new DateTime(Convert.ToInt16(p[0]), Convert.ToInt16(p[1]), Convert.ToInt16(p[2]));
            }
            if (!string.IsNullOrEmpty(pEndDate))
            {
                string[] p = pEndDate.Split('-');
                endDate = new DateTime(Convert.ToInt16(p[0]), Convert.ToInt16(p[1]), Convert.ToInt16(p[2]));
            }
            query = query.Where(u => u.Created >= startDate && u.Created <= endDate);
            // Create Excel package
            using (var package = new ExcelPackage())
            {
                // Create the worksheet
                var worksheet = package.Workbook.Worksheets.Add("รายงานซีลชำรุดประจำวัน");

                // Set header row
                worksheet.Cells[1, 1].Value = "วันที่/เวลา";
                worksheet.Cells[1, 2].Value = "ทะเบียนรถ";
                worksheet.Cells[1, 3].Value = "หมายเลขซีลเสีย";
                worksheet.Cells[1, 4].Value = "หมายเลขซีลใหม่";
                worksheet.Cells[1, 5].Value = "สาเหตุที่เปลี่ยน";
                worksheet.Cells[1, 6].Value = "ผู้จ่าย";
                // Populate data rows
                int row = 2;
                foreach (var item in query)
                {
                    worksheet.Cells[row, 1].Value = item.Created.ToString("yyyy/MM/dd HH:mm:ss", new CultureInfo("en-US"));
                    worksheet.Cells[row, 2].Value = item.TruckName;
                    worksheet.Cells[row, 3].Value = item.SealNoOld;
                    worksheet.Cells[row, 4].Value = item.SealNoNew;
                    worksheet.Cells[row, 5].Value = item.Remarks;
                    worksheet.Cells[row, 6].Value = item.CreatedBy;

                    row++;
                }

                // Auto fit columns
                worksheet.Cells.AutoFitColumns();

                // Convert package to byte array
                byte[] excelBytes = package.GetAsByteArray();

                // Set response headers
                string fileName = $"Report_SealChange_{Guid.NewGuid():N}.xlsx";
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                // Return the Excel file as response
                return File(excelBytes, contentType, fileName);

            }
        }
        [HttpGet("ExportRemaining")]
        public IActionResult ExportRemaining([FromQuery] string pStartDate = "", [FromQuery] string pEndDate = "")
        {

            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now;
            if (!string.IsNullOrEmpty(pStartDate))
            {
                string[] p = pStartDate.Split('-');
                startDate = new DateTime(Convert.ToInt16(p[0]), Convert.ToInt16(p[1]), Convert.ToInt16(p[2]));
            }
            if (!string.IsNullOrEmpty(pEndDate))
            {
                string[] p = pEndDate.Split('-');
                endDate = new DateTime(Convert.ToInt16(p[0]), Convert.ToInt16(p[1]), Convert.ToInt16(p[2]));
            }
            var query = Context.Seals
                        .Where(u => u.Created >= startDate && u.Created <= endDate) // ตรวจสอบว่าฟิลด์ Created.Date เท่ากับวันที่ค้นหา
                        .GroupBy(s => s.Created.Date)
                        .Select(g => new
                        {
                            ReportDate = g.Key.ToString("yyyy/MM/dd", new CultureInfo("en-US")),
                            SealStart = Context.Seals.Count(a => a.Created.Date == g.Key.Date.AddDays(-1) && a.IsActive == false),
                            SealIsActive = g.Count(s => s.IsActive == true),
                            SealAdditional = g.Count(),
                            SealBroken = g.Count(s => s.Status == 2),
                            SealBalances = g.Count(s => s.IsActive == false),
                        });
            // Create Excel package
            using (var package = new ExcelPackage())
            {
                // Create the worksheet
                var worksheet = package.Workbook.Worksheets.Add("รายงานซีลประจำวัน");

                // Set header row
                worksheet.Cells[1, 1].Value = "วันที่";
                worksheet.Cells[1, 2].Value = "จำนวนซีลเริ่มต้น (ตัว)";
                worksheet.Cells[1, 3].Value = "จำนวนซีลเริ่มจ่ายไป (ตัว)";
                worksheet.Cells[1, 4].Value = "จำนวนซีลใส่เพิ่ม (ตัว)";
                worksheet.Cells[1, 5].Value = "จำนวนซีลชำรุด (ตัว)";
                worksheet.Cells[1, 6].Value = "	ยอดคงเหลือ (ตัว)";
                // Populate data rows
                int row = 2;
                foreach (var item in query)
                {
                    worksheet.Cells[row, 1].Value = item.ReportDate;
                    worksheet.Cells[row, 2].Value = item.SealStart;
                    worksheet.Cells[row, 3].Value = item.SealIsActive;
                    worksheet.Cells[row, 4].Value = item.SealAdditional;
                    worksheet.Cells[row, 5].Value = item.SealBroken;
                    worksheet.Cells[row, 6].Value = item.SealBalances;

                    row++;
                }

                // Auto fit columns
                worksheet.Cells.AutoFitColumns();

                // Convert package to byte array
                byte[] excelBytes = package.GetAsByteArray();

                // Set response headers
                string fileName = $"Report_SealRemaining_{Guid.NewGuid():N}.xlsx";
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                // Return the Excel file as response
                return File(excelBytes, contentType, fileName);

            }
        }
        private byte[] ExporttoExcel<T>(List<T> table, string filename)
        {
            using ExcelPackage pack = new ExcelPackage();
            ExcelWorksheet ws = pack.Workbook.Worksheets.Add(filename);
            ws.Cells["A1"].LoadFromCollection(table, true, TableStyles.None);
            return pack.GetAsByteArray();
        }
        [HttpGet("GetRemaining")]
        public IActionResult GetRemaining([FromQuery] string pStartDate = "", [FromQuery] string pEndDate = "")
        {
            try
            {
                DateTime startDate = DateTime.Now;
                DateTime endDate = DateTime.Now;
                if (!string.IsNullOrEmpty(pStartDate))
                {
                    string[] p = pStartDate.Split('-');
                    startDate = new DateTime(Convert.ToInt16(p[0]), Convert.ToInt16(p[1]), Convert.ToInt16(p[2]));
                }
                if (!string.IsNullOrEmpty(pEndDate))
                {
                    string[] p = pEndDate.Split('-');
                    endDate = new DateTime(Convert.ToInt16(p[0]), Convert.ToInt16(p[1]), Convert.ToInt16(p[2]));
                }
                var query = Context.Seals
                            .Where(u => u.Created >= startDate && u.Created <= endDate) // ตรวจสอบว่าฟิลด์ Created.Date เท่ากับวันที่ค้นหา
                            .GroupBy(s => s.Created.Date)
                            .Select(g => new
                            {
                                ReportDate = g.Key.ToString("yyyy/MM/dd", new CultureInfo("en-US")),
                                SealStart = Context.Seals.Count(a => a.Created.Date == g.Key.Date.AddDays(-1) && a.IsActive == false),
                                SealIsActive = g.Count(s => s.IsActive == true),
                                SealAdditional = g.Count(),
                                SealBroken = g.Count(s => s.Status == 2),
                                SealBalances = g.Count(s => s.IsActive == false),
                            });
                if (query == null)
                {
                    return NotFound();
                }


                return Ok(new { result = query, message = "request successfully" });
            }
            catch (Exception error)
            {
                _logger.LogError($"Log GetRemaining: {error}");
                return StatusCode(500, new { result = "", message = error });
            }
        }
        [HttpGet("GetSealChanges")]
        public IActionResult GetSealChanges([FromQuery] string pStartDate = "", [FromQuery] string pEndDate = "")
        {
            try
            {
                DateTime startDate = DateTime.Now;
                DateTime endDate = DateTime.Now;
                var query = from sc in Context.SealChanges
                            join so in Context.SealOut
                            on sc.SealOutId equals so.SealOutId into joinedSealchangesSealout
                            from ju in joinedSealchangesSealout.DefaultIfEmpty()
                            select new
                            {
                                Id = sc.Id,
                                TruckName = ju.TruckName,
                                SealNoOld = sc.SealNoOld,
                                SealNoNew = sc.SealNoNew,
                                Remarks = sc.Remarks,
                                Created = sc.Created,
                                Update = sc.Updated,
                                CreatedBy = sc.CreatedBy,
                                Updatedby = sc.UpdatedBy
                            };
                if (!string.IsNullOrEmpty(pStartDate))
                {
                    string[] p = pStartDate.Split('-');
                    startDate = new DateTime(Convert.ToInt16(p[0]), Convert.ToInt16(p[1]), Convert.ToInt16(p[2]));
                }
                if (!string.IsNullOrEmpty(pEndDate))
                {
                    string[] p = pEndDate.Split('-');
                    endDate = new DateTime(Convert.ToInt16(p[0]), Convert.ToInt16(p[1]), Convert.ToInt16(p[2]));
                }
                query = query.Where(u => u.Created >= startDate && u.Created <= endDate);

                var result = query.ToList();
                return Ok(new { result = result, message = "request successfully" });
            }
            catch (Exception error)
            {
                _logger.LogError($"Log Get User: {error}");
                return StatusCode(500, new { result = "", message = error });
            }
        }
    }
}