using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace HospitalManagementSystem.Models
{
    public class Patient
    {
        [Required]
        public string MedicalReferenceNumber { get; set; } = string.Empty;
        
        [Required]
        public string PatientName { get; set; } = string.Empty;

        public bool IsValid()
        {
            // All patients must have both a Medical Reference Number, and a name
            if (string.IsNullOrWhiteSpace(MedicalReferenceNumber) || string.IsNullOrWhiteSpace(PatientName))
                return false;

            // A patient's name must be at least 2 words, and at least 5 alphabetical characters
            var words = PatientName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (words.Length < 2)
                return false;

            var alphabeticalChars = PatientName.Count(char.IsLetter);
            if (alphabeticalChars < 5)
                return false;

            // Patients may not have numbers or punctuation other than an apostrophe (') or hyphen (-) in their name
            var invalidPattern = @"[^a-zA-Z\s'\-]";
            if (Regex.IsMatch(PatientName, invalidPattern))
                return false;

            return true;
        }
    }
} 