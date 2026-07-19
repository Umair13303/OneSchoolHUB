using Microsoft.EntityFrameworkCore;
using SchoolManagement.API.Data;
using SchoolManagement.API.DTOs.Academics;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Services;

public interface ICalendarEventTypeService
{
    Task<List<CalendarEventTypeDto>> GetAllAsync();
    Task<CalendarEventTypeDto>       CreateAsync(CreateCalendarEventTypeDto dto, int createdBy);
    Task<bool>                       UpdateAsync(int id, CreateCalendarEventTypeDto dto, int updatedBy);
    Task<bool>                       DeleteAsync(int id, int deletedBy);
}

public class CalendarEventTypeService : ICalendarEventTypeService
{
    private readonly AppDbContext _db;
    public CalendarEventTypeService(AppDbContext db) => _db = db;

    public async Task<List<CalendarEventTypeDto>> GetAllAsync()
        => await _db.CalendarEventTypes
            .AsNoTracking()
            .Where(t => !t.IsDeleted)
            .OrderBy(t => t.SortOrder).ThenBy(t => t.Name)
            .Select(t => Map(t))
            .ToListAsync();

    public async Task<CalendarEventTypeDto> CreateAsync(CreateCalendarEventTypeDto dto, int createdBy)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Name is required.");

        var entity = new CalendarEventType
        {
            Name      = dto.Name.Trim(),
            Color     = dto.Color,
            Icon      = dto.Icon,
            SortOrder = dto.SortOrder,
            CreatedBy = createdBy
        };
        _db.CalendarEventTypes.Add(entity);
        await _db.SaveChangesAsync();
        return Map(entity);
    }

    public async Task<bool> UpdateAsync(int id, CreateCalendarEventTypeDto dto, int updatedBy)
    {
        var entity = await _db.CalendarEventTypes.FirstOrDefaultAsync(t => t.CalendarEventTypeId == id && !t.IsDeleted);
        if (entity is null) return false;

        entity.Name      = dto.Name.Trim();
        entity.Color     = dto.Color;
        entity.Icon      = dto.Icon;
        entity.SortOrder = dto.SortOrder;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id, int deletedBy)
    {
        var entity = await _db.CalendarEventTypes.FirstOrDefaultAsync(t => t.CalendarEventTypeId == id && !t.IsDeleted);
        if (entity is null) return false;

        var inUse = await _db.AcademicCalendarEvents.AnyAsync(e => e.CalendarEventTypeId == id && !e.IsDeleted);
        if (inUse) throw new InvalidOperationException("Cannot delete: this type is used by existing events.");

        entity.IsDeleted = true;
        entity.UpdatedBy = deletedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return true;
    }

    private static CalendarEventTypeDto Map(CalendarEventType t) => new()
    {
        CalendarEventTypeId = t.CalendarEventTypeId,
        Name                = t.Name,
        Color               = t.Color,
        Icon                = t.Icon,
        SortOrder           = t.SortOrder
    };
}
