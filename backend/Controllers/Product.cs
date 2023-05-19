using backend.Database;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{

    [ApiController]
    [Route("/api/[controller]")]
    public class ProductController : ControllerBase
    {

        ILogger<ProductController> _logger;
        public DatabaseContext Context { get; }
        public IConfiguration Configuration { get; }

        public ProductController(DatabaseContext context,
        ILogger<ProductController> logger,
        IConfiguration Configuration)
        {
            Context = context;
            _logger = logger;
            this.Configuration = Configuration;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var product = Context.Products.ToList();
                return Ok(new{result=product});
            }
            catch (Exception error)
            {
                _logger.LogError(error.Message);
                return StatusCode(500, new { result = "", message = error });
            }
        }
    }

}