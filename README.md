# HMS Mini - Hotel Management System API

A comprehensive .NET 8 Web API for hotel management with room inventory, guest check-in/check-out, and OCR-powered ID proof processing.

## Features

### Core Functionality
- **Room Type Management**: Define and manage different room categories (Single, Double, Suite, Deluxe)
- **Room Inventory**: Track room status and availability in real-time
- **Guest Check-in/Check-out**: Support for 1-3 guests per room with full transaction support
- **Multi-Guest Support**: Normalized database design for flexible guest management
- **Room Availability Tracking**: Intelligent date-range based availability queries
- **Automated Status Management**: Rooms automatically transition between Available → Occupied → Dirty → Available

### OCR Features
- **ID Proof Upload**: Upload guest ID proof images (Aadhaar, PAN, Driving License)
- **Smart Text Extraction**: Tesseract-based OCR with image preprocessing
- **Intelligent Parsing**: Auto-detect ID type and extract structured information
- **Auto-Fill Guest Data**: Extracted data includes name, address, city, state, mobile number

### Technical Features
- RESTful API with Swagger documentation
- Global exception handling with detailed error responses
- FluentValidation for input validation
- Structured logging with Serilog
- Entity Framework Core with SQL Server LocalDB
- Transaction support for data integrity

## Technology Stack

- **.NET 8** - Latest LTS version
- **ASP.NET Core Web API** - RESTful API framework
- **Entity Framework Core 8** - ORM with Code-First approach
- **SQL Server LocalDB** - Development database
- **Tesseract OCR** - Text extraction from images
- **SixLabors.ImageSharp** - Image preprocessing
- **FluentValidation** - Declarative validation
- **Serilog** - Structured logging
- **Swagger/OpenAPI** - Interactive API documentation

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server LocalDB (included with Visual Studio or SQL Server Express)
- A code editor (Visual Studio 2022, VS Code, or Rider)
- Git (for cloning the repository)

## Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd HMSMini
```

### 2. Restore Dependencies

```bash
dotnet restore
```

### 3. Database Setup

The application uses SQL Server LocalDB and will automatically create and seed the database on first run.

**Connection String** (in `src/HMSMini.API/appsettings.json`):
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=HMSMiniDB;Trusted_Connection=true;MultipleActiveResultSets=true"
}
```

**To manually create the database:**
```bash
cd src/HMSMini.API
dotnet ef database update
```

**Seed Data Includes:**
- 4 Room Types (Single, Double, Suite, Deluxe)
- 14 Sample Rooms with various statuses
- Sample check-in with test guest data

### 4. Run the Application

```bash
cd src/HMSMini.API
dotnet run
```

The API will start at: `http://localhost:5096`

Swagger UI will be available at: `http://localhost:5096` (root URL)

### 5. Build the Project

```bash
dotnet build
```

## Project Structure

```
HMSMini/
├── src/
│   └── HMSMini.API/
│       ├── Controllers/           # API endpoints
│       │   ├── RoomTypesController.cs
│       │   ├── RoomsController.cs
│       │   ├── CheckInsController.cs
│       │   └── GuestsController.cs
│       ├── Models/
│       │   ├── Entities/          # Database entities
│       │   ├── DTOs/              # Data transfer objects
│       │   └── Enums/             # Status enumerations
│       ├── Data/
│       │   ├── ApplicationDbContext.cs
│       │   ├── Configurations/    # EF Core configurations
│       │   └── DbInitializer.cs   # Seed data
│       ├── Services/
│       │   ├── Interfaces/        # Service contracts
│       │   └── Implementations/   # Business logic
│       ├── Validators/            # FluentValidation rules
│       ├── Middleware/            # Exception handling
│       ├── Exceptions/            # Custom exceptions
│       └── wwwroot/
│           ├── uploads/idproofs/  # Uploaded ID images
│           └── tessdata/          # Tesseract language data
├── tests/
│   └── HMSMini.Tests/
└── README.md
```

## API Endpoints

### Room Types

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/roomtypes` | Get all room types |
| GET | `/api/roomtypes/{id}` | Get room type by ID |
| POST | `/api/roomtypes` | Create new room type |
| PUT | `/api/roomtypes/{id}` | Update room type |
| DELETE | `/api/roomtypes/{id}` | Delete room type |

### Rooms

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/rooms` | Get all rooms |
| GET | `/api/rooms/{id}` | Get room by ID |
| GET | `/api/rooms/available?checkIn={date}&checkOut={date}` | Get available rooms for date range |
| POST | `/api/rooms` | Create new room |
| PUT | `/api/rooms/{id}/status` | Update room status |
| DELETE | `/api/rooms/{id}` | Delete room |

### Check-ins

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/checkins` | Get all check-ins |
| GET | `/api/checkins/{id}` | Get check-in by ID (with guests) |
| GET | `/api/checkins/active` | Get active check-ins only |
| POST | `/api/checkins` | Create new check-in with guests |
| POST | `/api/checkins/{id}/checkout` | Check out a guest |
| DELETE | `/api/checkins/{id}` | Delete check-in |

### Guests

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/guests/{id}` | Get guest by ID |
| GET | `/api/guests/checkin/{checkInId}` | Get all guests for a check-in |
| PUT | `/api/guests/{id}` | Update guest information |
| DELETE | `/api/guests/{id}` | Delete guest |

### OCR & Image Upload

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/guests/{id}/upload-photo` | Upload ID proof image |
| POST | `/api/guests/{id}/process-ocr?photoNumber={1\|2}` | Process OCR on uploaded image |
| GET | `/api/guests/{id}/photos/{photoNumber}` | Retrieve uploaded photo |

## Usage Examples

### 1. Get Available Rooms

```bash
curl "http://localhost:5096/api/rooms/available?checkIn=2025-12-28&checkOut=2025-12-30"
```

**Response:**
```json
[
  {
    "roomId": 1,
    "roomNumber": "101",
    "roomTypeId": 1,
    "roomTypeName": "Single",
    "roomStatus": 0,
    "roomStatusFromDate": null,
    "roomStatusToDate": null
  }
]
```

### 2. Create Check-in with Multiple Guests

```bash
curl -X POST "http://localhost:5096/api/checkins" \
  -H "Content-Type: application/json" \
  -d '{
    "roomNumber": "201",
    "checkInDate": "2025-12-28T14:00:00",
    "checkOutDate": "2025-12-30T11:00:00",
    "guests": [
      {
        "guestName": "John Doe",
        "address": "123 Main Street",
        "city": "Mumbai",
        "state": "Maharashtra",
        "country": "India",
        "mobileNo": "9876543210"
      },
      {
        "guestName": "Jane Doe",
        "address": "123 Main Street",
        "city": "Mumbai",
        "state": "Maharashtra",
        "country": "India",
        "mobileNo": "9876543211"
      }
    ]
  }'
```

### 3. Upload ID Proof Image

**Using curl:**
```bash
curl -X POST "http://localhost:5096/api/guests/1/upload-photo" \
  -F "photoNumber=1" \
  -F "file=@/path/to/aadhaar-card.jpg"
```

**Using PowerShell:**
```powershell
$uri = "http://localhost:5096/api/guests/1/upload-photo"
$filePath = "C:\path\to\aadhaar-card.jpg"
$form = @{
    photoNumber = "1"
    file = Get-Item -Path $filePath
}
Invoke-RestMethod -Uri $uri -Method Post -Form $form
```

**Response:**
```json
{
  "id": 1,
  "checkInId": 1,
  "guestNumber": 1,
  "guestName": "John Doe",
  "photo1Path": "wwwroot/uploads/idproofs/1/1_guest1_photo1_20251227104530.jpg"
}
```

### 4. Process OCR on Uploaded Image

```bash
curl -X POST "http://localhost:5096/api/guests/1/process-ocr?photoNumber=1"
```

**Response:**
```json
{
  "guestName": "RAJESH KUMAR",
  "address": "S/O RAM KUMAR, HOUSE NO 123, SECTOR 15",
  "city": "MUMBAI",
  "state": "Maharashtra",
  "country": "India",
  "mobileNo": "9876543210",
  "idType": "Aadhaar",
  "idNumber": "123456789012"
}
```

### 5. Check Out a Guest

```bash
curl -X POST "http://localhost:5096/api/checkins/1/checkout"
```

This will:
- Set check-in status to `CheckedOut`
- Record actual checkout timestamp
- Update room status to `Dirty` (needs cleaning)

## Room Status Workflow

```
Available (0) → [Check-in] → Occupied (1) → [Check-out] → Dirty (4) → [Housekeeping] → Available (0)
                                ↓
                          Maintenance (2) / Blocked (3) / Management (5)
```

**Status Codes:**
- `0` - Available
- `1` - Occupied
- `2` - Maintenance
- `3` - Blocked
- `4` - Dirty (needs cleaning)
- `5` - Management (reserved)

## OCR Supported ID Types

### 1. Aadhaar Card
**Extracts:**
- Name
- Address (with S/O, D/O, C/O)
- City (inferred from PIN code)
- State (pattern matching)
- Aadhaar number (12 digits)

### 2. PAN Card
**Extracts:**
- Name
- Father's name
- PAN number (10 characters)

### 3. Driving License
**Extracts:**
- Name
- Address
- City (inferred from PIN code)
- DL number

### OCR Workflow

```
1. Upload Image → ImageStorageService
2. Validate (size, type) → Save to disk
3. Update Guest.PhotoPath → Database
4. Process OCR → OcrService
5. Preprocess (grayscale, contrast) → Tesseract
6. Extract Text → Parse with Regex
7. Return GuestInfoDto → Frontend
8. User verifies/edits → Update Guest
```

## Configuration

### File Storage Settings (`appsettings.json`)

```json
"FileStorage": {
  "IdProofPath": "wwwroot/uploads/idproofs",
  "MaxFileSizeInMB": 10,
  "AllowedExtensions": [".jpg", ".jpeg", ".png"]
}
```

### OCR Settings

```json
"Ocr": {
  "TesseractDataPath": "wwwroot/tessdata",
  "Language": "eng"
}
```

### Logging Settings

```json
"Serilog": {
  "MinimumLevel": "Information",
  "WriteTo": [
    { "Name": "Console" },
    {
      "Name": "File",
      "Args": {
        "path": "Logs/log-.txt",
        "rollingInterval": "Day"
      }
    }
  ]
}
```

## Error Handling

The API uses global exception handling middleware that returns consistent error responses:

**404 Not Found:**
```json
{
  "message": "Guest with ID 999 was not found",
  "details": null
}
```

**400 Bad Request (Validation):**
```json
{
  "message": "Validation failed",
  "details": {
    "Guests": ["Number of guests must be between 1 and 3."],
    "CheckOutDate": ["Check-out date must be after check-in date."]
  }
}
```

**400 Bad Request (Business Rule):**
```json
{
  "message": "Room 201 is not available for the selected dates.",
  "details": null
}
```

## Database Schema

### Key Tables

**MRoomTypes**
- RoomTypeId (PK)
- RoomType (unique)
- RoomDescription

**RoomNo**
- RoomId (PK)
- RoomNumber (unique)
- RoomTypeId (FK)
- RoomStatus
- RoomStatusFromDate, RoomStatusToDate

**CheckIn**
- Id (PK)
- RoomId (FK)
- CheckInDate, CheckOutDate
- ActualCheckOutDate
- Pax (guest count)
- Status
- CreatedAt, UpdatedAt

**Guest** (Normalized - 1 to 3 per check-in)
- Id (PK)
- CheckInId (FK)
- GuestNumber (1-3)
- GuestName, Address, City, State, Country
- MobileNo
- Photo1Path, Photo2Path

## Testing

### Manual Testing with Swagger

1. Navigate to `http://localhost:5096`
2. Explore available endpoints
3. Use "Try it out" to test APIs interactively

### Testing Checklist

- [ ] Create room types
- [ ] Create rooms
- [ ] Query available rooms for date range
- [ ] Create check-in with 1 guest
- [ ] Create check-in with 3 guests
- [ ] Upload ID proof image
- [ ] Process OCR and verify extraction
- [ ] Check out guest
- [ ] Verify room status changes to Dirty
- [ ] Test validation errors (invalid dates, too many guests)
- [ ] Test duplicate room number prevention

### Unit Testing (TODO)

```bash
cd tests/HMSMini.Tests
dotnet test
```

## Troubleshooting

### Database Issues

**Problem:** Database doesn't exist
**Solution:**
```bash
cd src/HMSMini.API
dotnet ef database update
```

**Problem:** Migration errors
**Solution:**
```bash
dotnet ef database drop
dotnet ef database update
```

### OCR Issues

**Problem:** OCR returns empty text
**Solution:**
- Ensure image is clear and well-lit
- Check Tesseract data files exist in `wwwroot/tessdata/`
- Verify image format is .jpg or .png

**Problem:** Poor OCR accuracy
**Solution:**
- Use high-resolution scans (minimum 300 DPI)
- Ensure text is horizontal and not skewed
- Avoid shadows or glare on the ID card

### Port Conflicts

**Problem:** Port 5096 already in use
**Solution:** Change port in `launchSettings.json`:
```json
"applicationUrl": "http://localhost:5097"
```

## Security Considerations

**Current Implementation (Development):**
- No authentication/authorization
- CORS allows all origins
- File uploads limited to 10MB
- Only image files (.jpg, .jpeg, .png) allowed

**Production Recommendations:**
- Implement JWT authentication
- Add role-based authorization
- Configure CORS for specific origins
- Add rate limiting
- Scan uploaded files for malware
- Use HTTPS only
- Store connection strings in Azure Key Vault
- Enable request validation and sanitization

## Future Enhancements

- [ ] Authentication & Authorization (JWT)
- [ ] Pagination for list endpoints
- [ ] Advanced search and filtering
- [ ] Email notifications for bookings
- [ ] Payment integration
- [ ] Reporting and analytics
- [ ] Reservation system (future bookings)
- [ ] Guest history and preferences
- [ ] Housekeeping management
- [ ] Azure deployment (SQL Database, Blob Storage, App Service)
- [ ] Mobile app integration

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is created for educational and demonstration purposes.

## Support

For issues, questions, or contributions, please open an issue on the GitHub repository.

---

**Built with [Claude Code](https://claude.com/claude-code)**

**Co-Authored-By: Claude Sonnet 4.5**
