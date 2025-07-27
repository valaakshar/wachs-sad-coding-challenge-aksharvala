using HospitalManagementSystem.Models;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace HospitalManagementSystem.Services
{
    public class DataService
    {
        private readonly List<Patient> _patients = new();
        private readonly List<Hospital> _hospitals = new();
        private readonly List<Provider> _providers = new();
        private readonly List<Treatment> _treatments = new();

        public IReadOnlyList<Patient> Patients => _patients.AsReadOnly();
        public IReadOnlyList<Hospital> Hospitals => _hospitals.AsReadOnly();
        public IReadOnlyList<Provider> Providers => _providers.AsReadOnly();
        public IReadOnlyList<Treatment> Treatments => _treatments.AsReadOnly();

        public void LoadData()
        {
            LoadPatients();
            LoadHospitals();
            LoadProviders();
            LoadTreatments();
            ValidateReferences();
        }

        private void LoadPatients()
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null,
                TrimOptions = TrimOptions.Trim
            };

            using var reader = new StreamReader("Patients.csv");
            using var csv = new CsvReader(reader, config);
            
            csv.Read();
            csv.ReadHeader();
            
            while (csv.Read())
            {
                var medicalRef = csv.GetField("Medical Reference Number") ?? "";
                var patientName = csv.GetField("Patient Name") ?? "";
                
                // Skip rows with empty medical reference number or patient name
                if (string.IsNullOrWhiteSpace(medicalRef) || string.IsNullOrWhiteSpace(patientName))
                    continue;
                
                var patient = new Patient
                {
                    MedicalReferenceNumber = medicalRef,
                    PatientName = NormalizeName(patientName)
                };

                if (patient.IsValid())
                {
                    _patients.Add(patient);
                }
            }
        }

        private void LoadHospitals()
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null,
                TrimOptions = TrimOptions.Trim
            };

            using var reader = new StreamReader("Hospitals.csv");
            using var csv = new CsvReader(reader, config);
            
            csv.Read();
            csv.ReadHeader();
            
            while (csv.Read())
            {
                var name = csv.GetField("Name") ?? "";
                var identity = csv.GetField("Identity") ?? "";
                
                // Skip rows with empty name or identity
                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(identity))
                    continue;
                
                var hospital = new Hospital
                {
                    Name = NormalizeName(name),
                    Identity = identity
                };

                if (hospital.IsValid())
                {
                    _hospitals.Add(hospital);
                }
            }
        }

        private void LoadProviders()
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null,
                TrimOptions = TrimOptions.Trim
            };

            using var reader = new StreamReader("Providers.csv");
            using var csv = new CsvReader(reader, config);
            
            csv.Read();
            csv.ReadHeader();
            
            while (csv.Read())
            {
                var name = csv.GetField("Name") ?? "";
                var number = csv.GetField("Number") ?? "";
                
                // Skip rows with empty name or number
                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(number))
                    continue;
                
                var provider = new Provider
                {
                    Name = NormalizeName(name),
                    Number = number,
                    Hospital = NormalizeName(csv.GetField("Hospital") ?? ""),
                    IsDoctor = csv.GetField("Doctor")?.Equals("Yes", StringComparison.OrdinalIgnoreCase) ?? false
                };

                if (provider.IsValid())
                {
                    _providers.Add(provider);
                }
            }
        }

        private void LoadTreatments()
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                MissingFieldFound = null,
                HeaderValidated = null,
                TrimOptions = TrimOptions.Trim
            };

            using var reader = new StreamReader("Treatments.csv");
            using var csv = new CsvReader(reader, config);
            
            csv.Read();
            csv.ReadHeader();
            
            while (csv.Read())
            {
                var hospital = csv.GetField("Hospital") ?? "";
                var patient = csv.GetField("Patient") ?? "";
                
                // Skip rows with empty hospital or patient (required fields)
                if (string.IsNullOrWhiteSpace(hospital) || string.IsNullOrWhiteSpace(patient))
                    continue;
                
                var treatment = new Treatment
                {
                    Details = csv.GetField("Details") ?? "",
                    Hospital = NormalizeName(hospital),
                    Provider = NormalizeName(csv.GetField("Provider") ?? ""),
                    Patient = patient,
                    DateTimeDischarged = ParseDateTime(csv.GetField("Date/Time Discharged"))
                };

                if (treatment.IsValid())
                {
                    _treatments.Add(treatment);
                }
            }
        }

        private void ValidateReferences()
        {
            // Remove treatments that reference non-existent hospitals, patients, or providers
            _treatments.RemoveAll(t => 
                !_hospitals.Any(h => h.Name.Equals(t.Hospital, StringComparison.OrdinalIgnoreCase)) ||
                !_patients.Any(p => p.MedicalReferenceNumber.Equals(t.Patient, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrWhiteSpace(t.Provider) && !_providers.Any(p => p.Name.Equals(t.Provider, StringComparison.OrdinalIgnoreCase))));

            // Remove providers that reference non-existent hospitals
            _providers.RemoveAll(p => 
                !string.IsNullOrWhiteSpace(p.Hospital) && 
                !_hospitals.Any(h => h.Name.Equals(p.Hospital, StringComparison.OrdinalIgnoreCase)));
        }

        private DateTime? ParseDateTime(string dateTimeStr)
        {
            if (string.IsNullOrWhiteSpace(dateTimeStr))
                return null;

            if (DateTime.TryParseExact(dateTimeStr, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
                return result;

            return null;
        }

        private string NormalizeName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return name;

            // Normalize multiple spaces to single spaces
            return string.Join(" ", name.Split(' ', StringSplitOptions.RemoveEmptyEntries));
        }

        // Business logic methods
        public List<Provider> GetProvidersByHospital(string hospitalName)
        {
            return _providers.Where(p => p.Hospital.Equals(hospitalName, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        public List<string> GetPatientNamesByProvider(string providerName)
        {
            return _treatments
                .Where(t => t.Provider.Equals(providerName, StringComparison.OrdinalIgnoreCase))
                .Select(t => _patients.FirstOrDefault(p => p.MedicalReferenceNumber.Equals(t.Patient, StringComparison.OrdinalIgnoreCase))?.PatientName)
                .Where(name => !string.IsNullOrEmpty(name))
                .Distinct()
                .ToList();
        }

        public List<string> GetPatientMedicalRefsByDoctor(string doctorName)
        {
            var doctor = _providers.FirstOrDefault(p => p.Name.Equals(doctorName, StringComparison.OrdinalIgnoreCase) && p.IsDoctor);
            if (doctor == null) return new List<string>();

            return _treatments
                .Where(t => t.Provider.Equals(doctorName, StringComparison.OrdinalIgnoreCase))
                .Select(t => t.Patient)
                .Distinct()
                .ToList();
        }

        public List<string> GetPatientsByDoctorAtHospital(string doctorName, string hospitalName)
        {
            return _treatments
                .Where(t => t.Provider.Equals(doctorName, StringComparison.OrdinalIgnoreCase) && 
                           t.Hospital.Equals(hospitalName, StringComparison.OrdinalIgnoreCase))
                .Select(t => _patients.FirstOrDefault(p => p.MedicalReferenceNumber.Equals(t.Patient, StringComparison.OrdinalIgnoreCase))?.PatientName)
                .Where(name => !string.IsNullOrEmpty(name))
                .Distinct()
                .ToList();
        }

        public List<string> GetUntreatedPatientsByHospital(string hospitalName)
        {
            var treatedPatients = _treatments
                .Where(t => t.Hospital.Equals(hospitalName, StringComparison.OrdinalIgnoreCase))
                .Select(t => t.Patient)
                .Distinct()
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            return _patients
                .Where(p => !treatedPatients.Contains(p.MedicalReferenceNumber))
                .Select(p => p.PatientName)
                .ToList();
        }

        public void AddTreatment(Treatment treatment)
        {
            if (treatment.IsValid())
            {
                _treatments.Add(treatment);
            }
        }

        public void UpdateTreatment(int index, Treatment treatment)
        {
            if (treatment.IsValid() && index >= 0 && index < _treatments.Count)
            {
                _treatments[index] = treatment;
            }
        }
    }
} 