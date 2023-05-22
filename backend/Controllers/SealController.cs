﻿using backend.Database;
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
        public IActionResult Get()
        {
            try
            {
                var result = Context.Seals.ToList();
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
                             select new{
                                Id =seal.Id,
                                SealNo =seal.SealNo,
                                Type = seal.Type,
                                Status =seal.Status,
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