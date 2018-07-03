using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Sensor.Api.Controllers
{
    [Route("")]
    [AllowAnonymous]
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Json("Welcome in API");
        }
    }
}