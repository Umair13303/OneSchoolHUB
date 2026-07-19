using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SchoolManagement.API.Infrastructure;
using SchoolManagement.API.Models;

namespace SchoolManagement.API.Data;

public class AppDbContext : DbContext
{
    // Stored as a reference so the lambda captures the object, not a value.
    // EF Core re-evaluates the lambda each query, so reads are always current.
    private readonly ITenantContext _tenant;

    public AppDbContext(DbContextOptions<AppDbContext> options, ITenantContext? tenant = null) : base(options)
    {
        _tenant = tenant ?? NullTenantContext.Instance;
    }

    // Convenience helpers used inside query-filter lambdas.
    // Must be properties/methods (not captured locals) so EF can re-evaluate per query.
    private bool IsSuperAdmin => _tenant.IsSuperAdmin;
    private int? TenantId     => _tenant.InstituteId;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(w =>
            w.Ignore(RelationalEventId.PendingModelChangesWarning));
    }

    public DbSet<DevCompany> DevCompany => Set<DevCompany>();
    public DbSet<Institute> Institutes => Set<Institute>();
    public DbSet<Campus>    Campuses   => Set<Campus>();

    public DbSet<SchoolSettings> SchoolSettings => Set<SchoolSettings>();
    public DbSet<DaySchedule> DaySchedules => Set<DaySchedule>();
    public DbSet<ScheduleProfile> ScheduleProfiles => Set<ScheduleProfile>();
    public DbSet<ProfilePeriod>   ProfilePeriods   => Set<ProfilePeriod>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<MenuRolePermission> MenuRolePermissions => Set<MenuRolePermission>();
    public DbSet<AcademicYear> AcademicYears => Set<AcademicYear>();
    public DbSet<AcademicCalendarEvent> AcademicCalendarEvents => Set<AcademicCalendarEvent>();
    public DbSet<CalendarEventType> CalendarEventTypes => Set<CalendarEventType>();
    public DbSet<Class> Classes => Set<Class>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<ClassSubject> ClassSubjects => Set<ClassSubject>();
    public DbSet<FileStore> FileStores => Set<FileStore>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<StudentGuardian> StudentGuardians => Set<StudentGuardian>();
    public DbSet<StudentClassEnrollment> StudentClassEnrollments => Set<StudentClassEnrollment>();
    public DbSet<Period> Periods => Set<Period>();
    public DbSet<SchoolTimetable> SchoolTimetables => Set<SchoolTimetable>();
    public DbSet<TimetableSubstitution> TimetableSubstitutions => Set<TimetableSubstitution>();
    public DbSet<Attendance> Attendances => Set<Attendance>();
    public DbSet<Homework> Homeworks => Set<Homework>();
    public DbSet<HomeworkSubmission> HomeworkSubmissions => Set<HomeworkSubmission>();
    public DbSet<FeeType> FeeTypes => Set<FeeType>();
    public DbSet<FeeStructure> FeeStructures => Set<FeeStructure>();
    public DbSet<StudentFee> StudentFees => Set<StudentFee>();
    public DbSet<FeePayment> FeePayments => Set<FeePayment>();
    public DbSet<DiscountPolicy> DiscountPolicies => Set<DiscountPolicy>();
    public DbSet<StudentDiscount> StudentDiscounts => Set<StudentDiscount>();
    public DbSet<Staff> Staff => Set<Staff>();

    public DbSet<ChatConversation>       ChatConversations       => Set<ChatConversation>();
    public DbSet<ChatConversationMember> ChatConversationMembers => Set<ChatConversationMember>();
    public DbSet<ChatMessage>            ChatMessages            => Set<ChatMessage>();
    public DbSet<ChatMessageRead>        ChatMessageReads        => Set<ChatMessageRead>();

    // Activity logs live in the separate logging database — see LoggingDbContext.

    // Exam Module
    public DbSet<ExamPaper>          ExamPapers          => Set<ExamPaper>();
    public DbSet<ExamPaperSection>   ExamPaperSections   => Set<ExamPaperSection>();
    public DbSet<ExamSchedule>       ExamSchedules       => Set<ExamSchedule>();
    public DbSet<ExamResult>         ExamResults         => Set<ExamResult>();
    public DbSet<ExamQuestion>       ExamQuestions       => Set<ExamQuestion>();
    public DbSet<ExamQuestionOption> ExamQuestionOptions => Set<ExamQuestionOption>();

    // Inventory & POS Module
    public DbSet<UnitMaster>             UnitMasters             => Set<UnitMaster>();
    public DbSet<TaxMaster>              TaxMasters              => Set<TaxMaster>();
    public DbSet<ItemCategory>           ItemCategories          => Set<ItemCategory>();
    public DbSet<ItemMaster>             ItemMasters             => Set<ItemMaster>();
    public DbSet<Supplier>               Suppliers               => Set<Supplier>();
    public DbSet<PackageMaster>          PackageMasters          => Set<PackageMaster>();
    public DbSet<PackageDetail>          PackageDetails          => Set<PackageDetail>();
    public DbSet<PurchaseMaster>         PurchaseMasters         => Set<PurchaseMaster>();
    public DbSet<PurchaseDetail>         PurchaseDetails         => Set<PurchaseDetail>();
    public DbSet<PurchaseReturnMaster>   PurchaseReturnMasters   => Set<PurchaseReturnMaster>();
    public DbSet<PurchaseReturnDetail>   PurchaseReturnDetails   => Set<PurchaseReturnDetail>();
    public DbSet<SalesMaster>            SalesMasters            => Set<SalesMaster>();
    public DbSet<SalesDetail>            SalesDetails            => Set<SalesDetail>();
    public DbSet<SalesPayment>           SalesPayments           => Set<SalesPayment>();
    public DbSet<SalesReturnMaster>      SalesReturnMasters      => Set<SalesReturnMaster>();
    public DbSet<SalesReturnDetail>      SalesReturnDetails      => Set<SalesReturnDetail>();
    public DbSet<StockAdjustmentMaster>  StockAdjustmentMasters  => Set<StockAdjustmentMaster>();
    public DbSet<StockAdjustmentDetail>  StockAdjustmentDetails  => Set<StockAdjustmentDetail>();
    public DbSet<StockTransferMaster>    StockTransferMasters    => Set<StockTransferMaster>();
    public DbSet<StockTransferDetail>    StockTransferDetails    => Set<StockTransferDetail>();
    public DbSet<StockLedger>            StockLedgers            => Set<StockLedger>();
    public DbSet<CurrentStock>           CurrentStocks           => Set<CurrentStock>();
    public DbSet<InventorySettings>      InventorySettings       => Set<InventorySettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Global filters: soft-delete + tenant isolation.
        // Superadmin bypasses tenant filter (sees all schools).
        // All other users only see rows belonging to their InstituteId.
        // NOTE: null InstituteId is intentionally excluded from User rows so that
        // the global superadmin account does not appear in any institute's user list.
        modelBuilder.Entity<User>().HasQueryFilter(e =>
            !e.IsDeleted && (IsSuperAdmin || e.InstituteId == TenantId));
        modelBuilder.Entity<MenuItem>().HasQueryFilter(e => !e.IsDeleted);
        // Students belong to exactly one school — null InstituteId (legacy rows)
        // is visible only to superadmin, never shared across institutes.
        modelBuilder.Entity<Student>().HasQueryFilter(e =>
            !e.IsDeleted && (IsSuperAdmin || e.InstituteId == TenantId));
        // NOTE: unlike most tenant-scoped entities, null InstituteId is NOT treated as
        // "shared with every institute" here — a class/subject belongs to exactly one
        // institute (superadmin always assigns one on create) or is invisible to institute
        // users until assigned.
        modelBuilder.Entity<Class>().HasQueryFilter(e =>
            !e.IsDeleted && (IsSuperAdmin || e.InstituteId == TenantId));
        modelBuilder.Entity<Subject>().HasQueryFilter(e =>
            !e.IsDeleted && (IsSuperAdmin || e.InstituteId == TenantId));
        modelBuilder.Entity<AcademicYear>().HasQueryFilter(e =>
            IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId);
        modelBuilder.Entity<SchoolTimetable>().HasQueryFilter(e =>
            !e.IsDeleted && (IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId));
        modelBuilder.Entity<Attendance>().HasQueryFilter(e =>
            !e.IsDeleted && (IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId));
        modelBuilder.Entity<Homework>().HasQueryFilter(e =>
            !e.IsDeleted && (IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId));
        modelBuilder.Entity<Staff>().HasQueryFilter(e =>
            !e.IsDeleted && (IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId));
        modelBuilder.Entity<FeeType>().HasQueryFilter(f =>
            !f.IsDeleted && (IsSuperAdmin || f.InstituteId == null || f.InstituteId == TenantId));
        modelBuilder.Entity<FeeStructure>().HasQueryFilter(f =>
            !f.IsDeleted && (IsSuperAdmin || f.InstituteId == null || f.InstituteId == TenantId));
        modelBuilder.Entity<StudentFee>().HasQueryFilter(sf =>
            !sf.IsDeleted && (IsSuperAdmin || sf.InstituteId == null || sf.InstituteId == TenantId));
        modelBuilder.Entity<DiscountPolicy>().HasQueryFilter(d =>
            !d.IsDeleted && (IsSuperAdmin || d.InstituteId == null || d.InstituteId == TenantId));
        modelBuilder.Entity<StudentDiscount>().HasQueryFilter(sd =>
            !sd.IsDeleted && (IsSuperAdmin || sd.InstituteId == null || sd.InstituteId == TenantId));
        modelBuilder.Entity<ExamPaper>().HasQueryFilter(e =>
            IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId);
        modelBuilder.Entity<ExamSchedule>().HasQueryFilter(e =>
            IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId);
        modelBuilder.Entity<ExamResult>().HasQueryFilter(e =>
            IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId);
        modelBuilder.Entity<SchoolSettings>().HasQueryFilter(e =>
            IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId);
        modelBuilder.Entity<Period>().HasQueryFilter(e =>
            !e.IsDeleted && (IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId));
        modelBuilder.Entity<ScheduleProfile>().HasQueryFilter(e =>
            IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId);
        modelBuilder.Entity<AcademicCalendarEvent>().HasQueryFilter(e =>
            IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId);
        modelBuilder.Entity<FeePayment>().HasQueryFilter(e =>
            IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId);
        modelBuilder.Entity<ExamQuestion>().HasQueryFilter(e =>
            !e.IsDeleted && (IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId));

        // ── Tenant filters: entities with their own InstituteId column ────────
        // These were reachable without any filter, so a query using a
        // client-supplied id (e.g. GET sale/purchase detail rows) could read
        // another school's rows. Soft-delete stays explicit in the services.
        modelBuilder.Entity<CalendarEventType>().HasQueryFilter(e =>
            IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId);
        modelBuilder.Entity<ClassSubject>().HasQueryFilter(e =>
            IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId);
        modelBuilder.Entity<ExamPaperSection>().HasQueryFilter(e =>
            IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId);
        modelBuilder.Entity<StudentClassEnrollment>().HasQueryFilter(e =>
            IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId);
        modelBuilder.Entity<PackageDetail>().HasQueryFilter(e =>
            IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId);
        modelBuilder.Entity<PurchaseDetail>().HasQueryFilter(e =>
            IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId);
        modelBuilder.Entity<PurchaseReturnDetail>().HasQueryFilter(e =>
            IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId);
        modelBuilder.Entity<SalesDetail>().HasQueryFilter(e =>
            IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId);
        modelBuilder.Entity<SalesPayment>().HasQueryFilter(e =>
            IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId);
        modelBuilder.Entity<SalesReturnDetail>().HasQueryFilter(e =>
            IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId);
        modelBuilder.Entity<StockAdjustmentDetail>().HasQueryFilter(e =>
            IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId);
        modelBuilder.Entity<StockTransferDetail>().HasQueryFilter(e =>
            IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId);

        // ── Tenant filters via the parent (child has no InstituteId column) ──
        // Guardian rows are parent PII; submissions/periods/day-rows belong to
        // a filtered parent. The join keeps them invisible across schools even
        // when queried directly with a guessed id.
        modelBuilder.Entity<StudentGuardian>().HasQueryFilter(g =>
            IsSuperAdmin || g.Student.InstituteId == TenantId);
        modelBuilder.Entity<HomeworkSubmission>().HasQueryFilter(s =>
            IsSuperAdmin || s.Homework.InstituteId == null || s.Homework.InstituteId == TenantId);
        modelBuilder.Entity<ProfilePeriod>().HasQueryFilter(p =>
            IsSuperAdmin || p.Profile!.InstituteId == null || p.Profile!.InstituteId == TenantId);
        modelBuilder.Entity<DaySchedule>().HasQueryFilter(d =>
            IsSuperAdmin || d.Profile!.InstituteId == null || d.Profile!.InstituteId == TenantId);

        modelBuilder.Entity<ChatConversation>().HasKey(c => c.ChatConversationId);
        modelBuilder.Entity<ChatConversation>()
            .HasQueryFilter(c => !c.IsDeleted && (IsSuperAdmin || c.InstituteId == null || c.InstituteId == TenantId));

        modelBuilder.Entity<ChatConversationMember>().HasKey(m => m.MemberId);
        modelBuilder.Entity<ChatConversationMember>()
            .HasOne(m => m.Conversation).WithMany(c => c.Members).HasForeignKey(m => m.ConversationId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<ChatConversationMember>()
            .HasOne(m => m.User).WithMany().HasForeignKey(m => m.UserId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ChatMessage>().HasKey(m => m.ChatMessageId);
        modelBuilder.Entity<ChatMessage>()
            .HasQueryFilter(m => !m.IsDeleted && (IsSuperAdmin || m.InstituteId == null || m.InstituteId == TenantId));
        modelBuilder.Entity<ChatMessage>()
            .HasOne(m => m.Sender).WithMany().HasForeignKey(m => m.SenderId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ChatMessage>()
            .HasOne(m => m.Conversation).WithMany(c => c.Messages).HasForeignKey(m => m.ConversationId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ChatMessageRead>().HasKey(r => r.ReadId);
        modelBuilder.Entity<ChatMessageRead>()
            .HasOne(r => r.Message).WithMany(m => m.Reads).HasForeignKey(r => r.MessageId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<ChatMessageRead>()
            .HasOne(r => r.User).WithMany().HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Restrict);

        // Unique indexes
        // Email stays globally unique — it's the login key across all schools.
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        // AdmissionNo is unique PER INSTITUTE, not globally: each school runs its
        // own sequence, and the generator's max-query is tenant-filtered — a
        // global index would make School B collide forever with School A's
        // numbers it cannot see.
        modelBuilder.Entity<Student>().HasIndex(s => new { s.InstituteId, s.AdmissionNo }).IsUnique();
        modelBuilder.Entity<MenuRolePermission>()
            .HasIndex(m => new { m.MenuItemId, m.RoleId }).IsUnique();

        // AcademicCalendarEvent → AcademicYear
        modelBuilder.Entity<AcademicCalendarEvent>()
            .HasOne(e => e.AcademicYear)
            .WithMany()
            .HasForeignKey(e => e.AcademicYearId)
            .OnDelete(DeleteBehavior.Cascade);

        // AcademicCalendarEvent → CalendarEventType
        modelBuilder.Entity<AcademicCalendarEvent>()
            .HasOne(e => e.CalendarEventType)
            .WithMany(t => t.Events)
            .HasForeignKey(e => e.CalendarEventTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Seed default event types
        modelBuilder.Entity<CalendarEventType>().HasData(
            new CalendarEventType { CalendarEventTypeId = 1, Name = "Annual Holiday",   Color = "#7c3aed", Icon = "beach_access",  SortOrder = 1 },
            new CalendarEventType { CalendarEventTypeId = 2, Name = "Gazetted Holiday", Color = "#dc2626", Icon = "flag",           SortOrder = 2 },
            new CalendarEventType { CalendarEventTypeId = 3, Name = "Sports Day",       Color = "#d97706", Icon = "sports_soccer",  SortOrder = 3 },
            new CalendarEventType { CalendarEventTypeId = 4, Name = "Short Term Exam",  Color = "#0891b2", Icon = "quiz",           SortOrder = 4 },
            new CalendarEventType { CalendarEventTypeId = 5, Name = "Final Exam",       Color = "#059669", Icon = "school",         SortOrder = 5 },
            new CalendarEventType { CalendarEventTypeId = 6, Name = "Result Day",       Color = "#ea580c", Icon = "emoji_events",   SortOrder = 6 },
            new CalendarEventType { CalendarEventTypeId = 7, Name = "Other",            Color = "#6b7280", Icon = "event",          SortOrder = 7 }
        );

        // Role → Users
        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        // MenuItem self-referencing
        modelBuilder.Entity<MenuItem>()
            .HasOne(m => m.Parent)
            .WithMany(m => m.Children)
            .HasForeignKey(m => m.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // SchoolTimetable — no cascade to avoid multiple cascade paths
        modelBuilder.Entity<SchoolTimetable>()
            .HasOne(t => t.Class).WithMany().HasForeignKey(t => t.ClassId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<SchoolTimetable>()
            .HasOne(t => t.Subject).WithMany().HasForeignKey(t => t.SubjectId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<SchoolTimetable>()
            .HasOne(t => t.Teacher).WithMany().HasForeignKey(t => t.TeacherId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<SchoolTimetable>()
            .HasOne(t => t.Period).WithMany().HasForeignKey(t => t.PeriodId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<SchoolTimetable>()
            .HasOne(t => t.AcademicYear).WithMany().HasForeignKey(t => t.AcademicYearId).OnDelete(DeleteBehavior.Restrict);

        // Attendance — no cascade
        modelBuilder.Entity<Attendance>()
            .HasOne(a => a.Student).WithMany().HasForeignKey(a => a.StudentId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Attendance>()
            .HasOne(a => a.Class).WithMany().HasForeignKey(a => a.ClassId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Attendance>()
            .HasOne(a => a.Period).WithMany().HasForeignKey(a => a.PeriodId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Attendance>()
            .HasOne(a => a.MarkedByUser).WithMany().HasForeignKey(a => a.MarkedBy).OnDelete(DeleteBehavior.Restrict);

        // Homework — no cascade
        modelBuilder.Entity<Homework>()
            .HasOne(h => h.Class).WithMany().HasForeignKey(h => h.ClassId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Homework>()
            .HasOne(h => h.Subject).WithMany().HasForeignKey(h => h.SubjectId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Homework>()
            .HasOne(h => h.Teacher).WithMany().HasForeignKey(h => h.TeacherId).OnDelete(DeleteBehavior.Restrict);

        // HomeworkSubmission — no cascade
        modelBuilder.Entity<HomeworkSubmission>()
            .HasOne(s => s.Homework).WithMany(h => h.Submissions).HasForeignKey(s => s.HomeworkId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<HomeworkSubmission>()
            .HasOne(s => s.Student).WithMany().HasForeignKey(s => s.StudentId).OnDelete(DeleteBehavior.Restrict);

        // ClassSubject — no cascade
        modelBuilder.Entity<ClassSubject>()
            .HasOne(cs => cs.Class).WithMany(c => c.ClassSubjects).HasForeignKey(cs => cs.ClassId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ClassSubject>()
            .HasOne(cs => cs.Subject).WithMany(s => s.ClassSubjects).HasForeignKey(cs => cs.SubjectId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ClassSubject>()
            .HasOne(cs => cs.Teacher).WithMany().HasForeignKey(cs => cs.TeacherId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

        // StudentClassEnrollment — no cascade
        modelBuilder.Entity<StudentClassEnrollment>()
            .HasOne(e => e.Student).WithMany(s => s.Enrollments).HasForeignKey(e => e.StudentId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentClassEnrollment>()
            .HasOne(e => e.Class).WithMany(c => c.Enrollments).HasForeignKey(e => e.ClassId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentClassEnrollment>()
            .HasOne(e => e.AcademicYear).WithMany(a => a.Enrollments).HasForeignKey(e => e.AcademicYearId).OnDelete(DeleteBehavior.Restrict);

        // StudentGuardian
        modelBuilder.Entity<StudentGuardian>()
            .HasOne(g => g.Student).WithMany(s => s.Guardians).HasForeignKey(g => g.StudentId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentGuardian>()
            .HasOne(g => g.User).WithMany().HasForeignKey(g => g.UserId).OnDelete(DeleteBehavior.SetNull);

        // Explicit primary keys for entities not following Id/EntityNameId convention
        modelBuilder.Entity<FileStore>().HasKey(f => f.FileId);
        modelBuilder.Entity<FileStore>()
            .HasOne(f => f.UploadedByUser)
            .WithMany()
            .HasForeignKey(f => f.UploadedBy)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<HomeworkSubmission>().HasKey(h => h.SubmissionId);

        // Fee Module
        modelBuilder.Entity<FeeType>().HasKey(f => f.FeeTypeId);
        modelBuilder.Entity<FeeStructure>().HasKey(f => f.FeeStructureId);
        modelBuilder.Entity<StudentFee>().HasKey(sf => sf.StudentFeeId);
        modelBuilder.Entity<FeePayment>().HasKey(fp => fp.FeePaymentId);

        modelBuilder.Entity<FeeStructure>()
            .HasOne(f => f.FeeType).WithMany().HasForeignKey(f => f.FeeTypeId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FeeStructure>()
            .HasOne(f => f.Class).WithMany().HasForeignKey(f => f.ClassId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FeeStructure>()
            .HasOne(f => f.AcademicYear).WithMany().HasForeignKey(f => f.AcademicYearId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<StudentFee>()
            .HasOne(sf => sf.Student).WithMany().HasForeignKey(sf => sf.StudentId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentFee>()
            .HasOne(sf => sf.FeeStructure).WithMany().HasForeignKey(sf => sf.FeeStructureId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentFee>()
            .HasOne(sf => sf.AcademicYear).WithMany().HasForeignKey(sf => sf.AcademicYearId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FeePayment>()
            .HasOne(fp => fp.StudentFee).WithMany(sf => sf.Payments).HasForeignKey(fp => fp.StudentFeeId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<FeePayment>()
            .HasOne(fp => fp.CollectedByUser).WithMany().HasForeignKey(fp => fp.CollectedBy).OnDelete(DeleteBehavior.Restrict);

        // ExamQuestion / ExamQuestionOption — no cascade from Paper to avoid multiple cascade paths
        modelBuilder.Entity<ExamQuestion>().HasKey(q => q.ExamQuestionId);
        modelBuilder.Entity<ExamQuestion>()
            .HasOne(q => q.ExamPaper).WithMany(p => p.Questions).HasForeignKey(q => q.ExamPaperId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ExamQuestion>()
            .HasOne(q => q.ExamPaperSection).WithMany(s => s.Questions).HasForeignKey(q => q.ExamPaperSectionId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);


        modelBuilder.Entity<ExamQuestionOption>().HasKey(o => o.ExamQuestionOptionId);
        modelBuilder.Entity<ExamQuestionOption>()
            .HasOne(o => o.ExamQuestion).WithMany(q => q.Options).HasForeignKey(o => o.ExamQuestionId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<ExamQuestionOption>().HasQueryFilter(o => !o.IsDeleted);

        // Staff FK
        modelBuilder.Entity<Staff>().HasKey(s => s.StaffId);
        modelBuilder.Entity<Staff>()
            .HasOne(s => s.User).WithMany().HasForeignKey(s => s.UserId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<Staff>()
            .HasOne(s => s.Photo).WithMany().HasForeignKey(s => s.PhotoFileId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);

        // Discount Module
        modelBuilder.Entity<DiscountPolicy>().HasKey(d => d.DiscountPolicyId);
        modelBuilder.Entity<DiscountPolicy>().Property(d => d.Value).HasPrecision(10, 2);

        modelBuilder.Entity<StudentDiscount>().HasKey(sd => sd.StudentDiscountId);
        modelBuilder.Entity<StudentDiscount>().Property(sd => sd.OverrideValue).HasPrecision(10, 2);

        modelBuilder.Entity<StudentDiscount>()
            .HasOne(sd => sd.Student).WithMany().HasForeignKey(sd => sd.StudentId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentDiscount>()
            .HasOne(sd => sd.DiscountPolicy).WithMany(dp => dp.StudentDiscounts).HasForeignKey(sd => sd.DiscountPolicyId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StudentDiscount>()
            .HasOne(sd => sd.AcademicYear).WithMany().HasForeignKey(sd => sd.AcademicYearId).OnDelete(DeleteBehavior.Restrict);

        // Decimal precision for fee columns
        modelBuilder.Entity<FeeStructure>().Property(f => f.Amount).HasPrecision(12, 2);
        modelBuilder.Entity<StudentFee>().Property(sf => sf.AmountDue).HasPrecision(12, 2);
        modelBuilder.Entity<StudentFee>().Property(sf => sf.AmountPaid).HasPrecision(12, 2);
        modelBuilder.Entity<StudentFee>().Property(sf => sf.Discount).HasPrecision(12, 2);
        modelBuilder.Entity<FeePayment>().Property(fp => fp.AmountPaid).HasPrecision(12, 2);
        modelBuilder.Entity<StudentGuardian>().HasKey(g => g.GuardianId);
        modelBuilder.Entity<StudentClassEnrollment>().HasKey(e => e.EnrollmentId);
        modelBuilder.Entity<MenuRolePermission>().HasKey(m => m.Id);
        modelBuilder.Entity<RefreshToken>().HasKey(r => r.Id);
        modelBuilder.Entity<ClassSubject>().HasKey(c => c.Id);
        modelBuilder.Entity<SchoolTimetable>().HasKey(t => t.TimetableId);

        // TimetableSubstitution
        modelBuilder.Entity<TimetableSubstitution>().HasKey(s => s.SubstitutionId);
        modelBuilder.Entity<TimetableSubstitution>().HasQueryFilter(s => !s.IsDeleted);
        modelBuilder.Entity<TimetableSubstitution>()
            .HasOne(s => s.Timetable).WithMany().HasForeignKey(s => s.TimetableId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<TimetableSubstitution>()
            .HasOne(s => s.SubstituteTeacher).WithMany().HasForeignKey(s => s.SubstituteTeacherId).OnDelete(DeleteBehavior.Restrict);
        // One substitution per timetable slot per date
        modelBuilder.Entity<TimetableSubstitution>()
            .HasIndex(s => new { s.TimetableId, s.Date }).IsUnique();
        modelBuilder.Entity<Attendance>().HasKey(a => a.AttendanceId);
        modelBuilder.Entity<Homework>().HasKey(h => h.HomeworkId);
        modelBuilder.Entity<Period>().HasKey(p => p.PeriodId);
        modelBuilder.Entity<AcademicYear>().HasKey(a => a.AcademicYearId);

        // SchoolSettings — singleton (Id = 1 always)
        modelBuilder.Entity<SchoolSettings>().HasKey(s => s.Id);
        modelBuilder.Entity<SchoolSettings>().HasData(new SchoolSettings
        {
            Id                      = 1,
            RegularStartTime        = new TimeOnly(8, 0),
            RegularEndTime          = new TimeOnly(13, 0),
            RegularTotalPeriods     = 6,
            RegularBreakAllowed     = true,
            RegularBreakStart       = new TimeOnly(10, 0),
            RegularBreakEnd         = new TimeOnly(10, 20),
            RegularGapMinutes       = 0,
            FridayStartTime         = new TimeOnly(8, 0),
            FridayEndTime           = new TimeOnly(11, 30),
            FridayTotalPeriods      = 4,
            FridayBreakAllowed      = false,
            FridayBreakStart        = new TimeOnly(10, 0),
            FridayBreakEnd          = new TimeOnly(10, 10),
            FridayGapMinutes        = 0,
            SundayEnabled           = false,
            SundayStartTime         = new TimeOnly(8, 0),
            SundayEndTime           = new TimeOnly(13, 0),
            HolidaysJson            = "[]",
            MinPeriodDurationMinutes = 20,
            MaxPeriodDurationMinutes = 60,
            MaxPeriodsPerDay        = 10,
            UpdatedAt               = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UpdatedBy               = null
        });
        modelBuilder.Entity<Class>().HasKey(c => c.ClassId);
        modelBuilder.Entity<Subject>().HasKey(s => s.SubjectId);
        modelBuilder.Entity<Student>().HasKey(s => s.StudentId);
        modelBuilder.Entity<MenuItem>().HasKey(m => m.MenuItemId);
        modelBuilder.Entity<Role>().HasKey(r => r.RoleId);
        modelBuilder.Entity<User>().HasKey(u => u.UserId);

        // Seed Roles
        modelBuilder.Entity<Role>().HasData(
            new Role { RoleId = 1, RoleName = "superadmin", IsActive = true },
            new Role { RoleId = 2, RoleName = "admin", IsActive = true },
            new Role { RoleId = 3, RoleName = "principal", IsActive = true },
            new Role { RoleId = 4, RoleName = "teacher", IsActive = true },
            new Role { RoleId = 5, RoleName = "parent", IsActive = true },
            new Role { RoleId = 6, RoleName = "staff", IsActive = true }
        );

        // Seed SuperAdmin user (Password: Admin@123)
        modelBuilder.Entity<User>().HasData(new User
        {
            UserId = 1,
            FullName = "Super Admin",
            Email = "superadmin@school.com",
            Password = "Admin@123",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            RoleId = 1,
            IsActive = true,
            IsDeleted = false,
            CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        // Seed Periods
        modelBuilder.Entity<Period>().HasData(
            new Period { PeriodId = 1, PeriodNo = 1, PeriodName = "Period 1", StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(8, 40), IsBreak = false },
            new Period { PeriodId = 2, PeriodNo = 2, PeriodName = "Period 2", StartTime = new TimeOnly(8, 40), EndTime = new TimeOnly(9, 20), IsBreak = false },
            new Period { PeriodId = 3, PeriodNo = 3, PeriodName = "Period 3", StartTime = new TimeOnly(9, 20), EndTime = new TimeOnly(10, 0), IsBreak = false },
            new Period { PeriodId = 4, PeriodNo = 4, PeriodName = "Break",    StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(10, 20), IsBreak = true },
            new Period { PeriodId = 5, PeriodNo = 5, PeriodName = "Period 4", StartTime = new TimeOnly(10, 20), EndTime = new TimeOnly(11, 0), IsBreak = false },
            new Period { PeriodId = 6, PeriodNo = 6, PeriodName = "Period 5", StartTime = new TimeOnly(11, 0), EndTime = new TimeOnly(11, 40), IsBreak = false },
            new Period { PeriodId = 7, PeriodNo = 7, PeriodName = "Period 6", StartTime = new TimeOnly(11, 40), EndTime = new TimeOnly(12, 20), IsBreak = false }
        );

        // ── Seed Dynamic Menu (Module 2) ────────────────────────────────────
        // Role IDs (from above): 1=superadmin, 2=admin, 3=principal, 4=teacher, 5=parent, 6=staff.
        // Structure mirrors the "Module Access by Role" matrix in the task doc.
        var seedTs = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<MenuItem>().HasData(
            // SuperAdmin — Institute Management
            new MenuItem { MenuItemId = 41, ParentId = null, Title = "Institutes",        Icon = "domain",         RouteUrl = "/superadmin/institutes",    SortOrder = 5, IsActive = true, IsDeleted = false, CreatedAt = seedTs },
            new MenuItem { MenuItemId = 42, ParentId = null, Title = "Activity Logs",     Icon = "manage_history", RouteUrl = "/superadmin/activity-logs", SortOrder = 6, IsActive = true, IsDeleted = false, CreatedAt = seedTs },
            new MenuItem { MenuItemId = 43, ParentId = null, Title = "Company Settings",  Icon = "business",       RouteUrl = "/superadmin/company",       SortOrder = 7, IsActive = true, IsDeleted = false, CreatedAt = seedTs },

            // Roots
            new MenuItem { MenuItemId = 1,  ParentId = null, Title = "Dashboard",        Icon = "dashboard",      RouteUrl = "/dashboard",          SortOrder = 10, IsActive = true, IsDeleted = false, CreatedAt = seedTs },
            new MenuItem { MenuItemId = 2,  ParentId = null, Title = "Users",            Icon = "people",         RouteUrl = "/users",              SortOrder = 20, IsActive = true, IsDeleted = false, CreatedAt = seedTs },
            new MenuItem { MenuItemId = 3,  ParentId = null, Title = "Academics",        Icon = "school",         RouteUrl = null,                  SortOrder = 30, IsActive = true, IsDeleted = false, CreatedAt = seedTs },
            new MenuItem { MenuItemId = 25, ParentId = null, Title = "Teachers",         Icon = "badge",          RouteUrl = "/teachers",           SortOrder = 35, IsActive = true, IsDeleted = true,  CreatedAt = seedTs },
            new MenuItem { MenuItemId = 6,  ParentId = null, Title = "Students",         Icon = "groups",         RouteUrl = "/students",           SortOrder = 40, IsActive = true, IsDeleted = false, CreatedAt = seedTs },
            new MenuItem { MenuItemId = 9,  ParentId = null, Title = "Timetable",        Icon = "schedule",       RouteUrl = null,                  SortOrder = 50, IsActive = true, IsDeleted = false, CreatedAt = seedTs },
            new MenuItem { MenuItemId = 12, ParentId = null, Title = "Attendance",       Icon = "how_to_reg",     RouteUrl = null,                  SortOrder = 60, IsActive = true, IsDeleted = false, CreatedAt = seedTs },
            new MenuItem { MenuItemId = 15, ParentId = null, Title = "Homework",         Icon = "assignment",     RouteUrl = null,                  SortOrder = 70, IsActive = true, IsDeleted = false, CreatedAt = seedTs },
            new MenuItem { MenuItemId = 18, ParentId = null, Title = "Files",            Icon = "folder",         RouteUrl = "/files",              SortOrder = 80, IsActive = true, IsDeleted = false, CreatedAt = seedTs },
            new MenuItem { MenuItemId = 19, ParentId = null, Title = "Menu Management",  Icon = "settings",       RouteUrl = "/admin/menu",         SortOrder = 90, IsActive = true, IsDeleted = false, CreatedAt = seedTs },
            new MenuItem { MenuItemId = 26, ParentId = null, Title = "School Settings",  Icon = "tune",           RouteUrl = "/admin/settings",     SortOrder = 95, IsActive = true, IsDeleted = false, CreatedAt = seedTs },

            // HR menu
            new MenuItem { MenuItemId = 31, ParentId = null, Title = "HR",                    Icon = "badge",         RouteUrl = null,                SortOrder = 36, IsActive = true, IsDeleted = false, CreatedAt = seedTs },
            new MenuItem { MenuItemId = 32, ParentId = 31,   Title = "Teaching Staff",         Icon = "school",        RouteUrl = "/hr/teaching-staff",SortOrder = 37, IsActive = true, IsDeleted = false, CreatedAt = seedTs },
            new MenuItem { MenuItemId = 33, ParentId = 31,   Title = "Non-Teaching Staff",     Icon = "groups",        RouteUrl = "/hr/staff",         SortOrder = 38, IsActive = true, IsDeleted = false, CreatedAt = seedTs },

            // Academics children
            new MenuItem { MenuItemId = 4,  ParentId = 3,    Title = "Classes",            Icon = "class",          RouteUrl = "/academics/classes",   SortOrder = 31, IsActive = true, IsDeleted = false, CreatedAt = seedTs },
            new MenuItem { MenuItemId = 5,  ParentId = 3,    Title = "Academic Years",     Icon = "calendar_month", RouteUrl = "/academics/years",     SortOrder = 32, IsActive = true, IsDeleted = false, CreatedAt = seedTs },
            new MenuItem { MenuItemId = 24, ParentId = 3,    Title = "Subjects",              Icon = "library_books",  RouteUrl = "/academics/subjects",             SortOrder = 33, IsActive = true, IsDeleted = false, CreatedAt = seedTs },
            new MenuItem { MenuItemId = 27, ParentId = 3,    Title = "Teacher Assignments",   Icon = "assignment_ind", RouteUrl = "/academics/teacher-assignments",  SortOrder = 34, IsActive = true, IsDeleted = false, CreatedAt = seedTs },

            // Students children
            new MenuItem { MenuItemId = 7,  ParentId = 6,    Title = "New Admission",      Icon = "person_add",     RouteUrl = "/students/new",  SortOrder = 41, IsActive = true, IsDeleted = false, CreatedAt = seedTs },
            new MenuItem { MenuItemId = 52, ParentId = 6,    Title = "Add Student",        Icon = "person_add_alt", RouteUrl = "/students/add",  SortOrder = 42, IsActive = true, IsDeleted = false, CreatedAt = seedTs },
            new MenuItem { MenuItemId = 8,  ParentId = 6,    Title = "Student List",       Icon = "list",           RouteUrl = "/students/list", SortOrder = 43, IsActive = true, IsDeleted = false, CreatedAt = seedTs },

            // Timetable children
            new MenuItem { MenuItemId = 10, ParentId = 9,    Title = "Timetable Setup",    Icon = "edit_calendar", RouteUrl = "/timetable/setup", SortOrder = 51, IsActive = true, IsDeleted = false, CreatedAt = seedTs },
            new MenuItem { MenuItemId = 11, ParentId = 9,    Title = "View Timetable",     Icon = "event",         RouteUrl = "/timetable/view",  SortOrder = 52, IsActive = true, IsDeleted = false, CreatedAt = seedTs },

            // Attendance children
            new MenuItem { MenuItemId = 13, ParentId = 12,   Title = "Mark Attendance",    Icon = "check_circle", RouteUrl = "/attendance/mark", SortOrder = 61, IsActive = true, IsDeleted = false, CreatedAt = seedTs },
            new MenuItem { MenuItemId = 14, ParentId = 12,   Title = "View Attendance",    Icon = "visibility",   RouteUrl = "/attendance/view", SortOrder = 62, IsActive = true, IsDeleted = false, CreatedAt = seedTs },

            // Homework children
            new MenuItem { MenuItemId = 16, ParentId = 15,   Title = "Assign Homework",    Icon = "edit_note", RouteUrl = "/homework/assign", SortOrder = 71, IsActive = true, IsDeleted = false, CreatedAt = seedTs },
            new MenuItem { MenuItemId = 17, ParentId = 15,   Title = "Diary / Homework",   Icon = "menu_book", RouteUrl = "/homework/list",   SortOrder = 72, IsActive = true, IsDeleted = false, CreatedAt = seedTs },

            // Exam Module
            new MenuItem { MenuItemId = 34, ParentId = null, Title = "Exams",          Icon = "quiz",           RouteUrl = null,                   SortOrder = 65, IsActive = true, IsDeleted = false, CreatedAt = seedTs },
            new MenuItem { MenuItemId = 35, ParentId = 34,   Title = "Paper Setup",    Icon = "edit_document",  RouteUrl = "/exams/papers",         SortOrder = 66, IsActive = true, IsDeleted = false, CreatedAt = seedTs },
            new MenuItem { MenuItemId = 36, ParentId = 34,   Title = "Exam Schedule",  Icon = "calendar_month", RouteUrl = "/exams/schedule",       SortOrder = 67, IsActive = true, IsDeleted = false, CreatedAt = seedTs },
            new MenuItem { MenuItemId = 37, ParentId = 34,   Title = "Enter Results",  Icon = "grading",        RouteUrl = "/exams/results/enter",  SortOrder = 68, IsActive = true, IsDeleted = false, CreatedAt = seedTs },
            new MenuItem { MenuItemId = 38, ParentId = 34,   Title = "Result Cards",   Icon = "emoji_events",   RouteUrl = "/exams/results/cards",  SortOrder = 69, IsActive = true, IsDeleted = false, CreatedAt = seedTs },

            // Fees
            new MenuItem { MenuItemId = 20, ParentId = null, Title = "Fees",               Icon = "payments",   RouteUrl = null,                    SortOrder = 75, IsActive = true, IsDeleted = false, CreatedAt = seedTs },
            new MenuItem { MenuItemId = 21, ParentId = 20,   Title = "Fee Setup",          Icon = "settings",   RouteUrl = "/fees/setup",           SortOrder = 76, IsActive = true, IsDeleted = false, CreatedAt = seedTs },
            new MenuItem { MenuItemId = 22, ParentId = 20,   Title = "Student Fees",       Icon = "receipt",    RouteUrl = "/fees/student-fees",    SortOrder = 77, IsActive = true, IsDeleted = false, CreatedAt = seedTs },
            new MenuItem { MenuItemId = 23, ParentId = 20,   Title = "Fee Report",         Icon = "bar_chart",  RouteUrl = "/fees/report",          SortOrder = 78, IsActive = true, IsDeleted = false, CreatedAt = seedTs },
            new MenuItem { MenuItemId = 39, ParentId = 20,   Title = "Fee Challan",        Icon = "receipt_long", RouteUrl = "/fees/challan",        SortOrder = 79, IsActive = true, IsDeleted = false, CreatedAt = seedTs }
        );

        // Role permissions for each menu item.
        // Helper-style local list to keep this readable; expanded inline so HasData sees concrete objects.
        modelBuilder.Entity<MenuRolePermission>().HasData(
            // Dashboard (1) → everyone
            new MenuRolePermission { Id = 1,  MenuItemId = 1, RoleId = 1 },
            new MenuRolePermission { Id = 2,  MenuItemId = 1, RoleId = 2 },
            new MenuRolePermission { Id = 3,  MenuItemId = 1, RoleId = 3 },
            new MenuRolePermission { Id = 4,  MenuItemId = 1, RoleId = 4 },
            new MenuRolePermission { Id = 5,  MenuItemId = 1, RoleId = 5 },
            new MenuRolePermission { Id = 6,  MenuItemId = 1, RoleId = 6 },

            // Users (2) → SuperAdmin, Admin
            new MenuRolePermission { Id = 7,  MenuItemId = 2, RoleId = 1 },
            new MenuRolePermission { Id = 8,  MenuItemId = 2, RoleId = 2 },

            // Academics (3) + Classes (4) + Years (5) → SuperAdmin, Admin
            new MenuRolePermission { Id = 10, MenuItemId = 3, RoleId = 2 },
            new MenuRolePermission { Id = 12, MenuItemId = 4, RoleId = 2 },
            new MenuRolePermission { Id = 14, MenuItemId = 5, RoleId = 2 },

            // Students (6,7,50,8) → SuperAdmin, Admin, Principal
            new MenuRolePermission { Id = 16, MenuItemId = 6,  RoleId = 2 },
            new MenuRolePermission { Id = 17, MenuItemId = 6,  RoleId = 3 },
            new MenuRolePermission { Id = 19, MenuItemId = 7,  RoleId = 2 },
            new MenuRolePermission { Id = 20, MenuItemId = 7,  RoleId = 3 },
            new MenuRolePermission { Id = 141, MenuItemId = 52, RoleId = 2 },
            new MenuRolePermission { Id = 142, MenuItemId = 52, RoleId = 3 },
            new MenuRolePermission { Id = 22, MenuItemId = 8,  RoleId = 2 },
            new MenuRolePermission { Id = 23, MenuItemId = 8,  RoleId = 3 },

            // Timetable group (9) → everyone except Staff
            new MenuRolePermission { Id = 25, MenuItemId = 9, RoleId = 2 },
            new MenuRolePermission { Id = 26, MenuItemId = 9, RoleId = 3 },
            new MenuRolePermission { Id = 27, MenuItemId = 9, RoleId = 4 },
            new MenuRolePermission { Id = 28, MenuItemId = 9, RoleId = 5 },

            // Timetable Setup (10) → SuperAdmin, Admin, Principal
            new MenuRolePermission { Id = 30, MenuItemId = 10, RoleId = 2 },
            new MenuRolePermission { Id = 31, MenuItemId = 10, RoleId = 3 },

            // View Timetable (11) → everyone except Staff
            new MenuRolePermission { Id = 33, MenuItemId = 11, RoleId = 2 },
            new MenuRolePermission { Id = 34, MenuItemId = 11, RoleId = 3 },
            new MenuRolePermission { Id = 35, MenuItemId = 11, RoleId = 4 },
            new MenuRolePermission { Id = 36, MenuItemId = 11, RoleId = 5 },

            // Attendance group (12) → everyone except Staff
            new MenuRolePermission { Id = 38, MenuItemId = 12, RoleId = 2 },
            new MenuRolePermission { Id = 39, MenuItemId = 12, RoleId = 3 },
            new MenuRolePermission { Id = 40, MenuItemId = 12, RoleId = 4 },
            new MenuRolePermission { Id = 41, MenuItemId = 12, RoleId = 5 },

            // Mark Attendance (13) → Teacher only
            new MenuRolePermission { Id = 42, MenuItemId = 13, RoleId = 4 },

            // View Attendance (14) → everyone except Staff
            new MenuRolePermission { Id = 44, MenuItemId = 14, RoleId = 2 },
            new MenuRolePermission { Id = 45, MenuItemId = 14, RoleId = 3 },
            new MenuRolePermission { Id = 46, MenuItemId = 14, RoleId = 4 },
            new MenuRolePermission { Id = 47, MenuItemId = 14, RoleId = 5 },

            // Homework group (15) → everyone except Staff
            new MenuRolePermission { Id = 49, MenuItemId = 15, RoleId = 2 },
            new MenuRolePermission { Id = 50, MenuItemId = 15, RoleId = 3 },
            new MenuRolePermission { Id = 51, MenuItemId = 15, RoleId = 4 },
            new MenuRolePermission { Id = 52, MenuItemId = 15, RoleId = 5 },

            // Assign Homework (16) → Teacher only
            new MenuRolePermission { Id = 53, MenuItemId = 16, RoleId = 4 },

            // Diary / Homework (17) → everyone except Staff
            new MenuRolePermission { Id = 55, MenuItemId = 17, RoleId = 2 },
            new MenuRolePermission { Id = 56, MenuItemId = 17, RoleId = 3 },
            new MenuRolePermission { Id = 57, MenuItemId = 17, RoleId = 4 },
            new MenuRolePermission { Id = 58, MenuItemId = 17, RoleId = 5 },

            // Files (18) → everyone
            new MenuRolePermission { Id = 60, MenuItemId = 18, RoleId = 2 },
            new MenuRolePermission { Id = 61, MenuItemId = 18, RoleId = 3 },
            new MenuRolePermission { Id = 62, MenuItemId = 18, RoleId = 4 },
            new MenuRolePermission { Id = 63, MenuItemId = 18, RoleId = 5 },
            new MenuRolePermission { Id = 64, MenuItemId = 18, RoleId = 6 },

            // Menu Management (19) → SuperAdmin only
            new MenuRolePermission { Id = 78, MenuItemId = 24, RoleId = 2 },
            new MenuRolePermission { Id = 85, MenuItemId = 27, RoleId = 2 },
            new MenuRolePermission { Id = 80, MenuItemId = 25, RoleId = 2 },
            new MenuRolePermission { Id = 81, MenuItemId = 25, RoleId = 3 },

            // Institutes (41) → SuperAdmin only
            new MenuRolePermission { Id = 138, MenuItemId = 41, RoleId = 1 },
            // Activity Logs (42) → SuperAdmin only
            new MenuRolePermission { Id = 139, MenuItemId = 42, RoleId = 1 },
            // Company Settings (43) → SuperAdmin only
            new MenuRolePermission { Id = 200, MenuItemId = 43, RoleId = 1 },

            new MenuRolePermission { Id = 65, MenuItemId = 19, RoleId = 1 },
            new MenuRolePermission { Id = 82, MenuItemId = 26, RoleId = 1 },
            new MenuRolePermission { Id = 83, MenuItemId = 26, RoleId = 2 },

            // Fees group (20) → SuperAdmin, Admin, Principal
            new MenuRolePermission { Id = 67, MenuItemId = 20, RoleId = 2 },
            new MenuRolePermission { Id = 68, MenuItemId = 20, RoleId = 3 },

            // Fee Setup (21) → SuperAdmin, Admin
            new MenuRolePermission { Id = 70, MenuItemId = 21, RoleId = 2 },

            // Student Fees (22) → SuperAdmin, Admin, Principal
            new MenuRolePermission { Id = 72, MenuItemId = 22, RoleId = 2 },
            new MenuRolePermission { Id = 73, MenuItemId = 22, RoleId = 3 },

            // Fee Report (23) → SuperAdmin, Admin, Principal
            new MenuRolePermission { Id = 75, MenuItemId = 23, RoleId = 2 },
            new MenuRolePermission { Id = 76, MenuItemId = 23, RoleId = 3 },

            // Fee Challan (39) → SuperAdmin, Admin, Principal
            new MenuRolePermission { Id = 128, MenuItemId = 39, RoleId = 2 },
            new MenuRolePermission { Id = 129, MenuItemId = 39, RoleId = 3 },

            // HR (31) → SuperAdmin, Admin, Principal
            new MenuRolePermission { Id = 96,  MenuItemId = 31, RoleId = 2 },
            new MenuRolePermission { Id = 97,  MenuItemId = 31, RoleId = 3 },
            // Teaching Staff (32) → SuperAdmin, Admin, Principal
            new MenuRolePermission { Id = 99,  MenuItemId = 32, RoleId = 2 },
            new MenuRolePermission { Id = 100, MenuItemId = 32, RoleId = 3 },
            // Non-Teaching Staff (33) → SuperAdmin, Admin, Principal
            new MenuRolePermission { Id = 102, MenuItemId = 33, RoleId = 2 },
            new MenuRolePermission { Id = 103, MenuItemId = 33, RoleId = 3 },

            // Exam (34) → SuperAdmin, Admin, Principal, Teacher, Parent
            new MenuRolePermission { Id = 105, MenuItemId = 34, RoleId = 2 },
            new MenuRolePermission { Id = 106, MenuItemId = 34, RoleId = 3 },
            new MenuRolePermission { Id = 107, MenuItemId = 34, RoleId = 4 },
            new MenuRolePermission { Id = 108, MenuItemId = 34, RoleId = 5 },

            // Paper Setup (35) → SuperAdmin, Admin, Principal, Teacher
            new MenuRolePermission { Id = 110, MenuItemId = 35, RoleId = 2 },
            new MenuRolePermission { Id = 111, MenuItemId = 35, RoleId = 3 },
            new MenuRolePermission { Id = 112, MenuItemId = 35, RoleId = 4 },

            // Exam Schedule (36) → SuperAdmin, Admin, Principal, Teacher, Parent
            new MenuRolePermission { Id = 114, MenuItemId = 36, RoleId = 2 },
            new MenuRolePermission { Id = 115, MenuItemId = 36, RoleId = 3 },
            new MenuRolePermission { Id = 116, MenuItemId = 36, RoleId = 4 },
            new MenuRolePermission { Id = 117, MenuItemId = 36, RoleId = 5 },

            // Enter Results (37) → SuperAdmin, Admin, Principal, Teacher
            new MenuRolePermission { Id = 119, MenuItemId = 37, RoleId = 2 },
            new MenuRolePermission { Id = 120, MenuItemId = 37, RoleId = 3 },
            new MenuRolePermission { Id = 121, MenuItemId = 37, RoleId = 4 },

            // Result Cards (38) → SuperAdmin, Admin, Principal, Teacher, Parent
            new MenuRolePermission { Id = 123, MenuItemId = 38, RoleId = 2 },
            new MenuRolePermission { Id = 124, MenuItemId = 38, RoleId = 3 },
            new MenuRolePermission { Id = 125, MenuItemId = 38, RoleId = 4 },
            new MenuRolePermission { Id = 126, MenuItemId = 38, RoleId = 5 }
        );

        // ── Exam Module FK configuration ──────────────────────────────────────────
        modelBuilder.Entity<ExamPaper>().HasKey(e => e.ExamPaperId);
        modelBuilder.Entity<ExamPaper>().HasQueryFilter(e => !e.IsDeleted);

        modelBuilder.Entity<ExamPaper>()
            .HasOne(e => e.AcademicYear).WithMany().HasForeignKey(e => e.AcademicYearId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ExamPaper>()
            .HasOne(e => e.Class).WithMany().HasForeignKey(e => e.ClassId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ExamPaper>()
            .HasOne(e => e.Subject).WithMany().HasForeignKey(e => e.SubjectId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ExamPaper>()
            .HasOne(e => e.CreatedByUser).WithMany().HasForeignKey(e => e.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ExamPaperSection>().HasKey(s => s.ExamPaperSectionId);
        modelBuilder.Entity<ExamPaperSection>()
            .HasOne(s => s.ExamPaper).WithMany(p => p.Sections).HasForeignKey(s => s.ExamPaperId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ExamSchedule>().HasKey(s => s.ExamScheduleId);
        modelBuilder.Entity<ExamSchedule>().HasQueryFilter(s => !s.IsDeleted);
        modelBuilder.Entity<ExamSchedule>()
            .HasOne(s => s.ExamPaper).WithMany(p => p.Schedules).HasForeignKey(s => s.ExamPaperId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<ExamSchedule>()
            .HasOne(s => s.InvigilatorUser).WithMany().HasForeignKey(s => s.InvigilatorUserId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<ExamResult>().HasKey(r => r.ExamResultId);
        modelBuilder.Entity<ExamResult>()
            .HasOne(r => r.ExamPaper).WithMany(p => p.Results).HasForeignKey(r => r.ExamPaperId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ExamResult>()
            .HasOne(r => r.Student).WithMany().HasForeignKey(r => r.StudentId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ExamResult>()
            .HasOne(r => r.EnteredByUser).WithMany().HasForeignKey(r => r.EnteredByUserId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
        modelBuilder.Entity<ExamResult>()
            .HasIndex(r => new { r.ExamPaperId, r.StudentId }).IsUnique();
        modelBuilder.Entity<ExamResult>()
            .Property(r => r.ObtainedMarks).HasPrecision(8, 2);
        modelBuilder.Entity<ExamResult>()
            .Property(r => r.Percentage).HasPrecision(6, 2);

        // ── DevCompany (singleton, Id = 1) ───────────────────────────────────
        modelBuilder.Entity<DevCompany>().HasKey(d => d.Id);
        modelBuilder.Entity<DevCompany>().HasData(new DevCompany
        {
            Id            = 1,
            Name          = "Dev_Solutions",
            Tagline       = "Empowering Schools with Smart Technology",
            CopyrightText = $"© {DateTime.UtcNow.Year} Dev_Solutions. All rights reserved.",
            UpdatedAt     = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        // ── Institute / Campus ────────────────────────────────────────────────
        modelBuilder.Entity<Institute>().HasKey(i => i.InstituteId);
        modelBuilder.Entity<Institute>().HasQueryFilter(i => !i.IsDeleted);

        modelBuilder.Entity<Campus>().HasKey(c => c.CampusId);
        modelBuilder.Entity<Campus>().HasQueryFilter(c => !c.IsDeleted);
        modelBuilder.Entity<Campus>()
            .HasOne(c => c.Institute).WithMany(i => i.Campuses)
            .HasForeignKey(c => c.InstituteId).OnDelete(DeleteBehavior.Restrict);

        // User → Institute / Campus (nullable — superadmin has none)
        modelBuilder.Entity<User>()
            .HasOne(u => u.Institute).WithMany()
            .HasForeignKey(u => u.InstituteId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<User>()
            .HasOne(u => u.Campus).WithMany(c => c.Users)
            .HasForeignKey(u => u.CampusId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);

        // ══════════════════════════════════════════════════════════════════
        // Inventory & POS Module
        // ══════════════════════════════════════════════════════════════════

        // ── Primary keys ─────────────────────────────────────────────────────
        // These entities' Id properties drop the "Master" suffix (e.g. ItemMaster.ItemId),
        // so EF Core's default {ClassName}Id key-discovery convention can't find them —
        // without an explicit HasKey, model validation fails for the whole context.
        modelBuilder.Entity<ItemMaster>().HasKey(i => i.ItemId);
        modelBuilder.Entity<UnitMaster>().HasKey(u => u.UnitId);
        modelBuilder.Entity<PackageMaster>().HasKey(p => p.PackageId);
        modelBuilder.Entity<PurchaseMaster>().HasKey(p => p.PurchaseId);
        modelBuilder.Entity<PurchaseReturnMaster>().HasKey(p => p.PurchaseReturnId);
        modelBuilder.Entity<SalesMaster>().HasKey(s => s.SalesId);
        modelBuilder.Entity<SalesReturnMaster>().HasKey(s => s.SalesReturnId);
        modelBuilder.Entity<StockAdjustmentMaster>().HasKey(s => s.StockAdjustmentId);
        modelBuilder.Entity<StockTransferMaster>().HasKey(s => s.StockTransferId);
        modelBuilder.Entity<TaxMaster>().HasKey(t => t.TaxId);

        // ── Query filters (soft-delete + tenant isolation) ──────────────────
        modelBuilder.Entity<UnitMaster>().HasQueryFilter(e =>
            !e.IsDeleted && (IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId));
        modelBuilder.Entity<TaxMaster>().HasQueryFilter(e =>
            !e.IsDeleted && (IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId));
        modelBuilder.Entity<ItemCategory>().HasQueryFilter(e =>
            !e.IsDeleted && (IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId));
        modelBuilder.Entity<ItemMaster>().HasQueryFilter(e =>
            !e.IsDeleted && (IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId));
        modelBuilder.Entity<Supplier>().HasQueryFilter(e =>
            !e.IsDeleted && (IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId));
        modelBuilder.Entity<PackageMaster>().HasQueryFilter(e =>
            !e.IsDeleted && (IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId));
        modelBuilder.Entity<PurchaseMaster>().HasQueryFilter(e =>
            !e.IsDeleted && (IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId));
        modelBuilder.Entity<PurchaseReturnMaster>().HasQueryFilter(e =>
            !e.IsDeleted && (IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId));
        modelBuilder.Entity<SalesMaster>().HasQueryFilter(e =>
            !e.IsDeleted && (IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId));
        modelBuilder.Entity<SalesReturnMaster>().HasQueryFilter(e =>
            !e.IsDeleted && (IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId));
        modelBuilder.Entity<StockAdjustmentMaster>().HasQueryFilter(e =>
            !e.IsDeleted && (IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId));
        modelBuilder.Entity<StockTransferMaster>().HasQueryFilter(e =>
            !e.IsDeleted && (IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId));
        modelBuilder.Entity<StockLedger>().HasQueryFilter(e =>
            !e.IsDeleted && (IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId));
        modelBuilder.Entity<CurrentStock>().HasQueryFilter(e =>
            !e.IsDeleted && (IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId));
        modelBuilder.Entity<InventorySettings>().HasQueryFilter(e =>
            !e.IsDeleted && (IsSuperAdmin || e.InstituteId == null || e.InstituteId == TenantId));

        // ── Item Master ──────────────────────────────────────────────────────
        modelBuilder.Entity<ItemMaster>().HasIndex(i => i.ItemCode).IsUnique();
        modelBuilder.Entity<ItemMaster>().HasIndex(i => i.Barcode).IsUnique().HasFilter("[Barcode] IS NOT NULL");
        modelBuilder.Entity<ItemMaster>()
            .HasOne(i => i.Category).WithMany(c => c.Items).HasForeignKey(i => i.ItemCategoryId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ItemMaster>()
            .HasOne(i => i.Unit).WithMany(u => u.Items).HasForeignKey(i => i.UnitId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ItemMaster>().Property(i => i.MinimumStockLevel).HasPrecision(12, 2);
        modelBuilder.Entity<ItemMaster>().Property(i => i.ReorderLevel).HasPrecision(12, 2);
        modelBuilder.Entity<ItemMaster>().Property(i => i.LastPurchasePrice).HasPrecision(12, 2);
        modelBuilder.Entity<ItemMaster>().Property(i => i.AverageCost).HasPrecision(12, 2);
        modelBuilder.Entity<ItemMaster>().Property(i => i.SalePrice).HasPrecision(12, 2);
        modelBuilder.Entity<ItemMaster>().Property(i => i.WholesalePrice).HasPrecision(12, 2);
        modelBuilder.Entity<ItemMaster>().Property(i => i.MinimumSalePrice).HasPrecision(12, 2);
        modelBuilder.Entity<ItemMaster>().Property(i => i.TaxPercentage).HasPrecision(5, 2);
        modelBuilder.Entity<TaxMaster>().Property(t => t.TaxPercentage).HasPrecision(5, 2);

        // ── Package ──────────────────────────────────────────────────────────
        modelBuilder.Entity<PackageMaster>().HasIndex(p => p.PackageCode).IsUnique();
        modelBuilder.Entity<PackageMaster>().Property(p => p.PackagePrice).HasPrecision(12, 2);
        modelBuilder.Entity<PackageDetail>()
            .HasOne(d => d.Package).WithMany(p => p.Details).HasForeignKey(d => d.PackageId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<PackageDetail>()
            .HasOne(d => d.Item).WithMany().HasForeignKey(d => d.ItemId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<PackageDetail>().Property(d => d.Quantity).HasPrecision(12, 2);

        // ── Purchase ─────────────────────────────────────────────────────────
        modelBuilder.Entity<PurchaseMaster>().HasIndex(p => p.PurchaseNo).IsUnique();
        modelBuilder.Entity<PurchaseMaster>()
            .HasOne(p => p.Supplier).WithMany().HasForeignKey(p => p.SupplierId).OnDelete(DeleteBehavior.Restrict);
        foreach (var prop in new[] { "GrossAmount", "DiscountAmount", "TaxAmount", "NetAmount" })
            modelBuilder.Entity<PurchaseMaster>().Property(prop).HasPrecision(14, 2);
        modelBuilder.Entity<PurchaseDetail>()
            .HasOne(d => d.Purchase).WithMany(p => p.Details).HasForeignKey(d => d.PurchaseId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<PurchaseDetail>()
            .HasOne(d => d.Item).WithMany().HasForeignKey(d => d.ItemId).OnDelete(DeleteBehavior.Restrict);
        foreach (var prop in new[] { "Quantity", "PurchasePrice", "Discount", "Tax", "NetAmount" })
            modelBuilder.Entity<PurchaseDetail>().Property(prop).HasPrecision(14, 2);

        // ── Purchase Return ──────────────────────────────────────────────────
        modelBuilder.Entity<PurchaseReturnMaster>().HasIndex(p => p.ReturnNo).IsUnique();
        modelBuilder.Entity<PurchaseReturnMaster>()
            .HasOne(p => p.Purchase).WithMany().HasForeignKey(p => p.PurchaseId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<PurchaseReturnMaster>()
            .HasOne(p => p.Supplier).WithMany().HasForeignKey(p => p.SupplierId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<PurchaseReturnMaster>().Property(p => p.NetAmount).HasPrecision(14, 2);
        modelBuilder.Entity<PurchaseReturnDetail>()
            .HasOne(d => d.PurchaseReturn).WithMany(p => p.Details).HasForeignKey(d => d.PurchaseReturnId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<PurchaseReturnDetail>()
            .HasOne(d => d.Item).WithMany().HasForeignKey(d => d.ItemId).OnDelete(DeleteBehavior.Restrict);
        foreach (var prop in new[] { "Quantity", "Price", "NetAmount" })
            modelBuilder.Entity<PurchaseReturnDetail>().Property(prop).HasPrecision(14, 2);

        // ── POS / Sales ──────────────────────────────────────────────────────
        modelBuilder.Entity<SalesMaster>().HasIndex(s => s.InvoiceNo).IsUnique();
        foreach (var prop in new[] { "GrossAmount", "DiscountAmount", "TaxAmount", "NetAmount", "PaidAmount", "ChangeAmount" })
            modelBuilder.Entity<SalesMaster>().Property(prop).HasPrecision(14, 2);
        modelBuilder.Entity<SalesDetail>()
            .HasOne(d => d.Sales).WithMany(s => s.Details).HasForeignKey(d => d.SalesId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<SalesDetail>()
            .HasOne(d => d.Item).WithMany().HasForeignKey(d => d.ItemId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<SalesDetail>()
            .HasOne(d => d.Package).WithMany().HasForeignKey(d => d.PackageId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
        foreach (var prop in new[] { "Quantity", "Price", "Discount", "Tax", "Amount", "CostAmount" })
            modelBuilder.Entity<SalesDetail>().Property(prop).HasPrecision(14, 2);
        modelBuilder.Entity<SalesPayment>()
            .HasOne(p => p.Sales).WithMany(s => s.Payments).HasForeignKey(p => p.SalesId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<SalesPayment>().Property(p => p.Amount).HasPrecision(14, 2);

        // ── Sales Return ─────────────────────────────────────────────────────
        modelBuilder.Entity<SalesReturnMaster>().HasIndex(s => s.ReturnNo).IsUnique();
        modelBuilder.Entity<SalesReturnMaster>()
            .HasOne(s => s.Sales).WithMany().HasForeignKey(s => s.SalesId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<SalesReturnMaster>().Property(s => s.NetAmount).HasPrecision(14, 2);
        modelBuilder.Entity<SalesReturnDetail>()
            .HasOne(d => d.SalesReturn).WithMany(s => s.Details).HasForeignKey(d => d.SalesReturnId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<SalesReturnDetail>()
            .HasOne(d => d.Item).WithMany().HasForeignKey(d => d.ItemId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<SalesReturnDetail>()
            .HasOne(d => d.Package).WithMany().HasForeignKey(d => d.PackageId).IsRequired(false).OnDelete(DeleteBehavior.Restrict);
        foreach (var prop in new[] { "Quantity", "Price", "Amount" })
            modelBuilder.Entity<SalesReturnDetail>().Property(prop).HasPrecision(14, 2);

        // ── Stock Adjustment / Transfer ──────────────────────────────────────
        modelBuilder.Entity<StockAdjustmentMaster>().HasIndex(s => s.AdjustmentNo).IsUnique();
        modelBuilder.Entity<StockAdjustmentDetail>()
            .HasOne(d => d.StockAdjustment).WithMany(s => s.Details).HasForeignKey(d => d.StockAdjustmentId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<StockAdjustmentDetail>()
            .HasOne(d => d.Item).WithMany().HasForeignKey(d => d.ItemId).OnDelete(DeleteBehavior.Restrict);
        foreach (var prop in new[] { "QtyBefore", "QtyAfter", "QtyDiff", "Cost" })
            modelBuilder.Entity<StockAdjustmentDetail>().Property(prop).HasPrecision(14, 2);

        modelBuilder.Entity<StockTransferMaster>().HasIndex(s => s.TransferNo).IsUnique();
        modelBuilder.Entity<StockTransferDetail>()
            .HasOne(d => d.StockTransfer).WithMany(s => s.Details).HasForeignKey(d => d.StockTransferId).OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<StockTransferDetail>()
            .HasOne(d => d.Item).WithMany().HasForeignKey(d => d.ItemId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StockTransferDetail>().Property(d => d.Quantity).HasPrecision(14, 2);

        // ── Stock Ledger / Current Stock ─────────────────────────────────────
        modelBuilder.Entity<StockLedger>()
            .HasOne(l => l.Item).WithMany().HasForeignKey(l => l.ItemId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<StockLedger>().HasIndex(l => new { l.ItemId, l.TransactionDate });
        foreach (var prop in new[] { "QtyIn", "QtyOut", "Balance", "Cost" })
            modelBuilder.Entity<StockLedger>().Property(prop).HasPrecision(14, 2);

        modelBuilder.Entity<CurrentStock>()
            .HasOne(c => c.Item).WithMany().HasForeignKey(c => c.ItemId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<CurrentStock>().HasIndex(c => c.ItemId).IsUnique();
        modelBuilder.Entity<CurrentStock>().Property(c => c.QuantityOnHand).HasPrecision(14, 2);

        modelBuilder.Entity<InventorySettings>().Property(s => s.DefaultTaxPercentage).HasPrecision(5, 2);
    }
}
