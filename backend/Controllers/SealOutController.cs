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
                                 SealItemList = info.SealItemList,
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
                return StatusCode(500, new { result = "", message = error });
            }
        }
        [HttpGet("ShowReceipt/{id}")]
        public IActionResult ShowReceipt(string id)
        {
            try
            {
                Int32 sealOutId = Int32.Parse(id);
                var result = from so in Context.SealOut
                             join soif in Context.SealOutInfo on so.Id equals soif.SealOutId
                             //join si in Context.SealItem on soif.SealInId equals si.SealInId
                             where so.Id == sealOutId
                             select new
                             {
                                 Id = so.Id,
                                 SealTotal = so.SealTotal,
                                 TruckName = so.TruckName,
                                 Created = so.Created,
                                 SealBetween = soif.SealBetween,
                                 Pack = soif.Pack,
                                 SealType = soif.SealType,
                                 SealTypeName = soif.SealTypeName,
                                 //SealNo = si.SealNo
                             };

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
                    SealTotalExtra = model.SealTotalExtra,
                    TruckId = model.TruckId,
                    TruckName = model.TruckName,
                    CreatedBy = model.CreatedBy,
                    UpdatedBy = model.UpdatedBy
                };
                Context.SealOut.Add(sealout);
                var result = await Context.SaveChangesAsync();
                if (result > 0 && model.SealOutInfo != null)
                {
                    List<SealOutInfo> sealOutInfoList = new List<SealOutInfo>();
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
                            SealItemList = item.SealItemList
                        };
                        sealOutInfoList.Add(sealOutInfoModel);
                    }
                    Context.SealOutInfo.AddRange(sealOutInfoList);
                    await Context.SaveChangesAsync();
                }
                // if (result > 0)
                // {
                //     if (model.SealOutInfo != null)
                //     {
                //         List<SealOutInfo> sealOutInfoList = new List<SealOutInfo>();
                //         List<Int32> sealInIdList = new List<Int32>();
                //         List<Int32> sealInIdListExtra = new List<Int32>();
                //         //List<SealItem> sealItemsExtra = new List<SealItem>();

                //         foreach (var item in model.SealOutInfo)
                //         {

                //             var sealOutInfoModel = new SealOutInfo
                //             {
                //                 SealInId = item.SealInId,
                //                 SealOutId = sealout.Id,
                //                 SealBetween = item.SealBetween,
                //                 Pack = item.Pack,
                //                 SealType = item.SealType,
                //                 SealTypeName = item.SealTypeName,
                //                 SealItemList = item.SealItemList
                //             };

                //             //seal In Id
                //             if (item.SealType == 1) //ซีลปกติ
                //             {
                //                 var id = item.SealInId;
                //                 sealInIdList.Add(Convert.ToInt32(id));
                //             }
                //             if (item.SealType == 2) //ซีลพิเศษ
                //             {
                //                 //insert SealItem ซีลพิเศษ
                //                 var sealItemModel = new SealItem
                //                 {
                //                     SealNo = item.SealBetween,
                //                     Type = 2, //ปกติ
                //                     IsUsed = true,
                //                     Status = 1 //ซีลใช้งานได้ปกติ
                //                 };
                //                 //sealItemsExtra.Add(sealItemModel);
                //                 Context.SealItem.AddRange(sealItemModel);
                //                 Context.SaveChanges();
                //                 //Add Seal Extra to SealOutInfo

                //                 sealOutInfoModel = new SealOutInfo
                //                 {
                //                     SealInId = 0,
                //                     SealOutId = sealout.Id,
                //                     SealBetween = item.SealBetween,
                //                     Pack = item.Pack,
                //                     SealType = item.SealType,
                //                     SealTypeName = item.SealTypeName,
                //                     SealItemList = item.SealItemList
                //                 };
                //             }

                //             sealOutInfoList.Add(sealOutInfoModel);
                //         }
                //         Context.SealOutInfo.AddRange(sealOutInfoList);
                //         var save = await Context.SaveChangesAsync();
                //         if (save > 0)
                //         {

                //             string sql = @"UPDATE dbo.SealIn
                //                         SET IsActive = 1
                //                         WHERE SealIn.Id IN (SELECT distinct SealInId From SealOutInfo WHERE SealOutInfo.SealOutId =" + sealout.Id + ")";
                //             SqlDatabase.Exec_NonQuery(sql);


                //             sql = @"UPDATE dbo.SealItem
                //                     SET IsUsed = 1
                //                     WHERE SealItem.Id IN 
                //                     (SELECT distinct SealItemId FROM SealItem
                //                     INNER JOIN SealInTransaction
                //                     ON SealItem.Id = SealInTransaction.SealItemId
                //                     INNER JOIN SealOutInfo
                //                     ON SealInTransaction.SealInId = SealOutInfo.SealInId
                //                     WHERE SealOutInfo.SealOutId ="+sealout.Id+")";
                //             SqlDatabase.Exec_NonQuery(sql);
                //         }
                //     }
                // }

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