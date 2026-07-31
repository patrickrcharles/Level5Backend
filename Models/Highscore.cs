using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Level5Backend.Models;

// StringLength caps below mirror the column widths declared in Level5Context - without them, an
// over-length value wasn't rejected until Postgres threw on insert, surfacing as a 500 instead of
// a 400. Ipaddress is deliberately left unannotated: HighscoresApiController always overwrites it
// with a server-derived value, so validating whatever a client happened to send there would just
// reject otherwise-valid requests for a field that's discarded anyway.
public partial class Highscore
{
    public int Id { get; set; }

    public int Userid { get; set; }

    [StringLength(45)]
    public string? Username { get; set; }

    /// <summary>
    /// unique
    /// </summary>
    [StringLength(100)]
    public string? Scoreid { get; set; }

    public int Modeid { get; set; }

    [StringLength(45)]
    public string? ModeName { get; set; }

    public int Characterid { get; set; }

    public int Levelid { get; set; }

    [Required, StringLength(45)]
    public string Character { get; set; } = null!;

    [Required, StringLength(45)]
    public string Level { get; set; } = null!;

    [Required, StringLength(45)]
    public string Os { get; set; } = null!;

    [Required, StringLength(45)]
    public string Version { get; set; } = null!;

    [Required, StringLength(45)]
    public string Date { get; set; } = null!;

    // Nullable so omitting it can be told apart from explicitly sending 0 - the DB column defaults
    // this to 1, but EF always writes an explicit value on insert (it doesn't fall back to SQL
    // defaults for values it's given), so a plain non-nullable int would have silently stored 0
    // for any client that left this out, instead of the intended default. updateModeName backfills
    // it the same way it already does for ModeName/SniperModeName.
    public int? Difficulty { get; set; }

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

    // Nullable, not [Required] - the DB column defaults this to 'none' (see Level5Context), which
    // only ever takes effect if EF is told to omit the value entirely; a non-nullable/required C#
    // property meant no client could submit a score without explicitly sending this. updateModeName
    // below now backfills it the same way it already does for ModeName.
    [StringLength(45)]
    public string? SniperModeName { get; set; }

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
    [StringLength(45)]
    public string? Platform { get; set; }

    /// <summary>
    /// what specific device being used
    /// </summary>
    [StringLength(45)]
    public string? Device { get; set; }

    public string? Ipaddress { get; set; }

    public int P1TotalPoints { get; set; }

    public int P2TotalPoints { get; set; }

    public int P3TotalPoints { get; set; }

    public int P4TotalPoints { get; set; }

    [StringLength(50)]
    public string? FirstPlace { get; set; }

    [StringLength(50)]
    public string? SecondPlace { get; set; }

    [StringLength(50)]
    public string? ThirdPlace { get; set; }

    [StringLength(50)]
    public string? FourthPlace { get; set; }

    public int P1IsCpu { get; set; }

    public int P2IsCpu { get; set; }

    public int P3IsCpu { get; set; }

    public int P4IsCpu { get; set; }

    public int NumPlayers { get; set; }
}
