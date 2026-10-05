using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SchoolManagement.API.DTOs.Exam;
using SchoolManagement.API.DTOs.Reports;
using SchoolManagement.API.DTOs.Timetable;

namespace SchoolManagement.API.Services;

public interface IPdfReportService
{
    byte[] DailyAttendancePdf(AttendanceDailyReportDto data);
    byte[] MonthlyAttendancePdf(AttendanceMonthlyReportDto data);
    byte[] StudentAttendancePdf(StudentAttendanceReportDto data);
    byte[] EnrollmentPdf(ClassEnrollmentReportDto data);
    byte[] HomeworkPdf(HomeworkReportDto data);
    byte[] TeacherWorkloadPdf(TeacherWorkloadReportDto data);
    byte[] ClassTimetablePdf(ClassTimetablePdfModel data);
    byte[] ExamPaperPdf(ExamPaperPdfModel data);
}

public class PdfReportService : IPdfReportService
{
    // ── Colours ───────────────────────────────────────────────────────────────
    private static readonly string Accent   = "#1a56db";
    private static readonly string AccentBg = "#eff6ff";
    private static readonly string Green    = "#057a55";
    private static readonly string GreenBg  = "#f3faf7";
    private static readonly string Red      = "#c81e1e";
    private static readonly string RedBg    = "#fdf2f2";
    private static readonly string Orange   = "#b45309";
    private static readonly string OrangeBg = "#fffbeb";
    private static readonly string Grey     = "#374151";
    private static readonly string GreyBg   = "#f9fafb";
    private static readonly string Border   = "#e5e7eb";
    private static readonly string White    = "#ffffff";

    // ── Header shared across all reports ──────────────────────────────────────
    private static void AddHeader(IDocumentContainer container, string title, string subtitle)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(36);
            page.DefaultTextStyle(t => t.FontFamily("Arial").FontSize(9).FontColor(Grey));

            page.Header().Element(h => Header(h, title, subtitle));
            page.Footer().AlignCenter().Text(t =>
            {
                t.Span("Generated on ").FontSize(8).FontColor("#9ca3af");
                t.Span(DateTime.Now.ToString("dd MMM yyyy, hh:mm tt")).FontSize(8).FontColor("#9ca3af");
                t.Span("   |   Page ").FontSize(8).FontColor("#9ca3af");
                t.CurrentPageNumber().FontSize(8).FontColor("#9ca3af");
                t.Span(" of ").FontSize(8).FontColor("#9ca3af");
                t.TotalPages().FontSize(8).FontColor("#9ca3af");
            });
        });
    }

    private static void Header(IContainer c, string title, string subtitle)
    {
        c.Column(col =>
        {
            col.Item().Row(row =>
            {
                row.RelativeItem().Column(inner =>
                {
                    inner.Item().Text(title).FontSize(18).Bold().FontColor(Accent);
                    inner.Item().Text(subtitle).FontSize(9).FontColor("#6b7280").Italic();
                });
                row.ConstantItem(120).AlignRight().Column(inner =>
                {
                    inner.Item().Text("School Management").FontSize(10).Bold().FontColor(Accent);
                    inner.Item().Text("System").FontSize(9).FontColor("#6b7280");
                });
            });
            col.Item().PaddingTop(6).LineHorizontal(1.5f).LineColor(Accent);
            col.Item().Height(10);
        });
    }

    private static void StatRow(ColumnDescriptor col, params (string label, string value, string color, string bg)[] stats)
    {
        col.Item().Row(row =>
        {
            foreach (var (label, value, color, bg) in stats)
            {
                row.RelativeItem().Padding(4).Background(bg).Border(1).BorderColor(Border).Column(inner =>
                {
                    inner.Item().Text(value).FontSize(20).Bold().FontColor(color).AlignCenter();
                    inner.Item().Text(label).FontSize(8).FontColor(color).AlignCenter();
                });
            }
        });
        col.Item().Height(10);
    }

    private static IContainer TableHeader(IContainer c) =>
        c.Background(Accent).Padding(6);

    private static IContainer TableCell(IContainer c, bool alt = false) =>
        c.Background(alt ? GreyBg : White).BorderBottom(1).BorderColor(Border).Padding(5);

    private static string StatusColor(string status) => status.ToLower() switch
    {
        "present"    => Green,
        "absent"     => Red,
        "late"       => Orange,
        _            => Grey
    };

    // ═══════════════════════════════════════════════════════════════════════════
    // 1. Daily Attendance
    // ═══════════════════════════════════════════════════════════════════════════
    public byte[] DailyAttendancePdf(AttendanceDailyReportDto d)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(36);
                page.DefaultTextStyle(t => t.FontFamily("Arial").FontSize(9).FontColor(Grey));
                page.Header().Element(h => Header(h,
                    "Daily Attendance Report",
                    $"{d.ClassName} {d.Section}  |  {d.Date:dd MMM yyyy}"));
                page.Footer().AlignCenter().Text(t =>
                {
                    t.Span("Generated on ").FontSize(8).FontColor("#9ca3af");
                    t.Span(DateTime.Now.ToString("dd MMM yyyy, hh:mm tt")).FontSize(8).FontColor("#9ca3af");
                    t.Span("   |   Page ").FontSize(8).FontColor("#9ca3af");
                    t.CurrentPageNumber().FontSize(8).FontColor("#9ca3af");
                    t.Span(" of ").FontSize(8).FontColor("#9ca3af");
                    t.TotalPages().FontSize(8).FontColor("#9ca3af");
                });
                page.Content().Column(col =>
                {
                    StatRow(col,
                        ("Total Students", d.TotalStudents.ToString(), Accent, AccentBg),
                        ("Present",        d.Present.ToString(),       Green,  GreenBg),
                        ("Absent",         d.Absent.ToString(),        Red,    RedBg),
                        ("Late",           d.Late.ToString(),          Orange, OrangeBg),
                        ("Not Marked",     d.NotMarked.ToString(),     Grey,   GreyBg));

                    foreach (var period in d.Periods)
                    {
                        col.Item().Text(period.PeriodName).FontSize(11).Bold().FontColor(Accent);
                        col.Item().Height(4);
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(28);
                                c.RelativeColumn(3);
                                c.RelativeColumn(2);
                                c.RelativeColumn(2);
                                c.RelativeColumn(3);
                            });
                            table.Header(h =>
                            {
                                foreach (var hdr in new[] { "#", "Student Name", "Admission No", "Status", "Remarks" })
                                    h.Cell().Element(TableHeader).Text(hdr).FontColor(White).FontSize(8).Bold();
                            });
                            int i = 0;
                            foreach (var row in period.Rows)
                            {
                                bool alt = i++ % 2 == 1;
                                table.Cell().Element(c => TableCell(c, alt)).Text((i).ToString()).FontColor("#9ca3af");
                                table.Cell().Element(c => TableCell(c, alt)).Text(row.StudentName);
                                table.Cell().Element(c => TableCell(c, alt)).Text(row.AdmissionNo);
                                table.Cell().Element(c => TableCell(c, alt))
                                    .Text(row.Status).Bold().FontColor(StatusColor(row.Status));
                                table.Cell().Element(c => TableCell(c, alt)).Text(row.Remarks ?? "-").FontColor("#9ca3af");
                            }
                        });
                        col.Item().Height(14);
                    }
                });
            });
        }).GeneratePdf();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // 2. Monthly Attendance
    // ═══════════════════════════════════════════════════════════════════════════
    public byte[] MonthlyAttendancePdf(AttendanceMonthlyReportDto d)
    {
        var monthName = new DateTime(d.Year, d.Month, 1).ToString("MMMM yyyy");
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(36);
                page.DefaultTextStyle(t => t.FontFamily("Arial").FontSize(9).FontColor(Grey));
                page.Header().Element(h => Header(h,
                    "Monthly Attendance Report",
                    $"{d.ClassName} {d.Section}  |  {monthName}  |  Working Days: {d.WorkingDays}"));
                page.Footer().AlignCenter().Text(t =>
                {
                    t.Span("Generated on ").FontSize(8).FontColor("#9ca3af");
                    t.Span(DateTime.Now.ToString("dd MMM yyyy, hh:mm tt")).FontSize(8).FontColor("#9ca3af");
                    t.Span("   |   Page ").FontSize(8).FontColor("#9ca3af");
                    t.CurrentPageNumber().FontSize(8).FontColor("#9ca3af");
                    t.Span(" of ").FontSize(8).FontColor("#9ca3af");
                    t.TotalPages().FontSize(8).FontColor("#9ca3af");
                });
                page.Content().Column(col =>
                {
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(28);
                            c.RelativeColumn(3);
                            c.RelativeColumn(2);
                            c.RelativeColumn(1.5f);
                            c.RelativeColumn(1.5f);
                            c.RelativeColumn(1.5f);
                            c.RelativeColumn(2);
                        });
                        table.Header(h =>
                        {
                            foreach (var hdr in new[] { "#", "Student Name", "Admission No", "Present", "Absent", "Late", "Attendance %" })
                                h.Cell().Element(TableHeader).Text(hdr).FontColor(White).FontSize(8).Bold();
                        });
                        int i = 0;
                        foreach (var row in d.Students)
                        {
                            bool alt = i++ % 2 == 1;
                            var pct = row.AttendancePercent;
                            var pctColor = pct >= 75 ? Green : pct >= 50 ? Orange : Red;
                            table.Cell().Element(c => TableCell(c, alt)).Text((i).ToString()).FontColor("#9ca3af");
                            table.Cell().Element(c => TableCell(c, alt)).Text(row.StudentName);
                            table.Cell().Element(c => TableCell(c, alt)).Text(row.AdmissionNo);
                            table.Cell().Element(c => TableCell(c, alt)).Text(row.Present.ToString()).FontColor(Green).Bold();
                            table.Cell().Element(c => TableCell(c, alt)).Text(row.Absent.ToString()).FontColor(Red).Bold();
                            table.Cell().Element(c => TableCell(c, alt)).Text(row.Late.ToString()).FontColor(Orange).Bold();
                            table.Cell().Element(c => TableCell(c, alt)).Text($"{pct}%").FontColor(pctColor).Bold();
                        }
                    });
                });
            });
        }).GeneratePdf();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // 3. Student Attendance
    // ═══════════════════════════════════════════════════════════════════════════
    public byte[] StudentAttendancePdf(StudentAttendanceReportDto d)
    {
        var range = d.From.HasValue && d.To.HasValue
            ? $"{d.From:dd MMM yyyy} – {d.To:dd MMM yyyy}"
            : "All dates";
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(36);
                page.DefaultTextStyle(t => t.FontFamily("Arial").FontSize(9).FontColor(Grey));
                page.Header().Element(h => Header(h,
                    "Student Attendance Report",
                    $"{d.StudentName}  |  {d.AdmissionNo}  |  {d.ClassName} {d.Section}  |  {range}"));
                page.Footer().AlignCenter().Text(t =>
                {
                    t.Span("Generated on ").FontSize(8).FontColor("#9ca3af");
                    t.Span(DateTime.Now.ToString("dd MMM yyyy, hh:mm tt")).FontSize(8).FontColor("#9ca3af");
                    t.Span("   |   Page ").FontSize(8).FontColor("#9ca3af");
                    t.CurrentPageNumber().FontSize(8).FontColor("#9ca3af");
                    t.Span(" of ").FontSize(8).FontColor("#9ca3af");
                    t.TotalPages().FontSize(8).FontColor("#9ca3af");
                });
                page.Content().Column(col =>
                {
                    StatRow(col,
                        ("Total Records",   d.TotalRecords.ToString(),           Accent, AccentBg),
                        ("Present",         d.Present.ToString(),                Green,  GreenBg),
                        ("Absent",          d.Absent.ToString(),                 Red,    RedBg),
                        ("Late",            d.Late.ToString(),                   Orange, OrangeBg),
                        ("Attendance %",    $"{d.AttendancePercent}%",           d.AttendancePercent >= 75 ? Green : Red, d.AttendancePercent >= 75 ? GreenBg : RedBg));

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(28);
                            c.RelativeColumn(2);
                            c.RelativeColumn(2.5f);
                            c.RelativeColumn(2);
                            c.RelativeColumn(3);
                        });
                        table.Header(h =>
                        {
                            foreach (var hdr in new[] { "#", "Date", "Period", "Status", "Remarks" })
                                h.Cell().Element(TableHeader).Text(hdr).FontColor(White).FontSize(8).Bold();
                        });
                        int i = 0;
                        foreach (var row in d.Days)
                        {
                            bool alt = i++ % 2 == 1;
                            table.Cell().Element(c => TableCell(c, alt)).Text((i).ToString()).FontColor("#9ca3af");
                            table.Cell().Element(c => TableCell(c, alt)).Text(row.Date.ToString("dd MMM yyyy"));
                            table.Cell().Element(c => TableCell(c, alt)).Text(row.PeriodName);
                            table.Cell().Element(c => TableCell(c, alt)).Text(row.Status).Bold().FontColor(StatusColor(row.Status));
                            table.Cell().Element(c => TableCell(c, alt)).Text(row.Remarks ?? "-").FontColor("#9ca3af");
                        }
                    });
                });
            });
        }).GeneratePdf();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // 4. Enrollment
    // ═══════════════════════════════════════════════════════════════════════════
    public byte[] EnrollmentPdf(ClassEnrollmentReportDto d)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(36);
                page.DefaultTextStyle(t => t.FontFamily("Arial").FontSize(9).FontColor(Grey));
                page.Header().Element(h => Header(h,
                    "Enrollment Report",
                    $"{d.AcademicYearLabel}  |  {d.TotalClasses} Classes  |  {d.TotalStudents} Students"));
                page.Footer().AlignCenter().Text(t =>
                {
                    t.Span("Generated on ").FontSize(8).FontColor("#9ca3af");
                    t.Span(DateTime.Now.ToString("dd MMM yyyy, hh:mm tt")).FontSize(8).FontColor("#9ca3af");
                    t.Span("   |   Page ").FontSize(8).FontColor("#9ca3af");
                    t.CurrentPageNumber().FontSize(8).FontColor("#9ca3af");
                    t.Span(" of ").FontSize(8).FontColor("#9ca3af");
                    t.TotalPages().FontSize(8).FontColor("#9ca3af");
                });
                page.Content().Column(col =>
                {
                    StatRow(col,
                        ("Total Classes",   d.TotalClasses.ToString(),  Accent, AccentBg),
                        ("Total Students",  d.TotalStudents.ToString(), Green,  GreenBg));

                    foreach (var cls in d.Classes)
                    {
                        col.Item().Text($"{cls.ClassName} {cls.Section}  ({cls.TotalStudents} students)")
                            .FontSize(11).Bold().FontColor(Accent);
                        col.Item().Height(4);
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(28);
                                c.RelativeColumn(3);
                                c.RelativeColumn(2);
                                c.RelativeColumn(1.5f);
                                c.RelativeColumn(2);
                                c.RelativeColumn(2);
                            });
                            table.Header(h =>
                            {
                                foreach (var hdr in new[] { "#", "Student Name", "Admission No", "Gender", "Admission Date", "Status" })
                                    h.Cell().Element(TableHeader).Text(hdr).FontColor(White).FontSize(8).Bold();
                            });
                            int i = 0;
                            foreach (var s in cls.Students)
                            {
                                bool alt = i++ % 2 == 1;
                                var sColor = s.Status.ToLower() == "active" ? Green : s.Status.ToLower() == "withdrawn" ? Red : Orange;
                                table.Cell().Element(c => TableCell(c, alt)).Text((i).ToString()).FontColor("#9ca3af");
                                table.Cell().Element(c => TableCell(c, alt)).Text(s.StudentName);
                                table.Cell().Element(c => TableCell(c, alt)).Text(s.AdmissionNo);
                                table.Cell().Element(c => TableCell(c, alt)).Text(s.Gender ?? "-");
                                table.Cell().Element(c => TableCell(c, alt)).Text(s.AdmissionDate.ToString("dd MMM yyyy"));
                                table.Cell().Element(c => TableCell(c, alt)).Text(s.Status).Bold().FontColor(sColor);
                            }
                        });
                        col.Item().Height(14);
                    }
                });
            });
        }).GeneratePdf();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // 5. Homework
    // ═══════════════════════════════════════════════════════════════════════════
    public byte[] HomeworkPdf(HomeworkReportDto d)
    {
        var range = d.From.HasValue && d.To.HasValue
            ? $"{d.From:dd MMM yyyy} – {d.To:dd MMM yyyy}"
            : "All dates";
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(36);
                page.DefaultTextStyle(t => t.FontFamily("Arial").FontSize(9).FontColor(Grey));
                page.Header().Element(h => Header(h,
                    "Homework Report",
                    $"{d.ClassName} {d.Section}  |  {range}  |  {d.TotalHomework} assignments"));
                page.Footer().AlignCenter().Text(t =>
                {
                    t.Span("Generated on ").FontSize(8).FontColor("#9ca3af");
                    t.Span(DateTime.Now.ToString("dd MMM yyyy, hh:mm tt")).FontSize(8).FontColor("#9ca3af");
                    t.Span("   |   Page ").FontSize(8).FontColor("#9ca3af");
                    t.CurrentPageNumber().FontSize(8).FontColor("#9ca3af");
                    t.Span(" of ").FontSize(8).FontColor("#9ca3af");
                    t.TotalPages().FontSize(8).FontColor("#9ca3af");
                });
                page.Content().Column(col =>
                {
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(28);
                            c.RelativeColumn(3);
                            c.RelativeColumn(2);
                            c.RelativeColumn(2);
                            c.RelativeColumn(2);
                            c.RelativeColumn(2);
                            c.RelativeColumn(1.5f);
                            c.RelativeColumn(1.5f);
                            c.RelativeColumn(1.5f);
                            c.RelativeColumn(2);
                        });
                        table.Header(h =>
                        {
                            foreach (var hdr in new[] { "#", "Title", "Subject", "Teacher", "Assigned", "Due Date", "Students", "Submitted", "Reviewed", "Submit %" })
                                h.Cell().Element(TableHeader).Text(hdr).FontColor(White).FontSize(8).Bold();
                        });
                        int i = 0;
                        foreach (var row in d.Homework)
                        {
                            bool alt = i++ % 2 == 1;
                            var pct = row.SubmissionPercent;
                            var pctColor = pct >= 75 ? Green : pct >= 50 ? Orange : Red;
                            table.Cell().Element(c => TableCell(c, alt)).Text((i).ToString()).FontColor("#9ca3af");
                            table.Cell().Element(c => TableCell(c, alt)).Text(row.Title);
                            table.Cell().Element(c => TableCell(c, alt)).Text(row.SubjectName);
                            table.Cell().Element(c => TableCell(c, alt)).Text(row.TeacherName);
                            table.Cell().Element(c => TableCell(c, alt)).Text(row.AssignedDate.ToString("dd MMM yyyy"));
                            table.Cell().Element(c => TableCell(c, alt)).Text(row.DueDate.ToString("dd MMM yyyy"));
                            table.Cell().Element(c => TableCell(c, alt)).Text(row.TotalStudents.ToString());
                            table.Cell().Element(c => TableCell(c, alt)).Text(row.Submitted.ToString()).FontColor(Green).Bold();
                            table.Cell().Element(c => TableCell(c, alt)).Text(row.Reviewed.ToString()).FontColor(Accent).Bold();
                            table.Cell().Element(c => TableCell(c, alt)).Text($"{pct}%").FontColor(pctColor).Bold();
                        }
                    });
                });
            });
        }).GeneratePdf();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // 6. Teacher Workload
    // ═══════════════════════════════════════════════════════════════════════════
    public byte[] TeacherWorkloadPdf(TeacherWorkloadReportDto d)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(36);
                page.DefaultTextStyle(t => t.FontFamily("Arial").FontSize(9).FontColor(Grey));
                page.Header().Element(h => Header(h,
                    "Teacher Workload Report",
                    $"{d.AcademicYearLabel}  |  {d.Teachers.Count} Teachers"));
                page.Footer().AlignCenter().Text(t =>
                {
                    t.Span("Generated on ").FontSize(8).FontColor("#9ca3af");
                    t.Span(DateTime.Now.ToString("dd MMM yyyy, hh:mm tt")).FontSize(8).FontColor("#9ca3af");
                    t.Span("   |   Page ").FontSize(8).FontColor("#9ca3af");
                    t.CurrentPageNumber().FontSize(8).FontColor("#9ca3af");
                    t.Span(" of ").FontSize(8).FontColor("#9ca3af");
                    t.TotalPages().FontSize(8).FontColor("#9ca3af");
                });
                page.Content().Column(col =>
                {
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(c =>
                        {
                            c.ConstantColumn(28);
                            c.RelativeColumn(3);
                            c.RelativeColumn(3);
                            c.RelativeColumn(2);
                            c.RelativeColumn(2);
                            c.RelativeColumn(4);
                        });
                        table.Header(h =>
                        {
                            foreach (var hdr in new[] { "#", "Teacher Name", "Email", "Periods/Week", "HW Assigned", "Classes" })
                                h.Cell().Element(TableHeader).Text(hdr).FontColor(White).FontSize(8).Bold();
                        });
                        int i = 0;
                        foreach (var row in d.Teachers)
                        {
                            bool alt = i++ % 2 == 1;
                            table.Cell().Element(c => TableCell(c, alt)).Text((i).ToString()).FontColor("#9ca3af");
                            table.Cell().Element(c => TableCell(c, alt)).Text(row.TeacherName).Bold();
                            table.Cell().Element(c => TableCell(c, alt)).Text(row.Email).FontColor("#6b7280");
                            table.Cell().Element(c => TableCell(c, alt)).Text(row.TimetablePeriodsPerWeek.ToString()).FontColor(Accent).Bold();
                            table.Cell().Element(c => TableCell(c, alt)).Text(row.HomeworkAssigned.ToString()).FontColor(Green).Bold();
                            table.Cell().Element(c => TableCell(c, alt)).Text(string.Join(", ", row.ClassesTeaching)).FontColor("#6b7280");
                        }
                    });
                });
            });
        }).GeneratePdf();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // 7. Class Timetable (landscape — no browser chrome)
    // ═══════════════════════════════════════════════════════════════════════════
    public byte[] ClassTimetablePdf(ClassTimetablePdfModel d)
    {
        static string DayLabel(int day) => day switch
        {
            1 => "MON", 2 => "TUE", 3 => "WED", 4 => "THU",
            5 => "FRI", 6 => "SAT", 7 => "SUN", _ => day.ToString()
        };

        var days = d.Days.Count > 0 ? d.Days : new List<int> { 1, 2, 3, 4, 5 };

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(28);
                page.DefaultTextStyle(t => t.FontFamily("Arial").FontSize(9).FontColor(Grey));

                page.Header().Column(col =>
                {
                    col.Item().AlignCenter().Text(d.SchoolName).FontSize(16).Bold().FontColor("#0f172a");
                    if (!string.IsNullOrWhiteSpace(d.CampusName))
                        col.Item().AlignCenter().Text(d.CampusName!).FontSize(11).SemiBold().FontColor("#334155");
                    col.Item().PaddingTop(4).AlignCenter().Text("CLASS TIMETABLE")
                        .FontSize(10).Bold().FontColor("#475569");
                    col.Item().AlignCenter().Text(d.ClassLabel).FontSize(11).SemiBold().FontColor("#0f172a");
                    col.Item().PaddingTop(6).LineHorizontal(1.5f).LineColor("#0f172a");
                    col.Item().Height(8);
                });

                page.Footer().PaddingTop(6).BorderTop(1).BorderColor("#94a3b8").PaddingTop(6)
                    .Row(row =>
                    {
                        row.RelativeItem().Text($"Printed by: {d.PrintedBy}").FontSize(8).FontColor("#334155");
                        row.RelativeItem().AlignRight()
                            .Text($"Print on: {d.PrintedOn:MMM d, yyyy, h:mm tt}").FontSize(8).FontColor("#334155");
                    });

                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(c =>
                    {
                        c.ConstantColumn(28);
                        c.ConstantColumn(72);
                        foreach (var _ in days) c.RelativeColumn();
                    });

                    table.Header(h =>
                    {
                        h.Cell().Background("#f1f5f9").Border(1).BorderColor("#94a3b8").Padding(5)
                            .AlignCenter().Text("#").FontSize(8).Bold().FontColor("#475569");
                        h.Cell().Background("#f1f5f9").Border(1).BorderColor("#94a3b8").Padding(5)
                            .Text("TIME").FontSize(8).Bold().FontColor("#475569");
                        foreach (var day in days)
                        {
                            h.Cell().Background("#f1f5f9").Border(1).BorderColor("#94a3b8").Padding(5)
                                .Text(DayLabel(day)).FontSize(8).Bold().FontColor("#475569");
                        }
                    });

                    foreach (var period in d.Periods)
                    {
                        var bg = period.IsBreak ? "#fffbeb" : White;

                        table.Cell().Background(bg).Border(1).BorderColor("#94a3b8").Padding(5)
                            .AlignCenter().AlignMiddle()
                            .Text(period.IsBreak ? "—" : period.PeriodNo.ToString())
                            .FontSize(9).Bold();

                        table.Cell().Background(bg).Border(1).BorderColor("#94a3b8").Padding(5)
                            .AlignMiddle()
                            .Text($"{period.StartTime}–{period.EndTime}")
                            .FontSize(8).SemiBold();

                        foreach (var day in days)
                        {
                            var cell = period.Cells.TryGetValue(day, out var v) ? v : null;
                            var containerCell = table.Cell().Background(bg).Border(1).BorderColor("#94a3b8").Padding(4).MinHeight(28);

                            if (period.IsBreak)
                            {
                                containerCell.AlignCenter().AlignMiddle()
                                    .Text("Break").FontSize(9).Bold().FontColor("#92400e");
                            }
                            else if (cell is null || string.IsNullOrWhiteSpace(cell.SubjectName))
                            {
                                containerCell.Text(string.Empty);
                            }
                            else
                            {
                                containerCell.Column(inner =>
                                {
                                    inner.Item().Text(cell.SubjectName).FontSize(9).Bold().FontColor("#0f172a");
                                    if (!string.IsNullOrWhiteSpace(cell.TeacherName))
                                        inner.Item().Text(cell.TeacherName).FontSize(7.5f).FontColor("#475569");
                                });
                            }
                        }
                    }
                });
            });
        }).GeneratePdf();
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // 8. Exam Paper (student copy / optional answer key)
    // ═══════════════════════════════════════════════════════════════════════════
    public byte[] ExamPaperPdf(ExamPaperPdfModel d)
    {
        var ink = "#0f172a";
        var muted = "#475569";
        var line = "#94a3b8";

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(36);
                page.DefaultTextStyle(t => t.FontFamily("Arial").FontSize(10).FontColor(ink));

                page.Header().Column(col =>
                {
                    col.Item().AlignCenter().Text(d.SchoolName).FontSize(16).Bold().FontColor(ink);
                    if (!string.IsNullOrWhiteSpace(d.CampusName))
                        col.Item().AlignCenter().Text(d.CampusName!).FontSize(10).SemiBold().FontColor(muted);
                    col.Item().PaddingTop(4).AlignCenter()
                        .Text(d.IncludeAnswers ? "EXAM PAPER — ANSWER KEY" : "EXAMINATION PAPER")
                        .FontSize(11).Bold().FontColor(muted);
                    col.Item().PaddingTop(6).AlignCenter().Text(d.Title).FontSize(13).Bold().FontColor(ink);
                    col.Item().PaddingTop(8).LineHorizontal(1.5f).LineColor(ink);
                    col.Item().Height(8);

                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text($"Class: {d.ClassName}").FontSize(9).SemiBold();
                        row.RelativeItem().AlignCenter().Text($"Subject: {d.SubjectName}").FontSize(9).SemiBold();
                        row.RelativeItem().AlignRight().Text($"Year: {d.AcademicYear}").FontSize(9).SemiBold();
                    });
                    col.Item().PaddingTop(3).Row(row =>
                    {
                        row.RelativeItem().Text($"Type: {d.ExamType}").FontSize(9);
                        row.RelativeItem().AlignCenter().Text($"Total Marks: {d.TotalMarks}").FontSize(9).SemiBold();
                        row.RelativeItem().AlignRight().Text(
                            d.DurationMinutes.HasValue
                                ? $"Time: {d.DurationMinutes} min"
                                : $"Pass: {d.PassMarks}"
                        ).FontSize(9);
                    });
                    if (d.DurationMinutes.HasValue)
                        col.Item().PaddingTop(2).Text($"Pass Marks: {d.PassMarks}").FontSize(9);

                    if (!string.IsNullOrWhiteSpace(d.SyllabusNote))
                        col.Item().PaddingTop(4).Text($"Syllabus: {d.SyllabusNote}").FontSize(8).Italic().FontColor(muted);

                    if (!string.IsNullOrWhiteSpace(d.Instructions))
                    {
                        col.Item().PaddingTop(6).Background("#f8fafc").Border(1).BorderColor(line).Padding(8)
                            .Column(inner =>
                            {
                                inner.Item().Text("Instructions").FontSize(9).Bold();
                                inner.Item().PaddingTop(2).Text(d.Instructions!).FontSize(8).FontColor(muted);
                            });
                    }

                    col.Item().PaddingTop(6).Row(row =>
                    {
                        row.RelativeItem().Text("Name: _______________________________").FontSize(9);
                        row.RelativeItem().Text("Roll No: ____________").FontSize(9);
                        row.RelativeItem().AlignRight().Text("Date: ____________").FontSize(9);
                    });
                    col.Item().PaddingTop(8).LineHorizontal(0.75f).LineColor(line);
                    col.Item().Height(6);
                });

                page.Footer().PaddingTop(4).BorderTop(1).BorderColor(line).PaddingTop(4)
                    .Row(row =>
                    {
                        row.RelativeItem().Text($"Printed by: {d.PrintedBy}").FontSize(8).FontColor(muted);
                        row.RelativeItem().AlignCenter().Text(t =>
                        {
                            t.Span("Page ").FontSize(8).FontColor(muted);
                            t.CurrentPageNumber().FontSize(8).FontColor(muted);
                            t.Span(" of ").FontSize(8).FontColor(muted);
                            t.TotalPages().FontSize(8).FontColor(muted);
                        });
                        row.RelativeItem().AlignRight()
                            .Text($"Print on: {d.PrintedOn:MMM d, yyyy, h:mm tt}").FontSize(8).FontColor(muted);
                    });

                page.Content().Column(col =>
                {
                    foreach (var section in d.Sections)
                    {
                        col.Item().PaddingTop(8).PaddingBottom(4).Row(row =>
                        {
                            row.RelativeItem().Text(section.SectionName).FontSize(11).Bold().FontColor(ink);
                            row.ConstantItem(90).AlignRight()
                                .Text($"[{section.AllocatedMarks} Marks]").FontSize(9).SemiBold().FontColor(muted);
                        });

                        if (!string.IsNullOrWhiteSpace(section.SectionNote) || section.AttemptQuestions.HasValue)
                        {
                            var note = section.SectionNote ?? "";
                            if (section.AttemptQuestions.HasValue)
                                note = string.IsNullOrWhiteSpace(note)
                                    ? $"Attempt any {section.AttemptQuestions}"
                                    : $"{note}";
                            col.Item().PaddingBottom(4).Text(note).FontSize(8).Italic().FontColor(muted);
                        }

                        if (section.Questions.Count == 0)
                        {
                            col.Item().PaddingBottom(6).Text("No questions in this section.")
                                .FontSize(8).Italic().FontColor("#94a3b8");
                            continue;
                        }

                        foreach (var q in section.Questions)
                        {
                            col.Item().PaddingBottom(8).Column(qCol =>
                            {
                                qCol.Item().Row(row =>
                                {
                                    row.RelativeItem().Text($"Q{q.Number}. {q.QuestionText}")
                                        .FontSize(10).FontColor(ink);
                                    row.ConstantItem(42).AlignRight()
                                        .Text($"({q.Marks})").FontSize(9).FontColor(muted);
                                });

                                if (!string.IsNullOrWhiteSpace(q.QuestionNote))
                                    qCol.Item().PaddingLeft(18).PaddingTop(2)
                                        .Text(q.QuestionNote!).FontSize(8).Italic().FontColor(muted);

                                if (q.QuestionTypeId == 1) // MultipleChoice
                                {
                                    foreach (var opt in q.Options)
                                    {
                                        var mark = d.IncludeAnswers && opt.IsCorrect ? " *" : "";
                                        qCol.Item().PaddingLeft(18).PaddingTop(2)
                                            .Text($"({opt.Label})  {opt.Text}{mark}")
                                            .FontSize(9)
                                            .FontColor(d.IncludeAnswers && opt.IsCorrect ? Green : ink);
                                    }
                                }
                                else if (q.QuestionTypeId == 2) // TrueFalse
                                {
                                    var ans = d.IncludeAnswers && q.IsTrue.HasValue
                                        ? (q.IsTrue.Value ? "  [Answer: True]" : "  [Answer: False]")
                                        : "";
                                    qCol.Item().PaddingLeft(18).PaddingTop(2)
                                        .Text($"[ ] True     [ ] False{ans}").FontSize(9);
                                }
                                else if (q.QuestionTypeId == 3) // FillInBlanks
                                {
                                    if (d.IncludeAnswers && !string.IsNullOrWhiteSpace(q.CorrectAnswer))
                                        qCol.Item().PaddingLeft(18).PaddingTop(2)
                                            .Text($"Answer: {q.CorrectAnswer}").FontSize(9).FontColor(Green);
                                    else
                                        qCol.Item().PaddingLeft(18).PaddingTop(2)
                                            .Text("______________________________").FontSize(9).FontColor(muted);
                                }
                                else
                                {
                                    // Short / Long — ruled answer lines with handwriting height (~24pt)
                                    if (d.IncludeAnswers && !string.IsNullOrWhiteSpace(q.CorrectAnswer))
                                    {
                                        qCol.Item().PaddingLeft(18).PaddingTop(2)
                                            .Text($"Model answer: {q.CorrectAnswer}")
                                            .FontSize(8).Italic().FontColor(Green);
                                    }
                                    else
                                    {
                                        var lines = q.QuestionTypeId == 5 ? 5 : 3;
                                        const float lineHeight = 24f;
                                        qCol.Item().PaddingLeft(18).PaddingTop(4).Column(linesCol =>
                                        {
                                            for (var i = 0; i < lines; i++)
                                            {
                                                linesCol.Item().Height(lineHeight).AlignBottom()
                                                    .LineHorizontal(0.6f).LineColor(line);
                                            }
                                        });
                                    }
                                }
                            });
                        }
                    }
                });
            });
        }).GeneratePdf();
    }
}
