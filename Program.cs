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
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IAcademicYearService, AcademicYearService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IClassService, ClassService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IParentAccessService, ParentAccessService>();
builder.Services.AddScoped<IPeriodService, PeriodService>();
builder.Services.AddScoped<ITimetableService, TimetableService>();
builder.Services.AddScoped<ISubstitutionService, SubstitutionService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IHomeworkService, HomeworkService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IPdfReportService, PdfReportService>();
builder.Services.AddScoped<IActivityLogService, ActivityLogService>();
builder.Services.AddHostedService<ActivityLogCleanupService>();
builder.Services.AddScoped<IFeeService, FeeService>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IExamService, ExamService>();
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
