namespace Level5Backend.Models.Dto
{
    // What a client is actually allowed to set on account creation. Deliberately excludes Isdev,
    // Ipaddress, Signupdate, Lastlogin - those are server-assigned, never client input.
    public class UserRegisterDto
    {
        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Firstname { get; set; }

        public string? Lastname { get; set; }
    }
}
