using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using backend.Database;
using backend.Models;
using backend.Database;
using backend.Models;
using CryptoHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class UserController : ControllerBase
    {

        ILogger<AuthController> _logger;
        public DatabaseContext Context { get; }
        public IConfiguration Configuration { get; }

        public UserController(DatabaseContext context,
        ILogger<AuthController> logger,
        IConfiguration Configuration)
        {
            Context = context;
            _logger = logger;
            this.Configuration = Configuration;
        }

        [HttpGet("GetUser")]
        public IActionResult GetUser()
        {
            try
            {
                var query = from u in Context.Users
                            join r in Context.Roles
                            on u.RoleId equals r.Id into joinedUserRole
                            from ju in joinedUserRole.DefaultIfEmpty()
                            select new
                            {
                                Id = u.Id,
                                Username = u.Username,
                                Name = u.Name,
                                Email = u.Email,
                                RoleName = ju.Name,
                                IsActive = u.IsActive,
                                Created = u.Created,
                                Update = u.Updated
                            };
                var result = query.ToList();
                return Ok(new { result = result, message = "request successfully" });
            }
            catch (Exception error)
            {
                _logger.LogError($"Log Get User: {error}");
                return StatusCode(500, new { result = "", message = error });
            }
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] Users model)
        {
            try
            {
                var existingUser = Context.Users.FirstOrDefault(u => u.Username == model.Username);
                if (existingUser != null)
                {
                    return Ok(new { result = "failure", success = false, message = "มีข้อมูลผู้ใช้งาน" });
                }
                model.Password = Crypto.HashPassword(model.Password);
                Context.Users.Add(model);
                Context.SaveChanges();

                return Ok(new { result = "ok", message = "ลงทะเบียนผู้ใช้งานสำเร็จแล้ว" });
            }
            catch (Exception error)
            {
                _logger.LogError($"Log Register: {error}");
                return StatusCode(500, new { result = "failure", message = error });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Users users)
        {
            var result = await Context.Users.FindAsync(id);
            if (result == null)
            {
                return BadRequest();
            }
            else
            {
                result.Username = users.Username;
                result.Name = users.Name;
                result.Email = users.Email;
                result.IsActive = users.IsActive;
                result.RoleId = users.RoleId;
                if (!string.IsNullOrEmpty(users.Password))
                {
                    result.Password = users.Password;
                    users.Password = Crypto.HashPassword(users.Password);
                }
                Context.Users.Update(result);
                await Context.SaveChangesAsync();
                return Ok(new { result = "", message = "edit successfully" });
            }


        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var result = await Context.Users.FindAsync(id);
                if (result == null)
                {
                    return NotFound();
                }
                Context.Users.Remove(result);
                await Context.SaveChangesAsync();
                return Ok(new { result = "", message = "delete successfully" });
            }
            catch (Exception error)
            {

                return StatusCode(500, new { result = "", message = error });
            }
        }

        private string BuildToken(Users user)
        {
            string roleName = string.Empty;
            var result = Context.Roles.FirstOrDefault(role => role.Id == user.Id);
            if (result != null)
                roleName = result.Name;
            // key is case-sensitive
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, Configuration["Jwt:Subject"]),
                new Claim("id", user.Id.ToString()),
                new Claim("username", user.Username),
                new Claim("Name", user.Name),
                new Claim(ClaimTypes.Role, roleName)
            };

            var expires = DateTime.Now.AddDays(Convert.ToDouble(Configuration["Jwt:ExpireDay"]));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
                issuer: Configuration["Jwt:Issuer"],
                audience: Configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



    }
}