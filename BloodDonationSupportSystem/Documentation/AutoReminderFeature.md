# T�nh n?ng Auto Reminder cho L?ch hi?n m�u

## ?? T?ng quan

T�nh n?ng m?i n�y t? ??ng g?i th�ng b�o nh?c nh? cho nh?ng ng??i d�ng c� l?ch hi?n m�u v�o **ng�y mai**. H? th?ng s?:

1. **T? ??ng qu�t** danh s�ch ??ng k� hi?n m�u c� ng�y hi?n v�o ng�y mai
2. **G?i th�ng b�o** qua email v� notification trong h? th?ng
3. **Ch?y t? ??ng** m?i ng�y l�c 8:00 PM (20:00)

## ?? Endpoints m?i

### 1. L?y danh s�ch l?ch hi?n ng�y mai
```http
GET /api/donationreminder/tomorrow-schedules
```

**Response:**
```json
{
  "message": "Danh s�ch ng??i c� l?ch hi?n m�u v�o ng�y mai",
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
      "timeSlotName": "S�ng",
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

### 2. Trigger g?i th�ng b�o th? c�ng
```http
POST /api/donationreminder/send-tomorrow-reminders
```

**Response:**
```json
{
  "message": "?� ho�n th�nh g?i th�ng b�o nh?c nh? cho l?ch hi?n ng�y mai",
  "result": {
    "totalUpcomingDonations": 5,
    "successfulNotifications": 4,
    "failedNotifications": 1,
    "errorMessages": [
      "Donor ID 789: Thi?u th�ng tin email ho?c t�n"
    ],
    "processedAt": "2024-01-14T15:30:00Z",
    "processedBy": "System-AutoReminderJob",
    "executionTime": "00:00:03.2500000"
  }
}
```

## ? Scheduled Job

### TomorrowDonationReminderJob
- **Th?i gian ch?y**: M?i ng�y l�c 8:00 PM (20:00)
- **Cron Expression**: `0 0 20 * * ?`
- **M?c ?�ch**: T? ??ng g?i th�ng b�o cho ng??i c� l?ch hi?n v�o ng�y mai

### C?u h�nh trong Program.cs:
```csharp
var tomorrowReminderJobKey = new JobKey("TomorrowDonationReminderJob");
q.AddJob<TomorrowDonationReminderJob>(opts => opts.WithIdentity(tomorrowReminderJobKey));

q.AddTrigger(opts => opts
    .ForJob(tomorrowReminderJobKey)
    .WithIdentity("TomorrowDonationReminderJob-trigger")
    .WithCronSchedule("0 0 20 * * ?") // 8:00 PM m?i ng�y
    .WithDescription("Trigger to send donation reminders for tomorrow's schedules at 8 PM daily")
);
```

## ?? Email Template

Khi g?i th�ng b�o, h? th?ng s? g?i email v?i template ??p bao g?m:

- **Header**: Logo v� ti�u ?? h? th?ng
- **Th�ng tin l?ch hi?n**: Ng�y, gi?, ??a ?i?m
- **L?u � quan tr?ng**: Checklist chu?n b? tr??c khi hi?n m�u
- **Li�n h? h? tr?**: Email v� th�ng tin li�n l?c

## ?? C�c th�nh ph?n ?� th�m

### 1. DTOs m?i:
- `TomorrowDonationScheduleDTO`: Th�ng tin l?ch hi?n ng�y mai
- `AutoReminderJobResponseDTO`: K?t qu? th?c thi job

### 2. Repository methods:
- `GetUpcomingApprovedRegistrationsAsync()`: L?y danh s�ch ??ng k� ???c approve trong X ng�y t?i

### 3. Service methods:
- `GetTomorrowDonationSchedulesAsync()`: L?y danh s�ch l?ch hi?n ng�y mai
- `SendTomorrowDonationRemindersAsync()`: G?i th�ng b�o t? ??ng

### 4. Controller endpoints:
- `GET /api/donationreminder/tomorrow-schedules`
- `POST /api/donationreminder/send-tomorrow-reminders`

### 5. Quartz Job:
- `TomorrowDonationReminderJob`: Job t? ??ng ch?y h�ng ng�y

## ?? Logging v� Monitoring

Job s? log c�c th�ng tin sau:
- S? l??ng l?ch hi?n ???c t�m th?y
- S? th�ng b�o g?i th�nh c�ng/th?t b?i
- Th?i gian th?c thi
- Chi ti?t l?i (n?u c�)

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

## ?? L?i �ch

1. **T? ??ng h�a**: Kh�ng c?n can thi?p th? c�ng
2. **T?ng t? l? tham gia**: Nh?c nh? k?p th?i gi�p ng??i hi?n kh�ng qu�n l?ch
3. **Tr?i nghi?m t?t**: Email ??p, th�ng tin ??y ??
4. **Gi�m s�t ???c**: Log chi ti?t ?? theo d�i hi?u qu?
5. **Linh ho?t**: C� th? trigger th? c�ng khi c?n

## ??? C?u h�nh

### Database Requirements:
- B?ng `DonationRegistration` v?i status `Approved` (ID = 1)
- B?ng `DonationSchedule` v?i ng�y hi?n
- B?ng `User` v?i th�ng tin email

### Dependencies:
- `IDonationRegistrationRepository`
- `IUserNotificationService`
- `IEmailService`
- `Quartz.NET`

### Configuration:
- Job ch?y l�c 8:00 PM c� th? thay ??i b?ng c�ch s?a Cron expression
- Email template c� th? customize trong `GenerateTomorrowDonationReminderEmailTemplate()`