namespace BusinessLayer.Utils
{
    public class CertificateSettings
    {
        public TemplatePaths TemplatePaths { get; set; } = new();
        public string OutputDirectory { get; set; } = string.Empty;
    }

    public class TemplatePaths
    {
        public string BloodDonationCertificate { get; set; } = string.Empty;
    }
}