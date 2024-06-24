using System;
using System.Collections.Generic;

namespace Level5Backend.Models;

public partial class User
{
    public int Userid { get; set; }

    public string Username { get; set; } = null!;

    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Ipaddress { get; set; }

    public string? Signupdate { get; set; }

    public string? Lastlogin { get; set; }

    public int? Isdev { get; set; }
}
