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

        [HttpGet("GetSealOutInfoList/{SealOutId}")]
        public IActionResult GetSealOutInfoList(string SealOutId)
        {
            try
            {
                var result = Context.SealOutInfoList.Where(s => s.SealOutId == SealOutId);

                if (result == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(new { result = result.ToList(), message = "request successfully" });
                }

            }
            catch (Exception error)
            {
                _logger.LogError($"Log Get: {error}");
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
                             join info in Context.SealOutInfo on so.SealOutId equals info.SealOutId
                             where so.SealOutId == SealOutId
                             select new
                             {
                                 Id = so.Id,
                                 SealOutId = so.SealOutId,
                                 SealInId = info.SealInId,
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
                             join info in Context.SealOutInfo on so.SealOutId equals info.SealOutId
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
                        Int32 id = item.id;
                        var sealNoExtra = Context.Seals.FirstOrDefault(a => a.Id == id);
                        sealNoExtra.Status = 2;
                        Context.Seals.Update(sealNoExtra);
                    }
                }
                var result = await Context.SaveChangesAsync();

                if (result > 0 && model.SealOutInfo != null)
                {
                    //update Id Format
                    sealout.SealOutId = Utilities.GennerateId("SO", sealout.Id);
                    Context.Update(sealout);
                    List<SealOutInfo> sealOutInfo = new List<SealOutInfo>();
                    List<string> sealInId = new List<string>();
                    List<SealOutInfoList> sealOutInfoList = new List<SealOutInfoList>();
                    foreach (var item in model.SealOutInfo)
                    {

                        var sealOutInfoModel = new SealOutInfo
                        {
                            SealInId = item.SealInId,
                            SealOutId = sealout.SealOutId,
                            SealBetween = item.SealBetween,
                            Pack = item.Pack,
                            SealType = item.SealType,
                            SealTypeName = item.SealTypeName,
                            SealList = item.SealList
                        };
                        sealOutInfo.Add(sealOutInfoModel);
                        sealInId.Add(item.SealInId);
                    }
                    Context.SealOutInfo.AddRange(sealOutInfo);
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
                            p.IsActive = true;

                            //SealOutInfoList
                            var modelList = new SealOutInfoList
                            {
                                SealOutId = sealout.SealOutId,
                                SealInId = id,
                                sealId = p.Id,
                                SealNo = p.SealNo
                            };
                            sealOutInfoList.Add(modelList);
                        }

                    }

                    await Context.SealOutInfoList.AddRangeAsync(sealOutInfoList);
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

        [HttpPost("SealChange")]
        public async Task<IActionResult> SealChange([FromBody] SealChanges[] request)
        {
            //update SealOutInfoList
            foreach (var r in request)
            {
                var query = Context.SealOutInfoList.FirstOrDefault(a => a.SealOutId == r.SealOutId && a.SealInId == r.SealInId && a.SealNo == r.SealNoOld);
                var findSealNew = Context.Seals.FirstOrDefault(s => s.SealNo == r.SealNoNew);
                var findSealOld = Context.Seals.FirstOrDefault(s => s.SealNo == r.SealNoOld);
                var findSealInInfo = Context.SealInInfo.FirstOrDefault(a => a.SealInId == r.SealInId && a.SealNo == r.SealNoOld);
                if (query != null && findSealNew != null && findSealOld != null && findSealInInfo != null)
                {
                    query.sealId = findSealNew.Id;
                    query.SealNo = findSealNew.SealNo;
                    query.Updated = DateTime.Now;

                    //update status sealOid
                    findSealOld.Status = r.RemarkId;
                    findSealOld.Updated = DateTime.Now;
                    findSealOld.UpdatedBy =r.UpdatedBy;
                    //update active sealNew
                    findSealNew.IsActive = true;
                    findSealNew.Updated =DateTime.Now;
                    findSealNew.UpdatedBy =r.UpdatedBy;
                    //update SealInInfo
                    findSealInInfo.SealId =findSealNew.Id;
                    findSealInInfo.SealNo = findSealNew.SealNo;
                    findSealInInfo.UpdaetedBy =r.UpdatedBy;
                    findSealInInfo.Updated =DateTime.Now;

                }
            }

            var result = Context.SealChanges.AddRangeAsync(request);
            await Context.SaveChangesAsync();
            return Ok(new { result = "", success = true, message = "เปลียนหมายเลขซีล  เรียบร้อยแล้ว" });
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
                    var sealOutInfo = Context.SealOutInfo.Where(p => p.SealOutId == result.SealOutId);
                    var sealOutInfoList = Context.SealOutInfoList.Where(p => p.SealOutId == result.SealOutId);

                    Context.SealOutInfoList.RemoveRange(sealOutInfoList);
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