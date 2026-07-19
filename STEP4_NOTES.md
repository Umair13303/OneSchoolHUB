# Step 4 — Academic Year, Class, Subject Setup

## What was added

### DTOs — `DTOs/Academics/`
`AcademicYearDto`, `CreateAcademicYearDto`, `UpdateAcademicYearDto`,
`SubjectDto`, `CreateSubjectDto`, `UpdateSubjectDto`,
`ClassDto`, `CreateClassDto`, `UpdateClassDto`,
`ClassSubjectDto`, `AssignTeacherDto`.

### Services — `Services/`
- `AcademicYearService` (`IAcademicYearService`) — CRUD, set-active (deactivates
  others atomically with `ExecuteUpdateAsync`), refuses delete if classes exist.
- `SubjectService` (`ISubjectService`) — CRUD, unique-name guard, refuses
  delete if used in any `ClassSubject`.
- `ClassService` (`IClassService`) — CRUD scoped to `AcademicYear`,
  `AssignTeacherAsync` (upsert + role validation), `GetSubjectsForClassAsync`.

### Controllers — `Controllers/`
- `AcademicYearController` at `/api/academic-years`
- `SubjectController` at `/api/subjects`
- `ClassController` at `/api/classes`

### DI registrations added to `Program.cs`
```csharp
builder.Services.AddScoped<IAcademicYearService, AcademicYearService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IClassService, ClassService>();
```

## Endpoints

### Academic years (bonus — needed because classes are scoped to a year)
| Verb   | Route                              | Auth                  |
|--------|------------------------------------|-----------------------|
| GET    | `/api/academic-years`              | Any authenticated     |
| GET    | `/api/academic-years/active`       | Any authenticated     |
| GET    | `/api/academic-years/{id}`         | Any authenticated     |
| POST   | `/api/academic-years`              | superadmin, admin     |
| PUT    | `/api/academic-years/{id}`         | superadmin, admin     |
| POST   | `/api/academic-years/{id}/set-active` | superadmin, admin  |
| DELETE | `/api/academic-years/{id}`         | superadmin, admin     |

### Subjects (task-doc routes)
| Verb   | Route                | Auth                  |
|--------|----------------------|-----------------------|
| GET    | `/api/subjects`      | Any authenticated     |
| GET    | `/api/subjects/{id}` | Any authenticated     |
| POST   | `/api/subjects`      | superadmin, admin     |
| PUT    | `/api/subjects/{id}` | superadmin, admin     |
| DELETE | `/api/subjects/{id}` | superadmin, admin     |

### Classes (task-doc routes + `/assign-teacher`)
| Verb   | Route                                   | Auth                  |
|--------|-----------------------------------------|-----------------------|
| GET    | `/api/classes` (optional `?academicYearId=`) | Any authenticated     |
| GET    | `/api/classes/{id}`                     | Any authenticated     |
| GET    | `/api/classes/{id}/subjects`            | Any authenticated     |
| POST   | `/api/classes`                          | superadmin, admin     |
| PUT    | `/api/classes/{id}`                     | superadmin, admin     |
| DELETE | `/api/classes/{id}`                     | superadmin, admin     |
| POST   | `/api/classes/{id}/assign-teacher`      | superadmin, admin     |

## Role gate

Per the *Class & Subject Setup* row in the task doc, all writes are
`[Authorize(Roles = "superadmin,admin")]`. Reads stay open to any authenticated
user so Teacher/Parent/Student screens can populate dropdowns and timetables.

## Sample calls

```http
POST /api/academic-years
{
  "yearLabel": "2025-2026",
  "startDate": "2025-04-01",
  "endDate":   "2026-03-31",
  "isActive":  true
}

POST /api/subjects
{ "subjectName": "Mathematics", "isActive": true }

POST /api/classes
{
  "className": "Grade 5",
  "section":   "A",
  "academicYearId": 1,
  "isActive":  true
}

POST /api/classes/1/assign-teacher
{ "subjectId": 1, "teacherId": 12 }
```

`assign-teacher` validates that the user being assigned actually has `RoleName == "teacher"`.

## Migration note

These changes are code-only — no entity schema changes. The `Classes`,
`Subjects`, `ClassSubjects`, and `AcademicYears` tables were already created
by Step 1's `InitialCreate` migration. No new migration is required for Step 4.
