namespace Level5Backend.Models.Dto
{
    // What a client is actually allowed to change on their own account. Deliberately excludes
    // Isdev (grants the RequireDev authorization policy - must never be client-settable) and
    // Password (changing it goes through its own hashing flow, not a plain profile edit).
    public class UserUpdateDto
    {
        public string Username { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Firstname { get; set; }

        public string? Lastname { get; set; }
    }
}
