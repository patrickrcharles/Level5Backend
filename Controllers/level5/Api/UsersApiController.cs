
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Level5Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace level5Server.Models.level5
{
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
            foreach(User u in _context.Users)
            {
                HideUserDetails(u);
            }
            return await _context.Users.ToListAsync();
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
            if (!UserIdExists(userid))
            {
                return NotFound();
            }

            try
            {
                var users = await _context.Users
                    .FirstOrDefaultAsync(m => m.Userid == userid);
                if (users == null)
                {
                    return null;
                }

                HideUserDetails(users);

                return users;
            }
            catch (DbUpdateConcurrencyException e)
            {
                System.Diagnostics.Debug.WriteLine("----- SERVER : DbUpdateConcurrencyException : " + e);
                return BadRequest();
            }
        }

        //--------------------- HTTP GET Username ---------------------------------------------------
        // GET: /api/users/username/{userid}
        /// <summary>
        /// Get user by username
        /// </summary>
        //[Authorize]
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("username/{username}")]
        // GET: Users by userid
        // get user by user id
        public async Task<ActionResult<User>> GetUserByUsername(string username)
        {
            if (username == null)
            {
                return null;
            }
            if (!UserNameExists(username))
            {
                return NotFound();
            }
            else
            {
                try
                {
                    var user = await _context.Users
                        .FirstOrDefaultAsync(m => m.Username == username);
                    //HideUserDetails(user);

                    return user;
                }
                catch (DbUpdateConcurrencyException e)
                {
                    System.Diagnostics.Debug.WriteLine("----- SERVER : DbUpdateConcurrencyException : " + e);
                    return BadRequest();
                }
            }
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
            if (email == null)
            {
                return null;
            }
            if (!UserNameExists(email))
            {
                return NotFound();
            }
            else
            {
                try
                {
                    var user = await _context.Users
                        .FirstOrDefaultAsync(m => m.Email == email);

                    HideUserDetails(user);

                    return user;
                }
                catch (DbUpdateConcurrencyException e)
                {
                    System.Diagnostics.Debug.WriteLine("----- SERVER : DbUpdateConcurrencyException : " + e);
                    return BadRequest();
                }
            }
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

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
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
        /// Create new user
        /// </summary>
        //[Authorize]
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {
            if (UserNameExists(user.Username))
            {
                return BadRequest();
            }

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetAllUsers), new { id = user.Userid }, user);
            }
            catch (DbUpdateConcurrencyException e)
            {
                System.Diagnostics.Debug.WriteLine("----- SERVER : DbUpdateConcurrencyException : " + e);
                return BadRequest();
            }
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
