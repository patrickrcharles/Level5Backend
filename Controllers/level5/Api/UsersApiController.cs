
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Level5Backend.Models;
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
        /// Get all users in database
        /// </summary>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
        {
            var users = await _context.Users.AsNoTracking().ToListAsync();
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
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Userid)
            {
                return BadRequest();
            }

            if (!CallerOwnsUserid(id))
            {
                return Forbid();
            }

            _context.Entry(user).State = EntityState.Modified;
            // callers only ever intend to update profile fields - never let a PUT body silently
            // overwrite the password hash with whatever plaintext string happened to be in it.
            _context.Entry(user).Property(u => u.Password).IsModified = false;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserIdExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        //--------------------- HTTP POST ---------------------------------------------------
        // POST: api/Highscores
        /// <summary>
        /// Create new user. Registration is inherently anonymous - there's no token to require yet.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (UserNameExists(user.Username))
            {
                return BadRequest();
            }

            user.Password = PasswordHashing.Hash(user.Password);

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
        private bool UserIdExists(int id)
        {
            return _context.Users.Any(e => e.Userid == id);
        }

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
