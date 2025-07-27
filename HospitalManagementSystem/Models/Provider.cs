using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace HospitalManagementSystem.Models
{
    public class Provider
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        public string Number { get; set; } = string.Empty;
        
        public string Hospital { get; set; } = string.Empty;
        
        public bool IsDoctor { get; set; }

        public bool IsValid()
        {
            // All providers must have a name and number
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Number))
                return false;

            // The same rules for client names apply to provider names
            var words = Name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (words.Length < 2)
                return false;

            var alphabeticalChars = Name.Count(char.IsLetter);
            if (alphabeticalChars < 5)
                return false;

            // Providers may not have numbers or punctuation other than an apostrophe (') or hyphen (-) in their name
            var invalidPattern = @"[^a-zA-Z\s'\-]";
            if (Regex.IsMatch(Name, invalidPattern))
                return false;

            return true;
        }
    }
} 