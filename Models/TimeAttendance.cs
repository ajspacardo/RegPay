using System.ComponentModel.DataAnnotations;

namespace RegPay3.Models;

public sealed class TimeAttendance
{
    [Key] public long Id { get; set; }
    [WorkbookColumn("PAYROLL #", "PAYROLL #", 0)] public string? PayrollNumber { get; set; }
    [WorkbookColumn("EMP ID#", "EMP ID#", 1)] public string? EmployeeId { get; set; }
    [WorkbookColumn("LAST NAME", "LAST NAME", 2)] public string? LastName { get; set; }
    [WorkbookColumn("FIRST NAME", "FIRST NAME", 3)] public string? FirstName { get; set; }
    [WorkbookColumn("REG DAYS WRK", "REG DAYS WRK", 4)] public decimal? RegDaysWrk { get; set; }
    [WorkbookColumn("TARDY HRS", "TARDY HRS", 5)] public decimal? TardyHrs { get; set; }
    [WorkbookColumn("UNDERTIME HRS", "UNDERTIME HRS", 6)] public decimal? UndertimeHrs { get; set; }
    [WorkbookColumn("ABSENT DAYS", "ABSENT DAYS", 7)] public decimal? AbsentDays { get; set; }
    [WorkbookColumn("LWOP DAYS", "LWOP DAYS", 8)] public decimal? LwopDays { get; set; }
    [WorkbookColumn("VL DAYS", "VL DAYS", 9)] public decimal? VlDays { get; set; }
    [WorkbookColumn("SL DAYS", "SL DAYS", 10)] public decimal? SlDays { get; set; }
    [WorkbookColumn("OTHER LV DAYS", "OTHER LV DAYS", 11)] public decimal? OtherLvDays { get; set; }
    [WorkbookColumn("NITE-DIFF 10%", "NITE-DIFF 10%", 12)] public decimal? NiteDiff10 { get; set; }
    [WorkbookColumn("REGULAR-OT", "REGULAR-OT", 13)] public decimal? RegularOt { get; set; }
    [WorkbookColumn("REG-OT (NP HRS 10%)", "REG-OT (NP HRS 10%)", 14)] public decimal? RegOtNpHrs10 { get; set; }
    [WorkbookColumn("LEGAL HOL OT", "LEGAL HOL OT", 15)] public decimal? LegalHolOt { get; set; }
    [WorkbookColumn("LEGAL HOL OT (NP HRS 10%)", "LEGAL HOL OT (NP HRS 10%)", 16)] public decimal? LegalHolOtNpHrs10 { get; set; }
    [WorkbookColumn("LEGAL HOL OT > 8HRS", "LEGAL HOL OT > 8HRS", 17)] public decimal? LegalHolOt8hrs { get; set; }
    [WorkbookColumn("LEGAL HOL OT >8 (NP HRS 10%)", "LEGAL HOL OT >8 (NP HRS 10%)", 18)] public decimal? LegalHolOt8NpHrs10 { get; set; }
    [WorkbookColumn("LEGAL HOL RD", "LEGAL HOL RD", 19)] public decimal? LegalHolRd { get; set; }
    [WorkbookColumn("LEGAL HOL RD (NP HRS 10%)", "LEGAL HOL RD (NP HRS 10%)", 20)] public decimal? LegalHolRdNpHrs10 { get; set; }
    [WorkbookColumn("LEGAL HOL RD >8HRS", "LEGAL HOL RD >8HRS", 21)] public decimal? LegalHolRd8hrs { get; set; }
    [WorkbookColumn("LEGAL HOL RD >8 (NP HRS 10%)", "LEGAL HOL RD >8 (NP HRS 10%)", 22)] public decimal? LegalHolRd8NpHrs10 { get; set; }
    [WorkbookColumn("SPECL HOL OT", "SPECL HOL OT", 23)] public decimal? SpeclHolOt { get; set; }
    [WorkbookColumn("SPECL HOL OT (NP HRS 10%)", "SPECL HOL OT (NP HRS 10%)", 24)] public decimal? SpeclHolOtNpHrs10 { get; set; }
    [WorkbookColumn("SPECL HOLIDAY OT >8", "SPECL HOLIDAY OT >8", 25)] public decimal? SpeclHolidayOt8 { get; set; }
    [WorkbookColumn("SPECL HOL OT >8 (NP HRS 10%)", "SPECL HOL OT >8 (NP HRS 10%)", 26)] public decimal? SpeclHolOt8NpHrs10 { get; set; }
    [WorkbookColumn("SPECIAL HOLIDAY RD", "SPECIAL HOLIDAY RD", 27)] public decimal? SpecialHolidayRd { get; set; }
    [WorkbookColumn("SPECIAL HOL RD (NP HRS 10%)", "SPECIAL HOL RD (NP HRS 10%)", 28)] public decimal? SpecialHolRdNpHrs10 { get; set; }
    [WorkbookColumn("SPECL HOLIDAY ON RD > 8HRS", "SPECL HOLIDAY ON RD > 8HRS", 29)] public decimal? SpeclHolidayOnRd8hrs { get; set; }
    [WorkbookColumn("SPECL HOL RD > 8 (NP HRS 10%)", "SPECL HOL RD > 8 (NP HRS 10%)", 30)] public decimal? SpeclHolRd8NpHrs10 { get; set; }
    [WorkbookColumn("REST DAY OT", "REST DAY OT", 31)] public decimal? RestDayOt { get; set; }
    [WorkbookColumn("REST DAY OT (NP HRS 10%)", "REST DAY OT (NP HRS 10%)", 32)] public decimal? RestDayOtNpHrs10 { get; set; }
    [WorkbookColumn("REST DAY OT > 8HRS", "REST DAY OT > 8HRS", 33)] public decimal? RestDayOt8hrs { get; set; }
    [WorkbookColumn("REST DAY OT > 8 (NP HRS 10%)", "REST DAY OT > 8 (NP HRS 10%)", 34)] public decimal? RestDayOt8NpHrs10 { get; set; }
    [WorkbookColumn("DOUBLE HOLIDAY OT", "DOUBLE HOLIDAY OT", 35)] public decimal? DoubleHolidayOt { get; set; }
    [WorkbookColumn("DOUBLE HOLIDAY OT (NP HRS 10%)", "DOUBLE HOLIDAY OT (NP HRS 10%)", 36)] public decimal? DoubleHolidayOtNpHrs10 { get; set; }
    [WorkbookColumn("DOUBLE HOLIDAY OT > 8HRS", "DOUBLE HOLIDAY OT > 8HRS", 37)] public decimal? DoubleHolidayOt8hrs { get; set; }
    [WorkbookColumn("DOUBLE HOLIDAY OT > 8 (NP HRS 10%)", "DOUBLE HOLIDAY OT > 8 (NP HRS 10%)", 38)] public decimal? DoubleHolidayOt8NpHrs10 { get; set; }
    [WorkbookColumn("DOUBLE HOLIDAY RD", "DOUBLE HOLIDAY RD", 39)] public decimal? DoubleHolidayRd { get; set; }
    [WorkbookColumn("DOUBLE HOL RD (NP HRS 10%)", "DOUBLE HOL RD (NP HRS 10%)", 40)] public decimal? DoubleHolRdNpHrs10 { get; set; }
    [WorkbookColumn("DOUBLE HOL ON RD > 8HRS", "DOUBLE HOL ON RD > 8HRS", 41)] public decimal? DoubleHolOnRd8hrs { get; set; }
    [WorkbookColumn("DOUBLE HOL RD > 8 (NP HRS 10%)", "DOUBLE HOL RD > 8 (NP HRS 10%)", 42)] public decimal? DoubleHolRd8NpHrs10 { get; set; }
    [WorkbookColumn("Remarks", "Remarks", 43)] public string? Remarks { get; set; }
}
