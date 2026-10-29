/* ============================================================================
   Demo seed: teachers, subjects, class↔subject assignments, and weekly
   timetable for EVERY campus of each demo school (Class 1/2/3 — Section A).

   Prerequisite: seed-demo-schools.sql

   Targets:
     The Educators School System
     Beacon Light Grammar School
     Al-Noor Public School

   Teacher logins (password Admin@123):
     Main campus:  t.english@educators.com …
     Other camps:  t.english.c2@educators.com , t.english.c3@educators.com …

   Idempotent: skips rows that already exist (by email / class-subject / slot).
   Local API uses OneSchoolDB (see appsettings.Development.json):
     sqlcmd -S localhost -d OneSchoolDB -E -C -i seed-demo-teachers-timetable.sql
   ============================================================================ */

SET NOCOUNT ON;
SET XACT_ABORT ON;
SET QUOTED_IDENTIFIER ON;

DECLARE @now  datetime2     = SYSUTCDATETIME();
DECLARE @hash nvarchar(200) = N'$2a$11$m8wwRHK2G.pwSEn1C42Y9.O.fWOyD4IGo9dMluIOr84ZzB7V6qB.m'; -- Admin@123
DECLARE @TeacherRoleId int  = (SELECT TOP 1 RoleId FROM Roles WHERE RoleName = N'teacher');

IF @TeacherRoleId IS NULL
BEGIN
    RAISERROR('Teacher role not found — aborting.', 16, 1);
    RETURN;
END

DECLARE @Schools TABLE (
    InstituteId int PRIMARY KEY,
    Name nvarchar(200),
    Domain nvarchar(50),
    AcademicYearId int
);

INSERT INTO @Schools (InstituteId, Name, Domain, AcademicYearId)
SELECT i.InstituteId, i.Name,
       CASE i.Name
            WHEN N'The Educators School System' THEN N'educators.com'
            WHEN N'Beacon Light Grammar School' THEN N'beaconlight.com'
            WHEN N'Al-Noor Public School'       THEN N'alnoor.com'
       END,
       (
           SELECT TOP 1 y.AcademicYearId
           FROM AcademicYears y
           WHERE y.InstituteId = i.InstituteId AND y.IsDeleted = 0 AND y.IsActive = 1
           ORDER BY y.AcademicYearId DESC
       )
FROM Institutes i
WHERE i.IsDeleted = 0
  AND i.Name IN (N'The Educators School System',
                 N'Beacon Light Grammar School',
                 N'Al-Noor Public School');

IF NOT EXISTS (SELECT 1 FROM @Schools)
BEGIN
    RAISERROR('No demo schools found. Run seed-demo-schools.sql first.', 16, 1);
    RETURN;
END

DECLARE @Campuses TABLE (
    InstituteId int,
    CampusId int,
    CampusTag nvarchar(20),
    Domain nvarchar(50),
    AcademicYearId int,
    IsMain bit,
    PRIMARY KEY (InstituteId, CampusId)
);

INSERT INTO @Campuses (InstituteId, CampusId, CampusTag, Domain, AcademicYearId, IsMain)
SELECT s.InstituteId, x.CampusId,
       CASE WHEN x.rn = 1 THEN N'main' WHEN x.rn = 2 THEN N'c2' ELSE N'c3' END,
       s.Domain, s.AcademicYearId,
       CASE WHEN x.rn = 1 THEN 1 ELSE 0 END
FROM @Schools s
JOIN (
    SELECT c.InstituteId, c.CampusId,
           ROW_NUMBER() OVER (PARTITION BY c.InstituteId ORDER BY c.CampusId) AS rn
    FROM Campuses c
    WHERE c.IsDeleted = 0
) x ON x.InstituteId = s.InstituteId;

BEGIN TRAN;

/* ── Subjects (6) per school ───────────────────────────────────────────── */
INSERT INTO Subjects (SubjectName, IsActive, IsDeleted, CreatedAt, InstituteId, CampusId)
SELECT sn.SubjectName, 1, 0, @now, s.InstituteId, NULL
FROM @Schools s
CROSS JOIN (VALUES
    (N'English'), (N'Urdu'), (N'Mathematics'),
    (N'Science'), (N'Islamiyat'), (N'Computer')
) sn(SubjectName)
WHERE NOT EXISTS (
    SELECT 1 FROM Subjects sj
    WHERE sj.InstituteId = s.InstituteId
      AND sj.SubjectName = sn.SubjectName
      AND sj.IsDeleted = 0
);

/* ── Teachers: 6 specialists per campus ────────────────────────────────── */
;WITH TeacherSeed AS (
    SELECT c.InstituteId, c.CampusId, c.CampusTag, c.Domain, c.IsMain,
           t.SubjectName, t.FullName, t.LocalPart, t.Gender, t.Qual,
           CASE WHEN c.IsMain = 1
                THEN t.LocalPart + N'@' + c.Domain
                ELSE t.LocalPart + N'.' + c.CampusTag + N'@' + c.Domain
           END AS Email,
           CASE WHEN c.IsMain = 1
                THEN t.FullName
                ELSE t.FullName + N' (' + UPPER(c.CampusTag) + N')'
           END AS DisplayName
    FROM @Campuses c
    CROSS JOIN (VALUES
        (N'English',     N'Ayesha Siddiqui',  N't.english',   N'Female', N'M.A. English'),
        (N'Urdu',        N'Farhan Ali',       N't.urdu',      N'Male',   N'M.A. Urdu'),
        (N'Mathematics', N'Saira Malik',      N't.math',      N'Female', N'M.Sc. Mathematics'),
        (N'Science',     N'Imran Qureshi',    N't.science',   N'Male',   N'M.Sc. Physics'),
        (N'Islamiyat',   N'Nadia Hussain',    N't.islamiyat', N'Female', N'M.A. Islamic Studies'),
        (N'Computer',    N'Usman Raza',       N't.computer',  N'Male',   N'B.S. Computer Science')
    ) t(SubjectName, FullName, LocalPart, Gender, Qual)
)
INSERT INTO Users
    (FullName, Email, PasswordHash, Password, RoleId, IsActive,
     Phone, Gender, Qualification, Specialization, JoiningDate,
     IsDeleted, CreatedAt, InstituteId, CampusId)
SELECT ts.DisplayName, ts.Email, @hash, N'Admin@123', @TeacherRoleId, 1,
       N'0301-' + RIGHT('0000000' + CAST(ABS(CHECKSUM(ts.Email)) % 10000000 AS varchar(7)), 7),
       ts.Gender, ts.Qual, ts.SubjectName, '2025-08-01',
       0, @now, ts.InstituteId, ts.CampusId
FROM TeacherSeed ts
WHERE NOT EXISTS (
    SELECT 1 FROM Users u WHERE u.Email = ts.Email AND u.IsDeleted = 0
);

/* campus + subject → teacher */
DECLARE @Map TABLE (
    InstituteId int,
    CampusId int,
    SubjectName nvarchar(100),
    TeacherId int,
    PRIMARY KEY (InstituteId, CampusId, SubjectName)
);

INSERT INTO @Map (InstituteId, CampusId, SubjectName, TeacherId)
SELECT c.InstituteId, c.CampusId, sj.SubjectName, u.UserId
FROM @Campuses c
JOIN Subjects sj ON sj.InstituteId = c.InstituteId AND sj.IsDeleted = 0
JOIN Users u ON u.InstituteId = c.InstituteId
            AND u.RoleId = @TeacherRoleId
            AND u.IsDeleted = 0
            AND u.Specialization = sj.SubjectName
            AND u.Email = CASE WHEN c.IsMain = 1
                THEN N't.' + CASE sj.SubjectName
                        WHEN N'English' THEN N'english' WHEN N'Urdu' THEN N'urdu'
                        WHEN N'Mathematics' THEN N'math' WHEN N'Science' THEN N'science'
                        WHEN N'Islamiyat' THEN N'islamiyat' WHEN N'Computer' THEN N'computer'
                     END + N'@' + c.Domain
                ELSE N't.' + CASE sj.SubjectName
                        WHEN N'English' THEN N'english' WHEN N'Urdu' THEN N'urdu'
                        WHEN N'Mathematics' THEN N'math' WHEN N'Science' THEN N'science'
                        WHEN N'Islamiyat' THEN N'islamiyat' WHEN N'Computer' THEN N'computer'
                     END + N'.' + c.CampusTag + N'@' + c.Domain
            END;

/* ── ClassSubjects for all Class 1/2/3 A ───────────────────────────────── */
INSERT INTO ClassSubjects
    (ClassId, SubjectId, TeacherId, IsActive, IsDeleted, CreatedAt, InstituteId, CampusId)
SELECT cl.ClassId, sj.SubjectId, m.TeacherId, 1, 0, @now, cl.InstituteId, cl.CampusId
FROM Classes cl
JOIN @Campuses c ON c.InstituteId = cl.InstituteId AND c.CampusId = cl.CampusId
JOIN Subjects sj ON sj.InstituteId = cl.InstituteId AND sj.IsDeleted = 0
JOIN @Map m ON m.InstituteId = cl.InstituteId AND m.CampusId = cl.CampusId AND m.SubjectName = sj.SubjectName
WHERE cl.IsDeleted = 0
  AND cl.Section = N'A'
  AND cl.ClassName IN (N'Class 1', N'Class 2', N'Class 3')
  AND NOT EXISTS (
      SELECT 1 FROM ClassSubjects cs
      WHERE cs.ClassId = cl.ClassId AND cs.SubjectId = sj.SubjectId AND cs.IsDeleted = 0
  );

/* ── Weekly timetable Mon–Sat ──────────────────────────────────────────── */
DECLARE @SubjOrder TABLE (Ord int PRIMARY KEY, SubjectName nvarchar(100));
INSERT INTO @SubjOrder (Ord, SubjectName) VALUES
 (0, N'English'), (1, N'Urdu'), (2, N'Mathematics'),
 (3, N'Science'), (4, N'Islamiyat'), (5, N'Computer');

DECLARE @TeachPeriods TABLE (Slot int PRIMARY KEY, PeriodId int);
INSERT INTO @TeachPeriods (Slot, PeriodId) VALUES
 (0, 1), (1, 2), (2, 3), (3, 5), (4, 6), (5, 7);

DECLARE @Cls TABLE (
    ClassId int PRIMARY KEY,
    InstituteId int,
    CampusId int,
    SortKey int,
    AcademicYearId int
);

INSERT INTO @Cls (ClassId, InstituteId, CampusId, SortKey, AcademicYearId)
SELECT cl.ClassId, cl.InstituteId, cl.CampusId,
       CASE cl.ClassName WHEN N'Class 1' THEN 0 WHEN N'Class 2' THEN 1 WHEN N'Class 3' THEN 2 ELSE 9 END,
       c.AcademicYearId
FROM Classes cl
JOIN @Campuses c ON c.InstituteId = cl.InstituteId AND c.CampusId = cl.CampusId
WHERE cl.IsDeleted = 0
  AND cl.Section = N'A'
  AND cl.ClassName IN (N'Class 1', N'Class 2', N'Class 3')
  AND c.AcademicYearId IS NOT NULL;

INSERT INTO SchoolTimetables
    (ClassId, SubjectId, TeacherId, PeriodId, DayOfWeek, AcademicYearId,
     IsDeleted, CreatedAt, InstituteId, CampusId)
SELECT
    cl.ClassId, sj.SubjectId, m.TeacherId, tp.PeriodId, d.DayOfWeek, cl.AcademicYearId,
    0, @now, cl.InstituteId, cl.CampusId
FROM @Cls cl
CROSS JOIN (VALUES (1),(2),(3),(4),(5),(6)) d(DayOfWeek)
CROSS JOIN @TeachPeriods tp
JOIN @SubjOrder so ON so.Ord = ((tp.Slot + cl.SortKey + (d.DayOfWeek - 1)) % 6)
JOIN Subjects sj ON sj.InstituteId = cl.InstituteId AND sj.SubjectName = so.SubjectName AND sj.IsDeleted = 0
JOIN @Map m ON m.InstituteId = cl.InstituteId AND m.CampusId = cl.CampusId AND m.SubjectName = sj.SubjectName
WHERE NOT EXISTS (
    SELECT 1 FROM SchoolTimetables tt
    WHERE tt.ClassId = cl.ClassId
      AND tt.PeriodId = tp.PeriodId
      AND tt.DayOfWeek = d.DayOfWeek
      AND tt.AcademicYearId = cl.AcademicYearId
      AND tt.IsDeleted = 0
);

COMMIT;

SELECT s.Name AS School,
       (SELECT COUNT(*) FROM Subjects sj WHERE sj.InstituteId = s.InstituteId AND sj.IsDeleted = 0) AS Subjects,
       (SELECT COUNT(*) FROM Users u WHERE u.InstituteId = s.InstituteId AND u.RoleId = @TeacherRoleId AND u.IsDeleted = 0) AS Teachers,
       (SELECT COUNT(*) FROM ClassSubjects cs WHERE cs.InstituteId = s.InstituteId AND cs.IsDeleted = 0) AS ClassSubjects,
       (SELECT COUNT(*) FROM SchoolTimetables tt WHERE tt.InstituteId = s.InstituteId AND tt.IsDeleted = 0) AS TimetableSlots
FROM Institutes s
WHERE s.Name IN (N'The Educators School System', N'Beacon Light Grammar School', N'Al-Noor Public School')
  AND s.IsDeleted = 0
ORDER BY s.Name;

PRINT 'Done. Every Class 1/2/3-A now has subjects + teachers + weekly timetable.';
