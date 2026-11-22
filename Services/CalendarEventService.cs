using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Academics;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

public interface ICalendarEventService
{
    Task<List<AcademicCalendarEventDto>> GetByYearAsync(int academicYearId);
    Task<AcademicCalendarEventDto>       CreateAsync(int academicYearId, CreateCalendarEventDto dto, int createdBy);
    Task<bool>                           UpdateAsync(int academicYearId, int eventId, UpdateCalendarEventDto dto, int updatedBy);
    Task<bool>                           DeleteAsync(int academicYearId, int eventId, int deletedBy);
}

public class CalendarEventService : ICalendarEventService
{
    private readonly AppDbContext _db;
    public CalendarEventService(AppDbContext db) => _db = db;

    public async Task<List<AcademicCalendarEventDto>> GetByYearAsync(int academicYearId)
        => await _db.AcademicCalendarEvents
            .AsNoTracking()
            .Include(e => e.CalendarEventType)
            .Where(e => e.AcademicYearId == academicYearId && !e.IsDeleted)
            .OrderBy(e => e.StartDate)
            .Select(e => Map(e))
            .ToListAsync();

    public async Task<AcademicCalendarEventDto> CreateAsync(int academicYearId, CreateCalendarEventDto dto, int createdBy)
    {
        var year = await _db.AcademicYears.FirstOrDefaultAsync(y => y.AcademicYearId == academicYearId && !y.IsDeleted);
        if (year is null) throw new KeyNotFoundException("Academic year not found.");

        await ValidateEventAsync(year, dto.CalendarEventTypeId, dto.Title, dto.StartDate, dto.EndDate);

        var entity = new AcademicCalendarEvent
        {
            AcademicYearId      = academicYearId,
            CalendarEventTypeId = dto.CalendarEventTypeId,
            Title               = dto.Title.Trim(),
            StartDate           = dto.StartDate,
            EndDate             = dto.EndDate,
            Description         = dto.Description,
            CreatedBy           = createdBy
        };

        _db.AcademicCalendarEvents.Add(entity);
        await _db.SaveChangesAsync();

        await _db.Entry(entity).Reference(e => e.CalendarEventType).LoadAsync();
        return Map(entity);
    }

    public async Task<bool> UpdateAsync(int academicYearId, int eventId, UpdateCalendarEventDto dto, int updatedBy)
    {
        var year = await _db.AcademicYears.FirstOrDefaultAsync(y => y.AcademicYearId == academicYearId && !y.IsDeleted);
        if (year is null) throw new KeyNotFoundException("Academic year not found.");

        var entity = await _db.AcademicCalendarEvents
            .FirstOrDefaultAsync(e =>
                e.AcademicCalendarEventId == eventId &&
                e.AcademicYearId == academicYearId &&
                !e.IsDeleted);
        if (entity is null) return false;

        await ValidateEventAsync(year, dto.CalendarEventTypeId, dto.Title, dto.StartDate, dto.EndDate);

        entity.CalendarEventTypeId = dto.CalendarEventTypeId;
        entity.Title               = dto.Title.Trim();
        entity.StartDate           = dto.StartDate;
        entity.EndDate             = dto.EndDate;
        entity.Description         = dto.Description;
        entity.UpdatedBy           = updatedBy;
        entity.UpdatedAt           = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int academicYearId, int eventId, int deletedBy)
    {
        var entity = await _db.AcademicCalendarEvents
            .FirstOrDefaultAsync(e =>
                e.AcademicCalendarEventId == eventId &&
                e.AcademicYearId == academicYearId &&
                !e.IsDeleted);
        if (entity is null) return false;

        entity.IsDeleted = true;
        entity.UpdatedBy = deletedBy;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }

    private async Task ValidateEventAsync(
        AcademicYear year,
        int calendarEventTypeId,
        string? title,
        DateOnly startDate,
        DateOnly? endDate)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title is required.");

        if (endDate.HasValue && endDate.Value < startDate)
            throw new ArgumentException("End date must be on or after start date.");

        if (startDate < year.StartDate || startDate > year.EndDate)
            throw new ArgumentException("Start date must fall within the academic year.");

        if (endDate.HasValue && (endDate.Value < year.StartDate || endDate.Value > year.EndDate))
            throw new ArgumentException("End date must fall within the academic year.");

        var typeExists = await _db.CalendarEventTypes.AnyAsync(t => t.CalendarEventTypeId == calendarEventTypeId);
        if (!typeExists)
            throw new ArgumentException("Event type does not exist.");
    }

    private static AcademicCalendarEventDto Map(AcademicCalendarEvent e) => new()
    {
        AcademicCalendarEventId = e.AcademicCalendarEventId,
        AcademicYearId          = e.AcademicYearId,
        CalendarEventTypeId     = e.CalendarEventTypeId,
        EventTypeName           = e.CalendarEventType?.Name  ?? string.Empty,
        EventTypeColor          = e.CalendarEventType?.Color ?? "#6b7280",
        EventTypeIcon           = e.CalendarEventType?.Icon  ?? "event",
        Title                   = e.Title,
        StartDate               = e.StartDate,
        EndDate                 = e.EndDate,
        Description             = e.Description
    };
}
