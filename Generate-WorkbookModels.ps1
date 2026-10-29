$ErrorActionPreference = 'Stop'
$source = 'C:\Users\aspacardo-itg\Downloads\PAY_FORMULA.xlsx'
$project = $PSScriptRoot
Add-Type -AssemblyName System.IO.Compression
$fs = [IO.File]::Open($source, 'Open', 'Read', 'ReadWrite')
$zip = [IO.Compression.ZipArchive]::new($fs, 'Read')
function Read-Xml([string]$name) {
    $entry = $zip.GetEntry($name)
    $reader = [IO.StreamReader]::new($entry.Open())
    try { [xml]$reader.ReadToEnd() } finally { $reader.Dispose() }
}
function Xml-Text($node, [string]$xpath) { $n = $node.SelectSingleNode($xpath); if ($n) { $n.InnerText } else { '' } }
function Read-Row-Cell($row, [int]$column) {
    if ($column -le 0) { return '' }
    $letters = ''
    $n = $column
    while ($n -gt 0) { $rem = ($n - 1) % 26; $letters = [char]([int](65 + $rem)) + $letters; $n = [math]::Floor(($n - 1) / 26) }
    $cell = $row.SelectSingleNode("*[local-name()='c'][@r='$letters$($row.GetAttribute('r'))']")
    if (-not $cell) { return '' }
    $value = Xml-Text $cell "*[local-name()='v']"
    if ($cell.GetAttribute('t') -eq 's' -and $value -ne '') { $value = $shared[[int]$value] }
    return [string]$value
}
function Get-FieldType([string]$sheet, [string]$header) {
    if ($header -in @('PAY DATE','DATE','DATE EMPLOYED','DATE TERMINATED','RECUR START','RECUR END','FROM','TO')) { return 'DateOnly?' }
    if ($header -in @('PAYROLL #','PAY CODE','EMPLOYEE ID#','EMP ID#','LAST NAME','FIRST NAME','FILE STATUS','TAX STATUS','SALARY RATE TYPE','PAYMENT TYPE','BANK ACCT#','BANK NAME','COMMENT FIELD','ERROR FIELD','COST CENTER','CURRENCY','TAX TABLE','PAYMENT FREQ','OT CODE','FREQ','REMARKS','SYSTEM OT TIME','SYSTEM NP TIME','JE COST CENTER','Remarks')) { return 'string?' }
    if ($header -in @('OT MINUTES','OT NP MINUTES','OT NP2 MINUTES')) { return 'int?' }
    return 'decimal?'
}
function Get-SqlType([string]$fieldType) {
    switch ($fieldType) { 'DateOnly?' { 'date'; break } 'decimal?' { 'numeric'; break } 'int?' { 'integer'; break } default { 'text' } }
}
$stringsXml = Read-Xml 'xl/sharedStrings.xml'
$shared = [System.Collections.Generic.List[string]]::new()
foreach ($si in $stringsXml.SelectNodes("//*[local-name()='si']")) {
    $texts = $si.SelectNodes(".//*[local-name()='t']") | ForEach-Object { $_.InnerText }
    $shared.Add(($texts -join ''))
}
$tables = @(
    @{ Sheet='PAYREG'; Class='PayrollRegister'; Table='PAYREG' },
    @{ Sheet='OTREG'; Class='OvertimeRegister'; Table='OTREG' },
    @{ Sheet='TimeAttendance'; Class='TimeAttendance'; Table='TimeAttendance' }
)
$workbook = Read-Xml 'xl/workbook.xml'
$rels = Read-Xml 'xl/_rels/workbook.xml.rels'
$targets = @{}
foreach ($rel in $rels.SelectNodes("//*[local-name()='Relationship']")) { $targets[$rel.GetAttribute('Id')] = $rel.GetAttribute('Target') }
$allModels = @{}
$seed = @{ PAYREG=@(); OTREG=@() }
$migrationSql = [System.Collections.Generic.List[string]]::new()
foreach ($table in $tables) {
    $sheetNode = $workbook.SelectSingleNode("//*[local-name()='sheet'][@name='$($table.Sheet)']")
    $relId = $sheetNode.GetAttribute('id', 'http://schemas.openxmlformats.org/officeDocument/2006/relationships')
    $target = $targets[$relId].TrimStart('/')
    if (-not $target.StartsWith('xl/')) { $target = 'xl/' + $target }
    $sheetXml = Read-Xml $target
    $headerRow = $sheetXml.SelectSingleNode("//*[local-name()='sheetData']/*[local-name()='row'][@r='1']")
    $headers = [System.Collections.Generic.List[object]]::new()
    foreach ($cell in $headerRow.ChildNodes) {
        $ref = $cell.GetAttribute('r'); $letters = ($ref -replace '\d','')
        $col = 0; foreach ($ch in $letters.ToCharArray()) { $col = $col * 26 + ([int]$ch - [int][char]'A' + 1) }
        $value = Xml-Text $cell "*[local-name()='v']"
        if ($cell.GetAttribute('t') -eq 's' -and $value -ne '') { $value = $shared[[int]$value] }
        $headers.Add([pscustomobject]@{ Column=$col; Name=$value })
    }
    if ($table.Sheet -eq 'TimeAttendance') { $headers.Insert(0, [pscustomobject]@{ Column=0; Name='PAYROLL #' }) }
    $seen = @{}
    $storageSeen = @{}
    $lines = [System.Collections.Generic.List[string]]::new()
    $lines.Add('using System.ComponentModel.DataAnnotations;')
    $lines.Add('')
    $lines.Add('namespace RegPay3.Models;')
    $lines.Add('')
    $lines.Add('public sealed class ' + $table.Class)
    $lines.Add('{')
    $lines.Add('    [Key] public long Id { get; set; }')
    $props = [System.Collections.Generic.List[object]]::new()
    for ($i=0; $i -lt $headers.Count; $i++) {
        $header = $headers[$i].Name
        $words = [regex]::Matches($header.ToLowerInvariant(), '[a-z0-9]+')
        $prop = (($words | ForEach-Object { $_.Value.Substring(0,1).ToUpperInvariant() + $_.Value.Substring(1) }) -join '')
        if (-not $prop) { $prop = 'Column' + ($i+1) }
        if ($prop -eq 'Payroll') { $prop = 'PayrollNumber' }
        if ($prop -eq 'EmpId') { $prop = 'EmployeeId' }
        if ($prop -eq 'EmployeeId' -and $seen.ContainsKey($prop)) { $prop += 'Number' }
        if ($seen.ContainsKey($prop)) { $prop += ($i+1) }
        $seen[$prop] = $true
        $storage = $header
        if ($storageSeen.ContainsKey($storage)) { $storage += '__' + ($i + 1) }
        $storageSeen[$storage] = $true
        $escapedHeader = $header.Replace('\','\\').Replace('"','\"')
        $escapedStorage = $storage.Replace('\','\\').Replace('"','\"')
        $fieldType = Get-FieldType $table.Sheet $header
        $lines.Add('    [WorkbookColumn("' + $escapedHeader + '", "' + $escapedStorage + '", ' + $i + ')] public ' + $fieldType + ' ' + $prop + ' { get; set; }')
        $props.Add([pscustomobject]@{ Header=$header; Storage=$storage; Property=$prop; Column=$headers[$i].Column; FieldType=$fieldType })
    }
    $lines.Add('}')
    $lines | Set-Content -LiteralPath (Join-Path $project ('Models\' + $table.Class + '.cs')) -Encoding utf8
    $allModels[$table.Table] = $props
    $definitions = @($props | ForEach-Object { '"' + $_.Storage.Replace('"','""') + '" ' + (Get-SqlType $_.FieldType) + ' NULL' })
    $migrationSql.Add('CREATE TABLE "' + $table.Table + '" ("Id" bigint GENERATED BY DEFAULT AS IDENTITY PRIMARY KEY, ' + ($definitions -join ', ') + ');')
    if ($table.Sheet -in @('PAYREG','OTREG')) {
        $dataRows = @($sheetXml.SelectNodes("//*[local-name()='sheetData']/*[local-name()='row']") | Where-Object { [int]$_.GetAttribute('r') -gt 1 })
        if ($table.Sheet -eq 'PAYREG') { $dataRows = @($dataRows | Select-Object -First 2) }
        if ($table.Sheet -eq 'OTREG') {
            $employeeIdColumn = ($props | Where-Object { $_.Property -eq 'EmployeeId' } | Select-Object -First 1).Column
            $seedEmployeeIds = @($seed.PAYREG | ForEach-Object { $_['EMPLOYEE ID#'] })
            $dataRows = @($dataRows | Where-Object { $seedEmployeeIds -contains (Read-Row-Cell $_ $employeeIdColumn) })
        }
        foreach ($row in $dataRows) {
            $data = [ordered]@{}
            foreach ($prop in $props) {
                $value = ''
                if ($prop.Column -gt 0) {
                    $cell = $row.SelectSingleNode("*[local-name()='c'][@r='$([string]([char]0))']")
                    $columnName = ''
                    $n=$prop.Column; while($n -gt 0){$rem=($n-1)%26;$columnName=[char]([int](65+$rem))+$columnName;$n=[math]::Floor(($n-1)/26)}
                    $cellRef = $columnName + $row.GetAttribute('r')
                    $cell = $row.SelectSingleNode("*[local-name()='c'][@r='$cellRef']")
                    if ($cell) { $value = Xml-Text $cell "*[local-name()='v']"; if ($cell.GetAttribute('t') -eq 's' -and $value -ne '') { $value = $shared[[int]$value] } }
                }
                $data[$prop.Storage] = [string]$value
            }
            $seed[$table.Sheet] += ,$data
        }
    }
}
$seed | ConvertTo-Json -Depth 10 | Set-Content -LiteralPath (Join-Path $project 'SeedData.json') -Encoding utf8
$migration = @'
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using RegPay3.Data;

namespace RegPay3.Migrations;

[DbContext(typeof(PayrollDbContext))]
[Migration("202610290001_InitialPayrollTables")]
public sealed class InitialPayrollTables : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
'@
foreach ($sql in $migrationSql) { $escaped = $sql.Replace('"','""'); $migration += "        migrationBuilder.Sql(@`"$escaped`" );`n" }
$migration += @'
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP TABLE IF EXISTS \"TimeAttendance\", \"OTREG\", \"PAYREG\";");
    }
}
'@
$migration | Set-Content -LiteralPath (Join-Path $project 'Migrations\202610290001_InitialPayrollTables.cs') -Encoding utf8
$zip.Dispose()
Write-Host 'Generated entity models, workbook seed data and initial PostgreSQL migration from PAY_FORMULA.xlsx.'

