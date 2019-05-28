using System;
using System.Linq;
using System.Diagnostics;
using BeltExam.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        public HomeContext dbContext;

        public HomeController(HomeContext context)
        {
            dbContext = context;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            if(HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }

            User thisUser = dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));
            ViewBag.ThisUser = thisUser;

            List<Actvty> EveryActivity = dbContext.Activities
                .Include(w => w.ActivityAttendees)
                .ThenInclude(a => a.User)
                .OrderBy(ea => ea.ActivityDate)
                .ToList();

            foreach (Actvty a in EveryActivity.ToList())
            {
                if (a.ActivityDate < DateTime.Now)
                {
                    EveryActivity.Remove(a);
                }
            }

            ViewBag.AllActivities = EveryActivity;

            List<User> userCreators = dbContext.Users.ToList();
            ViewBag.Creators = userCreators;

            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            if(ModelState.IsValid)
            {
                if(dbContext.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use.");
                    return View("Index");
                }

                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);

                dbContext.Users.Add(user);
                dbContext.SaveChanges();
                HttpContext.Session.SetInt32("UserId", user.UserId);
                return RedirectToAction("Dashboard");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpPost("login")]
        public IActionResult Login(LoginUser logger)
        {
            if(ModelState.IsValid)
            {
                User userInDb = dbContext.Users.FirstOrDefault( u => u.Email == logger.LoginEmail);


                if(userInDb == null)
                {
                    ModelState.AddModelError("LoginEmail", "Invalid Email/Password.");
                    return View("Index");
                }
                var hasher = new PasswordHasher<LoginUser>();
                var result = hasher.VerifyHashedPassword(logger,userInDb.Password, logger.LoginPassword);
                if(result == 0)
                {
                    ModelState.AddModelError("LoginPassword", "Invalid Email/Password");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("UserId",userInDb.UserId);
                return RedirectToAction("Dashboard");
            }
            else
            {
                return View("Index");
            }
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [HttpGet("add")]
        public IActionResult Add()
        {
            if(HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.UserId = HttpContext.Session.GetInt32("UserId");
            return View();
        }

        [HttpPost("create")]
        public IActionResult Create(Actvty activity)
        {
            if(ModelState.IsValid)
            {
                activity.PlannerId = (int)HttpContext.Session.GetInt32("UserId");
                dbContext.Activities.Add(activity);
                dbContext.SaveChanges();
                Actvty thisActivity = dbContext.Activities.OrderByDescending(w => w.CreatedAt).FirstOrDefault();
                return RedirectToAction("Detail", new {id = activity.ActvtyId});
            }
            else
            {
                return View("Add", activity);
            }
        }
        [HttpGet("detail/{id}")]
        public IActionResult Detail(int id)
        {
            if(HttpContext.Session.GetInt32("UserId") == null)
            {
                return RedirectToAction("Index");
            }

            User thisUser = dbContext.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));
            ViewBag.ThisUser = thisUser;

            Actvty thisActivity = dbContext.Activities.FirstOrDefault(w => w.ActvtyId == id);
            ViewBag.ThisActivity = thisActivity;

            User eventCoord = dbContext.Users.FirstOrDefault(ec => ec.UserId == thisActivity.PlannerId);
            ViewBag.EventCoordinator = eventCoord;

            var actParticipants = dbContext.Activities
                .Include(w => w.ActivityAttendees)
                .ThenInclude(u => u.User)
                .FirstOrDefault(w => w.ActvtyId == id);
            
            ViewBag.AllParticipants = actParticipants.ActivityAttendees;

            return View();
        }

        [HttpGet("/join/{id}")]
        public IActionResult YesActivity(int id)
        {
            Actvty thisActivity = dbContext.Activities.FirstOrDefault(a => a.ActvtyId == id);
            User usersActivities = dbContext.Users
                .Include(a => a.AttendedActivities)
                .ThenInclude(e => e.Activity)
                .ToList().FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("UserId"));
            
            bool canAttend = true;
            foreach (var a in usersActivities.AttendedActivities)
            {
                if (a.Activity.ActivityDate.Date == thisActivity.ActivityDate.Date)
                {
                    canAttend = false;
                    Console.WriteLine("This user cannot attend activity: "+thisActivity.Title);
                }
            }

            if (canAttend)
            {
                Participation participation = new Participation();
                participation.UserId = (int)HttpContext.Session.GetInt32("UserId");
                participation.ActvtyId = id;
                dbContext.Participations.Add(participation);
                dbContext.SaveChanges();
            }

            return RedirectToAction("Dashboard");
        }

        [HttpGet("/leave/{partId}")]
        public IActionResult NoActivity(int partId)
        {
            Participation participation = dbContext.Participations.FirstOrDefault(a => a.ParticipationId == partId);
            dbContext.Participations.Remove(participation);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("delete/{actId}")]
        public IActionResult DeleteActivity(int actId)
        {
            Actvty actToDelete = dbContext.Activities.FirstOrDefault(w => w.ActvtyId == actId);
            dbContext.Activities.Remove(actToDelete);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }
    }
}
