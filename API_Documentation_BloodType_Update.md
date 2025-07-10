# API Documentation - Update Blood Type

## 1. Update Blood Type by User ID

**Endpoint:** `PATCH /api/user/{userId}/blood-type`

**Description:** C?p nh?t nh�m m�u cho m?t user d?a tr�n userId

**Parameters:**
- `userId` (path): ID c?a user c?n c?p nh?t (integer, required)

**Request Body:**
```json
{
  "bloodTypeId": 1
}
```

**Response:**
- **200 OK**: C?p nh?t th�nh c�ng
```json
{
  "status": "success",
  "message": "Blood type updated successfully"
}
```

- **400 Bad Request**: Invalid input
- **404 Not Found**: User kh�ng t?n t?i
- **500 Internal Server Error**: L?i server

## 2. Update Blood Type by Donor ID (Specific for Donation Records)

**Endpoint:** `PATCH /api/user/donor/blood-type`

**Description:** C?p nh?t nh�m m�u cho donor d?a tr�n donorId (ch? �p d?ng cho users c� RoleId = 3)

**Request Body:**
```json
{
  "donorId": 123,
  "bloodTypeId": 2
}
```

**Response:**
- **200 OK**: C?p nh?t th�nh c�ng
```json
{
  "status": "success",
  "message": "Donor blood type updated successfully",
  "donorId": 123,
  "newBloodTypeId": 2
}
```

**Error Responses:**
- **400 Bad Request**: Invalid input ho?c user kh�ng ph?i l� donor
- **404 Not Found**: Donor kh�ng t?n t?i
- **500 Internal Server Error**: L?i server

## Use Cases

### Use Case 1: Update t? User Profile
```javascript
// C?p nh?t blood type t? user profile
const response = await fetch(`/api/user/${userId}/blood-type`, {
  method: 'PATCH',
  headers: {
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    bloodTypeId: 3
  })
});
```

### Use Case 2: Update t? Donation Record Context
```javascript
// C?p nh?t blood type trong context hi?n m�u
const response = await fetch('/api/user/donor/blood-type', {
  method: 'PATCH',
  headers: {
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    donorId: 123,
    bloodTypeId: 2
  })
});
```

## Data Validation

- `bloodTypeId`: Ph?i l� s? nguy�n > 0
- `donorId`: Ph?i l� s? nguy�n > 0  
- User v?i `donorId` ph?i c� `RoleId = 3` (Donor role)

## Database Impact

- C?p nh?t field `BloodTypeId` trong b?ng `Users`
- T? ??ng set `UpdatedAt = DateTime.UtcNow`
- Kh�ng vi ph?m chu?n 3NF v� c?p nh?t tr?c ti?p tr�n User entity