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

        // Which stat a mode id is ranked by. Centralizes the mode-id groupings that used to be
        // duplicated across the Filtered/All endpoints below (they were kept in exact sync by hand);
        // updateModeName's per-modeid display-name switch is a separate, legitimately 1:1 lookup
        // (several modeids sharing a metric still get distinct display names) so it isn't folded in.
        private enum ScoreMetric { TotalPoints, MaxShotMade, TotalDistance, Time, ConsecutiveShots, EnemiesKilled }

        private static ScoreMetric? GetScoreMetric(int modeid) => modeid switch
        {
            1 => ScoreMetric.TotalPoints,
            > 14 and < 20 => ScoreMetric.TotalPoints,
            23 or 24 or 26 => ScoreMetric.TotalPoints,
            > 1 and < 5 => ScoreMetric.MaxShotMade,
            6 => ScoreMetric.TotalDistance,
            > 6 and < 10 => ScoreMetric.Time,
            25 => ScoreMetric.Time,
            14 => ScoreMetric.ConsecutiveShots,
            20 or 21 or 22 => ScoreMetric.EnemiesKilled,
            _ => null
        };

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
            // results was previously passed straight into Take() with no upper bound, and page
            // could go negative into Skip() - both directly attacker-controlled on an unauthenticated
            // endpoint. Clamped the same way GetAllHighscores already is.
            int take = Math.Clamp(results, 1, 200);
            int skip = Math.Max(page, 0) * 10;

            var task = QueryHighScoresByMetric(modeid, hardcore, traffic, sniper, enemies, skip, take);
            if (task == null)
            {
                return NotFound();
            }

            return await task;
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
            int take = Math.Clamp(results, 1, 200);
            int skip = Math.Max(page, 0) * 10;

            var task = QueryHighScoresByMetric(modeid, null, null, null, null, skip, take);
            if (task == null)
            {
                return NotFound();
            }

            return await task;
        }

        // Null filter arguments mean "unfiltered" (the "All" endpoint above); non-null values come
        // from the "Filtered" endpoint. Returns null when modeid doesn't map to any known metric.
        private Task<List<object>>? QueryHighScoresByMetric(int modeid, int? hardcore, int? traffic, int? sniper, int? enemies, int skip, int take)
        {
            return GetScoreMetric(modeid) switch
            {
                ScoreMetric.TotalPoints => GetByTotalPoints(modeid, hardcore, traffic, sniper, enemies, skip, take),
                ScoreMetric.MaxShotMade => GetByMaxShotMade(modeid, hardcore, traffic, sniper, enemies, skip, take),
                ScoreMetric.TotalDistance => GetByTotalDistance(modeid, hardcore, traffic, sniper, enemies, skip, take),
                ScoreMetric.Time => GetByTime(modeid, hardcore, traffic, sniper, enemies, skip, take),
                ScoreMetric.ConsecutiveShots => GetByConsecutiveShots(modeid, hardcore, traffic, sniper, enemies, skip, take),
                ScoreMetric.EnemiesKilled => GetByEnemiesKilled(modeid, hardcore, traffic, sniper, enemies, skip, take),
                _ => null
            };
        }

        private static IQueryable<Highscore> ApplyOptionalFilters(IQueryable<Highscore> query, int? hardcore, int? traffic, int? sniper, int? enemies)
        {
            if (hardcore.HasValue) query = query.Where(x => x.HardcoreEnabled == hardcore.Value);
            if (traffic.HasValue) query = query.Where(x => x.TrafficEnabled == traffic.Value);
            if (sniper.HasValue) query = query.Where(x => x.SniperEnabled == sniper.Value);
            if (enemies.HasValue) query = query.Where(x => x.EnemiesEnabled == enemies.Value);
            return query;
        }

        private async Task<List<object>> GetByTotalPoints(int modeid, int? hardcore, int? traffic, int? sniper, int? enemies, int skip, int take)
        {
            var query = ApplyOptionalFilters(_context.Highscores.Where(x => x.Modeid == modeid), hardcore, traffic, sniper, enemies);
            var highscores = await query
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
                .Skip(skip)
                .Take(take)
                .ToListAsync();
            return highscores.Cast<object>().ToList();
        }

        private async Task<List<object>> GetByMaxShotMade(int modeid, int? hardcore, int? traffic, int? sniper, int? enemies, int skip, int take)
        {
            var query = ApplyOptionalFilters(_context.Highscores.Where(x => x.Modeid == modeid), hardcore, traffic, sniper, enemies);
            var highscores = await query
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
                .Skip(skip)
                .Take(take)
                .ToListAsync();
            return highscores.Cast<object>().ToList();
        }

        private async Task<List<object>> GetByTotalDistance(int modeid, int? hardcore, int? traffic, int? sniper, int? enemies, int skip, int take)
        {
            var query = ApplyOptionalFilters(_context.Highscores.Where(x => x.Modeid == modeid), hardcore, traffic, sniper, enemies);
            var highscores = await query
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
                .Skip(skip)
                .Take(take)
                .ToListAsync();
            return highscores.Cast<object>().ToList();
        }

        private async Task<List<object>> GetByTime(int modeid, int? hardcore, int? traffic, int? sniper, int? enemies, int skip, int take)
        {
            var query = ApplyOptionalFilters(_context.Highscores.Where(x => x.Modeid == modeid), hardcore, traffic, sniper, enemies);
            var highscores = await query
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
                .Skip(skip)
                .Take(take)
                .ToListAsync();
            return highscores.Cast<object>().ToList();
        }

        private async Task<List<object>> GetByConsecutiveShots(int modeid, int? hardcore, int? traffic, int? sniper, int? enemies, int skip, int take)
        {
            var query = ApplyOptionalFilters(_context.Highscores.Where(x => x.Modeid == modeid), hardcore, traffic, sniper, enemies);
            var highscores = await query
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
                .Skip(skip)
                .Take(take)
                .ToListAsync();
            return highscores.Cast<object>().ToList();
        }

        private async Task<List<object>> GetByEnemiesKilled(int modeid, int? hardcore, int? traffic, int? sniper, int? enemies, int skip, int take)
        {
            IQueryable<Highscore> query = _context.Highscores.Where(x => x.Modeid == modeid);

            // Mirrors the original behavior exactly: hardcore==0 (the Filtered endpoint's default)
            // only filters by modeid; any other hardcore value applies the rest of the filters too.
            // The "All" endpoint passes hardcore: null and always gets the unfiltered query.
            if (hardcore.HasValue && hardcore.Value != 0)
            {
                query = ApplyOptionalFilters(query, hardcore, traffic, sniper, enemies);
            }

            var highscores = await query
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
                .Skip(skip)
                .Take(take)
                .ToListAsync();
            return highscores.Cast<object>().ToList();
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
            string? callerIp = HttpContext.Connection.RemoteIpAddress?.ToString();

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

                // server-derived, never trust whatever the client put in the request body
                highscore.Ipaddress = callerIp;
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

            // server-derived, never trust whatever the client put in the request body
            highscore.Ipaddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            updateModeName(highscore);
            _context.Highscores.Add(highscore);
            await _context.SaveChangesAsync();

            // ServerStats is recomputed periodically by ServerStatsBackgroundService, not inline here -
            // it used to run synchronously on every single POST, scanning the entire Highscores table.

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
        private bool ScoreIdExists(string scoreid)
        {
            return _context.Highscores.Any(e => e.Scoreid == scoreid);
        }

        // the JWT issued by TokenController carries the authenticated user's id as a "Userid"
        // claim - mutating endpoints use this to confirm the caller owns the score being touched.
        private bool TryGetCallerUserid(out int userid)
        {
            var claim = User.FindFirst("Userid")?.Value;
            return int.TryParse(claim, out userid);
        }

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


