using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace level5Server.Models.level5
{
    [Route("[controller]")]
    public class UserReportsController : Controller
    {
        private readonly Level5Context _context;

        public UserReportsController(Level5Context context)
        {
            _context = context;
        }

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> UserReports()
        {
            return View(await _context.UserReports.OrderByDescending(x => x.Date).ToListAsync());
        }
    }
}
