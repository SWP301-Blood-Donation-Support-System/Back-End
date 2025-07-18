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
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
            }
            catch (TimeZoneNotFoundException)
            {
                TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
                return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vietnamTimeZone);
            }
            catch (Exception)
            {
                return DateTime.UtcNow.AddHours(7);
            }
        }

        public async Task SendEmergencyBloodRequestEmailAsync(int bloodRequestId)
        {
            try
            {
                // Get blood request details
                var bloodRequest = await _bloodRequestService.GetBloodRequestsByIdAsync(bloodRequestId);
                
                if (bloodRequest == null)
                {
                    throw new ArgumentException($"Blood request with ID {bloodRequestId} not found");
                }

                // Get compatible blood types for the request
                var compatibleBloodTypeIds = await _bloodCompatibilityService.GetAllCompatibleDonorBloodTypeIdsAsync(bloodRequest.BloodTypeId);
                
                // Get all eligible donors with compatible blood types
                var eligibleDonors = new List<User>();
                
                foreach (var compatibleBloodTypeId in compatibleBloodTypeIds)
                {
                    var donorsWithBloodType = await _userRepository.GetByBloodTypeIdAsync(compatibleBloodTypeId);
                    var activeDonors = donorsWithBloodType.Where(u => 
                        u.IsActive && 
                        !u.IsDeleted && 
                        u.RoleId == 3 && // Donor role
                        !string.IsNullOrEmpty(u.Email) &&
                        u.DonationAvailabilityId == 1 && // Available for donation
                        (u.NextEligibleDonationDate == null || u.NextEligibleDonationDate <= DateTime.UtcNow)).ToList();
                    
                    eligibleDonors.AddRange(activeDonors);
                }

                // Remove duplicates
                eligibleDonors = eligibleDonors.GroupBy(d => d.UserId).Select(g => g.First()).ToList();

                Console.WriteLine($"Found {eligibleDonors.Count} eligible donors for emergency blood request {bloodRequestId}");

                // Send email to each eligible donor
                foreach (var donor in eligibleDonors)
                {
                    await SendEmergencyBloodEmailToDonor(donor, bloodRequest);
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
            // Since the BloodRequest already contains BloodTypeId and BloodComponentId, 
            // we don't need the additional parameters. Just call the main method.
            await SendEmergencyBloodRequestEmailAsync(bloodRequestId);
        }

        private async Task SendEmergencyBloodEmailToDonor(User donor, BloodRequest bloodRequest)
        {
            try
            {
                var subject = "🚨 KHẨN CẤP: Cần hiến máu cứu người!";
                var htmlBody = GenerateEmergencyBloodEmailTemplate(donor, bloodRequest);

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

        private string GenerateEmergencyBloodEmailTemplate(User donor, BloodRequest bloodRequest)
        {
            var displayName = !string.IsNullOrEmpty(donor.FullName) ? donor.FullName : donor.Email;
            var currentDate = GetVietnamTime().ToString("dd/MM/yyyy HH:mm");
            
            var vietnameseCulture = new CultureInfo("vi-VN");
            var requiredDate = bloodRequest.RequiredDateTime?.ToString("dddd, 'ngày' dd/MM/yyyy", vietnameseCulture) ?? "Ngay lập tức";
            
            // Get urgency level text
            var urgencyText = GetUrgencyText(bloodRequest.UrgencyId);
            var urgencyColor = GetUrgencyColor(bloodRequest.UrgencyId);

            var sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html lang='vi'>");
            sb.AppendLine("<head>");
            sb.AppendLine("    <meta charset='UTF-8'>");
            sb.AppendLine("    <meta name='viewport' content='width=device-width, initial-scale=1.0'>");
            sb.AppendLine("    <title>Yêu cầu Hiến máu Khẩn cấp - Giọt Máu Nghĩa Tình</title>");
            sb.AppendLine("    <link href='https://fonts.googleapis.com/css2?family=Roboto:wght@400;700&display=swap' rel='stylesheet'>"); // Thêm font Roboto
            sb.AppendLine("    <style>");
            sb.AppendLine("        body { font-family: 'Roboto', Arial, sans-serif; background-color: #f0f2f5; margin: 0; padding: 0; line-height: 1.6; color: #333; }");
            sb.AppendLine("        .container { max-width: 600px; margin: 30px auto; background-color: white; border-radius: 12px; box-shadow: 0 6px 20px rgba(0,0,0,0.08); overflow: hidden; }");
            sb.AppendLine("        .header { background: linear-gradient(135deg, #e74c3c, #c0392b); color: white; padding: 40px 20px; text-align: center; position: relative; border-bottom: 5px solid #d62c1a; }");
            sb.AppendLine("        .header::before { content: ''; position: absolute; top: 0; left: 0; right: 0; bottom: 0; background: url('data:image/svg+xml,<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 100 100\"><circle cx=\"50\" cy=\"50\" r=\"3\" fill=\"%23ffffff\" opacity=\"0.1\"/></svg>') repeat; opacity: 0.5; }");
            sb.AppendLine("        .emergency-badge { background-color: #ff6b6b; color: white; padding: 10px 20px; border-radius: 25px; font-size: 15px; font-weight: bold; display: inline-block; margin-bottom: 20px; animation: pulse 1.5s infinite ease-in-out; letter-spacing: 1px; }");
            sb.AppendLine("        @keyframes pulse { 0% { transform: scale(1); box-shadow: 0 0 0 0 rgba(255, 107, 107, 0.7); } 70% { transform: scale(1.05); box-shadow: 0 0 0 10px rgba(255, 107, 107, 0); } 100% { transform: scale(1); box-shadow: 0 0 0 0 rgba(255, 107, 107, 0); } }");
            sb.AppendLine("        h1 { margin: 0; font-size: 32px; font-weight: 700; text-shadow: 1px 1px 2px rgba(0,0,0,0.1); }");
            sb.AppendLine("        .header p { margin: 10px 0 0; font-size: 17px; opacity: 0.95; }");
            sb.AppendLine("        .content { padding: 30px 35px; }");
            sb.AppendLine("        h2 { color: #2c3e50; font-size: 24px; margin-top: 0; margin-bottom: 20px; }");
            sb.AppendLine("        .blood-info { background: linear-gradient(135deg, #ffebee, #f8d7da); padding: 25px; border-radius: 10px; margin: 25px 0; border-left: 6px solid #e74c3c; box-shadow: 0 2px 8px rgba(0,0,0,0.05); }");
            sb.AppendLine("        .urgency-high { background: linear-gradient(135deg, #ffebee, #ffcdd2); border-left-color: #c0392b; }");
            sb.AppendLine("        .urgency-medium { background: linear-gradient(135deg, #fff3e0, #ffe0b2); border-left-color: #f39c12; }");
            sb.AppendLine("        .urgency-low { background: linear-gradient(135deg, #e0f2f7, #bbdefb); border-left-color: #3498db; }"); /* Màu xanh cho mức độ thấp */
            sb.AppendLine("        .blood-info h3 { margin-top: 0; color: #c0392b; font-size: 20px; display: flex; align-items: center; }");
            sb.AppendLine("        .blood-info h3 .blood-drop { margin-right: 10px; font-size: 28px; }");
            sb.AppendLine("        table { width: 100%; border-collapse: collapse; margin-top: 15px; }");
            sb.AppendLine("        td { padding: 10px 0; border-bottom: 1px dashed #eee; }");
            sb.AppendLine("        td:last-child { text-align: right; font-weight: bold; }");
            sb.AppendLine("        .info-label { font-weight: bold; color: #555; }");
            sb.AppendLine("        .info-value { color: #e74c3c; font-weight: bold; }");
            sb.AppendLine("        .cta-section { background: #ecf0f1; padding: 30px; border-radius: 10px; text-align: center; margin: 25px 0; border: 1px dashed #bdc3c7; }");
            sb.AppendLine("        .cta-section h3 { color: #e74c3c; margin-top: 0; font-size: 22px; display: flex; justify-content: center; align-items: center; }");
            sb.AppendLine("        .cta-section h3 .heart-icon { margin-right: 10px; font-size: 28px; }");
            sb.AppendLine("        .cta-section p { margin: 15px 0 25px; font-size: 16px; color: #555; }");
            sb.AppendLine("        .cta-button { background: linear-gradient(135deg, #2ecc71, #27ae60); color: white; padding: 16px 35px; border: none; border-radius: 30px; font-size: 18px; font-weight: bold; text-decoration: none; display: inline-block; transition: all 0.3s ease; box-shadow: 0 4px 10px rgba(46, 204, 113, 0.4); }");
            sb.AppendLine("        .cta-button:hover { transform: translateY(-3px); box-shadow: 0 8px 20px rgba(46, 204, 113, 0.6); background: linear-gradient(135deg, #27ae60, #2ecc71); }");
            sb.AppendLine("        .contact-info { background: #e8f5e9; padding: 25px; border-radius: 10px; margin: 25px 0; border: 1px solid #c8e6c9; }"); /* Màu xanh lá nhạt hơn */
            sb.AppendLine("        .contact-info h3 { color: #27ae60; margin-top: 0; font-size: 20px; display: flex; align-items: center; }");
            sb.AppendLine("        .contact-info h3 .info-icon { margin-right: 10px; font-size: 24px; }");
            sb.AppendLine("        .contact-info ul { margin: 15px 0 0; padding-left: 25px; list-style: none; }");
            sb.AppendLine("        .contact-info ul li { margin-bottom: 8px; color: #555; }");
            sb.AppendLine("        .contact-info ul li strong { color: #333; }");
            sb.AppendLine("        .contact-info p { margin: 15px 0 0; font-style: italic; color: #777; font-size: 15px; }");
            sb.AppendLine("        .note-section { background: #fdf6e3; padding: 25px; border-radius: 10px; border: 1px solid #f9e79f; margin: 25px 0; }");
            sb.AppendLine("        .note-section h3 { color: #d35400; margin-top: 0; font-size: 20px; display: flex; align-items: center; }");
            sb.AppendLine("        .note-section h3 .warning-icon { margin-right: 10px; font-size: 24px; }");
            sb.AppendLine("        .note-section ul { margin: 15px 0 0; padding-left: 25px; color: #8e44ad; }"); /* Màu tím nhẹ */
            sb.AppendLine("        .footer { text-align: center; padding: 25px; background: #34495e; color: white; border-top: 5px solid #2c3e50; font-size: 15px; }");
            sb.AppendLine("        .footer p { margin: 0; }");
            sb.AppendLine("        .footer .quote { font-size: 18px; color: #ecf0f1; font-weight: bold; margin-bottom: 10px; }");
            sb.AppendLine("        .footer .timestamp { font-size: 12px; color: #bdc3c7; margin-top: 10px; }");
            sb.AppendLine("        .icon { vertical-align: middle; margin-right: 5px; }"); /* Cho các icon nhỏ trong nội dung */
            sb.AppendLine("    </style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("    <div class='container'>");
            sb.AppendLine("        <div class='header'>");
            sb.AppendLine("            <div class='emergency-badge'>**KHẨN CẤP**</div>");
            sb.AppendLine("            <h1>Cần hiến máu cứu người!</h1>");
            sb.AppendLine("            <p>Bạn có thể là người hùng mang lại hy vọng!</p>");
            sb.AppendLine("        </div>");
            sb.AppendLine("        ");
            sb.AppendLine("        <div class='content'>");
            sb.AppendLine($"            <h2>Kính chào {displayName}!</h2>");
            sb.AppendLine("            <p style='font-size: 16px;'>Chúng tôi có một yêu cầu hiến máu **KHẨN CẤP** cần sự giúp đỡ của bạn. Một bệnh nhân đang rất cần máu để tiếp tục cuộc sống và bạn có thể là người mang lại tia sáng hy vọng cho họ!</p>");

            sb.AppendLine($"            <div class='blood-info urgency-{GetUrgencyClass(bloodRequest.UrgencyId)}'>");
            sb.AppendLine("                <h3><span class='blood-drop'>🩸</span> Thông tin yêu cầu hiến máu</h3>");
            sb.AppendLine("                <table>");
            sb.AppendLine($"                    <tr><td class='info-label'>Mức độ khẩn cấp:</td><td class='info-value' style='color: {urgencyColor};'>{urgencyText}</td></tr>");
            sb.AppendLine($"                    <tr><td class='info-label'>Nhóm máu cần:</td><td class='info-value'>{bloodRequest.BloodType?.BloodTypeName ?? "Chưa xác định"}</td></tr>");
            sb.AppendLine($"                    <tr><td class='info-label'>Thành phần máu:</td><td class='info-value'>{bloodRequest.BloodComponent?.ComponentName ?? "Chưa xác định"}</td></tr>");
            sb.AppendLine($"                    <tr><td class='info-label'>Thời gian cần:</td><td class='info-value'>{requiredDate}</td></tr>");
            sb.AppendLine("                </table>");
            sb.AppendLine("            </div>");

            sb.AppendLine("            <div class='cta-section'>");
            sb.AppendLine("                <h3><span class='heart-icon'>❤️</span> Bạn có thể giúp đỡ!</h3>");
            sb.AppendLine("                <p>Mỗi giọt máu bạn hiến đều mang ý nghĩa to lớn, có thể cứu sống một cuộc đời. Hãy cùng chúng tôi lan tỏa nghĩa cử cao đẹp này!</p>");
            sb.AppendLine("                <a href='mailto:giotmaunghiatinh@gmail.com' class='cta-button'>Đăng ký Hiến máu Ngay</a>"); // Nút CTA thay đổi
            sb.AppendLine("            </div>");

            sb.AppendLine("            <div class='contact-info'>");
            sb.AppendLine("                <h3><span class='info-icon'>ℹ️</span> Thông tin liên hệ</h3>");
            sb.AppendLine("                <ul>");
            sb.AppendLine("                    <li><strong>Email:</strong> giotmaunghiatinh@gmail.com</li>");
            sb.AppendLine("                    <li><strong>Địa chỉ:</strong> Trung tâm Hiến máu - Bệnh viện Trung ương</li>");
            sb.AppendLine("                </ul>");
            sb.AppendLine("                <p>Vui lòng liên hệ với chúng tôi qua email nếu bạn có bất kỳ câu hỏi nào hoặc có thể hỗ trợ!</p>");
            sb.AppendLine("            </div>");

            sb.AppendLine("            <div class='note-section'>");
            sb.AppendLine("                <h3><span class='warning-icon'>⚠️</span> Lưu ý quan trọng khi hiến máu</h3>");
            sb.AppendLine("                <ul>");
            sb.AppendLine("                    <li>Đảm bảo sức khỏe tốt trong 24 giờ qua.</li>");
            sb.AppendLine("                    <li>Không uống rượu bia hoặc các chất kích thích trong 24 giờ trước khi hiến.</li>");
            sb.AppendLine("                    <li>Ăn no và uống đủ nước trước khi đến điểm hiến máu.</li>");
            sb.AppendLine("                    <li>Mang theo CCCD/CMND để xác thực thông tin cá nhân.</li>");
            sb.AppendLine("                    <li>Nếu bạn có bất kỳ thắc mắc nào về điều kiện hiến máu, đừng ngần ngại liên hệ với chúng tôi.</li>");
            sb.AppendLine("                </ul>");
            sb.AppendLine("            </div>");

            sb.AppendLine("        </div>");

            sb.AppendLine("        <div class='footer'>");
            sb.AppendLine("            <p class='quote'>\"Hiến máu cứu người - Một giọt máu cho đi, một cuộc đời ở lại\"</p>");
            sb.AppendLine("            <p>Cảm ơn bạn đã luôn sẵn sàng giúp đỡ những người cần giúp đỡ!</p>");
            sb.AppendLine($"            <p class='timestamp'>Email được gửi lúc: {currentDate}</p>");
            sb.AppendLine("        </div>");
            sb.AppendLine("    </div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
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
    }
}