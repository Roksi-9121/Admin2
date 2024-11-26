using BusinessLogic.Interface;
using DTO;
using Microsoft.AspNetCore.Mvc;

namespace Admin.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserManager _userManager;

        public AccountController(IUserManager userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login()
        {
            var isAdmin = HttpContext.Session.GetString("IsAdmin");
            if (!string.IsNullOrEmpty(isAdmin))
            {
                return isAdmin == "True"
                    ? RedirectToAction("ViewUsers")
                    : RedirectToAction("UserWelcome");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }

                var user = _userManager.AuthenticateUser(email, password);

                if (user != null)
                {
                    HttpContext.Session.SetString("UserEmail", user.Email);
                    HttpContext.Session.SetString("IsAdmin", user.Is_admin.ToString());

                    return user.Is_admin
                        ? RedirectToAction("ViewUsers")
                        : RedirectToAction("UserWelcome");
                }

                ViewBag.Error = "Invalid login attempt.";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        [HttpPost]
        public IActionResult Register(string userName, string email, string password)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View();
                }
                var newUser = new User
                {
                    User_name = userName,
                    Email = email,
                    Password_hash = password,
                    Is_active = true, 
                    Is_admin = false 
                };

                _userManager.AddUser(newUser);

                
                TempData["SuccessMessage"] = "Registration successful! Please log in.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }


        [HttpGet]
        public IActionResult UserWelcome()
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login");
            }

            ViewBag.UserEmail = userEmail;
            return View();
        }

        [HttpGet]
        public IActionResult ViewUsers()
        {
            var isAdmin = HttpContext.Session.GetString("IsAdmin");
            if (string.IsNullOrEmpty(isAdmin) || isAdmin != "True")
            {
                return RedirectToAction("Login");
            }

            var users = _userManager.ViewAllUsers();
            return View(users);
        }

        [HttpGet]
        public IActionResult EditUser(int id)
        {
            try
            {
                var user = _userManager.GetUserById(id);
                if (user == null)
                {
                    return NotFound();
                }

                return View(user);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return RedirectToAction("ViewUsers");
            }
        }

        [HttpPost]
        public IActionResult EditUser(User user)
        {
            try
            {
                _userManager.UpdateUser(user);
                return RedirectToAction("ViewUsers");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(user);
            }
        }

        [HttpPost]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                _userManager.RemoveUser(id);
                return RedirectToAction("ViewUsers");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return RedirectToAction("ViewUsers");
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

    }
}
