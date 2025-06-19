using BusinessLayer.IService;
using DataAccessLayer.Entity;
using DataAccessLayer.IRepository;
using iText.IO.Font;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Borders;
using iText.Layout.Element;
using iText.Layout.Properties;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class CertificateService : ICertificateService
    {
        private readonly IDonationRegistrationRepository _registrationRepository;
        
        public CertificateService(IDonationRegistrationRepository registrationRepository)
        {
            _registrationRepository = registrationRepository ?? 
                throw new ArgumentNullException(nameof(registrationRepository));
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
            
            // Generate the PDF
            using (MemoryStream ms = new MemoryStream())
            {
                // Create PDF writer and document
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf, PageSize.A5.Rotate());
                document.SetMargins(10, 10, 10, 10);
                
                // Create main table with two columns
                Table mainTable = new Table(2).UseAllAvailableWidth();
                
                // Left side of certificate
                Cell leftCell = new Cell();
                leftCell.SetBorder(new SolidBorder(ColorConstants.RED, 1));
                leftCell.SetPadding(10);
                
                // Title in red
                Paragraph title = new Paragraph("HIẾN MÁU CỨU NGƯỜI\nMỘT NGHĨA CỬ CAO ĐẸP")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontColor(ColorConstants.RED)
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                    .SetFontSize(14);
                leftCell.Add(title);
                
                // Points listed with numbers
                Paragraph point1 = new Paragraph("1. Giấy chứng nhận này được trao cho người hiến máu sau mỗi lần hiến máu tình nguyện.")
                    .SetFontColor(ColorConstants.RED)
                    .SetFontSize(10);
                leftCell.Add(point1);
                
                Paragraph point2 = new Paragraph("2. Có giá trị để được truyền máu miễn phí bằng số lượng máu đã hiến, khi bản thân người hiến máu có nhu cầu sử dụng máu tại tất cả các cơ sở y tế công lập trên toàn quốc.")
                    .SetFontColor(ColorConstants.RED)
                    .SetFontSize(10);
                leftCell.Add(point2);
                
                Paragraph point3 = new Paragraph("3. Người hiến máu cần xuất trình Giấy chứng nhận này để làm cơ sở cho các cơ sở y tế thực hiện việc truyền máu miễn phí.")
                    .SetFontColor(ColorConstants.RED)
                    .SetFontSize(10);
                leftCell.Add(point3);
                
                Paragraph point4 = new Paragraph("4. Cơ sở y tế có trách nhiệm ký, đóng dấu, xác nhận số lượng máu đã truyền miễn phí cho người hiến máu vào giấy chứng nhận.")
                    .SetFontColor(ColorConstants.RED)
                    .SetFontSize(10);
                leftCell.Add(point4);
                
                // Add horizontal line
                LineSeparator ls = new LineSeparator(new SolidLine(1f))
                    .SetMarginTop(10)
                    .SetMarginBottom(10)
                    .SetFontColor(ColorConstants.RED);
                leftCell.Add(ls);
                
                // Add medical facility certification section
                Paragraph medicalTitle = new Paragraph("CHỨNG NHẬN CỦA CƠ SỞ Y TẾ\nĐÃ TRUYỀN MÁU")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontColor(ColorConstants.RED)
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                    .SetFontSize(14);
                leftCell.Add(medicalTitle);
                
                // Add date and volume fields
                Paragraph dateField = new Paragraph("Ngày........tháng........năm.........")
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetFontColor(ColorConstants.RED)
                    .SetFontSize(10);
                leftCell.Add(dateField);
                
                Paragraph volumeField = new Paragraph("Số lượng: ...................ml")
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetFontColor(ColorConstants.RED)
                    .SetFontSize(10);
                leftCell.Add(volumeField);
                
                // Right side of certificate
                Cell rightCell = new Cell();
                rightCell.SetBorder(new SolidBorder(ColorConstants.RED, 1));
                rightCell.SetPadding(10);
                
                // Title
                Paragraph rightTitle = new Paragraph("CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM\nĐộc lập - Tự do - Hạnh phúc")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontColor(ColorConstants.RED)
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                    .SetFontSize(12);
                rightCell.Add(rightTitle);
                
                // Certificate title
                Paragraph certTitle = new Paragraph("GIẤY CHỨNG NHẬN\nHIẾN MÁU TÌNH NGUYỆN")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontColor(ColorConstants.RED)
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                    .SetFontSize(16);
                rightCell.Add(certTitle);
                
                // Blood donation center
                Paragraph center = new Paragraph($"BCD vận động hiến máu tình nguyện Tỉnh/TP......HCM........")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(10);
                rightCell.Add(center);
                
                // Certification
                Paragraph certifyText = new Paragraph("Chứng nhận:")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontColor(ColorConstants.RED)
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                    .SetFontSize(12);
                rightCell.Add(certifyText);
                
                // Donor information
                Paragraph donorInfo = new Paragraph($"Ông/Bà: {donor.FullName}")
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetFontSize(12);
                rightCell.Add(donorInfo);
                
                // Format birth date
                string birthDateStr = "";
                if (donor.DateOfBirth.HasValue)
                {
                    var birthDay = donor.DateOfBirth.Value;
                    birthDateStr = $"{birthDay.Day}     /     {birthDay.Month}     /     {birthDay.Year}";
                }
                
                Paragraph birthDate = new Paragraph($"Sinh ngày: {birthDateStr}")
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetFontSize(12);
                rightCell.Add(birthDate);
                
                // National ID
                Paragraph nationalId = new Paragraph($"Số CCCD: {donor.NationalId ?? ""}")
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetFontSize(12);
                rightCell.Add(nationalId);
                
                // Address
                Paragraph address = new Paragraph($"Địa chỉ: {donor.Address ?? ""}")
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetFontSize(12);
                rightCell.Add(address);
                
                // Donation confirmation
                Paragraph donated = new Paragraph("Đã hiến máu tình nguyện")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontColor(ColorConstants.RED)
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                    .SetFontSize(14);
                rightCell.Add(donated);
                
                // Facility where donation occurred
                Paragraph facility = new Paragraph("Tại cơ sở tiếp nhận máu: TTHMND")
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetFontSize(12);
                rightCell.Add(facility);
                
                // Volume options
                string vol250 = "☐";
                string vol350 = "☐";
                string vol450 = "☐";
                
                // Check the appropriate volume box based on donation record
                if (record.VolumeDonated.HasValue)
                {
                    decimal donatedVolume = record.VolumeDonated.Value;
                    if (donatedVolume <= 250) vol250 = "☒";
                    else if (donatedVolume <= 350) vol350 = "☒";
                    else vol450 = "☒";
                }
                
                Paragraph volume = new Paragraph($"Số lượng: 250ml{vol250} 350ml{vol350} 450ml{vol450}")
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetFontColor(ColorConstants.RED)
                    .SetFontSize(12);
                rightCell.Add(volume);
                
                // Gratitude message
                Paragraph gratitude = new Paragraph("Người bệnh luôn ghi ơn tấm lòng nhân ái của Ông/Bà.")
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetFontColor(ColorConstants.RED)
                    .SetFontSize(12);
                rightCell.Add(gratitude);
                
                // Blood group + Date
                var currentDate = DateTime.Now;
                Paragraph bloodGroup = new Paragraph($"Nhóm máu: {donor.BloodType?.BloodTypeName}      HCM, ngày {currentDate.Day} tháng {currentDate.Month} năm {currentDate.Year}")
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetFontSize(10);
                rightCell.Add(bloodGroup);
                
                // Signature
                Paragraph signature = new Paragraph("TM BAN CHỈ ĐẠO")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                    .SetFontSize(12);
                rightCell.Add(signature);
                
                // Add space for signature
                rightCell.Add(new Paragraph("\n\n"));
                
                // Certificate number at bottom
                Paragraph certificateNumber = new Paragraph($"Số: {record.CertificateId ?? "B 15786935"}")
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetFont(PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD))
                    .SetFontSize(12);
                rightCell.Add(certificateNumber);
                
                // Add the cells to the main table
                mainTable.AddCell(leftCell);
                mainTable.AddCell(rightCell);
                
                document.Add(mainTable);
                document.Close();
                
                return ms.ToArray();
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