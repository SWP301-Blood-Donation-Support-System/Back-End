using BusinessLayer.IService;
using BusinessLayer.Utils;
using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace BusinessLayer.Service
{
    public class CertificateService : ICertificateService
    {
        private readonly IDonationRegistrationRepository _registrationRepository;
        private readonly CertificateSettings _certificateSettings;

        public CertificateService(
            IDonationRegistrationRepository registrationRepository,
            IOptions<CertificateSettings> certificateSettings)
        {
            _registrationRepository = registrationRepository ??
                throw new ArgumentNullException(nameof(registrationRepository));
            _certificateSettings = certificateSettings.Value;
        }

        public async Task<byte[]> GenerateCertificateAsync(int registrationId)
        {
            // Get registration with donor and record data
            var registration = await _registrationRepository.GetRegistrationWithDonorAndRecordAsync(registrationId);

            if (registration == null || registration.DonationRecord == null || registration.Donor == null)
            {
                throw new KeyNotFoundException($"Donation record not found or incomplete");
            }

            var record = registration.DonationRecord;
            var donor = registration.Donor;

            // Get the template path from settings
            string templatePath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                _certificateSettings.TemplatePaths.BloodDonationCertificate);

            // Create a unique filename for the generated document
            string outputFileName = $"certificate_{record.CertificateId ?? Guid.NewGuid().ToString("N")}.docx";
            string outputPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                _certificateSettings.OutputDirectory,
                outputFileName);

            // Create the output directory if it doesn't exist
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

            // Load the template
            using (var doc = DocX.Load(templatePath))
            {
                // Format birth date
                string birthDateStr = "";
                if (donor.DateOfBirth.HasValue)
                {
                    var birthDay = donor.DateOfBirth.Value;
                    birthDateStr = $"{birthDay.Day} / {birthDay.Month} / {birthDay.Year}";
                }

                // Format current date
                var currentDate = DateTime.Now;
                string currentDateStr = $"{currentDate.Day} tháng {currentDate.Month} năm {currentDate.Year}";

                // Determine the volume checkbox
                string vol250 = "☐";
                string vol350 = "☐";
                string vol450 = "☐";

                if (record.VolumeDonated.HasValue)
                {
                    decimal donatedVolume = record.VolumeDonated.Value;
                    if (donatedVolume <= 250) vol250 = "☒";
                    else if (donatedVolume <= 350) vol350 = "☒";
                    else vol450 = "☒";
                }
                var donatedVol = record.VolumeDonated;

                // Use the new recommended approach with StringReplaceTextOptions
                var replacements = new Dictionary<string, string>
                {
                    {"{{DonorName}}", donor.FullName ?? ""},
                    {"{{DonorBirthDate}}", birthDateStr},
                    {"{{DonorNationalId}}", donor.NationalId ?? ""},
                    {"{{DonorAddress}}", donor.Address ?? ""},
                    {"{{BloodType}}", donor.BloodType?.BloodTypeName ?? "Ko thấy"},
                    {"{{CurrentDate}}", currentDateStr},
                    {"{{CertificateId}}", record.CertificateId ?? "B 15786935"},
                    {"{{Volume250}}", vol250},
                    {"{{Volume350}}", vol350},
                    {"{{Volume450}}", vol450},
                    {"{{VolumeDonated}}",donatedVol.Value.ToString("0.##")}
                };

                // Apply all replacements
                foreach (var replacement in replacements)
                {
                    // Use the modern replacement method
                    doc.ReplaceText(replacement.Key, replacement.Value, false, RegexOptions.None);
                }

                // Save the document to memory stream
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    doc.SaveAs(memoryStream);
                    memoryStream.Position = 0;
                    return memoryStream.ToArray();
                }
            }
        }

        public async Task<byte[]> GetCertificateByIdAsync(string certificateId)
        {
            if (string.IsNullOrEmpty(certificateId))
            {
                throw new ArgumentNullException(nameof(certificateId));
            }

            // Get the registration by certificate ID
            var registration = await _registrationRepository.GetRegistrationByCertificateIdAsync(certificateId);

            if (registration?.DonationRecord == null)
            {
                throw new KeyNotFoundException($"Certificate with ID {certificateId} not found");
            }

            // Generate the certificate
            return await GenerateCertificateAsync(registration.RegistrationId);
        }
    }
}