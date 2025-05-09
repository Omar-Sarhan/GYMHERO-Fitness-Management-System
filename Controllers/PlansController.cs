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
    public class PlansController : Controller
    {
        private readonly ModelContext _context;

        public PlansController(ModelContext context)
        {
            _context = context;
        }

        // GET: Plans
        public async Task<IActionResult> Index()
        {
            var trainerid = HttpContext.Session.GetInt32("id");
            var plan = _context.Plans
           .Where(p => p.Trainerid == trainerid)
           .ToList();

            return View(plan);
            
        }

        // GET: Plans/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Plans == null)
            {
                return NotFound();
            }

            var plan = await _context.Plans
                .Include(p => p.Trainer)
                .FirstOrDefaultAsync(m => m.Planid == id);
            if (plan == null)
            {
                return NotFound();
            }

            return View(plan);
        }

        // GET: Plans/Create
        public IActionResult Create()
        {
            
            return View();
        }

        // POST: Plans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Planid,Planname,Description,Price,Trainerid")] Plan plan)
        {
            var trainerid = HttpContext.Session.GetInt32("id");
            if (ModelState.IsValid)
            {
                _context.Add(plan);
                plan.Trainerid = trainerid;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            
            return View(plan);
        }

        // GET: Plans/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Plans == null)
            {
                return NotFound();
            }

            var plan = await _context.Plans.FindAsync(id);
            if (plan == null)
            {
                return NotFound();
            }
            
            return View(plan);
        }

        // POST: Plans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Planid,Planname,Description,Price,Trainerid")] Plan plan)
        {
            if (id != plan.Planid)
            {
                return NotFound();
            }
            var trainerid = HttpContext.Session.GetInt32("id");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(plan);
                    plan.Trainerid = trainerid;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlanExists(plan.Planid))
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
            
            return View(plan);
        }

        // GET: Plans/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Plans == null)
            {
                return NotFound();
            }

            var plan = await _context.Plans
                .Include(p => p.Trainer)
                .FirstOrDefaultAsync(m => m.Planid == id);
            if (plan == null)
            {
                return NotFound();
            }

            return View(plan);
        }

        // POST: Plans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Plans == null)
            {
                return Problem("Entity set 'ModelContext.Plans'  is null.");
            }
            var plan = await _context.Plans.FindAsync(id);
            if (plan != null)
            {
                _context.Plans.Remove(plan);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlanExists(decimal id)
        {
          return (_context.Plans?.Any(e => e.Planid == id)).GetValueOrDefault();
        }
    }
}
