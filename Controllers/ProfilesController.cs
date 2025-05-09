using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Gym.Models;
using Microsoft.AspNetCore.Hosting;

namespace Gym.Controllers
{
    public class ProfilesController : Controller
    {
        
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;

        
        public ProfilesController(ModelContext context, IWebHostEnvironment webHostEnviroment)
        {
            _context = context;
            _webHostEnviroment = webHostEnviroment;
        }

        // GET: Profiles
        public async Task<IActionResult> Index()
        {
              return _context.Profiles != null ? 
                          View(await _context.Profiles.ToListAsync()) :
                          Problem("Entity set 'ModelContext.Profiles'  is null.");
        }

        // GET: Profiles/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Profiles == null)
            {
                return NotFound();
            }

            var profile = await _context.Profiles
                .FirstOrDefaultAsync(m => m.Profileid == id);
            if (profile == null)
            {
                return NotFound();
            }

            return View(profile);
        }

        // GET: Profiles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Profiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Profileid,Fname,Lname,Gender,ImageFile")] Profile profile)
        {
            if (ModelState.IsValid)
            {
                if (profile.ImageFile != null)
                {
                    string wwwRootPath = _webHostEnviroment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + "_" + profile.ImageFile.FileName;
                    string path = Path.Combine(wwwRootPath + "/Images/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await profile.ImageFile.CopyToAsync(fileStream);
                    }
                    profile.Imagepath = fileName;
                }
                _context.Add(profile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(profile);
        }

        // GET: Profiles/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Profiles == null)
            {
                return NotFound();
            }

            var profile = await _context.Profiles.FindAsync(id);
            if (profile == null)
            {
                return NotFound();
            }
            return View(profile);
        }

        // POST: Profiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Profileid,Fname,Lname,Gender,ImageFile")] Profile profile)
        {
            var existingProfile = await _context.Profiles.AsNoTracking().FirstOrDefaultAsync(p => p.Profileid == id);
            if (id != profile.Profileid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (profile.ImageFile != null)
                    {
                        string wwwRootPath = _webHostEnviroment.WebRootPath;
                        string fileName = Guid.NewGuid().ToString() + "_" + profile.ImageFile.FileName;
                        string path = Path.Combine(wwwRootPath + "/Images/", fileName);
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await profile.ImageFile.CopyToAsync(fileStream);
                        }
                        profile.Imagepath = fileName;
                    }
                    else
                    {
                   
                        profile.Imagepath = existingProfile.Imagepath;

                    }

                    _context.Update(profile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfileExists(profile.Profileid))
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
            return View(profile);
        }

        // GET: Profiles/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Profiles == null)
            {
                return NotFound();
            }

            var profile = await _context.Profiles
                .FirstOrDefaultAsync(m => m.Profileid == id);
            if (profile == null)
            {
                return NotFound();
            }

            return View(profile);
        }

        // POST: Profiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Profiles == null)
            {
                return Problem("Entity set 'ModelContext.Profiles'  is null.");
            }
            var profile = await _context.Profiles.FindAsync(id);
            if (profile != null)
            {
                _context.Profiles.Remove(profile);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProfileExists(decimal id)
        {
          return (_context.Profiles?.Any(e => e.Profileid == id)).GetValueOrDefault();
        }
    }
}
