using System.ComponentModel.DataAnnotations;

namespace Level5Backend.Models.Dto
{
    // What a client is actually allowed to set on account creation. Deliberately excludes Isdev,
    // Ipaddress, Signupdate, Lastlogin - those are server-assigned, never client input.
    //
    // StringLength caps match the column widths in Level5Context (all 45 chars except Password,
    // which isn't stored raw - it's hashed before persisting, so its cap here is just a sane
    // request-size limit) - without them, an over-length value wasn't rejected until Postgres
    // threw on insert, surfacing as a 500 instead of a 400.
    public class UserRegisterDto
    {
        [Required, StringLength(45, MinimumLength = 3)]
        public string Username { get; set; } = null!;

        [Required, StringLength(128, MinimumLength = 8)]
        public string Password { get; set; } = null!;

        [Required, EmailAddress, StringLength(45)]
        public string Email { get; set; } = null!;

        [StringLength(45)]
        public string? Firstname { get; set; }

        [StringLength(45)]
        public string? Lastname { get; set; }
    }
}
