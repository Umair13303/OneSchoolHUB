# Seeds Dar-e-Arqam School into the local OneSchoolDev database.
# Usage (from repo root or Scripts folder):
#   powershell -ExecutionPolicy Bypass -File OneSchoolHUB/Scripts/seed-darearqam-local.ps1

$ErrorActionPreference = 'Stop'
$Server   = if ($env:ONESCHOOL_SQL_SERVER) { $env:ONESCHOOL_SQL_SERVER } else { 'localhost' }
$Database = if ($env:ONESCHOOL_SQL_DB)     { $env:ONESCHOOL_SQL_DB }     else { 'OneSchoolDev' }
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$SqlFile   = Join-Path $ScriptDir 'seed-darearqam-school.sql'

if (-not (Test-Path $SqlFile)) {
    throw "SQL file not found: $SqlFile"
}

Write-Host "Seeding Dar-e-Arqam School → $Server / $Database"
sqlcmd -S $Server -d $Database -E -C -b -i $SqlFile
if ($LASTEXITCODE -ne 0) {
    throw "sqlcmd failed with exit code $LASTEXITCODE"
}

Write-Host ""
Write-Host "Done. Logins (password Admin@123):"
Write-Host "  Admin:    admin@darearqam.local"
Write-Host "  Teachers: t.english@darearqam.local … t.computer@darearqam.local"
Write-Host "UI: http://localhost:4201/"
