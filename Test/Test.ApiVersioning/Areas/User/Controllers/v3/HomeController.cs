using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace Test.ApiVersioning.Areas.User.Controllers.v3
{
    [Area("User")]
    [ApiVersion("3.0")]
    //[ApiVersionNeutral]
    [Route("[area]/[controller]/[action]")]
    [ApiController]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public JsonResult GetJson()
        {
            return Json(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Namespace);
        }

    }
}
