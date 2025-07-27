using HospitalManagementSystem.Models;
using HospitalManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserService _userService;
        private readonly DataService _dataService;

        public AccountController(UserService userService, DataService dataService)
        {
            _userService = userService;
            _dataService = dataService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.Roles = Enum.GetValues<UserRole>();
            ViewBag.Providers = _dataService.Providers.Select(p => p.Name).ToList();
            ViewBag.Hospitals = _dataService.Hospitals.Select(h => h.Name).ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Login(UserRole role, string? providerName = null, string? hospitalLocation = null)
        {
            var userSession = new UserSession
            {
                Role = role,
                ProviderName = providerName,
                HospitalLocation = hospitalLocation
            };

            // Store in session
            HttpContext.Session.SetString("UserRole", role.ToString());
            if (!string.IsNullOrEmpty(providerName))
                HttpContext.Session.SetString("ProviderName", providerName);
            if (!string.IsNullOrEmpty(hospitalLocation))
                HttpContext.Session.SetString("HospitalLocation", hospitalLocation);

            TempData["Message"] = $"Logged in as {role}";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Message"] = "Logged out successfully";
            return RedirectToAction("Index", "Home");
        }
    }
} 