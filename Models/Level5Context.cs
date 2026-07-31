using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Level5Backend.Models;

public partial class Level5Context : DbContext
{
    public Level5Context()
    {
    }

    public Level5Context(DbContextOptions<Level5Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Application> Application { get; set; }

    public virtual DbSet<Highscore> Highscores { get; set; }

    public virtual DbSet<ServerMessage> ServerMessages { get; set; }

    public virtual DbSet<ServerStat> ServerStats { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserReport> UserReports { get; set; }

    // The app always constructs this context via DI (see Program.cs's AddDbContext call), which already
    // supplies configured options - so this fallback only matters for design-time tooling (e.g. running
    // "dotnet ef migrations add" directly). It reads from an environment variable rather than a literal
    // so a real connection string is never committed to source.
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
        {
            return;
        }

        string connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
            ?? throw new InvalidOperationException("Set the ConnectionStrings__DefaultConnection environment variable to run EF Core design-time tooling against this context.");

        optionsBuilder.UseNpgsql(connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Application");

            entity.HasIndex(e => e.CurrentVersion).IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CurrentVersion)
                .HasMaxLength(45)
                .HasColumnName("currentVersion");
        });

        modelBuilder.Entity<Highscore>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("highscores");

            entity.HasIndex(e => e.Scoreid).IsUnique();

            // Every leaderboard read in HighscoresApiController filters by Modeid (often plus
            // Userid) - without this, those queries were full table scans on the busiest table
            // in the schema. Composite index also covers Modeid-only filters (leftmost prefix).
            entity.HasIndex(e => new { e.Modeid, e.Userid });

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BonusPoints).HasColumnName("bonusPoints");
            entity.Property(e => e.Character)
                .HasMaxLength(45)
                .HasColumnName("character");
            entity.Property(e => e.Characterid).HasColumnName("characterid");
            entity.Property(e => e.ConsecutiveShots).HasColumnName("consecutiveShots");
            entity.Property(e => e.Date)
                .HasMaxLength(45)
                .HasColumnName("date");
            entity.Property(e => e.Device)
                .HasMaxLength(45)
                .HasComment("what specific device being used")
                .HasColumnName("device");
            entity.Property(e => e.Difficulty)
                .HasDefaultValueSql("'1'")
                .HasColumnName("difficulty");
            entity.Property(e => e.EnemiesEnabled).HasColumnName("enemiesEnabled");
            entity.Property(e => e.EnemiesKilled).HasColumnName("enemiesKilled");
            entity.Property(e => e.FirstPlace)
                .HasMaxLength(50)
                .HasColumnName("firstPlace");
            entity.Property(e => e.FourAtt).HasColumnName("fourAtt");
            entity.Property(e => e.FourMade).HasColumnName("fourMade");
            entity.Property(e => e.FourthPlace)
                .HasMaxLength(50)
                .HasColumnName("fourthPlace");
            entity.Property(e => e.HardcoreEnabled).HasColumnName("hardcoreEnabled");
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(45)
                .HasColumnName("ipaddress");
            entity.Property(e => e.Level)
                .HasMaxLength(45)
                .HasColumnName("level");
            entity.Property(e => e.Levelid).HasColumnName("levelid");
            entity.Property(e => e.LongestShot).HasColumnName("longestShot");
            entity.Property(e => e.MaxShotAtt).HasColumnName("maxShotAtt");
            entity.Property(e => e.MaxShotMade).HasColumnName("maxShotMade");
            entity.Property(e => e.ModeName)
                .HasMaxLength(45)
                .HasColumnName("modeName");
            entity.Property(e => e.Modeid).HasColumnName("modeid");
            entity.Property(e => e.MoneyBallAtt).HasColumnName("moneyBallAtt");
            entity.Property(e => e.MoneyBallMade).HasColumnName("moneyBallMade");
            entity.Property(e => e.NumPlayers).HasColumnName("numPlayers");
            entity.Property(e => e.Os)
                .HasMaxLength(45)
                .HasColumnName("os");
            entity.Property(e => e.P1IsCpu).HasColumnName("p1IsCpu");
            entity.Property(e => e.P1TotalPoints).HasColumnName("p1TotalPoints");
            entity.Property(e => e.P2IsCpu).HasColumnName("p2IsCpu");
            entity.Property(e => e.P2TotalPoints).HasColumnName("p2TotalPoints");
            entity.Property(e => e.P3IsCpu).HasColumnName("p3IsCpu");
            entity.Property(e => e.P3TotalPoints).HasColumnName("p3TotalPoints");
            entity.Property(e => e.P4IsCpu).HasColumnName("p4IsCpu");
            entity.Property(e => e.P4TotalPoints).HasColumnName("p4TotalPoints");
            entity.Property(e => e.Platform)
                .HasMaxLength(45)
                .HasComment("if desktop/mobile")
                .HasColumnName("platform");
            entity.Property(e => e.Scoreid)
                .HasMaxLength(100)
                .HasComment("unique")
                .HasColumnName("scoreid");
            entity.Property(e => e.SecondPlace)
                .HasMaxLength(50)
                .HasColumnName("secondPlace");
            entity.Property(e => e.SevenAtt).HasColumnName("sevenAtt");
            entity.Property(e => e.SevenMade).HasColumnName("sevenMade");
            entity.Property(e => e.SniperEnabled).HasColumnName("sniperEnabled");
            entity.Property(e => e.SniperHits).HasColumnName("sniperHits");
            entity.Property(e => e.SniperMode).HasColumnName("sniperMode");
            entity.Property(e => e.SniperModeName)
                .HasMaxLength(45)
                .HasDefaultValueSql("'none'")
                .HasColumnName("sniperModeName");
            entity.Property(e => e.SniperShots).HasColumnName("sniperShots");
            entity.Property(e => e.ThirdPlace)
                .HasMaxLength(50)
                .HasColumnName("thirdPlace");
            entity.Property(e => e.ThreeAtt).HasColumnName("threeAtt");
            entity.Property(e => e.ThreeMade).HasColumnName("threeMade");
            entity.Property(e => e.Time).HasColumnName("time");
            entity.Property(e => e.TotalDistance).HasColumnName("totalDistance");
            entity.Property(e => e.TotalPoints).HasColumnName("totalPoints");
            entity.Property(e => e.TrafficEnabled).HasColumnName("trafficEnabled");
            entity.Property(e => e.TwoAtt).HasColumnName("twoAtt");
            entity.Property(e => e.TwoMade)
                .HasDefaultValueSql("'0'")
                .HasColumnName("twoMade");
            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Username)
                .HasMaxLength(45)
                .HasColumnName("username");
            entity.Property(e => e.Version)
                .HasMaxLength(45)
                .HasColumnName("version");
        });

        modelBuilder.Entity<ServerMessage>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date)
                .HasMaxLength(45)
                .HasColumnName("date");
            entity.Property(e => e.Message)
                .HasMaxLength(255)
                .HasColumnName("message");
        });

        modelBuilder.Entity<ServerStat>(entity =>
        {
            // Id is an auto-increment identity column (see ValueGeneratedOnAdd below) - it was
            // scaffolded as a keyless entity only because the original MySQL table had a unique
            // index instead of an explicit PRIMARY KEY constraint. Without a real key, EF Core's
            // change tracker can't track/insert instances (ServerStatsService.RecomputeAsync
            // relies on being able to Add() new rows), so this needs to be a proper key.
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.LongestShotUsername).HasMaxLength(45);
            entity.Property(e => e.MostConsecutiveShotsUsername).HasMaxLength(45);
            entity.Property(e => e.MostPlayedCharacter).HasMaxLength(45);
            entity.Property(e => e.MostPlayedLevel).HasMaxLength(45);
            entity.Property(e => e.NumberOfUsers).HasColumnName("numberOfUsers");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Userid);

            entity.ToTable("users");

            entity.HasIndex(e => e.Userid).IsUnique();

            entity.HasIndex(e => e.Username).IsUnique();

            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Email)
                .HasMaxLength(45)
                .HasColumnName("email");
            entity.Property(e => e.Firstname)
                .HasMaxLength(45)
                .HasColumnName("firstname");
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(45)
                .HasColumnName("ipaddress");
            entity.Property(e => e.Isdev).HasColumnName("isdev");
            entity.Property(e => e.Lastlogin)
                .HasMaxLength(45)
                .HasColumnName("lastlogin");
            entity.Property(e => e.Lastname)
                .HasMaxLength(45)
                .HasColumnName("lastname");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Signupdate)
                .HasMaxLength(45)
                .HasColumnName("signupdate");
            entity.Property(e => e.Username)
                .HasMaxLength(45)
                .HasColumnName("username");
        });

        modelBuilder.Entity<UserReport>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("UserReport");

            entity.HasIndex(e => e.Id).IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date)
                .HasColumnName("date");
            entity.Property(e => e.Device)
                .HasMaxLength(45)
                .HasColumnName("device");
            entity.Property(e => e.DeviceName)
                .HasMaxLength(45)
                .HasColumnName("deviceName");
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(45)
                .HasColumnName("ipaddress");
            entity.Property(e => e.Os)
                .HasMaxLength(45)
                .HasColumnName("os");
            entity.Property(e => e.Report)
                .HasMaxLength(255)
                .HasColumnName("report");
            entity.Property(e => e.UserName)
                .HasMaxLength(45)
                .HasColumnName("userName");
            entity.Property(e => e.Userid).HasColumnName("userid");
            entity.Property(e => e.Version)
                .HasMaxLength(45)
                .HasColumnName("version");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
