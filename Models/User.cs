using System;
using System.Collections.Generic;

namespace Level5Backend.Models;

// Not bound directly from a request body anywhere (registration/profile-edit go through
// UserRegisterDto/UserUpdateDto, login through UserLoginDto) - so no validation attributes here.
// Login used to bind this entity directly; Email's non-nullability meant it was implicitly
// required by ASP.NET Core's model validation (this project has <Nullable>enable</Nullable>) even
// though login never uses it, 400ing any login payload that didn't happen to include an email.
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
