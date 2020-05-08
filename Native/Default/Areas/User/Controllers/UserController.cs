using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service;
using Default.Models;
using Microsoft.AspNetCore.Http;

namespace Default.Areas.User.Controllers
{
    [Area("User")]
    [ApiVersion("1.0")]
    //[ApiVersionNeutral]
    [Route("[area]/[controller]/[action]")]
    [ApiController]
    public class UserController : Controller, IBaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ILoginUserService _userService;

        public UserController(ILogger<HomeController> logger, ILoginUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public JsonResult GetJson()
        {
            return Json(_userService.GetLoginUser().ToString() + "来自："+ System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Namespace);
        }

    }
}
