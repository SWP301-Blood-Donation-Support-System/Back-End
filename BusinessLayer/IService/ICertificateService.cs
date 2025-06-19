using System;
using System.Threading.Tasks;

namespace BusinessLayer.IService
{
    public interface ICertificateService
    {
        Task<byte[]> GenerateCertificateAsync(int registrationId);
        Task<byte[]> GetCertificateByIdAsync(string certificateId);
    }
} 