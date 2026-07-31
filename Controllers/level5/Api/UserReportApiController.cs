using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using level5Server.Models;
using level5Server.Models.level5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Level5Backend.Models;
using Microsoft.AspNetCore.Cors;

namespace level5Server.Controllers.level5.Api
{
    [EnableCors("ApiCors")]
    //[ApiVersion("1")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [Route("api/userreport")]
    [ApiController]
    public class UserReportApiController : Controller
    {
        private readonly Level5Context _context;
        private readonly ILogger<UserReportApiController> _logger;

        public UserReportApiController(Level5Context context, ILogger<UserReportApiController> logger)
        {
            _context = context;
            _logger = logger;
        }

        //--------------------- HTTP GET ---------------------------------------------------
        // GET: /api/highscores
        // get all users
        /// <summary>
        /// Get all user reports, paginated (defaults to the first 50, capped at 200 per page).
        /// </summary>
        [Authorize(Policy = "RequireDev")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserReport>>> GetAllReports(int page = 0, int results = 50)
        {
            int take = Math.Clamp(results, 1, 200);
            int skip = Math.Max(page, 0) * take;

            return await _context.UserReports
                .OrderByDescending(r => r.Id)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<User>> PostUserReport(UserReport userReport)
        {
            // empty text
            if (String.IsNullOrEmpty(userReport.Report))
            {
                return BadRequest();
            }
            // text exists
            if (await ReportTextExistsAsync(userReport.Report))
            {
                return Conflict();
            }

            userReport.Date = DateTime.UtcNow;
            try
            {
                _context.UserReports.Add(userReport);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetAllReports), new { id = userReport.Id }, userReport);
            }
            catch (DbUpdateConcurrencyException e)
            {
                _logger.LogWarning(e, "Failed to save user report due to a concurrency conflict");
                return BadRequest();
            }
        }

        private async Task<bool> ReportTextExistsAsync(string report)
        {
            return await _context.UserReports.AnyAsync(e => e.Report == report);
        }
    }
}
