using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SchoolManagement.API.DTOs.Reports;

namespace SchoolManagement.API.Services;

public interface IPdfReportService
{
    byte[] DailyAttendancePdf(AttendanceDailyReportDto data);
    byte[] MonthlyAttendancePdf(AttendanceMonthlyReportDto data);
    byte[] StudentAttendancePdf(StudentAttendanceReportDto data);
    byte[] EnrollmentPdf(ClassEnrollmentReportDto data);
    byte[] HomeworkPdf(HomeworkReportDto data);
    byte[] TeacherWorkloadPdf(TeacherWorkloadReportDto data);
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
}
