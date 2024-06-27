using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using level5Server.Models;
using Level5Backend.Models;

namespace level5Server.Models.level5
{
    public class HighscoresController : Controller
    {
        private readonly Level5Context _context;
        private string previousSearchString;

        public HighscoresController(Level5Context context)
        {
            _context = context;
        }

        //// GET: Highscores
        //[Route("[controller]")]
        //public async Task<IActionResult> Index()
        //{
        //    //get last 50 scores
        //    return View(await _context.Highscores.OrderByDescending(x => x.Id).Skip(0).Take(50).ToListAsync());

        //}

        // GET: Highscores
        [Route("[controller]")]
        //[HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["DateSortParm"] = sortOrder == "date_asc" ? "date_desc" : "date_asc";
            ViewData["HardcoreSortParm"] = sortOrder == "hardcore_asc" ? "hardcore_desc" : "hardcore_asc";
            ViewData["UsernameSortParm"] = sortOrder == "username_asc" ? "username_desc" : "username_asc";
            ViewData["PlatformSortParm"] = sortOrder == "platform_asc" ? "platform_desc" : "platform_asc";
            ViewData["ModeSortParm"] = sortOrder == "mode_asc" ? "mode_desc" : "mode_asc";
            ViewData["CharacterSortParm"] = sortOrder == "character_asc" ? "character_desc" : "character_asc";
            ViewData["LevelSortParm"] = sortOrder == "level_asc" ? "level_desc" : "level_asc";
            ViewData["VersionSortParm"] = sortOrder == "version_asc" ? "version_desc" : "version_asc";
            ViewData["DifficultySortParm"] = sortOrder == "difficulty_asc" ? "difficulty_desc" : "difficulty_asc";
            ViewData["TimeSortParm"] = sortOrder == "time_asc" ? "time_desc" : "time_asc";
            ViewData["TotalPointsSortParm"] = sortOrder == "totalPoints_asc" ? "totalPoints_desc" : "totalPoints_asc";
            ViewData["LongestShotSortParm"] = sortOrder == "longestShot_asc" ? "longestShot_desc" : "longestShot_asc";
            ViewData["TotalDistanceSortParm"] = sortOrder == "totalDistance_asc" ? "totalDistance_desc" : "totalDistance_asc";
            ViewData["MaxShotMadeSortParm"] = sortOrder == "maxShotMade_asc" ? "maxShotMade_desc" : "maxShotMade_asc";
            ViewData["MaxShotAttSortParm"] = sortOrder == "maxShotAtt_asc" ? "maxShotAtt_desc" : "maxShotAtt_asc";
            ViewData["ConsecutiveShotsSortParm"] = sortOrder == "consecutiveShots_asc" ? "consecutiveShots_desc" : "consecutiveShots_asc";

            ViewData["TwoMadeSortParm"] = sortOrder == "twoMade_asc" ? "twoMade_desc" : "twoMade_asc";
            ViewData["TwoAttSortParm"] = sortOrder == "twoAtt_asc" ? "twoAtt_desc" : "twoAtt_asc";
            ViewData["ThreeMadeSortParm"] = sortOrder == "threeMade_asc" ? "threeMade_desc" : "threeMade_asc";
            ViewData["ThreeAttSortParm"] = sortOrder == "threeAtt_asc" ? "threeAtt_desc" : "threeAtt_asc";
            ViewData["FourMadeSortParm"] = sortOrder == "fourMade_asc" ? "fourMade_desc" : "fourMade_asc";
            ViewData["FourAttSortParm"] = sortOrder == "fourAtt_asc" ? "fourAtt_desc" : "fourAtt_asc";
            ViewData["SevenMadeSortParm"] = sortOrder == "sevenMade_asc" ? "sevenMade_desc" : "sevenMade_asc";
            ViewData["SevenAttSortParm"] = sortOrder == "sevenAtt_asc" ? "sevenAtt_desc" : "sevenAtt_asc";
            ViewData["BonusPointsSortParm"] = sortOrder == "bonusPoints_asc" ? "bonusPoints_desc" : "bonusPoints_asc";
            ViewData["MoneyBallMadeSortParm"] = sortOrder == "moneyBallMade_asc" ? "moneyBallMade_desc" : "moneyBallMade_asc";
            ViewData["MoneyBallAttSortParm"] = sortOrder == "moneyBallAtt_asc" ? "moneyBallAtt_desc" : "moneyBallAtt_asc";
            ViewData["TrafficSortParm"] = sortOrder == "traffic_asc" ? "traffic_desc" : "traffic_asc";
            ViewData["EnemiesSortParm"] = sortOrder == "enemies_asc" ? "enemies_desc" : "enemies_asc";
            ViewData["EnemiesKilledSortParm"] = sortOrder == "enemiesKilled_asc" ? "enemiesKilled_desc" : "enemiesKilled_asc";
            ViewData["SniperSortParm"] = sortOrder == "sniper_asc" ? "sniper_desc" : "sniper_asc";
            ViewData["SniperModeSortParm"] = sortOrder == "sniperMode_asc" ? "sniperMode_desc" : "sniperMode_asc";
            ViewData["SniperModeNameSortParm"] = sortOrder == "sniperModeName_asc" ? "sniperModeName_desc" : "sniperModeName_asc";
            ViewData["SniperHitsSortParm"] = sortOrder == "sniperHits_asc" ? "sniperHits_desc" : "sniperHits_asc";
            ViewData["SniperShotsSortParm"] = sortOrder == "sniperShots_asc" ? "sniperShots_desc" : "sniperShots_asc";

            //ViewData["P1TotalPointsSortParm"] = sortOrder == "p1TotalPoints_asc" ? "p1TotalPoints_desc" : "p1TotalPoints_asc";
            //ViewData["P2TotalPointsSortParm"] = sortOrder == "p2TotalPoints_asc" ? "p2TotalPoints_desc" : "p2TotalPoints_asc";
            //ViewData["P3TotalPointsSortParm"] = sortOrder == "p3TotalPoints_asc" ? "p3TotalPoints_desc" : "p3TotalPoints_asc";
            //ViewData["P4TotalPointsSortParm"] = sortOrder == "p4TotalPoints_asc" ? "p4TotalPoints_desc" : "p4TotalPoints_asc";

            ViewData["FirstPlaceSortParm"] = sortOrder == "firstPlace_asc" ? "firstPlace_desc" : "firstPlace_asc";
            ViewData["SecondPlaceSortParm"] = sortOrder == "secondPlace_asc" ? "secondPlace_desc" : "secondPlace_asc";
            ViewData["ThirdPaceSortParm"] = sortOrder == "thirdPlace_asc" ? "thirdPlace_desc" : "thirdPlace_asc";
            ViewData["FourthPlaceSortParm"] = sortOrder == "fourthPlace_asc" ? "fourthPlace_desc" : "fourthPlace_asc";

            //ViewData["P1IsCpuSortParm"] = sortOrder == "p1IsCpu_asc" ? "p1IsCpu_desc" : "p1IsCpu_asc";
            //ViewData["P2IsCpuSortParm"] = sortOrder == "p2IsCpu_asc" ? "p2IsCpu_desc" : "p2IsCpu_asc";
            //ViewData["P3IsCpuSortParm"] = sortOrder == "p3IsCpu_asc" ? "p3IsCpu_desc" : "p3IsCpu_asc";
            //ViewData["P4IsCpuSortParm"] = sortOrder == "p4IsCpu_asc" ? "p4IsCpu_desc" : "p4IsCpu_asc";

            //ViewData["NumPlayerSortParm"] = sortOrder == "numPlayers_asc" ? "numPlayers_desc" : "numPlayers_asc";


            var highscores = from h in _context.Highscores
                             select h;

            System.Diagnostics.Debug.WriteLine(highscores.Count());
            System.Diagnostics.Debug.WriteLine("searchString : "+ searchString);
            // if no search is submitted
            //if (String.IsNullOrEmpty(searchString))
            //{
            //    previousSearchString = "";
            //}
            // if search term is entered
            if (!String.IsNullOrEmpty(searchString))
            {
                previousSearchString = searchString; // save search in case of sorting
                highscores = highscores.Where
                    (s => s.Username.Contains(searchString) || s.Platform.Contains(searchString)
                    || s.Character.Contains(searchString) || s.Level.Contains(searchString)
                    || s.Version.Contains(searchString) || s.ModeName.Contains(searchString));
            }
            // if search term is not entered but sort has been clicked
            //if (!String.IsNullOrEmpty(previousSearchString) && !String.IsNullOrEmpty(sortOrder))
            //{
            //    highscores = highscores.Where
            //        (s => s.UserName.Contains(previousSearchString) || s.Platform.Contains(previousSearchString)
            //        || s.Character.Contains(previousSearchString) || s.Level.Contains(previousSearchString)
            //        || s.Version.Contains(previousSearchString));
            //}
            highscores = sortHighscores(sortOrder, highscores);
            return View(await highscores.ToListAsync());
        }

        [Route("[controller]/id/{id?}")]
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        // GET: Highscores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var highscores = await _context.Highscores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (highscores == null)
            {
                return NotFound();
            }

            return View(highscores);
        }

        //[Route("[controller]/create")]
        //// GET: Highscores/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        // POST: Highscores/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Userid,Modeid,Characterid,Levelid,Character,Level,Os,Version,Date,Time,TotalPoints,LongestShot,TotalDistance,MaxShotMade,MaxShotAtt,ConsecutiveShots,TrafficEnabled,HardcoreEnabled,EnemiesKilled,Platform,Device,Ipaddress,Scoreid,threeMade,TwoAtt,ThreeMade,ThreeAtt,FourMade,FourAtt,SevenMade,SevenAtt")] Highscores highscores)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(highscores);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(highscores);
        //}

        // GET: Highscores/Edit/5
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var highscores = await _context.Highscores.FindAsync(id);
            if (highscores == null)
            {
                return NotFound();
            }
            return View(highscores);
        }

        // POST: Highscores/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Userid,Modeid,Characterid,Levelid,Character,Level,Os,Version,Date,Time,TotalPoints,LongestShot,TotalDistance,MaxShotMade,MaxShotAtt,ConsecutiveShots,TrafficEnabled,HardcoreEnabled,EnemiesKilled,Platform,Device,Ipaddress,Scoreid,threeMade,TwoAtt,ThreeMade,ThreeAtt,FourMade,FourAtt,SevenMade,SevenAtt")] Highscores highscores)
        //{
        //    if (id != highscores.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(highscores);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!HighscoresExists(highscores.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(highscores);
        //}

        // GET: Highscores/Delete/5
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var highscores = await _context.Highscores
                .FirstOrDefaultAsync(m => m.Id == id);
            if (highscores == null)
            {
                return NotFound();
            }

            return View(highscores);
        }

        // POST: Highscores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var highscores = await _context.Highscores.FindAsync(id);
            _context.Highscores.Remove(highscores);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        internal bool HighscoresExists(int id)
        {
            return _context.Highscores.Any(e => e.Id == id);
        }

        internal static IQueryable<Highscore> sortHighscores(string sortOrder, IQueryable<Highscore> highscores)
        {
            if (String.IsNullOrEmpty(sortOrder))
            {
                highscores = highscores.OrderByDescending(x => x.Id).Skip(0).Take(50);
                return highscores;
            }
            if (sortOrder.Contains("desc"))
            {
                if (String.IsNullOrEmpty(sortOrder))
                {
                    switch (sortOrder)
                    {
                        case "date_desc":
                            highscores = highscores.OrderBy(s => s.Id).Take(50);
                            break;
                        case "hardcore_desc":
                            highscores = highscores.OrderBy(s => s.HardcoreEnabled).Take(50);
                            break;
                        case "username_desc":
                            highscores = highscores.OrderBy(s => s.Username).Take(50);
                            break;
                        case "platform_desc":
                            highscores = highscores.OrderBy(s => s.Platform).Take(50);
                            break;
                        case "mode_desc":
                            highscores = highscores.OrderBy(s => s.ModeName).Take(50);
                            break;
                        case "character_desc":
                            highscores = highscores.OrderBy(s => s.Character).Take(50);
                            break;
                        case "level_desc":
                            highscores = highscores.OrderBy(s => s.Level).Take(50);
                            break;
                        case "version_desc":
                            highscores = highscores.OrderBy(s => s.Version).Take(50);
                            break;
                        case "time_desc":
                            highscores = highscores.OrderBy(s => s.Time).Take(50);
                            break;
                        case "difficulty_desc":
                            highscores = highscores.OrderBy(s => s.Difficulty).Take(50);
                            break;
                        case "totalPoints_desc":
                            highscores = highscores.OrderBy(s => s.TotalPoints).Take(50);
                            break;
                        case "longestShot_desc":
                            highscores = highscores.OrderBy(s => s.LongestShot).Take(50);
                            break;
                        case "totalDistance_desc":
                            highscores = highscores.OrderBy(s => s.TotalDistance).Take(50);
                            break;
                        case "maxShotMade_desc":
                            highscores = highscores.OrderBy(s => s.MaxShotMade).Take(50);
                            break;
                        case "maxShotAtt_desc":
                            highscores = highscores.OrderBy(s => s.MaxShotAtt).Take(50);
                            break;
                        case "consecutiveShots_desc":
                            highscores = highscores.OrderBy(s => s.ConsecutiveShots).Take(50);
                            break;
                        case "twoMade_desc":
                            highscores = highscores.OrderBy(s => s.TwoMade).Take(50);
                            break;
                        case "twoAtt_desc":
                            highscores = highscores.OrderBy(s => s.TwoAtt).Take(50);
                            break;
                        case "threeMade_desc":
                            highscores = highscores.OrderBy(s => s.ThreeMade).Take(50);
                            break;
                        case "threeAtt_desc":
                            highscores = highscores.OrderBy(s => s.ThreeAtt).Take(50);
                            break;
                        case "fourMade_desc":
                            highscores = highscores.OrderBy(s => s.FourMade).Take(50);
                            break;
                        case "fourAtt_desc":
                            highscores = highscores.OrderBy(s => s.FourAtt).Take(50);
                            break;
                        case "sevenMade_desc":
                            highscores = highscores.OrderBy(s => s.SevenMade).Take(50);
                            break;
                        case "sevenAtt_desc":
                            highscores = highscores.OrderBy(s => s.FourAtt).Take(50);
                            break;
                        case "bonusPoints_desc":
                            highscores = highscores.OrderBy(s => s.BonusPoints).Take(50);
                            break;
                        case "moneyBallMade_desc":
                            highscores = highscores.OrderBy(s => s.MoneyBallMade).Take(50);
                            break;
                        case "moneyBallAtt_desc":
                            highscores = highscores.OrderBy(s => s.MoneyBallAtt).Take(50);
                            break;
                        case "traffic_desc":
                            highscores = highscores.OrderBy(s => s.TrafficEnabled).Take(50);
                            break;
                        case "enemies_desc":
                            highscores = highscores.OrderBy(s => s.EnemiesEnabled).Take(50);
                            break;
                        case "enemiesKilled_desc":
                            highscores = highscores.OrderBy(s => s.EnemiesKilled).Take(50);
                            break;
                        case "sniper_desc":
                            highscores = highscores.OrderBy(s => s.SniperEnabled).Take(50);
                            break;
                        case "sniperMode_desc":
                            highscores = highscores.OrderBy(s => s.SniperMode).Take(50);
                            break;
                        case "sniperModeName_desc":
                            highscores = highscores.OrderBy(s => s.SniperModeName).Take(50);
                            break;
                        case "sniperHits_desc":
                            highscores = highscores.OrderBy(s => s.SniperHits).Take(50);
                            break;
                        case "sniperShots_desc":
                            highscores = highscores.OrderBy(s => s.SniperShots).Take(50);
                            break;
                        default:
                            highscores = highscores.OrderByDescending(x => x.Id).Skip(0).Take(50);
                            break;
                    }
                }
            }
            if (sortOrder.Contains("asc"))
            {
                switch (sortOrder)
                {
                    case "date_asc":
                        highscores = highscores.OrderByDescending(s => s.Id).Take(50);
                        break;
                    case "hardcore_asc":
                        highscores = highscores.OrderByDescending(s => s.HardcoreEnabled).Take(50);
                        break;
                    case "username_asc":
                        highscores = highscores.OrderByDescending(s => s.Username).Take(50);
                        break;
                    case "platform_asc":
                        highscores = highscores.OrderByDescending(s => s.Platform).Take(50);
                        break;
                    case "mode_asc":
                        highscores = highscores.OrderByDescending(s => s.ModeName).Take(50);
                        break;
                    case "character_asc":
                        highscores = highscores.OrderByDescending(s => s.Character).Take(50);
                        break;
                    case "level_asc":
                        highscores = highscores.OrderByDescending(s => s.Level).Take(50);
                        break;
                    case "version_asc":
                        highscores = highscores.OrderByDescending(s => s.Version).Take(50);
                        break;
                    case "time_asc":
                        highscores = highscores.OrderByDescending(s => s.Time).Take(50);
                        break;
                    case "difficulty_asc":
                        highscores = highscores.OrderByDescending(s => s.Difficulty).Take(50);
                        break;
                    case "totalPoints_asc":
                        highscores = highscores.OrderByDescending(s => s.TotalPoints).Take(50);
                        break;
                    case "longestShot_asc":
                        highscores = highscores.OrderByDescending(s => s.LongestShot).Take(50);
                        break;
                    case "totalDistance_asc":
                        highscores = highscores.OrderByDescending(s => s.TotalDistance).Take(50);
                        break;
                    case "maxShotMade_asc":
                        highscores = highscores.OrderByDescending(s => s.MaxShotMade).Take(50);
                        break;
                    case "maxShotAtt_asc":
                        highscores = highscores.OrderByDescending(s => s.MaxShotAtt).Take(50);
                        break;
                    case "consecutiveShots_asc":
                        highscores = highscores.OrderByDescending(s => s.ConsecutiveShots).Take(50);
                        break;
                    case "twoMade_asc":
                        highscores = highscores.OrderByDescending(s => s.TwoMade).Take(50);
                        break;
                    case "twoAtt_asc":
                        highscores = highscores.OrderByDescending(s => s.TwoAtt).Take(50);
                        break;
                    case "threeMade_asc":
                        highscores = highscores.OrderByDescending(s => s.ThreeMade).Take(50);
                        break;
                    case "threeAtt_asc":
                        highscores = highscores.OrderByDescending(s => s.ThreeAtt).Take(50);
                        break;
                    case "fourMade_asc":
                        highscores = highscores.OrderByDescending(s => s.FourMade).Take(50);
                        break;
                    case "fourAtt_asc":
                        highscores = highscores.OrderByDescending(s => s.FourAtt).Take(50);
                        break;
                    case "sevenMade_asc":
                        highscores = highscores.OrderByDescending(s => s.SevenMade).Take(50);
                        break;
                    case "sevenAtt_asc":
                        highscores = highscores.OrderByDescending(s => s.FourAtt).Take(50);
                        break;
                    case "bonusPoints_asc":
                        highscores = highscores.OrderByDescending(s => s.BonusPoints).Take(50);
                        break;
                    case "moneyBallMade_asc":
                        highscores = highscores.OrderByDescending(s => s.MoneyBallMade).Take(50);
                        break;
                    case "moneyBallAtt_asc":
                        highscores = highscores.OrderByDescending(s => s.MoneyBallAtt).Take(50);
                        break;
                    case "traffic_asc":
                        highscores = highscores.OrderByDescending(s => s.TrafficEnabled).Take(50);
                        break;
                    case "enemies_asc":
                        highscores = highscores.OrderByDescending(s => s.EnemiesEnabled).Take(50);
                        break;
                    case "enemiesKilled_asc":
                        highscores = highscores.OrderByDescending(s => s.EnemiesKilled).Take(50);
                        break;
                    case "sniper_asc":
                        highscores = highscores.OrderByDescending(s => s.SniperEnabled).Take(50);
                        break;
                    case "sniperMode_asc":
                        highscores = highscores.OrderByDescending(s => s.SniperMode).Take(50);
                        break;
                    case "sniperModeName_asc":
                        highscores = highscores.OrderByDescending(s => s.SniperModeName).Take(50);
                        break;
                    case "sniperHits_asc":
                        highscores = highscores.OrderByDescending(s => s.SniperHits).Take(50);
                        break;
                    case "sniperShots_asc":
                        highscores = highscores.OrderByDescending(s => s.SniperShots).Take(50);
                        break;
                    default:
                        highscores = highscores.OrderByDescending(x => x.Id).Skip(0).Take(50);
                        break;
                }
            }
            return highscores;
        }
    }
}
