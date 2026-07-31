using System.ComponentModel.DataAnnotations;

namespace Level5Backend.Models.Dto
{
    // What a client is actually allowed to change on their own account. Deliberately excludes
    // Isdev (grants the RequireDev authorization policy - must never be client-settable) and
    // Password (changing it goes through its own hashing flow, not a plain profile edit).
    public class UserUpdateDto
    {
        [Required, StringLength(45, MinimumLength = 3)]
        public string Username { get; set; } = null!;

        [Required, EmailAddress, StringLength(45)]
        public string Email { get; set; } = null!;

        [StringLength(45)]
        public string? Firstname { get; set; }

        [StringLength(45)]
        public string? Lastname { get; set; }
    }
}
