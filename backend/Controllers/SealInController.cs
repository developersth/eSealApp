using System.Runtime.CompilerServices;
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
        public IActionResult Get([FromQuery] string pIsActive = "", [FromQuery] string pColumnSearch = "", [FromQuery] string searchTerm = "", [FromQuery] string pStartDate = "", [FromQuery] string pEndDate = "")
        {
            try
            {
                DateTime startDate = DateTime.Now;
                DateTime endDate = DateTime.Now;
                bool vIsActive = false;
                searchTerm = searchTerm.Trim();
                var query = Context.SealIn.AsQueryable();

                if (!string.IsNullOrEmpty(pColumnSearch))
                {

                    switch (pColumnSearch)
                    {
                        case "sealInId":
                            query = query.Where(p => p.SealInId.Contains(searchTerm));
                            break;

                        case "sealBetween":
                            query = query.Where(p => p.SealBetween.Contains(searchTerm));
                            break;
                        case "pack":
                            query = query.Where(p => p.Pack.ToString().Contains(searchTerm));
                            break;
                        case "sealNo":
                            query = from s in Context.SealIn
                                    join st in Context.SealInInfo on s.SealInId equals st.SealInId
                                    join sim in Context.Seals on st.SealId equals sim.Id
                                    where sim.SealNo == searchTerm
                                    select new SealIn
                                    {
                                        Id = s.Id,
                                        SealInId = s.SealInId,
                                        SealBetween = s.SealBetween,
                                        Pack = s.Pack,
                                        IsActive = s.IsActive,
                                        CreatedBy = s.CreatedBy,
                                        UpdatedBy = s.UpdatedBy,
                                        Created = s.Created,
                                        Updated = s.Updated
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

                if (!string.IsNullOrEmpty(pIsActive))
                {
                    if (pIsActive == "1") vIsActive = true;
                    query = query.Where(u => u.IsActive == vIsActive);
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
                return StatusCode(500, new { result = "", message = error.Message });
            }
        }

        // POST api/<SealInController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RequestSealIn[] request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Invalid request");

                using (var dbtransaction = await this.Context.Database.BeginTransactionAsync())
                {

                    try
                    {
                        int processCount  = 0;
                        foreach (var item in request)
                        {
                            // Add a new sealin
                            var _newSealIn = new SealIn
                            {
                                SealBetween = item.SealBetween,
                                Pack = item.Pack,
                                IsActive = false,
                                CreatedBy = item.CreatedBy,
                                UpdatedBy = item.UpdatedBy,

                            };
                            Context.SealIn.Add(_newSealIn);
                            Context.SaveChanges();
                            //Update SealInId              
                            //int lastId = Context.SealIn.Max(u => u.Id) + 1;
                            //string sealInId = Utilities.GennerateId("SI", _newSealIn.Id);
                            _newSealIn.SealInId = Utilities.GennerateId("SI", _newSealIn.Id);
                            Context.Update(_newSealIn);
                            //var result = Context.SaveChanges();
                            //Context.SaveChanges();
                            if (item.SealList != null)
                            {
                                List<Seals> seals = new List<Seals>();
                                List<SealInInfo> sealInInfo = new List<SealInInfo>();
                                foreach (var sItem in item.SealList)
                                {
                                    var model = new Seals
                                    {
                                        SealNo = sItem.SealNo,
                                        Type = 1, //ปกติ
                                        Status = 1, //ยังไม่ได้ใช้งาน
                                        CreatedBy = sItem.CreatedBy,
                                        UpdatedBy = sItem.UpdatedBy,
                                    };
                                    //sealItems.Add(model);
                                    //seals.Add(model);
                                    Context.Seals.Add(model);
                                    Context.SaveChanges();
                                    var modelInfo = new SealInInfo
                                    {
                                        SealInId = _newSealIn.SealInId,
                                        SealId = model.Id,
                                        SealNo = model.SealNo
                                    };
                                    sealInInfo.Add(modelInfo);

                                }
                                Context.SealInInfo.AddRange(sealInInfo);
                                //await Context.SaveChangesAsync();

                            }
                            processCount ++;

                        }
                        if (processCount  == request.Length)
                        {
                            await this.Context.SaveChangesAsync();
                            await dbtransaction.CommitAsync();
                        }
                        else
                        {
                             await dbtransaction.RollbackAsync();
                        }
                    }
                    catch (Exception error)
                    {
                        await dbtransaction.RollbackAsync();
                        _logger.LogError($"Log Add SealIn: {error}");
                        return StatusCode(500, new { result = "", message = error.Message });
                    }

                }
                return Ok(new { result = request, message = "Create SealIn Successfully" });
            }
            catch (Exception error)
            {
                _logger.LogError($"Log Add SealIn: {error}");
                return StatusCode(500, new { result = "", message = error.Message });
            }
        }
        private async Task<bool> AddSealAndSealInInfo(Seals _seal, string _sealnId)
        {
            bool result = false;
            try
            {
                var _existdata = this.Context.Seals.FirstOrDefault(item => item.SealNo == _seal.SealNo);
                if (_existdata != null)
                {
                    result = false;
                }
                else
                {
                    var _newrecord = new Seals()
                    {
                        SealNo = _seal.SealNo,
                        Type = 1, //ปกติ
                        Status = 1, //ยังไม่ได้ใช้งาน
                        CreatedBy = _seal.CreatedBy,
                        UpdatedBy = _seal.UpdatedBy,
                    };
                    await this.Context.Seals.AddAsync(_newrecord);
                    Context.SaveChanges();
                    var _sealInInfo = new SealInInfo
                    {
                        SealInId = _sealnId,
                        SealId = _newrecord.Id,
                        SealNo = _seal.SealNo,
                        CreatedBy = _seal.CreatedBy,
                        UpdaetedBy = _seal.UpdatedBy,
                    };
                    await this.Context.SealInInfo.AddAsync(_sealInInfo);
                }
                result = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }
        // PUT api/<SealInController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SealInController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(Int32 id)
        {
            try
            {
                var result = Context.SealIn.SingleOrDefault(p => p.Id == id);

                if (result == null)
                {
                    return NotFound();
                }

                if (result != null)
                {
                    var sealInInfo = Context.SealInInfo.Where(p => p.SealInId == result.SealInId).ToList();
                    if (sealInInfo != null)
                    {
                        foreach (var item in sealInInfo)
                        {
                            var resultSealItem = Context.Seals.FirstOrDefault(p => p.Id == item.SealId);
                            Context.Seals.Remove(resultSealItem);
                        }
                        Context.SaveChanges();
                    }
                    var sealInTransaction2 = Context.SealInInfo.Where(p => p.SealInId == result.SealInId);
                    Context.SealInInfo.RemoveRange(sealInTransaction2);
                    Context.SaveChanges();
                }
                Context.SealIn.Remove(result);
                Context.SaveChanges();

                return Ok(new { result = "", message = "delete data sucessfully" });
            }
            catch (Exception error)
            {
                _logger.LogError($"Log DeleteProduct: {error}");
                return StatusCode(500, new { result = "", message = error.Message });
            }
        }
    }
}
