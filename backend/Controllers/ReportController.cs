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
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {

         ILogger<ReportController> _logger;
        public DatabaseContext Context { get; }
        public IConfiguration Configuration { get; }

        public ReportController(DatabaseContext context,
        ILogger<ReportController> logger,
        IConfiguration Configuration)
        {
            Context = context;
            _logger = logger;
            this.Configuration = Configuration;
        }
    }
}