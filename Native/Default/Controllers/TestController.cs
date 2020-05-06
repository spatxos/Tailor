using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Service;
using Default.Models;
using Microsoft.AspNetCore.Http;

namespace Default.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]

    public class TestController : Controller, IBaseController
    {

        public virtual IActionResult Index()
        {
            ViewBag.Form = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Namespace;
            
            return Json($"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Namespace}.Test");
        }

    }
}
