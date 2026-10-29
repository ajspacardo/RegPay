namespace RegPay3.Models;

public sealed record PayrollOvertimeSummary(
    string? EmployeeId,
    string? LastName,
    string? FirstName,
    decimal? AttendanceDeduction,
    decimal Overtime);
