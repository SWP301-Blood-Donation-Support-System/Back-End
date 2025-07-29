# Tính n?ng Auto Reminder cho L?ch hi?n máu

## ?? T?ng quan

Tính n?ng m?i này t? ??ng g?i thông báo nh?c nh? cho nh?ng ng??i dùng có l?ch hi?n máu vào **ngày mai**. H? th?ng s?:

1. **T? ??ng quét** danh sách ??ng ký hi?n máu có ngày hi?n vào ngày mai
2. **G?i thông báo** qua email và notification trong h? th?ng
3. **Ch?y t? ??ng** m?i ngày lúc 8:00 PM (20:00)

## ?? Endpoints m?i

### 1. L?y danh sách l?ch hi?n ngày mai
```http
GET /api/donationreminder/tomorrow-schedules
```

**Response:**
```json
{
  "message": "Danh sách ng??i có l?ch hi?n máu vào ngày mai",
  "totalCount": 5,
  "data": [
    {
      "registrationId": 123,
      "donorId": 456,
      "donorName": "Nguy?n V?n A",
      "donorEmail": "nguyenvana@email.com",
      "donorPhone": "0123456789",
      "bloodTypeName": "A+",
      "scheduleDate": "2024-01-15T00:00:00",
      "timeSlotName": "Sáng",
      "startTime": "08:00",
      "endTime": "11:00",
      "location": "",
      "registrationStatusId": 1,
      "statusName": "Approved"
    }
  ],
  "retrievedAt": "2024-01-14T15:30:00Z"
}
```

### 2. Trigger g?i thông báo th? công
```http
POST /api/donationreminder/send-tomorrow-reminders
```

**Response:**
```json
{
  "message": "?ã hoàn thành g?i thông báo nh?c nh? cho l?ch hi?n ngày mai",
  "result": {
    "totalUpcomingDonations": 5,
    "successfulNotifications": 4,
    "failedNotifications": 1,
    "errorMessages": [
      "Donor ID 789: Thi?u thông tin email ho?c tên"
    ],
    "processedAt": "2024-01-14T15:30:00Z",
    "processedBy": "System-AutoReminderJob",
    "executionTime": "00:00:03.2500000"
  }
}
```

## ? Scheduled Job

### TomorrowDonationReminderJob
- **Th?i gian ch?y**: M?i ngày lúc 8:00 PM (20:00)
- **Cron Expression**: `0 0 20 * * ?`
- **M?c ?ích**: T? ??ng g?i thông báo cho ng??i có l?ch hi?n vào ngày mai

### C?u hình trong Program.cs:
```csharp
var tomorrowReminderJobKey = new JobKey("TomorrowDonationReminderJob");
q.AddJob<TomorrowDonationReminderJob>(opts => opts.WithIdentity(tomorrowReminderJobKey));

q.AddTrigger(opts => opts
    .ForJob(tomorrowReminderJobKey)
    .WithIdentity("TomorrowDonationReminderJob-trigger")
    .WithCronSchedule("0 0 20 * * ?") // 8:00 PM m?i ngày
    .WithDescription("Trigger to send donation reminders for tomorrow's schedules at 8 PM daily")
);
```

## ?? Email Template

Khi g?i thông báo, h? th?ng s? g?i email v?i template ??p bao g?m:

- **Header**: Logo và tiêu ?? h? th?ng
- **Thông tin l?ch hi?n**: Ngày, gi?, ??a ?i?m
- **L?u ý quan tr?ng**: Checklist chu?n b? tr??c khi hi?n máu
- **Liên h? h? tr?**: Email và thông tin liên l?c

## ?? Các thành ph?n ?ã thêm

### 1. DTOs m?i:
- `TomorrowDonationScheduleDTO`: Thông tin l?ch hi?n ngày mai
- `AutoReminderJobResponseDTO`: K?t qu? th?c thi job

### 2. Repository methods:
- `GetUpcomingApprovedRegistrationsAsync()`: L?y danh sách ??ng ký ???c approve trong X ngày t?i

### 3. Service methods:
- `GetTomorrowDonationSchedulesAsync()`: L?y danh sách l?ch hi?n ngày mai
- `SendTomorrowDonationRemindersAsync()`: G?i thông báo t? ??ng

### 4. Controller endpoints:
- `GET /api/donationreminder/tomorrow-schedules`
- `POST /api/donationreminder/send-tomorrow-reminders`

### 5. Quartz Job:
- `TomorrowDonationReminderJob`: Job t? ??ng ch?y hàng ngày

## ?? Logging và Monitoring

Job s? log các thông tin sau:
- S? l??ng l?ch hi?n ???c tìm th?y
- S? thông báo g?i thành công/th?t b?i
- Th?i gian th?c thi
- Chi ti?t l?i (n?u có)

Example log:
```
=== Starting Tomorrow Donation Reminder Job ===
Tomorrow Donation Reminder Job completed successfully:
- Total upcoming donations: 5
- Successful notifications: 4
- Failed notifications: 1
- Execution time: 3.25 seconds
=== Tomorrow Donation Reminder Job Finished ===
```

## ?? L?i ích

1. **T? ??ng hóa**: Không c?n can thi?p th? công
2. **T?ng t? l? tham gia**: Nh?c nh? k?p th?i giúp ng??i hi?n không quên l?ch
3. **Tr?i nghi?m t?t**: Email ??p, thông tin ??y ??
4. **Giám sát ???c**: Log chi ti?t ?? theo dõi hi?u qu?
5. **Linh ho?t**: Có th? trigger th? công khi c?n

## ??? C?u hình

### Database Requirements:
- B?ng `DonationRegistration` v?i status `Approved` (ID = 1)
- B?ng `DonationSchedule` v?i ngày hi?n
- B?ng `User` v?i thông tin email

### Dependencies:
- `IDonationRegistrationRepository`
- `IUserNotificationService`
- `IEmailService`
- `Quartz.NET`

### Configuration:
- Job ch?y lúc 8:00 PM có th? thay ??i b?ng cách s?a Cron expression
- Email template có th? customize trong `GenerateTomorrowDonationReminderEmailTemplate()`