using Microsoft.AspNetCore.Mvc;

namespace level5Server.Controllers
{
    public class ChangeLogController : Controller
    {
        [Route("[controller]")]
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index()
        {
            return View();
        }
    }
}
