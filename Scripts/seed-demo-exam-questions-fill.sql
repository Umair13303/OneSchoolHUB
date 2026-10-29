/* ============================================================================
   Top-up demo exam papers to full section question counts, and seed
   Question Bank items (MCQ / T-F / Fill / Short / Long) for Maths topics.

   Prerequisite: seed-demo-exam-papers.sql (papers already exist)

   Idempotent: only inserts missing questions by SortOrder / bank text.
   Run:
     sqlcmd -S localhost -d OneSchoolDB -E -C -i seed-demo-exam-questions-fill.sql
   ============================================================================ */

SET NOCOUNT ON;
SET XACT_ABORT ON;
SET QUOTED_IDENTIFIER ON;

DECLARE @now datetime2 = SYSUTCDATETIME();

BEGIN TRAN;

/* ── Helper: insert MCQ if SortOrder missing on section ─────────────────── */
DECLARE @Papers TABLE (
    ExamPaperId int PRIMARY KEY,
    Kind nvarchar(10),
    InstituteId int,
    CampusId int,
    TeacherId int
);

INSERT INTO @Papers (ExamPaperId, Kind, InstituteId, CampusId, TeacherId)
SELECT p.ExamPaperId,
       CASE WHEN p.TotalMarks = 50 THEN N'50' ELSE N'100' END,
       p.InstituteId, p.CampusId, p.CreatedByUserId
FROM ExamPapers p
WHERE p.IsDeleted = 0
  AND p.Title IN (
      N'Mid-Term - Mathematics - Class 3A (50 Marks)',
      N'Monthly Test - Mathematics - Class 3A (100 Marks)'
  );

DECLARE @McqBank TABLE (
    SortOrder int PRIMARY KEY,
    QText nvarchar(500),
    OptA nvarchar(200), OptB nvarchar(200), OptC nvarchar(200), OptD nvarchar(200),
    Correct char(1)
);

INSERT INTO @McqBank VALUES
 (1,  N'What is 25 + 37?',                          N'52',  N'62',  N'72',  N'82',  N'B'),
 (2,  N'Which number is even?',                     N'13',  N'21',  N'28',  N'35',  N'C'),
 (3,  N'What is 9 x 6?',                            N'45',  N'54',  N'56',  N'63',  N'B'),
 (4,  N'How many sides does a triangle have?',      N'2',   N'3',   N'4',   N'5',   N'B'),
 (5,  N'What is 100 - 45?',                         N'45',  N'55',  N'65',  N'75',  N'B'),
 (6,  N'What is the place value of 7 in 704?',      N'7',   N'70',  N'700', N'7000',N'C'),
 (7,  N'Half of 48 is:',                            N'12',  N'18',  N'24',  N'36',  N'C'),
 (8,  N'Which fraction is equal to one half?',      N'1/3', N'2/4', N'2/3', N'3/4', N'B'),
 (9,  N'What is 15 + 26?',                          N'31',  N'41',  N'51',  N'40',  N'B'),
 (10, N'7 tens and 3 ones make:',                   N'37',  N'73',  N'703', N'370', N'B'),
 (11, N'Which is the largest number?',              N'89',  N'98',  N'79',  N'97',  N'B'),
 (12, N'What is 8 x 7?',                            N'54',  N'56',  N'63',  N'64',  N'B'),
 (13, N'A square has how many equal sides?',        N'2',   N'3',   N'4',   N'5',   N'C'),
 (14, N'What is 90 / 9?',                           N'8',   N'9',   N'10',  N'11',  N'C'),
 (15, N'Odd number among these is:',                N'12',  N'18',  N'21',  N'30',  N'C'),
 (16, N'10 hundreds equal:',                        N'10',  N'100', N'1000',N'10000',N'C'),
 (17, N'Perimeter of equilateral triangle side 4:', N'8',   N'12',  N'16',  N'4',   N'B'),
 (18, N'3/4 of 20 is:',                             N'10',  N'12',  N'15',  N'16',  N'C'),
 (19, N'Successor of 299 is:',                      N'298', N'300', N'301', N'399', N'B'),
 (20, N'Which shape has no corners?',               N'Square', N'Triangle', N'Circle', N'Rectangle', N'C');

DECLARE @TfBank TABLE (SortOrder int PRIMARY KEY, QText nvarchar(500), IsTrue bit);
INSERT INTO @TfBank VALUES
 (101, N'Every even number is divisible by 2.', 1),
 (102, N'A triangle has 4 sides.', 0),
 (103, N'Zero is an odd number.', 0),
 (104, N'1000 has 4 digits.', 1);

DECLARE @FillBank TABLE (SortOrder int PRIMARY KEY, QText nvarchar(500), Answer nvarchar(200));
INSERT INTO @FillBank VALUES
 (201, N'5 x 5 = ____', N'25'),
 (202, N'The number after 99 is ____', N'100'),
 (203, N'A week has ____ days.', N'7'),
 (204, N'12 + ____ = 20', N'8');

DECLARE @ShortBank TABLE (SortOrder int PRIMARY KEY, QText nvarchar(500), Answer nvarchar(300));
INSERT INTO @ShortBank VALUES
 (1, N'Write the number name for 345.', N'Three hundred forty-five'),
 (2, N'Solve: 48 / 6 = ?', N'8'),
 (3, N'Find the perimeter of a square with side 5 cm.', N'20 cm'),
 (4, N'Arrange in ascending order: 92, 29, 59, 72.', N'29, 59, 72, 92'),
 (5, N'Convert 3 metres into centimetres.', N'300 cm'),
 (6, N'What is the next even number after 46?', N'48'),
 (7, N'Write 508 in expanded form.', N'500 + 0 + 8'),
 (8, N'Find the difference: 200 - 75.', N'125'),
 (9, N'How many minutes are there in 2 hours?', N'120 minutes'),
 (10, N'Round 67 to the nearest ten.', N'70'),
 (11, N'Write the smallest 3-digit number.', N'100'),
 (12, N'Add: 234 + 145.', N'379');

DECLARE @LongBank TABLE (SortOrder int PRIMARY KEY, QText nvarchar(900), Answer nvarchar(400));
INSERT INTO @LongBank VALUES
 (1, N'A shopkeeper sold 125 apples on Monday and 178 apples on Tuesday. How many apples did he sell in both days? Show your working.', N'125 + 178 = 303 apples'),
 (2, N'Draw a rectangle of length 6 cm and breadth 4 cm. Find its area and perimeter.', N'Area = 24 cm2, Perimeter = 20 cm'),
 (3, N'Explain with an example how multiplication is repeated addition. Use 4 x 3.', N'4 x 3 = 4 + 4 + 4 = 12'),
 (4, N'Ali has 250 rupees. He buys a book for 175 rupees. How much money is left? Show steps.', N'250 - 175 = 75 rupees'),
 (5, N'Make a word problem for 36 / 4 and solve it.', N'e.g. 36 sweets shared among 4 children = 9 each');

DECLARE
    @paperId int, @kind nvarchar(10), @s int, @camp int, @tid int,
    @secA int, @secB int, @secC int, @needA int, @needB int, @needC int,
    @qId int, @ord int, @qt nvarchar(900), @a nvarchar(200), @b nvarchar(200),
    @c nvarchar(200), @d nvarchar(200), @corr char(1), @ans nvarchar(400),
    @isTrue bit, @shortMarks int, @have int;

DECLARE paper_cur CURSOR LOCAL FAST_FORWARD FOR
    SELECT ExamPaperId, Kind, InstituteId, CampusId, TeacherId FROM @Papers;

OPEN paper_cur;
FETCH NEXT FROM paper_cur INTO @paperId, @kind, @s, @camp, @tid;

WHILE @@FETCH_STATUS = 0
BEGIN
    SELECT @secA = ExamPaperSectionId, @needA = ISNULL(TotalQuestions, 10)
    FROM ExamPaperSections WHERE ExamPaperId = @paperId AND SortOrder = 1 AND IsDeleted = 0;
    SELECT @secB = ExamPaperSectionId, @needB = ISNULL(TotalQuestions, 8)
    FROM ExamPaperSections WHERE ExamPaperId = @paperId AND SortOrder = 2 AND IsDeleted = 0;
    SELECT @secC = ExamPaperSectionId, @needC = ISNULL(TotalQuestions, 4)
    FROM ExamPaperSections WHERE ExamPaperId = @paperId AND SortOrder = 3 AND IsDeleted = 0;

    SET @shortMarks = CASE WHEN @kind = N'50' THEN 4 ELSE 5 END;

    /* ---- Section A: first fill MCQs 1..N, then T/F, then Fill ---- */
    DECLARE mcq_cur CURSOR LOCAL FAST_FORWARD FOR
        SELECT SortOrder, QText, OptA, OptB, OptC, OptD, Correct
        FROM @McqBank
        WHERE SortOrder <= @needA
        ORDER BY SortOrder;
    OPEN mcq_cur;
    FETCH NEXT FROM mcq_cur INTO @ord, @qt, @a, @b, @c, @d, @corr;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF NOT EXISTS (
            SELECT 1 FROM ExamQuestions q
            WHERE q.ExamPaperId = @paperId AND q.ExamPaperSectionId = @secA
              AND q.SortOrder = @ord AND q.IsDeleted = 0
        )
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
        END
        FETCH NEXT FROM mcq_cur INTO @ord, @qt, @a, @b, @c, @d, @corr;
    END
    CLOSE mcq_cur; DEALLOCATE mcq_cur;

    /* If section A still short (shouldn't for 50/100 with 10/20 MCQs), skip.
       Also replace last few of 100-mark A with T/F + Fill for variety when
       MCQ slots already filled — add T/F at SortOrder 21+ only if needA > 20.
       For demo variety on 50-mark: convert nothing; add 2 T/F + 2 Fill as
       extra only when TotalQuestions allows. For 50 we need exactly 10 MCQs.
       For 100 we need 20 MCQs. Done above. */

    /* Extra variety questions: swap is hard; instead append T/F & Fill on
       papers that already have full MCQ count under unused SortOrders only
       if TotalQuestions was higher. Skip — keep section A pure MCQ to match marks. */

    /* ---- Section B short ---- */
    DECLARE short_cur CURSOR LOCAL FAST_FORWARD FOR
        SELECT SortOrder, QText, Answer FROM @ShortBank
        WHERE SortOrder <= @needB ORDER BY SortOrder;
    OPEN short_cur;
    FETCH NEXT FROM short_cur INTO @ord, @qt, @ans;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF NOT EXISTS (
            SELECT 1 FROM ExamQuestions q
            WHERE q.ExamPaperId = @paperId AND q.ExamPaperSectionId = @secB
              AND q.SortOrder = @ord AND q.IsDeleted = 0
        )
        BEGIN
            INSERT INTO ExamQuestions
                (ExamPaperId, ExamPaperSectionId, QuestionType, QuestionText, Language,
                 Marks, SortOrder, CorrectAnswer, IsTrue, QuestionNote, CourseTopicId,
                 IsDeleted, CreatedBy, CreatedAt, InstituteId, CampusId)
            VALUES
                (@paperId, @secB, 4, @qt, N'en',
                 @shortMarks, @ord, @ans, NULL, NULL, NULL,
                 0, @tid, @now, @s, @camp);
        END
        FETCH NEXT FROM short_cur INTO @ord, @qt, @ans;
    END
    CLOSE short_cur; DEALLOCATE short_cur;

    /* ---- Section C long ---- */
    DECLARE long_cur CURSOR LOCAL FAST_FORWARD FOR
        SELECT SortOrder, QText, Answer FROM @LongBank
        WHERE SortOrder <= @needC ORDER BY SortOrder;
    OPEN long_cur;
    FETCH NEXT FROM long_cur INTO @ord, @qt, @ans;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF NOT EXISTS (
            SELECT 1 FROM ExamQuestions q
            WHERE q.ExamPaperId = @paperId AND q.ExamPaperSectionId = @secC
              AND q.SortOrder = @ord AND q.IsDeleted = 0
        )
        BEGIN
            INSERT INTO ExamQuestions
                (ExamPaperId, ExamPaperSectionId, QuestionType, QuestionText, Language,
                 Marks, SortOrder, CorrectAnswer, IsTrue, QuestionNote, CourseTopicId,
                 IsDeleted, CreatedBy, CreatedAt, InstituteId, CampusId)
            VALUES
                (@paperId, @secC, 5, @qt, N'en',
                 10, @ord, @ans, NULL, N'Attempt as per section note', NULL,
                 0, @tid, @now, @s, @camp);
        END
        FETCH NEXT FROM long_cur INTO @ord, @qt, @ans;
    END
    CLOSE long_cur; DEALLOCATE long_cur;

    /* Add T/F + Fill-in on Section A as additional demo types using
       SortOrders 91-94 / 95-98 only if not present — teachers can see types
       in Question Bank design UI. Keep marks = 1. Do not exceed allocated
       count visually: we insert only when current count < needA was already
       satisfied; so put them as SortOrder high but still under need if gaps.
       Better: for paper 50, replace none. Insert T/F/Fill into Section A
       at orders that are empty after MCQs when needA allows.

       For 50-mark: MCQs 1-10 fill needA. For 100: 1-20 fill needA.
       Variety goes to Question Bank instead. */

    FETCH NEXT FROM paper_cur INTO @paperId, @kind, @s, @camp, @tid;
END
CLOSE paper_cur; DEALLOCATE paper_cur;

/* ── Question Bank dummy items (Educators Maths topics 1-4) ─────────────── */
DECLARE @inst1 int = 1;
DECLARE @admin1 int = (SELECT TOP 1 UserId FROM Users WHERE InstituteId = 1 AND Email = N'admin@educators.com' AND IsDeleted = 0);

IF @admin1 IS NOT NULL
BEGIN
    DECLARE @BankSeed TABLE (
        TopicId int,
        QType int,
        QText nvarchar(500),
        Marks int,
        CorrectAnswer nvarchar(300),
        IsTrue bit,
        OptA nvarchar(200), OptB nvarchar(200), OptC nvarchar(200), OptD nvarchar(200),
        Correct char(1)
    );

    INSERT INTO @BankSeed (TopicId, QType, QText, Marks, CorrectAnswer, IsTrue, OptA, OptB, OptC, OptD, Correct) VALUES
     (1, 1, N'What is the smallest 2-digit number?', 1, NULL, NULL, N'1', N'10', N'11', N'99', N'B'),
     (1, 2, N'Zero is a whole number.', 1, NULL, 1, NULL, NULL, NULL, NULL, NULL),
     (1, 3, N'9 + 1 = ____', 1, N'10', NULL, NULL, NULL, NULL, NULL, NULL),
     (1, 4, N'Write any five odd numbers less than 20.', 2, N'1,3,5,7,9 (example)', NULL, NULL, NULL, NULL, NULL, NULL),
     (2, 1, N'In 458, the digit in tens place is:', 1, NULL, NULL, N'4', N'5', N'8', N'0', N'B'),
     (2, 4, N'Write the place value of 6 in 165.', 2, N'60', NULL, NULL, NULL, NULL, NULL, NULL),
     (2, 3, N'400 + 50 + 3 = ____', 1, N'453', NULL, NULL, NULL, NULL, NULL, NULL),
     (3, 1, N'Which is greater: 789 or 798?', 1, NULL, NULL, N'789', N'798', N'Equal', N'Cannot say', N'B'),
     (3, 2, N'999 is greater than 1000.', 1, NULL, 0, NULL, NULL, NULL, NULL, NULL),
     (3, 5, N'Compare 345 and 354 using <, > or =. Explain your answer in one sentence.', 5, N'345 < 354 because tens digit 4 < 5', NULL, NULL, NULL, NULL, NULL, NULL),
     (4, 1, N'67 rounded to nearest ten is:', 1, NULL, NULL, N'60', N'70', N'67', N'80', N'B'),
     (4, 4, N'Round 142 to the nearest hundred.', 2, N'100', NULL, NULL, NULL, NULL, NULL, NULL),
     (13, 1, N'Which word is a noun?', 1, NULL, NULL, N'run', N'happy', N'school', N'quickly', N'C'),
     (14, 1, N'Which word is a pronoun?', 1, NULL, NULL, N'Ali', N'they', N'book', N'city', N'B'),
     (14, 4, N'Replace the noun with a pronoun: Sara is reading. ____ is reading.', 2, N'She', NULL, NULL, NULL, NULL, NULL, NULL),
     (15, 2, N'Verbs show action or state.', 1, NULL, 1, NULL, NULL, NULL, NULL, NULL);

    DECLARE @topicId int, @qType int, @marks int, @corrAns nvarchar(300), @bankId int;
    DECLARE bank_cur CURSOR LOCAL FAST_FORWARD FOR
        SELECT TopicId, QType, QText, Marks, CorrectAnswer, IsTrue, OptA, OptB, OptC, OptD, Correct
        FROM @BankSeed;
    OPEN bank_cur;
    FETCH NEXT FROM bank_cur INTO @topicId, @qType, @qt, @marks, @corrAns, @isTrue, @a, @b, @c, @d, @corr;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF EXISTS (SELECT 1 FROM CourseTopics t WHERE t.CourseTopicId = @topicId AND t.IsDeleted = 0)
           AND NOT EXISTS (
               SELECT 1 FROM QuestionBankItems q
               WHERE q.InstituteId = @inst1 AND q.IsDeleted = 0 AND q.QuestionText = @qt
           )
        BEGIN
            INSERT INTO QuestionBankItems
                (CourseTopicId, QuestionType, QuestionText, Language, Marks,
                 CorrectAnswer, IsTrue, QuestionNote, IsActive,
                 IsDeleted, CreatedBy, CreatedAt, InstituteId, CampusId)
            VALUES
                (@topicId, @qType, @qt, N'en', @marks,
                 @corrAns, @isTrue, NULL, 1,
                 0, @admin1, @now, @inst1, NULL);
            SET @bankId = SCOPE_IDENTITY();

            IF @qType = 1 AND @a IS NOT NULL
            BEGIN
                INSERT INTO QuestionBankOptions
                    (QuestionBankItemId, OptionLabel, OptionText, IsCorrect, SortOrder,
                     IsDeleted, CreatedBy, CreatedAt, InstituteId, CampusId)
                VALUES
                    (@bankId, N'A', @a, CASE WHEN @corr = N'A' THEN 1 ELSE 0 END, 1, 0, @admin1, @now, @inst1, NULL),
                    (@bankId, N'B', @b, CASE WHEN @corr = N'B' THEN 1 ELSE 0 END, 2, 0, @admin1, @now, @inst1, NULL),
                    (@bankId, N'C', @c, CASE WHEN @corr = N'C' THEN 1 ELSE 0 END, 3, 0, @admin1, @now, @inst1, NULL),
                    (@bankId, N'D', @d, CASE WHEN @corr = N'D' THEN 1 ELSE 0 END, 4, 0, @admin1, @now, @inst1, NULL);
            END
        END
        FETCH NEXT FROM bank_cur INTO @topicId, @qType, @qt, @marks, @corrAns, @isTrue, @a, @b, @c, @d, @corr;
    END
    CLOSE bank_cur; DEALLOCATE bank_cur;
END

COMMIT;

/* Summary */
SELECT p.ExamPaperId, p.Title, p.TotalMarks,
       s.SectionName, s.TotalQuestions AS Target,
       (SELECT COUNT(*) FROM ExamQuestions q WHERE q.ExamPaperSectionId = s.ExamPaperSectionId AND q.IsDeleted = 0) AS Have
FROM ExamPapers p
JOIN ExamPaperSections s ON s.ExamPaperId = p.ExamPaperId AND s.IsDeleted = 0
WHERE p.InstituteId = 1 AND p.IsDeleted = 0
  AND p.Title IN (
      N'Mid-Term - Mathematics - Class 3A (50 Marks)',
      N'Monthly Test - Mathematics - Class 3A (100 Marks)'
  )
ORDER BY p.ExamPaperId, s.SortOrder;

SELECT QuestionType, COUNT(*) AS Cnt
FROM QuestionBankItems
WHERE InstituteId = 1 AND IsDeleted = 0
GROUP BY QuestionType
ORDER BY QuestionType;

SELECT COUNT(*) AS BankTotal FROM QuestionBankItems WHERE InstituteId = 1 AND IsDeleted = 0;
SELECT COUNT(*) AS BankOptions FROM QuestionBankOptions WHERE InstituteId = 1 AND IsDeleted = 0;
