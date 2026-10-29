using System.ComponentModel.DataAnnotations;

namespace RegPay3.Models;

public sealed class PayrollRegister
{
    [Key] public long Id { get; set; }
    [WorkbookColumn("PAYROLL #", "PAYROLL #", 0)] public string? PayrollNumber { get; set; }
    [WorkbookColumn("PAY DATE", "PAY DATE", 1)] public DateOnly? PayDate { get; set; }
    [WorkbookColumn("PAY CODE", "PAY CODE", 2)] public string? PayCode { get; set; }
    [WorkbookColumn("EMPLOYEE ID#", "EMPLOYEE ID#", 3)] public string? EmployeeId { get; set; }
    [WorkbookColumn("LAST NAME", "LAST NAME", 4)] public string? LastName { get; set; }
    [WorkbookColumn("FIRST NAME", "FIRST NAME", 5)] public string? FirstName { get; set; }
    [WorkbookColumn("FILE STATUS", "FILE STATUS", 6)] public string? FileStatus { get; set; }
    [WorkbookColumn("TAX STATUS", "TAX STATUS", 7)] public string? TaxStatus { get; set; }
    [WorkbookColumn("MONTHLY SALARY RATE", "MONTHLY SALARY RATE", 8)] public decimal? MonthlySalaryRate { get; set; }
    [WorkbookColumn("BASIC SALARY", "BASIC SALARY", 9)] public decimal? BasicSalary { get; set; }
    [WorkbookColumn("DAILY RATE", "DAILY RATE", 10)] public decimal? DailyRate { get; set; }
    [WorkbookColumn("SALARY RATE TYPE", "SALARY RATE TYPE", 11)] public string? SalaryRateType { get; set; }
    [WorkbookColumn("ATT DEDUCTION", "ATT DEDUCTION", 12)] public decimal? AttDeduction { get; set; }
    [WorkbookColumn("OVERTIME", "OVERTIME", 13)] public decimal? Overtime { get; set; }
    [WorkbookColumn("OTHER TX INCOME", "OTHER TX INCOME", 14)] public decimal? OtherTxIncome { get; set; }
    [WorkbookColumn("ADJUSTMENTS", "ADJUSTMENTS", 15)] public decimal? Adjustments { get; set; }
    [WorkbookColumn("GROSS INCOME", "GROSS INCOME", 16)] public decimal? GrossIncome { get; set; }
    [WorkbookColumn("EMPLOYEE SSS", "EMPLOYEE SSS", 17)] public decimal? EmployeeSss { get; set; }
    [WorkbookColumn("EMPLOYEE MCR", "EMPLOYEE MCR", 18)] public decimal? EmployeeMcr { get; set; }
    [WorkbookColumn("EMPLOYEE PAG-IBIG", "EMPLOYEE PAG-IBIG", 19)] public decimal? EmployeePagIbig { get; set; }
    [WorkbookColumn("TAXABLE INCOME", "TAXABLE INCOME", 20)] public decimal? TaxableIncome { get; set; }
    [WorkbookColumn("WITHOLDING TAX", "WITHOLDING TAX", 21)] public decimal? WitholdingTax { get; set; }
    [WorkbookColumn("NET SALARY AFTER TAX", "NET SALARY AFTER TAX", 22)] public decimal? NetSalaryAfterTax { get; set; }
    [WorkbookColumn("OTHER NTX INCOME", "OTHER NTX INCOME", 23)] public decimal? OtherNtxIncome { get; set; }
    [WorkbookColumn("LOAN PAYMENTS", "LOAN PAYMENTS", 24)] public decimal? LoanPayments { get; set; }
    [WorkbookColumn("DEDUCTIONS", "DEDUCTIONS", 25)] public decimal? Deductions { get; set; }
    [WorkbookColumn("NET SALARY", "NET SALARY", 26)] public decimal? NetSalary { get; set; }
    [WorkbookColumn("EMPLOYER SSS", "EMPLOYER SSS", 27)] public decimal? EmployerSss { get; set; }
    [WorkbookColumn("EMPLOYER MCR", "EMPLOYER MCR", 28)] public decimal? EmployerMcr { get; set; }
    [WorkbookColumn("EMPLOYER EC", "EMPLOYER EC", 29)] public decimal? EmployerEc { get; set; }
    [WorkbookColumn("EMPLOYER PAG-IBIG", "EMPLOYER PAG-IBIG", 30)] public decimal? EmployerPagIbig { get; set; }
    [WorkbookColumn("PAYROLL COST", "PAYROLL COST", 31)] public decimal? PayrollCost { get; set; }
    [WorkbookColumn("PREVIOUS YTD GROSS", "PREVIOUS YTD GROSS", 32)] public decimal? PreviousYtdGross { get; set; }
    [WorkbookColumn("PREVIOUS YTD WITHOLDING", "PREVIOUS YTD WITHOLDING", 33)] public decimal? PreviousYtdWitholding { get; set; }
    [WorkbookColumn("PREVIOUS YTD SSS", "PREVIOUS YTD SSS", 34)] public decimal? PreviousYtdSss { get; set; }
    [WorkbookColumn("PREVIOUS YTD MCR", "PREVIOUS YTD MCR", 35)] public decimal? PreviousYtdMcr { get; set; }
    [WorkbookColumn("PREVIOUS YTD PAG-IBIG", "PREVIOUS YTD PAG-IBIG", 36)] public decimal? PreviousYtdPagIbig { get; set; }
    [WorkbookColumn("PREVIOUS YTD BON/13TH NT", "PREVIOUS YTD BON/13TH NT", 37)] public decimal? PreviousYtdBon13thNt { get; set; }
    [WorkbookColumn("PREVIOUS YTD BON/13TH TX", "PREVIOUS YTD BON/13TH TX", 38)] public decimal? PreviousYtdBon13thTx { get; set; }
    [WorkbookColumn("PAYMENT TYPE", "PAYMENT TYPE", 39)] public string? PaymentType { get; set; }
    [WorkbookColumn("BANK ACCT#", "BANK ACCT#", 40)] public string? BankAcct { get; set; }
    [WorkbookColumn("BANK NAME", "BANK NAME", 41)] public string? BankName { get; set; }
    [WorkbookColumn("COMMENT FIELD", "COMMENT FIELD", 42)] public string? CommentField { get; set; }
    [WorkbookColumn("ERROR FIELD", "ERROR FIELD", 43)] public string? ErrorField { get; set; }
    [WorkbookColumn("DATE EMPLOYED", "DATE EMPLOYED", 44)] public DateOnly? DateEmployed { get; set; }
    [WorkbookColumn("DATE TERMINATED", "DATE TERMINATED", 45)] public DateOnly? DateTerminated { get; set; }
    [WorkbookColumn("COST CENTER", "COST CENTER", 46)] public string? CostCenter { get; set; }
    [WorkbookColumn("CURRENCY", "CURRENCY", 47)] public string? Currency { get; set; }
    [WorkbookColumn("EXCHANGE RATE", "EXCHANGE RATE", 48)] public decimal? ExchangeRate { get; set; }
    [WorkbookColumn("TAX TABLE", "TAX TABLE", 49)] public string? TaxTable { get; set; }
    [WorkbookColumn("PAYMENT FREQ", "PAYMENT FREQ", 50)] public string? PaymentFreq { get; set; }
    [WorkbookColumn("MTD GROSS", "MTD GROSS", 51)] public decimal? MtdGross { get; set; }
    [WorkbookColumn("MTD BASIC", "MTD BASIC", 52)] public decimal? MtdBasic { get; set; }
    [WorkbookColumn("MTD SSS EMPLOYEE", "MTD SSS EMPLOYEE", 53)] public decimal? MtdSssEmployee { get; set; }
    [WorkbookColumn("MTD MCR EMPLOYEE", "MTD MCR EMPLOYEE", 54)] public decimal? MtdMcrEmployee { get; set; }
    [WorkbookColumn("MTD PAG-IBIG EMPLOYEE", "MTD PAG-IBIG EMPLOYEE", 55)] public decimal? MtdPagIbigEmployee { get; set; }
    [WorkbookColumn("MTD SSS EMPLOYER", "MTD SSS EMPLOYER", 56)] public decimal? MtdSssEmployer { get; set; }
    [WorkbookColumn("MTD MCR EMPLOYER", "MTD MCR EMPLOYER", 57)] public decimal? MtdMcrEmployer { get; set; }
    [WorkbookColumn("MTD EC EMPLOYER", "MTD EC EMPLOYER", 58)] public decimal? MtdEcEmployer { get; set; }
    [WorkbookColumn("MTD PAG-IBIG EMPLOYER", "MTD PAG-IBIG EMPLOYER", 59)] public decimal? MtdPagIbigEmployer { get; set; }
    [WorkbookColumn("MTD WH TAX", "MTD WH TAX", 60)] public decimal? MtdWhTax { get; set; }
    [WorkbookColumn("MONTHLY ALLOW", "MONTHLY ALLOW", 61)] public decimal? MonthlyAllow { get; set; }
    [WorkbookColumn("MTD NTX", "MTD NTX", 62)] public decimal? MtdNtx { get; set; }
    [WorkbookColumn("FROM", "FROM", 63)] public DateOnly? From { get; set; }
    [WorkbookColumn("TO", "TO", 64)] public DateOnly? To { get; set; }
}
