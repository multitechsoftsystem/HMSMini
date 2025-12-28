# HMS Mini API - Testing Guide

## Quick Start

**API is running at:** `http://localhost:5096`
**Swagger UI:** `http://localhost:5096/swagger`

---

## Testing Methods

### 1. Swagger UI (Easiest)
1. Open browser: `http://localhost:5096/swagger`
2. Register a user via `POST /api/auth/register`
3. Copy the token from response
4. Click "Authorize" button, enter: `Bearer YOUR_TOKEN`
5. Test any endpoint!

---

### 2. cURL Commands

#### Step 1: Register & Get Token
```bash
curl -X POST "http://localhost:5096/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "testuser",
    "email": "test@example.com",
    "password": "Test123456",
    "fullName": "Test User",
    "role": 2
  }'
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "userId": 1,
  "username": "testuser",
  "email": "test@example.com",
  "fullName": "Test User",
  "role": "Receptionist",
  "expiresAt": "2025-12-29T00:00:00Z"
}
```

#### Step 2: Save Token (Linux/Mac/Git Bash)
```bash
export TOKEN="YOUR_TOKEN_HERE"
```

**Windows CMD:**
```cmd
set TOKEN=YOUR_TOKEN_HERE
```

**Windows PowerShell:**
```powershell
$TOKEN = "YOUR_TOKEN_HERE"
```

#### Step 3: Test Endpoints

**Get All Rooms:**
```bash
# Linux/Mac/Git Bash
curl -H "Authorization: Bearer $TOKEN" http://localhost:5096/api/rooms

# Windows CMD
curl -H "Authorization: Bearer %TOKEN%" http://localhost:5096/api/rooms

# PowerShell
curl -H "Authorization: Bearer $TOKEN" http://localhost:5096/api/rooms
```

**Create a Reservation:**
```bash
curl -X POST "http://localhost:5096/api/reservations" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "roomNumber": "101",
    "checkInDate": "2025-01-15",
    "checkOutDate": "2025-01-17",
    "numberOfGuests": 2,
    "guestName": "John Doe",
    "guestEmail": "john@example.com",
    "guestMobile": "1234567890",
    "specialRequests": "Early check-in"
  }'
```

**Get Upcoming Reservations:**
```bash
curl -H "Authorization: Bearer $TOKEN" \
  http://localhost:5096/api/reservations/upcoming
```

**Create Check-In:**
```bash
curl -X POST "http://localhost:5096/api/checkins" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "roomNumber": "102",
    "checkInDate": "2025-12-28",
    "checkOutDate": "2025-12-30",
    "guests": [
      {
        "guestName": "Jane Smith",
        "address": "123 Main St",
        "city": "Mumbai",
        "state": "Maharashtra",
        "country": "India",
        "mobileNo": "9876543210",
        "panOrAadharNo": "ABCDE1234F"
      }
    ]
  }'
```

**Get Active Check-Ins:**
```bash
curl -H "Authorization: Bearer $TOKEN" \
  http://localhost:5096/api/checkins/active
```

---

### 3. VS Code REST Client Extension

Install the "REST Client" extension in VS Code, then create a file `test.http`:

```http
### Variables
@baseUrl = http://localhost:5096
@token = {{login.response.body.token}}

### Register User
# @name register
POST {{baseUrl}}/api/auth/register
Content-Type: application/json

{
  "username": "testuser",
  "email": "test@example.com",
  "password": "Test123456",
  "fullName": "Test User",
  "role": 2
}

### Login
# @name login
POST {{baseUrl}}/api/auth/login
Content-Type: application/json

{
  "username": "testuser",
  "password": "Test123456"
}

### Get All Rooms
GET {{baseUrl}}/api/rooms
Authorization: Bearer {{token}}

### Create Reservation
POST {{baseUrl}}/api/reservations
Authorization: Bearer {{token}}
Content-Type: application/json

{
  "roomNumber": "101",
  "checkInDate": "2025-01-15",
  "checkOutDate": "2025-01-17",
  "numberOfGuests": 2,
  "guestName": "John Doe",
  "guestEmail": "john@example.com",
  "guestMobile": "1234567890",
  "specialRequests": "Early check-in"
}

### Get Upcoming Reservations
GET {{baseUrl}}/api/reservations/upcoming
Authorization: Bearer {{token}}

### Get Available Rooms
GET {{baseUrl}}/api/rooms/available?checkIn=2025-01-01&checkOut=2025-01-03
Authorization: Bearer {{token}}

### Create Check-In
POST {{baseUrl}}/api/checkins
Authorization: Bearer {{token}}
Content-Type: application/json

{
  "roomNumber": "102",
  "checkInDate": "2025-12-28",
  "checkOutDate": "2025-12-30",
  "guests": [
    {
      "guestName": "Jane Smith",
      "address": "123 Main St",
      "city": "Mumbai",
      "state": "Maharashtra",
      "country": "India",
      "mobileNo": "9876543210",
      "panOrAadharNo": "ABCDE1234F"
    }
  ]
}
```

Click "Send Request" above each request to execute!

---

### 4. Automated Testing (Already Done!)

Run the test suite:

```bash
dotnet test
```

**Current Status:** ✅ All 163 tests passing
- 135 existing tests (auth, rooms, check-ins, guests)
- 28 new reservation tests (unit + integration)

---

## Common Testing Workflows

### Workflow 1: Book a Future Reservation
1. **Register/Login** → Get token
2. **Get Available Rooms** → Find available room
3. **Create Reservation** → Book the room
4. **Confirm Reservation** → Change status to Confirmed
5. **Get Upcoming Reservations** → Verify it appears

### Workflow 2: Walk-in Guest Check-In
1. **Register/Login** → Get token
2. **Get Available Rooms** → Find available room
3. **Create Check-In** → Guest checks in directly (no reservation)
4. **Get Active Check-Ins** → Verify check-in is active

### Workflow 3: Reservation to Check-In
1. **Create Reservation** (advance booking)
2. **Confirm Reservation**
3. On arrival date: **Create Check-In** (using same room)
4. System should handle both existing

---

## Authentication & Authorization

### User Roles:
- **Admin (0):** Full access, can delete anything
- **Manager (1):** Create/update rooms, reports, all read operations
- **Receptionist (2):** Check-ins, check-outs, reservations, guest management

### Endpoints by Role:

**Public (No Auth Required):**
- None - all endpoints require authentication

**All Authenticated Users:**
- GET /api/rooms
- GET /api/checkins
- GET /api/reservations
- GET /api/guests

**Receptionist, Manager, Admin:**
- POST /api/reservations (create)
- PUT /api/reservations (update)
- POST /api/checkins
- PUT /api/guests

**Manager, Admin:**
- POST /api/rooms (create)
- PUT /api/rooms/status

**Admin Only:**
- DELETE /api/rooms
- DELETE /api/checkins
- DELETE /api/reservations
- DELETE /api/guests
- GET /api/auth (list users)

---

## Database Access

**SQL Server LocalDB** is being used. To view data:

### Using SQL Server Management Studio (SSMS):
1. Server: `(localdb)\mssqllocaldb`
2. Database: `HMSMiniDB`
3. Tables:
   - MRoomTypes
   - RoomNo
   - CheckIns
   - Guests
   - Reservations
   - Users

### Using Azure Data Studio:
Same connection string as above.

### Using Entity Framework CLI:
```bash
# View migrations
dotnet ef migrations list

# Generate SQL script
dotnet ef migrations script

# Update database
dotnet ef database update
```

---

## Troubleshooting

### Issue: 401 Unauthorized
**Solution:**
- Ensure you included `Authorization: Bearer YOUR_TOKEN`
- Token might be expired (24 hours), login again

### Issue: 400 Bad Request
**Solution:**
- Check request body format matches the DTO
- Review validation errors in response

### Issue: 404 Not Found
**Solution:**
- Verify the resource ID exists
- Check the endpoint URL is correct

### Issue: 403 Forbidden
**Solution:**
- Your user role doesn't have permission
- Register as Admin (role: 0) or Manager (role: 1) for more access

---

## Sample Test Data

The database is seeded with:
- 4 Room Types (Single, Double, Suite, Deluxe)
- 8 Rooms (101-104, 201-204)
- 1 Admin user (created on first registration)

---

## API Documentation

**Swagger/OpenAPI Spec:** http://localhost:5096/swagger/v1/swagger.json

You can import this into tools like:
- Postman (Import → Link → Paste swagger URL)
- Insomnia
- API testing tools

---

## Performance Testing

For load testing, use tools like:
- **Apache Bench:**
  ```bash
  ab -n 1000 -c 10 -H "Authorization: Bearer TOKEN" http://localhost:5096/api/rooms
  ```

- **k6.io:**
  ```javascript
  import http from 'k6/http';

  export default function() {
    http.get('http://localhost:5096/api/rooms', {
      headers: { 'Authorization': 'Bearer YOUR_TOKEN' }
    });
  }
  ```

---

## Next Steps

1. **Frontend Development:** Build a UI using React/Angular/Vue
2. **Deploy to Azure:** App Service + Azure SQL Database
3. **Add Reporting:** Occupancy rates, revenue reports
4. **Mobile App:** Flutter or React Native
5. **Email Notifications:** Confirmation emails for reservations
6. **Payment Integration:** Razorpay, Stripe for online payments

---

## Support

- **Repository:** https://github.com/multitechsoftsystem/HMSMini
- **Issues:** Report bugs via GitHub Issues
- **Documentation:** See README.md for architecture details
