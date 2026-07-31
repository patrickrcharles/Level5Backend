using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Level5Backend.Models;

// TokenController.Post is the only place this entity is still bound directly from a request body
// (registration/profile-edit went through UserRegisterDto/UserUpdateDto instead) - Username and
// Password are the only fields it actually reads, so those are the only ones annotated here.
// Adding [Required] to Email/Firstname/Lastname would risk 400ing a login payload that doesn't
// happen to include them, for fields the login flow never looks at.
public partial class User
{
    public int Userid { get; set; }

    [Required, StringLength(45)]
    public string Username { get; set; } = null!;

    public string? Firstname { get; set; }

    public string? Lastname { get; set; }

    [Required]
    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Ipaddress { get; set; }

    public string? Signupdate { get; set; }

    public string? Lastlogin { get; set; }

    public int? Isdev { get; set; }
}
