using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Level5Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace level5Server.Models.level5
{
    //[Route("users")]
    [Controller]
    public class UsersController : Controller
    {
        private readonly Level5Context _context;

        public UsersController(Level5Context context)
        {
            _context = context;
        }

        // GET: Users
        [Route("users")]
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        [Route("users/{id}")]
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = await _context.Users
                .FirstOrDefaultAsync(m => m.Userid == id);
            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }
        //[Route("[controller]/create")]
        //// GET: Users/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Create([Bind("Userid,Username,Firstname,Lastname,Password,Email,Ipaddress,Signupdate,Lastlogin")] User users)
        {
            if (ModelState.IsValid)
            {
                _context.Add(users);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(users);
        }

        // GET: Users/Edit/5
        [Route("[controller]/edit/{id}")]
        [HttpGet]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = await _context.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }
            return View(users);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("[controller]/edit/{id}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Edit(int id, [Bind("Userid,Username,Firstname,Lastname,Password,Email,Ipaddress,Signupdate,Lastlogin")] User users)
        {
            if (id != users.Userid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine("----- edit");
                    _context.Update(users);
                    System.Diagnostics.Debug.WriteLine("----- _context.Update(users);");
                    await _context.SaveChangesAsync();
                    System.Diagnostics.Debug.WriteLine("----- _context.Update(users);");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsersExists(users.Userid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(users);
        }

        // GET: Users/Delete/5
        [Route("[controller]/delete/{id}")]
        [HttpDelete]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var users = await _context.Users
                .FirstOrDefaultAsync(m => m.Userid == id);
            if (users == null)
            {
                return NotFound();
            }

            return View(users);
        }

        // POST: Users/Delete/5
        [Route("[controller]/delete/{id}")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var users = await _context.Users.FindAsync(id);
            _context.Users.Remove(users);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        internal bool UsersExists(int id)
        {
            return _context.Users.Any(e => e.Userid == id);
        }

        internal bool isDev(string username)
        {
            // find any user that matches username + isDev = 1;
            // this means, the username is a dev account username
            var isDev = _context.Users.Any(e => e.Username == username && e.Isdev == 1);
            return isDev;
        }
    }
}
