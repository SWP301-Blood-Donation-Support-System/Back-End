using BusinessLayer.IService;
using BusinessLayer.Utils;
using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using Microsoft.Extensions.Options;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BusinessLayer.Service                         
{
    public class CertificateService : ICertificateService
    {
        private readonly IDonationRegistrationRepository _registrationRepository;
        private readonly CertificateSettings _certificateSettings;
        private static readonly string ArialFontPath = Path.Combine("Utils", "Font", "arial.ttf");

        // The hex code for rgb(245, 245, 220) is #F5F5DC
        private static readonly string BeigeBgColor = "#F5F5DC";

        // The hex code for rgb(178, 34, 34) is #B22222
        private static readonly string DarkRedColor = "#B22222";

        public CertificateService(
            IDonationRegistrationRepository registrationRepository,
            IOptions<CertificateSettings> certificateSettings)
        {
            _registrationRepository = registrationRepository ??
                throw new ArgumentNullException(nameof(registrationRepository));
            _certificateSettings = certificateSettings.Value;
            
            // Set QuestPDF license - use community license for open-source projects
            QuestPDF.Settings.License = LicenseType.Community;

            // Register Arial font
            if (File.Exists(ArialFontPath))
            {
                FontManager.RegisterFont(File.OpenRead(ArialFontPath));
            }
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

            // Format birth date
            string birthDateStr = "";
            if (donor.DateOfBirth.HasValue)
            {
                var birthDay = donor.DateOfBirth.Value;
                birthDateStr = $"{birthDay.Day} / {birthDay.Month} / {birthDay.Year}";
            }

            // Format current date
            var currentDate = record.DonationDateTime;
            string currentDateStr = $"HCM, ngày {currentDate.Day} tháng {currentDate.Month} năm {currentDate.Year}";

            // Determine the volume checkbox markers
            string vol250 = " [ ]";
            string vol350 = " [ ]";
            string vol450 = " [ ]";

            if (record.VolumeDonated.HasValue)
            {
                decimal donatedVolume = record.VolumeDonated.Value;
                if (donatedVolume <= 250) vol250 = " [x]";
                else if (donatedVolume <= 350) vol350 = " [x]";
                else vol450 = " [x]";
            }

            // Generate PDF certificate using QuestPDF
            return GenerateCertificatePdf(
                certificateId: record.CertificateId ?? "BDC-00029-202506-001",
                donorName: donor.FullName ?? "",
                birthDate: birthDateStr,
                nationalId: donor.NationalId ?? "",
                address: donor.Address ?? "",
                bloodType: donor.BloodType?.BloodTypeName ?? "Chưa biết",
                currentDate: currentDateStr,
                vol250: vol250,
                vol350: vol350,
                vol450: vol450,
                volumeDonated: record.VolumeDonated?.ToString("0.##") ?? ""
            );
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

        private byte[] GenerateCertificatePdf(
            string certificateId,
            string donorName,
            string birthDate,
            string nationalId,
            string address,
            string bloodType,
            string currentDate,
            string vol250,
            string vol350,
            string vol450,
            string volumeDonated)
        {
            return Document.Create(document =>
            {
                document.Page(page =>
                {
                    // Configure page settings to match A3 landscape
                    page.Size(PageSizes.A3.Landscape());
                    page.Margin(0);
                    page.PageColor(BeigeBgColor); // Light beige background
                    page.DefaultTextStyle(x => x.FontFamily("Arial"));
                    // Main content container
                    page.Content()
                        .PaddingHorizontal(10)
                        .PaddingVertical(10)
                        .Row(row =>
                        {
                            // Left panel
                            row.RelativeItem().Border(2).BorderColor(DarkRedColor)
                                .Padding(20)
                                .Column(leftColumn =>
                                {
                                    // Title
                                    leftColumn.Item().AlignCenter().Text("HIẾN MÁU CỨU NGƯỜI\nMỘT NGHĨA CỬ CAO ĐẸP")
                                        .FontSize(22)
                                        .FontColor(DarkRedColor)
                                        .Bold();

                                    // Notes section
                                    leftColumn.Item().PaddingTop(30).Column(notes =>
                                    {
                                        notes.Item().Text("+ Giấy chứng nhận này được trao cho người hiến máu sau mỗi lần hiến máu tình nguyện.")
                                            .FontSize(18)
                                            .FontColor(DarkRedColor);
                                            
                                        notes.Item().PaddingTop(8).Text("+ Có giá trị để truyền máu miễn phí bằng số lượng máu đã hiến, khi bản thân người hiến máu có nhu cầu sử dụng máu, tại tất cả các cơ sở y tế công lập trên toàn quốc.")
                                            .FontSize(18)
                                            .FontColor(DarkRedColor);
                                            
                                        notes.Item().PaddingTop(8).Text("+ Người hiến máu cần xuất trình Giấy chứng nhận này để làm cơ sở cho các cơ sở y tế thực hiện việc truyền máu miễn phí.")
                                            .FontSize(18)
                                            .FontColor(DarkRedColor);
                                            
                                        notes.Item().PaddingTop(8).Text("+ Cơ sở y tế có trách nhiệm ký, đóng dấu, xác nhận số lượng máu đã truyền miễn phí cho người hiến máu vào giấy chứng nhận.")
                                            .FontSize(18)
                                            .FontColor(DarkRedColor);
                                    });

                                    // Horizontal line
                                    leftColumn.Item().PaddingTop(60)
                                        .BorderBottom(2)
                                        .BorderColor(DarkRedColor)
                                        .PaddingBottom(20);

                                    // Hospital signature section
                                    leftColumn.Item().PaddingTop(50).AlignCenter().Column(sign =>
                                    {
                                        sign.Item().AlignCenter().Text("CHỨNG NHẬN CỦA CƠ SỞ Y TẾ\nĐÃ TRUYỀN MÁU")
                                            .FontSize(20)
                                            .FontColor(DarkRedColor)
                                            .Bold();
                                            
                                        sign.Item().AlignCenter().PaddingTop(40).Text(currentDate)
                                            .FontSize(20)
                                            .FontColor(DarkRedColor);
                                            
                                        sign.Item().AlignCenter().PaddingTop(20).Text($"Số lượng: {volumeDonated} ml")
                                            .FontSize(20)
                                            .FontColor(DarkRedColor);
                                    });
                                });

                            // Spacing between panels
                            row.ConstantItem(20);

                            // Right panel
                            row.RelativeItem().Border(2).BorderColor(DarkRedColor)
                                .Padding(20)
                                .Column(rightColumn =>
                                {
                                    // Header - first line
                                    rightColumn.Item().AlignCenter().Text("CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM")
                                        .FontSize(18)
                                        .FontColor(DarkRedColor)
                                        .Bold();
                                    
                                    // Header - second line
                                    rightColumn.Item().AlignCenter().Text("Độc lập - Tự do - Hạnh phúc")
                                        .FontSize(18)
                                        .FontColor(DarkRedColor)
                                        .Bold();
                                    rightColumn.Item()
                                    .PaddingTop(10)
                                    .BorderBottom(1)
                                    .BorderColor(DarkRedColor);

                                    // Title
                                    rightColumn.Item().PaddingTop(20).AlignCenter().Text("GIẤY CHỨNG NHẬN\nHIẾN MÁU TÌNH NGUYỆN")
                                        .FontColor(DarkRedColor)
                                        .FontSize(26)
                                        .Bold();

                                    // Info section
                                    rightColumn.Item().PaddingTop(25).Column(info =>
                                    {
                                        // Location
                                        info.Item().AlignLeft().Text("Ban chỉ đạo hiến máu nhân đạo tỉnh/thành phố: HCM")
                                            .FontSize(20)
                                            .FontColor(DarkRedColor);

                                        // Certification
                                        info.Item().PaddingTop(25).AlignCenter().Text("Chứng nhận:")
                                            .FontSize(22)
                                            .FontColor(DarkRedColor)
                                            .Bold();

                                        // Donor information
                                        info.Item().PaddingTop(15).Column(donor => 
                                        {
                                            donor.Spacing(8);
                                            
                                            donor.Item().Text(text =>
                                            {
                                                text.Span("Ông/Bà: ").FontSize(18).FontColor(DarkRedColor);
                                                text.Span(donorName).FontSize(18).Bold().FontColor(DarkRedColor);
                                            });
                                            
                                            donor.Item().Text(text =>
                                            {
                                                text.Span("Sinh ngày: ").FontSize(18).FontColor(DarkRedColor);
                                                text.Span(birthDate).FontSize(18).Bold().FontColor(DarkRedColor);
                                            });
                                            
                                            donor.Item().Text(text =>
                                            {
                                                text.Span("Số CCCD: ").FontSize(18).FontColor(DarkRedColor);
                                                text.Span(nationalId).FontSize(18).Bold().FontColor(DarkRedColor);
                                            });
                                            
                                            donor.Item().Text(text =>
                                            {
                                                text.Span("Địa chỉ: ").FontSize(18).FontColor(DarkRedColor);
                                                text.Span(address).FontSize(18).Bold().FontColor(DarkRedColor);
                                            });
                                        });

                                        // Donation information
                                        info.Item().PaddingTop(25).AlignCenter().Text("Đã hiến máu tình nguyện")
                                            .FontSize(22)
                                            .FontColor(DarkRedColor)
                                            .Bold();
                                            
                                        info.Item().PaddingTop(15).Text(text => 
                                        {
                                            text.Span("Tại cơ sở tiếp nhận máu: ").FontSize(18).FontColor(DarkRedColor);
                                            text.Span("BloodDonation").FontSize(18).FontColor(DarkRedColor);
                                        });

                                        // Volume checkboxes
                                        info.Item().PaddingTop(15).Text(text => 
                                        {
                                            text.Span("Số lượng: ").FontSize(18).FontColor(DarkRedColor);
                                            text.Span($"250ml{vol250} 350ml{vol350} 450ml{vol450}").FontSize(18).FontColor(DarkRedColor).FontFamily("Arial");
                                        });

                                        // Thank you note
                                        info.Item().PaddingTop(15).Text("Người bệnh luôn ghi ơn tấm lòng nhân ái của Ông/Bà.")
                                            .FontSize(18)
                                            .FontColor(DarkRedColor);

                                        // Blood type and date row
                                        info.Item().PaddingTop(25).Row(row =>
                                        {
                                            row.RelativeItem().Text(text => 
                                            {
                                                text.Span("Nhóm máu: ").FontSize(18).FontColor(DarkRedColor);
                                                text.Span(bloodType).FontSize(18).FontColor(DarkRedColor);
                                            });
                                                
                                            row.RelativeItem().AlignRight().Text(currentDate)
                                                .FontSize(18)
                                                .FontColor(DarkRedColor);
                                        });

                                        // Signature section
                                        info.Item().PaddingTop(30).AlignRight().Column(sign =>
                                        {
                                            sign.Item().AlignCenter().Text("T/M BAN CHỈ ĐẠO")
                                                .FontSize(18)
                                                .FontColor(DarkRedColor)
                                                .Bold();
                                                
                                            sign.Item().AlignCenter().Text("(Ký tên, đóng dấu)")
                                                .FontSize(18)
                                                .FontColor(DarkRedColor);
                                                
                                            // Add more vertical space for signature
                                            sign.Item().PaddingTop(60);
                                        });

                                        // Certificate ID
                                        info.Item().PaddingTop(40).AlignLeft().Text(text => 
                                        {
                                            text.Span("Số: ").FontSize(18).Bold().FontColor(DarkRedColor);
                                            text.Span(certificateId).FontSize(18).Bold().FontColor(DarkRedColor);
                                        });
                                    });
                                });
                        });
                });
            }).GeneratePdf();
        }
    }
}