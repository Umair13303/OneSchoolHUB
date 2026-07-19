using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SchoolManagement.API.Data;
using SchoolManagement.API.Models;
using SchoolManagement.API.Services;
using System.Drawing;
using System.Security.Claims;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/students/import")]
[Authorize(Roles = "superadmin,admin,principal")]
[Produces("application/json")]
public class StudentImportController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IStudentService _studentSvc;

    public StudentImportController(AppDbContext db, IStudentService studentSvc)
    {
        _db = db;
        _studentSvc = studentSvc;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    /// <summary>
    /// GET /api/students/import/template?academicYearId=&classId=
    /// Returns a filled .xlsx template the user downloads and fills in.
    /// </summary>
    [HttpGet("template")]
    public async Task<IActionResult> DownloadTemplate(
        [FromQuery] int academicYearId,
        [FromQuery] int classId)
    {
        var cls  = await _db.Classes.FindAsync(classId);
        var year = await _db.AcademicYears.FindAsync(academicYearId);
        if (cls is null || year is null)
            return BadRequest(new { error = "Invalid academicYearId or classId." });

        var className = cls.ClassName + (cls.Section is not null ? " " + cls.Section : "");
        var fileName  = $"StudentImport_{year.YearLabel}_{className}.xlsx"
            .Replace(" ", "_").Replace("/", "-");

        using var pkg = new ExcelPackage();
        var ws = pkg.Workbook.Worksheets.Add("Students");

        // ── Header row ─────────────────────────────────────────────────────
        string[] headers =
        [
            "FirstName*", "LastName*", "DateOfBirth\n(DD/MM/YYYY)",
            "Gender\n(Male/Female/Other)", "BloodGroup\n(A+/A-/B+/B-/O+/O-/AB+/AB-)",
            "Religion", "Nationality", "Address", "Phone", "Email",
            "AdmissionDate\n(DD/MM/YYYY)", "ExistingAdmissionNo\n(leave blank=auto)",
            "GuardianName*", "GuardianPhone*",
            "GuardianRelation\n(Father/Mother/Guardian)", "GuardianCNIC", "GuardianOccupation"
        ];

        for (int c = 0; c < headers.Length; c++)
        {
            var cell = ws.Cells[1, c + 1];
            cell.Value = headers[c];
            cell.Style.Font.Bold = true;
            cell.Style.Font.Color.SetColor(Color.White);
            cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
            cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(30, 58, 95));
            cell.Style.WrapText = true;
            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            cell.Style.VerticalAlignment   = ExcelVerticalAlignment.Center;
        }

        // ── Info banner ────────────────────────────────────────────────────
        ws.Cells[1, headers.Length + 2].Value =
            $"Class: {className}  |  Year: {year.YearLabel}  |  * = required";
        ws.Cells[1, headers.Length + 2].Style.Font.Italic = true;
        ws.Cells[1, headers.Length + 2].Style.Font.Color.SetColor(Color.FromArgb(100, 100, 100));

        // ── Two sample rows ────────────────────────────────────────────────
        object[][] samples =
        [
            ["Muhammad", "Ali", "01/01/2015", "Male", "B+", "Islam", "Pakistani",
             "123 Main St, Lahore", "0300-1234567", "student@email.com",
             "01/04/2024", "", "Ahmad Ali", "0300-9876543", "Father", "35202-1234567-1", "Business"],
            ["Ayesha", "Khan", "15/06/2014", "Female", "A+", "Islam", "Pakistani",
             "456 Garden Town, Karachi", "0321-7654321", "",
             "01/04/2024", "ADM-2022-0045", "Noor Khan", "0321-1234567", "Father", "42201-9876543-2", "Teacher"]
        ];

        for (int r = 0; r < samples.Length; r++)
        {
            for (int c = 0; c < samples[r].Length; c++)
                ws.Cells[r + 2, c + 1].Value = samples[r][c];

            // Shade sample rows lightly
            ws.Cells[r + 2, 1, r + 2, headers.Length].Style.Fill.PatternType = ExcelFillStyle.Solid;
            ws.Cells[r + 2, 1, r + 2, headers.Length].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(240, 246, 255));
        }

        // ── Format & freeze ───────────────────────────────────────────────
        ws.Row(1).Height = 36;
        ws.View.FreezePanes(2, 1);
        for (int c = 1; c <= headers.Length; c++)
            ws.Column(c).Width = 22;
        ws.Column(8).Width = 30; // address wider

        ws.Cells[ws.Dimension.Address].Style.Border.BorderAround(ExcelBorderStyle.Thin);

        var bytes = pkg.GetAsByteArray();
        return File(bytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    /// <summary>
    /// POST /api/students/import?academicYearId=&classId=
    /// Accepts multipart/form-data with field "file" (.xlsx).
    /// Parses each row, attempts to create the student, and returns per-row results.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "admin,principal")]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB
    public async Task<IActionResult> Import(
        [FromQuery] int academicYearId,
        [FromQuery] int classId,
        IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { error = "No file uploaded." });

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (ext != ".xlsx")
            return BadRequest(new { error = "Only .xlsx files are accepted." });

        var cls  = await _db.Classes.FindAsync(classId);
        var year = await _db.AcademicYears.FindAsync(academicYearId);
        if (cls is null || year is null)
            return BadRequest(new { error = "Invalid academicYearId or classId." });

        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var stream = file.OpenReadStream();
        using var pkg    = new ExcelPackage(stream);
        var ws = pkg.Workbook.Worksheets[0];

        if (ws is null || ws.Dimension is null || ws.Dimension.Rows < 2)
            return BadRequest(new { error = "Spreadsheet is empty or has no data rows." });

        var createdBy = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : 0;
        var results   = new List<ImportRowResult>();

        for (int r = 2; r <= ws.Dimension.Rows; r++)
        {
            string Cell(int c) => ws.Cells[r, c].Text.Trim();

            var firstName = Cell(1);
            var lastName  = Cell(2);

            // Skip completely blank rows
            if (string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName))
                continue;

            var rowResult = new ImportRowResult { Row = r - 1, Name = $"{firstName} {lastName}".Trim() };

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            {
                rowResult.Success = false;
                rowResult.Error   = "FirstName and LastName are required.";
                results.Add(rowResult);
                continue;
            }

            var guardianName  = Cell(13);
            var guardianPhone = Cell(14);
            if (string.IsNullOrWhiteSpace(guardianName) || string.IsNullOrWhiteSpace(guardianPhone))
            {
                rowResult.Success = false;
                rowResult.Error   = "GuardianName and GuardianPhone are required.";
                results.Add(rowResult);
                continue;
            }

            // Parse optional dates
            DateOnly? dob          = TryParseDate(Cell(3));
            DateOnly? admissionDate = TryParseDate(Cell(11));
            var customAdmNo        = Cell(12);

            var dto = new DTOs.Students.CreateStudentAdmissionDto
            {
                FirstName        = firstName,
                LastName         = lastName,
                DateOfBirth      = dob,
                Gender           = NullIfEmpty(Cell(4)),
                BloodGroup       = NullIfEmpty(Cell(5)),
                Religion         = NullIfEmpty(Cell(6)),
                Nationality      = NullIfEmpty(Cell(7)),
                Address          = NullIfEmpty(Cell(8)),
                Phone            = NullIfEmpty(Cell(9)),
                Email            = NullIfEmpty(Cell(10)),
                AdmissionDate    = admissionDate,
                CustomAdmissionNo = NullIfEmpty(customAdmNo),
                AcademicYearId   = academicYearId,
                ClassId          = classId,
                Guardians =
                [
                    new DTOs.Students.CreateGuardianDto
                    {
                        FullName   = guardianName,
                        Phone      = guardianPhone,
                        Relation   = string.IsNullOrWhiteSpace(Cell(15)) ? "Father" : Cell(15),
                        CNIC       = NullIfEmpty(Cell(16)),
                        Occupation = NullIfEmpty(Cell(17))
                    }
                ]
            };

            try
            {
                var created = await _studentSvc.CreateAdmissionAsync(dto, createdBy);
                rowResult.Success     = true;
                rowResult.AdmissionNo = created.AdmissionNo;
            }
            catch (Exception ex)
            {
                rowResult.Success = false;
                rowResult.Error   = ex.Message;
            }

            results.Add(rowResult);
        }

        return Ok(new
        {
            totalRows    = results.Count,
            successCount = results.Count(x => x.Success),
            failCount    = results.Count(x => !x.Success),
            results
        });
    }

    // ── Helpers ──────────────────────────────────────────────────────────────
    private static DateOnly? TryParseDate(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return null;
        if (DateOnly.TryParseExact(s, ["dd/MM/yyyy", "d/M/yyyy", "yyyy-MM-dd", "MM/dd/yyyy"], null,
                System.Globalization.DateTimeStyles.None, out var d))
            return d;
        return null;
    }

    private static string? NullIfEmpty(string s) =>
        string.IsNullOrWhiteSpace(s) ? null : s;
}

public class ImportRowResult
{
    public int    Row         { get; set; }
    public string Name        { get; set; } = "";
    public bool   Success     { get; set; }
    public string? AdmissionNo { get; set; }
    public string? Error       { get; set; }
}
