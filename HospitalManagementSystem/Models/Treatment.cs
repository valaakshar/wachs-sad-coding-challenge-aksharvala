using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.Models
{
    public class Treatment
    {
        public string Details { get; set; } = string.Empty;
        
        [Required]
        public string Hospital { get; set; } = string.Empty;
        
        public string Provider { get; set; } = string.Empty;
        
        [Required]
        public string Patient { get; set; } = string.Empty;
        
        public DateTime? DateTimeDischarged { get; set; }

        public bool IsValid()
        {
            // All treatments must have a hospital and patient
            if (string.IsNullOrWhiteSpace(Hospital) || string.IsNullOrWhiteSpace(Patient))
                return false;

            // If a treatment has a dispatched date, it must have a provider and details
            if (DateTimeDischarged.HasValue)
            {
                if (string.IsNullOrWhiteSpace(Provider) || string.IsNullOrWhiteSpace(Details))
                    return false;
            }

            return true;
        }
    }
} 