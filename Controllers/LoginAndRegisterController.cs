using Gym.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Gym.Controllers
{
    public class LoginAndRegisterController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnviroment;

        public LoginAndRegisterController(ModelContext context, IWebHostEnvironment webHostEnviroment)
        {
            _context = context;
            _webHostEnviroment = webHostEnviroment;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Register([Bind("Profile,Userid,Email,Password,Roleid,Cardid")] User user)

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


                _context.Add(user);

                await _context.SaveChangesAsync();

                user.Roleid = 2;
                user.Cardid = 111111;
                

                await _context.SaveChangesAsync();

                return RedirectToAction("Login");

            }

            return View(user);

        }



        [HttpPost]
        public IActionResult Login([Bind("Email,Password")] User user)
        {
            var auth = _context.Users
            .Include(x => x.Profile)
            .Include(x => x.Role) 
            .Where(x => x.Email == user.Email && x.Password == user.Password)
            .SingleOrDefault();

            if (auth != null)
            {
                HttpContext.Session.SetInt32("id", (int)auth.Userid);
                HttpContext.Session.SetInt32("RoleId", (int)auth.Roleid);
                HttpContext.Session.SetString("FName", auth.Profile.Fname);
                HttpContext.Session.SetString("LName", auth.Profile.Lname);
                HttpContext.Session.SetString("RoleName", auth.Role.Rolename);
                HttpContext.Session.SetString("Email", auth.Email);

                switch (auth.Roleid)
                {
                    case 1://Admin

                        return RedirectToAction("Index", "Admin");

                    case 3://Trainer
                        
                        return RedirectToAction("Index", "Admin");
                    case 2://Member
                        return RedirectToAction("Index", "Home");

                }

            }else
            {
                ViewBag.Message = "Invalid Information";
            }

            return View();
        }
        public IActionResult Logout ()
        {
            var roleid = HttpContext.Session.GetInt32("RoleId");
            if(roleid == 1 || roleid == 3)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }
            else if(roleid == 2)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
           
        }
    }
}
