/* ============================================================================
   Demo seed: sample exam papers (50 + 100 marks) with A/B/C sections,
   MCQ / short / long questions, and MCQ options.

   Prerequisite: seed-demo-schools.sql + seed-demo-teachers-timetable.sql

   Targets Main Campus of each demo school:
     Class 3-A · Mathematics · teacher t.math@…

   Papers per school:
     1) Mid-Term - Mathematics - Class 3A   (50 marks, draft, with questions)
     2) Monthly Test - Mathematics - Class 3A (100 marks, published, with questions)

   Idempotent: skips a paper if the same Title already exists for that school.
   Run:
     sqlcmd -S localhost -d OneSchoolDB -E -C -i seed-demo-exam-papers.sql
   ============================================================================ */

SET NOCOUNT ON;
SET XACT_ABORT ON;
SET QUOTED_IDENTIFIER ON;

DECLARE @now datetime2 = SYSUTCDATETIME();

DECLARE @Schools TABLE (
    InstituteId    int PRIMARY KEY,
    Name           nvarchar(200),
    Domain         nvarchar(50),
    AcademicYearId int,
    CampusId       int,
    ClassId        int,
    SubjectId      int,
    TeacherId      int
);

INSERT INTO @Schools (InstituteId, Name, Domain, AcademicYearId, CampusId, ClassId, SubjectId, TeacherId)
SELECT
    i.InstituteId,
    i.Name,
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
    ),
    c.CampusId,
    cl.ClassId,
    sj.SubjectId,
    u.UserId
FROM Institutes i
CROSS APPLY (
    SELECT TOP 1 camp.CampusId
    FROM Campuses camp
    WHERE camp.InstituteId = i.InstituteId AND camp.IsDeleted = 0
    ORDER BY camp.CampusId
) c
JOIN Classes cl
  ON cl.InstituteId = i.InstituteId
 AND cl.CampusId = c.CampusId
 AND cl.ClassName = N'Class 3'
 AND cl.Section = N'A'
 AND cl.IsDeleted = 0
JOIN Subjects sj
  ON sj.InstituteId = i.InstituteId
 AND sj.SubjectName = N'Mathematics'
 AND sj.IsDeleted = 0
JOIN Users u
  ON u.InstituteId = i.InstituteId
 AND u.CampusId = c.CampusId
 AND u.IsDeleted = 0
 AND u.Email = N't.math@' + CASE i.Name
        WHEN N'The Educators School System' THEN N'educators.com'
        WHEN N'Beacon Light Grammar School' THEN N'beaconlight.com'
        WHEN N'Al-Noor Public School'       THEN N'alnoor.com'
    END
WHERE i.IsDeleted = 0
  AND i.Name IN (N'The Educators School System',
                 N'Beacon Light Grammar School',
                 N'Al-Noor Public School');

IF NOT EXISTS (SELECT 1 FROM @Schools)
BEGIN
    RAISERROR('No demo school/class/subject/teacher found. Run school + teacher seeds first.', 16, 1);
    RETURN;
END

BEGIN TRAN;

DECLARE @PaperMap TABLE (
    InstituteId int,
    Kind        nvarchar(20),   -- '50' | '100'
    ExamPaperId int,
    PRIMARY KEY (InstituteId, Kind)
);

/* ── Papers ─────────────────────────────────────────────────────────────── */
DECLARE @s int, @ay int, @camp int, @cls int, @subj int, @tid int, @dom nvarchar(50);
DECLARE @paperId int;

DECLARE school_cur CURSOR LOCAL FAST_FORWARD FOR
    SELECT InstituteId, AcademicYearId, CampusId, ClassId, SubjectId, TeacherId, Domain
    FROM @Schools;

OPEN school_cur;
FETCH NEXT FROM school_cur INTO @s, @ay, @camp, @cls, @subj, @tid, @dom;

WHILE @@FETCH_STATUS = 0
BEGIN
    /* 50-mark Mid-Term (draft) */
    IF NOT EXISTS (
        SELECT 1 FROM ExamPapers p
        WHERE p.InstituteId = @s AND p.IsDeleted = 0
          AND p.Title = N'Mid-Term - Mathematics - Class 3A (50 Marks)'
    )
    BEGIN
        INSERT INTO ExamPapers
            (AcademicYearId, ClassId, SubjectId, CreatedByUserId,
             ExamType, ClassGroup, Title, TotalMarks, PassMarks,
             DurationMinutes, Instructions, SyllabusNote,
             IsDraft, IsLocked, IsDeleted, CreatedBy, CreatedAt, InstituteId, CampusId)
        VALUES
            (@ay, @cls, @subj, @tid,
             3, 1, N'Mid-Term - Mathematics - Class 3A (50 Marks)', 50, 20,
             90,
             N'Read all questions carefully. Calculators are not allowed. Attempt questions as instructed in each section.',
             N'Chapters 1-5 (Numbers, Addition, Subtraction, Multiplication, Division)',
             1, 0, 0, @tid, @now, @s, @camp);

        SET @paperId = SCOPE_IDENTITY();
        INSERT INTO @PaperMap VALUES (@s, N'50', @paperId);

        INSERT INTO ExamPaperSections
            (ExamPaperId, SectionName, SectionType, AllocatedMarks,
             TotalQuestions, AttemptQuestions, MarksPerQuestion, SectionNote,
             SortOrder, IsDeleted, CreatedBy, CreatedAt, InstituteId, CampusId)
        VALUES
            (@paperId, N'Section A', N'Objective',   10, 10, NULL, 1,  N'All MCQs compulsory',      1, 0, @tid, @now, @s, @camp),
            (@paperId, N'Section B', N'ShortAnswer', 20,  8,    5, 4,  N'Attempt any 5 questions',  2, 0, @tid, @now, @s, @camp),
            (@paperId, N'Section C', N'LongAnswer',  20,  4,    2, 10, N'Attempt any 2 questions',  3, 0, @tid, @now, @s, @camp);
    END

    /* 100-mark Monthly Test (published) */
    IF NOT EXISTS (
        SELECT 1 FROM ExamPapers p
        WHERE p.InstituteId = @s AND p.IsDeleted = 0
          AND p.Title = N'Monthly Test - Mathematics - Class 3A (100 Marks)'
    )
    BEGIN
        INSERT INTO ExamPapers
            (AcademicYearId, ClassId, SubjectId, CreatedByUserId,
             ExamType, ClassGroup, Title, TotalMarks, PassMarks,
             DurationMinutes, Instructions, SyllabusNote,
             IsDraft, IsLocked, IsDeleted, CreatedBy, CreatedAt, InstituteId, CampusId)
        VALUES
            (@ay, @cls, @subj, @tid,
             2, 1, N'Monthly Test - Mathematics - Class 3A (100 Marks)', 100, 40,
             180,
             N'Time allowed: 3 hours. Section A is compulsory. Follow attempt rules for Sections B and C.',
             N'Full term syllabus - Chapters 1-8',
             0, 0, 0, @tid, @now, @s, @camp);

        SET @paperId = SCOPE_IDENTITY();
        INSERT INTO @PaperMap VALUES (@s, N'100', @paperId);

        INSERT INTO ExamPaperSections
            (ExamPaperId, SectionName, SectionType, AllocatedMarks,
             TotalQuestions, AttemptQuestions, MarksPerQuestion, SectionNote,
             SortOrder, IsDeleted, CreatedBy, CreatedAt, InstituteId, CampusId)
        VALUES
            (@paperId, N'Section A', N'Objective',   20, 20, NULL, 1,  N'All MCQs compulsory',      1, 0, @tid, @now, @s, @camp),
            (@paperId, N'Section B', N'ShortAnswer', 40, 12,    8, 5,  N'Attempt any 8 questions',  2, 0, @tid, @now, @s, @camp),
            (@paperId, N'Section C', N'LongAnswer',  40,  5,    4, 10, N'Attempt any 4 questions',  3, 0, @tid, @now, @s, @camp);
    END

    FETCH NEXT FROM school_cur INTO @s, @ay, @camp, @cls, @subj, @tid, @dom;
END

CLOSE school_cur;
DEALLOCATE school_cur;

/* ── Questions + options for newly inserted papers ─────────────────────── */
DECLARE @secA int, @secB int, @secC int, @qId int, @kind nvarchar(20);

DECLARE paper_cur CURSOR LOCAL FAST_FORWARD FOR
    SELECT pm.ExamPaperId, pm.Kind, pm.InstituteId, s.CampusId, s.TeacherId
    FROM @PaperMap pm
    JOIN @Schools s ON s.InstituteId = pm.InstituteId;

OPEN paper_cur;
FETCH NEXT FROM paper_cur INTO @paperId, @kind, @s, @camp, @tid;

WHILE @@FETCH_STATUS = 0
BEGIN
    SELECT @secA = ExamPaperSectionId FROM ExamPaperSections
     WHERE ExamPaperId = @paperId AND SortOrder = 1 AND IsDeleted = 0;
    SELECT @secB = ExamPaperSectionId FROM ExamPaperSections
     WHERE ExamPaperId = @paperId AND SortOrder = 2 AND IsDeleted = 0;
    SELECT @secC = ExamPaperSectionId FROM ExamPaperSections
     WHERE ExamPaperId = @paperId AND SortOrder = 3 AND IsDeleted = 0;

    /* Section A — MCQs (seed 5 for 50-mark, 8 for 100-mark) */
    DECLARE @mcq TABLE (
        SortOrder int PRIMARY KEY,
        QText nvarchar(500),
        OptA nvarchar(200), OptB nvarchar(200), OptC nvarchar(200), OptD nvarchar(200),
        Correct char(1)
    );
    DELETE FROM @mcq;

    INSERT INTO @mcq (SortOrder, QText, OptA, OptB, OptC, OptD, Correct) VALUES
     (1, N'What is 25 + 37?', N'52', N'62', N'72', N'82', N'B'),
     (2, N'Which number is even?', N'13', N'21', N'28', N'35', N'C'),
     (3, N'What is 9 × 6?', N'45', N'54', N'56', N'63', N'B'),
     (4, N'How many sides does a triangle have?', N'2', N'3', N'4', N'5', N'B'),
     (5, N'What is 100 − 45?', N'45', N'55', N'65', N'75', N'B');

    IF @kind = N'100'
    BEGIN
        INSERT INTO @mcq (SortOrder, QText, OptA, OptB, OptC, OptD, Correct) VALUES
         (6, N'What is the place value of 7 in 704?', N'7', N'70', N'700', N'7000', N'C'),
         (7, N'Half of 48 is:', N'12', N'18', N'24', N'36', N'C'),
         (8, N'Which fraction is equal to one half?', N'1/3', N'2/4', N'2/3', N'3/4', N'B');
    END

    DECLARE @ord int, @qt nvarchar(500), @a nvarchar(200), @b nvarchar(200), @c nvarchar(200), @d nvarchar(200), @corr char(1);
    DECLARE mcq_cur CURSOR LOCAL FAST_FORWARD FOR
        SELECT SortOrder, QText, OptA, OptB, OptC, OptD, Correct FROM @mcq ORDER BY SortOrder;
    OPEN mcq_cur;
    FETCH NEXT FROM mcq_cur INTO @ord, @qt, @a, @b, @c, @d, @corr;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        INSERT INTO ExamQuestions
            (ExamPaperId, ExamPaperSectionId, QuestionType, QuestionText, Language,
             Marks, SortOrder, CorrectAnswer, IsTrue, QuestionNote, CourseTopicId,
             IsDeleted, CreatedBy, CreatedAt, InstituteId, CampusId)
        VALUES
            (@paperId, @secA, 1, @qt, N'en',
             1, @ord, NULL, NULL, NULL, NULL,
             0, @tid, @now, @s, @camp);
        SET @qId = SCOPE_IDENTITY();

        INSERT INTO ExamQuestionOptions
            (ExamQuestionId, OptionLabel, OptionText, IsCorrect, SortOrder,
             IsDeleted, CreatedBy, CreatedAt, InstituteId, CampusId)
        VALUES
            (@qId, N'A', @a, CASE WHEN @corr = N'A' THEN 1 ELSE 0 END, 1, 0, @tid, @now, @s, @camp),
            (@qId, N'B', @b, CASE WHEN @corr = N'B' THEN 1 ELSE 0 END, 2, 0, @tid, @now, @s, @camp),
            (@qId, N'C', @c, CASE WHEN @corr = N'C' THEN 1 ELSE 0 END, 3, 0, @tid, @now, @s, @camp),
            (@qId, N'D', @d, CASE WHEN @corr = N'D' THEN 1 ELSE 0 END, 4, 0, @tid, @now, @s, @camp);

        FETCH NEXT FROM mcq_cur INTO @ord, @qt, @a, @b, @c, @d, @corr;
    END
    CLOSE mcq_cur;
    DEALLOCATE mcq_cur;

    /* Section B — Short questions */
    DECLARE @shortMarks int = CASE WHEN @kind = N'50' THEN 4 ELSE 5 END;
    INSERT INTO ExamQuestions
        (ExamPaperId, ExamPaperSectionId, QuestionType, QuestionText, Language,
         Marks, SortOrder, CorrectAnswer, IsTrue, QuestionNote, CourseTopicId,
         IsDeleted, CreatedBy, CreatedAt, InstituteId, CampusId)
    VALUES
        (@paperId, @secB, 4, N'Write the number name for 345.', N'en',
         @shortMarks, 1, N'Three hundred forty-five', NULL, NULL, NULL,
         0, @tid, @now, @s, @camp),
        (@paperId, @secB, 4, N'Solve: 48 ÷ 6 = ?', N'en',
         @shortMarks, 2, N'8', NULL, NULL, NULL,
         0, @tid, @now, @s, @camp),
        (@paperId, @secB, 4, N'Find the perimeter of a square with side 5 cm.', N'en',
         @shortMarks, 3, N'20 cm', NULL, NULL, NULL,
         0, @tid, @now, @s, @camp),
        (@paperId, @secB, 4, N'Arrange in ascending order: 92, 29, 59, 72.', N'en',
         @shortMarks, 4, N'29, 59, 72, 92', NULL, NULL, NULL,
         0, @tid, @now, @s, @camp);

    IF @kind = N'100'
    BEGIN
        INSERT INTO ExamQuestions
            (ExamPaperId, ExamPaperSectionId, QuestionType, QuestionText, Language,
             Marks, SortOrder, CorrectAnswer, IsTrue, QuestionNote, CourseTopicId,
             IsDeleted, CreatedBy, CreatedAt, InstituteId, CampusId)
        VALUES
            (@paperId, @secB, 4, N'Convert 3 metres into centimetres.', N'en',
             5, 5, N'300 cm', NULL, NULL, NULL,
             0, @tid, @now, @s, @camp),
            (@paperId, @secB, 4, N'What is the next even number after 46?', N'en',
             5, 6, N'48', NULL, NULL, NULL,
             0, @tid, @now, @s, @camp);
    END

    /* Section C — Long questions */
    INSERT INTO ExamQuestions
        (ExamPaperId, ExamPaperSectionId, QuestionType, QuestionText, Language,
         Marks, SortOrder, CorrectAnswer, IsTrue, QuestionNote, CourseTopicId,
         IsDeleted, CreatedBy, CreatedAt, InstituteId, CampusId)
    VALUES
        (@paperId, @secC, 5,
         N'A shopkeeper sold 125 apples on Monday and 178 apples on Tuesday. How many apples did he sell in both days? Show your working.',
         N'en', 10, 1,
         N'125 + 178 = 303 apples', NULL, N'Attempt any required as per section note', NULL,
         0, @tid, @now, @s, @camp),
        (@paperId, @secC, 5,
         N'Draw a rectangle of length 6 cm and breadth 4 cm. Find its area and perimeter.',
         N'en', 10, 2,
         N'Area = 24 cm², Perimeter = 20 cm', NULL, NULL, NULL,
         0, @tid, @now, @s, @camp);

    IF @kind = N'100'
    BEGIN
        INSERT INTO ExamQuestions
            (ExamPaperId, ExamPaperSectionId, QuestionType, QuestionText, Language,
             Marks, SortOrder, CorrectAnswer, IsTrue, QuestionNote, CourseTopicId,
             IsDeleted, CreatedBy, CreatedAt, InstituteId, CampusId)
        VALUES
            (@paperId, @secC, 5,
             N'Explain with an example how multiplication is repeated addition. Use 4 × 3.',
             N'en', 10, 3,
             N'4 × 3 = 4 + 4 + 4 = 12', NULL, NULL, NULL,
             0, @tid, @now, @s, @camp);
    END

    FETCH NEXT FROM paper_cur INTO @paperId, @kind, @s, @camp, @tid;
END

CLOSE paper_cur;
DEALLOCATE paper_cur;

COMMIT;

SELECT
    i.Name AS School,
    p.ExamPaperId,
    p.Title,
    p.TotalMarks,
    p.PassMarks,
    CASE WHEN p.IsDraft = 1 THEN N'Draft' ELSE N'Published' END AS Status,
    (SELECT COUNT(*) FROM ExamPaperSections s WHERE s.ExamPaperId = p.ExamPaperId AND s.IsDeleted = 0) AS Sections,
    (SELECT COUNT(*) FROM ExamQuestions q WHERE q.ExamPaperId = p.ExamPaperId AND q.IsDeleted = 0) AS Questions
FROM ExamPapers p
JOIN Institutes i ON i.InstituteId = p.InstituteId
WHERE p.IsDeleted = 0
  AND p.Title IN (
      N'Mid-Term - Mathematics - Class 3A (50 Marks)',
      N'Monthly Test - Mathematics - Class 3A (100 Marks)'
  )
ORDER BY i.Name, p.TotalMarks;
