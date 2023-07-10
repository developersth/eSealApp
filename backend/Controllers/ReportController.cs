
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
using backend.Services;
using System.Data;
//using AspNetCore.Reporting;
//using Microsoft.Reporting.NETCore;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
//using Microsoft.Reporting.WebForms;
//using Microsoft.Reporting.NETCore;
using System.IO;
using FastReport.Utils;
using FastReport;
using FastReport.Export.Html;
using FastReport.Export.PdfSimple;
namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {

        ILogger<ReportController> _logger;
        public DatabaseContext Context { get; }
        public IConfiguration Configuration { get; }

        public ReportService _reportService;
        private readonly IWebHostEnvironment environment;
        public IWebHostEnvironment _env { get; }
        public ReportController(IWebHostEnvironment Env,
         DatabaseContext context,
        ILogger<ReportController> logger,
        IConfiguration Configuration,
        ReportService _reportService)
        {
            this._env = Env;
            Context = context;
            _logger = logger;
            this.Configuration = Configuration;
            this._reportService = _reportService;
        }
        [HttpGet("GetSealChanges")]
        public async Task<IActionResult> GetSealChanges([FromQuery] string pStartDate = "", [FromQuery] string pEndDate = "")
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


                var result = await _reportService.GetSealChanges(startDate, endDate);
                return Ok(new { result = result, message = "request successfully" });
            }
            catch (Exception error)
            {
                _logger.LogError($"Log GetRemaining: {error.Message}");
                return StatusCode(500, new { result = "", message = error.Message });
            }

        }


        [HttpGet("GenerateReport")]
        public IActionResult GenerateReport()
        {
            // Load the report template
            //string reportPath = Path.Combine("Reports", "your_report_template.frx");
            try
            {
                string reportPath = Path.Combine(_env.WebRootPath, "Reports", "QR-Codes.frx");
                Report report = new Report();
                report.Load(reportPath);


                // Generate the report
                report.Prepare();

                using (MemoryStream ms = new MemoryStream())
                {
                    PDFSimpleExport pdfExport = new PDFSimpleExport();
                    pdfExport.Export(report, ms);
                    ms.Flush();
                    return File(ms.ToArray(), "application/pdf", Path.GetFileNameWithoutExtension("Master-Detail") + ".pdf");
                }
            }
            catch (System.Exception error)
            {
                _logger.LogError($"Log GetRemaining: {error.Message}");
                return StatusCode(500, new { result = error.Message, message = error.Message });
            }

        }

        //[HttpGet("GenReportSealOut/{SealOutId}")]
        //public async Task<IActionResult> GenReportSealOut(string SealOutId)
        //{
        //    string path = Path.Combine(_env.WebRootPath, "Reports", "rptReceipt.rdlc");
        //    try
        //    {

        //        var byteRes = new byte[] { };

        //        //string path = $"{_env.ContentRootPath}/Reports/rptReceipt.rdlc";
        //        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        //        //LocalReport report = new LocalReport(path);
        //        LocalReport report = new LocalReport();
        //        report.ReportPath = path;
        //        DataTable dtSealOutDetail = await _reportService.GetSealOutReceipt(SealOutId);
        //        //DataTable dtDetailExtra = await _reportService.GetSealOutReceiptExtra(SealOutId);
        //        report.DataSources.Add(new ReportDataSource("dtSealOutDetail", dtSealOutDetail));


        //        // Render the report to a byte array
        //        //var result = report.Render("PDF");
        //        byte[] result = report.Render("PDF");
        //        var stream = new MemoryStream(result);
        //        stream.Seek(0, SeekOrigin.Begin);
        //        var response = new FileStreamResult(stream, "application/pdf");
        //        string Filename = "Receipt_" + SealOutId + ".pdf";
        //        return File(response.FileStream, "application/pdf", Filename);

        //        // return Ok(new { result = result, message = "request successfully" });
        //    }
        //    catch (Exception error)
        //    {
        //        _logger.LogError($"Log GetRemaining: {error.Message}");
        //        return StatusCode(500, new { result = path, message = error.Message });
        //    }

        //}
        [HttpGet("ExportSealChanges")]
        public async Task<IActionResult> ExportSealChanges([FromQuery] string pStartDate = "", [FromQuery] string pEndDate = "")
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

            var result = await _reportService.GetSealChanges(startDate, endDate);
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
                foreach (var item in result)
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
                string fileName = $"ReportSealChange_{DateTime.Now.ToString("yyyyMMdd-HH:mm:ss", new CultureInfo("en-us"))}.xlsx";
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                // Return the Excel file as response
                return File(excelBytes, contentType, fileName);

            }
        }
        [HttpGet("GetRemaining")]
        public async Task<IActionResult> GetRemaining([FromQuery] string pStartDate = "", [FromQuery] string pEndDate = "")
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


                var result = await _reportService.GetRemaining(startDate, endDate);
                return Ok(new { result = result, message = "request successfully" });
            }
            catch (Exception error)
            {
                _logger.LogError($"Log GetRemaining: {error}");
                return StatusCode(500, new { result = "", message = error });
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
    }
}