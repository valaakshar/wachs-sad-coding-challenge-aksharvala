using System.ComponentModel.DataAnnotations;

namespace HospitalManagementSystem.Models
{
    public class Hospital
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Identity { get; set; } = string.Empty;

        public bool IsValid()
        {
            // All hospitals must have a name and identity
            return !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Identity);
        }
    }
} 