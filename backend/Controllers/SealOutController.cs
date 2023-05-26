using System.Net.Http.Headers;
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
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Json;
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

                        // case "sealBetween":
                        //     //find id SealOut
                        //     query = query.Where(p => p.SealItemList.ToString().Contains(searchTerm));
                        //     break;
                        // case "sealNo":
                        //     query = query.Where(p => p.SealItemList.ToString().Contains(searchTerm));
                        //     break;
                        case "TruckName":
                            query = query.Where(p => p.TruckName.Contains(searchTerm));
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
                var result = Context.SealOut.FirstOrDefault(s => s.Id == id);

                if (result == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(new { result = result, message = "request successfully" });
                }

            }
            catch (Exception error)
            {
                _logger.LogError($"Log Get: {error}");
                return StatusCode(500, new { result = "", message = error });
            }
        }
        [HttpGet("GetSealOutInfo/{SealOutId}")]
        public IActionResult GetSealOutInfo(string SealOutId)
        {
            try
            {
                var result = from so in Context.SealOut
                             join info in Context.SealOutInfo on so.Id equals info.SealOutId
                             where so.SealOutId == SealOutId
                             select new
                             {
                                 Id = so.Id,
                                 SealTotal = so.SealTotal,
                                 SealTotalExtra = so.SealTotalExtra,
                                 TruckId = so.TruckId,
                                 TruckName = so.TruckName,
                                 IsCancel = so.IsCancel,
                                 SealBetWeen = info.SealBetween,
                                 Pack = info.Pack,
                                 SealType = info.SealType,
                                 SealTypeName = info.SealTypeName,
                                 SealList = info.SealList,
                                 Created = info.Created
                             };

                if (result == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(new { result = result, message = "request successfully" });
                }
            }
            catch (Exception error)
            {
                _logger.LogError($"Log Get: {error}");
                return StatusCode(500, new { result = "", message = error.Message });
            }
        }

        [HttpGet("ShowReceipt/{id}")]
        public IActionResult ShowReceipt(int id)
        {
            try
            {
                var result = from so in Context.SealOut
                             join info in Context.SealOutInfo on so.Id equals info.SealOutId
                             where so.Id == id
                             select new
                             {
                                 Id = so.Id,
                                 SealTotal = so.SealTotal,
                                 SealTotalExtra = so.SealTotalExtra,
                                 TruckId = so.TruckId,
                                 TruckName = so.TruckName,
                                 IsCancel = so.IsCancel,
                                 SealBetWeen = info.SealBetween,
                                 Pack = info.Pack,
                                 SealType = info.SealType,
                                 SealTypeName = info.SealTypeName,
                                 SealList = info.SealList,
                                 Created = info.Created
                             };

                if (result == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(new { result = result, message = "request successfully" });
                }
            }
            catch (Exception error)
            {
                _logger.LogError($"Log Get: {error}");
                return StatusCode(500, new { result = "", message = error.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RequestSealOut model)
        {
            try
            {
                var sealout = new SealOut
                {
                    SealTotal = model.SealTotal,
                    SealTotalExtra = model.SealTotalExtra,
                    TruckId = model.TruckId,
                    TruckName = model.TruckName,
                    SealExtraList = model.SealExtraList,
                    CreatedBy = model.CreatedBy,
                    UpdatedBy = model.UpdatedBy
                };
                Context.SealOut.Add(sealout);
                //update status seal extra
                dynamic jsonObject = JsonConvert.DeserializeObject(model.SealExtraList);
                if (jsonObject.Count > 0)
                {
                    foreach (var item in jsonObject)
                    {
                        Int32 id =item.id;
                        var sealNoExtra = Context.Seals.FirstOrDefault(a=>a.Id==id);
                        sealNoExtra.Status =2;
                        Context.Seals.Update(sealNoExtra);
                    }
                }
                var result = await Context.SaveChangesAsync();

                if (result > 0 && model.SealOutInfo != null)
                {
                    //update Id Format
                    sealout.SealOutId = Utilities.GennerateId("S0", sealout.Id);
                    Context.Update(sealout);
                    List<SealOutInfo> sealOutInfoList = new List<SealOutInfo>();
                    List<string> sealInId = new List<string>();
                    foreach (var item in model.SealOutInfo)
                    {

                        var sealOutInfoModel = new SealOutInfo
                        {
                            SealInId = item.SealInId,
                            SealOutId = sealout.Id,
                            SealBetween = item.SealBetween,
                            Pack = item.Pack,
                            SealType = item.SealType,
                            SealTypeName = item.SealTypeName,
                            SealList = item.SealList
                        };
                        sealOutInfoList.Add(sealOutInfoModel);
                        sealInId.Add(item.SealInId);
                    }
                    Context.SealOutInfo.AddRange(sealOutInfoList);
                    foreach (var id in sealInId)
                    {
                        //update status = 2 ใช้งานแล้ว sealIn
                        List<SealIn> results = (from p in Context.SealIn
                                                where p.SealInId == id
                                                select p).ToList();

                        foreach (SealIn p in results)
                        {
                            p.IsActive = true;
                        }
                        //update seal status = 2 ใช้งานแล้ว
                        var query = (from info in Context.SealInInfo
                                     join s in Context.Seals on info.SealId equals s.Id
                                     where info.SealInId == id
                                     select s).ToList();
                        foreach (Seals p in query)
                        {
                            p.Status = 2;
                        }
                    }


                    await Context.SaveChangesAsync();
                }


                return Ok(new { result = sealout, success = true, message = "เพิ่มข้อมูล  เรียบร้อยแล้ว" });
            }
            catch (Exception error)
            {
                _logger.LogError($"Log Create Truck: {error}");
                return StatusCode(500, new { result = "", message = error });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = Context.SealOut.SingleOrDefault(p => p.Id == id);

                if (result == null)
                {
                    return NotFound();
                }
                else
                {
                    var sealOutInfo = Context.SealOutInfo.Where(p => p.SealOutId == result.Id);

                    Context.SealOutInfo.RemoveRange(sealOutInfo);
                    Context.SealOut.Remove(result);
                    await Context.SaveChangesAsync();
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