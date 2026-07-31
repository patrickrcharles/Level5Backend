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
            if (await ReportTextExistsAsync(userReport.Userid, userReport.Report))
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

        // Scoped per-user rather than globally - two different users legitimately submitting the
        // same report text (e.g. "crashes on level 3") shouldn't conflict with each other; this is
        // meant to catch one user re-submitting (a broken retry, or spam), not coincidental phrasing.
        private async Task<bool> ReportTextExistsAsync(int userid, string report)
        {
            return await _context.UserReports.AnyAsync(e => e.Userid == userid && e.Report == report);
        }
    }
}
