using backend.Database;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SealInController : ControllerBase
    {
        ILogger<SealInController> _logger;
        public DatabaseContext Context { get; }

        public SealInController(DatabaseContext context,
        ILogger<SealInController> logger)
        {
            Context = context;
            _logger = logger;
        }
        // GET: api/<SealInController>
        [HttpGet]
        public IActionResult Get([FromQuery] string pIsActive = "",[FromQuery] string pColumnSearch = "", [FromQuery] string searchTerm="", [FromQuery] string pStartDate ="", [FromQuery] string pEndDate="")
        {
            try
            {
                DateTime startDate = DateTime.Now;
                DateTime endDate = DateTime.Now;
                bool vIsActive = false;
                searchTerm =searchTerm.Trim();
                var query = Context.SealIn.AsQueryable();

                if (!string.IsNullOrEmpty(pColumnSearch))
                {

                    switch (pColumnSearch)
                    {
                        case "id":
                            int id = int.Parse(searchTerm);
                            query = query.Where(p => p.Id == id);
                            break;

                        case "sealBetween":
                            query = query.Where(p => p.SealBetween.Contains(searchTerm));
                            break;
                        case "pack":
                            query = query.Where(p => p.Pack.ToString().Contains(searchTerm));
                            break;
                        case "sealNo":
                            query = from s in Context.SealIn
                                    join si in Context.SealItem on s.Id equals si.SealInId into gj
                                    from subSi in gj.DefaultIfEmpty()
                                    where subSi.SealNo == searchTerm
                                    select new SealIn
                                    {
                                        Id =s.Id,
                                        SealBetween = s.SealBetween,
                                        Pack = s.Pack,
                                        IsActive =s.IsActive,
                                        CreatedBy =s.CreatedBy,
                                        UpdatedBy=s.UpdatedBy,
                                        Created=s.Created,
                                        Updated=s.Updated
                                    };
                            break;

                        default:
                            return BadRequest("Invalid search column");
                    }
                    
                }
                else
                {

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
                  
                }

                if(!string.IsNullOrEmpty(pIsActive))
                {
                    if(pIsActive=="1") vIsActive = true;
                    query = query.Where(u=>u.IsActive==vIsActive);
                }
                return Ok(new { result = query.ToList(), message = "request successfully" });
            }
            catch (Exception error)
            {
                _logger.LogError($"Log GetSealIn: {error}");
                return StatusCode(500, new { result = "", message = error });
            }
        }

        [HttpGet("GetSealBetWeen")]
        public IActionResult GetSealBetWeen()
        {
            try
            {
                var result = Context.SealIn.Where(p => p.IsActive == false); //find ยังไม่ได้ใช้งาน

                if (result == null)
                {
                    return NotFound();
                }
                return Ok(new { result = result, message = "request successfully" });
            }
            catch (Exception error)
            {
                _logger.LogError($"Log Get: {error}");
                return StatusCode(500, new { result = "", message = error });
            }
        }
        // GET api/<SealInController>/5
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            try
            {
                var product = Context.SealIn.SingleOrDefault(p => p.Id == id);

                if (product == null)
                {
                    return NotFound();
                }
                return Ok(new { result = product, message = "request successfully" });
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
                foreach (var item in sealIn)
                {
                    // Add a new sealin
                    var newSealIn = new SealIn
                    {
                        SealBetween = item.SealBetween,
                        Pack = item.Pack,
                        IsActive = false,
                        CreatedBy = item.CreatedBy,
                        UpdatedBy = item.UpdatedBy,

                    };
                    Context.SealIn.Add(newSealIn);
                    if (Context.SaveChanges() > 0)
                    {
                        if (item.SealItem != null)
                        {
                            List<SealItem> sealItems = new List<SealItem>();
                            foreach (var sealItem in item.SealItem)
                            {
                                var model = new SealItem
                                {
                                    SealNo = sealItem.SealNo,
                                    SealInId = newSealIn.Id,
                                    Type = 1, //ปกติ
                                    IsUsed = false,
                                    Status = 1, //ซีลใช้งานได้ปกติ
                                    CreatedBy = sealItem.CreatedBy,
                                    UpdaetedBy = sealItem.UpdaetedBy,

                                };
                                sealItems.Add(model);
                            }
                            Context.SealItem.AddRange(sealItems);
                            Context.SaveChanges();
                        }


                    }
                }


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
        public IActionResult Delete(int id)
        {
            try
            {
                var result = Context.SealIn.SingleOrDefault(p => p.Id == id);

                if (result == null)
                {
                    return NotFound();
                }

                Context.SealIn.Remove(result);
                if(Context.SaveChanges()>0)
                {
                   var sealItem = Context.SealItem.Where(p => p.SealInId == id);
                   Context.SealItem.RemoveRange(sealItem);
                   Context.SaveChanges();
                }

                return Ok(new { result = "", message = "delete product sucessfully" });
            }
            catch (Exception error)
            {
                _logger.LogError($"Log DeleteProduct: {error}");
                return StatusCode(500, new { result = "", message = error });
            }
        }
    }
}
