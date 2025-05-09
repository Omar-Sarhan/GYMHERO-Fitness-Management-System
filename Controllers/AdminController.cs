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

    public class AdminController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;

        public AdminController(ModelContext context, IWebHostEnvironment webHostEnviroment)
        {
        _context = context;
            _webHostEnviroment = webHostEnviroment;
        }
        public async Task<IActionResult> Index()
        {
            var trainerid = HttpContext.Session.GetInt32("id");

            var activeSubscriptions =
                from s in _context.Subscriptions
                join u in _context.Users on s.Memberid equals u.Userid
                join p in _context.Profiles on u.Profileid equals p.Profileid
                join pl in _context.Plans on s.Planid equals pl.Planid
                join t in _context.Users on pl.Trainerid equals t.Userid
                join tp in _context.Profiles on t.Profileid equals tp.Profileid
                where t.Roleid == 3
                orderby s.Todate descending
                select new
                {
                    SubscriptionId = s.Subscriptionid,
                    FullName = p.Fname + " " + p.Lname,
                    PlanName = pl.Planname,
                    FromDate = s.Fromdate,
                    ToDate = s.Todate,
                    Price = pl.Price,
                    TrainerName = tp.Fname + " " + tp.Lname
                };
            var submember =
                from s in _context.Subscriptions
                join u in _context.Users on s.Memberid equals u.Userid
                join p in _context.Profiles on u.Profileid equals p.Profileid
                join pl in _context.Plans on s.Planid equals pl.Planid
                join t in _context.Users on pl.Trainerid equals t.Userid
                join tp in _context.Profiles on t.Profileid equals tp.Profileid
                where  pl.Trainerid == trainerid
                orderby s.Todate descending
                select new
                {
                    SubscriptionId = s.Subscriptionid,
                    FullName = p.Fname + " " + p.Lname,
                    PlanName = pl.Planname,
                    FromDate = s.Fromdate,
                    ToDate = s.Todate,
                    Price = pl.Price,
                    TrainerName = tp.Fname + " " + tp.Lname
                };

            ViewBag.submember = submember.ToList();
            ViewBag.ActiveSubscriptions = activeSubscriptions.ToList();
            ViewBag.TotalPrice = activeSubscriptions.Sum(x => x.Price);
            ViewBag.MemberCount = _context.Users.Count(x => x.Roleid == 2);

            return View();
        }


        public async Task<IActionResult> Items()
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
        public IActionResult NewMember()
        {
            ViewData["Rolename"] = new SelectList(_context.Roles, "Roleid", "Rolename");

            return View();
        }

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> NewMember([Bind("Profile,Userid,Email,Password,Roleid,Cardid")] User user)

        {

            if (ModelState.IsValid)
            {
                if (user.Profile.ImageFile != null)
                {

                    string wwwRootPath = _webHostEnviroment.WebRootPath; //C:\Users\d.kanaan.ext\Desktop\ResturantMVC\ResturantMVC\wwwroot\

                    string fileName = Guid.NewGuid().ToString() + "_" + user.Profile.ImageFile.FileName;

                    string path = Path.Combine(wwwRootPath + "/Images/", fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))

                    {

                        await user.Profile.ImageFile.CopyToAsync(fileStream);
                    }

                    user.Profile.Imagepath = fileName;
                }
                if (user.Roleid == 2)
                {
                    user.Cardid = 111111;
                }


                _context.Add(user);

                await _context.SaveChangesAsync();


                return RedirectToAction("Items", "Admin");

            }
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

            var user = await _context.Users
          .Include(u => u.Profile) 
          .FirstOrDefaultAsync(u => u.Userid == id);
            if (user == null)
            {
                return NotFound();
            }
            
            ViewData["Roleid"] = new SelectList(_context.Roles, "Roleid", "Rolename", user.Roleid);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Profile,Userid,Email,Password,Roleid,Cardid")] User user)
        {
            if (id != user.Userid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                try
                {
                    if (user.Profile.ImageFile != null)
                    {

                        string wwwRootPath = _webHostEnviroment.WebRootPath; //C:\Users\d.kanaan.ext\Desktop\ResturantMVC\ResturantMVC\wwwroot\

                        string fileName = Guid.NewGuid().ToString() + "_" + user.Profile.ImageFile.FileName;

                        string path = Path.Combine(wwwRootPath + "/Images/", fileName);

                        using (var fileStream = new FileStream(path, FileMode.Create))

                        {

                            await user.Profile.ImageFile.CopyToAsync(fileStream);
                        }

                        user.Profile.Imagepath = fileName;
                    }
                    if (user.Roleid == 2)
                    {
                        user.Cardid = 111111;
                    }

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
                return RedirectToAction(nameof(Items));
            }
            
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
            return RedirectToAction(nameof(Items));
        }

        [HttpPost]
        public IActionResult Search(DateTime? startDate, DateTime? endDate)
        {
            var activeSubscriptions =
                from s in _context.Subscriptions
                join u in _context.Users on s.Memberid equals u.Userid
                join p in _context.Profiles on u.Profileid equals p.Profileid
                join pl in _context.Plans on s.Planid equals pl.Planid
                join t in _context.Users on pl.Trainerid equals t.Userid
                join tp in _context.Profiles on t.Profileid equals tp.Profileid
                where t.Roleid == 3
                orderby s.Todate descending
                select new
                {
                    SubscriptionId = s.Subscriptionid,
                    FullName = p.Fname + " " + p.Lname,
                    PlanName = pl.Planname,
                    FromDate = s.Fromdate,
                    ToDate = s.Todate,
                    Price = pl.Price,
                    TrainerName = tp.Fname + " " + tp.Lname
                };

           
            if (startDate != null && endDate == null)
            {
                activeSubscriptions = activeSubscriptions.Where(x => x.FromDate >= startDate);
            }
          
            else if (startDate == null && endDate != null)
            {
                activeSubscriptions = activeSubscriptions.Where(x => x.FromDate <= endDate);
            }
            
            else if (startDate != null && endDate != null)
            {
                activeSubscriptions = activeSubscriptions.Where(x => x.FromDate >= startDate && x.FromDate <= endDate);
            }

            ViewBag.ActiveSubscriptions = activeSubscriptions.ToList();
            ViewBag.TotalPrice = activeSubscriptions.Sum(x => x.Price);
            ViewBag.MemberCount = _context.Users.Count(x => x.Roleid == 2);
            return View("Index");
        }


        // الموافقة على التقييم
        
        public async Task<IActionResult> Approve(decimal id)
        {
            var testimonial = await _context.Testimonials.FindAsync(id);
            if (testimonial == null)
            {
                return NotFound();
            }

            testimonial.IsApprove = true;
            _context.Update(testimonial);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index","Testimonials");
        }

        public async Task<IActionResult> Reject(decimal id)
        {
            var testimonial = await _context.Testimonials.FindAsync(id);
            if (testimonial == null)
            {
                return NotFound();
            }

            testimonial.IsApprove = false;
            _context.Update(testimonial);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Testimonials");
        }


        private bool UserExists(decimal id)
        {
            return (_context.Users?.Any(e => e.Userid == id)).GetValueOrDefault();
        }
    }
}
