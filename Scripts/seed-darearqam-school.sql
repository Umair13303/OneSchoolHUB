/* ============================================================================
   Local seed: Dar-e-Arqam School (+ Main Campus, admin, year, classes,
   teachers, students, guardians, enrollments, fee setup, subjects).

   Logins (password Admin@123):
     Admin:    admin@darearqam.local
     Teachers: t.english@darearqam.local … t.computer@darearqam.local

   Safe / idempotent: aborts if institute "Dar-e-Arqam School" already exists.

   Local DB (appsettings.Development.json):
     sqlcmd -S localhost -d OneSchoolDev -E -C -i seed-darearqam-school.sql
   Or:  powershell -File Scripts/seed-darearqam-local.ps1
   ============================================================================ */

SET NOCOUNT ON;
SET XACT_ABORT ON;
SET QUOTED_IDENTIFIER ON;

IF EXISTS (SELECT 1 FROM Institutes WHERE Name = N'Dar-e-Arqam School' AND IsDeleted = 0)
BEGIN
    RAISERROR('Dar-e-Arqam School already exists — seed aborted.', 16, 1);
    RETURN;
END

DECLARE @now  datetime2     = SYSUTCDATETIME();
-- BCrypt hash of Admin@123 (matches app seed / other demo scripts)
DECLARE @hash nvarchar(200) = N'$2a$11$m8wwRHK2G.pwSEn1C42Y9.O.fWOyD4IGo9dMluIOr84ZzB7V6qB.m';
DECLARE @TeacherRoleId int  = (SELECT TOP 1 RoleId FROM Roles WHERE RoleName = N'teacher');
DECLARE @AdminRoleId int    = (SELECT TOP 1 RoleId FROM Roles WHERE RoleName = N'admin');

IF @TeacherRoleId IS NULL OR @AdminRoleId IS NULL
BEGIN
    RAISERROR('admin/teacher roles missing — aborting.', 16, 1);
    RETURN;
END

BEGIN TRAN;

/* ── 1. Institute ──────────────────────────────────────────────────────── */
DECLARE @InstituteId int;

INSERT INTO Institutes
    (Name, Address, Phone, Email, Website, LogoUrl, IsActive,
     ModuleAttendance, ModuleFees, ModuleHomework, ModuleExams,
     ModuleTimetable, ModuleHR, ModuleReports, ModuleCurriculum,
     ChallanTemplate, SchoolStampUrl, LicenseValidUntil,
     IsDeleted, CreatedAt, CampusId)
VALUES
    (N'Dar-e-Arqam School',
     N'Main Boulevard, Gulberg III, Lahore',
     N'042-35770001',
     N'info@darearqam.local',
     N'www.darearqam.local',
     -- Placeholder logo path (upload real logo via Institute settings)
     N'/assets/school-logos/darearqam-placeholder.png', 1,
     1,1,1,1,1,1,1,1,
     N'cash_memo', NULL, NULL,
     0, @now, NULL);

SET @InstituteId = SCOPE_IDENTITY();

/* ── 2. Main campus ────────────────────────────────────────────────────── */
DECLARE @CampusId int;

INSERT INTO Campuses (InstituteId, Name, Address, Phone, IsActive, IsDeleted, CreatedAt)
VALUES (@InstituteId, N'Main Campus', N'Main Boulevard, Gulberg III, Lahore', N'042-35770011', 1, 0, @now);

SET @CampusId = SCOPE_IDENTITY();

/* ── 3. School admin ───────────────────────────────────────────────────── */
INSERT INTO Users
    (FullName, Email, PasswordHash, Password, RoleId, IsActive,
     Phone, IsDeleted, CreatedAt, InstituteId, CampusId)
VALUES
    (N'Dar-e-Arqam Admin', N'admin@darearqam.local', @hash, N'Admin@123', @AdminRoleId, 1,
     N'0300-1112233', 0, @now, @InstituteId, @CampusId);

/* ── 4. Academic year ──────────────────────────────────────────────────── */
DECLARE @AcademicYearId int;

INSERT INTO AcademicYears
    (YearLabel, StartDate, EndDate, IsActive, IsDeleted, CreatedAt, InstituteId, CampusId)
VALUES
    (N'2026-2027', '2026-04-01', '2027-03-31', 1, 0, @now, @InstituteId, NULL);

SET @AcademicYearId = SCOPE_IDENTITY();

/* ── 5. Classes: Class 1..5 Section A ──────────────────────────────────── */
DECLARE @Cls TABLE (ClassId int, ClassName nvarchar(100), LevelNo int);

INSERT INTO Classes
    (ClassName, Section, AcademicYearId, IsActive, IsDeleted, CreatedAt, InstituteId, CampusId)
OUTPUT inserted.ClassId, inserted.ClassName, CAST(REPLACE(inserted.ClassName, N'Class ', N'') AS int)
    INTO @Cls (ClassId, ClassName, LevelNo)
SELECT N'Class ' + CAST(n AS nvarchar(2)), N'A', @AcademicYearId, 1, 0, @now, @InstituteId, @CampusId
FROM (VALUES (1),(2),(3),(4),(5)) v(n);

/* ── 6. Fee types + structures ─────────────────────────────────────────── */
DECLARE @FT TABLE (FeeTypeId int, Name nvarchar(100));

INSERT INTO FeeTypes
    (Name, Description, FeeCategory, IsActive, IsDeleted, CreatedAt, InstituteId, CampusId)
OUTPUT inserted.FeeTypeId, inserted.Name INTO @FT
VALUES
 (N'Tuition Fee',      N'Monthly tuition fee',                       0, 1, 0, @now, @InstituteId, NULL),
 (N'Admission Fee',    N'One-time admission fee',                    1, 1, 0, @now, @InstituteId, NULL),
 (N'Exam Fee',         N'Charged per exam term',                     2, 1, 0, @now, @InstituteId, NULL),
 (N'Security Deposit', N'Refundable security deposit on admission',  3, 1, 0, @now, @InstituteId, NULL);

INSERT INTO FeeStructures
    (FeeTypeId, ClassId, AcademicYearId, Amount, DueDay, IsActive,
     IsDeleted, CreatedAt, InstituteId, CampusId)
SELECT ft.FeeTypeId, c.ClassId, @AcademicYearId,
       CASE ft.Name
            WHEN N'Tuition Fee'      THEN 2500 + c.LevelNo * 500
            WHEN N'Admission Fee'    THEN 12000
            WHEN N'Exam Fee'         THEN 2000
            WHEN N'Security Deposit' THEN 5000
       END,
       c.ClassName + N' Fee Structure 2026-27',
       1, 0, @now, @InstituteId, @CampusId
FROM @Cls c
CROSS JOIN @FT ft;

/* ── 7. Subjects ───────────────────────────────────────────────────────── */
INSERT INTO Subjects (SubjectName, IsActive, IsDeleted, CreatedAt, InstituteId, CampusId)
SELECT sn.SubjectName, 1, 0, @now, @InstituteId, NULL
FROM (VALUES
    (N'English'), (N'Urdu'), (N'Mathematics'),
    (N'Science'), (N'Islamiyat'), (N'Computer')
) sn(SubjectName);

/* ── 8. Teachers (6) ───────────────────────────────────────────────────── */
INSERT INTO Users
    (FullName, Email, PasswordHash, Password, RoleId, IsActive,
     Phone, Gender, Qualification, Specialization, JoiningDate,
     IsDeleted, CreatedAt, InstituteId, CampusId)
VALUES
 (N'Ayesha Siddiqui',  N't.english@darearqam.local',   @hash, N'Admin@123', @TeacherRoleId, 1, N'0301-2001001', N'Female', N'M.A. English',          N'English',     '2025-08-01', 0, @now, @InstituteId, @CampusId),
 (N'Farhan Ali',       N't.urdu@darearqam.local',      @hash, N'Admin@123', @TeacherRoleId, 1, N'0301-2001002', N'Male',   N'M.A. Urdu',             N'Urdu',        '2025-08-01', 0, @now, @InstituteId, @CampusId),
 (N'Saira Malik',      N't.math@darearqam.local',      @hash, N'Admin@123', @TeacherRoleId, 1, N'0301-2001003', N'Female', N'M.Sc. Mathematics',     N'Mathematics', '2025-08-01', 0, @now, @InstituteId, @CampusId),
 (N'Imran Qureshi',    N't.science@darearqam.local',   @hash, N'Admin@123', @TeacherRoleId, 1, N'0301-2001004', N'Male',   N'M.Sc. Physics',         N'Science',     '2025-08-01', 0, @now, @InstituteId, @CampusId),
 (N'Nadia Hussain',    N't.islamiyat@darearqam.local', @hash, N'Admin@123', @TeacherRoleId, 1, N'0301-2001005', N'Female', N'M.A. Islamic Studies',  N'Islamiyat',   '2025-08-01', 0, @now, @InstituteId, @CampusId),
 (N'Usman Raza',       N't.computer@darearqam.local',  @hash, N'Admin@123', @TeacherRoleId, 1, N'0301-2001006', N'Male',   N'B.S. Computer Science', N'Computer',    '2025-08-01', 0, @now, @InstituteId, @CampusId);

/* ClassSubjects: every class × every subject → matching teacher */
INSERT INTO ClassSubjects
    (ClassId, SubjectId, TeacherId, IsActive, IsDeleted, CreatedAt, InstituteId, CampusId)
SELECT c.ClassId, sj.SubjectId, u.UserId, 1, 0, @now, @InstituteId, @CampusId
FROM @Cls c
CROSS JOIN Subjects sj
JOIN Users u ON u.InstituteId = @InstituteId
            AND u.RoleId = @TeacherRoleId
            AND u.IsDeleted = 0
            AND u.Specialization = sj.SubjectName
WHERE sj.InstituteId = @InstituteId AND sj.IsDeleted = 0;

/* ── 9. Students: 5 per class ──────────────────────────────────────────── */
CREATE TABLE #StuSeed (
    AdmissionNo nvarchar(50), FirstName nvarchar(100), LastName nvarchar(100),
    Gender nvarchar(20), DateOfBirth date, ClassId int);

INSERT INTO #StuSeed
SELECT N'ARQ-2026-' + RIGHT('0000' + CAST(ROW_NUMBER() OVER (ORDER BY c.LevelNo, n.n) AS varchar(4)), 4),
       CASE WHEN n.n % 2 = 1
            THEN CHOOSE((c.LevelNo + n.n) % 10 + 1, N'Ahmed', N'Ali', N'Hassan', N'Usman', N'Bilal',
                                                    N'Hamza', N'Zain', N'Umar', N'Ibrahim', N'Rayyan')
            ELSE CHOOSE((c.LevelNo + n.n) % 10 + 1, N'Ayesha', N'Fatima', N'Zainab', N'Maryam', N'Hira',
                                                    N'Amna', N'Khadija', N'Noor', N'Iqra', N'Mahnoor')
       END,
       CHOOSE((c.LevelNo * 3 + n.n) % 10 + 1, N'Khan', N'Ahmed', N'Malik', N'Butt', N'Sheikh',
                                              N'Raza', N'Qureshi', N'Chaudhry', N'Awan', N'Mirza'),
       CASE WHEN n.n % 2 = 1 THEN N'Male' ELSE N'Female' END,
       DATEADD(DAY, (c.LevelNo * 17 + n.n * 61) % 300, DATEFROMPARTS(2020 - c.LevelNo, 1, 1)),
       c.ClassId
FROM @Cls c
CROSS JOIN (VALUES (1),(2),(3),(4),(5)) n(n);

DECLARE @Stu TABLE (StudentId int, AdmissionNo nvarchar(50));

INSERT INTO Students
    (AdmissionNo, FirstName, LastName, DateOfBirth, Gender, Religion, Nationality,
     Address, Phone, AdmissionDate, IsActive, IsDeleted, CreatedAt, InstituteId, CampusId)
OUTPUT inserted.StudentId, inserted.AdmissionNo INTO @Stu
SELECT AdmissionNo, FirstName, LastName, DateOfBirth, Gender, N'Islam', N'Pakistani',
       N'House ' + RIGHT(AdmissionNo, 4) + N', Gulberg III, Lahore',
       N'0300-' + RIGHT('0000000' + CAST(ABS(CHECKSUM(AdmissionNo)) % 10000000 AS varchar(7)), 7),
       '2026-04-01', 1, 0, @now, @InstituteId, @CampusId
FROM #StuSeed;

INSERT INTO StudentGuardians
    (StudentId, UserId, Relation, FullName, Phone, Occupation, CNIC, IsDeleted, CreatedAt)
SELECT st.StudentId, NULL, N'Father',
       CHOOSE(st.StudentId % 8 + 1, N'Muhammad Akram', N'Abdul Rasheed', N'Tariq Mehmood',
              N'Shahid Iqbal', N'Naveed Anjum', N'Rashid Mahmood', N'Javed Akhtar', N'Imran Aslam')
           + N' ' + ss.LastName,
       N'0321-' + RIGHT('0000000' + CAST(ABS(CHECKSUM(ss.AdmissionNo, 7)) % 10000000 AS varchar(7)), 7),
       CHOOSE(st.StudentId % 5 + 1, N'Business', N'Government Service', N'Teacher', N'Engineer', N'Shopkeeper'),
       N'35202-' + RIGHT('0000000' + CAST(ABS(CHECKSUM(ss.AdmissionNo, 3)) % 10000000 AS varchar(7)), 7) + N'-1',
       0, @now
FROM @Stu st
JOIN #StuSeed ss ON ss.AdmissionNo = st.AdmissionNo;

INSERT INTO StudentClassEnrollments
    (StudentId, ClassId, AcademicYearId, EnrollmentDate, Status,
     IsDeleted, CreatedAt, InstituteId, CampusId)
SELECT st.StudentId, ss.ClassId, @AcademicYearId, '2026-04-01', N'Active',
       0, @now, @InstituteId, @CampusId
FROM @Stu st
JOIN #StuSeed ss ON ss.AdmissionNo = st.AdmissionNo;

/* Second guardian (Mother) on first 5 students */
INSERT INTO StudentGuardians
    (StudentId, UserId, Relation, FullName, Phone, Occupation, CNIC, IsDeleted, CreatedAt)
SELECT TOP 5 st.StudentId, NULL, N'Mother',
       N'Mrs. ' + ss.LastName,
       N'0333-' + RIGHT('0000000' + CAST(ABS(CHECKSUM(ss.AdmissionNo, 11)) % 10000000 AS varchar(7)), 7),
       N'Homemaker', NULL, 0, @now
FROM @Stu st
JOIN #StuSeed ss ON ss.AdmissionNo = st.AdmissionNo
ORDER BY st.StudentId;

/* Sibling pair demo: share CNIC + SiblingGroupKey/Order on first two Class-1 students */
DECLARE @Sib1 int, @Sib2 int, @SibCnic nvarchar(30), @SibKey nvarchar(50);
SELECT TOP 1 @Sib1 = st.StudentId
FROM @Stu st JOIN #StuSeed ss ON ss.AdmissionNo = st.AdmissionNo
WHERE ss.ClassId = (SELECT TOP 1 ClassId FROM @Cls WHERE LevelNo = 1)
ORDER BY st.StudentId;
SELECT TOP 1 @Sib2 = st.StudentId
FROM @Stu st JOIN #StuSeed ss ON ss.AdmissionNo = st.AdmissionNo
WHERE ss.ClassId = (SELECT TOP 1 ClassId FROM @Cls WHERE LevelNo = 1)
  AND st.StudentId <> @Sib1
ORDER BY st.StudentId;

IF @Sib1 IS NOT NULL AND @Sib2 IS NOT NULL
BEGIN
    SET @SibCnic = N'35202-1001001-1';
    SET @SibKey  = N'3520210010011';
    UPDATE StudentGuardians SET CNIC = @SibCnic
    WHERE StudentId IN (@Sib1, @Sib2) AND Relation = N'Father' AND IsDeleted = 0;
    UPDATE Students SET SiblingGroupKey = @SibKey, SiblingOrder = 1 WHERE StudentId = @Sib1;
    UPDATE Students SET SiblingGroupKey = @SibKey, SiblingOrder = 2 WHERE StudentId = @Sib2;
END

/* StudentFees: current-month tuition for every seeded student */
INSERT INTO StudentFees
    (StudentId, FeeStructureId, AcademicYearId, AmountDue, AmountPaid, Discount,
     DueDate, FeeMonth, Status, Remarks, IsDeleted, CreatedAt, InstituteId, CampusId)
SELECT st.StudentId, fs.FeeStructureId, @AcademicYearId, fs.Amount,
       CASE WHEN st.StudentId % 3 = 0 THEN fs.Amount WHEN st.StudentId % 3 = 1 THEN fs.Amount / 2 ELSE 0 END,
       0,
       DATEFROMPARTS(YEAR(@now), MONTH(@now), 10),
       MONTH(@now),
       CASE WHEN st.StudentId % 3 = 0 THEN N'Paid' WHEN st.StudentId % 3 = 1 THEN N'Partial' ELSE N'Unpaid' END,
       N'Seed tuition',
       0, @now, @InstituteId, @CampusId
FROM @Stu st
JOIN #StuSeed ss ON ss.AdmissionNo = st.AdmissionNo
JOIN FeeStructures fs ON fs.ClassId = ss.ClassId AND fs.AcademicYearId = @AcademicYearId AND fs.IsDeleted = 0
JOIN FeeTypes ft ON ft.FeeTypeId = fs.FeeTypeId AND ft.Name = N'Tuition Fee';

/* Sample attendance for last 5 school days × first teaching period (if periods exist) */
IF EXISTS (SELECT 1 FROM Periods WHERE IsDeleted = 0 AND IsBreak = 0)
BEGIN
    DECLARE @PeriodId int = (SELECT TOP 1 PeriodId FROM Periods WHERE IsDeleted = 0 AND IsBreak = 0 ORDER BY PeriodNo);
    DECLARE @AdminUserId int = (SELECT TOP 1 UserId FROM Users WHERE Email = N'admin@darearqam.local');

    ;WITH Days AS (
        SELECT CAST(DATEADD(DAY, -n, CAST(@now AS date)) AS date) AS D
        FROM (VALUES (1),(2),(3),(4),(5)) v(n)
    )
    INSERT INTO Attendances
        (StudentId, ClassId, PeriodId, Date, Status, MarkedBy, MarkedAt, Remarks, IsDeleted, InstituteId, CampusId)
    SELECT st.StudentId, ss.ClassId, @PeriodId, d.D,
           CASE WHEN ABS(CHECKSUM(st.StudentId, d.D)) % 10 = 0 THEN N'Absent'
                WHEN ABS(CHECKSUM(st.StudentId, d.D)) % 10 = 1 THEN N'Late'
                ELSE N'Present' END,
           @AdminUserId, @now, N'Seed', 0, @InstituteId, @CampusId
    FROM @Stu st
    JOIN #StuSeed ss ON ss.AdmissionNo = st.AdmissionNo
    CROSS JOIN Days d;
END

DROP TABLE #StuSeed;

/* ── 10. Optional weekly timetable (if Periods seeded) ─────────────────── */
IF EXISTS (SELECT 1 FROM Periods WHERE IsDeleted = 0 AND IsBreak = 0)
BEGIN
    DECLARE @SubjOrder TABLE (Ord int PRIMARY KEY, SubjectName nvarchar(100));
    INSERT INTO @SubjOrder (Ord, SubjectName) VALUES
     (0, N'English'), (1, N'Urdu'), (2, N'Mathematics'),
     (3, N'Science'), (4, N'Islamiyat'), (5, N'Computer');

    DECLARE @TeachPeriods TABLE (Slot int PRIMARY KEY, PeriodId int);
    ;WITH P AS (
        SELECT PeriodId, ROW_NUMBER() OVER (ORDER BY PeriodNo) - 1 AS Slot
        FROM Periods WHERE IsDeleted = 0 AND IsBreak = 0
    )
    INSERT INTO @TeachPeriods (Slot, PeriodId)
    SELECT Slot, PeriodId FROM P WHERE Slot BETWEEN 0 AND 5;

    INSERT INTO SchoolTimetables
        (ClassId, SubjectId, TeacherId, PeriodId, DayOfWeek, AcademicYearId,
         IsDeleted, CreatedAt, InstituteId, CampusId)
    SELECT c.ClassId, sj.SubjectId, u.UserId, tp.PeriodId, d.DayOfWeek, @AcademicYearId,
           0, @now, @InstituteId, @CampusId
    FROM @Cls c
    CROSS JOIN (VALUES (1),(2),(3),(4),(5),(6)) d(DayOfWeek) -- Mon-Sat
    CROSS JOIN @TeachPeriods tp
    JOIN @SubjOrder so ON so.Ord = (tp.Slot + c.LevelNo + d.DayOfWeek) % 6
    JOIN Subjects sj ON sj.InstituteId = @InstituteId AND sj.SubjectName = so.SubjectName AND sj.IsDeleted = 0
    JOIN Users u ON u.InstituteId = @InstituteId AND u.RoleId = @TeacherRoleId
                AND u.Specialization = so.SubjectName AND u.IsDeleted = 0;
END

COMMIT;

/* ── Summary ───────────────────────────────────────────────────────────── */
SELECT
    i.Name AS School,
    i.InstituteId,
    (SELECT COUNT(*) FROM Campuses c WHERE c.InstituteId = i.InstituteId AND c.IsDeleted = 0) AS Campuses,
    (SELECT COUNT(*) FROM Classes c WHERE c.InstituteId = i.InstituteId AND c.IsDeleted = 0) AS Classes,
    (SELECT COUNT(*) FROM Users u WHERE u.InstituteId = i.InstituteId AND u.RoleId = @TeacherRoleId AND u.IsDeleted = 0) AS Teachers,
    (SELECT COUNT(*) FROM Students s WHERE s.InstituteId = i.InstituteId AND s.IsDeleted = 0) AS Students
FROM Institutes i
WHERE i.InstituteId = @InstituteId;

PRINT 'Admin login:  admin@darearqam.local / Admin@123';
PRINT 'Teacher e.g: t.math@darearqam.local / Admin@123';
