using System.Globalization;
using System.Reflection;
using ClosedXML.Excel;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using RegPay3.Data;
using RegPay3.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Payroll")
    ?? throw new InvalidOperationException("ConnectionStrings:Payroll must be configured.");
builder.Services.AddDbContext<PayrollDbContext>(options => options.UseNpgsql(connectionString));

var app = builder.Build();

// Apply the checked-in PostgreSQL migration and load the two workbook seed rows per register.
await using (var scope = app.Services.CreateAsyncScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PayrollDbContext>();
    await db.Database.MigrateAsync();
    await WorkbookSeed.EnsureSeededAsync(db);
}

app.MapGet("/", () => Results.Ok(new
{
    name = "RegPay3 payroll API",
    endpoints = new[]
    {
        "POST /api/attendance/upload?payrollNo={value}",
        "GET /api/payroll/export?payrollNo={value}",
        "GET /api/payroll/overtime-report?payrollNo={value}"
    }
}));

app.MapPost("/api/attendance/upload", async (HttpRequest request, string payrollNo, PayrollDbContext db, CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(payrollNo)) return Results.BadRequest("Payroll # is required.");
    if (!request.HasFormContentType) return Results.BadRequest("Upload must be multipart/form-data with an .xlsx file.");
    var form = await request.ReadFormAsync(ct);
    var file = form.Files.GetFile("file") ?? form.Files.FirstOrDefault();
    if (file is null || file.Length == 0) return Results.BadRequest("Choose a non-empty Excel file in the 'file' field.");
    if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
        return Results.BadRequest("Only .xlsx workbooks are supported.");

    try
    {
        await using var stream = file.OpenReadStream();
        using var workbook = new XLWorkbook(stream);
        var sheet = workbook.Worksheets.FirstOrDefault(w => w.Name.Equals("TimeAttendance", StringComparison.OrdinalIgnoreCase));
        if (sheet is null) return Results.BadRequest("Workbook must contain a TimeAttendance sheet.");
        var used = sheet.RangeUsed();
        if (used is null) return Results.BadRequest("TimeAttendance sheet is empty.");
        var headers = used.FirstRow().Cells().ToDictionary(c => Normalize(c.GetString()), c => c.Address.ColumnNumber);
        var entities = new List<TimeAttendance>();
        foreach (var row in used.RowsUsed().Skip(1))
        {
            if (row.Cells().All(c => c.IsEmpty())) continue;
            var entity = new TimeAttendance { PayrollNumber = payrollNo };
            foreach (var property in WorkbookSeed.WorkbookProperties(typeof(TimeAttendance)))
            {
                var column = property.GetCustomAttribute<WorkbookColumnAttribute>()!.Header;
                if (column == "PAYROLL #" || !headers.TryGetValue(Normalize(column), out var col)) continue;
                property.SetValue(entity, WorkbookValue.Parse(CellText(row.Cell(col)), property.PropertyType));
            }
            entities.Add(entity);
        }
        if (entities.Count == 0) return Results.BadRequest("No attendance data rows were found.");
        await db.BulkInsertAsync(entities, cancellationToken: ct);
        return Results.Ok(new { uploaded = entities.Count, payrollNo });
    }
    catch (Exception ex) when (ex is not OperationCanceledException)
    {
        return Results.BadRequest(new { error = "Unable to read or upload workbook.", detail = ex.Message });
    }
});

app.MapGet("/api/payroll/export", async (string payrollNo, PayrollDbContext db, CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(payrollNo)) return Results.BadRequest("Payroll # is required.");
    var payRows = await db.PayrollRegisters.AsNoTracking().Where(x => x.PayrollNumber == payrollNo).ToListAsync(ct);
    var overtimeRows = await db.OvertimeRegisters.AsNoTracking().Where(x => x.PayrollNumber == payrollNo).ToListAsync(ct);
    using var workbook = new XLWorkbook();
    WriteSheet(workbook, "PAYREG", typeof(PayrollRegister), payRows);
    WriteSheet(workbook, "OTR", typeof(OvertimeRegister), overtimeRows);
    using var output = new MemoryStream();
    workbook.SaveAs(output);
    return Results.File(output.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Payroll-{payrollNo}.xlsx");
});

app.MapGet("/api/payroll/overtime-report", async (string payrollNo, PayrollDbContext db, CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(payrollNo)) return Results.BadRequest("Payroll # is required.");

    var payrollRows = await db.PayrollRegisters.AsNoTracking()
        .Where(x => x.PayrollNumber == payrollNo)
        .Select(x => new
        {
            x.EmployeeId,
            x.LastName,
            x.FirstName,
            x.AttDeduction
        })
        .ToListAsync(ct);

    if (payrollRows.Count == 0) return Results.NotFound($"No PAYREG records found for payroll {payrollNo}.");

    // PAYREG column N is the employee's total overtime; OTREG stores its component rows.
    // Sum OT AND NP AMOUNT (column U), not just OT AMOUNT, so night-premium amounts are included.
    var overtimeTotals = await db.OvertimeRegisters.AsNoTracking()
        .Where(x => x.PayrollNumber == payrollNo)
        .GroupBy(x => x.EmployeeId)
        .Select(group => new
        {
            EmployeeId = group.Key,
            TotalOvertimeAmount = group.Sum(x => x.OtAndNpAmount ?? 0m)
        })
        .ToListAsync(ct);

    var overtimeByEmployee = overtimeTotals
        .Where(x => !string.IsNullOrWhiteSpace(x.EmployeeId))
        .GroupBy(x => x.EmployeeId!.Trim(), StringComparer.OrdinalIgnoreCase)
        .ToDictionary(group => group.Key, group => group.Sum(x => x.TotalOvertimeAmount), StringComparer.OrdinalIgnoreCase);

    var reportRows = payrollRows.Select(row =>
    {
        var key = row.EmployeeId?.Trim() ?? string.Empty;
        var total = overtimeByEmployee.TryGetValue(key, out var amount) ? amount : 0m;
        return new PayrollOvertimeSummary(row.EmployeeId, row.LastName, row.FirstName, row.AttDeduction, total);
    }).ToList();

    using var workbook = new XLWorkbook();
    var worksheet = workbook.Worksheets.Add("OTREG");
    var headers = new[] { "EMPLOYEE ID#", "LAST NAME", "FIRST NAME", "ATT DEDUCTION", "OVERTIME" };
    for (var column = 0; column < headers.Length; column++)
        worksheet.Cell(1, column + 1).Value = headers[column];

    var outputRow = 2;
    foreach (var row in reportRows)
    {
        worksheet.Cell(outputRow, 1).Value = row.EmployeeId ?? string.Empty;
        worksheet.Cell(outputRow, 2).Value = row.LastName ?? string.Empty;
        worksheet.Cell(outputRow, 3).Value = row.FirstName ?? string.Empty;
        WriteCellValue(worksheet.Cell(outputRow, 4), row.AttendanceDeduction);
        WriteCellValue(worksheet.Cell(outputRow, 5), row.Overtime);
        outputRow++;
    }

    worksheet.Row(1).Style.Font.Bold = true;
    worksheet.SheetView.FreezeRows(1);
    worksheet.Columns().AdjustToContents();
    worksheet.Column(4).Style.NumberFormat.Format = "0.00";
    worksheet.Column(5).Style.NumberFormat.Format = "0.00";

    using var output = new MemoryStream();
    workbook.SaveAs(output);
    return Results.File(output.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"OTREG-{payrollNo}.xlsx");
});

app.Run();

static string Normalize(string value) => string.Concat(value.Where(char.IsLetterOrDigit)).ToUpperInvariant();
static string CellText(IXLCell cell) => cell.DataType == XLDataType.DateTime
    ? cell.GetDateTime().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
    : cell.GetFormattedString(CultureInfo.InvariantCulture);
static void WriteSheet<T>(XLWorkbook workbook, string name, Type entityType, IEnumerable<T> rows)
{
    var ws = workbook.Worksheets.Add(name);
    var properties = WorkbookSeed.WorkbookProperties(entityType).ToArray();
    for (var col = 0; col < properties.Length; col++)
    {
        var header = properties[col].GetCustomAttribute<WorkbookColumnAttribute>()!.Header;
        ws.Cell(1, col + 1).Value = header;
    }
    var rowIndex = 2;
    foreach (var row in rows)
    {
        for (var col = 0; col < properties.Length; col++)
            WriteCellValue(ws.Cell(rowIndex, col + 1), properties[col].GetValue(row));
        rowIndex++;
    }
    ws.Row(1).Style.Font.Bold = true;
    ws.SheetView.FreezeRows(1);
    ws.Columns().AdjustToContents();
}

static void WriteCellValue(IXLCell cell, object? value)
{
    switch (value)
    {
        case null: cell.Value = string.Empty; break;
        case DateOnly date: cell.Value = date.ToDateTime(TimeOnly.MinValue); break;
        case decimal number: cell.Value = number; break;
        case int number: cell.Value = number; break;
        case string text: cell.Value = text; break;
        default: cell.Value = Convert.ToString(value, CultureInfo.InvariantCulture) ?? string.Empty; break;
    }
}
