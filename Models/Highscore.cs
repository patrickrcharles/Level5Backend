using System;
using System.Collections.Generic;

namespace Level5Backend.Models;

public partial class Highscore
{
    public int Id { get; set; }

    public int Userid { get; set; }

    public string? Username { get; set; }

    /// <summary>
    /// unique
    /// </summary>
    public string? Scoreid { get; set; }

    public int Modeid { get; set; }

    public string? ModeName { get; set; }

    public int Characterid { get; set; }

    public int Levelid { get; set; }

    public string Character { get; set; } = null!;

    public string Level { get; set; } = null!;

    public string Os { get; set; } = null!;

    public string Version { get; set; } = null!;

    public string Date { get; set; } = null!;

    public int Difficulty { get; set; }

    public float Time { get; set; }

    public int TotalPoints { get; set; }

    public float LongestShot { get; set; }

    public float TotalDistance { get; set; }

    public int ConsecutiveShots { get; set; }

    public int TrafficEnabled { get; set; }

    public int HardcoreEnabled { get; set; }

    public int EnemiesEnabled { get; set; }

    public int EnemiesKilled { get; set; }

    public int SniperEnabled { get; set; }

    public int SniperMode { get; set; }

    public string SniperModeName { get; set; } = null!;

    public int SniperHits { get; set; }

    public int SniperShots { get; set; }

    public int MaxShotMade { get; set; }

    public int MaxShotAtt { get; set; }

    public int? TwoMade { get; set; }

    public int? TwoAtt { get; set; }

    public int? ThreeMade { get; set; }

    public int? ThreeAtt { get; set; }

    public int? FourMade { get; set; }

    public int? FourAtt { get; set; }

    public int? SevenMade { get; set; }

    public int? SevenAtt { get; set; }

    public int? BonusPoints { get; set; }

    public int? MoneyBallMade { get; set; }

    public int? MoneyBallAtt { get; set; }

    /// <summary>
    /// if desktop/mobile
    /// </summary>
    public string? Platform { get; set; }

    /// <summary>
    /// what specific device being used
    /// </summary>
    public string? Device { get; set; }

    public string? Ipaddress { get; set; }

    public int P1TotalPoints { get; set; }

    public int P2TotalPoints { get; set; }

    public int P3TotalPoints { get; set; }

    public int P4TotalPoints { get; set; }

    public string? FirstPlace { get; set; }

    public string? SecondPlace { get; set; }

    public string? ThirdPlace { get; set; }

    public string? FourthPlace { get; set; }

    public int P1IsCpu { get; set; }

    public int P2IsCpu { get; set; }

    public int P3IsCpu { get; set; }

    public int P4IsCpu { get; set; }

    public int NumPlayers { get; set; }
}
