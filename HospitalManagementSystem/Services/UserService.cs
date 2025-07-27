using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.Services
{
    public class UserService
    {
        private readonly DataService _dataService;

        public UserService(DataService dataService)
        {
            _dataService = dataService;
        }

        public bool CanViewProviderNumbers(UserRole role, string? providerName = null)
        {
            return role switch
            {
                UserRole.Clerk => false,
                UserRole.Administrator => true,
                UserRole.Doctor => !string.IsNullOrEmpty(providerName) && 
                                 _dataService.Providers.Any(p => p.Name == providerName && p.IsDoctor),
                UserRole.Nurse => !string.IsNullOrEmpty(providerName) && 
                                _dataService.Providers.Any(p => p.Name == providerName && !p.IsDoctor),
                _ => false
            };
        }

        public bool CanViewProviderNames(UserRole role)
        {
            return role switch
            {
                UserRole.Clerk => true,
                UserRole.Administrator => true,
                UserRole.Doctor => true,
                UserRole.Nurse => true,
                _ => false
            };
        }

        public bool CanViewProviderLocations(UserRole role, string? hospitalLocation = null)
        {
            return role switch
            {
                UserRole.Clerk => true,
                UserRole.Administrator => true,
                UserRole.Doctor => true,
                UserRole.Nurse => !string.IsNullOrEmpty(hospitalLocation),
                _ => false
            };
        }

        public bool CanViewTreatmentDetails(UserRole role, string? hospitalLocation = null)
        {
            return role switch
            {
                UserRole.Clerk => false,
                UserRole.Administrator => false,
                UserRole.Doctor => true,
                UserRole.Nurse => !string.IsNullOrEmpty(hospitalLocation),
                _ => false
            };
        }

        public bool CanEditTreatment(UserRole role, string? providerName = null, string treatmentProvider = "")
        {
            return role switch
            {
                UserRole.Clerk => false,
                UserRole.Administrator => false,
                UserRole.Doctor => !string.IsNullOrEmpty(providerName) && treatmentProvider == providerName,
                UserRole.Nurse => !string.IsNullOrEmpty(providerName) && treatmentProvider == providerName,
                _ => false
            };
        }

        public bool CanViewClientName(UserRole role)
        {
            return role switch
            {
                UserRole.Clerk => true,
                UserRole.Administrator => true,
                UserRole.Doctor => true,
                UserRole.Nurse => true,
                _ => false
            };
        }

        public bool CanViewClientMedicalRecordNumber(UserRole role)
        {
            return role switch
            {
                UserRole.Clerk => false,
                UserRole.Administrator => true,
                UserRole.Doctor => true,
                UserRole.Nurse => false,
                _ => false
            };
        }

        public bool CanViewPracticeInformation(UserRole role)
        {
            return true; // All roles can view practice information
        }

        public List<Provider> GetFilteredProviders(UserRole role, string? providerName = null, string? hospitalLocation = null)
        {
            var allProviders = _dataService.Providers.ToList();

            return role switch
            {
                UserRole.Clerk => allProviders.Where(p => !string.IsNullOrEmpty(p.Number)).ToList(), // No numbers
                UserRole.Administrator => allProviders,
                UserRole.Doctor => allProviders.Where(p => p.Name == providerName).ToList(),
                UserRole.Nurse => allProviders.Where(p => p.Name == providerName).ToList(),
                _ => new List<Provider>()
            };
        }

        public List<Treatment> GetFilteredTreatments(UserRole role, string? providerName = null, string? hospitalLocation = null)
        {
            var allTreatments = _dataService.Treatments.ToList();

            return role switch
            {
                UserRole.Clerk => allTreatments, // Can view but not details
                UserRole.Administrator => allTreatments, // Can view but not details
                UserRole.Doctor => allTreatments.Where(t => t.Provider == providerName).ToList(),
                UserRole.Nurse => allTreatments.Where(t => t.Provider == providerName && t.Hospital == hospitalLocation).ToList(),
                _ => new List<Treatment>()
            };
        }
    }
} 