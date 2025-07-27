using HospitalManagementSystem.Models;
using HospitalManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace HospitalManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataService _dataService;
        private readonly UserService _userService;

        public HomeController(DataService dataService, UserService userService)
        {
            _dataService = dataService;
            _userService = userService;
        }

        public IActionResult Index()
        {
            var userSession = GetCurrentUserSession();
            if (userSession == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            ViewBag.UserSession = userSession;
            ViewBag.UserService = _userService;
            return View();
        }

        [HttpGet]
        public IActionResult ProvidersByHospital()
        {
            var userSession = GetCurrentUserSession();
            if (userSession == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            ViewBag.Hospitals = _dataService.Hospitals.Select(h => h.Name).ToList();
            ViewBag.UserSession = userSession;
            return View();
        }

        [HttpPost]
        public IActionResult ProvidersByHospital(string hospitalName)
        {
            var userSession = GetCurrentUserSession();
            if (userSession == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            ViewBag.Hospitals = _dataService.Hospitals.Select(h => h.Name).ToList();
            ViewBag.SelectedHospital = hospitalName;
            ViewBag.Providers = _dataService.GetProvidersByHospital(hospitalName);
            ViewBag.UserSession = userSession;
            return View();
        }

        [HttpGet]
        public IActionResult PatientsByProvider()
        {
            var userSession = GetCurrentUserSession();
            if (userSession == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            ViewBag.Providers = _dataService.Providers.Select(p => p.Name).ToList();
            ViewBag.UserSession = userSession;
            return View();
        }

        [HttpPost]
        public IActionResult PatientsByProvider(string providerName)
        {
            var userSession = GetCurrentUserSession();
            if (userSession == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            ViewBag.Providers = _dataService.Providers.Select(p => p.Name).ToList();
            ViewBag.SelectedProvider = providerName;
            ViewBag.Patients = _dataService.GetPatientNamesByProvider(providerName);
            ViewBag.UserSession = userSession;
            return View();
        }

        [HttpGet]
        public IActionResult PatientRefsByDoctor()
        {
            var userSession = GetCurrentUserSession();
            if (userSession == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            ViewBag.Doctors = _dataService.Providers.Where(p => p.IsDoctor).Select(p => p.Name).ToList();
            ViewBag.UserSession = userSession;
            return View();
        }

        [HttpPost]
        public IActionResult PatientRefsByDoctor(string doctorName)
        {
            var userSession = GetCurrentUserSession();
            if (userSession == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            ViewBag.Doctors = _dataService.Providers.Where(p => p.IsDoctor).Select(p => p.Name).ToList();
            ViewBag.SelectedDoctor = doctorName;
            ViewBag.PatientRefs = _dataService.GetPatientMedicalRefsByDoctor(doctorName);
            ViewBag.UserSession = userSession;
            return View();
        }

        [HttpGet]
        public IActionResult PatientsByDoctorAtHospital()
        {
            var userSession = GetCurrentUserSession();
            if (userSession == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            ViewBag.Doctors = _dataService.Providers.Where(p => p.IsDoctor).Select(p => p.Name).ToList();
            ViewBag.Hospitals = _dataService.Hospitals.Select(h => h.Name).ToList();
            ViewBag.UserSession = userSession;
            return View();
        }

        [HttpPost]
        public IActionResult PatientsByDoctorAtHospital(string doctorName, string hospitalName)
        {
            var userSession = GetCurrentUserSession();
            if (userSession == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            ViewBag.Doctors = _dataService.Providers.Where(p => p.IsDoctor).Select(p => p.Name).ToList();
            ViewBag.Hospitals = _dataService.Hospitals.Select(h => h.Name).ToList();
            ViewBag.SelectedDoctor = doctorName;
            ViewBag.SelectedHospital = hospitalName;
            ViewBag.Patients = _dataService.GetPatientsByDoctorAtHospital(doctorName, hospitalName);
            ViewBag.UserSession = userSession;
            return View();
        }

        [HttpGet]
        public IActionResult UntreatedPatients()
        {
            var userSession = GetCurrentUserSession();
            if (userSession == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            ViewBag.Hospitals = _dataService.Hospitals.Select(h => h.Name).ToList();
            ViewBag.UserSession = userSession;
            return View();
        }

        [HttpPost]
        public IActionResult UntreatedPatients(string hospitalName)
        {
            var userSession = GetCurrentUserSession();
            if (userSession == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            ViewBag.Hospitals = _dataService.Hospitals.Select(h => h.Name).ToList();
            ViewBag.SelectedHospital = hospitalName;
            ViewBag.UntreatedPatients = _dataService.GetUntreatedPatientsByHospital(hospitalName);
            ViewBag.UserSession = userSession;
            return View();
        }

        [HttpGet]
        public IActionResult CreateTreatment()
        {
            var userSession = GetCurrentUserSession();
            if (userSession == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            ViewBag.Hospitals = _dataService.Hospitals.Select(h => h.Name).ToList();
            ViewBag.Patients = _dataService.Patients.Select(p => new { p.MedicalReferenceNumber, p.PatientName }).ToList();
            ViewBag.Providers = _dataService.Providers.Select(p => p.Name).ToList();
            ViewBag.UserSession = userSession;
            return View();
        }

        [HttpPost]
        public IActionResult CreateTreatment(Treatment treatment)
        {
            if (ModelState.IsValid)
            {
                _dataService.AddTreatment(treatment);
                TempData["Message"] = "Treatment created successfully!";
                return RedirectToAction(nameof(Treatments));
            }

            ViewBag.Hospitals = _dataService.Hospitals.Select(h => h.Name).ToList();
            ViewBag.Patients = _dataService.Patients.Select(p => new { p.MedicalReferenceNumber, p.PatientName }).ToList();
            ViewBag.Providers = _dataService.Providers.Select(p => p.Name).ToList();
            return View(treatment);
        }

        [HttpGet]
        public IActionResult EditTreatment(int id)
        {
            if (id < 0 || id >= _dataService.Treatments.Count)
                return NotFound();

            var treatment = _dataService.Treatments[id];
            ViewBag.Hospitals = _dataService.Hospitals.Select(h => h.Name).ToList();
            ViewBag.Patients = _dataService.Patients.Select(p => new { p.MedicalReferenceNumber, p.PatientName }).ToList();
            ViewBag.Providers = _dataService.Providers.Select(p => p.Name).ToList();
            ViewBag.TreatmentId = id;
            return View(treatment);
        }

        [HttpPost]
        public IActionResult EditTreatment(int id, Treatment treatment)
        {
            if (id < 0 || id >= _dataService.Treatments.Count)
                return NotFound();

            if (ModelState.IsValid)
            {
                _dataService.UpdateTreatment(id, treatment);
                TempData["Message"] = "Treatment updated successfully!";
                return RedirectToAction(nameof(Treatments));
            }

            ViewBag.Hospitals = _dataService.Hospitals.Select(h => h.Name).ToList();
            ViewBag.Patients = _dataService.Patients.Select(p => new { p.MedicalReferenceNumber, p.PatientName }).ToList();
            ViewBag.Providers = _dataService.Providers.Select(p => p.Name).ToList();
            ViewBag.TreatmentId = id;
            return View(treatment);
        }

        public IActionResult Treatments()
        {
            return View(_dataService.Treatments);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        private UserSession? GetCurrentUserSession()
        {
            var roleStr = HttpContext.Session.GetString("UserRole");
            if (string.IsNullOrEmpty(roleStr) || !Enum.TryParse<UserRole>(roleStr, out var role))
                return null;

            return new UserSession
            {
                Role = role,
                ProviderName = HttpContext.Session.GetString("ProviderName"),
                HospitalLocation = HttpContext.Session.GetString("HospitalLocation")
            };
        }
    }
} 