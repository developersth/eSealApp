using backend.Database;
using backend.Models;
using backend.Entity;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SealController : ControllerBase
    {
        ILogger<SealController> _logger;
        public DatabaseContext Context { get; }

        public SealController(DatabaseContext context,
        ILogger<SealController> logger)
        {
            Context = context;
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Get([FromQuery] string pStartDate = "", [FromQuery] string pEndDate = "")
        {
            try
            {
                DateTime startDate = DateTime.Now;
                DateTime endDate = DateTime.Now;
                var query = from s in Context.Seals
                            join ss in Context.SealStatus
                            on s.Status equals ss.Id into joinedSealStatus
                            from jss in joinedSealStatus.DefaultIfEmpty()
                            orderby s.Id descending
                            select new
                            {
                                Id = s.Id,
                                SealNo = s.SealNo,
                                Type = s.Type,
                                Status = s.Status,
                                StatusName = jss.Name,
                                IsActive = s.IsActive,
                                IsActiveName =Convert.ToBoolean(s.IsActive) ? "ใช้งานแล้ว" : "ยังไม่ได้ใช้งาน",
                                CreatedBy = s.CreatedBy,
                                UpdatedBy = s.UpdatedBy,
                                Created = s.Created,
                                Updated = s.Updated,
                            };
                if (query == null)
                {
                    return NotFound();
                }
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
                return Ok(new { result = query.ToList(), message = "request successfully" });
            }
            catch (Exception error)
            {
                _logger.LogError($"Log GetSealIn: {error}");
                return StatusCode(500, new { result = "", message = error.Message });
            }
        }
        // [HttpGet("GetTypes")]
        // public IActionResult GetTypes()
        // {
        //     try
        //     {
        //         var result = Context.SealTypes.ToList();
        //         return Ok(new { result = result, message = "request successfully" });
        //     }
        //     catch (Exception error)
        //     {
        //         _logger.LogError($"Log Get User: {error}");
        //         return StatusCode(500, new { result = "", message = error });
        //     }
        // }

        [HttpGet("GetStatus")]
        public IActionResult GetStatus()
        {
            try
            {
                var result = Context.SealStatus.ToList();
                return Ok(new { result = result, message = "request successfully" });
            }
            catch (Exception error)
            {
                _logger.LogError($"Log Get User: {error}");
                return StatusCode(500, new { result = "", message = error });
            }
        }
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            try
            {
                var result = Context.Seals.SingleOrDefault(p => p.Id == id);

                if (result == null)
                {
                    return NotFound();
                }
                return Ok(new { result = result, message = "request successfully" });
            }
            catch (Exception error)
            {
                _logger.LogError($"Log GetSealIn: {error}");
                return StatusCode(500, new { result = "", message = error });
            }
        }

        [HttpGet("GetSealChange")]
        public ActionResult GetSealChange()
        {
            try
            {
                var result = Context.Seals.Where(p => p.Status == 4 && p.Type == "ปกติ" && p.IsActive == false); //ยังไม่ได้ใช้งาน ซีลทดแทน

                if (result == null)
                {
                    return NotFound();
                }
                return Ok(new { result = result, message = "request successfully" });
            }
            catch (Exception error)
            {
                _logger.LogError($"Log GetSealIn: {error}");
                return StatusCode(500, new { result = "", message = error });
            }
        }
        [HttpGet("GetSealStatus")]
        public ActionResult GetSealStatus()
        {
            try
            {
                var result = Context.SealStatus.Where(p => p.Id != 1 && p.Id != 4); //ยังไม่ได้ใช้งาน

                if (result == null)
                {
                    return NotFound();
                }
                return Ok(new { result = result, message = "request successfully" });
            }
            catch (Exception error)
            {
                _logger.LogError($"Log GetSealIn: {error}");
                return StatusCode(500, new { result = "", message = error });
            }
        }
        [HttpGet("GetSealExtra")]
        public ActionResult GetSealExtra()
        {
            try
            {
                var result = Context.Seals.Where(p => p.IsActive == false && p.Type == "พิเศษ"); //ยังไม่ได้ใช้งานและซีลพิเศษ

                if (result == null)
                {
                    return NotFound();
                }
                return Ok(new { result = result.ToList(), message = "request successfully" });
            }
            catch (Exception error)
            {
                _logger.LogError($"Log GetSealIn: {error}");
                return StatusCode(500, new { result = "", message = error });
            }
        }
        [HttpGet("GetSealExtra/{id}")]
        public ActionResult GetSealExtra(int id)
        {
            try
            {
                var result = from s in Context.Seals
                             join ss in Context.SealStatus
                             on s.Status equals ss.Id into joinedSealStatus
                             from jss in joinedSealStatus.DefaultIfEmpty()
                             where s.Id == id
                             && s.Type == "พิเศษ"
                             select new
                             {
                                 Id = s.Id,
                                 SealNo = s.SealNo,
                                 Type = s.Type,
                                 Status = s.Status,
                                 StatusName = jss.Name,
                                 CreatedBy = s.CreatedBy,
                                 UpdatedBy = s.UpdatedBy,
                                 Created = s.Created,
                                 Updated = s.Updated,
                             };

                if (result == null)
                {
                    return NotFound();
                }
                return Ok(new { result = result.ToList(), message = "request successfully" });
            }
            catch (Exception error)
            {
                _logger.LogError($"Log GetSealIn: {error}");
                return StatusCode(500, new { result = "", message = error });
            }
        }

        [HttpGet("BySealInId/{id}")]
        public ActionResult GetBySealInId(string id)
        {
            try
            {
                var result = from info in Context.SealInItem
                             join s in Context.Seals on info.SealId equals s.Id
                             join ss in Context.SealStatus
                             on s.Status equals ss.Id into joinedSealStatus
                             from jss in joinedSealStatus.DefaultIfEmpty()
                             where info.SealInId == id
                             select new
                             {
                                 Id = s.Id,
                                 SealNo = s.SealNo,
                                 Type = s.Type,
                                 Status = s.Status,
                                 StatusName = jss.Name,
                                 CreatedBy = s.CreatedBy,
                                 UpdatedBy = s.UpdatedBy,
                                 Created = s.Created,
                                 Updated = s.Updated,
                             };

                if (result == null)
                {
                    return NotFound();
                }

                return Ok(new { result = result, message = "request successfully" });

            }
            catch (Exception error)
            {
                _logger.LogError($"Log GetSealIn: {error}");
                return StatusCode(500, new { result = "", message = error });
            }
        }

        // POST api/<SealInController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Seals requst)
        {
            try
            {
                var exist = Context.Seals.FirstOrDefault(s => s.SealNo == requst.SealNo);
                if (exist != null)
                {
                    return Ok(new { result = "", success = false, message = "มีหมายเลขซีลนี้ในระบบแล้ว" });
                }
                await Context.Seals.AddAsync(requst);
                await Context.SaveChangesAsync();
                return Ok(new { result = requst, success = true, message = "" });
            }
            catch (Exception error)
            {
                _logger.LogError($"Log Create Seal: {error}");
                return StatusCode(500, new { result = "", message = error.Message });
            }
        }

        // PUT api/<SealInController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Seals requst)
        {
            try
            {
                var result = Context.Seals.FirstOrDefault(s => s.Id == id);
                if (result == null)
                {
                    return Ok(new { result = "", success = false, message = "มีหมายเลขซีลนี้ในระบบแล้ว" });
                }
                result.SealNo = requst.SealNo;
                result.Type = requst.Type;
                result.Status = requst.Status;
                result.IsActive = requst.IsActive;
                result.UpdatedBy = requst.UpdatedBy;
                result.Updated = DateTime.Now;
                Context.Seals.Update(result);
                await Context.SaveChangesAsync();
                return Ok(new { result = requst, success = true, message = "Update Seal Successfully" });
            }
            catch (Exception error)
            {
                _logger.LogError($"Log Update Seal: {error}");
                return StatusCode(500, new { result = "", message = error.Message });
            }
        }

        // DELETE api/<SealInController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSealInfo(int id)
        {
            try
            {
                var result = await Context.Seals.FindAsync(id);
                if (result == null)
                {
                    return NotFound();
                }
                Context.Seals.Remove(result);
                await Context.SaveChangesAsync();
                return Ok(new { result = "", message = "delete successfully" });
            }
            catch (Exception error)
            {

                return StatusCode(500, new { result = "", message = error });
            }
        }
    }
}
