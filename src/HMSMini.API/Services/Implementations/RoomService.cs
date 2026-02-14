using Microsoft.EntityFrameworkCore;
using HMSMini.API.Data;
using HMSMini.API.Exceptions;
using HMSMini.API.Models.DTOs.Room;
using HMSMini.API.Models.Entities;
using HMSMini.API.Models.Enums;
using HMSMini.API.Services.Interfaces;

namespace HMSMini.API.Services.Implementations;

public class RoomService : IRoomService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<RoomService> _logger;

    public RoomService(ApplicationDbContext context, ILogger<RoomService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<RoomDto>> GetAllAsync()
    {
        return await _context.Rooms
            .Include(r => r.RoomType)
            .Select(r => new RoomDto
            {
                RoomId = r.RoomId,
                RoomNumber = r.RoomNumber,
                RoomTypeId = r.RoomTypeId,
                RoomTypeName = r.RoomType.RoomType,
                RoomStatus = r.RoomStatus,
                RoomStatusFromDate = r.RoomStatusFromDate,
                RoomStatusToDate = r.RoomStatusToDate
            })
            .ToListAsync();
    }

    public async Task<RoomDto> GetByIdAsync(int id)
    {
        var room = await _context.Rooms
            .Include(r => r.RoomType)
            .FirstOrDefaultAsync(r => r.RoomId == id);

        if (room == null)
            throw new NotFoundException(nameof(RoomNo), id);

        return new RoomDto
        {
            RoomId = room.RoomId,
            RoomNumber = room.RoomNumber,
            RoomTypeId = room.RoomTypeId,
            RoomTypeName = room.RoomType.RoomType,
            RoomStatus = room.RoomStatus,
            RoomStatusFromDate = room.RoomStatusFromDate,
            RoomStatusToDate = room.RoomStatusToDate
        };
    }

    public async Task<List<RoomDto>> GetAvailableRoomsAsync(DateTime checkIn, DateTime checkOut)
    {
        // Get all rooms
        var allRooms = await _context.Rooms
            .Include(r => r.RoomType)
            .ToListAsync();

        // Get occupied rooms in date range
        var occupiedRoomIds = await _context.CheckIns
            .Where(c => c.Status == CheckInStatus.Active &&
                        c.CheckInDate < checkOut &&
                        c.CheckOutDate > checkIn)
            .Select(c => c.RoomId)
            .ToListAsync();

        // Get reserved rooms in date range (rooms assigned to reservations)
        var reservedRoomIds = await _context.Reservations
            .Where(r => r.RoomId.HasValue &&
                        (r.Status == ReservationStatus.Confirmed || r.Status == ReservationStatus.Pending) &&
                        r.CheckInDate < checkOut &&
                        r.CheckOutDate > checkIn)
            .Select(r => (int)r.RoomId!)
            .ToListAsync();

        // Get rooms with maintenance/blocked status in date range
        var unavailableRoomIds = allRooms
            .Where(r => r.RoomStatus != RoomStatus.Available &&
                        (!r.RoomStatusFromDate.HasValue || !r.RoomStatusToDate.HasValue ||
                         (r.RoomStatusFromDate < checkOut && r.RoomStatusToDate > checkIn)))
            .Select(r => r.RoomId)
            .ToList();

        // Filter available rooms
        var availableRooms = allRooms
            .Where(r => !occupiedRoomIds.Contains(r.RoomId) &&
                        !reservedRoomIds.Contains(r.RoomId) &&
                        !unavailableRoomIds.Contains(r.RoomId))
            .Select(r => new RoomDto
            {
                RoomId = r.RoomId,
                RoomNumber = r.RoomNumber,
                RoomTypeId = r.RoomTypeId,
                RoomTypeName = r.RoomType.RoomType,
                RoomStatus = r.RoomStatus,
                RoomStatusFromDate = r.RoomStatusFromDate,
                RoomStatusToDate = r.RoomStatusToDate
            })
            .ToList();

        return availableRooms;
    }

    public async Task<RoomDto> CreateAsync(CreateRoomDto dto)
    {
        // Check if room number already exists
        if (await _context.Rooms.AnyAsync(r => r.RoomNumber == dto.RoomNumber))
            throw new BusinessRuleException($"Room number '{dto.RoomNumber}' already exists.");

        // Verify room type exists
        if (!await _context.RoomTypes.AnyAsync(rt => rt.RoomTypeId == dto.RoomTypeId))
            throw new NotFoundException(nameof(MRoomType), dto.RoomTypeId);

        var room = new RoomNo
        {
            RoomNumber = dto.RoomNumber,
            RoomTypeId = dto.RoomTypeId,
            RoomStatus = dto.RoomStatus,
            RoomStatusFromDate = dto.RoomStatusFromDate,
            RoomStatusToDate = dto.RoomStatusToDate
        };

        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created room {RoomNumber} with ID {Id}", room.RoomNumber, room.RoomId);

        return await GetByIdAsync(room.RoomId);
    }

    public async Task<RoomDto> UpdateStatusAsync(int id, UpdateRoomStatusDto dto)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null)
            throw new NotFoundException(nameof(RoomNo), id);

        room.RoomStatus = dto.RoomStatus;
        room.RoomStatusFromDate = dto.RoomStatusFromDate;
        room.RoomStatusToDate = dto.RoomStatusToDate;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated room {RoomNumber} status to {Status}", room.RoomNumber, dto.RoomStatus);

        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        var room = await _context.Rooms.FindAsync(id);
        if (room == null)
            throw new NotFoundException(nameof(RoomNo), id);

        // Check if room has any check-ins
        if (await _context.CheckIns.AnyAsync(c => c.RoomId == id))
            throw new BusinessRuleException("Cannot delete room that has check-in records.");

        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted room {RoomNumber}", room.RoomNumber);
    }

    public async Task<int> GetRoomIdByNumberAsync(string roomNumber)
    {
        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.RoomNumber == roomNumber);
        if (room == null)
            throw new NotFoundException($"Room with number '{roomNumber}' not found.");

        return room.RoomId;
    }

    public async Task<List<RoomAvailabilityDto>> GetRoomAvailabilityAsync(DateTime startDate, DateTime endDate)
    {
        // Get all rooms with their types
        var rooms = await _context.Rooms
            .Include(r => r.RoomType)
            .OrderBy(r => r.RoomNumber)
            .ToListAsync();

        // Get all check-ins that overlap with the date range
        var checkIns = await _context.CheckIns
            .Include(c => c.Guests)
            .Where(c => c.Status == CheckInStatus.Active &&
                        c.CheckInDate < endDate.AddDays(1) &&
                        c.CheckOutDate > startDate)
            .Select(c => new
            {
                c.RoomId,
                c.CheckInDate,
                c.CheckOutDate,
                GuestName = c.Guests.OrderBy(g => g.Id).FirstOrDefault() != null
                    ? c.Guests.OrderBy(g => g.Id).FirstOrDefault()!.GuestName
                    : "Guest"
            })
            .ToListAsync();

        // Get all reservations that overlap with the date range (grouped by room type)
        var allReservations = await _context.Reservations
            .Include(r => r.RoomType)
            .Include(r => r.Room)
            .Where(r => (r.Status == ReservationStatus.Confirmed || r.Status == ReservationStatus.Pending) &&
                        r.CheckInDate < endDate.AddDays(1) &&
                        r.CheckOutDate > startDate)
            .ToListAsync();

        // Separate reservations: those with assigned rooms vs those without
        var reservationsWithRooms = allReservations
            .Where(r => r.RoomId.HasValue)
            .Select(r => new
            {
                r.RoomId,
                r.CheckInDate,
                r.CheckOutDate,
                r.GuestName,
                r.ReservationNumber
            })
            .ToList();

        var reservationsWithoutRooms = allReservations
            .Where(r => !r.RoomId.HasValue)
            .ToList();

        var result = new List<RoomAvailabilityDto>();

        // Track which rooms have been allocated to reservations for each date
        var roomAllocations = new Dictionary<int, List<(DateTime start, DateTime end, string guestName, string reservationNumber)>>();

        // Pre-allocate reservations without assigned rooms to available rooms of matching type
        foreach (var reservation in reservationsWithoutRooms)
        {
            // Find available rooms of this type for this date range
            var roomsOfType = rooms.Where(r => r.RoomTypeId == reservation.RoomTypeId).ToList();

            // Find a room that's not already allocated for this period
            foreach (var room in roomsOfType)
            {
                // Check if room is blocked/maintenance during this period
                bool isBlocked = false;
                if (room.RoomStatus != RoomStatus.Available)
                {
                    if (room.RoomStatusFromDate.HasValue && room.RoomStatusToDate.HasValue)
                    {
                        if (reservation.CheckInDate.Date < room.RoomStatusToDate.Value.Date &&
                            reservation.CheckOutDate.Date > room.RoomStatusFromDate.Value.Date)
                        {
                            isBlocked = true;
                        }
                    }
                    else
                    {
                        isBlocked = true;
                    }
                }

                if (isBlocked) continue;

                // Check if room has a check-in during this period
                var hasCheckIn = checkIns.Any(c =>
                    c.RoomId == room.RoomId &&
                    c.CheckInDate.Date < reservation.CheckOutDate.Date &&
                    c.CheckOutDate.Date > reservation.CheckInDate.Date);

                if (hasCheckIn) continue;

                // Check if already allocated
                if (!roomAllocations.ContainsKey(room.RoomId))
                {
                    roomAllocations[room.RoomId] = new List<(DateTime, DateTime, string, string)>();
                }

                var hasConflict = roomAllocations[room.RoomId].Any(a =>
                    a.start < reservation.CheckOutDate.Date &&
                    a.end > reservation.CheckInDate.Date);

                if (!hasConflict)
                {
                    // Allocate this reservation to this room
                    roomAllocations[room.RoomId].Add((
                        reservation.CheckInDate.Date,
                        reservation.CheckOutDate.Date,
                        reservation.GuestName,
                        reservation.ReservationNumber
                    ));
                    break; // Move to next reservation
                }
            }
        }

        foreach (var room in rooms)
        {
            var roomAvailability = new RoomAvailabilityDto
            {
                RoomId = room.RoomId,
                RoomNumber = room.RoomNumber,
                RoomType = room.RoomType.RoomType,
                DailyAvailability = new List<DailyAvailability>()
            };

            // Generate daily availability for each date in range
            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                var dailyStatus = new DailyAvailability
                {
                    Date = date,
                    IsAvailable = true,
                    Status = "Available"
                };

                // Check if room is blocked/under maintenance
                if (room.RoomStatus != RoomStatus.Available)
                {
                    if (room.RoomStatusFromDate.HasValue && room.RoomStatusToDate.HasValue)
                    {
                        if (date >= room.RoomStatusFromDate.Value.Date &&
                            date < room.RoomStatusToDate.Value.Date)
                        {
                            dailyStatus.IsAvailable = false;
                            dailyStatus.Status = room.RoomStatus.ToString();
                            roomAvailability.DailyAvailability.Add(dailyStatus);
                            continue;
                        }
                    }
                    else
                    {
                        dailyStatus.IsAvailable = false;
                        dailyStatus.Status = room.RoomStatus.ToString();
                        roomAvailability.DailyAvailability.Add(dailyStatus);
                        continue;
                    }
                }

                // Check if room is occupied by a check-in
                var checkIn = checkIns.FirstOrDefault(c =>
                    c.RoomId == room.RoomId &&
                    date >= c.CheckInDate.Date &&
                    date < c.CheckOutDate.Date);

                if (checkIn != null)
                {
                    dailyStatus.IsAvailable = false;
                    dailyStatus.Status = "Occupied";
                    dailyStatus.GuestName = checkIn.GuestName;
                    roomAvailability.DailyAvailability.Add(dailyStatus);
                    continue;
                }

                // Check if room has a reservation with assigned room
                var reservationWithRoom = reservationsWithRooms.FirstOrDefault(r =>
                    r.RoomId == room.RoomId &&
                    date >= r.CheckInDate.Date &&
                    date < r.CheckOutDate.Date);

                if (reservationWithRoom != null)
                {
                    dailyStatus.IsAvailable = false;
                    dailyStatus.Status = "Reserved";
                    dailyStatus.GuestName = reservationWithRoom.GuestName;
                    dailyStatus.ReservationNumber = reservationWithRoom.ReservationNumber;
                    roomAvailability.DailyAvailability.Add(dailyStatus);
                    continue;
                }

                // Check if room has been allocated a reservation (room type based)
                if (roomAllocations.ContainsKey(room.RoomId))
                {
                    var allocation = roomAllocations[room.RoomId].FirstOrDefault(a =>
                        date >= a.start &&
                        date < a.end);

                    if (allocation != default)
                    {
                        dailyStatus.IsAvailable = false;
                        dailyStatus.Status = "Reserved";
                        dailyStatus.GuestName = allocation.guestName;
                        dailyStatus.ReservationNumber = allocation.reservationNumber;
                        roomAvailability.DailyAvailability.Add(dailyStatus);
                        continue;
                    }
                }

                // Room is available
                roomAvailability.DailyAvailability.Add(dailyStatus);
            }

            result.Add(roomAvailability);
        }

        return result;
    }

    public async Task<OccupancyReportDto> GetOccupancyReportAsync(DateTime date)
    {
        // Normalize the date to start of day
        var reportDate = date.Date;

        // Get all rooms with their types
        var rooms = await _context.Rooms
            .Include(r => r.RoomType)
            .ToListAsync();

        var totalRooms = rooms.Count;

        // Get currently occupied rooms (Active check-ins)
        var occupiedCheckIns = await _context.CheckIns
            .Where(c => c.Status == CheckInStatus.Active &&
                        c.CheckInDate.Date <= reportDate &&
                        c.CheckOutDate.Date > reportDate)
            .Select(c => c.RoomId)
            .ToListAsync();

        // Get checked-out rooms on this date
        var checkedOutCheckIns = await _context.CheckIns
            .Where(c => c.Status == CheckInStatus.CheckedOut &&
                        c.ActualCheckOutDate.HasValue &&
                        c.ActualCheckOutDate.Value.Date == reportDate)
            .Select(c => c.RoomId)
            .ToListAsync();

        // Count rooms by status
        int occupiedRooms = occupiedCheckIns.Count;
        int checkedOutRooms = checkedOutCheckIns.Count;
        int dirtyRooms = rooms.Count(r => r.RoomStatus == RoomStatus.Dirty);
        int maintenanceRooms = rooms.Count(r => r.RoomStatus == RoomStatus.Maintenance &&
                                                  (!r.RoomStatusFromDate.HasValue || !r.RoomStatusToDate.HasValue ||
                                                   (r.RoomStatusFromDate.Value.Date <= reportDate && r.RoomStatusToDate.Value.Date > reportDate)));
        int blockedRooms = rooms.Count(r => r.RoomStatus == RoomStatus.Blocked &&
                                            (!r.RoomStatusFromDate.HasValue || !r.RoomStatusToDate.HasValue ||
                                             (r.RoomStatusFromDate.Value.Date <= reportDate && r.RoomStatusToDate.Value.Date > reportDate)));

        // Available rooms are those that are:
        // 1. Not occupied by active check-ins
        // 2. Not checked out on this date
        // 3. Not in maintenance/blocked status on this date
        int availableRooms = totalRooms - occupiedRooms - checkedOutRooms - maintenanceRooms - blockedRooms;

        // Occupancy includes both occupied rooms and rooms that checked out on this date
        decimal occupancyPercentage = totalRooms > 0 ? (decimal)(occupiedRooms + checkedOutRooms) / totalRooms * 100 : 0;

        // Get room type breakdown
        var roomTypes = await _context.RoomTypes.ToListAsync();
        var roomTypeBreakdown = new List<RoomTypeOccupancyDto>();

        foreach (var roomType in roomTypes)
        {
            var roomsOfType = rooms.Where(r => r.RoomTypeId == roomType.RoomTypeId).ToList();
            var totalOfType = roomsOfType.Count;

            if (totalOfType == 0) continue;

            var occupiedOfType = roomsOfType.Count(r => occupiedCheckIns.Contains(r.RoomId));
            var checkedOutOfType = roomsOfType.Count(r => checkedOutCheckIns.Contains(r.RoomId));
            var availableOfType = totalOfType - occupiedOfType - checkedOutOfType;
            // Occupancy percentage includes both occupied and checked-out rooms
            var typeOccupancyPercentage = totalOfType > 0 ? (decimal)(occupiedOfType + checkedOutOfType) / totalOfType * 100 : 0;

            roomTypeBreakdown.Add(new RoomTypeOccupancyDto
            {
                RoomTypeName = roomType.RoomType,
                TotalRooms = totalOfType,
                OccupiedRooms = occupiedOfType,
                CheckedOutRooms = checkedOutOfType,
                AvailableRooms = availableOfType,
                OccupancyPercentage = Math.Round(typeOccupancyPercentage, 2)
            });
        }

        return new OccupancyReportDto
        {
            ReportDate = reportDate,
            TotalRooms = totalRooms,
            OccupiedRooms = occupiedRooms,
            CheckedOutRooms = checkedOutRooms,
            AvailableRooms = availableRooms,
            DirtyRooms = dirtyRooms,
            MaintenanceRooms = maintenanceRooms,
            BlockedRooms = blockedRooms,
            OccupancyPercentage = Math.Round(occupancyPercentage, 2),
            RoomTypeBreakdown = roomTypeBreakdown
        };
    }

    public async Task<byte[]> ExportOccupancyReportToExcelAsync(DateTime date)
    {
        var report = await GetOccupancyReportAsync(date);

        using var workbook = new ClosedXML.Excel.XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Occupancy Report");

        // Title
        worksheet.Cell(1, 1).Value = "Hotel Management System";
        worksheet.Cell(1, 1).Style.Font.Bold = true;
        worksheet.Cell(1, 1).Style.Font.FontSize = 16;
        worksheet.Range(1, 1, 1, 6).Merge();

        worksheet.Cell(2, 1).Value = "Occupancy Report";
        worksheet.Cell(2, 1).Style.Font.Bold = true;
        worksheet.Cell(2, 1).Style.Font.FontSize = 14;
        worksheet.Range(2, 1, 2, 6).Merge();

        worksheet.Cell(3, 1).Value = $"Report Date: {report.ReportDate:MMMM dd, yyyy}";
        worksheet.Cell(3, 1).Style.Font.Bold = true;
        worksheet.Range(3, 1, 3, 6).Merge();

        // Summary Section
        var currentRow = 5;
        worksheet.Cell(currentRow, 1).Value = "Summary";
        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 12;
        worksheet.Cell(currentRow, 1).Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;
        worksheet.Range(currentRow, 1, currentRow, 2).Merge();
        currentRow++;

        worksheet.Cell(currentRow, 1).Value = "Total Rooms";
        worksheet.Cell(currentRow, 2).Value = report.TotalRooms;
        currentRow++;

        worksheet.Cell(currentRow, 1).Value = "Occupied Rooms";
        worksheet.Cell(currentRow, 2).Value = report.OccupiedRooms;
        currentRow++;

        worksheet.Cell(currentRow, 1).Value = "Checked Out Today";
        worksheet.Cell(currentRow, 2).Value = report.CheckedOutRooms;
        currentRow++;

        worksheet.Cell(currentRow, 1).Value = "Available Rooms";
        worksheet.Cell(currentRow, 2).Value = report.AvailableRooms;
        currentRow++;

        worksheet.Cell(currentRow, 1).Value = "Dirty Rooms";
        worksheet.Cell(currentRow, 2).Value = report.DirtyRooms;
        currentRow++;

        worksheet.Cell(currentRow, 1).Value = "Maintenance Rooms";
        worksheet.Cell(currentRow, 2).Value = report.MaintenanceRooms;
        currentRow++;

        worksheet.Cell(currentRow, 1).Value = "Blocked Rooms";
        worksheet.Cell(currentRow, 2).Value = report.BlockedRooms;
        currentRow++;

        worksheet.Cell(currentRow, 1).Value = "Occupancy Percentage";
        worksheet.Cell(currentRow, 2).Value = $"{report.OccupancyPercentage}%";
        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
        worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
        currentRow += 2;

        // Room Type Breakdown Section
        worksheet.Cell(currentRow, 1).Value = "Occupancy by Room Type";
        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 12;
        worksheet.Cell(currentRow, 1).Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;
        worksheet.Range(currentRow, 1, currentRow, 6).Merge();
        currentRow++;

        // Table Headers
        worksheet.Cell(currentRow, 1).Value = "Room Type";
        worksheet.Cell(currentRow, 2).Value = "Total Rooms";
        worksheet.Cell(currentRow, 3).Value = "Occupied";
        worksheet.Cell(currentRow, 4).Value = "Checked Out";
        worksheet.Cell(currentRow, 5).Value = "Available";
        worksheet.Cell(currentRow, 6).Value = "Occupancy Rate";

        // Style headers
        var headerRange = worksheet.Range(currentRow, 1, currentRow, 6);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightBlue;
        headerRange.Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
        headerRange.Style.Border.InsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
        currentRow++;

        // Data Rows
        foreach (var typeData in report.RoomTypeBreakdown)
        {
            worksheet.Cell(currentRow, 1).Value = typeData.RoomTypeName;
            worksheet.Cell(currentRow, 2).Value = typeData.TotalRooms;
            worksheet.Cell(currentRow, 3).Value = typeData.OccupiedRooms;
            worksheet.Cell(currentRow, 4).Value = typeData.CheckedOutRooms;
            worksheet.Cell(currentRow, 5).Value = typeData.AvailableRooms;
            worksheet.Cell(currentRow, 6).Value = $"{typeData.OccupancyPercentage}%";

            // Add borders
            var dataRange = worksheet.Range(currentRow, 1, currentRow, 6);
            dataRange.Style.Border.OutsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;
            dataRange.Style.Border.InsideBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;

            currentRow++;
        }

        // Auto-fit columns
        worksheet.Columns().AdjustToContents();

        // Convert to byte array
        using var stream = new System.IO.MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
