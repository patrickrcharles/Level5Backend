using Level5Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Level5Backend.Services
{
    // Recomputes the ServerStats snapshot from the full Highscores table. This used to run
    // synchronously inline on every single highscore POST - full-table Sum/GroupBy scans on every
    // write meant every score submission got slower as the table grew, and the sync EF calls
    // blocked a thread-pool thread instead of yielding. It now only runs periodically, off the
    // request path, via ServerStatsBackgroundService.
    public class ServerStatsService : IServerStatsService
    {
        private readonly Level5Context _context;

        public ServerStatsService(Level5Context context)
        {
            _context = context;
        }

        public async Task RecomputeAsync(CancellationToken cancellationToken = default)
        {
            // Nothing to aggregate yet - avoids the "First() on empty sequence" crash the previous
            // implementation would hit if this ever ran against a fresh/empty database.
            if (!await _context.Highscores.AnyAsync(cancellationToken))
            {
                return;
            }

            var serverStats = new ServerStat
            {
                NumberOfUsers = await _context.Users.CountAsync(cancellationToken),
                NumberOfTotalTimePlayed = await _context.Highscores.SumAsync(x => x.Time, cancellationToken),
                NumberOfGamesPlayed = await _context.Highscores.CountAsync(cancellationToken),
                NumberOfGamesPlayedHardcore = await _context.Highscores.CountAsync(x => x.HardcoreEnabled == 1, cancellationToken),
                NumberofGamesPlayedTraffic = await _context.Highscores.CountAsync(x => x.TrafficEnabled == 1, cancellationToken),
                NumberofGamesPlayedEnemies = await _context.Highscores.CountAsync(x => x.EnemiesEnabled == 1, cancellationToken),
                NumberofGamesPlayedSniper = await _context.Highscores.CountAsync(x => x.SniperEnabled == 1, cancellationToken),
                NumberOfTotal2ShotsMade = await _context.Highscores.SumAsync(x => x.TwoMade, cancellationToken),
                NumberOfTotal2ShotsAtt = await _context.Highscores.SumAsync(x => x.TwoAtt, cancellationToken),
                NumberOfTotal3ShotsMade = await _context.Highscores.SumAsync(x => x.ThreeMade, cancellationToken),
                NumberOfTotal3ShotsAtt = await _context.Highscores.SumAsync(x => x.ThreeAtt, cancellationToken),
                NumberOfTotal4ShotsMade = await _context.Highscores.SumAsync(x => x.FourMade, cancellationToken),
                NumberOfTotal4ShotsAtt = await _context.Highscores.SumAsync(x => x.FourAtt, cancellationToken),
                NumberOfTotal7ShotsMade = await _context.Highscores.SumAsync(x => x.SevenMade, cancellationToken),
                NumberOfTotal7ShotsAtt = await _context.Highscores.SumAsync(x => x.SevenAtt, cancellationToken),
                NumberOfTotalMoneyShotsMade = await _context.Highscores.SumAsync(x => x.MoneyBallMade, cancellationToken),
                NumberOfTotalMoneyShotsAtt = await _context.Highscores.SumAsync(x => x.MoneyBallAtt, cancellationToken),
                NumberOfTotalTotalShotsMade = await _context.Highscores.SumAsync(x => x.MaxShotMade, cancellationToken),
                NumberOfTotalTotalShotsAtt = await _context.Highscores.SumAsync(x => x.MaxShotAtt, cancellationToken),
                NumberOfTotalTotalPointsScored = await _context.Highscores.SumAsync(x => x.TotalPoints, cancellationToken),
                NumberOfTotalEnemiesKilled = await _context.Highscores.SumAsync(x => x.EnemiesKilled, cancellationToken),
            };

            var mostPlayedCharacter = await _context.Highscores
                .GroupBy(x => x.Character)
                .Select(x => new { Character = x.Key, Count = x.Count() })
                .OrderByDescending(x => x.Count)
                .FirstOrDefaultAsync(cancellationToken);
            serverStats.MostPlayedCharacter = mostPlayedCharacter?.Character;

            var mostPlayedLevel = await _context.Highscores
                .GroupBy(x => x.Level)
                .Select(x => new { Level = x.Key, Count = x.Count() })
                .OrderByDescending(x => x.Count)
                .FirstOrDefaultAsync(cancellationToken);
            serverStats.MostPlayedLevel = mostPlayedLevel?.Level;

            var mostConsecutive = await _context.Highscores
                .OrderByDescending(x => x.ConsecutiveShots)
                .Select(x => new { x.Username, x.ConsecutiveShots })
                .FirstOrDefaultAsync(cancellationToken);
            serverStats.MostConsecutiveShots = mostConsecutive?.ConsecutiveShots;
            serverStats.MostConsecutiveShotsUsername = mostConsecutive?.Username;

            var longestShot = await _context.Highscores
                .OrderByDescending(x => x.LongestShot)
                .Select(x => new { x.Username, x.LongestShot })
                .FirstOrDefaultAsync(cancellationToken);
            serverStats.LongestShot = longestShot?.LongestShot;
            serverStats.LongestShotUsername = longestShot?.Username;

            _context.ServerStats.Add(serverStats);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
