using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Entity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service;
using Tailor.Custom3.Https.Models;

namespace Tailor.Custom3.Https.Controllers
{
    [ApiVersion("2.0")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class HomeController : Default.Controllers.HomeController
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
            return View(new LoginUser());
        }

        public IActionResult Index2()
        {
            ViewBag.Form = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Namespace;
            return View(new LoginUser());
        }
    }
}
