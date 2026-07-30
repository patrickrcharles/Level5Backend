using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Level5Backend.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;

namespace level5Server.Models.level5.Api
{
    //[Authorize]
    [ApiController]
    [EnableCors("ApiCors")]
    [Route("api/highscores")]

    public class HighscoresApiController : ControllerBase
    {
        private readonly Level5Context _context;

        public HighscoresApiController(Level5Context context)
        {
            _context = context;
        }

        //--------------------- HTTP GET ---------------------------------------------------
        // GET: /api/highscores?page=0&results=50
        /// <summary>
        /// Get all high scores, paginated (defaults to the first 50, capped at 200 per page).
        /// </summary>
        [EnableCors("ApiCors")]
        [HttpGet(Name = "GetHighScores")]
        public async Task<IEnumerable<Highscore>> GetAllHighscores(int page = 0, int results = 50)
        {
            int take = Math.Clamp(results, 1, 200);
            int skip = Math.Max(page, 0) * take;

            var highscores = await _context.Highscores.AsNoTracking()
                 .OrderByDescending(x => x.Id)
                 .Skip(skip)
                 .Take(take)
                 .ToListAsync();
            HideHighScoreDetails(highscores);

            return highscores;
        }

        //--------------------- HTTP GET  Platform ---------------------------------------------------
        // GET: /api/highscores/modeid/1
        /// <summary>
        /// Get high scores by platfoem. [handheld, desktop]
        /// </summary>
        /// 
        [EnableCors("ApiCors")]
        [HttpGet("platform/{platform}")]
        public async Task<ActionResult<IEnumerable<Highscore>>> GetHighScoreByPlatform(string platform)
        {
            var highscores = await _context.Highscores.AsNoTracking()
                .Where(x => x.Platform == platform)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
            HideHighScoreDetails(highscores);

            return highscores;
        }

        //--------------------- HTTP GET  Modeid by Userid ---------------------------------------------------
        // GET: /api/highscores/modeid/1/userid/1
        /// <summary>
        /// Get high scores by mode id and user id. 
        /// </summary>
        [HttpGet("modeid/{modeid}/userid/{userid}")]
        public async Task<ActionResult<IEnumerable<Highscore>>> GetHighScoreByModeIdUserId(int modeid, int userid)
        {
            var highscores = await _context.Highscores.AsNoTracking()
                .Where(x => x.Modeid == modeid && x.Userid == userid)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            HideHighScoreDetails(highscores);

            return highscores;
        }
        //--------------------- HTTP GET Modeid by Platform ---------------------------------------------------
        // GET: /api/highscores/modeid/1/platform/1
        /// <summary>
        /// Get high scores by mode id and platform. 
        /// </summary>
        [HttpGet("modeid/{modeid}/platform/{platform}")]
        public async Task<ActionResult<IEnumerable<Highscore>>> GetHighScoreByModeIdPlatform(int modeid, string platform)
        {
            var highscores = await _context.Highscores.AsNoTracking()
                .Where(x => x.Modeid == modeid && x.Platform == platform)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            HideHighScoreDetails(highscores);

            return highscores;
        }

        //--------------------- HTTP GET  Modeid by Modeid - Filtered  ---------------------------------------------------
        // GET: /api/highscores/modeid/{modeid}?hardcore={int}&traffic={int}&enemies={int}
        /// <summary>
        /// Get high scores by mode id and optional filters. [hardcoreEnabled, trafficEnabled, enemiesEnabled, sniperEnabled] 
        /// </summary>
        [HttpGet("modeid/filter/{modeid}")]
        public async Task<ActionResult<IEnumerable<Object>>> GetHighScoreByModeIdForGameDisplayFiltered(int modeid,
            int hardcore,
            int traffic,
            int enemies,
            int sniper,
            int page,
            int results)
        {
            ActionResult<IEnumerable<Object>> list = null;
            // totalpoints highscore
            if (modeid == 1 || (modeid > 14 && modeid < 20) || modeid == 23 || modeid == 24 || modeid == 26)
            {
                var highscores = await _context.Highscores
                    .Where(x => x.Modeid == modeid
                    && x.HardcoreEnabled == hardcore
                    && x.TrafficEnabled == traffic
                    && x.SniperEnabled == sniper
                    && x.EnemiesEnabled == enemies)
                    .Select(x => new
                    {
                        Score = x.TotalPoints.ToString(),
                        x.Character,
                        x.Level,
                        x.Date,
                        Time = x.Time.ToString(),
                        UserId = x.Userid.ToString(),
                        x.TotalPoints,
                        x.Username,
                        x.HardcoreEnabled,
                        x.EnemiesEnabled,
                        x.TrafficEnabled,
                        x.EnemiesKilled,
                        x.Platform
                    })
                    .OrderByDescending(x => x.TotalPoints)
                    .Skip(page * 10)
                    .Take(results)
                    .ToListAsync();
                list = highscores;
            }
            // maxshotmade highscore
            if ((modeid > 1 && modeid < 5))
            {
                var highscores = await _context.Highscores
                    .Where(x => x.Modeid == modeid
                    && x.HardcoreEnabled == hardcore
                    && x.TrafficEnabled == traffic
                    && x.SniperEnabled == sniper
                    && x.EnemiesEnabled == enemies)
                    .Select(x => new
                    {
                        Score = x.MaxShotMade.ToString(),
                        x.Character,
                        x.Level,
                        x.Date,
                        Time = x.Time.ToString(),
                        UserId = x.Userid.ToString(),
                        x.MaxShotMade,
                        x.Username,
                        x.HardcoreEnabled,
                        x.EnemiesEnabled,
                        x.TrafficEnabled,
                        x.EnemiesKilled,
                        x.Platform
                    })
                    .OrderByDescending(x => x.MaxShotMade)
                    .Skip(page * 10)
                    .Take(results)
                    .ToListAsync();
                list = highscores;
            }
            // totaldistance highscore
            if (modeid == 6)
            {
                var highscores = await _context.Highscores
                    .Where(x => x.Modeid == modeid
                    && x.HardcoreEnabled == hardcore
                    && x.TrafficEnabled == traffic
                    && x.SniperEnabled == sniper
                    && x.EnemiesEnabled == enemies)
                    .Select(x => new
                    {
                        Score = x.TotalDistance.ToString(),
                        x.Character,
                        x.Level,
                        x.Date,
                        Time = x.Time.ToString(),
                        UserId = x.Userid.ToString(),
                        x.TotalDistance,
                        x.Username,
                        x.HardcoreEnabled,
                        x.EnemiesEnabled,
                        x.TrafficEnabled,
                        x.EnemiesKilled,
                        x.Platform
                    })
                    .OrderByDescending(x => x.TotalDistance)
                    .Skip(page * 10)
                    .Take(results)
                    .ToListAsync();

                list = highscores;
            }
            // time highscore
            if ((modeid > 6 && modeid < 10) || modeid == 25)
            {
                var highscores = await _context.Highscores
                    .Where(x => x.Modeid == modeid
                    && x.HardcoreEnabled == hardcore
                    && x.TrafficEnabled == traffic
                    && x.SniperEnabled == sniper
                    && x.EnemiesEnabled == enemies)
                    .Select(x => new
                    {
                        Score = x.Time.ToString(),
                        x.Character,
                        x.Level,
                        x.Date,
                        x.Time,
                        UserId = x.Userid.ToString(),
                        x.Username,
                        x.HardcoreEnabled,
                        x.EnemiesEnabled,
                        x.TrafficEnabled,
                        x.EnemiesKilled,
                        x.Platform
                    })
                    .OrderBy(x => x.Time)
                    .Skip(page * 10)
                    .Take(results)
                    .ToListAsync();
                list = highscores;
            }

            // consecutive shots highscore
            if (modeid == 14)
            {
                var highscores = await _context.Highscores
                    .Where(x => x.Modeid == modeid
                    && x.HardcoreEnabled == hardcore
                    && x.TrafficEnabled == traffic
                    && x.SniperEnabled == sniper
                    && x.EnemiesEnabled == enemies)
                    .Select(x => new
                    {
                        Score = x.ConsecutiveShots.ToString(),
                        x.Character,
                        x.Level,
                        x.Date,
                        Time = x.Time.ToString(),
                        UserId = x.Userid.ToString(),
                        x.ConsecutiveShots,
                        x.Username,
                        x.EnemiesEnabled,
                        x.HardcoreEnabled,
                        x.TrafficEnabled,
                        x.EnemiesKilled,
                        x.Platform
                    })
                    .OrderByDescending(x => x.ConsecutiveShots)
                    .Skip(page * 10)
                    .Take(results)
                    .ToListAsync();
                list = highscores;
            }

            // enemies killed highscore
            if (modeid == 20 || modeid == 21 || modeid == 22)
            {
                var highscores = (dynamic)null;
                if (hardcore == 0)
                {
                    highscores = await _context.Highscores
                        .Where(x => x.Modeid == modeid)
                        .Select(x => new
                        {
                            Score = x.EnemiesKilled.ToString(),
                            x.Character,
                            x.Level,
                            x.Date,
                            Time = x.Time.ToString(),
                            UserId = x.Userid.ToString(),
                            x.Username,
                            x.HardcoreEnabled,
                            x.EnemiesEnabled,
                            x.TrafficEnabled,
                            x.EnemiesKilled,
                            x.Platform
                        })
                        .OrderByDescending(x => x.EnemiesKilled)
                        .Skip(page * 10)
                        .Take(results)
                        .ToListAsync();

                    list = highscores;
                }
                else
                {
                    highscores = await _context.Highscores
                    .Where(x => x.Modeid == modeid
                    && x.HardcoreEnabled == hardcore
                    && x.TrafficEnabled == traffic
                    && x.SniperEnabled == sniper
                    && x.EnemiesEnabled == enemies)
                    .Select(x => new
                    {
                        Score = x.EnemiesKilled.ToString(),
                        x.Character,
                        x.Level,
                        x.Date,
                        Time = x.Time.ToString(),
                        UserId = x.Userid.ToString(),
                        x.Username,
                        x.HardcoreEnabled,
                        x.EnemiesEnabled,
                        x.TrafficEnabled,
                        x.EnemiesKilled,
                        x.Platform
                    })
                    .OrderByDescending(x => x.EnemiesKilled)
                    .Skip(page * 10)
                    .Take(results)
                    .ToListAsync();

                    list = highscores;
                }
            }
            if (list == null)
            {
                return NotFound();
            }
            return list;
        }

        //--------------------- HTTP GET  Modeid by Modeid - All  ---------------------------------------------------
        // GET: /api/highscores/modeid/{modeid}?hardcore={int}&traffic={int}&enemies={int}
        /// <summary>
        /// Get all high scores for specific game mode by mode id
        /// </summary>
        [HttpGet("modeid/all/{modeid}")]
        public async Task<ActionResult<IEnumerable<Object>>> GetHighScoreByModeIdForGameDisplayAll(int modeid,
            int page,
            int results)
        {
            ActionResult<IEnumerable<Object>> list = null;
            // totalpoints highscore
            if (modeid == 1 || (modeid > 14 && modeid < 20) || modeid == 23 || modeid == 24 || modeid == 26)
            {
                var highscores = await _context.Highscores
                    .Where(x => x.Modeid == modeid)
                    .Select(x => new
                    {
                        Score = x.TotalPoints.ToString(),
                        x.Character,
                        x.Level,
                        x.Date,
                        Time = x.Time.ToString(),
                        UserId = x.Userid.ToString(),
                        x.TotalPoints,
                        x.Username,
                        x.HardcoreEnabled,
                        x.EnemiesEnabled,
                        x.TrafficEnabled,
                        x.EnemiesKilled,
                        x.Platform
                    })
                    .OrderByDescending(x => x.TotalPoints)
                    .Skip(page * 10)
                    .Take(results)
                    .ToListAsync();
                list = highscores;
            }
            // maxshotmade highscore
            if ((modeid > 1 && modeid < 5))
            {
                var highscores = await _context.Highscores
                    .Where(x => x.Modeid == modeid)
                    .Select(x => new
                    {
                        Score = x.MaxShotMade.ToString(),
                        x.Character,
                        x.Level,
                        x.Date,
                        Time = x.Time.ToString(),
                        UserId = x.Userid.ToString(),
                        x.MaxShotMade,
                        x.Username,
                        x.HardcoreEnabled,
                        x.EnemiesEnabled,
                        x.TrafficEnabled,
                        x.EnemiesKilled,
                        x.Platform
                    })
                    .OrderByDescending(x => x.MaxShotMade)
                    .Skip(page * 10)
                    .Take(results)
                    .ToListAsync();
                list = highscores;
            }
            // totaldistance highscore
            if (modeid == 6)
            {
                var highscores = await _context.Highscores
                    .Where(x => x.Modeid == modeid)
                    .Select(x => new
                    {
                        Score = x.TotalDistance.ToString(),
                        x.Character,
                        x.Level,
                        x.Date,
                        Time = x.Time.ToString(),
                        UserId = x.Userid.ToString(),
                        x.TotalDistance,
                        x.Username,
                        x.HardcoreEnabled,
                        x.EnemiesEnabled,
                        x.TrafficEnabled,
                        x.EnemiesKilled,
                        x.Platform
                    })
                    .OrderByDescending(x => x.TotalDistance)
                    .Skip(page * 10)
                    .Take(results)
                    .ToListAsync();

                list = highscores;
            }
            // time highscore
            if ((modeid > 6 && modeid < 10) || modeid == 25)
            {
                var highscores = await _context.Highscores
                    .Where(x => x.Modeid == modeid)
                    .Select(x => new
                    {
                        Score = x.Time.ToString(),
                        x.Character,
                        x.Level,
                        x.Date,
                        x.Time,
                        UserId = x.Userid.ToString(),
                        x.Username,
                        x.HardcoreEnabled,
                        x.EnemiesEnabled,
                        x.TrafficEnabled,
                        x.EnemiesKilled,
                        x.Platform
                    })
                    .OrderBy(x => x.Time)
                    .Skip(page * 10)
                    .Take(results)
                    .ToListAsync();
                list = highscores;
            }

            // consecutive shots highscore
            if (modeid == 14)
            {
                var highscores = await _context.Highscores
                    .Where(x => x.Modeid == modeid)
                    .Select(x => new
                    {
                        Score = x.ConsecutiveShots.ToString(),
                        x.Character,
                        x.Level,
                        x.Date,
                        Time = x.Time.ToString(),
                        UserId = x.Userid.ToString(),
                        x.ConsecutiveShots,
                        x.Username,
                        x.EnemiesEnabled,
                        x.HardcoreEnabled,
                        x.TrafficEnabled,
                        x.EnemiesKilled,
                        x.Platform
                    })
                    .OrderByDescending(x => x.ConsecutiveShots)
                    .Skip(page * 10)
                    .Take(results)
                    .ToListAsync();
                list = highscores;
            }

            // enemies killed highscore
            if (modeid == 20 || modeid == 21 || modeid == 22)
            {

                var highscores = await _context.Highscores
                    .Where(x => x.Modeid == modeid)
                    .Select(x => new
                    {
                        Score = x.EnemiesKilled.ToString(),
                        x.Character,
                        x.Level,
                        x.Date,
                        Time = x.Time.ToString(),
                        UserId = x.Userid.ToString(),
                        x.Username,
                        x.HardcoreEnabled,
                        x.EnemiesEnabled,
                        x.TrafficEnabled,
                        x.EnemiesKilled,
                        x.Platform
                    })
                    .OrderByDescending(x => x.EnemiesKilled)
                    .Skip(page * 10)
                    .Take(results)
                    .ToListAsync();

                list = highscores;
            }
            if (list == null)
            {
                return NotFound();
            }
            return list;
        }

        //--------------------- HTTP PUT ---------------------------------------------------
        // PUT: api/Highscores/scoreid/5
        /// <summary>
        /// Insert high score or replace if already exists
        /// </summary>
        [Authorize]
        [HttpPut("scoreid/{scoreid}")]
        public async Task<IActionResult> PutHighscore(string scoreid, Highscore highscores)
        {
            if (scoreid != highscores.Scoreid)
            {
                return BadRequest();
            }

            if (!TryGetCallerUserid(out int callerUserid) || highscores.Userid != callerUserid)
            {
                return Forbid();
            }

            _context.Entry(highscores).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScoreIdExists(scoreid))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        //--------------------- HTTP POST Unsubmitted Highscores ---------------------------------------------------
        // POST: api/Highscores
        /// <summary>
        /// Create new high score
        /// </summary>
        /// 
        [Authorize]
        [EnableCors("ApiCors")]
        [HttpPost]
        [Route("unsubmitted")]
        public async Task<ActionResult<List<Highscore>>> PostUnSubmittedHighscore([FromBody] List<Highscore> highscores)
        {
            if (highscores == null) { return BadRequest(); }

            if (!TryGetCallerUserid(out int callerUserid) || !await _context.Users.AnyAsync(u => u.Userid == callerUserid))
            {
                return Forbid();
            }

            // one batched lookup instead of two queries per item (was N+1 before)
            var incomingScoreIds = highscores.Select(h => h.Scoreid).ToList();
            var existingScoreIds = (await _context.Highscores
                .Where(e => incomingScoreIds.Contains(e.Scoreid))
                .Select(e => e.Scoreid)
                .ToListAsync())
                .ToHashSet();

            List<Highscore> list = new List<Highscore>();

            foreach (var highscore in highscores)
            {
                // skip (not abort) anything that's a duplicate, missing a username, or doesn't
                // belong to the authenticated caller - one bad item shouldn't drop the rest of
                // the batch, which is what the previous "break" did.
                if (existingScoreIds.Contains(highscore.Scoreid)
                    || string.IsNullOrEmpty(highscore.Username)
                    || highscore.Userid != callerUserid)
                {
                    continue;
                }

                updateModeName(highscore);
                _context.Highscores.Add(highscore);
                list.Add(highscore);
            }

            await _context.SaveChangesAsync();
            return list;
        }

        //--------------------- HTTP POST Highscore ---------------------------------------------------
        // POST: api/Highscores
        /// <summary>
        /// Create new high score
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Highscore>> PostHighscore([FromBody] Highscore highscore)
        {
            if (!TryGetCallerUserid(out int callerUserid) || highscore.Userid != callerUserid)
            {
                return Forbid();
            }

            // check if unique scoreid already exists in database
            if (await _context.Highscores.AnyAsync(e => e.Scoreid == highscore.Scoreid))
            {
                return Conflict();
            }
            // if empty Username or userid NOT in user table
            if (string.IsNullOrEmpty(highscore.Username) || !await _context.Users.AnyAsync(e => e.Userid == highscore.Userid))
            {
                return BadRequest();
            }

            updateModeName(highscore);
            _context.Highscores.Add(highscore);
            await _context.SaveChangesAsync();

            // update serverstats
            ServerStatsController server = new ServerStatsController(_context);
            server.getServerStats();

            return CreatedAtAction(nameof(GetAllHighscores), new { id = highscore.Id }, highscore);
        }

        //--------------------- HTTP DELETE HighScore ---------------------------------------------------
        /// <summary>
        /// Delete high score by score id
        /// </summary>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Highscore>> DeleteHighscore(int id)
        {
            var highscores = await _context.Highscores.FindAsync(id);
            if (highscores == null)
            {
                return NotFound();
            }

            if (!TryGetCallerUserid(out int callerUserid) || highscores.Userid != callerUserid)
            {
                return Forbid();
            }

            _context.Highscores.Remove(highscores);
            await _context.SaveChangesAsync();

            return highscores;
        }

        //--------------------- UTILITY FUNCTIONS ---------------------------------------------------
        //private bool HighscoreExists(string scoreid)
        //{
        //    return _context.Highscores.Any(e => e.Scoreid == scoreid);
        //}

        private int GetDatabaseIdByScoreId(string scoreid)
        {
            int id = _context.Highscores.Where(e => e.Scoreid == scoreid).FirstOrDefault().Id;

            return id;
        }

        private bool ScoreIdExists(string scoreid)
        {
            return _context.Highscores.Any(e => e.Scoreid == scoreid);
        }


        private bool PlatformExists(string platform)
        {
            return _context.Highscores.Any(e => e.Platform == platform);
        }

        // the JWT issued by TokenController carries the authenticated user's id as a "Userid"
        // claim - mutating endpoints use this to confirm the caller owns the score being touched.
        private bool TryGetCallerUserid(out int userid)
        {
            var claim = User.FindFirst("Userid")?.Value;
            return int.TryParse(claim, out userid);
        }

        //public bool isDev(string Username)
        //{
        //    // find any user that matches Username + isDev = 1;
        //    // this means, the Username is a dev account Username
        //    var isDev = _context.Users.Any(e => e.Username == Username && e.IsDev == 1);
        //    return isDev;
        //}

        /// <summary>
        /// Get # high scores for game mode by mode id
        /// </summary>
        [HttpGet("modeid/count")]
        public async Task<ActionResult<IEnumerable<Object>>> ModePlayedCount(int modeid)
        {
            var modeidList = _context.Highscores
                .GroupBy(e => e.Modeid)
                .Select(e => new { Modeid = e.Key, Count = e.Count() }).ToListAsync();
            return await modeidList;
        }

        //--------------------- HTTP GET  Modeid by Modeid ---------------------------------------------------
        // GET: /api/highscores/modeid/{modeid}?hardcore={int}&traffic={int}&enemies={int}
        /// <summary>
        /// Get # high scores for game mode by mode id with optional filters
        /// </summary>
        [HttpGet("modeid/count/{modeid}")]
        public ActionResult<object> GetHighScoreCountByModeId(int modeid,
            int hardcore,
            int traffic,
            int sniper,
            int enemies)
        {
            var count = _context.Highscores
                .Where(x => x.Modeid == modeid
                && x.HardcoreEnabled == hardcore
                && x.TrafficEnabled == traffic
                && x.SniperEnabled == sniper
                && x.EnemiesEnabled == enemies)
                .Select(x => x.Id)
                .Count();

            return count;
        }

        private void updateModeName(Highscore highscores)
        {
            System.Diagnostics.Debug.WriteLine("upDateModeName()");
            //foreach (Highscores h in highscores)

            // if modename is null, insert based on modeid
            if (String.IsNullOrEmpty(highscores.ModeName))
            {
                {
                    //System.Diagnostics.Debug.WriteLine("highscores.Modeid : " + highscores.Modeid);
                    switch (highscores.Modeid)
                    {
                        case 1:
                            highscores.ModeName = "Total Points";
                            break;
                        case 2:
                            highscores.ModeName = "Total 3 Pointers";
                            break;
                        case 3:
                            highscores.ModeName = "Total 4 Pointers";
                            break;
                        case 4:
                            highscores.ModeName = "Total 7 Pointers";
                            break;
                        case 6:
                            highscores.ModeName = "Total Distance";
                            break;
                        case 7:
                            highscores.ModeName = "Spot up some 3s";
                            break;
                        case 8:
                            highscores.ModeName = "Spot up some 4s";
                            break;
                        case 9:
                            highscores.ModeName = "Spot up some All";
                            break;
                        case 10:
                            highscores.ModeName = "Moneyball 3s";
                            break;
                        case 11:
                            highscores.ModeName = "Moneyball 4s";
                            break;
                        case 12:
                            highscores.ModeName = "Moneyball All";
                            break;
                        case 14:
                            highscores.ModeName = "Consecutive Shots";
                            break;
                        case 15:
                            highscores.ModeName = "In the Pocket";
                            break;
                        case 16:
                            highscores.ModeName = "3 point Contest";
                            break;
                        case 17:
                            highscores.ModeName = "4 point Contest";
                            break;
                        case 18:
                            highscores.ModeName = "All point Contest";
                            break;
                        case 19:
                            highscores.ModeName = "Points by Distance";
                            break;
                        case 20:
                            highscores.ModeName = "Bash up some Nerds";
                            break;
                        case 21:
                            highscores.ModeName = "Battle Royal";
                            break;
                        case 22:
                            highscores.ModeName = "Cage Match";
                            break;
                        case 23:
                            highscores.ModeName = "Versus";
                            break;
                        case 24:
                            highscores.ModeName = "7 point Contest";
                            break;
                        case 25:
                            highscores.ModeName = "Spot up some 7s";
                            break;
                        case 26:
                            highscores.ModeName = "Beat tha Computahs";
                            break;
                        case 98:
                            highscores.ModeName = "Arcade";
                            break;
                        case 99:
                            highscores.ModeName = "Free Play";
                            break;
                        default:
                            highscores.ModeName = "none";
                            break;
                    }
                    System.Diagnostics.Debug.WriteLine("highscores.ModeName : " + highscores.ModeName);
                }
                _context.SaveChanges();
            }
        }

        private static void HideHighScoreDetails(List<Highscore> highscores)
        {
            foreach (Highscore h in highscores)
            {
                h.Os = "*************";
                h.Scoreid = "*************";
                h.Device = "*************";
                h.Ipaddress = "*************";
            }
        }
    }
}


