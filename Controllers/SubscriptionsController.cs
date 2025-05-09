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
    public class SubscriptionsController : Controller
    {
        private readonly ModelContext _context;

        public SubscriptionsController(ModelContext context)
        {
            _context = context;
        }

        // GET: Subscriptions
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.Subscriptions.Include(s => s.Member).Include(s => s.Plan);
            return View(await modelContext.ToListAsync());
        }

        // GET: Subscriptions/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Subscriptions == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions
                .Include(s => s.Member)
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(m => m.Subscriptionid == id);
            if (subscription == null)
            {
                return NotFound();
            }

            return View(subscription);
        }

        // GET: Subscriptions/Create
        public IActionResult Create()
        {
            ViewData["Memberid"] = new SelectList(_context.Users, "Userid", "Userid");
            ViewData["Planid"] = new SelectList(_context.Plans, "Planid", "Planid");
            return View();
        }

        // POST: Subscriptions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Subscriptionid,Fromdate,Todate,Memberid,Planid")] Subscription subscription)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subscription);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Memberid"] = new SelectList(_context.Users, "Userid", "Userid", subscription.Memberid);
            ViewData["Planid"] = new SelectList(_context.Plans, "Planid", "Planid", subscription.Planid);
            return View(subscription);
        }

        // GET: Subscriptions/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Subscriptions == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription == null)
            {
                return NotFound();
            }
            ViewData["Memberid"] = new SelectList(_context.Users, "Userid", "Userid", subscription.Memberid);
            ViewData["Planid"] = new SelectList(_context.Plans, "Planid", "Planid", subscription.Planid);
            return View(subscription);
        }

        // POST: Subscriptions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Subscriptionid,Fromdate,Todate,Memberid,Planid")] Subscription subscription)
        {
            if (id != subscription.Subscriptionid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subscription);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubscriptionExists(subscription.Subscriptionid))
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
            ViewData["Memberid"] = new SelectList(_context.Users, "Userid", "Userid", subscription.Memberid);
            ViewData["Planid"] = new SelectList(_context.Plans, "Planid", "Planid", subscription.Planid);
            return View(subscription);
        }

        // GET: Subscriptions/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Subscriptions == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions
                .Include(s => s.Member)
                .Include(s => s.Plan)
                .FirstOrDefaultAsync(m => m.Subscriptionid == id);
            if (subscription == null)
            {
                return NotFound();
            }

            return View(subscription);
        }

        // POST: Subscriptions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Subscriptions == null)
            {
                return Problem("Entity set 'ModelContext.Subscriptions'  is null.");
            }
            var subscription = await _context.Subscriptions.FindAsync(id);
            if (subscription != null)
            {
                _context.Subscriptions.Remove(subscription);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubscriptionExists(decimal id)
        {
          return (_context.Subscriptions?.Any(e => e.Subscriptionid == id)).GetValueOrDefault();
        }
    }
}
