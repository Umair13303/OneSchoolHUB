using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Academics;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

public interface ICalendarEventService
{
    Task<List<AcademicCalendarEventDto>> GetByYearAsync(int academicYearId);
    Task<AcademicCalendarEventDto>       CreateAsync(int academicYearId, CreateCalendarEventDto dto, int createdBy);
    Task<bool>                           UpdateAsync(int eventId, UpdateCalendarEventDto dto, int updatedBy);
    Task<bool>                           DeleteAsync(int eventId, int deletedBy);
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
        var yearExists = await _db.AcademicYears.AnyAsync(y => y.AcademicYearId == academicYearId && !y.IsDeleted);
        if (!yearExists) throw new KeyNotFoundException("Academic year not found.");

        var entity = new AcademicCalendarEvent
        {
            AcademicYearId      = academicYearId,
            CalendarEventTypeId = dto.CalendarEventTypeId,
            Title               = dto.Title,
            StartDate           = dto.StartDate,
            EndDate             = dto.EndDate,
            Description         = dto.Description,
            CreatedBy           = createdBy
        };

        _db.AcademicCalendarEvents.Add(entity);
        await _db.SaveChangesAsync();
        return Map(entity);
    }

    public async Task<bool> UpdateAsync(int eventId, UpdateCalendarEventDto dto, int updatedBy)
    {
        var entity = await _db.AcademicCalendarEvents
            .FirstOrDefaultAsync(e => e.AcademicCalendarEventId == eventId && !e.IsDeleted);
        if (entity is null) return false;

        entity.CalendarEventTypeId = dto.CalendarEventTypeId;
        entity.Title               = dto.Title;
        entity.StartDate           = dto.StartDate;
        entity.EndDate     = dto.EndDate;
        entity.Description = dto.Description;
        entity.UpdatedBy   = updatedBy;
        entity.UpdatedAt   = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int eventId, int deletedBy)
    {
        var entity = await _db.AcademicCalendarEvents
            .FirstOrDefaultAsync(e => e.AcademicCalendarEventId == eventId && !e.IsDeleted);
        if (entity is null) return false;

        entity.IsDeleted = true;
        entity.UpdatedBy = deletedBy;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return true;
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
