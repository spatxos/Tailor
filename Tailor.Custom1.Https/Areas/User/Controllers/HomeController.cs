using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service;
using Default.Models;
using Microsoft.AspNetCore.Http;

namespace Tailor.Custom1.Https.Areas.User.Controllers
{
    [Area("User")]
    [ApiVersion("2.0")]
    [Route("[area]/[controller]/[action]")]
    [ApiController]
    public class HomeController : Default.Areas.User.Controllers.HomeController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILoginUserService _userService;

        public HomeController(ILogger<HomeController> logger, ILoginUserService userService) : base(logger, userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public override IActionResult Index()
        {
            ViewBag.Form = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Namespace;
            
            return View(_userService.GetLoginUser());
        }

        
    }
}
