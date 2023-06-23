using System.Net.Http.Headers;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using backend.Database;
using backend.Models;
using backend.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Json;
using PdfSharpCore;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using TheArtOfDev.HtmlRenderer.PdfSharp;
namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SealOutController : ControllerBase
    {
        ILogger<SealOutController> _logger;
        DatabaseContext Context;
        private readonly IWebHostEnvironment environment;

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

        [HttpGet("SealOutItem/{SealOutId}")]
        public async Task<IActionResult> GetSealOutItem(string SealOutId)
        {
            try
            {
                var result = from so in Context.SealOutItem
                             join si in Context.SealInItem on so.SealInId equals si.SealInId
                             where so.SealOutId == SealOutId
                             select new
                             {
                                 Id = so.Id,
                                 SealOutId = so.SealOutId,
                                 SealInId = si.SealInId,
                                 SealId = si.SealId,
                                 SealNo = si.SealNo,
                                 SealBetWeen = so.SealBetween,
                                 Pack = so.Pack,
                                 Created = so.Created,
                                 Updated = so.Updated
                             };

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
        [HttpGet("GetSealRemarks")]
        public IActionResult GetSealRemarks()
        {
            try
            {
                var result = Context.SealRemarks.ToList();

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
                             join info in Context.SealOutItem on so.SealOutId equals info.SealOutId
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
                                 Created = info.Created,
                                 Updated = info.Updated
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

        [HttpGet("ShowReceiptHeader/{SealOutId}")]
        public IActionResult ShowReceiptHeader(string SealOutId)
        {
            try
            {
                var result = Context.SealOut.FirstOrDefault(a => a.SealOutId == SealOutId);

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
        [HttpGet("ShowReceiptDetail/{SealOutId}")]
        public IActionResult ShowReceipt(string SealOutId)
        {
            try
            {
                var result = from so in Context.SealOutItem
                             join si in Context.SealInItem on so.SealInId equals si.SealInId
                             where so.SealOutId == SealOutId
                             select new
                             {
                                 Id = so.Id,
                                 SealOutId = so.SealOutId,
                                 SealInId = si.SealId,
                                 SealBetween = so.SealBetween,
                                 Pack = so.Pack,
                                 SealId = si.SealId,
                                 SealNo = si.SealNo
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
        [HttpGet("GetSealExtraDetail/{SealOutId}")]
        public IActionResult GetSealExtraDetail(string SealOutId)
        {
            try
            {
                var result = Context.SealOutExtraItem.Where(a => a.SealOutId == SealOutId);

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

        [HttpGet("GeneratePdf/{SealOutId}")]
        public async Task<IActionResult> GeneratePdf(string SealOutId)
        {
            var header = Context.SealOut.FirstOrDefault(s => s.SealOutId == SealOutId);
            var detail = from so in Context.SealOut
                         join info in Context.SealOutItem on so.SealOutId equals info.SealOutId
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
                             Created = info.Created
                         };
            var document = new PdfDocument();
            string htmlelement = "<div  style='width:100%';font-family:'Courier New';>";
            // string imgeurl = "https://res.cloudinary.com/demo/image/upload/v1312461204/sample.jpg";
            //string imgeurl = "https://" + HttpContext.Request.Host.Value + "/Uploads/common/logo.jpeg";
            string imgeurl = "data:image/png;base64, " + Getbase64string() + "";
            htmlelement += "<img style='width:80px;height:80%' src='" + imgeurl + "'   />";
            htmlelement += "<h1>ใบจ่ายซีล</h2>";
            htmlelement += "<h2>คลังน้ำมันเอสโซ่ ศรีราชา</h2>";
            if (header != null)
            {
                htmlelement += "<h2> รหัสการจ่ายซีล:" + header.SealOutId + " & Invoice Date:" + header.Created.ToString("yyyy/MM/dd") + "</h2>";
                htmlelement += "<h3> ทะเบียนรถ : " + header.TruckName + "</h3>";
            }
            htmlelement += "</div>";
            PdfGenerator.AddPdfPages(document, htmlelement, PageSize.A4);
            byte[] response = null;
            using (MemoryStream ms = new MemoryStream())
            {
                document.Save(ms);
                response = ms.ToArray();
            }
            string fileName = "report-" + DateTime.Now + ".pdf";
            return File(response, "application/pdf", fileName);
        }
        [NonAction]
        public string Getbase64string()
        {
            string filepath = this.Env.WebRootPath + "/Uploads/img/logo-esso.png";
            byte[] imgarray = System.IO.File.ReadAllBytes(filepath);
            string base64 = Convert.ToBase64String(imgarray);
            return base64;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RequestSealOut model)
        {
            try
            {
                using (var dbtransaction = await this.Context.Database.BeginTransactionAsync())
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
                        //update status seal extra
                        var result = await Context.SaveChangesAsync();
                        var processCount = 0;
                        if (result > 0 && model.SealOutItem != null)
                        {
                            //update Id Format
                            sealout.SealOutId = Utilities.GennerateId("SO", sealout.Id);
                            Context.Update(sealout);
                            List<SealOutItem> sealOutInfo = new List<SealOutItem>();
                            List<string> sealInId = new List<string>();

                            dynamic jsonObject = JsonConvert.DeserializeObject(model.SealExtraList);
                            if (jsonObject.Count > 0)
                            {
                                List<SealOutExtraItem> sealOutExtraItem = new List<SealOutExtraItem>();
                                foreach (var item in jsonObject)
                                {
                                    Int32 id = item.id;
                                    var sealExtra = Context.Seals.FirstOrDefault(a => a.Id == id);
                                    sealExtra.Status = 1;
                                    sealExtra.IsActive = true;
                                    sealExtra.Type = "พิเศษ";
                                    Context.Seals.Update(sealExtra);
                                    var modelSealExtraItem = new SealOutExtraItem
                                    {
                                        SealOutId = sealout.SealOutId,
                                        SealId = id,
                                        SealNo = item.sealNo,
                                        CreatedBy = model.CreatedBy,
                                        UpdatedBy = model.UpdatedBy
                                    };
                                    sealOutExtraItem.Add(modelSealExtraItem);
                                }
                                Context.SealOutExtraItem.AddRange(sealOutExtraItem);
                            }

                            foreach (var item in model.SealOutItem)
                            {
                                var sealOutInfoModel = new SealOutItem
                                {
                                    SealInId = item.SealInId,
                                    SealOutId = sealout.SealOutId,
                                    SealBetween = item.SealBetween,
                                    Pack = item.Pack,
                                    CreatedBy = model.CreatedBy,
                                    UpdatedBy = model.UpdatedBy
                                };
                                sealOutInfo.Add(sealOutInfoModel);
                                sealInId.Add(item.SealInId);
                                processCount++;
                            }
                            await Context.SealOutItem.AddRangeAsync(sealOutInfo);
                            //await Context.SaveChangesAsync();

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
                                var query = (from sealItem in Context.SealInItem
                                             join s in Context.Seals on sealItem.SealId equals s.Id
                                             where sealItem.SealInId == id
                                             select s).ToList();
                                foreach (Seals p in query)
                                {
                                    p.IsActive = true;

                                }

                            }
                        }
                        if (processCount == model.SealOutItem.Count)
                        {
                            await Context.SaveChangesAsync();
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
                        _logger.LogError($"Log Add SealOut: {error.Message}");
                        return StatusCode(500, new { result = "", message = error.Message });
                    }

                }

                return Ok(new { result = "", success = true, message = "เพิ่มข้อมูล  เรียบร้อยแล้ว" });
            }
            catch (Exception error)
            {
                _logger.LogError($"Log Create Truck: {error}");
                return StatusCode(500, new { result = "", message = error });
            }
        }

        [HttpPost("SealChange")]
        public async Task<IActionResult> SealChange([FromBody] RequestSealChanges[] request)
        {
            try
            {
                using (var dbtransaction = await this.Context.Database.BeginTransactionAsync())
                {
                    List<SealChanges> sealChangeList = new List<SealChanges>();
                    //update SealOutInfoList
                    foreach (var r in request)
                    {
                        // var sealNoNew = Context.Seals.FirstOrDefault(s => s.SealNo == r.SealNoNew);
                        // var findSealOld = Context.Seals.FirstOrDefault(s => s.SealNo == r.SealNoOld);
                        // var findSealInItem = Context.SealInItem.FirstOrDefault(a => a.SealInId == r.SealInId && a.SealNo == r.SealNoOld);
                        int? remarkId = 0;
                        string remarks = string.Empty;
                        //update status sealOid
                        if (r.RemarkId == 0)
                        {
                            var sealRemarks = new SealRemarks { Name = r.Remarks };
                            Context.SealRemarks.Add(sealRemarks);
                            Context.SaveChanges();
                            remarkId = sealRemarks.Id;
                            remarks = r.Remarks;
                        }
                        else
                        {
                            var sealRemarks = Context.SealRemarks.FirstOrDefault(a => a.Id == r.RemarkId);
                            remarkId = r.RemarkId;
                            remarks = sealRemarks.Name;
                        }
                        //**********find sealNew ****************************************
                        int? sealIdNew = 0;
                        string sealNoNew = string.Empty;
                        if (r.SealNoNew == string.Empty)
                        {
                            var sealNew = Context.Seals.FirstOrDefault(a => a.Id == r.SealIdNew);
                            sealIdNew = sealNew.Id;
                            sealNoNew = sealNew.SealNo;
                        }
                        else
                        {
                            sealIdNew = r.SealIdNew;
                            sealNoNew = r.SealNoNew;
                        }
                        //***************************************************************
                        var sealInItem = Context.SealInItem.FirstOrDefault(a => a.SealInId == r.SealInId && a.SealId == r.SealIdOld);
                        if (sealInItem != null)
                        {
                            sealInItem.SealId = sealIdNew;
                            sealInItem.SealNo = sealNoNew;
                            sealInItem.UpdatedBy = r.CreatedBy;
                            sealInItem.Updated = DateTime.Now;
                            Context.SealInItem.Update(sealInItem);
                        }
                        //update stutus sealOld เป็นซีลชำรุด
                        var sealOld = Context.Seals.FirstOrDefault(a => a.Id == r.SealIdOld);
                        if (sealOld != null)
                        {
                            sealOld.Status = 3;
                            sealOld.Updated = DateTime.Now;
                            sealOld.UpdatedBy = r.UpdatedBy;
                            Context.Seals.Update(sealOld);
                        }
                        //update active sealNew
                        var seal = Context.Seals.FirstOrDefault(a => a.Id == r.SealIdNew);
                        if (seal != null)
                        {
                            seal.IsActive = true;
                            seal.Updated = DateTime.Now;
                            seal.UpdatedBy = r.UpdatedBy;
                            Context.Seals.Update(seal);
                            //add SealChanges
                            var modelList = new SealChanges
                            {
                                SealOutId = r.SealOutId,
                                SealInId = r.SealInId,
                                SealIdOld = r.SealIdOld,
                                SealNoOld = r.SealNoOld,
                                SealIdNew = sealIdNew,
                                SealNoNew = sealNoNew,
                                RemarkId = remarkId,
                                Remarks = remarks,
                                CreatedBy = r.CreatedBy,
                                UpdatedBy = r.UpdatedBy,
                            };
                            sealChangeList.Add(modelList);
                        }
                    }
                    await Context.SealChanges.AddRangeAsync(sealChangeList);
                    await Context.SaveChangesAsync();
                    await dbtransaction.CommitAsync();

                }
                return Ok(new { result = "", success = true, message = "เปลียนหมายเลขซีล  เรียบร้อยแล้ว" });
            }
            catch (System.Exception error)
            {
                _logger.LogError($"Log SealChange : {error.Message}");
                return StatusCode(500, new { result = "", message = error.Message });
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
                    var sealOutItem = Context.SealOutItem.Where(p => p.SealOutId == result.SealOutId);
                    Context.SealOutItem.RemoveRange(sealOutItem);
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