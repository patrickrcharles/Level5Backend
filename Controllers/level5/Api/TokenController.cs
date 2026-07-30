
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using level5Server.Controllers.Utility;
using level5Server.Models;
using level5Server.Models.level5;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Level5Backend.Models;
using Microsoft.AspNetCore.Cors;

namespace level5Server.Controllers.level5.Api
{
    [EnableCors("ApiCors")]
    [Route("api/token")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    public class TokenController : Controller
    {
        public IConfiguration _configuration;
        private readonly Level5Context _context;

        public TokenController(IConfiguration config, Level5Context context)
        {
            _configuration = config;
            _context = context;
        }
        /// <summary>
        /// Get bearer token for in game game log in and high score post
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> Post(User _userData)
        {
            if (_userData != null && _userData.Username != null && _userData.Password != null)
            {
                var user = await GetUser(_userData.Username, _userData.Password);

                if (user != null)
                {
                    //create claims details based on the user information
                    var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    // "iat" is a registered NumericDate claim (RFC 7519) - it must be serialized as a
                    // JSON number, not the human-readable date string this used to produce, or newer
                    // JWT parsers reject the token outright while reading it.
                    new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                    new Claim("Userid",user.Userid.ToString()),
                    new Claim("Firstname", user.Firstname.ToString()),
                    new Claim("Lastname", user.Lastname.ToString()),
                    new Claim("username", user.Username.ToString()),
                    new Claim( "email", user.Email.ToString())
                   };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], claims, expires: DateTime.UtcNow.AddDays(1), signingCredentials: signIn);

                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    return BadRequest("Invalid credentials");
                }
            }
            else
            {
                return BadRequest();
            }
        }

        private async Task<User?> GetUser(string username, string password)
        {
            // Password is a PBKDF2 hash (see PasswordHashing/UsersApiController.PostUser) - it is
            // never compared with == against the raw submitted password.
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null || !PasswordHashing.Verify(user.Password, password))
            {
                return null;
            }

            return user;
        }
    }
}
