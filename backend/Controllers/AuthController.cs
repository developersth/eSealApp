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

namespace backend.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AuthController : ControllerBase
    {

        ILogger<AuthController> _logger;
        public DatabaseContext Context { get; }
        public IConfiguration Configuration { get; }

        public AuthController(DatabaseContext context,
        ILogger<AuthController> logger,
        IConfiguration Configuration)
        {
            Context = context;
            _logger = logger;
            this.Configuration = Configuration;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] Users model)
        {
            try
            {
                var existingUser = Context.Users.FirstOrDefault(u => u.Username == model.Username);
                if (existingUser != null)
                {
                    return Ok(new { result = "failure",success=false, message = "มีข้อมูลผู้ใช้งาน" });
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


        [HttpPost("login")]
        public IActionResult Login([FromBody] Users model)
        {
            try
            {
                var result = Context.Users.SingleOrDefault(u => u.Username == model.Username);
                if (result == null)
                {
                    return Unauthorized();
                }

                if (!Crypto.VerifyHashedPassword(result.Password, model.Password))
                {
                    return Unauthorized(); ;
                }

                var token = BuildToken(result);
                return Ok(new { token = token, User = result, message = "login successfully" });
            }
            catch (Exception error)
            {
                _logger.LogError($"Log Login: {error}");
                return StatusCode(500, new { result = "failure", message = error });
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