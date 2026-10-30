using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR;
using QuestPDF.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SchoolManagement.API.Data;
using SchoolManagement.API.Infrastructure;
using SchoolManagement.API.Models;
using SchoolManagement.API.Helpers;
using SchoolManagement.API.Services;
using SchoolManagement.API.Hubs;
using System.Text;


QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// ── Tenant context (must be before DbContext so interceptor can use it) ───────
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantContext, TenantContext>();
builder.Services.AddScoped<TenantStampInterceptor>();
builder.Services.AddScoped<ActivityLogInterceptor>();

// ── Databases ─────────────────────────────────────────────────────────────────
// Logging DB must be registered first: ActivityLogInterceptor (attached to
// AppDbContext) writes audit rows into it.
builder.Services.AddDbContext<LoggingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("LoggingConnection")));

builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.AddInterceptors(
        sp.GetRequiredService<TenantStampInterceptor>(),
        sp.GetRequiredService<ActivityLogInterceptor>());
});

// ── JWT Authentication ────────────────────────────────────────────────────────
var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
        // SignalR sends token via query string or header
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                var accessToken = ctx.Request.Query["access_token"].ToString();
                if (string.IsNullOrEmpty(accessToken))
                    accessToken = ctx.Request.Headers.Authorization.ToString().Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);
                var path = ctx.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    (path.StartsWithSegments("/hubs", StringComparison.OrdinalIgnoreCase) ||
                     path.Value?.Contains("/hubs/", StringComparison.OrdinalIgnoreCase) == true))
                {
                    ctx.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
});
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();


// ── Services ──────────────────────────────────────────────────────────────────
builder.Services.AddScoped<JwtHelper>();
builder.Services.AddScoped<IInstituteMembershipService, InstituteMembershipService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IAcademicYearService, AcademicYearService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IClassService, ClassService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IParentAccessService, ParentAccessService>();
builder.Services.AddScoped<IPeriodService, PeriodService>();
builder.Services.AddScoped<ITimetableService, TimetableService>();
builder.Services.AddScoped<ISubstitutionService, SubstitutionService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IHomeworkService, HomeworkService>();
builder.Services.AddScoped<ICurriculumService, CurriculumService>();
builder.Services.AddScoped<IClassroomAssessmentService, ClassroomAssessmentService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IPdfReportService, PdfReportService>();
builder.Services.AddScoped<IActivityLogService, ActivityLogService>();
builder.Services.AddHostedService<ActivityLogCleanupService>();
builder.Services.AddScoped<IFeeService, FeeService>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IExamService, ExamService>();
builder.Services.Configure<SchoolManagement.API.Services.Ai.AiGenerationOptions>(
    builder.Configuration.GetSection(SchoolManagement.API.Services.Ai.AiGenerationOptions.SectionName));
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient("AiProvider");
builder.Services.AddScoped<SchoolManagement.API.Services.Ai.OpenAiCompatibleProvider>();
builder.Services.AddScoped<SchoolManagement.API.Services.Ai.FakeAiProvider>();
builder.Services.AddScoped<SchoolManagement.API.Services.Ai.IAiProvider>(sp =>
{
    var opts = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<SchoolManagement.API.Services.Ai.AiGenerationOptions>>().Value;
    if (string.Equals(opts.Provider, "Fake", StringComparison.OrdinalIgnoreCase))
        return sp.GetRequiredService<SchoolManagement.API.Services.Ai.FakeAiProvider>();
    return sp.GetRequiredService<SchoolManagement.API.Services.Ai.OpenAiCompatibleProvider>();
});
builder.Services.AddScoped<SchoolManagement.API.Services.Ai.IAiContentService, SchoolManagement.API.Services.Ai.AiContentService>();
builder.Services.AddScoped<ISettingsService, SettingsService>();
builder.Services.AddScoped<IDayScheduleService, DayScheduleService>();
builder.Services.AddScoped<IScheduleProfileService, ScheduleProfileService>();
builder.Services.AddScoped<ICalendarEventService, CalendarEventService>();
builder.Services.AddScoped<ICalendarEventTypeService, CalendarEventTypeService>();

// ── Inventory & POS Module ────────────────────────────────────────────────────
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IInventoryMasterService, InventoryMasterService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IPackageService, PackageService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();
builder.Services.AddScoped<IPurchaseReturnService, PurchaseReturnService>();
builder.Services.AddScoped<IPosService, PosService>();
builder.Services.AddScoped<ISalesReturnService, SalesReturnService>();
builder.Services.AddScoped<IInventoryReportService, InventoryReportService>();
builder.Services.AddScoped<IInventorySettingsService, InventorySettingsService>();

// ── CORS (origins come from config: appsettings.Development.json locally, ───
// ── environment variables / appsettings.Production.json in the cloud) ───────
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                      ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowConfiguredOrigins", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ── Controllers & Swagger ─────────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        // camelCase for Angular
        o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        // DateOnly support (not built-in before .NET 7 converters)
        o.JsonSerializerOptions.Converters.Add(new DateOnlyJsonConverter());
        o.JsonSerializerOptions.Converters.Add(new NullableDateOnlyJsonConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "School Management System API",
        Version = "v1",
        Description = "Backend API for School Management System"
    });

    // JWT Bearer in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token. Example: Bearer eyJhbGci..."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Include XML comments for Swagger descriptions
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// ── Middleware Pipeline ───────────────────────────────────────────────────────
// Detailed error pages only in Development — never leak stack traces publicly.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Swagger: on by default in Development; in other environments it stays off
// unless explicitly enabled via config (Swagger:Enabled / env var Swagger__Enabled),
// e.g. temporarily during cloud staging/testing before the public launch.
var swaggerEnabled = app.Environment.IsDevelopment()
                      || builder.Configuration.GetValue<bool>("Swagger:Enabled");
if (swaggerEnabled)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "School Management API v1");
        c.RoutePrefix = string.Empty;   // Swagger at root: http://localhost:5000/
    });
}

app.UseCors("AllowConfiguredOrigins");
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat", options =>
{
    options.Transports = HttpTransportType.WebSockets
                       | HttpTransportType.ServerSentEvents
                       | HttpTransportType.LongPolling;
});

// Production does not run EF MigrateAsync. Apply ONLY selective idempotent ensures
// (Users.PhotoFileId, Institutes.ModuleCurriculum, Classroom menu insert-only)
// before listening so login/menu do not fail on a missing column/row.
// Separate batches: SQL Server cannot compile CREATE INDEX on a column added in the same batch.
// Idempotent / additive only — no PasswordHash resets or full migration apply.
try
{
    using var photoScope = app.Services.CreateScope();
    var photoDb = photoScope.ServiceProvider.GetRequiredService<AppDbContext>();

    await photoDb.Database.ExecuteSqlRawAsync(@"
IF COL_LENGTH('dbo.Users', 'PhotoFileId') IS NULL
    ALTER TABLE [Users] ADD [PhotoFileId] int NULL;");

    await photoDb.Database.ExecuteSqlRawAsync(@"
IF COL_LENGTH('dbo.Users', 'PhotoFileId') IS NOT NULL
AND NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'IX_Users_PhotoFileId' AND object_id = OBJECT_ID(N'dbo.Users'))
    CREATE INDEX [IX_Users_PhotoFileId] ON [Users] ([PhotoFileId]);");

    await photoDb.Database.ExecuteSqlRawAsync(@"
IF COL_LENGTH('dbo.Users', 'PhotoFileId') IS NOT NULL
AND OBJECT_ID(N'dbo.FileStores') IS NOT NULL
AND NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_Users_FileStores_PhotoFileId')
    ALTER TABLE [Users] ADD CONSTRAINT [FK_Users_FileStores_PhotoFileId]
        FOREIGN KEY ([PhotoFileId]) REFERENCES [FileStores] ([FileId]) ON DELETE SET NULL;");

    await photoDb.Database.ExecuteSqlRawAsync(@"
IF COL_LENGTH('dbo.Users', 'PhotoFileId') IS NOT NULL
AND NOT EXISTS (
    SELECT 1 FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261002203649_AddUserProfilePhoto')
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261002203649_AddUserProfilePhoto', N'9.0.0');");

    // Login/menu touch Institutes.ModuleCurriculum (from AddCurriculumModule) without
    // applying the full curriculum migration set.
    await photoDb.Database.ExecuteSqlRawAsync(@"
IF COL_LENGTH('dbo.Institutes', 'ModuleCurriculum') IS NULL
    ALTER TABLE [Institutes] ADD [ModuleCurriculum] bit NOT NULL
        CONSTRAINT [DF_Institutes_ModuleCurriculum] DEFAULT (1);");

    // Classroom menu (Teacher Daily Workflow). Insert-only — never UPDATE existing
    // MenuItem 59 / permission 214 so admin customizations are preserved.
    await photoDb.Database.ExecuteSqlRawAsync(@"
SET IDENTITY_INSERT MenuItems ON;
IF NOT EXISTS (SELECT 1 FROM MenuItems WHERE MenuItemId = 59)
INSERT INTO MenuItems (MenuItemId, ParentId, Title, Icon, RouteUrl, SortOrder, IsActive, IsDeleted, CreatedAt, CampusId, InstituteId, CreatedBy, UpdatedAt, UpdatedBy)
VALUES (59, NULL, N'Classroom', N'play_lesson', N'/classroom/today', 12, 1, 0, '2025-01-01', NULL, NULL, NULL, NULL, NULL);
SET IDENTITY_INSERT MenuItems OFF;

SET IDENTITY_INSERT MenuRolePermissions ON;
IF NOT EXISTS (SELECT 1 FROM MenuRolePermissions WHERE Id = 214)
INSERT INTO MenuRolePermissions (Id, MenuItemId, RoleId) VALUES (214, 59, 4);
SET IDENTITY_INSERT MenuRolePermissions OFF;");

    // One-time: orphan (NULL InstituteId) school rows became invisible after admin/teacher
    // login healed Users.InstituteId. Stamp those rows onto the active school tenant.
    // Does NOT delete anything. Skips shared rows (MenuItems, system lookups, superadmin).
    await photoDb.Database.ExecuteSqlRawAsync(@"
IF NOT EXISTS (
    SELECT 1 FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'Manual_BackfillNullTenant_20261031')
BEGIN
    DECLARE @Target INT = NULL;
    DECLARE @Campus INT = NULL;
    DECLARE @SuperAdminRole INT = (
        SELECT TOP 1 RoleId FROM Roles WHERE RoleName = N'superadmin');

    SELECT TOP 1 @Target = u.InstituteId
    FROM Users u
    INNER JOIN Roles r ON r.RoleId = u.RoleId
    WHERE u.IsDeleted = 0
      AND u.InstituteId IS NOT NULL
      AND r.RoleName IN (N'admin', N'teacher', N'principal')
    ORDER BY
      CASE WHEN LOWER(u.Email) LIKE N'%@happyday.%' THEN 0 ELSE 1 END,
      u.UpdatedAt DESC;

    IF @Target IS NULL
        SELECT TOP 1 @Target = InstituteId
        FROM Institutes
        WHERE IsDeleted = 0 AND IsActive = 1
          AND REPLACE(LOWER(Name), N' ', N'') LIKE N'%happyday%'
        ORDER BY InstituteId;

    IF @Target IS NULL AND (SELECT COUNT(*) FROM Institutes WHERE IsDeleted = 0 AND IsActive = 1) = 1
        SELECT TOP 1 @Target = InstituteId FROM Institutes WHERE IsDeleted = 0 AND IsActive = 1;

    IF @Target IS NOT NULL
    BEGIN
        IF OBJECT_ID(N'dbo.Campuses', N'U') IS NOT NULL
            SELECT TOP 1 @Campus = CampusId
            FROM Campuses
            WHERE InstituteId = @Target AND IsDeleted = 0
            ORDER BY CampusId;

        IF OBJECT_ID(N'dbo.Students', N'U') IS NOT NULL
            UPDATE Students SET InstituteId = @Target, CampusId = COALESCE(CampusId, @Campus)
            WHERE InstituteId IS NULL AND IsDeleted = 0;

        IF OBJECT_ID(N'dbo.StudentGuardians', N'U') IS NOT NULL
            UPDATE StudentGuardians SET InstituteId = @Target, CampusId = COALESCE(CampusId, @Campus)
            WHERE InstituteId IS NULL AND IsDeleted = 0;

        IF OBJECT_ID(N'dbo.StudentClassEnrollments', N'U') IS NOT NULL
            UPDATE StudentClassEnrollments SET InstituteId = @Target, CampusId = COALESCE(CampusId, @Campus)
            WHERE InstituteId IS NULL AND IsDeleted = 0;

        IF OBJECT_ID(N'dbo.Classes', N'U') IS NOT NULL
            UPDATE Classes SET InstituteId = @Target, CampusId = COALESCE(CampusId, @Campus)
            WHERE InstituteId IS NULL AND IsDeleted = 0;

        IF OBJECT_ID(N'dbo.Subjects', N'U') IS NOT NULL
            UPDATE Subjects SET InstituteId = @Target, CampusId = COALESCE(CampusId, @Campus)
            WHERE InstituteId IS NULL AND IsDeleted = 0;

        IF OBJECT_ID(N'dbo.ClassSubjects', N'U') IS NOT NULL
            UPDATE ClassSubjects SET InstituteId = @Target, CampusId = COALESCE(CampusId, @Campus)
            WHERE InstituteId IS NULL AND IsDeleted = 0;

        IF OBJECT_ID(N'dbo.Staff', N'U') IS NOT NULL
            UPDATE Staff SET InstituteId = @Target, CampusId = COALESCE(CampusId, @Campus)
            WHERE InstituteId IS NULL AND IsDeleted = 0;

        IF OBJECT_ID(N'dbo.Users', N'U') IS NOT NULL
            UPDATE Users SET InstituteId = @Target, CampusId = COALESCE(CampusId, @Campus)
            WHERE InstituteId IS NULL AND IsDeleted = 0
              AND (@SuperAdminRole IS NULL OR RoleId <> @SuperAdminRole);

        IF OBJECT_ID(N'dbo.Attendances', N'U') IS NOT NULL
            UPDATE Attendances SET InstituteId = @Target, CampusId = COALESCE(CampusId, @Campus)
            WHERE InstituteId IS NULL AND IsDeleted = 0;

        IF OBJECT_ID(N'dbo.Homeworks', N'U') IS NOT NULL
            UPDATE Homeworks SET InstituteId = @Target, CampusId = COALESCE(CampusId, @Campus)
            WHERE InstituteId IS NULL AND IsDeleted = 0;

        IF OBJECT_ID(N'dbo.HomeworkSubmissions', N'U') IS NOT NULL
            UPDATE HomeworkSubmissions SET InstituteId = @Target, CampusId = COALESCE(CampusId, @Campus)
            WHERE InstituteId IS NULL AND IsDeleted = 0;

        IF OBJECT_ID(N'dbo.FeeTypes', N'U') IS NOT NULL
            UPDATE FeeTypes SET InstituteId = @Target, CampusId = COALESCE(CampusId, @Campus)
            WHERE InstituteId IS NULL AND IsDeleted = 0;

        IF OBJECT_ID(N'dbo.FeeStructures', N'U') IS NOT NULL
            UPDATE FeeStructures SET InstituteId = @Target, CampusId = COALESCE(CampusId, @Campus)
            WHERE InstituteId IS NULL AND IsDeleted = 0;

        IF OBJECT_ID(N'dbo.StudentFees', N'U') IS NOT NULL
            UPDATE StudentFees SET InstituteId = @Target, CampusId = COALESCE(CampusId, @Campus)
            WHERE InstituteId IS NULL AND IsDeleted = 0;

        IF OBJECT_ID(N'dbo.FeePayments', N'U') IS NOT NULL
            UPDATE FeePayments SET InstituteId = @Target, CampusId = COALESCE(CampusId, @Campus)
            WHERE InstituteId IS NULL AND IsDeleted = 0;

        IF OBJECT_ID(N'dbo.Periods', N'U') IS NOT NULL
            UPDATE Periods SET InstituteId = @Target, CampusId = COALESCE(CampusId, @Campus)
            WHERE InstituteId IS NULL AND IsDeleted = 0;

        IF OBJECT_ID(N'dbo.ScheduleProfiles', N'U') IS NOT NULL
            UPDATE ScheduleProfiles SET InstituteId = @Target, CampusId = COALESCE(CampusId, @Campus)
            WHERE InstituteId IS NULL AND IsDeleted = 0;

        IF OBJECT_ID(N'dbo.SchoolTimetables', N'U') IS NOT NULL
            UPDATE SchoolTimetables SET InstituteId = @Target, CampusId = COALESCE(CampusId, @Campus)
            WHERE InstituteId IS NULL AND IsDeleted = 0;

        IF OBJECT_ID(N'dbo.SchoolSettings', N'U') IS NOT NULL
            UPDATE SchoolSettings SET InstituteId = @Target, CampusId = COALESCE(CampusId, @Campus)
            WHERE InstituteId IS NULL;

        IF OBJECT_ID(N'dbo.ExamPapers', N'U') IS NOT NULL
            UPDATE ExamPapers SET InstituteId = @Target, CampusId = COALESCE(CampusId, @Campus)
            WHERE InstituteId IS NULL AND IsDeleted = 0;

        IF OBJECT_ID(N'dbo.ExamSchedules', N'U') IS NOT NULL
            UPDATE ExamSchedules SET InstituteId = @Target, CampusId = COALESCE(CampusId, @Campus)
            WHERE InstituteId IS NULL AND IsDeleted = 0;

        IF OBJECT_ID(N'dbo.ExamResults', N'U') IS NOT NULL
            UPDATE ExamResults SET InstituteId = @Target, CampusId = COALESCE(CampusId, @Campus)
            WHERE InstituteId IS NULL AND IsDeleted = 0;

        IF OBJECT_ID(N'dbo.CoursePlans', N'U') IS NOT NULL
            UPDATE CoursePlans SET InstituteId = @Target, CampusId = COALESCE(CampusId, @Campus)
            WHERE InstituteId IS NULL AND IsDeleted = 0;

        IF OBJECT_ID(N'dbo.AcademicYears', N'U') IS NOT NULL
            UPDATE AcademicYears SET InstituteId = @Target, CampusId = COALESCE(CampusId, @Campus)
            WHERE InstituteId IS NULL AND IsDeleted = 0;
    END

    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'Manual_BackfillNullTenant_20261031', N'9.0.0');
END");

    // Detach users that were bulk-stamped onto Happy Day but do not belong there.
    await photoDb.Database.ExecuteSqlRawAsync(@"
IF NOT EXISTS (
    SELECT 1 FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'Manual_DetachMismatchedHappyDayUsers_20261031')
BEGIN
    UPDATE Users
    SET InstituteId = NULL,
        CampusId = NULL,
        UpdatedAt = SYSUTCDATETIME()
    WHERE IsDeleted = 0
      AND InstituteId = 1
      AND (
            LOWER(Email) LIKE N'%te4@gmail.com%'
         OR LOWER(Email) LIKE N'vc@happyday%'
         OR LOWER(Email) LIKE N'%vc@happyday%'
         OR LOWER(FullName) LIKE N'%vc@happyday%'
      );

    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'Manual_DetachMismatchedHappyDayUsers_20261031', N'9.0.0');
END");
}
catch (Exception ex)
{
    Console.WriteLine($"Users.PhotoFileId ensure warning: {ex.Message}");
}

// ── Background Database initialization (non-blocking for fast IIS startup) ────
_ = Task.Run(async () =>
{
    try
    {
        await Task.Delay(1000); // Allow server to bind and respond to health probes first
        using var scope = app.Services.CreateScope();
        if (app.Environment.IsDevelopment())
        {
            await scope.ServiceProvider.GetRequiredService<LoggingDbContext>().Database.MigrateAsync();
            await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.MigrateAsync();
        }

        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        const string ensureChatTablesSql = @"
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ChatConversations')
BEGIN
    CREATE TABLE ChatConversations (
        ChatConversationId INT IDENTITY(1,1) PRIMARY KEY,
        Name NVARCHAR(255) NULL,
        ConversationType NVARCHAR(50) NOT NULL DEFAULT 'direct',
        ClassId INT NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        CreatedBy INT NULL,
        UpdatedBy INT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        InstituteId INT NULL,
        CampusId INT NULL
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ChatConversationMembers')
BEGIN
    CREATE TABLE ChatConversationMembers (
        MemberId INT IDENTITY(1,1) PRIMARY KEY,
        ConversationId INT NOT NULL,
        UserId INT NOT NULL,
        IsAdmin BIT NOT NULL DEFAULT 0,
        JoinedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_ChatConversationMembers_ChatConversations FOREIGN KEY (ConversationId) REFERENCES ChatConversations(ChatConversationId) ON DELETE CASCADE,
        CONSTRAINT FK_ChatConversationMembers_Users FOREIGN KEY (UserId) REFERENCES Users(UserId) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ChatMessages')
BEGIN
    CREATE TABLE ChatMessages (
        ChatMessageId INT IDENTITY(1,1) PRIMARY KEY,
        ConversationId INT NOT NULL,
        SenderId INT NOT NULL,
        Content NVARCHAR(MAX) NOT NULL DEFAULT '',
        SentAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        AttachmentUrl NVARCHAR(MAX) NULL,
        AttachmentName NVARCHAR(255) NULL,
        AttachmentType NVARCHAR(50) NULL,
        AttachmentSize BIGINT NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        CreatedBy INT NULL,
        UpdatedBy INT NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        UpdatedAt DATETIME2 NULL,
        InstituteId INT NULL,
        CampusId INT NULL,
        CONSTRAINT FK_ChatMessages_ChatConversations FOREIGN KEY (ConversationId) REFERENCES ChatConversations(ChatConversationId) ON DELETE CASCADE,
        CONSTRAINT FK_ChatMessages_Users FOREIGN KEY (SenderId) REFERENCES Users(UserId)
    );
END;

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ChatMessageReads')
BEGIN
    CREATE TABLE ChatMessageReads (
        ReadId INT IDENTITY(1,1) PRIMARY KEY,
        MessageId INT NOT NULL,
        UserId INT NOT NULL,
        ReadAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CONSTRAINT FK_ChatMessageReads_ChatMessages FOREIGN KEY (MessageId) REFERENCES ChatMessages(ChatMessageId) ON DELETE CASCADE,
        CONSTRAINT FK_ChatMessageReads_Users FOREIGN KEY (UserId) REFERENCES Users(UserId)
    );
END;";
        await db.Database.ExecuteSqlRawAsync(ensureChatTablesSql);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Background database initialization warning: {ex.Message}");
    }
});

app.Run();
