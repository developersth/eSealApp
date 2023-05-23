using backend.Database;
using backend.Models;
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
                            join st in Context.SealTypes
                            on s.Type equals st.Id into joinedSealTypes
                            from js in joinedSealTypes.DefaultIfEmpty()
                            join ss in Context.SealStatus
                            on s.Status equals ss.Id into joinedSealStatus
                            from jss in joinedSealStatus.DefaultIfEmpty()
                            select new
                            {
                                Id = s.Id,
                                SealNo = s.SealNo,
                                Type = s.Type,
                                TypeName = js.TypeName,
                                Status = s.Status,
                                StatusName = jss.Name,
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

        [HttpGet("BySealInId/{id}")]
        public ActionResult GetBySealInId(string id)
        {
            try
            {
                var result = from info in Context.SealInInfo
                             join seal in Context.Seals on info.SealId equals seal.Id
                             where info.SealInId == id
                             select new
                             {
                                 Id = seal.Id,
                                 SealNo = seal.SealNo,
                                 Type = seal.Type,
                                 Status = seal.Status,
                                 Created = seal.Created
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
        public async Task<IActionResult> Post([FromBody] SealInTodo[] sealIn)
        {
            try
            {
                return Ok(new { result = sealIn, message = "Create SealIn Successfully" });
            }
            catch (Exception error)
            {
                _logger.LogError($"Log CreateProduct: {error}");
                return StatusCode(500, new { result = "", message = error });
            }
        }

        // PUT api/<SealInController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SealInController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
