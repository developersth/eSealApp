using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using backend.Database;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SealOutController : ControllerBase
    {
        ILogger<SealOutController> _logger;
        DatabaseContext Context;

        public IWebHostEnvironment Env { get; }

        public SealOutController(
            IWebHostEnvironment Env,
            ILogger<SealOutController> logger, DatabaseContext context)
        {
            this.Env = Env;
            _logger = logger;
            Context = context;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] string pIsActive = "", [FromQuery] string pColumnSearch = "", [FromQuery] string searchTerm = "", [FromQuery] string pStartDate = "", [FromQuery] string pEndDate = "")
        {
            try
            {
                DateTime startDate = DateTime.Now;
                DateTime endDate = DateTime.Now;
                bool vIsActive = false;
                searchTerm = searchTerm.Trim();
                var query = Context.SealOut.AsQueryable();

                if (!string.IsNullOrEmpty(pColumnSearch))
                {

                    switch (pColumnSearch)
                    {
                        case "id":
                            int id = int.Parse(searchTerm);
                            query = query.Where(p => p.Id == id);
                            break;

                        case "sealBetween":
                            //find id SealOut
                            var sealOutInfo = Context.SealOutInfo.SingleOrDefault(a => a.SealBetween == searchTerm);
                            if (sealOutInfo != null)
                            {
                                Int32 sealOutId = sealOutInfo.SealOutId;
                                query = query.Where(p => p.Id.ToString().Contains(sealOutId.ToString()));
                            }
                            break;
                        case "sealNo":
                            //find sealInId ก่อน
                            var sealItem = Context.SealItem.SingleOrDefault(a => a.SealNo == searchTerm);
                            if (sealItem != null)
                            {
                                Int32 sealInId = sealItem.SealInId;
                                var info = Context.SealOutInfo.SingleOrDefault(a => a.SealInId == sealItem.SealInId);
                                query = query.Where(p => p.Id.ToString().Contains(info.SealOutId.ToString()));
                            }
                            break;
                        case "TruckName":
                            query = query.Where(p => p.TruckName == searchTerm);
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
                return Ok(new { result = query.ToList(), message = "request successfully" });
            }
            catch (Exception error)
            {
                _logger.LogError($"Log GetSealIn: {error}");
                return StatusCode(500, new { result = "", message = error });
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var result = Context.Truck.SingleOrDefault(p => p.TruckId == id);

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

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SealOutTodo model)
        {
            try
            {
                var sealout = new SealOut
                {
                    SealTotal = model.SealTotal,
                    TruckId = model.TruckId,
                    TruckName = model.TruckName,

                    CreatedBy = model.CreatedBy,
                    UpdatedBy = model.UpdatedBy
                };
                Context.SealOut.Add(sealout);
                var result = Context.SaveChanges();
                if (result > 0)
                {
                    if (model.SealOutInfo != null)
                    {
                        List<SealOutInfo> sealOutInfoList = new List<SealOutInfo>();
                        List<Int32> sealInIdList = new List<Int32>();
                        foreach (var item in model.SealOutInfo)
                        {
                            var sealOutInfoModel = new SealOutInfo
                            {
                                SealInId = item.SealInId,
                                SealOutId = sealout.Id,
                                SealBetween = item.SealBetween,
                                Pack = item.Pack,
                                SealType = item.SealType,
                                SealTypeName = item.SealTypeName

                            };
                            sealOutInfoList.Add(sealOutInfoModel);

                            //seal In Id
                            if (item.SealType == 1) //ซีลปกติ
                            {
                                var id = item.SealInId;
                                sealInIdList.Add(Convert.ToInt32(id));
                            }
                        }
                        Context.SealOutInfo.AddRange(sealOutInfoList);
                        if (Context.SaveChanges() > 0)
                        {
                            //change IsActive = 1
                            var query = Context.SealIn.Where(p => sealInIdList.Contains(p.Id)).ToList();
                            foreach (var item in query)
                            {
                                item.IsActive = true;
                                //update SealItem IsUsed
                                var sealItem = Context.SealItem.Where(p => p.SealInId == item.Id);
                                foreach (var seal in sealItem)
                                {
                                    seal.IsUsed = true;
                                }
                                //Context.SealItem.UpdateRange(sealItem);
                                //Context.SaveChanges();
                            }
                            //Context.UpdateRange(query);
                            Context.SaveChanges();
                        }
                    }
                }

                return Ok(new { result = sealout, success = true, message = "เพิ่มข้อมูล  เรียบร้อยแล้ว" });
            }
            catch (Exception error)
            {
                _logger.LogError($"Log Create Truck: {error}");
                return StatusCode(500, new { result = "", message = error });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult>
        EditProduct([FromForm] Truck data, int id)
        {
            try
            {
                var product = Context.Truck.SingleOrDefault(p => p.TruckId == id);

                if (product == null)
                {
                    return NotFound();
                }

                Context.Truck.Update(product);
                Context.SaveChanges();

                return Ok(new { result = "", message = "update product successfully" });
            }
            catch (Exception error)
            {
                _logger.LogError($"Log UpdateProduct: {error}");
                return StatusCode(500, new { result = "", message = error });
            }
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            try
            {
                var product = Context.Truck.SingleOrDefault(p => p.TruckId == id);

                if (product == null)
                {
                    return NotFound();
                }

                Context.Truck.Remove(product);
                Context.SaveChanges();

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