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