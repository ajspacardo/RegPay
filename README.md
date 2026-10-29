# RegPay3 payroll API

ASP.NET Core 9 Minimal API for the `PAY_FORMULA.xlsx` exercise. The project uses no controllers.

## Database and migration

The code-first models and PostgreSQL migration create `PAYREG`, `OTREG`, and `TimeAttendance` using row 1 of each source sheet. `TimeAttendance` also includes `PAYROLL #` for associating imported rows with a payroll run. Workbook identifiers and labels are stored as text; dates use PostgreSQL `date`; payroll values and hour/day measurements use `numeric`; minute counts use `integer`. Each table has an internal identity key.

After applying the initial migration, startup ensures the first two PAYREG rows and all matching OTREG detail rows for those employees are seeded from the workbook. Seeding is idempotent and fills missing fixture rows without deleting existing data.

Set `ConnectionStrings__Payroll` to a PostgreSQL connection string, or change the sample local-development credentials in `appsettings.json`. Start with:

```powershell
dotnet run --project .\RegPay3.csproj
```

## Upload attendance

Send `POST /api/attendance/upload?payrollNo=61` as multipart form data, with the `.xlsx` workbook in the `file` field. It must contain a `TimeAttendance` sheet with the source headers in row 1. The API assigns the query parameter as `PAYROLL #` to all rows and inserts them with `EFCore.BulkExtensions`.

```powershell
curl.exe -F "file=@attendance.xlsx" "http://localhost:61571/api/attendance/upload?payrollNo=61"
```

## Export payroll registers

Request `GET /api/payroll/export?payrollNo=61`. The API returns one `.xlsx` workbook with a `PAYREG` sheet and an `OTR` sheet. Both contain rows matching the requested `PAYROLL #`.

## Regenerate workbook-derived files

`Generate-WorkbookModels.ps1` reads `PAY_FORMULA.xlsx` from the Downloads folder and regenerates the typed models, seed data, and initial migration from the workbook headers and sample rows.

## Calculated overtime report

`GET /api/payroll/overtime-report?payrollNo=61` exports one `OTREG-61.xlsx` workbook with an `OTREG` sheet and the columns EMPLOYEE ID#, LAST NAME, FIRST NAME, ATT DEDUCTION, and OVERTIME. The API reads the workbook-calculated attendance deduction from PAYREG column M and calculates overtime from the matching OTREG rows by summing `OT AND NP AMOUNT` (column U). This includes night-premium amounts and matches PAYREG column N when all OTREG rows for that payroll are present.

