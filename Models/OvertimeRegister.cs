using System.ComponentModel.DataAnnotations;

namespace RegPay3.Models;

public sealed class OvertimeRegister
{
    [Key] public long Id { get; set; }
    [WorkbookColumn("PAYROLL #", "PAYROLL #", 0)] public string? PayrollNumber { get; set; }
    [WorkbookColumn("PAY DATE", "PAY DATE", 1)] public DateOnly? PayDate { get; set; }
    [WorkbookColumn("PAY CODE", "PAY CODE", 2)] public string? PayCode { get; set; }
    [WorkbookColumn("EMPLOYEE ID#", "EMPLOYEE ID#", 3)] public string? EmployeeId { get; set; }
    [WorkbookColumn("LAST NAME", "LAST NAME", 4)] public string? LastName { get; set; }
    [WorkbookColumn("FIRST NAME", "FIRST NAME", 5)] public string? FirstName { get; set; }
    [WorkbookColumn("FILE STATUS", "FILE STATUS", 6)] public string? FileStatus { get; set; }
    [WorkbookColumn("OT CODE", "OT CODE", 7)] public string? OtCode { get; set; }
    [WorkbookColumn("DATE", "DATE", 8)] public DateOnly? Date { get; set; }
    [WorkbookColumn("COST CENTER", "COST CENTER", 9)] public string? CostCenter { get; set; }
    [WorkbookColumn("OT RATE", "OT RATE", 10)] public decimal? OtRate { get; set; }
    [WorkbookColumn("OT HOURS", "OT HOURS", 11)] public decimal? OtHours { get; set; }
    [WorkbookColumn("OT MINUTES", "OT MINUTES", 12)] public int? OtMinutes { get; set; }
    [WorkbookColumn("OT AMOUNT", "OT AMOUNT", 13)] public decimal? OtAmount { get; set; }
    [WorkbookColumn("OT NP HOURS", "OT NP HOURS", 14)] public decimal? OtNpHours { get; set; }
    [WorkbookColumn("OT NP MINUTES", "OT NP MINUTES", 15)] public int? OtNpMinutes { get; set; }
    [WorkbookColumn("OT NP AMOUNT", "OT NP AMOUNT", 16)] public decimal? OtNpAmount { get; set; }
    [WorkbookColumn("OT NP2 HOURS", "OT NP2 HOURS", 17)] public decimal? OtNp2Hours { get; set; }
    [WorkbookColumn("OT NP2 MINUTES", "OT NP2 MINUTES", 18)] public int? OtNp2Minutes { get; set; }
    [WorkbookColumn("OT NP2 AMOUNT", "OT NP2 AMOUNT", 19)] public decimal? OtNp2Amount { get; set; }
    [WorkbookColumn("OT AND NP AMOUNT", "OT AND NP AMOUNT", 20)] public decimal? OtAndNpAmount { get; set; }
    [WorkbookColumn("RECUR START", "RECUR START", 21)] public DateOnly? RecurStart { get; set; }
    [WorkbookColumn("RECUR END", "RECUR END", 22)] public DateOnly? RecurEnd { get; set; }
    [WorkbookColumn("FREQ", "FREQ", 23)] public string? Freq { get; set; }
    [WorkbookColumn("REMARKS", "REMARKS", 24)] public string? Remarks { get; set; }
    [WorkbookColumn("SYSTEM OT HOURS", "SYSTEM OT HOURS", 25)] public decimal? SystemOtHours { get; set; }
    [WorkbookColumn("SYSTEM OT TIME", "SYSTEM OT TIME", 26)] public string? SystemOtTime { get; set; }
    [WorkbookColumn("SYSTEM NP HOURS", "SYSTEM NP HOURS", 27)] public decimal? SystemNpHours { get; set; }
    [WorkbookColumn("SYSTEM NP TIME", "SYSTEM NP TIME", 28)] public string? SystemNpTime { get; set; }
    [WorkbookColumn("FROM", "FROM", 29)] public DateOnly? From { get; set; }
    [WorkbookColumn("TO", "TO", 30)] public DateOnly? To { get; set; }
    [WorkbookColumn("JE COST CENTER", "JE COST CENTER", 31)] public string? JeCostCenter { get; set; }
}
