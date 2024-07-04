using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using level5Server.Models;
using level5Server.Models.level5;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Level5Backend.Models;

namespace level5Server.Controllers
{
    [Route("api/application")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly Level5Context _context;
        private static readonly string[] Summaries = new[]
{
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        public ApplicationController(Level5Context context)
        {
            _context = context;
        }

        //--------------------- HTTP GET ---------------------------------------------------
        // GET: /api/highscores
        /// <summary>
        /// Get all application versions
        /// </summary>
        [HttpGet("version")]
        public IEnumerable<String> GetAllVersions()
        {

            //return await _context.Application.OrderByDescending(x => x.Id)
            //    .ToListAsync();
            return Summaries;
        }

        //--------------------- HTTP GET ---------------------------------------------------
        // GET: /api/highscores
        /// <summary>
        /// Get current application versions
        /// </summary>
        [HttpGet("version/current")]
        public ActionResult<object> GetCurrentVersion()
        {
            var version = _context.Application
                .OrderByDescending(x => x.Id)
                .Select(x => x.CurrentVersion)
                .ToList()
                .First();

            return version;
        }

        //--------------------- HTTP POST new application version ---------------------------------------------------
        // POST: api/Highscores
        /// <summary>
        /// Add new application version
        /// </summary>
        [Route("version")]
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        public async Task<ActionResult<Application>> PostHighscore(Application application)
        {
            //_context.Users.Where(e => e.Userid == highscores.Userid).Any();
            // if empty username  or userid NOT in user table
            if (string.IsNullOrEmpty(application.CurrentVersion)
                || _context.Application.Where(e => e.CurrentVersion == application.CurrentVersion).Any())
            {
                return BadRequest();
            }
            else
            {
                _context.Application.Add(application);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetAllVersions), new { id = application.Id }, application);
            }
        }

        ////--------------------- HTTP GET  Modeid by Modeid ---------------------------------------------------
        //// GET: /api/highscores/modeid/{modeid}?hardcore={int}&traffic={int}&enemies={int}
        //// highscores by modeid with optiona; filters by hardcore, traffic, enemies
        //[HttpGet("modeid/count/{modeid}")]
        //public ActionResult<object> GetHighScoreCountByModeId(int modeid,
        //    int hardcore,
        //    int traffic,
        //    int enemies)
        //{
        //    var count = _context.Highscores
        //        .Where(x => x.Modeid == modeid
        //        && x.HardcoreEnabled == hardcore
        //        && x.TrafficEnabled == traffic
        //        && x.EnemiesEnabled == enemies)
        //        .Select(x => x.Id)
        //        .Count();

        //    return count;
        //}
    }
}

