using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Gym.Models;

namespace Gym.Controllers
{
    public class UsersController : Controller
    {
        private readonly ModelContext _context;

        public UsersController(ModelContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.Users.Include(u => u.Card).Include(u => u.Profile).Include(u => u.Role);
            return View(await modelContext.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Card)
                .Include(u => u.Profile)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Userid == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["Cardid"] = new SelectList(_context.Cards, "Cardid", "Cardid");
            ViewData["Profileid"] = new SelectList(_context.Profiles, "Profileid", "Profileid");
            ViewData["Roleid"] = new SelectList(_context.Roles, "Roleid", "Roleid");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Userid,Email,Password,Roleid,Profileid,Cardid")] User user)
        {
            if (ModelState.IsValid)
            {

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Cardid"] = new SelectList(_context.Cards, "Cardid", "Cardid", user.Cardid);
            ViewData["Profileid"] = new SelectList(_context.Profiles, "Profileid", "Profileid", user.Profileid);
            ViewData["Roleid"] = new SelectList(_context.Roles, "Roleid", "Roleid", user.Roleid);
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["Cardid"] = new SelectList(_context.Cards, "Cardid", "Cardid", user.Cardid);
            ViewData["Profileid"] = new SelectList(_context.Profiles, "Profileid", "Profileid", user.Profileid);
            ViewData["Roleid"] = new SelectList(_context.Roles, "Roleid", "Roleid", user.Roleid);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Userid,Email,Password,Roleid,Profileid,Cardid")] User user)
        {
            if (id != user.Userid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Userid))
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
            ViewData["Cardid"] = new SelectList(_context.Cards, "Cardid", "Cardid", user.Cardid);
            ViewData["Profileid"] = new SelectList(_context.Profiles, "Profileid", "Profileid", user.Profileid);
            ViewData["Roleid"] = new SelectList(_context.Roles, "Roleid", "Roleid", user.Roleid);
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Card)
                .Include(u => u.Profile)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Userid == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'ModelContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(decimal id)
        {
          return (_context.Users?.Any(e => e.Userid == id)).GetValueOrDefault();
        }
    }
}
