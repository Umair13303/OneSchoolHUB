# Step 6 — Timetable & Periods

## What was added

### DTOs — `DTOs/Timetable/`
`PeriodDto`, `CreatePeriodDto`, `UpdatePeriodDto`,
`TimetableEntryDto`, `CreateTimetableEntryDto`, `UpdateTimetableEntryDto`,
`TimetableDayDto` (grouped-by-day for teacher view).

### Services
- **`Services/PeriodService.cs`** — `IPeriodService` + implementation.
  CRUD with time-overlap detection and unique `PeriodNo` / `PeriodName`.
- **`Services/TimetableService.cs`** — `ITimetableService` + implementation.
  List by class, teacher view grouped by day, CRUD with conflict checks.

### Controllers
- **`Controllers/PeriodController.cs`** at `/api/periods` (read = any
  authenticated, write = `superadmin`).
- **`Controllers/TimetableController.cs`** at `/api/timetable` (task-doc
  routes).

### DI registered in `Program.cs`
```csharp
builder.Services.AddScoped<IPeriodService, PeriodService>();
builder.Services.AddScoped<ITimetableService, TimetableService>();
```

## Endpoints — Timetable (match the task doc exactly)

| Verb   | Route                                        | Auth                                      |
|--------|----------------------------------------------|-------------------------------------------|
| GET    | `/api/timetable?classId=&dayOfWeek=`         | superadmin, admin, principal, teacher, parent |
| GET    | `/api/timetable/{id}`                        | superadmin, admin, principal, teacher, parent |
| GET    | `/api/timetable/teacher/{teacherId}`         | superadmin, admin, principal, teacher, parent |
| POST   | `/api/timetable`                             | superadmin, admin, principal              |
| PUT    | `/api/timetable/{id}`                        | superadmin, admin, principal              |
| DELETE | `/api/timetable/{id}`                        | superadmin, admin, principal              |

`?academicYearId=` is also accepted on the GETs to scope to a specific year.

## Endpoints — Periods (bonus, needed for setup UI)

| Verb   | Route                  | Auth              |
|--------|------------------------|-------------------|
| GET    | `/api/periods`         | Any authenticated |
| GET    | `/api/periods/{id}`    | Any authenticated |
| POST   | `/api/periods`         | superadmin        |
| PUT    | `/api/periods/{id}`    | superadmin        |
| DELETE | `/api/periods/{id}`    | superadmin        |

## Conflict rules enforced on Create / Update

1. **Class slot uniqueness** — `(ClassId, PeriodId, DayOfWeek, AcademicYearId)`
   must be unique. Prevents two subjects scheduled for the same class in the
   same period.
2. **Teacher double-booking** — `(TeacherId, PeriodId, DayOfWeek, AcademicYearId)`
   must be unique. Prevents a teacher from being in two classes at once.
3. **No subjects on breaks** — if the chosen Period has `IsBreak = true`,
   the call is rejected.
4. **Teacher role check** — the assigned user must have `RoleName == "teacher"`.
5. **Foreign keys** — Class, Subject, Period (not deleted), AcademicYear
   (not deleted), and Teacher (active user) must all exist.

## Day-of-week convention

The seed comment on `SchoolTimetable.DayOfWeek` says `1=Monday … 5=Friday`.
DTOs accept `1..7` (Range validation) so weekend days can be scheduled if a
school chooses to. The teacher view names them Monday through Sunday in the
response.

## Period-overlap rule

Two periods can share an end/start boundary (e.g. `08:40-09:20` and
`09:20-10:00`) but cannot overlap. Strict less-than comparisons handle this:
`a.StartTime < b.EndTime AND b.StartTime < a.EndTime` → overlap.

## Sample payloads

```http
POST /api/timetable
{
  "classId":        3,
  "subjectId":      1,
  "teacherId":     12,
  "periodId":       1,
  "dayOfWeek":      1,
  "academicYearId": 1
}

GET /api/timetable?classId=3&dayOfWeek=1
GET /api/timetable/teacher/12?academicYearId=1
```

## No migration needed

`Periods` and `SchoolTimetable` tables already exist from Step 1's
`InitialCreate`. Step 1 also seeded 7 default periods. Step 6 is code-only.
