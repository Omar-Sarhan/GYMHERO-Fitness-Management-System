using Gym.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Numerics;
using System.IO; // للتعامل مع الملفات
using iText.Kernel.Pdf; // مكتبة PDF الأساسية
using iText.Layout; // للتعامل مع تخطيط المستند
using iText.Layout.Element;



namespace Gym.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;

        public HomeController(ModelContext context, IWebHostEnvironment webHostEnviroment , ILogger<HomeController> logger)
        {
            _logger = logger;
            _context = context;
            _webHostEnviroment = webHostEnviroment;
        }


        public IActionResult Index()
        {
            var trainers = _context.Users
                .Join(_context.Profiles, u => u.Profileid, p => p.Profileid, (u, p) => new { u, p })
                .Where(x => x.u.Roleid == 3)
                .Select(x => new
                {
                    TrainerId = x.u.Userid,
                    TrainerName = x.p.Fname + " " + x.p.Lname,
                    ProfileImage = x.p.Imagepath
                })
                .ToList();

            ViewBag.trainers = trainers;

            var approvedTestimonials =  _context.Testimonials
            .Where(t => t.IsApprove) 
            .Include(t => t.User)
            .ThenInclude(u => u.Profile)
            .ToList();

            ViewBag.ApprovedTestimonials = approvedTestimonials;
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ShowPlan(int trainerid)
        {
            var plan = _context.Plans
        .Where(p => p.Trainerid == trainerid)
        .ToList();
              
            return View(plan);
        }
        
        public async Task<IActionResult> Profile(decimal? id)
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

            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(decimal id, [Bind("Profile,Userid,Email,Password,Roleid,Cardid")] User user)
        {
            var roleId = HttpContext.Session.GetInt32("RoleId");
            var userRecord = await _context.Users
                                .AsNoTracking()
                                .FirstOrDefaultAsync(u => u.Userid == id);
            var existingProfile = await _context.Profiles
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(p => p.Profileid == userRecord.Profileid);

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
                    else
                    {

                        user.Profile.Imagepath = existingProfile.Imagepath;

                    }
                   
                    _context.Update(user);
                    if (roleId == 1 )
                    {
                        user.Roleid = 1;
                    }
                    else if (roleId == 2)
                    {
                        user.Roleid = 2;
                        user.Cardid = 111111;
                    }
                    else if (roleId == 3)
                    {
                        user.Roleid = 3;
                    }
                   
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
                if(roleId == 1 || roleId == 3)
                {
                    return RedirectToAction("Index", "Admin");
                }else if(roleId == 2)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            
            return View(user);
        }

        public async Task<IActionResult> Cart(decimal? planid)
        {
            HttpContext.Session.SetInt32("planid", (int)planid);
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Cart(decimal cardNumber, decimal password)
        {
            var fullName = HttpContext.Session.GetString("FName") + " " + HttpContext.Session.GetString("LName");
            var planid = HttpContext.Session.GetInt32("planid");
            int? userId = HttpContext.Session.GetInt32("id");

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = _context.Users.Include(u => u.Card).FirstOrDefault(u => u.Userid == userId);
            if (user?.Card == null || user.Card.Cardid != cardNumber || user.Card.Password != password)
            {
                ViewBag.Message = "Invalid card details.";
                return View("Cart");
            }

            var existingSubscription = _context.Subscriptions
                .FirstOrDefault(s => s.Memberid == user.Userid && s.Planid == planid);

            if (existingSubscription != null)
            {
                ViewBag.Message = "You have subscribed to this plan before.";
                return View("Cart");
            }

            var plan = _context.Plans.FirstOrDefault(p => p.Planid == planid);
            if (user.Card.Balance < plan.Price)
            {
                ViewBag.Message = "Insufficient balance.";
                return View("Cart");
            }

            user.Card.Balance -= plan.Price;
            _context.SaveChanges();

            var subscription = new Subscription
            {
                Memberid = user.Userid,
                Planid = plan.Planid,
                Fromdate = DateTime.Now,
                Todate = DateTime.Now.AddMonths(3)
            };

            _context.Subscriptions.Add(subscription);
            _context.SaveChanges();

            var fileName = $"SubscriptionReceipt_{user.Userid}_{DateTime.Now.Ticks}.pdf";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/temp", fileName);

            using (var writer = new iText.Kernel.Pdf.PdfWriter(filePath))
            {
                var pdf = new iText.Kernel.Pdf.PdfDocument(writer);
                var document = new iText.Layout.Document(pdf);

                document.Add(new iText.Layout.Element.Paragraph("Thank you for your Subsicribe."));
                document.Add(new iText.Layout.Element.Paragraph($"Full Name: {fullName}"));
                document.Add(new iText.Layout.Element.Paragraph($"Plan Name: {plan.Planname}"));
                document.Add(new iText.Layout.Element.Paragraph($"Plan Name: {plan.Price}"));
                document.Add(new iText.Layout.Element.Paragraph($"Subscription Date: {DateTime.Now}"));
                document.Close();
            }
            ViewBag.MessageDone = "The operation was completed successfully,You can download the invoice from here";
            ViewBag.FileName = fileName;
            return View();

        }
        [HttpGet]
        public IActionResult DownloadReceipt(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/temp", fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("File not found.");
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            return File(fileBytes, "application/pdf", "SubscriptionReceipt.pdf");
        }




        public IActionResult AboutUs()
        {
         return View();
        }
        public IActionResult Contact()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        private bool UserExists(decimal id)
        {
            return (_context.Users?.Any(e => e.Userid == id)).GetValueOrDefault();
        }
    }
    
}