using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service;

namespace Test.Web3.Controllers
{
    [ApiVersion("2.0")]
    [Route("[controller]/[action]")]
    public class MyHomeController : Default.Controllers.HomeController
    {
        private readonly ILogger<MyHomeController> _logger;
        private readonly ILoginUserService _userService;

        public MyHomeController(ILogger<MyHomeController> logger, ILoginUserService userService) : base(logger, userService)
        {

        }


        public override IActionResult Index()
        {
            ViewBag.Form = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Namespace;
            return View(null);
        }

        public IActionResult Index2()
        {
            ViewBag.Form = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Namespace;
            return View(null);
        }
    }
}
