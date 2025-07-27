using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.Models
{
    public class UserSession
    {
        public UserRole Role { get; set; }
        public string? ProviderName { get; set; } // For doctors/nurses to identify themselves
        public string? HospitalLocation { get; set; } // For nurses to restrict to their location
    }
} 