using DataAccessLayer.DTO;
using DataAccessLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.IService
{
    public interface IEmergencyBloodEmailService
    {
        Task SendEmergencyBloodRequestEmailAsync(int bloodRequestId);
        Task SendEmergencyBloodRequestToCompatibleDonorsAsync(int bloodRequestId, int bloodTypeId, int componentId);
    }
}