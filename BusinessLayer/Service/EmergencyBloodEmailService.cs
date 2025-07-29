using AutoMapper;
using BusinessLayer.IService;
using BuisinessLayer.Utils.EmailConfiguration;
using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class EmergencyBloodEmailService : IEmergencyBloodEmailService
    {
        private readonly IBloodRequestService _bloodRequestService;
        private readonly IUserRepository _userRepository;
        private readonly IBloodCompatibilityService _bloodCompatibilityService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;

        public EmergencyBloodEmailService(
            IBloodRequestService bloodRequestService,
            IUserRepository userRepository,
            IBloodCompatibilityService bloodCompatibilityService,
            IEmailService emailService,
            IMapper mapper)
        {
            _bloodRequestService = bloodRequestService;
            _userRepository = userRepository;
            _bloodCompatibilityService = bloodCompatibilityService;
            _emailService = emailService;
            _mapper = mapper;
        }

        private DateTime GetVietnamTime()
        {
            try
            {
                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now, vietnamTimeZone);
            }
            catch (TimeZoneNotFoundException)
            {
                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.Now, vietnamTimeZone);
            }
            catch (Exception)
            {
                return DateTime.Now.AddHours(7);
            }
        }

        public async Task SendEmergencyBloodRequestEmailAsync(int bloodRequestId)
        {
            try
            {
                var bloodRequest = await _bloodRequestService.GetBloodRequestsByIdAsync(bloodRequestId);
                
                if (bloodRequest == null)
                {
                    throw new ArgumentException($"Blood request with ID {bloodRequestId} not found");
                }

                var compatibleBloodTypeIds = await _bloodCompatibilityService.GetAllCompatibleDonorBloodTypeIdsAsync(bloodRequest.BloodTypeId);
                var eligibleDonors = new List<User>();
                
                foreach (var compatibleBloodTypeId in compatibleBloodTypeIds)
                {
                    var donorsWithBloodType = await _userRepository.GetByBloodTypeIdAsync(compatibleBloodTypeId);
                    var activeDonors = donorsWithBloodType.Where(u => 
                        u.IsActive && 
                        !u.IsDeleted && 
                        u.RoleId == 3 && 
                        !string.IsNullOrEmpty(u.Email) &&
                        u.DonationAvailabilityId == 1 && 
                        (u.NextEligibleDonationDate == null || u.NextEligibleDonationDate <= DateTime.Now)).ToList();
                    
                    eligibleDonors.AddRange(activeDonors);
                }

                eligibleDonors = eligibleDonors.GroupBy(d => d.UserId).Select(g => g.First()).ToList();

                Console.WriteLine($"Found {eligibleDonors.Count} eligible donors for emergency blood request {bloodRequestId}");

                foreach (var donor in eligibleDonors)
                {
                    await SendEmergencyBloodEmailToDonor(donor, bloodRequest, compatibleBloodTypeIds);
                }

                Console.WriteLine($"Emergency blood request emails sent to {eligibleDonors.Count} donors");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending emergency blood request emails: {ex.Message}");
                throw;
            }
        }

        public async Task SendEmergencyBloodRequestToCompatibleDonorsAsync(int bloodRequestId, int bloodTypeId, int componentId)
        {
            await SendEmergencyBloodRequestEmailAsync(bloodRequestId);
        }

        private async Task SendEmergencyBloodEmailToDonor(User donor, BloodRequest bloodRequest, IEnumerable<int> compatibleBloodTypeIds)
        {
            try
            {
                var subject = "KHẨN CẤP: Cần hiến máu cứu người!";
                var htmlBody = await GenerateEmergencyBloodEmailTemplate(donor, bloodRequest, compatibleBloodTypeIds);

                var message = new Message(
                    to: new string[] { donor.Email },
                    subject: subject,
                    content: htmlBody);

                _emailService.SendEmail(message);

                Console.WriteLine($"Emergency blood email sent successfully to {donor.Email}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending emergency blood email to {donor.Email}: {ex.Message}");
            }
        }

        private async Task<string> GenerateEmergencyBloodEmailTemplate(User donor, BloodRequest bloodRequest, IEnumerable<int> compatibleBloodTypeIds)
        {
            var displayName = !string.IsNullOrEmpty(donor.FullName) ? donor.FullName : donor.Email;
            var currentDate = GetVietnamTime().ToString("dd/MM/yyyy HH:mm");
            
            var vietnameseCulture = new CultureInfo("vi-VN");
            var requiredDate = bloodRequest.RequiredDateTime?.ToString("dddd, 'ngày' dd/MM/yyyy", vietnameseCulture) ?? "Ngay lập tức";
            
            var urgencyText = GetUrgencyText(bloodRequest.UrgencyId);
            var compatibleBloodTypeNames = await GetCompatibleBloodTypeNames(compatibleBloodTypeIds);
            var donorBloodTypeName = donor.BloodType?.BloodTypeName ?? "Chưa xác định";
            
            var isCompatible = compatibleBloodTypeIds.Contains(donor.BloodTypeId ?? 0);
            var compatibilityMessage = GenerateCompatibilityMessage(donorBloodTypeName, bloodRequest.BloodType?.BloodTypeName, isCompatible);

            var htmlTemplate = $@"
<!DOCTYPE html>
<html lang='vi'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Yêu cầu Hiến máu Khẩn cấp</title>
    <style>
        body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0; }}
        .container {{ max-width: 600px; margin: 0 auto; background-color: white; padding: 20px; border-radius: 8px; }}
        .header {{ background-color: #dc3545; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
        .content {{ padding: 20px 0; }}
        .blood-info {{ background-color: #f8d7da; padding: 15px; border-radius: 4px; margin: 15px 0; }}
        .compatibility-info {{ background-color: #d4edda; padding: 15px; border-radius: 4px; margin: 15px 0; }}
        .blood-type-badge {{ background-color: #28a745; color: white; padding: 2px 8px; border-radius: 12px; font-size: 12px; margin-right: 5px; }}
        .footer {{ text-align: center; margin-top: 20px; padding-top: 20px; border-top: 1px solid #ddd; color: #666; }}
        table {{ width: 100%; border-collapse: collapse; }}
        td {{ padding: 5px; border-bottom: 1px solid #eee; }}
        .label {{ font-weight: bold; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>KHẨN CẤP: Cần hiến máu cứu người!</h1>
            <p>Bạn có thể là người hùng mang lại hy vọng!</p>
        </div>
        <div class='content'>
            <h2>Kính chào {displayName}!</h2>
            <p>Chúng tôi có một yêu cầu hiến máu KHẨN CẤP cần sự giúp đỡ của bạn. Một bệnh nhân đang rất cần máu để tiếp tục cuộc sống.</p>
            
            <div class='blood-info'>
                <h3>Thông tin yêu cầu hiến máu</h3>
                <table>
                    <tr><td class='label'>Nhóm máu cần:</td><td>{bloodRequest.BloodType?.BloodTypeName ?? "Chưa xác định"}</td></tr>
                    <tr><td class='label'>Mức độ khẩn cấp:</td><td>{urgencyText}</td></tr>
                    <tr><td class='label'>Thời gian cần:</td><td>{requiredDate}</td></tr>
                </table>
            </div>

            <div class='compatibility-info'>
                <h3>Thông tin tương thích</h3>
                <p>{compatibilityMessage}</p>
                <p><strong>Các nhóm máu có thể hiến:</strong></p>
                <div>
                    {string.Join("", compatibleBloodTypeNames.Select(bt => $"<span class='blood-type-badge'>{bt}</span>"))}
                </div>
            </div>

            <div style='background: #d1ecf1; padding: 20px; text-align: center; border-radius: 4px; margin: 20px 0;'>
                <h3>Bạn có thể tham gia hiến máu ngay!</h3>
                <p>Mỗi đơn vị máu bạn hiến có thể cứu được tới 3 người.</p>
                <p><strong>Liên hệ: giotmaunghiatinh@gmail.com</strong></p>
            </div>

            <div style='background: #fff3cd; padding: 15px; border-radius: 4px;'>
                <h4>Lưu ý quan trọng:</h4>
                <ul>
                    <li>Đảm bảo bạn đủ sức khỏe để hiến máu</li>
                    <li>Không uống rượu bia trong 24h trước khi hiến máu</li>
                    <li>Ăn no và uống đủ nước trước khi hiến máu</li>
                    <li>Mang theo giấy tờ tùy thân</li>
                    <li>Đảm bảo bạn đã ngủ đủ giấc trước khi hiến máu</li> 
                </ul>
            </div>
        </div>
        
        <div class='footer'>
            <p><strong>""Hiến máu cứu người - Nghĩa cử cao đẹp""</strong></p>
            <p>Cảm ơn bạn đã quan tâm và sẵn sàng giúp đỡ!</p>
            <p>Email được gửi lúc: {currentDate}</p>
        </div>
    </div>
</body>
</html>";

            return htmlTemplate;
        }

        private string GetUrgencyClass(int? urgencyId)
        {
            return urgencyId switch
            {
                1 => "high",
                2 => "medium",
                3 => "low", 
                _ => "high"
            };
        }

        private async Task<List<string>> GetCompatibleBloodTypeNames(IEnumerable<int> bloodTypeIds)
        {
            var bloodTypeNames = new List<string>();
            
            foreach (var bloodTypeId in bloodTypeIds)
            {
                try
                {
                    var usersWithBloodType = await _userRepository.GetByBloodTypeIdAsync(bloodTypeId);
                    var firstUser = usersWithBloodType.FirstOrDefault();
                    if (firstUser?.BloodType != null)
                    {
                        bloodTypeNames.Add(firstUser.BloodType.BloodTypeName);
                    }
                    else
                    {
                        bloodTypeNames.Add(GetBloodTypeNameById(bloodTypeId));
                    }
                }
                catch
                {
                    bloodTypeNames.Add(GetBloodTypeNameById(bloodTypeId));
                }
            }
            
            return bloodTypeNames.Distinct().ToList();
        }

        private string GetBloodTypeNameById(int bloodTypeId)
        {
            return bloodTypeId switch
            {
                1 => "A+",
                2 => "A-",
                3 => "B+",
                4 => "B-",
                5 => "AB+",
                6 => "AB-",
                7 => "O+",
                8 => "O-",
                _ => $"BloodType#{bloodTypeId}"
            };
        }

        private string GenerateCompatibilityMessage(string donorBloodType, string recipientBloodType, bool isCompatible)
        {
            if (!isCompatible)
            {
                return "Có vẻ như có lỗi trong hệ thống tương thích máu. Vui lòng liên hệ để xác nhận.";
            }

            if (donorBloodType == recipientBloodType)
            {
                return $"Nhóm máu của bạn ({donorBloodType}) hoàn toàn phù hợp với bệnh nhân cần máu ({recipientBloodType}).";
            }

            return $"Nhóm máu của bạn ({donorBloodType}) có thể hiến máu cho bệnh nhân nhóm máu {recipientBloodType} một cách an toàn.";
        }

        private string GetUrgencyText(int? urgencyId)
        {
            return urgencyId switch
            {
                1 => "RẤT KHẨN CẤP",
                2 => "KHẨN CẤP",
                3 => "CẦN THIẾT",
                _ => "KHẨN CẤP"
            };
        }

        private string GetUrgencyColor(int? urgencyId)
        {
            return urgencyId switch
            {
                1 => "#d32f2f",
                2 => "#f57c00", 
                3 => "#7b1fa2",
                _ => "#dc3545"
            };
        }
    }
}