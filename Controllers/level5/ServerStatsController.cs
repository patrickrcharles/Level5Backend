using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using level5Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Level5Backend.Models;

namespace level5Server.Models.level5
{

    [Route("[controller]")]
    public class ServerStatsController : Controller
    {
        public List<MostConsective> MostConsecutiveShotsUsernameList;
        public List<string> MostPlayedCharactersList;
        public List<MostPlayedLevel> MostPlayedLevelList;
        public List<LongestShot> LongestShotList;

        public struct MostConsective
        {
            string username;
            int numConsecutive;

            public MostConsective(string uname, int consec)
            {
                username = uname;
                numConsecutive = consec;
            }
        }

        public struct MostPlayedLevel
        {
            string level;
            int count;

            public MostPlayedLevel(string lvl, int cnt)
            {
                level = lvl;
                count = cnt;
            }
        }

        public struct LongestShot
        {
            string username;
            float distance;

            public LongestShot(string uname, float dist)
            {
                username = uname;
                distance = dist;
            }
        }

        private readonly Level5Context _context;
        public ServerStatsController(Level5Context context)
        {
            _context = context;
        }

        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Index()
        {
            //ServerStats serverStats = new ServerStats();
            //getMostPlayedCharacter(serverStats);
            //getMostConsecutivesShotsList(serverStats);
            //getPlayedLevelList(serverStats);
            //getLongestShotList(serverStats);

            //ViewBag.MostConsecutiveShotsUsernameList = MostConsecutiveShotsUsernameList;
            //ViewBag.MostPlayedCharactersList = MostPlayedCharactersList;
            //ViewBag.MostPlayedLevelList = MostPlayedLevelList;
            //ViewBag.LongestShotList = LongestShotList;

            return View(await _context.ServerStats.ToListAsync());
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public void getServerStats()
        {

            ServerStat serverStats = new ServerStat();
            serverStats.NumberOfUsers = _context.Users.Select(x => x.Userid).Count();
            //System.Diagnostics.Debug.WriteLine("serverStats.NumberOfUsers : " + serverStats.NumberOfUsers);
            serverStats.NumberOfTotalTimePlayed = _context.Highscores.Sum(x => x.Time);
            //System.Diagnostics.Debug.WriteLine("serverStats.NumberOfTotalTimePlayed : " + (serverStats.NumberOfTotalTimePlayed / 3600) + " days");
            serverStats.NumberOfGamesPlayed = _context.Highscores.Select(x => x.Id).Count();
            serverStats.NumberOfGamesPlayedHardcore = _context.Highscores.Where(x => x.HardcoreEnabled == 1).Select(x => x.Id).Count();
            serverStats.NumberofGamesPlayedTraffic = _context.Highscores.Where(x => x.TrafficEnabled == 1).Select(x => x.Id).Count();
            serverStats.NumberofGamesPlayedEnemies = _context.Highscores.Where(x => x.EnemiesEnabled == 1).Select(x => x.Id).Count();
            serverStats.NumberofGamesPlayedSniper = _context.Highscores.Where(x => x.SniperEnabled == 1).Select(x => x.Id).Count();
            serverStats.NumberOfTotal2ShotsMade = _context.Highscores.Select(x => x.TwoMade).Sum();
            serverStats.NumberOfTotal2ShotsAtt = _context.Highscores.Select(x => x.TwoAtt).Sum();
            serverStats.NumberOfTotal3ShotsMade = _context.Highscores.Select(x => x.ThreeMade).Sum();
            serverStats.NumberOfTotal3ShotsAtt = _context.Highscores.Select(x => x.ThreeAtt).Sum();
            serverStats.NumberOfTotal4ShotsMade = _context.Highscores.Select(x => x.FourMade).Sum();
            serverStats.NumberOfTotal4ShotsAtt = _context.Highscores.Select(x => x.FourAtt).Sum();
            serverStats.NumberOfTotal7ShotsMade = _context.Highscores.Select(x => x.SevenMade).Sum();
            serverStats.NumberOfTotal7ShotsAtt = _context.Highscores.Select(x => x.SevenAtt).Sum();
            serverStats.NumberOfTotalMoneyShotsMade = _context.Highscores.Select(x => x.MoneyBallMade).Sum();
            serverStats.NumberOfTotalMoneyShotsAtt = _context.Highscores.Select(x => x.MoneyBallAtt).Sum();
            serverStats.NumberOfTotalTotalShotsMade = _context.Highscores.Select(x => x.MaxShotMade).Sum();
            serverStats.NumberOfTotalTotalShotsAtt = _context.Highscores.Select(x => x.MaxShotAtt).Sum();
            serverStats.NumberOfTotalTotalPointsScored = _context.Highscores.Select(x => x.TotalPoints).Sum();
            serverStats.NumberOfTotalEnemiesKilled = _context.Highscores.Select(x => x.EnemiesKilled).Sum();

            getMostPlayedCharacter(serverStats);
            getMostConsecutivesShotsList(serverStats);
            getPlayedLevelList(serverStats);
            getLongestShotList(serverStats);

            _context.ServerStats.Add(serverStats);
            _context.SaveChanges();
        }

        private void getMostPlayedCharacter(ServerStat serverStats)
        {
            var mostPlayedChar = _context.Highscores
                .GroupBy(x => x.Character)
                .Select(x => new { character = x.Key, Count = x.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToList();

            serverStats.MostPlayedCharacter = mostPlayedChar.First().character;

            MostPlayedCharactersList = new List<string>();
            foreach (var s in mostPlayedChar)
            {
                MostPlayedCharactersList.Add(s.character);
            }
        }

        internal void getMostConsecutivesShotsList(ServerStat serverStats)
        {
            var mostConsecUsernames = _context.Highscores
                    .Select(x => new
                    {
                     x.Username,
                     x.ConsecutiveShots
                    })
                    .OrderByDescending(x => x.ConsecutiveShots)
                    .Take(10)
                    .ToList();

            serverStats.MostConsecutiveShots = mostConsecUsernames.First().ConsecutiveShots;
            serverStats.MostConsecutiveShotsUsername = mostConsecUsernames.First().Username;

            MostConsecutiveShotsUsernameList = new List<MostConsective>();
            foreach (var s in mostConsecUsernames)
            {
                MostConsecutiveShotsUsernameList.Add(new MostConsective(s.Username, s.ConsecutiveShots));
            }
        }

        internal void getPlayedLevelList(ServerStat serverStats)
        {
            var mostPlayedLevelList = _context.Highscores
                .GroupBy(x => x.Level)
                .Select(x => new { level = x.Key, Count = x.Count() })
                .OrderByDescending(x => x.Count)
                .Take(10)
                .ToList();

            serverStats.MostPlayedLevel = mostPlayedLevelList.First().level;

            MostPlayedLevelList = new List<MostPlayedLevel>();
            foreach (var s in mostPlayedLevelList)
            {
                //System.Diagnostics.Debug.WriteLine("s.Level : " + s.level);
                //System.Diagnostics.Debug.WriteLine("s.Count : " + s.Count);
                MostPlayedLevelList.Add(new MostPlayedLevel(s.level, s.Count));
            }
        }

        internal void getLongestShotList(ServerStat serverStats)
        {
            var longestShotList = _context.Highscores
                .Select(x => new { x.Username, x.LongestShot })
                .OrderByDescending(x => x.LongestShot)
                .Take(10)
                .ToList();

            serverStats.LongestShot = longestShotList.First().LongestShot;
            serverStats.LongestShotUsername = longestShotList.First().Username;

            LongestShotList = new List<LongestShot>();
            foreach (var s in longestShotList)
            {
                LongestShotList.Add(new LongestShot(s.Username, s.LongestShot));
            }
        }
    }
}
