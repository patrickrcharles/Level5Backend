using System.ComponentModel.DataAnnotations;

namespace Level5Backend.Models.Dto
{
    // TokenController.Post used to bind the full User entity for login. Email is non-nullable on
    // that model, and with <Nullable>enable</Nullable> project-wide, ASP.NET Core's model
    // validation implicitly treats non-nullable reference-type properties as required - so a
    // login request that only sent Username/Password (which is all TokenController ever reads)
    // was rejected with a 400 over a missing Email it never needed. This DTO only asks for what
    // login actually uses.
    public class UserLoginDto
    {
        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
