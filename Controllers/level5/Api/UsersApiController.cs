
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Level5Backend.Models;
using Level5Backend.Models.Dto;
using level5Server.Controllers.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace level5Server.Models.level5
{
    [EnableCors("ApiCors")]
    [Route("api/users")]
    [ApiController]
    public class UsersApiController : Controller
    {
        private readonly Level5Context _context;

        public UsersApiController(Level5Context context)
        {
            _context = context;
        }

        //--------------------- HTTP GET ---------------------------------------------------
        // GET: /api/highscores
        // get all users
        /// <summary>
        /// Get all users in database, paginated (defaults to the first 50, capped at 200 per page).
        /// </summary>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers(int page = 0, int results = 50)
        {
            int take = Math.Clamp(results, 1, 200);
            int skip = Math.Max(page, 0) * take;

            var users = await _context.Users.AsNoTracking()
                .OrderBy(u => u.Userid)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
            foreach (User u in users)
            {
                HideUserDetails(u);
            }
            return users;
        }


        //--------------------- HTTP GET Userid ---------------------------------------------------
        // GET: /api/highscores/userid/{userid}
        /// <summary>
        /// Get user by user id
        /// </summary>
        [Authorize]
        [HttpGet("userid/{userid}")]
        // GET: Users by userid
        // get user by user id
        public async Task<ActionResult<User>> GetUserById(int userid)
        {
            var user = await _context.Users.AsNoTracking()
                .FirstOrDefaultAsync(m => m.Userid == userid);
            if (user == null)
            {
                return NotFound();
            }

            HideUserDetails(user);

            return user;
        }

        //--------------------- HTTP GET Username ---------------------------------------------------
        // GET: /api/users/username/{userid}
        /// <summary>
        /// Get user by username. Used pre-login (looking a user up by name happens before the
        /// caller has a token), so this intentionally stays anonymous - but the password (and
        /// other sensitive fields) must never be part of the response.
        /// </summary>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("username/{username}")]
        public async Task<ActionResult<User>> GetUserByUsername(string username)
        {
            var user = await _context.Users.AsNoTracking()
                .FirstOrDefaultAsync(m => m.Username == username);
            if (user == null)
            {
                return NotFound();
            }

            HideUserDetails(user);

            return user;
        }

        //--------------------- HTTP GET Username ---------------------------------------------------
        // GET: /api/users/username/{userid}
        /// <summary>
        /// Get username by user id
        /// </summary>
        [Authorize]
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("email/{email}")]
        public async Task<ActionResult<User>> GetUserByEmail(string email)
        {
            var user = await _context.Users.AsNoTracking()
                .FirstOrDefaultAsync(m => m.Email == email);
            if (user == null)
            {
                return NotFound();
            }

            HideUserDetails(user);

            return user;
        }

        //--------------------- HTTP PUT ---------------------------------------------------
        // PUT: api/users/5
        /// <summary>
        /// Update user data
        /// </summary>
        // Binds a UserUpdateDto rather than the User entity - Isdev (which grants the RequireDev
        // authorization policy) and Password must never be settable by a profile-edit request.
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, UserUpdateDto dto)
        {
            if (!CallerOwnsUserid(id))
            {
                return Forbid();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Username = dto.Username;
            user.Email = dto.Email;
            user.Firstname = dto.Firstname;
            user.Lastname = dto.Lastname;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        //--------------------- HTTP POST ---------------------------------------------------
        // POST: api/Highscores
        /// <summary>
        /// Create new user. Registration is inherently anonymous - there's no token to require yet.
        /// </summary>
        // Binds a UserRegisterDto rather than the User entity - Isdev must never be settable at
        // signup either, or any account could self-grant the RequireDev authorization policy.
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(UserRegisterDto dto)
        {
            if (UserNameExists(dto.Username))
            {
                return BadRequest();
            }

            var user = new User
            {
                Username = dto.Username,
                Password = PasswordHashing.Hash(dto.Password),
                Email = dto.Email,
                Firstname = dto.Firstname,
                Lastname = dto.Lastname,
                Ipaddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                Signupdate = DateTime.UtcNow.ToString("o"),
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            HideUserDetails(user);
            return CreatedAtAction(nameof(GetUserById), new { userid = user.Userid }, user);
        }

        //--------------------- HTTP DELETE ---------------------------------------------------
        // DELETE: api/users/5
        /// <summary>
        /// Deletes user by user id.
        /// </summary>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<User>> DeleteUser(int id)
        {
            if (!CallerOwnsUserid(id))
            {
                return Forbid();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return user;
        }

        //--------------------- UTILITY FUNCTIONS ---------------------------------------------------
        private bool UserNameExists(string username)
        {
            return _context.Users.Any(e => e.Username == username);
        }

        // the JWT issued by TokenController carries the authenticated user's id as a "Userid"
        // claim - mutating/deleting endpoints must confirm the caller matches the target account.
        private bool CallerOwnsUserid(int id)
        {
            var claim = User.FindFirst("Userid")?.Value;
            return int.TryParse(claim, out int callerUserid) && callerUserid == id;
        }

        private static void HideUserDetails(User users)
        {
            users.Firstname = "*************";
            users.Lastname = "*************";
            users.Email = "*************";
            users.Password = "*************";
            users.Ipaddress = "*************";
            users.Lastlogin = "*************";
            users.Signupdate = "*************";
        }
    }
}
