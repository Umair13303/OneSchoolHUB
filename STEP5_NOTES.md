# Step 5 — Student Admission Form

## What was added

### DTOs — `DTOs/Students/`
`StudentListDto`, `StudentDetailDto`, `CreateStudentAdmissionDto`,
`UpdateStudentDto`, `GuardianDto`, `CreateGuardianDto`, `StudentDocumentDto`.

### Service — `Services/StudentService.cs`
`IStudentService` + `StudentService`.

### Controller — `Controllers/StudentController.cs`
Mounted at `/api/students`. All endpoints require `superadmin`, `admin`, or
`principal` (per the *Student Admission* row of the access matrix).

### DI
```csharp
builder.Services.AddScoped<IStudentService, StudentService>();
```

## Endpoints (match the task doc exactly)

| Verb | Route                                              | Purpose |
|------|----------------------------------------------------|---------|
| POST | `/api/students`                                    | New admission. Creates Student + Guardians + initial Enrollment + tags uploaded documents — all in one transaction. |
| GET  | `/api/students?classId=&academicYearId=&search=`   | List with optional filters. Returns the *current active* class for each student. |
| GET  | `/api/students/{id}`                               | Full profile incl. guardians + current enrollment. |
| PUT  | `/api/students/{id}`                               | Update editable profile fields. AdmissionNo is immutable; class transfer is not handled here. |
| GET  | `/api/students/{id}/documents`                     | All FileStore rows tagged for this student (photo, birth-cert, prev-school-cert). |

## Admission flow

1. The user fills the New Admission form in Angular.
2. Photo / Birth Cert / Prev School Cert are **uploaded first** to the
   FileServer via `POST /api/files/upload` (Step 2). Each call returns a
   `fileId`.
3. Angular submits the admission body to `POST /api/students`, including any
   `PhotoFileId`, `BirthCertificateFileId`, `PreviousSchoolCertFileId` it got
   back from the FileServer.
4. `StudentService.CreateAdmissionAsync` runs in a single transaction:
   - Generates the AdmissionNo (`ADM-YYYY-NNNN`)
   - Inserts the Student row (retries up to 5× on `AdmissionNo` collision)
   - Inserts each Guardian
   - Inserts the initial `StudentClassEnrollment` (status `Active`)
   - Tags the uploaded files in `FileStore` with `EntityType` + `EntityId`
     so `GET /api/students/{id}/documents` can find them
5. Returns the full `StudentDetailDto`.

## Admission number

Format: **`ADM-{Year}-{0001}`** (e.g. `ADM-2026-0042`).

The next sequence is computed by scanning existing `AdmissionNo` rows that
start with `ADM-{Year}-` and taking `max + 1`. The unique index on
`Students.AdmissionNo` (created in Step 1) is the safety net, and the
admission method retries up to 5 times on collision.

## Document tagging convention

Files attached to a student carry these `EntityType` values in `FileStore`:

| EntityType         | Purpose                  |
|--------------------|--------------------------|
| `student-photo`    | Profile photo            |
| `birth-certificate`| Birth certificate scan   |
| `prev-school-cert` | Previous school cert     |

These strings line up with the FileServer's accepted `entityType` values
defined in `SchoolManagement.FileServer/Models/EntityTypes.cs`.

## Sample admission body

```http
POST /api/students
Authorization: Bearer <token>
Content-Type: application/json

{
  "firstName": "Asha",
  "lastName":  "Khan",
  "dateOfBirth": "2013-08-21",
  "gender":     "Female",
  "bloodGroup": "B+",
  "religion":   "Islam",
  "nationality": "Pakistani",
  "address":    "House 12, Street 4, F-7, Islamabad",
  "phone":      "+92-300-1234567",
  "email":      "asha.khan@example.com",

  "admissionDate":  "2026-04-10",
  "academicYearId": 1,
  "classId":        3,

  "photoFileId":              101,
  "birthCertificateFileId":   102,
  "previousSchoolCertFileId": 103,

  "guardians": [
    { "fullName": "Aslam Khan",  "relation": "Father", "phone": "+92-300-1111111", "occupation": "Engineer",  "cnic": "12345-6789012-3" },
    { "fullName": "Sara Khan",   "relation": "Mother", "phone": "+92-300-2222222", "occupation": "Teacher",   "cnic": "12345-1234567-8" }
  ]
}
```

## No migration needed

All tables (`Students`, `StudentGuardians`, `StudentClassEnrollments`,
`FileStores`) already exist from Step 1's `InitialCreate`. Step 5 is
code-only.
