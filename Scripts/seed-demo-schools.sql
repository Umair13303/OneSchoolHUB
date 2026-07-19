/* ============================================================================
   Demo seed: 3 schools, each with 2-3 campuses, classes, students, guardians,
   enrollments, fee types and fee structures — ready for challan generation.

   Schools & logins (all passwords: Admin@123):
     1. The Educators School System  → admin@educators.com    (challan: cash_memo,  3 campuses)
     2. Beacon Light Grammar School  → admin@beaconlight.com  (challan: bank_3copy, 2 campuses, license till 2026-12-31)
     3. Al-Noor Public School        → admin@alnoor.com       (challan: detailed,   2 campuses)

   Per school: academic year 2026-2027 (active), 3 classes per campus
   (Class 1/2/3, Section A), 5 students per class with father as guardian,
   active enrollment, 4 fee types (Tuition/Admission/Exam/Security) and a
   fee structure per class.  StudentFees (challans) are intentionally NOT
   seeded — generate them from the UI (Fees → bulk assign) to test the flow.

   Safe to run once; aborts if any of the three school names already exist.
   Run:  sqlcmd -S localhost -d SchoolDB -E -C -i seed-demo-schools.sql
   ============================================================================ */

SET NOCOUNT ON;
SET XACT_ABORT ON;
SET QUOTED_IDENTIFIER ON;   -- required: DB has filtered indexes (per-school AdmissionNo)

IF EXISTS (SELECT 1 FROM Institutes
           WHERE Name IN (N'The Educators School System',
                          N'Beacon Light Grammar School',
                          N'Al-Noor Public School'))
BEGIN
    RAISERROR('Demo schools already exist — seed aborted. Delete them first to re-seed.', 16, 1);
    RETURN;
END

BEGIN TRAN;

DECLARE @now  datetime2     = SYSUTCDATETIME();
-- BCrypt hash of "Admin@123" (same hash the app seeds for the superadmin)
DECLARE @hash nvarchar(200) = N'$2a$11$m8wwRHK2G.pwSEn1C42Y9.O.fWOyD4IGo9dMluIOr84ZzB7V6qB.m';

/* ── 1. Institutes ──────────────────────────────────────────────────────── */
DECLARE @Inst TABLE (InstituteId int, Name nvarchar(200));

INSERT INTO Institutes
    (Name, Address, Phone, Email, Website, LogoUrl, IsActive,
     ModuleAttendance, ModuleFees, ModuleHomework, ModuleExams,
     ModuleTimetable, ModuleHR, ModuleReports,
     ChallanTemplate, SchoolStampUrl, LicenseValidUntil,
     IsDeleted, CreatedAt, CampusId)
OUTPUT inserted.InstituteId, inserted.Name INTO @Inst
VALUES
 (N'The Educators School System', N'12-A Model Town, Lahore',      N'042-35880001', N'info@educators.com',   N'www.educators.com',   NULL, 1, 1,1,1,1,1,1,1, N'cash_memo',  NULL, NULL,          0, @now, NULL),
 (N'Beacon Light Grammar School', N'45 Satellite Town, Rawalpindi', N'051-44550002', N'info@beaconlight.com', N'www.beaconlight.com', NULL, 1, 1,1,1,1,1,1,1, N'bank_3copy', NULL, '2026-12-31',  0, @now, NULL),
 (N'Al-Noor Public School',       N'Main GT Road, Gujranwala',      N'055-38210003', N'info@alnoor.com',      N'www.alnoor.com',      NULL, 1, 1,1,1,1,1,1,1, N'detailed',   NULL, NULL,          0, @now, NULL);

/* ── 2. Campuses (3 + 2 + 2) ───────────────────────────────────────────── */
DECLARE @Camp TABLE (CampusId int, InstituteId int, Name nvarchar(200));

INSERT INTO Campuses (InstituteId, Name, Address, Phone, IsActive, IsDeleted, CreatedAt)
OUTPUT inserted.CampusId, inserted.InstituteId, inserted.Name INTO @Camp
SELECT i.InstituteId, c.CampusName, c.Address, c.Phone, 1, 0, @now
FROM (VALUES
 (N'The Educators School System', N'Main Campus',    N'12-A Model Town, Lahore',        N'042-35880011'),
 (N'The Educators School System', N'North Campus',   N'Block C, Wapda Town, Lahore',    N'042-35880012'),
 (N'The Educators School System', N'Gulberg Campus', N'MM Alam Road, Gulberg, Lahore',  N'042-35880013'),
 (N'Beacon Light Grammar School', N'Main Campus',    N'45 Satellite Town, Rawalpindi',  N'051-44550021'),
 (N'Beacon Light Grammar School', N'City Campus',    N'Committee Chowk, Rawalpindi',    N'051-44550022'),
 (N'Al-Noor Public School',       N'Boys Campus',    N'Main GT Road, Gujranwala',       N'055-38210031'),
 (N'Al-Noor Public School',       N'Girls Campus',   N'Civil Lines, Gujranwala',        N'055-38210032')
) c(InstName, CampusName, Address, Phone)
JOIN @Inst i ON i.Name = c.InstName;

/* ── 3. One admin user per school (RoleId 2 = admin) ───────────────────── */
INSERT INTO Users
    (FullName, Email, PasswordHash, Password, RoleId, IsActive,
     IsDeleted, CreatedAt, InstituteId, CampusId)
SELECT a.FullName, a.Email, @hash, N'Admin@123', 2, 1, 0, @now, i.InstituteId, NULL
FROM (VALUES
 (N'The Educators School System', N'Educators Admin',    N'admin@educators.com'),
 (N'Beacon Light Grammar School', N'Beacon Light Admin', N'admin@beaconlight.com'),
 (N'Al-Noor Public School',       N'Al-Noor Admin',      N'admin@alnoor.com')
) a(InstName, FullName, Email)
JOIN @Inst i ON i.Name = a.InstName;

/* ── 4. Active academic year 2026-2027 per school ──────────────────────── */
DECLARE @Year TABLE (AcademicYearId int, InstituteId int);

INSERT INTO AcademicYears
    (YearLabel, StartDate, EndDate, IsActive, IsDeleted, CreatedAt, InstituteId, CampusId)
OUTPUT inserted.AcademicYearId, inserted.InstituteId INTO @Year
SELECT N'2026-2027', '2026-04-01', '2027-03-31', 1, 0, @now, InstituteId, NULL
FROM @Inst;

/* ── 5. Classes: Class 1..3, Section A, per campus ─────────────────────── */
DECLARE @Cls TABLE (ClassId int, ClassName nvarchar(100), InstituteId int, CampusId int);

INSERT INTO Classes
    (ClassName, Section, AcademicYearId, IsActive, IsDeleted, CreatedAt, InstituteId, CampusId)
OUTPUT inserted.ClassId, inserted.ClassName, inserted.InstituteId, inserted.CampusId INTO @Cls
SELECT N'Class ' + CAST(lvl.n AS nvarchar(2)), N'A', y.AcademicYearId, 1, 0, @now,
       c.InstituteId, c.CampusId
FROM @Camp c
JOIN @Year y ON y.InstituteId = c.InstituteId
CROSS JOIN (VALUES (1),(2),(3)) lvl(n);

/* ── 6. Fee types per school ───────────────────────────────────────────── */
-- FeeCategory: 0=Recurring, 1=OneTime, 2=OnDemand, 3=RefundableDeposit
DECLARE @FT TABLE (FeeTypeId int, Name nvarchar(100), InstituteId int);

INSERT INTO FeeTypes
    (Name, Description, FeeCategory, IsActive, IsDeleted, CreatedAt, InstituteId, CampusId)
OUTPUT inserted.FeeTypeId, inserted.Name, inserted.InstituteId INTO @FT
SELECT t.Name, t.Descr, t.Cat, 1, 0, @now, i.InstituteId, NULL
FROM @Inst i
CROSS JOIN (VALUES
 (N'Tuition Fee',      N'Monthly tuition fee',                       0),
 (N'Admission Fee',    N'One-time admission fee',                    1),
 (N'Exam Fee',         N'Charged per exam term',                     2),
 (N'Security Deposit', N'Refundable security deposit on admission',  3)
) t(Name, Descr, Cat);

/* ── 7. Fee structures per class (DueDay = structure title used by UI) ─── */
INSERT INTO FeeStructures
    (FeeTypeId, ClassId, AcademicYearId, Amount, DueDay, IsActive,
     IsDeleted, CreatedAt, InstituteId, CampusId)
SELECT ft.FeeTypeId, c.ClassId, y.AcademicYearId,
       CASE ft.Name
            WHEN N'Tuition Fee'      THEN 2000 + CAST(RIGHT(c.ClassName,1) AS int) * 500
                                          + (c.InstituteId % 3) * 250   -- vary a bit per school
            WHEN N'Admission Fee'    THEN 10000
            WHEN N'Exam Fee'         THEN 1500
            WHEN N'Security Deposit' THEN 5000
       END,
       c.ClassName + N' Fee Structure 2026-27',
       1, 0, @now, c.InstituteId, c.CampusId
FROM @Cls c
JOIN @Year y ON y.InstituteId = c.InstituteId
JOIN @FT  ft ON ft.InstituteId = c.InstituteId;

/* ── 8. Students: 5 per class, admission no ADM-2026-#### per school ───── */
CREATE TABLE #StuSeed (
    AdmissionNo nvarchar(50), FirstName nvarchar(100), LastName nvarchar(100),
    Gender nvarchar(20), DateOfBirth date, ClassId int, InstituteId int, CampusId int);

INSERT INTO #StuSeed
SELECT N'ADM-2026-' + RIGHT('0000' + CAST(ROW_NUMBER() OVER
           (PARTITION BY c.InstituteId ORDER BY c.CampusId, c.ClassId, n.n) AS varchar(4)), 4),
       CASE WHEN n.n % 2 = 1
            THEN CHOOSE((c.ClassId + n.n) % 10 + 1, N'Ahmed', N'Ali', N'Hassan', N'Usman', N'Bilal',
                                                    N'Hamza', N'Zain', N'Umar', N'Ibrahim', N'Rayyan')
            ELSE CHOOSE((c.ClassId + n.n) % 10 + 1, N'Ayesha', N'Fatima', N'Zainab', N'Maryam', N'Hira',
                                                    N'Amna', N'Khadija', N'Noor', N'Iqra', N'Mahnoor')
       END,
       CHOOSE((c.ClassId * 3 + n.n) % 10 + 1, N'Khan', N'Ahmed', N'Malik', N'Butt', N'Sheikh',
                                              N'Raza', N'Qureshi', N'Chaudhry', N'Awan', N'Mirza'),
       CASE WHEN n.n % 2 = 1 THEN N'Male' ELSE N'Female' END,
       DATEADD(DAY, (c.ClassId * 17 + n.n * 61) % 300,
               DATEFROMPARTS(2020 - CAST(RIGHT(c.ClassName,1) AS int), 1, 1)),
       c.ClassId, c.InstituteId, c.CampusId
FROM @Cls c
CROSS JOIN (VALUES (1),(2),(3),(4),(5)) n(n);

DECLARE @Stu TABLE (StudentId int, AdmissionNo nvarchar(50), InstituteId int);

INSERT INTO Students
    (AdmissionNo, FirstName, LastName, DateOfBirth, Gender, Religion, Nationality,
     Address, Phone, AdmissionDate, IsActive, IsDeleted, CreatedAt, InstituteId, CampusId)
OUTPUT inserted.StudentId, inserted.AdmissionNo, inserted.InstituteId INTO @Stu
SELECT AdmissionNo, FirstName, LastName, DateOfBirth, Gender, N'Islam', N'Pakistani',
       N'House ' + RIGHT(AdmissionNo, 4) + N', Demo Street',
       N'0300-' + RIGHT('0000000' + CAST(ABS(CHECKSUM(AdmissionNo)) % 10000000 AS varchar(7)), 7),
       '2026-04-01', 1, 0, @now, InstituteId, CampusId
FROM #StuSeed;

/* ── 9. Guardians (father) ─────────────────────────────────────────────── */
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
JOIN #StuSeed ss ON ss.InstituteId = st.InstituteId AND ss.AdmissionNo = st.AdmissionNo;

/* ── 10. Active enrollment per student ─────────────────────────────────── */
INSERT INTO StudentClassEnrollments
    (StudentId, ClassId, AcademicYearId, EnrollmentDate, Status,
     IsDeleted, CreatedAt, InstituteId, CampusId)
SELECT st.StudentId, ss.ClassId, y.AcademicYearId, '2026-04-01', N'Active',
       0, @now, ss.InstituteId, ss.CampusId
FROM @Stu st
JOIN #StuSeed ss ON ss.InstituteId = st.InstituteId AND ss.AdmissionNo = st.AdmissionNo
JOIN @Year y ON y.InstituteId = ss.InstituteId;

DROP TABLE #StuSeed;

COMMIT;

/* ── Summary ───────────────────────────────────────────────────────────── */
SELECT i.Name AS School,
       (SELECT COUNT(*) FROM Campuses  c WHERE c.InstituteId = i.InstituteId AND c.IsDeleted = 0) AS Campuses,
       (SELECT COUNT(*) FROM Classes   c WHERE c.InstituteId = i.InstituteId AND c.IsDeleted = 0) AS Classes,
       (SELECT COUNT(*) FROM Students  s WHERE s.InstituteId = i.InstituteId AND s.IsDeleted = 0) AS Students,
       (SELECT COUNT(*) FROM FeeStructures f WHERE f.InstituteId = i.InstituteId AND f.IsDeleted = 0) AS FeeStructures,
       i.ChallanTemplate
FROM Institutes i
WHERE i.Name IN (N'The Educators School System', N'Beacon Light Grammar School', N'Al-Noor Public School');
